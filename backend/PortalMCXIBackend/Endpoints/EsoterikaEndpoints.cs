using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class EsoterikaEndpoints
{
    public static void MapEsoterikaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/esoterika");

        group.MapGet("/tatvy", () =>
        {
            var now = DateTime.Now;
            var sunrise = new DateTime(
                now.Year,
                now.Month,
                now.Day,
                6,
                0,
                0);

            if (now < sunrise)
            {
                sunrise = sunrise.AddDays(-1);
            }

            var tatvyList = new[]
            {
                ("Akáša", "Prostor/Éter", "Černá/Tmavě modrá"),
                ("Váju", "Vzduch", "Zelená/Modrá"),
                ("Tédžas", "Oheň", "Červená"),
                ("Prithví", "Země", "Žlutá"),
                ("Ápas", "Voda", "Stříbrná/Bílá")
            };

            var minutesSinceSunrise = (now - sunrise).TotalMinutes;
            var tatvaIndex =
                (int)Math.Floor(minutesSinceSunrise / 24) % 5;

            var currentTatva = tatvyList[tatvaIndex];
            var nextChange = sunrise.AddMinutes(
                Math.Ceiling(minutesSinceSunrise / 24) * 24);

            return Results.Ok(new TatvaInfo(
                currentTatva.Item1,
                currentTatva.Item2,
                currentTatva.Item3,
                nextChange.ToString("HH:mm")));
        })
        .WithName("GetTatvy");

        group.MapGet("/numerologie", (string birthDateStr) =>
        {
            if (!DateTime.TryParse(
                    birthDateStr,
                    out var birthDate))
            {
                return Results.BadRequest(
                    "Neplatný formát data. Použijte RRRR-MM-DD.");
            }

            static int SumDigits(int number) =>
                number == 0
                    ? 0
                    : number % 10 + SumDigits(number / 10);

            static int ReduceToSingleDigit(int number)
            {
                while (number > 9 &&
                       number != 11 &&
                       number != 22 &&
                       number != 33)
                {
                    number = SumDigits(number);
                }

                return number;
            }

            var lifeNumberRaw =
                SumDigits(birthDate.Year) +
                SumDigits(birthDate.Month) +
                SumDigits(birthDate.Day);

            var lifeNumber =
                ReduceToSingleDigit(lifeNumberRaw);

            var currentYearRaw =
                SumDigits(DateTime.Now.Year) +
                SumDigits(birthDate.Month) +
                SumDigits(birthDate.Day);

            var personalYear =
                ReduceToSingleDigit(currentYearRaw);

            return Results.Ok(new NumerologieResult(
                lifeNumber,
                personalYear,
                $"Tvé životní číslo je {lifeNumber}. " +
                "Detailní text z databáze dodáme později."));
        })
        .WithName("GetNumerologie");

        group.MapGet("/kondiciogram", (string birthDateStr) =>
        {
            if (!DateTime.TryParse(
                    birthDateStr,
                    out var birthDate))
            {
                return Results.BadRequest(
                    "Neplatný formát data. Použijte RRRR-MM-DD.");
            }

            var daysAlive =
                (DateTime.Now - birthDate).TotalDays;

            double CalcBiorhythm(double cycleDays) =>
                Math.Sin(
                    (2 * Math.PI * daysAlive) / cycleDays) * 100;

            return Results.Ok(new KondiciogramResult(
                Math.Round(CalcBiorhythm(23), 1),
                Math.Round(CalcBiorhythm(28), 1),
                Math.Round(CalcBiorhythm(33), 1)));
        })
        .WithName("GetKondiciogram");
    }
}
