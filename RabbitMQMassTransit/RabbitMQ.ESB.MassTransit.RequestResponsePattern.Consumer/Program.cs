using MassTransit;
using RabbitMQ.ESB.MassTransit.RequestResponsePattern.Consumer.Consumers;

string rabbitMQUri = "amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw";

string queueName = "request-queue";

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
    //Publisher dan beklediğimiz yapıyı bu şekilde tanıtmamız gerekmektedir.
    factory.ReceiveEndpoint(queueName, endpoint =>
    {
        endpoint.Consumer<RequestMesssageConsumer>();
    });
});

await bus.StartAsync();

Console.ReadKey();