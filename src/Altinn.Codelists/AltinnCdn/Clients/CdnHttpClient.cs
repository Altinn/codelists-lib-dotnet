using Altinn.App.Core.Models;

namespace Altinn.Codelists.AltinnCdn.Clients;

/// <summary>
/// Client for getting code lists from Altinn CDN.
/// </summary>
public class CdnHttpClient : ICdnHttpClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Creates an instance of <see cref="CdnHttpClient"/>.
    /// </summary>
    public CdnHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<List<AppOption>> GetCodeList(string orgName, string codeListId, string version, string language)
    {
        var url = $"https://altinncdn.no/orgs/{orgName}/codelists/{codeListId}/{version}/{language}.json";
        var response = await _httpClient.GetAsync(url);
        return await response.Content.ReadAsAsync<List<AppOption>>();
    }
}
