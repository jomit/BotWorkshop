(function (directLineToken) {
    var request = require("request-promise")

    directLineToken.generateToken = async function (req, res) {
        var userId = "dl_default_user";
        var options = {
            uri: "https://directline.botframework.com/v3/directline/tokens/generate",
            method: "POST",
            body:  { User: { Id: userId } },
            json: true,
            headers: {
                "Authorization": "Bearer YOUR_WEBCHAT_TOKEN_HERE"
            }
        }
        try {
            var result = await request(options);
            //console.log(result)
            res.json(result);
        } catch (err) {
            console.error(err);
        }
    };
})(module.exports);