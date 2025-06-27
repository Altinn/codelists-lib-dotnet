namespace Altinn.Codelists.Posten.Clients;

internal sealed class PostalCodesHttpClient(HttpClient httpClient) : IPostalCodesClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly Uri _uri = new("https://www.bring.no/postnummerregister-ansi.txt");

    public async Task<List<PostalCodeRecord>> GetPostalCodes()
    {
        using var response = await _httpClient.GetAsync(_uri.ToString());
        await using var responseStream = await response.Content.ReadAsStreamAsync();

        var parser = new PostalCodesCsvParser(responseStream);
        List<PostalCodeRecord> result = await parser.Parse();

        return result;
    }
}
