using Altinn.Codelists.RestCountries.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Altinn.Codelists.RestCountries;

/// <summary>
/// Extends the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="ICountryClient"/> interface
    /// </summary>
    public static IServiceCollection AddRestCountriesClient(this IServiceCollection services)
    {
        services.TryAddSingleton<ICountryClient, CountriesClient>();

        return services;
    }
}
