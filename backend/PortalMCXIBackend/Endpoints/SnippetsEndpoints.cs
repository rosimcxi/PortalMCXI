// =====================================================================// Soubor: backend/Endpoints/SnippetsEndpoints.cs// Projekt: PortalMCXIBackend// Verze: v1.0// Autor: Ing. Roman Fišer// Popis: Modul pro evidenci zdrojových kódů (Snippets), návodů a//        rychlých referencí pro programátory.// Změny:// - v1.0 (17.05.2026): Vytvoření modulu s In-Memory úložištěm.// =====================================================================using System.Collections.Concurrent;using PortalMCXIBackend.Models;namespace PortalMCXIBackend.Endpoints;public static class SnippetsEndpoints{private static readonly ConcurrentBag _snippets = new();private static int _nextId = 1;public static void MapSnippetsEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/snippets");

    if (_snippets.IsEmpty)
    {
        _snippets.Add(new CodeSnippet(_nextId++, "Nginx Docker Reset", "bash", "docker compose down && docker compose up -d", "Rychlý restart infrastruktury", "Roman"));
        _snippets.Add(new CodeSnippet(_nextId++, "C# EF Core Migrace", "csharp", "dotnet ef migrations add Init", "Přidání první databázové migrace", "Roman"));
    }

    group.MapGet("/", () => Results.Ok(_snippets.OrderByDescending(s => s.Id).ToList())).WithName("GetAllSnippets");

    group.MapPost("/", (CreateSnippetRequest req) => 
    {
        var snip = new CodeSnippet(_nextId++, req.Title, req.Language, req.Code, req.Description, "Roman");
        _snippets.Add(snip);
        return Results.Created($"/api/snippets/{snip.Id}", snip);
    }).WithName("CreateSnippet");
}
}