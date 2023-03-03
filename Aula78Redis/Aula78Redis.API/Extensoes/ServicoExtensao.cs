using Aula78Redis.Models;
using Aula78Redis.RabbitMQ;

namespace Aula78Redis.API.Extensoes
{
    public static class ServicoExtensao
    {
        public static void ConfigurarServicos(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRabbitMQNegocio, RabbitMQNegocio>();

            services.Configure<DadosBaseRabbitMQ>(configuration.GetSection("DadosBaseRabbitMQ"));
            services.AddSingleton<RabbitMQFactory>();
        }
    }
}
