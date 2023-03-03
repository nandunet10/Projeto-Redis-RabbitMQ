using Aula78Redis.API.Extensoes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigurarServicos(builder.Configuration);

builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.InstanceName = builder.Configuration.GetSection("REDIS:InstanceName").Value;
    opt.Configuration = builder.Configuration.GetSection("REDIS:URL").Value;
});

builder.Services.AddMemoryCache();

//builder.Services.Configure<DadosBaseRabbitMQ>(builder.Configuration.GetSection("DadosBaseRabbitMQ"));
//builder.Services.AddScoped<RabbitMQFactory>();
//builder.Services.AddScoped<IRabbitMQNegocio, RabbitMQNegocio>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();
