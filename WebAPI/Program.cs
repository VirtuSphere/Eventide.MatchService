using Eventide.MatchService.Application;
using Eventide.MatchService.Infrastructure;
using Eventide.MatchService.Application.EventHandlers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BracketGeneratedConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("eventide");
            h.Password("eventide_pass");
        });
        
        cfg.ReceiveEndpoint("bracket-generated-queue", e =>
        {
            e.ConfigureConsumer<BracketGeneratedConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();