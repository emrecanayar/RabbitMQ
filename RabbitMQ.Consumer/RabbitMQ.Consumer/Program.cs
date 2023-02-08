
//RabbitMQ ile bağlatnı oluşturma. Kullandığımız yapı 
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Rabbit-MQ da eğer farklı bir kuyruk mimarisi kullanmazsak default olarak Round-Robin Dispatching olarak davranır. (Publisher dan gelen mesajları consumer async olarak işler consumer lardan hangisi müsaitse mesajı üstüne alır ve işler.)
ConnectionFactory factory = new();
factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

//Bağlantıyı aktlifleştirme ve kanal açma.
using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

//Queue Oluşturma
channel.QueueDeclare(queue: "example-queue", exclusive: false, durable: true); //Consumer'da da kuyruk publisher'daki ile birebir aynı yapılandırılmada tanımlanmalıdır.
//Kuyruk Publisher da durable true olarak tanımlandıysa consumer tarafında da bunu true olarak tanımlamamız gerekmektedir.


//Queue Mesaj Okuma
EventingBasicConsumer consumer = new(channel);
//autoAck parametesi true ise kuyruk mesajdan otomatik olarak silinir.
//Eğer Message Acknowledgement mimarisini benimseyeceksek yani gönderilen mesaj işlendikten sonra kuyruktan kaldırılamsı gerekiyorsa buradaki autoAck parametresini false yapmamız gerekmektedir.
channel.BasicConsume(queue: "example-queue", autoAck: false, consumer: consumer);

//Consumerları eşit bir şekilde çalıştırmak istiyorsak eğer başka bir deyişle Fair Dispatch mekanızmasını kuracaksak aşağıdaki gibi bir kodlama işlemi yapmamız gerekmektedir. Bu metot sayesinde kuyruğu kaç tane consumer tüketirse tüketsin hepsi eşit bir tempoda üzerinde düşen görevi adil bir şekilde gerçekleştirir. (BasicQos)

channel.BasicQos(0, 1, false);
/*
 İlk parametre mesajın boyutunu (byte cinsinden) belirler. Eğer sıfır verirsek sınırsız olacak demektir.
 İkinci parametre ise bir consumer tarafından aynı anda işleme alınabilecek mesaj sayısını belirler.
 Üçüncü parametre ise bu konfügürasyonun tüm consumerler için mi yoksa sadece çağrı yapılan consumerlar için mi geçerli olup olmayacağını belirer.
 
 */

consumer.Received += (sender, args) =>
{
    //Kuyruğa gelen mesajların işlendiği yerdir.
    //args.Body = Kuyruktaki mesajın verisini bütünsel olarak getirecektir.
    //args.Body.Span veya e.Body.ToArray() : Kuyruktaki mesajın byte verisini getirecektir.

    Console.WriteLine(Encoding.UTF8.GetString(args.Body.Span));
    //BasicAck metodu ile mesajın başarıyla işlendiğini RabbitMQ ya bildiriyoruz ve artık kuyruktan silinmesi gerektiğini belirtiyoruz.
    channel.BasicAck(args.DeliveryTag, multiple: false); //multiple parametresi eğer true olursa buna dair diğer mesajları da onaylandığını bildirmiş olur. Fakat false olursa sadece spesifik olarak ilgili mesajın sonlandırıldığı anlamı katılır.

    //Bazen consumerlara kendi kontrollerimiz neticesinde mesajları işletmemek istemeyebiliriz.
    /*
     Böyle durumlarda channel.BasicNack metodunu kullanarak RabbitMQ ya bilgi verebilir ve mesajı tekrardan işletebiliriz.

    Burada requeue parametresi oldukça önem arz etmektedir. Bu parametre, bu consumer tarafından işlenmeyecek mesajın tekrardan kuyruğa eklenmesini sağlar.

    True değeri verildiği taktirde mesaj kuyruğa tekrardan işlenmek üzere kuyruğa eklenecek, false değerinde ise kuyruğa eklenmeyecek silinecektir.
     */
    //channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);

    // channel.BasicCancel metodu ile verilen consumerTag değerine karşılık gelen queue'daki tüm mesajlar redderilerek, işlenmez.

    // channel.BasicReject istediğimiz mesajları spesifik olarak reddetebiliriz.
};

Console.Read();