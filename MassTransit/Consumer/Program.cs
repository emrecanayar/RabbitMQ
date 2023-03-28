
using Consumer.Consumers;
using MassTransit;

string rabbitMQUri = "amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw";

string queueName = "example-queue";

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
    factory.ReceiveEndpoint(queueName, endpoint => { endpoint.Consumer<ExampleMessageConsumer>(); });
});

await bus.StartAsync();

Console.Read();