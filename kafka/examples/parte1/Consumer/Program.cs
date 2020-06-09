using Confluent.Kafka;
using System;
using System.Threading;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var topic = args[0] ?? "foo";

            // Set the Consumer config
            var conf = new ConsumerConfig
            { 
                // Name of the consumer group
                GroupId = "test-consumer-group",
                // <docker-machine-ip>>:<kafka-port>
                BootstrapServers = "192.168.99.101:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,

            };

            using var consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
            consumer.Subscribe(topic);

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };
            
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(cts.Token);
                    Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
                catch (OperationCanceledException) 
                {
                    Console.WriteLine($"[Ctrl + C] typed");
                    consumer.Close();
                    break;
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Unhandled Exception: {ex.Message}");
                    consumer.Close();
                    break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}