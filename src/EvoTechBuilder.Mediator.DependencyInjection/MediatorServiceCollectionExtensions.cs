using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Abstractions.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EvoTechBuilder.Mediator.DependencyInjection;

/// <summary>
/// Provides extension methods for registering Mediator services.
/// </summary>
public static class MediatorServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Mediator services in the specified service collection.
    /// </summary>
    /// <param name="services">
    /// The service collection.
    /// </param>
    /// <param name="options">
    ///  An optional delegate to configure Mediator registration options.
    /// </param>
    /// <returns>
    /// The service collection for chaining registrations.
    /// </returns>
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Action<MediatorBuilderOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builderOptions = new MediatorBuilderOptions();
        
        options?.Invoke(builderOptions);
        
        if (builderOptions.IsAutoRegisterEnabled)
        {
            throw new NotImplementedException();
        }
        
        services.AddOptions<MediatorOptions>()
            .Configure(configure =>
            {
                configure.NotificationExecution =
                    builderOptions.MediatorOptions.NotificationExecution;

                configure.MaxConcurrentHandlers =
                    builderOptions.MediatorOptions.MaxConcurrentHandlers;
            })
            .ValidateOnStart();

        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IValidateOptions<MediatorOptions>,
                MediatorOptionsValidator>());

        services.TryAddScoped<Mediator>();

        services.TryAddScoped<ISender>(
            serviceProvider => serviceProvider.GetRequiredService<Mediator>());

        services.TryAddScoped<IPublisher>(
            serviceProvider => serviceProvider.GetRequiredService<Mediator>());

        return services;
    }
}
