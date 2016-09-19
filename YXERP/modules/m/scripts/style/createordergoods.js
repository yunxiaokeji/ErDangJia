define(function (require,exports,module) {
    var ObjectJS = {};
    var doT = require("dot");
    var City = require("city"), CityInvoice,
        Global = require("m_global");
    var isCreateOrder = false;
    var colorList = {}, tempOrder = {}, tempList = [];

    ObjectJS.showOrderGoodsLayer = function (model, user) {
        colorList = {}, tempOrder = {}, tempList = [];
        ObjectJS.model = model;
        ObjectJS.getOrderAttr();
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

            innerHtml.find(".quantity").change(function () {
                var _this = $(this);
                if (!_this.val().isInt() || _this.val() < 0) {
                    _this.val(0);
                }
                ObjectJS.SumNumPrice();
            });

            $(".overlay-addOrder").html(innerHtml).show();

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

            $(".overlay-addOrder .attr-ul li").unbind().click(function () {
                var _this = $(this);
                //数量验证
                var totalnum = 0;
                $("#sizelist .quantity").each(function () {
                    totalnum += parseInt($(this).val());
                });
                if (totalnum > 0) {
                    $('#colorlist li.select').addClass("hasquantity");
                }
                if ($('#colorlist li.select').length > 0) {
                    ObjectJS.setOrdersCache();
                    $("#sizelist .quantity").each(function () {
                        $(this).val(0);
                    });
                }
                _this.siblings().removeClass("select");
                _this.removeClass("hasquantity").addClass("select");
                $('.data-item').each(function () {
                    if (colorList[_this.data("remark")].indexOf($(this).data("remark")) == -1) {
                        $(this).hide();
                        $(this).find('.quantity').val(0);
                    } else {
                        $(this).show();
                    }
                });
                ObjectJS.SumNumPrice();
                ObjectJS.setOrdersQuantity();
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
                        productid: model.ProductID,
                        dids: "",
                        cmclientid: model.ClientID
                    };

                    ObjectJS.createOrders(item);
                } else {
                    alert('请先保存收货信息', 2);
                }
                return false;
            });

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

            CityInvoice = City.createCity({
                cityCode: user.CityCode,
                elementID: "citySpan"
            });
        });
    };

    ObjectJS.createOrders = function (model) {
        var _self = this;
        _self.setOrdersCache();
        if ($.isEmptyObject(tempOrder)) {
            alert("请选择颜色尺码，并填写对应采购数量");
            return false;
        }
        if (!isCreateOrder) {
            //大货单遍历下单明细 
            var dids = "";
            $.each(tempOrder, function (i, obj) {
                dids += obj.detailid + ":" + obj.quantity + ",";
            });
            model.dids = dids;
            $(".btn-sureAdd").text("下单中...");
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

    //绑定颜色尺码
    ObjectJS.getOrderAttr = function () {
        var _self = ObjectJS;
        for (var i = 0; i < _self.model.ProductDetails.length; i++) {
            var item = _self.model.ProductDetails[i];
            if (item.AttrValue != "" && item.AttrValue != null) {
                var color = item.AttrValue.split(",");
                var key = "[" + color[0] + "]" + (color.length > 1 ? "[" + color[1] + "]" : "");
                colorList[key] = item.ProductDetailID;
                if ($.inArray(color[0], tempList) == -1) {
                    colorList[color[0]] = color.length > 1 ? color[1] : color[0];
                    tempList.push(color[0]);
                } else {
                    colorList[color[0]] = colorList[color[0]] + (color.length > 1 ? "," + color[1] : "");
                }
            }
        }
    };
    //下单数量与件数
    ObjectJS.SumNumPrice = function () {
        var _self = this;
        var sumnum = 0, sumprice = 0.00;
        if (!$.isEmptyObject(tempOrder)) {
            $.each(tempOrder, function (i, obj) {
                sumnum += parseInt(obj.quantity);
            });
        }
        $('#sizelist .quantity').each(function () {
            $(this).val($(this).val().replace(/\D/g, ''));
            var num = $(this).val();
            if (num != '') {
                sumnum += parseInt(num);
            }
        });
        sumprice = (sumnum * parseFloat(_self.model.Price)).toFixed(2);
        $('#totalnum').html(sumnum);
        $('#totalprice').html(sumprice);
        $('#totalnum').parent().show();
    };
    //数量缓存
    ObjectJS.setOrdersCache = function () {
        $('#sizelist .quantity').each(function () {
            var _this = $(this);
            var size = _this.parents('tr').data("remark");
            var color = '';
            if ($('#colorlist li').length > 0) {
                if ($('#colorlist li.select').length > 0) {
                    color = $('#colorlist li.select').data("remark");
                } else {
                    return false;
                }
            }
            var key = (color != "" ? "[" + color + "]" : "") + "[" + size + "]";
            var item = { quantity: _this.val(), detailid: colorList[key], key: key }
            if (_this.val() > 0) {
                tempOrder[key] = item;
            } else {
                if (typeof (tempOrder[key]) != 'undefined' && typeof (colorList[key]) != 'undefined') {
                    delete tempOrder[key];
                }
            }
        });
    };
    //数赋值
    ObjectJS.setOrdersQuantity = function () {
        $('#sizelist .quantity').each(function () {
            var _this = $(this);
            var size = _this.parents('tr').data("remark");
            var color = '';
            if ($('#colorlist li.select').length > 0) {
                var color = $('#colorlist li.select').data("remark");
            }
            var key = (color != "" ? "[" + color + "]" : "") + "[" + size + "]";
            if (typeof (tempOrder[key]) != 'undefined' && typeof (colorList[key]) != 'undefined') {
                _this.val(tempOrder[key].quantity);
            }
        });
    };

    module.exports = ObjectJS;
});