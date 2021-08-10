// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.
/*jslint es6 */

/*global CrComLib, loggerService, translateModule, serviceModule, utilsModule */
var templateHeaderModule = (function () {
  "use strict";

  let horizontalMenuSwiperThumb = document.getElementById("horizontal-menu-swiper-thumb");
  let navbarThumb = document.querySelector(".swiper-thumb");
  let themeTimer;
  let sidebarToggle = document.getElementById("sidebarToggle");
  let response = {};

  /**
   * This is public method to change the theme
   * @param {string} theme pass theme type like 'light-theme', 'dark-theme'
   */
  function changeTheme(theme) {
    clearTimeout(themeTimer);
    themeTimer = setTimeout(() => {
      let selectedTheme;
      let body = document.body;
      for (let i = 0; i < response.themes.length; i++) {
        body.classList.remove(response.themes[i].name);
      }
      let selectedThemeName = "";
      if (theme) {
        selectedThemeName = theme.trim();
      } else {
        selectedThemeName = response.selectedTheme.trim();
      }
      body.classList.add(selectedThemeName);
      selectedTheme = response.themes.find((tempObj) => tempObj.name.trim().toLowerCase() === selectedThemeName.trim().toLowerCase());
      
      if(document.getElementById("brandLogo")){
        document.getElementById("brandLogo").setAttribute("url", selectedTheme.brandLogo.url);
      }
    }, 500);
  }

  /**
   * This method will invoke on body click
   */
  document.body.addEventListener("click", function (event) {
    if (event.target.id === "sidebarToggle") {
      event.stopPropagation();
    } else {
      if (navbarThumb) {
        navbarThumb.classList.remove("open");
      }
      if (horizontalMenuSwiperThumb) {
        horizontalMenuSwiperThumb.classList.remove("open");
      }
      if (sidebarToggle) {
        sidebarToggle.classList.remove("active");
      }
    }
  });

  /**
   * This is public method to toggle left navigation sidebar
   */
  function toggleSidebar() {
    if (sidebarToggle) {
      sidebarToggle.classList.toggle("active");
    }
    if (navbarThumb) {
      navbarThumb.classList.toggle("open");
    }
  }

  /**
   * Load the emulator, theme, default language and listeners
   */
  function pageInit(projectConfigResponse) {
    response = projectConfigResponse;
    changeTheme();
    /* Note: You can uncomment below line to enable remote logger.
       * Refer below documentation link to know more about remote logger.
       * https://sdkcon78221.crestron.com/sdk/Crestron_HTML5UI/Content/Topics/UI-Remote-Logger.htm
       */
    // initializeLogger(serverIPAddress, serverPortNumber);
  }

  /**
   * Initialize remote logger
   * @param {string} hostName - docker server IPaddress / Hostname
   * @param {string} portNumber - docker server Port number
   */
  function initializeLogger(hostName, portNumber) {
    setTimeout(() => {
      loggerService.setRemoteLoggerConfig(hostName, portNumber);
    });
  }

  /**
   * All public method and properties exporting here
   */
  return {
    pageInit: pageInit,
    toggleSidebar: toggleSidebar,
    changeTheme: changeTheme
  };

})();
