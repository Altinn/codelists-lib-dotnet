namespace Altinn.Codelists.Posten.Clients;

/// <summary>
/// Client for getting the offical list of post codes in Norway.
/// </summary>
public class PostalCodesHttpClient(HttpClient httpClient) : IPostalCodesClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly Uri _uri = new("https://www.bring.no/postnummerregister-ansi.txt");

    /// <summary>
    /// Gets all postal codes
    /// </summary>
    public async Task<List<PostalCodeRecord>> GetPostalCodes()
    {
        using var response = await _httpClient.GetAsync(_uri.ToString());
        await using var responseStream = await response.Content.ReadAsStreamAsync();

        var parser = new PostalCodesCsvParser(responseStream);
        List<PostalCodeRecord> result = await parser.Parse();

        return result;
    }
}
