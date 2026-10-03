// =====================================================================// Soubor: backend/Endpoints/CasNaEndpoints.cs// Popis: Modul pro sledování času úkolů. Zatím používá paměť serveru//        (ConcurrentDictionary), dokud nepřipojíme PostgreSQL a//        Entity Framework.// =====================================================================using System.Collections.Concurrent;using PortalMCXIBackend.Models;namespace PortalMCXIBackend.Endpoints;public static class CasNaEndpoints{// -----------------------------------------------------------------// IN-MEMORY DATABÁZE (Stav držíme přímo v RAM serveru)// Používáme "Concurrent", protože u webových API může chodit// více požadavků současně v různých vláknech (Thread Safety).// -----------------------------------------------------------------private static readonly ConcurrentDictionary<int, string> _projects = new();private static readonly ConcurrentDictionary<int, (int ProjectId, string Name)> _tasks = new();private static readonly ConcurrentBag _timeEntries = new();// Globální proměnná pro aktuálně běžící úkol (stopky)
private static RunningTask? _currentRunningTask = null;

// Zámek (lock) pro bezpečné přidávání IDček
private static int _nextProjectId = 1;
private static int _nextTaskId = 1;

public static void MapCasNaEndpoints(this WebApplication app)
{
    // Skupina URL začínající na /api/casna
    var group = app.MapGroup("/api/casna");

    // Seed: Pokud je slovník prázdný, vytvoříme si ukázková data
    if (_projects.IsEmpty)
    {
        _projects.TryAdd(_nextProjectId, "PortalMCXI - Architektura");
        _tasks.TryAdd(_nextTaskId++, (_nextProjectId++, "Návrh .NET API"));
        
        _projects.TryAdd(_nextProjectId, "Styrax - Databáze");
        _tasks.TryAdd(_nextTaskId++, (_nextProjectId, "Optimalizace SQL dotazů"));
    }

    // -------------------------------------------------------------
    // ENDPOINT: Natažení dat do Dashboardu
    // Zobrazí všechny projekty, úkoly a řekne nám, jestli nějaký běží.
    // -------------------------------------------------------------
    group.MapGet("/dashboard", () => {
        var prjs = _projects.Select(p => new CasNaProject(p.Key, p.Value)).ToList();
        var tsks = _tasks.Select(t => new CasNaTask(t.Key, t.Value.ProjectId, t.Value.Name)).ToList();
        
        return Results.Ok(new CasNaDashboardResponse(prjs, tsks, _currentRunningTask));
    }).WithName("GetCasNaDashboard");

    // -------------------------------------------------------------
    // ENDPOINT: Start (Spuštění stopek)
    // Očekává JSON s TaskId.
    // -------------------------------------------------------------
    group.MapPost("/start", (CasNaStartRequest request) => {
        if (!_tasks.ContainsKey(request.TaskId)) 
            return Results.NotFound("Úkol s tímto ID neexistuje.");
            
        if (_currentRunningTask != null) 
            return Results.BadRequest("Již běží jiný úkol. Nejprve jej zastavte.");

        // Uložíme do paměti, jaký úkol a v kolik hodin jsme spustili
        _currentRunningTask = new RunningTask(request.TaskId, _tasks[request.TaskId].Name, DateTime.Now);
        
        return Results.Ok(new SimpleMessageResponse($"Odstartováno: {_currentRunningTask.TaskName}", DateTime.Now));
    }).WithName("StartCasNa");

    // -------------------------------------------------------------
    // ENDPOINT: Stop (Zastavení stopek)
    // Očekává JSON s poznámkou, co se udělalo.
    // -------------------------------------------------------------
    group.MapPost("/stop", (CasNaStopRequest request) => {
        if (_currentRunningTask == null) 
            return Results.BadRequest("Žádný úkol aktuálně neběží. Není co zastavovat.");
        
        var endTime = DateTime.Now;
        var duration = endTime - _currentRunningTask.StartTime;
        
        // Uložíme odpracovaný blok do naší In-Memory tabulky _timeEntries
        _timeEntries.Add(new TimeEntry(_currentRunningTask.TaskId, _currentRunningTask.StartTime, endTime, request.Note));
        
        var stoppedTaskName = _currentRunningTask.TaskName;
        
        // Vyčistíme stopky
        _currentRunningTask = null; 

        return Results.Ok(new SimpleMessageResponse($"Ukončeno: {stoppedTaskName}. Zaznamenáno {(int)duration.TotalMinutes} minut.", endTime));
    }).WithName("StopCasNa");
}
}