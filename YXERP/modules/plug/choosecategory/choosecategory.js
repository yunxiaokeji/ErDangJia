define(function (require, exports, module) {
    require("plug/choosecategory/choosecategory.css");
    var DoT = require("dot");
    (function () {
        var menuData = [];
        var cacheData = []; 
        var CacheCategory = [];
        var options;
        var _eleHeader;
        var _menuContent;

        $.fn.chooseCategory = function (option) {
            return this.each(function () {
                var _this = $(this);
                _this.val('');
                options = $.extend({}, $.fn.chooseCategory.default, option);
                cacheData = options.data;
                if (options.isInit) {
                    $(".change-menu-body").remove();
                    menuData = [];
                } 
                CacheCategory = cacheData; 
                for (var i = 1; i <= options.layer; i++) {
                    var data = {
                        layer: i,
                        id: "",
                        name: ""
                    };
                    menuData.push(data);
                }
                $.fn.chooseCategory.bindMenu(_this);
            });
        }; 

        $.fn.chooseCategory.default = {
            width: 400,
            data: [],
            layer: 3,
            isInit: false,/*是否初始化控件*/
            defaults: {
                headerText: "请选择",
                headerID: ""
            },
            onHeaderChange: function () {
            },
            onCategroyChange: function () {
            }
        };

        $.fn.chooseCategory.bindMenu = function (obj) {
            //绑定样式
            obj.css({
                "background": "url('/modules/images/ico-dropdown.png') no-repeat right 5px center"
            });

            obj.click(function () {
                var _this = $(this);
                var offset = obj.offset();
                if ($(".change-menu-body").length == 0) {
                    DoT.exec("../modules/plug/choosecategory/choosecategory.html", function (template) {
                        var innerText = template();
                        innerText = $(innerText);
                        innerText.css({
                            "width": options.width + "px",
                            "left": offset.left + "px",
                            "top": (offset.top + 27) + "px"
                        });
                        innerText.find('.close-layer').click(function () {
                            $(".change-menu-body").hide();
                        });
                        $('body').append(innerText);
                        bindObj(cacheData, obj, options.defaults);
                    });
                } else {
                    $(".change-menu-body").css({ "left": offset.left + "px", "top": (offset.top + 27) + "px" }).show();
                }
                return false;
            });
        };

        var bindObj = function (data, obj, _headerData) {
            /*头部数据处理*/
            if (_headerData.headerText) {
                $(".change-menu-header").find('li').removeClass('hover');
                $(".change-menu-header").find('li:last-child').html(_headerData.headerText);
                var _headerMenu = $('<li class="hand hover" data-id="' + _headerData.headerID + '">请选择</li>');
                $(".change-menu-header ul").append(_headerMenu);
                _headerMenu.click(function () {
                    if (!$(this).hasClass('hover')) {
                        $(this).siblings().removeClass('hover');
                        $(this).addClass('hover');
                        _headerMenu.nextAll().remove();

                        var headerData = {
                            headerText: '',
                            headerID: '',
                            headerLayer:0
                        };
                        $(this).html("请选择");
                        $(".change-menu-content").find('ul').empty();
                        var id = $(this).data("id");
                        var item = getCacheDataByID(id);
                        if (!item) {
                            $(".change-menu-content").append('<div class="nodata-txt">暂无分类信息！</div>');
                        } else {
                            $(".change-menu-content").find('.nodata-txt').remove();
                            bindObj(item, obj, headerData);
                            !options.onHeaderChange || options.onHeaderChange(GetDetails(id, CacheCategory));
                        }
                    }
                });
            }

            /*子分类数据处理*/
            if (data) {
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    var _childMenu = $('<li class="hand categategory-item" data-layer="' + item.Layers + '" data-name="' + item.CategoryName + '" data-id="' + item.CategoryID + '">' + item.CategoryName + '</li>');
                    $(".change-menu-content").find('ul').append(_childMenu);
                    _childMenu.click(function () {
                        var _this = $(this);
                        var id = $(this).data("id");
                        if (_this.data('layer') < options.layer) {
                           
                            var headerData = {
                                headerText: _this.text(),
                                headerID: _this.data("id"),
                                headerLayer: _this.data("layer")
                            };
                            var item = getCacheDataByID(id);
                            $(".change-menu-content").find('ul').empty();
                            if (item==null || item.length == 0) {
                                $(".change-menu-content").append('<div class="nodata-txt">暂无分类信息！</div>');
                                bindObj(item, obj, headerData);
                                $(".change-menu-body").hide();
                            } else {
                                $(".change-menu-content").find('.nodata-txt').remove();
                                bindObj(item, obj, headerData);
                            }
                        } else {
                            $(".change-menu-header").find('li:last-child').html(_this.text());
                            $(".change-menu-body").hide();
                        }

                        /*保存文本框中填入值的层级、id、name*/
                        var _layer = _this.data('layer');
                        menuData[_layer - 1].id = _this.data('id');
                        menuData[_layer - 1].name = _this.data('name');
                        for (var k = 0; k < menuData.length; k++) {
                            var itemMD = menuData[k];
                            if (itemMD.layer <= _layer) {
                                continue;
                            }
                            itemMD.id = '';
                            itemMD.name = '';
                        }

                        /*拼接文本框中的值*/
                        var _desc = "";
                        for (var j = 0; j < menuData.length; j++) {
                            var itemMD = menuData[j];
                            if (!itemMD.id) {
                                continue;
                            }
                            if (_desc) {
                                _desc += "/";
                            }
                            _desc += itemMD.name;
                        }
                        obj.val(_desc); 
                        !options.onCategroyChange ||options.onCategroyChange(GetDetailEntity(id, CacheCategory));
                    });
                }
            }
        };

        //获取子分类
        var getCacheDataByID = function (id) {
            if (!id)
                return cacheData;
            var s = [];
            s = GetDetails(id, CacheCategory);
            return s; 
        };
        var GetDetails = function (id, obj) {
            var s = [];
            if (obj != null) {
                for (var i = 0; i < obj.length; i++) {  
                    if (obj[i].CategoryID == id) {
                        s= obj[i].ChildCategorys;
                        break;
                    } else {
                        for (var j = 0; j < obj[i].ChildCategorys.length; j++) {
                            if (s.length == 0) {
                                s = GetDetails(id, obj[i].ChildCategorys);
                            } else {
                                break;
                            }
                        }
                    }
                }  
            }
            return s;
        }; 
        var GetDetailEntity = function (id, obj) {
            var s = null;
            if (obj != null) {
                for (var i = 0; i < obj.length; i++) { 
                    if (obj[i].CategoryID == id) { 
                        s = obj[i];
                        break;
                    } else {
                        for (var j = 0; j < obj[i].ChildCategorys.length; j++) { 
                            if (s==null) {
                                s = GetDetailEntity(id, obj[i].ChildCategorys);
                            } else {
                                break;
                            }
                        }
                    }
                }
            }
            return s;
        };
        $(document).click(function (e) {
            var ele = $(e.target);
            if (!ele.parents().hasClass('change-menu-body') && !ele.hasClass('change-menu-body')
                && !ele.parents().hasClass('change-menu-content')&& !ele.hasClass('categategory-item')) {
                $(".change-menu-body").hide();
            }
        });
    })(jQuery);
});