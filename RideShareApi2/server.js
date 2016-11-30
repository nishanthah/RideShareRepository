// =======================
// get the packages we need ============
// =======================

var express     = require('express');
var app         = express();
var bodyParser  = require('body-parser');
var morgan      = require('morgan');
var mongoose    = require('mongoose');
var nodemailer = require('nodemailer');
var HttpClient = require('node-rest-client').Client;
var urbanAirshipClient = require("./urban_airship_client");
var userMapper = require("./helpers/user_mapper");
var guid = require("guid");
var httpClient = new HttpClient();

var jwt    = require('jsonwebtoken'); // used to create, sign, and verify tokens
var config = require('./config'); // get our config file
var UserCoordinate   = require('./app/models/user'); // get our mongoose model
var RideHistory = require('./app/models/ride_history.js'); // RideHistory mongoose model
var UserVehicle = require('./app/models/vehicle.js'); // Vehicle mongoose model
var VehicleDefinitionData = require('./app/models/vehicle_def.js');
var UserFavouritePlace = require('./app/models/favourite_place.js'); // Favourite Place mongoose model

// =======================
// configuration =========
// =======================
var port = process.env.PORT || 8079; // used to create, sign, and verify tokens
mongoose.Promise = global.Promise;
var dbConnection = process.env.RIDESHAREDB || config.database;
mongoose.connect(dbConnection); // connect to database

var secret = process.env.RIDESHARE_SECRET || config.secret;
app.set('superSecret', secret); // secret variable

// use body parser so we can get info from POST and/or URL parameters
app.use(bodyParser.urlencoded({ extended: false, limit: '50mb' }));
app.use(bodyParser.json({ limit: '50mb' }));

// use morgan to log requests to the console
app.use(morgan('dev'));

var io = require('socket.io').listen(app.listen(port));
ReadData();
console.log('API started at http://localhost:' + port);
// API ROUTES -------------------

// get an instance of the router for api routes
var apiRoutes = express.Router(); 

//To populate the vehicle details definition data. Uses the VehicleDefinitionData.xml file to read data. The collection is vehicledefinitiondata.
function ReadData()
{
    VehicleDefinitionData.find(function (err, vehicledefinitiondata) {
        if (err) res.json({ success: false, message: err });
        
        if (vehicledefinitiondata.length == 0) {
            var xmldom = require('xmldom').DOMParser,
                fs = require('fs');
            
            fs.readFile('VehicleDefinitionData.xml', 'utf-8', function (err, data) {
                if (err) {
                    throw err;
                }
                var vehicle,
                    thisvehicleobject,
                    thismodel,
                    doc,
                    vehicles;
                doc = new xmldom().parseFromString(data, 'application/xml');
                vehicles = doc.getElementsByTagName('vehicles')[0].childNodes;
                
                for (vehicle in vehicles) {
                    thisvehicleobject = vehicles[vehicle];
                    if (thisvehicleobject.firstChild) {
                        if (thisvehicleobject.attributes[0].nodeName = 'make') {                            
                            //all the 'model' objects
                            for (a1 in thisvehicleobject.childNodes) {
                                thismodel = thisvehicleobject.childNodes[a1];
                                if (thismodel.firstChild && thismodel.tagName == 'model') {                                    

                                    var newVehicleDefinitionData = new VehicleDefinitionData({
                                        make: thisvehicleobject.attributes[0].nodeValue,
                                        model: thismodel.firstChild.nodeValue                                       
                                    });

                                    newVehicleDefinitionData.save(function (err) {
                                        if (err) res.json({ success: false, message: err });
                                        
                                        console.log('Vehicle definition data added successfully');                                  
                                        
                                    });
                                }
                            }
                        }
            
                    }
                }
            });
        }
    });
    
};

apiRoutes.get('/vehicledefinitiondata', function (req, res) {
    
    VehicleDefinitionData.find(function (err, userVehicleDefData) {
        
        if (err) res.json({ success: false, message: err });
        
        var userVehicleDefinitionData = new Array();
        
        if (userVehicleDefData.length != 0) {
            userVehicleDefData.forEach(function (oneUserVehicleDefData) {
                var userVehiDefData = {};
                userVehiDefData.make = oneUserVehicleDefData.make;
                userVehiDefData.model = oneUserVehicleDefData.model;
                
                userVehicleDefinitionData.push(userVehiDefData);
            });
            
            res.json({ userVehicleDefData: userVehicleDefinitionData, success: true, message: 'User Vehicle definition data retrived successfully' });
        }
        else {
            res.json({ success: false, message: 'No User Vehicle definition data found' });
        }
    });

});


apiRoutes.post('/users', function (req, res) {	        
            var newUserCoordinate = new UserCoordinate({
                gender: req.body.gender,
                userName: req.body.userName,
                email: req.body.email,
                firstName: req.body.firstName,
                lastName: req.body.lastName,
                profileImage : req.body.profileImage,
                resetPasswordGuid : req.body.resetPasswordGuid,
                deviceID : req.body.deviceID,
				mobileNo: req.body.mobileNo
            });
            
            newUserCoordinate.save(function (err) {
                if (err) res.json({ success: false, message: err });
                
                console.log('User added successfully');
                io.emit('coordinate_changed', "Changed");
                res.json({ success: true });
            });
        
	
	
    
});

apiRoutes.post('/vehicles', function (req, res) {
    
    var newUserVehicle = new UserVehicle({
        userName: req.body.userName,
        vehicleMake: req.body.vehicleMake,
        vehicleModel: req.body.vehicleModel,
        vehicleColor: req.body.vehicleColor,
        vehicleMaxPassengerCount: req.body.vehicleMaxPassengerCount,
        vehicleNumberPlate: req.body.vehicleNumberPlate
    });
    
    newUserVehicle.save(function (err) {
        if (err) res.json({ success: false, message: err });
        
        console.log('Inserted User Vehicle successfully');
        io.emit('coordinate_changed', "Changed");
        res.json({ success: true, message: 'Inserted User Vehicle successfully' });
    });
        
       
});

apiRoutes.post('/favouriteplaces', function (req, res) {
    
    var newUserFavPlace = new UserFavouritePlace({
        userName: req.body.userName,
        userGivenplaceName: req.body.userGivenplaceName,
        placeName: req.body.placeName,
        longitude: req.body.longitude,
        latitude: req.body.latitude,
        placeID: req.body.placeID,
        placeReference: req.body.placeReference
    });
    
    newUserFavPlace.save(function (err) {
        if (err) res.json({ success: false, message: err });
        
        console.log('Inserted User Favourite Places successfully');
        res.json({ success: true, message: 'Inserted User Favourite Places successfully' });
    });
});

apiRoutes.get('/users/:userName', function (req, res) {
        
    UserCoordinate.findOne({ userName : req.params.userName }, function (err, userCoordinate) {
        
        if (err) res.json({ success: false, message: err });
			
        else if (!userCoordinate) {
            res.json({ success: false, message: "User Not Found" });
        }
        else {
            userMapper.mapSingleUser(userCoordinate, function (userData) {
                var userData = userData;
                res.json({ userData: userData, success: true });
            });
        }
			
			
	
    });

});

apiRoutes.get('/users/device/:deviceID', function (req, res) {
    
    UserCoordinate.findOne({ deviceID : req.params.deviceID }, function (err, userCoordinate) {
        
        if (err) res.json({ success: false, message: err });
			
        else if (!userCoordinate) {
            res.json({ success: false, message: "User does not exist" });
        }
        else {
            res.json({ userName: userCoordinate.userName, success: true, message: "User exists" });
        }
			
			
	
    });

});


// route middleware to verify a token
apiRoutes.use(function(req, res, next) {

	// check header or url parameters or post parameters for token
	var token = req.body.token || req.query.token || req.headers['x-access-token'];

    var args = {
		headers: { "Content-Type": "application/json","x-access-token": token}
	};
	 
    httpClient.get("http://vauthapp.herokuapp.com/authapp/userinfo", args, function (userInfo, response) {
		
	
			if(userInfo.success)
			{
				// if everything is good, save to request for use in other routes
				req.userInfo = userInfo;    
				next();
			}
			else
			{
				return res.json({ success: false, message: 'Failed to authenticate token.' });
			}

	});

	
});

 apiRoutes.get('/', function(req, res) {
  res.json({ message: 'Welcome to the coolest API on earth!' });
});

  

/* apiRoutes.post('/updatecoordinates', function(req, res) {
  UserCoordinate.findOne({userName : req.userInfo.userName}, function(err, userCoordinate) {
  
	if (err) res.json({ success: false, message:err });
    userCoordinate.name=req.userInfo.name,
	userCoordinate.longitude= req.body.longitude;
	userCoordinate.latitude= req.body.latitude;
	userCoordinate.save(function(err) {
		if (err) res.json({ success: false, message:err });
		console.log('Coordinate saved successfully');
		io.emit('coordinate_changed', "Changed");
		res.json({ success: true });
	});
	
  });
});   */ 

apiRoutes.delete('/users/:userName', function (req, res) {
    UserCoordinate.findOne({ userName: req.params.userName }, function (err, selecteduser) {
        
        if (err) res.json({ success: false, message: err });

        else if (!selecteduser) {
            res.json({ success: false, message: "User not found" });
        }
        else {
            selecteduser.remove(function (err) {
                if (err) res.json({ success: false, message: "Error deleting user" });

                deleteVehicles(req.params.userName, function (resultVehicle) {
                    deleteFavouritePlaces(req.params.userName, function (result) {                        
                        res.json({ success: true });
                    });
                });      
                
            });

            
        }

    });
});

apiRoutes.put('/users', function (req, res) {
    UserCoordinate.findOne({ userName : req.body.userName }, function (err, userCoordinate) {
        if (err) res.json({ success: false, message: err });
        
        if (userCoordinate != null) {
            userCoordinate.firstName = req.body.firstName;
            userCoordinate.lastName = req.body.lastName;
            userCoordinate.email = req.body.email;
            userCoordinate.profileImage = req.body.profileImage;
            userCoordinate.gender = req.body.gender;
            userCoordinate.resetPasswordGuid = req.body.resetPasswordGuid;
			userCoordinate.mobileNo = req.body.mobileNo;
            userCoordinate.save(function (err) {
                if (err) res.json({ success: false, message: err });
                
                console.log('User data successfully updated');
                io.emit('coordinate_changed', "Changed");
                res.json({ success: true });
            });
        }
	
        else {
            res.json({ success: false, message: 'User does not exist' });
        }
	
	
    });
});

function deleteVehicles(useName, resultCallback) {
    UserVehicle.find({ userName : useName }, function (err, userVehicles) {
        
        if (err) resultCallback({ success: false, message: err });
        
        if (userVehicles.length != 0) {
            userVehicles.forEach(function (userVehicle) {
                userVehicle.remove(function (err) {
                    if (err) { resultCallback({ success: false, message: "Error deleting vehicle" }); return; }                
                    
                })
            });
            resultCallback({ success: true });        
        }
        else {
            resultCallback({ success: false, message: 'No User Vehicle details found' });
        }
    });
}

function deleteFavouritePlaces(useName, resultCallback) {
    UserFavouritePlace.find({ userName : useName }, function (err, userfavPlaces) {
        
        if (err) resultCallback({ success: false, message: err });        
        
        
        if (userfavPlaces.length != 0) {
            userfavPlaces.forEach(function (userfavPlace) {
                userfavPlace.remove(function (err) {
                    if (err) resultCallback({ success: false, message: "Error deleting favourite place" });                    
                })
            });
            resultCallback({ success: true });
        }
        else {
            resultCallback({ success: false, message: 'No User favourite places found' });
        }
    });
}

apiRoutes.get('/logoutnotificationself/:userName', function (req, res) {	
		
		var notificationData = {};
		notificationData.userName = req.params.userName;
		//notificationData.id = saved.id;
		notificationData.title = "You have been logged out. Please login again to resume your drive";
		
		urbanAirshipClient.sendNotification(notificationData, function (notificationSentStatus) {
			console.log('To self: ' + notificationSentStatus.message);	
			res.json({ success: true });
		});			
	});



apiRoutes.get('/logoutnotificationconnections/:userName/:userType', function (req, res) {	
	getConnections(req.params.userName, req.params.userType, function (connections) {	
		if (connections != null && connections.length != 0) {
            connections.forEach(function (connection) {
				if(req.params.userType == "Driver"){
					var notificationData = {};
					notificationData.userName = connection.userName;
					//notificationData.id = saved.id;
					notificationData.title = "Driver " + connections.driverUserName + " has been logged out. Please contact the driver to resume the ride.";
		
					urbanAirshipClient.sendNotification(notificationData, function (notificationSentStatus) {
						console.log('To rider: ' + notificationSentStatus.message);						
					});
					
				}
				else{
					var notificationData = {};
					notificationData.userName = connection.driverUserName;
					//notificationData.id = saved.id;
					notificationData.title = "Driver " + connections.userName + " has been logged out. Please contact the driver to resume the ride.";
		
					urbanAirshipClient.sendNotification(notificationData, function (notificationSentStatus) {
						console.log('To driver: ' + notificationSentStatus.message);						
					});					
				}
            }); 
			res.json({ success: true });			
        }
		else{
			res.json({ success: false, message: 'No connections to send notifications' });
		}
	});
});


 function getConnections(userName, userType, resultCallback){
 
	if(userType == 'Driver'){
	
		RideHistory.find({ $and: [ {driverUserName : userName}, {requestStatus : { $in: [ 1, 2 ] }} ]}, function (err, connections) {
        
			if (err) resultCallback(null);   
        
			if (connections.length != 0) {            
				resultCallback(connections);
			}
			else {
				resultCallback(null);
			}
		});
	}
	else{
		RideHistory.find({ $and: [{userName : userName}, {requestStatus : { $in: [ 1, 2 ] } }]}, function (err, connections) {
        
			if (err) resultCallback(null);       
        
			if (connections.length != 0) {            
				resultCallback(connections);
			}
			else {
				resultCallback(null);
			}
		});
	}
 }

apiRoutes.put('/users/:userName/location', function (req, res) {
    UserCoordinate.findOne({ userName : req.params.userName }, function (err, userCoordinate) {
        
        if (err) res.json({ success: false, message: err });

	    userCoordinate.longitude = req.body.longitude;
        userCoordinate.latitude = req.body.latitude;
        
        userCoordinate.save(function (err) {
            if (err) res.json({ success: false, message: err });
            
            console.log('Coordinate saved successfully');
            io.emit('coordinate_changed', "Changed");
            res.json({ success: true });
        });
	
    });
});

apiRoutes.put('/vehicles', function (req, res) {
    
    UserVehicle.findOne({ $and: [{ userName : req.body.userName }, { vehicleNumberPlate : req.body.previousVehicleNumberPlate }] }, function (err, userVehicle) {
        
        if (err) res.json({ success: false, message: err });
        
        if (userVehicle != null) {
            
            userVehicle.userName = req.body.userName;
            userVehicle.vehicleMake = req.body.vehicleMake;
            userVehicle.vehicleModel = req.body.vehicleModel;
            userVehicle.vehicleColor = req.body.vehicleColor;
            userVehicle.vehicleMaxPassengerCount = req.body.vehicleMaxPassengerCount;
            userVehicle.vehicleNumberPlate = req.body.vehicleNumberPlate;
            
            userVehicle.save(function (err) {
                if (err) res.json({ success: false, message: err });
                
                console.log('Updated User Vehicle successfully');
                io.emit('coordinate_changed', "Changed");
                res.json({ success: true, message: 'Updated User Vehicle successfully' });
            });
        }
        else {            
            var newUserVehicle = new UserVehicle({
                userName: req.body.userName,
                vehicleMake: req.body.vehicleMake,
                vehicleModel: req.body.vehicleModel,
                vehicleColor: req.body.vehicleColor,
                vehicleMaxPassengerCount: req.body.vehicleMaxPassengerCount,
                vehicleNumberPlate: req.body.vehicleNumberPlate
            });
            
            newUserVehicle.save(function (err) {
                if (err) res.json({ success: false, message: err });
                
                console.log('Inserted User Vehicle successfully');
                io.emit('coordinate_changed', "Changed");
                res.json({ success: true, message: 'Inserted User Vehicle successfully' });
            });
        }
    });
});


apiRoutes.put('/favouriteplaces', function (req, res) {
    
    UserFavouritePlace.findOne({ $and: [{ userName : req.body.userName }, { userGivenplaceName : req.body.previousUserGivenPlaceName }] }, function (err, userFavPlace) {
        
        if (err) res.json({ success: false, message: err });
        
        if (userFavPlace != null) {
            
            userFavPlace.userName = req.body.userName;
            userFavPlace.userGivenplaceName = req.body.userGivenplaceName;
            userFavPlace.placeName = req.body.placeName;
            userFavPlace.longitude = req.body.longitude;
            userFavPlace.latitude = req.body.latitude;
            userFavPlace.placeID = req.body.placeID;
            userFavPlace.placeReference = req.body.placeReference;
            
            userFavPlace.save(function (err) {
                if (err) res.json({ success: false, message: err });
                
                console.log('Updated User Favourite Places successfully');
                res.json({ success: true, message: 'Updated User Favourite Places successfully' });
            });
        }
        else {
            var newUserFavPlace = new UserFavouritePlace({
                userName: req.body.userName,
                userGivenplaceName: req.body.userGivenplaceName,
                placeName: req.body.placeName,
                longitude: req.body.longitude,
                latitude: req.body.latitude,
                placeID: req.body.placeID,
                placeReference: req.body.placeReference
            });
            
            newUserFavPlace.save(function (err) {
                if (err) res.json({ success: false, message: err });
                
                console.log('Inserted User Favourite Places successfully');
                res.json({ success: true, message: 'Inserted User Favourite Places successfully' });
            });
        }
    });
});

apiRoutes.put('/users/:userName/login_status', function (req, res) {
	UserCoordinate.findOne({ userName : req.params.userName }, function (err, userCoordinate) {
			
			if (err) res.json({ success: false, message: err });
			
			userCoordinate.isLoggedIn = req.body.loginStatus;
			
			userCoordinate.save(function (err) {
				if (err) res.json({ success: false, message: err });
				
				console.log('Updated user loggin status to '+req.body.loginStatus);
				res.json({ success: true });
			});
		
		});
});

apiRoutes.put('/users/:userName/destination', function (req, res) {
    UserCoordinate.findOne({ userName : req.params.userName }, function (err, userCoordinate) {
        
        if (err) res.json({ success: false, message: err });
		
		userCoordinate.destinationName = req.body.destinationName;
	    userCoordinate.destinationLongitude = req.body.destinationLongitude;
        userCoordinate.destinationLatitude = req.body.destinationLatitude;
        
        userCoordinate.save(function (err) {
            if (err) res.json({ success: false, message: err });
            
            console.log('Destination saved successfully');
            res.json({ success: true });
        });
	
    });
}); 

apiRoutes.get('/drivers', function (req, res) {    
    sendResponseByUserType(2, req, res);
});

apiRoutes.get('/riders', function (req, res) {
    
    sendResponseByUserType(1, req, res);

});

function sendResponseByUserType(userType,req,res)
{
    UserCoordinate.find({ userType : userType }, function (err, userCoordinates) {
        
        if (err) res.json({ success: false, message: err });
        
		else
		{
			 userMapper.mapUsersList(userCoordinates, function (userList) {
            var userDatas = userList;
            res.json({ userData: userDatas, success: true });
			});
		}

    });
}
// Notification Status : 1 - Sent, 2 - Delivered, 3 - Opened
function updateNotificationStatus(reqId,notificationStatus,resultCallback)
{
	RideHistory.update( {_id : reqId }, {$set:{notificationStatus : notificationStatus }},{ multi: false }, function (err, rideHistoryItems) {
        
        if (err) resultCallback({ success: false, message: err });
        
        else if (!rideHistoryItems) {
			resultCallback({ success: false, message: "Ride Histories Not Found" });
        }
        else {

                console.log('Updated notification status to '+notificationStatus+ " ["+reqId+"]");
				resultCallback({ success: true });

        }

    });
}

function updateNotificationStatus(reqId,notificationStatus)
{
	RideHistory.update( {_id : reqId }, {$set:{notificationStatus : notificationStatus }},{ multi: false }, function (err, rideHistoryItems) {
        
        if (err) console.log('Error updating notification status');
        
        else if (!rideHistoryItems) {
			console.log('Ride History not found');
        }
        else {

                console.log('Updated notification status to '+notificationStatus+ " ["+reqId+"]");				
        }

    });
}

apiRoutes.put('/ridehistory/notification_status/:id', function (req, res) {
	
	updateNotificationStatus(req.params.id,req.body.notificationStatus,function(data){
		
		if(data.success ==  true)
		{
			res.json({ success: true });
		}
		else
		{
			res.json({ success: data.success, message: data.message });
		}
	});
	
});

function updateUserRecentRequest(userName,requestId)
{
    UserCoordinate.findOne({ userName : userName }, function (err, user) {
        
        if (err) res.json({ success: false, message: err });
		
        user.resentRequest = requestId;
        user.save(function (err) {
            if (err) res.json({ success: false, message: err });
            
            console.log('Successfully Updated User request');
        });
    });
}

apiRoutes.get('/users', function(req, res) {

	UserCoordinate.find({}, function(err, userCoordinates) {
  
		if (err) res.json({ success: false, message:err });
		
		var userDatas=new Array();
		userCoordinates.forEach(function(userCoordinate){
			
            var userData = {};
                userData.gender=userCoordinate.gender;
				userData.userName=userCoordinate.userName;
				userData.email=userCoordinate.email;
				userData.firstName=userCoordinate.firstName;
				userData.lastName=userCoordinate.lastName;
				userData.longitude=userCoordinate.longitude;
				userData.latitude = userCoordinate.latitude;
				userData.mobileNo = userCoordinate.mobileNo;
				userData.userType = userCoordinate.userType;
				userData.destinationName = userCoordinate.destinationName;
				userData.destinationLongitude = userCoordinate.destinationLongitude;
				userData.destinationLatitude = userCoordinate.destinationLatitude;				
				userDatas.push(userData);
		});
		res.json({ userCoordinates: userDatas, success: true });
	});

});

apiRoutes.get('/users/:userName/vehicles', function (req, res) {
    
    UserVehicle.find({ userName : req.params.userName}, function (err, userVehicles) {
        
        if (err) res.json({ success: false, message: err });
        
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
            
            res.json({ userVehicles: userVehicleData, success: true, message: 'User Vehicle details retrived successfully' });
        }
        else {
            res.json({ success: false, message: 'No User Vehicle details found' });
        }
    });

});

apiRoutes.get('/users/:userName/favouriteplaces', function (req, res) {
    
    UserFavouritePlace.find({ userName : req.params.userName }, function (err, userFavouritePlaces) {
        
        if (err) res.json({ success: false, message: err });
        
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
            
            res.json({ userFavPlaces: userFavPlaceData, success: true, message: 'User Favourite Places retrived successfully' });
        }
        else {
            res.json({ success: false, message: 'No User Favourite Places found' });
        }
    });

});

apiRoutes.put('/users/:userName/type', function (req, res) {
	
	UserCoordinate.findOne({ userName : req.params.userName }, function (err, user) {
		
		if (err) res.json({ success: false, message: err });
		
		if (!user) {
			res.json({ success: false, message: "User Not Found" });
		}
		else {
			user.userType = req.body.userType;
			
			user.save(function (err) {
				if (err) res.json({ success: false, message: err });
				
				console.log('Updated User successfully');
				res.json({ success: true });
			});

		}

	});

   
});

//apiRoutes.get('/users/:queryString', function (req, res) {
	
//	var name = req.query.filter;
//	var value = req.params.queryString;
//	var query = {};
//	query[name] = value;

//	UserCoordinate.find(query, function (err, userCoordinates) {
		
//		if (err) res.json({ success: false, message: err });
		
//		var userDatas = new Array();
//		userCoordinates.forEach(function (userCoordinate) {
			
//			var userData = {};
//            userDatas = userMapper.mapSingleUser(userCoordinate);
//			userDatas.push(userData);
//		});
//		res.json({ userCoordinates: userDatas, success: true });
//	});

//});

// Request Ststus : 1-Pending, 2-Driver Accepted, 3-Driver Rejected , 4-RiderMet, 5-RideCompleted
apiRoutes.post('/ridehistory', function (req, res) {
	
	var newRideHistory = new RideHistory({
		userName: req.userInfo.userName,
		driverUserName: req.body.driverUserName,
		requestedTime: new Date(),
		requestStatus: 1,
		sourseName: req.body.sourseName,
		destinationName: req.body.destinationName,
		sourceLongitude: req.body.sourceLongitude,
		sourceLatitude: req.body.sourceLatitude,
		destinationLongitude: req.body.destinationLongitude,
		destinationLatitude: req.body.destinationLatitude,
		polyLine:req.body.polyLine
	});
	
	newRideHistory.save(function (err, saved) {
		if (err) res.json({ success: false, message: err });
		
		var notificationData = {};
		notificationData.userName = saved.driverUserName;
		notificationData.id = saved.id;
		notificationData.title="New ride request received to "+saved.destinationName;
		updateUserRecentRequest(saved.userName,saved.id);
		updateUserRecentRequest(saved.driverUserName,saved.id);
		urbanAirshipClient.sendNotification(notificationData, function (notificationSentStatus) {
			console.log(notificationSentStatus.message);
			updateNotificationStatus(saved.id,1);
		});
		
		console.log('Added new history item successfully');
		res.json({ success: true, id : saved._id });
	});

});


apiRoutes.get('/ridehistory/:queryString', function (req, res) {
	
	var name = req.query.filter;
	var value = req.params.queryString;
	var query = {};
	query[name] = value;
	
	RideHistory.find(query, function (err, rideHistoryItems) {
		
		if (err) res.json({ success: false, message: err });
		
		if (!rideHistoryItems) {
			res.json({ success: false, message: "Ride History Not Found" });
		}
		else {
			
			res.json({ success: true, data : rideHistoryItems });
		}

	});
});

apiRoutes.put('/ridehistory/status/:id', function (req, res) {
	
	RideHistory.findOne({ _id : req.params.id }, function (err, rideHistoryItem) {
		
		if (err) res.json({ success: false, message: err });
		
		if (!rideHistoryItem) {
			res.json({ success: false, message: "Ride History Not Found" });
		}
		else {
			rideHistoryItem.requestStatus = req.body.status;
			
			rideHistoryItem.save(function (err,saved) {
				if (err) res.json({ success: false, message: err });
				
				if(rideHistoryItem.requestStatus == 2)
				{
					var notificationData = {};
					notificationData.userName = saved.userName;
					notificationData.id = saved.id;
					notificationData.title="Your ride request "+ saved.destinationName +" accepted by driver";
					urbanAirshipClient.sendNotification(notificationData, function (notificationSentStatus) {
						console.log(notificationSentStatus.message);
					});
				}

				if(rideHistoryItem.requestStatus == 3)
				{
					var notificationData = {};
					notificationData.userName = saved.userName;
					notificationData.id = saved.id;
					notificationData.title="Your ride request to " + saved.destinationName + " rejected by driver";
					urbanAirshipClient.sendNotification(notificationData, function (notificationSentStatus) {
						console.log(notificationSentStatus.message);
					});
				}
				
				console.log('Updated history item status successfully');
				res.json({ success: true });
			});

		}

	});
});

apiRoutes.put('/ridehistory/:id/poly_line', function (req, res) {
    
    RideHistory.findOne({ _id : req.params.id }, function (err, rideHistoryItem) {
        
        if (err) res.json({ success: false, message: err });
        
        if (!rideHistoryItem) {
            res.json({ success: false, message: "Ride History Not Found" });
        }
        else {
            
            rideHistoryItem.polyLine = req.body.polyLine;
            
            rideHistoryItem.save(function (err) {
                if (err) res.json({ success: false, message: err });
                
                console.log('Successfully Updated The Poly Line');
                res.json({ success: true });
            });

        }

    });

   
});

apiRoutes.put('/ridehistory/:id/:timeCol', function (req, res) {
    
    RideHistory.findOne({ _id : req.params.id }, function (err, rideHistoryItem) {
        
        if (err) res.json({ success: false, message: err });
        
        if (!rideHistoryItem) {
            res.json({ success: false, message: "Ride History Not Found" });
        }
        else {
            
            rideHistoryItem[req.params.timeCol] = req.body.time;
            
            rideHistoryItem.save(function (err) {
                if (err) res.json({ success: false, message: err });
                
                console.log('Updated history item status successfully');
                res.json({ success: true });
            });

        }

    });

   
});



apiRoutes.put('/ridehistory/driver/:userName/ride/end', function (req, res) {
    
    RideHistory.update( {$and:[{ driverUserName : req.params.userName },{ requestStatus : 4 }]}, {$set:{requestStatus : 5 }},{ multi: true }, function (err, rideHistoryItems) {
        
        if (err) res.json({ success: false, message: err });
        
        if (!rideHistoryItems) {
            res.json({ success: false, message: "Ride Histories Not Found" });
        }
        else {

                console.log('Ended '+rideHistoryItems.nModified + ' rides');
                res.json({ success: true });

        }

    });

   
});

// apply the routes to our application with the prefix /api
app.use('/api', apiRoutes);


/* io.on('connection', function (socket) {
  socket.on('hi', function(msg){
    io.emit('hinew', msg);
  });
}); */
