using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Abstracts.Options;
using Sample.ChatHub.Infrastructure;
using Sample.ChatHub.Worker;
using Sample.ChatHub.Worker.Consumers;

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
                services.AddScoped<IChatProcessStream,ChatProcessStream>();
                services.ConfigureInfrastructure();               
                               
                services.AddOptions();
                services.Configure<BusOptions>(hostContext.Configuration.GetSection(BusOptions.Key));
                services.Configure<MongoOptions>(hostContext.Configuration.GetSection(MongoOptions.Key));                
                services.AddBus(connectionFactory);

                services.AddHostedService<CreateChatHandlerConsumer>();
                services.AddHostedService<SendMessageHandlerConsumer>();
                services.AddHostedService<UserJoinChatHandlerConsummer>();
           });
    }
       
}