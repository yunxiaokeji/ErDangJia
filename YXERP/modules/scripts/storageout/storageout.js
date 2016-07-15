
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
        status: 2,
        outstatus: 0,
        sendstatus: -1,
        returnstatus: -1,
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
        $('.searth-module').html('');
        require.async("search", function() {
            $(".searth-module").searchKeys(function(keyWords) {
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
                Params.outstatus = _this.data("id");
                _self.getList();
            }
        });

        $(".search-sendstatus li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.sendstatus = _this.data("id");
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
            Params.PageIndex = 1;
            Params.BeginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.EndTime = end ? end.format("YYYY-MM-DD") : "";
            _self.getList();
        });
        //全部选中
        $("#checkAll").click(function () {
            var _this = $(this).find(".checkbox");
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $(".table-list .checkbox").addClass("hover");
            } else {
                _this.removeClass("hover");
                $(".table-list .checkbox").removeClass("hover");
            }
        });
        _self.checkClick();

        //批量打印
        $("#printOrderOut").click(function () {
            var checks = $(".table-list .checkbox.hover");
            if (checks.length ==1) {
                var headstr = "<html><head><title></title></head><body>";
                var footstr = "</body>";
                var newstr = "暂无数据,请刷新页面重试";
                $.ajax({
                    url: '/StorageOut/StorageOutPrintModel/' + $(checks[0]).data('id'),
                    type: "GET",
                    async: false,
                    success: function (data) { 
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
                    alert("您尚未选择要打印的出库单");
                } else {
                    alert("目前只支持单条打印的出库单");
                }
            }
        });
    }
    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='12'><div class='data-loading' ><div></td></tr>");

        var url = "/StorageOut/GetAgentOrders",
            template = "template/storageout/storageout.html";

        Global.post(url, { filter: JSON.stringify(Params) }, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec(template, function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText); 
                    $(".tr-header").after(innerText);
                    _self.checkClick();
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='12'><div class='nodata-txt' >暂无数据!</div></td></tr>");
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

    //审核页初始化
    ObjectJS.initDetail = function (orderid) {
        var _self = this;
        _self.orderid = orderid;

        Global.post("/System/GetAllWareHouses", {}, function (data) {
            _self.wares = data.items; 
        });

        Global.post("/Plug/GetExpress", {}, function (data) {
            _self.express = data.items;
        });
        //审核入库
        $("#btnSubmit").click(function () {
            _self.changeWare();
            
        })
    }

    ObjectJS.changeWare = function () {
        var _self = this;
        doT.exec("template/storageout/outdetail.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-orderout-detail",
                    header: "订单出库",
                    content: innerText,
                    yesFn: function () {
                        if (!$("#wares").data("id")) {
                            alert("请选择仓库！");
                            return false;
                        }

                        var paras = {
                            orderid: _self.orderid,
                            wareid: $("#wares").data("id"),
                            issend: $("#checkSend").hasClass("ico-checked") ? 1 : 0,
                            expressid: $("#expressid").data("id"),
                            expresscode: $("#expressCode").val().trim()
                        };
                        Global.post("/StorageOut/ConfirmAgentOrderOut", paras, function (data) {
                            if (data.result != 1) {
                                alert(data.errinfo);
                            } else {
                                location.href = "/StorageOut/StorageOut";
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
                var dropdown= $("#wares").dropdown({
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
            //快递公司
            require.async("dropdown", function () {
                var dropdown = $("#expressid").dropdown({
                    prevText: "",
                    defaultText: "请选择",
                    defaultValue: "",
                    data: _self.express,
                    dataValue: "ExpressID",
                    dataText: "Name",
                    width: "180",
                    isposition: true,
                    onChange: function (data) {

                    }
                });
            });

            $("#checkSend").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                } else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });
        });
    }
     
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
    }
    module.exports = ObjectJS;
})