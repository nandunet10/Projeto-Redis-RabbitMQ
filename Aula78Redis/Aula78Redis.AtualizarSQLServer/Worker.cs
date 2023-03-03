using Aula78Redis.DAL;
using Aula78Redis.Models;
using Aula78Redis.RabbitMQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Aula78Redis.AtualizarSQLServer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQFactory _factory;

        public Worker(ILogger<Worker> logger, RabbitMQFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var canal = _factory.GetChannel();

                BasicGetResult retorno = canal.BasicGet("Atualiza SQL SERVER", false);
                if (retorno != null)
                {
                    try
                    {
                        var dados = JsonConvert.DeserializeObject<CepDTO>(Encoding.UTF8.GetString(retorno.Body.ToArray()));
                        if (dados != null)
                        {
                            //validando Conexão com o banco
                            var retornoSql = await new CepDAL().ObterCEPBancoDeDadosSQl(dados.CEP);
                            if (retornoSql != null)
                            {
                                //metodo de persistir no Redis
                                await new CepDAL().AlterarCEPBancoDeDadosSql(dados);
                            }
                            else
                            {
                                _logger.LogWarning($"Banco SQL SERVER está fora! ou aconteceu algum problema entre a comunicação.");
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
    }
}