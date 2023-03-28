using MassTransit;
using PublisherWorkerService.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(configurator =>
        {
            configurator.UsingRabbitMq((context, _configurator) =>
            {
                _configurator.Host("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");
            });
        });

        services.AddHostedService<PublishMessageService>(provider =>
        {
            using IServiceScope scope = provider.CreateScope();
            IPublishEndpoint? publishEndPoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            return new(publishEndPoint);
        });
    })
    .Build();

host.Run();
