define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        moment = require("moment");
    require("daterangepicker");
    require("pager");

    var Params = {
        PayStatus: -1,
        InvoiceStatus: -1,
        Keywords: "",
        BeginTime: "",
        EndTime: "",
        PageIndex: 1,
        PageSize: 20
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.getList();
        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function (type) {
        var _self = this;
        //$(document).click(function (e) {
        //    //隐藏下拉
        //    if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
        //        $(".dropdown-ul").hide();
        //    }
        //});

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

        //切换状态
        $(".search-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.PayStatus = _this.data("id");
                _self.getList();
            }
        });
        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.PageIndex = 1;
                Params.Keywords = keyWords;
                _self.getList();
            });
        });

        $("#invoiceStatus li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.InvoiceStatus = _this.data("id");
                _self.getList();
            }
        })

        //全部选中
        $("#checkAll").click(function () {
            var _this = $(this);
            if (!_this.hasClass("ico-checked")) {
                _this.addClass("ico-checked").removeClass("ico-check");
                $(".table-list .check").addClass("ico-checked").removeClass("ico-check");
            } else {
                _this.addClass("ico-check").removeClass("ico-checked");
                $(".table-list .check").addClass("ico-check").removeClass("ico-checked");
            }
        });       
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#checkAll").addClass("ico-check").removeClass("ico-checked");
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='9'><div class='data-loading' ><div></td></tr>");

        Global.post("/Finance/GetPayableBills", { filter: JSON.stringify(Params) }, function (data) {
            _self.bindList(data);
        });
    }
    //加载列表
    ObjectJS.bindList = function (data) {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (data.items.length > 0) {
            doT.exec("template/finance/payablebills.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                //下拉事件
                innerhtml.find(".check").click(function () {
                    var _this = $(this);
                    if (!_this.hasClass("ico-checked")) {
                        _this.addClass("ico-checked").removeClass("ico-check");
                    } else {
                        _this.addClass("ico-check").removeClass("ico-checked");
                    }
                    return false;
                });

                $(".tr-header").after(innerhtml);

            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='9'><div class='nodata-txt' >暂无数据!<div></td></tr>");
        }

        $("#pager").paginate({
            total_count: data.totalCount,
            count: data.pageCount,
            start: Params.PageIndex,
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
                Params.PageIndex = page;
                _self.getList();
            }
        });
    }

    module.exports = ObjectJS;
});