// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.  
/*jslint es6 */
/*global  CrComLib webkit JSInterface */


// This function is temporary until CrComLib.isCrestronTouchscreen is added to the CrComLib library. 

(function () {
    'use strict';
    if (CrComLib.isCrestronTouchscreen === undefined) {
        console.log('CrComLib.isCrestronTouchscreen polyfill added');
        CrComLib.isCrestronTouchscreen = function () {
            if (window.navigator.userAgent.toLowerCase().includes("crestron")) {
                return true;
            }
            if (typeof(JSInterface) !== "undefined" && typeof(JSInterface.bridgeSendBooleanToNative) !== "undefined") {
                return true;
            }
            if (typeof(webkit) !== "undefined" && typeof(webkit.messageHandlers) != "undefined" 
                && typeof(webkit.messageHandlers.bridgeSendBooleanToNative) !== "undefined") {
                return true;
            }
            return false;
        }
    }
}());
