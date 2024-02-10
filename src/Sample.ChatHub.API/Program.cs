using RabbitMQ.Client;
using Sample.ChatHub.Bus;

var connectionFactory = new ConnectionFactory();
connectionFactory.Password = "guest";
connectionFactory.UserName = "guest";
connectionFactory.HostName = "localhost";
        

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions();

builder.Services.AddBus(connectionFactory);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
