namespace TrackingCustomerService.Services
{
    public interface IUnitOfWork
    {
        Task AddTrackingDataToMongoDb();
    }
}
