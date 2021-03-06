// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.ServiceBus;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class PaymentStatusDialog : CancelAndHelpDialog
    {
        IConfiguration currentConfiguration;
        public PaymentStatusDialog(IConfiguration configuration)
            : base(nameof(PaymentStatusDialog))
        {
            currentConfiguration = configuration;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GetStatusStepAsync,
                AwaitingResultsStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> GetStatusStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var apDetails = (AccountsPayableDetails)stepContext.Options;
            var messageText = $"Finding payment status for invoice number : {apDetails.InvoiceNumber} and vendor: {apDetails.VendorName}...";
            //var messageText = GetTeamsDetails(stepContext);
            var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(message, cancellationToken);
            var userQueueName = "response"; // stepContext.Context.Activity.Recipient.Id;

            dynamic messageDetails = new
            {
                Type = "InvoicePayment",
                UserQueueName = userQueueName,
                InvoiceNumber = apDetails.InvoiceNumber,
                Vendor = apDetails.VendorName
            };

            var status = await GetPaymentStatusMessageAsync(JsonConvert.SerializeObject(messageDetails), userQueueName);
            //var status = "<TODO>...";

            apDetails.Result = status;
            return await stepContext.NextAsync(status, cancellationToken);
        }

        private async Task<string> GetPaymentStatusMessageAsync(string messageBody, string userQueueName)
        {
            var botRequestor = new BotMessageRequestor(currentConfiguration, userQueueName);
            await botRequestor.SendMessageAsync(messageBody);

            var response = await botRequestor.ReceiveMessageAsync();
            return response;
        }

        private async Task<DialogTurnResult> AwaitingResultsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var apDetails = (AccountsPayableDetails)stepContext.Options;
            if (!string.IsNullOrEmpty(apDetails.Result))
            {
                return await stepContext.EndDialogAsync(apDetails, cancellationToken);
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private string GetTeamsDetails(WaterfallStepContext stepContext)
        {
            return $"ServiceUrl: {stepContext.Context.Activity.ServiceUrl}, TeamId: {stepContext.Context.Activity.TeamsGetTeamInfo().Id}, TeamChannelId: {stepContext.Context.Activity.TeamsGetChannelId()}";
        }
    }
}
