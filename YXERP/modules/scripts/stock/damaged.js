﻿
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot"),
        Dialog = require("dialog"),
        moment = require("moment");
    require("daterangepicker");
    require("pager");

    var Params = {
        keyWords: "",
        wareid: "",
        status: -1,
        pageIndex: 1,
        totalCount: 0,
        begintime: "",
        endtime: "",
        type: 3
    };
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (wares) {
        var _self = this;
        wares = JSON.parse(wares.replace(/&quot;/g, '"'));
        _self.bindEvent(wares);
        _self.getList();
    }
    //绑定事件
    ObjectJS.bindEvent = function (wares) {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")
                && !$(e.target).parents().hasClass("bghulan") && !$(e.target).hasClass("bghulan")) {
                $(".dropdown-ul").hide();
            }
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
            Params.pageIndex = 1;
            Params.begintime = start ? start.format("YYYY-MM-DD") : "";
            Params.endtime = end ? end.format("YYYY-MM-DD") : "";
            _self.getList();
        });

        //仓库
        $(".search-wares li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.pageIndex = 1;
                Params.wareid = _this.data("id");
                _self.getList();
            }
        });

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                _self.getList();
            });
        });

        //切换状态
        $(".search-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.pageIndex = 1;
                Params.status = _this.data("id");
                _self.getList();
            }
        });

        //新建报损
        $("#btnCreate").click(function () {
            var _this = $(this);
            doT.exec("template/stock/chooseware.html", function (template) {
                var innerHtml = template(wares);
                Easydialog.open({
                    container: {
                        id: "show-model-chooseware",
                        header: "选择报损仓库",
                        content: innerHtml,
                        yesFn: function () {
                            var wareid = $(".ware-items .hover").data("id");
                            if (!wareid) {
                                alert("请选择报损仓库！");
                                return false;
                            } else {
                                location.href = "/Stock/CreateDamaged/" + wareid;
                            }
                        },
                        callback: function () {

                        }
                    }
                });

                $(".ware-items .ware-item").click(function () {
                    $(this).siblings().removeClass("hover");
                    $(this).addClass("hover");
                });
            });
        });

        //审核
        $("#audit").click(function () {
            location.href = "/Stock/DamagedDetail/" + _self.docid;
        });
        //作废
        $("#invalid").click(function () {
            location.href = "/Stock/DamagedDetail/" + _self.docid;
        });
        //删除
        $("#delete").click(function () {
            location.href = "/Stock/DamagedDetail/" + _self.docid;
        });
        $("#dropdown").click(function () {
            var position = $("#dropdown").position();
            $("#exceldropdown").css({ "top": position.top + 30, "left": position.left - 80 }).show().mouseleave(function () {
                $(this).hide();
            });
        });
        $('#exportPurchases').click(function () {
            Params.filleName = '损耗单导出';
            Params.doctype = 3;
            Dialog.exportModel("/Purchase/ExportFromPurchases", Params);
        });
    }
    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='7'><div class='data-loading' ><div></td></tr>");
        var url = "/Stock/GetStorageDocs",
            template = "template/stock/storagedocs.html";

        Global.post(url, Params, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec(template, function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    $(".tr-header").after(innerText);

                    //下拉事件
                    $(".dropdown").click(function () {
                        var _this = $(this);
                        if (_this.data("status") == 0) {
                            $("#invalid").show();
                            $("#delete").show();
                        } else {
                            $("#invalid").hide();
                            $("#delete").hide();
                        }
                        var position = _this.find(".ico-dropdown").position();
                        $("#opeardropdown").css({ "top": position.top + 15, "left": position.left - 40 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                        _self.docid = _this.data("id");
                    });
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='7'><div class='nodata-txt' >暂无数据!</div></td></tr>");
            }

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Params.pageIndex,
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
                    Params.pageIndex = page;
                    _self.getList();
                }
            });
        });
    }

    //审核页初始化
    ObjectJS.initDetail = function (docid) {
        var _self = this;
        _self.docid = docid;
        
        $("#btnInvalid").click(function () {
            confirm("报损单作废后不可恢复,确认作废吗？", function () {
                Global.post("/Stock/InvalidDamagedDoc", { docid: _self.docid }, function (data) {
                    if (data.status) {
                        location.href = "/Stock/Damaged";
                    } else {
                        alert("作废失败！");
                    }
                });
            });
        });

        $("#btnDelete").click(function () {
            confirm("报损单删除后不可恢复,确认删除吗？", function () {
                Global.post("/Stock/DeleteDamagedDoc", { docid: _self.docid }, function (data) {
                    if (data.status) {
                        location.href = "/Stock/Damaged";
                    } else {
                        alert("删除失败！");
                    }
                });
            });
        });

        $("#btnAudit").click(function () {
            confirm("确认审核报损单吗？", function () {
                Global.post("/Stock/AuditDamagedDoc", { docid: _self.docid }, function (data) {
                    if (data.result == 1) {
                        location.href = "/Stock/Damaged";
                    } else {
                        alert(data.errinfo);
                    }
                });
            });
        });
    }

    module.exports = ObjectJS;
})