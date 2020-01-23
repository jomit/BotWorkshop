using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotRequestProcessor
{
    public class MessageProcessor
    {
        static IQueueClient receiveQueueClient;
        static string serviceBusConnectionString;

        public static async Task StartAsync(string _serviceBusConnectionString, string _requestsQueueName)
        {
            serviceBusConnectionString = _serviceBusConnectionString;
            receiveQueueClient = new QueueClient(_serviceBusConnectionString, _requestsQueueName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Bot Request Processor Start...");
            Console.WriteLine("======================================================");

            RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();

            await receiveQueueClient.CloseAsync();
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            receiveQueueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{messageBody}");

            dynamic messageDetails = JsonConvert.DeserializeObject(messageBody);
            if (messageDetails.Type == "InvoicePayment")
            {
                var status = new InvoicePayment().GetPaymentStatus();
                string userQueueName = messageDetails.UserQueueName;
                await SendMessageAsync(status, userQueueName);
            }
            await receiveQueueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static async Task SendMessageAsync(string status, string userQueueName)
        {
            var statusMessage = new Message(Encoding.UTF8.GetBytes(status));
            var sendUserQueueClient = new QueueClient(serviceBusConnectionString, userQueueName);
            Console.WriteLine($"Sending message: {statusMessage}");
            await sendUserQueueClient.SendAsync(statusMessage);
            await sendUserQueueClient.CloseAsync();
        }

        // Use this Handler to look at the exceptions received on the MessagePump
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
