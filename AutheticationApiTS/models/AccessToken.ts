import BaseResponse = require("./common/BaseResponse");

class AccessTokenResponse extends BaseResponse {
    token: string;
}

export = AccessTokenResponse;