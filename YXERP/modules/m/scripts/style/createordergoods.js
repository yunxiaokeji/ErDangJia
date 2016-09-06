define(function (require,exports,module) {
    var ObjectJS = {};
    var doT = require("dot");
    var City = require("city"), CityInvoice,
        Global = require("m_global");
    var isCreateOrder = false;
    ObjectJS.showOrderGoodsLayer = function (modle,user) {
        doT.exec("m/template/style/style-buy.html", function (code) {
            var innerHtml = code(modle);
            innerHtml = $(innerHtml);

            innerHtml.find("#customerName").val(user.Name);
            innerHtml.find("#customerTel").val(user.MobilePhone);
            innerHtml.find("#customerAddress").val(user.Address);

            $(".overlay-addOrder").html(innerHtml).show();

            $('.productsalesattr').each(function () {
                var _this = $(this);
                if (_this.find('.attr-item').height() <= 68) {
                    _this.find('.show-more').remove();
                } else {
                    _this.find('.attr-item').addClass('show-attr-box');
                }
            });

            $(".overlay-addOrder .style-content").animate({ height: "450px" }, 200);

            $(".overlay-addOrder").unbind().click(function (e) {
                if (!$(e.target).parents().hasClass("style-content") && !$(e.target).hasClass("style-content")) {
                    $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                        $("body,html").removeClass('ohidden');
                        $(".overlay-addOrder").hide();
                    });
                }
            });

            $(".overlay-addOrder .close").unbind().click(function () {
                $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                    $("body,html").removeClass('ohidden');
                    $(".overlay-addOrder").hide();
                });
            });

            $(".overlay-addOrder .attr-ul li.size,.overlay-addOrder .attr-ul li.color").unbind().click(function () {
                var _self = $(this);
                if (_self.hasClass("select")) {
                    _self.removeClass("select");
                } else {
                    _self.addClass("select");
                }
                ObjectJS.createOrderGoods();
            });

            $(".overlay-addOrder .btn-sureAdd").unbind().click(function () {
                var model = {
                    OrderID: modle.orderID,
                    PersonName: $("#customerName").val(),
                    MobileTele: $("#customerTel").val(),
                    CityCode: CityInvoice.getCityCode(),
                    Address: $("#customerAddress").val(),
                    GoodsName: modle.goodsName,
                    GoodsID: modle.goodsID,
                    ClientID: modle.clientID,
                    IntGoodsCode: modle.intGoodsCode,
                    CategoryID: modle.categoryID,
                    FinalPrice: modle.finalPrice,
                    OrderImage: modle.orderImage,
                    Details: []
                };
                ObjectJS.createOrders(model);
                return false;
            });

            innerHtml.find('.layout-attr').change(function () {
                var _this = $(this);
                var isContinue = true;
                var _thisTitle = _this.parents('.productsalesattr').find('.attr-title');
                if (!_this.val().trim()) {
                    _this.val('');
                    return false;
                }
                _this.parent().siblings().each(function () {
                    var obj = $(this);
                    if (_this.val().trim() == obj.text().trim()) {
                        alert(obj.data('type') == 1 ? "" + _thisTitle.text() + "已存在" : "" + _thisTitle.text() + "已存在", 2);
                        isContinue = false;
                        return false;
                    }
                });
                if (isContinue) {
                    var attrObj = $("<li class='" + (_thisTitle.data('type') == 1 ? "size" : "color") + "' data-type='" + _thisTitle.data('type') + "' data-id='|' data-value='" + _this.val().trim() + "'>" + _this.val().trim() + "</li>");
                    attrObj.click(function () {
                        var _self = $(this);
                        if (_self.hasClass("select")) {
                            _self.removeClass("select");
                        } else {
                            _self.addClass("select");
                        }
                        ObjectJS.createOrderGoods();
                    });
                    _this.parent().before(attrObj);
                    attrObj.click();
                    _this.val('');
                }
            });

            innerHtml.find(".show-more").click(function () {
                var _this = $(this);
                if (_this.data('isget') != 1) {
                    _this.parent().find('.attr-item').removeClass('show-attr-box');
                    _this.data('isget', 1);
                    _this.text("收起");
                } else {
                    _this.parent().find('.attr-item').addClass('show-attr-box');
                    _this.data('isget', 2);
                    _this.text("更多+");
                }
            });

            CityInvoice = City.createCity({
                cityCode: user.CityCode,
                elementID: "citySpan"
            });
        });
    };

    ObjectJS.createOrderGoods = function () {
        $(".attr-box").empty();
        $(".attr-box").append('<table class="table-list"></table>');
        $(".attr-box .table-list").append('<tr class="tr-header" ><td class="tLeft">规格</td><td>数量</td><td>操作</td></tr>');
        $(".attr-ul .size.select").each(function () {
            var _this = $(this);
            $(".attr-ul .color.select").each(function () {
                var dataAttr = $("#attrBox").data('id') + ',' + $("#colorBox").data('id');
                var dataValue = _this.data('id') + ',' + $(this).data('id');
                var dataAttrValue = $("#attrBox").data('id') + ":" + _this.data('id') + "," + $("#colorBox").data('id') + ":" + $(this).data('id');
                var description = '【尺码:' + _this.data('value') + '】【颜色:' + $(this).data('value') + '】';

                var trHtml = $("<tr class='detail-attr' data-attr='" + dataAttr + "' data-value='" + dataValue + "' data-attrandvalue='" + dataAttrValue + "' data-xremark='【" + _this.data('value') + "】' data-yremark='【" + $(this).data('value') + "】' data-xyremark='【" + _this.data('value') + "】【" + $(this).data('value') + "】' data-remark='" + description + "'></tr>");
                trHtml.append("<td class='tLeft'>" + description + "</td>");
                trHtml.append("<td class='center'><input style='width:50px;height:20px;padding:3px; 0' class='quantity center' type='text' value='0' /></td>");
                trHtml.append("<td class='iconfont center' style='font-size:30px;color:#4a98e7;'>&#xe651;</td>");

                trHtml.find('.iconfont').click(function () {
                    $(this).parents('tr').remove();
                    return false;
                });
                trHtml.find('.quantity').change(function () {
                    var _this = $(this);
                    if (!_this.val().isInt() || _this.val() < 0) {
                        _this.val(0);
                    }
                });
                $(".attr-box .table-list").append(trHtml);
            });
        });
    };

    ObjectJS.createOrders = function (model) {
        var _self = this;
        if (!isCreateOrder) {
            var totalnum = 0;
            $(".attr-box .table-list .quantity").each(function () {
                totalnum += parseInt($(this).val());
            });
            if (totalnum == 0) {
                alert("请选择颜色尺码，并填写对应采购数量", 2);
                return false;
            }
            //大货单遍历下单明细 
            $(".attr-box .table-list .quantity").each(function () {
                var _this = $(this);
                var _thisTr = _this.parents('tr');
                if (_this.val() > 0) {
                    model.Details.push({
                        SaleAttr: _thisTr.data('attr'),
                        AttrValue: _thisTr.data('value'),
                        SaleAttrValue: _thisTr.data('attrandvalue'),
                        Quantity: _this.val(),
                        XRemark: _thisTr.data('xremark'),
                        YRemark: _thisTr.data('yremark'),
                        XYRemark: _thisTr.data('xyremark'),
                        Remark: _thisTr.data('remark')
                    });
                }
            });
            $(".btn-sureAdd").text("下单中...");
            isCreateOrder = true;
            Global.post("/IntFactoryOrder/CreateOrderEDJ", { entity: JSON.stringify(model) }, function (data) {
                isCreateOrder = false;
                $(".btn-sureAdd").text("确定");
                if (data.result) {
                    alert("下单成功");
                    $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                        $("body,html").removeClass('ohidden');
                        $(".overlay-addOrder").hide();
                    });
                } else {
                    alert(data.error_message, 2);
                }
            });
        }
    };
    module.exports = ObjectJS;
});