
/*
    --选择客户插件--
    --引用
    choosecustomer = require("choosecustomer");
    choosecustomer.create({});
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global"),
        doT = require("dot"),
        City = require("city"), CityOrder,
        Easydialog = require("easydialog");

    require("plug/choosecustomer/style.css");

    var PlugJS = function (options) {
        var _this = this;
        _this.setting = $.extend([], _this.default, options);
        _this.init();
    }

    //默认参数
    PlugJS.prototype.default = {
        title:"选择客户", //标题
        isAll: false,
        cacheTypes: [],
        callback: null   //回调
    };

    PlugJS.prototype.init = function () {

        var _self = this;

        doT.exec("/plug/choosecustomer/choosecustomer.html", function (template) {
            var innerHtml = template({});

            Easydialog.open({
                container: {
                    id: "choose-customer-add",
                    header: _self.setting.title,
                    content: innerHtml,
                    yesFn: function () {
                        if (_self.customerid) {
                            _self.setting.callback && _self.setting.callback({
                                customerid: _self.customerid,
                                typeid: $("#orderTypes").data("id"),
                                name: $("#orderContact").val().trim(),
                                mobile: $("#orderMobile").val().trim(),
                                address: $("#orderAddress").val().trim(),
                                remark: $("#orderRemark").val().trim(),
                                cityCode: CityOrder.getCityCode()
                            });
                        } 
                    },
                    callback: function () {

                    }
                }
            });
            //绑定事件
            _self.bindEvent();
        });
    };

    //绑定事件
    PlugJS.prototype.bindEvent = function () {
        var _self = this;
        //搜索
        require.async("autocomplete", function () {
            $("#choosecustomerSearch").autocomplete({
                url: "/Customer/GetCustomersByKeywords",
                placeholder:"输入客户名称、联系电话......",
                keywords: "keywords",
                params: {
                    isAll: _self.setting.isAll ? 1 : 0
                },
                width: "700",
                isposition:true,
                asyncCallback: function (data, response) {
                    response($.map(data.items, function (item) {
                        return {
                            text: item.Name + "(联系人：" + (item.ContactName || "--") + ")",
                            name: item.Name,
                            id: item.CustomerID
                        }
                    }));
                },
                select: function (item) {
                    _self.customerid = item.value;
                    $(".choosecustomer-body").empty();
                    $(".choosecustomer-body").append("<div class='data-loading'><div>")
                    Global.post("/Customer/GetCustomerAndContactByID", {
                        customerid: item.value
                    }, function (data) {
                        var model = data.model;
                        $(".choosecustomer-body").empty();
                        doT.exec("template/customer/create-order.html", function (template) {
                            var innerHtml = template(model);
                            $(".choosecustomer-body").append(innerHtml);

                            require.async("dropdown", function () {
                                $("#orderTypes").dropdown({
                                    prevText: "",//文本前缀
                                    defaultText: _self.setting.cacheTypes[0] && _self.setting.cacheTypes[0].TypeName,
                                    defaultValue: _self.setting.cacheTypes[0] && _self.setting.cacheTypes[0].TypeID,
                                    data: _self.setting.cacheTypes,
                                    dataValue: "TypeID",
                                    dataText: "TypeName",
                                    width: "180",
                                    isposition: true,
                                    onChange: function () { }
                                });
                                $("#orderContacts").dropdown({
                                    prevText: "",//文本前缀
                                    defaultText: "选择联系人",
                                    defaultValue: "",
                                    data: model.Contacts,
                                    dataValue: "ContactID",
                                    dataText: "Name",
                                    width: "180",
                                    isposition: true,
                                    onChange: function (contact) {
                                        for (var i = 0; i < model.Contacts.length; i++) {
                                            if (model.Contacts[i].ContactID == contact.value) {
                                                var item = model.Contacts[i];
                                                $("#orderContact").val(item.Name || $("#orderContact").val());
                                                $("#orderMobile").val(item.MobilePhone || $("#orderMobile").val());
                                                $("#orderAddress").val(item.Address || $("#orderAddress").val());
                                                CityOrder.setValue(item.CityCode || CityOrder.getCityCode());
                                                break;
                                            }
                                        }
                                    }
                                });
                            });

                            CityOrder = City.createCity({
                                cityCode: model.CityCode,
                                elementID: "orderCity"
                            });
                        });
                    });
                }
            });
        });
    }

    exports.create = function (options) {
        return new PlugJS(options);
    }
});