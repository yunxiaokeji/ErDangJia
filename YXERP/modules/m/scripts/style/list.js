define(function (require,exports,module) {
    var Global = require("m_global"),
        doT = require("dot");

    var Params = {
        pageSize: 5,
        pageIndex: 1        
    };
    
    var taskParms = {
        taskID: "",
        replyPageIndex: 1,
        logPageIndex: 1,
        endTime: ""
    }

    var ObjectJS = {};
    var cacheOrder = [];
    ObjectJS.PageCount = 0;
    ObjectJS.IsLoading = false;
    ObjectJS.init = function (providers) {
        providers = JSON.parse(providers.replace(/&quot;/g, '"'));
        for (var i = 0; i < providers.length; i++) {
            var p=providers[i];
            $(".task-filtertype").append('<li data-id="' + p.ProviderID + '">' + p.Name + '</li>');
        }
        ObjectJS.bindEvent();
        ObjectJS.getList();  
    };

    ObjectJS.bindEvent = function () {
        //滚动加载数据
        $(window).scroll(function () {
            if (document.body.scrollTop > 30) {
                $(".getback").slideDown("slow");
            } else {
                $(".getback").slideUp("slow");
            }
            var bottom = $(document).height() - document.documentElement.scrollTop - document.body.scrollTop - $(window).height();
            if (bottom <= 200) {
                if (!ObjectJS.IsLoading) {
                    Params.pageIndex++;
                    if (Params.pageIndex <= ObjectJS.PageCount) {
                        ObjectJS.getList(true);
                    } else {
                        $(".prompt").remove();
                        $(".list").append('<div class="prompt">已经是最后一条啦</div>');
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

            Params.filtertype = $(this).data("filtertype");
            ObjectJS.getList();
        });

        //任务状态切换
        $(".task-status li").click(function () {
            
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover").siblings().removeClass("hover");
            }

            Params.pageIndex = 1;
            Params.finishStatus = $(this).data("status");
            ObjectJS.getList();
        });

        //显示过滤下拉框
        $(".filter-task li").click(function () {
            var slideLi = $("." + $(this).data("id"));
            slideLi.slideToggle(400).siblings().slideUp("slow");
        });

        //订单类型切换
        $(".order-type li").click(function () {
            $(".type-span").text($(this).text());
            $(this).parent().hide();

            Params.pageIndex = 1;
            Params.orderType = $(this).data("id");
            ObjectJS.getList();
        });

        //任务排序
        $(".task-sort li").click(function () {
            $(".sort-span").text($(this).text());
            $(this).parent().hide();
            
            Params.isAsc = $(this).data("takepo");
            Params.taskOrderColumn = $(this).data("id");
            ObjectJS.getList();
        });
        
        //返回顶部
        $(".getback").click(function () {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        });
    };

    ObjectJS.getList = function (noEmpty) {
        if (!noEmpty) {
            $(".list").empty();
        }
        //获取任务列表(页面加载)
        $(".list").append('<div class="data-loading"></div>');
        $.post("/IntFactoryOrder/GetProductList", Params, function (data) {
            $(".data-loading").remove();
            if (data.items.length == 0) {
                $(".list").append("<div class='nodata-txt'>暂无数据 !</div>");
            } else {
                ObjectJS.PageCount = data.pageCount;
                doT.exec("m/template/style/style-list.html", function (code) {
                    var innerHtml = code(data.items);
                    innerHtml = $(innerHtml);
                    $(".list").append(innerHtml);

                    innerHtml.find(".btn-addOrder").click(function () {
                        var _this = $(this);
                        var id = _this.data('orderid');
                        if (!cacheOrder[id]) {
                            Global.post("../IntFactoryOrders/GetOrderDetail", {
                                orderID: id,
                                clientID: _this.data('clientid')
                            }, function (data) {
                                if (data.result.error_code == 0) {
                                    cacheOrder[id] = data.result.order;
                                    doT.exec("m/template/style/style-buy.html", function (code) {
                                        var innerHtml = code(cacheOrder[id]);
                                        $(".overlay-addOrder").html(innerHtml).show();
                                        var height = $(document).height();
                                        $(".overlay-addOrder").css("height", height + "px");
                                        $(document).scrollTop(height - $(window).height());
                                        $(".overlay-addOrder .style-content").animate({ height: "450px" }, 200);

                                        $(".overlay-addOrder").unbind().click(function (e) {
                                            if (!$(e.target).parents().hasClass("style-content") && !$(e.target).hasClass("style-content")) {
                                                $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                                                    $(".overlay-addOrder").hide();
                                                });
                                            }
                                        });

                                        $(".overlay-addOrder .close").unbind().click(function () {
                                            $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                                                $(".overlay-addOrder").hide();
                                            });
                                        });

                                        $(".overlay-addOrder .attr-ul li").unbind().click(function () {
                                            var _self = $(this);
                                            if (_self.hasClass("select")) {
                                                _self.removeClass("select");
                                            } else {
                                                _self.addClass("select");
                                            }
                                            var bl = false, details = [], isFirst = true, xattr = [], yattr = [];
                                            $(".productsalesattr").each(function () {
                                                bl = false;
                                                var _attr = $(this), attrdetail = details;
                                                //组合规格
                                                _attr.find(".attr-ul ul li.select").each(function () {
                                                    bl = true;
                                                    var _value = $(this);
                                                    //首个规格
                                                    if (isFirst) {
                                                        var model = {};
                                                        model.ids = _attr.data("id") + ":" + _value.data("id");
                                                        model.saleAttr = _attr.data("id");
                                                        model.attrValue = _value.data("id");
                                                        model.xRemark = _value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "";
                                                        model.yRemark = _value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "";
                                                        model.xyRemark = "【" + _value.data("text") + "】";
                                                        model.names = "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                                        model.layer = 1;
                                                        details.push(model);
                                                    } else {
                                                        for (var i = 0, j = attrdetail.length; i < j; i++) {
                                                            if (attrdetail[i].ids.indexOf(_value.data("attrid")) < 0) {
                                                                var model = {};
                                                                model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.data("id");
                                                                model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                                                model.attrValue = attrdetail[i].attrValue + "," + _value.data("id");
                                                                model.xRemark = attrdetail[i].xRemark + (_value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "");
                                                                model.yRemark = attrdetail[i].yRemark + (_value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "");
                                                                model.xyRemark = attrdetail[i].xyRemark + "【" + _value.data("text") + "】";
                                                                model.names = attrdetail[i].names + "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                                                model.layer = attrdetail[i].layer + 1;
                                                                details.push(model);
                                                            }
                                                        }
                                                    }
                                                    //处理二维表
                                                    if (_value.data("type") == 1 && xattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                                        xattr.push("【" + _value.data("text") + "】");
                                                    } else if (_value.data("type") == 2 && yattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                                        yattr.push("【" + _value.data("text") + "】");
                                                    }
                                                });
                                                isFirst = false;
                                            });
                                            $(".attr-box").empty();
                                            //选择所有属性
                                            if (bl) {
                                                var layer = $(".productsalesattr").length, items = [];
                                                for (var i = 0, j = details.length; i < j; i++) {
                                                    var model = details[i];
                                                    if (model.layer == layer) {
                                                        items.push(model);
                                                        CacheItems[model.xyRemark] = model;
                                                    }
                                                }
                                                var tableModel = {};
                                                tableModel.xAttr = xattr;
                                                tableModel.yAttr = yattr;
                                                tableModel.items = items;

                                                //加载子产品
                                                doT.exec("template/default/orders_child_list.html", function (templateFun) {
                                                    var innerText = templateFun(tableModel);
                                                    innerText = $(innerText);
                                                    $(".attr-box").append(innerText);
                                                    //数量必须大于0的数字
                                                    innerText.find(".quantity").change(function () {
                                                        var _this = $(this);
                                                        if (!_this.val().isInt() || _this.val() <= 0) {
                                                            _this.val("0");
                                                        }

                                                        var total = 0;
                                                        $(".child-product-table .tr-item").each(function () {
                                                            var _tr = $(this), totaly = 0;
                                                            if (!_tr.hasClass("total")) {
                                                                _tr.find(".quantity").each(function () {
                                                                    var _this = $(this);
                                                                    if (_this.val() > 0) {
                                                                        totaly += _this.val() * 1;
                                                                    }
                                                                });
                                                                _tr.find(".total-y").text(totaly);
                                                            } else {
                                                                _tr.find(".total-y").each(function () {
                                                                    var _td = $(this), totalx = 0;
                                                                    $(".child-product-table .quantity[data-x='" + _td.data("x") + "']").each(function () {
                                                                        var _this = $(this);
                                                                        if (_this.val() > 0) {
                                                                            totalx += _this.val() * 1;
                                                                        }
                                                                    });
                                                                    total += totalx;
                                                                    _td.text(totalx);
                                                                });
                                                                _tr.find(".total-xy").text(total);
                                                            }
                                                        });

                                                    });
                                                    //if (innerText.find(".quantity").length > 0) {
                                                    //    $('#btnSubmit').show();
                                                    //} else {
                                                    //    $('#btnSubmit').hide();
                                                    //}
                                                });
                                            }
                                        });
                                    });
                                } else {
                                    alert("请重新尝试");
                                }
                            });
                        } else {
                            doT.exec("m/template/style/style-buy.html", function (code) {
                                var innerHtml = code(cacheOrder[id]);
                                $(".overlay-addOrder").html(innerHtml).show();
                                $(".overlay-addOrder .style-content").animate({ height: "450px" }, 200);

                                $(".overlay-addOrder").unbind().click(function (e) {
                                    if (!$(e.target).parents().hasClass("style-content") && !$(e.target).hasClass("style-content")) {
                                        $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                                            $(".overlay-addOrder").hide();
                                        });
                                    }
                                });
                                $(".overlay-addOrder .close").unbind().click(function () {
                                    $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                                        $(".overlay-addOrder").hide();
                                    });
                                });
                                $(".overlay-addOrder .attr-ul li").unbind().click(function () {
                                    var _self = $(this);
                                    if (_self.hasClass("select")) {
                                        _self.removeClass("select");
                                    } else {
                                        _self.addClass("select");
                                    }

                                    var bl = false, details = [], isFirst = true, xattr = [], yattr = [];
                                    $(".productsalesattr").each(function () {
                                        bl = false;
                                        var _attr = $(this), attrdetail = details;
                                        //组合规格
                                        _attr.find("li.hover").each(function () {
                                            bl = true;
                                            var _value = $(this);
                                            //首个规格
                                            if (isFirst) {
                                                var model = {};
                                                model.ids = _attr.data("id") + ":" + _value.data("id");
                                                model.saleAttr = _attr.data("id");
                                                model.attrValue = _value.data("id");
                                                model.xRemark = _value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "";
                                                model.yRemark = _value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "";
                                                model.xyRemark = "【" + _value.data("text") + "】";
                                                model.names = "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                                model.layer = 1;
                                                details.push(model);
                                            } else {
                                                for (var i = 0, j = attrdetail.length; i < j; i++) {
                                                    if (attrdetail[i].ids.indexOf(_value.data("attrid")) < 0) {
                                                        var model = {};
                                                        model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.data("id");
                                                        model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                                        model.attrValue = attrdetail[i].attrValue + "," + _value.data("id");
                                                        model.xRemark = attrdetail[i].xRemark + (_value.data("type") == 1 ? ("【" + _value.data("text") + "】") : "");
                                                        model.yRemark = attrdetail[i].yRemark + (_value.data("type") == 2 ? ("【" + _value.data("text") + "】") : "");
                                                        model.xyRemark = attrdetail[i].xyRemark + "【" + _value.data("text") + "】";
                                                        model.names = attrdetail[i].names + "【" + _attr.data("text") + "：" + _value.data("text") + "】";
                                                        model.layer = attrdetail[i].layer + 1;
                                                        details.push(model);
                                                    }
                                                }
                                            }
                                            //处理二维表
                                            if (_value.data("type") == 1 && xattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                                xattr.push("【" + _value.data("text") + "】");
                                            } else if (_value.data("type") == 2 && yattr.indexOf("【" + _value.data("text") + "】") < 0) {
                                                yattr.push("【" + _value.data("text") + "】");
                                            }
                                        });
                                        isFirst = false;
                                    });
                                    $(".attr-box").empty();
                                    //选择所有属性
                                    if (bl) {
                                        var layer = $(".productsalesattr").length, items = [];
                                        for (var i = 0, j = details.length; i < j; i++) {
                                            var model = details[i];
                                            if (model.layer == layer) {
                                                items.push(model);
                                                CacheItems[model.xyRemark] = model;
                                            }
                                        }
                                        var tableModel = {};
                                        tableModel.xAttr = xattr;
                                        tableModel.yAttr = yattr;
                                        tableModel.items = items;

                                        //加载子产品
                                        doT.exec("template/default/orders_child_list.html", function (templateFun) {
                                            var innerText = templateFun(tableModel);
                                            innerText = $(innerText);
                                            $(".attr-box").append(innerText);
                                            //数量必须大于0的数字
                                            innerText.find(".quantity").change(function () {
                                                var _this = $(this);
                                                if (!_this.val().isInt() || _this.val() <= 0) {
                                                    _this.val("0");
                                                }

                                                var total = 0;
                                                $(".child-product-table .tr-item").each(function () {
                                                    var _tr = $(this), totaly = 0;
                                                    if (!_tr.hasClass("total")) {
                                                        _tr.find(".quantity").each(function () {
                                                            var _this = $(this);
                                                            if (_this.val() > 0) {
                                                                totaly += _this.val() * 1;
                                                            }
                                                        });
                                                        _tr.find(".total-y").text(totaly);
                                                    } else {
                                                        _tr.find(".total-y").each(function () {
                                                            var _td = $(this), totalx = 0;
                                                            $(".child-product-table .quantity[data-x='" + _td.data("x") + "']").each(function () {
                                                                var _this = $(this);
                                                                if (_this.val() > 0) {
                                                                    totalx += _this.val() * 1;
                                                                }
                                                            });
                                                            total += totalx;
                                                            _td.text(totalx);
                                                        });
                                                        _tr.find(".total-xy").text(total);
                                                    }
                                                });

                                            });
                                            //if (innerText.find(".quantity").length > 0) {
                                            //    $('#btnSubmit').show();
                                            //} else {
                                            //    $('#btnSubmit').hide();
                                            //}
                                        });
                                    }
                                });
                            });
                        }
                    });
                    //延迟加载图片
                    $(".task-list-img").each(function () {
                        var _this = $(this);
                        setTimeout(function () {
                            _this.attr("src", _this.data("src") + "?imageView2/1/w/120/h/120");
                        }, 1000)
                    });

                });
            }
            
        });
    };

    module.exports = ObjectJS;
});