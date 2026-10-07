// PortalMCXI – obnovená deklarace z importovaného zdroje.
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class MonitoringEndpoints
{
public static void MapMonitoringEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/monitoring");
    group.MapGet("/dashboard", () => {
        var now = DateTime.Now;

        // 1. Zástupná data pro služby (Zde se v budoucnu napíše reálný Ping)
        var services = new List<ServiceHealth> {
            new ServiceHealth("PostgreSQL 16", "Online", 12, now, "Database"),
            new ServiceHealth("Nginx Proxy", "Online", 4, now, "Proxy"),
            new ServiceHealth("Redis Cache", "Offline", 0, now, "Cache"),
            new ServiceHealth("PortalMCXI API", "Online", 1, now, "API")
        };

        // 2. Metriky databáze
        var dbMetrics = new DbMetrics("PortalMCXI_DB", 24, 18.5, 2, "7 weeks");

        // 3. Historie událostí
        var history = new List<StatusHistoryEvent> {
            new StatusHistoryEvent(now.AddMinutes(-5), "Redis Cache", "Error", "Ztráta spojení se serverem"),
            new StatusHistoryEvent(now.AddHours(-12), "PostgreSQL 16", "Warning", "Zaznamenán pomalý dotaz (> 2000ms)"),
            new StatusHistoryEvent(now.AddDays(-1), "Nginx Proxy", "Info", "SSL certifikát úspěšně obnoven"),
            new StatusHistoryEvent(now.AddDays(-2), "PortalMCXI API", "Info", "Nasazena nová verze (v1.3)")
        };

        return Results.Ok(new MonitoringDashboard(services, dbMetrics, history));
    }).WithName("GetMonitoringDashboard");
}
}
