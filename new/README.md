# Sample C# AP Bot

### High Level Steps

- Create Web App Bot in Azure and download the source code
- Install latest VS Project Templates and Bot Emulator from [here](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-sdk-quickstart?view=azure-bot-service-4.0)

- Open the solution in VS 2019 and update all the Nuget packages
- Open the Bot Emulator and configure [ngrok](https://github.com/Microsoft/BotFramework-Emulator/wiki/Tunneling-(ngrok))
- Create new Bot Configuration as shown [here](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-sdk-quickstart?view=azure-bot-service-4.0#start-the-emulator-and-connect-your-bot) and make sure to add the ApplicationId and ApplicationPassword (copy these values from appsettings.json file).

- Open LUIS portal.
    - (*Note) Migrate all LUIS apps if prompted.
- Import the LUIS model CognitiveModels\AccountsPayable.json file
- Train -> Test and Publish the Model

- Install the [botbuilder-tools](https://github.com/microsoft/botbuilder-tools)
- Generate the LUIS Recognizer class. (This step is only need if you make any chagnes to LUIS model)
    - LUISGen .\AccountsPayable.json -cs Luis.AccountsPayable -o .

- Run the Bot project in VS and test it in Emulator    
    - e.g. what is the payment status for invoice number 9900 and vendor MarsMangos