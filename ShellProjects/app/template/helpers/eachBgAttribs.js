module.exports = function (sTheme, themes) {
    var bgAttributes = '';
    themes.forEach(function (elem) {
        if (sTheme === elem.name) {
            if (elem.backgroundProperties !== "undefined") {
                for (var prop in elem.backgroundProperties) {

                    if (prop === "url") {
                        if (typeof elem.backgroundProperties.url === "object") {
                            elem.backgroundProperties.url = elem.backgroundProperties.url.join(" | ");
                        }
                    }
                    if (prop === "backgroundColor") {
                        if (typeof elem.backgroundProperties.backgroundColor === "object") {
                            elem.backgroundProperties.backgroundColor = elem.backgroundProperties.backgroundColor.join(' | ');
                        }
                    }

                    if (elem.backgroundProperties[prop] !== "") {
                        bgAttributes += prop + '="' + elem.backgroundProperties[prop] + '" ';
                    }
                }
            }

        }
    });
    return bgAttributes;
};
