using MassTransit;

namespace TrackingCustomerService.RabbitMQ.Consume
{

    public class SyncManagementDataConsumer : IConsumer<ResponseSyncManagementData>
    {
        public Task Consume(ConsumeContext<ResponseSyncManagementData> context)
        {
           var response = context.Message;

            return Task.CompletedTask;
        }
    }

    public class ResponseSyncManagementData
    {
        public List<Model> Models { get; set; }
    }

    public class Model
    {
        public int Id { get; set; }
        public string Caption { get; set; }

    }
}
