# Kafka + Docker

Para levantar nuestro ambiente creamos un archivo de Docker Compose donde instanciaremos un servicio de _Zookeeper_ y un servicio de _Kafka_ (luego se pueden levantar mas y armar los clusters).

Las imágenes base que vamos a utilizar son las de los amigos de _[Confluence](https://github.com/confluentinc/cp-docker-images)_. No son las únicas, también podemos usar las de _[Bitmani|Zookeeper](https://bitnami.com/stack/zookeeper/containers)_, _[Bitmani|Kafka](https://bitnami.com/stack/kafka/containers)_, _[Spotify](https://hub.docker.com/r/spotify/kafka/)_ o _[Wurstmeister](https://github.com/wurstmeister/kafka-docker)_.

Nuestro docker-compose file nos queda de esta forma:

``` docker-compose
version: '2'
services:
    zookeeper:
        image: confluentinc/cp-zookeeper:latest
        environment:
            ZOOKEEPER_CLIENT_PORT: 2181
            ZOOKEEPER_TICK_TIME: 2000
    kafka:
        image: confluentinc/cp-kafka:latest
        depends_on:
            - zookeeper
        ports:
            - 9092:9092
        environment:
            KAFKA_BROKER_ID: 1
            KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
            KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:29092,PLAINTEXT_HOST://localhost:9092
            KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
            KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
            KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
```

> **Imporatante:** Para acceder a _Kafka_ desde fuera del container se debe reeamplazar `localhost` en _ADVERTISED_ por el IP del Docker-Machine.

Con archivo de docker compose creado, vamos a configurar nuestro ambiente.

Abrimos la consola de docker y creamos una nueva *Docker Machine* ejecutando el siguiente comando

```bash
docker-machine create --driver virtualbox confluent
```

Indicamos que vamos a utilizar el driver de *VirtualBox* para crear la máquina virtual del host y la llamaremos _confluent_ (podemos usar cualquier nombre)

Si usamos Linux o Windows Profesional en adelante, podemos saltarnos esta opción e ir directamente al `docker-compose up`, pero para quienes usamos Windows 10 Home o versiones mas antiguas con Docker Toolbox es un paso que necesitamos hacer para tener una experiencia similar.

Revisamos que se haya creado

```bash
docker-machine ls
```

Lo siguiente a realizar es entrar a la *docker machine* que acabamos de crear para que los comandos se ejecuten sobre ella (sino se haría sobre la default)

```bash
eval $(docker-machine env confluent)
```

Obtenemos el IP para luego poder conectarnos desde *C#* y para usarla en la variable de entorno _KAFKA_ADVERTISED_LISTENERS_ del archivo de docker compose

```bash
docker-machine ip confluent
```

Por último, corremos `docker-compose` en la carpeta donde hayamos puesto el archivo yaml

```bash
docker-compose up -d
```

Verificamos que se encuentren Kafka y Zookeeper levantados

```bash
docker container ps
```

Con esta mínima configuración tenemos *Kafka* listo para las primeras pruebas, let's go! 💪😀

***

## Divirtámonos con Kafka

Por default, **Kafka** viene con scrips para crear topics, publicar/leer mensajes, etc. Durante esta parte, vamos a ver como trabajar con ellos

### Crear un topic nuevo

```bash
docker container exec <kafka-container-id> kafka-topics \
--create \
--topic foo \
--partitions 1 \
--replication-factor 1 \
--if-not-exists \
--zookeeper zookeeper:2181
```

Creamos un nuevo _Topic_ en _Kafka_ con una factor de replicación de 1 y con una sola partición. Si quisiéramos aumentar estos valores debemos levantar otras instancias de Kafka.
El parámetro _--if-not-exists_ indica que el topic se creará si no existe previamente.

### Listar los topics

```bash
docker container exec <kafka-container-id> kafka-topics \
--list \
--zookeeper zookeeper:2181
```

Este comando va a listar únicamente los nombres de los _Topics_ que tenemos creados.

### Ver la descripción de un topic

```bash
docker container exec <kafka-container-id> kafka-topics \
--describe \
--topic foo \
--zookeeper zookeeper:2181
```

Nos muestra información relevante del Topic

### Publicar un mensaje al topic

En este caso es mejor ingresar a la consola del container de Kafka y ejecutar el Producer desde allí

```bash
docker container exec -it <kafka-container-id> /bin/bash
```

Ahora ejecutamos el _Publisher_

```bash
kafka-console-producer \
--request-required-acks 1 \
--broker-list <docker-machine-ip>:9092 \
--topic foo
```

Luego por cada linea que escribamos (separadas por un [Enter]) se enviará un mensaje (usamos [ctrl+c] para salir del comando y exit para salir del container).

### Leer de un topic

```bash
docker container exec <kafka-container-id> kafka-console-consumer \
--bootstrap-server <docker-machine-ip>:9092 \
--topic foo \
--zookeeper  zookeeper:2181
```

Estos son los básicos para comenzar y hacer las primeras pruebas. Cada uno tiene múltiples parámetros a investigar y sacar mayor provecho.
Para un major detalle pueden ir a la docu [oficial](https://kafka.apache.org/documentation/#operations).

# C# conoce Kafka

Ya tenemos _Kafka_ ejecutándose en nuestro PC, ahora veremos como publicar y leer mensajes en un _Topic_.

**Producer**
Creamos una aplicación de consola y usamos el siguiente código reemplazando el _<IP>_ por el que tiene el _Docker-Machine_.

```csharp
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
```

Compilamos y ejecutamos indicando el nombre del topic al cual enviaremos mensajes. La aplicación queda a la espera de que escribamos mensajes uno por línea.

```bash
dotnet build
.
.
.
dotnet run foo

```

**Consumer**
Para el Consumer, creamos una nueva aplicación de consola y usamos el siguiente código reemplazando el valor de _<IP>_ por el IP del _Docker-Machine_ que estamos usando.

```csharp
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
```

Compilamos y corremos indicando que _Topic_ queremos escuchar
```bash
dotnet build
.
.
.
dotnet run foo
```

Ejecutando ambos programas en la consola una al lado de la otra, podremos ver que cuando publicamos un mensaje, éste aparece en la otra.