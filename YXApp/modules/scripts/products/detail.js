define(function (require, exports, modules) {
    require("jquery");
    var Global = require("global"),
        doT = require("dot");
    var ObjectJS = {};

    ObjectJS.init = function () {
        ObjectJS.bindEvent();
    };

    ObjectJS.bindEvent = function () {
        $(".add-shopping").click(function () {
            doT.exec("template/product/shopping-list.html",function (template) {
                var innerHtml = template();
                innerHtml = $(innerHtml);
                $(".product-list .table-list .tr-header").after(innerHtml);
            });
        });
    };

    modules.exports = ObjectJS;
})