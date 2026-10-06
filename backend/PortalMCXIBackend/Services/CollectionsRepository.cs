// =====================================================================
// Soubor: CollectionsRepository.cs
// Projekt: PortalMCXIBackend
// Verze: 1.0.0
// Datum: 2026-10-06
// Ucel: Read/write pristup modulu Sbirky do PostgreSQL bez ORM.
// =====================================================================

using Npgsql;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Services;

public sealed class CollectionsRepository(NpgsqlDataSource dataSource)
{
    public async Task<CollectionsSummary> GetSummaryAsync(CancellationToken ct)
    {
        const string sql = """
            SELECT
                count(*)::int AS item_rows,
                coalesce(sum(quantity), 0)::int AS pieces,
                coalesce(sum(purchase_price * quantity), 0)::numeric AS purchase_total,
                coalesce(sum(market_estimate * quantity), 0)::numeric AS market_total
            FROM collections.v_item_current_value;
            """;

        await using var cmd = dataSource.CreateCommand(sql);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);

        return new CollectionsSummary(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetDecimal(2),
            reader.GetDecimal(3),
            "CZK");
    }

    public async Task<List<CollectionItemDto>> GetItemsAsync(CancellationToken ct)
    {
        const string sql = """
            SELECT
                item_id, item_type, name, country, year, denomination, quantity,
                metal, fineness, weight_g, purchase_price, purchase_currency,
                valued_at, metal_value, market_min, market_estimate, market_max,
                valuation_currency, confidence
            FROM collections.v_item_current_value
            ORDER BY coalesce(market_estimate, purchase_price, 0) * quantity DESC,
                     name, year;
            """;

        var result = new List<CollectionItemDto>();
        await using var cmd = dataSource.CreateCommand(sql);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            result.Add(new CollectionItemDto(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetInt32(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.GetInt32(6),
                reader.IsDBNull(7) ? null : reader.GetString(7),
                reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                reader.IsDBNull(9) ? null : reader.GetDecimal(9),
                reader.IsDBNull(10) ? null : reader.GetDecimal(10),
                reader.GetString(11),
                reader.IsDBNull(12) ? null : reader.GetFieldValue<DateTimeOffset>(12),
                reader.IsDBNull(13) ? null : reader.GetDecimal(13),
                reader.IsDBNull(14) ? null : reader.GetDecimal(14),
                reader.IsDBNull(15) ? null : reader.GetDecimal(15),
                reader.IsDBNull(16) ? null : reader.GetDecimal(16),
                reader.IsDBNull(17) ? null : reader.GetString(17),
                reader.IsDBNull(18) ? null : reader.GetDecimal(18)));
        }

        return result;
    }

    public async Task<CollectionImportResult> ImportAsync(CollectionImportRequest request, CancellationToken ct)
    {
        var inserted = 0;
        var updated = 0;

        await using var connection = await dataSource.OpenConnectionAsync(ct);
        await using var transaction = await connection.BeginTransactionAsync(ct);

        foreach (var item in request.Items)
        {
            const string sql = """
                INSERT INTO collections.item (
                    item_id, item_type, name, country, year, denomination, quantity,
                    condition, catalog_number, metal, fineness, weight_g, diameter_mm,
                    purchase_price, purchase_currency, purchase_date, acquisition_source,
                    storage_location, notes, identity_confidence, identity_status
                )
                VALUES (
                    @item_id, @item_type, @name, @country, @year, @denomination, @quantity,
                    @condition, @catalog_number, @metal, @fineness, @weight_g, @diameter_mm,
                    @purchase_price, @purchase_currency, @purchase_date, @acquisition_source,
                    @storage_location, @notes, @identity_confidence, @identity_status
                )
                ON CONFLICT (item_id) DO UPDATE SET
                    item_type = excluded.item_type,
                    name = excluded.name,
                    country = excluded.country,
                    year = excluded.year,
                    denomination = excluded.denomination,
                    quantity = excluded.quantity,
                    condition = excluded.condition,
                    catalog_number = excluded.catalog_number,
                    metal = excluded.metal,
                    fineness = excluded.fineness,
                    weight_g = excluded.weight_g,
                    diameter_mm = excluded.diameter_mm,
                    purchase_price = excluded.purchase_price,
                    purchase_currency = excluded.purchase_currency,
                    purchase_date = excluded.purchase_date,
                    acquisition_source = excluded.acquisition_source,
                    storage_location = excluded.storage_location,
                    notes = excluded.notes,
                    identity_confidence = excluded.identity_confidence,
                    identity_status = excluded.identity_status,
                    updated_at = now()
                RETURNING (xmax = 0) AS inserted;
                """;

            await using var cmd = new NpgsqlCommand(sql, connection, transaction);
            cmd.Parameters.AddWithValue("item_id", item.ItemId);
            cmd.Parameters.AddWithValue("item_type", item.ItemType);
            cmd.Parameters.AddWithValue("name", item.Name);
            cmd.Parameters.AddWithValue("country", (object?)item.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("year", (object?)item.Year ?? DBNull.Value);
            cmd.Parameters.AddWithValue("denomination", (object?)item.Denomination ?? DBNull.Value);
            cmd.Parameters.AddWithValue("quantity", item.Quantity);
            cmd.Parameters.AddWithValue("condition", (object?)item.Condition ?? DBNull.Value);
            cmd.Parameters.AddWithValue("catalog_number", (object?)item.CatalogNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("metal", (object?)item.Metal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("fineness", (object?)item.Fineness ?? DBNull.Value);
            cmd.Parameters.AddWithValue("weight_g", (object?)item.WeightG ?? DBNull.Value);
            cmd.Parameters.AddWithValue("diameter_mm", (object?)item.DiameterMm ?? DBNull.Value);
            cmd.Parameters.AddWithValue("purchase_price", (object?)item.PurchasePrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("purchase_currency", item.PurchaseCurrency);
            cmd.Parameters.AddWithValue("purchase_date", (object?)item.PurchaseDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("acquisition_source", (object?)item.AcquisitionSource ?? DBNull.Value);
            cmd.Parameters.AddWithValue("storage_location", (object?)item.StorageLocation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("notes", (object?)item.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("identity_confidence", (object?)item.IdentityConfidence ?? DBNull.Value);
            cmd.Parameters.AddWithValue("identity_status", item.IdentityStatus);

            var isInserted = (bool)(await cmd.ExecuteScalarAsync(ct) ?? false);
            if (isInserted) inserted++; else updated++;
        }

        await transaction.CommitAsync(ct);
        return new CollectionImportResult(request.Items.Count, inserted, updated);
    }
}
