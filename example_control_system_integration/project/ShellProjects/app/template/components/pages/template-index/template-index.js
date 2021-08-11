// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.
/*jslint es6 */

/*global CrComLib, loggerService, translateModule, serviceModule, utilsModule, templateHeaderModule, templateContentModule, webXPanelModule */
var templateIndexModule = (function () {
  "use strict";

  let response = {};

  /**
  * This method is used to fetch project-config.json file
  */
  function getProjectConfigJSON() {
    return new Promise((resolve, reject) => {
      serviceModule.loadJSON("./assets/data/project-config.json", (dataResponse) => {
        response = JSON.parse(dataResponse);
        resolve(response);
      }, error => {
        reject(error);
      });
    });
  }

  /**
  * Load the emulator, theme, default language and listeners
  */
  function pageInit() {
    getProjectConfigJSON().then(projectConfigResponse => {
      translateModule.initializeDefaultLanguage().then(() => {
        templateHeaderModule.pageInit(projectConfigResponse);
        templateContentModule.pageInit(projectConfigResponse);
        serviceModule.initialize(projectConfigResponse);

        /* 
         * Initialize if WebXPanel is enabled in project-config.json
         */
        if(projectConfigResponse.useWebXPanel){
          webXPanelModule.connect(projectConfigResponse);
        }
      });
    });
  }

  window.addEventListener("orientationchange", function () {
    try {
      templateContentModule.setMenuActive();
    // eslint-disable-next-line no-empty
    } catch (e) {
    }
  }, false);
  
  document.addEventListener("DOMContentLoaded", pageInit);

  /**
  * All public method and properties exporting here
  */
  return {
    getProjectConfigJSON: getProjectConfigJSON
  };
})();

/**
 * Loader method is for spinner
 */
function loader() {
  "use strict";
  let spinner = document.getElementById("loader");
  setTimeout(function () {
    spinner.style.display = "none";
  }, 1000);
}

document.addEventListener("DOMContentLoaded", loader, false);