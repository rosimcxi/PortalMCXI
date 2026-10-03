using PortalMCXIBackend.Services;

namespace PortalMCXIBackend.Endpoints;

public static class EsoterikaEndpoints
{
    public static void MapEsoterikaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/esoterika");

        group.MapGet("/tatvy", async (
            MorningInfoService morningInfo,
            CancellationToken cancellationToken) =>
        {
            var morning = await morningInfo.GetAsync(cancellationToken);

            if (morning.Personal?.Tatva is null)
            {
                return Results.Problem(
                    title: "Tatvy nejsou dostupné",
                    detail: string.Join("; ", morning.Errors),
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            return Results.Ok(morning.Personal.Tatva);
        })
        .WithName("GetTatvy");

        group.MapGet("/numerologie", (
            string birthDateStr,
            PersonalCalculationService calculations) =>
        {
            if (!DateOnly.TryParse(birthDateStr, out var birthDate))
            {
                return Results.BadRequest(
                    "Neplatný formát data. Použijte RRRR-MM-DD.");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            return Results.Ok(
                calculations.CalculateNumerology(
                    birthDate,
                    today));
        })
        .WithName("GetNumerologie");

        group.MapGet("/kondiciogram", (
            string birthDateStr,
            PersonalCalculationService calculations) =>
        {
            if (!DateOnly.TryParse(birthDateStr, out var birthDate))
            {
                return Results.BadRequest(
                    "Neplatný formát data. Použijte RRRR-MM-DD.");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            return Results.Ok(
                calculations.CalculateBiorhythm(
                    birthDate,
                    today));
        })
        .WithName("GetKondiciogram");
    }
}
