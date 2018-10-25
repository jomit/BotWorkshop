const { ActivityTypes, TurnContext } = require('botbuilder');
const { LuisRecognizer } = require('botbuilder-ai');

const LUIS_CONFIGURATION = 'BasicBotLuisApplication';

// Supported LUIS Intents.
const GREETING_INTENT = 'Greeting';
const CANCEL_INTENT = 'Cancel';
const HELP_INTENT = 'Help';
const NONE_INTENT = 'None';
const POSTATUS_INTENT = 'POStatus';

const MessageBus = require('./messagebus');

class FinanceBot {
    /**
     * Constructs the necessary pieces for this bot to operate:
     * 1. LUIS client
     *
     * @param {BotConfiguration} botConfig contents of the .bot file
     */
    constructor(botConfig, adapter) {
        if (!botConfig) throw ('Missing parameter.  botConfig is required');

        // Add the LUIS recognizer.
        const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
        if (!luisConfig || !luisConfig.appId) throw ('Missing LUIS configuration. Please follow README.MD to create required LUIS applications.\n\n');
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            endpoint: luisConfig.getEndpoint(),
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: luisConfig.authoringKey
        });
        this.adapter = adapter;
    }

    /**
     * Driver code that does one of the following:
     * 1. Display a welcome card upon receiving ConversationUpdate activity 
     * 2. Use LUIS to recognize intents for incoming user message
     * 3. Start a greeting dialog
     * 4. Optionally handle Cancel or Help interruptions
     *
     * @param {Context} context turn context from the adapter
     */
    async onTurn(context) {
        if (context.activity.type === ActivityTypes.Message) {

            // Perform a call to LUIS to retrieve results for the current activity message.
            const results = await this.luisRecognizer.recognize(context);
            const topIntent = LuisRecognizer.topIntent(results);

            // Determine what we should do based on the top intent from LUIS.
            switch (topIntent) {
                case GREETING_INTENT:
                    await context.sendActivity(`Hello`);
                    break;
                case HELP_INTENT:
                    await context.sendActivity(`Let me try to provide some help.`);
                    await context.sendActivity(`I understand greetings, being asked for help, or being asked to cancel what I am doing.`);
                    break;
                case CANCEL_INTENT:
                    await context.sendActivity(`I have nothing to cancel.`);
                    break;
                case POSTATUS_INTENT:
                    await this.getPOStatus(results, context, this.adapter);
                    break;
                case NONE_INTENT:
                default:
                    await context.sendActivity(`I didn't understand what you just said to me.`);
                    break;
            }
        }
    }

    /**
    * Helper function to process PO Status
    *
    * @param {LuisResults} luisResults - LUIS recognizer results
    * @param {Context} context - context
    */
    async getPOStatus(luisResult, context, adapter) {
        let poNumberEntityName = "PONumber";
        let poNumber = 0;
        if (Object.keys(luisResult.entities).length !== 1) {
            if (luisResult.entities[poNumberEntityName] !== undefined) {
                poNumber = luisResult.entities[poNumberEntityName][0];

                context.sendActivity(`Finding PO Status...`);

                // add message to service bus queue and wait for reply.
                var message = await this.getDefaultMessage(context);
                message.poNumber = poNumber;
                message.messageType = "POStatus";
                const reference = TurnContext.getConversationReference(context.activity);
                MessageBus.sendMessage(message, async function (result) {
                    await adapter.continueConversation(reference, async (proactiveTurnContext) => {
                        await proactiveTurnContext.sendActivity(`Status for PO Number ${poNumber} is ${result.status}.`);

                        // Sample response using adaptive cards
                        
                        /* const responseCard = CardFactory.adaptiveCard({                           
                            type : "AdaptiveCard",
                            version: "1.0",
                            body: [
                                {
                                    type: "TextBlock",
                                    weight : "bolder",
                                    text: `Status for PO Number ${poNumber} is ${result.status}.`
                                }
                            ]
                        });
                        await proactiveTurnContext.sendActivity({ attachments: [responseCard] });
                        */
                    });
                });
            } else {
                await context.sendActivity(`PO Number not found.`);
            }
        }
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

module.exports.FinanceBot = FinanceBot;