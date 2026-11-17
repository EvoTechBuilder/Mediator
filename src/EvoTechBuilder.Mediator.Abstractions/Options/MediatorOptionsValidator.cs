using Microsoft.Extensions.Options;

namespace EvoTechBuilder.Mediator.Abstractions.Options;

/// <summary>
/// Validates the configuration of the Mediator implementation.
/// </summary>
public sealed class MediatorOptionsValidator
    : IValidateOptions<MediatorOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, MediatorOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return !Enum.IsDefined(options.NotificationExecution)
            ? ValidateOptionsResult.Fail(
                $"The notification execution strategy '{options.NotificationExecution}' is invalid.")
            : ValidateOptionsResult.Success;
    }
}
