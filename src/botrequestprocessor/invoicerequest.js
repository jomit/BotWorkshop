(function (invoicerequest) {

    invoicerequest.getPOStatus = function (requestMessage, callback) {
        console.log(`Finding status for poNumber ${requestMessage.poNumber}`);
        var resultMsg = requestMessage;
        resultMsg.status = "InReview";  // TODO: Implement code to connect with on premises system here
        callback(resultMsg);
    };

})(module.exports);