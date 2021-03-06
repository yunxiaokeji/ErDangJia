﻿define(function (require, exports, module) {
    var Global = require("global"),
        ChooseUser = require("chooseuser"),
        doT = require("dot"),
        ec = require("echarts/echarts"),
        moment = require("moment");
    require("echarts/chart/pie");
    require("echarts/chart/map");
    require("daterangepicker");
    var Params = {
        type:1,
        beginTime:new Date().setMonth(new Date().getMonth() - 6).toString().toDate("yyyy-MM-dd"),
        endTime: Date.now().toString().toDate("yyyy-MM-dd"),
        UserID: "",
        TeamID: "",
        AgentID: ""
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.customermapChart = ec.init(document.getElementById('customermapRPT'));
        _self.customerindustryRPT = ec.init(document.getElementById('customerindustryRPT'));
        _self.bindEvent();
    }
    ObjectJS.bindEvent = function () {
        var _self = this;

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
            Params.beginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.endTime = end ? end.format("YYYY-MM-DD") : "";
            ObjectJS.customerFind();
        });
        $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);

        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
            }
            $('.branchitem').show();
            $('#chooseBranch').show();
            Params.type = _this.data("type");
            $(".source-box").hide();
            $("#" + _this.data("id")).show(); 
            ObjectJS.customerFind();
        });

        require.async("choosebranch", function () {
            $("#chooseBranch").chooseBranch({
                prevText: "人员-",
                defaultText: "全部",
                defaultValue: "",
                userid: "-1",
                isTeam: true,
                width: "180",
                onChange: function (data) {
                    Params.UserID = data.userid;
                    Params.TeamID = data.teamid;
                    ObjectJS.customerFind();
                }
            });
        });

        ObjectJS.customermap();

    }
    //Params.type事件调用
    ObjectJS.customerFind=function()
    {
        var _self = this;
        if (Params.type == 1) {
            _self.customermap();
        } else if (Params.type == 2 || Params.type == 3 || Params.type == 4) {
            _self.customerindustry();
        } else if (Params.type == 5 || Params.type == 6) {
            if (Params.type == 5) {
                $('#chooseBranch').hide();
            } else {
                $('.branchitem').hide();
            }
            _self.getUserCustomer();
        }
    }
    //客户地区分布统计
    ObjectJS.customermap = function () {
        var _self = this;
        _self.customermapChart.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });

        Global.post("/CustomerRPT/GetCustomerReport", Params, function (data) {
            var maxCount = 0;
            var dataItems = [];
            var selDataItems = [];
            var selNameItems = [];
            var total = 0;
            for (var i = 0; len = data.items.length, i < len; i++) {
                maxCount = parseInt(data.items[0].value);
                total += data.items[i].value;
                if (i < 3) {
                    selDataItems.push({name: data.items[i].name,value:data.items[i].value });
                    selNameItems.push(data.items[i].name);
                    data.items[i].selected = true;
                }
                dataItems.push(data.items[i]);
            }

            option = {
                title: {
                    text: '客户地区占比',
                    subtext: "合计：" + total + "(100.00%)"
                },
                tooltip: {
                    trigger: 'item'
                },
                legend: {
                    x: 'right',
                    selectedMode: false,
                    data: selNameItems
                },
                dataRange: {
                    orient: 'horizontal',
                    min: 0,
                    max: maxCount,
                    text: ['高', '低'],           // 文本，默认为数值文本
                    splitNumber: 0
                },
                toolbox: {
                    show: true,
                    orient: 'vertical',
                    x: 'right',
                    y: 'center',
                    feature: {
                        mark: { show: true },
                        dataView: { show: true, readOnly: false },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                series: [
                    {
                        name: '客户地区占比',
                        type: 'map',
                        mapType: 'china',
                        mapLocation: {
                            x: 'left'
                        },
                        selectedMode: 'multiple',
                        itemStyle: {
                            normal: { label: { show: true } },
                            emphasis: { label: { show: true } }
                        },
                        data:dataItems
                    },
                    {
                        name: '客户地区分布',
                        type: 'pie',
                        roseType: 'area',
                        tooltip: {
                            trigger: 'item',
                            formatter: "{a} <br/>{b} : {c} ({d}%)"
                        },
                        center: [document.getElementById('customermapRPT').offsetWidth - 250, 225],
                        radius: [30, 120],
                        data:selDataItems,
                        animation: false
                    }
                ]
            };
            var ecConfig = require('echarts/config');
            _self.customermapChart.on(ecConfig.EVENT.MAP_SELECTED, function(param) {
                var selected = param.selected;
                var mapSeries = option.series[0];
                var data = [];
                var legendData = [];
                var name;
                for (var p = 0, len = mapSeries.data.length; p < len; p++) {
                    name = mapSeries.data[p].name;
                    if (selected[name]) {
                        data.push({
                            name: name,
                            value: mapSeries.data[p].value
                        });
                        legendData.push(name);
                    }
                }
                option.legend.data = legendData;
                option.series[1].data = data;
                _self.customermapChart.setOption(option, true);
            });
            _self.customermapChart.hideLoading();
            _self.customermapChart.setOption(option);
        });
    }
 
    //客户行业分布统计
    ObjectJS.customerindustry = function () {
        var _self = this;
        _self.customerindustryRPT.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });

        Global.post("/CustomerRPT/GetCustomerReport", Params, function (data) {
            var title = [], items = [],total=0,titleRPT='';

            _self.customerindustryRPT.clear();

            var ExtentArr = ["0-49人", "50-99人", "100-199人", "200-499人", "500-999人", "1000+人"];
            titleRPT = "客户行业占比";
            if (Params.type == 3) {
                titleRPT = "客户规模占比";
            } else if (Params.type == 4) {
                titleRPT = "客户标签占比";
            }
            for (var i = 0, j = data.items.length; i < j; i++) {
                total += data.items[i].value;
                if (Params.type == 3) {
                    data.items[i].name = ExtentArr[parseInt(data.items[i].name) - 1];
                }
            }

            for (var i2 = 0, j2 = data.items.length; i2 < j2; i2++) {
                data.items[i2].name = data.items[i2].name + "：" + data.items[i2].value + "(" + ( (data.items[i2].value / total)*100 ).toFixed(2) + "%)";
                title.push(data.items[i2].name);
                items.push(data.items[i2]);
            }

            option = {
                title: {
                    text: titleRPT,
                    subtext: "合计：" + total + "(100.00%)",
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    x: 'left',
                    data: title
                },
                calculable: true,
                toolbox: {
                    show: true,
                    orient: 'vertical',
                    x: 'right',
                    y: 'center',
                    feature: {
                        mark: { show: true },
                        dataView: { show: true, readOnly: false },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                series: [
                    {
                        name: titleRPT,
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', 245],
                        data: items
                    }
                ]
            }; 
            _self.customerindustryRPT.hideLoading();
            _self.customerindustryRPT.setOption(option);
        });
    }
    ///团队汇总
    ObjectJS.getUserCustomer = function () {
        var _self = this; 
        $("#userTotalRPT .tr-header").nextAll().remove();
        Global.post("/CustomerRPT/GetUserCustomers", Params, function (data) {
            if (Params.type == 6) {
                var crslist = ['TotalList', 'NCSRList', 'OCSRList', 'SCSRList'];
                for (var j = 0; j < crslist.length; j++) {
                    $("#" + crslist[j] + " .tr-header").nextAll().remove();
                    var innerhtml = ''; 
                    if (data.items[crslist[j]].length > 0) {
                        for (var i = 0; i < data.items[crslist[j]].length; i++) {
                            innerhtml += '<tr class="item-tr"><td class="center">' + data.items[crslist[j]][i].PName + '</td>' +
                                '<td class="center">' + data.items[crslist[j]][i].Name + '</td>' +
                                '<td class="center">' + (j == 0 ? data.items[crslist[j]][i].TotalNum : j == 1 ? data.items[crslist[j]][i].NCSRNum : j == 2 ? data.items[crslist[j]][i].OCSRNum : j == 3 ? data.items[crslist[j]][i].SCSRNum : 0) + '</td>' +
                                '</tr>';
                        }
                    } else {
                        innerhtml = '<tr class="item-tr"><td class="center" colspan="3">暂无</td></tr>';
                    }
                    $("#" + crslist[j] + " .tr-header").after(innerhtml);
                }
            } else {
                var cache = [];
                for (var i = 0; i < data.items.length; i++) {
                    if (data.items[i].ChildItems && data.items[i].ChildItems.length > 0) {
                        cache[data.items[i].GUID] = data.items[i].ChildItems;
                    }
                }
                doT.exec("template/report/teamcustomers.html", function(template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    innerhtml.find(".useritemtotal").each(function() {
                        var _this = $(this), total = 0;
                        _this.parent().prevAll().find(".useritem[data-stageid='" + _this.data("stageid") + "']").each(function() {
                            total += $(this).html() * 1;
                        });
                        _this.html(total);
                    });
                    innerhtml.find(".usertotal").each(function() {
                        var _this = $(this), total = 0;
                        _this.prevAll(".useritem").each(function() {
                            total += $(this).html() * 1;
                        });
                        _this.html(total);
                    });
                    //选择汇总
                    innerhtml.find(".check").click(function() {
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
                    innerhtml.find(".open-child").click(function() {
                        var _this = $(this);
                        if (!_this.data("first") || _this.data("first") == 0) {
                            _this.data("first", 1).data("status", "open");
                            if (cache[_this.data("id")]) {
                                _self.bindChild(cache[_this.data("id")], _this.parent());
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
            }
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