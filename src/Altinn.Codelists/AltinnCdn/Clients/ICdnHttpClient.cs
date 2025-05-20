using Altinn.App.Core.Models;

namespace Altinn.Codelists.AltinnCdn.Clients;

/// <summary>
/// Client for fetching code lists from Altinn CDN.
/// </summary>
public interface ICdnHttpClient
{
    /// <summary>
    /// Fetches a code list from Altinn CDN.
    /// </summary>
    Task<List<AppOption>> GetCodeList(string orgName, string codeListid, string version, string language);
}