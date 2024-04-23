using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Abstracts.Options;
using Sample.ChatHub.Domain.Contracts;
using Sample.ChatHub.Domain.Contracts.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Infrastructure;
using Sample.ChatHub.Server.API.Protos;
using Sample.ChatHub.Worker.API;
using Sample.ChatHub.Worker.API.Consumers;
using Sample.ChatHub.Worker.API.Services;
using Sample.ChatHub.Worker.Core.Chat.Projections;

var builder = WebApplication.CreateBuilder(args);
BusOptions? busOptions = builder.Configuration.GetSection(BusOptions.Key).Get<BusOptions>();

var connectionFactory = new ConnectionFactory();
connectionFactory.Password = busOptions?.Password;
connectionFactory.UserName = busOptions?.UserName; 
connectionFactory.HostName = busOptions?.Host;
connectionFactory.VirtualHost = busOptions?.VirtualHost;


builder.Services.AddGrpc();

builder.Services.AddGrpcClient<UserSync.UserSyncClient>(e =>
{
    e.Address = new(builder.Configuration["UrlGrpcServer"]!);

    e.ChannelOptionsActions.Add((opt) =>
    {
        opt.UnsafeUseInsecureChannelCallCredentials = true;
    });
})
.AddCallCredentials((context, metadata) =>
{
    metadata.Add("Authorization", builder.Configuration["AuthGrpcServer"]!);
    return Task.CompletedTask;
});

builder.Services.AddScoped<SyncMessageService>();

builder.Services.AddScoped<IChatProcessStream, ChatProcessStream>();
builder.Services.AddScoped<IMessageProcessStream, MessageProcessStream>();


builder.Services.AddScoped<IChatDecoratorProjection, DefauftProjection>()
    .Decorate<IChatDecoratorProjection, ChatMembersProjection>();

builder.Services.ConfigureInfrastructure();

builder.Services.AddOptions();
builder.Services.Configure<BusOptions>(builder.Configuration.GetSection(BusOptions.Key));
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection(MongoOptions.Key));
builder.Services.AddBus(connectionFactory);

builder.Services.AddConsumer<CreateChatHandlerConsumer, CreateChat>(e => e.ExchangeName = "create-chat-consumer");
builder.Services.AddConsumer<UserJoinChatHandlerConsummer, UserJoinChat>(e => e.ExchangeName = "user-joinChat-consumer");
builder.Services.AddConsumer<MessageReceivedHandlerConsumer, MessageReceived>(e => e.ExchangeName = "message-received-consumer");
builder.Services.AddConsumer<SyncMessageHandler, SyncUserMessage>(e => e.ExchangeName = "sync-message-consumer");
builder.Services.AddConsumer<SendMessageHandlerConsumer, SendMessage>(e => e.ExchangeName = "send-message-consumer");

var app = builder.Build();

app.MapGrpcService<UserInfoProtoHandler>();

app.Run();
