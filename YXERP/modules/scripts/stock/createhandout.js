﻿
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        ChooseProduct = require("chooseproduct"),
        doT = require("dot");

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (wareid) {
        var _self = this;
        _self.wareid = wareid;
        _self.bindEvent(wareid);
    }
    //绑定事件
    ObjectJS.bindEvent = function (wareid) {
        var _self = this;

        $("#btnChooseProduct").click(function () {
            ChooseProduct.create({
                title: "选择出库产品",
                type: 7, //1采购 2出库 3报损 4报溢 5调拨 7手工出库
                wareid: wareid,
                callback: function (products) {
                    if (products.length > 0) {
                        var entity = {}, items = [];
                        entity.guid = wareid;
                        entity.type = 7;
                        for (var i = 0; i < products.length; i++) {
                            items.push({
                                ProductID: products[i].pid,
                                ProductDetailID: products[i].did,
                                BatchCode: products[i].batch,
                                DepotID: products[i].depotid,
                                SaleAttrValueString: products[i].remark,
                            });
                        }
                        entity.Products = items;

                        Global.post("/ShoppingCart/AddShoppingCartBatchOut", { entity: JSON.stringify(entity) }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            }
                        });
                    }
                }   
            });
        });

        //编辑数量
        $(".quantity").change(function () {
            if ($(this).val().isInt() && $(this).val() > 0) {
                _self.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });
        //删除
        $(".ico-del").click(function () {
            var _this = $(this);
            confirm("确认删除此产品吗？", function () {
                Global.post("/ShoppingCart/DeleteCart", {
                    ordertype: 7,
                    guid: _self.wareid,
                    productid: _this.data("id"),
                    name: ""
                }, function (data) {
                    if (!data.status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("tr.item").remove();
                    }
                });
            });
        });

        //提交订单
        $("#btnconfirm").click(function () {

            if ($(".cart-item").length == 0) {
                alert("请选择出库产品！");
                return;
            }
            confirm("出库单提交后不可编辑，确认提交吗？", function () {
                _self.submitOrder();
            });

        });
    }
    //更改数量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/ShoppingCart/UpdateCartQuantity", {
            autoid: ele.data("id"),
            guid: _self.wareid,
            quantity: ele.val()
        }, function (data) {
            if (!data.Status) {
                ele.val(ele.data("value"));
                alert("系统异常，请重新操作！");
            } else {
                ele.data("value", ele.val());
            }
        });
    }
    //保存
    ObjectJS.submitOrder = function () {
        var _self = this;

        if ($(".cart-item").length == 0) {
            alert("请选择出库产品！");
            return;
        }
        Global.post("/Stock/SubmitHandOutDoc", {
            wareid: _self.wareid,
            remark: $("#remark").val().trim()
        }, function (data) {
            if (data.status) {
                location.href = "/Stock/HandOut";
            } else {
                location.href = location.href;
            }
        })
    }

    module.exports = ObjectJS;
})