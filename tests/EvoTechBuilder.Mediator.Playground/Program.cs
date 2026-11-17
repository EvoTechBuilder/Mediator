using EvoTechBuilder.Mediator;
using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Playground;
using EvoTechBuilder.Mediator.Playground.Behaviors;
using EvoTechBuilder.Mediator.Playground.Handlers;
using EvoTechBuilder.Mediator.Playground.Request;

using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddTransient<IRequestHandler<GetCustomerQuery, Customer>, GetCustomerHandler>()
    .AddTransient<ISender, Mediator>()
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