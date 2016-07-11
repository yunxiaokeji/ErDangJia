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
            Global.post("/Home/IsExistAccount", {
                type: 2,
                account: $("#loginName").val(),
                companyID: "",
                name: $("#name").val(),
                customerID: ObjectJS.customerID,
                clientID: ObjectJS.clientID,
                verification:1
            }, function (data) {
                if (data.result == 1) {

                } else {
                    
                }
            });
        });

        /*发送验证码*/
        $("#btnSendMsg").click(function () {
            if ($("#loginName").val() == '') {
                $("#code-error").fadeIn().find(".error-msg").html("手机号不能为空");
                return;
            }
            else {
                if (Global.validateMobilephone($("#loginName").val())) {
                    Global.post("/Home/IsExistAccount", {
                        type: 2,
                        account: $("#loginName").val(),
                        verification:2
                    }, function (data) {
                        if (data.result == 1) {
                            $(".registerErr").html("").slideUp();
                            ObjectJS.SendMobileMessage("btnSendMsg", $("#loginName").val());
                        } else {
                            $(".registerErr").html("手机号已被注册").slideDown();

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
        });
    }

    module.exports = ObjectJS;
});