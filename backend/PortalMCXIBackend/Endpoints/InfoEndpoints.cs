using PortalMCXIBackend.Services;

namespace PortalMCXIBackend.Endpoints;

public static class InfoEndpoints
{
    public static void MapInfoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/info");

        group.MapGet("/morning", async (
            MorningInfoService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetAsync(cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetMorningInfo");
    }
}
