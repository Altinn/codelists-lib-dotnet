﻿using Altinn.Codelists.Kartverket.AdministrativeUnits;
using Altinn.Codelists.Kartverket.AdministrativeUnits.Clients;
using Altinn.Codelists.Tests.Kartverket.AdministrativeUnits.Mocks;
using Microsoft.Extensions.Caching.Memory;

namespace Altinn.Codelists.Tests.Kartverket.AdministrativeUnits.Clients;

public class CountiesClientCachedTests
{
    [Fact]
    public async Task GetCounties_EmptyCache_ShouldReturnAllCounties()
    {
        var administrativeUnitsHttpClientMock = new AdministrativeUnitsHttpClientMock(Options.Create(new AdministrativeUnitsOptions()));
        var administrativeUnitsHttpClientCached = new AdministrativeUnitsHttpClientCached(administrativeUnitsHttpClientMock, new MemoryCache(new MemoryCacheOptions()));

        var counties = await administrativeUnitsHttpClientCached.GetCounties();

        counties.Should().HaveCount(11);
    }

    [Fact]
    public async Task GetCounties_CacheFilled_ShouldReturnFromCache()
    {
        var administrativeUnitsHttpClientMock = new AdministrativeUnitsHttpClientMock(Options.Create(new AdministrativeUnitsOptions()));
        var administrativeUnitsHttpClientCached = new AdministrativeUnitsHttpClientCached(administrativeUnitsHttpClientMock, new MemoryCache(new MemoryCacheOptions()));

        // First request will fill the cache
        await administrativeUnitsHttpClientCached.GetCounties();

        // Second request should not trigger another http request from the client
        var counties = await administrativeUnitsHttpClientCached.GetCounties();

        counties.Should().HaveCount(11);
        administrativeUnitsHttpClientMock.HttpMessageHandlerMock.GetMatchCount(administrativeUnitsHttpClientMock.MockedCountiesRequest).Should().Be(1);
    }

    [Fact]
    public async Task GetCounties_CacheExpired_ShouldPopulateAgain()
    {
        var administrativeUnitsHttpClientMock = new AdministrativeUnitsHttpClientMock(Options.Create(new AdministrativeUnitsOptions()));
        var administrativeUnitsHttpClientCached = new AdministrativeUnitsHttpClientCached(
            administrativeUnitsHttpClientMock,
            new MemoryCache(new MemoryCacheOptions()),
            () =>
            {
                // Let the cache entry live for 100 milliseconds
                return new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(100),
                    Priority = CacheItemPriority.Normal
                };
            });

        // First request will fill the cache
        await administrativeUnitsHttpClientCached.GetCounties();

        // Wait for the cached entry to be evicted
        Thread.Sleep(200);

        // This should trigger another http request and fill the cache again
        var counties = await administrativeUnitsHttpClientCached.GetCounties();

        counties.Should().HaveCount(11);
        administrativeUnitsHttpClientMock.HttpMessageHandlerMock.GetMatchCount(administrativeUnitsHttpClientMock.MockedCountiesRequest).Should().Be(2);
    }
}
