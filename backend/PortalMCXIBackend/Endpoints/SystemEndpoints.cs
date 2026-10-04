using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class SystemEndpoints
{
    public static void MapSystemEndpoints(this WebApplication app)
    {
        var apiGroup = app.MapGroup("/api");

        app.MapGet("/", () =>
            "PortalMCXI API is running on .NET 10 (Contabo VPS)");

        app.MapGet("/system", () =>
            TypedResults.Ok(new SystemStatus(
                "Online",
                Environment.OSVersion.ToString(),
                Environment.Version.ToString(),
                DateTime.Now,
                Environment.MachineName)))
            .WithName("GetSystemStatus");

        apiGroup.MapGet("/stats", () =>
            TypedResults.Ok(new DashboardStats(
                "99.98%",
                "14,502",
                "2.1 GB",
                124)))
            .WithName("GetStats");

        apiGroup.MapGet("/projects", () =>
            TypedResults.Ok(new[]
            {
                new ProjectItem(
                    1,
                    "Jóga s Miškou",
                    "https://jogasmiskou.cz",
                    "online",
                    "Web / Rezervace"),
                new ProjectItem(
                    2,
                    "PortalMCXI API",
                    "https://api.rosimcxi.eu/scalar/v1",
                    "online",
                    ".NET 10 API"),
                new ProjectItem(
                    3,
                    "Kondiciogramy",
                    "https://wisdomes.eu",
                    "online",
                    "Aplikace"),
                new ProjectItem(
                    4,
                    "Osobní vizitka",
                    "https://roman.rosimcxi.eu",
                    "online",
                    "Web")
            }))
            .WithName("GetProjects");
    }
}
