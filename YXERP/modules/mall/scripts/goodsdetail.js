define(function (require, exports, module) {
    var City = require("city"), CityInvoice,
        Verify = require("verify"), VerifyInvoice,
        Global = require("global"),
        Easydialog = require("easydialog");
    require("smartzoom");
    require("pager"); 

    var ObjectJS = {};

    ObjectJS.init = function (clientid,  model,citycode) {
        var _self = this;
        _self.clientid = clientid;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"')); 
        _self.bindStyle(_self.model);
        _self.CityCode = citycode;
        _self.getOrderAttr(); 
    }

    ObjectJS.bindStyle = function (model) {
        var _self = this;  
        //样图
        _self.bindOrderImages(model.ProductImage); 
        $('.bottombtn').bind('click', function () { _self.savePDT() });
        $('.moneyinfo').css("width", "740px"); 
        $("#description").html(decodeURI(model.Description));
    } 
    //绑定样式图
    ObjectJS.bindOrderImages = function (orderimages) {
        var _self = this;
        var images = orderimages.split(",");
        _self.images = images; 
        if (orderimages == null || orderimages=="" || images.length == 0) {
            $(".order-imgs-list").html('<li class="hover"><img src="/modules/images/none-img.png"></li>');
        } else {
            for (var i = 0; i < images.length; i++) {
                if (images[i]) {
                    if (i == 0) {
                        $("#orderImage").attr("src", images[i].split("?")[0]);
                    }
                    var img = $('<li class="' + (i == 0 ? 'hover' : "") + '"><img src="' + images[i] + '" /></li>');
                    $(".order-imgs-list").append(img);
                }
            }
        }
        $(".order-imgs-list li").click(function () {
            var _this = $(this); 
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $("#orderImage").attr("src", _this.find("img").attr("src").split("?")[0]);
            if ($("#orderImage").width() > $("#orderImage").height()) {
                $("#orderImage").css("width", 350);
            } else {
                $("#orderImage").css("height", 350);
            } 
        }); 
        $($(".order-imgs-list .hover")).click();

        //图片放大功能
        var width = document.documentElement.clientWidth, height = document.documentElement.clientHeight;
        $("#orderImage").click(function () {
            if ($(this).attr("src")) {
                $(".enlarge-image-bgbox,.enlarge-image-box").fadeIn();
                $(".right-enlarge-image,.left-enlarge-image").css({ "top": height / 2 - 80 })

                $(".enlarge-image-item").append('<img id="enlargeImage" src="' + $(this).attr("src") + '"/>');
                $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });

                $(".close-enlarge-image").unbind().click(function () {
                    $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                    $(".enlarge-image-item").empty();
                });
                $(".enlarge-image-bgbox").unbind().click(function () {
                    $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                    $(".enlarge-image-item").empty();
                });
                $(".zoom-botton").unbind().click(function (e) {
                    var scaleToAdd = 0.8;
                    if (e.target.id == 'zoomOutButton')
                        scaleToAdd = -scaleToAdd;
                    $('#enlargeImage').smartZoom('zoom', scaleToAdd);
                    return false;
                });

                $(".left-enlarge-image").unbind().click(function () {
                    var ele = $(".order-imgs-list .hover").prev();
                    if (ele && ele.find("img").attr("src")) {
                        var _img = ele.find("img");
                        $(".order-imgs-list .hover").removeClass("hover");
                        ele.addClass("hover"); 
                        $("#orderImage").attr("src", _img.attr("src").split("?")[0]);
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src").split("?")[0] + '"/>');
                        $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                    }
                });

                $(".right-enlarge-image").unbind().click(function () {
                    var ele = $(".order-imgs-list .hover").next();
                    if (ele && ele.find("img").attr("src")) {
                        var _img = ele.find("img");
                        $(".order-imgs-list .hover").removeClass("hover");
                        ele.addClass("hover"); 
                        $("#orderImage").attr("src", _img.attr("src"));
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src").split("?")[0] + '"/>');
                        $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                    }
                });
            }
        });

        if ($("#orderImage").width() > $("#orderImage").height()) {
            $("#orderImage").css("width", 350);
        } else {
            $("#orderImage").css("height", 350);
        }
    }
    //绑定颜色尺码
    var colorList = {}, tempOrder= {},tempList = [];
    ObjectJS.getOrderAttr = function () {
        var _self = this;
        for (var i = 0; i < _self.model.ProductDetails.length; i++) {
            var item = _self.model.ProductDetails[i];
            if (item.AttrValue != "" && item.AttrValue != null) {
                var color = item.AttrValue.split(",");
                var key = "[" + color[0] + "]" + (color.length > 1 ? "[" + color[1] + "]" : "");
                colorList[key] = item.ProductDetailID;
                if ($.inArray(color[0], tempList)==-1) {
                    colorList[color[0]] = color.length > 1 ? color[1] : color[0];
                    tempList.push(color[0]);
                } else {
                    colorList[color[0]] = colorList[color[0]] + (color.length > 1 ?  "," +color[1] : "");
                }
            }
        }
        var chtml = "",shtml = ""; 
        for (var i = 0; i < _self.model.SaleAttrs.length; i++) {
            var item = _self.model.SaleAttrs[i];
            for (var j = 0; j < item.AttrValues.length; j++) {
                var citem = item.AttrValues[j]; 
                if (_self.model.SaleAttrs.length == 1 || i == 1) {
                    if (i == (_self.model.SaleAttrs.length - 1)) {
                        $('#sizeName').html(item.AttrName + ":");
                        shtml += '<tr class="last-row trattr" data-remark="' + citem.ValueName + '">';
                    } else {
                        $('#colorName').html(item.AttrName + ":");
                        shtml += '<tr calss="trattr" data-remark="' + citem.ValueName + '">';
                    }
                    shtml += '<td class="name" >' + citem.ValueName.replace('【', ' ').replace('】', ' ') + '</td>' +
                        '<td class="price">' + _self.model.Price.toFixed(2) + '元</td>' +
                        '<td class="center"><div class="choose-quantity left mLeft30">' +
                        '<span class="quantity-jian" style="left:1px;padding: 0 5px;border-right: 0 none;box-sizing:border-box;height:26px;">-</span>' +
                        '<input type="text" class="quantity" style="width:70px;border-radius:0px;box-sizing:border-box;height:26px;" value="0"  data-remark="' + citem.ValueName + '" />' +
                        '<span class="quantity-add"  style="right:1px;padding: 0 2px; border-left: 0 none;box-sizing:border-box;height:26px;">+</span>' +
                        '</div></td>' +
                        '</tr>';
                } else {
                    $('#colorName').html(item.AttrName+":");
                    chtml += '<li class="left" data-id="' + item.AttrID + '" data-remark="' + citem.ValueName + '" >' + citem.ValueName.replace('【', ' ').replace('】', ' ') + '</li>';
                }
            }
        }
        $('#colorlist').html(chtml);
        if (chtml == "") {
            $('#licolor').hide();
        }
        $('#sizelist').html(shtml);
        if (shtml != ""){
            $('.sizediv').addClass("linetopbottom");
            $('#btnSubmit').show();
        } 
        $('#colorlist li').click(function () {
            var _this = $(this);
            //数量验证
            var totalnum = 0; 
            $("#sizelist .quantity").each(function () {
                totalnum += parseInt($(this).val());
            });
            if (totalnum > 0) {
                $('#colorlist li.hover').addClass("hasquantity"); 
            }
            if ($('#colorlist li.hover').length > 0) {
                _self.setOrdersCache();
                $("#sizelist .quantity").each(function() {
                    $(this).val(0);
                });
            }  
            _this.siblings().removeClass("hover");
            _this.removeClass("hasquantity").addClass("hover"); 
            $('.trattr').each(function () {
                if(colorList[_this.data("remark")].indexOf($(this).data("remark"))==-1){
                    $(this).hide();
                    $(this).find('.quantity').val(0);  
                } else {
                    $(this).show();
                }
            });
            _self.SumNumPrice();
            _self.setOrdersQuantity(); 
        });

        $('#sizelist .quantity').keyup(function () {
            $(this).val($(this).val().replace(/\D/g, ''));
        }).blur(function() {
            if ($(this).val() == "") {
                $(this).val('0');
            }
            _self.SumNumPrice();
        }); 
        $('#sizelist .quantity-jian').click(function() {
            var _this = $(this);
            if (_this.next().val() != 0) {
                _this.next().val(parseInt(_this.next().val()) - 1);
                _self.SumNumPrice();
            } 
        }).mouseover(function () {
            $(this).next().addClass("hover");
        }).mouseout(function () {
            $(this).next().removeClass("hover");
        });
        $('#sizelist .quantity-add').click(function () {
            var _this = $(this);
            if (_this.prev().val() != "") {
                _this.prev().val(parseInt(_this.prev().val()) + 1);
                _self.SumNumPrice();
            }
        }).mouseover(function () {
            $(this).prev().addClass("hover");
        }).mouseout(function () {
            $(this).prev().removeClass("hover");
        }); 
    }
    ObjectJS.SumNumPrice = function () {
        var _self = this;
        var sumnum = 0, sumprice = 0.00;
        if (!$.isEmptyObject(tempOrder)) {
            $.each(tempOrder, function(i, obj) {
                sumnum += parseInt(obj.quantity);
            });
        }
        $('#sizelist .quantity').each(function() {
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
    }
    ObjectJS.savePDT = function () {
        $('.bottombtn').unbind('click');
        var _self = this;
        _self.setOrdersCache(); 
        if ($.isEmptyObject(tempOrder)) {
            $('.bottombtn').bind('click', function () { _self.savePDT() });
            alert("请选择颜色尺码，并填写对应采购数量");
            return false;
        } 
        _self.showUserInfo();
        $('.bottombtn').bind('click', function () { _self.savePDT() });
    }
    ObjectJS.showUserInfo = function () {
        var _self = this;
        var openhtml = '<ul class="table-add">' +
                   '<li> <span class="width80">联系人：</span> <span><input type="text" id="ContactName" class="verify" value="' + $('#iptcontactName').val() + '" data-empty="联系人不能为空!"></span> </li>' +
                   '<li> <span class="width80">联系电话：</span> <span><input type="text" id="MobilePhone" class="verify" value="' + $('#iptmobilePhone').val() + '" maxlength="11" data-empty="电话不能为空!"></span> </li>' +
                   '<li> <span class="width80">地址：</span> <span id="citySpan"></span></li>' +
                   '<li> <span class="width80"></span> <span><input type="text" placeholder="详细地址..." class="width300" id="Address" value="' + $('#iptaddress').val() + '"></span> </li>' +
                   '</ul>';
        Easydialog.open({
            container: {
                id: "orderinfo-box",
                header: "收货信息确认",
                content: openhtml,
                yesFn: function () {
                    _self.submitOrder($('#ContactName').val(), $('#MobilePhone').val(), CityInvoice.getCityCode(), $('#Address').val());
                }
            }
        });
        CityInvoice = City.createCity({
            cityCode: _self.CityCode,
            elementID: "citySpan"
        });
    };
    ObjectJS.submitOrder = function (personname,mobiletele,citycode,address) {
        var _self = this; 
        var dids = '';
        var details = [];
        $.each(tempOrder, function(i, obj) {
            //details.push({DetailID:obj.detailid,Quantity:obj.quantity,Remark:obj.key });
            dids += obj.detailid + ":" + obj.quantity + ",";
        }); 
        Global.post("/Mall/Store/CreatePurchaseOrder",
            {
                productid: _self.model.ProductID,
                price: _self.model.Price,
                parentprid: _self.clientid,
                goodsid: _self.model.CMGoodsID,
                goodscode: _self.model.CMGoodsCode,
                goodsname: _self.model.ProductName,
                personname: personname,
                mobiletele: mobiletele,
                citycode: citycode,
                dids: dids,
                address: address
                //,entity: JSON.stringify(details)
            }, function (data) {
            if (data.result==1) {
                confirm("新增成功,是否返回继续选购产品！",
                    function () {
                        $('#btnback').click();
                    },
                    function () {
                        $('#btndetail').parent().attr("href", $('#ipturl').val() + '/Purchase/DocDetail/' + data.PurchaseID); 
                        $('#btndetail').click();
                    });
            } else {
                alert(data.errMsg);
            }
        });
    }
    //数量缓存
    ObjectJS.setOrdersCache = function() {
        $('#sizelist .quantity').each(function () {
            var _this = $(this);
            var size = _this.parent().parent().parent().data("remark"); 
            var color = '';
            if ($('#colorlist li').length>0 ){
                if ($('#colorlist li.hover').length > 0) {
                    var color = $('#colorlist li.hover').data("remark");
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
        $('#sizelist .quantity').each(function() {
            var _this = $(this);
            var size = _this.parent().parent().parent().data("remark");
            var color = '';
            if ($('#colorlist li.hover').length > 0) {
                var color = $('#colorlist li.hover').data("remark");
            }
            var key = (color != "" ? "[" + color + "]" : "") + "[" + size + "]";  
            if (typeof (tempOrder[key]) != 'undefined' && typeof (colorList[key]) != 'undefined') {
                _this.val(tempOrder[key].quantity);
            }
        }); 
    };

    module.exports = ObjectJS;
})