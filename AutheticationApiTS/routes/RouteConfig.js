"use strict";
var express = require('express');
var AuthenticationAPI = require('./AuthenticationApiController');
var RouteConfig = (function () {
    function RouteConfig(app) {
        this.apiRoutes = express.Router();
        this.app = app;
    }
    RouteConfig.prototype.initApiRoutes = function () {
        var authenticationApi = new AuthenticationAPI();
        this.apiRoutes.get('/userinfosendemailwithguid/:userName', authenticationApi.userinfosendemailwithguid);
        this.apiRoutes.put('/userinfosendemailwithcode/:userName/:code/:flag', authenticationApi.userinfosendemailwithcode);
        this.apiRoutes.get('/userinfobyguid/:resetPasswordGuid', authenticationApi.userinfobyguid);
        this.apiRoutes.post('/useraccount', authenticationApi.useraccount);
        this.apiRoutes.post('/accesstoken', authenticationApi.accesstoken);
        // route middleware to verify a token
        this.apiRoutes.use((authenticationApi.token).bind(authenticationApi));
        this.apiRoutes.put('/useraccount', authenticationApi.account);
        this.apiRoutes.delete('/useraccount', authenticationApi.deleteaccount);
        this.apiRoutes.get('/userinfo', authenticationApi.userinfo);
        this.apiRoutes.post('/registrationcode/:userName/:code', authenticationApi.updateregistrationcode);
        this.app.use('/authapp', this.apiRoutes);
    };
    return RouteConfig;
}());
module.exports = RouteConfig;
//# sourceMappingURL=RouteConfig.js.map