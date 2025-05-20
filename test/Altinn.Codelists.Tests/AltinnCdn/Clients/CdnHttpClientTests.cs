using Altinn.Codelists.AltinnCdn.Clients;

namespace Altinn.Codelists.Tests.AltinnCdn.Clients
{
    public class CdnHttpClientTests
    {
        [Fact(Skip = "This calls out to the api and is primarily used to test during development.")]
        public async Task GetCodeList_ShouldReturnCodeList()
        {
            var client = new CdnHttpClient(new HttpClient());

            var codelist = await client.GetCodeList("ttd", "yesNoMaybe", "1970-01-01T00-00-00Z", "en");

            codelist.Count.Should().Be(3);
        }
    }
}