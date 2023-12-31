using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Tracking.Relay;


var host = Host.CreateDefaultBuilder(args)

               .ConfigureServices((hostContext, services) =>
               {
                   services.AddMassTransit(x =>
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
                           cfg.Message<TrackingData>(top =>
                           {
                               top.SetEntityName("Tracking.Relay:TrackingData");
                           });


                           // Configurations per Publish
                           cfg.Publish<TrackingData>(top =>
                           {
                               top.ExchangeType = ExchangeType.Topic;
                               top.Durable = true;
                               top.BindQueue("Tracking.Relay:TrackingData", "Tracking.Relay.TrackingData", bind =>
                               {
                                   bind.Durable = true;
                                   bind.ExchangeType = ExchangeType.Topic;
                                   bind.SetQuorumQueue();
                                   //bind.SetQueueArgument("x-expires", 300000); // The queue will expire and be deleted after 300000 milliseconds (5 minutes)
                                 //  bind.SetQueueArgument("x-queue-mode", "lazy"); // For Save in Hard
                               });
                           });



                           cfg.ConfigureEndpoints(context);
                       });
                   });

                   services.AddHostedService<Worker>();
               })
               .Build();


await host.RunAsync();




public class Worker : BackgroundService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public Worker(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _publishEndpoint.Publish<TrackingData>(new TrackingData
            {
              IMEI = Guid.NewGuid().ToString(),
              Lat = new Random().Next(15),
              Long = new Random().Next(15),

            });

            Console.WriteLine("Message published to the queue.");

            //await Task.Delay(1000, stoppingToken);
        }
    }
}
