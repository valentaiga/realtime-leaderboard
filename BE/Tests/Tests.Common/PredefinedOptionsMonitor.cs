using Microsoft.Extensions.Options;

namespace Tests.Common;

internal class PredefinedOptionsMonitor<TOptions>(TOptions options) : IOptionsMonitor<TOptions>
{
    public TOptions Get(string? name) => options;

    public IDisposable? OnChange(Action<TOptions, string?> listener)
    {
        throw new NotImplementedException();
    }

    public TOptions CurrentValue => options;
}