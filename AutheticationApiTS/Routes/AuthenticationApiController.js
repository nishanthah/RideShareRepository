"use strict";
var User = require("../models/User");
var UserResponse = require("../models/UserResponse");
var TokenPayload = require("../models/TokenPayload");
var AccessToken = require("../models/AccessToken");
var Config = require("../config");
var jwt = require('jsonwebtoken');
var AuthhenticationAPIController = (function () {
    function AuthhenticationAPIController(userDAO) {
        AuthhenticationAPIController.userDAO = userDAO;
    }
    // /useraccount
    AuthhenticationAPIController.prototype.useraccount = function (req, res) {
        var user = new User();
        user.email = req.body.email;
        user.firstName = req.body.firstName;
        user.lastName = req.body.lastName;
        user.password = req.body.password;
        user.userName = req.body.userName;
        user.profileImage = req.body.profileImage;
        try {
            AuthhenticationAPIController.userDAO.addUser(user);
            AuthhenticationAPIController.userDAO.onUserAdded = function (error, status) {
                if (status) {
                    res.json({ success: true });
                }
                else {
                    res.json({ success: false, message: "Cant Create a User" });
                }
            };
        }
        catch (e) {
            res.json({ success: false, message: e.message });
        }
    };
    // /accesstoken
    AuthhenticationAPIController.prototype.accesstoken = function (req, res) {
        AuthhenticationAPIController.userDAO.getSelectedUser(req.body.userName);
        AuthhenticationAPIController.userDAO.onSelectedUserDataReceived = function (error, user) {
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
    };
    // /userinfo
    AuthhenticationAPIController.prototype.userinfo = function (req, res) {
        var user = AuthhenticationAPIController.userDAO.getSelectedUser(req.body.userName);
        AuthhenticationAPIController.userDAO.onSelectedUserDataReceived = function (error, user) {
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
    };
    // /account
    AuthhenticationAPIController.prototype.account = function (req, res) {
        var user = new User();
        user.email = req.body.email;
        user.firstName = req.body.firstName;
        user.lastName = req.body.lastName;
        user.password = req.body.password;
        user.userName = req.body.userName;
        user.profileImage = req.body.profileImage;
        AuthhenticationAPIController.userDAO.updateUser(user);
        AuthhenticationAPIController.userDAO.onUserUpdated = function (error, status) {
            if (error) {
                res.json({ success: false, message: error.message });
            }
            if (status) {
                res.json({ success: true });
            }
            else {
                res.json({ success: false, message: "Cant update the User" });
            }
        };
    };
    AuthhenticationAPIController.prototype.token = function (req, res, next) {
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
                    else
                        res.json({ success: false, message: 'No user found' });
                }
                else {
                    res.json({ success: false, message: 'Cant decode user data' });
                }
            });
        }
        else
            return res.json({ success: false, message: 'Token not found.' });
    };
    return AuthhenticationAPIController;
}());
module.exports = AuthhenticationAPIController;
//# sourceMappingURL=AuthenticationApiController.js.map