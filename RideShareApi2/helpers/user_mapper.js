var UserVehicle = require('../app/models/vehicle.js');
var UserFavouritePlace = require('../app/models/favourite_place.js');
var mongoose = require('mongoose');
module.exports = function () {
    
    function mapUsersList(usersData, mapUsersListCallback) {

        var usersList = new Array();
		var itemsProcessed = 0;
        usersData.forEach(function (singleUser) {
            
            var userData = mapSingleUser(singleUser, function (singleUser) {
                var userData = singleUser;
                usersList.push(userData);
                itemsProcessed++;
				
				if(itemsProcessed == usersData.length)
				{
					mapUsersListCallback(usersList);
				}
				
            });
            
        });
	
    }
    
    function mapSingleUser(singleUser, mapSingleUserCallBack) {
        
        var user = {};
        user.gender = singleUser.gender;
        user.userName = singleUser.userName;
        user.email = singleUser.email;
        user.firstName = singleUser.firstName;
        user.lastName = singleUser.lastName;
        user.mobileNo = singleUser.mobileNo;
        user.userType = singleUser.userType;
		user.profileImage = singleUser.profileImage;
		user.resentRequest = singleUser.resentRequest;
		user.isLoggedIn = singleUser.isLoggedIn;
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
			getUserFavouritePlaces(singleUser.userName, function (favPlacesList) {
				userData.favPlaces = favPlacesList;
				mapSingleUserCallBack(userData);
			});            
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
	
	function getUserFavouritePlaces(userName, callBack){
        
        
        UserFavouritePlace.find({ userName : userName }, function (err, userFavouritePlaces) {           
            
            var userFavPlaceData = new Array();
            
            if (userFavouritePlaces.length != 0) {
            userFavouritePlaces.forEach(function (userFavouritePlace) {
                var oneObj = {};
                oneObj.userName = userFavouritePlace.userName;
                oneObj.userGivenplaceName = userFavouritePlace.userGivenplaceName;
                oneObj.placeName = userFavouritePlace.placeName;
                oneObj.longitude = userFavouritePlace.longitude;
                oneObj.latitude = userFavouritePlace.latitude;
				oneObj.placeID = userFavouritePlace.placeID;
                oneObj.placeReference = userFavouritePlace.placeReference;
                                
                userFavPlaceData.push(oneObj);
            });            
            
        }
            
            callBack(userFavPlaceData);
            
        });
        
    }

    return {
        mapSingleUser: mapSingleUser,
        mapUsersList: mapUsersList
    };
}();