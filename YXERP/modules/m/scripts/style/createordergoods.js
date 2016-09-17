define(function (require,exports,module) {
    var ObjectJS = {};
    var doT = require("dot");
    var City = require("city"), CityInvoice,
        Global = require("m_global");
    var details = [];
    var isCreateOrder = false;
    ObjectJS.showOrderGoodsLayer = function (model, user) {
        details = model.ProductDetails;
        doT.exec("m/template/style/style-buy.html", function (code) {
            var innerHtml = code(model);
            innerHtml = $(innerHtml);

            /*编辑发货信息*/
            innerHtml.find("#customerName").val(user.Name)
                                            .data('value', user.Name);
            innerHtml.find("#customerTel").val(user.MobilePhone)
                                            .data('value', user.MobilePhone);
            innerHtml.find("#customerAddress").val(user.Address)
                                                .data('value', user.Address);
            innerHtml.find("#citySpan").data('value', user.CityCode);

            /*展示发货信息*/
            innerHtml.find("#showCustomerName").text(user.Name);
            innerHtml.find("#showCustomerTel").text(user.MobilePhone);
            innerHtml.find("#showCustomerAddress").text(user.Address);

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
                        $(".overlay-addOrder").fadeOut();
                    });
                }
            });

            $(".overlay-addOrder .close").unbind().click(function () {
                $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                    $("body,html").removeClass('ohidden');
                    $(".overlay-addOrder").fadeOut();;
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
                if ($('.edit-customer').css('display') == 'none') {
                    var item = {
                        personname: $("#customerName").val(),
                        mobiletele: $("#customerTel").val(),
                        citycode: CityInvoice.getCityCode(),
                        address: $("#customerAddress").val(),
                        goodsid: model.CMGoodsID,
                        goodsname: model.ProductName,
                        goodscode: model.CMGoodsCode,
                        parentprid: model.ClientID,
                        price: model.Price,
                        productid:model.ProductID,
                        entity: []
                    };

                    ObjectJS.createOrders(item);
                } else {
                    alert('请先保存收货信息', 2);
                }
                return false;
            });

            /*自定义规格*/
            //innerHtml.find('.layout-attr').change(function () {
            //    var _this = $(this);
            //    var isContinue = true;
            //    var _thisTitle = _this.parents('.productsalesattr').find('.attr-title');
            //    if (!_this.val().trim()) {
            //        _this.val('');
            //        return false;
            //    }
            //    _this.parent().siblings().each(function () {
            //        var obj = $(this);
            //        if (_this.val().trim() == obj.text().trim()) {
            //            alert(obj.data('type') == 1 ? "" + _thisTitle.text() + "已存在" : "" + _thisTitle.text() + "已存在", 2);
            //            isContinue = false;
            //            return false;
            //        }
            //    });
            //    if (isContinue) {
            //        var attrObj = $("<li class='" + (_thisTitle.data('type') == 1 ? "size" : "color") + "' data-type='" + _thisTitle.data('type') + "' data-id='|' data-value='" + _this.val().trim() + "'>" + _this.val().trim() + "</li>");
            //        attrObj.click(function () {
            //            var _self = $(this);
            //            if (_self.hasClass("select")) {
            //                _self.removeClass("select");
            //            } else {
            //                _self.addClass("select");
            //            }
            //            ObjectJS.createOrderGoods();
            //        });
            //        _this.parent().before(attrObj);
            //        attrObj.click();
            //        _this.val('');
            //    }
            //});

            innerHtml.find('.customer-base-info .ico-edit').click(function () {
                var _this = $(this);
                $(".customer-base-info").hide();
                $(".edit-customer").show();
            });

            innerHtml.find('.save-customer,.cancel-customer').click(function () {
                var _this = $(this);
                $(".customer-base-info").show();
                $(".edit-customer").hide();
                if (_this.data('type') == 'save') {
                    /*保存*/
                    $("#customerName").data('value', $("#customerName").val());
                    $("#customerTel").data('value', $("#customerTel").val());
                    $("#customerAddress").data('value', $("#customerAddress").val());
                    $("#citySpan").data('value', CityInvoice.getCityCode());
                } else {
                    /*取消*/
                    $("#customerName").val($("#customerName").data('value'));
                    $("#customerTel").val($("#customerTel").data('value'))
                    $("#customerAddress").val($("#customerAddress").data('value'))
                    $("#citySpan").html('').data($("#citySpan").data('value'));
                    CityInvoice = City.createCity({
                        cityCode: $("#citySpan").data('value'),
                        elementID: "citySpan"
                    });
                }
                /*更改发货信息显示*/
                $("#showCustomerName").text($("#customerName").val());
                $("#showCustomerTel").text($("#customerTel").val());
                $("#showCustomerAddress").text($("#customerAddress").val());
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
        $(".attr-box .table-list").append('<tr class="tr-header" ><td class="tLeft">规格</td><td>数量</td><td class="tRight">操作</td></tr>');
        $(".attr-ul .size.select").each(function () {
            var _this = $(this);
            $(".attr-ul .color.select").each(function () {
                var description = '【' + _this.parents('.productsalesattr').find('.attr-title').text() + ':' + _this.data('value') + '】【' + $(this).parents('.productsalesattr').find('.attr-title').text() + ':' + $(this).data('value') + '】';
                var isContinue = false;
                for (var i = 0; i < details.length; i++) {
                    var _detail = details[i].AttrValue;
                    if (_detail == (_this.data('value') + ($(this).data('value') && ',' + $(this).data('value')))) {
                        isContinue = true;
                    }
                }
                if (isContinue) {
                    var trHtml = $("<tr class='detail-attr' data-remark='" + description + "'></tr>");
                    trHtml.append("<td class='tLeft'>" + description + "</td>");
                    trHtml.append("<td class='center'><input style='width:50px;height:20px;padding:3px; 0' maxlength='9' class='quantity center' type='tel' value='' /></td>");
                    trHtml.append("<td class='iconfont center red tRight' style='font-size:14px;padding-right:10px;'>&#xe606;</td>");

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
                }
            });
        });
    };

    ObjectJS.createOrders = function (model) {
        var _self = this;
        if (!isCreateOrder) {
            var totalnum = 0;
            $(".attr-box .table-list .quantity").each(function () {
                totalnum += parseInt(!$(this).val() ? 0 : $(this).val());
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
                    model.entity.push({
                        Quantity: _this.val(),
                        Remark: _thisTr.data('remark')
                    });
                }
            });

            $(".btn-sureAdd").text("下单中...");
            model.entity = JSON.stringify(model.entity);
            isCreateOrder = true;
            Global.post("/Mall/Store/CreatePurchaseOrder", model, function (data) {
                isCreateOrder = false;
                $(".btn-sureAdd").text("确定");
                if (data.result == 1) {
                    alert("下单成功");
                    $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                        $("body,html").removeClass('ohidden');
                        $(".overlay-addOrder").fadeOut();;
                    });
                } else {
                    alert(data.errMsg);
                    return false;
                }
            });
        }
    };
    module.exports = ObjectJS;
});