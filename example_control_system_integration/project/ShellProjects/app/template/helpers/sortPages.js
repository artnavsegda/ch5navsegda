module.exports = function (object, options) {
  var seqObject = {};
  var ret = "";

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
  function getAllNavigations(pages) {
    let navigations = pages.filter(function (pageObj) {
      return isValidObject(pageObj.navigation);
    });
    return navigations;
  }

  seqObject = getAllNavigations(object);

  seqObject.sort(dynamicsort("asc", "navigation", "sequence"));

  for (var i = 0, j = seqObject.length; i < j; i++) {
    ret = ret + options.fn(seqObject[i]);
  }
  return ret;
};
