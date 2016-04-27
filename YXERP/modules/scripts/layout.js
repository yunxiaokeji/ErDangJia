/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        doT = require("dot"),
        Global = require("global"),
        Easydialog = require("easydialog");

    var LayoutObject = {};
    //初始化数据
    LayoutObject.init = function () {
        
        LayoutObject.bindStyle();
        LayoutObject.bindEvent();

    }

    //绑定元素定位和样式
    LayoutObject.bindStyle = function () {

    }
    //绑定事件
    LayoutObject.bindEvent = function () {
        var _self = this;

        //展开筛选
        $(".btn-filter").click(function () {
            $(".search-body").show("fast");
        });
        //折叠筛选
        $(".close-filter span").click(function () {
            $(".search-body").hide("fast");
        });
        //打开新窗口
        $("body").delegate(".btn-open-window", "click", function () {
            var _this = $(this),
                parent = $(window.parent.document),
                nav = parent.find("#windowItems li[data-id='" + _this.data("id") + "']");

            parent.find("#windowItems li").removeClass("hover");
            parent.find(".iframe-window").hide();
            if (nav.length == 1) {
                nav.addClass("hover");
                parent.find("#iframe" + _this.data("id")).show();
            } else {
                parent.find("#windowItems").append('<li data-id="' + _this.data("id") + '" class="hover" title="' + _this.data("name") + '">'
                                              + _this.data("name") + ' <span title="关闭" class="iconfont close">&#xe606;</span>'
                                       + '</li>');
                parent.find("#iframeBox").append('<iframe id="iframe' + _this.data("id") + '" class="iframe-window" src="' + _this.data("url") + '"></iframe>');

                var height = window.parent.document.documentElement.clientHeight;
                parent.find("#iframe" + _this.data("id")).css("height", height - 100);
            }
        });

    }

    module.exports = LayoutObject;
})