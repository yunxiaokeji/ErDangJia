define(function (require, exports, module) {
    var Global = require("global"),
        ChooseUser = require("chooseuser"),
        ec = require("echarts/echarts"),
        moment = require("moment");
    require("daterangepicker");
    require("echarts/chart/pie");
    require("echarts/chart/line");
    require("echarts/chart/bar");
    var Params = {
        searchType: "sourceRPT",
        dateType: 3,
        beginTime:new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd") ,
        endTime: Date.now().toString().toDate("yyyy-MM-dd"),
        UserID: "",
        TeamID: "",
        AgentID: ""
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.scaleChart = ec.init(document.getElementById('sourceRPT'));
        _self.bindEvent();
        _self.sourceScale();
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
            Params.pageIndex = 1;
            Params.beginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.endTime = end ? end.format("YYYY-MM-DD") : "";
            if (Params.searchType == "sourceRPT") {
                _self.sourceScale();
            } else {
                _self.sourceDate();
            }
        });
        $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.searchType = _this.data("id");
                Params.dateType = _this.data("type");
                $(".source-box").hide();
                $("#" + _this.data("id")).show();
                if (!_self.dateChart) {
                    _self.dateChart = ec.init(document.getElementById('sourceDateRPT'));
                }
                 
                if (Params.dateType == 3) {
                    Params.beginTime = new Date().setFullYear(new Date().getFullYear() - 1).toString().toDate("yyyy-MM-dd");
                } else if (Params.dateType == 4) {
                    Params.beginTime = new Date().setMonth(new Date().getMonth() - 3).toString().toDate("yyyy-MM-dd");
                } else if (Params.dateType == 5) {
                    Params.beginTime = new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd");
                } 
                if (_this.data("endtime")) {
                    Params.endTime = _this.data("endtime");
                } else {
                    Params.endTime = Date.now().toString().toDate("yyyy-MM-dd");
                }
                $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);
                if (Params.searchType == "sourceRPT") {
                    _self.sourceScale();
                } else {
                    _self.sourceDate();
                }
                $(".search-type .hover").data("endtime", Params.endTime);
            }
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
    }
    //客户来源比例
    ObjectJS.sourceScale = function () {
        var _self = this;
        _self.scaleChart.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });

        Global.post("/CustomerRPT/GetCustomerSourceScale", Params, function (data) {
            var title = [], items = [], name = "", total = 0;
            for (var i = 0, j = data.items.length; i < j; i++) {
                total += data.items[i].Value;
                name = data.items[i].Name + "：" + data.items[i].Value + "(" + data.items[i].Scale + ")";
                title.push(name);
                items.push({
                    value: data.items[i].Value,
                    name: name
                });
            }
            option = {
                title: {
                    text: '客户来源比例',
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
                toolbox: {
                    show: true,
                    feature: {
                        dataView: { show: true, readOnly: false },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                calculable: true,
                series: [
                    {
                        name: '客户来源',
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '60%'],
                        data: items
                    }
                ]
            };
            _self.scaleChart.hideLoading();
            _self.scaleChart.setOption(option);
        }); 
    }
    //按时间周期
    ObjectJS.sourceDate = function () {
        var _self = this;
        _self.dateChart.showLoading({
            text: "数据正在努力加载...",
            x: "center",
            y: "center",
            textStyle: {
                color: "red",
                fontSize: 14
            },
            effect: "spin"
        });
        Global.post("/CustomerRPT/GetCustomerSourceDate", Params, function (data) {

            var title = [], items = [], datanames = [];
            _self.dateChart.clear();
            for (var i = 0, j = data.items.length; i < j; i++) {
                title.push(data.items[i].Name);
                var _items = [];
                for (var ii = 0, jj = data.items[i].Items.length; ii < jj; ii++) {
                    if (i == 0) {
                        datanames.push(data.items[i].Items[ii].Name);
                    }
                    _items.push(data.items[i].Items[ii].Value);
                }
                items.push({
                    name: data.items[i].Name,
                    type: 'line',
                    stack: '总量',
                    data: _items
                });
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
                        dataView: {
                            show: true,
                            readOnly: false,
                            optionToContent: function (opt) {
                                var axisData = opt.xAxis[0].data;
                                var series = opt.series;
                                var table = '<table class="table-list"><tr class="tr-header">'
                                             + '<td>时间</td>';
                               for (var i = 0, l = series.length; i < l; i++) {
                                   table += '<td>' + series[i].name + '</td>'
                               }
                               table += '</tr>';
                                for (var i = 0, l = axisData.length; i < l; i++) {
                                    table += '<tr>'
                                    + '<td class="center">' + axisData[i] + '</td>'
                                    for (var ii = 0, ll = series.length; ii < ll; ii++) {
                                        table += '<td class="center">' + series[ii].data[i] + '</td>';
                                    }
                                    
                                    table += '</tr>';
                                }
                                table += '</table>';
                                return table;
                            }
                        },
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
            _self.dateChart.hideLoading();
            _self.dateChart.setOption(option);
        });
    }

    module.exports = ObjectJS;
});