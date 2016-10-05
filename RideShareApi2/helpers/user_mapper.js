var UserVehicle = require('../app/models/vehicle.js');
var mongoose = require('mongoose');
module.exports = function () {
    
    function mapUsersList(usersData, mapUsersList) {

        var usersList = new Array();
        usersData.forEach(function (singleUser) {
            
            var userData = mapSingleUser(singleUser, function (singleUser) {
                var userData = singleUser;
                usersList.push(userData);
                mapUsersList(usersList);
            });
            
        });
        
    }
    
    function mapSingleUser(singleUser, mapSingleUserCallBack) {
        
        var user = {};
        user.userName = singleUser.userName;
        user.email = singleUser.email;
        user.firstName = singleUser.firstName;
        user.lastName = singleUser.lastName;
        user.mobileNo = singleUser.mobileNo;
        user.userType = singleUser.userType;
		user.profileImage = singleUser.profileImage;
		user.resentRequest = singleUser.resentRequest;
        var location = {};
        location.longitude = singleUser.longitude;      
        location.latitude = singleUser.latitude;
        
        var destination = {};
        destination.name = singleUser.destinationName,
        destination.longitude = singleUser.destinationLongitude;      
        destination.latitude = singleUser.destinationLatitude;
        
        var userData = {};
        userData.user = user;
        userData.location = location;
        userData.destination = destination;
        getUserVehicleDetails(singleUser.userName, function (vehicleList) {
            userData.vehicles = vehicleList;
            mapSingleUserCallBack(userData);
        });
        
    }
    
    function getUserVehicleDetails(userName, callBack){
        
        
        UserVehicle.find({ userName : userName }, function (err, userVehicles) {           
            
            var userVehicleData = new Array();
            
            if (userVehicles.length != 0) {
                userVehicles.forEach(function (userVehicle) {
                    var userVehiData = {};
                    userVehiData.userName = userVehicle.userName;
                    userVehiData.vehicleMake = userVehicle.vehicleMake;
                    userVehiData.vehicleModel = userVehicle.vehicleModel;
                    userVehiData.vehicleColor = userVehicle.vehicleColor;
                    userVehiData.vehicleMaxPassengerCount = userVehicle.vehicleMaxPassengerCount;
                    userVehiData.vehicleNumberPlate = userVehicle.vehicleNumberPlate;
                    
                    userVehicleData.push(userVehiData);
                });             
            }
            
            callBack(userVehicleData);
            
        });
        
    }

    return {
        mapSingleUser: mapSingleUser,
        mapUsersList: mapUsersList
    };
}();