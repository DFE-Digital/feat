using Microsoft.AspNetCore.Http;

namespace feat.web.tests.PageTests;

public sealed class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();

    public IEnumerable<string> Keys => _store.Keys;
    public string Id { get; } = Guid.NewGuid().ToString();
    public bool IsAvailable { get; set; } = true;

    public void Clear() => _store.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public void Remove(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        _store.Remove(key);
    }

    public void Set(string key, byte[] value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        _store[key] = value;
    }

    public bool TryGetValue(string key, out byte[] value)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (_store.TryGetValue(key, out var stored))
        {
            value = stored;
            return true;
        }

        value = Array.Empty<byte>();
        return false;
    }
}