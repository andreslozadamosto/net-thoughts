using RabbitMQ.Client;
using System;
using System.Text;

namespace BasicRabbitMQExample.Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() {
                HostName = "192.168.99.100",
                //Uri = "amqp://andres:andres@192.168.99.100:5672/andres";
            };

            using var connection = factory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                channel.ConfirmSelect();

                var props = channel.CreateBasicProperties();
                props.CorrelationId = Guid.NewGuid().ToString("N");

                string message = args[0];
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: props,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", message);
            }

            //Console.WriteLine(" Press [enter] to exit.");
            //Console.ReadLine();
        }
    }
}
