
define(function (require, exports, module) {
    var City = require("city"), CityObj,
        Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");
    require("pager");
    var ObjectJS = {};

    //添加页初始化
    ObjectJS.init = function (opportunityid, model, ordertypes) {
        var _self = this;
        _self.model = JSON.parse(model.replace(/&quot;/g, '"'));
        _self.model.OrderTypes = JSON.parse(ordertypes.replace(/&quot;/g, '"'));
        _self.opportunityid = opportunityid;
        _self.bindEvent();
        _self.bindStyle(_self.model);
    }

    //样式
    ObjectJS.bindStyle = function (model) {

        var stages = $(".stage-items"), width = stages.width();

        stages.find("li .leftbg").first().removeClass("leftbg");
        stages.find("li .rightbg").last().removeClass("rightbg");
        stages.find("li").width(width / stages.find("li").length - 20);

        //处理阶段
        var stage = $(".stage-items li[data-id='" + model.StageID + "']");
        stage.addClass("hover");
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        //转移负责人
        $("#changeOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            Global.post("/Opportunitys/UpdateOpportunityOwner", {
                                userid: items[0].id,
                                ids: _this.data("id")
                            }, function (data) {
                                if (data.status) {
                                    $("#lblOwner").text(items[0].name);
                                }
                            });
                        } else {
                            alert("请选择不同人员进行转移!");
                        }
                    }
                }
            });
        });

        //编辑数量
        $(".quantity").change(function () {
            if ($(this).val().isInt() && $(this).val() > 0) {
                _self.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑单价
        $(".price").change(function () {
            var _this = $(this);
            if (_this.val().isDouble() && _this.val() > 0) {

                Global.post("/Opportunitys/UpdateOpportunityProductPrice", {
                    opportunityid: _self.opportunityid,
                    productid: _this.data("id"),
                    name: _this.data("name"),
                    price: _this.val()
                }, function (data) {
                    if (!data.status) {
                        _this.val(_this.data("value"));
                        alert("价格编辑失败，请刷新页面后重试！", function () {
                            location.href = location.href;
                        });
                    } else {
                        _this.parent().nextAll(".amount").html((_this.parent().nextAll(".tr-quantity").find("input").val() * _this.val()).toFixed(2));
                        _this.data("value", _this.val());
                        _self.getAmount();
                    }
                });

               
            } else {
                _this.val(_this.data("value"));
            }
        });

        //删除产品
        $(".ico-del").click(function () {
            var _this = $(this);
            confirm("确认移除此产品吗？", function () {
                Global.post("/ShoppingCart/DeleteCart", {
                    ordertype: 10,
                    guid: _self.opportunityid,
                    productid: _this.data("id"),
                    name: _this.data("name")
                }, function (data) {
                    if (!data.status) {
                        alert("网络异常或数据状态有变更，请重新操作", function () {
                            location.href = location.href;
                        });
                    } else {
                        alert("产品移除成功");
                        _this.parents("tr.item").remove();
                        _self.getAmount();
                    }
                });
            });
        });

        //提交订单
        $("#btnconfirm").click(function () {

            if ($(".cart-item").length == 0) {
                alert("您尚未选择产品！");
                return;
            }

            confirm("机会转为订单后不可撤销，确认转为订单吗？", function () {
                _self.submitOrder();
            });
            
        });

        //关闭机会
        $("#btnClose").click(function () {
            confirm("机会关闭后不可恢复，确认关闭吗？", function () {
                _self.closeOpportunity();
            });
        });

        //切换阶段
        $(".stage-items li").click(function () {
            var _this = $(this);

            _self.model.Status == 1 && !_this.hasClass("hover") && confirm("确认将机会切换到此阶段吗?", function () {
                Global.post("/Opportunitys/UpdateOpportunityStage", {
                    ids: _self.opportunityid,
                    stageid: _this.data("id")
                }, function (data) {
                    if (data.status) {
                        Global.post("/Opportunitys/GetStageItems", {
                            stageid: _this.data("id")
                        }, function (items) {
                            $("#stageItems").empty();
                            for (var i = 0; i < items.items.length; i++) {
                                $("#stageItems").append("<li>" + items.items[i].ItemName + "</li>")
                            }
                        });
                        _this.siblings().removeClass("hover");
                        _this.addClass("hover");
                    }
                });
            });
        });

        //切换模块
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            $("#addInvoice").hide();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getLogs(1);
            } else if (_this.data("id") == "navRemark" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.initTalk(_self.opportunityid);
            }
        });
        
        $("#editOpportunity").click(function () {
            _self.updateOpportunity(_self.model);
        });

        //分享到明道
        require.async("sharemingdao", function () {
            $("#btnShareMD").sharemingdao({
                post_pars: {
                    content: _self.model.Customer.Name + "的机会：" + _self.model.OpportunityCode,
                    groups: [],
                    share_type: 0
                },
                task_pars: {
                    name: _self.model.Customer.Name + "的机会：" + _self.model.OpportunityCode,
                    end_date: "",
                    charger: _self.model.Owner,
                    members: [_self.model.Owner],
                    des: "",
                    url: "/Opportunitys/Detail?id=" + _self.opportunityid + "&source=md"
                },
                schedule_pars: {
                    name: _self.model.Customer.Name + "的机会：" + _self.model.OpportunityCode,
                    start_date: "",
                    end_date: "",
                    members: [_self.model.Owner],
                    address: _self.model.Address,
                    des: "",
                    url: "/Opportunitys/Detail?id=" + _self.opportunityid + "&source=md"
                },
                callback: function (type, url) {
                    if (type == "Calendar") {
                        url = "<a href='" + url + "' target='_blank'>分享明道日程，点击查看详情</a>";
                    } else if (type == "Task") {
                        url = "<a href='" + url + "' target='_blank'>分享明道任务，点击查看详情</a>";
                    }

                    var entity = {
                        GUID: _self.opportunityid,
                        Content: encodeURI(url),
                        FromReplyID: "",
                        FromReplyUserID: "",
                        FromReplyAgentID: ""
                    };
                    _self.saveReply(entity);
                }
            });
        });
    }

    //编辑信息
    ObjectJS.updateOpportunity = function (model) {
        var _self = this;
        doT.exec("template/sales/opportunity-detail.html", function (template) {
            var innerText = template(model);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: "编辑机会",
                    content: innerText,
                    yesFn: function () {
                        var entity = {
                            OpportunityID: _self.opportunityid,
                            PersonName: $("#personName").val().trim(),
                            MobileTele: $("#mobileTele").val().trim(),
                            CityCode: CityObj.getCityCode(),
                            Address: $("#address").val().trim(),
                            TypeID: $("#orderType").val().trim(),
                            Remark: $("#remark").val().trim()
                        };
                        Global.post("/Opportunitys/UpdateOpportunity", { entity: JSON.stringify(entity) }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            } else {
                                alert("机会编辑失败，请刷新页面重试！");
                            }
                        })
                    },
                    callback: function () {

                    }
                }
            });

            CityObj = City.createCity({
                cityCode: model.CityCode,
                elementID: "city"
            });

            $("#orderType").val(model.TypeID);
        });
    }

    //获取日志
    ObjectJS.getLogs = function (page) {
        var _self = this;
        $("#opportunityLog").empty();
        $("#opportunityLog").append("<div class='data-loading'><div>");
        Global.post("/Opportunitys/GetOpportunityLogs", {
            opportunityid: _self.opportunityid,
            pageindex: page
        }, function (data) {
            $("#opportunityLog").empty();
            if (data.items.length > 0) {
                doT.exec("template/common/logs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    $("#opportunityLog").append(innerhtml);
                });
            } else {
                $("#opportunityLog").append("<div class='nodata-txt'>暂无日志<div>");
            }
            $("#pagerLogs").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: page,
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
                    _self.getLogs(customerid, page);
                }
            });
        });
    }

    //计算总金额
    ObjectJS.getAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            _this.html((_this.prevAll(".tr-quantity").find("input").val() * _this.prevAll(".tr-price").find("input").val()).toFixed(2));
            amount += _this.html() * 1;
        });
        $("#amount").text(amount.toFixed(2));
    }

    //更改数量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/Opportunitys/UpdateOpportunityProductQuantity", {
            opportunityid: _self.opportunityid,
            productid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("网络异常或数据状态有变更，请重新操作", function () {
                    location.href = location.href;
                });
            } else {
                ele.parent().nextAll(".amount").html((ele.parent().prevAll(".tr-price").find("input").val() * ele.val()).toFixed(2));
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //转为订单
    ObjectJS.submitOrder = function () {
        var _self = this;
        if ($(".cart-item").length == 0) {
            alert("您尚未选择产品！");
            return;
        }
        Global.post("/Opportunitys/SubmitOrder", { opportunityid: _self.opportunityid }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else {
                location.href = location.href;
            }
        })
    }

    //关闭机会
    ObjectJS.closeOpportunity = function () {
        var _self = this;
        Global.post("/Opportunitys/CloseOpportunity", { opportunityid: _self.opportunityid }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else {
                alert("机会关闭失败，可能因为机会状态已改变，请刷新页面后重试！");
            }
        });
    }

    //讨论备忘
    ObjectJS.initTalk = function (opportunityid) {
        var _self = this;

        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");
            if (txt.val().trim()) {
                var model = {
                    GUID: opportunityid,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: ""
                };
                _self.saveReply(model);

                txt.val("");
            }

        });
        _self.getReplys(opportunityid, 1);

    }

    //获取备忘
    ObjectJS.getReplys = function (opportunityid, page) {
        var _self = this;
        $("#replyList").empty();
        $("#replyList").append("<div class='data-loading'><div>");
        Global.post("/Opportunitys/GetReplys", {
            guid: opportunityid,
            pageSize: 10,
            pageIndex: page
        }, function (data) {
            $("#replyList").empty();
            if (data.items.length > 0) {
                doT.exec("template/customer/replys.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#replyList").append(innerhtml);

                    innerhtml.find(".btn-reply").click(function () {
                        var _this = $(this), reply = _this.nextAll(".reply-box");
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

                });
            } else {
                $("#replyList").append("<div class='nodata-txt'>暂无备忘<div>");
            }

            $("#pagerReply").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: page,
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
                    _self.getReplys(orderid, page);
                }
            });
        });
    }

    ObjectJS.saveReply = function (model) {
        var _self = this;
        $("#replyList .nodata-txt").remove();
        Global.post("/Opportunitys/SavaReply", { entity: JSON.stringify(model) }, function (data) {
            doT.exec("template/customer/replys.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                $("#replyList").prepend(innerhtml);

                innerhtml.find(".btn-reply").click(function () {
                    var _this = $(this), reply = _this.nextAll(".reply-box");
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
            });
        });
    }

    module.exports = ObjectJS;
})