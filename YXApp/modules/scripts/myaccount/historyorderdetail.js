define(function (require, exports, modules) {
    require("jquery");
    var Global = require("global"),
        doT = require("dot");
    var ObjectJS = {};
    var CacheArr = new Array();

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

        $(".color-item").click(function () {
            var _this = $(this);
            if (!_this.hasClass('hover')) {
                $('.txt-model').val(0);
                $(".color-item").removeClass('hover');
                _this.addClass('hover');
                console.log(CacheArr);
            }
        });

        $(".menu-module").click(function () {
            var _this = $(this);
            if (!_this.hasClass('hover')) {
                $(".menu-module").removeClass('hover');
                _this.addClass('hover');
                $('.content-main div[data-module=' + _this.attr('id') + ']').show().siblings().hide();
            }
        });

        $(".txt-model").blur(function () {
            var _this = $(this);
            if ($(".color-item.hover").data(_this.data('name')) == _this.data('id')) {
                CacheArr[$(".color-item.hover").data('id')][$(".color-item.hover").data('id') + _this.data('id')].Quantity = _this.val();
                alert(CacheArr[$(".color-item.hover").data('id')][$(".color-item.hover").data('id') + _this.data('id')].Quantity);
            } else {
                var model = [];
                var modelDetail = {
                    Quantity: $(_this).val(),
                    SaleAttr: $(".color-box").data('id') + ',' + $(".attr-box").data('id'),
                    AttrValue: $(".color-item.hover").data('id') + ',' + _this.data('id'),
                    SaleAttrValue: $(".color-box").data('id') + ":" + $(".color-item.hover").data('id') + "," + $(".attr-box").data('id') + _this.data('id'),
                    Description: '[颜色:' + $(".color-item.hover").data('value') + ',尺码:' + _this.data('value') + ']'
                };
                model[$(".color-item.hover").data('id') + _this.data('id')] = modelDetail;
                CacheArr[$(".color-item.hover").data('id')] = model;
                $(".color-item.hover").data(_this.data('name'), _this.data('id'));
            }
        });
    };

    modules.exports = ObjectJS;
})