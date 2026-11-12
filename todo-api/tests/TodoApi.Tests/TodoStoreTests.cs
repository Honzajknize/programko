namespace TodoApi.Tests;

/// <summary>
/// Jednotkové testy úložiště úkolů.
/// </summary>
public class TodoStoreTests
{
    private readonly TodoStore _store = new();

    [Fact]
    public void Add_AssignsIncrementingIds()
    {
        var first = _store.Add("První");
        var second = _store.Add("Druhý");

        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
    }

    [Fact]
    public void Add_CreatesNotCompletedItem()
    {
        var item = _store.Add("Nakoupit");

        Assert.Equal("Nakoupit", item.Title);
        Assert.False(item.IsCompleted);
    }

    [Fact]
    public void Get_ReturnsExistingItem()
    {
        var added = _store.Add("Uklidit");

        var found = _store.Get(added.Id);

        Assert.Equal(added, found);
    }

    [Fact]
    public void Get_ReturnsNullForUnknownId()
    {
        Assert.Null(_store.Get(42));
    }

    [Fact]
    public void GetAll_ReturnsItemsOrderedById()
    {
        _store.Add("A");
        _store.Add("B");
        _store.Add("C");

        var all = _store.GetAll();

        Assert.Equal(new[] { 1, 2, 3 }, all.Select(i => i.Id));
    }

    [Fact]
    public void SetCompleted_UpdatesItem()
    {
        var item = _store.Add("Napsat testy");

        var updated = _store.SetCompleted(item.Id, true);

        Assert.NotNull(updated);
        Assert.True(updated.IsCompleted);
        Assert.True(_store.Get(item.Id)!.IsCompleted);
    }

    [Fact]
    public void SetCompleted_ReturnsNullForUnknownId()
    {
        Assert.Null(_store.SetCompleted(99, true));
    }
}
