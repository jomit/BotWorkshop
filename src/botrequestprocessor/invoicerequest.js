(function (invoicerequest) {

    var sqlserverdb = require("./sqlserverdb");

    invoicerequest.getPOStatus = function (requestMessage, callback) {
        console.log(`Finding status for poNumber ${requestMessage.poNumber}`);
        var resultMsg = requestMessage;
        resultMsg.status = "InReview";  // TODO: Implement code to connect with on premises system here
        callback(resultMsg);
    };

    invoicerequest.getInvoiceStatus = async function (requestMessage, callback) {
        console.log(`Finding status for invoiceNumber ${requestMessage.poNumber}`);
        var resultMsg = requestMessage;
        var invoiceData = await sqlserverdb.invoiceStatus();
        if(invoiceData != null ){
             resultMsg.isPaid = false;
             resultMsg.invoicePaidDate = '03/10/2019';
         } else{
             resultMsg.notFound = true;
         }
        callback(resultMsg);
    };

})(module.exports);