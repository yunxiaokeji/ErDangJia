define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject;


    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (activityid) {
        var _self = this;
        _self.activityid = activityid;
        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        if (_self.activityid) {
            $("#source option[data-code='Source-Activity']").prop("selected", true);
            $("#source").prop("disabled", true);
            Global.post("/Customer/GetActivityBaseInfoByID", { activityid: _self.activityid }, function (data) {
                if (data.model.Name) {
                    $("#activityName").html("活动：" + data.model.Name);
                }
            });
            $('#activity').val(_self.activityid);
        } else {
            if ($('#source').children(":selected").data("code") == "Source-Activity") {
                $('#activity').show();
            }
        }
        $('#source').bind("change", function () {
            if ($(this).children(":selected").data("code") == "Source-Activity") {
                $('#activity').show();
            } else {
                $('#activity').hide();
            }
        });
        //保存
        $("#btnSave").click(function () { 
            if (_self.isLoading) {
                alert("数据处理中，请勿重复操作");
                return false;
            }
            if (!VerifyObject.isPass()) {
                return false;
            }
            _self.isLoading = true;
            _self.saveModel();
        });

       

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
        CityObject = City.createCity({
            elementID: "city"
        });
        //切换类型
        $(".customtype").click(function () {
            var _this = $(this);
            if (!_this.hasClass("ico-checked")) {
                $(".customtype").removeClass("ico-checked").addClass("ico-check");
                _this.addClass("ico-checked").removeClass("ico-check");
            }
        });

        $("#name").focus();
    }

    //保存实体
    ObjectJS.saveModel = function () {
        console.log($('#source').children(":selected").data("code"));
        console.log($('#activity').val());
        var _self = this; 
        var model = {
            Name: $("#name").val().trim(),
            Type: $("#companyCustom").hasClass("ico-checked") ? 1 : 0,
            IndustryID: $("#industry").val().trim(),
            ActivityID: $('#source').children(":selected").data("code") == "Source-Activity"?$('#activity').val():"",
            SourceID: $("#source").val().trim(),
            Extent: $("#extent").val().trim(),
            CityCode: CityObject.getCityCode(),
            Address: $("#address").val().trim(),
            ContactName: $("#contactName").val().trim(),
            MobilePhone: $("#contactMobile").val().trim(),
            Email: $("#email").val().trim(),
            Jobs: $("#jobs").val().trim(),
            Description: $("#remark").val().trim()
        };
        Global.post("/Customer/SaveCustomer", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.CustomerID) {
                alert("客户创建成功", function () {
                    location.href = "/Customer/Detail/" + data.model.CustomerID;
                    _self.isLoading = false;
                });
            } else {
                alert("客户创建失败,请稍后重试!");
                _self.isLoading = false;
            }
            
        });
    }

    module.exports = ObjectJS;
});