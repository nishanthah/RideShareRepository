import Config = require("./config");
import UserMongooseDAO = require('./dataaccess/UserDAO');
import mongoose = require("mongoose");

class ApplicationContext {

    static isDBConnected: boolean = false;

    static getDB(): UserMongooseDAO{

        if (!ApplicationContext.isDBConnected) {
            var database = process.env.RIDESHAREDB || Config.database;
            mongoose.connect(database);
            ApplicationContext.isDBConnected = true;
        }
        return new UserMongooseDAO();
    }
}
export = ApplicationContext;