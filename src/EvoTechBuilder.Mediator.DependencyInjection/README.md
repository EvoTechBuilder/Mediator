# EvoTechBuilder.Mediator.DependencyInjection

Dependency injection integration for EvoTechBuilder.Mediator.

This package provides the `AddMediator()` extension method for registering Mediator services in an `IServiceCollection`.

## Installation

```bash id="u93lsf"
dotnet add package EvoTechBuilder.Mediator.DependencyInjection
```

## Usage

```csharp
builder.Services.AddMediator(options =>
{
    options.MaxConcurrentHandlers = 5;
});
```

## Features

- Registers `ISender` and `IPublisher`.
- Configures `MediatorOptions`.
- Validates configuration on application startup.
- Registers the Mediator implementation with scoped lifetime.

## Related packages

- `EvoTechBuilder.Mediator`
- `EvoTechBuilder.Mediator.Abstractions`