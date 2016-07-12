define(function (require, exports, modules) {
    require("jquery");
    var Global = require("global"),
        doT = require("dot");
    var ObjectJS = {};
    var Params = {
        zngcClientID: "",
        pageSize: 1,
        pageIndex: 1
    };
    var IsLoadding = false;
    var PageCount = 0;

    ObjectJS.init = function () {
        ObjectJS.getProductList();
        ObjectJS.bindEvent();
    };

    ObjectJS.bindEvent = function () {
        $(window).bind("scroll", function () {
            var bottom = $(document).height() - document.documentElement.scrollTop - document.body.scrollTop - $(window).height();

            if (bottom <= 20) {
                //$("#tableLoad").attr("class", "");
                setTimeout(function () {
                    if (!IsLoadding) {
                        Params.pageIndex++;
                        if (Params.pageIndex <= PageCount) {
                            ObjectJS.getProductList();
                        } else {
                            if ($('.last-page-tip').length == 0) {
                                $(".product-items").append("<div class='last-page-tip center'>已经是最后一页了</div>");
                            }
                        }
                    }
                }, 1000);
            }
        });
    };

    ObjectJS.getProductList = function () {
        IsLoadding = true;
        Global.post("/Product/GetProductList", Params, function (data) {
            IsLoadding = false;
            PageCount = data.item.pageCount;
            var orders = data.item.orders;
            if (orders.length > 0) {
                doT.exec("template/myaccount/history-order-list.html", function (template) {
                    var innerHtml = template(orders);
                    innerHtml = $(innerHtml);
                    $(".product-items").append(innerHtml);
                });
            }
        });
    };

    modules.exports = ObjectJS;
})