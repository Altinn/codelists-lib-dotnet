using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Altinn.Codelists.Utilities;

/// <summary>
/// Helper class for embeded resources.
/// </summary>
internal static class EmbeddedResource
{
    private static readonly Assembly _assembly = typeof(EmbeddedResource).Assembly;

    /// <summary>
    /// Finds an embeded resource, by name, within the executing assembly and reads it as a <see cref="Stream"/>
    /// </summary>
    /// <param name="resourceName">The name of the resource including namespace.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static Stream LoadDataAsStream(string resourceName)
    {
        Stream? resourceStream = _assembly.GetManifestResourceStream(resourceName);
        try
        {
            if (resourceStream is null)
                ThrowForNullStream(resourceName);

            return resourceStream;
        }
        catch (Exception)
        {
            resourceStream?.Dispose();
            throw;
        }
    }

    [DoesNotReturn]
    private static void ThrowForNullStream(string resourceName)
    {
        throw new InvalidOperationException(
            $"Unable to find resource {resourceName} embedded in assembly {_assembly.FullName}."
        );
    }
}
