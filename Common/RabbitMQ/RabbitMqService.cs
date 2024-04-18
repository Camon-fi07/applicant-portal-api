using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.RabbitMQ
{
    public class RabbitMqService:IRabbitMqService
    {
        public void SendMessage(object obj, string routingKey, string queueName)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message, routingKey, queueName);
        }

        public void SendMessage(string message, string routingKey, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                               routingKey: routingKey,
                               basicProperties: null,
                               body: body);

                channel.Close();
                connection.Close();
            }
        }
    }
}
