//RabbitMQ ile bağlatnı oluşturma. Kullandığımız yapı 
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

#region P2P(Point - To - Point Tasarımı)
/*
 Bu tasarımda bir publisher iligli mesajı direkt bir kuyruğa gönderir ve bu mesaj kuyruğu işleyen bir cunsomer tarafından tüketilir. Eğer ki senaryo gereği bir mesajın bir tüketici tarafından işlenmesi gerekiyorsa bu yaklaşım kullanılır.
 
 */

////Şimdi bir kuyruk oluşturalım ve tanımlayalım.

//string queueName = "example-p2p-queue";

//channel.QueueDeclare(
//    queue: queueName,
//    durable: false,
//    exclusive: false,
//    autoDelete: false
//    );

////Göndereceğimiz mesajı tanımlayalım.
//byte[] message = Encoding.UTF8.GetBytes("Merhaba P2P");

////P2P olduğu için herhangi bir exchange kullanmıyoruz.
//channel.BasicPublish(
//    exchange: string.Empty,
//    routingKey: queueName,
//    body: message
//    );


#endregion

#region Publish/Subscribe (Pub/Sub) Tasarımı
/*
 Bu tasarımda publisher mesajı bir exchange'e gönderir ve böylece mesaj bu exchange'e bind edilmiş olan tüm kuyruklara gönderilir. Bu tasarım bir mesajın bir çok tüketici tarafından işlenmesi gerektiği durumlarda kullanılır.
 
 */

//Bir exchange oluşturalım.
//string exchangeName = "example-pub-sub-exchange";

////Oluştuurlan mesaj sub olan bütün kuyruklara iletileceği için biz burada fanout exchange tanımlıyoruz.
//channel.ExchangeDeclare(
//    exchange: exchangeName,
//    type: ExchangeType.Fanout
//    );

//for (int i = 0; i < 100; i++)
//{
//    await Task.Delay(200);
//    //Göndereceğimiz mesajı tanımlayalım.
//    byte[] message = Encoding.UTF8.GetBytes("Merhaba Publish/Subscribe");

//    channel.BasicPublish(
//        exchange: exchangeName,
//        routingKey: string.Empty,
//        body: message
//        );
//}


#endregion

#region Work Queue(İş Kuyruğu Tasarımı) 
/*
 Bu tasarımda publisher tarafından yayınlanmış bir mesajın birden fazla cunsomer arasından yalnızca biri tarafından tüketilmesi amaçlanmaktadır. Böyle mesajların işlenmesi sürecinde tüm consumer'lar aynı iş yüküne ve eşit görev dağılımına sahip olacaktırlar.
 
 Her bir mesaj farklı bir consumer tarafından işlenmektedir. Eşit yük dağılımıyla.
 */

//Kuyruk oluşturuldu.
//string queueName = "example-work-queue";

//channel.QueueDeclare(
//    queue: queueName,
//    durable: false,
//    exclusive: false,
//    autoDelete: false);

////Mesaj gönderimi sağlanıldı.
//for (int i = 0; i < 100; i++)
//{
//    await Task.Delay(200);

//    byte[] message = Encoding.UTF8.GetBytes("Work Queue" + i);

//    channel.BasicPublish(
//        exchange: string.Empty,
//        routingKey: queueName,
//        body: message);
//}

#endregion

#region Request/Response Tasarımı​

/*
 Bu tasarımda publisher bir request yapar gibi kuyruğa mesaj gönderir ve bu mesajı tüketen consumer dan sonuca dair başka bir kuyruktan yanıt/response bekler. Bu tarz senaryolar için oldukça uygun bir tasarımdır.

Burada dikkat edilmesi gereken husus şudur. Bir publisher mesajı gönderdikten sonra bunu consumer tükettikten sonra tekrardan başka bir kuyruk üzerinden publisher a geri döndürür. Bu durumda publisher aynı zamanda bir consumer da olmaktadır. Tabi bu döngü consumer içinde geçerli olacaktır. Yani consumer da aynı zamanda geriye bir mesaj döndüğü için bir publisherdır.
 */

string requestQueueName = "example-request-response-queue";
channel.QueueDeclare(
    queue: requestQueueName,
    durable: false,
    exclusive: false,
    autoDelete: false);

string replyQueueName = channel.QueueDeclare().QueueName;

//Bunu tanımlamamızın sebebi gönderdiğimiz mesajlar sonucunda gelecek olan response u ayırt etmemiz için gereklidir.
string correlationId = Guid.NewGuid().ToString();

#region Request Mesajını Oluşturma ve Gönderme
IBasicProperties properties = channel.CreateBasicProperties();
properties.CorrelationId = correlationId;
properties.ReplyTo = replyQueueName;

for (int i = 0; i < 10; i++)
{
    byte[] message = Encoding.UTF8.GetBytes("Merhaba Request Response" + i);
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: requestQueueName,
        body: message,
        basicProperties: properties);
}
#endregion
#region Response Kuyruğu Dinleme
EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: replyQueueName,
    autoAck: true,
    consumer: consumer);

consumer.Received += (sender, e) =>
{
    if (e.BasicProperties.CorrelationId == correlationId)
    {
        //....
        Console.WriteLine($"Response : {Encoding.UTF8.GetString(e.Body.Span)}");
    }
};
#endregion

#endregion

Console.Read();