using Aula78Redis.AtualizarSQLServer;
using Aula78Redis.Models;
using Aula78Redis.RabbitMQ;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();

        #region RabbitMQ
        services.Configure<DadosBaseRabbitMQ>(context.Configuration.GetSection("DadosBaseRabbitMQ"));
        services.AddSingleton<RabbitMQFactory>();
        #endregion
    })
    .Build();

await host.RunAsync();
