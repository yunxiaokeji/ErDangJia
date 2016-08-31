define(function (require,exports,module) {
    var Global = require("global"),
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
    ObjectJS.init = function () {
        ObjectJS.bindEvent();
        ObjectJS.getList();
        //ObjectJS.getTaskLableColors();        
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

    //绑定时间控件
    ObjectJS.bindTimerPicker = function () {        
        var defaultParas = {
            preset: 'datetime',
            theme: 'android-ics light', //皮肤样式
            display: 'modal', //显示方式 
            mode: 'scroller', //日期选择模式
            lang: 'zh',
            onSelect: function (date) {
                taskParms.endTime = date;
                var _this = $(this);
                taskParms.taskID = _this.data("id");
                ObjectJS.showConfirmForm(0);
                _this.val("接受任务");
            }
        };
        $(".btn-acceptTaskTime").mobiscroll().datetime(defaultParas);
    }

    //设置任务到期时间
    ObjectJS.setTaskEndTime = function () {
        Global.post("/Task/UpdateTaskEndTime", taskParms, function (data) {
            if (data == 1) {                
                $("#btnAcceptTaskTime_" + taskParms.taskID).remove();
                $("#iconFontDetails_" + taskParms.taskID).html("&#xe621;").addClass("color333");
            } else if (data == 0) {
                alert("失败",2);
            } else if (data == 2) {
                alert("有前面阶段任务未完成", 2);
            } else if (data == 3) {
                alert("没有权限", 2);
            } else if (data == 4) {
                alert("任务没有接受，不能设置完成", 2);
            } else if (data == 5) {
                alert("任务有未完成步骤", 2);
            }
        });
    }

    //标记完成任务
    ObjectJS.finishTask = function () {
        Global.post("/Task/FinishTask", taskParms, function (data) {
            if (data == 1) {                
                $("#btnFinishTask_" + taskParms.taskID).remove();
                $("#iconFontDetails_" + taskParms.taskID).html("&#xe61f;");
            } else if (data == 0) {
                alert("失败", 2);
            } else if (data == 2) {
                alert("有前面阶段任务未完成", 2);
            } else if (data == 3) {
                alert("没有权限", 2);
            } else if (data == 4) {
                alert("任务没有接受，不能设置完成", 2);
            } else if (data == 5) {
                alert("任务有未完成步骤", 2);
            }
        });
    }

    //接受任务、标记任务完成的弹出浮层
    ObjectJS.showConfirmForm = function (showStatus) {
        var alertMsg = showStatus == 0 ? "任务到期时间不可逆,确定设置?" : "标记完成的任务不可逆,确定设置?";
        confirm(alertMsg, function () {
            if (showStatus == 0) {
                ObjectJS.setTaskEndTime();
            } else {
                ObjectJS.finishTask();
            }
        }, "设置");
    }

    ObjectJS.getList = function (noEmpty) {
        ObjectJS.IsLoading = true;
        if (!noEmpty) {
            $(".list").empty();
        }
        //获取任务列表(页面加载)
        $(".list").append('<div class="data-loading"></div>');
        $.post("/IntFactoryOrder/GetProductList", Params, function (data) {
            //获取用户名
            $(".login-name").text(data.userName);
            //判断有无数据
            if (data.items.length == 0) {
                $(".list").append("<div class='nodata-txt'>暂无数据 !</div>");
            } else {
                //分页数据
                ObjectJS.PageCount = data.pageCount;
                //引用doT模板
                doT.exec("m/template/task/task-list.html", function (code) {
                    var $result = code(data.items);                        
                    $(".list").append($result);
                   
                    if (Params.filtertype!=1) {
                        $(".btn-acceptTaskTime").hide();
                        $(".btn-finishTask").hide();
                    }

                    //延迟加载图片
                    $(".task-list-img").each(function () {
                        var _this = $(this);
                        setTimeout(function () {
                            _this.attr("src", _this.data("src") + "?imageView2/1/w/120/h/120");
                        }, 1000)
                    });

                });
                
                ObjectJS.IsLoading = false;
            }
            $(".data-loading").remove();
        });
    };

    ObjectJS.getTaskLableColors=function(){
        $.post("/Task/GetTaskLableColors", null, function (data) {
            data = JSON.parse(data);
            var items = data.items;
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var html = '';
                html += '<li data-id="' + item.ColorID + '"><span class="lable-color" style="background-color:' + item.ColorValue + '"></span><span>' + item.ColorName + '</span></li>';

                $(".task-colormark").append(html);
            }
            //任务颜色标记切换
            $(".task-colormark li").click(function () {
                $(".colormark-span").text($(this).text());
                $(this).parent().hide();

                Params.pageIndex = 1;
                Params.colorMark = $(this).data("id");
                ObjectJS.getList();
            });
            
        });
    };

    module.exports = ObjectJS;
});