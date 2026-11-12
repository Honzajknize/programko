using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Úložiště úkolů žije v paměti po celou dobu běhu aplikace.
builder.Services.AddSingleton<TodoStore>();

var app = builder.Build();

// Kontrola stavu aplikace (používá se např. pro sondy v Kubernetes).
app.MapGet("/health", () => Results.Ok(new { status = "ok", version = AppInfo.Version }));

var todos = app.MapGroup("/api/todos");

// Vrátí všechny úkoly.
todos.MapGet("/", (TodoStore store) => Results.Ok(store.GetAll()));

// Vrátí jeden úkol podle id.
todos.MapGet("/{id:int}", (int id, TodoStore store) =>
    store.Get(id) is { } item ? Results.Ok(item) : Results.NotFound());

// Vytvoří nový úkol.
todos.MapPost("/", (CreateTodoRequest request, TodoStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { error = "Název úkolu (title) nesmí být prázdný." });
    }

    var item = store.Add(request.Title.Trim());
    return Results.Created($"/api/todos/{item.Id}", item);
});

// Nastaví, zda je úkol dokončen.
todos.MapPut("/{id:int}", (int id, UpdateTodoRequest request, TodoStore store) =>
    store.SetCompleted(id, request.IsCompleted) is { } item ? Results.Ok(item) : Results.NotFound());

app.Run();

// Nutné pro integrační testy (WebApplicationFactory<Program>).
public partial class Program { }
