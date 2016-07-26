"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var BaseResponse = require("./common/BaseResponse");
var AccessTokenResponse = (function (_super) {
    __extends(AccessTokenResponse, _super);
    function AccessTokenResponse() {
        _super.apply(this, arguments);
    }
    return AccessTokenResponse;
}(BaseResponse));
module.exports = AccessTokenResponse;
//# sourceMappingURL=AccessToken.js.map