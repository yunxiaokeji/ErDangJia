define(function (require,exports,module) {
    var Common = require("/modules/m/scripts/style/createordergoods.js");
    var Global = require("m_global"),
        doT = require("dot");
    var Params = {
        categoryID: "",
        clientid: "",
        beginPrice: "",
        endPrice: "",
        orderBy: "",
        isAsc: false,
        pageIndex: 1,
        pageSize: 5,
        keyWords: ""
    };
    var ObjectJS = {};
    var CacheCategory = [];
    var CacheChildCategory = [];
    var CacheOrder = [];
    var IsShowOrder = false;

    ObjectJS.PageCount = 0;
    ObjectJS.IsLoading = false;
    ObjectJS.init = function (providerid, user) {
        Params.clientid = providerid;
        //providers = JSON.parse(providers.replace(/&quot;/g, '"'));
        //for (var i = 0; i < providers.length; i++) {              
        //    var p = providers[i];
        //    if (p.CMClientID != "" && p.CMClientID != null) {
        //        $(".task-filtertype").append('<li data-providerid="' + p.ProviderID + '" data-id="' + p.CMClientID + '">' + p.Name + '</li>');
        //    }
        //}
        user = JSON.parse(user.replace(/&quot;/g, '"'));
        ObjectJS.user = user;

        ObjectJS.bindEvent();
        ObjectJS.getList();
        ObjectJS.getAllCategory();
    };

    ObjectJS.bindEvent = function () {
        //滚动加载数据
        $(window).scroll(function () {
            if ($(".overlay-addOrder").css('display') == 'none' && $(".filter-object").css('display') == "none") {
                if (document.body.scrollTop > 30) {
                    $(".getback").slideDown("slow");
                } else {
                    $(".getback").slideUp("slow");
                }

                var bottom = $(document).height() - document.documentElement.scrollTop - document.body.scrollTop - $(window).height();
                if (bottom <= 60) {
                    if (!ObjectJS.IsLoading) {
                        setTimeout(function () {
                            Params.pageIndex++;
                            if (Params.pageIndex <= ObjectJS.PageCount) {
                                ObjectJS.getList();
                            }
                            //else {
                            //    $(".prompt").remove();
                            //    $(".list").append('<div class="prompt">已经是最后一条啦</div>');
                            //}
                        }, 300);
                    }
                }
            }
        });

        //页面点击
        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("btn-menu") && !$(e.target).hasClass("btn-menu")) {
                $(".menu-box").slideUp(400);
            }
            if (!$(e.target).parents().hasClass("filter-task") && !$(e.target).hasClass("filter-task")) {
                $(".dropdownlist .potion").slideUp(400);
            }
            if (!$(e.target).parents().hasClass("btn-task-filtertype") && !$(e.target).hasClass("btn-task-filtertype")) {
                $(".task-filtertype").slideUp(400);
            }
            if (!$(e.target).hasClass("category-block") &&!$(e.target).hasClass("show-category")&&
                !$(e.target).parents().hasClass("filter-object") && !$(e.target).parents().hasClass("layer-box")&&
                !$(e.target).parents().hasClass("category-box") && !$(e.target).parents().hasClass("show-category")) {
                $(".layer-body").fadeOut(400);
                $(".filter-object").removeClass('outlayer');
                setTimeout(function () {
                    $(".filter-object").hide();
                }, 401);
            }
        });

        //显示关键字遮罩层
        $(".iconfont-search").click(function () {
            $(".btn-search").text("确定");
            $(".txt-search").val("").focus();
            $(".shade,.search").show();
            $(".span-search").css("width", (document.body.clientWidth - 150) + "px");
        });

        //关键字查询
        $(".btn-search").click(function () {
            var name = $(this).text();
            if (name == "确定") {
                var txt = $(".txt-search").val();
                if (txt != "") {
                    $(".shade").slideUp("slow");
                    $(this).text("取消");
                    Params.pageIndex = 1;
                    Params.keyWords = txt;
                    ObjectJS.getList();

                } else {
                    $(".search").hide();
                }
            } else {
                $(".search").hide();
                Params.keyWords = "";
                Params.pageIndex = 1;
                ObjectJS.getList();
            }
            $(".shade").hide();
        });

        //搜索内容发生变化
        $(".txt-search").keyup(function () {
            
            var changeAfter = $(".txt-search").val();
            if (changeAfter == "") {
                $(".cencal").text("取消");
            } else if (Params.keyWords == changeAfter) {
                $(".cencal").text("取消");
            } else {
                $(".cencal").text("确定");
            }
        });

        //点击遮罩层空白区域
        $(".shade").click(function () {
            $(".shade").hide();
            $(".search").hide();
        });

        //显示主菜单
        $(".btn-menu").click(function () {
            $(".menu-box").slideToggle(400);
        });

        //显示任务过滤类型
        $(".btn-task-filtertype").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $(".task-filtertype").slideDown();
            } else {
                _this.removeClass("hover");
                $(".task-filtertype").slideUp();
            }            
        });

        //任务过滤类型切换
        $(".task-filtertype li").click(function () {
            $(".btn-task-filtertype div:first").text($(this).text());
            $(this).parent().hide();
            Params.pageIndex = 1;
            Params.clientid = $(this).data("id");
            //if ($(this).data("id") == '-1') {
            //    var id = "";
            //    $(".task-filtertype li").each(function () {
            //        var _this = $(this);
            //        if (_this.data("id") != '-1') {
            //            if (_this.index() != $(".task-filtertype li").length - 1) {
            //                id += _this.data("id") + '\',\'';
            //            } else {
            //                id += _this.data("id");
            //            }
            //        }
            //    });
            //    Params.clientid = id;
            //}
            ObjectJS.getList();
        });

        //列表排序
        $(".sort-item").click(function () {
            var _self = $(this);
            if (!_self.hasClass("hover")) {
                _self.addClass("hover").siblings().removeClass("hover");
            }
            var isasc = _self.data("isasc");
            var isactive = _self.attr("data-isactive");
            var orderbycloumn = _self.data("orderbycloumn");
            $(".filter-list div[data-isactive='1']").data("isactive", 0).find("span").removeClass("hover");
            if (isactive == 1) {
                if (isasc == 1) {
                    _self.find(".asc").removeClass("hover");
                    _self.find(".desc").addClass("hover");
                } else {
                    _self.find(".desc").removeClass("hover");
                    _self.find(".asc").addClass("hover");
                }
                isasc = isasc == 1 ? 0 : 1;
            } else {
                if (isasc == 1) {
                    _self.find(".desc").removeClass("hover");
                    _self.find(".asc").addClass("hover");
                } else {
                    _self.find(".asc").removeClass("hover");
                    _self.find(".desc").addClass("hover");
                }
            }
            _self.data("isasc", isasc).attr("data-isactive", 1);

            Params.orderBy = orderbycloumn + (isasc == 1 ? " asc" : " desc");
            Params.pageIndex = 1;
            ObjectJS.getList();
        });
        
        //返回顶部
        $(".getback").click(function () {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        });

        $("#beginPrice,#endPrice").change(function () {
            var _this = $(this);
            if (!_this.val().isDouble()) {
                _this.val('');
                return false;
            }
        });

        //显示选择分类弹出层
        $(".show-category").click(function () {
            $(".filter-object").show(1);
            $(".layer-body").fadeIn(400);
            setTimeout(function () {
                $(".filter-object").addClass('outlayer');
            }, 10);
        });

        /*确定筛选条件*/
        $(".confirm-fliter").click(function () {
            Params.categoryID = $(".category-box .category-block.hover").eq($(".category-box .category-block.hover").length - 1).data('id') || "";
            Params.pageIndex = 1;
            Params.beginPrice = $("#beginPrice").val();
            Params.endPrice = $("#endPrice").val();
            ObjectJS.getList();
            $(".layer-body").fadeOut(400);
            $(".filter-object").removeClass('outlayer');
            setTimeout(function () {
                $(".filter-object").hide();
            }, 410);
        });

        /*重置筛选条件*/
        $(".confirm-reset").click(function () {
            $("#beginPrice").val('');
            $("#endPrice").val('');
            $(".layer-box").eq(1).nextAll().remove();
            $(".layer-box .category-block").removeClass('hover');

            Params.categoryID = "";
            Params.pageIndex = 1;
            Params.beginPrice = "";
            Params.endPrice = "";
            ObjectJS.getList();
            $(".layer-body").fadeOut(400);
            $(".filter-object").removeClass('outlayer');
            setTimeout(function () {
                $(".filter-object").hide();
            }, 410);
        });
    };

    ObjectJS.getList = function () {
        if (Params.pageIndex == 1) {
            $(".list").empty();
        }
        $(".list").append('<div class="data-loading"></div>');
        var template = "m/template/style/style-list.html";
        var control = "/Mall/Store/GetProduct";
        ObjectJS.IsLoading = true;
        $.post(control, Params, function (data) {
            $(".data-loading").remove();
            
            if (data.items.length == 0) {
                $(".list").append("<div class='nodata-txt'>暂无数据 !</div>");
            } else {
                doT.exec(template, function (code) {
                    data.items.clientID = Params.clientid;
                    var innerHtml = code(data.items);
                    innerHtml = $(innerHtml);
                    $(".list").append(innerHtml);

                    innerHtml.find(".btn-addOrder").click(function () {
                        var _this = $(this);
                        if (!IsShowOrder) {
                            var id = _this.data('orderid');
                            if (!CacheOrder[id]) {
                                IsShowOrder = true;
                                Global.post("/M/IntFactoryOrders/GetOrderDetail", {
                                    orderID: id,
                                }, function (data) {
                                    IsShowOrder = false;
                                    if (data.result) {
                                        CacheOrder[id] = data.result;
                                        Common.showOrderGoodsLayer(CacheOrder[id], ObjectJS.user);
                                    } else {
                                        alert("请重新尝试", 2);
                                    }
                                });
                            } else {
                                Common.showOrderGoodsLayer(CacheOrder[id], ObjectJS.user);
                            }
                        } else {
                            alert("正在加载，请稍等", 2);
                        }
                    });

                    //延迟加载图片
                    $(".list-img").each(function () {
                        var _this = $(this);
                        setTimeout(function () {
                            _this.attr("src", _this.data("src") + "?imageView2/1/w/120/h/120");
                        }, 1000)
                    });
                });
                ObjectJS.PageCount = data.pageCount;
                ObjectJS.IsLoading = false;
            }
        });
    };
    
    ObjectJS.getAllCategory = function () {
        var _self = this;
        $(".category-box").append("<div class='data-loading'></div>");
        Global.post("/Mall/Store/GetEdjCateGory", null, function (data) {
            $(".category-box .data-loading").remove();
            if (data.items.length > 0) {
                CacheCategory = data.items;
                for (var i = 0; i < data.items.length; i++) {
                    var item = data.items[i];
                    CacheChildCategory[item.CategoryID] = item;
                    var obj = $("<li class='category-block' data-id='" + item.CategoryID + "' layer='" + item.Layers + "'>" + item.CategoryName + "</li>");
                    obj.click(function () {
                        var _this = $(this);
                        var _thisParent = _this.parents('.layer-box');
                        _this.addClass('hover');
                        _this.siblings().removeClass('hover');
                        _thisParent.nextAll().remove();
                        if (CacheChildCategory[_this.data('id')]) {
                            var item = CacheChildCategory[_this.data('id')];
                            if (item.ChildCategorys.length > 0) {
                                _self.createCategory(_this.data('id'), _thisParent);
                            }
                        }
                    });
                    $(".category-box").append(obj);
                }
            } else {
                $(".category-box").append("<div class='nodata-txt' >暂无分类</div>");
            }
        });
    }

    ObjectJS.createCategory = function (id, parentObj) {
        if (CacheChildCategory[id]) {
            var item = CacheChildCategory[id];
            if (item.ChildCategorys.length > 0) {
                var _html = $('<div class="layer-box" style="display:table;"></div>'),
                    _htmlTitle = $('<div class="mLeft5 mBottom10" data-id="' + id + '">' + item.CategoryName + '：</div>'),
                    _htmlChildItems = $('<div></div>'),
                    _htmlChildBox = $('<ul class="category-box row"></ul>');

                for (var j = 0; j < item.ChildCategorys.length; j++) {
                    var _child = item.ChildCategorys[j];
                    CacheChildCategory[_child.CategoryID] = _child;
                    var _obj = $("<li class='category-block' data-id='" + _child.CategoryID + "' layer='" + _child.Layers + "'>" + _child.CategoryName + "</li>");
                    _obj.click(function () {
                        var _this = $(this);
                        var _thisParent = _this.parents('.layer-box');
                        _this.addClass('hover');
                        _this.siblings().removeClass('hover');
                        _thisParent.nextAll().remove();
                        if (CacheChildCategory[_this.data('id')]) {
                            var item = CacheChildCategory[_this.data('id')];
                            if (item.ChildCategorys.length > 0) {
                                ObjectJS.createCategory(_this.data('id'), _thisParent);
                            }
                        }
                    });
                    _htmlChildBox.append(_obj);
                }
                _htmlChildItems.append(_htmlChildBox)
                _html.append(_htmlTitle).append(_htmlChildItems);
                parentObj.after(_html);
            }
        }
    };

    module.exports = ObjectJS;
});