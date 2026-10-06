// =====================================================================
// Soubor: CollectionsEndpoints.cs
// Projekt: PortalMCXIBackend
// Verze: 1.0.0
// Datum: 2026-10-06
// Ucel: REST API pro katalog, souhrn a privátní import modulu Sbirky.
// =====================================================================

using PortalMCXIBackend.Models;
using PortalMCXIBackend.Services;

namespace PortalMCXIBackend.Endpoints;

public static class CollectionsEndpoints
{
    public static void MapCollectionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/collections");

        group.MapGet("/summary", async (CollectionsRepository repository, CancellationToken ct) =>
            TypedResults.Ok(await repository.GetSummaryAsync(ct)))
            .WithName("GetCollectionsSummary");

        group.MapGet("/items", async (CollectionsRepository repository, CancellationToken ct) =>
            TypedResults.Ok(await repository.GetItemsAsync(ct)))
            .WithName("GetCollectionItems");

        group.MapPost("/import", async (
            CollectionImportRequest request,
            CollectionsRepository repository,
            CancellationToken ct) =>
        {
            if (request.Items.Count == 0)
                return Results.BadRequest(new SimpleMessageResponse("Import neobsahuje zadne polozky."));

            if (request.Items.Any(x =>
                x.ItemId == Guid.Empty ||
                string.IsNullOrWhiteSpace(x.ItemType) ||
                string.IsNullOrWhiteSpace(x.Name) ||
                x.Quantity <= 0 ||
                x.PurchaseCurrency.Length != 3))
            {
                return Results.BadRequest(new SimpleMessageResponse("Import obsahuje neplatnou polozku."));
            }

            var result = await repository.ImportAsync(request, ct);
            return Results.Ok(result);
        })
        .WithName("ImportCollectionItems");
    }
}
