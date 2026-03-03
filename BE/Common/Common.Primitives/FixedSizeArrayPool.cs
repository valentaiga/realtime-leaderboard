using System.Collections.Concurrent;

namespace Common.Primitives;

public sealed class FixedSizeArrayPool<T>(ushort size)
{
    private readonly ConcurrentBag<T[]> _pool = [];

    public T[] Rent() => _pool.TryTake(out var obj) ? obj : new T[size];

    public void Return(T[] array, bool clearArray = false)
    {
        if (clearArray)
            Array.Clear(array, 0, array.Length);
        _pool.Add(array);
    }
}