using Aula78Redis.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Aula78Redis.RabbitMQ
{
    public class RabbitMQFactory
    {
        private IModel _channel;
        private readonly IConnection _connection;

        public RabbitMQFactory(IOptions<DadosBaseRabbitMQ> dadosBaseRabbitMQ)
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = dadosBaseRabbitMQ.Value.HostName,
                Port = dadosBaseRabbitMQ.Value.Port,
                UserName = dadosBaseRabbitMQ.Value.UserName,
                Password = dadosBaseRabbitMQ.Value.Password,

            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public IModel GetChannel()
        {
            if (!_channel.IsOpen)
            {
                _channel.Dispose();
                _channel = _connection.CreateModel();
            }
            return _channel;
        }
    }
}
