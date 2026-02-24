using System.Collections.Concurrent;
using Common.Primitives;

namespace BackOffice.Identity.Identity;

public sealed class UserService
{
    private readonly ConcurrentDictionary<string, ulong> _users = new(10, 1_000); // todo vm: use db, fast solution is a dictionary 
    private ulong _incrementingId;
    
    public Task<LoginUserResult> LoginUserAsync(string username, string password, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (password.Equals("incorrect", StringComparison.OrdinalIgnoreCase))
            throw new BusinessException("User not found or has incorrect password", BusinessErrorCode.NotFound);

        if (_users.TryGetValue(username, out var id))
            return Task.FromResult(new LoginUserResult(id));

        id = Interlocked.Increment(ref _incrementingId);
        _users[username] = id;
        return Task.FromResult(new LoginUserResult(id));
    }

    public Task<string> GetUserByIdAsync(ulong userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var result = _users.FirstOrDefault(x => x.Value == userId).Key;

        return Task.FromResult(result);
    }
}