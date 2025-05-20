using Altinn.App.Core.Models;
using Altinn.Codelists.AltinnCdn.Clients;
using RichardSzalay.MockHttp;

namespace Altinn.Codelists.Tests.AltinnCdn.Mocks;

public class CdnHttpClientMock : ICdnHttpClient
{
    private const string YesNoMaybeCodelist = "Altinn.Codelists.Tests.AltinnCdn.Testdata.yesNoMaybe.en.json";
    private const string NumbersCodelist = "Altinn.Codelists.Tests.AltinnCdn.Testdata.numbers.en.json";

    private readonly ICdnHttpClient _client;

    private MockHttpMessageHandler HttpMessageHandlerMock { get; }

    public CdnHttpClientMock()
    {
        HttpMessageHandlerMock = new MockHttpMessageHandler();

        HttpMessageHandlerMock
            .When("https://altinncdn.no/orgs/ttd/codelists/yesNoMaybe/1970-01-01T00-00-00Z/en.json")
            .Respond("application/json", EmbeddedResource.LoadDataAsString(YesNoMaybeCodelist).Result);
        
        HttpMessageHandlerMock
            .When("https://altinncdn.no/orgs/ttd/codelists/numbers/1970-01-01T00-00-00Z/en.json")
            .Respond("application/json", EmbeddedResource.LoadDataAsString(NumbersCodelist).Result);

        _client = new CdnHttpClient(new HttpClient(HttpMessageHandlerMock));
    }

    public async Task<List<AppOption>> GetCodeList(string orgName, string codeListid, string version, string language)
    {
        return await _client.GetCodeList(orgName, codeListid, version, language);
    }
}