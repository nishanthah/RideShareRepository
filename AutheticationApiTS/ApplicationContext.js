var Config = require("./config");
var UserMongooseDAO = require('./dataaccess/UserDAO');
var mongoose = require("mongoose");
var ApplicationContext = (function () {
    function ApplicationContext() {
    }
    ApplicationContext.getDB = function () {
        if (!ApplicationContext.isDBConnected) {
            mongoose.connect(Config.database);
            ApplicationContext.isDBConnected = true;
        }
        return new UserMongooseDAO();
    };
    ApplicationContext.isDBConnected = false;
    return ApplicationContext;
})();
module.exports = ApplicationContext;
//# sourceMappingURL=ApplicationContext.js.map