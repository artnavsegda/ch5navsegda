// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.  
"use strict";
process.env["NODE_CONFIG_DIR"] = "./shell-utilities/config/";
const config = require("config");

const logger = {
  allowLogging: config.logger.allowLogging,

  LOG_LEVELS: {
    TRACE: 1,
    DEBUG: 2,
    INFO: 6,
    WARN: 9,
    ERROR: 13,
    FATAL: 20,
    OFF: 99,
  },

  logLevel: config.logger.logLevel, //this.LOG_LEVELS.INFO,

  Reset: "\x1b[0m",
  Bright: "\x1b[1m",
  Dim: "\x1b[2m",
  Underscore: "\x1b[4m",
  Blink: "\x1b[5m",
  Reverse: "\x1b[7m",
  Hidden: "\x1b[8m",

  FgBlack: "\x1b[30m",
  FgRed: "\x1b[31m",
  FgGreen: "\x1b[32m",
  FgYellow: "\x1b[33m",
  FgBlue: "\x1b[34m",
  FgMagenta: "\x1b[35m",
  FgCyan: "\x1b[36m",
  FgWhite: "\x1b[37m",

  BgBlack: "\x1b[40m",
  BgRed: "\x1b[41m",
  BgGreen: "\x1b[42m",
  BgYellow: "\x1b[43m",
  BgBlue: "\x1b[44m",
  BgMagenta: "\x1b[45m",
  BgCyan: "\x1b[46m",
  BgWhite: "\x1b[47m",

  SpaceChar: "%s",

  /**
   * 
   * @param {*} noOfLineBreaks 
   */
  linebreak(noOfLineBreaks) {
    let lineBreakText = "";
    if (noOfLineBreaks && noOfLineBreaks > 0) {
      for (var i = 0; i < noOfLineBreaks; i++) {
        lineBreakText += "\n";
      }
      console.log(lineBreakText);
    }
  },

  /**
   * 
   * @param  {...any} input 
   */
  log(...input) {
    if (this.allowLogging === true && this.logLevel <= this.LOG_LEVELS.DEBUG) {
      console.log(this.FgBlue, ...input, this.Reset);
    }
  },

  /**
   * 
   * @param  {...any} input 
   */
  warn(...input) {
    if (this.allowLogging === true && this.logLevel <= this.LOG_LEVELS.WARN) {
      console.warn(this.FgYellow, ...input, this.Reset);
    }
  },

  /**
   * 
   * @param  {...any} input 
   */
  error(...input) {
    if (this.allowLogging === true && this.logLevel <= this.LOG_LEVELS.ERROR) {
      console.error(this.FgRed, ...input, this.Reset);
    }
  },

  /**
   * 
   * @param  {...any} input 
   */
  info(...input) {
    if (this.allowLogging === true && this.logLevel <= this.LOG_LEVELS.INFO) {
      console.info(this.FgMagenta, ...input, this.Reset);
    }
  },

  /**
   * 
   * @param  {...any} input 
   */
  trace(...input) {
    if (this.allowLogging === true && this.logLevel <= this.LOG_LEVELS.TRACE) {
      console.trace(this.FgCyan, ...input, this.Reset);
    }
  },

  /**
   * 
   * @param  {...any} input 
   */
  printSuccess(...input) {
    console.info(this.FgGreen, ...input, this.Reset);
  },

  /**
   * 
   * @param  {...any} input 
   */
  printWarning(...input) {
    console.info(this.FgYellow, ...input, this.Reset);
  },

  /**
   * 
   * @param  {...any} input 
   */
  printError(...input) {
    console.error(this.FgRed, ...input, this.Reset);
  },

  /**
   * 
   * @param  {...any} input 
   */
  printLog(...input) {
    console.error(...input);
  },

  /**
   * Throw any error raised
   * @param {any} err
   */
  onErr(err) {
    this.error(err);
    throw err;
  }

};

module.exports = logger;