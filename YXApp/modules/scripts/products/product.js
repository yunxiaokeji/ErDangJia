define(function (require, exports, modules) {
    require("jquery");
    var Global = require("global"),
        doT = require("dot");
    var ObjectJS = {};
    var Params = {
        yxClientCode: "",
        zngcClientID: "",
        pageSize: 10,
        pageIndex: 1
    };
    var IsLoadding = false;
    var PageCount = 0;

    ObjectJS.init = function (yxClientCode, zngcClientID) {
        Params.yxClientCode = yxClientCode;
        Params.zngcClientID = zngcClientID;
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
                        ObjectJS.getProductList();
                    }
                }, 1000);
            }
        });
    };

    ObjectJS.getProductList = function () {
        IsLoadding = true;
        Global.post("/Product/GetProductList", Params, function (data) {
            IsLoadding = false;
            var orders = data.item.orders;
            if (orders.length > 0) {
                doT.exec("template/product/product-list.html", function (template) {
                    var innerHtml = template(orders);
                    innerHtml = $(innerHtml);
                    $(".product-items").append(innerHtml);
                });
            }
        });
    };

    modules.exports = ObjectJS;
})