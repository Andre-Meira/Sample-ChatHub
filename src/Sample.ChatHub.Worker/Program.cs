using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.ChatHub.Domain.Abstracts.Options;

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
                services.AddOptions();
                services.Configure<BusOptions>(hostContext.Configuration.GetSection(BusOptions.Key));
                services.Configure<MongoOptions>(hostContext.Configuration.GetSection(MongoOptions.Key));                
           });
    }
       
}