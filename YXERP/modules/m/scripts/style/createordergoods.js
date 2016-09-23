define(function (require,exports,module) {
    var ObjectJS = {};
    var doT = require("dot");
    var City = require("city"), CityInvoice,
        Global = require("m_global");

    var isCreateOrder = false;
    var AttrList = [];
    ObjectJS.showOrderGoodsLayer = function (model, currentClient) {
        if (model.AttrLists.length == 0 || model.SaleAttrs.length == 0) {
            alert("该产品暂不支持下单", 2);
        }
        ObjectJS.model = model;
        model.currentClient = currentClient;
        ObjectJS.setOrderAttr();

        doT.exec("m/template/style/style-buy.html", function (code) {
            var innerHtml = code(model);
            innerHtml = $(innerHtml);

            /*编辑发货信息*/
            //innerHtml.find("#customerName").val(currentClient.ContactName)
            //                                .data('value', currentClient.ContactName);
            //innerHtml.find("#customerTel").val(currentClient.MobilePhone)
            //                                .data('value', currentClient.MobilePhone);
            //innerHtml.find("#customerAddress").val(currentClient.Address)
            //                                    .data('value', currentClient.Address);
            //innerHtml.find("#citySpan").data('value', currentClient.CityCode);
            ///*展示发货信息*/
            //innerHtml.find("#showCustomerName").text(currentClient.ContactName);
            //innerHtml.find("#showCustomerTel").text(currentClient.MobilePhone);
            //innerHtml.find("#showCustomerAddress").text(currentClient.Address);

            innerHtml.find(".quantity").bind({
                change:function(){
                    var _this = $(this);
                    if ($("#colorlist li").hasClass('select')) {
                        var _saleID = $("#colorlist li.select").data('id');
                        ObjectJS.setOrderAttrQuantity(_saleID, _this.parents('tr').data('id'), _this.val() || 0);
                    }
                    if (!_this.val().isInt() || _this.val() < 0) {
                        _this.val('');
                        return false;
                    }
                },
                focus: function () {
                    $(".btn-sureAdd").addClass('unable');
                    $(".overlay-addOrder .btn-sureAdd").unbind();
                },
                blur: function () {
                    $(".btn-sureAdd").removeClass('unable');
                    setTimeout(function () {
                        $(".overlay-addOrder .btn-sureAdd").unbind().click(function () {
                            createOrder();
                        });
                    }, 10);
                }
            });

            $(".overlay-addOrder").html(innerHtml).show();

            $(".overlay-addOrder .style-content").animate({ height: ($(window).height() > 460 ? 450 : 400) + "px" }, 200);

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
                    $("#colorlist li.select").addClass('hasquantity');
                }

                ObjectJS.setTrQuantity(_this.data('id'));
                _this.siblings().removeClass("select");
                _this.removeClass("hasquantity").addClass("select");
            });

            $(".overlay-addOrder .btn-sureAdd").unbind().click(function () {
                createOrder();
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

            function createOrder() {
                if ($('.edit-customer').css('display') == 'none') {
                    var item = {
                        personName: $("#customerName").val(),
                        mobilePhone: $("#customerTel").val(),
                        cityCode: CityInvoice.getCityCode(),
                        address: $("#customerAddress").val(),
                        goodsID: model.goodsID,
                        goodsName: model.goodsName,
                        goodsCode: model.intGoodsCode,
                        price: $("#productPrice").text(),
                        productDetails: "",
                        cmClientID: $("#EDJProvider").data('id'),
                        totalMoney: $("#totalprice").text() * 1,
                        saleAttrStr: model.SaleAttrs[0].AttrName + ',' + model.AttrLists[0].AttrName,
                        productImage: model.orderImage,
                        zngcOrderID: model.orderID,
                        zngcClientID: model.clientID,
                        zngcProductEntity: ""
                    };
                    if ($("#pirceRangeBox").length > 0 && $("#minOrderNum").length == 1) {
                        if ($("#totalnum").text() * 1 < $("#minOrderNum").text() * 1) {
                            alert("下单数最少" + $("#minOrderNum").text() + "件", 2);
                            return false;
                        }
                    }
                    ObjectJS.createOrders(item);
                } else {
                    alert('请先保存收货信息', 2);
                }
            };

            CityInvoice = City.createCity({
                cityCode: currentClient.CityCode,
                elementID: "citySpan"
            });
        });
    };

    ObjectJS.setOrderAttr = function () {
        var _self = ObjectJS;
        var SaleAttrs = _self.model.SaleAttrs[0];
        if (SaleAttrs) {
            for (var i = 0; i < SaleAttrs.AttrValues.length; i++) {
                var _sale = SaleAttrs.AttrValues[i];
                var _model = {};
                _model.SaleRemark = _sale.ValueName;
                _model.SaleID = _sale.ValueID;

                var AttrLists = _self.model.AttrLists[0];
                var _details = {};
                for (var j = 0; j < AttrLists.AttrValues.length; j++) {
                    var _attr = AttrLists.AttrValues[j];
                    _details[_attr.ValueID] = {
                        ValueName: _attr.ValueName,
                        Quantity: 0,
                        ValueID: _attr.ValueID
                    };
                }
                _model.AttrsList = _details;

                AttrList[_sale.ValueID] = _model;
            }
        }
    }

    ObjectJS.setOrderAttrQuantity = function (saleID, attrID, quantity) {
        AttrList[saleID].AttrsList[attrID].Quantity = quantity;
        /*设置总计*/
        var totalCount = 0;
        for (var i in AttrList) {
            var _item = AttrList[i];
            var _sale = _item.AttrsList;
            var _thisAttrCount = 0;
            for (var j in _sale) {
                var _attr = _sale[j];
                totalCount += _attr.Quantity * 1;
                _thisAttrCount += _attr.Quantity * 1;
            }
            var obj = $("#colorlist li[data-id='" + _item.SaleID + "']").find('.quantity-lump');
            obj.removeClass('quantity-more');
            if (_thisAttrCount > 99) {
                _thisAttrCount = '99+';
                obj.addClass('quantity-more');
            }
            obj.text(_thisAttrCount);
        }
        $("#totalnum").parent().show();
        $("#totalnum").text(totalCount);

        $("#pirceRangeBox .range").removeClass('hover');
        $("#productPrice").text($("#productPrice").data('price'));
        $("#productPrice").prev().show();
        var k = $("#pirceRangeBox .range").length*1 - 1;
        while (k >= 0) {
            var obj = $("#pirceRangeBox .range").eq(k);
            if (totalCount * 1 >= obj.find('.quantity').text() * 1) {
                obj.addClass('hover');
                $("#productPrice").text(obj.find('.price').text());
                $("#productPrice").prev().hide();
                $("#totalprice").text(obj.find('.price').text() * 1 * $("#totalnum").text() * 1);
                break;
            }
            k--;
        }
        $("#totalprice").text(totalCount * $("#productPrice").text() * 1);
    };

    ObjectJS.setTrQuantity = function (saleID) {
        var item = AttrList[saleID].AttrsList;
        $("#sizelist .data-item").each(function () {
            var _this = $(this);
            var _data = item[_this.data('id')];
            _this.find('.quantity').val(_data.Quantity || '');
        });
    };

    ObjectJS.createOrders = function (model) {
        var _self = this;
        if (!isCreateOrder) {
            //大货单遍历下单明细 
            var productDetails = "";
            var zngcProductEntity = [];
            for (var i in AttrList) {
                var _sale = AttrList[i];
                for (var j in _sale.AttrsList) {
                    var _attrs = _sale.AttrsList[j];
                    if (_attrs.Quantity > 0) {
                        productDetails += _sale.SaleRemark + ',' + _attrs.ValueName + '|';
                        productDetails += $("#colorlist").prev().text().trim() + ':' + _sale.SaleRemark + ',' + $("#sizelist .tr-header .attr-title").text().trim() + ':' + _attrs.ValueName + '|';
                        productDetails += _attrs.Quantity + '|';
                        productDetails += $("#productPrice").text() + '|';
                        productDetails += '[' + $("#colorlist").prev().text().trim() + '：' + _sale.SaleRemark + '][' + $("#sizelist .tr-header .attr-title").text().trim() + '：' + _attrs.ValueName + ']&';
                        
                        var zngcModel = {};
                        zngcModel.price = $("#productPrice").text();
                        zngcModel.productCode = _self.model.intGoodsCode;
                        zngcModel.productName = _self.model.goodsName;
                        zngcModel.productImage = _self.model.orderImage;
                        zngcModel.quantity = _attrs.Quantity;
                        zngcModel.saleAttr = $("#sizelist .tr-header .attr-title").data('id') + ',' + $("#colorlist").prev().data('id');
                        zngcModel.attrValue = _attrs.ValueID + ',' + _sale.SaleID;
                        zngcModel.saleAttrValue = $("#sizelist .tr-header .attr-title").data('id') + ':' + _attrs.ValueID + ',' + $("#colorlist").prev().data('id') + ':' + _sale.SaleID;
                        zngcModel.remark = '【' + $("#sizelist .tr-header .attr-title").text().trim() + '：' + _attrs.ValueName + '】【' + $("#colorlist").prev().text().trim() + '：' + _sale.SaleRemark + '】';
                        zngcModel.xRemark = '【' + _attrs.ValueName + '】';
                        zngcModel.yRemark = '【' + _sale.SaleRemark + '】';
                        zngcModel.xYRemark = '【' + _attrs.ValueName + '】【' + _sale.SaleRemark + '】';
                        zngcModel.description = "";
                        zngcProductEntity.push(zngcModel);
                    }
                }
            }
            model.productDetails = productDetails && productDetails.substring(0, productDetails.length - 1);
            model.zngcProductEntity = JSON.stringify(zngcProductEntity);
            if (!model.productDetails) {
                alert("请填写下单数量",2);
                return false;
            }
            $(".btn-sureAdd").text("下单中...");
            isCreateOrder = true;
            Global.post("/M/IntFactoryOrders/AddIntfactoryPurchaseDoc", model, function (data) {
                isCreateOrder = false;
                $(".btn-sureAdd").text("下单");
                if (data.result) {
                    if (data.result.id) {
                        alert("下单成功");
                        $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                            $("body,html").removeClass('ohidden');
                            $(".overlay-addOrder").fadeOut();;
                        });
                    }
                } else {
                    alert("下单失败,请重试", 2);
                    return false;
                }
            });
        }
    };

    module.exports = ObjectJS;
});