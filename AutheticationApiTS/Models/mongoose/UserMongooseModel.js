var mongoose = require("mongoose");
var userSchema = new mongoose.Schema({
    firstName: String,
    lastName: String,
    email: String,
    userName: String,
    password: String
});
var UserMongooseModel = mongoose.model("User", userSchema);
module.exports = UserMongooseModel;
//# sourceMappingURL=UserMongooseModel.js.map