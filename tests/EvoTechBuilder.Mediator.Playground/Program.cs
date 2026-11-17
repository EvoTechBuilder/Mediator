using EvoTechBuilder.Mediator;
using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Abstractions.Enums;
using EvoTechBuilder.Mediator.DependencyInjection;
using EvoTechBuilder.Mediator.Playground;
using EvoTechBuilder.Mediator.Playground.Behaviors;
using EvoTechBuilder.Mediator.Playground.Handlers;
using EvoTechBuilder.Mediator.Playground.Notifications;
using EvoTechBuilder.Mediator.Playground.Request;

using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddMediator(options =>
{
    options.MaxConcurrentHandlers = 1;
});

services.AddTransient<
    IRequestHandler<GetCustomerQuery, Customer>,
    GetCustomerHandler>();

services.AddTransient<
    IRequestHandler<CreateCustomerCommand>,
    CreateCustomerHandler>();

services.AddTransient<
    INotificationHandler<CustomerCreatedNotification>,
    SendWelcomeEmailHandler>();

services.AddTransient<
    INotificationHandler<CustomerCreatedNotification>,
    UpdateCustomerStatisticsHandler>();

services.AddTransient(
    typeof(IPipelineBehavior<>),
    typeof(LoggingBehavior<>));

services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(LoggingBehavior<,>));

using var serviceProvider = services.BuildServiceProvider(
    new ServiceProviderOptions
    {
        ValidateScopes = true,
        ValidateOnBuild = true
    });

using var scope = serviceProvider.CreateScope();

var scopedProvider = scope.ServiceProvider;

var mediator = scopedProvider.GetRequiredService<Mediator>();
var sender = scopedProvider.GetRequiredService<ISender>();
var publisher = scopedProvider.GetRequiredService<IPublisher>();

Console.WriteLine($"Same sender instance: {ReferenceEquals(mediator, sender)}");
Console.WriteLine($"Same publisher instance: {ReferenceEquals(mediator, publisher)}");

using var cancellationTokenSource = new CancellationTokenSource();

try
{
    var customerTask = sender.Send(
        new GetCustomerQuery(Guid.NewGuid()),
        cancellationTokenSource.Token);

    await Task.Delay(TimeSpan.FromSeconds(1));

    await cancellationTokenSource.CancelAsync();

    var customer = await customerTask;

    Console.WriteLine(customer.Name);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request cancelled.");
}

try
{
    await publisher.Publish(
        new CustomerCreatedNotification(Guid.NewGuid()));

    Console.WriteLine("[Publisher] Completed");
}
catch (InvalidOperationException exception)
{
    Console.WriteLine(
        $"[Publisher] Configuration error: {exception.Message}");
}
catch (AggregateException exception)
{
    foreach (var innerException in exception.InnerExceptions)
    {
        Console.WriteLine(
            $"[Publisher] Handler failed: {innerException.Message}");
    }
}