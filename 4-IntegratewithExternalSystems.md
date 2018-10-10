# Integrate Bot with External Systems


### Connect with On-Premises systems

There are multiple ways to [connect an on-premises network to Azure](https://docs.microsoft.com/en-us/azure/architecture/reference-architectures/hybrid-networking/). For this excercise we will use a more simpler route to connect via [message patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/category/messaging) using [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview).

To enable two way communication with Service Bus, we will need a listener app running on premises which can securely access our on premises system and then communicate back with our Azure Bot Service. The Bot request will be handled by a single queue but for response we will create a new queue per user. Here are steps to integrate the two way communication:

    1. Create Service Bus Queue
    2. Create on premises Bot Request Processor App
    3. Create a new Bot Class which communicates with Bot Request Processor App
    4. Test the integration  

### 1. Create the Service Bus Queue

- Follow the instructions [here](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues#1-create-a-namespace-using-the-azure-portal) and create a namespace

- Under `Queues`, create a new queue named `requests`.

- Under `Shared Access Policies : RootManageSharedAccessKey`, copy the `Primary Connection String`.

### 2. Create the on premises Bot Request Processor App

- Copy the `src\botrequestprocessor` folder to your on premises server. (To run this app you need to install [Nodejs](https://nodejs.org/en/) 8.12 or greater)

- (OPTIONAL) Update the implementation code under `src\botrequestprocessor\invoicequest.js`

- Create an environment file `.env` under the folder with 2 values:

    - `ServiceBusConnectionString=<Primary Connection String>`
    - `ServiceBusQueueName=requests`

- Open command prompt under the folder and run `npm install`

### 3. Create a new Bot Class which communicates with the Bot Request Processor App

- Open your existing Bot folder in Visual Studio Code

- Copy `src\webapp\financebot.js` and `src\webapp\messagebus.js` files into the existing bot app

- Update the `index.js` file to use the new FinanceBot class instead of the BasicBot:


    - Include the file 

    ```javascript
    // This bot's main dialog.
    //const { BasicBot } = require('./bot');

    // Read botFilePath and botFileSecret from .env file
    // Note: Ensure you have a .env file and include botFilePath and botFileSecret.
    
    const ENV_FILE = path.join(__dirname, '.env');
    const env = require('dotenv').config({ path: ENV_FILE });

    const { FinanceBot } = require('./financebot');
    ```

    - Instantiate the class
    
    ```javascript

    // Create the main dialog.
    let bot;
    try {
        //bot = new BasicBot(botConfig);
        bot = new FinanceBot(botConfig, adapter);
    } catch (err) {
        console.error(`[botInitializationError]: ${ err }`);
        process.exit();
    }

    ```

- Add 2 environemnt variables in the `.env` file

    - `ServiceBusConnectionString=<Primary Connection String>`
    - `ServiceBusQueueName=requests`

### 4. Test the integration  

- Test the integration locally using Emulator

    - Open terminal under `botrequestprocessor` folder and run `npm start`

    - Open another terminal under bot web app folder and run `npm start`

    - Open Bot Emulator and select the `.bot` file (as shown in Section 3)

    - Type `what is the status for po number 12345`, make sure the status returned matches the expected value.

    ![Bot Emulator](https://raw.githubusercontent.com/jomit/BotWorkshop/master/images/4-1.png)

- Test the integration with Azure

    - In the Azure Portal under Bot, click on `Application Settings` and add the 2 environment variables:

        - `ServiceBusConnectionString=<Primary Connection String>`
        - `ServiceBusQueueName=requests`

    - Click on Build and Open online code editor.

    - Click on `Open Console` button from the left navigation and run the following command to install the service bus package:

        - `npm install --save azure-sb`

    - Use Source Control options in VSCode to Commit & Push the updated code.

    - In Azure Portl under App Service, click on `Deployment Options`, you should see a new build.

    - Once the build status is `Active`, go back to Web App bot and test it using `Test in Web App`

    ![Azure Bot Service](https://raw.githubusercontent.com/jomit/BotWorkshop/master/images/4-2.png)