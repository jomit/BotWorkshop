using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace BotRequestProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var serviceBusConnectionString = configuration["ServiceBusConnectionString"];
            var requestQueueName = configuration["RequestQueueName"];

            //TestCreateQueue(configuration);

            if (!string.IsNullOrEmpty(serviceBusConnectionString) && !string.IsNullOrEmpty(requestQueueName))
                MessageProcessor.StartAsync(serviceBusConnectionString, requestQueueName).GetAwaiter().GetResult();
            else
            {
                Console.WriteLine("Specify -ConnectionString and -QueueName to execute the example.");
                Console.ReadKey();
            }
        }

        static void TestCreateQueue(IConfigurationRoot configuration)
        {
            var qm = new QueueManagement();
            qm.CreateQueue(configuration);
        }
    }
}
