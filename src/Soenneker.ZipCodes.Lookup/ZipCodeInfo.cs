namespace Soenneker.ZipCodes.Lookup;

/// <summary>
/// A ZIP code geography record.
/// </summary>
public sealed record ZipCodeInfo(string ZipCode, string City, string State, double Latitude, double Longitude);
