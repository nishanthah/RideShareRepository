// =======================
// get the packages we need ============
// =======================

var express     = require('express');
var app         = express();
var bodyParser  = require('body-parser');
var morgan      = require('morgan');
var mongoose    = require('mongoose');
var HttpClient = require('node-rest-client').Client;
var urbanAirshipClient = require("./urban_airship_client");
var userMapper = require("./helpers/user_mapper");
var httpClient = new HttpClient();

var jwt    = require('jsonwebtoken'); // used to create, sign, and verify tokens
var config = require('./config'); // get our config file
var UserCoordinate   = require('./app/models/user'); // get our mongoose model
var RideHistory = require('./app/models/ride_history.js'); // RideHistory mongoose model

// =======================
// configuration =========
// =======================
var port = process.env.PORT || 8079; // used to create, sign, and verify tokens
mongoose.connect(config.database); // connect to database
app.set('superSecret', config.secret); // secret variable

// use body parser so we can get info from POST and/or URL parameters
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

// use morgan to log requests to the console
app.use(morgan('dev'));

var io = require('socket.io').listen(app.listen(port));

console.log('API started at http://localhost:' + port);
// API ROUTES -------------------

// get an instance of the router for api routes
var apiRoutes = express.Router(); 

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

apiRoutes.post('/users', function(req, res) {
  UserCoordinate.findOne({userName : req.body.userName}, function(err, userCoordinate) {
  
	if (err) res.json({ success: false, message:err });

	if(userCoordinate != null)
	{
		
		userCoordinate.firstName= req.body.firstName;
		userCoordinate.lastName= req.body.lastName;
		userCoordinate.longitude= req.body.longitude;
		userCoordinate.latitude= req.body.latitude;
		userCoordinate.userType = req.body.userType;
		userCoordinate.mobileNo = req.body.mobileNo;
		userCoordinate.save(function(err) {
			if (err) res.json({ success: false, message:err });

			console.log('User data successfully updated');
			io.emit('coordinate_changed', "Changed");
			res.json({ success: true });
		});
	}
	
	else
	{
		var newUserCoordinate= new UserCoordinate({ 
			userName:req.body.userName,
			email:req.body.email,
			firstName:req.body.firstName,
			lastName:req.body.lastName,
			longitude: req.body.longitude,
			latitude: req.body.latitude,
			userType: req.body.userType,
			mobileNo: req.body.mobileNo
		});
		
		newUserCoordinate.save(function(err) {
			if (err) res.json({ success: false, message:err });

			console.log('User added successfully');
			io.emit('coordinate_changed', "Changed");
			res.json({ success: true });
		});
	}
	
	
  });
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
        
        var userDatas = userMapper.mapUsersList(userCoordinates);
        
        res.json({ userData: userDatas, success: true });
    });
}

apiRoutes.get('/users/:userName', function(req, res) {

  
	UserCoordinate.findOne({userName : req.params.userName}, function(err, userCoordinate) {
  
			if (err) res.json({ success: false, message:err });
			
			if(!userCoordinate)
			{
				res.json({ success: false, message:"User Not Found" });				
			}
			else {
                var userData = userMapper.mapSingleUser(userCoordinate);
				res.json({ userData: userData, success: true });
			}
			
			
	
	});

}); 

apiRoutes.get('/users', function(req, res) {

	UserCoordinate.find({}, function(err, userCoordinates) {
  
		if (err) res.json({ success: false, message:err });
		
		var userDatas=new Array();
		userCoordinates.forEach(function(userCoordinate){
			
				var userData={};
				userData.userName=userCoordinate.userName;
				userData.email=userCoordinate.email;
				userData.firstName=userCoordinate.firstName;
				userData.lastName=userCoordinate.lastName;
				userData.longitude=userCoordinate.longitude;
				userData.latitude = userCoordinate.latitude;
				userData.mobileNo = userCoordinate.mobileNo;
				userData.userType = userCoordinate.userType;
				userDatas.push(userData);
		});
		res.json({ userCoordinates: userDatas, success: true });
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
		notificationData.driverUserName = saved.driverUserName;
		notificationData.id = saved.id;
		notificationData.sourseName = saved.sourseName;
		notificationData.sourceLongitude = saved.sourceLongitude;
		notificationData.sourceLatitude = saved.sourceLatitude;
		notificationData.destinationName = saved.destinationName;
		notificationData.destinationLongitude = saved.destinationLongitude;
		notificationData.destinationLatitude = saved.destinationLatitude;
        notificationData.polyLine= saved.polyLine
		urbanAirshipClient.sendNotification(notificationData, function (notificationSentStatus) {
			console.log(notificationSentStatus.message);
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
			
			rideHistoryItem.save(function (err) {
				if (err) res.json({ success: false, message: err });
				
				console.log('Updated history item status successfully');
				res.json({ success: true });
			});

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