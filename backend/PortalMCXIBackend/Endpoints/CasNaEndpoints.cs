using System.Collections.Concurrent;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class CasNaEndpoints
{
    private static readonly ConcurrentDictionary<int, string> Projects =
        new();

    private static readonly ConcurrentDictionary<
        int,
        (int ProjectId, string Name)> Tasks =
        new();

    private static readonly ConcurrentBag<TimeEntry> TimeEntries =
        new();

    private static RunningTask? _currentRunningTask;
    private static int _nextProjectId = 1;
    private static int _nextTaskId = 1;

    public static void MapCasNaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/casna");

        EnsureSeedData();

        group.MapGet("/dashboard", () =>
        {
            var projects = Projects
                .Select(item =>
                    new CasNaProject(
                        item.Key,
                        item.Value))
                .OrderBy(item => item.Id)
                .ToList();

            var tasks = Tasks
                .Select(item =>
                    new CasNaTask(
                        item.Key,
                        item.Value.ProjectId,
                        item.Value.Name))
                .OrderBy(item => item.Id)
                .ToList();

            return Results.Ok(
                new CasNaDashboardResponse(
                    projects,
                    tasks,
                    _currentRunningTask));
        })
        .WithName("GetCasNaDashboard");

        group.MapPost("/start", (CasNaStartRequest request) =>
        {
            if (!Tasks.TryGetValue(
                    request.TaskId,
                    out var task))
            {
                return Results.NotFound(
                    "Úkol s tímto ID neexistuje.");
            }

            if (_currentRunningTask is not null)
            {
                return Results.BadRequest(
                    "Již běží jiný úkol. " +
                    "Nejprve jej zastavte.");
            }

            _currentRunningTask = new RunningTask(
                request.TaskId,
                task.Name,
                DateTime.Now);

            return Results.Ok(
                new SimpleMessageResponse(
                    $"Odstartováno: " +
                    $"{_currentRunningTask.TaskName}",
                    DateTime.Now));
        })
        .WithName("StartCasNa");

        group.MapPost("/stop", (CasNaStopRequest request) =>
        {
            if (_currentRunningTask is null)
            {
                return Results.BadRequest(
                    "Žádný úkol aktuálně neběží. " +
                    "Není co zastavovat.");
            }

            var runningTask = _currentRunningTask;
            var endTime = DateTime.Now;
            var duration =
                endTime - runningTask.StartTime;

            TimeEntries.Add(
                new TimeEntry(
                    runningTask.TaskId,
                    runningTask.StartTime,
                    endTime,
                    request.Note));

            _currentRunningTask = null;

            return Results.Ok(
                new SimpleMessageResponse(
                    $"Ukončeno: {runningTask.TaskName}. " +
                    $"Zaznamenáno " +
                    $"{(int)duration.TotalMinutes} minut.",
                    endTime));
        })
        .WithName("StopCasNa");
    }

    private static void EnsureSeedData()
    {
        if (!Projects.IsEmpty)
        {
            return;
        }

        var portalProjectId =
            Interlocked.Increment(ref _nextProjectId) - 1;

        Projects.TryAdd(
            portalProjectId,
            "PortalMCXI - Architektura");

        Tasks.TryAdd(
            Interlocked.Increment(ref _nextTaskId) - 1,
            (portalProjectId, "Návrh .NET API"));

        var styraxProjectId =
            Interlocked.Increment(ref _nextProjectId) - 1;

        Projects.TryAdd(
            styraxProjectId,
            "Styrax - Databáze");

        Tasks.TryAdd(
            Interlocked.Increment(ref _nextTaskId) - 1,
            (styraxProjectId, "Optimalizace SQL dotazů"));
    }
}
