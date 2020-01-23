using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BotRequestProcessor
{
    public class QueueManagement
    {
        public async void CreateQueue(IConfigurationRoot configuration)
        {
            try
            {
                var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(configuration["MicrosoftAppId"], configuration["MicrosoftAppPassword"], configuration["TenantId"], AzureEnvironment.AzureGlobalCloud);
                var serviceBusManager = ServiceBusManager.Authenticate(credentials, configuration["SubscriptionId"]);
                var serviceBusNamespace = serviceBusManager.Namespaces.GetByResourceGroup(configuration["ResourceGroupName"], configuration["ResourceName"]);

                var newqueue = serviceBusNamespace.Queues.Define("mynewqueue")
                        .WithDefaultMessageTTL(TimeSpan.FromMinutes(10))
                        .WithExpiredMessageMovedToDeadLetterQueue()
                        .WithMessageMovedToDeadLetterQueueOnMaxDeliveryCount(40)
                        .Create();
                Console.WriteLine(newqueue.Status);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
