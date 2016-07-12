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
            //doT.exec("template/product/shopping-list.html",function (template) {
            //    var innerHtml = template();
            //    innerHtml = $(innerHtml);
            //    $(".product-list .table-list .tr-header").after(innerHtml);
            //});
        });

        $(".menu-module").click(function () {
            var _this = $(this);
            if (!_this.hasClass('hover')) {
                $(".menu-module").removeClass('hover');
                _this.addClass('hover');
                $('.content-main div[data-module=' + _this.attr('id') + ']').show().siblings().hide();
            }
        });
    };

    modules.exports = ObjectJS;
})