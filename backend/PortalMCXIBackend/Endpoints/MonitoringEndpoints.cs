using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class MonitoringEndpoints
{
    public static void MapMonitoringEndpoints(
        this WebApplication app)
    {
        var group = app.MapGroup("/api/monitoring");

        group.MapGet("/dashboard", () =>
        {
            var now = DateTime.Now;

            var services = new List<ServiceHealth>
            {
                new(
                    "PostgreSQL 16",
                    "Online",
                    12,
                    now,
                    "Database"),
                new(
                    "Nginx Proxy",
                    "Online",
                    4,
                    now,
                    "Proxy"),
                new(
                    "Redis Cache",
                    "Offline",
                    0,
                    now,
                    "Cache"),
                new(
                    "PortalMCXI API",
                    "Online",
                    1,
                    now,
                    "API")
            };

            var dbMetrics = new DbMetrics(
                "PortalMCXI_DB",
                24,
                18.5,
                2,
                "7 weeks");

            var history =
                new List<StatusHistoryEvent>
                {
                    new(
                        now.AddMinutes(-5),
                        "Redis Cache",
                        "Error",
                        "Ztráta spojení se serverem"),
                    new(
                        now.AddHours(-12),
                        "PostgreSQL 16",
                        "Warning",
                        "Zaznamenán pomalý dotaz (> 2000ms)"),
                    new(
                        now.AddDays(-1),
                        "Nginx Proxy",
                        "Info",
                        "SSL certifikát úspěšně obnoven"),
                    new(
                        now.AddDays(-2),
                        "PortalMCXI API",
                        "Info",
                        "Nasazena nová verze (v1.3)")
                };

            return Results.Ok(
                new MonitoringDashboard(
                    services,
                    dbMetrics,
                    history));
        })
        .WithName("GetMonitoringDashboard");
    }
}
