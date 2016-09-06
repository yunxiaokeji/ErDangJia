define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog");
        doT = require("dot"); 
        require("pager");

    var CacheProduct = [];

    var Params = {
        categoryID: "",
        clientid: "",
        BeginPrice: "",
        EndPrice: "",
        pageIndex: 1,
        pageSize: 10,
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
        var providerids = '';
        for (var t = 0; t < _self.providers.length; t++) {
            if (_self.providers[t].CMClientID != "" && _self.providers[t].CMClientID != null) {
                providerids += "'" + _self.providers[t].CMClientID + "',";
            }
        } 
        if (providerids != "") {
            providerids = providerids.substring(1, providerids.length - 2);
        } 
        Params.clientid = providerids;
        require.async("dropdown", function () {
            var dropdown = $("#ddlProviders").dropdown({
                prevText: "供应商-",
                defaultText: "全部",
                defaultValue:"",
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
        $('#spanCategory').mouseover(function () {
            var xy = $(this).position();
            $('.divcategory').css("left", xy.left);
            $('.divcategory').show();
        }).mouseout(function () { 
            $('.divcategory').hide();
        });
        $('#spanCategory').click(function() {
            Params.categoryID = '';
            _self.getProducts();
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

        Global.post("/IntFactoryModel/IntFactoryOrder/GetProductList", Params, function (data) {
            $("#productlist").empty();
            if (data.items.length > 0) {
                doT.exec("intfactoryorder/template/orders/zngc-products.html", function (templateFun) {
                    var html = templateFun(data.items);
                    html = $(html);

                    //打开产品详情页
                    html.find(".productimg,.name").each(function () {
                        $(this).data("url", $(this).data("url") + "&type=" + _self.type + "&guid=" + _self.guid);
                    }); 
                    $("#productlist").append(html);
                });
            } else {
                $("#productlist").append("<div class='nodata-box' >暂无数据!</div>");
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
        Global.post("/IntFactoryModel/IntFactoryOrder/GetAllCateGory", null, function (data) { 
            if (data.items.length > 0) {
                var html = ""; 
                for (var i = 0; i < data.items.length; i++) { 
                    var item = data.items[i];
                    html += "<dl><dt><a  data-id='" + item.CategoryID + "'>" + item.CategoryName + "</a></dt>";
                    for (var j = 0; j < item.ChildCategory.length; j++) {
                        var childcate = item.ChildCategory[j];
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
                _self.getProducts();
                $('.divcategory').hide();
            });
        });
    }

    module.exports = ObjectJS;
});