// CHANGE THIS LINE in "index.js"
//------------------------------------------------
// Create the main dialog.
//const dialog = new MainDialog(logger,adapter);
//------------------------------------------------

const { ComponentDialog, DialogSet, DialogTurnStatus, TextPrompt, WaterfallDialog } = require('botbuilder-dialogs');
const MAIN_WATERFALL_DIALOG = 'mainWaterfallDialog';

const MessageBus = require('./messagebus');
const { ActivityTypes, TurnContext } = require('botbuilder');


class MainDialog extends ComponentDialog {

    constructor(logger, adapter) {
        super('MainDialog');

        if (!logger) {
            logger = console;
            logger.log('[MainDialog]: logger not passed in, defaulting to console');
        }

        this.logger = logger;
        this.adapter = adapter;

        this.addDialog(new TextPrompt('TextPrompt'))
            .addDialog(new WaterfallDialog(MAIN_WATERFALL_DIALOG, [
                this.askInvoiceNumber.bind(this),
                this.askCompanyName.bind(this),
                this.findInvoiceStatus.bind(this)
            ]));

        this.initialDialogId = MAIN_WATERFALL_DIALOG;
        this.invoiceDetails = {};
    }

    /**
     * The run method handles the incoming activity (in the form of a TurnContext) and passes it through the dialog system.
     * If no dialog is active, it will start the default dialog.
     * @param {*} turnContext
     * @param {*} accessor
     */
    async run(turnContext, accessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);
        
        this.turnContext = turnContext;
        const dialogContext = await dialogSet.createContext(turnContext);
        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }

    async askInvoiceNumber(stepContext) {
        return await stepContext.prompt('TextPrompt', { prompt: 'Enter your Invoice Number:' });
    }

    async askCompanyName(stepContext) {
        this.invoiceDetails.invoiceNumber = stepContext.result;
        return await stepContext.prompt('TextPrompt', { prompt: 'Enter your Company Name' });
    }

    async findInvoiceStatus(stepContext) {
        this.invoiceDetails.vendorName = stepContext.result;
        await stepContext.context.sendActivity("Finding Invoice Status...");
        const reference = TurnContext.getConversationReference(this.turnContext.activity);

        var message = await this.getDefaultMessage(this.turnContext);
        message.poNumber = this.invoiceDetails.invoiceNumber;
        message.messageType = "POStatus";
        var currentAdapter = this.adapter;
        MessageBus.sendMessage(message, async function (result) {
            await currentAdapter.continueConversation(reference, async (turnContext) => {
                await turnContext.sendActivity(`Status for Invoice Number ${message.poNumber} is ${result.status}.`);
            });
        });
        return await stepContext.endDialog();
    }

    async getDefaultMessage(context) {
        var userId = context.activity.from.id.replace(/:/g,'_');
        return {
            userId: userId,
            epoch: Date.now(),
            replyTo: userId,  //to create reply queue for each user
            channel: 'web'
        }
    }
}

module.exports.MainDialog = MainDialog;
