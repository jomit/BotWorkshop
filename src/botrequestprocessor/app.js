require("dotenv").load();
var messagebus = require("./messagebus");
var fs = require("fs");

console.log("Starting Bot Processing Job...");
messagebus.startprocessing();