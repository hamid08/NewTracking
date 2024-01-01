using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Tracking.Relay;
using Tracking.Relay.Events;

var host = Host.CreateDefaultBuilder(args)

               .ConfigureServices((hostContext, services) =>
               {
                   services.AddMassTransit(x =>
                   {
                       x.UsingRabbitMq((ctx, cfg) =>
                       {
                           cfg.Host("amqp://guest:guest@localhost:5672");
                       });
                   });


                   // OPTIONAL, but can be used to configure the bus options
                   services.AddOptions<MassTransitHostOptions>()
                       .Configure(options =>
                       {
                           // if specified, waits until the bus is started before
                           // returning from IHostedService.StartAsync
                           // default is false
                           options.WaitUntilStarted = true;

                           // if specified, limits the wait time when starting the bus
                           options.StartTimeout = TimeSpan.FromSeconds(10);

                           // if specified, limits the wait time when stopping the bus
                           options.StopTimeout = TimeSpan.FromSeconds(30);
                       });

                   services.AddHostedService<Worker>();

               })
               .Build();

await host.RunAsync();




public class Worker : BackgroundService
{
    private readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(new AddTrackingDataEvent
            {
                IMEI = Guid.NewGuid().ToString(),
                Lat = new Random().Next(15),
                Long = new Random().Next(15),
                Alt = new Random().Next(15),
                Speed = new Random().Next(3),


            });

            Console.WriteLine("Message published to the queue.");

            await Task.Delay(1000, stoppingToken);
        }
    }
}
