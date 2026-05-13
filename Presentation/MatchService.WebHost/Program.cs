using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MatchService.Infrastructure.EntityFramework;
using MatchService.WebHost.Heplers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString(nameof(ApplicationDbContext));

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string for MatchServiceDbContext is not configured.");
}

builder.Services.AddNpgsql<ApplicationDbContext>(connectionString, options =>
{
    options.MigrationsAssembly("MatchService.Infrastructure.EntityFramework");

});

builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Match service API",
            Description = "API for creating, viewing, storing, modifying, and deleting matches."
        });
    });

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseNpgsql(connectionString);
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.MigrateDatabase<ApplicationDbContext>();

app.Run();
