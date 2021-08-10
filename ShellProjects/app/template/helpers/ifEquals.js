module.exports = function(arg1, arg2, options) {
   return (arg1.toLowerCase() == arg2.toLowerCase()) ? options.fn(this) : options.inverse(this);
};
