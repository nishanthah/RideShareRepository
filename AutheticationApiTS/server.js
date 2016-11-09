"use strict";
var express = require('express');
var bodyParser = require('body-parser');
var morgan = require('morgan');
var RouteConfig = require('./routes/RouteConfig');
// all environments
var port = process.env.PORT || 8078;
var app = express();
app.use(bodyParser.urlencoded({ extended: false, limit: '50mb' }));
app.use(bodyParser.json({ limit: '50mb' }));
// use morgan to log requests to the console
app.use(morgan('dev'));
app.listen(port);
console.log('Started Authentication Server at http://localhost:' + port);
// Initialize API
var routeConfig = new RouteConfig(app);
routeConfig.initApiRoutes();
//# sourceMappingURL=server.js.map