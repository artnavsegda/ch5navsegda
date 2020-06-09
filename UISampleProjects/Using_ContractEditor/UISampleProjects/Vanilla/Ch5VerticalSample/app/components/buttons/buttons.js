/*jslint es6 */
/*global document, window, appModule, setTimeout */
var buttonsModule = (function () {
    'use strict';

    /**
     * All method is concating in one method
     */
    function buttonsInit() {
        appModule.addClassOnClick('.shadow-pulse-button:not([disabled])', 'shadow-pulse-button-once');
        appModule.addClassOnClick('.shadow-pulse-gradient-button:not([disabled])', 'shadow-pulse-gradient-button-once');
        appModule.addClassOnClick('.outline-animate-button:not([disabled])', 'outline-animate-button-once');
    }

    /**
     * All public or private methods which need to call on init
     */
    let buttonsPage = document.querySelector('.buttons-page');
    buttonsPage.addEventListener('afterLoad', buttonsInit);

    /**
     * All public method and properties exporting here
     */
    return {};
}());