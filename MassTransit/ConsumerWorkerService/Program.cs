using ConsumerWorkerService.Consumers;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<ExampleMessageConsumer>();
            configurator.UsingRabbitMq((context, _configurator) =>
            {
                _configurator.Host("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

                _configurator.ReceiveEndpoint("example-message-queue", e => e.ConfigureConsumer<ExampleMessageConsumer>(context));
            });
        });
    })
    .Build();

host.Run();
