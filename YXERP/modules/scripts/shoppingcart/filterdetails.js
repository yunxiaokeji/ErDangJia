
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");

    require("cart");

    var ObjectJS = {};

    //添加页初始化
    ObjectJS.init = function (model, detailid, ordertype, guid) {
        var _self = this;
        _self.detailid = detailid;
        _self.ordertype = ordertype;
        _self.guid = guid;
        model = JSON.parse(model.replace(/&quot;/g, '"'));

        $("#productStockQuantity").text(model.StockIn - model.LogicOut);

        _self.bindDetail(model);
        _self.bindEvent(model);

        if (ordertype && ordertype > 0) {
            $(".content-body").createCart({
                ordertype: ordertype,
                guid: guid
            });
        } else {
            $(".choose-div").hide();
        }

        $(".product-info").css("width", $(".content-body").width() - 450);
        
    }

    //绑定事件
    ObjectJS.bindEvent = function (model) {
        var _self = this;

        //选择规格
        $("#productDetails li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                _this.siblings().removeClass("hover");
                for (var i = 0, j = model.ProductDetails.length; i < j; i++) {
                    if (model.ProductDetails[i].ProductDetailID == _this.data("id")) {
                        $("#addcart").prop("disabled", false).removeClass("addcartun");
                        _self.detailid = model.ProductDetails[i].ProductDetailID;
                        $("#price").text("￥" + model.ProductDetails[i].Price.toFixed(2));
                        $("#productimg").attr("src", (model.ProductDetails[i].ImgS || model.ProductImage));
                        $("#productStockQuantity").text(model.ProductDetails[i].StockIn - model.ProductDetails[i].LogicOut);
                        return;
                    } else {
                        $("#addcart").prop("disabled", true).addClass("addcartun");
                    }
                }
            }
        });

        //产品数量
        $("#quantity").blur(function () {
            if (!$(this).val().isInt()) {
                $(this).val("1");
            } else if ($(this).val() < 1) {
                $(this).val("1");
            }
        });
        //+1
        $("#quantityadd").click(function () {
            $("#quantity").val($("#quantity").val() * 1 + 1);
        });
        //-1
        $("#quantityreduce").click(function () {
            if ($("#quantity").val() != "1") {
                $("#quantity").val($("#quantity").val() * 1 - 1);
            }
        });

        //加入购物车
        $("#addcart").click(function () {
            var cart = $("#shopping-cart").offset();
            var temp = $("<div style='width:30px;height:30px;'><img style='width:30px;height:30px;' src='" + $("#productimg").attr("src") + "' /></div>");
            temp.offset({ top: $(this).offset().top + 20, left: $(this).offset().left + 100 });
            temp.css("position", "absolute");
            $("body").append(temp);
            temp.animate({ top: cart.top, left: cart.left }, 500, function () {
                temp.remove();
                var remark = "";
                Global.post("/ShoppingCart/AddShoppingCart", {
                    productid: _self.productid,
                    detailsid: _self.detailid,
                    quantity: $("#quantity").val(),
                    unitid: $("#small").data("id"),
                    isBigUnit: 0,
                    ordertype: _self.ordertype,
                    guid: _self.guid,
                    remark: $("#productDetails li.hover").data("name")
                }, function (data) {
                    if (data.Status) {
                        $("#quantity").val("1");
                        $("#shopping-cart .totalcount").html($("#shopping-cart .totalcount").html() * 1 + 1);
                    }
                });
            });
        });

    }

    //绑定信息
    ObjectJS.bindDetail = function (model) {
        var _self = this;
        _self.productid = model.ProductID;
        //绑定子产品详情
        for (var i = 0, j = model.ProductDetails.length; i < j; i++) {
            if (model.ProductDetails[i].ProductDetailID == _self.detailid) {
                $("#productDetails li[data-id='" + _self.detailid + "']").addClass("hover");
                $("#price").text("￥" + model.ProductDetails[i].Price.toFixed(2));
                $("#productimg").attr("src", model.ProductDetails[i].ImgS || model.ProductImage);
                $("#productStockQuantity").text(model.ProductDetails[i].StockIn - model.ProductDetails[i].LogicOut);
                break;
            }
        }
        $("#description").html(decodeURI(model.Description));
    }

    module.exports = ObjectJS;
})