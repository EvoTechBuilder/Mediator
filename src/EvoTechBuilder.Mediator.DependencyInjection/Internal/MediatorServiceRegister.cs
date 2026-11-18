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
            .Where(IsDiscoverableType)
            .SelectMany(GetRegistrations)
            .OrderBy(
                registration => registration.ServiceType.FullName,
                StringComparer.Ordinal)
            .ThenBy(
                registration => registration.ImplementationType.FullName,
                StringComparer.Ordinal);

        foreach (var (serviceType, implementationType) in registrations)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Scoped(
                    serviceType,
                    implementationType));
        }
    }

    private static IEnumerable<(Type ServiceType, Type ImplementationType)>
        GetRegistrations(Type implementationType)
    {
        foreach (var serviceType in implementationType.GetInterfaces())
        {
            if (!IsMediatorComponent(serviceType))
            {
                continue;
            }

            if (implementationType.IsGenericTypeDefinition)
            {
                if (!serviceType.ContainsGenericParameters)
                {
                    continue;
                }

                var implementationArguments =
                    implementationType.GetGenericArguments();

                var serviceArguments =
                    serviceType.GetGenericArguments();

                if (implementationArguments.Length != serviceArguments.Length ||
                    !implementationArguments.SequenceEqual(serviceArguments))
                {
                    continue;
                }

                yield return (
                    serviceType.GetGenericTypeDefinition(),
                    implementationType);

                continue;
            }

            if (!implementationType.ContainsGenericParameters)
            {
                yield return (serviceType, implementationType);
            }
        }
    }

    private static bool IsDiscoverableType(Type type)
    {
        return type.IsClass && !type.IsAbstract;
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
