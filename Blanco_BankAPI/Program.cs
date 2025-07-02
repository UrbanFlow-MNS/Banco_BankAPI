using System;
using Blanco_BankAPI;
using Blanco_BankAPI.Consumers;
using Blanco_BankAPI.Database;
using Blanco_BankAPI.Service;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings__DefaultConnection");
Console.WriteLine($"Using connection string: {connectionString}");

builder.Services.AddDbContext<BlancoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(connectionString)));

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
    //x.AddConsumer<CreateBalanceConsumer>();

    //x.SetRabbitMqReplyToRequestClientFactory(); // a creuser, potentiellement utilsier ça pour obtenir le replyTo et répondre directement sans passer par le Helper

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("user");
            h.Password("password");
        });

        cfg.UseRawJsonSerializer();

        cfg.ReceiveEndpoint("balance_queue", e =>
        {
            e.ClearSerialization();
            e.UseRawJsonSerializer();
            e.ConfigureConsumer<BalanceConsumer>(context);
            //e.ConfigureConsumer<CreateBalanceConsumer>(context);
        });

    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BlancoDbContext>();

        Console.WriteLine("Connecting to database...");
        await context.Database.CanConnectAsync();
        Console.WriteLine("Database connection successful!");

        // Appliquer les migrations
        Console.WriteLine("Applying database migrations...");
        await context.Database.MigrateAsync();
        Console.WriteLine("Database migrations applied successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while setting up the database: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw; 
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();