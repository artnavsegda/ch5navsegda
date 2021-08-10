// Copyright (C) 2020 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement
// under which you licensed this source code.
/* global CrComLib, serviceModule */
var translateModule = (function () {
  'use strict';
  /**
   * All public and local properties
   */
  let langData = [];
  let crComLibTranslator = CrComLib.translationFactory.translator;
  let currentLng = document.getElementById("currentLng");
  let defaultLng = "en";
  let languageTimer;
  let setLng = "en";

  /**
   * This is public method to fetch language data(JSON).
   * @param {string} lng is language code string like en, fr etc...
   */
  function getLanguage(lng) {
    if (!langData[lng]) {
      let output = {};
      loadJSON("./app/template/assets/data/translation/", lng).then((responseTemplate) => {
        output = mergeJSON(output, responseTemplate);
        loadJSON("./app/project/assets/data/translation/", lng).then((responseProject) => {
          output = mergeJSON(output, responseProject);
          langData[lng] = {
            translation: output,
          };
          setLanguage(lng);
        });
      }).catch((error) => {
        loadJSON("./app/project/assets/data/translation/", lng).then((responseProject) => {
          output = mergeJSON(output, responseProject);
          langData[lng] = {
            translation: output,
          };
          setLanguage(lng);
        });
      });
    } else {
      setLanguage(lng);
    }
  }

  function initializeDefaultLanguage() {
    return new Promise((resolve, reject) => {
      if (!langData[defaultLng]) {
        let output = {};
        loadJSON("./app/template/assets/data/translation/", defaultLng).then((responseTemplate) => {
          output = mergeJSON(output, responseTemplate);
          loadJSON("./app/project/assets/data/translation/", defaultLng).then((responseProject) => {
            output = mergeJSON(output, responseProject);
            langData[defaultLng] = {
              translation: output,
            };
            setLanguage(defaultLng);
            resolve();
          });
        }).catch((error) => {
          loadJSON("./app/project/assets/data/translation/", defaultLng).then((responseProject) => {
            output = mergeJSON(output, responseProject);
            langData[defaultLng] = {
              translation: output,
            };
            setLanguage(defaultLng);
            resolve();
          });
        });
      } else {
        setLanguage(defaultLng);
        resolve();
      }
    });
  }

  // function translateInstantMultipleKeys(...key) {
  //   try {
  //     let objOutput = langData[setLng].translation;
  //     for (let i = 0; i < key.length; i++) {
  //       objOutput = objOutput[key[i]];
  //     }
  //     if (objOutput) {
  //       return objOutput;
  //     } else {
  //       return key[key.length - 1];
  //     }
  //   } catch (e) {
  //     return key[key.length - 1];
  //   }
  // }

  // function translateInstant(keyInput) {
  //   try {
  //     let key = keyInput.split(".");
  //     return translateInstantMultipleKeys(key);
  //   } catch (e) {
  //     return keyInput[0];
  //   }
  // }

  /**
   * 
   * @param {String} keyInput 
   * @param {Object} values 
   */
  function translateInstant(keyInput, values) {
    try {
      return crComLibTranslator.t(keyInput, values);
    } catch (e) {
      return keyInput[0];
    }
  }

  function loadJSON(path, lng) {
    return new Promise((resolve, reject) => {
      serviceModule.loadJSON(path + lng + ".json", (response) => {
        if (response) {
          resolve(JSON.parse(response));
        } else {
          reject("FILE NOT FOUND");
        }
      }, error => {
        reject("FILE NOT FOUND");
      });
    });
  }

  /**
   * 
   * @param {*} object1 
   * @param {*} object2 
   */
  function jsonConcat(object1, object2) {
    for (let key in object2) {
      object1[key] = object2[key];
    }
    return object1;
  }

  /**
   * 
   * @param  {...any} args 
   */
  function mergeJSON(...args) {
    let target = {};
    // Merge the object into the target object

    //Loop through each object and conduct a merge
    for (let i = 0; i < args.length; i++) {
      target = merger(target, args[i]);
    }
    return target;
  }

  function merger(target, obj) {
    for (let prop in obj) {
      // eslint-disable-next-line no-prototype-builtins
      if (obj.hasOwnProperty(prop)) {
        if (Object.prototype.toString.call(obj[prop]) === '[object Object]') {
          // If we're doing a deep merge and the property is an object
          target[prop] = mergeJSON(target[prop], obj[prop]);
          // target = merger(target, obj[prop]);
        } else {
          // Otherwise, do a regular merge
          target[prop] = obj[prop];
        }
      }
    }
    return target;
  }

  /**
   * Set the language
   * @param {string} lng
   */
  function setLanguage(lng) {
    // update selected language
    crComLibTranslator.changeLanguage(lng);
    setLng = lng;
  }

  /**
   * This is private method to init ch5 i18next translate library
   */
  function initCh5LibTranslate() {
    CrComLib.registerTranslationInterface(crComLibTranslator, "-+", "+-");
    crComLibTranslator.init({
      fallbackLng: "en",
      language: currentLng,
      debug: true,
      resources: langData,
    });
  }

  /**
   * This is public method, it invokes on language change
   * @param {string} lng is language code string like en, fr etc...
   */
  function changeLang(lng) {
    clearTimeout(languageTimer);
    languageTimer = setTimeout(function () {
      if (lng !== defaultLng) {
        defaultLng = lng;
        // invoke language
        getLanguage(lng);
      }
    }, 500);
  }

  /**
   * 
   */
  function getTranslator() {
    return crComLibTranslator;
  }

  /**
   * All public or private methods which need to call on init
   */
  initCh5LibTranslate();

  /**
   * All public method and properties exporting here
   */
  return {
    initializeDefaultLanguage: initializeDefaultLanguage,
    getLanguage: getLanguage,
    changeLang: changeLang,
    currentLng: currentLng,
    defaultLng: defaultLng,
    getTranslator: getTranslator, 
    translateInstant: translateInstant
  };
})();
