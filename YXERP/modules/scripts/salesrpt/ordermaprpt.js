﻿define(function (require, exports, module) {
    var Global = require("global"),
        ChooseUser = require("chooseuser"),
        ec = require("echarts/echarts"),
        moment = require("moment");
    require("daterangepicker");
    require("echarts/chart/pie");
    require("echarts/chart/map");
    var Params = {
        type:1,
        beginTime: "",
        endTime: "",
        OrderMapType: 1,
        UserID: "",
        TeamID: "",
        AgentID: ""
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (types) {
        var _self = this;
        _self.ordermapChart = ec.init(document.getElementById('ordermapRPT'));
        _self.bindEvent(types);
    }
    ObjectJS.bindEvent = function (types) {
        var _self = this; 
        require.async("dropdown", function () {
            var OrderMapType=[
                { name: "订单金额", value:"1" },
                { name:"订单数量", value:"2"  }
            ];
            $("#OrderMapType").dropdown({
                prevText: "统计类型-",
                defaultText: "订单金额",
                defaultValue: "1",
                data: OrderMapType,
                dataValue: "value",
                dataText: "name",
                width: "140",
                onChange: function (data)
                {
                    Params.OrderMapType = data.value;
                    ObjectJS.clickli();
                }
            });

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
                    ObjectJS.clickli();
                }
            });
        });

        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
            }
            Params.type = _this.data("type");
            $(".source-box").hide();
            $("#" + _this.data("id")).show();
            if (!_self.ordertypeRPT) {
                _self.ordertypeRPT = ec.init(document.getElementById('ordertypeRPT'));
            }
            ObjectJS.clickli();
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
            Params.beginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.endTime = end ? end.format("YYYY-MM-DD") : "";
            ObjectJS.clickli();
        });

        ObjectJS.ordermap();

    }
    ObjectJS.clickli = function () {
        var _self = this;
        if (Params.type == 1) {
            _self.ordermap();
        }
        else if (Params.type == 2) {
            _self.ordertype();
        }
    }
    //订单地区分布统计
    ObjectJS.ordermap = function () {
        var _self = this;
        _self.ordermapChart.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });

        Global.post("/SalesRPT/GetOrderMapReport", Params, function (data) {
            var maxCount = 0;
            var dataItems = [];
            var selDataItems = [];
            var selNameItems = [];
            var title = "订单总额";
            var total = 0;
            if (Params.OrderMapType == 2)
                title = "订单数量";

            for (var i = 0; len = data.items.length, i < len; i++)
            {
                maxCount = parseInt(data.items[0].value);
                total += data.items[i].value;
                if (Params.OrderMapType==1)
                    data.items[i].value = data.items[i].total_money;

                if (i < 3)
                {
                    selDataItems.push({name: data.items[i].name,value:data.items[i].value });
                    selNameItems.push(data.items[i].name);
                    data.items[i].selected = true;
                }
                dataItems.push(data.items[i]);
            }

            option = {
                title: {
                    text: '订单地区占比',
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
                        name: title,
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
                        name: title,
                        type: 'pie',
                        roseType: 'area',
                        tooltip: {
                            trigger: 'item',
                            formatter: "{a} <br/>{b} : {c} ({d}%)"
                        },
                        center: [document.getElementById('ordermapRPT').offsetWidth - 250, 225],
                        radius: [30, 120],
                        data:selDataItems,
                        animation: false
                    }
                ]
            };
            var ecConfig = require('echarts/config');
            _self.ordermapChart.on(ecConfig.EVENT.MAP_SELECTED, function (param) {
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
                _self.ordermapChart.setOption(option, true);
            })
            _self.ordermapChart.hideLoading();
            _self.ordermapChart.setOption(option);
        });
    }
 
    //订单类型分布统计
    ObjectJS.ordertype = function () {
        var _self = this;
        _self.ordertypeRPT.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "whirling"
        });

        Global.post("/SalesRPT/GetOrderMapReport", Params, function (data) {
            var title = [], items = [],total=0; 
            for (var i = 0, j = data.items.length; i < j; i++) {
                if (Params.OrderMapType == 1)
                    data.items[i].value = data.items[i].total_money;
                total += data.items[i].value;
            }

            for (var i2 = 0, j2 = data.items.length; i2 < j2; i2++) {
                data.items[i2].name = data.items[i2].name + "：" + data.items[i2].value + "(" + ((data.items[i2].value / total) * 100).toFixed(2) + "%)";
                title.push(data.items[i2].name);
                items.push(data.items[i2]);
            }

            option = {
                title: {
                    text: '订单类型占比',
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
                noDataLoadingOption:{
                    text: "暂无数据",
                    x: "center",
                    y: "center",
                    textStyle: {
                        color: "red",
                        fontSize: 14
                    },
                    effect: "bubble"
                }, 
                series: [
                    {
                        name: '订单类型',
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', 225],
                        data: items
                    }
                ]
            };

            _self.ordertypeRPT.hideLoading();
            _self.ordertypeRPT.setOption(option);
            title = []; items = [];

        });
    }

    module.exports = ObjectJS;
});