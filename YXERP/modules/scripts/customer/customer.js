﻿define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Dialog = require("dialog"), 
        moment = require("moment");
    require("daterangepicker");
    require("pager");
    require("colormark");
    var $ = require('jquery');
    require("parser")($);
    require("form")($);

    var Params = {
        SearchType: 1,
        Type: -1,
        SourceID: "",
        StageID: "-1",
        Status: 1,
        Mark: -1,
        OrderBy: "cus.CreateTime desc",
        UserID: "",
        AgentID: "",
        TeamID: "",
        Keywords: "",
        BeginTime: "",
        EndTime: "",
        ExcelType:0,
        PageIndex: 1,
        PageSize: 20
    };

    var ObjectJS = {};
    ObjectJS.ColorList = [];
    //初始化
    ObjectJS.init = function (type,colorList) {
        var _self = this;
        Params.SearchType = type;
        _self.ColorList = JSON.parse(colorList.replace(/&quot;/g, '"'));
        _self.getList();
        _self.bindEvent(type);
    }

    //绑定事件
    ObjectJS.bindEvent = function (type) {
        var _self = this; 
        //客户阶段
        $(".search-stages li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.StageID = _this.data("id");
                _self.getList();
            }
        });

        //导入导出excel
        $("#exportExcel").click(function () {
            ObjectJS.ShowExportExcel();
        });

        //客户状态
        $(".search-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.Status = _this.data("id");
                _self.getList();
            }
        });

        //客户类型
        $("#customerType li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.Type = _this.data("id");
                _self.getList();
            }
        });

        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.PageIndex = 1;
                Params.Keywords = keyWords;
                _self.getList();
            });
        });

        //客户来源
        Global.post("/Customer/GetCustomerSources", {}, function (data) {
            for (var i = 0; i < data.items.length; i++) {
                $("#customerSource").append('<li data-id="' + data.items[i].SourceID + '">' + data.items[i].SourceName + '</li>')
            }
            $("#customerSource li").click(function () {
                var _this = $(this);
                if (!_this.hasClass("hover")) {
                    _this.siblings().removeClass("hover");
                    _this.addClass("hover");
                    Params.PageIndex = 1;
                    Params.SourceID = _this.data("id");
                    _self.getList();
                }
            });
        });

        if (type == 2) {
            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "下属-",
                    defaultText: "全部",
                    defaultValue: "",
                    userid: "",
                    isTeam: false,
                    width: "180",
                    onChange: function (data) {
                        Params.PageIndex = 1;
                        Params.UserID = data.userid;
                        Params.TeamID = data.teamid;
                        _self.getList();
                    }
                });
            });
        } else if (type == 3) {
            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "人员-",
                    defaultText: "全部",
                    defaultValue: "",
                    userid: "-1",
                    isTeam: true,
                    width: "180",
                    onChange: function (data) {
                        Params.PageIndex = 1;
                        Params.UserID = data.userid;
                        Params.TeamID = data.teamid;
                        _self.getList();
                    }
                });
            });
        }
        //全部选中
        $("#checkAll").click(function () {
            var _this = $(this).find(".checkbox");
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $(".table-box-list .checkbox").addClass("hover");
            } else {
                _this.removeClass("hover");
                $(".table-box-list .checkbox").removeClass("hover");
            }
        });

        $("#dropdown").click(function () { 
            var position = $(".dropdown").position();
            $(".dropdown-ul").css({ "top": position.top + 30, "left": position.left - 80 }).show().mouseleave(function () {
                $(this).hide();
            });
        });
         
        $(document).click(function (e) { 
            if (!$(e.target).parents().hasClass("dropdown-ul")  && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        //批量转移
        $("#batchChangeOwner").click(function () {
            var checks = $(".table-box-list .checkbox.hover");
            if (checks.length > 0) {
                ChooseUser.create({
                    title: "批量更换负责人",
                    type: 1,
                    single: true,
                    callback: function (items) {
                        if (items.length > 0) {
                            var ids = "", userid = items[0].id;
                            checks.each(function () {
                                var _this = $(this);
                                if (_this.data("userid") != userid) {
                                    ids += _this.data("id") + ",";
                                }
                            });
                            if (ids.length > 0) {
                                _self.ChangeOwner(ids, userid);
                            } else {
                                alert("请选择不同人员进行转移!");
                            }
                        }
                    }
                });
            } else {
                alert("您尚未选择客户");
            }
        });

        ///Excel导出客户
        $("#batchCustomerExport").click(function () {
            Params.ExcelType = 0;
            Dialog.exportModel("/Customer/ExportFromCustomer", { filter: JSON.stringify(Params), filleName: "客户" });
        });

        ///Excel导出联系人
        $("#batchContactExport").click(function () {
            Params.ExcelType = 1;
            Dialog.exportModel("/Customer/ExportFromCustomer", { filter: JSON.stringify(Params), filleName: "联系人" });
        });

        //过滤标记
        $("#filterMark").markColor({
            isAll: true,
            top: 30,
            left: 5,
            data:_self.ColorList,
            onChange: function (obj, callback) {
                callback && callback(true);
                Params.PageIndex = 1;
                Params.Mark = obj.data("value");
                _self.getList();
            }
        });
        //批量标记
        $("#batchMark").markColor({
            isAll: true,
            left: 10,
            data: _self.ColorList, 
            onChange: function (obj, callback) { 
                var checks = $(".table-box-list .checkbox.hover");
                if (checks.length > 0) {
                    var ids = "";
                    checks.each(function () {
                        var _this = $(this);
                        ids += _this.data("id") + ",";
                    });
                    _self.markCustomer(ids, obj.data("value"), function (status) {
                        _self.getList();
                        callback && callback(status);
                    });
                    
                } else {
                    alert("您尚未选择客户");
                }
            }
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
            Params.PageIndex = 1;
            Params.BeginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.EndTime = end ? end.format("YYYY-MM-DD") : "";
            _self.getList();
        });

        //排序
        $(".sort-item").click(function () {
            var _this = $(this);
            if (_this.hasClass("hover")) {
                if (_this.find(".asc").hasClass("hover")) {
                    _this.find(".asc").removeClass("hover");
                    _this.find(".desc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " desc ";
                } else {
                    _this.find(".desc").removeClass("hover");
                    _this.find(".asc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " asc ";
                }
            } else {
                _this.addClass("hover").siblings().removeClass("hover");
                _this.siblings().find(".hover").removeClass("hover");
                _this.find(".desc").addClass("hover");
                Params.OrderBy = _this.data("column") + " desc ";
            }
            _self.getList();
        });

    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#checkAll").removeClass("hover");
        $(".box-header").nextAll().remove();
        $(".box-header").after("<div class='data-loading'><div>");

        Global.post("/Customer/GetCustomers", { filter: JSON.stringify(Params) }, function (data) {
            _self.bindList(data);
        });
    }

    //加载列表
    ObjectJS.bindList = function (data) {
        var _self = this;
        $(".box-header").nextAll().remove();

        if (data.items.length > 0) {
            doT.exec("template/customer/customers.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                innerhtml.find(".checkbox").click(function () {
                    var _this = $(this);
                    if (!_this.hasClass("hover")) {
                        _this.addClass("hover");
                    } else {
                        _this.removeClass("hover");
                    }
                    return false;
                });

                innerhtml.find(".mark").markColor({
                    isAll: false,
                    top: 25,
                    left: 5,
                    data:_self.ColorList,
                    onChange: function (obj, callback) {
                        _self.markCustomer(obj.data("id"), obj.data("value"), callback);
                    }
                });
                $(".box-header").after(innerhtml);
            });
        } else {
            $(".box-header").after("<div class='nodata-box' >暂无数据<div>");
        }

        $("#pager").paginate({
            total_count: data.totalCount,
            count: data.pageCount,
            start: Params.PageIndex,
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
                Params.PageIndex = page;
                _self.getList();
            }
        });
    }

    //标记客户
    ObjectJS.markCustomer = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!");
            return false;
        }
        Global.post("/Customer/UpdateCustomMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            callback && callback(data.status);
        });
    }

    //转移客户
    ObjectJS.ChangeOwner = function (ids, userid) {
        var _self = this;
        Global.post("/Customer/UpdateCustomOwner", {
            userid: userid,
            ids: ids
        }, function (data) {
            if (data.status) {
                _self.getList();
            }
        });
    }

    ObjectJS.ShowExportExcel = function () { 
        $('#show-customer-export').empty();
        var guid = Global.guid() + "_";
        Dialog.open({
            container: {
                id: "show-customer-export",
                header: "导入客户信息",
                importUrl: '/Customer/CustomerImport',
                yesFn: function() {
                    $('#upfileForm').form('submit', {
                        onSubmit: function() {
                            Dialog.setOverlay(guid, true);
                        },
                        success: function(data) {
                            Dialog.setOverlay(guid, false);
                            if (data == "操作成功") {
                                Dialog.close(guid);
                            }
                            alert(data);
                        }
                    });
                },
                docWidth: 450,
                exportUrl: '/Customer/ExportFromCustomer',
                exportParam: { test: true, model: 'Item|OwnItem' },
                herf: '/Customer/CustomerImport',
                noFn:true,
                yesText:'导入',
                callback: function () {

                }
            },
            guid: guid
        });  
    }

    module.exports = ObjectJS;
});