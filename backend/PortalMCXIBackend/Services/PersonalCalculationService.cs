using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Services;

public sealed class PersonalCalculationService
{
    private static readonly (string Name, string Element, string Color)[] Tatvas =
    [
        ("Akáša", "Prostor/Éter", "Černá/Tmavě modrá"),
        ("Váju", "Vzduch", "Zelená/Modrá"),
        ("Tédžas", "Oheň", "Červená"),
        ("Prithví", "Země", "Žlutá"),
        ("Ápas", "Voda", "Stříbrná/Bílá")
    ];

    public TatvaInfo CalculateTatva(
        DateTime localNow,
        DateTime latestSunrise)
    {
        if (latestSunrise > localNow)
        {
            throw new ArgumentOutOfRangeException(
                nameof(latestSunrise),
                "Latest sunrise must not be in the future.");
        }

        var minutesSinceSunrise =
            (localNow - latestSunrise).TotalMinutes;

        var cycleIndex =
            (int)Math.Floor(minutesSinceSunrise / 24d);

        var tatvaIndex =
            ((cycleIndex % Tatvas.Length) + Tatvas.Length) %
            Tatvas.Length;

        var current = Tatvas[tatvaIndex];
        var nextChange =
            latestSunrise.AddMinutes((cycleIndex + 1) * 24d);

        return new TatvaInfo(
            current.Name,
            current.Element,
            current.Color,
            nextChange.ToString("HH:mm"));
    }

    public NumerologieResult CalculateNumerology(
        DateOnly birthDate,
        DateOnly localDate)
    {
        var lifeRaw =
            SumDigits(birthDate.Year) +
            SumDigits(birthDate.Month) +
            SumDigits(birthDate.Day);

        var lifeNumber = Reduce(lifeRaw);

        var personalYear = Reduce(
            SumDigits(localDate.Year) +
            SumDigits(birthDate.Month) +
            SumDigits(birthDate.Day));

        return new NumerologieResult(
            lifeNumber,
            personalYear,
            $"Životní číslo {lifeNumber}, osobní rok {personalYear}.");
    }

    public KondiciogramResult CalculateBiorhythm(
        DateOnly birthDate,
        DateOnly localDate)
    {
        var daysAlive =
            localDate.DayNumber - birthDate.DayNumber;

        double Cycle(double days) =>
            Math.Sin((2d * Math.PI * daysAlive) / days) * 100d;

        return new KondiciogramResult(
            Math.Round(Cycle(23d), 1),
            Math.Round(Cycle(28d), 1),
            Math.Round(Cycle(33d), 1));
    }

    private static int SumDigits(int number)
    {
        number = Math.Abs(number);
        var sum = 0;

        do
        {
            sum += number % 10;
            number /= 10;
        }
        while (number > 0);

        return sum;
    }

    private static int Reduce(int number)
    {
        while (number > 9 &&
               number is not 11 and not 22 and not 33)
        {
            number = SumDigits(number);
        }

        return number;
    }
}
