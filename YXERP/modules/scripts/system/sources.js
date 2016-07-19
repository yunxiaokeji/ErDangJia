﻿define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Upload = require("upload"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog");
   
    require("switch");
    var $ = require('jquery');
    require("color")($);

    var Model = {};
    var ColorModel = {};
    var ObjectJS = {};
    var reg = /^[0-9]*[1-9][0-9]*$/; 
    var reg2 = /^(((\d[0]\.\d+))|\+?0\.\d*|\+?1)$/;
    var reg3 = /^\d+(\.\d+)?$/;
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.bindElementEvent($(".industry-item"));
        _self.getList();
    }
     
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(document).click(function (e) { 
            /*隐藏下拉*/ 
            if ((!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown"))
                && !$(e.target).parents().hasClass("levelitem") && (!$(e.target).parents().hasClass("color-item") && !$(e.target).hasClass("color-item"))) {
                $(".dropdown-ul").hide();
            }
        });

        $('#integerFee').change(function() {
            if (!reg3.test($(this).val())) {
                alert('积分比例格式输入有误,请重新输入');
                $(this).val($(this).data('oldvalue'));
            } else {
                $('#saveClientRule').show();
            }
        });
        $('#tipInfo').mousemove(function() {
            var _this = $(this);
            var position = _this.position();
            $("#wareInfo").css({ "top": position.top-45, "left": position.left + 30 }).show().mouseleave(function() {
                $(this).hide();
            });
        }).mouseout(function() {
            $("#wareInfo").hide(); 
        });

        $('#setToCustomers').click(function() {
            _self.setToCustomers();
        });
        /*添加来源*/
        $("#createModel").click(function () {
            var _this = $(this);
            Model.SourceID = "";
            Model.SourceName = "";
            Model.SourceCode = "";
            _self.createModel();
        });
        /*添加行业*/
        $("#createIndustry").click(function () { 
            var _this = $(this);
            var _ele = $('<li class="industry-item"><input type="text" data-id="" data-value="" value="" /><span data-id="" class="ico-delete"></span></li>');
            _self.bindElementEvent(_ele);
            _this.before(_ele);
            _ele.find("input").focus();
        });
        /*删除来源*/
        $("#deleteObject").click(function () { 
            _self.deleteModel($(this));
        });
        /*修改来源*/
        $("#updateObject").click(function () {
            _self.SourceEdit($(this));
        });
        /*积分规则保存*/
        $('#saveClientRule').click(function() {
            _self.saveClientRule();
        });
        /*积分等级保存*/
        $('#saveMemberLevel').click(function () {
            _self.saveMemberLevel();
        });
        /*积分等级新建*/
        $('#createMemberLevel').click(function () {
            _self.createMemberLevel();
        });
        /*客户标签新建*/
        $('#createColor').click(function () {
            var _this = $(this);
            ColorModel.ColorID = 0;
            ColorModel.ColorName = "";
            ColorModel.ColorValue = "";
            _self.createColor();
        });
        //删除颜色
        $("#deleteColor").click(function () {
            var _this = $(this); 
            confirm("标签删除后不可恢复,确认删除吗？", function() {
                _self.deleteColor(_this.data("id"), function(result) {
                    if (result == 1) {
                        alert("标签删除成功");
                        ObjectJS.bindColorList();
                    } else if (result == 10002) {
                        alert("标签已关联客户，删除失败");
                    }  else if (result == -100) {
                        alert("标签不能全部删除,操作失败");
                    }  else if (result == -200) {
                        alert("标签已已被删除,请刷新查看");
                    } else {
                        alert("删除失败");
                    }
                });
            });
        });
        /*编辑颜色*/
        $("#updateColor").click(function () {
            var _this = $(this);
            Global.post("/System/GetCustomerColorByColorID", { colorid: _this.data("id") }, function (data) {
                var model = data.model;
                ColorModel.ColorID = model.ColorID;
                ColorModel.ColorName = model.ColorName;
                ColorModel.ColorValue = model.ColorValue; 
                _self.createColor();
            });
        });
        /*切换模块*/
        $(".search-tab li").click(function () { 
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                $(".nav-part,.btn-add").hide();
                $("#" + _this.data("id")).show();
                $("#deleteObject").show();
                $(".btn-add[data-id='" + _this.data("id") + "']").show();
                
                if (_this.data("id") == "colorList" && (!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    _self.bindColorList();
                } else if (_this.data("id") == "memberList" && (!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1"); 
                    _self.getMemberLevelList();
                } 
            }
        });
    }
    ObjectJS.bindElementEvent = function (elments) { 
        elments.find("input").focus(function () {
            var _this = $(this);
            _this.select();
        });
        elments.find("input").blur(function () {
            var _this = $(this); 
            if (_this.val() == "") {
                if (_this.data("id") == "") {
                    _this.parent().remove();
                } else {
                    _this.val(_this.data("value"));
                }
            } else if (_this.val() != _this.data("value")) {
                var Industry = {
                    ClientIndustryID: _this.data('id'),
                    Name: _this.val(),
                    Description: ''
                }
                var unit = {
                    UnitID: _this.data("id"),
                    UnitName: _this.val(),
                    Description: ""
                }; 
                Global.post("/System/SaveClientIndustry", { clientindustry: JSON.stringify(Industry) }, function (data) {
                    if (data.result == 1) { 
                    } else if (data.result == 2) {
                        alert("保存失败,行业已存在!");
                    } else {
                        alert(result);
                    }
                });
            }
        });
        elments.find(".ico-delete")
        elments.find(".ico-delete").click(function() {
            var _this = $(this);
            if (_this.data("id") != "") {
                confirm("客户行业删除后不可恢复,确认删除吗？", function() {
                    Global.post("/System/DeleteClientIndustry", { clientindustryid: _this.data("id") }, function(data) {
                        if (data.result) {
                            _this.parent().remove();
                        } else {
                            alert("行业存在关联数据，删除失败");
                        }
                    });
                });
            } else {
                _this.parent().remove();
            }
        });
    }
    ObjectJS.SourceEdit = function (obj) {
        var _self = this;
        Global.post("/System/GetCustomSourceByID", { id: obj.data("id") }, function (data) {
            var model = data.model;
            Model.SourceID = model.SourceID;
            Model.SourceName = model.SourceName;
            Model.SourceCode = model.SourceCode;
            Model.IsChoose = model.IsChoose;
            _self.createModel();
        });
    }

    /*客户标签弹窗*/
    ObjectJS.createColor = function () {
        var _self = this;
        doT.exec("template/system/sources-color.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !ColorModel.ColorID ? "新建客户标签" : "编辑客户标签",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        ColorModel.ColorName = $("#colorName").val();
                        ColorModel.ColorValue = $("#colorName").data('value');
                        _self.saveColorModel(ColorModel);
                    },
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
            ColorModel.ColorValue = ColorModel.ColorValue == "" ? "#d9d9d9" : ColorModel.ColorValue
            $("#colorName").data('value',ColorModel.ColorValue);
            $("#colorName").val(ColorModel.ColorName);
            $("#colorValue").spectrum({
                color: ColorModel.ColorValue, 
                showInput: true,
                className: "full-spectrum",
                showPalette: true,
                showSelectionPalette: true,
                maxPaletteSize: 10,
                preferredFormat: "hex",
                cancelText: '取消',
                chooseText: '确认',
                togglePaletteOnly: true,
                hide: function () {
                    $('#colorName').data('value', $("#colorValue").val());
                },
                palette: [
                    ["rgb(0, 0, 0)", "rgb(67, 67, 67)", "rgb(102, 102, 102)", "rgb(204, 204, 204)", "rgb(217, 217, 217)", "rgb(255, 255, 255)",
                    "rgb(152, 0, 0)", "rgb(255, 0, 0)", "rgb(255, 153, 0)", "rgb(255, 255, 0)", "rgb(0, 255, 0)",
                    "rgb(0, 255, 255)", "rgb(74, 134, 232)", "rgb(0, 0, 255)", "rgb(153, 0, 255)", "rgb(255, 0, 255)", "rgb(230, 184, 175)", "rgb(244, 204, 204)", "rgb(252, 229, 205)", "rgb(255, 242, 204)", "rgb(217, 234, 211)",
                    "rgb(208, 224, 227)", "rgb(201, 218, 248)", "rgb(207, 226, 243)", "rgb(217, 210, 233)", "rgb(234, 209, 220)",
                    "rgb(221, 126, 107)", "rgb(234, 153, 153)", "rgb(249, 203, 156)", "rgb(255, 229, 153)", "rgb(182, 215, 168)",
                    "rgb(162, 196, 201)", "rgb(164, 194, 244)", "rgb(159, 197, 232)", "rgb(180, 167, 214)", "rgb(213, 166, 189)",
                    "rgb(204, 65, 37)", "rgb(224, 102, 102)", "rgb(246, 178, 107)", "rgb(255, 217, 102)", "rgb(147, 196, 125)",
                    "rgb(118, 165, 175)", "rgb(109, 158, 235)", "rgb(111, 168, 220)", "rgb(142, 124, 195)", "rgb(194, 123, 160)",
                    "rgb(166, 28, 0)", "rgb(204, 0, 0)", "rgb(230, 145, 56)", "rgb(241, 194, 50)", "rgb(106, 168, 79)",
                    "rgb(69, 129, 142)", "rgb(60, 120, 216)", "rgb(61, 133, 198)", "rgb(103, 78, 167)", "rgb(166, 77, 121)",
                    "rgb(91, 15, 0)", "rgb(102, 0, 0)", "rgb(120, 63, 4)", "rgb(127, 96, 0)", "rgb(39, 78, 19)",
                    "rgb(12, 52, 61)", "rgb(28, 69, 135)", "rgb(7, 55, 99)", "rgb(32, 18, 77)", "rgb(76, 17, 48)"]
                ]
            });
        });
    }

    /*客户标签保存*/
    ObjectJS.saveColorModel = function (model) {
        var _self = this;
        Global.post("/System/SaveCustomerColor", { customercolor: JSON.stringify(model) }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            if (data.ID > 0) {
                ObjectJS.bindColorList();
            } else {
                alert("系统已存在相同标签颜色！");
                return;
            }
        });
    }

    /*客户来源弹窗*/
    ObjectJS.createModel = function () {
        var _self = this;
        doT.exec("template/system/sources-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !Model.SourceID ? "新建客户来源" : "编辑客户来源",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        Model.SourceName = $("#modelName").val();
                        Model.SourceCode = $("#modelCode").val();
                        Model.IsChoose = $("#isChoose").prop("checked") ? 1 : 0;
                        _self.saveModel(Model);
                    },
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

            if (Model.SourceID) {
                $("#modelCode").attr("disabled", "disabled");
            }

            $("#modelName").focus();
            $("#modelName").val(Model.SourceName);
            $("#modelCode").val(Model.SourceCode);
            $("#isChoose").prop("checked", true);
            if(Model.SourceID) $('.red').hide();
            // $("#isChoose").prop("checked", Model.IsChoose == 1);
        }); 
    }

    /*客户来源列表获取*/
    ObjectJS.getList = function () { 
        var _self = this;
        $("#sourceList .tr-header").nextAll().remove();
        $("#sourceList .tr-header").after("<tr><td colspan='6'><div class='data-loading' ><div></td></tr>");
        Global.post("/System/GetCustomSources", {}, function (data) {
            _self.bindList(data.items);
        });
    }

    /*客户来源列表填充*/
    ObjectJS.bindList = function (items) {
        $("#sourceList .tr-header").nextAll().remove();
        var _self = this; 
        if (items.length > 0) {
            doT.exec("template/system/sources.html", function (template) {
                var innerhtml = template(items);
                innerhtml = $(innerhtml);
                /* 下拉事件 */
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);
                    if (_this.data("type") == 1) {
                        $("#deleteObject").hide();
                    } else {
                        $("#deleteObject").show();
                    }
                    var position = _this.find(".ico-dropdown").position();
                    $("#ddlSource li").data("id", _this.data("id"));
                    $("#ddlSource").css({ "top": position.top + 20, "left": position.left - 55 }).show().mouseleave(function () {
                        $(this).hide();
                    }); 
                });

                /*绑定启用插件*/
                innerhtml.find(".status").switch({
                    open_title: "点击启用",
                    close_title: "点击禁用",
                    value_key: "value",
                    change: function (data, callback) {
                        _self.editIsChoose(data, data.data("id"), data.data("value"), callback);
                    }
                });

                $("#sourceList .tr-header").after(innerhtml);
            });
        } else {
            $("#sourceList .tr-header").after("<tr><td colspan='6'><div class='nodata-txt' >暂无数据!</div></td></tr>");
        }
    }

    /*客户来源删除*/
    ObjectJS.deleteModel = function (obj) {
        var _self = this;
        confirm("客户来源删除后不可恢复,确认删除吗？", function () {
            Global.post("/System/DeleteCustomSource", { id: obj.data("id") }, function (data) {
                if (data.result) {
                    _self.getList();
                }
            });
        });
    }

    /*客户来源类型修改*/
    ObjectJS.editIsChoose = function (obj, id, status, callback) {
        var _self = this;
        var model = {};
        model.SourceID = id;
        model.IsChoose = status ? 0 : 1;
        Global.post("/System/SaveCustomSource", {
            entity: JSON.stringify(model)
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            !!callback && callback(data.result);
        });
    }

    /*客户来源保存*/
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/System/SaveCustomSource", { entity: JSON.stringify(model) }, function (data) {
            if (data.result == 1) {
                _self.getList();
            } else if (data.result == 2) {
                alert("保存失败,编码已存在!");
            }
        });
    }

    /* 客户标签配置*/
    ObjectJS.bindColorList = function () { 
        $("#colorList").html('');
        var urlItem = "";
        Global.post("/System/GetCustomColor", {}, function (data) {
            if (data.items.length > 0) {
                for (var i = 0; i < data.items.length; i++) {
                    var item = data.items[i];
                    urlItem += '<li data-id="' + item.ColorID + '" data-value="' + item.ColorValue + '" data-name="' + item.ColorName + '"  class="color-item"><div class="left color-leftzuoyou" style=" border-right:18px solid ' + item.ColorValue + '; "></div><div class="left colordiv" style="background-color:' + item.ColorValue + '";>' + item.ColorName + '</div>' +
                        '<div class="dropdown left" style=" width:18px;background-color:' + item.ColorValue + '"> <span class="ico-dropdown-white" style="color:#000;"></span></div></li>';
                }
            } 
            $("#colorList").html(urlItem);
            $("#colorList").find(".color-item").click(function () {
                var _this = $(this);
                var position = _this.position(); 
                $(".colordrop li").data("id", _this.data("id"));
                $(".colordrop").css({ "top": position.top+40, "left": position.left+80 }).show().mouseleave(function () {
                    $('.colordrop').hide();
                }); 
            });  
        });
    }
 
    /* 客户标签删除*/
    ObjectJS.deleteColor = function (colorid, callback) {
        Global.post("/System/DeleteColor", { colorid: colorid }, function (data) {
            !!callback && callback(data.result);
        });
    }

    /* 位置计算*/
    ObjectJS.getLeft = function (b) {
        return 10 + $(".sourceul li").eq(b - 1).offset().left - $(".sourceul li").eq(0).offset().left;
    }
    /*客户会员等级列表*/
    ObjectJS.getMemberLevelList = function () {
        $(".memberlevelul").html('');
        $(".memberlevelul").html("<h1><div class='data-loading' ><div></h1>");
        Global.post("/System/GetClientMemberLevel", {}, function (data) {
            $(".memberlevelul").html('');
            var items = data.items;
            if (items.length > 0) {
                var innnerHtml = ''; 
                for (var i = 0; i < items.length; i++) {
                    //if (i == 0) {
                    //innnerHtml += "<li id='memberLi-1' class='lineHeight30'><div class='levelitem left' data-origin='-1' data-imgurl=''  data-integfeemore='0' data-name='" + items[i].Name + "' data-discountfee='" + items[i].DiscountFee + "' data-id='" + items[i].LevelID + "' title='点击替换等级图标 创建人:" + (items[i].CreateUser ? items[i].CreateUser.Name : "--") + "' >" +
                    //                        "<div class='left'><span  class='spanimg mTop5'><img name='MemberImg' id='MemberImg-1' style='display:inline-block;' class='memberimg'  src='/Content/menuico/custom.png' alt=''></span></div><span class='hide' id='SpanImg-1'></span>" +
                    //                        "<span  class='mLeft5 mRight5'>当客户积分在</span><input id='IntegFeeMore-1' name='IntegFeeMore'  class='width50 mRight5' type='text' disabled='disabled' value='0' /><span class='mRight5'>到</span>" +
                    //                        "<input id='changeFeeMore-1' name='IntegFeeMore' class='width50 mRight5' type='text' disabled='disabled' value='"+items[i].IntegFeeMore + "' /><span class='mRight5'>之间，可享受</span><input disabled='disabled' name='DiscountFee' class='width50 mRight5' type='text' value='1' />" +
                    //                        "<span  class='mRight5'>折优惠 &nbsp; | 等级名称</span><input class='width80 mRight5'  type='text' disabled='disabled'  name='MemberName' placeholder='请填写等级名' value='新客户' /><span id='delMeber-1' data-ind='-1' class='hide borderccc circle12 mLeft10'>X</span>" +
                    //                        "</div></li>";
                    //}
                    if (i == 0) {
                        innnerHtml += "<li id='memberLi" + i + "' class='lineHeight30'><div class='levelitem left' data-origin='" + (items[i].Origin - 1) + "' data-imgurl='" + items[i].ImgUrl + "'  data-integfeemore='" + items[i].IntegFeeMore + "' data-name='" + items[i].Name + "' data-discountfee='" + items[i].DiscountFee + "' data-id='" + items[i].LevelID + "' title='创建人:" + (items[i].CreateUser ? items[i].CreateUser.Name : "--") + "' >" +
                            "<div class='left'><span  class='spanimg mTop5' ><img name='MemberImg' id='MemberImg" + i + "' style='display:inline-block;' class='memberimg' title='点击替换等级图标 '  src='" + (items[i].ImgUrl != '' ? items[i].ImgUrl : '/Content/menuico/custom.png') + "' alt=''></span></div><span class='hide' id='SpanImg" + i + "'></span>" +
                            "<span  class='mLeft5 mRight5' style='display:inline-block;'>当客户积分在</span><input id='IntegFeeMore" + i + "' name='IntegFeeMore'  disabled='disabled' class='width50 mRight5' type='text' value='" + items[i].IntegFeeMore + "' /><span class='mRight5'>到</span>" +
                            "<input id='changeFeeMore" + i + "' name='IntegFeeMore' class='width50 mRight5' type='text' readOnly='readOnly' disabled='disabled'  value='" + (i == items.length - 1 ? '无上限' : items[i + 1].IntegFeeMore) + "' /><span class='mRight5'>之间，可享受</span><input name='DiscountFee' class='width50 mRight5' type='text' value='" + items[i].DiscountFee + "' />" +
                            "<span  class='mRight5'>折优惠 &nbsp; | 等级名称</span><input class='width80 mRight5' type='text' name='MemberName' placeholder='请填写等级名' value='" + items[i].Name + "' /><span id='delMeber" + i + "' data-ind='" + i + "' class='" + (i == 0 ? "hide" : i == items.length - 1 ? "" : "hide") + " borderccc circle12 mLeft10'>X</span>" +
                            "</div></li>";
                    } else {
                        innnerHtml += "<li id='memberLi" + i + "' class='lineHeight30'><div class='levelitem left' data-origin='" + (items[i].Origin - 1) + "' data-imgurl='" + items[i].ImgUrl + "'  data-integfeemore='" + items[i].IntegFeeMore + "' data-name='" + items[i].Name + "' data-discountfee='" + items[i].DiscountFee + "' data-id='" + items[i].LevelID + "' title='创建人:" + (items[i].CreateUser ? items[i].CreateUser.Name : "--") + "' >" +
                            "<div class='left'><span  class='spanimg mTop5' ><img name='MemberImg' id='MemberImg" + i + "' style='display:inline-block;' class='memberimg' title='点击替换等级图标 '  src='" + (items[i].ImgUrl != '' ? items[i].ImgUrl : '/Content/menuico/custom.png') + "' alt=''></span></div><span class='hide' id='SpanImg" + i + "'></span>" +
                            "<span  class='mLeft5 mRight5' style='display:inline-block;'>当客户积分在</span><input id='IntegFeeMore" + i + "' name='IntegFeeMore'  class='width50 mRight5' type='text' value='" + items[i].IntegFeeMore + "' /><span class='mRight5'>到</span>" +
                            "<input id='changeFeeMore" + i + "' name='IntegFeeMore' class='width50 mRight5' type='text' readOnly='readOnly' disabled='disabled'  value='" + (i == items.length - 1 ? '无上限' : items[i + 1].IntegFeeMore) + "' /><span class='mRight5'>之间，可享受</span><input name='DiscountFee' class='width50 mRight5' type='text' value='" + items[i].DiscountFee + "' />" +
                            "<span  class='mRight5'>折优惠 &nbsp; | 等级名称</span><input class='width80 mRight5' type='text' name='MemberName' placeholder='请填写等级名' value='" + items[i].Name + "' /><span id='delMeber" + i + "' data-ind='" + i + "' class='" + (i == 0 ? "hide" : i == items.length - 1 ? "" : "hide") + " borderccc circle12 mLeft10'>X</span>" +
                            "</div></li>";
                    }
                }
                $(".memberlevelul").html(innnerHtml);
                ObjectJS.bindMemberLi();
            } else {
                $(".memberlevelul").html("<h1><div class='nodata-txt' >暂无数据!<div></h1>");
            }
        });
    }
    /*客户会员等级弹窗*/
    ObjectJS.createMemberLevel = function () {
        var _self = this;
        var i = $('.levelitem').length - 1; 
        $('#delMeber' + i).hide();
        var intefee = parseInt($('#memberLi' +i + ' div:first-child').data('integfeemore'))+300;
        $('#changeFeeMore' + i).val(intefee);
        i = i + 1;
        var innnerHtml = "<li id='memberLi" + i + "' class='lineHeight30'><div class='levelitem left' data-origin='" + i + "' data-imgurl=''  data-integfeemore='" + intefee + "' data-name='' data-discountfee='1.00' data-id='' title='' >" +
                      "<div class='left'><span  class='spanimg mTop5' ><span class='hide' id='SpanImg" + i + "'></span><img name='MemberImg' style='display:inline-block;' id='MemberImg" + i + "' class='memberimg'   src='/Content/menuico/custom.png' alt=''></span></div>" +
                      "<span  class='mLeft5 mRight5'>当客户积分在</span><input id='IntegFeeMore" + i + "' name='IntegFeeMore' class='width50 mRight5' type='text' value='" + intefee + "' /><span class='mRight5'>到</span>" +
                      "<input id='changeFeeMore" + i + "'  class='width50 mRight5' placeholder='请填写积分' disabled='disabled'  type='text' value='无上限' /><span class='mRight5'>之间，可享受</span><input name='DiscountFee'  class='width50 mRight5' placeholder='请填写折扣'  type='text' value='1.00' />" +
                      "<span  class='mRight5'>折优惠 &nbsp; | 等级名称</span><input class='width80 mRight5' name='MemberName' type='text'  placeholder='请填写等级名' value='' /><span id='delMeber" + i + "' data-ind='" + i + "' class=' borderccc circle12 mLeft10'>X</span>" +
                      "</div></li>";
        $(".memberlevelul li:last-child").after(innnerHtml);
        _self.bindMemberLi();
    }
 
    ObjectJS.saveMemberLevel = function () {
        var list = [];
        var _self = this;
        var gonext = true;
        $('.levelitem').each(function (i, v) {
            if ($(v).data('origin') != '-1') {
                if ($(v).data('name') == '') {
                    gonext = false;
                }
                var item = {};
                item.IntegFeeMore = $(v).data('integfeemore');
                item.DiscountFee = $(v).data('discountfee');
                item.Name = $(v).data('name');
                item.ImgUrl = $(v).data('imgurl');
                item.Origin = parseInt($(v).data('origin')) + 1;
                item.LevelID = $(v).data('id');
                list.push(item);
            }
        });
        if (gonext) {
            Global.post("/System/SaveClientMemberLevel", { clientmemberlevel: JSON.stringify(list) }, function(data) {
                if (data.result == "") {
                    alert('等级配置成功');
                    _self.getMemberLevelList();
                } else {
                    alert(data.result);
                }
            });
        } else {
            alert('客户等级不能为空，请修改后再保存');
        }
    }

    ObjectJS.hideMember= function(ind) {
        $('#memberLi' + ind).remove();
        $('#changeFeeMore' + (ind - 1)).val('无上限');
        if (ind > 1) {
            $('#delMeber' + (ind - 1)).show();
        }
    }

    ObjectJS.bindMemberLi= function() {
        $(".circle12").click(function () { ObjectJS.hideMember($(this).parent().data("origin")); });
        $("input[name^='IntegFeeMore']").change(function () {
            ObjectJS.changeInput(1, $(this));
        });
        $("input[name^='DiscountFee']").change(function () {
            ObjectJS.changeInput(2, $(this));
        });
        $("input[name^='MemberName']").change(function () {
            ObjectJS.changeInput(3, $(this));
        }); 

        $("img[name^='MemberImg']").unbind('click').click(function () { 
            var _this = $(this); 
            var elem = "#SpanImg" + _this[0].id.replace('MemberImg', '');
            $(elem).html(''); 
            Upload.createUpload({
                element: elem,
                buttonText: "",
                className: "",
                data: { folder: '', action: 'add', oldPath: '' },
                success: function (data, status) {
                    if (data.Items.length > 0) { 
                        _this.attr("src", data.Items[0]); 
                        _this.parent().parent().data('imgurl', data.Items[0]);
                    } else {
                        alert("只能上传jpg/png/gif类型的图片，且大小不能超过1M！");
                    }
                }
            });   
            $( elem + '_buttonSubmit').click(); 
        });
    }

    ObjectJS.changeInput = function (type,_this) { 
        var s = parseInt(_this.parent().data("origin"))-1;
        if (type == 1){
            if (reg.test(_this.val())) {
                if (s != $('.levelitem').length) {
                    if (parseInt($('#IntegFeeMore' + (s+2)).val()) <= parseInt(_this.val())) {
                        alert('当前积分阶段不能大于等于下一等级积分阶段');
                        _this.val(_this.parent().data('integfeemore'));
                    } if (parseInt($('#IntegFeeMore' +s).val()) >= parseInt(_this.val())) {
                        alert('当前积分阶段不能小于等于上一等级积分阶段');
                        _this.val(_this.parent().data('integfeemore'));
                    } else {
                        $('#changeFeeMore' + s).val(_this.val());
                        _this.parent().data('integfeemore', _this.val());
                    }
                } else {
                    _this.parent().data('integfeemore', _this.val());
                }
            } else {
                alert('积分格式输入有误，请重新输入');
                _this.val(_this.parent().data('integfeemore'));
            }
        } else if (type == 2) { 
            if (!reg2.test(_this.val())) {
                alert('折扣格式输入有误，请重新输入');
                _this.val(_this.parent().data('discountfee'));
            } else {
                _this.parent().data('discountfee', _this.val());
            }
        } else if (type == 3) {
            if (_this.val() != '') {
                _this.parent().data('name', _this.val());
            }
        }  
    }

    ObjectJS.saveClientRule= function() {
        Global.post("/System/SaveClietRule", { integerFee: $('#integerFee').val() }, function (data) {
            if (data.result) {
                alert('积分兑换比例设置成功');
                $('#integerFee').data('oldvalue', $('#integerFee').val());
                $('#saveClientRule').hide();
            } else {
                alert('操作失败');
            }
        });
    }
    ObjectJS.setToCustomers= function() {
        Global.post("/System/ChangeCumoterLevel", null, function (data) {
            if (data.result) {
                alert('客户等级刷新成功'); 
            } else {
                alert('操作失败');
            }
        });
    }
    module.exports = ObjectJS;
});