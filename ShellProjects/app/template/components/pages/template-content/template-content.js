// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.
/*jslint es6 */

/*global CrComLib, loggerService, translateModule, serviceModule, utilsModule */
var templateContentModule = (function () {
  'use strict';

  let triggerview = document.querySelector(".triggerview");
  let horizontalMenuSwiperThumb = document.getElementById("horizontal-menu-swiper-thumb");
  let navbarThumb = document.querySelector(".swiper-thumb");
  let activeIndex = 0;
  let previousActiveIndex = -1;
  let sidebarToggle = document.getElementById("sidebarToggle");
  let response = {};

  /**
   * This is public method for bottom navigation to navigate to next page
   * @param {number} idx is current index for navigate to appropriate page
   */
  function navMenu(idx) {
    // idx !== activeIndex is very important - this disables crash of the UI code
    if (triggerview !== null && idx !== activeIndex) {
      triggerview.setActiveView(idx);
      setMenuActive();
    }
  }

  /**
   * This is public method for bottom navigation to set active state
   * @param {number} idx is current index for active state
   */
  function navActiveState(idx) {
    // If the previous and current index are same then return
    if (previousActiveIndex === idx) {
      return;
    }
    CrComLib.publishEvent("b", "active_state_class_" + String(previousActiveIndex), false);
    previousActiveIndex = idx;
    activeIndex = idx;
    navMenu(idx);

    setMenuActive();
    CrComLib.publishEvent("b", "active_state_class_" + String(idx), true);
  }

  function setMenuActive() {
    if (triggerview !== null) {
      if (response.menuOrientation === 'horizontal') { // || response.menuOrientation === 'vertical') {
        CrComLib.publishEvent("n", "scrollToMenu", activeIndex);
      }
    }
  }

  /**
   * This is public method for triggerview
   * @param {number} navItemSize is number of bottom navigation list
   */
  function triggerviewOnInit() {
    let responseArrayForNavPages = [];
    for (let i = 0; i < response.content.pages.length; i++) {
      if (response.content.pages[i].navigation) {
        responseArrayForNavPages.push(response.content.pages[i]);
      }
    }

    responseArrayForNavPages = responseArrayForNavPages.sort(utilsModule.dynamicsort("asc", "navigation", "sequence"));
    for (let i = 0; i < responseArrayForNavPages.length; i++) {
      const menu = document.getElementById("menu-list-id-" + i);
      if (menu) {
        if (typeof responseArrayForNavPages[i].navigation !== "undefined" && typeof responseArrayForNavPages[i].navigation.iconUrl !== "undefined") {
          menu.setAttribute("iconUrl", responseArrayForNavPages[i].navigation.iconUrl);
        }
        if (responseArrayForNavPages[i].navigation.isI18nLabel === true) {
          menu.setAttribute("label", translateModule.translateInstant(responseArrayForNavPages[i].navigation.label));
        } else {
          menu.setAttribute("label", responseArrayForNavPages[i].navigation.label);
        }
        menu.setAttribute("iconClass", responseArrayForNavPages[i].navigation.iconClass);
        if (response.menuOrientation === 'horizontal') {
          menu.setAttribute("iconPosition", responseArrayForNavPages[i].navigation.iconPosition);
        }
      }
    }

    let newIndex = -99;
    if (response.content.$defaultView && response.content.$defaultView !== "") {
      for (let i = 0; i < responseArrayForNavPages.length; i++) {
        if (responseArrayForNavPages[i].pageName.toString().trim().toUpperCase() === response.content.$defaultView.toString().trim().toUpperCase()) {
          newIndex = i;
          break;
        } else {
          newIndex = -1;
        }
      }
    } else {
      newIndex = 0;
    }

    if (newIndex === -99) {
      newIndex = 0;
      navActiveState(newIndex);
    } else if (newIndex === -1) {
      // Must be handled by Developer 
      // This condition occurs when a new page is added into the project-config.json which does not have navigation.
    } else {
      navActiveState(newIndex);
    }
  }

  /**
   * This is public method to show/hide bottom navigation in smaller screen
   */
  function openThumbNav() {
    horizontalMenuSwiperThumb.className += " open";
    event.stopPropagation();
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
    triggerviewOnInit();
    triggerview.addEventListener("select", function (event) {
      setTimeout(() => {
        activeIndex = event.detail;
        navActiveState(activeIndex);
      });
    });
  }

  /**
   * All public method and properties exporting here
   */
  return {
    pageInit: pageInit,
    toggleSidebar: toggleSidebar,
    openThumbNav: openThumbNav,
    navMenu: navMenu,
    setMenuActive: setMenuActive
  };

})();
