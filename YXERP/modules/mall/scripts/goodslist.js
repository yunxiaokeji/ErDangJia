define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog");
        doT = require("dot"); 
        require("pager");

    var Params = {
        categoryID: "",
        clientid: "",
        beginPrice: "",
        endPrice: "",
        orderBy: "",
        isAsc: false,
        pageIndex: 1,
        pageSize: 12,
        keyWords: "" 
    }

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (clientid, categoryid) {
        Params.categoryID = categoryid;
        var _self = this;
        _self.clientid = clientid;
        _self.providers = [];
        _self.bindEvent();  
    } 
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this; 
        Params.clientid = _self.clientid;;
        //搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                _self.getProducts();
            });
        });
        //分类
        $('#divCategory').mouseover(function () {
            var xy = $(this).position(); 
            $('.divcategory').css("left", xy.left-1);
            $('.divcategory').css("top", xy.top + $(this).height());
            $('.divcategory').show();
        }).mouseout(function () { 
            $('.divcategory').hide();
        });
        //价格区间
        $('#pricerange').mouseover(function () {
            $(this).addClass("btnlihover");
            $('#btnPriceRange').show();
        }).mouseout(function () {
            $(this).removeClass("btnlihover");
            $('#btnPriceRange').hide();
        });
        //价格排序
        $('#liprice').mouseover(function () { 
            $('.divprice').show();
        }).mouseout(function () {
            $('.divprice').hide();
        });
        
        $('.seachul li').click(function () {
            var _this = $(this); 
            if (!_this.find(".link").hasClass("hover")) {
                $('.seachul li .link').removeClass("hover");
                $(_this.find(".link")).addClass("hover");
            }
            if (_this.data("name") == "sales" || _this.data("name")=="zh") {
                $('#parentprice').html('价格');
                Params.orderBy = _this.data("desc");
                Params.isAsc = _this.data("asc");
                _self.getProducts();
            } 
        });

        $('.divprice li').click(function () { 
            Params.orderBy = $(this).data("desc");
            Params.isAsc = $(this).data("asc");
            $('#parentprice').html($(this).find('.link').html()); 
            $('.seachul li .link').removeClass("hover"); 
            $('#parentprice').addClass("hover");
            _self.getProducts();
        });
     
        $('#categoryContent').click(function () {
            Params.categoryID = $('.categoryMenu.hover').data('id'); 
            _self.getProducts();
        }); 

        $('#btnPriceRange').click(function () { 
            var beginp = $('#beginprice').val();
            var endp = $('#endprice').val();
            if ((beginp != "" && isNaN(Number(beginp))) || (endp != "" && isNaN(Number(endp)))) {
                alert('价格格式输入有误，请重新输入');
            } else {
                Params.beginPrice = beginp;
                Params.endPrice = endp;
                _self.getProducts();
            }
        }); 
        $('#cgbtn').click(function () {
            location.replace($('#ipturl').val() + '/Purchase/Purchases?souceType=2%26name=采购订单');
        }); 
        $('#myself').click(function () {
            location.replace($('#ipturl').val() + '/MyAccount/Index?&name=个人中心');
        });

        $(".divcategory").find('a').click(function () {
            Params.categoryID = $(this).data("id");
            Params.orderBy = "";
            Params.isAsc = false;
            _self.getProducts();
            $('.divcategory').hide();
        });
        
        _self.getProducts(); 
    }

    //绑定产品列表
    ObjectJS.getProducts = function (params) {
        var _self = this;
        var attrs = [];
        $("#productlist").empty();
        $("#productlist").append("<div class='data-loading' ><div>"); 

        var beginp = $('#beginprice').val();
        var endp = $('#endprice').val();
        if (beginp == "") {
            Params.beginPrice = beginp;
        }
        if (endp == "") {
            Params.endPrice = endp;
        } 
        Global.post("/Mall/Store/GetProduct", Params, function (data) {
            $("#productlist").empty();
            if (data.items.length > 0) {
                doT.exec("Mall/template/goodslist/zngc-products.html", function (templateFun) {
                    var html = templateFun(data.items);
                    html = $(html); 
                    $("#productlist").append(html);
                    $("#productlist a").each(function() {
                        var href = $(this).data("href");
                        $(this).data("href", href + "%26clientid=" + _self.clientid);
                    });
                    $(html).find('.product-item').click(function () {
                        //目前先隐藏
                        //window.open($(this).data('href'), $(this).data('name'));    
                        var src = 'http://qrickit.com/api/qr?qrsize=240&d=' + $('#ipturl').val() +$(this).find('a').data('href');
                        var xy = $(this).offset(); 
                        $('#qrcodediv').css("top", xy.top -20).css("left", xy.left - 20);
                        $('#qrcode').attr('src', src);
                        $('#qrcodediv').show(); 
                    });
                });
            } else {
                $("#productlist").append("<div class='nodata-div'>暂无数据!</div>");
            }

            $("#pager").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: Params.pageIndex,
                display: 5,
                border: true,
                border_color: '#fff',
                text_color: '#333',
                background_color: '#fff',
                border_hover_color: '#ccc',
                text_hover_color: '#000',
                background_hover_color: '#efefef',
                rotate: true,
                images: false,
                mouse: 'slide',
                float: "normal",
                onChange: function (page) {
                    Params.pageIndex = page;
                    ObjectJS.getProducts();
                }
            });
            $('#stylebody').css('min-height', $(window).height() + 'px');
        });
    }

    module.exports = ObjectJS;
});