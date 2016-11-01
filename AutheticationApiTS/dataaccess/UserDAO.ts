import IUserDAO = require("../common/IUserDAO");
import IUser = require("../models/common/IUser");
import User = require("../models/mongoose/UserMongooseModel");
import Config = require("../config");

class UserMongooseDAO implements IUserDAO
{
    onSelectedUserDataReceived: (error: Error, userData: IUser) => void ;
    onUserAdded: (error: Error, status: boolean) => void;
    onUserUpdated: (error: Error, status: boolean) => void;
    constructor() {
        
    }
    addUser(user: IUser)
    {        
        // create a sample user
        var userModel = new User();
        userModel.userName = user.userName;
        userModel.firstName = user.firstName;
        userModel.lastName = user.lastName;
        userModel.email = user.email;
        userModel.password = user.password;
        userModel.profileImage = user.profileImage;
        userModel.gender = user.gender;
        userModel.resetPasswordGuid = user.resetPasswordGuid;

        var status: boolean;
        var self = this;
        // save the sample user
        userModel.save(function (err) {
            if (err) self.onUserAdded(new Error("Error saving user."), null);

            self.onUserAdded(null, true);
        });
    }

    updateUser(user: IUser)
    {
        var status: boolean;
        var self = this;
        User.findOne({ userName: user.userName }, function (err, selecteduser) {

            if (err) self.onUserUpdated(new Error("Error getting user for update."), null);

            else if (!selecteduser) {
                self.onUserUpdated(new Error("User not found."), null);
            }
            else {
                selecteduser.email = user.email;
                selecteduser.gender = user.gender;
                selecteduser.firstName = user.firstName;
                selecteduser.lastName = user.lastName;
                if (user.password != null)
                    selecteduser.password = user.password;
                selecteduser.userName = user.userName;
                selecteduser.profileImage = user.profileImage;
                selecteduser.resetPasswordGuid = user.resetPasswordGuid;
                selecteduser.save(function (err) {
                    if (err) self.onUserUpdated(new Error("Error updating user."), null);

                    self.onUserUpdated(null, true);
                });
            }

        });
    }

    getSelectedUser(userName: string)
    {
        var userData: IUser = new User();
        var self = this;
         User.findOne({userName: userName}, function (err, user) {
             
             if (err) self.onSelectedUserDataReceived(new Error("Error retriving user."), null);

             else if (!user) {
                 self.onSelectedUserDataReceived(new Error("User not found."), null);
             }
             else{
                 userData.email = user.email;
                 userData.gender = user.gender;
                 userData.firstName = user.firstName;
                 userData.lastName = user.lastName;
                 userData.password = user.password;
                 userData.userName = user.userName;
                 userData.profileImage = user.profileImage;
                 userData.resetPasswordGuid = user.resetPasswordGuid;
                 self.onSelectedUserDataReceived(null, userData);
             }

         });
   
    }

    getSelectedUserByGuid(resetPasswordGuid: string) {
        var userData: IUser = new User();
        var self = this;
        User.findOne({ resetPasswordGuid: resetPasswordGuid }, function (err, user) {
            
            if (err) self.onSelectedUserDataReceived(new Error("Error retriving user."), null);

            else if (!user) {
                self.onSelectedUserDataReceived(new Error("User not found."), null);
            }
            else {
                userData.email = user.email;
                userData.gender = user.gender;
                userData.firstName = user.firstName;
                userData.lastName = user.lastName;
                userData.password = user.password;
                userData.userName = user.userName;
                userData.profileImage = user.profileImage;
                userData.resetPasswordGuid = user.resetPasswordGuid;
                self.onSelectedUserDataReceived(null, userData);
            }

        });

    }
}

export = UserMongooseDAO;