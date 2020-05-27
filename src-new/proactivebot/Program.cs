using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProactiveBot
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string teamInternalId = configuration["TeamInternalId"];
            string serviceUrl = configuration["BotServiceUrl"];
            string botClientID = configuration["BotAppId"];
            string botClientSecret = configuration["BotAppSecret"];
            string tenantId = configuration["TenantId"];
            string recipientUserPrincipalName = "<user>@<tenant>.onmicrosoft.com";
            string teamsChannelId = configuration["TeamsChannelId"];

            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
            var connectorClient = new ConnectorClient(new Uri(serviceUrl), new MicrosoftAppCredentials(botClientID, botClientSecret));

            await SendPersonalMessageToUser(connectorClient, tenantId, teamInternalId, recipientUserPrincipalName, "Hello from Bot !");
            
            //await SendMessageInTeamChannel(connectorClient, teamsChannelId, "Hello from Bot - inside a channel!");

            //await SendMessageInTeamChannelWithMention(connectorClient, teamInternalId, teamsChannelId, recipientUserPrincipalName, "How are you?");
        }


        static async Task SendPersonalMessageToUser(ConnectorClient connectorClient, string tenantId, string teamInternalId, string recipientUserPrincipalName, string message)
        {
            var teamMembers = await connectorClient.Conversations.GetConversationMembersAsync(teamInternalId, default);
            var user = teamMembers
                .Select(channelAccount => JObject.FromObject(channelAccount).ToObject<TeamsChannelAccount>())
                .First(user => user.UserPrincipalName == recipientUserPrincipalName);

            var personalMessageActivity = MessageFactory.Text(message);
            var conversationParameters = new ConversationParameters()
            {
                ChannelData = new TeamsChannelData
                {
                    Tenant = new TenantInfo
                    {
                        Id = tenantId,
                    }
                },
                Members = new List<ChannelAccount>() { user }
            };

            var response = await connectorClient.Conversations.CreateConversationAsync(conversationParameters);
            await connectorClient.Conversations.SendToConversationAsync(response.Id, personalMessageActivity);
        }


        static async Task SendMessageInTeamChannel(ConnectorClient connectorClient, string teamsChannelId, string message)
        {
            var topLevelMessageActivity = MessageFactory.Text(message);
            var conversationParameters = new ConversationParameters
            {
                IsGroup = true,
                ChannelData = new TeamsChannelData
                {
                    Channel = new ChannelInfo(teamsChannelId),
                },
                Activity = topLevelMessageActivity
            };

            await connectorClient.Conversations.CreateConversationAsync(conversationParameters);
        }

        static async Task SendMessageInTeamChannelWithMention(ConnectorClient connectorClient, string teamInternalId, string teamsChannelId, string recipientUserPrincipalName, string message)
        {
            var teamMembers = await connectorClient.Conversations.GetConversationMembersAsync(teamInternalId, default);
            var userToMention = teamMembers
                .Select(channelAccount => JObject.FromObject(channelAccount).ToObject<TeamsChannelAccount>())
                .First(user => user.UserPrincipalName == recipientUserPrincipalName);

            var mention = new Mention
            {
                Mentioned = userToMention,
                Text = $"<at>{userToMention.Name}</at>",
            };

            var msgActivity = MessageFactory.Text($"{mention.Text} - {message}");
            msgActivity.Entities = new List<Entity> { mention };

            var conversationParameters = new ConversationParameters
            {
                IsGroup = true,
                ChannelData = new TeamsChannelData
                {
                    Channel = new ChannelInfo(teamsChannelId),
                },
                Activity = msgActivity
            };

            await connectorClient.Conversations.CreateConversationAsync(conversationParameters);

        }
    }
}
