using MassTransit;
using Microsoft.AspNetCore.Mvc;
using TrackingCustomerService.RabbitMQ.Publish;

namespace TrackingCustomerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SyncDataController : ControllerBase
    {
        private readonly ILogger<SyncDataController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;



        public SyncDataController(ILogger<SyncDataController> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("SendDevices")]
        public async Task SendDevices()
        {
            var devices = new SendDevices();
            devices.CustomerId = Guid.NewGuid().ToString();
            devices.DeviceInfo = new DeviceInfo
            {
                IMEI= Guid.NewGuid().ToString(),
                ModelId= Guid.NewGuid().ToString(),
                SimcardNumber = Guid.NewGuid().ToString(),
                
            };

            await _publishEndpoint.Publish<SendDevices>(devices);

        }

        [HttpPost("RequestSyncManagementData")]
        public async Task RequestSyncManagementData()
        {
            await _publishEndpoint.Publish<RequestSyncManagementData>(new RequestSyncManagementData());

        }
    }
}