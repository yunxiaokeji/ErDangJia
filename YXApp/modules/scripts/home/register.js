

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
        $("#btnRegister").click(function () {
            //if (!$("#intfactoryName").val()) {
            //    $(".registerErr").html("请输入工厂名称").slideDown();
            //    return;
            //}
            //if (!$("#name").val()) {
            //    $(".registerErr").html("请输入姓名").slideDown();
            //    return;
            //}
            if (!$("#loginName").val()) {
                $(".registerErr").html("请输入账号").slideDown();
                return;
            }
            //if (!$("#code").val()) {
            //    $(".registerErr").html("请输入验证码").slideDown();
            //    return;
            //}
            Global.post("/Home/IsExistAccount", {
                type: 2,
                account: $("#loginName").val(),
                companyID: "",
                name: $("#name").val(),
                customerID:ObjectJS.customerID
            }, function (data) {
                if (data.result == 1) {

                } else {
                    alert("帐号已存在");
                }
            });
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

    module.exports = ObjectJS;
});