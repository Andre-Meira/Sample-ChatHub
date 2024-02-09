using Sample.ChatHub.Domain.Abstracts.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions();
builder.Services.Configure<BusOptions>(builder.Configuration.GetSection(BusOptions.Key));
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection(MongoOptions.Key));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
