using Blanco_BankAPI;
using Blanco_BankAPI.Consumers;
using Blanco_BankAPI.Database;
using Blanco_BankAPI.Service;
using MassTransit;
using Microsoft.EntityFrameworkCore;

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
    x.AddConsumer<GetBalanceConsumer>();
    x.AddConsumer<CreateBalanceConsumer>();

    //x.SetRabbitMqReplyToRequestClientFactory(); // a creuser, potentiellement utilsier ça pour obtenir le replyTo et répondre directement sans passer par le Helper

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("user");
            h.Password("password");
        });

        cfg.UseRawJsonSerializer();

        cfg.ReceiveEndpoint("balance_queue", e =>
        {
            e.ClearSerialization();
            e.UseRawJsonSerializer();
            e.ConfigureConsumer<GetBalanceConsumer>(context);
            e.ConfigureConsumer<CreateBalanceConsumer>(context);
        });

    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();