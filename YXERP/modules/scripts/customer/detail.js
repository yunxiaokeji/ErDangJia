define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject, CityContact,
        Verify = require("verify"), VerifyObject, VerifyContact,
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");
    require("pager");
    require("replys");
    require("colormark");

    var ObjectJS = {}, CacheContacts = null;
    ObjectJS.ColorList = [];

    //初始化
    ObjectJS.init = function (customerid, colorList, navid) {
        var _self = this;
        _self.customerid = customerid;
        _self.ColorList=JSON.parse(colorList.replace(/&quot;/g, '"'));

        var nav = $(".tab-nav-ul li[data-id='" + navid + "']");
        if (nav.length > 0) {
            nav.addClass("hover");
        } else {
            $(".tab-nav-ul li").first().addClass("hover").data("first", "1");
            $("#navRemark").show();
            $("#navRemark").getObjectReplys({
                guid: _self.customerid,
                type: 1, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
                pageSize: 10
            });
        }

        Global.post("/Customer/GetCustomerByID", { customerid: customerid }, function (data) {
            if (data.model.CustomerID) {
                $('#customercolor').data('value', data.model.Mark); 
                _self.bindCustomerInfo(data.model);
                _self.bindEvent(data.model, navid);
            }
        });
    } 

    //基本信息
    ObjectJS.bindCustomerInfo = function (model) {
        var _self = this;

        if (model.Status == 2 || model.Status == 3) {
            $("#lblCustomerName").html(model.Name + "(已关闭)").addClass("colorccc");
        } else {
            $("#lblCustomerName").html(model.Name);
        }
       
        $("#lblMobile").text(model.MobilePhone || "--");
        $("#lblEmail").text(model.Email || "--");
        $("#lblIndustry").text(model.Industry ? model.Industry.Name : "--");
        $("#lblExtent").text(model.ExtentStr || "--");
        $("#lblCity").text(model.City ? model.City.Description : "--");
        $("#lblAddress").text(model.Address || "--");
        $("#lblContactName").text(model.ContactName || "--");
        $("#lblJobs").text(model.Jobs || "--");
        $("#lblUser").text(model.CreateUser ? model.CreateUser.Name : "--");
        $("#lblTime").text(model.CreateTime.toDate("yyyy-MM-dd hh:mm:ss"));
        if (model.Activity) {
            if (model.Source.SourceCode == "Source-Activity") {
                $("#aSource").data("url", "/Activity/Detail/" + model.ActivityID);
                $("#aSource").data("id", Global.guid());
                $("#aSource").data("name", "活动详情-" + model.Activity.Name);
                $("#aSource").html(model.Activity.Name);
                $("#aSource").show();
                $("#lblSource").hide();
            }
        } else {
            $("#lblSource").text(model.Source ? model.Source.SourceName : "--");
        }
        $("#lblOwner").text(model.Owner ? model.Owner.Name : "--");
        $("#changeOwner").data("userid", model.OwnerID);
        $("#lblReamrk").text(model.Description);

        if (model.Type == 1) {
            $("#lblType").html("企业客户");
        } else {
            $("#lblType").html("个人客户");
        }

        $(".tab-nav-ul li[data-id='navOrder']").html("销售订单（" + model.OrderCount + "）");
        $(".tab-nav-ul li[data-id='navOppor']").html("销售机会（" + model.OpportunityCount + "）");
    }

    //绑定事件
    ObjectJS.bindEvent = function (model, navid) {
        var _self = this;
        $('#customercolor').markColor({
            isAll: false,
            top: 25,
            data: _self.ColorList,
            onChange: function (obj, callback) { 
                if (obj.data("value") < 0) {
                    alert("不能标记此选项!"); return false;
                }
                Global.post("/Customer/UpdateCustomMark", { ids: model.CustomerID, mark: $('#customercolor').data('value') }, function (data) {
                    callback && callback(data.status);
                });
            }
        }); 

        //隐藏下拉
        $(document).click(function(e) {
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $("#ddlContact").hide();
            }
            if (!$(e.target).hasClass("btn-dropdown")) {
                $("#ddlOperate").hide();
            }
        });

        //编辑客户信息
        $("#updateCustomer").click(function () {
            _self.editCustomer(model);
        });

        if (model.Status == 1) {
            $("#closeCustomer").show();

            //丢失客户
            $("#loseCustomer").click(function () {
                confirm("确认更换客户状态为丢失吗?", function () {
                    Global.post("/Customer/LoseCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });

            //关闭客户
            $("#closeCustomer").click(function () {
                confirm("确认关闭此客户吗?", function () {
                    Global.post("/Customer/CloseCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });
        } else if (model.Status == 2 || model.Status == 3) {

            $("#recoveryCustomer").show();
            //恢复客户
            $("#recoveryCustomer").click(function () {
                confirm("确认恢复此客户吗?", function () {
                    Global.post("/Customer/RecoveryCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });
        }

        //添加联系人
        $("#btnCreateContact").click(function () {
            _self.addContact();
        });

        //更换负责人
        $("#changeOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            Global.post("/Customer/UpdateCustomOwner", {
                                userid: items[0].id,
                                ids: model.CustomerID
                            }, function (data) {
                                if (data.status) {
                                    _this.data("userid", items[0].id);
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

        //切换模块
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {  /*日志*/
                _this.data("first", "1");
                require.async("logs", function () {
                    $("#navLog").getObjectLogs({
                        guid: _self.customerid,
                        type: 1, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
                        pageSize: 10
                    });
                });
                
            } else if (_this.data("id") == "navRemark" && (!_this.data("first") || _this.data("first") == 0)) { /*备忘*/
                _this.data("first", "1");
                $("#navRemark").getObjectReplys({
                    guid: _self.customerid,
                    type: 1, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
                    pageSize: 10
                });
            } else if (_this.data("id") == "navContact" && (!_this.data("first") || _this.data("first") == 0)) { /*联系人*/
                _this.data("first", "1");
                _self.getContacts();
            } else if (_this.data("id") == "navOrder" && (!_this.data("first") || _this.data("first") == 0)) { /*订单*/
                _this.data("first", "1");
                _self.getOrders(model.CustomerID, 1);
            } else if (_this.data("id") == "navOppor" && (!_this.data("first") || _this.data("first") == 0)) { /*机会*/
                _this.data("first", "1");
                _self.getOpportunitys(model.CustomerID, 1);
            }
        });

        //编辑联系人
        $("#editContact").click(function () {
            var _this = $(this);
            Global.post("/Customer/GetContactByID", { id: _this.data("id") }, function (data) {
                _self.addContact(data.model);
            });
        });

        //删除联系人
        $("#deleteContact").click(function () {
            var _this = $(this);
            confirm("确认删除此联系人吗？", function () {
                Global.post("/Customer/DeleteContact", {
                    id: _this.data("id"),
                    name: _this.data("name"),
                    customerid: _self.customerid
                }, function (data) {
                    if (data.status) {
                        _self.getContacts(_self.customerid);
                    } else {
                        alert("默认联系人不能删除");
                    }
                });
            });
        });

        //联系人设为默认
        $("#editContactDefault").click(function () {
            var _this = $(this);
            Global.post("/Customer/UpdateContactDefault", {
                id: _this.data("id"),
                name: _this.data("name"),
                customerid: _self.customerid
            }, function (data) {
                if (data.status) {
                    _self.getContacts(_self.customerid);
                    $("#lblContactName").text(_this.data("name"));
                } else {
                    alert("操作失败");
                }
            });
        });

        //分享到明道
        require.async("sharemingdao", function () {
            $("#btnShareMD").sharemingdao({
                post_pars: {
                    content: model.Name,
                    groups: [],
                    share_type: 0
                },
                task_pars: {
                    name: model.Name,
                    end_date: "",
                    charger: model.Owner,
                    members: [model.Owner],
                    des: "",
                    url: "/Customer/Detail?id=" + model.CustomerID + "&source=md"
                },
                schedule_pars: {
                    name: model.Name,
                    start_date: "",
                    end_date: "",
                    members: [model.Owner],
                    address: model.Address,
                    des: "",
                    url: "/Customer/Detail?id=" + model.CustomerID + "&source=md"
                },
                callback: function (type, url) {
                    if (type == "Calendar") {
                        url = "<a href='" + url + "' target='_blank'>分享明道日程，点击查看详情</a>";
                    } else if (type == "Task") {
                        url = "<a href='" + url + "' target='_blank'>分享明道任务，点击查看详情</a>";
                    }

                    var entity = {
                        GUID: model.CustomerID,
                        Content: encodeURI(url),
                        FromReplyID: "",
                        FromReplyUserID: "",
                        FromReplyAgentID: ""
                    };
                    _self.saveReply(entity);
                }
            });
        });

        //新建机会,新建订单
        $("#btnCreateOpportunity,#btnCreateOrder").click(function () {
            var _this = $(this);
            if (model.CacheTypes && model.CacheTypes.length > 0) {
                if (CacheContacts) {
                    model.Contacts = CacheContacts;
                    _self.createOpporOrOrder(model, _this.data("id"));
                } else {
                    _self.getContacts(function (contacts) {
                        model.Contacts = contacts;
                        _self.createOpporOrOrder(model, _this.data("id"));
                    })
                }
            } else {
                Global.post("/System/GetOrderTypes", {}, function (data) {
                    model.CacheTypes = data.items;
                    if (CacheContacts) {
                        model.Contacts = CacheContacts;
                        _self.createOpporOrOrder(model, _this.data("id"));
                    } else {
                        _self.getContacts(function (contacts) {
                            model.Contacts = contacts;
                            _self.createOpporOrOrder(model, _this.data("id"));
                        })
                    }
                });
            }
        });

        /*展开操作项*/
        $("#btnOperate").click(function () {
            var _this = $(this);
            var position = _this.position();
            $("#ddlOperate").css({ "top": position.top + 24, "left": position.left - 40 }).show().mouseleave(function () {
                $(this).hide();
            });
        });

        //默认选中标签页
        if (navid) {
            $(".tab-nav-ul li[data-id='" + navid + "']").click();
        }
    }

    //创建机会或者订单 type 1 机会 2订单
    ObjectJS.createOpporOrOrder = function (items, type) {
        var _self = this;
        var url = type == 1 ? "/Opportunitys/Create" : "/Orders/Create";
        doT.exec("template/customer/choose-ordertype.html", function (template) {
            var innerHtml = template(items);
            Easydialog.open({
                container: {
                    id: "show-model-choosetype",
                    header: type == 1 ? "新建机会" : "新建订单",
                    content: innerHtml,
                    yesFn: function () {
                        var typeid = $(".ordertype-items .hover").data("id");
                        if (!typeid) {
                            alert("请选择订单类型！");
                            return false;
                        } else {
                            Global.post(url, {
                                customerid: _self.customerid,
                                typeid: typeid
                            }, function (data) {
                                if (data.id && data.id.length > 0) {
                                    if (type == 1) {
                                        alert("机会创建成功", function () {
                                            _self.getOpportunitys(_self.customerid, 1);
                                        });
                                    } else {
                                        alert("订单创建成功", function () {
                                            _self.getOrders(_self.customerid, 1);
                                        });
                                    }
                                    
                                } else {
                                    alert((type == 1 ? "机会" : "订单") + "创建失败");
                                }
                            });
                        }
                    },
                    callback: function () {

                    }
                }
            });

            $(".ordertype-items .item").click(function () {
                $(this).siblings().removeClass("hover");
                $(this).addClass("hover");
            });
        });
    }

    //获取订单
    ObjectJS.getOrders = function (customerid, page) {
        var _self = this;
        $("#navOrder .box-header").nextAll().remove();
        $("#navOrder .box-header").after("<div class='data-loading'><div>");
        Global.post("/Orders/GetOrdersByCustomerID", {
            customerid: customerid,
            pagesize: 10,
            pageindex: page
        }, function (data) {
            $("#navOrder .box-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/customer/customerorders.html", function (template) {
                    var innerhtml = template(data.items);

                    innerhtml = $(innerhtml);
                    $("#navOrder .box-header").after(innerhtml);
                });
            } else {
                $("#navOrder .box-header").after("<div class='nodata-box' >暂无数据!<div>");
            }
            $("#pagerOrders").paginate({
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
                    _self.getOrders(customerid, page);
                }
            });
        });
    }

    //获取订单
    ObjectJS.getOpportunitys = function (customerid, page) {
        var _self = this;
        $("#navOppor .box-header").nextAll().remove();
        $("#navOppor .box-header").after("<div class='data-loading'><div>");
        Global.post("/Opportunitys/GetOpportunityByCustomerID", {
            customerid: customerid,
            pagesize: 10,
            pageindex: page
        }, function (data) {
            $("#navOppor .box-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/customer/customeroppors.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    $("#navOppor .box-header").after(innerhtml);
                });
            } else {
                $("#navOppor .box-header").after("<div class='nodata-box' >暂无数据!<div>");
            }
            $("#pagerOppors").paginate({
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
                    _self.getOpportunitys(customerid, page);
                }
            });
        });
    }

    //获取联系人
    ObjectJS.getContacts = function (callback) {
        var _self = this;
        $("#navContact .box-header").nextAll().remove();
        $("#navContact .box-header").after("<div class='data-loading'><div>");
        Global.post("/Customer/GetContacts", {
            customerid: _self.customerid
        }, function (data) {
            $("#navContact .box-header").nextAll().remove();
            if (data.items.length > 0) {
                callback && callback(data.items);
                CacheContacts = data.items;
                doT.exec("template/customer/contacts.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    innerhtml.find(".dropdown").click(function () {
                        var _this = $(this);
                        if (_this.data("type") == 1) {
                            $("#editContactDefault,#deleteContact").hide();
                        } else {
                            $("#editContactDefault,#deleteContact").show();
                        }
                        var position = _this.find(".ico-dropdown").position();
                        $("#ddlContact li").data("id", _this.data("id")).data("name", _this.data("name"));
                        $("#ddlContact").css({ "top": position.top + 20, "left": position.left - 65 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                        return false;
                    });

                    $("#navContact .box-header").after(innerhtml);
                });
            } else {
                $("#navContact .box-header").after("<div class='nodata-box' >暂无数据!<div>");
            }
        });
    }

    //添加编辑联系人
    ObjectJS.addContact = function (model) {
        var _self = this;
        $("#show-model-detail").empty();
        doT.exec("template/customer/contact-detail.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-contact-detail",
                    header: !model ? "添加联系人" : "编辑联系人",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyContact.isPass()) {
                            return false;
                        }
                        var entity = {
                            ContactID: model ? model.ContactID : "",
                            CustomerID: _self.customerid,
                            Name: $("#name").val().trim(),
                            CityCode: CityContact.getCityCode(),
                            Address: $("#address").val().trim(),
                            MobilePhone: $("#contactMobile").val().trim(),
                            Email: $("#email").val().trim(),
                            Jobs: $("#jobs").val().trim(),
                            Type: model ? model.Type : 0,
                            Description: $("#remark").val().trim()
                        };
                        _self.saveContact(entity);
                    },
                    callback: function () {

                    }
                }
            });

            $("#name").focus();

            if (model) {
                $("#name").val(model.Name);
                $("#jobs").val(model.Jobs);
                $("#contactMobile").val(model.MobilePhone);
                $("#email").val(model.Email);
                $("#address").val(model.Address);
                $("#remark").val(model.Description);
            }

            CityContact = City.createCity({
                cityCode: model ? model.CityCode : "",
                elementID: "contactcity"
            });
            VerifyContact = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //保存联系人
    ObjectJS.saveContact = function (model) {
        var _self = this;
        Global.post("/Customer/SaveContact", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.ContactID) {
                /*处理客户联系人*/
                if (model.Type == 1 || $("#navContact ul").length < 2) {
                    $("#lblContactName").text(model.Name);
                }
                _self.getContacts(model.CustomerID);
                
            } else {
                alert("操作失败");
            }
        });
    }

    //编辑信息
    ObjectJS.editCustomer = function (model) {
        var _self = this;
        $("#show-contact-detail").empty();
        doT.exec("template/customer/customer-detail.html", function (template) {
            var innerText = template(model);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: "编辑客户信息",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var entity = {
                            CustomerID: model.CustomerID,
                            Name: $("#name").val().trim(),
                            Jobs: $("#jobs").val().trim(),
                            Type: $("#companyCustom").hasClass("ico-checked") ? 1 : 0,
                            IndustryID: $("#industry").val().trim(),
                            Extent: $("#extent").val().trim(),
                            CityCode: CityObject.getCityCode(),
                            Address: $("#address").val().trim(),
                            MobilePhone: $("#contactMobile").val().trim(),
                            Email: $("#email").val().trim(),
                            Description: $("#remark").val().trim()
                        };
                        _self.saveModel(entity);
                    },
                    callback: function () {

                    }
                }
            });

            CityObject = City.createCity({
                cityCode: model.CityCode,
                elementID: "city"
            });
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });

            if (model.Extent) {
                $("#extent").val(model.Extent);
            }

            if (model.IndustryID) {
                $("#industry").val(model.IndustryID);
            }
            //切换类型
            $(".customtype").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    $(".customtype").removeClass("ico-checked").addClass("ico-check");
                    _this.addClass("ico-checked").removeClass("ico-check");
                }
            });
        });
    }

    //保存客户
    ObjectJS.saveModel = function (model) {
        var _self = this;

        Global.post("/Customer/SaveCustomer", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.CustomerID) {
                location.href = location.href;
                
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }

    module.exports = ObjectJS;
});