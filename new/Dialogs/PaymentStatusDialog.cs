// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class PaymentStatusDialog : CancelAndHelpDialog
    {
        private const string DestinationStepMsgText = "Where would you like to travel to?";
        private const string OriginStepMsgText = "Where are you traveling from?";

        public PaymentStatusDialog()
            : base(nameof(PaymentStatusDialog))
        {
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
            var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(message, cancellationToken);
            Thread.Sleep(5000);
            var result = $"Status for invoice number : {apDetails.InvoiceNumber} and vendor: {apDetails.VendorName} is PAID";
            apDetails.Result = result;
            return await stepContext.NextAsync(result, cancellationToken);
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
    }
}
