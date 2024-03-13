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

var connectionFactory = new ConnectionFactory();
connectionFactory.Password = "guest";
connectionFactory.UserName = "guest";
connectionFactory.HostName = "localhost";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddGrpcClient<UserSync.UserSyncClient>(e =>
{
    e.Address = new("http://localhost:5002");
    
    e.ChannelOptionsActions.Add((opt) =>
    {
        opt.UnsafeUseInsecureChannelCallCredentials = true;
    });
})
.AddCallCredentials((context, metadata) =>
{
    metadata.Add("Authorization", $"Basic c2lzdGVtYTphNTE5OTlmZS0zZTg1LTQ3YmItOTRjZS1mMmMyY2Y2YmQ2N2U=");
    return Task.CompletedTask;
});

builder.Services.AddScoped<SyncMessageService>();

builder.Services.AddScoped<IChatProcessStream,ChatProcessStream>();
builder.Services.AddScoped<IMessageProcessStream, MessageProcessStream>();


builder.Services.AddScoped<IChatDecoratorProjection, DefauftProjection>()
    .Decorate<IChatDecoratorProjection, ChatMembersProjection>();

builder.Services.ConfigureInfrastructure();               

builder.Services.AddOptions();
builder.Services.Configure<BusOptions>(builder.Configuration.GetSection(BusOptions.Key));
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection(MongoOptions.Key));                
builder.Services.AddBus(connectionFactory);

builder.Services.AddConsumer<CreateChatHandlerConsumer, CreateChat>(() =>
{
    return new ConsumerOptions("create-chat-consumer", ExchangeType.Direct, prefetchCount: 20);
});

builder.Services.AddConsumer<UserJoinChatHandlerConsummer, UserJoinChat>(() =>
{
    return new ConsumerOptions("user-join-consumer", ExchangeType.Direct, prefetchCount: 20);
});

builder.Services.AddConsumer<MessageReceivedHandlerConsumer, MessageReceived>(() =>
{
    return new ConsumerOptions("message-received-consumer", ExchangeType.Direct, prefetchCount: 20);
});

builder.Services.AddConsumer<SyncMessageHandler, SyncUserMessage>(() =>
{
    return new ConsumerOptions("sync-message-consumer", ExchangeType.Direct, prefetchCount: 20);
});

builder.Services.AddConsumer<SendMessageHandlerConsumer, SendMessage>(() =>
{
    return new ConsumerOptions("send-message-consumer", ExchangeType.Direct, prefetchCount: 20);
});

var app = builder.Build();

app.MapGrpcService<UserInfoProtoHandler>();

app.Run();
