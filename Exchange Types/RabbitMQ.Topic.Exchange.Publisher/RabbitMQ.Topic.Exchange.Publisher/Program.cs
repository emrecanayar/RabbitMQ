/*
 
 Öncelikle Topic Exchange nasıl davranıyordu buna bir girizgah yapalım. 

    Topic Exhange : Routing key'leri kullanarak mesajları kuyruklara yönlendirmek için kullanılan bir exchange dir. Bu exchange ile routing keyin bir kımsına/formatına/yapısına veya yapısındaki keylere göre kuyruklara mesajlar gönderilir.

    Kuyruklar da routing key'e göre bu exchange abone olabilir ve sadece ilgili routing key'e göre gönderilen mesajları alabilirler.

 */

using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

//Öncelikle topic exchange oluşturalım (Tipini Topic olarak ayarladık.)
channel.ExchangeDeclare(
    exchange: "topic-exchange-example",
    type: ExchangeType.Topic
    );

// Daha sonra mesajı göndereceğimizz yapıyı ve kullanıcıdan gelecek olan topic bilgisini alacağımız yapıyı kurgulayalım.
for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba {i}");
    Console.Write("Mesajın Gönderileceği Topic Formatını Bildiriniz : ");
    string topic = Console.ReadLine(); //Topic bilgisinide aldıktan sonra artık BasicPublish metoduyla mesajımızı gönderebiliriz.

    channel.BasicPublish(
        exchange: "topic-exchange-example",
        routingKey: topic, //Kullanıcıdan aldığımız topic bilgisini burada routing key olarak belirtiyoruz.
        body: message
        );
}

Console.Read();