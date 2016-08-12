module.exports = function () {
    
    function mapUsersList(usersData) {

        var usersList = new Array();
        usersData.forEach(function (singleUser) {
            
            var userData = mapSingleUser(singleUser);
            usersList.push(userData);
        });
        return usersList;
    }
    
    function mapSingleUser(singleUser) {
        
        var user = {};
        user.userName = singleUser.userName;
        user.email = singleUser.email;
        user.firstName = singleUser.firstName;
        user.lastName = singleUser.lastName;
        user.mobileNo = singleUser.mobileNo;
        user.userType = singleUser.userType;
		user.profileImage = singleUser.profileImage;        
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
        return userData;
    }

    return {
        mapSingleUser: mapSingleUser,
        mapUsersList: mapUsersList
    };
}();