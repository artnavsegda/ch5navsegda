module.exports = function (object, options) {
  var seqObject = {};
  var ret = "";
  
  /**
   * Is object empty
   * @param {object} input 
   */
  function isValidInput(input) {
    if (typeof input === 'number') {
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
    } else if (typeof input === 'undefined') {
      return false;
    } else {
      return false;
    }    
  }

  /**
   * Check whether object exists
   * @param {object} input 
   */
  function isValidObject(input) {
    if (!input || input === {} || !isValidInput(input)) {
      return false;
    } else {
      return true;
    }
  }

  /**
   * Get pages with navigation property 
   */
  function getAllStandAloneViewPages(pages) {
    let navigations = pages.filter(function (pageObj) {
      return (!isValidObject(pageObj.navigation) && pageObj.standAloneView === true);
    });
    return navigations;
  }

  seqObject = getAllStandAloneViewPages(object);

  for (var i = 0, j = seqObject.length; i < j; i++) {
    ret = ret + options.fn(seqObject[i]);
  }
  return ret;
};
