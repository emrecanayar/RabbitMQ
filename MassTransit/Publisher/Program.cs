using MassTransit;
using Shared.Messages;

string rabbitMQUri = "amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw";

string queueName = "example-queue";

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
});

ISendEndpoint sendEndpoint = await bus.GetSendEndpoint(new($"{rabbitMQUri}/{queueName}"));

Console.Write("Gönderilecek mesaj : ");
string message = Console.ReadLine();
await sendEndpoint.Send<IMessage>(new ExampleMessage()
{
    Text = message
});

Console.Read();