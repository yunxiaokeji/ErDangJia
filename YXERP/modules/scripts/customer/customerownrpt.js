define(function (require, exports, module) {
    var Global = require("global"), 
        moment = require("moment"); 
    require("daterangepicker");
    var Params = {
        type: 0,
        ownerid:'',
        beginTime: new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd"),
        endTime: Date.now().toString().toDate("yyyy-MM-dd")
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (userid) {
        var _self = this;
        Params.ownerid = userid;
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
             _self.showChart();
        });
        $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);

        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                $(".nav-partdiv").hide(); 
                Params.type = _this.data("type");
                Params.endTime = Date.now().toString().toDate("yyyy-MM-dd"); 
                Params.beginTime = new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd");
                $("#iptCreateTime").val(Params.beginTime + ' 至 ' + Params.endTime);
                _self.showChart();
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
                        var lineheight = 270 / item.sourceItem.length;
                        if (j < item.sourceItem.length - 1) {
                            if (j == 0) {
                                if (parseInt(sourceitem.value) < parseInt(item.sourceItem[j + 1].value)) {
                                    lineheight = lineheight - 20;
                                }
                            } else if (j == item.sourceItem.length - 1) {
                                if (parseInt(sourceitem.value) > parseInt(item.sourceItem[j - 1].value)) {
                                    lineheight = lineheight + 20;
                                }
                            } else {
                                if (parseInt(sourceitem.value) > parseInt(item.sourceItem[j - 1].value) && parseInt(sourceitem.value) > parseInt(item.sourceItem[j + 1].value)) {
                                    lineheight = lineheight + 20;
                                }
                            }
                        }
                        html += '<li class=" pauto20 center" style="vertical-align:middle;min-height:' + (lineheight - 1) + 'px; border-bottom: 1px solid #fff;" title="' + sourceitem.Name + ' （转化率:' + sourceitem.cvalue + '）">' +
                            '<div class="pTop20 "> ' + sourceitem.Name + '<br/><sapn class=" font16"> ' + sourceitem.Value + '</span><br/>' +
                            '<span class="" title="' + sourceitem.Name + '占比率">占比率:' + sourceitem.cvalue + '</span>' +
                            '<div></li>';
                    } else if (i == 1) {
                        var width = 480 / item.sourceItem.length;
                        if (j < item.sourceItem.length - 1) {
                            if (j == 0) {
                                if (parseInt(sourceitem.value) < parseInt(item.sourceItem[j + 1].value)) {
                                    width = width - 20;
                                }
                            } else if (j == item.sourceItem.length - 1) {
                                if (parseInt(sourceitem.value) > parseInt(item.sourceItem[j - 1].value)) {
                                    width = width + 20;
                                }
                            } else if (parseInt(sourceitem.value) != 0 && parseInt(sourceitem.value) >= parseInt(item.sourceItem[j - 1].value)) {
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
                        html += '</li>';
                    }
                }
                if (i == 0) {
                    $('#cunstomervalue').html(Params.type == 0 ? item.desc : '客户:' + item.iValue + ' /' + item.desc);
                    $('#actcontent').html(html);
                } else if (i == 1) {
                    $('#opporcontentvalue').html(item.desc + ' 转化率(' + item.value + '%)');
                    $('#opporcontent').html(html);
                    $('#opporcontent li').mouseover(function () {
                        var diclist = $(this).find('div');
                        diclist.eq(0).hide();
                        diclist.eq(1).show();
                    });
                    $('#opporcontent li').mouseout(function () {
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
            if (data.items[1].iValue > data.items[0].iValue) {
                diffheight = 150;
                if (data.items[1].iValue < data.items[2].iValue) {
                    diffheight = 110;
                    $('#customertooppor').css('min-height', 150 + diffheight + 'px').css('border-top-width', "20px");
                } else {
                    $('#customertooppor').css('min-height', 120 + diffheight + 'px').css('border-top-width', "50px");
                }
                $('.min-height170').css("min-height", 170 + diffheight + "px");
                $('#customertooppor').css('border-left-width', '0px');
                $('#customertooppor').css('border-right-width', "40px").css('border-right-style', 'solid').css('border-right-color', '#D8D6F1');
            } else {
                $('#customertooppor').css('border-right-width', '0px');
                $('#opporcontent').css('min-height', '170px');
                $('#customertooppor').css('min-height', '170px').css('border-top-width', "100px").css('border-left-width', '30px').css('border-left-style', 'solid').css('border-left-color', '#D8D6F1');
            }
            if (data.items[1].iValue >= data.items[2].iValue) {
                if (data.items[1].iValue > data.items[0].iValue) {
                    $('#opportoorder').css("min-height", 40 + diffheight + "px").css("border-top-width", "120px").css('border-right-width', "0px").css('border-left-width', "30px");
                }
                $('.min-height150').css("min-height", "150px");
            } else {
                $('.min-height150').css("min-height", 150 + diffheight + "px");
                $('.min-height120').css("min-height", 190 + diffheight + "px");
                $('#opportoorder').css("border-right-width", "0px");
                $('#opportoorder').css('min-height', 170 + diffheight + "px").css('border-left-width', "0px").css('border-top-width', "20px").css('border-right-width', "30px").css('border-right-style', 'solid').css('border-right-color', '#C3E7F6');
            }
        });
    }
    module.exports = ObjectJS;
});