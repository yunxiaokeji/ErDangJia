define(function (require, exports, module) {
    var Global = require("global"),
        ec = require("echarts/echarts"),
        moment = require("moment");
    require("echarts/chart/funnel");
    require("echarts/chart/line");
    require("echarts/chart/bar");
    require("daterangepicker");
    var Params = {
        ownerid: '',
        type: 0,
        beginTime: new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd"),
        endTime: Date.now().toString().toDate("yyyy-MM-dd")
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.teamChart = ec.init(document.getElementById('teamRPT'));
        _self.bindEvent();
        _self.showChart();
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
            if (Params.type <2) {
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
                $(".nav-partdiv").hide(); 
                if (!_self.teamChart) {
                    _self.teamChart = ec.init(document.getElementById('teamRPT'));
                }
                Params.type = _this.data("type");
                Params.endTime = Date.now().toString().toDate("yyyy-MM-dd");
                if (Params.type > 1) {
                    $("#" + _this.data("id")).fadeIn(1000);
                }
                if (Params.type == 3) {
                    Params.beginTime = new Date().setMonth(new Date().getMonth() - 3).toString().toDate("yyyy-MM-dd");
                    _self.showTeamChart();
                } else if (Params.type == 2) {
                    Params.beginTime = new Date().setFullYear(new Date().getFullYear() - 1).toString().toDate("yyyy-MM-dd");
                    _self.showTeamChart();
                } else if (Params.type < 2) {
                    Params.beginTime = new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd");
                    _self.showChart();
                }
                $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);
            }
        }); 
    }

    ObjectJS.showChart = function () {
        $('#chartRPT').fadeIn(1000);
        Global.post("/CustomerRPT/GetCustomerStageRate", Params, function (data) {
            var colorList = ['#c3cfdc', '#869fb8', '#688aad', '#587a9d', '#4a7095', '#2a5065', '#104f8f', '#11467c'];
            for (var i = 0; i < data.items.length; i++) {
                var html = '';
                var item = data.items[i];
                for (var j = 0; j < item.sourceItem.length; j++) {
                    var sourceitem = item.sourceItem[j];
                    if (i == 0) {
                        var lineheight = (270 / item.sourceItem.length);
                        //if (j < item.sourceItem.length - 1) { 
                        //    if (j == 0) {
                        //        if (parseInt(sourceitem.value) < parseInt(item.sourceItem[j + 1].value)) {
                        //            lineheight = lineheight - 20;
                        //        }
                        //    } else if (j == item.sourceItem.length - 1) {
                        //        if (parseInt(sourceitem.value) > parseInt(item.sourceItem[j - 1].value)) {
                        //            lineheight = lineheight + 20;
                        //        } 
                        //    } else {
                        //        if (parseInt(sourceitem.value) > parseInt(item.sourceItem[j - 1].value) && parseInt(sourceitem.value) > parseInt(item.sourceItem[j + 1].value)) {
                        //            lineheight = lineheight + 20;
                        //        }
                        //    }
                        //}
                        //console.log(lineheight);
                        html += '<li class=" pauto10 center pTop5 pBottom5 font16" style="max-width:150px;vertical-align:middle;min-height:' + (lineheight - 1) + 'px; border-bottom: 1px solid #fff;" title="' + sourceitem.Name + ' （转化率:' + sourceitem.cvalue + '）">' +
                            '<div class=" "> ' + sourceitem.Name + '<br/><sapn class=" font14"> ' + sourceitem.Value + '</span><br/>' +
                            '<span class="" title="' + sourceitem.Name + '占比率">占比率:' + sourceitem.cvalue + '</span>' +
                            '<div></li>'; 
                    } else if (i == 1) { 
                        var width = 480 / item.sourceItem.length;
                        if (j < item.sourceItem.length - 1) {
                            if (j == 0 ){
                                if (parseInt(sourceitem.value) < parseInt(item.sourceItem[j + 1].value)) {
                                    width = width - 20 ;
                                }
                            } else if (j == item.sourceItem.length - 1 ){
                                if (parseInt(sourceitem.value) > parseInt(item.sourceItem[j - 1].value)) {
                                    width = width + 20;
                                } 
                            } else if (parseInt(sourceitem.value)!=0 &&  parseInt(sourceitem.value) >= parseInt(item.sourceItem[j - 1].value)) {
                                if (parseInt(sourceitem.value) > parseInt(item.sourceItem[j - 1].value) && parseInt(sourceitem.value) > parseInt(item.sourceItem[j + 1].value)) {
                                    width = width + 20;
                                } else if (parseInt(sourceitem.value) == parseInt(item.sourceItem[j - 1].value) || parseInt(sourceitem.value) == parseInt(item.sourceItem[j + 1].value)) {
                                    width = width + 10;
                                } 
                            }
                        }
                        html += '<li class=" left center min-height170" style="min-width:' + width + 'px; background-color:' + (j > colorList.length - 1 ? colorList[colorList.length] : colorList[j]) + ';">' +
                            '<div class="pTop30"> <span class="colorfff">' + sourceitem.Name + '</span><br/>' +
                            '<p class="mTop20 font16 colorfff"  title="' + sourceitem.Name + '新增机会数" >' + sourceitem.Value + '</p><br/>' +
                            '<span class="colorfff" title="' + sourceitem.Name + '转化率">' + sourceitem.value + '%</span>' +
                            '</div>' +
                            '<div class="pTop30 bolder colorfff hide" style="color:#000;">' + sourceitem.desc + '</div>';
                           html +=  '</li>';
                    }
                }
                if (i == 0) {
                    $('#cunstomervalue').html(Params.type == 0 ? item.desc : '客户:' + item.iValue+' /'+item.desc);
                    $('#actcontent').html(html);
                    
                } else if (i == 1) {
                    $('#opporcontentvalue').html(item.desc + ' 转化率(' + item.value + '%)');
                    $('#opporcontent').html(html);
                    $('#opporcontent li').mouseover(function() {
                        var diclist = $(this).find('div');
                        diclist.eq(0).hide();
                        diclist.eq(1).show();
                    }).mouseout(function () {
                        var diclist = $(this).find('div');;
                        diclist.eq(1).hide();
                        diclist.eq(0).show();
                    });
                } else if (i == 2) {
                    $('#ordervalue').html(item.desc + '<br/>转化率(' + item.value + '%)');
                    $('#ordervaluerate').html('订单 <br/><span class="colorfff font16">' + data.items[2].iValue + '</span><br/>');
                    $('#orderrate').html('(' + data.items[2].value + '%)');
                }
            }
            var diffheight = 0;  
            $('#customertooppor').css('border-right-width', '0px'); 
            $('#customertooppor').css('min-height', '170px').css('border-top-width', $('#actcontent').height()-170+"px").css('border-left-width', '30px').css('border-left-style', 'solid').css('border-left-color', '#D8D6F1');

            if (data.items[1].iValue > data.items[0].iValue) {
                $('#opportoorder').css("min-height", 40 + diffheight + "px").css("border-top-width", "120px").css('border-right-width', "0px").css('border-left-width', "30px");
            } 
            $('#chartRPT').css('top', $('#actcontent').height() + 50); 
        }); 
    }

    ObjectJS.showTeamChart = function () {
        $('#chartRPT').hide(); 
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

        Global.post("/CustomerRPT/GetCustomerStageRate", Params, function (data) {
            var title = [], items = [], datanames = [];
            _self.teamChart.clear();
            if (Params.type == 3) {
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
            } else if (Params.type == 2) {
                for (var i = 0, j = data.items.length; i < j; i++) {
                    title.push(data.items[i].Name);
                    var _items = [];
                    for (var ii = 0, jj = data.items[i].Stages.length; ii < jj; ii++) {
                        if (i == 0) {
                            datanames.push(data.items[i].Stages[ii].Name);
                        }
                        _items.push(data.items[i].Stages[ii].Count);
                    }
                    items.push({
                        name: data.items[i].Name,
                        type: 'line',
                        stack: '总量',
                        data: _items
                    });
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
                    text: "暂无数据",
                    x: "center",
                    y: "center",
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