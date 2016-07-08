

define(function (require, exports, module) {

    require("jquery");

    var Global = require("global")

    var ObjectJS = {};

    ObjectJS.init = function (customerID, clientID) {
        ObjectJS.customerID = customerID;
        ObjectJS.clientID = clientID;
        ObjectJS.getCustomerBaseInfo();
        ObjectJS.bindEvent();
    };

    ObjectJS.bindEvent = function () {
        $("#btnRegister").click(function () {
            if (!$("#loginName").val()) {
                $(".registerErr").html("请输入账号").slideDown();
                return;
            }
        });
    };

    //判断用户是否存在
    ObjectJS.getCustomerBaseInfo = function () {
        Global.post("/Home/GetCustomerBaseInfo", { customerID: ObjectJS.customerID, clientID: ObjectJS.clientID }, function (data) {
            console.log(data);
        });
    };

    module.exports = ObjectJS;
});