// =====================================================================
// Soubor: backend/Models/AppModels.cs
// Projekt: PortalMCXIBackend
// Verze: v1.5
// Autor: Ing. Roman Fišer
// Popis: Datové struktury pro API a Native AOT kompilaci.
// =====================================================================

using System.Text.Json.Serialization;

namespace PortalMCXIBackend.Models;

public record SystemStatus(string Status, string OS, string DotnetVersion, DateTime ServerTime, string MachineName);
public record DashboardStats(string Uptime, string Requests, string Ram, int Users);
public record ProjectItem(int Id, string Name, string Url, string Status, string Type);
public record SimpleMessageResponse(string Message, DateTime? Timestamp = null);

public record UserAccount(int Id, string Username, string Role, bool IsActive);
public record LoginRequest(string Username, string Password);
public record AuthResponse(string Token, UserAccount User);

public record CodeSnippet(int Id, string Title, string Language, string Code, string Description, string Author);
public record CreateSnippetRequest(string Title, string Language, string Code, string Description);

public record TatvaInfo(string AktualniTatva, string Element, string Barva, string DalsiZmena);
public record NumerologieResult(int LifeNumber, int PersonalYear, string Description);
public record KondiciogramResult(double Physical, double Emotional, double Intellectual);

public record ProjectDetail(int Id, string Name, string Description, string Status, DateTime? Deadline, string? Url);
public record CreateProjectRequest(string Name, string Description, DateTime? Deadline, string? Url);

public record TodoItem(int Id, string Title, bool IsCompleted, string Category, DateTime? DueDate);
public record CreateTodoRequest(string Title, string Category, DateTime? DueDate);

public record RunningTask(int TodoId, int ProjectId, string TaskName, DateTime StartTime);
public record TimeEntry(int TodoId, int ProjectId, DateTime Start, DateTime End, string Note);
public record CasNaStartRequest(int TodoId, int ProjectId, string TaskName);
public record CasNaStopRequest(string Note);
public record CasNaDashboardResponse(RunningTask? ActiveTask, List History);

public record ServiceHealth(string Name, string Status, int PingMs, DateTime LastCheck, string Type);
public record DbMetrics(string DbName, int ActiveConnections, double CpuUsage, int SlowQueries, string Uptime);
public record StatusHistoryEvent(DateTime Timestamp, string ServiceName, string EventType, string Details);
public record MonitoringDashboard(List Services, DbMetrics DbStatus, List History);

[JsonSerializable(typeof(SystemStatus))]
[JsonSerializable(typeof(DashboardStats))]
[JsonSerializable(typeof(ProjectItem))]
[JsonSerializable(typeof(ProjectItem[]))]
[JsonSerializable(typeof(SimpleMessageResponse))]
[JsonSerializable(typeof(UserAccount))]
[JsonSerializable(typeof(List))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(AuthResponse))]
[JsonSerializable(typeof(CodeSnippet))]
[JsonSerializable(typeof(List))]
[JsonSerializable(typeof(CreateSnippetRequest))]
[JsonSerializable(typeof(TatvaInfo))]
[JsonSerializable(typeof(NumerologieResult))]
[JsonSerializable(typeof(KondiciogramResult))]
[JsonSerializable(typeof(ProjectDetail))]
[JsonSerializable(typeof(List))]
[JsonSerializable(typeof(CreateProjectRequest))]
[JsonSerializable(typeof(TodoItem))]
[JsonSerializable(typeof(List))]
[JsonSerializable(typeof(CreateTodoRequest))]
[JsonSerializable(typeof(RunningTask))]
[JsonSerializable(typeof(TimeEntry))]
[JsonSerializable(typeof(List))]
[JsonSerializable(typeof(CasNaStartRequest))]
[JsonSerializable(typeof(CasNaStopRequest))]
[JsonSerializable(typeof(CasNaDashboardResponse))]
[JsonSerializable(typeof(ServiceHealth))]
[JsonSerializable(typeof(List))]
[JsonSerializable(typeof(DbMetrics))]
[JsonSerializable(typeof(StatusHistoryEvent))]
[JsonSerializable(typeof(List))]
[JsonSerializable(typeof(MonitoringDashboard))]
[JsonSerializable(typeof(string))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }