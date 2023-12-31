using MassTransit;
using RabbitMQ.Client;
using System.Reflection;
using TrackingCustomerService.RabbitMQ.Consume;
using TrackingCustomerService.RabbitMQ.Publish;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
            //h.UseSsl(s =>
            //{
            //    s.Protocol = System.Security.Authentication.SslProtocols.Tls12;
            //});
        });

        // Remove default wrapper message
        cfg.ClearSerialization();
        cfg.UseRawJsonSerializer();


        // Set exchange name
        cfg.Message<RequestSyncManagementData>(top =>
        {
            top.SetEntityName("TrackingCustomerService:RequestSyncManagementData");
        });


        // Configurations per Publish
        cfg.Publish<RequestSyncManagementData>(top =>
        {
            top.ExchangeType = ExchangeType.Topic;
            top.Durable = true;
            top.BindQueue("TrackingCustomerService:RequestSyncManagementData", "TrackingCustomerService.RequestSyncManagementData", bind =>
            {
                bind.Durable = true;
                bind.ExchangeType = ExchangeType.Topic;
                bind.SetQuorumQueue();
            });
        });


        // Set exchange name
        cfg.Message<SendDevices>(top =>
        {
            top.SetEntityName("TrackingCustomerService:SendDevices");
        });


        // Configurations per Publish
        cfg.Publish<SendDevices>(top =>
        {
            top.ExchangeType = ExchangeType.Topic;
            top.Durable = true;
            top.BindQueue("TrackingCustomerService:SendDevices", "TrackingCustomerService.SendDevices", bind =>
            {
                bind.Durable = true;
                bind.ExchangeType = ExchangeType.Topic;
                bind.SetQuorumQueue();
            });
        });



        //--------------------------------------------Consume

        cfg.ReceiveEndpoint("Tracking.Agent.SendManagementData", e =>
        {
            e.ExchangeType = ExchangeType.Topic;
            e.Durable = true;
            e.SetQuorumQueue();
            e.Consumer<SyncManagementDataConsumer>(context);
            e.Bind("TrackingCustomerService:ResponseSyncManagementData", x => x.RoutingKey = "");
            e.ConfigureConsumeTopology = false;


        });


        cfg.ReceiveEndpoint("Tracking.Agent.SendDevices", e =>
        {
            e.ExchangeType = ExchangeType.Topic;
            e.Durable = true;
            e.SetQuorumQueue();
            e.Consumer<ReceiveDevicesConsumer>(context);
            e.Bind("TrackingCustomerService:ReceiveDevices", x => x.RoutingKey = "");
            e.ConfigureConsumeTopology = false;

        });


        cfg.ReceiveEndpoint("Tracking.Agent.TrackingData", e =>
        {
            e.ExchangeType = ExchangeType.Topic;
            e.Durable = true;
            e.SetQuorumQueue();
            e.Consumer<ReceiveTrackingDataConsumer>();
            e.Bind("TrackingCustomerService:ReceiveTrackingData", x => x.RoutingKey = "");
            e.ConfigureConsumeTopology = false;
            

        });

        cfg.UseDelayedMessageScheduler();


        cfg.ConfigureEndpoints(context);
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
