
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
        sourcetype:-1,
        type: 1
    };
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (type, providers,sourcetype) {
        var _self = this;
        Params.type = type;
        Params.sourcetype = sourcetype;
        providers = JSON.parse(providers.replace(/&quot;/g, '"'));
        _self.bindEvent(providers);
        _self.getList();
        
    }
    //绑定事件
    ObjectJS.bindEvent = function (providers) {
        var _self = this;
         
        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")
                && !$(e.target).parents().hasClass("bghulan") && !$(e.target).hasClass("bghulan")) {
                $(".dropdown-ul").hide();
            }
        });

        $(".searth-module").html('');

        Global.post("/ShoppingCart/GetShoppingCartCount", {
            ordertype: 1,
            guid: ""
        }, function (data) {
            $("#btnSubmit").html("提交采购单 ( " + data.Quantity + " ) ");
        });


        require.async("dropdown", function () {
            var sourceList = [
                { Name: "本地采购", SourceType: "1" },
                { Name: "在线下单", SourceType: "2" }
            ];
            var dropdown1 = $("#ddlSourceType").dropdown({
                prevText: "订单来源-",
                defaultText: "全部",
                defaultValue: Params.sourcetype,
                data: sourceList,
                dataValue: "SourceType",
                dataText: "Name",
                width: "180",
                isposition: true,
                onChange: function (data) {
                    Params.pageIndex = 1;
                    Params.sourcetype = data.value;
                    _self.getList();
                }
            });
        });
        if (Params.sourcetype == 2) {
            $("#ddlSourceType").hide();
        }
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

        //删除单据
        $("#delete").click(function () {
            var _this = $(this);
            confirm("采购单删除后不可恢复,确认删除吗？", function () {
                Global.post("/Purchase/DeletePurchase", { docid: _this.data("id") }, function (data) {
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
            $("#exceldropdown").css({ "top": position.top + 30, "left": position.left - 110 }).show().mouseleave(function () {
                $(this).hide();
            });
        });

        $('#exportPurchases').click(function () {
            Params.filleName = '采购单导出';
            Params.doctype = 1;
            Dialog.exportModel("/Purchase/ExportFromPurchases", Params);
        });
        //打印之后初始化事件
        _self.checkClick();

        _self.dropdownul();

        $("#checkAll").click(function () { 
            var _this = $(this).find(".checkbox");
            _this.unbind("click");
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $(".table-items-detail .checkbox").addClass("hover");
            } else {
                _this.removeClass("hover");
                $(".table-items-detail .checkbox").removeClass("hover");
            }
        }); 
        //批量打印
        $("#printOrderOut").click(function () {
            var checks = $(".table-items-detail .checkbox.hover");
            if (checks.length == 1) {
                var headstr = "<html><head><title></title></head><body>";
                var footstr = "</body>";
                var newstr = "暂无数据,请刷新页面重试";
                $.ajax({
                    url: '/Purchase/DocDetail/' + $(checks[0]).data('id'),
                    type: "GET",
                    async: false,
                    success: function (data) { 
                        data = data.replace('content-body', 'content-body pBottom1'); 
                        newstr = data;
                    }
                });
                var oldstr = document.body.innerHTML;
                document.body.innerHTML = headstr + newstr + footstr;
                window.print();
                document.body.innerHTML = oldstr;
                ObjectJS.bindEvent();
                return false;
            } else {
                if (checks.length == 0) {
                    alert("您尚未选择要打印的采购单");
                } else {
                    alert("目前只支持单条打印的采购单");
                }
            }
        });
    }

    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".table-header").show();
        $(".table-header").nextAll().remove();
        $(".table-header").after("<tr><td colspan='10'><div class='data-loading'><div></td></tr> ");
        var url = "/Purchase/GetPurchases",
            template = "template/purchase/purchases.html";
        Global.post(url, Params, function (data) {
            $(".table-header").nextAll().remove();

            if (data.items.length > 0) {
                $(".table-header").hide();
                doT.exec(template, function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    //if (Params.sourcetype == 2) {
                    //    innerText.find('.ico-dropdown').hide();
                    //}
                    $(".table-header").after(innerText);
                    _self.checkClick();
                    _self.dropdownul();
                });
            }
            else {
                $(".table-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!</div></td></tr>");
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
        _self.bindDetailEvent();

        Global.post("/System/GetDepotSeatsByWareID", { wareid: _self.model.WareID }, function (data) {
            CacheDepot = data.Items;
        });
    }

    ObjectJS.bindDetailEvent = function () {
        var _self = this;

        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "navStorageIn" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getDocList();
            }

        });
        var lihover = $(".tab-nav-ul li.hover").data("id");
        if (lihover == "navStorageIn") {
            $(".nav-partdiv").hide();
            $("#" + lihover).show();
            $(".tab-nav-ul li.hover").data("first", "1");
            _self.getDocList();
        }
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

        $("#btnOver").click(function () {
            if (_self.model.Status == 0) {
                confirm("您尚未登记入库产品，完成采购单后不能再登记入库，确认操作吗？", function () {
                    Global.post("/Purchase/AuditPurchase", {
                        docid: _self.docid,
                        doctype: 101,
                        isover: 1,
                        details: "",
                        remark: ""
                    }, function (data) {
                        if (data.status) {
                            alert("操作成功!", function () {
                                location.href = location.href;
                            });
                        } else if (data.result == "10001") {
                            alert("您没有操作权限!")
                        } else {
                            alert("操作失败！");
                        }
                    });
                });
            } else {
                confirm("完成采购单后不能再登记入库，确认操作吗？", function () {
                    Global.post("/Purchase/AuditPurchase", {
                        docid: _self.docid,
                        doctype: 101,
                        isover: 1,
                        details: "",
                        remark: ""
                    }, function (data) {
                        if (data.status) {
                            alert("操作成功!", function () {
                                location.href = location.href;
                            });
                        } else if (data.result == "10001") {
                            alert("您没有操作权限!")
                        } else {
                            alert("操作失败！");
                        }
                    });
                });
            }
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
                        var details = "";
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
    ObjectJS.InStock = [];
    //获取入库明细
    ObjectJS.getDocList = function () {
        var _self = this;
        $("#navStorageIn").empty();
        Global.post("/Purchase/GetPurchasesDetails", {
            docid: _self.docid
        }, function (data) {
            doT.exec("template/purchase/purchases-details.html", function (templateFun) {
                var innerText = templateFun(data.items); 
                _self.InStock = data.items;
                innerText = $(innerText);
                innerText.find('.shenhe').click(function () { 
                    _self.auditStoragePartIn($(this).data('did'));
                });

                $("#navStorageIn").append(innerText);
            });
        });
    }
    ObjectJS.getPartDetail = function (did) {
        var _self = this;
        var item = {};
        for (var i = 0; i < _self.InStock.length; i++) { 
            if (_self.InStock[i].DocID == did.toLowerCase()) {
                item = _self.InStock[i];
                break;
            }
        }
        return item;
    }
    ObjectJS.auditStoragePartIn = function (did) {
        var _self = this;
        doT.exec("template/purchase/audit_storagein.html", function (template) {
            var item = _self.getPartDetail(did); 
            var innerText = template(item.Details);
            Easydialog.open({
                container: {
                    id: "showAuditDocPartIn",
                    header: "入库单审核",
                    content: innerText,
                    yesFn: function () {
                        var details = "";
                        $("#showAuditDocPartIn .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0 && _this.find("select").val()) {
                                details += _this.data("id") + "-" + quantity + ":" + _this.find("select").val() + ",";
                            }
                        });
                        if ($("#showAuditDocPartIn .checkbox").hasClass("hover") && $('#navStorageIn .shenhe').length > 1) {
                            alert("存在多个未审核的单据，不能强制完成采购");
                            return false;
                        } 
                        if (details.length > 0 || $("#showAuditDocPartIn .checkbox").hasClass("hover")) {
                            Global.post("/Purchase/AuditDocPart", {
                                docid: did,
                                originid: _self.docid,
                                doctype: 101,
                                isover: $("#showAuditDocPartIn .checkbox").hasClass("hover") ? 1 : 0,
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
            $("#showAuditDocPartIn .ruku").hide();
            $("#showAuditDocPartIn .checkbox").click(function () {
                var _this = $(this);
                if (!_this.hasClass("hover")) {
                    _this.addClass("hover");
                } else {
                    _this.removeClass("hover");
                }
            });
            $("#showAuditDocPartIn").find(".quantity").change(function () {
                var _this = $(this);
                if (!_this.val().isInt() || _this.val() < 0) {
                    _this.val("0");
                }
            });

            $("#showAuditDocPartIn").find("select").each(function () {
                var _this = $(this);
                _self.bindDepot(_this);
            });
        });
    };
    //绑定复选框点击事件
    ObjectJS.checkClick = function () {
        $(".checkbox").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
            } else {
                _this.removeClass("hover");
            }
            return false;
        });
        $("#checkAll").find(".checkbox").unbind("click");
    }

    //绑定下拉框点击事件
    ObjectJS.dropdownul = function () { 
        $(".dropdown").click(function () {
            var _this = $(this); 
            if (_this.data("status") == 0) {
                $("#delete").show();
            } else {
                $("#delete").hide();
            }
            var position = _this.find(".ico-dropdown").position();
            $("#auditDropdown").css({ "top": position.top + 15, "left": position.left - 40 }).show().mouseleave(function () {
                $(this).hide();
            });
            $("#auditDropdown li").data("id", _this.data("id")).data("url", _this.data("url"));
        });
    }

    module.exports = ObjectJS;
})