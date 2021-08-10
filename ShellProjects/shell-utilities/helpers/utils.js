// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.  
"use strict";

const rimraf = require("rimraf");

const utils = {

  /**
   * Check if input is valid. Invalid are "", 0, {}, [], null, undefined.
   * @param {*} input 
   */
  isValidInput(input) {
    if(typeof input === 'undefined' || input === null) {
      return false;
    }else if (typeof input === 'number') {
      return true;
    } else if (typeof input === 'string') {
      if (input && input.trim() !== "") {
        return true;
      } else {
        return false;
      }
    } else if (typeof input === 'boolean') {
      return true;
    } else if (typeof input === 'object') {
      if (input) {
        return true;
      } else {
        return false;
      }
    } else {
      return false;
    }
  },

  /**
   * 
   * @param {*} input 
   */
  isValidObject(input) {
    // Polyfill is array check
    if (!Array.isArray) {
      Array.isArray = function(arg) {
        return Object.prototype.toString.call(arg) === '[object Array]';
      };
    }
    if (!input || !utils.isValidInput(input)) {
      return false;
    } else if(typeof (input) === "object" && !Array.isArray(input)) {
      return true;
    } else {
      return false;
    }
  },

  /**
   * Convert string to array
   * @param {*} inputArray 
   */
  convertArrayToString(inputArray, delimiter = ",") {
    if(typeof (delimiter) != "string") {
      delimiter = ', '; // forcing default
    }
    if(typeof (inputArray) === "string") {
      return inputArray; // since its of string type, return as is
    } else if (utils.isValidInput(inputArray) && inputArray.length > 0) {
      let output = "";
      for (let i = 0; i < inputArray.length; i++) {
        output += inputArray[i] + delimiter;
      }
      if (delimiter.trim() === "") {
        return output.trim();
      } else {
        return output.substr(0, output.length - delimiter.length).trim();
      }
    } else {
      return "";
    }
  },

  /**
   * Convert string to boolean
   * @param {*} input 
   */
  convertStringToBoolean(input) {
    if (this.isValidInput(input) && typeof (input) === "string") {
      if (input.trim().toLowerCase() === 'n') {
        return false;
      } else if (input.trim().toLowerCase() === 'y') {
        return true;
      } else {
        return false;
      }
    } else {
      return false
    }
  },

  /**
 * Delete directory by path
 * @param {string} directoryName
 */
  async deleteFolder(directoryName) {
    try {
      return await rimraf(directoryName);
    } catch (e) {
      return false;
    }
  },

  /**
 * Gets the text from the config default.json file.
 * @param {*} key 
 * @param  {...any} values 
 */
  getText(DYNAMIC_TEXT_MESSAGES, key, ...values) {
    try {
      let output = "";
      if (String(key).indexOf(".") !== -1) {
        const newArray = key.split(".");
        output = DYNAMIC_TEXT_MESSAGES[newArray[0]];
        for (let i = 1; i < newArray.length; i++) {
          output = output[newArray[i]];
        }
      } else {
        output = DYNAMIC_TEXT_MESSAGES[key];
      }
      if (values && values.length > 0) {
        for (let i = 0; i < values.length; i++) {
          output = utils.replaceAll(output, "{" + i + "}", values[i]);
        }
      }
      return output;
    } catch (e) {
      return key;
    }
  },

  /**
   * 
   * @param {*} order 
   * @param  {...any} property 
   */
  dynamicsort(order, ...property) {
    let sort_order = 1;
    if (order === "desc") {
      sort_order = -1;
    }
    return (a, b) => {
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
  },

  /**
   * 
   * @param {*} input 
   */
  deepCopy(input) {
    if(utils.isValidInput(input) && typeof (input) === 'object')
      return JSON.parse(JSON.stringify(input));
    else
      return {};
  },

  /**
   * 
   * @param {*} str 
   * @param {*} find 
   * @param {*} replace 
   */
  replaceAll(str, find, replace) {
    if (str && String(str).trim() !== "") {
      return String(str).split(find).join(replace);
    } else {
      return str;
    }
  }

};

module.exports = utils;