using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using System.Text;

namespace IQBusinessAssessment.RabbitMQ.Producer.Test
{
    [TestFixture]
    public class SendMessageTest
    {
        [Test]
        public void SendToExchangeWithBoundQueue()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            ConfigureQueueBinding(rabbitServer, "RabbitMQ-exchange", "RabbitMQ-queue");

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                const string message = "I love IQ Business!";
                var messageBody = Encoding.ASCII.GetBytes(message);
                channel.BasicPublish(exchange: "RabbitMQ-queue", routingKey: null, mandatory: false, basicProperties: null, body: messageBody);
            }

            Assert.That(rabbitServer.Queues["RabbitMQ-queue"].Messages.Count, Is.EqualTo(1));
        }

        private void ConfigureQueueBinding(RabbitServer rabbitServer, string exchangeName, string queueName)
        {
            var connectionFactory = new FakeConnectionFactory(rabbitServer);
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

                channel.QueueBind(queueName, exchangeName, null);
            }
        }
    }
}
