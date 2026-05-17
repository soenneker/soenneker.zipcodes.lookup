[![](https://img.shields.io/nuget/v/soenneker.zipcodes.lookup.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.zipcodes.lookup/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.zipcodes.lookup/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.zipcodes.lookup/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.zipcodes.lookup.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.zipcodes.lookup/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.ZipCodes.Lookup
### Fast ZipCode Resolution for .NET

## Installation

```
dotnet add package Soenneker.ZipCodes.Lookup
```

## Usage

```csharp
services.AddZipCodeLookupUtilAsSingleton();
```

```csharp
ZipCodeInfo? info = await zipCodeLookupUtil.Get("90210");
string? city = await zipCodeLookupUtil.GetCity("90210");
string? state = await zipCodeLookupUtil.GetState("90210");
(double Latitude, double Longitude)? coordinates = await zipCodeLookupUtil.GetLatitudeLongitude("90210");
string? timeZoneId = await zipCodeLookupUtil.GetTimeZoneId("90210");

ZipCodeCoordinates? fastCoordinates = await zipCodeLookupUtil.GetCoordinates("90210");

if (fastCoordinates != null)
{
    // use fastCoordinates.Value.Latitude and fastCoordinates.Value.Longitude
}

IReadOnlyList<ZipCodeInfo> californiaZipCodes = await zipCodeLookupUtil.GetByState("CA");
IReadOnlyList<ZipCodeInfo> beverlyHillsZipCodes = await zipCodeLookupUtil.GetByCityAndState("Beverly Hills", "CA");
```

The lookup data is loaded lazily from `USZipCodeGeometry.txt`, provided by `Soenneker.ZipCodes.Data.GeoNames`.
