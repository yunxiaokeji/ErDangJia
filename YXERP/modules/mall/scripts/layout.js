define(function(require, exports, module) {
    var Global = require("global"), 
    doT = require("dot"); 

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (clientid) {
        var _self = this;
        _self.clientid = clientid; 
        _self.bindEvent();
    }
    ObjectJS.bindEvent = function () {
        var _self = this; 
        //分类
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
            location.replace($('#ipturl').val() + '/Purchase/Purchases?souceType=2%26name=采购订单');
        });
        $('#providerinfo').mouseover(function () {
            var xy = $(this).position();
            $('.providerdiv').css("left", xy.left+30);
            $('.providerdiv').css("top", xy.top + $(this).height()-5);
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
    ObjectJS.getAllCategory = function (categpryid) {
        if (categpryid == $('.categoryMenu:last').data('id') && categpryid !='-1') {
            return;
        }
        if ($('.categoryMenu:last').data('id') == '-1') {
            $('.categoryMenu:last').data('id', '');
        }
        categpryid = categpryid == '-1' ? '' : categpryid;
        var _self = this;
        $(".divcategory").html('');
        Global.post("/Mall/Store/GetEdjCateGory", { clientid: _self.clientid, categoryid: categpryid }, function (data) {
            if (data.items.length > 0) {
                var html = "";
                for (var i = 0; i < data.items.length; i++) {
                    var item = data.items[i];
                    html += "<dl ><dt data-id='" + categpryid + "'><a  data-id='" + item.CategoryID + "' data-name='" + item.CategoryName + "' style='font-size: 14px;'>" + item.CategoryName + "</a></dt>";
                    for (var j = 0; j < item.ChildCategorys.length; j++) {
                        var childcate = item.ChildCategorys[j];
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
                $(".divcategory").append("<div class='nodata-box' >暂无分类</div>");
            }
            $(".divcategory").find('a').click(function () {
                var _this = $(this);
                $('.categoryMenu').removeClass('hover');
                if (_this.data('id') != '') {
                    var pind = _self.getCategoryIndex(categpryid);
                    if (pind > -1) {
                        $('.categoryMenu').eq(pind).nextAll().remove();
                    }  
                    var ind = _self.getCategoryIndex(_this.data('id')); 
                    if (_this.parent().data('id') == "" || typeof (_this.parent().data('name')) == 'undefined') {
                        if (ind > -1) {
                            $('.categoryMenu').eq(ind).nextAll().remove();
                        } else {
                            $('.categoryMenu').eq(pind>-1?pind:0).nextAll().remove();
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
                $('.categoryMenu').unbind('mouseover').bind('mouseover', function() {
                    _self.getAllCategory($(this).data('id'));
                });
                $('.categoryMenu').click(function() {
                    $(this).addClass('hover').nextAll().remove();
                     
                });
            });
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
    ObjectJS.getClientDetail = function () {
        var _self = this;
        Global.post("/Mall/Store/GetClientDetail", { clientid: _self.clientid }, function (data) {
            if (data != null) {
                $('.pname').html(data.ContactName + " &nbsp; " + data.MobilePhone);
                $('.pcompanyName').html(data.CompanyName);
                $('#pcityname').html(data.City != null ? data.City.Description + data.Address : data.Address);
                $('#ptype').html('店铺分销');

            }
        });
    } 
    module.exports = ObjectJS;

});