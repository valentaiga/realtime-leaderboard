using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Common.MQ.Primitives;

// all messages should be sent to the MQ, even if application is stopping
[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
[SuppressMessage("Reliability", "CA2016:Forward the \'CancellationToken\' parameter to methods")]
public abstract class MessageSenderBase<TMessage>(IOptionsMonitor<MessageSenderOptions> optionsMonitor) : BackgroundService
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    protected MessageSenderOptions Options { get; } = optionsMonitor.Get(nameof(TMessage));
    
    /// <summary> Read a message. </summary>
    /// <remarks> Should call a <see cref="OperationCanceledException"/> to finish sending. </remarks>
    protected abstract Task<TMessage> ReadMessageAsync();

    /// <summary> Triggered on <see cref="ReadMessageAsync"/> error. </summary>
    protected virtual Task OnReadMessageErrorAsync(Exception exception) => Task.CompletedTask;

    /// <summary>
    /// Reads message then sends it to the MQ. <br/>
    /// Cancellation token should be ignored because everything 
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken _)
    {
        while (true)
            try
            {
                var message = await ReadMessageAsync();
                await SendMessageUntilSuccessAsync(message);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exc)
            {
                await OnReadMessageErrorAsync(exc);
                await Task.Delay(Options.ReadMessageErrorRetryInterval);
            }
    }

    private async Task SendMessageUntilSuccessAsync(TMessage message)
    {
        while (true)
            try
            {
                await SendMessageAsync(message);
                break;
            }
            catch
            {
                await Task.Delay(Options.SendMessageErrorRetryInterval);
            }
    }

    protected abstract Task SendMessageAsync(TMessage message);
}
