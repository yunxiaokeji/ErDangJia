define(function (require,exports,module) {
    var Common = require("/modules/m/scripts/style/createordergoods.js");
    var Global = require("m_global"),
        doT = require("dot");
    var Params = {
        clientid: "",/*款式中供应商ID*/
        pageSize: 5,
        pageIndex: 1,
        keyWords: "",
        providerid: ""/*订单方法中供应商ID*/,
        sourcetype: 2,
        totalCount: 0
    };
    
    var taskParms = {
        taskID: "",
        replyPageIndex: 1,
        logPageIndex: 1,
        endTime: ""
    }

    var ObjectJS = {};
    var cacheOrder = [];
    var isShowOrder = false;

    ObjectJS.PageCount = 0;
    ObjectJS.IsLoading = false;
    ObjectJS.init = function (providers,user) {
        providers = JSON.parse(providers.replace(/&quot;/g, '"'));
        user = JSON.parse(user.replace(/&quot;/g, '"'));
        ObjectJS.user = user;
        var providerids = "";
        for (var i = 0; i < providers.length; i++) {              
            var p = providers[i];
            if (p.CMClientID != "" && p.CMClientID != null) {
                providerids += "'" + p.CMClientID + "',";
                $(".task-filtertype").append('<li data-providerid="' + p.ProviderID + '" data-id="' + p.CMClientID + '">' + p.Name + '</li>');
            }           
        }
        if (providerids != "") {
            providerids = providerids.substring(1, providerids.length - 2);
        } else {
            providerids = "1";
        }
        Params.clientid = providerids;
        ObjectJS.bindEvent();
        ObjectJS.getList();  
    };

    ObjectJS.bindEvent = function () {
        //滚动加载数据
        $(window).scroll(function () {
            if ($(".overlay-addOrder").css('display') == 'none') {
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
            Params.pageIndex = 1;
            Params.clientid = $(this).data("id");
            Params.providerid = $(this).data("providerid");

            if ($(this).data("id") == '-1') {
                var id = "";
                $(".task-filtertype li").each(function () {
                    var _this = $(this);
                    if (_this.data("id") != '-1') {
                        if (_this.index() != $(".task-filtertype li").length - 1) {
                            id += _this.data("id") + '\',\'';
                        } else {
                            id += _this.data("id");
                        }
                    }
                });
                Params.clientid = id;
                Params.providerid = "";
            }
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
        var template = "m/template/style/style-list.html";
        var control = "/IntFactoryOrder/GetProductList";
        if ($(".task-status li.hover").data('status') == 1) {
            template = "m/template/style/order-list.html";
            control = "/Purchase/GetPurchases";
        }

        $.post(control, Params, function (data) {
            $(".data-loading").remove();
            if (data.items.length == 0) {
                $(".list").append("<div class='nodata-txt'>暂无数据 !</div>");
            } else {
                ObjectJS.PageCount = data.pageCount || data.PageCount;
                doT.exec(template, function (code) {
                    var innerHtml = code(data.items);
                    innerHtml = $(innerHtml);
                    $(".list").append(innerHtml);

                    innerHtml.find(".btn-addOrder").click(function () {
                        var _this = $(this);
                        if (!isShowOrder) {
                            var id = _this.data('orderid');
                            if (!cacheOrder[id]) {
                                isShowOrder = true;
                                Global.post("../IntFactoryOrders/GetOrderDetail", {
                                    orderID: id,
                                    clientID: _this.data('clientid')
                                }, function (data) {
                                    isShowOrder = false;
                                    if (data.result.error_code == 0) {
                                        cacheOrder[id] = data.result.order;
                                        Common.showOrderGoodsLayer(cacheOrder[id], ObjectJS.user);
                                    } else {
                                        alert("请重新尝试");
                                    }
                                });
                            } else {
                                Common.showOrderGoodsLayer(cacheOrder[id], ObjectJS.user);
                            }
                        } else {
                            alert("正在加载，清稍等");
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