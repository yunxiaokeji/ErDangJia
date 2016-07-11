define(function (require, exports, modules) {
    require("jquery");
    var Global = require("global"),
        doT = require("dot");
    var ObjectJS = {};

    ObjectJS.init = function () {
        ObjectJS.getProductList();
        ObjectJS.bindEvent();
    };

    ObjectJS.bindEvent = function () {
    };

    ObjectJS.getProductList = function () {
        doT.exec("template/product/list.html", function (template) {
            var innerHtml = template();
            innerHtml = $(innerHtml);
            $(".product-items").append(innerHtml);
        });
    };

    modules.exports = ObjectJS;
})