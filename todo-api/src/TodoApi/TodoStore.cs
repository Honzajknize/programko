using System.Collections.Concurrent;

namespace TodoApi;

/// <summary>
/// Jednoduché vláknově bezpečné úložiště úkolů v paměti.
/// </summary>
public class TodoStore
{
    private readonly ConcurrentDictionary<int, TodoItem> _items = new();
    private int _lastId;

    public IReadOnlyList<TodoItem> GetAll()
    {
        return _items.Values.OrderBy(item => item.Id).ToList();
    }

    public TodoItem? Get(int id)
    {
      if (_items.TryGetValue(id, out var item))
            return item;

        return null;
    }

    public TodoItem Add(string title)
    {
        var id = Interlocked.Increment(ref _lastId);
            var item = new TodoItem(id, title, false, DateTimeOffset.UtcNow);
        _items[id] = item;
        return item;
    }

    public TodoItem? SetCompleted(int id, bool isCompleted)
    {
        while (_items.TryGetValue(id, out var current))
        {
            var updated = current with { IsCompleted = isCompleted };
            if (_items.TryUpdate(id, updated, current))
                return updated;
        }

        return null;
    }

    public bool Remove(int id)
    {
        return _items.TryRemove(id, out _);
    }
}
