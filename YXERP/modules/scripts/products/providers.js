
define(function (require, exports, module) {
    var City = require("city"), BrandCity,
        Global = require("global"),
        Verify = require("verify"), VerifyObject,
        doT = require("dot"),
        Easydialog = require("easydialog");
    require("pager");
    var Params = {
        keyWords: "",
        type: -1,
        pageSize: 20,
        pageIndex: 1,
        totalCount: 0
    };
    var ObjectJS = {};

    //列表页初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.getList();
        _self.bindEvent();
    }
    //绑定列表页事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                _self.getList();
            });
        });

        //类型
        $(".search-type li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.type = _this.data("id");
                _self.getList();
            }
        });

        //添加
        $("#addObject").click(function () {
            _self.createModel();
        });
    }

    //添加/编辑弹出层
    ObjectJS.createModel = function (model) {
        var _self = this;

        doT.exec("template/products/provider-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !model ? "添加供应商" : "编辑供应商",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var entity = {
                            ProviderID: model ? model.ProviderID : "",
                            Name: $("#providerName").val().trim(),
                            Contact: $("#contact").val().trim(),
                            MobileTele: $("#mobiletele").val().trim(),
                            CityCode: BrandCity.getCityCode(),
                            Address: $("#address").val().trim(),
                            Remark: $("#description").val().trim()
                        };
                        Global.post("/Products/SavaProviders", { entity: JSON.stringify(entity) }, function (data) {
                            if (data.ID.length > 0) {
                                _self.getList();
                            }
                        });                    },
                    callback: function () {

                    }
                }
            });
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });


            $("#providerName").focus();

            if (model) {

                $("#providerName").val(model.Name);
                $("#contact").val(model.Contact);
                $("#mobiletele").val(model.MobileTele);
                $("#address").val(model.Address)
                $("#description").val(model.Remark);

                BrandCity = City.createCity({
                    elementID: "city",
                    cityCode: model.CityCode
                });
            } else {
                _self.IcoPath = "";
                BrandCity = City.createCity({
                    elementID: "city"
                });
            }
        });
    }

    //获取品牌列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='7'><div class='data-loading' ><div></td></tr>");

        Global.post("/Products/GetProviders", Params, function (data) {
            $(".tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/products/providers.html", function (templateFun) {
                    var innerText = templateFun(data.items);
                    innerText = $(innerText);
                    $(".tr-header").after(innerText);

                    //删除
                    innerText.find(".ico-del").click(function () {
                        var _this = $(this), msg = "供应商删除后不可恢复,确认删除吗？";

                        if (_this.data("type") == 2) {
                            msg = "此供应商为商家店铺，删除后不再关注此店铺，确认删除吗？";
                        }
                        confirm(msg, function () {
                            Global.post("/Products/DeleteProvider", { id: _this.data("id") }, function (data) {
                                if (data.result == 1) {
                                    alert("删除成功");
                                    _self.getList();
                                } else if (data.result == 10002) {
                                    alert("存在关联数据，删除失败");
                                } else if (data.result == 10003) {
                                    alert("删除失败，系统内至少保留一个供应商");
                                } else {
                                    alert("删除失败！");
                                }
                            });
                        });
                    });

                    //编辑
                    innerText.find(".ico-edit").click(function () {
                        var _this = $(this);

                        Global.post("/Products/GetProviderDetail", { id: _this.data("id") }, function (data) {
                            var model = data.model;
                            _self.createModel(model);
                        });
                    });
                });
            }
            else {
                $(".tr-header").after("<tr><td colspan='7'><div class='nodata-txt' >暂无数据!</div></td></tr>");
            }

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Params.pageIndex,
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
                onChange: function (page) {
                    Params.pageIndex = page;
                    _self.getList();
                }
            });
        });
    }

    module.exports = ObjectJS;
})