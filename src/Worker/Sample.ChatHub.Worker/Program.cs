using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Abstracts.Options;
using Sample.ChatHub.Infrastructure;
using Sample.ChatHub.Server.API.Protos;
using Sample.ChatHub.Worker;
using Sample.ChatHub.Worker.Consumers;
using Sample.ChatHub.Worker.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();  
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true)
               .Build();

        var connectionFactory = new ConnectionFactory();
        connectionFactory.Password = "guest";
        connectionFactory.UserName = "guest";
        connectionFactory.HostName = "localhost";
        

        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(appConfig =>
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .Build();
                appConfig.AddConfiguration(configuration);
            })                       
            .ConfigureServices((hostContext, services) =>
            {
                services.AddGrpcClient<UserSync.UserSyncClient>(e =>
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

                services.AddScoped<SyncMessageService>();

                services.AddScoped<IChatProcessStream,ChatProcessStream>();
                services.AddScoped<IMessageProcessStream, MessageProcessStream>();
                services.ConfigureInfrastructure();               
                               
                services.AddOptions();
                services.Configure<BusOptions>(hostContext.Configuration.GetSection(BusOptions.Key));
                services.Configure<MongoOptions>(hostContext.Configuration.GetSection(MongoOptions.Key));                
                services.AddBus(connectionFactory);

                services.AddHostedService<CreateChatHandlerConsumer>();
                services.AddHostedService<SendMessageHandlerConsumer>();
                services.AddHostedService<UserJoinChatHandlerConsummer>();
                services.AddHostedService<MessageReceivedHandlerConsumer>();
                services.AddHostedService<SyncMessageHandler>();
            });
    }
       
}