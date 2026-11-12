namespace TodoApi;

/// <summary>
/// Jeden úkol v seznamu.
/// </summary>
public record TodoItem(int Id, string Title, bool IsCompleted, DateTimeOffset CreatedAt);

/// <summary>
/// Tělo požadavku POST /api/todos.
/// </summary>
public record CreateTodoRequest(string? Title);

/// <summary>
/// Tělo požadavku PUT /api/todos/{id}.
/// </summary>
public record UpdateTodoRequest(bool IsCompleted);
