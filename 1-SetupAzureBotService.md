# Setup Azure Bot Service

### Azure Bot Web App

- Open Azure Portal and create new Azure Bot Web App:

    | Setting | Value |
    | --- | --- |
    | Bot name | Display name of your Bot |
    | App name | Unique name of your Bot Web App. e.g. `http://<app name>.azurewebsites.net` |
    | Bot Template | Basic Bot (NodeJS) under SDK v4 |
    | App service plan/Location | Unique name of the plan. Choose the same location as Bot Service |
    | Microsoft App ID and password | As provided by your Admin or use Auto Create |

    ![Azure Bot Service](https://raw.githubusercontent.com/jomit/BotWorkshop/master/images/1-1.png)


- Test the Bot using `Test in Web Chat`

### Update Deployment Credentials

- Open Azure Portal and navigate to the Web App Bot

- Under `App Service Settings` click on `All App service settings` 

- Click on `Deployment Credentials`, update the username and password and click Save.


### Setup Git Deployment

- Under App Service click on `Deployment Options`

- Select `Local Git Repository` and click OK.


### Test the Deployment

- Under App Service click on `Overview` and copy the `Git clone url`

- Open command prompt on your local machine and run the following command in the directory where you want to download the source code: (Use the password provided for deployment credentials)
    - `git clone <Git clone url>`

- Open the source code folder in Visual Studio Code.

- Update the Greeting Intent activity text in `bot.js`

- Use Source Control options in VSCode to Commit & Push the updated code.

- In Azure Portal under App Service, click again on `Deployment Options`, you should see a new build.

- Once the build status is `Active`, go back to Web App bot and test it using `Test in Web App`


### Additional Resources

- [Quickstart Tutorial - Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-quickstart?view=azure-bot-service-4.0)

- [Configure deployment credentials for Azure App Service](https://docs.microsoft.com/en-us/azure/app-service/app-service-deployment-credentials)