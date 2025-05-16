using Altinn.App.Core.Models;

namespace Altinn.Codelists.AltinnCdn.Clients;

/// <summary>
/// Client for fetching code lists from CDN
/// </summary>
public interface ICdnHttpClient
{
    /// <summary>
    /// Fetches a code list from Altinn CDN.
    /// </summary>
    /// <returns></returns>
   Task<AppOptions> GetCodeList(string orgName, string codeListid, string version, string language);
}