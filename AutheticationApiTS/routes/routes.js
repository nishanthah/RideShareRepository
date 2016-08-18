"use strict";
var express = require('express');
var AuthenticationAPI = require('./authapi');
var UserMongooseDAO = require('../dataaccess/UserDAO');
var RouteConfig = (function () {
    function RouteConfig() {
        this.apiRoutes = express.Router();
        this.initApiRoutes();
    }
    RouteConfig.prototype.initApiRoutes = function () {
        var authenticationApi = new AuthenticationAPI(new UserMongooseDAO());
        this.apiRoutes.post('/useraccount', authenticationApi.useraccount);
        this.apiRoutes.post('/accesstoken', authenticationApi.accesstoken);
        this.apiRoutes.post('/userinfo', authenticationApi.userinfo);
    };
    return RouteConfig;
}());
module.exports = RouteConfig;
//# sourceMappingURL=routes.js.map