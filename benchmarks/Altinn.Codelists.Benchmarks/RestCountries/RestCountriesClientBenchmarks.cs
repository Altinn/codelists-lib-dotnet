using System.Collections.Generic;
using System.Threading.Tasks;
using Altinn.Codelists.RestCountries;
using Altinn.Codelists.RestCountries.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Altinn.Codelists.Benchmarks.RestCountries;

[Config(typeof(Config))]
public class DateVerificationRegexBenchmarks
{
    private ServiceProvider _serviceProvider;
    private ICountryClient _client;
    private Filter[] _filters;

    private sealed class Config : ManualConfig
    {
        public Config()
        {
            this.SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
            this.AddDiagnoser(MemoryDiagnoser.Default);
            // this.AddDiagnoser(new DotTraceDiagnoser());
            this.AddColumn(RankColumn.Arabic);
            this.Orderer = new DefaultOrderer(SummaryOrderPolicy.SlowestToFastest, MethodOrderPolicy.Declared);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddRestCountriesClient();

        _serviceProvider = services.BuildServiceProvider(
            new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }
        );

        _client = _serviceProvider.GetRequiredService<ICountryClient>();
        _filters = [new() { Region = "Europe" }];
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceProvider.Dispose();
    }

    [Benchmark]
    public Task<List<Country>> GetCountries() => _client.GetCountries();

    [Benchmark]
    public Task<List<Country>> GetCountriesWithFilters() => _client.GetCountries(_filters);
}
