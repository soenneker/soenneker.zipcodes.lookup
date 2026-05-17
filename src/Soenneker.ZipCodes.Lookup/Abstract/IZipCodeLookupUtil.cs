using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.ZipCodes.Lookup.Abstract;

/// <summary>
/// Fast ZipCode Resolution for .NET
/// </summary>
public interface IZipCodeLookupUtil
{
    /// <summary>
    /// Gets all known ZIP code records.
    /// </summary>
    ValueTask<IReadOnlyCollection<ZipCodeInfo>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the ZIP code record, if present.
    /// </summary>
    ValueTask<ZipCodeInfo?> Get(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets whether a ZIP code exists in the lookup data.
    /// </summary>
    ValueTask<bool> Exists(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the city for a ZIP code, if present.
    /// </summary>
    ValueTask<string?> GetCity(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the state or territory abbreviation for a ZIP code, if present.
    /// </summary>
    ValueTask<string?> GetState(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latitude for a ZIP code, if present.
    /// </summary>
    ValueTask<double?> GetLatitude(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the longitude for a ZIP code, if present.
    /// </summary>
    ValueTask<double?> GetLongitude(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latitude and longitude for a ZIP code, if present.
    /// </summary>
    ValueTask<(double Latitude, double Longitude)?> GetLatitudeLongitude(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get the latitude and longitude for a ZIP code.
    /// </summary>
    ValueTask<ZipCodeCoordinates?> GetCoordinates(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the IANA time zone identifier for a ZIP code, if present.
    /// </summary>
    ValueTask<string?> GetTimeZoneId(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all ZIP code records in a state or territory.
    /// </summary>
    ValueTask<IReadOnlyList<ZipCodeInfo>> GetByState(string state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all ZIP code records for a city.
    /// </summary>
    ValueTask<IReadOnlyList<ZipCodeInfo>> GetByCity(string city, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all ZIP code records for a city and state or territory.
    /// </summary>
    ValueTask<IReadOnlyList<ZipCodeInfo>> GetByCityAndState(string city, string state, CancellationToken cancellationToken = default);
}
