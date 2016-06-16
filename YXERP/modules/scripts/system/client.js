define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Upload = require("upload"),
        Easydialog = require("easydialog"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject,
        moment = require("moment");
    require("daterangepicker");

    require("pager");

    var ObjectJS = {};

    ObjectJS.Params = {
        PageSize: 10,
        PageIndex: 1,
        Status: -1,
        Type: -1,
        BeginDate: '',
        EndDate:''
    };

    //初始化
    ObjectJS.init = function (option) {
        var _self = this;

        _self.getDetail();
        _self.getClientOrders(1);
        _self.bindEvent();

        if (option !== 1) {
            $(".search-stages li[data-id='" + option + "']").click(); 
        }
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        $(document).click(function (e) {
            //隐藏下拉 
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        //切换
        $(".search-nav-box li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                $(".nav-part").hide();
                $("#" + _this.data("id")).show();
                if (_this.data("id") == "navOrderLog" && (!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    ObjectJS.getClientAuthorizeData(1);
                }
            }
        });

        //搜索
        require.async("dropdown", function () {
            var OrderStatus = [
                {
                    ID: "0",
                    Name: "未支付"
                },
                {
                    ID: "1",
                    Name: "已支付"
                },
                {
                    ID: "9",
                    Name: "已关闭"
                }
            ];
            $("#OrderStatus").dropdown({
                prevText: "订单状态-",
                defaultText: "所有",
                defaultValue: "-1",
                data: OrderStatus,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    $(".tr-header").nextAll().remove();
                    ObjectJS.Params.Status = parseInt(data.value);
                    ObjectJS.getClientOrders(1);
                }
            });

            var OrderTypes = [
                {
                    ID: "1",
                    Name: "购买系统"
                },
                {
                    ID: "2",
                    Name: "购买人数"
                },
                {
                    ID: "3",
                    Name: "续费"
                }
            ];
            $("#OrderTypes").dropdown({
                prevText: "订单类型-",
                defaultText: "所有",
                defaultValue: "-1",
                data: OrderTypes,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    $(".tr-header").nextAll().remove();
                    ObjectJS.Params.Type = parseInt(data.value);
                    ObjectJS.getClientOrders(1);
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
            ObjectJS.Params.BeginDate = start ? start.format("YYYY-MM-DD") : "";
            ObjectJS.Params.EndDate = end ? end.format("YYYY-MM-DD") : "";
            ObjectJS.getClientOrders(1);
        });

        //继续支付客户端订单
        $("#PayClientOrder").click(function () {
            var id = $(this).data("id"), type = $(this).data("type");

            var url = "/Auction/BuyNow";
            if (type == 2) {
                url = "/Auction/BuyUserQuantity";
            } else if (type == 3) {
                url = "/Auction/ExtendNow";
            }
            url += "/" + id;

            location.href = url;
        });

        //关闭客户端订单
        $("#CloseClientOrder").click(function () {
            Global.post("/System/CloseClientOrder",{id:$(this).data("id")},function(data){
                if (data.Result == 1) {
                    ObjectJS.getClientOrders();
                }
                else {
                    alert("关闭失败");
                }
            });
        });

        //绑定编辑客户信息
        $("#updateClient").click(function () {
            ObjectJS.editClient();
        });
    }

    //获取详情
    ObjectJS.getDetail = function () {
        var _self = this;
        Global.post("/System/GetClientDetail", null, function(data) {
            if (data.Client) {
                var item = data.Client;
                _self.item = data.Client;
                //基本信息 
                $("#divCompanyName").html(item.CompanyName);
                $("#lblCompanyName").html(item.CompanyName);
                $("#lblContactName").html(item.ContactName);
                $("#lblMobilePhone").html(item.MobilePhone);
                $("#lblOfficePhone").html(item.OfficePhone);
                $("#lblIndustry").html(item.IndustryEntity != null ? item.IndustryEntity.Name : "");
                if (item.City) {
                    $("#lblcitySpan").html(item.City ? item.City.Description : "--");
                }
                if (item.Logo) {
                    $("#posterDisImg").attr("src", item.Logo);
                }
                $("#lblAddress").html(item.Address);
                //$("#lblDescription").html(item.Description);

                //授权信息
                var agent = data.Agent;
                $("#UserQuantity").html(agent.UserQuantity);
                $("#EndTime").html(agent.EndTime.toDate("yyyy-MM-dd"));
                $("#agentRemainderDays").html(data.Days);

                if (agent.authorizeType == 0) {
                    $(".btn-buy").html("立即购买");
                }
                else {
                    if (parseInt(data.remainderDays) < 31) {
                        $("#agentRemainderDays").addClass("red");
                        $(".btn-buy").html("续费").data("url", "/Auction/ExtendNow");
                    }
                    else {
                        $(".btn-buy").html("购买人数").data("url", "/Auction/BuyUserQuantity");
                    }
                }
            }
        });
    }

    ObjectJS.editClient = function () {
        var _self = this;
        $("#show-contact-detail").empty();
        doT.exec("template/system/client-detail.html", function (template) {
            var innerText = template(_self.item);
            Easydialog.open({
                container: {
                    id: "#show-model-detail",
                    header: "编辑公司信息",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var modules = [];
                        var entity = {
                            CompanyName: $("#CompanyName").val(),
                            Logo: $("#posterDisImg").attr("src"),
                            ContactName: $("#ContactName").val(),
                            MobilePhone: $("#MobilePhone").val(),
                            OfficePhone: $("#OfficePhone").val(),
                            CityCode: CityObject.getCityCode(),
                            Industry: $("#Industry").val(),
                            Address: $("#Address").val(),
                            Description: ""//$("#Description").val() 
                        };
                        ObjectJS.saveModel(entity);
                    },
                    callback: function () {

                    }
                }
            });
            ObjectJS.Setindustry(_self.item);
            CityObject = City.createCity({
                cityCode: _self.item.CityCode,
                elementID: "citySpan"
            });
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
            //选择海报图片
            PosterIco = Upload.createUpload({
                element: "#Logo",
                buttonText: "选择LOGO",
                className: "",
                data: { folder: '', action: 'add', oldPath: _self.item.Logo },
                success: function (data, status) {
                    if (data.Items.length > 0) {
                        $("#posterDisImg").attr("src", data.Items[0]);
                    } else {
                        alert("只能上传jpg/png/gif类型的图片，且大小不能超过10M！");
                    }
                }
            });
        });
    }

    ObjectJS.Setindustry = function (model) {
        $('#Industry').html($('#industrytemp').html());
        $('#Industry').val(model.Industry || '');
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this; 
        Global.post("/System/SaveClient", { entity: JSON.stringify(model) }, function(data) {
            if (data.Result == 1) {
                alert("保存成功");
                ObjectJS.getDetail();
            }
        });
    }

    //获取客户端的订单列表
    ObjectJS.getClientOrders = function(index) {
        var _self = this;
        $("#client-order").nextAll().remove();
        $("#client-order").after("<tr><td colspan='8'><div class='data-loading' ><div></td></tr>");
        Global.post("/System/GetClientOrders", {
            pageSize: ObjectJS.Params.PageSize,
            pageIndex: index,
            status: ObjectJS.Params.Status,
            type: ObjectJS.Params.Type,
            beginDate: ObjectJS.Params.BeginDate,
            endDate: ObjectJS.Params.EndDate
        },
            function (data) {
                $("#client-order").nextAll().remove();
                if (data.Items.length > 0) {
                    doT.exec("template/system/client-orders.html", function (template) {
                        var innerhtml = template(data.Items);
                        innerhtml = $(innerhtml);
                        $("#client-order").after(innerhtml);
                        //下拉事件
                        innerhtml.find(".dropdown").click(function () {
                            var _this = $(this);
                            var position = _this.find(".ico-dropdown").position();
                            $(".dropdown-ul li").data("id", _this.data("id")).data("type", _this.data("type"));
                            $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 80 }).show().mouseleave(function () {
                                $(this).hide();
                            });
                            return false;
                        });
                    });
                } else {
                    $("#client-order").after("<tr><td colspan='8'><div class='nodata-txt' >暂无数据!<div></td></tr>");
                }
                $("#pager").paginate({
                    total_count: data.TotalCount,
                    count: data.PageCount,
                    start: index,
                    display: 5,
                    images: false,
                    mouse: 'slide',
                    onChange: function (page) {
                        _self.getClientOrders(page);
                    }
                });
            }
        );
    };

    //获取授权记录 
    ObjectJS.getClientAuthorizeData = function (index) {
        var _self = this;
        $("#client-header").nextAll().remove();
        _self.Params.pageIndex = index;
        Global.post("/System/GetClientAuthorizeLogs", _self.Params, function (data) { 
            if (data.Items.length > 0) {
                doT.exec("template/system/client-authorizelog.html?1", function(templateFun) {
                    var innerText = templateFun(data.Items);
                    innerText = $(innerText);
                    $("#client-header").after(innerText);
                });
            } else {
                $("#client-header").after("<tr><td colspan='6'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
            $("#pager2").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: index,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    _self.getClientAuthorizeData(page);
                }
            });
        });
    };

    module.exports = ObjectJS;
});