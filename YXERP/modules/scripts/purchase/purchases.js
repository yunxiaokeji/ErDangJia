
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot"),
        Dialog = require("dialog"),
        moment = require("moment");
    require("daterangepicker");
    require("pager");

    //缓存货位
    var CacheDepot = [];

    var Params = {
        keyWords: "",
        wareid: "",
        status: -1,
        pageIndex: 1,
        totalCount: 0,
        begintime: "",
        endtime: "",
        providerid: "",
        type: 1
    };
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (type, wares) {
        var _self = this;
        Params.type = type;
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

        //供应商
        $(".search-providers li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.pageIndex = 1;
                Params.providerid = _this.data("id");
                _self.getList();
            }
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

        //新建采购
        $("#btnCreate").click(function () {
            var _this = $(this);
            doT.exec("template/stock/chooseware.html", function (template) {
                var innerHtml = template(wares);
                Easydialog.open({
                    container: {
                        id: "show-model-chooseware",
                        header: "选择采购仓库",
                        content: innerHtml,
                        yesFn: function () {
                            var wareid = $(".ware-items .hover").data("id");
                            if (!wareid) {
                                alert("请选择采购仓库！");
                                return false;
                            } else {
                                location.href = "/Purchase/ConfirmPurchase/" + wareid;
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
            location.href = "/Purchase/AuditDetail/" + _self.docid;
        });
        //作废
        $("#invalid").click(function () {
            confirm("采购单作废后不可恢复,确认作废吗？", function () {
                Global.post("/Purchase/InvalidPurchase", { docid: _self.docid }, function (data) {
                    if (data.Status) {
                        alert("采购单作废成功");
                    } else {
                        alert("采购单作废失败");
                    }
                    _self.getList();
                });
            });
        });

        $("#delete").click(function () {
            confirm("采购单删除后不可恢复,确认删除吗？", function () {
                Global.post("/Purchase/DeletePurchase", { docid: _self.docid }, function (data) {
                    if (data.Status) {
                        alert("采购单删除成功");
                    } else {
                        alert("采购单删除失败");
                    }
                    _self.getList();
                });
            });
        });
        $("#dropdown").click(function () {
            var position = $("#dropdown").position();
            $("#exceldropdown").css({ "top": position.top + 30, "left": position.left - 80 }).show().mouseleave(function () {
                $(this).hide();
            });
        });
        $('#exportPurchases').click(function () {
            Params.filleName = '采购单导出';
            Params.doctype = 1;
            Dialog.exportModel("/Purchase/ExportFromPurchases", Params);
        });
    }
    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='8'><div class='data-loading' ><div></td></tr>");
        var url = "/Purchase/GetPurchases",
            template = "template/purchase/purchases.html";
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
                $(".tr-header").after("<tr><td colspan='8'><div class='nodata-txt' >暂无数据!</div></td></tr>");
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
    ObjectJS.initDetail = function (docid, model) {
        var _self = this;
        _self.docid = docid;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"'));

        Global.post("/System/GetDepotSeatsByWareID", { wareid: _self.model.WareID }, function (data) {
            CacheDepot = data.Items;
        });

        //审核入库
        $("#btnconfirm").click(function () {
            _self.auditStorageIn();
        })
    }

    //审核入库
    ObjectJS.auditStorageIn = function () {
        var _self = this;

        doT.exec("template/purchase/audit_storagein.html", function (template) {
            var innerText = template(_self.model.Details);

            Easydialog.open({
                container: {
                    id: "showAuditStorageIn",
                    header: "采购单入库",
                    content: innerText,
                    yesFn: function () {
                        var details = ""
                        $("#showAuditStorageIn .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0 && _this.find("select").val()) {
                                details += _this.data("id") + "-" + quantity + ":" + _this.find("select").val() + ",";
                            }
                        });

                        if (details.length > 0 || $("#showAuditStorageIn .check").hasClass("ico-checked")) {

                            Global.post("/Purchase/AuditPurchase", {
                                docid: _self.docid,
                                doctype: 101,
                                isover: $("#showAuditStorageIn .check").hasClass("ico-checked") ? 1 : 0,
                                details: details,
                                remark: $("#expressRemark").val().trim()
                            }, function (data) {
                                if (data.status) {
                                    alert("入库成功!", function () {
                                        location.href = location.href;
                                    });
                                } else if (data.result == "10001") {
                                    alert("您没有操作权限!")
                                } else {
                                    alert("审核入库失败！");
                                }
                            });
                        } else {
                            alert("请正确填写入库数量和货位！");
                            return false;
                        }
                    },
                    callback: function () {

                    }
                }
            });
            $("#showAuditStorageIn .check").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                } else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });
            $("#showAuditStorageIn").find(".quantity").change(function () {
                var _this = $(this);
                if (!_this.val().isInt() || _this.val() < 0) {
                    _this.val("0");
                }
            });

            $("#showAuditStorageIn").find("select").each(function () {
                var _this = $(this);
                _self.bindDepot(_this);
            });
        });
    };

    //绑定货位
    ObjectJS.bindDepot = function (depotbox) {
        var _self = this;

        for (var i = 0, j = CacheDepot.length; i < j; i++) {
            if (CacheDepot[i].Status == 1) {
                depotbox.append($("<option value='" + CacheDepot[i].DepotID + "' >" + CacheDepot[i].DepotCode + "</option>"));
            }
        }

        depotbox.val(depotbox.data("id"));
    }


    module.exports = ObjectJS;
})