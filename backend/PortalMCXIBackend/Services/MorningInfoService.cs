using System.Globalization;
using System.Text.Json;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Services;

public sealed class MorningInfoService(HttpClient httpClient)
{
    public async Task<MorningInfoResponse> GetAsync(
        CancellationToken cancellationToken = default)
    {
        var latText = Environment.GetEnvironmentVariable("PORTAL_LOCATION_LAT");
        var lonText = Environment.GetEnvironmentVariable("PORTAL_LOCATION_LON");
        var locationName = Environment.GetEnvironmentVariable("PORTAL_LOCATION_NAME") ?? "Configured location";
        var timezone = Environment.GetEnvironmentVariable("PORTAL_TIMEZONE") ?? "Europe/Prague";

        if (!double.TryParse(latText, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude) ||
            !double.TryParse(lonText, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
        {
            return new MorningInfoResponse(
                GeneratedAtUtc: DateTimeOffset.UtcNow,
                Location: new MorningLocation(locationName, null, null, timezone, "CONFIG_REQUIRED"),
                Weather: null,
                Sun: null,
                OverallStatus: "CONFIG_REQUIRED",
                Errors: ["PORTAL_LOCATION_LAT/PORTAL_LOCATION_LON nejsou nakonfigurovány."]);
        }

        var query =
            $"https://api.open-meteo.com/v1/forecast" +
            $"?latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
            $"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}" +
            "&current=temperature_2m,apparent_temperature,precipitation,weather_code,cloud_cover,pressure_msl,wind_speed_10m,wind_gusts_10m" +
            "&daily=sunrise,sunset,daylight_duration" +
            $"&timezone={Uri.EscapeDataString(timezone)}" +
            "&forecast_days=1";

        try
        {
            using var response = await httpClient.GetAsync(query, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var root = json.RootElement;
            var current = root.GetProperty("current");
            var daily = root.GetProperty("daily");

            var weather = new WeatherSnapshot(
                Source: "Open-Meteo",
                ObservedAt: current.GetProperty("time").GetString(),
                TemperatureC: current.GetProperty("temperature_2m").GetDouble(),
                ApparentTemperatureC: current.GetProperty("apparent_temperature").GetDouble(),
                PrecipitationMm: current.GetProperty("precipitation").GetDouble(),
                WeatherCode: current.GetProperty("weather_code").GetInt32(),
                CloudCoverPercent: current.GetProperty("cloud_cover").GetDouble(),
                PressureMslHpa: current.GetProperty("pressure_msl").GetDouble(),
                WindSpeedKmh: current.GetProperty("wind_speed_10m").GetDouble(),
                WindGustKmh: current.GetProperty("wind_gusts_10m").GetDouble(),
                Status: "OK");

            var sunrise = daily.GetProperty("sunrise")[0].GetString();
            var sunset = daily.GetProperty("sunset")[0].GetString();
            var daylightSeconds = daily.GetProperty("daylight_duration")[0].GetDouble();

            var sun = new SunSnapshot(
                Source: "Open-Meteo",
                Date: daily.GetProperty("time")[0].GetString(),
                Sunrise: sunrise,
                Sunset: sunset,
                DaylightMinutes: Math.Round(daylightSeconds / 60d, 1),
                Status: "OK");

            return new MorningInfoResponse(
                GeneratedAtUtc: DateTimeOffset.UtcNow,
                Location: new MorningLocation(locationName, latitude, longitude, timezone, "OK"),
                Weather: weather,
                Sun: sun,
                OverallStatus: "OK",
                Errors: []);
        }
        catch (Exception ex) when (
            ex is HttpRequestException or
            TaskCanceledException or
            JsonException or
            InvalidOperationException)
        {
            return new MorningInfoResponse(
                GeneratedAtUtc: DateTimeOffset.UtcNow,
                Location: new MorningLocation(locationName, latitude, longitude, timezone, "OK"),
                Weather: null,
                Sun: null,
                OverallStatus: "PARTIAL",
                Errors: [$"Open-Meteo: {ex.GetType().Name}: {ex.Message}"]);
        }
    }
}
