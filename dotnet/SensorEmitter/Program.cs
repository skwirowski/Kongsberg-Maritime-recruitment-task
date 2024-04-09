using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace SensorEmitter;

class Program
{
    public static async Task Main(string[] args) {
        // the client that owns the connection and can be used to create senders and receivers
        ServiceBusClient client;

        // the sender used to publish messages to the topic
        ServiceBusSender sender;

        // reads secrets from secrets.json file
        ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        IConfiguration configuration = configurationBuilder.AddUserSecrets<Program>().Build();
        string namespaceConnectionString = configuration["namespaceConnectionString"];
        string topicName = configuration["topicName"];

        // deserialize sensorConfig.json file
        SensorConfigList sensorConfigList = await DeserializeConfig.ReadAsync();

        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.
        client = new ServiceBusClient(namespaceConnectionString);
        sender = client.CreateSender(topicName);

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

        ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
        Console.WriteLine("Press Escape to stop sending messages to Service Bus");

        while (await timer.WaitForNextTickAsync() && keyInfo.Key != ConsoleKey.Escape)
        {
            // create a batch 
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            foreach (SensorConfig sensorConfig in sensorConfigList.Sensors)
            {
                // try adding a message to the batch
                var serviceBusMessage = new ServiceBusMessage(CreateMessageFromTelegram.CreateMessage(sensorConfig));
                serviceBusMessage.MessageId = sensorConfig.ID.ToString();

                if (!messageBatch.TryAddMessage(serviceBusMessage))
                {
                    // if it is too large for the batch
                    throw new Exception($"The message {sensorConfig.ID} is too large to fit in the batch.");
                }

                Console.WriteLine($"Message #{messageBatch.Count} added to the batch. Content: {serviceBusMessage.Body}");
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus topic
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {sensorConfigList.Sensors.Length} messages has been published to the topic.");
                Console.WriteLine("--------------------------------------------");
            } finally {
                messageBatch.Dispose();

                if (Console.KeyAvailable) {
                    keyInfo = Console.ReadKey(true);
                }  
            }
        }

        // Calling DisposeAsync on client types is required to ensure that network
        // resources and other unmanaged objects are properly cleaned up.
        await sender.DisposeAsync();
        await client.DisposeAsync();
        Console.WriteLine("Dispose and clean up.");

        Console.WriteLine("Press any key to end the application");
        Console.ReadKey();
    }
}

