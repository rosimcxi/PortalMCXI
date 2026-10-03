// =====================================================================// Soubor: backend/Endpoints/EsoterikaEndpoints.cs// Popis: Výpočetní logika pro Tatvy, Numerologii a Biorytmy.// =====================================================================using PortalMCXIBackend.Models;namespace PortalMCXIBackend.Endpoints;public static class EsoterikaEndpoints{public static void MapEsoterikaEndpoints(this WebApplication app){// Skupina URL začínající na /api/esoterikavar group = app.MapGroup("/api/esoterika");    // -------------------------------------------------------------
    // A. TATVY
    // Princip: Den začíná východem slunce (fixně 6:00 pro ukázku).
    // Každých 24 minut se prostřídá 5 tater (5 elementů).
    // -------------------------------------------------------------
    group.MapGet("/tatvy", () => {
        var now = DateTime.Now;
        var sunrise = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0); 
        
        // Pokud je po půlnoci, ale ještě nebylo 6:00 ráno, počítáme to k včerejšku
        if (now < sunrise) sunrise = sunrise.AddDays(-1);

        // Pole tater (pořadí je pevně dané)
        var tatvyList = new[] { 
            ("Akáša", "Prostor/Éter", "Černá/Tmavě modrá"), 
            ("Váju", "Vzduch", "Zelená/Modrá"), 
            ("Tédžas", "Oheň", "Červená"), 
            ("Prithví", "Země", "Žlutá"), 
            ("Ápas", "Voda", "Stříbrná/Bílá") 
        };

        // Zjistíme, kolik minut uběhlo od východu slunce a vydělíme to 24 (délka jedné tatvy)
        var minutesSinceSunrise = (now - sunrise).TotalMinutes;
        var tatvaIndex = (int)Math.Floor(minutesSinceSunrise / 24) % 5;
        
        var currentTatva = tatvyList[tatvaIndex];
        
        // Výpočet času, kdy začne další tatva
        var nextChange = sunrise.AddMinutes(Math.Ceiling(minutesSinceSunrise / 24) * 24);

        return Results.Ok(new TatvaInfo(currentTatva.Item1, currentTatva.Item2, currentTatva.Item3, nextChange.ToString("HH:mm")));
    }).WithName("GetTatvy");

    // -------------------------------------------------------------
    // B. NUMEROLOGIE
    // Princip: Sečíst všechny číslice v datu narození, dokud nedostaneme
    // jednociferné číslo (výjimkou jsou tzv. mistrovská čísla 11, 22, 33).
    // -------------------------------------------------------------
    group.MapGet("/numerologie", (string birthDateStr) => {
        if (!DateTime.TryParse(birthDateStr, out DateTime birthDate))
            return Results.BadRequest("Neplatný formát data. Použijte RRRR-MM-DD.");

        // Lokální rekurzivní funkce pro sčítání číslic čísla (např. 1985 -> 1+9+8+5)
        int sumDate(int number) => number == 0 ? 0 : number % 10 + sumDate(number / 10);
        
        // Lokální funkce pro redukci na jednociferné nebo mistrovské číslo
        int reduceToSingleDigit(int num) {
            while (num > 9 && num != 11 && num != 22 && num != 33) {
                num = sumDate(num);
            }
            return num;
        }

        // Životní číslo z data narození
        int lifeNumberRaw = sumDate(birthDate.Year) + sumDate(birthDate.Month) + sumDate(birthDate.Day);
        int lifeNumber = reduceToSingleDigit(lifeNumberRaw);
        
        // Osobní roční číslo (aktuální rok + měsíc a den narození)
        int currentYearRaw = sumDate(DateTime.Now.Year) + sumDate(birthDate.Month) + sumDate(birthDate.Day);
        int personalYear = reduceToSingleDigit(currentYearRaw);

        return Results.Ok(new NumerologieResult(lifeNumber, personalYear, $"Tvé životní číslo je {lifeNumber}. Detailní text z databáze dodáme později."));
    }).WithName("GetNumerologie");

    // -------------------------------------------------------------
    // C. KONDICIOGRAM (Biorytmy)
    // Princip: Život běží v přesných cyklech (Fyzický 23, Emocionální 28, 
    // Intelektuální 33 dní). Používá se matematická funkce Sinus.
    // -------------------------------------------------------------
    group.MapGet("/kondiciogram", (string birthDateStr) => {
        if (!DateTime.TryParse(birthDateStr, out DateTime birthDate))
            return Results.BadRequest("Neplatný formát data. Použijte RRRR-MM-DD.");

        var daysAlive = (DateTime.Now - birthDate).TotalDays;

        // Sinusová křivka: vrací výsledek od -100% do +100%
        double calcBiorhythm(double cycleDays) => Math.Sin((2 * Math.PI * daysAlive) / cycleDays) * 100;

        return Results.Ok(new KondiciogramResult(
            Math.Round(calcBiorhythm(23), 1),
            Math.Round(calcBiorhythm(28), 1),
            Math.Round(calcBiorhythm(33), 1)
        ));
    }).WithName("GetKondiciogram");
}
}