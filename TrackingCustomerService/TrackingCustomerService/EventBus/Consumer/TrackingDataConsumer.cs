using MassTransit;
using Tracking.Relay.Events;
using TrackingCustomerService.Services;

namespace TrackingCustomerService.EventBus.Consumer
{
    public class TrackingDataConsumer : IConsumer<AddTrackingDataEvent>
    {
        private readonly ILocationService _locationService;

        public TrackingDataConsumer(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public async Task Consume(ConsumeContext<AddTrackingDataEvent> context)
        {
          await  _locationService.HandleData(context.Message);
        }
    }
}
