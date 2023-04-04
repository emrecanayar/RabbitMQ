using MassTransit;
using RabbitMQ.ESB.MassTransit.RequestResponsePattern.Shared.RequestResponseMessages;

string rabbitMQUri = "amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw";

string queueName = "request-queue";

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
});

await bus.StartAsync(); //Devamlı dinleme modun da olacağı için start diyerek bunu devamlı çalışma modunda tutuyoruz. Çünkü amacımız aslında gönderilen mesajı geriye almasını sağlamak bu sebeple de burada bu şekilde bir konfigurasyon yapmamız gerekmektedir.

IRequestClient<RequestMessages> request = bus.CreateRequestClient<RequestMessages>(new Uri($"{rabbitMQUri}/{queueName}"));

int i = 1;

while (true)
{
    await Task.Delay(200);
    //Burada mesaj gönderirken aynı zamanda gönderdiğimiz mesajın da response nu almak istiyorsak. Aşağıdaki gibi GetResponse metodunu kullanarak bu işi gerçekleştirebiliriz.
    Response<ResponseMessage> response = await request.GetResponse<ResponseMessage>(new() { MessageNo = i, Text = $"{i}.Message" });
    Console.WriteLine($"Response Received : {response.Message.Text}");
    i++;
}

Console.ReadKey();