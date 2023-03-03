using Aula78Redis.Models;
using Aula78Redis.RabbitMQ;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Aula78Redis.AtualizarRedis
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQFactory _factory;
        private readonly IDistributedCache _distributedCache;

        public Worker(ILogger<Worker> logger, RabbitMQFactory factory, IDistributedCache distributedCache)
        {
            _logger = logger;
            _factory = factory;
            _distributedCache = distributedCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var canal = _factory.GetChannel();

                BasicGetResult retorno = canal.BasicGet("Atualiza Redis", false);
                if (retorno != null)
                {
                    try
                    {
                        var dados = JsonConvert.DeserializeObject<CepDTO>(Encoding.UTF8.GetString(retorno.Body.ToArray()));
                        if (dados != null)
                        {
                            //validando Redis
                            var retornoRedis = await _distributedCache.GetStringAsync(dados.CEP);
                            if (!string.IsNullOrWhiteSpace(retornoRedis))
                            {
                                //metodo de persistir no Redis
                                await AdicionarDadosRedis(dados);
                            }
                            else
                            {
                                _logger.LogWarning($"Banco Redis está fora! ou aconteceu algum problema entre a comunicação.");
                                canal.BasicNack(retorno.DeliveryTag, false, true);
                            }

                            _logger.LogInformation(dados.ToString());

                            canal.BasicAck(retorno.DeliveryTag, true);
                        }
                        else
                        {
                            _logger.LogWarning($"Não existe mensagem na fila.");
                            canal.BasicNack(retorno.DeliveryTag, false, true);

                        }
                    }
                    catch
                    {
                        _logger.LogError("Não foi possível realizar a operação");
                        canal.BasicNack(retorno.DeliveryTag, false, true);
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public async Task AdicionarDadosRedis(CepDTO cep)
        {
            var memoryCacheEntryPoints = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            };

            await _distributedCache.SetStringAsync(cep.CEP, JsonConvert.SerializeObject(cep), memoryCacheEntryPoints);
        }
    }
}