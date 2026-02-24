using System.Collections.Concurrent;

namespace Common.Primitives;

public sealed class FixedSizeArrayPool<T>
{
    private readonly ushort _size;
    private readonly ConcurrentBag<T[]> _pool;

    public FixedSizeArrayPool(ushort size)
    {
        _size = size;
        _pool = [];
    }
    
    public T[] Rent() => _pool.TryTake(out var obj) ? obj : new T[_size];

    public void Return(T[] array, bool clearArray = false)
    {
        if (clearArray)
            Array.Clear(array, 0, array.Length);
        _pool.Add(array);
    }
}