define(function (require, exports, module) {
    var Global = require("global"),
        Upload = require("upload"),
        doT = require("dot"),
        OrderGoods = require("/modules/scripts/task/ordergoods");

    var Paras = {
        orderID: "",
        taskID: "",
        replyPageIndex: 1,
        logPageIndex: 1,
        endTime: ""
    };

    var TaskReplyParas = {
        taskID: "",
        content: "",
        fromReplyID: "",
        fromReplyUserID: "",
        fromReplyAgentID: ""
    };

    var IsLoading = false;
    var ReplyPageCount = 1;
    var LogPageCount = 1;
    var ModuleType = "talk-status";
    var GetOrAddReply = "GetReply";
    var WindowScrollTop = 0;

    var ObjectJS = {};
    ObjectJS.init = function (orderImagesCount, isOwner, userID, task, userName) {
        var jsonTask = JSON.parse(task.replace(/&quot;/g, '"'));
        Paras.orderID = jsonTask.orderID;
        Paras.taskID = jsonTask.taskID;
        ObjectJS.task = jsonTask;
        ObjectJS.order = jsonTask.order;
        ObjectJS.orderImagesCount = orderImagesCount;
        ObjectJS.userID = userID;
        ObjectJS.userName = userName;
        ObjectJS.isOwner = isOwner;
        TaskReplyParas.taskID = jsonTask.taskID;

        OrderGoods.init(Global, doT);
        OrderGoods.orderid = jsonTask.orderID;
        OrderGoods.taskid = jsonTask.taskID;

        ObjectJS.bindTimerPicker();
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
        
        var uploader = Upload.uploader({
            browse_button: 'reply-attachment',
            container: 'addition',
            drop_element: 'addition',
            file_path: "/Content/UploadFiles/Task/",
            picture_container: "pic-list",
            file_container: "doc-list",
            maxQuantity: 5,
            maxSize: 5,
            successItems: '.file li',
            image_view: "?imageView2/1/w/120/h/80",
            fileType: 3,
            init: {
            }
        });

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

            //讨论
            if (classname == "talk-status") {
                if (!isGet) {
                    ObjectJS.getTaskReplys();
                    _this.data("isget", "1");
                }
            }
            //材料
            else if (classname == "shop-status") {
                if (!isGet) {
                    ObjectJS.GetOrderDetailsByOrderID();
                    _this.data("isget", "1");
                }
            }
            //工艺说明
            else if (classname == "print-status") {
                if (!isGet) {
                    ObjectJS.getPlateMakings();
                    _this.data("isget", "1");
                }
            }
            //日志
            else if (classname == "log-status") {
                if (!isGet) {
                    ObjectJS.getTaskLogs();
                    _this.data("isget", "1");
                }
                    
            }
            //手工成本
            else if (classname === "navCosts") {
                if (!isGet) {
                    ObjectJS.getOrderCosts();
                    _this.data("isget", "1");
                }
            }
            //打样发货
            else if (classname === "navSendDYDoc") {
                if (!isGet) {
                    OrderGoods.getGetGoodsDoc(classname, 2);
                }
            }
            //裁剪
            else if (classname == "navCutoutDoc") {
                if (!isGet) {
                    $(".talk-status").data('isget', '1');
                    if ($(".task-operate-module").length > 0) {
                        ObjectJS.getOrderGoods(1);
                    }
                    OrderGoods.getGetGoodsDoc(classname, 1);
                    _this.data("isget", "1");
                }
            }
            //车缝
            else if (classname == "navSewnDoc") {
                if (!isGet) {
                    if ($(".task-operate-module").length > 0) {
                        ObjectJS.getOrderGoods(11);
                    }
                    OrderGoods.getGetGoodsDoc(classname, 11);
                    _this.data("isget", "1");
                }
            }
            //发货
            else if (classname == "navSendDoc") {
                if (!isGet) {
                    OrderGoods.getGetGoodsDoc(classname, 22);
                    _this.data("isget", "1");
                }
            }
        });
        //点击回到顶部
        $(".getback").click(function () {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        });

        //绑定完成任务
        if ($(".btn-finishTask").length > 0) {
            if ($('.btn-finishTask').val() == "标记完成") {
                if (ObjectJS.isOwner) {
                    $(".btn-finishTask").click(function () {
                        ObjectJS.showConfirmForm(1);
                    });
                }
            }
        }

        //锁定任务
        if ($("#lockTask").length > 0) {
            $("#lockTask").click(function () {
                var _this = $(this);
                confirm("锁定后不能对任务进行任何操作,确定要锁定?", function () {
                    _this.val('锁定中...').attr('disablebed', 'disabled');
                    Global.post("/Task/LockTask", { taskID: Paras.taskID }, function (data) {;
                        if (data.result == 1) {
                            $(".task-operate-module").remove();
                            $(".show-goods").parent().remove();
                            _this.parent().html('<span>已完成</span>');
                        } else {
                            _this.val("锁定任务").removeAttr("disabled");
                            alert("网络繁忙,解锁失败",2);
                        }
                    });
                });
            });
        }
        //关闭任务讨论浮层
        $(".cancel-reply").click(function () {
            $("body,html").removeClass('layer');
            $(".reply-layer").removeClass('show');
            setTimeout(function () {
                $(".reply-layer").hide();
                $('body,html').animate({ scrollTop: WindowScrollTop }, 100);
            }, 800);
        });

        //发表任务讨论
        $(".finish-reply").click(function () {
            GetOrAddReply = "AddReply";
            var _this = $(this);
            //讨论内容
            var newHtml = $('.reply-layer-content').clone();
            newHtml.find('.pic-list,.doc-list').remove();
            newHtml.find('div.clear').remove();
            if (!newHtml.text().trim()) {
                alert("讨论内容不能为空！", 2);
                return false;
            }
            if ($('.task-file li').find('.mark-progress').length > 0) {
                alert("文件上传中，请稍等", 2);
                return false;
            }
            _this.html('发表中...');
            if (IsLoading) {
                alert("发表中,请稍候再试.", 2);
                return false;
            }

            var attachments = [];
            $('.task-file li').each(function () {
                var _this = $(this);
                attachments.push({
                    "ServerUrl": _this.data("server"),
                    "Type": _this.data('isimg'),
                    "FilePath": _this.data('filepath'),
                    "FileName": _this.data('filename'),
                    "OriginalName": _this.data('originalname'),
                    "Size": _this.data("filesize"),
                    "ThumbnailName": ""
                });
            });

            var divContent = "";
            if (newHtml.find('div').length > 0) {
                newHtml.find('div').each(function () {
                    var _this = $(this);
                    if (_this.index() != newHtml.find('div').length - 1) {
                        /*替换换行符*/
                        divContent += _this.text().replace(/</g, '&lt;').replace(/>/g, '&gt;') + "<br>";
                    } else {
                        /*如果是最后一个DIV则不换行*/
                        divContent += _this.text().replace(/</g, '&lt;').replace(/>/g, '&gt;');
                    }
                });
                newHtml.find('div').remove();
                /*替换换行符*/
                divContent = (newHtml.html().trim() != "" ? newHtml.html().replace(/</g, '&lt;').replace(/>/g, '&gt;') + "<br>" : "") + divContent;
            } else {
                divContent = newHtml.html().replace(/</g, '&lt;').replace(/>/g, '&gt;');
            }
            TaskReplyParas.content = divContent;
            var msgReply = JSON.stringify(TaskReplyParas);

            IsLoading = true;
            $.post("/Task/AddTaskReply", {
                resultReply: msgReply,
                entityAttachments: JSON.stringify(attachments)
            }, function (data) {
                IsLoading = false;
                ObjectJS.GetOrAddTaskReply(data, GetOrAddReply);

                _this.html('发表');
                $('.reply-layer-content').html('');
                $("body,html").removeClass('layer');
                $(".reply-layer").removeClass('show');
                setTimeout(function () { $(".reply-layer").hide(); }, 500);
            });
        });

        //点击发表讨论出现弹出层
        $(".txt-talkcontent").click(function () {
            if (IsLoading) {
                alert("上一条评论发表中,请稍候再试.", 2);
                return false;
            }
            if ($("#pic-list").length == 0) {
                $(".reply-layer-content").append('<div class="text-content" style="display:block;min-height:40px;"></div>');
                $(".reply-layer-content").append('<ul class="pic-list task-file mTop20" id="pic-list" contenteditable="false"></ul><div class="clear"></div>');
            }
            if ($("#doc-list").length == 0) {
                $(".reply-layer-content").append('<ul class="doc-list task-file mTop20 upload-file" id="doc-list" contenteditable="false"></ul><div class="clear"></div>');
            }

            WindowScrollTop = $(document).scrollTop();
            $("body,html").addClass('layer');
            $(".reply-title").html('发表讨论');
            $(".reply-layer").show();
            setTimeout(function () { $(".reply-layer").addClass('show') }, 10);
            setTimeout(function () { $(".reply-layer-content").focus(); }, 1000);
            if ($(".reply-layer-content .text-content").length == 0) {
                $(".reply-layer-content").prepend('<div class="text-content" style="display:block;min-height:40px;"></div>');
            }

            TaskReplyParas.fromReplyID = '';
            TaskReplyParas.fromReplyUserID = '';
            TaskReplyParas.fromReplyAgentID = '';
        });

        //删除附件
        $(".reply-layer .del").click(function () {
            var _this = $(this);
            _this.parents('.file').remove();
        });

        /*显示表情浮层*/
        $(".qqface").click(function () {

        });

        ObjectJS.bindScroll();

        $("nav ul li.menuchecked").click();
    }

    //获取下单明细
    ObjectJS.getOrderGoods = function (type) {
        Global.post("/Orders/GetOrderGoods", { orderID: Paras.orderID }, function (result) {
            if (result) {
                var items = JSON.parse(result);
                var template = "template/task/task-sewn.html";/*type为11*/
                if (type == 1) {
                    template = "template/task/task-cutgoods.html";
                }
                doT.exec(template, function (templateFun) {
                    var innerHtml = templateFun(items);
                    innerHtml = $(innerHtml);
                    innerHtml.find('.add-doc').click(function () {
                        var _thisBtn = $(this);
                        if (_thisBtn.data('isSubmit') != 1) {
                            var details = "", bl = true;
                            var models = [];
                            innerHtml.find(".list-item").each(function () {
                                var _thisTr = $(this);
                                var quantity = _thisTr.find(".quantity").val();
                                if (quantity > 0) {
                                    if (type == 11) {
                                        if (quantity > (_thisTr.find(".cut-quantity").text() * 1) - (_thisTr.find(".sewn-quantity").text() * 1)) {
                                            bl = false;
                                        }
                                    }
                                    var model = {
                                        Tr: _thisTr,
                                        Quantity: quantity,
                                        Remark: _thisTr.find(".remark").text(),
                                        ReturnQuantity: 0
                                    };
                                    models.push(model);
                                    details += _thisTr.data("id") + "-" + quantity + ",";
                                }
                            });
                            if (!bl) {
                                alert("数量输入过大",2);
                                return false;
                            }
                            var showMsg = type == 11 ? "车缝" : type == 1 ? "裁剪" : "--";
                            if (details.length > 0) {
                                _thisBtn.data('isSubmit', 1);
                                _thisBtn.text("提交中...");
                                Global.post("/Orders/CreateOrderGoodsDoc", {
                                    orderid: Paras.orderID,
                                    taskid: Paras.taskID,
                                    doctype: type,
                                    isover: 0,
                                    details: details,
                                    remark: "",
                                    ownerid: ""
                                }, function (data) {
                                    _thisBtn.data('isSubmit', 0);
                                    _thisBtn.text(showMsg);
                                    var item = JSON.parse(data);
                                    if (item.id) {
                                        var total = 0;
                                        for (var i = 0; i < models.length; i++) {
                                            var model = models[i];
                                            var sewnQuantityHtml = $(model.Tr).find((type == 11 ? '.sewn-quantity' : '.cut-quantity'));
                                            sewnQuantityHtml.text((sewnQuantityHtml.text() * 1) + (model.Quantity * 1));
                                            total += model.Quantity * 1;
                                        }

                                        /*录入成功后添加一个新的单据*/
                                        var newDoc = {
                                            Quantity: total,
                                            Details: models,
                                            DocType: type,
                                            CreateTime: "/date" + new Date().getTime() + "/",
                                            Owner: { Name: ObjectJS.userName }
                                        };
                                        doT.exec("template/orders/cutoutdoc.html", function (fun) {
                                            var docHtml = fun([newDoc]);
                                            docHtml = $(docHtml);
                                            docHtml.find('.doc-header').click(function () {
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

                                            $(".nav-partdiv").prepend(docHtml);
                                            $(".nav-partdiv .nodata-txt").remove();
                                        });

                                        $(".goods-items input").val(0);
                                        alert(showMsg + "录入成功");
                                    } else if (data.result == "10001") {
                                        alert("您没有操作权限!",2)
                                    } else {
                                        alert(showMsg + "登记失败！",2);
                                    }
                                });
                            } else {
                                alert("请输入" + showMsg + "数量",2);
                                return false;
                            }
                        }
                    });
                    innerHtml.find('.quantity').change(function () {
                        var _this = $(this);
                        if (!_this.val().isInt() || _this.val() <= 0) {
                            _this.val(0);
                            return false;
                        }
                    });
                    $(".show-goods").click(function () {
                        var _this = $(this);
                        var sewn = $(".goods-items");
                        if (sewn.length > 0) {
                            if (sewn.data('isget') == 1) {
                                sewn.fadeOut();
                                sewn.data('isget', 0);
                                if (ObjectJS.task.mark == 13) {
                                    _this.text("裁剪录入");
                                } else {
                                    _this.text("车缝录入");
                                }
                            } else {
                                sewn.fadeIn();
                                sewn.data('isget', 1);
                                _this.text("收起信息");
                            }
                        }
                    });

                    $(".task-operate-module").append(innerHtml);
                });
            }
        });
    };

    //窗体加载绑定讨论下拉
    ObjectJS.bindScroll = function () {
        $(window).bind("scroll", function () {
            //滚动条超过图片显示致顶图标
            if ($(document).scrollTop() >= 60 + $(window).width()) {
                $(".getback").show();
            } else {
                $(".getback").hide();
            }

            if (ModuleType == "talk-status" || ModuleType == "log-status") {
                var bottom = $(document).height() - document.documentElement.scrollTop - document.body.scrollTop - $(window).height();
                if (bottom <= 20) {
                    setTimeout(function () {
                        if (ModuleType == "talk-status") {
                            Paras.replyPageIndex++;
                            ObjectJS.getTaskReplys();
                        }
                        else if (ModuleType == "log-status") {
                            Paras.logPageIndex++;
                            ObjectJS.getTaskLogs();
                        }
                    }, 1000);
                }
            }
        });
    }

    //绑定时间控件
    ObjectJS.bindTimerPicker = function () {
        if ($(".btn-acceptTaskTime").length == 0) { return; }
        if (!ObjectJS.isOwner) { return; }
        var defaultParas = {
            preset: 'datetime',
            theme: 'android-ics light', //皮肤样式
            display: 'modal', //显示方式 
            mode: 'scroller', //日期选择模式
            lang: 'zh',
            onSelect: function (date) {
                Paras.endTime = date;
                ObjectJS.showConfirmForm(0);
                $(".btn-acceptTaskTime").val("接受任务");
            }
        };
        $(".btn-acceptTaskTime").mobiscroll().datetime(defaultParas);
    }

    //设置任务到期时间
    ObjectJS.setTaskEndTime = function () {
        $.post("/Task/UpdateTaskEndTime", Paras, function (data) {
            if (data == 1) {
                $(".end-time").html(new Date(Paras.endTime.replace(/-/g, '/')).toString('yyyy-MM-dd'));
                $(".task-accept").html("<input type='button' class='btn-finishTask header-btn' readonly='readonly' value='标记完成' />");
                if ((ObjectJS.task.mark == 14 || ObjectJS.task.mark == 13) && ObjectJS.task.orderType == 2) {
                    $("#docInfo").prepend('<div class="task-operate-module"></div>')
                                 .prepend('<div class="row"><span class="btn right show-goods mRight5">裁剪录入</span></div>');
                    ObjectJS.getOrderGoods(ObjectJS.task.mark == 14 ? 11 : 1);
                }
                $(".task-accept").find(".btn-finishTask").bind('click', function () {
                    ObjectJS.showConfirmForm(1);
                });
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

    //标记完成任务
    ObjectJS.finishTask = function () {
        $.post("/Task/FinishTask", Paras, function (data) {
            if (data == 1) {
                $(".task-accept").html("<span>已完成</span>");
                if ((ObjectJS.task.mark == 14 || ObjectJS.task.mark == 13) && ObjectJS.task.orderType == 2) {
                    $(".task-operate-module").remove();
                    $(".show-goods").parent().remove();
                }
                $(".complete-time").html(new Date().toString("yyyy-MM-dd"));
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
        var alertMsg = showStatus == 0 ? "任务到期时间不可逆,确定设置?" : "确定完成任务?";
        confirm(alertMsg, function () {
            if (showStatus == 0) {
                ObjectJS.setTaskEndTime();
            } else {
                ObjectJS.finishTask();
            }
        }, "设置");
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

    //获取任务讨论列表
    ObjectJS.getTaskReplys = function () {
        GetOrAddReply = "GetReply";
        if (ReplyPageCount >= Paras.replyPageIndex) {
            $(".main-box .talk-main").append('<div class="data-loading"></div>');
            $.post("/Task/GetDiscussInfo", Paras, function (data) {
                $(".main-box .talk-status .data-loading").remove();
                ReplyPageCount = data.pagecount;
                if (ReplyPageCount == 0) {
                    $(".main-box .talk-main").append('<div class="nodata-txt">暂无数据</div>');
                }
                else {
                    ObjectJS.GetOrAddTaskReply(data, GetOrAddReply);
                }
            });
        } else {
            if ($(".reply-box").length > 0) {
                if ($(".alert-lastreplypage").length == 0) {
                    $(".main-box .talk-main").append("<div class='alert-lastreplypage center mTop10 color999'>已经是最后一条啦</div>");
                }
            }
        }
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

    //获取获取订单材料列表
    ObjectJS.GetOrderDetailsByOrderID = function () {
        Global.post("/Orders/GetOrderDetailsByOrderID", { orderID: Paras.orderID }, function (data) {
            data = JSON.parse(data);
            if (data.items.length == 0) {
                $(".shop-status").html("<div class='nodata-txt'>暂无材料</div>");
            } else {
                doT.exec("template/task/task-products.html", function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    //展开详情
                    innerText.find('.doc-header').click(function () {
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
                    $(".shop-status").html(innerText);
                });
            }
        });
    }

    //获取或添加任务讨论
    ObjectJS.GetOrAddTaskReply = function (data, replyOperate) {
        doT.exec("template/task/detail-reply.html", function (templateFun) {
            var innerText = templateFun(data.items);
            innerText = $(innerText);
            if (GetOrAddReply == "GetReply") {
                innerText.find(".reply-content").each(function () {
                    $(this).html(Global.replaceQqface($(this).html()));
                    $(this).find("img").css({ "width": "36px", "height": "36px" });
                });
                $(".talk-main").append(innerText);
            } else {
                if ($(".talk-main").find('.nodata-txt').length > 0) {
                    $(".talk-main .nodata-txt").remove();
                }
                $('body,html').animate({ scrollTop: WindowScrollTop }, 100);
                $(".talk-main").prepend(innerText);
            }

            //点击回复把用户名写入文本框
            innerText.find(".return-reply .iconfont").click(function () {
                if (IsLoading) {
                    alert("上一条评论发表中,请稍候再试.", 2);
                    return false;
                }
                if ($("#pic-list").length == 0) {
                    $(".reply-layer-content").append('<div class="text-content" style="display:block;min-height:40px;"></div>');
                    $(".reply-layer-content").append('<ul class="pic-list task-file mTop20" id="pic-list" contenteditable="false"></ul><div class="clear"></div>');
                }
                if ($("#doc-list").length == 0) {
                    $(".reply-layer-content").append('<ul class="doc-list task-file mTop20 upload-file" id="doc-list" contenteditable="false"></ul><div class="clear"></div>');
                }
                $("body,html").addClass('layer');
                $(".reply-layer").show();
                setTimeout(function () { $(".reply-layer").addClass('show'); }, 10);
                setTimeout(function () { $(".reply-layer-content").focus() }, 1000);

                TaskReplyParas.fromReplyID = $(this).data("replyid");
                TaskReplyParas.fromReplyUserID = $(this).data("userid");
                TaskReplyParas.fromReplyAgentID = $(this).data("agentid");
                $(".reply-title").html('@' + $(this).data('name'));
            });
        });
    }

    //获取手工成本
    ObjectJS.getOrderCosts = function () {
        var _self = this;
        $("#navCosts .tr-header").nextAll().remove();
        $("#navCosts .tr-header").after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        Global.post("/Orders/GetOrderCosts", {
            orderid: Paras.orderID
        }, function (data) {
            $("#navCosts .tr-header").nextAll().remove();
            data = JSON.parse(data);
            if (data.items.length > 0) {
                doT.exec("template/task/task-costs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    innerhtml.find(".cost-price").each(function () {
                        $(this).text($(this).text() * $("#navCosts").data("quantity"))
                    });
                    $("#navCosts .tr-header").after(innerhtml);
                });
            } else {
                $("#navCosts .tr-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!</div></td></tr>");
            }
        });
    };

    //获取制版工艺说明
    ObjectJS.getPlateMakings = function () {
        var _self = this;
        $(".tb-plates").html("<div class='data-loading'><div>");

        Global.post("/Orders/GetPlateMakings", {
            orderID: _self.order.orderType == 1 ? _self.order.orderID : _self.order.originalID
        }, function (data) {
            $(".tb-plates").html('');
            data = JSON.parse(data);
            if (data.items.length > 0) {
                doT.exec("template/orders/platematrings.html", function (template) {
                    PlateMakings = data.items;
                    var html = template(data.items);
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
        });
    };

    module.exports = ObjectJS;
});
