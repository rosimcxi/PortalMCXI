// =====================================================================
// Soubor: backend/Endpoints/SpravaProjektuEndpoints.cs
// Projekt: PortalMCXIBackend
// Verze: v1.2
// Autor: Ing. Roman Fišer
// Popis: Modul pro ucelenou správu projektů. Eviduje detailní informace.
// =====================================================================

using System.Collections.Concurrent;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class SpravaProjektuEndpoints
{
    private static readonly ConcurrentBag<ProjectDetail> _projects = new();
    private static int _nextProjectId = 1;

    public static void MapSpravaProjektuEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects-management");

        if (_projects.IsEmpty)
        {
            _projects.Add(new ProjectDetail(_nextProjectId++, "PortalMCXI", "Komplexní dashboard a API server na VPS. Hlavní řídící centrum mých aplikací a modulů.", "V řešení", new DateTime(2026, 6, 30), "[https://rosimcxi.eu](https://rosimcxi.eu)"));
            _projects.Add(new ProjectDetail(_nextProjectId++, "Lokální DB Asistent", "Nástroj pro generování AI SQL dotazů (Python/C#)", "Plánováno", null, null));
            _projects.Add(new ProjectDetail(_nextProjectId++, "Jóga s Miškou", "E-shop a rezervační systém web", "Dokončeno", new DateTime(2025, 12, 1), "[https://jogasmiskou.cz](https://jogasmiskou.cz)"));
            _projects.Add(new ProjectDetail(_nextProjectId++, "Esoterické nástroje", "Moduly pro Tatvy, numerologii, kondiciogramy (biorytmy) a generování Nostradamových predikcí.", "V provozu", null, "[https://rosimcxi.eu/#esoterika](https://rosimcxi.eu/#esoterika)"));
            _projects.Add(new ProjectDetail(_nextProjectId++, "Nástroje pro převod dat", "Univerzální konvertory (XML, CSV)", "Ideace", null, null));
            _projects.Add(new ProjectDetail(_nextProjectId++, "Párty hry a výuka", "Interaktivní hry pro více hráčů a konfigurovatelné testy.", "Plánováno", null, null));
        }

        group.MapGet("/", () => 
        {
            var sortedProjects = _projects.OrderBy(p => p.Status == "Dokončeno").ThenBy(p => p.Id).ToList();
            return Results.Ok(sortedProjects);
        }).WithName("GetAllProjects");

        group.MapPost("/", (CreateProjectRequest request) => 
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Results.BadRequest("Název projektu je povinný.");

            var newProject = new ProjectDetail(
                _nextProjectId++, 
                request.Name, 
                request.Description ?? "", 
                "Plánováno", 
                request.Deadline,
                request.Url
            );
            
            _projects.Add(newProject);
            
            return Results.Created($"/api/projects-management/{newProject.Id}", newProject);
        }).WithName("CreateProject");
    }
}