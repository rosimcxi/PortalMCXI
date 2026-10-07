// PortalMCXI – obnovená deklarace z importovaného zdroje.
using Microsoft.AspNetCore.Http.HttpResults;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class SystemEndpoints
{
public static void MapSystemEndpoints(this WebApplication app)
{
    var apiGroup = app.MapGroup("/api");
    // Úvodní kontrolní stránka, zda kontejner vůbec běží.
    app.MapGet("/", () => "PortalMCXI API is running on .NET 10");

    // GET /system
    // Vrací základní informace o prostředí pro případný monitoring (Zabbix apod.)
    app.MapGet("/system", () => TypedResults.Ok(new SystemStatus(
        "Online", Environment.OSVersion.ToString(), Environment.Version.ToString(), DateTime.Now, Environment.MachineName
    ))).WithName("GetSystemStatus");

    // GET /api/stats
    // Vrací vymyšlená data pro vrchní karty v Dashboardu (později napojíme na reálné Linux metriky)
    apiGroup.MapGet("/stats", () => TypedResults.Ok(new DashboardStats(
        "UNKNOWN", "UNKNOWN", "UNKNOWN", 0, true
    ))).WithName("GetStats");

    // GET /api/projects
    // Zde řídíme tlačítka a odkazy pro levé menu a hlavní plochu Reactu.
    // Díky tomuto už nepotřebujeme data držet v main.jsx!
    apiGroup.MapGet("/projects", () => TypedResults.Ok(new[] {
        new ProjectItem(1, "Jóga s Miškou", "https://jogasmiskou.cz", "UNKNOWN", "Web / Rezervace"),
        new ProjectItem(2, "PortalMCXI API", "https://api.rosimcxi.eu/scalar/v1", "UNKNOWN", ".NET 10 API"),
        new ProjectItem(3, "Kondiciogramy", "https://wisdomes.eu", "UNKNOWN", "Aplikace"),
        new ProjectItem(4, "Osobní vizitka", "https://roman.rosimcxi.eu", "UNKNOWN", "Web")
    })).WithName("GetProjects");
}
}
