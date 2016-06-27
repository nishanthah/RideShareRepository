import BaseResponse = require("./common/BaseResponse");

class TokenPayload{
    userName: string;
    canAccessUserInfo: boolean;
}

export = TokenPayload;