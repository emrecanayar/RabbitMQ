using MassTransit;
using Shared.Messages;

namespace PublisherWorkerService.Services
{
    public class PublishMessageService : BackgroundService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public PublishMessageService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int i = 0;

            while (true)
            {
                ExampleMessage message = new()
                {
                    Text = $"{++i}.mesaj"
                };
                //Bu exchange baglı olan bütün kuyruklara mesaj gönderir.
                await _publishEndpoint.Publish(message);
            }
        }
    }
}
