define(function (require, exports, module) {
    var Global = require("global"),
        ChooseUser = require("chooseuser"),
        ec = require("echarts/echarts"),
        moment = require("moment");
    require("echarts/chart/funnel");
    require("echarts/chart/line");
    require("echarts/chart/bar");
    require("daterangepicker");
    var Params = {
        type: 1,
        OrderMapType: 1,
        beginTime: new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd"),
        endTime: Date.now().toString().toDate("yyyy-MM-dd"),
        UserID: "",
        TeamID: "",
        AgentID: ""
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.teamChart = ec.init(document.getElementById('teamRPT')); 
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
            if (Params.type < 2) {
                _self.showChart();
            } else {
                _self.showTeamChart();
            }
        });
        $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                $(".source-box").hide();
                $("#" + _this.data("id")).show(); 
                Params.type = _this.data("type"); 
                if (_this.data("begintime")) {
                    Params.beginTime= _this.data("begintime"); 
                }
                if (_this.data("endtime")) {
                    Params.endTime = _this.data("endtime");
                } else {
                    Params.endTime = Date.now().toString().toDate("yyyy-MM-dd");
                }
                $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);
                ObjectJS.getRptData(); 
            }
        });

        require.async("dropdown", function () {
            var OrderMapType = [
               { name: "机会金额", value: "1" },
               { name: "机会数量", value: "2" }
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
                    ObjectJS.getRptData();
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
                    ObjectJS.getRptData();
                }
            });
        });
        ObjectJS.getRptData();

    }
    ObjectJS.getRptData = function () {
        var _self = this;
        if (Params.type == 1) {
            $("#showType").hide();
            $("#chooseBranch").show();
            _self.showChart();
        } else if (Params.type == 2) {
            $("#showType").show();
            $("#chooseBranch").hide();
            _self.showTeamChart();
        }
    }
    ObjectJS.showChart = function () {
        var _self = this; 
        Global.post("/SalesRPT/GetOpportunityStageRate", Params, function (data) {
            var colorList = ['#9BA8AD', '#60DCFF', '#D8D6F1', '#FFABAB', '#7FC2EC', '#60DCFF', '#C0BBFB', '#18384C','#C13F3F'];
            var innerhtml = '';
            var liList = '';
            var marginleft = 30;var totalleft = 0;
            var totallent = 500;
            for (var i = 0, j = data.items.length; i < j; i++) {
                var item = data.items[i];
                if (i == 0) {
                    if ((i + 1) < j && item.value > data.items[i + 1]) {
                        marginleft += 10;
                    }
                } else if (i == (j - 1)) {
                    if (i > 0 && item.value > data.items[i-1]) {
                        marginleft += 10;
                    } 
                } else {
                    if (item.value > data.items[i + 1] && item.value > data.items[i - 1]) {
                        marginleft += 20;
                    } else if( item.value < data.items[i + 1] && item.value > data.items[i - 1]){
                        marginleft += 10;
                    } else if (item.value < data.items[i + 1] && item.value < data.items[i - 1]) {
                        marginleft -= 5;
                    }
                }
                if (i == 0) {
                    totallent += 2 * marginleft;
                }
                var headerdiv = '<div class="cont mTop10" style=""  title="' + data.items[i].name +' 数量:'+ data.items[i].iValue + '(' + data.items[i].value+ ')">';;
                
                var previousleft = totalleft;
                totalleft += marginleft;
                if (i < j - 1) { 
                    innerhtml += headerdiv.replace('style="', 'style="margin-left:' + (i > 0 ? previousleft : 0) + 'px;"') + '<div class="taper-left" style="border-top-width:' + marginleft * 2 + 'px;border-left-width:' + marginleft + 'px;border-top-color:' + colorList[i] + ';"></div>' +
                        '<div class="taper-center"  style="background-color:' + colorList[i] + ';width: ' + (i > 0 ? totallent - totalleft * 2 : 500) + 'px;height:' + (marginleft * 2 - 10) + 'px; line-height:' + (marginleft - 5) + 'px;">' + data.items[i].name.replace('(', '<br/>(') + ':' + data.items[i].iValue + '</br>' + data.items[i].desc + ' 占比率: ' + data.items[i].value + '</div>' +
                        '<div class="taper-right" style="border-top-color:' + colorList[i] + ';border-top-width:' + marginleft * 2 + 'px;border-right-width:' + marginleft + 'px;"></div>' +
                        '</div>';
                }
                if (i == j - 1) {
                    var lastwidth = (i > 0 ? totallent - totalleft * 2 : 500);
                    var lastheight = marginleft; 
                    innerhtml += headerdiv.replace('style="', 'style="margin-left:' + (i > 0 ? previousleft : 0) + 'px;"') + '<div class="taper-left" style="border-top-width:' + marginleft * 2 + 'px;border-left-width:' + marginleft + 'px;border-top-color:' + colorList[i] + ';"></div>' +
                        '<div class="taper-center"  style="background-color:' + colorList[i] + ';width: ' + lastwidth + 'px;height:' + (marginleft * 2 - 10) + 'px; line-height:' + (marginleft - 5) + 'px;">' + data.items[i].name.replace('(', '<br/>(') + ':' + data.items[i].iValue + '</br>' + data.items[i].desc + ' 占比率: ' + data.items[i].value + '</div>' +
                        '<div class="taper-right" style="border-top-color:' + colorList[i] + ';border-top-width:' + marginleft * 2 + 'px;border-right-width:' + marginleft + 'px;"></div>' +
                        '</div>'; 
                    innerhtml += headerdiv.replace("cont mTop10", "").replace('style="', 'style="margin-left:' + (i > 0 ? totalleft : 0) + 'px;border-bottom-width: 0px;text-align:center; float:left;border-style: solid;border-color:' + colorList[i] + ' transparent;border-left-width:' + lastwidth / 16 + 'px;border-right-width:' + lastwidth / 16 + 'px;border-top-width:' + lastwidth / 8 + 'px;width:' + lastwidth *7/8+ 'px;') + '</div>';
                }
                marginleft = 30;
                liList += '<li style="list-style-type: none;overflow: auto"><span class="mTop3 left" style="min-width:11px;min-height:14px;background-color:' + colorList[i] + ';"></span><span class="mLeft10 left">' + data.items[i].name + '('+data.items[i].value+')</span> </li>';
            }
            $('#funnelContent').html(innerhtml);
            $('#colorList').html(liList);
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
                noDataLoadingOption: {
                    text: "",                   
                    textStyle: {
                        color: "red",
                        fontSize: 14
                    },
                    effect: "bubble"
                },
                series: items
            };
            _self.teamChart.hideLoading();
            _self.teamChart.setOption(option);
        });
    }
    module.exports = ObjectJS;
});