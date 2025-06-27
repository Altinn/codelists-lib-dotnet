using System.Runtime.CompilerServices;
using VerifyTests;

namespace Altinn.Codelists.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Auto-accept changes to snapshots when not running in CI
        // This allows snapshots to be automatically updated during local development
        // but ensures they are verified (not auto-updated) in CI environments
        if (Environment.GetEnvironmentVariable("CI") != "true")
        {
            VerifierSettings.AutoVerify();
        }
    }
}
