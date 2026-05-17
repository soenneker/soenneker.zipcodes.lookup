using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Asyncs.Initializers;
using Soenneker.Extensions.ValueTask;
using Soenneker.TimeZones.Lookup.Abstract;
using Soenneker.Utils.File.Abstract;
using Soenneker.Utils.Paths.Resources.Abstract;
using Soenneker.ZipCodes.Lookup.Abstract;

namespace Soenneker.ZipCodes.Lookup;

/// <inheritdoc cref="IZipCodeLookupUtil"/>
public sealed class ZipCodeLookupUtil : IZipCodeLookupUtil
{
    private const string _fileName = "USZipCodeGeometry.txt";
    private static readonly IReadOnlyList<ZipCodeInfo> _emptyList = [];

    private readonly IFileUtil _fileUtil;
    private readonly IResourcesPathUtil _resourcesPathUtil;
    private readonly ITimeZoneLookupUtil _timeZoneLookupUtil;
    private readonly AsyncInitializer _initializer;
    private ZipCodeIndex? _index;

    public ZipCodeLookupUtil(IFileUtil fileUtil, IResourcesPathUtil resourcesPathUtil, ITimeZoneLookupUtil timeZoneLookupUtil)
    {
        _fileUtil = fileUtil;
        _resourcesPathUtil = resourcesPathUtil;
        _timeZoneLookupUtil = timeZoneLookupUtil;
        _initializer = new AsyncInitializer(Initialize);
    }

    public async ValueTask<IReadOnlyCollection<ZipCodeInfo>> GetAll(CancellationToken cancellationToken = default)
    {
        ZipCodeIndex index = await GetIndex(cancellationToken);
        return index.All;
    }

    public async ValueTask<ZipCodeInfo?> Get(string zipCode, CancellationToken cancellationToken = default)
    {
        if (!TryParseZipCode(zipCode, out int normalizedZipCode))
            return null;

        ZipCodeIndex index = await GetIndex(cancellationToken).NoSync();
        index.ByZipCode.TryGetValue(normalizedZipCode, out ZipCodeInfo? info);
        return info;
    }

    public async ValueTask<bool> Exists(string zipCode, CancellationToken cancellationToken = default)
    {
        if (!TryParseZipCode(zipCode, out int normalizedZipCode))
            return false;

        ZipCodeIndex index = await GetIndex(cancellationToken).NoSync();
        return index.ByZipCode.ContainsKey(normalizedZipCode);
    }

    public async ValueTask<string?> GetCity(string zipCode, CancellationToken cancellationToken = default)
    {
        ZipCodeInfo? info = await Get(zipCode, cancellationToken).NoSync();
        return info?.City;
    }

    public async ValueTask<string?> GetState(string zipCode, CancellationToken cancellationToken = default)
    {
        ZipCodeInfo? info = await Get(zipCode, cancellationToken);
        return info?.State;
    }

    public async ValueTask<double?> GetLatitude(string zipCode, CancellationToken cancellationToken = default)
    {
        ZipCodeCoordinates? coordinates = await GetCoordinates(zipCode, cancellationToken).NoSync();
        return coordinates?.Latitude;
    }

    public async ValueTask<double?> GetLongitude(string zipCode, CancellationToken cancellationToken = default)
    {
        ZipCodeCoordinates? coordinates = await GetCoordinates(zipCode, cancellationToken);
        return coordinates?.Longitude;
    }

    public async ValueTask<(double Latitude, double Longitude)?> GetLatitudeLongitude(string zipCode, CancellationToken cancellationToken = default)
    {
        ZipCodeCoordinates? coordinates = await GetCoordinates(zipCode, cancellationToken).NoSync();

        if (coordinates == null)
            return null;

        return (coordinates.Value.Latitude, coordinates.Value.Longitude);
    }

    public async ValueTask<ZipCodeCoordinates?> GetCoordinates(string zipCode, CancellationToken cancellationToken = default)
    {
        if (!TryParseZipCode(zipCode, out int normalizedZipCode))
            return null;

        ZipCodeIndex index = await GetIndex(cancellationToken).NoSync();
        return index.ByZipCodeCoordinates.TryGetValue(normalizedZipCode, out ZipCodeCoordinates coordinates) ? coordinates : null;
    }

    public async ValueTask<string?> GetTimeZoneId(string zipCode, CancellationToken cancellationToken = default)
    {
        ZipCodeCoordinates? coordinates = await GetCoordinates(zipCode, cancellationToken).NoSync();

        if (coordinates == null)
            return null;

        return await _timeZoneLookupUtil.GetTimeZoneId(coordinates.Value.Latitude, coordinates.Value.Longitude, cancellationToken).NoSync();
    }

    public async ValueTask<IReadOnlyList<ZipCodeInfo>> GetByState(string state, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(state))
            return _emptyList;

        ZipCodeIndex index = await GetIndex(cancellationToken).NoSync();
        return index.ByState.GetValueOrDefault(state.Trim(), _emptyList);
    }

    public async ValueTask<IReadOnlyList<ZipCodeInfo>> GetByCity(string city, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
            return _emptyList;

        ZipCodeIndex index = await GetIndex(cancellationToken).NoSync();
        return index.ByCity.GetValueOrDefault(city.Trim(), _emptyList);
    }

    public async ValueTask<IReadOnlyList<ZipCodeInfo>> GetByCityAndState(string city, string state, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(state))
            return _emptyList;

        string key = BuildCityStateKey(city.Trim(), state.Trim());

        ZipCodeIndex index = await GetIndex(cancellationToken).NoSync();
        return index.ByCityState.GetValueOrDefault(key, _emptyList);
    }

    private ValueTask<ZipCodeIndex> GetIndex(CancellationToken cancellationToken)
    {
        if (_index != null)
            return new ValueTask<ZipCodeIndex>(_index);

        return GetIndexSlow(cancellationToken);
    }

    private async ValueTask<ZipCodeIndex> GetIndexSlow(CancellationToken cancellationToken)
    {
        await _initializer.Init(cancellationToken);
        return _index!;
    }

    private async ValueTask Initialize(CancellationToken cancellationToken)
    {
        _index = await LoadIndex(cancellationToken).NoSync();
    }

    private async ValueTask<ZipCodeIndex> LoadIndex(CancellationToken cancellationToken)
    {
        string filePath = await GetDataFilePath(cancellationToken).NoSync();

        var all = new List<ZipCodeInfo>(45000);
        var byZipCode = new Dictionary<int, ZipCodeInfo>();
        var byZipCodeCoordinates = new Dictionary<int, ZipCodeCoordinates>();
        var byState = new Dictionary<string, List<ZipCodeInfo>>(StringComparer.OrdinalIgnoreCase);
        var byCity = new Dictionary<string, List<ZipCodeInfo>>(StringComparer.OrdinalIgnoreCase);
        var byCityState = new Dictionary<string, List<ZipCodeInfo>>(StringComparer.OrdinalIgnoreCase);

        var lineNumber = 0;

        await using FileStream fileStream = _fileUtil.OpenRead(filePath, log: false);
        using var reader = new StreamReader(fileStream);

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] columns = line.Split('\t');

            if (columns.Length != 5)
                throw new InvalidDataException($"Unexpected ZIP code geometry format at line {lineNumber}. Expected 5 tab-delimited columns.");

            if (!TryParseZipCode(columns[0], out int zipCode))
                throw new InvalidDataException($"Unexpected ZIP code geometry format at line {lineNumber}. ZIP code must be numeric.");

            var info = new ZipCodeInfo(columns[0], columns[1], columns[2], double.Parse(columns[3], CultureInfo.InvariantCulture),
                double.Parse(columns[4], CultureInfo.InvariantCulture));

            all.Add(info);
            byZipCode[zipCode] = info;
            byZipCodeCoordinates[zipCode] = new ZipCodeCoordinates(info.Latitude, info.Longitude);
            Add(byState, info.State, info);
            Add(byCity, info.City, info);
            Add(byCityState, BuildCityStateKey(info.City, info.State), info);
        }

        return new ZipCodeIndex(all.ToArray(), byZipCode.ToFrozenDictionary(), byZipCodeCoordinates.ToFrozenDictionary(), Freeze(byState), Freeze(byCity),
            Freeze(byCityState));
    }

    private async ValueTask<string> GetDataFilePath(CancellationToken cancellationToken)
    {
        string filePath = await _resourcesPathUtil.GetResourceFilePath(_fileName, cancellationToken).NoSync();

        if (await _fileUtil.Exists(filePath, cancellationToken).NoSync())
            return filePath;

        throw new FileNotFoundException(
            $"Could not locate {filePath}. Ensure the Soenneker.ZipCodes.Data.GeoNames content file is copied to the output directory.", filePath);
    }

    private static bool TryParseZipCode(string zipCode, out int result)
    {
        result = 0;

        if (string.IsNullOrWhiteSpace(zipCode))
            return false;

        zipCode = zipCode.Trim();
        int length = Math.Min(zipCode.Length, 5);

        for (var i = 0; i < length; i++)
        {
            char value = zipCode[i];

            if (value is < '0' or > '9')
                return false;

            result = (result * 10) + value - '0';
        }

        return true;
    }

    private static void Add(Dictionary<string, List<ZipCodeInfo>> dictionary, string key, ZipCodeInfo info)
    {
        if (!dictionary.TryGetValue(key, out List<ZipCodeInfo>? entries))
        {
            entries = [];
            dictionary[key] = entries;
        }

        entries.Add(info);
    }

    private static FrozenDictionary<string, IReadOnlyList<ZipCodeInfo>> Freeze(Dictionary<string, List<ZipCodeInfo>> source)
    {
        var result = new Dictionary<string, IReadOnlyList<ZipCodeInfo>>(source.Count, source.Comparer);

        foreach (KeyValuePair<string, List<ZipCodeInfo>> pair in source)
        {
            result[pair.Key] = pair.Value.ToArray();
        }

        return result.ToFrozenDictionary(source.Comparer);
    }

    private static string BuildCityStateKey(string city, string state)
    {
        return $"{state}\u001F{city}";
    }

    private sealed record ZipCodeIndex(
        IReadOnlyCollection<ZipCodeInfo> All,
        FrozenDictionary<int, ZipCodeInfo> ByZipCode,
        FrozenDictionary<int, ZipCodeCoordinates> ByZipCodeCoordinates,
        FrozenDictionary<string, IReadOnlyList<ZipCodeInfo>> ByState,
        FrozenDictionary<string, IReadOnlyList<ZipCodeInfo>> ByCity,
        FrozenDictionary<string, IReadOnlyList<ZipCodeInfo>> ByCityState);
}
