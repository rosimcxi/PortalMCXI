// Soubor: SpravaProjektuEndpoints.cs | Verze: 1.2.1 | Datum: 2026-05-19 | Autor: Roman Fišer
// Popis: Endpointy pro správu projektů.
// Historie: 2026-05-19 Vytvořeno pro PortalMCXI.

using System.Collections.Concurrent;
using PortalMCXIBackend.Models;
namespace PortalMCXIBackend.Endpoints;

public static class SpravaProjektuEndpoints {
    public static void MapSpravaProjektuEndpoints(this WebApplication app) {
        app.MapGroup("/api/projects-management");
    }
}