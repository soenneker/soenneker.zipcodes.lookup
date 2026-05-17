using System.Collections.Generic;
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
    public async ValueTask Get_should_return_zip_code_info()
    {
        ZipCodeInfo? result = await _util.Get("90210");

        result.Should().NotBeNull();
        result!.ZipCode.Should().Be("90210");
        result.City.Should().Be("Beverly Hills");
        result.State.Should().Be("CA");
        result.Latitude.Should().BeApproximately(34.0901, 0.001);
        result.Longitude.Should().BeApproximately(-118.4065, 0.001);
    }

    [Test]
    public async ValueTask Get_should_accept_zip_plus_four()
    {
        ZipCodeInfo? result = await _util.Get("90210-1234");

        result.Should().NotBeNull();
        result!.ZipCode.Should().Be("90210");
    }

    [Test]
    public async ValueTask GetLatitudeLongitude_should_return_coordinates()
    {
        (double Latitude, double Longitude)? result = await _util.GetLatitudeLongitude("10001");

        result.Should().NotBeNull();
        result!.Value.Latitude.Should().BeApproximately(40.7484, 0.001);
        result.Value.Longitude.Should().BeApproximately(-73.9967, 0.001);
    }

    [Test]
    public async ValueTask GetCoordinates_should_return_coordinates_without_full_record_lookup()
    {
        ZipCodeCoordinates? result = await _util.GetCoordinates("10001");

        result.Should().NotBeNull();
        result!.Value.Latitude.Should().BeApproximately(40.7484, 0.001);
        result.Value.Longitude.Should().BeApproximately(-73.9967, 0.001);
    }

    [Test]
    public async ValueTask GetTimeZoneId_should_return_time_zone_id()
    {
        string? chicago = await _util.GetTimeZoneId("60601");
        string? losAngeles = await _util.GetTimeZoneId("90210");

        chicago.Should().Be("America/Chicago");
        losAngeles.Should().Be("America/Los_Angeles");
    }

    [Test]
    public async ValueTask GetCity_and_GetState_should_return_values()
    {
        string? city = await _util.GetCity("60601");
        string? state = await _util.GetState("60601");

        city.Should().Be("Chicago");
        state.Should().Be("IL");
    }

    [Test]
    public async ValueTask Get_should_return_null_for_unknown_zip_code()
    {
        ZipCodeInfo? result = await _util.Get("00000");

        result.Should().BeNull();
    }

    [Test]
    public async ValueTask GetByState_should_return_state_zip_codes()
    {
        IReadOnlyList<ZipCodeInfo> results = await _util.GetByState("CA");

        results.Should().Contain(x => x.ZipCode == "90210");
    }

    [Test]
    public async ValueTask GetByCityAndState_should_return_city_state_zip_codes()
    {
        IReadOnlyList<ZipCodeInfo> results = await _util.GetByCityAndState("Beverly Hills", "CA");

        results.Should().Contain(x => x.ZipCode == "90210");
    }
}
