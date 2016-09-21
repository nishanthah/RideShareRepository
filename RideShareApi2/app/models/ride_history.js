// get an instance of mongoose and mongoose.Schema
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

// set up a mongoose model and pass it using module.exports
module.exports = mongoose.model('RideHistory', new Schema({
    userName: String,
    driverUserName: String,
    requestedTime: Date,
    requestStatus:Number,
    sourseName: String,
    destinationName:String,
    sourceLongitude: String,
    sourceLatitude: String,
    destinationLongitude: String,
    destinationLatitude: String,
	requestAcceptTime: Date,
    requestRejectTime: Date,
    riderMeetTime: Date,
    riderEndTime: Date,
    polyLine:String,
	notificationStatus:Number
}));