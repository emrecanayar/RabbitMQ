using MassTransit;
using Shared.Messages;

namespace Consumer.Consumers
{
    public class ExampleMessageConsumer : IConsumer<IMessage>
    {
        public Task Consume(ConsumeContext<IMessage> context)
        {
            Console.WriteLine($"Gelen Mesaj : {context.Message.Text}");
            return Task.CompletedTask;
        }
    }
}
