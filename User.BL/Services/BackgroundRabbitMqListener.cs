using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Hosting;
using Common.RabbitMQ.RoutingKeys;
using User.BL.Services.IServices;
using Newtonsoft.Json;
using Common.Models;
using Microsoft.Extensions.DependencyInjection;

namespace User.BL.Services.RabbitMQ
{
    public class BackgroundRabbitMqListener : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundRabbitMqListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "user", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            using (var scope = _serviceProvider.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (ch, ea) =>
                {
                    var content = JsonConvert.DeserializeObject<UserRegisterModel>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                    var type = ea.RoutingKey;

                   /* switch (type)
                    {
                        case UserRoutingKey.RegisterUser:
                            var tokenResponse = await authService.RegisterUser(content);
                            var user = await tokenService.GetUserByToken(tokenResponse.AccessToken);
                            _channel.BasicPublish(exchange: "",
                                   routingKey: ApplicantRoutingKey.RegisterApplicant,
                                   basicProperties: null,
                                   body: Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(tokenResponse)));
                            break;
                    }*/

                    _channel.BasicAck(ea.DeliveryTag, false);
                };

                _channel.BasicConsume("user", false, consumer);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
