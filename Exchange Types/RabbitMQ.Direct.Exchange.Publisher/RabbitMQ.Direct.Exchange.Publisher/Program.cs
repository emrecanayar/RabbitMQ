//RabbitMQ ile bağlatnı oluşturma. Kullandığımız yapı 
using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

//Direct exchange oluşturma işlemi (Direct exchange kullanımında rabbitmq routing key e bakar. Buna karşışık gelen key e göre ilgili kuyruğa yönlendirme yapar.)
channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);

while (true)
{
    //Ekrandan bir mesaj alıyoruz ve onu byte array türüne çeviriyoruz.
    Console.Write("Mesaj Giriniz : ");
    string message = Console.ReadLine();
    byte[] byteMessage = Encoding.UTF8.GetBytes(message);

    //Direct exchange türünde routingKey üzerinden kuyrukların ayırt edilebilirliği sağlanır. Bu sebepten routing key kullanmak önemlidir.

    channel.BasicPublish(exchange: "direct-exchange-example", // Kullanacağımız exchange'in adı
                         routingKey: "direct-queue-example", // Mesajı göndereceğimiz kuyruğun adı
                         body: byteMessage); //Göndereceğimiz byte[] türündeki mesaj           

}


Console.Read();