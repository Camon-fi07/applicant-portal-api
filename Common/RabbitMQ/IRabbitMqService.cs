using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RabbitMQ
{
    public interface IRabbitMqService
    {
        void SendMessage(object obj, string routingKey, string queueName);
        void SendMessage(string message, string routingKey, string queueName);
    }
}
