define(function (require, exports, module) {
    require("jquery");
    require("verify");
    var Global = require("global");

    var ObjectJS = {};

    ObjectJS.init = function (customerID, clientID) {
        ObjectJS.customerID = customerID;
        ObjectJS.clientID = clientID;
        if (ObjectJS.customerID != "") {
            ObjectJS.getCustomerBaseInfo();
        }
        ObjectJS.bindEvent();
    };

    ObjectJS.bindEvent = function () {
        /*注册*/
        $("#btnRegister").click(function () {
            if ($("#btnRegister").data('isLoadding') != 1) {
                if (!$("#name").val()) {
                    $(".registerErr").html("请输入姓名").slideDown();
                    return;
                }
                if (!$("#loginName").val()) {
                    $(".registerErr").html("请输入手机号").slideDown();
                    return;
                }
                if (!$("#code").val()) {
                    $(".registerErr").html("请输入验证码").slideDown();
                    return;
                }
                if (Global.validateMobilephone($("#loginName").val())) {
                    $("#btnRegister").html("注册中...");
                    $("#btnRegister").data('isLoadding', 1);
                    Global.post("/Home/RegisterClient", {
                        type: 2,
                        account: $("#loginName").val(),
                        name: $("#name").val(),
                        customerID: ObjectJS.customerID,
                        zngcClientID: ObjectJS.clientID,
                        code: $("#code").val()
                    }, function (data) {
                        $("#btnRegister").html("注册");
                        $("#btnRegister").data('isLoadding', 0);
                        if (data.result == 1) {
                            if (data.status == 1) {
                                alert("注册成功");
                                //location.href = "/Product/ProductList";
                            } else if (data.status == 2) {
                                confirm("手机号已注册，登陆后自动绑定供应商,是否前往登陆?", function () {
                                    location.href = "/Home/Login?" + "zngcClientID=" + data.zngcClientID + "";
                                });
                            } else {
                                alert("注册失败");
                            }
                        } else if (data.result == 2) {
                            confirm("手机号已注册，登陆后自动绑定供应商,是否前往登陆?", function () {
                                location.href = "/Home/Login?" + "zngcClientID=" + data.zngcClientID + "";
                            });
                        } else if (data.result == 3) {
                            alert('验证码有误');
                        } else {
                            alert("注册失败");
                        }
                    });
                } else {
                    $(".registerErr").html("请输入正确手机号").slideDown();
                    return;
                }
            }
        });

        /*测试！！切换供应商注册*/
        $(".test-reg").click(function () {
            var _this = $(this);
            if (_this.data('is') == 1) {
                _this.data('is', 2);
                ObjectJS.clientID = "a89cbb94-e32b-4f99-bab9-2db1d9cff607";
                alert("当前客户端ID：a89cbb94-e32b-4f99-bab9-2db1d9cff607");
                $("#loginCompany").html("-云销科技");
            } else {
                _this.data('is', 1);
                ObjectJS.clientID = "e3aa5f69-0362-450d-a6f2-a9a055c11d59";
                alert("当前客户端ID：e3aa5f69-0362-450d-a6f2-a9a055c11d59");
                $("#loginCompany").html("-诸暨市大唐斯达特针纺织厂");
            }
            ObjectJS.customerID = $(".customerid").val();
        });

        /*发送验证码*/
        $("#btnSendMsg").click(function () {
            if (!Global.validateMobilephone($("#loginName").val())) {
                $(".registerErr").html("请输入正确手机号").slideDown();
                return;
            }
            if ($("#loginName").val() == '') {
                $(".registerErr").html("请输入手机号").slideDown();
                return;
            }
            else {
                if (Global.validateMobilephone($("#loginName").val())) {
                    Global.post("/Home/IsExistAccount", {
                        type: 2,
                        account: $("#loginName").val()
                    }, function (data) {
                        if (data.result == 1) {
                            $(".registerErr").html("").slideUp();
                            ObjectJS.SendMobileMessage("btnSendMsg", $("#loginName").val());
                        } else {
                            confirm("手机号已注册，登陆后自动绑定供应商,是否前往登陆?", function () {
                                location.href = "/Home/Login?" + "zngcClientID=" + ObjectJS.clientID + "";
                            });
                        }
                    });
                }
                else {
                    $(".registerErr").html("请输入正确手机号").slideDown();
                    return;
                }
            }
        });
    };

    /*通过智能工厂ID获取用户信息*/
    ObjectJS.getCustomerBaseInfo = function () {
        Global.post("/Home/GetCustomerBaseInfo", { customerID: ObjectJS.customerID, clientID: ObjectJS.clientID }, function (data) {
            $("#loginCompany").html(data.clientName == "" ? "" : "-" + data.clientName);
            var customer = data.item.customer;
            $("#name").val(customer.name);
            $("#loginName").val(customer.mobilePhone);
        });
    };

    /*发送手机验证码*/
    var timeCount = 60;
    var interval = null;
    ObjectJS.SendMobileMessage = function (id, mobilePhone) {
        var $btnSendCode = $("#" + id);
        $btnSendCode.attr("disabled", "disabled");

        $("#" + id).css("background-color", "#aaa");
        interval = setInterval(function () {
            var $btnSendCode = $("#" + id);
            timeCount--;
            $btnSendCode.val(timeCount + "秒后重发");

            if (timeCount == 0) {
                clearInterval(interval);
                timeCount = 60;
                $btnSendCode.val("获取验证码").css("background-color", "#4a98e7");
                $btnSendCode.removeAttr("disabled");
            }

        }, 1000);

        Global.post("/Home/SendMobileMessage", { mobilePhone: mobilePhone }, function (data) {
            if (data.Result == 0) {
                alert("验证码发送失败");
            }
            alert("验证码是：" + data.code);
            $(".test-reg").html("验证码：" + data.code);
        });
    }

    module.exports = ObjectJS;
});