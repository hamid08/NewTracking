using Tracking.Relay.Events;

namespace TrackingCustomerService.Services
{
    public interface ILocationService
    {
        Task HandleData(AddTrackingDataEvent @event);
        Task GetData();
    }
}
