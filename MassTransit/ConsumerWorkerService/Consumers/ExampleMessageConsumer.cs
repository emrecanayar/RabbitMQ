using MassTransit;
using Shared.Messages;

namespace ConsumerWorkerService.Consumers
{
    public class ExampleMessageConsumer : IConsumer<IMessage>
    {
        public async Task Consume(ConsumeContext<IMessage> context)
        {
            Console.WriteLine($"Gelen Mesaj : {context.Message.Text}");
        }
    }
}
