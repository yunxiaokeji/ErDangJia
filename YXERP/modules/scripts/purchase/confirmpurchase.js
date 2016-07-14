
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        ChooseProduct = require("chooseproduct");

    var ObjectJS = {};
    //添加页初始化
    ObjectJS.init = function (guid, wares) {
        var _self = this;
        _self.guid = guid;
        wares = JSON.parse(wares.replace(/&quot;/g, '"'));
        _self.bindEvent(wares);
        _self.getAmount();
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

        //编辑数量
        $(".quantity").change(function () {
            if ($(this).val().isInt() && $(this).val() > 0) {
                _self.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑单价
        $(".price").change(function () {
            var _this = $(this);
            if (_this.val().isDouble() && _this.val() > 0) {

                Global.post("/ShoppingCart/UpdateCartPrice", {
                    autoid: _this.data("id"),
                    guid: _self.guid,
                    price: _this.val()
                }, function (data) {
                    if (!data.Status) {
                        _this.val(_this.data("value"));
                        alert("价格编辑失败，请刷新页面后重试！");
                    } else {
                        _this.parent().nextAll(".amount").html((_this.parent().nextAll(".tr-quantity").find("input").val() * _this.val()).toFixed(2));
                        _this.data("value", _this.val());
                        _self.getAmount();
                    }
                });

               
            } else {
                _this.val(_this.data("value"));
            }
        });

        //删除产品
        $(".ico-del").click(function () {
            var _this = $(this);
            confirm("确认移除此产品吗？", function () {
                Global.post("/ShoppingCart/DeleteCart", {
                    ordertype: 1,
                    guid: _self.guid,
                    productid: _this.data("id"),
                    name:""
                }, function (data) {
                    if (!data.status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("ul.item").remove();
                        _self.getAmount();
                    }
                });
            });
        });

        //选择提交产品
        $(".cart-item .checkbox").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
            } else {
                _this.removeClass("hover");
            }
            _self.getAmount();
        });

        //全选
        $(".check-all").click(function () {
            var _this = $(this).find(".checkbox");
            if (!_this.hasClass("hover")) {
                $(".checkbox").addClass("hover");
            } else {
                $(".checkbox").removeClass("hover");
            }
            _self.getAmount();
        });

        //选择单个供应商
        $(".check-part").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                $(".checkbox[data-id='" + _this.data("id") + "']").addClass("hover");
            } else {
                $(".checkbox[data-id='" + _this.data("id") + "']").removeClass("hover");
            }
            _self.getAmount();
        });

        //提交采购单
        $("#btnconfirm").click(function () {
            var bl = false;
            
            if ($(".cart-item .checkbox.hover").length == 0) {
                alert("请选择采购产品");
                return;
            }

            if (!$("#wareid").data("id")) {
                alert("请选择仓库");
                return;
            }
            confirm("采购单提交后不可编辑，确认提交吗？", function () {
                _self.submitOrder();
            });
        });

    }

    //计算总金额
    ObjectJS.getAmount = function () {
        var amount = 0;
        $(".box-item.item").each(function () {
            var _this = $(this);
            _this.find(".amount").html((_this.find(".tr-quantity").find("input").val() * _this.find(".tr-price").find("input").val()).toFixed(2));
            if (_this.find(".checkbox").hasClass("hover")) {
                amount += _this.find(".amount").html() * 1;
            }
        });
        $("#amount").text(amount.toFixed(2));
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
                ele.parent().nextAll(".amount").html((ele.parent().prevAll(".tr-price").find("input").val() * ele.val()).toFixed(2));
                ele.data("value", ele.val());
                _self.getAmount();
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

        Global.post("/Purchase/SubmitPurchase", {
            ids: ids,
            wareid: $("#wareid").data("id"),
            remark: $("#remark").val().trim()
        }, function (data) {
            if (data.status) {
                location.href = "/Purchase/Purchases";
            } else {
                alert("提交失败，请重新操作", function () {
                    location.href = location.href;
                });
            }
        });
    }

    module.exports = ObjectJS;
})