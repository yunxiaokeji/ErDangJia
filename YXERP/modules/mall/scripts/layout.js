define(function(require, exports, module) {
    var Global = require("global"), 
    doT = require("dot");
    var categorylist = [];
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (clientid,categoryid) {
        var _self = this;
        _self.clientid = clientid; 
        _self.categoryid = categoryid;
        _self.bindEvent();
    }
    ObjectJS.bindEvent = function () {
        var _self = this; 
        //分类
        $('#userinfo').click(function () {
            var xy = $(this).offset(); 
            $('.dropdown-userinfo').css("left", xy.left);
            $('.dropdown-userinfo').css("top", xy.top + $(this).height());
            $('.dropdown-userinfo').show();
        });
        $(document).click(function(e) {
            if (!$(e.target).parents().hasClass("userinfo") && !$(e.target).hasClass("userinfo")) {
                $(".dropdown-userinfo").fadeOut("1000");
            }
            if ($(e.target).parents().hasClass("qrcodediv") || $(e.target).hasClass("qrcodediv")) {
                $(".qrcodediv").fadeOut("1000");
            }
        });

        $('.categoryMenu').mouseover(function() {
            _self.getAllCategory($(this).data('id'));
        });
        $('#categoryContent').mouseover(function () {
            var xy = $(this).position();
            $('.divcategory').css("left", xy.left);
            $('.divcategory').css("top", xy.top + $(this).height());
            $('.divcategory').show();
        }).mouseout(function () {
            $('.divcategory').hide();
        });

        $('#purchaseurl').click(function () {
            location.replace($('#ipturl').val() + '/Purchase/Purchases?souceType=1%26name=采购订单');
        });

        $('#providerinfo').mouseover(function () {
            var xy = $(this).offset();
            $('.providerdiv').css("left", xy.left);
            $('.providerdiv').css("top", xy.top + $(this).height());
            $('.providerdiv').show();
        }).mouseout(function () {
            $('.providerdiv').hide();
        });

        $('#purchaseurl').click(function () {
            location.replace($('#ipturl').val() + '/Purchase/Purchases?souceType=2%26name=采购订单');
        });
        _self.getClientDetail();
        _self.getAllCategory('');
    }
    //获取所有分类
    ObjectJS.getAllCategory = function (categoryid) { 
        var _self = this;
        $(".divcategory").html(''); 
        if (categorylist.length == 0) {
            Global.post("/Mall/Store/GetEdjCateGory", { clientid: _self.clientid, categoryid: categoryid }, function (data) {
                categorylist = data.items;
                _self.setCategoryMenu();
                _self.bindCategory(categorylist, _self.categoryid);
            });
        } else {  
            _self.bindCategory(categorylist,categoryid);
        }
    }
    //绑定分类
    ObjectJS.bindCategory = function (clist, categoryid) {
        var templist = [];
        var pcategory= {};
        for (var i = 0; i < clist.length; i++) { 
            if (clist[i].PID == categoryid) {
                templist.push(clist[i]);
            }
            if (clist[i].CategoryID == categoryid) {
                pcategory = clist[i];
            }
        }
        var _self = this; var html = "";
        if (templist.length > 0) {
            for (var i = 0; i < templist.length; i++) {
                var item = templist[i];
                html += "<dl ><dt data-id='" + categoryid + "'><a  data-id='" + item.CategoryID + "' data-name='" + item.CategoryName + "' style='font-size: 14px;'>" + item.CategoryName + "</a></dt>";
                var childList = typeof (item.ChildCategorys) != 'undefined' ? item.ChildCategorys : item.ChildCategory;

                for (var j = 0; j < childList.length; j++) {
                    var childcate = childList[j];
                    html += "<dd data-id='" + item.CategoryID + "' data-name='" + item.CategoryName + "' ";
                    if ((j + 1) % 3 == 0) {
                        html += "style='margin-right:0px;'";
                    }
                    html += "><a data-id='" + childcate.CategoryID + "' data-name='" + childcate.CategoryName + "'>" + childcate.CategoryName + "</a></dd>";
                }
                html += "</dl>";
            }
            html += "<div class='clear'></div>";
            $(".divcategory").append(html);
        } else { 
            if (typeof (pcategory.PID) != 'undefined' && clist.length>0) {
                _self.bindCategory(clist, pcategory.PID);
            }
            if (clist.length > 0) {
                $(".divcategory").hide();
            } else {
                $(".divcategory").append("<p class='center'>暂未获取到分类信息</p>");
            }
        }
        $(".divcategory").find('a').click(function () {
            var _this = $(this);
            $('.categoryMenu').removeClass('hover');
            if (_this.data('id') != '') {
                var pind = _self.getCategoryIndex(categoryid);
                if (pind > -1) {
                    $('.categoryMenu').eq(pind).nextAll().remove();
                }
                var ind = _self.getCategoryIndex(_this.data('id'));
                if (_this.parent().data('id') == "" || typeof (_this.parent().data('name')) == 'undefined') {
                    if (ind > -1) {
                        $('.categoryMenu').eq(ind).nextAll().remove();
                    } else {
                        $('.categoryMenu').eq(pind > -1 ? pind : 0).nextAll().remove();
                    }
                    $('.categoryMenu:last').after('<div class="header-title left pLeft10 categoryMenu hover" data-id="' + _this.data('id') + '">' + _this.data('name') + '</div>');
                } else {
                    if (ind > -1) {
                        $('.categoryMenu').eq(ind).nextAll().remove();
                        $('.categoryMenu:last').after('<div class="header-title left pLeft10 categoryMenu hover" data-id="' + _this.data('id') + '">' + _this.data('name') + '</div>');
                    } else {
                        $('.categoryMenu').eq(pind > -1 ? pind : 0).nextAll().remove();
                        $('.categoryMenu:last').after('<div class="header-title left pLeft10 categoryMenu " data-id="' + _this.parent().data('id') + '">' + _this.parent().data('name') + '</div>' +
                            '<div class="header-title left pLeft10 categoryMenu hover" data-id="' + _this.data('id') + '">' + _this.data('name') + '</div>');
                    }
                }
            } else {
                $('.categoryMenu:first').next().nextAll().remove();
            }
            $('.categoryMenu').unbind('mouseover').bind('mouseover', function () {
                _self.getAllCategory($(this).data('id'));
            });
            _self.getAllCategory($(this).data('id'));
        });
        $('.categoryMenu').click(function () {
            $(this).addClass('hover').nextAll().remove();
            _self.getAllCategory($(this).data('id'));
        });
    }

    ObjectJS.getCategoryIndex = function(id) {
        var ind = -1;
        $('.categoryMenu').each(function (i, obj) {
            if ($(obj).data('id') == id) {
                ind= i;
                return false;
            }
        });
        return ind;
    }
    ObjectJS.setCategoryMenu = function () {
        var _self = this;
     
        $('.categoryMenu:first').next().nextAll().remove();
        $('.categoryMenu:first').removeClass('hover').after(_self.getCategoryMenuHtml(_self.categoryid));
        $('.categoryMenu:last').addClass('hover'); 
        $('.categoryMenu').unbind('mouseover').bind('mouseover', function () {
            _self.getAllCategory($(this).data('id'));
        }); 
    }
    ObjectJS.getCategoryMenuHtml = function (categoryid) {
        var _self = this;
        var item = '';  
        if (categoryid != '') {
            for (var i = 0; i < categorylist.length; i++) { 
                if (categorylist[i].CategoryID == categoryid) { 
                    if (categorylist[i].PID != '') {
                        item += _self.getCategoryMenuHtml(categorylist[i].PID);
                    }
                    item += '<div class="header-title left pLeft10 categoryMenu" data-id="' + categorylist[i].CategoryID + '">' + categorylist[i].CategoryName + '</div>';
                }
            }
        } 
        return item;
        
    }
    ObjectJS.getClientDetail = function () {
        var _self = this;
        Global.post("/Mall/Store/GetClientDetail", { clientid: _self.clientid }, function (data) {
            if (data != null) {
                $('.pname').html(data.ContactName + " &nbsp; " + data.MobilePhone);
                $('.pcompanyName').html(data.CompanyName);
                $('#pcityname').html(data.City != null ? data.City.Description + '<br/>'+data.Address : data.Address);
                $('#ptype').html('店铺分销');
                 
            }
        });
    }
    
    module.exports = ObjectJS;

});