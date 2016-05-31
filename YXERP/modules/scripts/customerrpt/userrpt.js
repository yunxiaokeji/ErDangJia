define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        moment = require("moment");
    require("daterangepicker");
    var Params = {
        type: 1,
        userid: "",
        teamid: "",
        beginTime: "",
        endTime: ""
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
    }
    ObjectJS.bindEvent = function () {
        var _self = this;

        $(".search-type li").click(function () {
            var _this = $(this);
            
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.type = _this.data("type");

                _self.getUserCustomer();
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
            Params.beginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.endTime = end ? end.format("YYYY-MM-DD") : "";
            _self.getUserCustomer();
        });
    }

    ObjectJS.getUserCustomer = function () {
        var _self = this;
        $("#userTotalRPT .tr-header").nextAll().remove();
        Global.post("/CustomerRPT/GetUserCustomers", Params, function (data) {

            var cache = [];
            for (var i = 0; i < data.items.length; i++) {
                if (data.items[i].ChildItems && data.items[i].ChildItems.length > 0) {
                    cache[data.items[i].GUID] = data.items[i].ChildItems;
                }
            }

            doT.exec("template/report/teamcustomers.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);
                
                innerhtml.find(".useritemtotal").each(function () {
                    var _this = $(this), total = 0;
                    _this.parent().prevAll().find(".useritem[data-stageid='" + _this.data("stageid") + "']").each(function () {
                        total += $(this).html() * 1;
                    });
                    _this.html(total);
                });

                innerhtml.find(".usertotal").each(function () {
                    var _this = $(this), total = 0;
                    _this.prevAll(".useritem").each(function () {
                        total += $(this).html() * 1;
                    });
                    _this.html(total);
                });

                //选择汇总
                innerhtml.find(".check").click(function () {
                    var _this = $(this);
                    if (!_this.hasClass("ico-checked")) {
                        _this.parent().parent().addClass("tr-checked").removeClass("tr-check");
                        if (_this.hasClass("check-all")) {
                            innerhtml.find(".check").parent().parent().addClass("tr-checked").removeClass("tr-check");
                            innerhtml.find(".check").addClass("ico-checked").removeClass("ico-check");
                        }
                        _this.addClass("ico-checked").removeClass("ico-check");
                    } else {
                        _this.parent().parent().addClass("tr-check").removeClass("tr-checked");
                        if (_this.hasClass("check-all")) {
                            innerhtml.find(".check").parent().parent().addClass("tr-check").removeClass("tr-checked");
                            innerhtml.find(".check").addClass("ico-check").removeClass("ico-checked");
                        }
                        _this.addClass("ico-check").removeClass("ico-checked");
                    }

                    _self.reportTotal();

                });

                //展开
                innerhtml.find(".open-child").click(function () {
                    var _this = $(this);
                    if (!_this.data("first") || _this.data("first") == 0) {
                        _this.data("first", 1).data("status", "open");
                        if (cache[_this.data("id")]) {
                            
                            _self.bindChild(cache[_this.data("id")], _this.parent())
                        } 
                    } else {
                        if (_this.data("status") == "open") {
                            _this.data("status", "close");
                            _this.parent().nextAll("tr[data-pid='" + _this.data("id") + "']").hide();
                        } else {
                            _this.data("status", "open");
                            _this.parent().nextAll("tr[data-pid='" + _this.data("id") + "']").show();
                        }
                    }
                });

                $("#userTotalRPT .tr-header").after(innerhtml);
            });
           
        }); 
    }
    ObjectJS.bindChild = function (items, ele) {
        doT.exec("template/report/usercustomers.html", function (template) {
            var innerhtml = template(items);
            innerhtml = $(innerhtml);
            innerhtml.find(".usertotal").each(function () {
                var _this = $(this), total = 0;
                _this.prevAll(".useritem").each(function () {
                    total += $(this).html() * 1;
                });
                _this.html(total);
            });
            ele.after(innerhtml);
        });
    }

    //汇总
    ObjectJS.reportTotal = function () {
        var total = 0;
        $("#userTotalRPT").find(".useritemtotal").each(function () {
            var _this = $(this), stagetotal = 0;
            _this.parent().prevAll(".tr-checked").find(".useritem[data-stageid='" + _this.data("stageid") + "']").each(function () {
                stagetotal += $(this).html() * 1;
            });
            _this.html(stagetotal);
            total += stagetotal;
        });
        $("#userTotalRPT .total-tr .usertotal").html(total);
    }
    module.exports = ObjectJS;
});