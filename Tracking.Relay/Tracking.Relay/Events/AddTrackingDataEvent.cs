namespace Tracking.Relay.Events
{
    public class AddTrackingDataEvent : IntegrationBaseEvent
    {
        public int Alt { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string IMEI { get; set; }
        public int Speed { get; set; }
        public int MyProperty { get; set; }
    }
}
