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
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the collection returned by get All.</returns>
    ValueTask<IReadOnlyCollection<ZipCodeInfo>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the ZIP code record, if present.
    /// </summary>
    /// <param name="zipCode">ZIP code to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested zip Code Info.</returns>
    ValueTask<ZipCodeInfo?> Get(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets whether a ZIP code exists in the lookup data.
    /// </summary>
    /// <param name="zipCode">ZIP code to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>true if gets whether a ZIP code exists in the lookup data; otherwise, false.</returns>
    ValueTask<bool> Exists(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the city for a ZIP code, if present.
    /// </summary>
    /// <param name="zipCode">ZIP code to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get City.</returns>
    ValueTask<string?> GetCity(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the state or territory abbreviation for a ZIP code, if present.
    /// </summary>
    /// <param name="zipCode">ZIP code to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get State.</returns>
    ValueTask<string?> GetState(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latitude for a ZIP code, if present.
    /// </summary>
    /// <param name="zipCode">ZIP code to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested value.</returns>
    ValueTask<double?> GetLatitude(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the longitude for a ZIP code, if present.
    /// </summary>
    /// <param name="zipCode">ZIP code to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested value.</returns>
    ValueTask<double?> GetLongitude(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latitude and longitude for a ZIP code, if present.
    /// </summary>
    /// <param name="zipCode">ZIP code to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested (double Latitude, double Longitude).</returns>
    ValueTask<(double Latitude, double Longitude)?> GetLatitudeLongitude(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get the latitude and longitude for a ZIP code.
    /// </summary>
    /// <param name="zipCode">ZIP code to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested zip Code Coordinates.</returns>
    ValueTask<ZipCodeCoordinates?> GetCoordinates(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the IANA time zone identifier for a ZIP code, if present.
    /// </summary>
    /// <param name="zipCode">ZIP code to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get Time Zone ID.</returns>
    ValueTask<string?> GetTimeZoneId(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all ZIP code records in a state or territory.
    /// </summary>
    /// <param name="state">State value used by the variant.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the collection returned by get By State.</returns>
    ValueTask<IReadOnlyList<ZipCodeInfo>> GetByState(string state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all ZIP code records for a city.
    /// </summary>
    /// <param name="city">City name to search for.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the collection returned by get By City.</returns>
    ValueTask<IReadOnlyList<ZipCodeInfo>> GetByCity(string city, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all ZIP code records for a city and state or territory.
    /// </summary>
    /// <param name="city">City name to search for.</param>
    /// <param name="state">State value used by the variant.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the collection returned by get By City And State.</returns>
    ValueTask<IReadOnlyList<ZipCodeInfo>> GetByCityAndState(string city, string state, CancellationToken cancellationToken = default);
}
