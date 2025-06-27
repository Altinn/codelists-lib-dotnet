using Altinn.App.Core.Features;
using Altinn.App.Core.Models;

namespace Altinn.Codelists.Kartverket.AdministrativeUnits;

/// <summary>
/// Provides a codelist for counties of Norway.
/// </summary>
internal sealed class CountiesCodelistProvider(IAdministrativeUnitsClient countiesHttpClient) : IAppOptionsProvider
{
    private readonly IAdministrativeUnitsClient _countiesHttpClient = countiesHttpClient;

    /// <inheritdoc/>
    public string Id => "fylker-kv";

    /// <inheritdoc/>
    public async Task<AppOptions> GetAppOptionsAsync(string? language, Dictionary<string, string> keyValuePairs)
    {
        var counties = await _countiesHttpClient.GetCounties();

        var appOptions = new AppOptions()
        {
            Options = counties.Select(x => new AppOption() { Value = x.Number, Label = x.Name }).ToList(),
        };

        return appOptions;
    }
}
