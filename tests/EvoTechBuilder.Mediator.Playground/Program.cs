
using EvoTechBuilder.Mediator;
using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Abstractions.Enums;
using EvoTechBuilder.Mediator.Abstractions.Options;
using EvoTechBuilder.Mediator.Playground;
using EvoTechBuilder.Mediator.Playground.Behaviors;
using EvoTechBuilder.Mediator.Playground.Handlers;
using EvoTechBuilder.Mediator.Playground.Notifications;
using EvoTechBuilder.Mediator.Playground.Request;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var services = new ServiceCollection();

services.AddSingleton<IOptions<MediatorOptions>>(
    Options.Create(new MediatorOptions
    {
        NotificationExecution = NotificationExecution.Sequential
    }));

services.AddTransient<
    INotificationHandler<CustomerCreatedNotification>,
    SendWelcomeEmailHandler>();

services.AddTransient<
    INotificationHandler<CustomerCreatedNotification>,
    UpdateCustomerStatisticsHandler>();

services.AddTransient<IRequestHandler<GetCustomerQuery, Customer>, GetCustomerHandler>()
    .AddTransient<ISender, Mediator>()
    .AddTransient<IPublisher, Mediator>()
    .AddTransient<IRequestHandler<CreateCustomerCommand>, CreateCustomerHandler>()
    .AddTransient(typeof(IPipelineBehavior<>), typeof(LoggingBehavior<>))
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

var serviceProvider = services.BuildServiceProvider();

var sender = serviceProvider.GetRequiredService<ISender>();

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

var publisher = serviceProvider.GetRequiredService<IPublisher>();

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
