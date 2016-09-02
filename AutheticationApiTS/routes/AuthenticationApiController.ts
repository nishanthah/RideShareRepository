import express = require('express');
import IUserDAO = require("../common/IUserDAO");
import User = require("../models/User");
import IUser = require("../models/common/IUser");
import UserResponse = require("../models/UserResponse");
import TokenPayload = require("../models/TokenPayload");
import AccessToken = require("../models/AccessToken");
import Config = require("../config");
import ApplicationContext = require("../ApplicationContext");
import jwt = require('jsonwebtoken'); 

class AuthhenticationAPIController{

    constructor() {
       
    }

    // /useraccount
    useraccount(req: express.Request, res: express.Response) {

        var user = new User();
        user.email = req.body.email;
        user.firstName = req.body.firstName;
        user.lastName = req.body.lastName;
        user.password = req.body.password;
        user.userName = req.body.userName;
        user.profileImage = req.body.profileImage;
        try {
            var dataAccess = ApplicationContext.getDB();
            dataAccess.addUser(user);
            dataAccess.onUserAdded = (error: Error, status: boolean) => {
                if (status) {
                    res.json({ success: true });
                }
                else {
                    res.json({ success: false, message: "Cant Create a User" });
                }
            };
           
        }
        catch (e) {
            res.json({ success: false, message: e.message});
        }
       
    }

    // /accesstoken
    accesstoken(req: express.Request, res: express.Response) {
        var dataAccess = ApplicationContext.getDB();
        dataAccess.getSelectedUser(req.body.userName);
        dataAccess.onSelectedUserDataReceived = (error: Error, user: IUser) => {
                if (error) {
                    res.json({ success: false, message: error.message });
                }
                else if (user.password != req.body.password) {
                    res.json({ success: false, message: 'Authentication failed. Wrong password.' });
                }
                else {

                    // if user is found and password is right
                    // create a token
                    var tokenPayload = new TokenPayload();
                    tokenPayload.userName = user.userName;
                    tokenPayload.canAccessUserInfo = true;

                    var token = jwt.sign(tokenPayload, Config.secret, {
                        expiresIn: '24h' // expires in 24 hours
                    });

                    var accessToken = new AccessToken();
                    accessToken.token = token;
                    accessToken.success = true;

                    // return the information including token as JSON
                    res.json(accessToken);
                }
            };

    }

    // /userinfo
    userinfo(req: express.Request, res: express.Response) {              
        var dataAccess = ApplicationContext.getDB();
        var user = dataAccess.getSelectedUser(req.body.userName);

        dataAccess.onSelectedUserDataReceived = (error: Error, user: IUser) => {

                if (error) {
                    res.json({ success: false, message: error.message });
                }

                else if (req.body.canAccessUserInfo) {

                    var userResponse = new UserResponse();
                    userResponse.email = user.email;
                    userResponse.firstName = user.firstName;
                    userResponse.lastName = user.lastName;
                    userResponse.userName = user.userName;
                    userResponse.profileImage = user.profileImage;
                    userResponse.success = true;
                    res.json(userResponse);
                }
                else {
                    res.json({ success: false, message: 'No Permissions to access' });
                }
            };

    }

    // /account
    account(req: express.Request, res: express.Response) {

        var user = new User();
        user.email = req.body.email;
        user.firstName = req.body.firstName;
        user.lastName = req.body.lastName;
        user.password = req.body.password;
        user.userName = req.body.userName;
        user.profileImage = req.body.profileImage;
        var dataAccess = ApplicationContext.getDB();
        dataAccess.updateUser(user);
        dataAccess.onUserUpdated = (error: Error, status: boolean) => {

                if (error) {
                    res.json({ success: false, message: error.message });
                }

                else if (status) {
                    res.json({ success: true });
                }
                else {
                    res.json({ success: false, message: "Cant update the User" });
                }
            };

       
    }

    token(req: express.Request, res: express.Response, next: express.NextFunction) {

        var token = req.body.token || req.query.token || req.headers['x-access-token'];
        var self = this;
        // decode token
        if (token) {
            jwt.verify(token, Config.secret, function (err, decoded) {
                if (err) {
                    res.json({ success: false, message: 'Failed to authenticate token.' });
                }

                else if (decoded) {

                    if (decoded.userName) {
                        req.body.userName = decoded.userName;
                        req.body.canAccessUserInfo = decoded.canAccessUserInfo;
                        next();
                    }
                    else res.json({ success: false, message: 'No user found' });
                }
                else {
                    res.json({ success: false, message: 'Cant decode user data' });
                }
            });
        }
        else
            return res.json({ success: false, message: 'Token not found.' });
    }

}

export = AuthhenticationAPIController