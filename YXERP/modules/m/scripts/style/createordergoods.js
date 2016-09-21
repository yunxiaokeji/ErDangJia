define(function (require,exports,module) {
    var ObjectJS = {};
    var doT = require("dot");
    var City = require("city"), CityInvoice,
        Global = require("m_global");
    var isCreateOrder = false;
    var AttrList = [];
    ObjectJS.showOrderGoodsLayer = function (model, user) {
        ObjectJS.model = model;
        ObjectJS.getOrderAttr();
        doT.exec("m/template/style/style-buy.html", function (code) {
            var innerHtml = code(model);
            innerHtml = $(innerHtml);

            /*编辑发货信息*/
            innerHtml.find("#customerName").val(user.ContactName)
                                            .data('value', user.ContactName);
            innerHtml.find("#customerTel").val(user.MobilePhone)
                                            .data('value', user.MobilePhone);
            innerHtml.find("#customerAddress").val(user.Address)
                                                .data('value', user.Address);
            innerHtml.find("#citySpan").data('value', user.CityCode);

            /*展示发货信息*/
            innerHtml.find("#showCustomerName").text(user.ContactName);
            innerHtml.find("#showCustomerTel").text(user.MobilePhone);
            innerHtml.find("#showCustomerAddress").text(user.Address);

            innerHtml.find(".quantity").change(function () {
                var _this = $(this);
                if (!_this.val().isInt() || _this.val() < 0) {
                    _this.val(0);
                    return false;
                }
                if ($("#colorlist li").hasClass('select')) {
                    var _saleID = $("#colorlist li.select").data('id');
                    ObjectJS.setOrderAttrQuantity(_saleID, _this.parents('tr').data('id'), _this.val() || 0);
                }
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
                var num = 0;
                $("#sizelist .data-item").each(function () {
                    var _sizeTr = $(this);
                    //第一次，没有选中任何颜色处理
                    if (!$(".attr-item li").hasClass('select')) {
                        ObjectJS.setOrderAttrQuantity(_this.data('id'), _sizeTr.data('id'), _sizeTr.find('.quantity').val() || 0, 1);
                    } else {
                        num += _sizeTr.find('.quantity').val() * 1;
                    }
                });
                if (num > 0) {
                    //$()
                }

                ObjectJS.setTrQuantity(_this.data('id'));
                _this.siblings().removeClass("select");
                _this.removeClass("hasquantity").addClass("select");
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

    ObjectJS.getOrderAttr = function () {
        var _self = ObjectJS;
        for (var i = 0; i < _self.model.SaleAttrs[0].AttrValues.length; i++) {
            var _sale = _self.model.SaleAttrs[0].AttrValues[i];
            var _model = {};
            _model.SaleRemark = _sale.ValueName;
            var _details = {};
            for (var j = 0; j < _self.model.AttrLists[0].AttrValues.length; j++) {
                var _attr = _self.model.AttrLists[0].AttrValues[j];
                _details[_attr.ValueID] = {
                    ValueName: _attr.ValueName,
                    Quantity: 0,
                    ValueID: _attr.ValueID
                };
            }
            _model.AttrsList = _details;
            AttrList[_sale.ValueID] = _model;
        }
    };

    ObjectJS.setOrderAttrQuantity = function (saleID, attrID, quantity) {
        AttrList[saleID].AttrsList[attrID].Quantity = quantity;
        /*设置总计*/
        var totalCount = 0;
        var totalPrice = 0;
        for (var i in AttrList) {
            var _sale = AttrList[i].AttrsList;
            for (var j in _sale) {
                var _attr = _sale[j];
                totalCount += _attr.Quantity * 1;
                totalPrice += _attr.Quantity * ($(".data-item").eq(0).find('.price').text() * 1);
            }
        }
        $("#totalnum").parent().show();
        $("#totalnum").text(totalCount);
        $("#totalprice").text(totalPrice);
    };

    ObjectJS.setTrQuantity = function (saleID) {
        var item = AttrList[saleID].AttrsList;
        $("#sizelist .data-item").each(function () {
            var _this = $(this);
            var _data = item[_this.data('id')];
            _this.find('.quantity').val(_data.Quantity);
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

    module.exports = ObjectJS;
});