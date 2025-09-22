using Altinn.Codelists.RestCountries;
using Altinn.Codelists.RestCountries.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Altinn.Codelists.Tests.RestCountries.Clients;

public class CountriesClientTests
{
    private readonly ITestOutputHelper _output;

    public CountriesClientTests(ITestOutputHelper outputHelper)
    {
        _output = outputHelper;
    }

    private sealed record Fixture(IServiceProvider ServiceProvider, ICountryClient Client) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            if (ServiceProvider is IAsyncDisposable disposable)
                await disposable.DisposeAsync();
        }

        public static Fixture Create()
        {
            var services = new ServiceCollection();
            services.AddRestCountriesClient();

            var serviceProvider = services.BuildServiceProvider(
                new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }
            );

            var client = serviceProvider.GetRequiredService<ICountryClient>();
            return new(serviceProvider, client);
        }
    }

    [Fact]
    public async Task GetCountries_NoFilter_ShouldReturnAll()
    {
        var data = new List<List<Country>>(2);
        for (int i = 0; i < 2; i++)
        {
            await using var fixture = Fixture.Create();
            var client = fixture.Client;

            var countries = await client.GetCountries();
            Assert.Equal(250, countries.Count);

            data.Add(countries);
        }

        AssertCountryListInvariants(data);
    }

    [Fact]
    public async Task GetCountries_SmokeTest()
    {
        var tasks = new Task[Environment.ProcessorCount];
        var countdown = new CountdownEvent(initialCount: tasks.Length);
        long shouldRun = 0;

        await using var fixture = Fixture.Create();
        for (int i = 0; i < tasks.Length; i++)
        {
            var id = i;
            tasks[id] = Task.Run(async () =>
            {
                var data = new List<List<Country>>(2);
                var client = fixture.Client;

                var spinWait = default(SpinWait);
                // Signal to the outer test that we are ready to race for the `GetCountries` call
                countdown.Signal();
                while (Interlocked.Read(ref shouldRun) == 0)
                    spinWait.SpinOnce();

                var countries = await client.GetCountries();
                Assert.Equal(250, countries.Count);
                data.Add(countries);

                for (int i = 0; i < 2; i++)
                {
                    countries = await client.GetCountries();
                    Assert.Equal(250, countries.Count);
                    data.Add(countries);
                }

                AssertCountryListInvariants(data);
            });
        }

        countdown.Wait();
        Interlocked.Increment(ref shouldRun);

        await Task.WhenAll(tasks);
    }

    [Fact]
    public async Task GetCountries_FilterOnRegion_ShouldReturnOnlyInRegion()
    {
        var data = new List<List<Country>>(2);
        for (int i = 0; i < 2; i++)
        {
            await using var fixture = Fixture.Create();
            var client = fixture.Client;

            var countries = await client.GetCountries([new() { Region = "Europe" }]);
            Assert.Equal(53, countries.Count);

            data.Add(countries);
        }

        AssertCountryListInvariants(data);
    }

    [Fact]
    public async Task GetCountries_WithFilter_SmokeTest()
    {
        var tasks = new Task[Environment.ProcessorCount];
        var countdown = new CountdownEvent(initialCount: tasks.Length);
        long shouldRun = 0;
        Filter[] filters = [new() { Region = "Europe" }];

        await using var fixture = Fixture.Create();
        for (int i = 0; i < tasks.Length; i++)
        {
            var id = i;
            tasks[id] = Task.Run(async () =>
            {
                var data = new List<List<Country>>(2);
                var client = fixture.Client;

                var spinWait = default(SpinWait);
                // Signal to the outer test that we are ready to race for the `GetCountries` call
                countdown.Signal();
                while (Interlocked.Read(ref shouldRun) == 0)
                    spinWait.SpinOnce();

                var countries = await client.GetCountries(filters);
                Assert.Equal(53, countries.Count);
                data.Add(countries);

                for (int i = 0; i < 2; i++)
                {
                    countries = await client.GetCountries(filters);
                    Assert.Equal(53, countries.Count);
                    data.Add(countries);
                }

                AssertCountryListInvariants(data);
            });
        }

        countdown.Wait();
        Interlocked.Increment(ref shouldRun);

        await Task.WhenAll(tasks);
    }

    [Fact]
    public async Task GetCountries_EmptyFilters()
    {
        var data = new List<List<Country>>(2);
        for (int i = 0; i < 2; i++)
        {
            await using var fixture = Fixture.Create();
            var client = fixture.Client;

            var countries = await client.GetCountries([]);

            Assert.Equal(250, countries.Count);
            data.Add(countries);
        }

        AssertCountryListInvariants(data);
    }

    [Fact]
    public async Task GetCountries_FilterOnMultipleRegions_ShouldReturnOnlyInRegions()
    {
        var data = new List<List<Country>>(2);
        for (int i = 0; i < 2; i++)
        {
            await using var fixture = Fixture.Create();
            var client = fixture.Client;

            var countries = await client.GetCountries(
                [new() { Region = "Europe" }, new() { Region = "Asia", SubRegion = "Eastern Asia" }]
            );

            countries.ForEach(c => _output.WriteLine(c.Name.Common));
            Assert.Equal(61, countries.Count);
            Assert.All(
                countries,
                country =>
                {
                    Assert.True(
                        country.Region == "Europe" || (country.Region == "Asia" && country.SubRegion == "Eastern Asia")
                    );
                }
            );

            data.Add(countries);
        }

        AssertCountryListInvariants(data);
    }

    [Fact]
    public void Clone_ShouldCreateDeepCopy()
    {
        // Arrange - create a fully populated Country
        var original = new Country(new Name("Norway", "Kingdom of Norway"))
        {
            CountryCodeAlpha2 = "NO",
            CountryCodeAlpha3 = "NOR",
            CountryCodeNumeric3 = "578",
            Independent = true,
            Status = "officially-assigned",
            UnitedNationsMember = true,
            EmojiFlag = "🇳🇴",
            Region = "Europe",
            SubRegion = "Northern Europe",
            Languages = new Dictionary<string, string> { { "nor", "Norwegian" } },
            Translations = new Dictionary<string, Name> { { "fra", new Name("Norvège", "Royaume de Norvège") } },
            Currencies = new Dictionary<string, Currency> { { "NOK", new Currency("Norwegian krone", "kr") } },
            LatitudeLongitude = [62.0m, 10.0m],
            TopLevelDomains = [".no"],
        };

        // Act
        var clone = original.Clone();

        // Assert - verify deep copy (no shared references)
        Assert.NotSame(original, clone);
        Assert.NotSame(original.Name, clone.Name);
        Assert.NotSame(original.Languages, clone.Languages);
        Assert.NotSame(original.Translations, clone.Translations);
        Assert.NotSame(original.Currencies, clone.Currencies);
        Assert.NotSame(original.LatitudeLongitude, clone.LatitudeLongitude);
        Assert.NotSame(original.TopLevelDomains, clone.TopLevelDomains);

        // Verify all values are equal
        Assert.Equal(original.Name.Common, clone.Name.Common);
        Assert.Equal(original.Name.Official, clone.Name.Official);
        Assert.Equal(original.CountryCodeAlpha2, clone.CountryCodeAlpha2);
        Assert.Equal(original.CountryCodeAlpha3, clone.CountryCodeAlpha3);
        Assert.Equal(original.Languages.Count, clone.Languages.Count);
        Assert.Equal(original.Translations.Count, clone.Translations.Count);
        Assert.Equal(original.Currencies.Count, clone.Currencies.Count);
        Assert.Equal(original.LatitudeLongitude.Length, clone.LatitudeLongitude.Length);
        Assert.Equal(original.TopLevelDomains.Length, clone.TopLevelDomains.Length);

        // Verify independence - modifying original doesn't affect clone
        original.Languages.Add("test", "test");
        original.Translations.Add("test", new Name("test", "test"));
        original.Currencies.Add("TEST", new Currency("Test Currency", "T"));

        Assert.NotEqual(original.Languages.Count, clone.Languages.Count);
        Assert.NotEqual(original.Translations.Count, clone.Translations.Count);
        Assert.NotEqual(original.Currencies.Count, clone.Currencies.Count);
    }

    private static void AssertCountryListInvariants(List<List<Country>> data)
    {
        // Just ensures that we are returning different lists and object instances
        // even though we are caching pretty heavily in the client internals
        // (i.e. verifies that we are cloning Coutry objects and producing new lists)
        Assert.True(data.Count > 1);
        var first = data[0];
        var last = data[^1];
        last.Add(new Country(new Name("TESTING", "TESTING")));
        Assert.NotSame(first, last);
        Assert.NotEqual(first.Count, last.Count);
        Assert.NotSame(first[0], last[0]);
        Assert.NotSame(first[0].Name, last[0].Name);
        Assert.Equal(first[0].Name.Common, last[0].Name.Common);
        last[0].Name = new Name("TESTING", "TESTING");
        Assert.NotEqual(first[0].Name.Common, last[0].Name.Common);
    }
}
