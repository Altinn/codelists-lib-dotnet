using Altinn.App.Core.Features;
using Altinn.App.Core.Models;
using Altinn.Codelists.AltinnCdn.Clients;

namespace Altinn.Codelists.AltinnCdn;

/// <summary>
/// Provides code lists created by service owners from Altinn CDN.
/// </summary>
public class CdnCodelistProvider : IAppOptionsProvider
{
    private readonly ICdnHttpClient _cdnHttpClient;
    
    /// <summary>
    /// Initialises a new instance of the <see cref="CdnCodelistProvider"/> class.
    /// </summary>
    public CdnCodelistProvider(ICdnHttpClient cdnHttpClient)
    {
        _cdnHttpClient = cdnHttpClient;
    }
    
    /// <inheritdoc />
    public string Id => "altinn-cdn";
    
    /// <inheritdoc />
    public async Task<AppOptions> GetAppOptionsAsync(string language, Dictionary<string, string> keyValuePairs)
    {
        var orgName = keyValuePairs["orgName"];
        var codeListId = keyValuePairs["codeListId"];
        var version = keyValuePairs["version"];
        
        return await _cdnHttpClient.GetCodeList(orgName, codeListId, version, language);
    }
}