﻿/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Dialog = require("dialog"),
        Easydialog = require("easydialog"); 
    require("parser")($);
    require("form")($);
    var Category = {
        CategoryID: "",
        PID: ""
    };
    var CacheCategorys = [], CacheDel = [], CacheAttrs = [];

    var ObjectJS = {};

    //初始化数据
    ObjectJS.init = function (type) {
        var _self = this;
        _self.type = type;
        ObjectJS.bindEvent();
        ObjectJS.bindCategory();
        ObjectJS.cache();
        ObjectJS.bindStyle();
    }

    //缓存数据
    ObjectJS.cache = function () {
        //获取所有属性
        Global.post("/Products/GetAttrs", {}, function (data) {
            CacheAttrs = data.items;
        });
    }

    ObjectJS.bindStyle = function () {
        var _self = this;
        if (_self.type == "add") {
            $('#btndiv').remove();
            $(".category-add,.category-edit,.category-del").remove();
        }else{ 
            $('#btndiv').show();
        }
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        /**
         * 绑定下拉框
         */
        $("#dropdown").click(function () {
            var position = $("#dropdown").position();
            $(".dropdown-ul").css({ "top": position.top + 30, "left": position.left - 80 }).show().mouseleave(function () {
                $(this).hide();
            });
        });
        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        $("#exportExcel").click(function () {
            _self.ShowExportExcel();
        });
        $('#exportCategory').click(function () {
            Dialog.exportModel("/Products/ExportFromCategory", {  filleName: "产品类别导出" });
        });

        //展开
        $(".content-body").delegate(".openchild", "click", function () {
            var _this = $(this);
            if (_this.data("state") == "close") {
                _this.data("state", "open");
                _this.removeClass("icoopen").addClass("icoclose");

                if ($("#" + _this.data("id")).length == 0) {
                    var _obj = _self.getChild(_this.data("id"), _this.prevUntil("div").html(), _this.data("eq"));
                    _this.parent().after(_obj);
                }
                _self.bindStyle();

                $("#" + _this.data("id")).show();
            } else { //隐藏子下属
                _this.data("state", "close");
                _this.removeClass("icoclose").addClass("icoopen");

                $("#" + _this.attr("data-id")).hide();
            }
        });

        //添加子类
        $(".content-body").delegate(".category-add", "click", function () {
            var _this = $(this).prevAll(".openspan");

            var _leftLine = $(this).prevAll(".left-box");

            Category.CategoryID = "";
            Category.PID = _this.data("id");
            _self.showCategory(function (model) {

                if (model.Layers == 1) {
                    location.href = location.href;
                    return;
                }

                $("#" + _this.data("id")).remove();

                if (!_this.hasClass("icoopen") && !_this.hasClass("icoclose")) {
                    _this.addClass("icoclose").addClass("openchild").data("state", "open");
                } else if (_this.hasClass("icoopen")) {
                    _this.addClass("icoclose").removeClass("icoopen").data("state", "open");
                }

                Global.post("/Products/GetCategoryByID", { categoryid: _this.data("id") }, function (data) {
                    CacheCategorys[_this.data("id")] = data.model.ChildCategorys;

                    var _leftBg = $(document.createElement("div")).css("display", "inline-block").addClass("left");
                    _leftBg.append(_leftLine.html());
                    var _obj = _self.getChild(_this.data("id"), _leftBg.html(), _this.data("eq"));

                    _this.parent().after(_obj);
                    $("#" + _this.data("id")).show();
                });
            });
        });

        //删除分类
        $(".content-body").delegate(".category-del", "click", function () {
            var _this = $(this);
            confirm("分类删除后不可恢复,确认删除吗？", function () {
                Global.post("/Products/DeleteCategory", { id: _this.data("id") }, function (data) {
                    if (data.status == 1) {
                        if (_this.parent().nextAll().length == 0) {
                            _this.parent().prev().find(".leftline").removeClass("leftline").addClass("lastline");
                            _this.parent().prev().find(".openspan").data("eq", "last");
                        }
                        if (_this.parent().siblings().length == 0) {
                            _this.parent().parent().prev().find(".openspan").removeClass("icoclose").removeClass("icoopen");
                        }
                        _this.parent().remove();
                        //缓存删除分类
                        CacheDel[_this.data("id")] = _this.data("id");
                        
                    } else if (data.status == 10002) {
                        alert("存在关联数据,不能删除,可以选择不启用");
                    } else {
                        alert("删除失败");
                    }
                });
            });
        });

        //编辑分类
        $(".content-body").delegate(".category-edit", "click", function () {
            var _this = $(this);

            Global.post("/Products/GetCategoryDetailsByID", {
                categoryid: _this.data("id")
            }, function (data) {
                Category = data.model;
                _self.showCategory(function (model) {
                    alert("编辑成功");
                    _this.prev().html(model.CategoryName);
                    if (model.Status == 1) {
                        _this.prev().removeClass("close");
                    } else {
                        _this.prev().addClass("close");
                    }
                });
            });
            return false;
        });
    }

    //绑定一级分类
    ObjectJS.bindCategory = function () {
        var _self = this;

        Global.post("/Products/GetChildCategorysByID", {
            categoryid: ""
        }, function (data) {
            doT.exec("template/products/categorys.html", function (template) {
                var innerHtml = template(data.items);
                innerHtml = $(innerHtml);

                $("#categoryBox").append(innerHtml);

                for (var i = 0; i < data.items.length; i++) {
                    var item=data.items[i];
                    CacheCategorys[item.CategoryID] = item.ChildCategorys;
                }

                _self.bindStyle();
            });
        });
    }

    //展开下级
    ObjectJS.getChild = function (categoryid, provHtml, isLast) {
        var _self = this;
        var _div = $(document.createElement("div")).attr("id", categoryid).addClass("childbox").addClass("hide");
        for (var i = 0; i < CacheCategorys[categoryid].length; i++) {
            var _item = $(document.createElement("div")).addClass("category-item");

            //添加左侧背景图
            var _leftBg = $(document.createElement("div")).css("display", "inline-block").addClass("left").addClass("left-box");
            _leftBg.append(provHtml);
            if (isLast == "last") {
                _leftBg.append("<span class='null left'></span>");
            } else {
                _leftBg.append("<span class='line left'></span>");
            }
            _item.append(_leftBg);

            //是否最后一位
            if (i == CacheCategorys[categoryid].length - 1) {
                _item.append("<span class='lastline left'></span>");

                //加载显示下属图标和缓存数据
                if (CacheCategorys[categoryid][i].ChildCategorys && CacheCategorys[categoryid][i].ChildCategorys.length > 0) {
                    _item.append("<span data-id='" + CacheCategorys[categoryid][i].CategoryID + "' data-eq='last' data-state='close' class='icoopen openchild left openspan'></span>");
                    if (!CacheCategorys[CacheCategorys[categoryid][i].CategoryID]) {
                        CacheCategorys[CacheCategorys[categoryid][i].CategoryID] = CacheCategorys[categoryid][i].ChildCategorys;
                    }
                } else {
                    _item.append("<span data-id='" + CacheCategorys[categoryid][i].CategoryID + "' data-eq='last' data-state='close' class='left openspan'></span>");
                }
            } else {
                _item.append("<span class='leftline left'></span>");

                //加载显示下属图标和缓存数据
                if (CacheCategorys[categoryid][i].ChildCategorys && CacheCategorys[categoryid][i].ChildCategorys.length > 0) {
                    _item.append("<span data-id='" + CacheCategorys[categoryid][i].CategoryID + "' data-eq='' data-state='close' class='icoopen openchild left openspan'></span>");
                    if (!CacheCategorys[CacheCategorys[categoryid][i].CategoryID]) {
                        CacheCategorys[CacheCategorys[categoryid][i].CategoryID] = CacheCategorys[categoryid][i].ChildCategorys;
                    }
                } else {
                    _item.append("<span data-id='" + CacheCategorys[categoryid][i].CategoryID + "' data-eq='' data-state='close' class='left openspan'></span>");
                }
            }
            _item.append('<span class="left category ' + (CacheCategorys[categoryid][i].Status == 1 || 'close') + '">' + CacheCategorys[categoryid][i].CategoryName + '</span>');
            _item.append('<a class="category-edit" data-id="' + CacheCategorys[categoryid][i].CategoryID + '" href="javascript:void(0)">编辑</a>');
            _item.append('<a class="category-add" data-id="' + CacheCategorys[categoryid][i].CategoryID + '" href="javascript:void(0)">添加子类</a>');
            _item.append('<a class="btn-open-window"  data-id="103010500" data-name="添加产品" data-url="/Products/ProductAdd/' + CacheCategorys[categoryid][i].CategoryID + '"  href="javascript:void(0)">添加产品</a>');
            _item.append('<a class="category-del" data-id="' + CacheCategorys[categoryid][i].CategoryID + '" href="javascript:void(0)">删除</a>');
            
            _div.append(_item);

            //默认加载下级
            _item.find(".openchild").each(function () {
                var _this = $(this);
                var _obj = _self.getChild(_this.data("id"), _leftBg.html(), _this.data("eq"));
                _this.parent().after(_obj);
            });
        }
        return _div;
    }

    //添加分类弹出层
    ObjectJS.showCategory = function (callback) {
        var _self = this;
        doT.exec("template/products/category_add.html", function (templateFun) {
            var html = templateFun(CacheAttrs);
            Easydialog.open({
                container: {
                    id: "category-add-div",
                    header: Category.CategoryID == "" ? "添加分类" : "编辑分类",
                    content: html,
                    yesFn: function () {

                        if (!VerifyObject.isPass("#category-add-div")) {
                            return false;
                        }

                        if ($("#saleAttr1").val() != "-1" && $("#saleAttr1").val() == $("#saleAttr2").val()) {
                            alert("两种规格不能相同");
                            return false;
                        }

                        var model = {
                            CategoryID: Category.CategoryID,
                            CategoryCode: $("#categoryCode").val().trim(),
                            CategoryName: $("#categoryName").val().trim(),
                            SaleAttr:"",
                            SaleAttrStr: "",
                            AttrList: "",
                            AttrListStr: "",
                            PID: Category.PID,
                            Status: $("#categoryStatus").hasClass("hover") ? 1 : 0,
                            Description: $("#description").val()
                        };

                        var isAgain = false;;
                        //属性
                        $("#attrList .checkbox.hover").each(function () {
                            if (model.AttrListStr.indexOf($(this).data("name") + ",") >= 0) {
                                isAgain = true;
                            }
                            model.AttrList += $(this).data("id") + ",";
                            model.AttrListStr += $(this).data("name") + ",";
                        });

                        if (isAgain) {
                            alert("选择的属性有重复名称，不能提交");
                            return false;
                        }

                        //规格
                        if ($("#saleAttr1").val() != "-1") {
                            model.SaleAttr += $("#saleAttr1").val() + ",";
                            model.SaleAttrStr += $("#saleAttr1 option:selected").data("name") + ",";
                        }
                        if ($("#saleAttr2").val() != "-1") {
                            model.SaleAttr += $("#saleAttr2").val() + ",";
                            model.SaleAttrStr += $("#saleAttr2 option:selected").data("name") + ",";
                        }
                        _self.saveCategory(model, callback);

                        return false;
                    },
                    callback: function () {

                    }
                }
            });

            $("#categoryName").focus();

            //编辑填充数据
            if (Category.CategoryID) {
                $("#categoryName").val(Category.CategoryName);
                $("#categoryCode").val(Category.CategoryCode);
                Category.Status == 1 || $("#categoryStatus").removeClass("hover");
                $("#description").val(Category.Description);

                for (var i = 0; i < Category.AttrLists.length; i++) {
                    $("#attrList .checked[data-id='" + Category.AttrLists[i].AttrID + "']").find(".checkbox").addClass("hover");
                }
                for (var i = 0; i < Category.SaleAttrs.length; i++) {
                    if (i == 0) {
                        $("#saleAttr1").val(Category.SaleAttrs[i].AttrID);
                    } else if (i == 1) {
                        $("#saleAttr2").val(Category.SaleAttrs[i].AttrID);
                    } 
                }
            }

            $("#category-add-div .checked").click(function () {
                var _this = $(this);

                if (_this.find(".checkbox").hasClass("hover")) {
                    _this.find(".checkbox").removeClass("hover");
                    $("#category-add-div .checked[data-id='" + _this.data("id") + "']").show();

                } else {
                    if (_this.data("type") == 2 && $("#saleAttr .checkbox.hover").length >= 2) {
                        alert("产品规格不能超过2个");
                        return false;
                    }
                    _this.find(".checkbox").addClass("hover");
                    $("#category-add-div .checked[data-id='" + _this.data("id") + "']").hide();
                    _this.show();
                }
            });

            //是否启用
            $("#categoryStatus").parent().click(function () {
                var _this = $(this);
                if (_this.find(".checkbox").hasClass("hover")) {
                    _this.find(".checkbox").removeClass("hover");
                } else {
                    _this.find(".checkbox").addClass("hover");
                }
            });

            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //保存分类
    ObjectJS.saveCategory = function (category, callback) {
        Global.post("/Products/SavaCategory", {
            category: JSON.stringify(category)
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！", function () {
                    Easydialog.close();
                });
                return;
            } else if (data.status) {
                !!callback && callback(data.model);
                Easydialog.close();
            } else if (data.result == 2) {
                alert("分类编码已存在");
            } else {
                alert("保存失败", function () {
                    Easydialog.close();
                });
            }

            
        });
    };
    ObjectJS.ShowExportExcel = function () {
        $('#show-category-import').empty();
        var guid = Global.guid() + "_";
        Dialog.open({
            container: {
                id: "show-catagory-import",
                header: "导入产品分类",
                importUrl: '/Products/CategoryImport',
                yesFn: function () {
                    $('#upfileForm').form('submit', {
                        onSubmit: function () {
                            Dialog.setOverlay(guid, true);
                        },
                        success: function (data) {
                            Dialog.setOverlay(guid, false);
                            if (data == "操作成功") {
                                Dialog.close(guid);
                                ObjectJS.bindCategory();
                            }
                            alert(data);
                        }
                    });
                },
                docWidth: 450,
                exportUrl: '/Products/ExportFromCategory',
                exportParam: { test: true, model: 'Item' },
                herf: '/Products/CategoryImport',
                noFn: true,
                yesText: '导入',
                callback: function () {

                }
            },
            guid: guid
        });

    }
    module.exports = ObjectJS;
})