using Altinn.App.Core.Features;
using Altinn.Codelists.AltinnCdn;
using Altinn.Codelists.Tests.AltinnCdn.Mocks;

namespace Altinn.Codelists.Tests.AltinnCdn
{
    public class CdnCodelistProviderTests
    {
        [Fact]
        public async Task GetAppOptionsAsync_ShouldReturnCodeList()
        {
            var httpClientMock = new CdnHttpClientMock();
            IAppOptionsProvider appOptionsProvider = new CdnCodelistProvider(httpClientMock);
            var queryParameters = new Dictionary<string, string>
            {
                { "orgName", "ttd" },
                { "codeListId", "yesNoMaybe" },
                { "version", "1970-01-01T00-00-00Z" }
            };

            var appOptions = await appOptionsProvider.GetAppOptionsAsync("en", queryParameters);

            appOptions.Options.Should().HaveCount(3);
            var yesOption = appOptions.Options.First(x => x.Value == "yes");
            yesOption.Value.Should().Be("yes");
            yesOption.Label.Should().Be("Yes");
            yesOption.Description.Should().Be("Description for yes");
            yesOption.HelpText.Should().Be("Help text for yes");
        }

        [Fact]
        public async Task GetAppOptionsAsync_ShouldReturnParameters()
        {
            var httpClientMock = new CdnHttpClientMock();
            IAppOptionsProvider appOptionsProvider = new CdnCodelistProvider(httpClientMock);
            var queryParameters = new Dictionary<string, string>
            {
                { "orgName", "ttd" },
                { "codeListId", "yesNoMaybe" },
                { "version", "1970-01-01T00-00-00Z" }
            };

            var appOptions = await appOptionsProvider.GetAppOptionsAsync("en", queryParameters);

            appOptions.Parameters.Should().HaveCount(3);
            appOptions.Parameters["orgName"].Should().Be("ttd");
            appOptions.Parameters["codeListId"].Should().Be("yesNoMaybe");
            appOptions.Parameters["version"].Should().Be("1970-01-01T00-00-00Z");
        }

        [Fact(Skip = "codelists-lib-dotnet currently does not preserve value types.")]
        public async Task GetAppOptionsAsync_ShouldPreserveNumberValues()
        {
            var httpClientMock = new CdnHttpClientMock();
            IAppOptionsProvider appOptionsProvider = new CdnCodelistProvider(httpClientMock);
            var queryParameters = new Dictionary<string, string>
            {
                { "orgName", "ttd" },
                { "codeListId", "numbers" },
                { "version", "1970-01-01T00-00-00Z" }
            };

            var appOptions = await appOptionsProvider.GetAppOptionsAsync("en", queryParameters);

            var firstOption = appOptions.Options[0];
            firstOption.Value.Should().Be("1");
            // The property ValueType and enum AppOptionValueType is not available to codelists-lib
            // firstOption.ValueType.Should().Be(AppOptionValueType.Number);  
        }
    }
}