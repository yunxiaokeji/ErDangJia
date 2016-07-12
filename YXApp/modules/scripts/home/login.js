define(function (require, exports, module) {
    require("jquery");
    var Global = require("global");
    var Home = {};

    //登陆初始化
    Home.initLogin = function (zngcClientID) {
        Home.zngcClientID = zngcClientID;
        Home.bindLoginEvent();
    }

    //绑定事件
    Home.bindLoginEvent = function () {

        $(document).on("keypress", function (e) {
            if (e.keyCode == 13) {
                $("#btnLogin").click();
            }
        });

        //登录
        $("#btnLogin").click(function () {
            if (!$("#iptUserName").val()) {
                $(".registerErr").html("请输入账号").slideDown();
                return;
            }
            if (!$("#iptPwd").val()) {
                $(".registerErr").html("请输入密码").slideDown();
                return;
            }

            $(this).html("登录中...").attr("disabled", "disabled");

            Global.post("/Home/UserLogin", {
                type: 2,
                userName: $("#iptUserName").val(),
                pwd: $("#iptPwd").val(),
                companyID: "",
                zngcClientID: Home.zngcClientID
            },
            function (data) {
                $("#btnLogin").html("登录").removeAttr("disabled");

                if (data.result == 1) {
                    //if (Home.returnUrl != '') {
                    //    location.href = Home.returnUrl;
                    //}
                    //else {
                    //    location.href = "/task/list";
                    //}
                    location.href = "/Product/ProductList";
                }
                else if (data.result == 0) {
                    $(".registerErr").html("账号或密码有误").slideDown();
                }
                else if (data.result == 2) {
                    $(".registerErr").html("密码输入错误超过3次，请2小时后再试").slideDown();
                }
                else if (data.result == 3) {
                    $(".registerErr").html("账号或密码有误,您还有" + (3 - parseInt(data.errorCount)) + "错误机会").slideDown();
                }
                else if (data.result == 4) {
                    $(".registerErr").html("该系统已绑定过阿里账户,不能再绑定");
                }
                else if (data.result == 5) {
                    alert("请重新阿里授权");
                    setTimeout(function () { location.href = "/home/login"; }, 500);
                }
                else if (data.result == -1) {
                    $(".registerErr").html("账号已冻结，请" + data.forbidTime + "分钟后再试").slideDown();
                }
            });
        });

        $(".txtBoxPassword").click(function () {
            $(this).hide();
            $("#iptPwd").focus();
        });

        $("#iptPwd").blur(function () {
            if ($(this).val() == '')
                $(".txtBoxPassword").show();
        }).focus(function () {
            if ($(this).val() == '')
                $(".txtBoxPassword").hide();
        });

    }

    Home.placeholderSupport = function () {
        if (!('placeholder' in document.createElement('input'))) {   // 判断浏览器是否支持 placeholder
            $('[placeholder]').focus(function () {
                var input = $(this);
                input.css("color", "#333");
                if (input.val() == input.attr('placeholder')) {
                    input.val('');
                    input.removeClass('placeholder');
                }

            }).blur(function () {
                var input = $(this);
                if (input.val() == '' || input.val() == input.attr('placeholder')) {
                    input.addClass('placeholder');
                    input.val(input.attr('placeholder')).css("color", "#999");
                }
            }).blur();

        };
    }

    module.exports = Home;
});