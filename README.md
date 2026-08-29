[![](https://img.shields.io/nuget/v/soenneker.zipcodes.lookup.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.zipcodes.lookup/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.zipcodes.lookup/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.zipcodes.lookup/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.zipcodes.lookup.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.zipcodes.lookup/)

# Soenneker.ZipCodes.Lookup

Fast ZipCode Resolution for .NET.

## Install

```bash
dotnet add package Soenneker.ZipCodes.Lookup
```

## Quick start

```csharp
using Soenneker.ZipCodes.Lookup.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddZipCodeLookupUtilAsSingleton();
```

Adds `IZipCodeLookupUtil` as a singleton service.

## What you get

- `IZipCodeLookupUtil` — Fast ZipCode Resolution for .NET.
- `ZipCodeLookupUtilRegistrar` — Fast ZipCode Resolution for .NET.
- `ZipCodeCoordinates` — Latitude and longitude coordinates for a ZIP code.
- `ZipCodeInfo` — A ZIP code geography record.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IZipCodeLookupUtil.GetAll(cancellationToken)` | Gets all known ZIP code records. | The matching records as a materialized collection. |
| `IZipCodeLookupUtil.Get(zipCode, cancellationToken)` | Gets the ZIP code record, if present. | A task whose result is the requested zip Code Info. |
| `IZipCodeLookupUtil.Exists(zipCode, cancellationToken)` | Gets whether a ZIP code exists in the lookup data. | Returns `true` when at least one matching document exists; otherwise, `false`. |
| `IZipCodeLookupUtil.GetCity(zipCode, cancellationToken)` | Gets the city for a ZIP code, if present. | A task whose result is the text returned by get City. |
| `IZipCodeLookupUtil.GetState(zipCode, cancellationToken)` | Gets the state or territory abbreviation for a ZIP code, if present. | A task whose result is the text returned by get State. |
| `IZipCodeLookupUtil.GetLatitude(zipCode, cancellationToken)` | Gets the latitude for a ZIP code, if present. | A task whose result is the requested value. |
| `IZipCodeLookupUtil.GetLongitude(zipCode, cancellationToken)` | Gets the longitude for a ZIP code, if present. | A task whose result is the requested value. |
| `IZipCodeLookupUtil.GetLatitudeLongitude(zipCode, cancellationToken)` | Gets the latitude and longitude for a ZIP code, if present. | A task whose result is the requested (double Latitude, double Longitude). |
| `IZipCodeLookupUtil.GetCoordinates(zipCode, cancellationToken)` | Attempts to get the latitude and longitude for a ZIP code. | A task whose result is the requested zip Code Coordinates. |
| `IZipCodeLookupUtil.GetTimeZoneId(zipCode, cancellationToken)` | Gets the IANA time zone identifier for a ZIP code, if present. | A task whose result is the text returned by get Time Zone ID. |
| `IZipCodeLookupUtil.GetByState(state, cancellationToken)` | Gets all ZIP code records in a state or territory. | The matching records as a materialized collection. |
| `IZipCodeLookupUtil.GetByCity(city, cancellationToken)` | Gets all ZIP code records for a city. | The matching records as a materialized collection. |
| `IZipCodeLookupUtil.GetByCityAndState(city, state, cancellationToken)` | Gets all ZIP code records for a city and state or territory. | The matching records as a materialized collection. |
| `ZipCodeLookupUtilRegistrar.AddZipCodeLookupUtilAsSingleton(services)` | Adds `IZipCodeLookupUtil` as a singleton service. | The same service collection, so additional registrations can be chained. |
| `ZipCodeLookupUtilRegistrar.AddZipCodeLookupUtilAsScoped(services)` | Adds `IZipCodeLookupUtil` as a scoped service. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
