using Aula78Redis.AtualizarRedis;
using Aula78Redis.Models;
using Aula78Redis.RabbitMQ;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();

        services.AddStackExchangeRedisCache(opt =>
        {
            opt.InstanceName = context.Configuration.GetSection("REDIS:InstanceName").Value;
            opt.Configuration = context.Configuration.GetSection("REDIS:URL").Value;
        });

        #region RabbitMQ
        services.Configure<DadosBaseRabbitMQ>(context.Configuration.GetSection("DadosBaseRabbitMQ"));
        services.AddSingleton<RabbitMQFactory>();
        #endregion
    })
    .Build();

await host.RunAsync();
