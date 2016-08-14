
/* 
作者：Allen
日期：2016-7-1
修改：Michaux 2016-08-11 支持附件上传 图片放大 文件下载
示例:
    $(...).getObjectReply(options);
*/

define(function (require, exports, module) {
    require("plug/replys/style.css");
    var Global = require("global"),
        doT = require("dot"),
        Tip = require("tip"), 
        Upload = require("upload");
    require("pager");
    require("smartzoom");
    (function ($) {
        $.fn.getObjectReplys = function (options) {
            var opts = $.extend({}, $.fn.getObjectReplys.defaults, options);
            $(document).click(function (e) { 
                if (!$(e.target).parents().hasClass("taskreply-box") && !$(e.target).hasClass("taskreply-box") && 
                    !$(e.target).parents().hasClass("ico-delete") && !$(e.target).hasClass("ico-delete") &&
                    !$(e.target).parents().hasClass("ico-delete-upload") && !$(e.target).hasClass("ico-delete-upload") &&
                    !$(e.target).hasClass("qn-delete") &&
                    !$(e.target).parents().hasClass("alert") && !$(e.target).hasClass("alert")) {
                    $(".taskreply-box").removeClass("taskreply-box-hover");
                }
            });
            return this.each(function() {
                var _this = $(this);
                $.fn.drawObjectReplys(_this, opts);
            });
        }

        $.fn.getObjectReplys.defaults = {
            guid: "",
            type: 1, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
            pageSize: 10
        };
        $.fn.drawObjectReplys = function (obj, opts) {
            obj.empty();
            doT.exec("plug/replys/replys.html", function (template) {
                var innerhtml = template([]);
                innerhtml = $(innerhtml); 
                obj.append(innerhtml);
                $('#btnSaveTalk').click(function() {
                    var txt =$(".txt-content");
                    if (txt.val().trim()) {
                        var model = {
                            GUID: opts.guid,
                            Content: txt.val().trim(),
                            FromReplyID: "",
                            FromReplyUserID: "",
                            FromReplyAgentID: ""
                        };
                        var attchments = $.fn.getAttchments("");
                        $.fn.saveReplyItem(obj, opts, model, attchments);
                        txt.val("");
                        $('#reply-box').find(".task-file").empty();
                    }
                });
                opts.pageIndex = 1;
                $.fn.drawReplysItems(obj, opts);
                $.fn.bindReplyAttachment("");
            });
        }

        $.fn.drawReplysItems = function (obj, opts) {
            obj.find(".content-list").empty();
            obj.find(".content-list").append("<div class='data-loading'><div>");
            Global.post("/Plug/GetReplys", {
                guid: opts.guid,
                type: opts.type, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
                pageSize: opts.pageSize,
                pageIndex: opts.pageIndex
            }, function (data) {
                obj.find(".content-list").empty(); 
                if (data.items.length > 0) {
                    doT.exec("plug/replys/replysitem.html", function (template) {
                        var innerhtml = template(data.items); 
                        innerhtml = $(innerhtml);
                        obj.find(".content-list").append(innerhtml);
                        $.fn.bindReplyOperate(innerhtml, obj, opts); 
                    });
                } else {
                    obj.find(".content-list").append("<div class='nodata-txt'>暂无数据<div>");
                }
                $(".taskreply-box").click(function () {
                    $(this).addClass("taskreply-box-hover").find(".reply-content").focus();
                });
                obj.find(".pager-box").paginate({
                    total_count: data.totalCount,
                    count: data.pageCount,
                    start: opts.pageIndex,
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
                    float: "left",
                    onChange: function (page) {
                        opts.pageIndex = page;
                        $.fn.drawReplysItems(obj, opts);
                    }
                });
            });
            //tip提示
            $("#reply-attachment").Tip({
                width: 100,
                msg: "上传附件最多10个"
            });
        }
        $.fn.bindReplyOperate = function(replys, obj, opts) {
            //打开讨论盒
            replys.find(".btn-reply").click(function() {
                var _this = $(this);
                var reply = _this.nextAll(".reply-box");
                $("#replyList .reply-box").each(function() {
                    if ($(this).data("replyid") != reply.data("replyid")) {
                        $(this).hide();
                    }
                });
                if (reply.is(":visible")) {
                    reply.slideUp(300);
                } else {
                    reply.slideDown(300);
                }
                reply.find("textarea").focus();
                if (_this.data('isget') != 1) {
                    _this.data('isget', 1);
                    $.fn.bindReplyAttachment(reply.data('replyid'));
                }
                //提示
                $("#reply-attachment" + _this.data("replyid")).Tip({
                    width: 100,
                    msg: "上传附件最多10个"
                });
            });
            $(".save-reply").click(function() {
                var _this = $(this);
                if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                    var entity = {
                        GUID: _this.data("id"),
                        Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                        FromReplyID: _this.data("replyid"),
                        FromReplyUserID: _this.data("createuserid"),
                        FromReplyAgentID: _this.data("agentid")
                    };
                    var attchments = $.fn.getAttchments(_this.data("replyid"));
                    $.fn.saveReplyItem(obj, opts, entity, attchments);
                    $("#Msg_" + _this.data("replyid")).val('');
                    $(this).parent().slideUp(100);
                    _this.parents('.reply-box').find(".task-file").empty();
                }
            });
            //下载图标下滑切换
            replys.find(".upload-file li").hover(function () {
                $(this).find(".popup-download").stop(true).slideDown(300);
            }, function () {
                $(this).find(".popup-download").stop(true).slideUp(300);
            });
            //绑定图片放大功能
            var width = document.documentElement.clientWidth, height = document.documentElement.clientHeight;
            replys.find(".orderImage-repay").click(function () {
                if ($(this).attr("src")) {
                    $("#Images-reply .hoverimg").removeClass("hoverimg");
                    $(this).parent().addClass("hoverimg");
                    $(".enlarge-image-bgbox,.enlarge-image-box").fadeIn();
                    $(".right-enlarge-image,.left-enlarge-image").css({ "top": height / 2 - 80 });
                    $(".enlarge-image-item").append('<img id="enlargeImage" src="' + $(this).data("src") + '"/>'); 
                    $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });

                    $(".close-enlarge-image").unbind().click(function () {
                        $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                        $(".enlarge-image-item").empty();
                    });
                    $(".enlarge-image-bgbox").unbind().click(function () {
                        $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                        $(".enlarge-image-item").empty();
                    });
                    $(".zoom-botton").unbind().click(function (e) {
                        var scaleToAdd = 0.8;
                        if (e.target.id == 'zoomOutButton')
                            scaleToAdd = -scaleToAdd;
                        $('#enlargeImage').smartZoom('zoom', scaleToAdd);
                        return false;
                    });

                    $(".left-enlarge-image").unbind().click(function () {
                        var ele = $("#Images-reply .hoverimg").prev();
                        if (ele && ele.find("img").attr("src")) {
                            var _img = ele.find("img");
                            $("#Images-reply .hoverimg").removeClass("hoverimg");
                            ele.addClass("hoverimg");
                            $(".enlarge-image-item").empty();
                            $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.data("src") + '"/>');
                            $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                        }
                    });
                    $(".right-enlarge-image").unbind().click(function () {
                        var ele = $("#Images-reply .hoverimg").next();
                        if (ele && ele.find("img").attr("src")) {
                            var _img = ele.find("img");
                            $("#Images-reply .hoverimg").removeClass("hoverimg");
                            ele.addClass("hoverimg");
                            $(".enlarge-image-item").empty();
                            $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.data("src") + '"/>');
                            $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                        }
                    });
                }
            });

        }

        $.fn.getAttchments = function (replyid) {
            var attchments = [];
            $(".upload-files-" + replyid + " li").each(function () {
                var _this = $(this);
                attchments.push({
                    "Size": _this.data("filesize"),
                    "Type": _this.data('isimg'),
                    "ServerUrl": _this.data("server"),
                    "FilePath": _this.data('filepath'),
                    "FileName": _this.data('filename'),
                    "OriginalName": _this.data('originalname'),
                    "ThumbnailName": ""
                });
            });
            return attchments;
        }

        $.fn.saveReplyItem = function (obj, opts, model,attchments) {
            Global.post("/Plug/SavaReply", {
                type: opts.type, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
                entity: JSON.stringify(model),
                attchmentEntity: JSON.stringify(attchments)
        }, function (data) {
                obj.find(".content-list .nodata-txt").remove();

                doT.exec("plug/replys/replysitem.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    obj.find(".content-list").prepend(innerhtml);
                    $.fn.bindReplyOperate(innerhtml, obj, opts); 
                });
            });
        }
        //上传附件
        $.fn.bindReplyAttachment = function (replyId) {
            var uploader = Upload.uploader({
                browse_button: 'reply-attachment' + replyId,
                picture_container: "reply-imgs" + replyId,
                file_container: "reply-files" + replyId,
                image_view: "?imageView2/1/w/120/h/80",
                successItems: '.upload-files-' + replyId + ' li',
                file_path: "/Content/UploadFiles/TalkAbout/",
                maxQuantity: 10,
                maxSize: 15,
                fileType: 3,
                init: {}
            });
        }
    })(jQuery)
    module.exports = jQuery;
});