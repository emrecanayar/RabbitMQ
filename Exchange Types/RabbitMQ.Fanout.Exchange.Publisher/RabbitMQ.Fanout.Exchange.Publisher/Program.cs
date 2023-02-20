/*
 
 Öncelikle Fanout Exchange nasıl davranıyordu buna bir girizgah yapalım. 

    Fanount Exhange : Mesajların, bu exhange'e bind olmuş olan tüm kuyruklara gönderilmesini sağlar. Publisher mesajların gönderilidiği kuyruk isimlerini dikkate almaz ve mesajları tüm kuyruklara gönderir.

 */

using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

//Burada kanal üzerinden fanout exchange tanımladık. Tanımladığımız exhange in adını ve tipini burada bu şekilde tanımlıyoruz.
channel.ExchangeDeclare(
    exchange: "fanout-exchange-example",
    type: ExchangeType.Fanout
    );

//Burada otomatik olarak bir döngü vasıtasıyla mesaj yayınlayacağız.

for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba {i}");

    //Burada kanal üzerinden basicPublish metodu sayesinde parametre olarak belirtitğimiz exchange adı, routing Key i boş bırakıyoruz bunun nedeni ise fanout exhange mesajları direkt kuyruklara iletir herhangi bir routing key ihtiyacımız kalmaz. Eğer belirli bir kuyruğa bu mesajı iletmek istiyorsak işte o zaman fanout exchange kullanmamamız gerekmektedir. Son olarak da body parametresiyle beraber tasarladığımız mesajı gönderiyoruz.
    channel.BasicPublish(
        exchange: "fanout-exchange-example",
        routingKey: String.Empty,
        body: message
        );
}

Console.Read();