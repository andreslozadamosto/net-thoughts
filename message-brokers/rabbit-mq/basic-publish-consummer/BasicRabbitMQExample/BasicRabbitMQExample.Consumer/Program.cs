using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace BasicRabbitMQExample.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() {
                HostName = "192.168.99.100"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            int acc = 0;

            consumer.Received += (model, ea) =>
            {
                acc++;
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                if ((new Random()).Next() % 2 == 0)
                {
                    Console.WriteLine($" [{acc}][{ea.ConsumerTag}|-|{ea.DeliveryTag}|-|{ea.BasicProperties.CorrelationId}][x] Rejected message");
                    channel.BasicReject(ea.DeliveryTag, true);
                }
                else
                {
                    Console.WriteLine($" [{acc}][{ea.ConsumerTag}|-|{ea.DeliveryTag}|-|{ea.BasicProperties.CorrelationId}][.] Received {message}");
                    channel.BasicAck(ea.DeliveryTag, true);
                }
            };
            channel.BasicConsume(queue: "hello",
                                    autoAck: false,
                                    consumer: consumer);

            Console.WriteLine("Waiting messages...");
            Console.ReadLine();

            channel.Close();
            connection.Close();

        }
    }
}
