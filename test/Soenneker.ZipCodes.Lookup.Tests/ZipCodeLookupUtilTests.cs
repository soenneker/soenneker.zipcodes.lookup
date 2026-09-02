using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.ZipCodes.Lookup;
using Soenneker.ZipCodes.Lookup.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.ZipCodes.Lookup.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class ZipCodeLookupUtilTests : HostedUnitTest
{
    private readonly IZipCodeLookupUtil _util;

    public ZipCodeLookupUtilTests(Host host) : base(host)
    {
        _util = Resolve<IZipCodeLookupUtil>(true);
    }

    [Test]
    public async ValueTask Get_should_return_zip_code_info(CancellationToken cancellationToken)
    {
        ZipCodeInfo? result = await _util.Get("90210", cancellationToken: cancellationToken);

        result.Should().NotBeNull();
        result!.ZipCode.Should().Be("90210");
        result.City.Should().Be("Beverly Hills");
        result.State.Should().Be("CA");
        result.Latitude.Should().BeApproximately(34.0901, 0.001);
        result.Longitude.Should().BeApproximately(-118.4065, 0.001);
    }

    [Test]
    public async ValueTask Get_should_accept_zip_plus_four(CancellationToken cancellationToken)
    {
        ZipCodeInfo? result = await _util.Get("90210-1234", cancellationToken: cancellationToken);

        result.Should().NotBeNull();
        result!.ZipCode.Should().Be("90210");
    }

    [Test]
    public async ValueTask GetLatitudeLongitude_should_return_coordinates(CancellationToken cancellationToken)
    {
        (double Latitude, double Longitude)? result = await _util.GetLatitudeLongitude("10001", cancellationToken: cancellationToken);

        result.Should().NotBeNull();
        result!.Value.Latitude.Should().BeApproximately(40.7484, 0.001);
        result.Value.Longitude.Should().BeApproximately(-73.9967, 0.001);
    }

    [Test]
    public async ValueTask GetCoordinates_should_return_coordinates_without_full_record_lookup(CancellationToken cancellationToken)
    {
        ZipCodeCoordinates? result = await _util.GetCoordinates("10001", cancellationToken: cancellationToken);

        result.Should().NotBeNull();
        result!.Value.Latitude.Should().BeApproximately(40.7484, 0.001);
        result.Value.Longitude.Should().BeApproximately(-73.9967, 0.001);
    }

    [Test]
    public async ValueTask GetTimeZoneId_should_return_time_zone_id(CancellationToken cancellationToken)
    {
        string? chicago = await _util.GetTimeZoneId("60601", cancellationToken: cancellationToken);
        string? losAngeles = await _util.GetTimeZoneId("90210", cancellationToken: cancellationToken);

        chicago.Should().Be("America/Chicago");
        losAngeles.Should().Be("America/Los_Angeles");
    }

    [Test]
    public async ValueTask GetCity_and_GetState_should_return_values(CancellationToken cancellationToken)
    {
        string? city = await _util.GetCity("60601", cancellationToken: cancellationToken);
        string? state = await _util.GetState("60601", cancellationToken: cancellationToken);

        city.Should().Be("Chicago");
        state.Should().Be("IL");
    }

    [Test]
    public async ValueTask Get_should_return_null_for_unknown_zip_code(CancellationToken cancellationToken)
    {
        ZipCodeInfo? result = await _util.Get("00000", cancellationToken: cancellationToken);

        result.Should().BeNull();
    }

    [Test]
    public async ValueTask GetByState_should_return_state_zip_codes(CancellationToken cancellationToken)
    {
        IReadOnlyList<ZipCodeInfo> results = await _util.GetByState("CA", cancellationToken: cancellationToken);

        results.Should().Contain(x => x.ZipCode == "90210");
    }

    [Test]
    public async ValueTask GetByCityAndState_should_return_city_state_zip_codes(CancellationToken cancellationToken)
    {
        IReadOnlyList<ZipCodeInfo> results = await _util.GetByCityAndState("Beverly Hills", "CA", cancellationToken: cancellationToken);

        results.Should().Contain(x => x.ZipCode == "90210");
    }
}
