define(function (require, exports, module) {
    var Global = require("global"),
        Upload = require("upload"),
        doT = require("dot");
    var Common = require("/modules/m/scripts/style/createordergoods.js");

    var ObjectJS = {};
    ObjectJS.init = function (orderImagesCount, order,user) {
        ObjectJS.orderImagesCount = orderImagesCount;
        var jsonOrder = JSON.parse(order.replace(/&quot;/g, '"'));
        ObjectJS.order = jsonOrder;
        ObjectJS.user = JSON.parse(user.replace(/&quot;/g, '"'));

        ObjectJS.bindEvent();

        //设置图片显示宽高
        $(".pic-list li").css({ "margin-right": "10px", "border": "1px solid #ccc" });
        $(".pic-list .pic-box img").css({ "width": "100%", "height": "200px" });
        $(".platemakingBody table tr td:last-child").remove();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        if (ObjectJS.orderImagesCount > 0) {
            if (ObjectJS.orderImagesCount > 1) {
                $(".main_image").touchSlider({
                    flexible: true,
                    speed: 200,
                    paging: $(".flicking_con a"),
                    counter: function (e) {
                        $(".flicking_con a").removeClass("on").eq(e.current - 1).addClass("on");
                    }
                });
            }

            ObjectJS.setImagesSize();
        }
       
        //菜单切换模块事件
        $("nav ul li").click(function () {
            var _this = $(this);
            _this.addClass("menuchecked").siblings().removeClass("menuchecked");
            _this.parent().parent().find("i").css("color", "#9e9e9e");
            _this.find("i").css("color", "#4a98e7");
            var classname = _this.data("classname");
            ModuleType = classname;
            $(".main-box ." + classname).show().siblings().hide();
            var isGet = _this.data("isget");
            if (classname == "plate-des") {
                if (!isGet) {
                    ObjectJS.getPlateMakings();
                    _this.data("isget", "1");
                }
            }

            ////讨论
            //if (classname == "talk-status") {
            //    if (!isGet) {
            //        ObjectJS.getTaskReplys();
            //        _this.data("isget", "1");
            //    }
            //}
            ////材料
            //else if (classname == "shop-status") {
            //    if (!isGet) {
            //        ObjectJS.GetOrderDetailsByOrderID();
            //        _this.data("isget", "1");
            //    }
            //}
            ////工艺说明
            //else if (classname == "print-status") {
            //    if (!isGet) {
            //        ObjectJS.getPlateMakings();
            //        _this.data("isget", "1");
            //    }
            //}
            ////日志
            //else if (classname == "log-status") {
            //    if (!isGet) {
            //        ObjectJS.getTaskLogs();
            //        _this.data("isget", "1");
            //    }
                    
            //}

        });
        //点击回到顶部
        $(".getback").click(function () {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        });
        //下单
        $(".btn-addOrder").click(function () {
            Common.showOrderGoodsLayer(ObjectJS.order, ObjectJS.user);
        });
    }

    //设置图片宽高
    ObjectJS.setImagesSize = function () {
        var windowWidth = $(window).width();
        $(".main_image").css({ "height": windowWidth + "px", "width": windowWidth + "px" });
        $(".main_image ul li").css({ "height": windowWidth + "px", "width": windowWidth + "px" });

        $(".main_image ul li").each(function () {
            if ($(this).find('img').width() > $(this).find('img').height()) {
                $(this).find('img').css("height", windowWidth + "px");
            } else {
                $(this).find('img').css("width", windowWidth + "px");
            }
        });
    }

    //获取任务详情日志列表
    ObjectJS.getTaskLogs = function () {
        if ($(".log-status").data('isLoading') != 1) {
            if (LogPageCount >= Paras.logPageIndex) {
                $(".log-status").append('<div class="data-loading"></div>');
                $(".log-status").data('isLoading', 1);
                $.post("/Task/GetLogInfo", Paras, function (data) {
                    $(".log-status").data('isLoading', 0);
                    $(".log-status .data-loading").remove();
                    LogPageCount = data.pagecount;
                    if (LogPageCount == 0) {
                        $(".log-status").html("<div class='nodata-txt'>暂无数据</div>");
                    }
                    else {
                        doT.exec("template/task/task-log.html", function (templateFun) {
                            var items = data.items;
                            var innerText = templateFun(items);
                            $('.log-status').append(innerText);
                        });
                    }
                })
            } else {
                if ($(".log-status .log-box").length > 0 && $(".alert-lastlogpage").length == 0) {
                    $(".main-box .log-status").append("<div class='alert-lastlogpage center mTop10 color999'>已经是最后一条啦</div>");
                }
            }
        }
    }

    //获取制版工艺说明
    ObjectJS.getPlateMakings = function () {
        $(".tb-plates").html('');
        var items = ObjectJS.order.plateMakings;
        if (items.length > 0) {
            doT.exec("m/template/order/platematrings.html", function (template) {
                var html = template(items);
                html = $(html);
                html.find('.doc-header').click(function () {
                    var _this = $(this);
                    if (!_this.next().is(":animated")) {
                        if (!_this.hasClass('hover')) {
                            _this.addClass('hover');
                            _this.find('.lump').addClass('hover');
                            _this.next().slideDown(400, function () {
                            });
                        } else {
                            _this.removeClass('hover');
                            _this.find('.lump').removeClass('hover');
                            _this.next().slideUp(400, function () {

                            });
                        }
                    }
                });
                $(".tb-plates").append(html);
            });
        }
        else {
            $(".tb-plates").append("<div class='nodata-txt'>暂无工艺说明<div>");
        }
    };

    module.exports = ObjectJS;
});
