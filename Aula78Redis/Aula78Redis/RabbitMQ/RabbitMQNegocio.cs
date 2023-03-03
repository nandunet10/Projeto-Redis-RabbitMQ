using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Aula78Redis.RabbitMQ
{
    public class RabbitMQNegocio : IRabbitMQNegocio
    {
        private readonly RabbitMQFactory _rabbitMQFactory;
        public RabbitMQNegocio(RabbitMQFactory rabbitMQFactory)
        {
            _rabbitMQFactory = rabbitMQFactory;
        }

        public void PublicarMensagem(object obj, string exchange, string fila)
        {
            var canal = _rabbitMQFactory.GetChannel();
            IBasicProperties iBasicProperties = canal.CreateBasicProperties();

            var corpoMensagem = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

            canal.BasicPublish(exchange: exchange, routingKey: fila, basicProperties: iBasicProperties, body: corpoMensagem);
        }
    }
}
