using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Domain.Abstracts.Options;
using Sample.ChatHub.Server.API;
using Sample.ChatHub.Server.API.Services;
using Sample.ChatHub.Woker.API.Protos;

var builder = WebApplication.CreateBuilder(args);

BusOptions? busOptions = builder.Configuration.GetSection(BusOptions.Key).Get<BusOptions>();

var connectionFactory = new ConnectionFactory();
connectionFactory.Password = busOptions?.Password;
connectionFactory.UserName = busOptions?.UserName;
connectionFactory.HostName = busOptions?.Host;
connectionFactory.VirtualHost = busOptions?.VirtualHost;

builder.Services.AddDistributedRedisCache(options =>
           {
               options.Configuration = builder.Configuration["ConnectionStringsRedis"]!;
               options.InstanceName = "";
           });

builder.Services.AddGrpcClient<UserInfo.UserInfoClient>(e =>
{
    e.Address = new(builder.Configuration["WokerGrpc"]!);

    e.ChannelOptionsActions.Add((opt) =>
    {
        opt.UnsafeUseInsecureChannelCallCredentials = true;
    });
});

builder.Services.AddOptions();
builder.Services.Configure<UserSettings>(builder.Configuration.GetSection("Users"));

builder.Services.AddGrpc();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(BasicAuthenticationHandler.Schema, new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = BasicAuthenticationHandler.Schema,
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = BasicAuthenticationHandler.Schema
                }
            },
            new string[] { BasicAuthenticationHandler.Schema }
        }
    });
});


builder.Services.AddAuthentication(config =>
{
    config.DefaultScheme = BasicAuthenticationHandler.Schema;
    config.DefaultAuthenticateScheme = BasicAuthenticationHandler.Schema;
})
.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.Schema, null);

builder.Services.AddAuthorization();
builder.Services.AddBus(connectionFactory);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapGrpcService<SyncMessageProtoHandler>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHubServer>("/chatHub").RequireAuthorization();

app.Run();
