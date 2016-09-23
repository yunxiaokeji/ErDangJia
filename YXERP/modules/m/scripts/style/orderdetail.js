define(function (require, exports, module) {
    var Global = require("global"),
        Upload = require("upload"),
        doT = require("dot");

    var ObjectJS = {};
    ObjectJS.init = function (model) {
        var model = JSON.parse(model.replace(/&quot;/g, '"'));
        ObjectJS.order = model;
        ObjectJS.getOrderDetails();
        ObjectJS.bindEvent();
        ObjectJS.getZngcOrderStatus();
    };

    //绑定事件
    ObjectJS.bindEvent = function () {
        //菜单切换模块事件
        $("nav ul li").click(function () {
            var _this = $(this);
            _this.addClass("menuchecked").siblings().removeClass("menuchecked");
            _this.parent().parent().find("i").css("color", "#9e9e9e");
            _this.find("i").css("color", "#4a98e7");
            var classname = _this.data("classname");
            ModuleType = classname;
            $(".main-box ." + classname).show().siblings().hide();
            var isGet = _this.data("isget");
            if (classname == "plate-des") {
                if (!isGet) {
                    ObjectJS.getOrderDetails();
                    _this.data("isget", "1");
                }
            }
        });
        //点击回到顶部
        $(".getback").click(function () {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        });
    }

    //获取下单明细
    ObjectJS.getOrderDetails = function () {
        $(".tb-order-detail").html('');
        var items = ObjectJS.order.Details;
        if (items.length > 0) {
            doT.exec("m/template/order/orderdetails.html", function (template) {
                var html = template(items);
                html = $(html);
                html.find('.doc-header').click(function () {
                    var _this = $(this);
                    if (!_this.next().is(":animated")) {
                        if (!_this.hasClass('hover')) {
                            _this.addClass('hover');
                            _this.find('.lump').addClass('hover');
                            _this.next().slideDown(400, function () {
                            });
                        } else {
                            _this.removeClass('hover');
                            _this.find('.lump').removeClass('hover');
                            _this.next().slideUp(400, function () {

                            });
                        }
                    }
                });
                $(".tb-order-detail").append(html);
            });
        }
        else {
            $(".tb-order-detail").append("<div class='nodata-txt'>暂无工艺说明<div>");
        }
    };

    //获取订单在智能工厂状态
    ObjectJS.getZngcOrderStatus = function () {
        $("#moduleBox").append('<div class="data-loading"></div>');
        Global.post("/M/Orders/GetZngcOrderStatus", { id: ObjectJS.order.DocID }, function (data) {
            $("#moduleBox .data-loading").remove();
            data = JSON.parse(data);
            if (data.items) {
                doT.exec("m/template/order/zngcorderstatus.html", function (template) {
                    var innerHtml = template(data.items);
                    innerHtml = $(innerHtml);
                    $("#moduleBox").append(innerHtml);
                });
            }
        });
    };

    module.exports = ObjectJS;
});
