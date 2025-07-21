using System.Text.Json;
using Blanco_BankAPI;
using Blanco_BankAPI.Consumers;
using Blanco_BankAPI.Database;
using Blanco_BankAPI.Service;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlancoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
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

        context.Database.EnsureCreated();

    }
    catch (Exception ex)
    {
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