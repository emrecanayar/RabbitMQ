namespace RabbitMQ.ESB.MassTransit.RequestResponsePattern.Shared.RequestResponseMessages
{
    public record RequestMessages
    {
        public int MessageNo { get; set; }
        public string Text { get; set; }
    }
}
