import IUser = require("../models/common/IUser");

interface IUserDAO {
    onSelectedUserDataReceived: (error: Error, userData: IUser) => void ;
    onUserAdded: (error: Error, status: boolean)=> void;
    addUser(user: IUser);
    updateUser(user: IUser);
    getSelectedUser(userName: string);
}

export = IUserDAO;