using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var topic = args[0] ?? "foo";

            // Set the Producer config
            var config = new ProducerConfig
            {
                // <docker-machine-ip>>:<kafka-port>
                BootstrapServers = "192.168.99.101:9092",
            };

            // Producer builder with our config
            using var producer = new ProducerBuilder<Null, string>(config)
                // Error handler
                .SetErrorHandler((_, e) =>
                {
                    Console.WriteLine($"Kafka Error {e.Code}: {e.Reason}");
                })
                .Build();

            try
            {
                do 
                {
                    var msj = Console.ReadLine();

                    var dr = await producer.ProduceAsync(
                        topic: topic, // Topic name previouslly createad
                        message: new Message<Null, string> { Value = msj } // Message we want send
                        );
                    
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}' with status {dr.Status.ToString()}");
                }
                while(true);
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
            catch (Exception ex) {
                Console.WriteLine($"Delivery failed: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
