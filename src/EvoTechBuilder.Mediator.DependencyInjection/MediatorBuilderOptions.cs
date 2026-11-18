using System.Reflection;

using EvoTechBuilder.Mediator.Abstractions.Options;

namespace EvoTechBuilder.Mediator.DependencyInjection;

/// <summary>
/// Defines the configuration options for registering Mediator services.
/// </summary>
public sealed class MediatorBuilderOptions
{
    private readonly HashSet<Assembly> _assemblies = [];

    /// <summary>
    /// Gets the Mediator execution options.
    /// </summary>
    public MediatorOptions MediatorOptions { get; } = new();

    /// <summary>
    /// Gets the assemblies selected for automatic component registration.
    /// </summary>
    internal IReadOnlyCollection<Assembly> Assemblies => _assemblies;

    /// <summary>
    /// Adds the assembly containing the specified marker type to automatic registration.
    /// </summary>
    /// <typeparam name="TAssembly">
    /// A type used to identify the assembly to scan.
    /// </typeparam>
    public void AutoRegisterFromAssembly<TAssembly>()
    {
        AutoRegisterFromAssembly(typeof(TAssembly).Assembly);
    }

    /// <summary>
    /// Adds the specified assembly to automatic registration.
    /// </summary>
    /// <param name="assembly">The assembly to scan for mediator components.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="assembly"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified assembly is dynamic.
    /// </exception>
    public void AutoRegisterFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        if (assembly.IsDynamic)
        {
            throw new ArgumentException(
                "Dynamic assemblies cannot be scanned.",
                nameof(assembly));
        }

        _assemblies.Add(assembly);
    }
}