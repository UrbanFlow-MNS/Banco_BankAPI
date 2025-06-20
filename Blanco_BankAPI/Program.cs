using System.Reflection;
using System.Text;
using Blanco_BankAPI;
using Blanco_BankAPI.Database;
using Blanco_BankAPI.Service;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<BlancoDbContext>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddMassTransit(x =>
{
    // Enregistrer le consumer
    x.AddConsumer<BalanceConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureJsonSerializerOptions(option =>
        {
            return option;
        });

        cfg.Host("localhost", "/", h =>
        {
            h.Username("user");
            h.Password("password");
        });

        // IMPORTANT: Configurer l'endpoint pour votre consumer
        cfg.ReceiveEndpoint("BalanceConsumer", e =>
        {


            e.ConfigureConsumer<BalanceConsumer>(context);

            // Optionnel: Configuration supplémentaire
            e.PrefetchCount = 16;
            e.UseConcurrencyLimit(1);
        });

        // Alternative: Configuration automatique basée sur le nom du consumer
        // cfg.ConfigureEndpoints(context);
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

