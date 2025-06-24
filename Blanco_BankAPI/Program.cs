using System.Reflection;
using System.Text;
using System.Text.Json;
using Blanco_BankAPI;
using Blanco_BankAPI.Database;
using Blanco_BankAPI.DTO;
using Blanco_BankAPI.Service;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<BlancoDbContext>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BalanceConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {


        cfg.Host("localhost", "/", h =>
        {
            h.Username("user");
            h.Password("password");
        });

        // Configuration globale pour les messages bruts
        cfg.UseRawJsonSerializer();

        cfg.ReceiveEndpoint("balance_queue", e =>
        {
            e.ClearSerialization();
            e.UseRawJsonSerializer();
            e.ConfigureConsumer<BalanceConsumer>(context);
            e.PurgeOnStartup = false;
        });
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

// Logs pour vérifier que MassTransit démarre
Console.WriteLine("=== Démarrage de l'application ===");
Console.WriteLine("MassTransit va démarrer...");

app.Run();