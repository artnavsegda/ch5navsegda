// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.
const fs = require("fs"); // global object - always available
const process = require("process"); // global object - always available
const fsExtra = require("fs-extra");

process.env["NODE_CONFIG_DIR"] = "./shell-utilities/config/";
const config = require("config");

const Enquirer = require('enquirer');
const enquirer = new Enquirer();

const utils = require("../helpers/utils");
const logger = require("../helpers/logger");
const projectConfig = require("../project-config/project-config");
const componentHelper = require("../helpers/components");
const namingHelpers = require("../helpers/naming-helpers");

const generatePage = {

  CONFIG_FILE: config.generatePage,
  outputResponse: {},
  processArgs: [],

  /**
   * Public Method 
   */
  run() {
    this.processArgs = componentHelper.processArgs();
    if (this.processArgs["help"] === true) {
      componentHelper.displayHelp(this.CONFIG_FILE.templatesPath + "help.template");
    } else {
      this.generatePage();
    }
  },

  /**
   * Initialize
   */
  initialize() {
    this.outputResponse = {
      result: false,
      errorMessage: "",
      warningMessage: "",
      data: {
        pageName: "",
        menuOption: "",
        fileName: "",
        folderPath: ""
      }
    };
    if (this.processArgs.length === 0) {
      this.processArgs = componentHelper.processArgs();
    }
  },

  /**
   * Method for generating page
   * @param {*} processArgs 
   */
  async generatePage() {
    try {
      // Initialize
      this.initialize();

      // Pre-requisite validations
      this.checkPrerequisiteValidations();

      // Verify input params
      this.verifyInputParams();

      // Ask details to developer based on input parameter validation
      await this.checkPromptQuestions();

      // Update project-config first (so that if this fails, we don't worry about file deletion). Next Delete Files
      await this.processRequest();

      // Clean up
      this.cleanUp();

    } catch (e) {
      if (e && utils.isValidInput(e.message)) {
        if (e.message.trim().toLowerCase() === 'error') {
          this.outputResponse.errorMessage = this.getText("ERRORS.SOMETHING_WENT_WRONG");
        } else {
          this.outputResponse.errorMessage = e.message;
        }
      } else {
        this.outputResponse.errorMessage = this.getText("ERRORS.SOMETHING_WENT_WRONG");
        logger.log(e);
      }
    }

    // Show output response
    this.logOutput();

    return this.outputResponse.result; // The return is required to validate in automation test case
  },

  /**
   * Check any validations that need to be done before verifying input parameters
   */
  checkPrerequisiteValidations() {
    // Nothing for this process
  },

  /**
   * Verify input parameters
   */
  verifyInputParams() {
    const tabDisplayText = this.getText("ERRORS.TAB_DELIMITER");
    if (utils.isValidInput(this.processArgs["name"])) {
      const validationResponse = this.validatePageName(this.processArgs["name"]);
      if (validationResponse === "") {
        this.outputResponse.data.pageName = this.processArgs["name"];
      } else {
        this.outputResponse.warningMessage += tabDisplayText + this.getText("ERRORS.PAGE_NAME_INVALID_ENTRY", validationResponse);
      }
    }

    if (utils.isValidInput(this.processArgs["menu"])) {
      const validationResponse = this.validateMenuOption(this.processArgs["menu"]);
      if (validationResponse === "") {
        this.outputResponse.data.menuOption = this.processArgs["menu"];
      } else {
        this.outputResponse.warningMessage += tabDisplayText + validationResponse + "\n";
      }
    }

    if (utils.isValidInput(this.outputResponse.warningMessage)) {
      logger.printWarning(this.getText("ERRORS.MESSAGE_TITLE", this.outputResponse.warningMessage));
    }
  },

  /**
   * Check if there are questions to be prompted to the developer
   */
  async checkPromptQuestions() {
    if (!utils.isValidInput(this.outputResponse.data.pageName)) {
      let pages = projectConfig.getAllPages();
      pages = pages.sort(utils.dynamicsort("asc", "pageName"));
      logger.log("pages", pages);
      const newPageNameToSet = this.loopAndCheckPage(pages);

      const questionsArray = [
        {
          type: "text",
          name: "pageName",
          initial: newPageNameToSet,
          hint: "",
          message: this.getText("VALIDATIONS.GET_PAGE_NAME"),
          validate: (compName) => {
            let output = this.validatePageName(compName);
            return output === "" ? true : output;
          }
        }];
      const response = await enquirer.prompt(questionsArray);
      if (!utils.isValidInput(response.pageName)) {
        throw new Error(this.getText("ERRORS.PAGE_NAME_EMPTY_IN_REQUEST"));
      }
      logger.log("  response.pageName: ", response.pageName);
      this.outputResponse.data.pageName = response.pageName;
    }

    if (!utils.isValidInput(this.outputResponse.data.menuOption)) {
      const questionsArray = [
        {
          type: 'select',
          name: 'menuOption',
          message: this.getText("VALIDATIONS.GET_ADD_TO_MENU_MESSAGE"),
          choices: [
            { message: this.getText("VALIDATIONS.GET_ADD_TO_MENU_YES"), hint: this.getText("VALIDATIONS.GET_ADD_TO_MENU_HINT_YES"), value: 'Y' },
            { message: this.getText("VALIDATIONS.GET_ADD_TO_MENU_NO"), hint: this.getText("VALIDATIONS.GET_ADD_TO_MENU_HINT_NO"), value: 'N' }
          ],
          initial: 0
        }
      ];
      const response = await enquirer.prompt(questionsArray);
      if (!utils.isValidInput(response.menuOption)) {
        throw new Error(this.getText("ERRORS.ADD_TO_MENU_EMPTY_IN_REQUEST"));
      }
      logger.log("  response.menuOption: ", response.menuOption);
      this.outputResponse.data.menuOption = response.menuOption;
    }

    let originalInputName = namingHelpers.convertMultipleSpacesToSingleSpace(this.outputResponse.data.pageName.trim().toLowerCase());
    this.outputResponse.data.pageName = namingHelpers.camelize(originalInputName);
    logger.log("  this.outputResponse.data.pageName: ", this.outputResponse.data.pageName);
    logger.log("  this.outputResponse.data.menuOption: ", this.outputResponse.data.menuOption);
    this.outputResponse.data.fileName = namingHelpers.dasherize(originalInputName);
    logger.log("  this.outputResponse.data.fileName: ", this.outputResponse.data.fileName);

    if (!utils.isValidInput(this.outputResponse.data.pageName) && !utils.isValidInput(this.outputResponse.data.menuOption)) {
      throw new Error(this.getText("ERRORS.PROGRAM_STOPPED_OR_UNKNOWN_ERROR"));
    } else if (!utils.isValidInput(this.outputResponse.data.pageName)) {
      throw new Error(this.getText("ERRORS.PAGE_NAME_EMPTY_IN_REQUEST"));
    } else if (!utils.isValidInput(this.outputResponse.data.menuOption)) {
      throw new Error(this.getText("ERRORS.ADD_TO_MENU_EMPTY_IN_REQUEST"));
    } else if (!utils.isValidInput(this.outputResponse.data.fileName)) {
      throw new Error(this.getText("ERRORS.SOMETHING_WENT_WRONG"));
    }
  },

  /**
   * Implement this component's main purpose
   */
  async processRequest() {
    if (projectConfig.isPageExistInJSON(this.outputResponse.data.pageName)) {
      throw new Error(this.getText("ERRORS.PAGE_EXISTS_IN_PROJECT_CONFIG_JSON"));
    } else {
      await this.createFolder().then(async (folderPathResponseGenerated) => {
        logger.log("  Folder Path (generated): " + folderPathResponseGenerated);
        this.outputResponse.data.folderPath = folderPathResponseGenerated;

        if (utils.isValidInput(this.outputResponse.data.folderPath)) {
          await this.createNewFile("html", "html.template", "");
          await this.createNewFile("js", "js.template", "");
          await this.createNewFile("scss", "scss.template", "");
          await this.createNewFile("json", "emulator.template", "-emulator");

          projectConfig.savePageToJSON(this.createPageObject(this.outputResponse.data.menuOption));
          this.outputResponse.result = true;
        } else {
          throw new Error(this.getText("ERRORS.ERROR_IN_FOLDER_PATH"));
        }
      }).catch((err) => {
        throw new Error(err);
      });
    }
  },

  /**
   * Clean up
   */
  cleanUp() {
    // Nothing to cleanup for this process
  },

  /**
   * Log Final Response Message
   */
  logOutput() {
    if (this.outputResponse.result === false) {
      logger.printError(this.outputResponse.errorMessage);
    } else {
      logger.printSuccess(this.getText("SUCCESS_MESSAGE", this.outputResponse.data.pageName, this.outputResponse.data.folderPath));
      if (this.outputResponse.data.menuOption === "Y") {
        logger.printSuccess(this.getText("SUCCESS_MESSAGE_NAVIGATION_ADDED"));
      }
      logger.printSuccess(this.getText("SUCCESS_MESSAGE_CONCLUSION"));
    }
  },

  /**
   * Create Folder for the Pages to be created
   */
  async createFolder() {
    let isFolderCreated = false;
    let fullPath = "";

    let folderPath = this.CONFIG_FILE.basePathForPages + this.outputResponse.data.fileName + "/";
    let folderPathSplit = folderPath.toString().split("/");
    for (let i = 0; i < folderPathSplit.length; i++) {
      logger.log(folderPathSplit[i]);
      if (folderPathSplit[i] && folderPathSplit[i].trim() !== "") {
        let previousPath = fullPath;
        fullPath += folderPathSplit[i] + "/";
        if (!fs.existsSync(fullPath)) {
          logger.log("Creating new folder " + folderPathSplit[i] + " inside the folder " + previousPath);
          fs.mkdirSync(fullPath, {
            recursive: true,
          });
          isFolderCreated = true;
        } else {
          logger.log(fullPath + " exists !!!");
        }
      }
    }

    // Check if Folder exists
    if (isFolderCreated === false) {
      // No folder is created. This implies that the page folder already exists.

      let files = fs.readdirSync(fullPath);

      if (files.length === 0) {
        // Check if folder is empty.
        return fullPath;
      } else {
        // listing all files using forEach
        for (let j = 0; j < files.length; j++) {
          let file = files[j];
          // If a single file exists, do not continue the process of generating page
          // If not, ensure to send message that folder is not empty but still created new files
          logger.log(file);
          let fileName = this.outputResponse.data.fileName;
          logger.log(fileName.toLowerCase() + ".html");
          if (file.toLowerCase() === fileName.toLowerCase() + ".html" || file.toLowerCase() === fileName.toLowerCase() + ".scss" || file.toLowerCase() === fileName.toLowerCase() + ".js") {
            throw new Error(this.getText("ERRORS.HTML_FILE_EXISTS", fileName.toLowerCase() + ".html", fullPath));
          }
        }
        return fullPath;
      }
    } else {
      return fullPath;
    }
  },

  /**
   * Create New File based on templates
   * @param {string} fileExtension - File extension - applicable values are .html, .js, .scss
   * @param {string} templateFile - Template file name
   */
  async createNewFile(fileExtension, templateFile, fileNameSuffix) {
    if (templateFile !== "") {
      let actualContent = fsExtra.readFileSync(this.CONFIG_FILE.templatesPath + templateFile);
      actualContent = utils.replaceAll(actualContent, "<%pageName%>", this.outputResponse.data.pageName);
      actualContent = utils.replaceAll(actualContent, "<%titlePageName%>", namingHelpers.capitalizeEachWordWithSpaces(this.outputResponse.data.pageName));
      actualContent = utils.replaceAll(actualContent, "<%stylePageName%>", namingHelpers.dasherize(this.outputResponse.data.pageName));
      actualContent = utils.replaceAll(actualContent, "<%copyrightYear%>", String(new Date().getFullYear()));
      actualContent = utils.replaceAll(actualContent, "<%fileName%>", this.outputResponse.data.fileName);

      let commonContentInGeneratedFiles = this.CONFIG_FILE.commonContentInGeneratedFiles;
      for (let i = 0; i < commonContentInGeneratedFiles.length; i++) {
        actualContent = utils.replaceAll(actualContent, "<%" + commonContentInGeneratedFiles[i].key + "%>", commonContentInGeneratedFiles[i].value);
      }

      const completeFilePath = this.outputResponse.data.folderPath + this.outputResponse.data.fileName + fileNameSuffix + "." + fileExtension;
      fsExtra.writeFileSync(completeFilePath, actualContent);
      // Success case, the file was saved
      logger.log("File contents saved!");
    }
  },

  /**
   * Creates the page object in project-config.json.
   * If the menuOrientation is horizontal, then iconPosition is set to bottom by default.
   * If the menuOrientation is vertical, then iconPosition is set to empty by default.
   * If the menuOrientation is none, then iconPosition and iconUrl are set to empty.
   */
  createPageObject() {
    const allowNavigation = utils.convertStringToBoolean(this.outputResponse.data.menuOption);
    let pageObject = {
      "pageName": this.outputResponse.data.pageName,
      "fullPath": this.outputResponse.data.folderPath,
      "fileName": this.outputResponse.data.fileName + '.html',
      "standAloneView": !allowNavigation,
      "pageProperties": {
        "class": ""
      }
    };
    if (allowNavigation === true) {
      const projectConfigJSON = projectConfig.getJson();
      if (projectConfigJSON.menuOrientation === 'horizontal') {
        pageObject.navigation = {
          "sequence": projectConfig.getHighestNavigationSequence() + 1,
          "label": this.outputResponse.data.pageName.toLowerCase(),
          "isI18nLabel": false,
          "iconClass": "",
          "iconUrl": "./app/project/assets/img/navigation/page.svg",
          "iconPosition": "bottom"
        };
      } else if (projectConfigJSON.menuOrientation === 'vertical') {
        pageObject.navigation = {
          "sequence": projectConfig.getHighestNavigationSequence() + 1,
          "label": this.outputResponse.data.pageName.toLowerCase(),
          "isI18nLabel": false,
          "iconClass": "",
          "iconUrl": "./app/project/assets/img/navigation/page.svg",
          "iconPosition": ""
        };
      } else {
        pageObject.navigation = {
          "sequence": projectConfig.getHighestNavigationSequence() + 1,
          "label": this.outputResponse.data.pageName.toLowerCase(),
          "isI18nLabel": false,
          "iconClass": "",
          "iconUrl": "",
          "iconPosition": ""
        };
      }
    }
    return pageObject;
  },

  /**
   * Loop and check the next valid page to set
   * @param {*} pages 
   */
  loopAndCheckPage(pages) {
    let pageFound = false;
    let newPageNameToSet = "";
    let i = 1;
    do {
      newPageNameToSet = "Page" + i;
      pageFound = false;
      for (let j = 0; j < pages.length; j++) {
        if (pages[j].pageName.trim().toLowerCase() === newPageNameToSet.toString().toLowerCase()) {
          pageFound = true;
          break;
        }
      }
      i++;
    }
    while (pageFound === true);
    return newPageNameToSet;
  },

  /**
   * Method to validate Page Name
   * @param {string} pageName
   */
  validatePageName(pageName) {
    logger.log("pageName to Validate", pageName);
    if (utils.isValidInput(pageName)) {
      pageName = String(pageName).trim();
      if (pageName.length < this.CONFIG_FILE.minLengthOfPageName || pageName.length > this.CONFIG_FILE.maxLengthOfPageName) {
        return this.getText("ERRORS.PAGE_NAME_LENGTH", this.CONFIG_FILE.minLengthOfPageName, this.CONFIG_FILE.maxLengthOfPageName);
      } else {
        let pageValidity = new RegExp(/^[a-zA-Z][a-zA-Z0-9-_ $]*$/).test(pageName);
        if (pageValidity === false) {
          return this.getText("ERRORS.PAGE_NAME_MANDATORY");
        } else {
          let originalInputName = namingHelpers.convertMultipleSpacesToSingleSpace(pageName.trim().toLowerCase());
          originalInputName = namingHelpers.camelize(originalInputName);

          logger.log("  originalInputName: " + originalInputName);
          if (projectConfig.isPageExistInJSON(originalInputName)) {
            return this.getText("ERRORS.PAGE_EXISTS_IN_PROJECT_CONFIG_JSON");
          } else if (projectConfig.isWidgetExistInJSON(originalInputName)) {
            return this.getText("ERRORS.WIDGET_EXISTS_IN_PROJECT_CONFIG_JSON");
          } else if (this.checkPageNameForDisallowedKeywords(originalInputName, "startsWith") === true) {
            return this.getText("ERRORS.PAGE_CANNOT_START_WITH", this.getInvalidPageStartWithValues());
          } else if (this.checkPageNameForDisallowedKeywords(originalInputName, "equals") === true) {
            return this.getText("ERRORS.PAGE_DISALLOWED_KEYWORDS");
          } else {
            return "";
          }
        }
      }
    } else {
      return this.getText("ERRORS.PAGE_NAME_MANDATORY");
    }
  },

  /**
   * Gets the keywords that are not allowed for pages to start
   */
  getInvalidPageStartWithValues() {
    let output = "";
    for (let i = 0; i < config.templateNames.disallowed["startsWith"].length; i++) {
      output += "'" + config.templateNames.disallowed["startsWith"][i] + "', ";
    }
    output = output.trim();
    return output.substr(0, output.length - 1);
  },

  /**
   * Checks if the pagename has disallowed keywords
   * @param {*} pageName 
   * @param {*} type 
   */
  checkPageNameForDisallowedKeywords(pageName, type) {
    if (type === "startsWith") {
      for (let i = 0; i < config.templateNames.disallowed[type].length; i++) {
        if (pageName.trim().toLowerCase().startsWith(config.templateNames.disallowed[type][i].trim().toLowerCase())) {
          return true;
        }
      }
    } else if (type === "equals") {
      for (let i = 0; i < config.templateNames.disallowed[type].length; i++) {
        if (pageName.trim().toLowerCase() === config.templateNames.disallowed[type][i].trim().toLowerCase()) {
          return true;
        }
      }
    }
    return false;
  },

  /**
   * Validate Menu Option
   * @param {*} menuOption 
   */
  validateMenuOption(menuOption) {
    logger.log("menuOption to Validate", menuOption);
    if (utils.isValidInput(menuOption)) {
      menuOption = String(menuOption).trim().toLowerCase();
      if (menuOption === "y" || menuOption === "n") {
        return "";
      } else {
        return this.getText("ERRORS.ADD_TO_MENU_INVALID_ENTRY");
      }
    } else {
      return this.getText("ERRORS.ADD_TO_MENU_INVALID_ENTRY");
    }
  },
  
  /**
   * Get the String output from default.json file in config
   * @param {*} key 
   * @param  {...any} values 
   */
  getText(key, ...values) {
    const DYNAMIC_TEXT_MESSAGES = this.CONFIG_FILE.textMessages;
    return utils.getText(DYNAMIC_TEXT_MESSAGES, key, ...values);
  }

};

module.exports = generatePage;
