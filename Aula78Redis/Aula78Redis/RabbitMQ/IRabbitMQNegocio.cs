
namespace Aula78Redis.RabbitMQ
{
    public interface IRabbitMQNegocio
    {
        void PublicarMensagem(object obj, string exchange = "", string fila = "");
    }
}
