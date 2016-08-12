// get an instance of mongoose and mongoose.Schema
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

// set up a mongoose model and pass it using module.exports
module.exports = mongoose.model('UserCoordinate', new Schema({
	firstName: String,
	lastName: String,
	mobileNo: String,
	email: String,
	userName: String,
	longitude: String,
	latitude: String,
	destinationName:String,
	destinationLongitude:String,
	destinationLatitude:String,
    userType: Number,
    isLoggedIn:Boolean,
	profileImage: String
}));
