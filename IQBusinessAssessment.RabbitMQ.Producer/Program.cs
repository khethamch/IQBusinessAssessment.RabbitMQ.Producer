using IQBusinessAssessment.RabbitMQ.Producer.Factory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace IQBusinessAssessment.RabbitMQ.Producer
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            RegisterServices();
            IServiceScope scope = _serviceProvider.CreateScope();
            var factory = scope.ServiceProvider.GetService<IRabbitMQConnectionFactory>();
            ConnectionFactory connectionFactory = factory.GetConnectionFactory();

            string name = string.Empty;
            Console.WriteLine("Please enter your name:");
            name = Console.ReadLine();
            SendMessage(connectionFactory, name);
            Console.ReadLine();

            DisposeServices();
        }

        private static void SendMessage(ConnectionFactory factory, string name)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("RabbitMQ-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

                var message = new { Name = name, Message = $"Hello my name is, {name}" };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish("", "RabbitMQ-queue", null, body);
            }
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
