//RabbitMQ ile bağlatnı oluşturma. Kullandığımız yapı 
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

//1.Adım
//Direct exchange türünde publisher da tanımlanan exhange türü consumer da da aynı şekilde tanımlanacaktır.
channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);

//2.Adım
string queueName = channel.QueueDeclare().QueueName;

//3.Adım
channel.QueueBind(queue: queueName,
                  exchange: "direct-exchange-example",
                  routingKey: "direct-queue-example");



EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};

Console.Read();






//1. Adım : Publisher'da ki exchange ile birebir aynı isim ve type'a sahip bir exxhange tanımlamılıdır.
//2. Adım : Publisher tarafından routing key'de bulunan değerdeki kuyruğa gönderilen mesajları kendi oluşturduğumuz kuyruğa yönlendirerek tüketmemiz gerekmektedir. Bunun için öncelikle bir kuyruk oluşturmalıdır.