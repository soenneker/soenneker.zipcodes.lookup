namespace Soenneker.ZipCodes.Lookup;

/// <summary>
/// Latitude and longitude coordinates for a ZIP code.
/// </summary>
public readonly record struct ZipCodeCoordinates(double Latitude, double Longitude);
