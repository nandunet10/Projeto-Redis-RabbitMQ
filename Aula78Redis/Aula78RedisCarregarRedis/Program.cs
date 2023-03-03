using Aula78RedisCarregarRedis;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();

        services.AddStackExchangeRedisCache(opt =>
        {
            opt.InstanceName = context.Configuration.GetSection("REDIS:InstanceName").Value;
            opt.Configuration = context.Configuration.GetSection("REDIS:URL").Value;
        });
    })
    .Build();

await host.RunAsync();
