// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.
"use strict";

const fs = require("fs"); // global object - always available
const process = require("process"); // global object - always available
const utils = require("../helpers/utils");
const logger = require("../helpers/logger");

const COMPLETE_PARAMETERS = [
  {
    "key": "all",
    "type": "boolean",
    "default": true,
    "valueIfNotFound": false,
    "alias": ['--all']
  },
  {
    "key": "list",
    "type": "array",
    "default": [],
    "valueIfNotFound": [],
    "alias": ['-l', '--list']
  },
  {
    "key": "force",
    "type": "boolean",
    "default": true,
    "valueIfNotFound": false,
    "alias": ['-f', '--force']
  },
  {
    "key": "help",
    "type": "boolean",
    "default": true,
    "valueIfNotFound": false,
    "alias": ['-h', '--help']
  },
  {
    "key": "menu",
    "type": "string",
    "default": "",
    "valueIfNotFound": "",
    "alias": ['-m', '--menu']
  },
  {
    "key": "name",
    "type": "string",
    "default": "",
    "valueIfNotFound": "",
    "alias": ['-n', '--name']
  },
  {
    "key": "zipFile",
    "type": "string",
    "default": "",
    "valueIfNotFound": "",
    "alias": ['-z', '--zipFile']
  }
];

const componentsHelper = {

  displayHelp(path) {
    fs.readFile(path, { encoding: "utf-8" }, (err, data) => {
      if (!err) {
        logger.printLog(data);
      } else {
        throw logger.onErr(err);
      }
    });
  },

  processArgs() {
    const args = process.argv.slice(2);
    return this.processArgsAnalyze(args);
  },

  processArgsAnalyze(args) {
    const output = {};
    let arrayKey = null;
    let arrayParam = null;
    let continueProcess = false;
    args.forEach((val, index, array) => {
      if (String(val).indexOf('--') === 0 || String(val).indexOf('-') === 0) {
        let optionName = null;
        if (String(val).indexOf('--') === 0) {
          optionName = val.replace('--', '');
        } else if (String(val).indexOf('-') === 0) {
          optionName = val.replace('-', '');
        }
        const paramObj = COMPLETE_PARAMETERS.find((tempObj) => tempObj.alias.map(v => v.toLowerCase()).includes(val.trim().toLowerCase()));
        if (paramObj) {
          arrayKey = paramObj.key;
          arrayParam = paramObj.type;
          if (arrayParam === "array") {
            output[arrayKey] = [];
          } else if (arrayParam === "boolean" || arrayParam === "string") {
            output[arrayKey] = paramObj.default;
          }
          continueProcess = true;
        } else {
          // Currently we don't do anything here. Some thoughts could be to push the data as a value similar to the
          // else statement below. Or we could nullify arrayKey and arrayParam.
        }
      } else {
        if (arrayKey != null) {
          if (arrayParam === "array") {
            output[arrayKey].push(val);
          } else if (arrayParam === "boolean" || arrayParam === "string") {
            if (continueProcess === true) {
              output[arrayKey] = val;
              continueProcess = false;
            }
          }
        }
      }
    });
    logger.log("processArgs Before", output);
    for (let i = 0; i < COMPLETE_PARAMETERS.length; i++) {
      if (!output[COMPLETE_PARAMETERS[i]["key"]]) {
        output[COMPLETE_PARAMETERS[i]["key"]] = COMPLETE_PARAMETERS[i]["valueIfNotFound"];
      }
    }
    logger.log("processArgs After", output);
    return output;
  }

};

module.exports = componentsHelper;
