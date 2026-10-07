// Název: PortalMCXI.Checks | Datum: 2026-10-07 | Verze: 1.0.0
// Účel: Ověřit MorningInfo na syntetických datech bez externích API, DB a RFU.
using System.Net;
using System.Text.Json;
using PortalMCXIBackend.Services;

var checks = 0;
void Check(string name, bool condition)
{
    if (!condition) throw new InvalidOperationException(name);
    checks++;
    Console.WriteLine($"CHECK=PASS {name}");
}

// Fixture je izolovaný proces CI. Žádné osobní údaje ani produkční konfigurace.
Environment.SetEnvironmentVariable("PORTAL_LOCATION_NAME", "CI fixture");
Environment.SetEnvironmentVariable("PORTAL_LOCATION_LAT", "50");
Environment.SetEnvironmentVariable("PORTAL_LOCATION_LON", "14");
Environment.SetEnvironmentVariable("PORTAL_TIMEZONE", "Europe/Prague");
Environment.SetEnvironmentVariable("PORTAL_BIRTH_DATE", "2000-01-01");
using var handler = new FixtureHandler();
using var client = new HttpClient(handler);
var calculations = new PersonalCalculationService();
var service = new MorningInfoService(client, calculations);
var morning = await service.GetAsync();
Check("complete snapshot", morning.OverallStatus == "OK" && morning.Errors.Length == 0);
Check("weather and sun", morning.Weather?.TemperatureC == 20 && morning.Sun?.DaylightMinutes == 720);
Check("currency amount normalization", morning.Finance?.UsdCzk == 25 && morning.Finance?.EurCzk == 24);
Check("metals CZK per gram", morning.Metals?.Gold.CzkPerGram == Math.Round(1000d / 31.1034768d * 25, 2));
Check("personal calculations", morning.Personal?.Numerology?.LifeNumber == 4 && morning.Personal.Tatva is not null);
var sunrise = new DateTime(2026, 1, 1, 6, 0, 0);
Check("tatva exact boundary", calculations.CalculateTatva(sunrise, sunrise).DalsiZmena == "06:24");
Check("tatva next boundary", calculations.CalculateTatva(sunrise.AddMinutes(24), sunrise).DalsiZmena == "06:48");
Environment.SetEnvironmentVariable("PORTAL_BIRTH_DATE", "2999-01-01");
morning = await service.GetAsync();
Check("future profile rejected", morning.Personal?.Numerology is null && morning.Personal?.Biorhythm is null);
handler.MalformedWeather = true;
morning = await service.GetAsync();
Check("malformed weather retains finance", morning.OverallStatus == "PARTIAL" && morning.Weather is null && morning.Finance is not null);
Console.WriteLine($"MORNING_FIXTURE=PASS CHECKS={checks} RFU_DISPATCH=NO DATABASE_ACCESS=NO");

sealed class FixtureHandler : HttpMessageHandler
{
    public bool MalformedWeather { get; set; }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri!;
        string content;
        if (uri.Host == "www.cnb.cz")
            content = "01 Jan 2026 #1\nCountry|Currency|Amount|Code|Rate\nUSA|dollar|2|USD|50.000\nEMU|euro|1|EUR|24.000\n";
        else if (uri.Host == "api.gold-api.com")
        {
            var symbol = uri.AbsolutePath.Split('/')[^1];
            content = JsonSerializer.Serialize(new { symbol, name = symbol, price = symbol == "XAU" ? 1000 : 20, currency = "USD" });
        }
        else if (uri.Host == "api.open-meteo.com")
        {
            var today = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague")).Date;
            var yesterday = today.AddDays(-1);
            content = MalformedWeather ? "{}" : JsonSerializer.Serialize(new
            {
                current = new { time = today.ToString("yyyy-MM-dd"), temperature_2m = 20, apparent_temperature = 20, precipitation = 0, weather_code = 0, cloud_cover = 0, pressure_msl = 1013, wind_speed_10m = 5, wind_gusts_10m = 10 },
                daily = new
                {
                    time = new[] { yesterday.ToString("yyyy-MM-dd"), today.ToString("yyyy-MM-dd") },
                    sunrise = new[] { yesterday.AddHours(6).ToString("s"), today.AddHours(6).ToString("s") },
                    sunset = new[] { yesterday.AddHours(18).ToString("s"), today.AddHours(18).ToString("s") },
                    daylight_duration = new[] { 43200, 43200 }
                }
            });
        }
        else throw new InvalidOperationException("Unexpected outbound request");
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) });
    }
}
