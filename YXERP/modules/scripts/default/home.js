

define(function (require, exports, module) {

    require("jquery");
    var Global = require("global"),
        doT = require("dot");

    var ObjectJS = {};

    //首页JS
    ObjectJS.init = function () {
        
        ObjectJS.bindStyle();
        ObjectJS.bindEvent();

        Global.post('/Home/GetAgentActions', {}, function (data) {
            for (var i = 0; i < data.model.Actions.length; i++) {
                var model = data.model.Actions[i];
                if (model.ObjectType == 0) {
                    $("#loginDay").text(model.DayValue.toFixed(0));
                    $("#loginWeek").text(model.WeekValue.toFixed(0));
                    $("#loginMonth").text(model.MonthValue.toFixed(0));
                } else if (model.ObjectType == 1) {
                    $("#customDay").text(model.DayValue.toFixed(0));
                    $("#customWeek").text(model.WeekValue.toFixed(0));
                    $("#customMonth").text(model.MonthValue.toFixed(0));
                } else if (model.ObjectType == 2) {
                    $("#orderDay").text(model.DayValue.toFixed(0));
                    $("#orderWeek").text(model.WeekValue.toFixed(0));
                    $("#orderMonth").text(model.MonthValue.toFixed(0));
                } else if (model.ObjectType == 3) {
                    $("#activeDay").text(model.DayValue.toFixed(0));
                    $("#activeWeek").text(model.WeekValue.toFixed(0));
                    $("#activeMonth").text(model.MonthValue.toFixed(0));
                } else if (model.ObjectType == 7) {
                    $("#opporDay").text(model.DayValue.toFixed(0));
                    $("#opporWeek").text(model.WeekValue.toFixed(0));
                    $("#opporMonth").text(model.MonthValue.toFixed(0));
                }
            }
        });

    }

    //首页样式
    ObjectJS.bindStyle = function () {

        $(".report-box").fadeIn();

        var width = document.documentElement.clientWidth - 300, height = document.documentElement.clientHeight - 200;

        var unit = 302;
        
        $(".report-box").css({
            width: unit * 3 + 120
        });

        $(".report-box").css({
            marginTop: (document.documentElement.clientHeight - 500) / 2
        });
       
    }

    ObjectJS.bindEvent = function () {
        //调整浏览器窗体
        $(window).resize(function () {
            ObjectJS.bindStyle();
        });
    }
    module.exports = ObjectJS;
});