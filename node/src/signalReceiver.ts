import {
    delay,
    ServiceBusClient,
    ServiceBusMessage,
    isServiceBusError,
    ProcessErrorArgs,
} from "@azure/service-bus";
import { parseTelegram } from "./utils/parseTelegram";

export const receiveSignal = (subscriptionName: string) => {
    const connectionString = process.env.CONNECTION_STRING || "<CONNECTION_STRING>";
    const topicName = process.env.TOPIC || "<TOPIC>";

    async function main() {
        // create a Service Bus client using the connection string to the Service Bus namespace
        const sbClient = new ServiceBusClient(connectionString);
        sbClient;

        // createReceiver() can also be used to create a receiver for a queue.
        const receiver = sbClient.createReceiver(topicName, subscriptionName);

        // function to handle messages
        const myMessageHandler = async (messageReceived: ServiceBusMessage) => {
            console.log(parseTelegram(messageReceived.body.toString()));
        };

        // subscribe and specify the message and error handlers
        const subscription = receiver.subscribe({
            processMessage: myMessageHandler,
            processError: async (args: ProcessErrorArgs) => {
                console.error(`Error from source ${args.errorSource} occurred: `, args.error);

                // the `subscribe() call will not stop trying to receive messages without explicit intervention from you.
                if (isServiceBusError(args.error)) {
                    switch (args.error.code) {
                        case "MessagingEntityDisabled":
                        case "MessagingEntityNotFound":
                        case "UnauthorizedAccess":
                            // It's possible you have a temporary infrastructure change (for instance, the entity being
                            // temporarily disabled). The handler will continue to retry if `close()` is not called on the subscription - it is completely up to you
                            // what is considered fatal for your program.
                            console.error(
                                `An unrecoverable error occurred. Stopping processing. ${args.error.code}`,
                                args.error,
                            );
                            await subscription.close();
                            break;
                        case "MessageLockLost":
                            console.error(`Message lock lost for message`, args.error);
                            break;
                        case "ServiceBusy":
                            // choosing an arbitrary amount of time to wait.
                            await delay(1000);
                            break;
                    }
                }
            },
        });

        // Keep connection opened until app is closed
        // await receiver.close();
        // await sbClient.close();
    }

    // call the main function
    main().catch((err) => {
        console.error("Error occurred: ", err);
        process.exit(1);
    });
};
