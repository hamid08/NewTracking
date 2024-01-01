using MassTransit;
using RabbitMQ.Client;
using System.Reflection;
using TrackingCustomerService.EventBus.Consumer;
using TrackingCustomerService.RabbitMQ.Consume;
using TrackingCustomerService.RabbitMQ.Publish;
using TrackingCustomerService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// OPTIONAL, but can be used to configure the bus options
builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options =>
    {
        // if specified, waits until the bus is started before
        // returning from IHostedService.StartAsync
        // default is false
        options.WaitUntilStarted = true;

        // if specified, limits the wait time when starting the bus
        options.StartTimeout = TimeSpan.FromMinutes(1);

        // if specified, limits the wait time when stopping the bus
        options.StopTimeout = TimeSpan.FromMinutes(1);
    });



builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TrackingDataConsumer>();

    x.UsingRabbitMq((context, cfg) =>
        {

            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");


                cfg.ReceiveEndpoint("tracking_data_queue", c =>
                {
                    c.ConfigureConsumer<TrackingDataConsumer>(context);
                });


                cfg.ConfigureEndpoints(context);
            });
        });
});
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpClient<ILocationService, LocationService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var location = services.GetRequiredService<ILocationService>();
        location.GetData();

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}



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
