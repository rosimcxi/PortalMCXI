// =====================================================================// Soubor: backend/Endpoints/UzivateleEndpoints.cs// Projekt: PortalMCXIBackend// Verze: v1.0// Autor: Ing. Roman Fišer// Popis: Základní modul pro správu uživatelů, autentizaci a role.//        Zatím In-Memory mockování pro budoucí napojení na JWT a DB.// Změny:// - v1.0 (17.05.2026): Vytvoření základních endpointů.// =====================================================================using System.Collections.Concurrent;using PortalMCXIBackend.Models;namespace PortalMCXIBackend.Endpoints;public static class UzivateleEndpoints{private static readonly ConcurrentBag _users = new();public static void MapUzivateleEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/users");

    if (_users.IsEmpty)
    {
        _users.Add(new UserAccount(1, "roman_admin", "Admin", true));
        _users.Add(new UserAccount(2, "guest_viewer", "User", true));
    }

    // Seznam uživatelů (Později chráněno pouze pro Adminy)
    group.MapGet("/", () => Results.Ok(_users.ToList())).WithName("GetUsers");

    // Mock Login (Příprava pro generování JWT Tokenu)
    group.MapPost("/login", (LoginRequest req) => 
    {
        // V produkci zde bude hashování hesel v Postgresu!
        var user = _users.FirstOrDefault(u => u.Username == req.Username);
        if (user == null) return Results.Unauthorized();

        // Zástupný token
        string fakeToken = $"eyJhbGciOiJIUzI1Ni...{user.Id}"; 
        
        return Results.Ok(new AuthResponse(fakeToken, user));
    }).WithName("LoginUser");
}
}