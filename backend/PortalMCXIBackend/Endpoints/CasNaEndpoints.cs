using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

// Vývojový in-memory modul. Každý proces má vlastní ukázková data.
public static class CasNaEndpoints
{
    public static void MapCasNaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/casna");
        var sync = new object();
        var projects = new List<CasNaProject>
        {
            new(1, "PortalMCXI - Architektura"), new(2, "Databáze - Vývoj")
        };
        var tasks = new List<CasNaTask>
        {
            new(1, 1, "Návrh .NET API"), new(2, 2, "Optimalizace SQL dotazů")
        };
        var history = new List<TimeEntry>();
        RunningTask? activeTask = null;

        group.MapGet("/dashboard", () =>
        {
            lock (sync)
                return Results.Ok(new CasNaDashboardResponse(projects, tasks, activeTask, history.ToList()));
        }).WithName("GetCasNaDashboard");

        group.MapPost("/start", (CasNaStartRequest request) =>
        {
            lock (sync)
            {
                var task = tasks.FirstOrDefault(t => t.Id == request.TaskId);
                if (task is null) return Results.NotFound("Úkol s tímto ID neexistuje.");
                if (activeTask is not null)
                    return Results.BadRequest("Již běží jiný úkol. Nejprve jej zastavte.");
                activeTask = new RunningTask(task.Id, task.ProjectId, task.Name, DateTime.UtcNow);
                return Results.Ok(new SimpleMessageResponse($"Odstartováno: {task.Name}", activeTask.StartTime));
            }
        }).WithName("StartCasNa");

        group.MapPost("/stop", (CasNaStopRequest request) =>
        {
            lock (sync)
            {
                if (activeTask is null) return Results.BadRequest("Žádný úkol aktuálně neběží.");
                var end = DateTime.UtcNow;
                history.Add(new TimeEntry(activeTask.TaskId, activeTask.ProjectId,
                    activeTask.StartTime, end, request.Note ?? ""));
                var name = activeTask.TaskName;
                activeTask = null;
                return Results.Ok(new SimpleMessageResponse($"Ukončeno: {name}", end));
            }
        }).WithName("StopCasNa");
    }
}
