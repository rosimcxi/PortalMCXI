// PortalMCXI – obnovená deklarace z importovaného zdroje.
using System.Collections.Concurrent;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class UzivateleEndpoints
{
private static readonly ConcurrentBag<UserAccount> _users = new();
public static void MapUzivateleEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/users");

    if (_users.IsEmpty)
    {
        _users.Add(new UserAccount(1, "roman_admin", "Admin", true));
        _users.Add(new UserAccount(2, "guest_viewer", "User", true));
    }

    // Seznam uživatelů (Později chráněno pouze pro Adminy)
    group.MapGet("/", () => Results.Ok(_users.ToList())).WithName("GetUsers");

    // Autentizace není implementovaná; nikdy nevydávat falešný token.
    group.MapPost("/login", () => Results.StatusCode(StatusCodes.Status501NotImplemented))
        .WithName("LoginUser");
}
}
