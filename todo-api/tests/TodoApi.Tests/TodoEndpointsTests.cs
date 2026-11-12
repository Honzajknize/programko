using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TodoApi.Tests;

/// <summary>
/// Integrační testy HTTP endpointů. Každý test dostane vlastní instanci aplikace,
/// takže testy nesdílejí data v úložišti.
/// </summary>
public class TodoEndpointsTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TodoEndpointsTests()
    {
        Environment.SetEnvironmentVariable("APP_VERSION", "test");
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private record HealthResponse(string Status, string Version);

    [Fact]
    public async Task Health_ReturnsOkWithVersion()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("ok", body!.Status);
        Assert.Equal("test", body.Version);
    }

    [Fact]
    public async Task GetTodos_ReturnsEmptyListInitially()
    {
        var todos = await _client.GetFromJsonAsync<List<TodoItem>>("/api/todos");

        Assert.NotNull(todos);
        Assert.Empty(todos);
    }

    [Fact]
    public async Task PostTodo_ReturnsCreatedItem()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "Naučit se GitHub Actions" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var item = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.Equal("Naučit se GitHub Actions", item!.Title);
    }

    [Fact]
    public async Task PostTodo_WithEmptyTitle_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "   " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTodoById_ReturnsItem()
    {
        var created = await CreateTodoAsync("Přečíst zadání");

        var item = await _client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");

        Assert.Equal(created.Id, item!.Id);
        Assert.Equal("Přečíst zadání", item.Title);
    }

    [Fact]
    public async Task GetTodoById_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/todos/12345");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutTodo_MarksItemAsCompleted()
    {
        var created = await CreateTodoAsync("Odevzdat úkol");

        var response = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", new { isCompleted = true });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var item = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.True(item!.IsCompleted);
    }

    [Fact]
    public async Task PutTodo_UnknownId_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync("/api/todos/12345", new { isCompleted = true });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<TodoItem> CreateTodoAsync(string title)
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoItem>())!;
    }
}
