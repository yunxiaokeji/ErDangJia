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
            var _this=$(this);
            var details = [];
            for (var cacheKey in CacheArr) {
                var item = CacheArr[cacheKey];
                for (var itemKey in item) {
                    var detail = item[itemKey];
                    if (detail.Quantity > 0) {
                        details.push(detail);
                    }
                }
            }
            if (details.length > 0) {
                console.log(details);
                Global.post("/MyAccount/CreateOrder", {
                    zngcOrderID: _this.data('orderid'),
                    price: _this.data('price'),
                    detailsEntity: JSON.stringify(details),
                    zngcClientID: _this.data('clientid')
                }, function () {
                    alert("下单成功");
                });
            } else {
                alert("请输入下单数量");
            }

            //doT.exec("template/product/shopping-list.html",function (template) {
            //    var innerHtml = template();
            //    innerHtml = $(innerHtml);
            //    $(".product-list .table-list .tr-header").after(innerHtml);
            //});
        });

        $(".color-item").click(function () {
            /*切换之前处理*/
            var item = CacheArr[$('.color-item.hover').data('id')]
            if (item) {
                for (var temKey in item) {
                    if (item[temKey].Quantity > 0) {
                        $('.color-item.hover').addClass('has');
                        break;
                    }
                }
            }

            /*切换之后处理*/
            var _this = $(this);
            if (!_this.hasClass('hover')) {
                $('.txt-model').val(0);
                $(".txt-model").each(function () {
                    if (CacheArr[_this.data('id')]) {
                        if (CacheArr[_this.data('id')][_this.data('id') + $(this).data('id')]) {
                            if ((CacheArr[_this.data('id')][_this.data('id') + $(this).data('id')]).Quantity > 0) {
                                $(this).val((CacheArr[_this.data('id')][_this.data('id') + $(this).data('id')]).Quantity);
                            }
                        }
                    }
                });
                $(".color-item").removeClass('hover');
                _this.addClass('hover').removeClass('has');  
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
            } else {
                var model = [];
                var modelDetail = {
                    price: $('.add-shopping').data('price'),
                    Quantity: $(_this).val(),
                    SaleAttr: $(".color-box").data('id') + ',' + $(".attr-box").data('id'),
                    AttrValue: $(".color-item.hover").data('id') + ',' + _this.data('id'),
                    SaleAttrValue: $(".color-box").data('id') + ":" + $(".color-item.hover").data('id') + "," + $(".attr-box").data('id') + _this.data('id'),
                    Description: '[颜色:' + $(".color-item.hover").data('value') + '] [尺码:' + _this.data('value') + ']'
                };
                model[$(".color-item.hover").data('id') + _this.data('id')] = modelDetail;
                var data = CacheArr[$(".color-item.hover").data('id')];
                if (data == null) {
                    CacheArr[$(".color-item.hover").data('id')] = model;
                } else {
                    CacheArr[$(".color-item.hover").data('id')][$(".color-item.hover").data('id') + _this.data('id')] = modelDetail;
                }
                $(".color-item.hover").data(_this.data('name'), _this.data('id'));
            }
        });
    };

    modules.exports = ObjectJS;
})