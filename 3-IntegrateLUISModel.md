# Integrate LUIS Model with Bot

### Add support for POStatus Intent

- In Visual Studio Code, replace the `bot.js` file with `src\webapp\bot.js`

- Review the changes

### Start the Bot locally

- In the Azure portal, for Web App Bot resource, select the Application Settings and copy the values of `botFilePath` and `botFileSecret`.

- Open a terminal window and install the npm dependencies for the bot:
    
    - `npm install`

- Create a file to hold the environment variables the bot code looks for. Name the file `.env`. Add the following environment variables:

    - `botFilePath=`

    - `botFileSecret=`

- Set the values of the environment variables to the values you copied from the Azure bot service's Application Settings.

- Start the bot

    - `npm start`

    ![Bot started](https://raw.githubusercontent.com/jomit/BotWorkshop/master/images/3-1.png)

### Test the bot in emulator

- Open Bot Emulator

- In the bot emulator, select the `.bot` file in the root of the project and enter the bot secret you copied from the Azure bot service's Application Settings:

    ![Bot Emulator Secret](https://raw.githubusercontent.com/jomit/BotWorkshop/master/images/3-2.png)

    ![Bot Emulator](https://raw.githubusercontent.com/jomit/BotWorkshop/master/images/3-3.png)

- In the bot emulator, enter `What is the status for po 12345` and test the response.

    ![Bot Emulator](https://raw.githubusercontent.com/jomit/BotWorkshop/master/images/3-4.png)


### Deploy to Azure   




### Additional Resources

- [Tutorial - LUIS bot in Node.js](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-nodejs-tutorial-bf-v4#modify-bot-code)