using System.Collections.Concurrent;
using Microsoft.Extensions.ObjectPool;

namespace Common.Primitives;

public sealed class ObjectRingBuffer<T> : ObjectPool<T> where T : class, new()
{
    private readonly ConcurrentBag<T> _ring;

    public ObjectRingBuffer() : this(10)
    {
    }
    
    public ObjectRingBuffer(ushort initialCapacity)
    {
        _ring = [];

        for (var i = 0; i < initialCapacity; i++)
            _ring.Add(new T());
    }

    public override T Get() => _ring.TryTake(out var obj) ? obj : new T();

    public override void Return(T obj) => _ring.Add(obj);
}