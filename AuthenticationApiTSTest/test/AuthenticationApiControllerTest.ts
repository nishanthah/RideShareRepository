/// <reference path="../Scripts/typings/mocha/mocha.d.ts" />
//import AuthhenticationAPIController from '../'; 

import AuthhenticationAPIController from '../node_modules/test-type-script-api/';

//import * as _ from 'test-type-script-api';

describe('AuthhenticationAPIController', () => {
    var newuser: AuthhenticationAPIController;

  

    describe('#useraccount', () => {
        it('should create the user', () => {
            var result: Object = newuser.adduser("TestUser");
           
        });
    });
});

var assert = require('assert');
describe('Array', function() {
  describe('#indexOf()', function() {
    it('should return -1 when the value is not present', function() {
      assert.equal(-1, [1,2,3].indexOf(4));
    });
  });
});