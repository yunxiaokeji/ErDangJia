
/* 
作者：Michaux
日期：2016-06-13
示例:
    $(...).markColor(options);
*/
define(function (require, exports, module) {
    require("plug/colormark/style.css");
    var Global = require("global");
    (function($) {
        $.fn.markColor = function(options) {
            var opts = $.extend({}, $.fn.markColor.defaults, options);
            return this.each(function() {
                var _this = $(this);
                $.fn.drawmarkColor(_this, opts);
            });
        }
        $.fn.markColor.defaults = {
            isAll: false,
            data: [],
            dataValue: "ColorID",
            dataColor: "ColorValue",
            dataText: "ColorName",
            onChange: function() {}
        };
        $.fn.drawmarkColor = function (obj, opts) {
            obj.data("itemid", Global.guid());
            obj.addClass("mark-color");

            if (obj.data("value") >= 0) {
                obj.css("background-color", $.fn.getColor(obj.data("value"), opts));
                if (obj.data("value") == 0) {
                    obj.css("border", "solid 1px #ccc");
                } else { 
                    obj.css("border", "solid 1px " + $.fn.getColor(obj.data("value"), opts)); 
                }
            } else {
                obj.addClass("mark-color-all");
            }
            var width = 0;
            obj.click(function() {
                $(".mark-color-list").hide();
                var _this = $(this);
                var position = _this.position();
                if ($("#" + _this.data("itemid")).length == 0) {
                    var _colorBody = $("<ul id='" + _this.data("itemid") + "' class='mark-color-list'></ul>");

                    if (opts.isAll) {
                        var _all = $("<li data-value='-1' title='全部' class='mark-color-item all'><span></span><div >全部</div></li>");
                        _colorBody.append(_all);
                        width += 10;
                    } 
                    for (var i = 0; i < opts.data.length; i++) {
                        var tempguid = Global.guid();
                        var tempcolor = opts.data[i][opts.dataColor];
                        var _color = $("<li id='" + tempguid + "'    data-value='" + opts.data[i][opts.dataValue] + "' class='mark-color-item'><span></span><div >" + opts.data[i][opts.dataText] + "</div></li>");
                        _color.find("span").css("background-color", tempcolor);
                        _colorBody.append(_color);
                        _color.find("li").mouseover( function() {console.log(1)});
                    } 
                    _colorBody.find(".mark-color-item").click(function() {
                        var _changeColor = $(this);
                        //更换才触发 
                        if (_changeColor.data("value") != _this.data("value")) {
                            _this.data("value", _changeColor.data("value"));
                            !!opts.onChange && opts.onChange(_this, function(status) {
                                if (status) {
                                    if (_changeColor.data("value") > 0) {
                                        if (_changeColor.data("value") == 0) {
                                            _this.css("border", "solid 1px #ccc");
                                        } else {
                                            _this.css("border", "solid 1px " +  $.fn.getColor(_changeColor.data("value"), opts));
                                        }
                                        _this.css("background-color",  $.fn.getColor(_changeColor.data("value"), opts));
                                        _this.removeClass("mark-color-all");
                                    } else {
                                        _this.css("border", "none");
                                        _this.addClass("mark-color-all");
                                    }
                                }
                            });
                        }
                        _colorBody.hide();
                    });
                    _colorBody.css({ "top": position.top + 20, "left": position.left - 9 }).show();
                    $("body").append(_colorBody);
                } else {
                    $("#" + _this.data("itemid")).css({ "top": position.top + 20, "left": position.left - 9 }).show();
                }
                return false;
            });

            $(document).click(function(e) {
                if ($(e.target).data("itemid") != obj.data("itemid") && $(e.target).attr("id") != obj.data("itemid")) {
                    $("#" + obj.data("itemid")).hide();
                }
            });

        }
        $.fn.getColor = function(value, opts) {
            var color = "";
            for (var i = 0; i < opts.data.length; i++) {
               
                if (opts.data[i][opts.dataValue] == value) { 
                    color = opts.data[i][opts.dataColor]; 
                    break;
                }
            } 
            return color;
        };
        $.fn.bindmouseover = function (colorvalue, li) { 
            $('#' + li).css("background-color", colorvalue);
        };
        $.fn.bindmouseout = function (colorvalue, li) {
            $('#' + li).css("background-color", "");
        };
    })(jQuery);
    module.exports = jQuery;
}); 