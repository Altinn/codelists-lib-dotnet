﻿using Altinn.Codelists.SSB.Models;
using Microsoft.Extensions.Options;

namespace Altinn.Codelists.SSB.Clients;

/// <summary>
/// Http client to get information on norways offical administrative units for counties and communes.
/// </summary>
public class ClassificationsHttpClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassificationsHttpClient"/> class.
    /// </summary>
    public ClassificationsHttpClient(IOptions<ClassificationOptions> options, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.BaseApiUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json;charset=utf-8");
    }

    /// <summary>
    /// Gets the codes for the specified classification.
    /// </summary>
    /// <param name="classification"></param>
    /// <returns></returns>
    public async Task<ClassificationCodes> GetClassificationCodes(Classification classification)
    {
        int classificationNumber = (int)classification;
        string fromDate = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");
        var response = await _httpClient.GetAsync($"{classificationNumber}/codes?from={fromDate}");
        var responseJson = await response.Content.ReadAsStringAsync();

        var classificationCodes = JsonSerializer.Deserialize<ClassificationCodes>(responseJson);

        return classificationCodes ?? new ClassificationCodes();
    }
}
