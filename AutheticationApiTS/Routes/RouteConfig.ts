import express = require('express');
import AuthenticationAPI = require('./AuthenticationApiController');
import UserMongooseDAO = require('../dataaccess/UserDAO');

class RouteConfig {

    private apiRoutes: express.Router;
    private app: express.Express;

    constructor(app: express.Express) {
        this.apiRoutes = express.Router();
        this.app = app;
    }

    initApiRoutes() {

        var authenticationApi = new AuthenticationAPI(new UserMongooseDAO());
        this.apiRoutes.post('/useraccount', authenticationApi.useraccount);
        this.apiRoutes.post('/accesstoken', authenticationApi.accesstoken);
        // route middleware to verify a token
        this.apiRoutes.use((authenticationApi.token).bind(authenticationApi));

        this.apiRoutes.put('/useraccount', authenticationApi.account);
        this.apiRoutes.get('/userinfo', authenticationApi.userinfo);
        this.app.use('/authapp', this.apiRoutes);
    }
}

export = RouteConfig;