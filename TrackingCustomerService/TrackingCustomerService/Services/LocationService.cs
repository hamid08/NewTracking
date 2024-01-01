using Tracking.Relay.Events;

namespace TrackingCustomerService.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        public LocationService(IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
        }


        public async Task GetData()
        {
           await _httpClient.GetAsync("https://google.com");
        }

        public async Task HandleData(AddTrackingDataEvent @event)
        {
           await _unitOfWork.AddTrackingDataToMongoDb();
        }
    }
}
