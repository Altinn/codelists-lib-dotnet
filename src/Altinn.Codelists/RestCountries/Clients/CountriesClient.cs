using System.Diagnostics;
using Altinn.Codelists.RestCountries.Data;
using Altinn.Codelists.RestCountries.Models;
using Altinn.Codelists.Utilities;

namespace Altinn.Codelists.RestCountries.Clients;

/// <summary>
/// Client to get information on all countries of the world.
/// Note that this is not an http client but uses a static json embedded within
/// this dll to resolve the the list of countries.
/// </summary>
internal sealed class CountriesClient : ICountryClient
{
    private long _cacheInitialization = 0;
    private Country[]? _data;
    private readonly TaskCompletionSource<Country[]> _dataTcs = new TaskCompletionSource<Country[]>(
        TaskCreationOptions.RunContinuationsAsynchronously
    );

    private ValueTask<Country[]> GetAll()
    {
        if (_data is not null)
            return new ValueTask<Country[]>(_data);

        return GetAllAsync();

        async ValueTask<Country[]> GetAllAsync()
        {
            if (Interlocked.CompareExchange(ref _cacheInitialization, 1, 0) != 0)
                return await _dataTcs.Task;

            try
            {
                // Streams from EmbeddedResources are non async because the data is embedded into the DLL and loaded into memory
                // We still just use async APIs because this only happens once when we cache it like this
                // But if that changes we should reconsider
                await using var stream = EmbeddedResource.LoadDataAsStream(Resources.CountriesJson);
                var countries = await JsonSerializer.DeserializeAsync<Country[]>(stream);
                _data = countries ?? Array.Empty<Country>();
                _dataTcs.TrySetResult(_data);
                return _data;
            }
            catch (Exception ex)
            {
                _dataTcs.TrySetException(ex);
                throw;
            }
        }
    }

    /// <summary>
    /// Sends a asynchronus internal request to get all the countries of the world.
    /// </summary>
    public Task<List<Country>> GetCountries()
    {
        var data = _data;
        if (data is not null)
        {
            if (data.Length == 0)
                return Task.FromResult(new List<Country>());
            var result = ListFrom(data);
            return Task.FromResult(result);
        }

        return GetCountriesAsync();
        async Task<List<Country>> GetCountriesAsync()
        {
            var data = await GetAll();
            if (data.Length == 0)
                return new List<Country>();
            return ListFrom(data);
        }
    }

    /// <summary>
    /// Sends a asynchronus internal request to get all countries of the world,
    /// matching the specified filters.
    /// Values within the same filter object are AND'ed,
    /// while values between filter objects are OR'ed.
    /// </summary>
    public Task<List<Country>> GetCountries(IEnumerable<Filter> filters)
    {
        var data = _data;
        if (data is not null)
        {
            if (data.Length == 0)
                return Task.FromResult(new List<Country>());
            var hasFilters = filters.Any();
            if (!hasFilters)
                return Task.FromResult(ListFrom(data));

            return Task.FromResult(Query(data, filters));
        }

        return GetCountriesAsync(filters);

        async Task<List<Country>> GetCountriesAsync(IEnumerable<Filter> filters)
        {
            var data = await GetAll();
            if (data.Length == 0)
                return new List<Country>();
            var hasFilters = filters.Any();
            if (!hasFilters)
                return ListFrom(data);

            return Query(data, filters);
        }
    }

    private static List<Country> Query(Country[] countries, IEnumerable<Filter> filters)
    {
        Debug.Assert(filters.Any());
        Func<Country, bool> predicate = c => false;
        foreach (var filter in filters)
        {
            Func<Country, bool> subPredicate = c => true;
            if (!string.IsNullOrEmpty(filter.Region))
            {
                var currentSubPredicate = subPredicate;
                subPredicate = c => currentSubPredicate(c) && c.Region.Equals(filter.Region, StringComparison.Ordinal);
            }

            if (!string.IsNullOrEmpty(filter.SubRegion))
            {
                var currentSubPredicate = subPredicate;
                subPredicate = c =>
                    currentSubPredicate(c) && c.SubRegion.Equals(filter.SubRegion, StringComparison.Ordinal);
            }

            var currentPredicate = predicate;
            predicate = c => currentPredicate(c) || subPredicate(c);
        }

        var result = new List<Country>(countries.Length);
        for (int i = 0; i < countries.Length; i++)
        {
            if (!predicate(countries[i]))
                continue;
            result.Add(countries[i].Clone());
        }

        return result;
    }

    internal static List<Country> ListFrom(Country[] data)
    {
        var result = new List<Country>(data.Length);
        for (int i = 0; i < data.Length; i++)
            result.Add(data[i].Clone());
        return result;
    }
}
