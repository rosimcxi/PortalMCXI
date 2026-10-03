using System.Globalization;
using System.Text.Json;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Services;

public sealed class MorningInfoService(HttpClient httpClient)
{
    private const string CnbRatesUrl =
        "https://www.cnb.cz/en/financial-markets/foreign-exchange-market/" +
        "central-bank-exchange-rate-fixing/central-bank-exchange-rate-fixing/daily.txt";

    public async Task<MorningInfoResponse> GetAsync(
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        var latText = Environment.GetEnvironmentVariable("PORTAL_LOCATION_LAT");
        var lonText = Environment.GetEnvironmentVariable("PORTAL_LOCATION_LON");
        var locationName = Environment.GetEnvironmentVariable("PORTAL_LOCATION_NAME") ?? "Configured location";
        var timezone = Environment.GetEnvironmentVariable("PORTAL_TIMEZONE") ?? "Europe/Prague";

        double? latitude = null;
        double? longitude = null;

        if (double.TryParse(latText, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedLat))
        {
            latitude = parsedLat;
        }

        if (double.TryParse(lonText, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedLon))
        {
            longitude = parsedLon;
        }

        WeatherSnapshot? weather = null;
        SunSnapshot? sun = null;
        FinanceSnapshot? finance = null;
        MetalsSnapshot? metals = null;

        if (latitude is not null && longitude is not null)
        {
            (weather, sun) = await LoadWeatherAsync(
                latitude.Value,
                longitude.Value,
                timezone,
                errors,
                cancellationToken);
        }
        else
        {
            errors.Add("PORTAL_LOCATION_LAT/PORTAL_LOCATION_LON nejsou nakonfigurovány.");
        }

        finance = await LoadFinanceAsync(errors, cancellationToken);

        if (finance is not null)
        {
            metals = await LoadMetalsAsync(
                finance.UsdCzk,
                errors,
                cancellationToken);
        }
        else
        {
            errors.Add("Kovy nelze převést do CZK bez USD/CZK.");
        }

        var status =
            errors.Count == 0
                ? "OK"
                : weather is not null || sun is not null || finance is not null || metals is not null
                    ? "PARTIAL"
                    : "CONFIG_REQUIRED";

        return new MorningInfoResponse(
            GeneratedAtUtc: DateTimeOffset.UtcNow,
            Location: new MorningLocation(
                locationName,
                latitude,
                longitude,
                timezone,
                latitude is not null && longitude is not null ? "OK" : "CONFIG_REQUIRED"),
            Weather: weather,
            Sun: sun,
            Finance: finance,
            Metals: metals,
            OverallStatus: status,
            Errors: errors.ToArray());
    }

    private async Task<(WeatherSnapshot? Weather, SunSnapshot? Sun)> LoadWeatherAsync(
        double latitude,
        double longitude,
        string timezone,
        List<string> errors,
        CancellationToken cancellationToken)
    {
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
                "Open-Meteo",
                current.GetProperty("time").GetString(),
                current.GetProperty("temperature_2m").GetDouble(),
                current.GetProperty("apparent_temperature").GetDouble(),
                current.GetProperty("precipitation").GetDouble(),
                current.GetProperty("weather_code").GetInt32(),
                current.GetProperty("cloud_cover").GetDouble(),
                current.GetProperty("pressure_msl").GetDouble(),
                current.GetProperty("wind_speed_10m").GetDouble(),
                current.GetProperty("wind_gusts_10m").GetDouble(),
                "OK");

            var sun = new SunSnapshot(
                "Open-Meteo",
                daily.GetProperty("time")[0].GetString(),
                daily.GetProperty("sunrise")[0].GetString(),
                daily.GetProperty("sunset")[0].GetString(),
                Math.Round(daily.GetProperty("daylight_duration")[0].GetDouble() / 60d, 1),
                "OK");

            return (weather, sun);
        }
        catch (Exception ex) when (
            ex is HttpRequestException or
            TaskCanceledException or
            JsonException or
            InvalidOperationException)
        {
            errors.Add($"Open-Meteo: {ex.GetType().Name}: {ex.Message}");
            return (null, null);
        }
    }

    private async Task<MetalsSnapshot?> LoadMetalsAsync(
        double usdCzk,
        List<string> errors,
        CancellationToken cancellationToken)
    {
        const double gramsPerTroyOunce = 31.1034768d;

        try
        {
            var goldTask = LoadMetalQuoteAsync("XAU", cancellationToken);
            var silverTask = LoadMetalQuoteAsync("XAG", cancellationToken);

            await Task.WhenAll(goldTask, silverTask);

            var gold = await goldTask;
            var silver = await silverTask;

            MetalPrice Convert(MetalQuote quote)
            {
                var usdPerGram = quote.UsdPerTroyOunce / gramsPerTroyOunce;
                var czkPerGram = usdPerGram * usdCzk;

                return new MetalPrice(
                    Symbol: quote.Symbol,
                    Name: quote.Name,
                    SourceUpdatedAt: quote.UpdatedAt,
                    UsdPerTroyOunce: Math.Round(quote.UsdPerTroyOunce, 4),
                    UsdPerGram: Math.Round(usdPerGram, 4),
                    CzkPerGram: Math.Round(czkPerGram, 2));
            }

            return new MetalsSnapshot(
                Source: "Gold API",
                Gold: Convert(gold),
                Silver: Convert(silver),
                Status: "OK",
                Note: "Orientační spot/reference cena; nejde o výkupní ani prodejní nabídku.");
        }
        catch (Exception ex) when (
            ex is HttpRequestException or
            TaskCanceledException or
            JsonException or
            InvalidOperationException)
        {
            errors.Add($"Gold API: {ex.GetType().Name}: {ex.Message}");
            return null;
        }
    }

    private async Task<MetalQuote> LoadMetalQuoteAsync(
        string symbol,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(
            $"https://api.gold-api.com/price/{symbol}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        using var json =
            await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);

        var root = json.RootElement;

        var returnedSymbol = root.GetProperty("symbol").GetString();
        var name = root.GetProperty("name").GetString();
        var price = root.GetProperty("price").GetDouble();
        var currency = root.TryGetProperty("currency", out var currencyElement)
            ? currencyElement.GetString()
            : "USD";
        var updatedAt = root.TryGetProperty("updatedAt", out var updatedElement)
            ? updatedElement.GetString()
            : null;

        if (!string.Equals(returnedSymbol, symbol, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(currency, "USD", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(name) ||
            price <= 0)
        {
            throw new InvalidOperationException(
                $"Neočekávaná odpověď pro {symbol}.");
        }

        return new MetalQuote(
            returnedSymbol!,
            name,
            price,
            updatedAt);
    }

    private sealed record MetalQuote(
        string Symbol,
        string Name,
        double UsdPerTroyOunce,
        string? UpdatedAt);

    private async Task<FinanceSnapshot?> LoadFinanceAsync(
        List<string> errors,
        CancellationToken cancellationToken)
    {
        try
        {
            var text = await httpClient.GetStringAsync(CnbRatesUrl, cancellationToken);
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (lines.Length < 3)
            {
                throw new InvalidOperationException("ČNB daily.txt nemá očekávaný formát.");
            }

            var rateDate = lines[0].Split('#')[0].Trim();
            double? usd = null;
            double? eur = null;

            foreach (var line in lines.Skip(2))
            {
                var parts = line.Split('|');
                if (parts.Length != 5)
                {
                    continue;
                }

                var code = parts[3].Trim();
                if (code is not ("USD" or "EUR"))
                {
                    continue;
                }

                if (!double.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var amount) ||
                    !double.TryParse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture, out var totalRate) ||
                    amount <= 0)
                {
                    continue;
                }

                var czkPerUnit = totalRate / amount;

                if (code == "USD") usd = czkPerUnit;
                if (code == "EUR") eur = czkPerUnit;
            }

            if (usd is null || eur is null)
            {
                throw new InvalidOperationException("ČNB nevrátila USD a EUR.");
            }

            return new FinanceSnapshot(
                Source: "ČNB",
                RateDate: rateDate,
                UsdCzk: Math.Round(usd.Value, 4),
                EurCzk: Math.Round(eur.Value, 4),
                Status: "OK");
        }
        catch (Exception ex) when (
            ex is HttpRequestException or
            TaskCanceledException or
            InvalidOperationException)
        {
            errors.Add($"ČNB: {ex.GetType().Name}: {ex.Message}");
            return null;
        }
    }
}
