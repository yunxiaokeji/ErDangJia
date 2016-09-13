define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog");
        doT = require("dot"); 
        require("pager");

    var CacheProduct = [];

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
    ObjectJS.init = function (clientid) {
        var _self = this;
        _self.clientid = clientid;
        _self.providers = [];//JSON.parse(providers.replace(/&quot;/g, '"')); 
        _self.bindEvent();  
    } 
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        var providerids = _self.clientid == "" ? "-1" : _self.clientid;
        //for (var t = 0; t < _self.providers.length; t++) {
        //    if (_self.providers[t].CMClientID != "" && _self.providers[t].CMClientID != null) {
        //        providerids += "''" + _self.providers[t].CMClientID + "'',";
        //    }
        //} 
        //if (providerids != "") {
        //    providerids = providerids.substring(2, providerids.length - 3);
        //} 
        Params.clientid = providerids;
        require.async("dropdown", function () {
            var dropdown = $("#ddlProviders").dropdown({
                prevText: "供应商-",
                defaultText: "全部",
                defaultValue:"-1",
                data: _self.providers,
                dataValue: "CMClientID",
                dataText: "Name",
                width: "180",
                isposition: true,
                onChange: function (data) {
                    Params.pageIndex = 1;
                    if (data.value == "") {
                        Params.clientid = providerids;
                    } else {
                         Params.clientid = data.value;
                    }
                    _self.getProducts();
                }
            });
        });
      
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
     
        $('#spanCategory').click(function() {
            Params.categoryID = '';
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
        $('#imglogo').click(function() {
            location.replace(location);
        });
        $('#myself').click(function () {
            location.replace($('#ipturl').val() + '/MyAccount/Index?&name=个人中心');
        });
        _self.getAllCategory();
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
                        var href = $(this).attr("href");
                        $(this).attr("href", href + "&clientid=" + _self.clientid);
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
        });
    }

    ObjectJS.getAllCategory = function () {
        var _self = this;
        Global.post("/Mall/Store/GetEdjCateGory", null, function (data) {
            if (data.items.length > 0) {
                var html = ""; 
                for (var i = 0; i < data.items.length; i++) { 
                    var item = data.items[i];
                    html += "<dl><dt><a  data-id='" + item.CategoryID + "' style='font-size: 14px;'>" + item.CategoryName + "</a></dt>";
                    for (var j = 0; j < item.ChildCategorys.length; j++) {
                        var childcate = item.ChildCategorys[j];
                        html += "<dd ";
                        if ((j + 1) % 3 == 0) {
                            html += "style='margin-right:0px;'";
                        }
                        html += "><a data-id='" + item.CategoryID + "'>" + childcate.CategoryName + "</a></dd>";
                    }
                    html += "</dl>";
                }
                html += "<div class='clear'></div>";
                $(".divcategory").append(html);
            } else {
                $(".divcategory").append("<div class='nodata-box' >暂无分类</div>");
            }
            $(".divcategory").find('a').click(function() {
                Params.categoryID = $(this).data("id");
                Params.orderBy = "";
                Params.isAsc = false;
                _self.getProducts();
                $('.divcategory').hide();
            });
        });
    }

    module.exports = ObjectJS;
});