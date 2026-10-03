// =====================================================================// Soubor: backend/Endpoints/UkolnicekEndpoints.cs// Projekt: PortalMCXIBackend// Verze: v1.0// Autor: Ing. Roman Fišer// Popis: Modul pro evidenci checklistů, opakujících se vzorů a//        běžných úkolů (To-Do). Slouží k rychlé organizaci dne.// Změny:// - v1.0 (17.05.2026): Vytvoření modulu s In-Memory úložištěm.// =====================================================================using System.Collections.Concurrent;using PortalMCXIBackend.Models;namespace PortalMCXIBackend.Endpoints;public static class UkolnicekEndpoints{// In-Memory seznam úkolůprivate static readonly ConcurrentBag _todos = new();private static int _nextTodoId = 1;public static void MapUkolnicekEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/todos");

    // Seed ukázkových úkolů (podle tvé historie požadavků)
    if (_todos.IsEmpty)
    {
        _todos.Add(new TodoItem(_nextTodoId++, "Zavést entity v Postgres", false, "Vývoj", DateTime.Now.AddDays(2)));
        _todos.Add(new TodoItem(_nextTodoId++, "Ověřit Contabo zálohy", false, "Server", DateTime.Now.AddDays(7)));
        _todos.Add(new TodoItem(_nextTodoId++, "Vyzkoušet nové UI v Reactu", true, "Vývoj", null));
        _todos.Add(new TodoItem(_nextTodoId++, "Doplnit predikce Nostradamus", false, "Esoterika", null));
    }

    // -------------------------------------------------------------
    // ENDPOINT: Získat všechny checklisty a úkoly
    // -------------------------------------------------------------
    group.MapGet("/", () => 
    {
        // Vracíme nesplněné jako první, splněné nakonec
        var sortedTodos = _todos.OrderBy(t => t.IsCompleted).ThenByDescending(t => t.Id).ToList();
        return Results.Ok(sortedTodos);
    }).WithName("GetAllTodos");

    // -------------------------------------------------------------
    // ENDPOINT: Přidat nový úkol (Checklist item)
    // -------------------------------------------------------------
    group.MapPost("/", (CreateTodoRequest request) => 
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return Results.BadRequest("Název úkolu nesmí být prázdný.");

        var newTodo = new TodoItem(
            _nextTodoId++, 
            request.Title, 
            false, 
            string.IsNullOrWhiteSpace(request.Category) ? "Obecné" : request.Category, 
            request.DueDate
        );
        
        _todos.Add(newTodo);
        
        return Results.Created($"/api/todos/{newTodo.Id}", newTodo);
    }).WithName("CreateTodo");

    // -------------------------------------------------------------
    // ENDPOINT: Přepnout stav splnění (Toggle)
    // -------------------------------------------------------------
    group.MapPut("/{id}/toggle", (int id) => 
    {
        var item = _todos.FirstOrDefault(t => t.Id == id);
        if (item == null)
            return Results.NotFound($"Úkol s ID {id} nebyl nalezen.");

        // Protože používáme recordy (immutable), musíme vytvořit novou kopii
        var updatedItem = item with { IsCompleted = !item.IsCompleted };
        
        // Nahrazení v kolekci (ConcurrentBag nepodporuje přímý Update, toto je bezpečný hack pro In-Memory)
        var newBag = new ConcurrentBag<TodoItem>(_todos.Where(t => t.Id != id));
        newBag.Add(updatedItem);
        
        _todos.Clear();
        foreach(var t in newBag) _todos.Add(t);

        return Results.Ok(updatedItem);
    }).WithName("ToggleTodoState");
}
}