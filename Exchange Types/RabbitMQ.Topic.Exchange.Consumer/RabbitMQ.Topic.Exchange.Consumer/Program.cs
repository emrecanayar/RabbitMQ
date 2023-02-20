

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

//Publisher daki exchange yapısıyla aynı yapıya sahip bir topic exchange oluşturulacak.
channel.ExchangeDeclare(
    exchange: "topic-exchange-example",
    type: ExchangeType.Topic
    );

//Publisher ile eşlecek olan topic bilgisini kullanıcıdan alalım.
Console.Write("Dinlenecek Topic Formatını Belirtiniz : ");
string topic = Console.ReadLine();
//Channeldan oluşan Queue name i alalım.
string queueName = channel.QueueDeclare().QueueName;

//Bu aşamadan sonra oluşturduğumuz kuyruk ile exchange bind etmemiz gerekmektedir.
channel.QueueBind(
    queue: queueName,
    exchange: "topic-exchange-example",
    routingKey: topic
    );

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: queueName,
    autoAck: true,
    consumer: consumer
    );

//Mesajımızı elde edelim

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};

Console.Read();
