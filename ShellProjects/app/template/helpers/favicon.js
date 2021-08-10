module.exports = function (path) {
    if(path.trim() !== ""){
        return "<link rel=\"icon\" type=\"image/x-icon\" href=\""+ path +"\" />";
    }
};