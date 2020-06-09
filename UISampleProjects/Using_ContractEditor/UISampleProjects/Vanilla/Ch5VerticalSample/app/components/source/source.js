/*jslint es6 */
/*global document, window, CrComLib, appModule, setTimeout */
var sourceModule = (function () {
    'use strict';
    /**
     * All public and local(prefix '_') properties
     */
    let _sourceItemSize;

    /**
     * This is private method, it invokes on tile click.
     * @param {number} idx is current index of element for active state
     */
    function _addSourceItemClickListener(idx) {
        let itemElem = document.querySelector('.source-btn-' + idx);
        itemElem.addEventListener('click', function () {
            CrComLib.subscribeState('n', 'SourceList.NumberOfSources', function (numBtn) {
                let i = 0;
                while (i <= numBtn) {
                    CrComLib.publishEvent('b', `SourceList.Sources[${i}].SetSourceSelected`, false);
                    i += 1;
                }
            });
            CrComLib.publishEvent('b', `SourceList.Sources[${idx}].SetSourceSelected`, true);
        });
    }

    /**
     * Using this method we getting translated data of source tiles
     * @param {object} srcObj is translated data of source tiles
     */
    function updateTranslateObj(srcObj) {
        // published source navigation size
        if (_sourceItemSize !== srcObj.length) {
            _sourceItemSize = srcObj.length;
            CrComLib.publishEvent('n', 'SourceList.NumberOfSources', _sourceItemSize);
        }

        if (srcObj.length) {
            srcObj.forEach(function (src, idx) {
                CrComLib.publishEvent('s', `SourceList.Sources[${idx}].NameOfSource`, src.title);
                CrComLib.publishEvent('s', `SourceList.Sources[${idx}].IconClassOfSource`, src.icon);
                _addSourceItemClickListener(idx);
            });
        }
    }

    /**
     * All method is concating in one method
     */
    function _sourcesInit() {}

    /**
     * All public or private methods which need to call on init
     */
    let sourcePage = document.querySelector('.source-page');
    sourcePage.addEventListener('afterLoad', _sourcesInit);

    /**
     * All public method and properties exporting here
     */
    return {
        updateTranslateObj: updateTranslateObj
    };
}());