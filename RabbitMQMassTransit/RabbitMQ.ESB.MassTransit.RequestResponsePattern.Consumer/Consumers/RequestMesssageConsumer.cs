using MassTransit;
using RabbitMQ.ESB.MassTransit.RequestResponsePattern.Shared.RequestResponseMessages;

namespace RabbitMQ.ESB.MassTransit.RequestResponsePattern.Consumer.Consumers
{
    public class RequestMesssageConsumer : IConsumer<RequestMessages>
    {
        //Publisher dan gelen mesajları MassTransit kullanarak consume etme metodu.
        public async Task Consume(ConsumeContext<RequestMessages> context)
        {
            //Burada mesajı consume ettikten sonra geriye de response döndürmemiz gerekmektedir.
            Console.WriteLine(context.Message.Text);

            //Response işlemi

            await context.RespondAsync<ResponseMessage>(new()
            {
                Text = $"{context.Message.MessageNo} response to request"
            });
        }
    }
}
