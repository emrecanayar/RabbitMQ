using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

channel.ExchangeDeclare(
    exchange: "header-exchange-example",
    type: ExchangeType.Headers);

for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba {i}");
    Console.Write("Lütfen header value'sunu giriniz : ");
    string value = Console.ReadLine();

    IBasicProperties basicProperties = channel.CreateBasicProperties();
    basicProperties.Headers = new Dictionary<string, object>
    {
        ["no"] = value
    };

    channel.BasicPublish(
        exchange: "header-exchange-example",
        routingKey: string.Empty,
        body: message,
        basicProperties: basicProperties
        );
}
