define(function (require, exports, module) {
    var City = require("city"), CityInvoice,
        Verify = require("verify"), VerifyInvoice,
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog"),
        Upload = require("upload");
    require("smartzoom");
    require("pager"); 

    var ObjectJS = {}, CacheItems=[];

    ObjectJS.init = function (clientid,  model,citycode) {
        var _self = this;
        _self.clientid = clientid;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"')); 
        console.log(_self.model);
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
    ObjectJS.getOrderAttr = function () {
        var _self = this;
        Global.post("/StyleCenter/StyleCenter/GetOrderAttrsList", { goodsid: _self.model.CMGoodsID }, function (data) {
            var chtml = "";
            var shtml = "";
            console.log(data);
            for (var i = 0; i < data.items.length; i++) {
                var item = data.items[i]; 
                if (item.AttrType == 2) {
                    chtml += '<li data-id="' + item.OrderAttrID + '" data-remark="' + item.AttrName + '" >' + item.AttrName.replace('【', ' ').replace('】', ' ') + '</li>';
                } else {
                    if (i == (data.items.length - 1)) {
                        shtml += '<tr class="last-row" data-remark="' + item.AttrName + '">';
                    } else {
                        shtml += '<tr data-remark="' + item.AttrName + '">';
                    }
                    shtml += 
                        '<td class="name" >' + item.AttrName.replace('【', ' ').replace('】', ' ') + '</td>' +
                        '<td class="price">' + item.FinalPrice.toFixed(2) + '元</td>' +
                        '<td class="center"><div class="choose-quantity left">' +
                        '<span class="quantity-jian" style="left:1px;padding: 0 5px;border-right: 0 none;">-</span>' +
                        '<input type="text" class="quantity" style="width:70px;border-radius:0px;" value="0"  data-remark="' + item.AttrName + '" />' +
                        '<span class="quantity-add"  style="right:1px;padding: 0 2px; border-left: 0 none;">+</span>' +
                        '</div></td>' +
                        '</tr>';
                }
            }
            $('#colorlist').html(chtml);
            $('#sizelist').html(shtml);

            $('#colorlist li').click(function() {
                if ($(this).hasClass("hover")) {
                    $(this).removeClass("hover");
                } else {
                    $(this).addClass("hover");
                }
            });

            $('#sizelist .quantity').keyup(function () {
                $(this).val($(this).val().replace(/\D/g, ''));
            }).blur(function() {
                if ($(this).val() == "") {
                    $(this).val('0');
                }
            }); 
            $('#sizelist .quantity-jian').click(function() {
                var _this = $(this);
                if (_this.next().val() != 0) {
                    _this.next().val(parseInt(_this.next().val()) - 1);
                }
            });
            $('#sizelist .quantity-add').click(function () {
                var _this = $(this);
                if (_this.prev().val() != "") {
                    _this.prev().val(parseInt(_this.prev().val()) + 1);
                }
            });
        });
    }
    ObjectJS.savePDT = function () {
        $('.bottombtn').unbind('click');
        var _self = this;
        var totalnum = 0;
        var hashover = false;
        $("#sizelist .quantity").each(function () {
            totalnum += parseInt($(this).val());
        });
        $("#colorlist li").each(function () {
            if ($(this).hasClass("hover")) {
                hashover = true;
            }
        });
        if (totalnum == 0 || !hashover) {
            $('.bottombtn').bind('click', function () { _self.savePDT() });
            alert("请选择颜色尺码，并填写对应采购数量");
            return false;
        }
        Global.post("/StyleCenter/StyleCenter/IsLogin", null, function (data) {
            if (data.result) {
                _self.showUserInfo();
            } else {
                alert("请登陆后，再提交信息");
                return false;
                //$('#orderlogin-box').empty();
                //var openhtml = '<div class="login-body"><div class="main"><div class="login">' +
                //    '<div class="registerErr hide"></div>' +
                //    '<div class="loginbar">' +
                //        '<input type="text" class="input icoUserName" id="iptUserName" placeholder="请输入账号" maxlength="25"/>'+
                //    '</div>' +
                //    '<div class="loginbar" style="position:relative;"><input type="password" class="input icoUserPwd" id="iptPwd"/>'+
                //    '</div>'+
                //    '<div class="remember-box">'+
                //        '<div class="cb-remember-password ico-check ">一周内保持登录</div>' +
                //        '<div class="right pRight5"><a href="/Home/FindPassword" class="linkRegister">忘记密码?</a></div>'+
                //    '<div class="clear"></div>' +
                //'</div>'+
                //'<div class="btnLogin" id="btnLogin">登录</div></div>';
                //Easydialog.open({
                //    container: {
                //        id: "orderlogin-box",
                //        header: "<a style='font-size:18px;'>账号登录</a>",
                //        content: openhtml 
                //    }
                //});
                ////登录 
                //$('#orderlogin-box').find("#btnLogin").click(function () {
                //    if (!$("#iptUserName").val()) {
                //        $(".registerErr").html("请输入账号").slideDown();
                //        return;
                //    }
                //    if (!$("#iptPwd").val()) {
                //        $(".registerErr").html("请输入密码").slideDown();
                //        return;
                //    }
                //    $(this).html("登录中...").attr("disabled", "disabled");
                //    Global.post("/Home/UserLogin", {
                //        userName: $("#iptUserName").val(),
                //        pwd: $("#iptPwd").val(),
                //        otherid: $("#OtherID").val(),
                //        remember: $(".cb-remember-password").hasClass("ico-checked") ? 1 : 0,
                //        bindAccountType: 10000
                //    },
                //    function (data) {
                //        $("#btnLogin").html("登录").removeAttr("disabled");
                //        if (data.result == 1) {
                //            Easydialog.close();
                //            Global.post("/StyleCenter/StyleCenter/LoginCallBack", {
                //                sign: data.sign,
                //                uid: data.uid,
                //                aid: data.aid
                //            }, function(dresult) {
                //                _self.CityCode = dresult.ccode;
                //                $('#iptaddress').val(dresult.address);
                //                $('#iptmobilePhone').val(dresult.mphone);
                //                $('#iptcontactName').val(dresult.cname);
                //                _self.showUserInfo();
                //            });
                //        }
                //        else if (data.result == 0) {
                //            $(".registerErr").html("账号或密码有误").slideDown();
                //        }
                //        else if (data.result == 2) {
                //            $(".registerErr").html("密码输入错误超过3次，请2小时后再试").slideDown();
                //        }
                //        else if (data.result == 3) {
                //            $(".registerErr").html("账号或密码有误,您还有" + (3 - parseInt(data.errorCount)) + "错误机会").slideDown();
                //        }
                //        else if (data.result == -1) {
                //            $(".registerErr").html("账号已冻结，请" + data.forbidTime + "分钟后再试").slideDown();
                //        }
                        
                //    });
                //});
                ////记录密码
                //$('#orderlogin-box').find(".cb-remember-password").click(function () { 
                //    var _this = $(this);
                //    if (_this.hasClass("ico-check")) {
                //        _this.removeClass("ico-check").addClass("ico-checked");
                //    } else {
                //        _this.removeClass("ico-checked").addClass("ico-check");
                //    }
                //});
            }
        });
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
        //大货单遍历下单明细 
        var details = [];
        $(".child-product-table .quantity").each(function () {
            var _this = $(this);
            if (_this.val() > 0) {
                $("#colorlist li").each(function() {
                    if ($(this).hasClass("hover")) {
                        details.push({
                            Quantity: _this.val(),
                            Remark: $(this).data("remark") + _this.data("remark")
                    });
                    }
                });
            }
        });
        Global.post("/StyleCenter/StyleCenter/CreatePurchaseOrder",
            {
                productid: _self.model.ProductID,
                price: _self.model.Price,
                parentprid: _self.clientid,
                goodsid: _self.model.CMGoodsID,
                goodscode: _self.model.CMGoodsCode,
                goodsname: _self.model.ProductName,
                entity: JSON.stringify(details)
            }, function (data) {
            if (data.result==1) {
                confirm("新增成功,是否返回继续选购产品！",
                    function () {
                        $('#btnback').click();
                    },
                    function () {
                        $('#btndetail').parent().attr("href", $('#ipturl').val() + '/Purchase/DocDetail/' + data.PurchaseID);
                        //window.open($('#ipturl').val() + $('#btndetail').parent().data("url") + "PurchaseID", "", "fullscreen=1");

                        $('#btndetail').click();
                    });
            } else {
                alert(data.error_message);
            }
        });
    }
    module.exports = ObjectJS;
})