
/* 
作者：Allen
日期：2016-7-1
示例:
    $(...).getObjectReply(options);
*/

define(function (require, exports, module) {
    require("plug/replys/style.css");
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    (function ($) {
        $.fn.getObjectReplys = function (options) {
            var opts = $.extend({}, $.fn.getObjectReplys.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawObjectReplys(_this, opts);
            })
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

                innerhtml.find(".btn-save").click(function () {
                    var txt = innerhtml.find(".txt-content");
                    if (txt.val().trim()) {
                        var model = {
                            GUID: opts.guid,
                            Content: txt.val().trim(),
                            FromReplyID: "",
                            FromReplyUserID: "",
                            FromReplyAgentID: ""
                        };
                        $.fn.saveReplyItem(obj, opts, model);

                        txt.val("");
                    }
                });
                obj.append(innerhtml);
                opts.pageIndex = 1;
                $.fn.drawReplysItems(obj, opts);
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

                        innerhtml.find(".btn-reply").click(function () {
                            var _this = $(this), reply = _this.parent().nextAll(".reply-box");
                            reply.slideDown(500);
                            reply.find("textarea").focus();
                            reply.find("textarea").blur(function () {
                                if (!$(this).val().trim()) {
                                    reply.slideUp(200);
                                }
                            });
                        });
                        innerhtml.find(".save-reply").click(function () {
                            var _this = $(this);
                            if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                                var entity = {
                                    GUID: _this.data("id"),
                                    Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                    FromReplyID: _this.data("replyid"),
                                    FromReplyUserID: _this.data("createuserid"),
                                    FromReplyAgentID: _this.data("agentid")
                                };

                                _self.saveReply(entity);
                            }

                            $("#Msg_" + _this.data("replyid")).val('');
                            $(this).parent().slideUp(100);
                        });

                        //require.async("businesscard", function () {
                        //    innerhtml.find(".user-avatar").businessCard();
                        //});
                    });
                } else {
                    obj.find(".content-list").append("<div class='nodata-txt'>暂无数据<div>");
                }

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
        }

        $.fn.saveReplyItem = function (obj, opts, model) {

            Global.post("/Plug/SavaReply", {
                type: opts.type, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
                entity: JSON.stringify(model)
            }, function (data) {

                obj.find(".content-list .nodata-txt").remove();

                doT.exec("plug/replys/replysitem.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    obj.find(".content-list").prepend(innerhtml);

                    innerhtml.find(".btn-reply").click(function () {
                        var _this = $(this), reply = _this.parent().nextAll(".reply-box");
                        reply.slideDown(500);
                        reply.find("textarea").focus();
                        reply.find("textarea").blur(function () {
                            if (!$(this).val().trim()) {
                                reply.slideUp(200);
                            }
                        });
                    });
                    innerhtml.find(".save-reply").click(function () {
                        var _this = $(this);
                        if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                            var entity = {
                                GUID: _this.data("id"),
                                Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                FromReplyID: _this.data("replyid"),
                                FromReplyUserID: _this.data("createuserid"),
                                FromReplyAgentID: _this.data("agentid")
                            };
                            $.fn.saveReplyItem(obj, opts, entity);
                        }
                        $("#Msg_" + _this.data("replyid")).val('');
                        $(this).parent().slideUp(100);
                    });

                    //require.async("businesscard", function () {
                    //    innerhtml.find(".user-avatar").businessCard();
                    //});
                });
            });
        }
    })(jQuery)
    module.exports = jQuery;
});