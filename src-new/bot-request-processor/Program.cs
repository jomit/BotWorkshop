using Microsoft.Azure.ServiceBus;
using System;

namespace BotRequestProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Move these to config file later
            var serviceBusConnectionString = "";
            var requestQueueName = "requests";

            if (!string.IsNullOrEmpty(serviceBusConnectionString) && !string.IsNullOrEmpty(requestQueueName))
                MessageProcessor.StartAsync(serviceBusConnectionString, requestQueueName).GetAwaiter().GetResult();
            else
            {
                Console.WriteLine("Specify -ConnectionString and -QueueName to execute the example.");
                Console.ReadKey();
            }
        }
    }
}
