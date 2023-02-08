
using RabbitMQ.Client;
using System.Text;

//RabbitMQ ile bağlatnı oluşturma. Kullandığımız yapı 
ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

//Queue Oluşturma
//durable parametresi kuyruğun rabbitmq üzerinde kalıcı olmasını sağlar. (Tabiki değeri true ise)
channel.QueueDeclare(queue: "example-queue", exclusive: false, durable: true);

//Oluşturulan kuyruğa mesaj gönderme işlemi.
//RabbitMQ kuyruğa atacağı mesajları byte türünden kabul etmektedir. Bu sebepten dolayı göndereceğimiz mesajları byte a dönüştürmemiz gerekmektedir.

//Basit bir tek mesaj gönderme şekli 
//byte[] message = Encoding.UTF8.GetBytes("Merhaba RabbitMQ");
//channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message); //Basit bir yapı kurduğumuz için exchange boş geçiyoruz. RoutingKey ise exchange boş olduğu için queue ismiyle aynı olacaktır.

//Bu kısım da mesajın rabbitmq da kalıcı olmasını sağlar. Burada verdiğimiz ayarıda aşağıdaki BasicPublish de basicProperties e tanımlamamız gerekmektedir.
IBasicProperties properties = channel.CreateBasicProperties();
properties.Persistent = true;

//Basit çoklu mesaj gönderme işlemi
for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes("Merhaba RabbitMQ " + i);
    channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message, basicProperties: properties); //basicProperties verdiğimiz değer sayesinde bu mesajı da RabbitMQ üzerinde kalıcı hale getiriyoruz.
}

Console.Read();