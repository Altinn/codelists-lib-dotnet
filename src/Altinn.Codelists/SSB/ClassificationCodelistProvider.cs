﻿using Altinn.App.Core.Features;
using Altinn.App.Core.Models;
using Altinn.Codelists.SSB.Models;

namespace Altinn.Codelists.SSB;

/// <summary>
/// Base class providing functions for getting codelist.
/// </summary>
public abstract class ClassificationCodelistProvider
{
    private readonly IClassificationsClient _classificationsClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassificationCodelistProvider"/> class.
    /// </summary>
    /// <param name="classificationsClient"></param>
    protected ClassificationCodelistProvider(IClassificationsClient classificationsClient)
    {
        _classificationsClient = classificationsClient;
    }

    /// Gets the <see cref="AppOptions"/> based on the provided classification, options id and key value pairs.
    protected async Task<AppOptions> GetAppOptionsAsync(Classification classification, string language, Dictionary<string, string> keyValuePairs)
    {
        string? date = keyValuePairs.GetValueOrDefault("date");
        DateOnly dateOnly = date == null ? DateOnly.FromDateTime(DateTime.Today) : DateOnly.Parse(date);
        string level = keyValuePairs.GetValueOrDefault("level") ?? string.Empty;

        var classificationCode = await _classificationsClient.GetClassificationCodes(classification, language, dateOnly, level);

        string parentCode = keyValuePairs.GetValueOrDefault("parentCode") ?? string.Empty;
        var appOptions = new AppOptions();

        // The api we use doesn't support filtering on partentCode,
        // hence we need to filter afterwards.
        appOptions.Options = string.IsNullOrEmpty(parentCode)
            ? classificationCode.Codes.Select(x => new AppOption() { Value = x.Code, Label = x.Name }).ToList()
            : classificationCode.Codes.Where(c => c.ParentCode == parentCode).Select(x => new AppOption() { Value = x.Code, Label = x.Name }).ToList();

        return appOptions;
    }
}