// =====================================================================// Soubor: backend/Endpoints/SystemEndpoints.cs// Popis: Zde jsou definovány základní routy pro tvůj Dashboard.// =====================================================================using Microsoft.AspNetCore.Http.HttpResults;using PortalMCXIBackend.Models;namespace PortalMCXIBackend.Endpoints;public static class SystemEndpoints{// Tato metoda "přilepí" (Map) naše adresy k aplikacipublic static void MapSystemEndpoints(this WebApplication app){// Vytvoříme si "skupinu", aby všechno začínalo na /api// a nemuseli jsme to psát všude znovu.var apiGroup = app.MapGroup("/api");    // GET /
    // Úvodní kontrolní stránka, zda kontejner vůbec běží.
    app.MapGet("/", () => "PortalMCXI API is running on .NET 10 (Contabo VPS)");

    // GET /system
    // Vrací základní informace o prostředí pro případný monitoring (Zabbix apod.)
    app.MapGet("/system", () => TypedResults.Ok(new SystemStatus(
        "Online", Environment.OSVersion.ToString(), Environment.Version.ToString(), DateTime.Now, Environment.MachineName
    ))).WithName("GetSystemStatus");

    // GET /api/stats
    // Vrací vymyšlená data pro vrchní karty v Dashboardu (později napojíme na reálné Linux metriky)
    apiGroup.MapGet("/stats", () => TypedResults.Ok(new DashboardStats(
        "99.98%", "14,502", "2.1 GB", 124
    ))).WithName("GetStats");

    // GET /api/projects
    // Zde řídíme tlačítka a odkazy pro levé menu a hlavní plochu Reactu.
    // Díky tomuto už nepotřebujeme data držet v main.jsx!
    apiGroup.MapGet("/projects", () => TypedResults.Ok(new[] {
        new ProjectItem(1, "Jóga s Miškou", "https://jogasmiskou.cz", "online", "Web / Rezervace"),
        new ProjectItem(2, "PortalMCXI API", "https://api.rosimcxi.eu/scalar/v1", "online", ".NET 10 API"),
        new ProjectItem(3, "Kondiciogramy", "https://wisdomes.eu", "online", "Aplikace"),
        new ProjectItem(4, "Osobní vizitka", "https://roman.rosimcxi.eu", "online", "Web")
    })).WithName("GetProjects");
}
}