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
                        doT.exec("m/template/style/style-buy.html", function (code) {
                            var innerHtml = code({});
                            $(".overlay-addOrder").html(innerHtml).show();
                            $(".overlay-addOrder .style-content").animate({ height: "450px" }, 200);

                            $(".overlay-addOrder").click(function (e) {
                                if (!$(e.target).parents().hasClass("style-content") && !$(e.target).hasClass("style-content")) {
                                    $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                                        $(".overlay-addOrder").hide();
                                    });
                                }
                            });

                            $(".overlay-addOrder .close").click(function () {
                                $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200,function(){
                                    $(".overlay-addOrder").hide();
                                });
                            });

                            $(".overlay-addOrder .attr-ul li").click(function () {
                                var _self = $(this);
                                if (_self.hasClass("select")) {
                                    _self.removeClass("select");
                                } else {
                                    _self.addClass("select");
                                }
                            });
                        });
                       
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