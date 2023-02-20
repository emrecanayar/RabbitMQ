using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

//Şimdi burada öncelikle Publisher da yer alan exchange i aynısını tanımlamamız gerekiyor.
channel.ExchangeDeclare(
    exchange: "fanout-exchange-example",
    type: ExchangeType.Fanout
    );

//Ekrandan kuyruk adını istedik.
Console.Write("Kuyruk adını giriniz : ");
string queueName = Console.ReadLine();

//Almış olduğumuz kuyruk adında bir kuyruk oluşturacağız.
channel.QueueDeclare(
    queue: queueName,
    exclusive: false
    );

//Oluşturduğumuz kuyruğu artık bind etmemiz gerekmektedir.
channel.QueueBind(
    queue: queueName,
    exchange: "fanout-exchange-example",
    routingKey: String.Empty //Fanout exchange kullandığımız için burası boş geçilecek
    );

//Artık Publisher dan gelen mesajları okuma zamanı geldi. Şimdi bunu oluşturalım.

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer); // autoAck false yaptık çünkü mesajı işledikten sonra kuyruktan düşmesini gerektiğini ben yönetmek istiyorum.

//Gelen mesajı almak için received eventinden faydalanmamız gerekmektedir.
consumer.Received += (sender, args) =>
{
    //Gelen mesajı byte türünden stringe çevirdik ve ekranda gösterdik.
    string message = Encoding.UTF8.GetString(args.Body.Span);
    Console.WriteLine(message);

    //BasicAck metodu ile mesajın başarıyla işlendiğini RabbitMQ ya bildiriyoruz ve artık kuyruktan silinmesi gerektiğini belirtiyoruz.

    channel.BasicAck(args.DeliveryTag, multiple: false);
    //multiple parametresi eğer true olursa buna dair diğer mesajları da onaylandığını bildirmiş olur. Fakat false olursa sadece spesifik olarak ilgili mesajın sonlandırıldığı anlamı katılır.
};

//Not : Publisher da gelen mesajları sadece bu exhange e bind olmus olan kuyruklar kullanabilir.

Console.Read();