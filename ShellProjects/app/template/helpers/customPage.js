module.exports = function (pages, pageName, options) {
    let pagePath = "";
    for (var index = 0; index < pages.length; index++) {
        if (pages[index].navigation === undefined) {
            if(pageName === pages[index].pageName){
                pagePath = pages[index].fullPath + pages[index].fileName
            }
        }
    }
    return pagePath;
};