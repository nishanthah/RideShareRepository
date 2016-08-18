"use strict";
var mongoose = require("mongoose");
var userSchema = new mongoose.Schema({
    firstName: String,
    lastName: String,
    email: String,
    userName: String,
    password: String
});
var User = mongoose.model("User", userSchema);
module.exports = User;
//# sourceMappingURL=IUserModel.js.map