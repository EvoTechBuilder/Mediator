using EvoTechBuilder.Mediator;
using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Playground;

using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddTransient<IRequestHandler<GetCustomerQuery, Customer>, GetCustomerHandler>()
    .AddTransient<ISender, Mediator>();

var serviceProvider = services.BuildServiceProvider();

var sender = serviceProvider.GetRequiredService<ISender>();

var customer = await sender.Send(new GetCustomerQuery(Guid.NewGuid()));

Console.WriteLine(customer.Name);