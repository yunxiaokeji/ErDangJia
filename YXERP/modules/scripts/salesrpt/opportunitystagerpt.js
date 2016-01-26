﻿define(function (require, exports, module) {
    var Global = require("global"),
        ChooseUser = require("chooseuser"),
        ec = require("echarts/echarts");
    require("echarts/chart/funnel");
    require("echarts/chart/line");
    require("echarts/chart/bar");
    var Params = {
        type: 1,
        OrderMapType: 1,
        beginTime: "",
        endTime: "",
        UserID: "",
        TeamID: "",
        AgentID: ""
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        
        _self.myChart = ec.init(document.getElementById('chartRPT'));

        _self.bindEvent();
    }
    ObjectJS.bindEvent = function () {
        var _self = this;

        $("#beginTime").val(new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd"));
        $("#endTime").val(Date.now().toString().toDate("yyyy-MM-dd"));

        $(".search-type li").click(function () {
            var _this = $(this);
            
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                $(".source-box").hide();
                $("#" + _this.data("id")).show();

                if (!_self.teamChart) {
                    _self.teamChart = ec.init(document.getElementById('teamRPT'));
                }

                Params.type = _this.data("type");
                if (Params.type == 1) {
                    $("#showType").hide();
                    $("#chooseBranch").show();
                } else {
                    $("#showType").show();
                    $("#chooseBranch").hide();
                }
                if (_this.data("begintime")) {
                    $("#beginTime").val(_this.data("begintime"));
                }
                if (_this.data("endtime")) {
                    $("#endTime").val(_this.data("endtime"));
                } else {
                    $("#endTime").val(Date.now().toString().toDate("yyyy-MM-dd"));
                }

                $("#btnSearch").click();
            }

        });

        require.async("dropdown", function () {
            var OrderMapType = [
               {
                   name: "机会金额",
                   value: "1"
               },
               {
                   name: "机会数量",
                   value: "2"
               }
            ];

            $("#showType").dropdown({
                prevText: "类型-",
                defaultText: "机会金额",
                defaultValue: "1",
                data: OrderMapType,
                dataValue: "value",
                dataText: "name",
                width: "140",
                onChange: function (data) {
                    Params.OrderMapType = data.value;
                    $("#btnSearch").click();
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
                    $("#btnSearch").click();
                }
            });
        });

        $("#btnSearch").click(function () {
            Params.beginTime = $("#beginTime").val().trim();
            Params.endTime = $("#endTime").val().trim();
            if (Params.beginTime && Params.endTime && Params.beginTime > Params.endTime) {
                alert("开始日期不能大于结束日期！");
                return;
            }
            if (Params.type == 1) {
                _self.showChart();
            } else if (Params.type == 2) {
                _self.showTeamChart();
            }
            $(".search-type .hover").data("begintime", Params.beginTime).data("endtime", Params.endTime);
        });

        $("#btnSearch").click();

    }

    ObjectJS.showChart = function () {
        var _self = this;
        _self.myChart.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });

        Global.post("/SalesRPT/GetOpportunityStageRate", Params, function (data) {
            var title = [];

            for (var i = 0, j = data.items.length; i < j; i++) {
                title.push(data.items[i].name);
            }
            option = {
                title: {
                    text: '销售漏斗',
                    subtext: "预计成交总金额：" + data.forecast.toFixed(2),
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{b} : {c}%"
                },
                toolbox: {
                    show: true,
                    feature: {
                        mark: { show: true },
                        dataView: { show: true, readOnly: false },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                //legend: {
                //    orient: "vertical",
                //    x: "right",
                //    y: "center",
                //    data: title
                //},
                series: [
                    {
                        name: '机会订单阶段',
                        type: 'funnel',
                        width: '50%',
                        x: "20%",
                        data: data.items
                    }
                ]
            };
            _self.myChart.hideLoading();
            _self.myChart.setOption(option);
        }); 
    }

    ObjectJS.showTeamChart = function () {
        var _self = this;
        _self.teamChart.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });

        Global.post("/SalesRPT/GetUserOpportunitys", Params, function (data) {

            var title = [], items = [], datanames = [];
            _self.teamChart.clear();

            if (Params.OrderMapType == 2) {
                for (var i = 0, j = data.items.length; i < j; i++) {
                    datanames.push(data.items[i].Name);
                    for (var ii = 0, jj = data.items[i].Stages.length; ii < jj; ii++) {
                        if (i == 0) {
                            title.push(data.items[i].Stages[ii].Name);
                            items.push({
                                name: data.items[i].Stages[ii].Name,
                                type: 'line',
                                stack: '总量',
                                data: [data.items[i].Stages[ii].Count]
                            });
                        } else {
                            items[ii].data.push(data.items[i].Stages[ii].Count);
                        }
                    }
                }
            } else {
                for (var i = 0, j = data.items.length; i < j; i++) {
                    datanames.push(data.items[i].Name);
                    for (var ii = 0, jj = data.items[i].Stages.length; ii < jj; ii++) {
                        if (i == 0) {
                            title.push(data.items[i].Stages[ii].Name);
                            items.push({
                                name: data.items[i].Stages[ii].Name,
                                type: 'line',
                                stack: '总量',
                                data: [data.items[i].Stages[ii].Money]
                            });
                        } else {
                            items[ii].data.push(data.items[i].Stages[ii].Money);
                        }
                    }
                }
            }

            option = {
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: title
                },
                toolbox: {
                    show: true,
                    feature: {
                        magicType: { show: true, type: ['line', 'bar'] },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                xAxis: [
                    {
                        type: 'category',
                        boundaryGap: false,
                        data: datanames
                    }
                ],
                yAxis: [
                    {
                        type: 'value'
                    }
                ],
                series: items
            };
            _self.teamChart.hideLoading();
            _self.teamChart.setOption(option);
        });
    }
    module.exports = ObjectJS;
});