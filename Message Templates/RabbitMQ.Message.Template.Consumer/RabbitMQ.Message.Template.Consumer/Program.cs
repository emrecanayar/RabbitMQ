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

//Şimdi bir kuyruk oluşturalım ve tanımlayalım.

//string queueName = "example-p2p-queue";

//channel.QueueDeclare(
//    queue: queueName,
//    durable: false,
//    exclusive: false,
//    autoDelete: false
//    );

////RabbitMQ dan gelen mesajı okuma işlemi.

//EventingBasicConsumer consumer = new(channel);
//channel.BasicConsume(
//    queue: queueName,
//    autoAck: false,
//    consumer: consumer);

////Gelen mesajı tüketme işlemi
//consumer.Received += Consumer_Received;

//void Consumer_Received(object? sender, BasicDeliverEventArgs e)
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//}

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

////Bu exchange e abone olacak olan kuyruğumuzu oluşturalım.

//string queueName = channel.QueueDeclare().QueueName; //Kuyruk oluşturduk ve kuyruğun adını aldık.

////İlgili kuyruğu exchange bind edelim.

//channel.QueueBind(
//    queue: queueName,
//    exchange: exchangeName,
//    routingKey: string.Empty);

////Gelen mesajı okuma işlemleri
//EventingBasicConsumer consumer = new(channel);
//channel.BasicConsume(
//    queue: queueName,
//    autoAck: false,
//    consumer: consumer
//    );

//consumer.Received += Consumer_Received;

//void Consumer_Received(object? sender, BasicDeliverEventArgs e)
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//}

#endregion

#region Work Queue(İş Kuyruğu Tasarımı) 
/*
 Bu tasarımda publisher tarafından yayınlanmış bir mesajın birden fazla cunsomer arasından yalnızca biri tarafından tüketilmesi amaçlanmaktadır. Böyle mesajların işlenmesi sürecinde tüm consumer'lar aynı iş yüküne ve eşit görev dağılımına sahip olacaktırlar.
 
 Her bir mesaj farklı bir consumer tarafından işlenmektedir. Eşit yük dağılımıyla.

 */

//Kuyruk Oluşturuldu.
//string queueName = "example-work-queue";

//channel.QueueDeclare(
//    queue: queueName,
//    durable: false,
//    exclusive: false,
//    autoDelete: false);

//EventingBasicConsumer consumer = new(channel);
//channel.BasicConsume(
//    queue: queueName,
//    autoAck: true,
//    consumer: consumer);

////Her bir consumer birer birer mesaj alır. Fakat globalde sınırsız olacak şekilde işlem yapabilir.
//channel.BasicQos(
//    prefetchCount: 1,
//    prefetchSize: 0,
//    global: false);

//consumer.Received += (sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};

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

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: requestQueueName,
    autoAck: true,
    consumer: consumer);

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
    //.....
    byte[] responseMessage = Encoding.UTF8.GetBytes($"İşlem tamamlandı. : {message}");
    IBasicProperties properties = channel.CreateBasicProperties();
    properties.CorrelationId = e.BasicProperties.CorrelationId;
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: e.BasicProperties.ReplyTo,
        basicProperties: properties,
        body: responseMessage);
};


#endregion


Console.Read();