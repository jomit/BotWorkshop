# Update LUIS Model

***Note: If you have created the Web App Bot the LUIS app will be created for you automatically.***

- Log in to [www.luis.ai](http://www.luis.ai/applications) and under `My Apps` click the app that was created.

- Copy name of the LUIS App and update it in the `luis-model\financebot.json` file.

- Under `Manage` tab, click on `Versions`

- Click on `Import new version` and upload the `src\luis-model\financebot.json` file.

- Click on `Train Button` to train the updated model.

- Click on `Publish` button to publish the model to Production. 

![LUIS](https://raw.githubusercontent.com/jomit/BotWorkshop/master/images/2-1.png)


### Additional Resources

- [What is Language Understanding (LUIS) ?](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/what-is-luis)

- [Best practices for building a language understanding app with Cognitive Services](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-best-practices)

