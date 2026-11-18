using System.Reflection;

using EvoTechBuilder.Mediator.Abstractions.Contracts;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EvoTechBuilder.Mediator.DependencyInjection.Internal;

/// <summary>
/// Discovers and registers Mediator components from loaded assemblies.
/// </summary>
internal static class MediatorServiceRegister
{
    /// <summary>
    /// Discovers and registers all supported Mediator components.
    /// </summary>
    /// <param name="services">
    /// The service collection.
    /// </param>
    public static void Register(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var registrations = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .SelectMany(GetLoadableTypes)
            .Where(IsConcreteType)
            .SelectMany(implementationType =>
                implementationType.GetInterfaces()
                    .Where(IsMediatorComponent)
                    .Select(serviceType => new
                    {
                        ServiceType = serviceType,
                        ImplementationType = implementationType
                    }))
            .OrderBy(
                registration => registration.ServiceType.FullName,
                StringComparer.Ordinal)
            .ThenBy(
                registration => registration.ImplementationType.FullName,
                StringComparer.Ordinal);

        foreach (var registration in registrations)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Scoped(
                    registration.ServiceType,
                    registration.ImplementationType));
        }
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }

    private static bool IsConcreteType(Type type)
    {
        return type.IsClass
            && !type.IsAbstract
            && !type.ContainsGenericParameters;
    }

    private static bool IsMediatorComponent(Type serviceType)
    {
        if (!serviceType.IsGenericType)
        {
            return false;
        }

        var genericTypeDefinition = serviceType.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(IHandler<>)
            || genericTypeDefinition == typeof(IHandler<,>)
            || genericTypeDefinition == typeof(INotificationHandler<>)
            || genericTypeDefinition == typeof(IPipelineBehavior<>)
            || genericTypeDefinition == typeof(IPipelineBehavior<,>);
    }
}
