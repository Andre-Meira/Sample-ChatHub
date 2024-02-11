using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Server.API;

var connectionFactory = new ConnectionFactory();
connectionFactory.Password = "guest";
connectionFactory.UserName = "guest";
connectionFactory.HostName = "localhost";    

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.Configure<UserSettings>(builder.Configuration.GetSection("Users"));


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

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHubServer>("/chatHub").RequireAuthorization();

app.Run();
