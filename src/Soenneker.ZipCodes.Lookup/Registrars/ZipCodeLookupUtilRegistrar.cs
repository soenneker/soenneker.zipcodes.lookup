using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.TimeZones.Lookup.Registrars;
using Soenneker.Utils.File.Registrars;
using Soenneker.Utils.Paths.Resources.Registrars;
using Soenneker.ZipCodes.Lookup.Abstract;

namespace Soenneker.ZipCodes.Lookup.Registrars;

/// <summary>
/// Fast ZipCode Resolution for .NET
/// </summary>
public static class ZipCodeLookupUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="IZipCodeLookupUtil"/> as a singleton service. <para/>
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddZipCodeLookupUtilAsSingleton(this IServiceCollection services)
    {
        services.AddFileUtilAsSingleton()
                .AddResourcesPathUtilAsSingleton()
                .AddTimeZoneLookupUtilAsSingleton()
                .TryAddSingleton<IZipCodeLookupUtil, ZipCodeLookupUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IZipCodeLookupUtil"/> as a scoped service. <para/>
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddZipCodeLookupUtilAsScoped(this IServiceCollection services)
    {
        services.AddFileUtilAsScoped()
                .AddResourcesPathUtilAsScoped()
                .AddTimeZoneLookupUtilAsScoped()
                .TryAddScoped<IZipCodeLookupUtil, ZipCodeLookupUtil>();

        return services;
    }
}
