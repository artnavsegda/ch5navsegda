module.exports = function (sTheme, themes) {
    var imageAttributes = '';
    themes.forEach(function (elem) {
        if (sTheme === elem.name) {
            if (elem.brandLogo !== "undefined") {
                for (var prop in elem.brandLogo) {
                    if (elem.brandLogo[prop] !== "") {
                        imageAttributes += prop + '="' + elem.brandLogo[prop] + '" ';
                    }
                }
            }
        }
    });
    return imageAttributes;
};
