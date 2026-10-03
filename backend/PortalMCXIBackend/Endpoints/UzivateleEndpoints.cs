using System.Collections.Concurrent;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class UzivateleEndpoints
{
    private static readonly ConcurrentBag<UserAccount> Users =
        new();

    public static void MapUzivateleEndpoints(
        this WebApplication app)
    {
        var group = app.MapGroup("/api/users");

        if (Users.IsEmpty)
        {
            Users.Add(
                new UserAccount(
                    1,
                    "roman_admin",
                    "Admin",
                    true));

            Users.Add(
                new UserAccount(
                    2,
                    "guest_viewer",
                    "User",
                    true));
        }

        group.MapGet(
                "/",
                () => Results.Ok(Users.ToList()))
            .WithName("GetUsers");

        group.MapPost("/login", (LoginRequest request) =>
        {
            var user = Users.FirstOrDefault(
                item =>
                    item.Username == request.Username);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            var fakeToken =
                $"eyJhbGciOiJIUzI1Ni...{user.Id}";

            return Results.Ok(
                new AuthResponse(
                    fakeToken,
                    user));
        })
        .WithName("LoginUser");
    }
}
