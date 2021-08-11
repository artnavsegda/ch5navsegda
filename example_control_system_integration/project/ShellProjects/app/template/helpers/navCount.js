module.exports = function (obj) {
    var counter = 0;
    for (const iterator of obj) {
        if (typeof iterator.navigation !== "undefined") {
            counter++;
        }
    }
    return (counter)
};