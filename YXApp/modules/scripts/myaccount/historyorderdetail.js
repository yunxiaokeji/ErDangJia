define(function (require, exports, modules) {
    require("jquery");
    var Global = require("global"),
        doT = require("dot");
    var ObjectJS = {};
    ObjectJS.init = function () {
        ObjectJS.bindEvent();
    };

    ObjectJS.bindEvent = function () {
        $(".add-shopping").click(function () {
            var _this=$(this);
            var details = [];

            $("#attr-list .detail-attr").each(function () {
                var _this = $(this);
                var quantity = _this.find('.quantity').val();
                if (quantity > 0) {
                    var detail = {
                        price: $('.add-shopping').data('price'),
                        Quantity: quantity,
                        SaleAttr: _this.data('attr'),
                        AttrValue: _this.data('value'),
                        SaleAttrValue: _this.data('name'),
                        Description: _this.data('attrvalue')
                    };
                    details.push(detail);
                }
            });
            if (details.length > 0) {
                Global.post("/MyAccount/CreateOrder", {
                    zngcOrderID: _this.data('orderid'),
                    price: _this.data('price'),
                    detailsEntity: JSON.stringify(details),
                    zngcClientID: _this.data('clientid')
                }, function () {
                    alert("下单成功");
                });
            } else {
                alert("请输入下单数量");
            }
        });

        $(".attr-item").click(function () {
            if (!$(this).hasClass('hover')) {
                $(this).addClass('hover');
            } else {
                $(this).removeClass('hover');
            }
            $("#attr-list .tr-header").nextAll().remove();
            $(".attr-item.color.hover").each(function () {
                var _this = $(this);
                $(".attr-item.size.hover").each(function () {
                    var dataAttr = $("#color-box").data('id') + ',' + $("#attr-box").data('id');
                    var dataValue = _this.data('id') + ',' + $(this).data('id');
                    var dataAttrValue = $("#color-box").data('id') + ":" + _this.data('id') + "," + $("#attr-box").data('id') + $(this).data('id');
                    var description = '[颜色:' + _this.data('value') + '] [尺码:' + $(this).data('value') + ']';

                    var trHtml = $("<tr class='detail-attr' data-attr='" + dataAttr + "' data-value='" + dataValue + "' data-name='" + dataAttrValue + "' data-attrvalue='" + description + "'></tr>");
                    trHtml.append("<td class='center'>[颜色:" + _this.data('value') + "] [规格:" + $(this).data('value') + "]</td>");
                    trHtml.append("<td class='center'><input class='quantity' type='text' value='1' /></td>");
                    trHtml.append("<td class='iconfont center' style='font-size:30px;color:#4a98e7;'>&#xe651;</td>");

                    trHtml.find('.iconfont').click(function () {
                        $(this).parents('tr').remove();
                    });
                    $("#attr-list .tr-header").after(trHtml);
                });
            });
        });

        $(".menu-module").click(function () {
            var _this = $(this);
            if (!_this.hasClass('hover')) {
                $(".menu-module").removeClass('hover');
                _this.addClass('hover');
                $('.content-main div[data-module=' + _this.attr('id') + ']').show().siblings().hide();
            }
        });

        $(".define-color").blur(function () {
            var _this=$(this);
            if (_this.val().trim() != "") {
                var isContinue = 1;
                $("#color-box .attr-item").each(function () {
                    if (_this.val() == $(this).text().trim()) {
                        isContinue = 0;
                        alert("该颜色已存在,请更换其他颜色");
                        return;
                    }
                });
                if (isContinue == 1) {
                    var colorAttr = $("<div class='left attr-item color' data-value='" + _this.val() + "'>" + _this.val() + "</div>");
                    colorAttr.click(function () {
                        if (!$(this).hasClass('hover')) {
                            $(this).addClass('hover');
                        } else {
                            $(this).removeClass('hover');
                        }
                        $("#attr-list .tr-header").nextAll().remove();
                        $(".attr-item.color.hover").each(function () {
                            var _thisColor = $(this);
                            $(".attr-item.size.hover").each(function () {
                                var dataAttr = $("#color-box").data('id') + ',' + $("#attr-box").data('id');
                                var dataValue = _thisColor.data('id') + ',' + $(this).data('id');
                                var dataAttrValue = $("#color-box").data('id') + ":" + _thisColor.data('id') + "," + $("#attr-box").data('id') + $(this).data('id');
                                var description = '[颜色:' + _thisColor.data('value') + '] [尺码:' + $(this).data('value') + ']';

                                var trHtml = $("<tr class='detail-attr' data-attr='" + dataAttr + "' data-value='" + dataValue + "' data-name='" + dataAttrValue + "' data-attrvalue='" + description + "'></tr>");
                                trHtml.append("<td class='center'>[颜色:" + _thisColor.data('value') + "] [规格:" + $(this).data('value') + "]</td>");
                                trHtml.append("<td class='center'><input class='quantity' type='text' value='1' /></td>");
                                trHtml.append("<td class='iconfont center' style='font-size:30px;color:#4a98e7;'>&#xe651;</td>");

                                trHtml.find('.iconfont').click(function () {
                                    $(this).parents('tr').remove();
                                });
                                $("#attr-list .tr-header").after(trHtml);
                            });
                        });
                    });
                    _this.before(colorAttr);
                    colorAttr.click();
                    alert("添加成功");
                }
            }
            _this.val("");
        });
    };

    modules.exports = ObjectJS;
})