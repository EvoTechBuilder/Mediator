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
    .AddTransient(typeof(IPipelineBehavior<>), typeof(LoggingBehavior<>));

var serviceProvider = services.BuildServiceProvider();

var sender = serviceProvider.GetRequiredService<ISender>();

var customer = await sender.Send(new GetCustomerQuery(Guid.NewGuid()));

await sender.Send(new CreateCustomerCommand(customer.Name));

Console.WriteLine(customer.Name);