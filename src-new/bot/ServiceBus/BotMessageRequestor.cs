using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Bot.Builder;

namespace CoreBot.ServiceBus
{
    public class BotMessageRequestor
    {
        string serviceBusConnectionString = string.Empty;
        string requestQueueName = string.Empty;
        string userQueueName = string.Empty;

        public BotMessageRequestor(IConfiguration configuration, string _userQueueName)
        {
            serviceBusConnectionString = configuration["ServiceBusConnectionString"];
            requestQueueName = configuration["RequestQueueName"];
            userQueueName = _userQueueName;
        }

        public async Task SendMessageAsync(string messageBody)
        {
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            var messageSender = new MessageSender(serviceBusConnectionString, requestQueueName);         

            await messageSender.SendAsync(message);
            await messageSender.CloseAsync();
        }

        public async Task<string> ReceiveMessageAsync()
        {
            var messageReceiver = new MessageReceiver(serviceBusConnectionString, userQueueName, ReceiveMode.PeekLock);
            Message message = await messageReceiver.ReceiveAsync();
            //var messageText = $"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}";
            var messageText = $"{Encoding.UTF8.GetString(message.Body)}";
            await messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
            return messageText;
        }
    }
}
