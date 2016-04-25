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
        //调整浏览器窗体
       
    }

    module.exports = LayoutObject;
})