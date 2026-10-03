// Soubor: AppModels.cs | Verze: 1.5.0 | Datum: 2026-05-19 | Autor: Roman Fišer
// Popis: Datové modely pro celý systém.
// Historie: 2026-05-19 Kompletní verze.

using System.Text.Json.Serialization;
namespace PortalMCXIBackend.Models;

public record SystemStatus(string Status, string OS, string DotnetVersion, DateTime ServerTime, string MachineName);
public record ProjectDetail(int Id, string Name, string Description, string Status, DateTime? Deadline, string? Url);
public record TatvaInfo(string AktualniTatva, string Element, string Barva, string DalsiZmena);
public record MonitoringDashboard(List<ServiceHealth> Services, DbMetrics DbStatus, List<StatusHistoryEvent> History);

internal partial class AppJsonSerializerContext : JsonSerializerContext { }