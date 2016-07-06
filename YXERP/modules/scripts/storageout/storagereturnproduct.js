﻿
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog"),
        moment = require("moment");
    require("daterangepicker");
    require("pager");

    //缓存货位
    var CacheDepot = [];

    var Params = {
        keywords: "",
        status: -1,
        outstatus: -1,
        sendstatus: -1,
        returnstatus: 1,
        agentid: "",
        BeginTime: "",
        EndTime: "",
        pageindex: 1,
        pagesize: 20
    };
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.getList();
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keywords = keyWords;
                _self.getList();
            });
        });
        $(".search-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.returnstatus = _this.data("id");
                _self.getList();
            }
        });
        $("#invalid").click(function () {
            var _this = $(this);
            confirm("确认驳回退货申请吗？", function () {
                Global.post("/StorageOut/InvalidApplyReturnProduct", { orderid: _this.data("id") }, function (data) {
                    if (data.result == 1) {
                        Params.pageIndex = 1;
                        _self.getList();
                    } else {
                        alert(data.errinfo);
                    }
                });
            });
        });
        $("#audit").click(function () {
            var _this = $(this);
            location.href = "/StorageOut/ReturnDetail/" + _this.data("id");
        });
        //日期插件
        $("#iptCreateTime").daterangepicker({
            showDropdowns: true,
            empty: true,
            opens: "right",
            ranges: {
                '今天': [moment(), moment()],
                '昨天': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                '上周': [moment().subtract(6, 'days'), moment()],
                '本月': [moment().startOf('month'), moment().endOf('month')]
            }
        }, function (start, end, label) {
            Params.PageIndex = 1;
            Params.BeginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.EndTime = end ? end.format("YYYY-MM-DD") : "";
            _self.getList();
        });

    }

    //退货审核
    ObjectJS.initDetail = function (orderid) {
        var _self = this;
        _self.orderid = orderid;
        Global.post("/System/GetAllWareHouses", {}, function (data) {
            _self.wares = [];
            for (var i = 0; i < data.items.length; i++) {
                if (data.items[i] && data.items[i].Status == 1) {
                    _self.wares.push(data.items[i]);
                }
            }
        });

        $("#btnSubmit").click(function () {
            var _this = $(this);
            if (_this.data("status") == 0) {
                confirm("确认审核退单申请吗？", function () {
                    Global.post("/StorageOut/AuditApplyReturn", { orderid: _this.data("id") }, function (data) {
                        if (data.result == 1) {
                            alert("审核成功", function () {
                                location.href = "/StorageOut/AuditReturnProduct";
                            });
                        } else {
                            alert(data.errinfo);
                        }
                    });
                });
            } else {
                _self.changeWare();
            }
        });
    }

    ObjectJS.changeWare = function () {
        var _self = this;
        doT.exec("template/storageout/auditreturn.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-wares-detail",
                    header: "退货审核",
                    content: innerText,
                    yesFn: function () {
                        if (!$("#wares").data("id")) {
                            alert("请选择退入仓库！");
                            return false;
                        }
                        var paras = {
                            orderid: _self.orderid,
                            wareid: $("#wares").data("id"),
                        };
                        Global.post("/StorageOut/AuditApplyReturnProduct", paras, function (data) {
                            if (data.result == 1) {
                                location.href = "/StorageOut/AuditReturnProduct";
                            } else {
                                alert(data.errinfo);
                            }
                        });
                        return false;
                    },
                    callback: function () {

                    }
                }
            });

            //仓库
            require.async("dropdown", function () {
                var dropdown = $("#wares").dropdown({
                    prevText: "",
                    defaultText: "请选择",
                    defaultValue: "",
                    data: _self.wares,
                    dataValue: "WareID",
                    dataText: "Name",
                    width: "180",
                    isposition: true,
                    onChange: function (data) {

                    }
                });
            });
        });
    }

    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='9'><div class='data-loading' ><div></td></tr>");
        var url = "/StorageOut/GetAgentOrders",
            template = "template/storageout/storagereturnproduct.html";

        Global.post(url, { filter: JSON.stringify(Params) }, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec(template, function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);

                    innerText.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));
                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 50 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                        return false;
                    });

                    $(".tr-header").after(innerText);
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='9'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
            
            $("#pager").paginate({
                total_count: data.totalcount,
                count: data.pagecount,
                start: Params.pageindex,
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
                onChange: function (page) {
                    Params.pageindex = page;
                    _self.getList();
                }
            });
        });
    }

    module.exports = ObjectJS;
})