var User = require("../models/mongoose/UserMongooseModel");
var UserMongooseDAO = (function () {
    function UserMongooseDAO() {
    }
    UserMongooseDAO.prototype.addUser = function (user) {
        // create a sample user
        var userModel = new User();
        userModel.userName = user.userName;
        userModel.firstName = user.firstName;
        userModel.lastName = user.lastName;
        userModel.email = user.email;
        userModel.password = user.password;
        userModel.profileImage = user.profileImage;
        var status;
        var self = this;
        // save the sample user
        userModel.save(function (err) {
            if (err)
                self.onUserAdded(new Error("Error saving user."), null);
            self.onUserAdded(null, true);
        });
    };
    UserMongooseDAO.prototype.updateUser = function (user) {
        var status;
        var self = this;
        User.findOne({ userName: user.userName }, function (err, selecteduser) {
            if (err)
                self.onUserUpdated(new Error("Error getting user for update."), null);
            else if (!selecteduser) {
                self.onUserUpdated(new Error("User not found."), null);
            }
            else {
                selecteduser.email = user.email;
                selecteduser.firstName = user.firstName;
                selecteduser.lastName = user.lastName;
                selecteduser.password = user.password;
                selecteduser.userName = user.userName;
                selecteduser.profileImage = user.profileImage;
                selecteduser.save(function (err) {
                    if (err)
                        self.onUserUpdated(new Error("Error updating user."), null);
                    self.onUserUpdated(null, true);
                });
            }
        });
    };
    UserMongooseDAO.prototype.getSelectedUser = function (userName) {
        var userData = new User();
        var self = this;
        User.findOne({ userName: userName }, function (err, user) {
            if (err)
                self.onSelectedUserDataReceived(new Error("Error retriving user."), null);
            else if (!user) {
                self.onSelectedUserDataReceived(new Error("User not found."), null);
            }
            else {
                userData.email = user.email;
                userData.firstName = user.firstName;
                userData.lastName = user.lastName;
                userData.password = user.password;
                userData.userName = user.userName;
                userData.profileImage = user.profileImage;
                self.onSelectedUserDataReceived(null, userData);
            }
        });
    };
    return UserMongooseDAO;
})();
module.exports = UserMongooseDAO;
//# sourceMappingURL=UserDAO.js.map