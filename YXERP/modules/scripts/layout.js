﻿/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        doT = require("dot"),
        Global = require("global"),
        Easydialog = require("easydialog");

    var LayoutObject = {};
    //初始化数据
    LayoutObject.init = function () {
        
        LayoutObject.bindStyle();
        LayoutObject.bindEvent();
        LayoutObject.getAgentInfo();
        LayoutObject.bindUpcomings();
    }

    //绑定元素定位和样式
    LayoutObject.bindStyle = function () {


    }
    //绑定事件
    LayoutObject.bindEvent = function () {
        var _self = this;
        //调整浏览器窗体
        $(window).resize(function () {
            LayoutObject.bindStyle();
        });

        $(document).click(function (e) {

            if (!$(e.target).parents().hasClass("currentuser") && !$(e.target).hasClass("currentuser")) {
                $(".dropdown-userinfo").fadeOut("1000");
            }

            if (!$(e.target).parents().hasClass("company-logo") && !$(e.target).hasClass("company-logo")) {
                $(".dropdown-companyinfo").fadeOut("1000");
            }
        });

        //登录信息展开
        $("#currentUser").click(function () {
            $(".dropdown-userinfo").fadeIn("1000");
        });

        //公司名称信息展开
        $("#companyName,#companyLogo").click(function () {
            $(".dropdown-companyinfo").fadeIn("1000");
        });

        //一级菜单图标事件处理
        $("#modulesMenu li").mouseenter(function () {
            var _this = $(this).find("img");
            _this.attr("src", _this.data("hover"));
        });

        $("#modulesMenu li").mouseleave(function () {
            if (!$(this).hasClass("hover")) {
                var _this = $(this).find("img");
                _this.attr("src", _this.data("ico"));
            }
        });

        $("#modulesMenu li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                _this.siblings().removeClass("hover");
                _this.siblings().find("img").each(function () {
                    $(this).attr("src", $(this).data("ico"));
                });

                $.get("/Default/LeftMenu", { id: _this.data("code") }, function (html) {
                    $("#leftNav").empty();
                    $("#leftNav").append(html);

                    _self.bindUpcomings();
                });
            }
        });

        //意见反馈浮层
        $(".help-feedback .ico-open").click(function () {
            var _this = $(this);
            if (_this.data("open") && _this.data("open") == "1") {
                _this.data("open", "0");
                _self.setRotateL(_this, 45, 0);

                _this.parent().animate({ "height": "45px" }, "fast");
            } else {
                _this.data("open", "1");
                _self.setRotateR(_this, 0, 45);

                _this.parent().animate({ "height": "180px" }, "fast");
            }
        });

        //意见反馈
        $(".ico-feedback").click(function () {
            doT.exec("template/default/feedback-add.html", function (template) {
                var html = template([]);
                Easydialog.open({
                    container: {
                        id: "show-model-feedback",
                        header: "意见反馈",
                        content: html,
                        yesFn: function () {
                            if ($("#feedback-title").val() == "")
                            {
                                alert("标题不能为空");
                                return false;
                            }
                            var entity = {
                                Title: $("#feedback-title").val(),
                                ContactName: $("#feedback-contactname").val(),
                                MobilePhone: $("#feedback-mobilephone").val(),
                                Type: $("#feedback-type").val(),
                                FilePath: $("#feedback-filepath").val(),
                                Remark: $("#feedback-remark").val()
                            };
                            Global.post("/Default/SaveFeedBack", { entity: JSON.stringify(entity) }, function (data) {
                                if (data.Result == 1) {
                                    alert("反馈提交成功，谢谢反馈");
                                }
                            });
                        },
                        callback: function () {

                        }
                    }
                });

                $("#feedback-contactname").val($("#txt_username").val());
                $("#feedback-mobilephone").val($("#txt_usermobilephone").val());

                var Upload = require("upload");
                //选择意见反馈附件
                Upload.createUpload({
                    element: "#feedback-file",
                    buttonText: "选择附件",
                    className: "",
                    data: { folder: '/Content/tempfile/', action: 'add', oldPath: "" },
                    success: function (data, status) {
                        if (data.Items.length > 0) {
                            $("#feedback-filepath").val(data.Items[0]);
                            var arr=data.Items[0].split("/");
                            $("#feedback-filename").html(arr[arr.length-1]);
                        }
                    }
                });

            });

        });
    }
    //旋转按钮（顺时针）
    LayoutObject.setRotateR = function (obj, i, v) {
        var _self = this;
        if (i < v) {
            i += 3;
            setTimeout(function () {
                obj.css("transform", "rotate(" + i + "deg)");
                _self.setRotateR(obj, i, v);
            }, 5)
        }
    }
    //旋转按钮(逆时针)
    LayoutObject.setRotateL = function (obj, i, v) {
        var _self = this;
        if (i > v) {
            i -= 3;
            setTimeout(function () {
                obj.css("transform", "rotate(" + i + "deg)");
                _self.setRotateL(obj, i, v);
            }, 5)
        } 
    }

    //待办小红点
    LayoutObject.bindUpcomings = function () {
        Global.post("/Default/GetClientUpcomings", {}, function (data) {
            for (var i = 0; i < data.items.length; i++) {
                var item = data.items[i];

                //采购
                if (item.DocType == 1) {
                    if ($("#modulesMenu li[data-code='103000000']").find(".point").length == 0) {
                        $("#modulesMenu li[data-code='103000000']").find(".name").after("<span class='point'></span>");
                    }

                    var controller = $("nav .controller[data-code='103020000']");
                    if (controller.find(".point").length == 0) {
                        controller.find(".controller-item .name").after("<span class='point'></span>");
                    }
                    if (controller.find("li[data-code='103020200']").find(".point").length == 0) {
                        controller.find("li[data-code='103020200']").find(".name").append("<span class='point'></span>");
                    }
                } else if (item.DocType == 3) { //报损
                    if ($("#modulesMenu li[data-code='103000000']").find(".point").length == 0) {
                        $("#modulesMenu li[data-code='103000000']").find(".name").after("<span class='point'></span>");
                    }

                    var controller = $("nav .controller[data-code='103030000']");
                    if (controller.find(".point").length == 0) {
                        controller.find(".controller-item .name").after("<span class='point'></span>");
                    }
                    if (controller.find("li[data-code='103030300']").find(".point").length == 0) {
                        controller.find("li[data-code='103030300']").find(".name").append("<span class='point'></span>");
                    }
                } else if (item.DocType == 4) { //报溢
                    if ($("#modulesMenu li[data-code='103000000']").find(".point").length == 0) {
                        $("#modulesMenu li[data-code='103000000']").find(".name").after("<span class='point'></span>");
                    }

                    var controller = $("nav .controller[data-code='103030000']");
                    if (controller.find(".point").length == 0) {
                        controller.find(".controller-item .name").after("<span class='point'></span>");
                    }
                    if (controller.find("li[data-code='103030400']").find(".point").length == 0) {
                        controller.find("li[data-code='103030400']").find(".name").append("<span class='point'></span>");
                    }
                } else if (item.DocType == 6) { //退货
                    if ($("#modulesMenu li[data-code='103000000']").find(".point").length == 0) {
                        $("#modulesMenu li[data-code='103000000']").find(".name").after("<span class='point'></span>");
                    }

                    var controller = $("nav .controller[data-code='103030000']");
                    if (controller.find(".point").length == 0) {
                        controller.find(".controller-item .name").after("<span class='point'></span>");
                    }
                    if (controller.find("li[data-code='103031000']").find(".point").length == 0) {
                        controller.find("li[data-code='103031000']").find(".name").append("<span class='point'></span>");
                    }
                } else if (item.DocType == 7) { //手工出库
                    if ($("#modulesMenu li[data-code='103000000']").find(".point").length == 0) {
                        $("#modulesMenu li[data-code='103000000']").find(".name").after("<span class='point'></span>");
                    }

                    var controller = $("nav .controller[data-code='103030000']");
                    if (controller.find(".point").length == 0) {
                        controller.find(".controller-item .name").after("<span class='point'></span>");
                    }
                    if (controller.find("li[data-code='103030500']").find(".point").length == 0) {
                        controller.find("li[data-code='103030500']").find(".name").append("<span class='point'></span>");
                    }
                } else if (item.DocType == 21) { //代理商采购单
                    if ($("#modulesMenu li[data-code='103000000']").find(".point").length == 0) {
                        $("#modulesMenu li[data-code='103000000']").find(".name").after("<span class='point'></span>");
                    }

                    var controller = $("nav .controller[data-code='103040000']");
                    if (controller.find(".point").length == 0) {
                        controller.find(".controller-item .name").after("<span class='point'></span>");
                    }
                    if (item.SendStatus == 0) {
                        if (controller.find("li[data-code='103040100']").find(".point").length == 0) {
                            controller.find("li[data-code='103040100']").find(".name").append("<span class='point'></span>");
                        }
                    } else if (item.SendStatus == 1) {
                        if (controller.find("li[data-code='103040200']").find(".point").length == 0) {
                            controller.find("li[data-code='103040200']").find(".name").append("<span class='point'></span>");
                        }
                    }

                    if (item.ReturnStatus == 1 && item.SendStatus == 0) {
                        if (controller.find("li[data-code='103040300']").find(".point").length == 0) {
                            controller.find("li[data-code='103040300']").find(".name").append("<span class='point'></span>");
                        }
                    } else if (item.ReturnStatus == 1 && item.SendStatus > 0) {
                        if (controller.find("li[data-code='103040400']").find(".point").length == 0) {
                            controller.find("li[data-code='103040400']").find(".name").append("<span class='point'></span>");
                        }
                    }
                } else if (item.DocType == 111) { //财务
                    if (item.SendStatus == 1) {
                        if ($("#modulesMenu li[data-code='104000000']").find(".point").length == 0) {
                            $("#modulesMenu li[data-code='104000000']").find(".name").after("<span class='point'></span>");
                        }

                        var controller = $("nav .controller[data-code='104010000']");
                        if (controller.find(".point").length == 0) {
                            controller.find(".controller-item .name").after("<span class='point'></span>");
                        }

                        if (controller.find("li[data-code='104010200']").find(".point").length == 0) {
                            controller.find("li[data-code='104010200']").find(".name").append("<span class='point'></span>");
                        }
                    }
                }
            }
        });
    }

    //获取代理商授权信息
    LayoutObject.getAgentInfo = function () {
        Global.post("/Default/GetAgentAuthorizes", null, function (data) {
            $("#remainderDays").html(data.remainderDays);

            if (data.authorizeType == 0) {
                $(".btn-buy").html("立即购买");
            }
            else {
                if (parseInt(data.remainderDays) < 31) {
                    $("#remainderDays").addClass("red");
                    $(".btn-buy").html("续费").attr("href", "/Auction/ExtendNow");
                }
                else {
                    $(".btn-buy").html("购买人数").attr("href", "/Auction/BuyUserQuantity");
                }
            }
        });
    }

    // 判断浏览器是否支持 placeholder
    LayoutObject.placeholderSupport = function () {
        if (! ('placeholder' in document.createElement('input')) ) {   
            $('[placeholder]').focus(function () {
                var input = $(this);
                if (input.val() == input.attr('placeholder')) {
                    input.val('');
                    input.removeClass('placeholder');
                }
            }).blur(function () {
                var input = $(this);
                if (input.val() == '' || input.val() == input.attr('placeholder')) {
                    input.addClass('placeholder');
                    input.val(input.attr('placeholder'));
                }
            }).blur();
        };
    }

    module.exports = LayoutObject;
})