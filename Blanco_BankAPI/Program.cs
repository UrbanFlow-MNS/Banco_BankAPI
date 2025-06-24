using System.Reflection;
using System.Text;
using System.Text.Json;
using Blanco_BankAPI;
using Blanco_BankAPI.Database;
using Blanco_BankAPI.DTO;
using Blanco_BankAPI.Service;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlancoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Personal services Dependancy Injection
builder.Services.AddScoped<BlancoDbContext>();
builder.Services.AddScoped<IAccountService, AccountService>();


// CONFIG RABBITMQ - MASSTRANSIT
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

        cfg.UseRawJsonSerializer(); // NE PAS SUPPRIMER

        cfg.ReceiveEndpoint("balance_queue", e =>
        {
            e.ClearSerialization();   // NE PAS SUPPRIMER
            e.UseRawJsonSerializer(); // NE PAS SUPPRIMER
            e.ConfigureConsumer<BalanceConsumer>(context);
            e.PurgeOnStartup = false;
        });
    });
});

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<BlancoDbContext>();

//    context.Database.Migrate();
//}

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