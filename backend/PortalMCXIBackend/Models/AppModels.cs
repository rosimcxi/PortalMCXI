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
public record CasNaProject(int Id, string Name);
public record CasNaTask(int Id, int ProjectId, string Name);
public record RunningTask(int TaskId, string TaskName, DateTime StartTime);
public record TimeEntry(int TaskId, DateTime Start, DateTime End, string Note);
public record CasNaStartRequest(int TaskId);
public record CasNaStopRequest(string Note);
public record CasNaDashboardResponse(List<CasNaProject> Projects, List<CasNaTask> Tasks, RunningTask? ActiveTask);
public record ServiceHealth(string Name, string Status, int PingMs, DateTime LastCheck, string Type);
public record DbMetrics(string DbName, int ActiveConnections, double CpuUsage, int SlowQueries, string Uptime);
public record StatusHistoryEvent(DateTime Timestamp, string ServiceName, string EventType, string Details);
public record MonitoringDashboard(List<ServiceHealth> Services, DbMetrics DbStatus, List<StatusHistoryEvent> History);

public record MorningLocation(
    string Name,
    double? Latitude,
    double? Longitude,
    string Timezone,
    string Status);

public record WeatherSnapshot(
    string Source,
    string? ObservedAt,
    double TemperatureC,
    double ApparentTemperatureC,
    double PrecipitationMm,
    int WeatherCode,
    double CloudCoverPercent,
    double PressureMslHpa,
    double WindSpeedKmh,
    double WindGustKmh,
    string Status);

public record SunSnapshot(
    string Source,
    string? Date,
    string? Sunrise,
    string? Sunset,
    double DaylightMinutes,
    string Status);

public record MorningInfoResponse(
    DateTimeOffset GeneratedAtUtc,
    MorningLocation Location,
    WeatherSnapshot? Weather,
    SunSnapshot? Sun,
    string OverallStatus,
    string[] Errors);

[JsonSerializable(typeof(SystemStatus))]
[JsonSerializable(typeof(DashboardStats))]
[JsonSerializable(typeof(ProjectItem))]
[JsonSerializable(typeof(ProjectItem[]))]
[JsonSerializable(typeof(SimpleMessageResponse))]
[JsonSerializable(typeof(UserAccount))]
[JsonSerializable(typeof(List<UserAccount>))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(AuthResponse))]
[JsonSerializable(typeof(CodeSnippet))]
[JsonSerializable(typeof(List<CodeSnippet>))]
[JsonSerializable(typeof(CreateSnippetRequest))]
[JsonSerializable(typeof(TatvaInfo))]
[JsonSerializable(typeof(NumerologieResult))]
[JsonSerializable(typeof(KondiciogramResult))]
[JsonSerializable(typeof(ProjectDetail))]
[JsonSerializable(typeof(List<ProjectDetail>))]
[JsonSerializable(typeof(CreateProjectRequest))]
[JsonSerializable(typeof(TodoItem))]
[JsonSerializable(typeof(List<TodoItem>))]
[JsonSerializable(typeof(CreateTodoRequest))]
[JsonSerializable(typeof(CasNaProject))]
[JsonSerializable(typeof(List<CasNaProject>))]
[JsonSerializable(typeof(CasNaTask))]
[JsonSerializable(typeof(List<CasNaTask>))]
[JsonSerializable(typeof(RunningTask))]
[JsonSerializable(typeof(TimeEntry))]
[JsonSerializable(typeof(List<TimeEntry>))]
[JsonSerializable(typeof(CasNaStartRequest))]
[JsonSerializable(typeof(CasNaStopRequest))]
[JsonSerializable(typeof(CasNaDashboardResponse))]
[JsonSerializable(typeof(ServiceHealth))]
[JsonSerializable(typeof(List<ServiceHealth>))]
[JsonSerializable(typeof(DbMetrics))]
[JsonSerializable(typeof(StatusHistoryEvent))]
[JsonSerializable(typeof(List<StatusHistoryEvent>))]
[JsonSerializable(typeof(MonitoringDashboard))]
[JsonSerializable(typeof(MorningLocation))]
[JsonSerializable(typeof(WeatherSnapshot))]
[JsonSerializable(typeof(SunSnapshot))]
[JsonSerializable(typeof(MorningInfoResponse))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(string[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
