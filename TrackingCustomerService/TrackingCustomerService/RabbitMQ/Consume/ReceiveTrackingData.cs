using MassTransit;

namespace TrackingCustomerService.RabbitMQ.Consume
{

    public class ReceiveTrackingDataConsumer : IConsumer<ReceiveTrackingData>
    {
        public Task Consume(ConsumeContext<ReceiveTrackingData> context)
        {
            var response = context.Message;

            Console.WriteLine($"{DateTime.Now}");

            //context.Publish<ReceiveDevices>(new());

            return Task.CompletedTask;
        }
    }

    public class ReceiveTrackingData
    {
        public int Lat { get; set; }
        public int Long { get; set; }
        public string IMEI { get; set; }
    }
}
