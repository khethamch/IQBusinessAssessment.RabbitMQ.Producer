using RabbitMQ.Client;
using System;

namespace IQBusinessAssessment.RabbitMQ.Producer.Factory
{
    public class RabbitMQConnectionFactory : IRabbitMQConnectionFactory
    {
        public ConnectionFactory GetConnectionFactory()
        {
            return new ConnectionFactory()
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
        }
    }
}
