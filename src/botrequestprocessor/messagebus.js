(function (messagebus) {

    var azure = require('azure-sb');
    var async = require('async');
    var jobInterval = 100; //ms

    var invoiceRequest = require('./invoicerequest');

    messagebus.startprocessing = function () {
        var connStr = process.env.ServiceBusConnectionString;
        var requestsQueue = process.env.ServiceBusQueueName;
        var serviceBus = azure.createServiceBusService(connStr);
        serviceBus.getQueue(requestsQueue, function (err, res) {
            if (err) {
                console.log(err);
            } else {
                //console.log('current msg count ' + res.MessageCount);
                setInterval(messagebus.checkForMessages.bind(null, serviceBus, requestsQueue,
                    messagebus.processMessage.bind(null, serviceBus)), jobInterval);
            }
        });
    };

    messagebus.checkForMessages = function (serviceBus, requestsQueueName, callback) {
        serviceBus.receiveQueueMessage(requestsQueueName, { isPeekLock: true }, function (err, lockedMessage) {
            if (err) {
                if (err !== 'No messages to receive') {
                    callback(err);
                }
            } else {
                callback(null, lockedMessage);
            }
        });
    };

    messagebus.processMessage = function (serviceBus, err, queueMessage) {
        if (err) {
            console.log(err);
        } else {
            var requestMessage = JSON.parse(queueMessage.body);
            if (requestMessage.messageType == "POStatus") {
                invoiceRequest.getPOStatus(requestMessage, function(resultMsg){
                    messagebus.sendResult(serviceBus, resultMsg, requestMessage.replyTo, queueMessage);
                })
            }
            else if (requestMessage.messageType == "InvoiceStatus") {
                invoiceRequest.getInvoiceStatus(requestMessage, function(resultMsg){
                    messagebus.sendResult(serviceBus, resultMsg, requestMessage.replyTo, queueMessage);
                })
            }
            else {
                console.log("Invalid message type !");
                return;
            }
        }
    };

    messagebus.sendResult = async function (serviceBus, resultMessage, replyTo, queueMessage) {
            messagebus.deleteAllExistingMessages(serviceBus, replyTo, function () {
                messagebus.sendMessage(serviceBus, resultMessage, replyTo, function () {
                    messagebus.deletemessage(serviceBus, queueMessage);
                });
            });
    };

    messagebus.sendMessage = function (serviceBus, resultMessage, responseQueueName, callback) {
        serviceBus.createQueueIfNotExists(responseQueueName, function (err, res) {
            serviceBus.sendQueueMessage(responseQueueName, JSON.stringify(resultMessage), function (err) {
                if (err) {
                    console.log(err);
                } else {
                    console.log('Result Message Sent : ' + JSON.stringify(resultMessage));
                    callback();
                }
            });
        });
    };

    messagebus.deletemessage = function (serviceBus, queueMessage) {
        serviceBus.deleteMessage(queueMessage, function (deleteError) {
            if (deleteError) {
                console.log(deleteError);
            } else {
                console.log('Message Deleted : ' + JSON.stringify(queueMessage));
            }
        })
    };

    messagebus.deleteAllExistingMessages = function (serviceBus, queueName, callback) {
        // TODO: Need to add logic to only delete messages which are older than 60 minutes
        serviceBus.getQueue(queueName, function (err, res) {
            console.log('Found ' + res.MessageCount + ' exisiting messages');
            if (res.MessageCount == 0) {
                callback();
            } else {
                async.times(res.MessageCount, function (item, next) {
                    serviceBus.receiveQueueMessage(queueName, function (error, receivedMessage) {
                        if (error) {
                            console.log(err);
                        }
                        next(err);
                    });
                }, function (err) {
                    console.log('Deleted all existing messages.');
                    callback();
                });
            }
        });
    };

})(module.exports);