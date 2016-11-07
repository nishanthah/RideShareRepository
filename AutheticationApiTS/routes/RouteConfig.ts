import express = require('express');
import AuthenticationAPI = require('./AuthenticationApiController');


class RouteConfig {

    private apiRoutes: express.Router;
    private app: express.Express;

    constructor(app: express.Express) {
        this.apiRoutes = express.Router();
        this.app = app;
    }

    initApiRoutes() {

        var authenticationApi = new AuthenticationAPI();
        this.apiRoutes.get('/userinfosendemailwithguid/:userName', authenticationApi.userinfosendemailwithguid);
        this.apiRoutes.put('/userinfosendemailwithcode/:userName/:code', authenticationApi.userinfosendemailwithcode);
        this.apiRoutes.get('/userinfobyguid/:resetPasswordGuid', authenticationApi.userinfobyguid);
        this.apiRoutes.post('/useraccount', authenticationApi.useraccount);
        this.apiRoutes.post('/accesstoken', authenticationApi.accesstoken);
        // route middleware to verify a token
        this.apiRoutes.use((authenticationApi.token).bind(authenticationApi));

        this.apiRoutes.put('/useraccount', authenticationApi.account);
        this.apiRoutes.delete('/useraccount', authenticationApi.deleteaccount);
        this.apiRoutes.get('/userinfo', authenticationApi.userinfo);
        this.app.use('/authapp', this.apiRoutes);
    }
}

export = RouteConfig;