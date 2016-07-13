
define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot"),
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
    ObjectJS.init = function (type, providers) {
        var _self = this;
        Params.type = type;
        providers = JSON.parse(providers.replace(/&quot;/g, '"'));
        _self.bindEvent(providers);
        _self.getList();
    }
    //绑定事件
    ObjectJS.bindEvent = function (providers) {
        var _self = this;

        Global.post("/ShoppingCart/GetShoppingCartCount", {
            ordertype: 1,
            guid: ""
        }, function (data) {
            $("#btnSubmit").html("提交采购单 ( " + data.Quantity + " ) ");
        });

        require.async("dropdown", function () {
            var dropdown = $("#ddlProviders").dropdown({
                prevText: "供应商-",
                defaultText: "全部",
                defaultValue: "",
                data: providers,
                dataValue: "ProviderID",
                dataText: "Name",
                width: "180",
                isposition: true,
                onChange: function (data) {
                    Params.pageIndex = 1;
                    Params.providerid = data.value;
                    _self.getList();
                }
            });
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
    }

    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".box-header").nextAll().remove();
        $(".box-header").after("<div class='data-loading' ><div>");
        var url = "/Purchase/GetPurchases",
            template = "template/purchase/purchases.html";
        Global.post(url, Params, function (data) {
            $(".box-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec(template, function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    $(".box-header").after(innerText);
                });
            }
            else {
                $(".box-header").after("<div class='nodata-box' >暂无数据!</div>");
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
        });

        //作废
        $("#invalid").click(function () {
            confirm("采购单作废后不可恢复,确认作废吗？", function () {
                Global.post("/Purchase/InvalidPurchase", { docid: _self.docid }, function (data) {
                    if (data.Status) {
                        alert("采购单作废成功", function () {
                            location.href = location.href;
                        });
                    } else {
                        alert("采购单作废失败", function () {
                            location.href = location.href;
                        });
                    }
                });
            });
        });

        //删除单据
        $("#delete").click(function () {
            confirm("采购单删除后不可恢复,确认删除吗？", function () {
                Global.post("/Purchase/DeletePurchase", { docid: _self.docid }, function (data) {
                    if (data.Status) {
                        alert("采购单删除成功", function () {
                            location.href = location.href;
                        });
                    } else {
                        alert("采购单删除失败", function () {
                            location.href = location.href;
                        });
                    }
                });
            });
        });
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

                        if (details.length > 0 || $("#showAuditStorageIn .checkbox").hasClass("hover")) {

                            Global.post("/Purchase/AuditPurchase", {
                                docid: _self.docid,
                                doctype: 101,
                                isover: $("#showAuditStorageIn .checkbox").hasClass("hover") ? 1 : 0,
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
            $("#showAuditStorageIn .checkbox").click(function () {
                var _this = $(this);
                if (!_this.hasClass("hover")) {
                    _this.addClass("hover");
                } else {
                    _this.removeClass("hover");
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