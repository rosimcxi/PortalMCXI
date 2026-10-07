// PortalMCXI – obnovená deklarace z importovaného zdroje.
using System.Collections.Concurrent;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class UkolnicekEndpoints
{
private static readonly ConcurrentBag<TodoItem> _todos = new();
private static int _nextTodoId = 0;
private static readonly object _sync = new();
public static void MapUkolnicekEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/api/todos");

    // Seed ukázkových úkolů (podle tvé historie požadavků)
    if (_todos.IsEmpty)
    {
        _todos.Add(new TodoItem(Interlocked.Increment(ref _nextTodoId), "Zavést entity v Postgres", false, "Vývoj", DateTime.Now.AddDays(2)));
        _todos.Add(new TodoItem(Interlocked.Increment(ref _nextTodoId), "Ověřit Contabo zálohy", false, "Server", DateTime.Now.AddDays(7)));
        _todos.Add(new TodoItem(Interlocked.Increment(ref _nextTodoId), "Vyzkoušet nové UI v Reactu", true, "Vývoj", null));
        _todos.Add(new TodoItem(Interlocked.Increment(ref _nextTodoId), "Doplnit predikce Nostradamus", false, "Esoterika", null));
    }

    // -------------------------------------------------------------
    // ENDPOINT: Získat všechny checklisty a úkoly
    // -------------------------------------------------------------
    group.MapGet("/", () => 
    {
        lock (_sync) {
        // Vracíme nesplněné jako první, splněné nakonec
        var sortedTodos = _todos.OrderBy(t => t.IsCompleted).ThenByDescending(t => t.Id).ToList();
        return Results.Ok(sortedTodos);
        }
    }).WithName("GetAllTodos");

    // -------------------------------------------------------------
    // ENDPOINT: Přidat nový úkol (Checklist item)
    // -------------------------------------------------------------
    group.MapPost("/", (CreateTodoRequest request) => 
    {
        lock (_sync) {
        if (string.IsNullOrWhiteSpace(request.Title))
            return Results.BadRequest("Název úkolu nesmí být prázdný.");

        var newTodo = new TodoItem(
            Interlocked.Increment(ref _nextTodoId), 
            request.Title, 
            false, 
            string.IsNullOrWhiteSpace(request.Category) ? "Obecné" : request.Category, 
            request.DueDate
        );
        
        _todos.Add(newTodo);
        
        return Results.Created($"/api/todos/{newTodo.Id}", newTodo);
        }
    }).WithName("CreateTodo");

    // -------------------------------------------------------------
    // ENDPOINT: Přepnout stav splnění (Toggle)
    // -------------------------------------------------------------
    group.MapPut("/{id}/toggle", (int id) => 
    {
        lock (_sync) {
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
        }
    }).WithName("ToggleTodoState");
}
}
