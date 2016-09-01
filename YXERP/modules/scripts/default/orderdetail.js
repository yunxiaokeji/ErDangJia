﻿define(function (require, exports, module) {
    var City = require("city"), CityInvoice,
        Verify = require("verify"), VerifyInvoice,
        Global = require("global"),
        doT = require("dot"),
        Upload = require("upload") ;
    require("pager"); 

    var ObjectJS = {}, CacheItems=[];

    ObjectJS.init = function (orderid,  model) {
        var _self = this;
        _self.orderid = orderid; 
        _self.model = JSON.parse(model.replace(/&quot;/g, '"')); 
        _self.bindStyle(_self.model);  
    }

    ObjectJS.bindStyle = function (model) {
        var _self = this; 
        if (model.platemaking) {
            $("#navEngravingInfo").html(decodeURI(model.platemaking));
            $("#navEngravingInfo .ico-dropdown").remove();
            $("#navEngravingInfo tr").each(function () {
                $(this).find("td").last().remove();
            });
        } else {
            $("#navEngravingInfo").html("<div class='nodata-txt'>暂无制版信息<div>");
        }
        //样图
        _self.bindOrderImages(model.orderImages);
        //规格
        _self.showAttrForOrder(model, 'pdttitle', 'pdtAttr');
        _self.getPlateMakings();
        $('.bottombtn').bind('click', function () { _self.savePDT() });
        if (window.screen.width < 1400) {
            $('.moneyinfo').css("width", "720px");
        } 
    } 

    //获取制版工艺说明
    ObjectJS.getPlateMakings = function () {
        var _self = this;
        $(".tb-plates").html('');
        $(".tb-plates").html("<tr><td colspan='5'><div class='data-loading'><div></td></tr>"); 
        $(".tb-plates").html(''); 
        var PlateMakings = _self.model.plateMakings; 
        if (PlateMakings.length > 0) {
            doT.exec("template/default/platematring-orderdatail.html", function (template) {
                var html = template(PlateMakings);
                html = $(html);
                html.find(".dropdown").remove();
                $(".tb-plates").append(html);
            });
        }
        else {
            $(".tb-plates").append("<tr><td colspan='5'><div class='nodata-txt'>暂无工艺说明</div></td></tr>");
        } 
    }
     
    ObjectJS.showAttrForOrder = function (categoryList, obj, contendid) {
        $(".productsalesattr").remove();
        $("#" + contendid).empty();
        CacheItems = [];
        doT.exec("template/default/createorder-checkattr.html", function (template) {
            var innerhtml = template(categoryList);
            innerhtml = $(innerhtml);
            //组合产品
            innerhtml.find(".check-box").click(function () {
                var _this = $(this).find(".checkbox");
                if (_this.hasClass("hover")) {
                    _this.removeClass("hover");
                } else {
                    _this.addClass("hover");
                }

                var bl = false, details = [], isFirst = true, xattr = [], yattr = [];
                $(".productsalesattr").each(function () {
                    bl = false;
                    var _attr = $(this), attrdetail = details;
                    //组合规格
                    _attr.find(".checkbox.hover").each(function () {
                        bl = true;
                        var _value = $(this);
                        //首个规格
                        if (isFirst) {
                            var model = {};
                            model.ids = _attr.data("id") + ":" + _value.data("id");
                            model.saleAttr = _attr.data("id");
                            model.attrValue = _value.data("id");
                            model.xRemark = _value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "";
                            model.yRemark = _value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "";
                            model.xyRemark = "【" + _value.data("text") + "】";
                            model.names = "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                            model.layer = 1;
                            details.push(model);

                        } else {
                            for (var i = 0, j = attrdetail.length; i < j; i++) {
                                if (attrdetail[i].ids.indexOf(_value.data("attrid")) < 0) {
                                    var model = {};
                                    model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.data("id");
                                    model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                    model.attrValue = attrdetail[i].attrValue + "," + _value.data("id");
                                    model.xRemark = attrdetail[i].xRemark + (_value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "");
                                    model.yRemark = attrdetail[i].yRemark + (_value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "");
                                    model.xyRemark = attrdetail[i].xyRemark + "【" + _value.data("text") + "】";
                                    model.names = attrdetail[i].names + "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                    model.layer = attrdetail[i].layer + 1;
                                    details.push(model);
                                }
                            }
                        }
                        //处理二维表
                        if (_value.data("type") == 1 && xattr.indexOf("【" + _value.data("text") + "】") < 0) {
                            xattr.push("【" + _value.data("text") + "】");
                        } else if (_value.data("type") == 2 && yattr.indexOf("【" + _value.data("text") + "】") < 0) {
                            yattr.push("【" + _value.data("text") + "】");
                        }

                    });
                    isFirst = false;
                });
                $("#" + contendid).empty();
                //选择所有属性
                if (bl) {
                    var layer = $(".productsalesattr").length, items = [];
                    for (var i = 0, j = details.length; i < j; i++) {
                        var model = details[i];
                        if (model.layer == layer) {
                            items.push(model);
                            CacheItems[model.xyRemark] = model;
                        }
                    }
                    var tableModel = {};
                    tableModel.xAttr = xattr;
                    tableModel.yAttr = yattr;
                    tableModel.items = items;

                    //加载子产品
                    doT.exec("template/default/orders_child_list.html", function (templateFun) {
                        var innerText = templateFun(tableModel);
                        innerText = $(innerText);
                        $("#" + contendid).append(innerText);
                        //数量必须大于0的数字
                        innerText.find(".quantity").change(function () {
                            var _this = $(this);
                            if (!_this.val().isInt() || _this.val() <= 0) {
                                _this.val("0");
                            }

                            var total = 0;
                            $(".child-product-table .tr-item").each(function () {
                                var _tr = $(this), totaly = 0;
                                if (!_tr.hasClass("total")) {
                                    _tr.find(".quantity").each(function () {
                                        var _this = $(this);
                                        if (_this.val() > 0) {
                                            totaly += _this.val() * 1;
                                        }
                                    });
                                    _tr.find(".total-y").text(totaly);
                                } else {
                                    _tr.find(".total-y").each(function () {
                                        var _td = $(this), totalx = 0;
                                        $(".child-product-table .quantity[data-x='" + _td.data("x") + "']").each(function () {
                                            var _this = $(this);
                                            if (_this.val() > 0) {
                                                totalx += _this.val() * 1;
                                            }
                                        });
                                        total += totalx;
                                        _td.text(totalx);
                                    });
                                    _tr.find(".total-xy").text(total);
                                }
                            });
                           
                        });
                        if (innerText.find(".quantity").length > 0) {
                            $('#btnSubmit').show();
                        } else {
                            $('#btnSubmit').hide();
                        }
                    });
                }
            });
            $(innerhtml).find(".column-title").css("width", "50px");
            $("#" + obj).after(innerhtml);
            $('#' + contendid).css("height", "274px").css("overflow-y", "auto");
            
        });
    };
    //绑定样式图
    ObjectJS.bindOrderImages = function (orderimages) {
        var _self = this;
        var images = orderimages.split(",");
        _self.images = images; 
        if (orderimages == null || orderimages=="" || images.length == 0) {
            $(".order-imgs-list").html('<li class="hover"><img src="/modules/images/none-img.png"></li>');
        } else {
            for (var i = 0; i < images.length; i++) {
                if (images[i]) {
                    if (i == 0) {
                        $("#orderImage").attr("src", images[i].split("?")[0]);
                    }
                    var img = $('<li class="' + (i == 0 ? 'hover' : "") + '"><img src="' + images[i] + '" /></li>');
                    $(".order-imgs-list").append(img);
                }
            }
        }
        $(".order-imgs-list img").parent().click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                $("#orderImage").attr("src", _this.find("img").attr("src").split("?")[0]);

                if ($("#orderImage").width() > $("#orderImage").height()) {
                    $("#orderImage").css("width", 350);
                } else {
                    $("#orderImage").css("height", 350);
                }
            }
        });
    }
    ObjectJS.savePDT = function () {
        $('.bottombtn').unbind('click');
        var _self = this;
        var totalnum = 0;
        $(".child-product-table .quantity").each(function () { 
            totalnum += parseInt($(this).val());
        });
        if (totalnum == 0) {
            $('.bottombtn').bind('click', function () { _self.savePDT() });
            alert("请选择颜色尺码，并填写对应采购数量");
            return false;
        }
        var model = {
            OrderID: _self.model.orderID,
            GoodsName: _self.model.goodsName,
            GoodsID: _self.model.goodsID,
            ClientID: _self.model.clientID,
            IntGoodsCode: _self.model.intGoodsCode,
            CategoryID: _self.model.categoryID,
            FinalPrice: _self.model.finalPrice, 
            OrderImage: _self.model.orderImage, 
            Details: []
        };
        //大货单遍历下单明细 
        $(".child-product-table .quantity").each(function () {
            var _this = $(this);
            if (_this.val() > 0) { 
                var item = CacheItems[_this.data("remark")]; 
                model.Details.push({
                    SaleAttr: item.saleAttr,
                    AttrValue: item.attrValue,
                    SaleAttrValue: item.ids,
                    Quantity: _this.val(),
                    XRemark: item.xRemark,
                    YRemark: item.yRemark,
                    XYRemark: item.xyRemark,
                    Remark: item.names
                });
            }
        }); 
        Global.post("/IntFactoryOrder/CreateOrderEDJ", { entity: JSON.stringify(model) }, function (data) {
            if (data.result) {
                confirm("新增成功,是否返回继续选购产品！",
                    function() {
                         $('#btnback').click();
                    },
                    function () { 
                        $('#btndetail').parent().data("id", data.PurchaseID);
                        $('#btndetail').parent().data("url", $('#btndetail').parent().data("url")+data.PurchaseID);
                        $('#btndetail').click();
                    //$('.moneyinfo').find(".check-box").each(function (i, v) {
                    //    if ($(v).find(".checkbox").hasClass("hover")) {
                    //        $(v).find(".checkbox").removeClass("hover");
                    //    }
                    //});
                    //$('#pdtAttr').html('');
                    //$('#btnSubmit').hide();
                }); 
            } else {
                alert(data.error_message);
            }
            $('.bottombtn').bind('click', function () { _self.savePDT() });
        });
    }
    module.exports = ObjectJS;
})