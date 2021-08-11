// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.
var utilsModule = (function () {
  "use strict";

  function log(...input) {
    let allowLogging = true;
    if (allowLogging === true) {
      console.log(...input);
    }
  }

  function dynamicsort(order, ...property) {
    var sort_order = 1;
    if (order === "desc") {
      sort_order = -1;
    }
    return function (a, b) {
      if (property.length > 1) {
        let propA = a[property[0]];
        let propB = b[property[0]];
        for (let i = 1; i < property.length; i++) {
          propA = propA[property[i]];
          propB = propB[property[i]];
        }
        // a should come before b in the sorted order
        if (propA < propB) {
          return -1 * sort_order;
          // a should come after b in the sorted order
        } else if (propA > propB) {
          return 1 * sort_order;
          // a and b are the same
        } else {
          return 0 * sort_order;
        }
      } else {
        // a should come before b in the sorted order
        if (a[property] < b[property]) {
          return -1 * sort_order;
          // a should come after b in the sorted order
        } else if (a[property] > b[property]) {
          return 1 * sort_order;
          // a and b are the same
        } else {
          return 0 * sort_order;
        }
      }
    }
  }

  return {
    log: log,
    dynamicsort: dynamicsort
  };
})();
