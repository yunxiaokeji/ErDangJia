
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        ChooseProduct = require("chooseproduct"),
        doT = require("dot");

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (guid, wares) {
        var _self = this;
        _self.guid = guid;
        wares = JSON.parse(wares.replace(/&quot;/g, '"'));
        _self.bindEvent(wares);
    }
    //绑定事件
    ObjectJS.bindEvent = function (wares) {
        var _self = this;

        //仓库
        require.async("dropdown", function () {
            var dropdown = $("#wareid").dropdown({
                prevText: "仓库-",
                defaultText: "请选择",
                defaultValue: "",
                data: wares,
                dataValue: "WareID",
                dataText: "Name",
                width: "180",
                isposition: true,
                onChange: function (data) {

                }
            });
        });

        $(".check-all").click(function () {
            var _this = $(this).find(".checkbox");
            if (!_this.hasClass("hover")) {
                $(".checkbox").addClass("hover");
            } else {
                $(".checkbox").removeClass("hover");
            }
        });

        //选择提交产品
        $(".cart-item .checkbox").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
            } else {
                _this.removeClass("hover");
            }
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
                    ordertype: 4,
                    guid: _self.guid,
                    productid: _this.data("id"),
                    name: ""
                }, function (data) {
                    if (!data.status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("ul.item").remove();
                    }
                });
            });
        });

        //提交订单
        $("#btnconfirm").click(function () {

            if ($(".cart-item .checkbox.hover").length == 0) {
                alert("请选择报溢产品");
                return;
            }

            if (!$("#wareid").data("id")) {
                alert("请选择仓库");
                return;
            }

            confirm("报溢单提交后不可编辑，确认提交吗？", function () {
                _self.submitOrder();
            });

        });
    }

    //更改数量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/ShoppingCart/UpdateCartQuantity", {
            autoid: ele.data("id"),
            guid: _self.guid,
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

        var ids = "";

        $(".cart-item").each(function () {
            if ($(this).find(".checkbox").hasClass("hover")) {
                ids += $(this).data("autoid") + ",";
            }
        });

        Global.post("/Stock/SubmitOverflowDoc", {
            ids: ids,
            wareid: $("#wareid").data("id"),
            remark: $("#remark").val().trim()
        }, function (data) {
            if (data.status) {
                location.href = "/Stock/Overflow";
            } else {
                alert("提交失败，请重新操作", function () {
                    location.href = location.href;
                });
            }
        });
    }

    module.exports = ObjectJS;
})