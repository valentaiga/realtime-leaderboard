using System.Diagnostics.CodeAnalysis;
using Common.MQ.Primitives;

namespace Common.MQ;

public static class MQDiExtensions
{
    public static IServiceCollection AddMessageSender<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageSender, TMessage>(this IServiceCollection services)
        where TMessageSender : MessageSenderBase<TMessage>
    {
        services.AddOptions<MessageSenderOptions>(nameof(TMessage));

        services.AddHostedService<TMessageSender>();
        return services;
    }

    public static IServiceCollection AddMessageSender<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageSender, TMessage>(this IServiceCollection services, IConfiguration configuration, string configSectionPath)
        where TMessageSender : MessageSenderBase<TMessage>
    {
        services.AddOptions<MessageSenderOptions>(nameof(TMessage))
            .Configure(options => configuration.GetSection(configSectionPath).Bind(options));

        services.AddHostedService<TMessageSender>();
        return services;
    }
}