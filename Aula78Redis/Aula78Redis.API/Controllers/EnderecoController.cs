using Aula78Redis.Models;
using Aula78Redis.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Aula78Redis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnderecoController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRabbitMQNegocio _rabbitMQNegocio;
        public EnderecoController(IDistributedCache distributedCache, IRabbitMQNegocio rabbitMQNegocio)
        {
            _distributedCache = distributedCache;
            _rabbitMQNegocio = rabbitMQNegocio;
        }
        [HttpGet]
        public async Task<CepDTO> GetEnderecoPorCEPRedis(string cep)
        {
            for (int i = 0; i < 3; i++)
            {
                var dadosRedis = await _distributedCache.GetStringAsync(cep);
                if (!string.IsNullOrWhiteSpace(dadosRedis))
                {
                    return JsonConvert.DeserializeObject<CepDTO>(dadosRedis);
                };

                await Task.Delay(5000);
            }

            throw new Exception("Cep não encontrado, procure o time da TI!");
        }

        [HttpPut]
        public void AtualizarEndereco(CepDTO cep)
        {
            _rabbitMQNegocio.PublicarMensagem(cep, "atualizar.bancos", "");
            //await new CepDAL().AlterarCEPBancoDeDadosSql(cep);
            //await _distributedCache.SetStringAsync(cep.CEP, JsonConvert.SerializeObject(cep));
        }
    }
}
