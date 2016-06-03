/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject, CategoryVerifyObject,
        AttrPlug = require("scripts/products/attrplug"),
        Easydialog = require("easydialog");
    var Category = {
        CategoryID: "",
        PID: ""
    };
    var CacheCategorys = [];
    var CacheDel = [];
    var CacheAttrs = [];

    var ObjectJS = {};
    //初始化数据
    ObjectJS.init = function () {
        ObjectJS.bindEvent();
        ObjectJS.bindCategory();
        //ObjectJS.cache();
    }
    //缓存数据
    ObjectJS.cache = function () {
        //获取所有属性
        Global.post("/Products/GetAttrsByCategoryID", {
            categoryid: ""
        }, function (data) {
            CacheAttrs = data.Items;
        });
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

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

            Global.post("/Products/GetCategoryByID", {
                categoryid: _this.data("id")
            }, function (data) {
                Category = data.model;
                _self.showCategory(function (model) {
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
            _item.append('<a class="category-child" data-id="' + CacheCategorys[categoryid][i].CategoryID + '" href="javascript:void(0)">添加产品</a>');
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

            var html = templateFun([]);

            Easydialog.open({
                container: {
                    id: "category-add-div",
                    header: Category.CategoryID == "" ? "添加分类" : "编辑分类",
                    content: html,
                    yesFn: function () {

                        if (!CategoryVerifyObject.isPass("#category-add-div")) {
                            return false;
                        }

                        var model = {
                            CategoryID: Category.CategoryID,
                            CategoryCode: "",
                            CategoryName: $("#categoryName").val(),
                            PID: Category.PID,
                            Status: $("#categoryStatus").prop("checked") ? 1 : 0,
                            Description: $("#description").val()
                        };

                        var attrs = "", saleattrs = "";

                        _self.saveCategory(model, attrs, saleattrs, callback);
                    },
                    callback: function () {

                    }
                }
            });

            $("#categoryName").focus();

            //编辑填充数据
            if (Category.CategoryID) {
                $("#categoryName").val(Category.CategoryName);
                $("#categoryStatus").prop("checked", Category.Status == 1);
                $("#description").val(Category.Description);
            }

            CategoryVerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //保存分类
    ObjectJS.saveCategory = function (category, attrs, saleattrs, callback) {
        Global.post("/Products/SavaCategory", {
            category: JSON.stringify(category),
            attrlist: attrs,
            saleattr: saleattrs
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            if (data.status) {
                !!callback && callback(data.model);
            }
        });
    };

    //元素绑定事件
    ObjectJS.bindElementEvent = function (element) {
        var _self = this;

        //下拉事件
        element.find(".ddlcategory").click(function () {
            var _this = $(this);

            var position = _this.offset();
            $("#ddlCategory li").data("id", _this.data("id"));
            $("#ddlCategory").css({ "top": position.top + 20, "left": position.left }).show().mouseleave(function () {
                $(this).hide();
            });
            return false;
        });

        //点击分类名称事件（展开下级和属性）
        element.click(function () {
            var _this = $(this), layer = _this.data("layer");
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            _this.parents(".category-layer").nextAll(".category-layer,.category-attr-layer,.common-attr-layer").remove();
            
            //加载下级分类
            Global.post("/Products/GetChildCategorysByID", {
                categoryid: _this.data("id")
            }, function (data) {
                doT.exec("template/products/categorys.html", function (templateFun) {
                    var html = templateFun(data.Items);
                    html = $(html);
                    //绑定添加事件
                    html.find(".category-header").html(_this.find(".category-name").html()+" >>");

                    _self.addBindEvent(html.find(".create-child").data("id", _this.data("id")).data("layer", _this.data("layer") * 1 + 1));

                    _self.bindElementEvent(html.find("li"));

                    _this.parents(".category-layer").after(html);
                    _self.bindStyle();

                    _self.showAttrs(_this);
                });
            });
        });
    }
    //显示分类属性
    ObjectJS.showAttrs = function (obj) {

        var _self = this;
        //属性设置
        Global.post("/Products/GetAttrsByCategoryID", {
            categoryid: obj.data("id")
        }, function (data) {
            doT.exec("template/products/category-attrs.html", function (templateFun) {
                var html = templateFun(data.Items);
                html = $(html);
                //绑定添加事件
                html.find(".category-attr-header span").html(obj.find(".category-name").html() + "-属性列表");

                html.find(".ico-add").click(function () {
                    var _attrdiv = $(this);
                    AttrPlug.init({
                        attrid: "",
                        categoryid: obj.data("id"),
                        callback: function (Attr) {
                            _self.innerAttr(_attrdiv.parent().siblings("[data-type=" + Attr.Type + "]"), Attr);
                        }
                    });
                });

                _self.bindAttrElementEvent(html.find(".attritem"));

                obj.parents(".category-layer").next().after(html);

                _self.bindCommonAttr(obj.data("id"), html);
            });
        });
    }
    //保存属性后添加到列表
    ObjectJS.innerAttr = function (parentele, Attr) {
        var _self = this;
        var ele = $('<li class="attritem" data-id="' + Attr.AttrID + '" data-category="' + Attr.CategoryID + '" title="' + Attr.Description + '">' +
                                                    '<span class="category-attr-name long">' + Attr.AttrName + '</span>' +
                                                    '<span data-id="' + Attr.AttrID + '" data-categoryid="' + Attr.CategoryID + '" data-type="' + Attr.Type + '" class="ico-dropdown ddlattr right"></span>' +
                                                '</li>');
        _self.bindAttrElementEvent(ele);
        parentele.append(ele);
    }
    //绑定通用属性
    ObjectJS.bindCommonAttr = function (categoryid, ele) {
        var _self = this;
        ele.next().remove();
        doT.exec("template/products/common-attrs.html", function (templateFun) {
            var html = templateFun(CacheAttrs);
            html = $(html);

            //隐藏已有属性添加按钮
            html.find(".addcommon").each(function () { //[data-type=" + $(this).data("type") + "]
                if ($(".category-attr-layer").find("ul li[data-id=" + $(this).data("id") + "]").length > 0) {
                    $(this).hide();
                }
            });

            html.find(".addcommon").click(function () {
                _self.addCommonAttr(categoryid, $(this));
            });

            ele.after(html);

            _self.bindStyle();
        });
    }
    //添加通用属性事件
    ObjectJS.addCommonAttr = function (categoryid, ele) {
        var _self = this;
        var _attrlist = $(".category-attr-layer").find("ul[data-type=" + ele.data("type") + "]");
        if (_attrlist.find(".category-attr-layer li[data-id=" + ele.data("id") + "]").length === 0) {
            Global.post("/Products/AddCategoryAttr", {
                categoryid: categoryid,
                attrid: ele.data("id"),
                type: ele.data("type")
            }, function (data) {
                if (data.Status) {
                    ele.parent().find(".addcommon").hide();
                    var _model = {
                        CategoryID: categoryid,
                        AttrID: ele.data("id"),
                        AttrName: ele.data("name"),
                        Type: ele.data("type"),
                        Description: ""
                    }
                    _self.innerAttr(_attrlist, _model);
                }
            });
            
        } else {
            ele.parent().find(".addcommon").hide();
            alert("此分类已存在该" + (ele.data("type") == 1 ? "属性" : "规格") + "，不能重复添加！");
        }
    }
    //绑定属性事件
    ObjectJS.bindAttrElementEvent = function (element) {
        var _self = this;

        //下拉事件
        element.find(".ddlattr").click(function () {
            var _this = $(this);

            var position = _this.offset();
            $("#ddlAttr li").data("id", _this.data("id")).data("type", _this.data("type")).data("categoryid", _this.data("categoryid"));
            $("#ddlAttr").css({ "top": position.top + 20, "left": position.left }).show().mouseleave(function () {
                $(this).hide();
            });
            return false;
        });

        element.click(function () {
            _self.showValues($(this).data("id"));
        })
    }
    
    //显示属性值悬浮层
    ObjectJS.showValues = function (attrID) {
        var height = document.documentElement.clientHeight;
        $("#attrValueBox").css("height", height + "px");
        $("#attrValueBox").animate({ right: "0px" }, "fast");
        Value.AttrID = attrID;
        ObjectJS.getAttrDetail();
    }
    //获取属性明细
    ObjectJS.getAttrDetail = function () {
        Global.post("/Products/GetAttrByID", { attrID: Value.AttrID }, function (data) {
            $("#attrValueBox").find(".header-title").html(data.Item.AttrName);
            ObjectJS.innerValuesItems(data.Item.AttrValues, true);
        });
    }
    //加载值数据
    ObjectJS.innerValuesItems = function (items, clear) {
        var _self = this;
        if (clear) {
            $("#attrValues").empty();
        }
        for (var i = 0, j = items.length; i < j; i++) {
            var item = $('<li data-id="' + items[i].ValueID + '" class="item">' +
                               '<input type="text" data-id="' + items[i].ValueID + '" data-value="' + items[i].ValueName + '" value="' + items[i].ValueName + '" />' +
                               '<span data-id="' + items[i].ValueID + '" class="ico-delete"></span>' +
                         '</li>');
            _self.bindValueElementEvent(item);
            $("#attrValues").prepend(item);
        }
    }
    //隐藏属性值悬浮层
    ObjectJS.hideValues = function () {
        $("#attrValueBox").animate({ right: "-302px" }, "fast");
    }
    //元素绑定事件
    ObjectJS.bindValueElementEvent = function (elments) {
        var _self = this;
        elments.find("input").focus(function () {
            var _this = $(this);
            _this.select();
        });
        elments.find("input").blur(function () {
            var _this = $(this);
            //为空
            if (_this.val() == "") {
                if (_this.data("id") == "") {
                    _this.parent().remove();
                } else {
                    _this.val(_this.data("value"));
                }
            } else if (_this.val() != _this.data("value")) {

                Value.ValueID = _this.data("id");
                Value.ValueName = _this.val();
                //保存属性值
                _self.saveValue(function () {
                    _this.data("value", Value.ValueName);
                });
            }
        });
        elments.find(".ico-delete").click(function () {
            var _this = $(this);
            if (_this.data("id") != "") {
                confirm("删除后不可恢复,确认删除吗？", function () {
                    _self.deleteValue(_this.data("id"), function (status) {
                        status && _this.parent().remove();
                    });
                });
            } else {
                _this.parent().remove();
            }
        })
    }
    //保存属性值
    ObjectJS.saveValue = function (editback) {
        var _self = this;
        Global.post("/Products/SaveAttrValue", { value: JSON.stringify(Value) }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            if (data.ID.length > 0) {
                if (Value.ValueID == "") {
                    Value.ValueID = data.ID;
                    _self.innerValuesItems([Value], false);
                }
                !!editback && editback();
            } else {
                alert("操作失败,请稍后重试!");
            }
        });
    }
    //删除属性值
    ObjectJS.deleteValue = function (valueid, callback) {
        Global.post("/Products/DeleteAttrValue", { valueid: valueid }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            !!callback && callback(data.Status);
        });
    }

    module.exports = ObjectJS;
})