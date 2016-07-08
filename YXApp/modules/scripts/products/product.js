define(function (require, exports, modules) {
    require("jquery");
    var Global = require("global");
    var ObjectJS = {};

    ObjectJS.init = function () {
        ObjectJS.bindEvent();
    };

    ObjectJS.bindEvent = function () {
        /*监视手机横评还是竖屏*/

    };

    modules.exports = ObjectJS;
})