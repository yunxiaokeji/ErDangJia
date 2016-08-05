define(function (require, exports, module) {
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot"),
        moment = require("moment"),
        Verify = require("verify"), VerifyObject;

    require("daterangepicker");
    require("jquery");
    require("pager");

    var ObjectJS = {};

    ObjectJS.Params = {
        pageIndex: 1,
        type: -1,
        status: -1,
        beginDate: '',
        endDate: '',
        keyWords: '',
        id: ''
    };

    //初始化
    ObjectJS.init = function (departs, option) {
        var _self = this;
        departs = JSON.parse(departs.replace(/&quot;/g, '"'));
        
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
        _self.bindEvent();
        _self.getDetail(departs); 
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(".bindloginmobile").hide();
        $("#btnExportExcel").click(function () {
            var form = $("<form>");//定义一个form表单
            form.attr("style", "display:none");
            form.attr("target", "");
            form.attr("method", "post");
            form.attr("action", "/MyAccount/ExportFromCfg");
            $("body").append(form);
            form.submit();//表单提交 
        });
        //用户基本信息
        $("#btnSaveAccountInfo").click(function () {
            if (!VerifyObject.isPass("#userInfo")) {
                return false;
            };
            _self.saveAccountInfo();
        });
        //切换模块
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if (_this.data("id") == "userInfo" && (!_this.data("frist") || _this.data("frist") == 0)) {
                _this.data("frist", "1");
                _self.getDetail();
            } else if (_this.data("id") == "userAccount" ) {
                
            } else if (_this.data("id") == "userImg" ) {
                
            } else if (_this.data("id") == "userPassWord" ) {
                 
            } else if (_this.data("id") == "UserFeedBack") {
                _self.bindData(); 
            }
        });
        $('#labelpassWord').click(function() {
            $(".tab-nav-ul li").eq(3).click();
        });

        /*我的反馈绑定*/
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                if (ObjectJS.Params.keyWords != keyWords) {
                    ObjectJS.Params.pageIndex = 1;
                    ObjectJS.Params.keyWords = keyWords;
                    ObjectJS.bindData();
                }
            });
        });

        //日期插件
        $("#iptCreateTime").daterangepicker({
            showDropdowns: true,
            empty: true,
            opens: "right",
            ranges: {
                '今天': [moment(), moment()],
                '昨天': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                '上周': [moment().subtract(6, 'days'), moment()],
                '本月': [moment().startOf('month'), moment().endOf('month')]
            }
        }, function (start, end, label) {
            ObjectJS.Params.PageIndex = 1;
            ObjectJS.Params.beginDate = start ? start.format("YYYY-MM-DD") : "";
            ObjectJS.Params.endDate = end ? end.format("YYYY-MM-DD") : "";
            ObjectJS.bindData();
        });
    }

    //获取用户详情
    ObjectJS.getDetail = function (departs) {

        Global.post("/MyAccount/GetAccountDetail", null, function (data) {
            if (data) {
                var item = data;
                //基本信息
                $("#Name").val(item.Name);
                $("#Jobs").val(item.Jobs);
                $('#labName').text(item.Name);
                $('#labDepart').text(item.DepartmentName);
                $('#labJob').text(item.Jobs);
                $('#imgtitle').attr("src", item.Avatar);
                var birthday = item.Birthday.toDate("yyyy-MM-dd");
                $("#Birthday").val(birthday != "3939-01-01" ? birthday : "");
                $("#Age").val(item.Age);
                //部门
                $("#DepartmentName").val(item.DepartmentName);
                $("#DepartID").val(item.DepartID);
                require.async("dropdown", function (item) {
                    $("#ddlDepart").dropdown({
                        prevText: "部门-",
                        defaultText: $("#DepartmentName").val(),
                        defaultValue: $("#DepartID").val(),
                        data: departs,
                        dataValue: "DepartID",
                        dataText: "Name",
                        width: "157",
                        isposition: true,
                        onChange: function (data) {
                            $("#DepartID").val(data.value);
                        }
                    });
                });

                //联系信息
                $("#MobilePhone").val(item.MobilePhone);
                $("#OfficePhone").val(item.OfficePhone);
                $("#Email").val(item.Email);
                 
                //绑定的手机号
                if (item.BindMobilePhone != '') {

                    $("#BindMobile").val(item.BindMobilePhone).attr("disabled", "disabled").hide();
                    $("#S_BindMobile").html(item.BindMobilePhone);

                    if (item.LoginName != '') {
                        $("#btnSaveAccountBindMobile").html("解绑").click(function () {
                            ObjectJS.SaveAccountBindMobile(2);
                        });
                    }
                    else {
                        $("#li-code").hide();
                        $("#div-mobile").hide();
                    }
                }
                else {
                    $("#btnSaveAccountBindMobile").html("绑定").click(function () {
                        ObjectJS.SaveAccountBindMobile(1);
                    });
                }

                //账户管理
                if (item.LoginName || item.BindMobilePhone) {
                    //设置密码
                    $("#LoginName").val(item.LoginName).attr("disabled", "disabled").hide();
                    $("#S_LoginName").html(item.LoginName);

                    $("#LoginOldPWD").blur(function () {
                        if ($(this).val() != '') {
                            Global.post("/MyAccount/ConfirmLoginPwd", { loginPwd: $(this).val() }, function (data) {

                                if (data.Result) {
                                    $("#LoginOldPWDError").html("");
                                } else {
                                    $("#LoginOldPWDError").html("原密码有误");
                                }
                            });
                        } else {
                            $("#LoginOldPWDError").html("原密码不能为空");
                        }
                    });
                } else {
                    //新增账户
                    $("#li_loginOldPWD").hide();
                    $("#LoginName").blur(function () {
                        if ($(this).val() != '') {
                            if ($(this).val().length > 4) {
                                $("#LoginNameError").html("");
                                Global.post("/MyAccount/IsExistLoginName", { loginName: $(this).val() }, function (data) {

                                    if (data.Result) {
                                        $("#LoginNameError").html("账户已存在");
                                    }
                                    else {
                                        $("#LoginNameError").html("");
                                    }
                                });
                            }
                            else {
                                $("#LoginNameError").html("账户名称过短");
                            }

                        }
                        else {
                            $("#LoginNameError").html("账户不能为空");
                        }
                    });
                }
            }
        });
    }

    //保存基本信息
    ObjectJS.saveAccountInfo = function () {
        var _self = this;
        var model = {
            Name: $("#Name").val(),
            Jobs: $("#Jobs").val(),
            Birthday: $("#Birthday").val(),
            Age: 0,
            DepartID: $("#DepartID").val(),
            MobilePhone: $("#MobilePhone").val(),
            OfficePhone: $("#OfficePhone").val(),
            Email: $("#Email").val()
        };

        Global.post("/MyAccount/SaveAccountInfo", { entity: JSON.stringify(model), departmentName: $("#DepartmentName").val() }, function (data) {
            if (data.Result == 1) {
                alert("保存成功");
            }
        })
    }

    //绑定发聩数据列表
    ObjectJS.bindData = function () {
        $(".tr-header").nextAll().remove();
        Global.post("/MyAccount/GetFeedBacks", ObjectJS.Params, function (data) {
            doT.exec("template/myaccount/myfeedback-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
                $(".a").bind("click", function () { ObjectJS.getFeedBackDetail($(this).data("id")); });
            });

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: ObjectJS.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    ObjectJS.Params.pageIndex = page;
                    ObjectJS.bindData();
                }
            });

        });
    }
    
    //反馈详情
    ObjectJS.getFeedBackDetail = function (id) {
        Global.post("/MyAccount/GetFeedBackDetail", { id: id }, function (data) {
            $("#show-contact-detail").empty();
            doT.exec("template/myaccount/myfeedback-detail.html?3", function (templateFun) {
                var innerText = templateFun(data.Item);
                Easydialog.open({
                    container: {
                        id: "show-model-detail",
                        header: "反馈详情",
                        content: innerText,
                        yesFn: function () {
                        },
                        callback: function () {
                        }
                    }
                });
                $(".edit-company").hide();
            });
        });
    };

    module.exports = ObjectJS;
});