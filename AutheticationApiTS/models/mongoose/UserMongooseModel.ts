import mongoose = require("mongoose");

import IUser = require("./../common/IUser");

var userSchema = new mongoose.Schema({
    firstName: String,
    lastName: String,
    email: String,
    userName: String,
    password: String,
    profileImage: String,
    gender: String,
    resetPasswordGuid: String
});

interface IUserModel extends IUser, mongoose.Document { }

var UserMongooseModel = mongoose.model<IUserModel>("User", userSchema);

export = UserMongooseModel;