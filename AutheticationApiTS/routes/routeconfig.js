var express = require('express');
var AuthenticationAPI = require('./AuthenticationApiController');
var RouteConfig = (function () {
    function RouteConfig(app) {
        this.apiRoutes = express.Router();
        this.app = app;
    }
    RouteConfig.prototype.initApiRoutes = function () {
        var authenticationApi = new AuthenticationAPI();
        this.apiRoutes.post('/useraccount', authenticationApi.useraccount);
        this.apiRoutes.post('/accesstoken', authenticationApi.accesstoken);
        // route middleware to verify a token
        this.apiRoutes.use((authenticationApi.token).bind(authenticationApi));
        this.apiRoutes.put('/useraccount', authenticationApi.account);
        this.apiRoutes.get('/userinfo', authenticationApi.userinfo);
        this.app.use('/authapp', this.apiRoutes);
    };
    return RouteConfig;
})();
module.exports = RouteConfig;
//# sourceMappingURL=routeconfig.js.map