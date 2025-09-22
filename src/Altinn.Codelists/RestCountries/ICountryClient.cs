using Altinn.Codelists.RestCountries.Models;

namespace Altinn.Codelists.RestCountries;

/// <summary>
/// Information on all countries of the world.
/// </summary>
public interface ICountryClient
{
    // TODO: make client methods return immutable/readonly lists/enumerables
    // If the caller has to modify, they can allocate themselves (this is probably not a common case)
    // Will clarify intended use and can improve perf

    /// <summary>
    /// Get all the countries of the world.
    /// </summary>
    Task<List<Country>> GetCountries();

    /// <summary>
    /// Get all countries matching the provided filters.
    /// Values within the same filter object are AND'ed, while
    /// values between filter objects are OR'ed.
    /// </summary>
    Task<List<Country>> GetCountries(IEnumerable<Filter> filters);
}
