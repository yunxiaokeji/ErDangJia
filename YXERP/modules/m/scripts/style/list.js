define(function (require,exports,module) {
    var Global = require("m_global"),
        doT = require("dot");
    var City = require("city"), CityInvoice;
    var Params = {
        pageSize: 5,
        pageIndex: 1        
    };
    
    var taskParms = {
        taskID: "",
        replyPageIndex: 1,
        logPageIndex: 1,
        endTime: ""
    }

    var ObjectJS = {};
    var cacheOrder = [];
    ObjectJS.PageCount = 0;
    ObjectJS.IsLoading = false;
    ObjectJS.init = function (providers,user) {
        providers = JSON.parse(providers.replace(/&quot;/g, '"'));
        user = JSON.parse(user.replace(/&quot;/g, '"'));
        ObjectJS.user = user;
        for (var i = 0; i < providers.length; i++) {    
            var p=providers[i];
            $(".task-filtertype").append('<li data-id="' + p.ProviderID + '">' + p.Name + '</li>');
        }
        ObjectJS.bindEvent();
        ObjectJS.getList();  
    };

    ObjectJS.bindEvent = function () {
        //滚动加载数据
        $(window).scroll(function () {
            if (document.body.scrollTop > 30) {
                $(".getback").slideDown("slow");
            } else {
                $(".getback").slideUp("slow");
            }
            var bottom = $(document).height() - document.documentElement.scrollTop - document.body.scrollTop - $(window).height();
            if (bottom <= 200) {
                if (!ObjectJS.IsLoading) {
                    Params.pageIndex++;
                    if (Params.pageIndex <= ObjectJS.PageCount) {
                        ObjectJS.getList(true);
                    } else {
                        $(".prompt").remove();
                        $(".list").append('<div class="prompt">已经是最后一条啦</div>');
                    }
                }
            }
        });

        //页面点击
        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("btn-menu") && !$(e.target).hasClass("btn-menu")) {
                $(".menu-box").slideUp(400);
            }
            if (!$(e.target).parents().hasClass("filter-task") && !$(e.target).hasClass("filter-task")) {
                $(".dropdownlist .potion").slideUp(400);
            }
            if (!$(e.target).parents().hasClass("btn-task-filtertype") && !$(e.target).hasClass("btn-task-filtertype")) {
                $(".task-filtertype").slideUp(400);
            }
        });

        //显示关键字遮罩层
        $(".iconfont-search").click(function () {
            $(".btn-search").text("确定");
            $(".txt-search").val("").focus();
            $(".shade,.search").show();
            $(".span-search").css("width", (document.body.clientWidth - 150) + "px");
        });

        //关键字查询
        $(".btn-search").click(function () {
            var name = $(this).text();
            if (name == "确定") {
                var txt = $(".txt-search").val();
                if (txt != "") {
                    $(".shade").slideUp("slow");
                    $(this).text("取消");

                    Params.pageIndex = 1;
                    Params.keyWords = txt;
                    ObjectJS.getList();
                    
                } else {
                    $(".search").hide();
                }
            } else {
                $(".search").hide();

                Params.keyWords = "";
                ObjectJS.getList();
            }
            $(".shade").hide();
        });

        //搜索内容发生变化
        $(".txt-search").keyup(function () {
            
            var changeAfter = $(".txt-search").val();
            if (changeAfter == "") {
                $(".cencal").text("取消");
            } else if (Params.keyWords == changeAfter) {
                $(".cencal").text("取消");
            } else {
                $(".cencal").text("确定");
            }
        });

        //点击遮罩层空白区域
        $(".shade").click(function () {
            $(".shade").hide();
            $(".search").hide();
        });

        //显示主菜单
        $(".btn-menu").click(function () {
            $(".menu-box").slideToggle(400);
        });

        //显示任务过滤类型
        $(".btn-task-filtertype").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $(".task-filtertype").slideDown();
            } else {
                _this.removeClass("hover");
                $(".task-filtertype").slideUp();
            }            
        });

        //任务过滤类型切换
        $(".task-filtertype li").click(function () {
            $(".btn-task-filtertype div:first").text($(this).text());
            $(this).parent().hide();

            Params.filtertype = $(this).data("filtertype");
            ObjectJS.getList();
        });

        //任务状态切换
        $(".task-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover").siblings().removeClass("hover");
            }

            Params.pageIndex = 1;
            Params.finishStatus = $(this).data("status");
            ObjectJS.getList();
        });

        //显示过滤下拉框
        $(".filter-task li").click(function () {
            var slideLi = $("." + $(this).data("id"));
            slideLi.slideToggle(400).siblings().slideUp("slow");
        });

        //订单类型切换
        $(".order-type li").click(function () {
            $(".type-span").text($(this).text());
            $(this).parent().hide();

            Params.pageIndex = 1;
            Params.orderType = $(this).data("id");
            ObjectJS.getList();
        });

        //任务排序
        $(".task-sort li").click(function () {
            $(".sort-span").text($(this).text());
            $(this).parent().hide();
            
            Params.isAsc = $(this).data("takepo");
            Params.taskOrderColumn = $(this).data("id");
            ObjectJS.getList();
        });
        
        //返回顶部
        $(".getback").click(function () {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        });
    };

    ObjectJS.getList = function (noEmpty) {
        if (!noEmpty) {
            $(".list").empty();
        }
        //获取任务列表(页面加载)
        $(".list").append('<div class="data-loading"></div>');
        $.post("/IntFactoryOrder/GetProductList", Params, function (data) {
            $(".data-loading").remove();
            if (data.items.length == 0) {
                $(".list").append("<div class='nodata-txt'>暂无数据 !</div>");
            } else {
                ObjectJS.PageCount = data.pageCount;
                doT.exec("m/template/style/style-list.html", function (code) {
                    var innerHtml = code(data.items);
                    innerHtml = $(innerHtml);
                    $(".list").append(innerHtml);

                    innerHtml.find(".btn-addOrder").click(function () {
                        var _this = $(this);
                        var id = _this.data('orderid');
                        if (!cacheOrder[id]) {
                            Global.post("../IntFactoryOrders/GetOrderDetail", {
                                orderID: id,
                                clientID: _this.data('clientid')
                            }, function (data) {
                                if (data.result.error_code == 0) {
                                    cacheOrder[id] = data.result.order;
                                    doT.exec("m/template/style/style-buy.html", function (code) {
                                        var innerHtml = code(cacheOrder[id]);
                                        innerHtml = $(innerHtml);

                                        innerHtml.find("#customerName").val(ObjectJS.user.Name);
                                        innerHtml.find("#customerTel").val(ObjectJS.user.MobilePhone);
                                        innerHtml.find("#customerAddress").val(ObjectJS.user.Address);


                                        $(".overlay-addOrder").html(innerHtml).show();
                                        
                                        $(".overlay-addOrder .style-content").animate({ height: "450px" }, 200);

                                        $(".overlay-addOrder").unbind().click(function (e) {
                                            if (!$(e.target).parents().hasClass("style-content") && !$(e.target).hasClass("style-content")) {
                                                $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                                                    $(".overlay-addOrder").hide();
                                                });
                                            }
                                        });

                                        $(".overlay-addOrder .close").unbind().click(function () {
                                            $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                                                $(".overlay-addOrder").hide();
                                            });
                                        });

                                        $(".overlay-addOrder .attr-ul li.size,.overlay-addOrder .attr-ul li.color").unbind().click(function () {
                                            var _self = $(this);
                                            if (_self.hasClass("select")) {
                                                _self.removeClass("select");
                                            } else {
                                                _self.addClass("select");
                                            }
                                            ObjectJS.createOrderDetails();
                                        });

                                        $(".overlay-addOrder .btn-sureAdd").unbind().click(function () {
                                            var model = {
                                                OrderID: cacheOrder[id].orderID,
                                                PersonName: $("#customerName").val(),
                                                MobileTele: $("#customerTel").val(),
                                                CityCode: CityInvoice.getCityCode(),
                                                Address: $("#customerAddress").val(),
                                                GoodsName: cacheOrder[id].goodsName,
                                                GoodsID: cacheOrder[id].goodsID,
                                                ClientID: cacheOrder[id].clientID,
                                                IntGoodsCode: cacheOrder[id].intGoodsCode,
                                                CategoryID: cacheOrder[id].categoryID,
                                                FinalPrice: cacheOrder[id].finalPrice,
                                                OrderImage: cacheOrder[id].orderImage,
                                                Details: []
                                            };
                                            ObjectJS.createOrders(model);
                                            return false;
                                        });

                                        innerHtml.find('.layout-attr').change(function () {
                                            var _this = $(this);
                                            var isContinue = true;
                                            var _thisTitle=_this.parents('.productsalesattr').find('.attr-title');
                                            if (!_this.val().trim()) {
                                                _this.val('');
                                                return false;
                                            }
                                            _this.parent().siblings().each(function () {
                                                var obj = $(this);
                                                if (_this.val().trim() == obj.text().trim()) {
                                                    alert(obj.data('type') == 1 ? "" + _thisTitle.text() + "已存在" : "" + _thisTitle.text() + "已存在");
                                                    isContinue = false;
                                                    return false;
                                                }
                                            });
                                            if (isContinue) {
                                                var attrObj = $("<li class='" + (_thisTitle.data('type') == 1 ? "size" : "color") + "' data-type='" + _thisTitle.data('type') + "' data-id='|' data-value='" + _this.val().trim() + "'>" + _this.val().trim() + "</li>");
                                                attrObj.click(function () {
                                                    var _self = $(this);
                                                    if (_self.hasClass("select")) {
                                                        _self.removeClass("select");
                                                    } else {
                                                        _self.addClass("select");
                                                    }
                                                    ObjectJS.createOrderDetails();
                                                });
                                                _this.parent().before(attrObj);
                                                attrObj.click();
                                                _this.val('');
                                            }
                                        });

                                        CityInvoice = City.createCity({
                                            cityCode: ObjectJS.user.CityCode,
                                            elementID: "citySpan"
                                        });
                                    });
                                } else {
                                    alert("请重新尝试");
                                }
                            });
                        } else {
                            doT.exec("m/template/style/style-buy.html", function (code) {
                                var innerHtml = code(cacheOrder[id]);
                                innerHtml = $(innerHtml);

                                innerHtml.find("#customerName").val(ObjectJS.user.Name);
                                innerHtml.find("#customerTel").val(ObjectJS.user.MobilePhone);
                                innerHtml.find("#customerAddress").val(ObjectJS.user.Address);


                                $(".overlay-addOrder").html(innerHtml).show();

                                $(".overlay-addOrder .style-content").animate({ height: "450px" }, 200);

                                $(".overlay-addOrder").unbind().click(function (e) {
                                    if (!$(e.target).parents().hasClass("style-content") && !$(e.target).hasClass("style-content")) {
                                        $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                                            $(".overlay-addOrder").hide();
                                        });
                                    }
                                });

                                $(".overlay-addOrder .close").unbind().click(function () {
                                    $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                                        $(".overlay-addOrder").hide();
                                    });
                                });

                                $(".overlay-addOrder .attr-ul li.size,.overlay-addOrder .attr-ul li.color").unbind().click(function () {
                                    var _self = $(this);
                                    if (_self.hasClass("select")) {
                                        _self.removeClass("select");
                                    } else {
                                        _self.addClass("select");
                                    }
                                    ObjectJS.createOrderDetails();
                                });

                                $(".overlay-addOrder .btn-sureAdd").unbind().click(function () {
                                    var model = {
                                        OrderID: cacheOrder[id].orderID,
                                        PersonName: $("#customerName").val(),
                                        MobileTele: $("#customerTel").val(),
                                        CityCode: CityInvoice.getCityCode(),
                                        Address: $("#customerAddress").val(),
                                        GoodsName: cacheOrder[id].goodsName,
                                        GoodsID: cacheOrder[id].goodsID,
                                        ClientID: cacheOrder[id].clientID,
                                        IntGoodsCode: cacheOrder[id].intGoodsCode,
                                        CategoryID: cacheOrder[id].categoryID,
                                        FinalPrice: cacheOrder[id].finalPrice,
                                        OrderImage: cacheOrder[id].orderImage,
                                        Details: []
                                    };
                                    ObjectJS.createOrders(model);
                                    return false;
                                });

                                innerHtml.find('.layout-attr').change(function () {
                                    var _this = $(this);
                                    var isContinue = true;
                                    var _thisTitle = _this.parents('.productsalesattr').find('.attr-title');
                                    if (!_this.val().trim()) {
                                        _this.val('');
                                        return false;
                                    }
                                    _this.parent().siblings().each(function () {
                                        var obj = $(this);
                                        if (_this.val().trim() == obj.text().trim()) {
                                            alert(obj.data('type') == 1 ? "" + _thisTitle.text() + "已存在" : "" + _thisTitle.text() + "已存在");
                                            isContinue = false;
                                            return false;
                                        }
                                    });
                                    if (isContinue) {
                                        var attrObj = $("<li class='" + (_thisTitle.data('type') == 1 ? "size" : "color") + "' data-type='" + _thisTitle.data('type') + "' data-id='|' data-value='" + _this.val().trim() + "'>" + _this.val().trim() + "</li>");
                                        attrObj.click(function () {
                                            var _self = $(this);
                                            if (_self.hasClass("select")) {
                                                _self.removeClass("select");
                                            } else {
                                                _self.addClass("select");
                                            }
                                            ObjectJS.createOrderDetails();
                                        });
                                        _this.parent().before(attrObj);
                                        attrObj.click();
                                        _this.val('');
                                    }
                                });

                                CityInvoice = City.createCity({
                                    cityCode: ObjectJS.user.CityCode,
                                    elementID: "citySpan"
                                });
                            });
                        }
                    });

                    //延迟加载图片
                    $(".task-list-img").each(function () {
                        var _this = $(this);
                        setTimeout(function () {
                            _this.attr("src", _this.data("src") + "?imageView2/1/w/120/h/120");
                        }, 1000)
                    });
                });
            }
            
        });
    };

    ObjectJS.createOrderDetails = function () {
        $(".attr-box").empty();
        $(".attr-box").append("<table class='table-list'></table>");
        $(".attr-box .table-list").append('<tr class="tr-header" ><td class="tLeft">规格</td><td>数量</td><td>操作</td></tr>');
        $(".attr-ul .color.select").each(function () {
            var _this = $(this);
            $(".attr-ul .size.select").each(function () {
                var dataAttr = $("#colorBox").data('id') + ',' + $("#attrBox").data('id');
                var dataValue = _this.data('id') + ',' + $(this).data('id');
                var dataAttrValue = $("#colorBox").data('id') + ":" + _this.data('id') + "," + $("#attrBox").data('id') + $(this).data('id');
                var description = '【尺码:' + $(this).data('value') + '】【颜色:' + _this.data('value') + '】';

                var trHtml = $("<tr class='detail-attr' data-attr='" + dataAttr + "' data-value='" + dataValue + "' data-attrAndvalue='" + dataAttrValue + "' data-xremark='【" + $(this).data('value') + "】' data-yremark='【" + _this.data('value') + "】' data-xyremark='【" + $(this).data('value') + "】【" + _this.data('value') + "】' data-remark='" + description + "'></tr>");
                trHtml.append("<td class='tLeft'>" + description + "</td>");
                trHtml.append("<td class='center'><input style='width:50px;' class='quantity center' type='text' value='0' /></td>");
                trHtml.append("<td class='iconfont center' style='font-size:30px;color:#4a98e7;'>&#xe651;</td>");

                trHtml.find('.iconfont').click(function () {
                    $(this).parents('tr').remove();
                    return false;
                });
                trHtml.find('.quantity').change(function () {
                    var _this = $(this);
                    if (!_this.val().isInt() || _this.val() < 0) {
                        _this.val(0);
                    }
                });
                $(".attr-box .table-list").append(trHtml);
            });
        });
    };

    ObjectJS.createOrders = function (model) {
        var _self = this;
        var totalnum = 0;
        $(".attr-box .table-list .quantity").each(function () {
            totalnum += parseInt($(this).val());
        });
        if (totalnum == 0) {
            alert("请选择颜色尺码，并填写对应采购数量", 2);
            return false;
        }
        //大货单遍历下单明细 
        $(".attr-box .table-list .quantity").each(function () {
            var _this = $(this);
            var _thisTr = _this.parents('tr');
            if (_this.val() > 0) {
                model.Details.push({
                    SaleAttr: _thisTr.data('attr'),
                    AttrValue: _thisTr.data('value'),
                    SaleAttrValue: _thisTr.data('attrandvalue'),
                    Quantity: _this.val(),
                    XRemark: _thisTr.data('xremark'),
                    YRemark: _thisTr.data('yremark'),
                    XYRemark: _thisTr.data('xyremark'),
                    Remark: _thisTr.data('remark')
                });
            }
        });

        Global.post("/IntFactoryOrder/CreateOrderEDJ", { entity: JSON.stringify(model) }, function (data) {
            if (data.result) {
                alert("下单成功");
                $(".overlay-addOrder .style-content").animate({ height: "0px" }, 200, function () {
                    $(".overlay-addOrder").hide();
                });
            } else {
                alert(data.error_message);
            }
        });
    };

    module.exports = ObjectJS;
});