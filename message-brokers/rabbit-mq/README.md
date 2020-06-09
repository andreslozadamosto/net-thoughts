# RabbitMQ

## Message Broker

Antes de comenzar a conocer a **RabbitMQ** debemos estar al tanto de que es un _**Message Broker**_ y de que viene todo este lío de ida y vuelta de mensajes.

Un Message Broker es un software que sirve de intemediario entre un componente que desea publicicar un mensaje/evento/informacion (puede ser una acción que ya sucedió, una orden de ejecución de cierta tarea, etc) hacia el exterior del mismo (a.k.a. _Publisher_) y otros componentes interesados en recibir dicha notificación(a.k.a. _Consumer_).

Este componente al recibir un mensaje de un _Publisher_ lo coloca en una cola (a.k.a. _Queue_) desde la cual los _Consumers_ pueden registrarse para "escuchar" los mensajes que llegan, tomar o descartarlos y realizar una acción como respuesta del mismo.

A nivel de arquitectura de softare, un __Message Broker__ es un _Middleware_ [[+info](https://es.wikipedia.org/wiki/Middleware)] del tipo _MOM (Message-oriented middleware)_ [[+info](https://en.wikipedia.org/wiki/Message-oriented_middleware)].

Un **Message Broker** puede proveer funcionalidades como:

* Routing inteligente de mensajes hacia uno y/o multiples destinatarios o bien por medio de topics (usando el patron publish-subscriber)
* Transformacion del formato del mensaje
* Message aggregation
* Interactuar con repositorios externos para extender el mensaje o guardarlo
* Confirmación de recepción del mensaje al Publisher
* Transacciones
* Multiples protocolos de mensajeria
* Llamadas RPC

Entre otras funcionalidades que pueden incorporar de forma particular cada **Message Broker** del mercado.

Resumiendo y hablando en castellano básico, un **Message Broker** permite que 2 sistemas/componentes separados puedan interactuar sin que se conozcan. Será él, el encargado de que la notificación generada por el _Publisher_ llege al _Consumer_ interesado.

### ¿Cuándo esta bueno utilizar un Message Broker?

Hay algunos casos principales:

* Desacoplar los componentes de un sistema complejo y/o de alto tráfico
* Integración de sistemas
* Solicitud de ejecución de tareas en un orden

Veamos ciertos casos de uso:

__Microservicios__

Hay muchos métodos de interacción entre Microservicios, el más conocido es por medio de llamadas HTTP a APIs (POST/GET/PUT/etc) pero no es el único.

Podemos utilizar como intermediario un __Message Broker__ a la que un _Publisher_ va populando la Queue con mensajes y luego un _Publisher_ va consumiendo los mensajes entrantes y ejecutando una acción.

Un ejemplo bien simple es el envío de emails. Tenemos una aplicación que debe enviar un email y un servicio que lo único que hace es la acción de enviar emails. La aplicación publica un mensaje en la Queue y el servicio de emails va leyendo a medida que tiene capacidad.
De esta forma estamos desacoplando ambos servicios y reduciendo complejidad. Por otro lado, si el servicio de emails se cae o esta inactivo no repercute en la aplicación ya que ésta continuará enviando mensajes a la Queue de emails que serán atendidos cuando el servicio vuelva a estar online.

__Integración de Sistemas__

La utilización de mensajes es uno de los patrones posibles para integración de sistemas mensionado en el muy buen libro [Enterprise Integration Patterns](https://www.enterpriseintegrationpatterns.com/)

Supongamos que tenemos 2 sistemas totalmente independientes, es más uno fue desarrollado in-house y el otro heredado de la adquisición de un competidor. 
Uno es nuestro sistema de e-commerce que en cada venta de producto se notifique al almacén y se proceda a su empaque y distribución; el otro, es el sistema del almacén que controla el inventario y la órdenes de envío.
Una manera de hacerlo es actualizar el e-commerce (_Publisher_) para que publique notificaciones en una Queue/Message Broker y actualizar el sistema de almacén (_Consumer_) para que reaccione ante los mensajes entrantes de la Queue.

Otro ejemplo de integración, es cuando un evento sucede y éste debe ser tratado por 1, 2, 4 o 10 sistemas independientes de acuerdo a su criticidad. El sistema en que sucede el evento publica en el Message Broker un mensaje y luego éste, por medio de reglas, puede enviar el mensaje a las Queues que sean necesarias y luego cada sistema estará escuchando la Queue que le es de interés. De esta manera evitamos agregar complejidad de integración y que un sistema conozca el API de muchos otros.

__IOT (Internet of Things)__

Cuando estamos en un proyecto IOT se reciben millones de datos simultáneamente de muchísimos dispositivos lo que puede sobrecargar y hacer caer el sistema. Utilizando un Message Broker permite que manejar toda esta demanda y asegurarnos que todos los datos son procesados, principalmente porque estos datos por lo general no requiren de una respuesta inmediata por lo que pueden permanecer en la Queue del Message Broker hasta que el sistema pueda manejarlo.

__Escalabilidad__

Si se tiene un API que recibe cientos de request por segundo, es buena idea separar el tratamiento entrante del request con la tarea a realizar. Esto se puede hacer por medio de un Message Broker publicando un mensaje en la Queue y el Worker la tomará. Esto permite que los request no ocupen recursos y el API pueda procesar mayor demanda de incomming requests. Si es necesario agrandar la capacidad, se pueden agregar mayor cantidad de Workers que escuchen la Queue aumentando así el paralelismo.

__Logging y Activity Tracking__

Si continuamente estamos guardando Logs de todo lo que hace nuestra aplicación a una una Base de datos, tiene un costo en cuanto a performance algo (debdio a la latencia del I/O). Una forma de minimizar esto, es enviando los logs a una Message Broker y que un _Consumer_ pr detras realice el trabajo de guardado en la BD.

Lo mismo ocurre si hacemos un web tracking de un usuario en una web o el movimiento de una persona usando el GPS (como en algunas aplicaciones de runners).

### Opciones de Message Brokers

__Open Source__

* RabbitMQ
* Kafka
* ActiveMQ
* Redis
* Kestrel

__Cloud__ (hay muchas que llevan las opensource a la nube)

* Azure Event Hub
* Amazon MQ

__Empresariales__ (o mejor dicho con una empresa grande por detras, pero que muchos utilizan como base alguna opcion opensource)

* IBM Integration Bus / IBM MQ / WebSphere Message Broker
* Microsoft BizTalk Server
* JMS
* SAP PI
* Oracle WebLogic Server
* TIBCO
* Mulesoft Anypoint MQ
* Red Hat AMQ

## ¿Qué es RabbitMQ?

Proximamente