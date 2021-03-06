﻿
define(function (require, exports, module) {
    var City = require("city"),  CityInvoice,
        Verify = require("verify"), VerifyInvoice,
        Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");
    require("pager");
    var ObjectJS = {};

    ObjectJS.init = function (orderid, status, model) {
        var _self = this;
        _self.orderid = orderid;
        _self.status = status;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"'));
        _self.bindEvent();
        _self.bindCache();
    }

    ObjectJS.bindCache = function () {
        var _self = this;

        //订单类型
        Global.post("/System/GetOrderTypes", {}, function (data) {
            _self.model.OrderTypes = data.items;
        });

        if (_self.status > 1 && _self.status != 9) {
            Global.post("/Finance/GetOrderBillByID", { id: _self.orderid }, function (data) {
                var model = data.model;
                if (model.BillingID) {
                    $("#infoPaymoney").text(model.PayMoney.toFixed(2));
                    _self.getPays(model.BillingPays, true);
                    _self.getInvoices(model.BillingInvoices, true);

                    //申请开票
                    if (model.InvoiceStatus == 0) {
                        _self.billingid = model.BillingID;
                        
                    } else {
                        $("#addInvoice").addClass("nolimits");
                    }
                }
            });
        }
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        //转移负责人
        $("#changeOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            Global.post("/Orders/UpdateOrderOwner", {
                                userid: items[0].id,
                                ids: _this.data("id")
                            }, function (data) {
                                if (data.status) {
                                    $("#lblOwner").text(items[0].name);
                                }
                            });
                        } else {
                            alert("请选择不同人员进行转移!");
                        }
                    }
                }
            });
        });

        //编辑单价
        $(".price").change(function () {
            var _this = $(this);
            if (_this.val().isDouble() && _this.val() > 0) {
                _self.editPrice(_this);
            } else {
                _this.val(_this.data("value"));
            }
        });

        //删除订单
        $("#btndelete").click(function () {
            confirm("订单删除后不可恢复，确认删除吗？", function () {
                _self.deleteOrder();
            });
        });

        //审核订单
        $("#btnconfirm").click(function () {
            confirm("订单审核后产品信息和收货人信息不可编辑，请检查信息是否正确，确认继续审核订单吗？", function () {
                Global.post("/Orders/EffectiveOrder", { orderid: _self.orderid }, function (data) {
                    if (data.status) {
                        location.href = location.href;
                    } else {
                        if (data.result = 0) {
                            alert("订单审核失败，可能因为订单状态已改变，请刷新页面后重试！");
                        } else if (data.result = 2) {
                            alert("产品库存不足，订单审核失败！");
                        } else if (data.result = 3) {
                            alert("账户余额不足，订单审核失败！");
                        } else {
                            alert("订单审核失败！");
                        }
                    }
                });
            });
        });

        //编辑订单
        $("#updateOrderInfo").click(function () {
            _self.editOrder(_self.model);
        });

        if (_self.status == 2) {
            $("#lblStatus").addClass("blue");

            if (_self.model.SendStatus > 0) {
                $("#btnreturn").html("申请退货")
            }

            if (_self.model.ReturnStatus == 0) {
                $("#btnreturn").show();
                $("#btnreturn").click(function () {
                    //未出库
                    if (_self.model.SendStatus == 0) {
                        confirm("确认申请退单吗？", function () {
                            Global.post("/Orders/ApplyReturnOrder", { orderid: _self.orderid }, function (data) {
                                if (data.status) {
                                    location.href = location.href;
                                } else {
                                    alert("申请退单失败，可能因为订单状态已改变，请刷新页面后重试！");
                                }
                            });
                        });
                    } else {
                        location.href = "/Orders/ApplyReturn/" + _self.orderid;
                    }
                });

            } else if (_self.model.ReturnStatus == 1) {
                if (_self.model.SendStatus == 0) {
                    $("#lblStatus").html("申请退单中");
                } else {
                    $("#lblStatus").html("申请退货中");
                }
            } else if (_self.model.ReturnStatus == 2) {
                $("#lblStatus").html("部分退货");
                $("#btnreturn").show();
                $("#btnreturn").click(function () {
                    location.href = "/Orders/ApplyReturn/" + _self.orderid;
                });
            } else if (_self.model.ReturnStatus == 3) {
                $("#lblStatus").html("已退货");
            }
        } else if (_self.status > 2) {
            $("#lblStatus").addClass("red");
        }

        //添加发票申请
        $("#addInvoice").click(function () {
            _self.addInvoice();
        });

        //删除发票信息
        $("#deleteInvoice").click(function () {
            var _this = $(this);
            confirm("撤销后不可恢复，确认撤销此开票申请吗？", function () {
                Global.post("/Finance/DeleteBillingInvoice", {
                    id: _this.data("id"),
                    billingid: _this.data("billingid")
                }, function (data) {
                    if (data.status) {
                        $("#addInvoice").removeClass("nolimits").show();
                        var ele = $("#navInvoices tr[data-id='" + _this.data("id") + "']");
                        ele.remove();
                    } else {
                        alert("申请已通过审核，不能删除!");
                    }
                });
            });
        });

        //切换模块
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv,.nav-btn-box .btn").hide();
            $("#" + _this.data("id")).show();

            $(".nav-btn-box .btn[data-id='" + _this.data("id") + "']").show();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                require.async("logs", function () {
                    $("#navLog").getObjectLogs({
                        guid: _self.orderid,
                        type: 2, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
                        pageSize: 10
                    });
                });
                
            } else if (_this.data("id") == "navRemark" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                require.async("replys", function () {
                    $("#navRemark").getObjectReplys({
                        guid: _self.orderid,
                        type: 2, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
                        pageSize: 10
                    });
                });
            } 
        });

        //删除产品
        $("#navProducts .ico-del").click(function () {
            var _this = $(this);
            confirm("确认移除此产品吗？", function () {
                Global.post("/ShoppingCart/DeleteCart", {
                    ordertype: 11,
                    guid: _self.orderid,
                    productid: _this.data("id"),
                    name: _this.data("name")
                }, function (data) {
                    if (!data.status) {
                        alert("网络异常或数据状态有变更，请重新操作", function () {
                            location.href = location.href;
                        });
                    } else {
                        alert("产品移除成功");
                        _this.parents("tr.item").remove();
                        _self.getAmount();
                    }
                });
            });
        });

        //编辑数量
        $("#navProducts .quantity").change(function () {
            if ($(this).val().isInt() && $(this).val() > 0 && $(this).val() * 1 != $(this).data("value") * 1) {
                _self.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑单价
        $("#navProducts .price").change(function () {
            var _this = $(this);
            if (_this.val().isDouble() && _this.val() > 0 && _this.val() * 1 != _this.data("value") * 1) {

                Global.post("/Orders/UpdateOrderProductPrice", {
                    orderid: _self.orderid,
                    productid: _this.data("id"),
                    name: _this.data("name"),
                    price: _this.val()
                }, function (data) {
                    if (!data.status) {
                        _this.val(_this.data("value"));
                        alert("价格编辑失败，请刷新页面后重试！", function () {
                            location.href = location.href;
                        });
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
    }

    //编辑信息
    ObjectJS.editOrder = function (model) {
        var _self = this;
        doT.exec("template/sales/order-detail.html", function (template) {
            var innerText = template(model);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: "编辑订单信息",
                    content: innerText,
                    yesFn: function () {
                        var entity = {
                            OrderID: _self.orderid,
                            PersonName: $("#personName").val().trim(),
                            MobileTele: $("#mobileTele").val().trim(),
                            CityCode: CityObj.getCityCode(),
                            Address: $("#address").val().trim(),
                            TypeID: $("#orderType").val().trim(),
                            ExpressType: $("#expressType").val().trim(),
                            PostalCode: $("#postalcode").val().trim(),
                            Remark: $("#remark").val().trim()
                        };
                        Global.post("/Orders/EditOrder", { entity: JSON.stringify(entity) }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            } else {
                                alert("订单信息编辑失败，请刷新页面重试！");
                            }
                        })
                    },
                    callback: function () {

                    }
                }
            });

            CityObj = City.createCity({
                cityCode: model.CityCode,
                elementID: "city"
            });

            model.TypeID && $("#orderType").val(model.TypeID);
        });
    }

    //更改数量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateOrderProductQuantity", {
            orderid: _self.orderid,
            productid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("网络异常或数据状态有变更，请重新操作", function () {
                    location.href = location.href;
                });
            } else {
                ele.parent().nextAll(".amount").html((ele.parent().prevAll(".tr-price").find("input").val() * ele.val()).toFixed(2));
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //登记发票
    ObjectJS.addInvoice = function () {
        var _self = this;
        doT.exec("template/finance/orderinvoice-detail.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-invoice-detail",
                    header: "开票申请",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyInvoice.isPass()) {
                            return false;
                        }
                        var entity = {
                            BillingID: _self.billingid,
                            Type:1,
                            CustomerType: $("#customertype").val(),
                            InvoiceMoney: $("#invoicemoney").val().trim(),
                            InvoiceTitle: $("#invoicetitle").val().trim(),
                            CityCode: CityInvoice.getCityCode(),
                            Address: $("#invoiceaddress").val().trim(),
                            PostalCode: $("#invoicepostalcode").val().trim(),
                            ContactName: $("#contactname").val().trim(),
                            ContactPhone: $("#contactmobile").val().trim(),
                            Remark: $("#remark").val().trim()
                        };
                        if (entity.InvoiceMoney <= 0) {
                            alert("开票金额必须大于0！");
                            return false;
                        }
                        confirm("提交后不能更改，确认提交吗？", function () {
                            _self.saveOrderInvoice(entity);
                        })
                        return false;
                    },
                    callback: function () {

                    }
                }
            });

            $("#invoicemoney").focus();

            $("#invoicemoney").val(_self.model.TotalMoney.toFixed(2));
            $("#invoicetitle").val(_self.model.Customer.Name);
            $("#invoiceaddress").val(_self.model.Address);
            $("#invoicepostalcode").val(_self.model.PostalCode);
            $("#contactname").val(_self.model.PersonName);
            $("#contactmobile").val(_self.model.MobileTele);

            CityInvoice = City.createCity({
                elementID: "invoicecity",
                cityCode: _self.model.CityCode
            });

            VerifyInvoice = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //保存发票信息
    ObjectJS.saveOrderInvoice = function (model) {
        Easydialog.close();
        var _self = this;
        Global.post("/Finance/SaveBillingInvoice", { entity: JSON.stringify(model) }, function (data) {
            if (data.item.InvoiceID) {
                $("#addInvoice").addClass("nolimits");
                _self.getInvoices([data.item], false);
            } else {
                alert("已提交过申请，不能重复操作！");
            }
        });
    }

    //绑定支付列表
    ObjectJS.getPays = function (items, empty) {
        var _self = this;
        if (empty) {
            $("#navPays .tr-header").nextAll().remove();
        }
        if (items.length > 0) {
            doT.exec("template/finance/billingpays.html", function (template) {
                var innerhtml = template(items);
                innerhtml = $(innerhtml);
                $("#navPays .tr-header").after(innerhtml);
            });
        } else {
            $("#navPays .tr-header").after("<tr><td colspan='12'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }
    }

    //绑定开票列表
    ObjectJS.getInvoices = function (items, empty) {
        var _self = this;
        if (empty) {
            $("#navInvoices .tr-header").nextAll().remove();
        } else {
            $("#navInvoices .nodata-tr").remove();
        }
        if (items.length > 0) {
            doT.exec("template/finance/billinginvoices.html", function (template) {
                var innerhtml = template(items);
                innerhtml = $(innerhtml);

                innerhtml.find(".ico-dropdown").each(function () {
                    if ($(this).data("status") == 1) {
                        $(this).remove();
                    }
                });
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);
                    if ($(this).data("status") != 1) {
                        var position = _this.find(".ico-dropdown").position();
                        $(".dropdown-ul li").data("id", _this.data("id")).data("billingid", _this.data("billingid"));
                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 50 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    }
                    return false;
                });

                $("#navInvoices .tr-header").after(innerhtml);
            });
        } else {
            $("#navInvoices .tr-header").after("<tr class='nodata-tr'><td colspan='12'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }
    }

    //计算总金额
    ObjectJS.getAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            _this.html((_this.prevAll(".tr-quantity").find("input").val() * _this.prevAll(".tr-price").find("input").val()).toFixed(2));
            amount += _this.html() * 1;
        });
        $("#amount,#lblTotalMoney").text(amount.toFixed(2));
    }

    //更改数量
    ObjectJS.editPrice = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateOrderPrice", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            price: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("价格修改失败，可能因为订单状态已改变，请刷新页面后重试！");
            } else {
                ele.parent().nextAll(".amount").html((ele.parent().prevAll(".tr-quantity").find("label").text() * ele.val()).toFixed(2));
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //删除订单
    ObjectJS.deleteOrder = function () {
        var _self = this;
        Global.post("/Orders/DeleteOrder", { orderid: _self.orderid }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else {
                alert("订单删除失败，可能因为订单状态已改变，请刷新页面后重试！");
            }
        });
    }

    //讨论备忘
    ObjectJS.initTalk = function (guid) {
        var _self = this;

        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");
            if (txt.val().trim()) {
                var model = {
                    GUID: guid,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: ""
                };
                _self.saveReply(model);

                txt.val("");
            }

        });
        _self.getReplys(guid, 1);
    }

    //获取备忘
    ObjectJS.getReplys = function (guid, page) {
        var _self = this;
        $("#replyList").empty();
        $("#replyList").append("<div class='data-loading'><div>");
        Global.post("/Plug/GetReplys", {
            guid: guid,
            type: 2, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
            pageSize: 10,
            pageIndex: page
        }, function (data) {
            $("#replyList").empty();

            if (data.items.length > 0) {
                doT.exec("template/common/replys.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#replyList").append(innerhtml);

                    innerhtml.find(".btn-reply").click(function () {
                        var _this = $(this), reply = _this.parent().nextAll(".reply-box");
                        reply.slideDown(500);
                        reply.find("textarea").focus();
                        reply.find("textarea").blur(function () {
                            if (!$(this).val().trim()) {
                                reply.slideUp(200);
                            }
                        });
                    });
                    innerhtml.find(".save-reply").click(function () {
                        var _this = $(this);
                        if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                            var entity = {
                                GUID: _this.data("id"),
                                Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                FromReplyID: _this.data("replyid"),
                                FromReplyUserID: _this.data("createuserid"),
                                FromReplyAgentID: _this.data("agentid")
                            };

                            _self.saveReply(entity);
                        }

                        $("#Msg_" + _this.data("replyid")).val('');
                        $(this).parent().slideUp(100);
                    });

                    //require.async("businesscard", function () {
                    //    innerhtml.find(".user-avatar").businessCard();
                    //});
                });
            } else {
                $("#replyList").append("<div class='nodata-txt'>暂无数据<div>");
            }

            $("#pagerReply").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: page,
                display: 5,
                border: true,
                border_color: '#fff',
                text_color: '#333',
                background_color: '#fff',
                border_hover_color: '#ccc',
                text_hover_color: '#000',
                background_hover_color: '#efefef',
                rotate: true,
                images: false,
                mouse: 'slide',
                float: "left",
                onChange: function (page) {
                    _self.getReplys(guid, page);
                }
            });
        });
    }

    ObjectJS.saveReply = function (model) {
        var _self = this;

        Global.post("/Plug/SavaReply", {
            type: 2, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
            entity: JSON.stringify(model)
        }, function (data) {

            $("#replyList .nodata-txt").remove();

            doT.exec("template/common/replys.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                $("#replyList").prepend(innerhtml);

                innerhtml.find(".btn-reply").click(function () {
                    var _this = $(this), reply = _this.parent().nextAll(".reply-box");
                    reply.slideDown(500);
                    reply.find("textarea").focus();
                    reply.find("textarea").blur(function () {
                        if (!$(this).val().trim()) {
                            reply.slideUp(200);
                        }
                    });
                });
                innerhtml.find(".save-reply").click(function () {
                    var _this = $(this);
                    if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                        var entity = {
                            GUID: _this.data("id"),
                            Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                            FromReplyID: _this.data("replyid"),
                            FromReplyUserID: _this.data("createuserid"),
                            FromReplyAgentID: _this.data("agentid")
                        };
                        _self.saveReply(entity);
                    }
                    $("#Msg_" + _this.data("replyid")).val('');
                    $(this).parent().slideUp(100);
                });

                require.async("businesscard", function () {
                    innerhtml.find("img").businessCard();
                });
            });
        });
    }

    module.exports = ObjectJS;
})