using System.Collections.Concurrent;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class SnippetsEndpoints
{
    private static readonly ConcurrentBag<CodeSnippet> Snippets =
        new();

    private static int _nextId = 1;

    public static void MapSnippetsEndpoints(
        this WebApplication app)
    {
        var group = app.MapGroup("/api/snippets");

        if (Snippets.IsEmpty)
        {
            Snippets.Add(
                new CodeSnippet(
                    _nextId++,
                    "Nginx Docker Reset",
                    "bash",
                    "docker compose down && docker compose up -d",
                    "Rychlý restart infrastruktury",
                    "Roman"));

            Snippets.Add(
                new CodeSnippet(
                    _nextId++,
                    "C# EF Core Migrace",
                    "csharp",
                    "dotnet ef migrations add Init",
                    "Přidání první databázové migrace",
                    "Roman"));
        }

        group.MapGet(
                "/",
                () => Results.Ok(
                    Snippets
                        .OrderByDescending(item => item.Id)
                        .ToList()))
            .WithName("GetAllSnippets");

        group.MapPost("/", (CreateSnippetRequest request) =>
        {
            var snippet = new CodeSnippet(
                _nextId++,
                request.Title,
                request.Language,
                request.Code,
                request.Description,
                "Roman");

            Snippets.Add(snippet);

            return Results.Created(
                $"/api/snippets/{snippet.Id}",
                snippet);
        })
        .WithName("CreateSnippet");
    }
}
