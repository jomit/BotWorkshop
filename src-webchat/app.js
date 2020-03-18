const express = require('express');
const cookieParser = require("cookie-parser");
const bodyParser = require("body-parser");
const path = require("path");
var directLineToken = require("./directLineToken");

const app = express();

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, "public")));
const port = process.env.port || 3000;

app.post("/generateToken", (req, res, next) => directLineToken.generateToken(req, res).catch(next));

app.listen(port, () => console.log(`Example app listening on port ${port}!`));