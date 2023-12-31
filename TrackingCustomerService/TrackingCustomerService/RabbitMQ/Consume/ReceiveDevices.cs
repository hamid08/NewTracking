using MassTransit;

namespace TrackingCustomerService.RabbitMQ.Consume
{

    public class ReceiveDevicesConsumer : IConsumer<ReceiveDevices>
    {
        public Task Consume(ConsumeContext<ReceiveDevices> context)
        {
            Console.WriteLine($"{DateTime.Now}");
            var response = context.Message;

            return Task.CompletedTask;
        }
    }




    public class ReceiveDevices
    {
        public List<string> IMEIs { get; set; }

    }
}
