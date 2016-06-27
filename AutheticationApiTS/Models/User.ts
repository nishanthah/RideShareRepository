import IUser = require("./common/IUser");

class User implements IUser{
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    userName: string;
}

export = User;