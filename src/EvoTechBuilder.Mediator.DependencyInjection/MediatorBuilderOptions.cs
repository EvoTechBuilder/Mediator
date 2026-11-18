using EvoTechBuilder.Mediator.Abstractions.Options;

namespace EvoTechBuilder.Mediator.DependencyInjection;

/// <summary>
/// Defines the configuration options for registering Mediator services.
/// </summary>
public sealed class MediatorBuilderOptions
{
    /// <summary>
    /// Gets the Mediator execution options.
    /// </summary>
    public MediatorOptions MediatorOptions { get; } = new();

    /// <summary>
    /// Gets whether automatic component registration is enabled.
    /// </summary>
    public bool IsAutoRegisterEnabled { get; private set; }

    /// <summary>
    /// Enables automatic discovery and registration of Mediator components.
    /// </summary>
    public void AutoRegister()
    {
        IsAutoRegisterEnabled = true;
    }
}