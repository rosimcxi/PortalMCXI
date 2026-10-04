using System.Collections.Concurrent;
using PortalMCXIBackend.Models;

namespace PortalMCXIBackend.Endpoints;

public static class UkolnicekEndpoints
{
    private static readonly ConcurrentBag<TodoItem> Todos =
        new();

    private static int _nextTodoId = 1;

    public static void MapUkolnicekEndpoints(
        this WebApplication app)
    {
        var group = app.MapGroup("/api/todos");

        EnsureSeedData();

        group.MapGet("/", () =>
        {
            var sortedTodos = Todos
                .OrderBy(item => item.IsCompleted)
                .ThenByDescending(item => item.Id)
                .ToList();

            return Results.Ok(sortedTodos);
        })
        .WithName("GetAllTodos");

        group.MapPost("/", (CreateTodoRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(
                    "Název úkolu nesmí být prázdný.");
            }

            var newTodo = new TodoItem(
                _nextTodoId++,
                request.Title,
                false,
                string.IsNullOrWhiteSpace(request.Category)
                    ? "Obecné"
                    : request.Category,
                request.DueDate);

            Todos.Add(newTodo);

            return Results.Created(
                $"/api/todos/{newTodo.Id}",
                newTodo);
        })
        .WithName("CreateTodo");

        group.MapPut("/{id}/toggle", (int id) =>
        {
            var item = Todos
                .FirstOrDefault(todo => todo.Id == id);

            if (item is null)
            {
                return Results.NotFound(
                    $"Úkol s ID {id} nebyl nalezen.");
            }

            var updatedItem =
                item with
                {
                    IsCompleted = !item.IsCompleted
                };

            var replacement =
                new ConcurrentBag<TodoItem>(
                    Todos.Where(todo => todo.Id != id));

            replacement.Add(updatedItem);

            Todos.Clear();

            foreach (var todo in replacement)
            {
                Todos.Add(todo);
            }

            return Results.Ok(updatedItem);
        })
        .WithName("ToggleTodoState");
    }

    private static void EnsureSeedData()
    {
        if (!Todos.IsEmpty)
        {
            return;
        }

        Todos.Add(
            new TodoItem(
                _nextTodoId++,
                "Zavést entity v Postgres",
                false,
                "Vývoj",
                DateTime.Now.AddDays(2)));

        Todos.Add(
            new TodoItem(
                _nextTodoId++,
                "Ověřit Contabo zálohy",
                false,
                "Server",
                DateTime.Now.AddDays(7)));

        Todos.Add(
            new TodoItem(
                _nextTodoId++,
                "Vyzkoušet nové UI v Reactu",
                true,
                "Vývoj",
                null));

        Todos.Add(
            new TodoItem(
                _nextTodoId++,
                "Doplnit predikce Nostradamus",
                false,
                "Esoterika",
                null));
    }
}
