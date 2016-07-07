define(function (require, exports, module) {
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

    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.getList();
    }
     
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(document).click(function (e) { 
            /*隐藏下拉*/ 
            if ((!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown"))
                && !$(e.target).hasClass("industrylist")  && !$(e.target).parents().hasClass("levelitem") && (!$(e.target).parents().hasClass("color-item") && !$(e.target).hasClass("color-item"))) {
                $(".dropdown-ul").hide();
            }
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
            _self.Industry = {};
            _self.Industry.ClientIndustryID = "";
            _self.Industry.Description = "";
            _self.Industry.Name = ""; 
            _self.createIndustry();
        });
        /*添加会员等级*/
        $("#createMeber").click(function () {
            _self.MemberLevel = {};
            _self.MemberLevel.IntegFeeMore = ""; 
            _self.MemberLevel.DiscountFee = "";
            _self.MemberLevel.Name = "";
            _self.MemberLevel.ImgUrl = "";
            _self.createMemberLevel();
        });
        $("#deleteObject").click(function () { 
            ObjectJS.deleteModel($(this));
        });

        $("#updateObject").click(function () {
            ObjectJS.SourceEdit($(this));
        });

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
                } else if (_this.data("id") == "industryList" && (!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                   
                    _self.getIndustryList();
                } else if (_this.data("id") == "memberList" && (!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1"); 
                    _self.getMemberLevelList();
                }

                $("#deleteObject").unbind().click(function () {
                    var _thisnow = $(this);
                    if (_this.data("id") == "industryList") {
                        confirm("客户行业删除后不可恢复,确认删除吗？", function () {
                            Global.post("/System/DeleteClientIndustry", { clientindustryid: _thisnow.data("id") }, function (data) {
                                if (data.result) {
                                    _self.getIndustryList();
                                } else {
                                    alert("行业存在关联数据，删除失败");
                                }
                            });
                        });
                    } else if (_this.data("id") == "sourceList") {
                        ObjectJS.deleteModel(_thisnow);
                    } else if (_this.data("id") == "memberList") {
                        confirm("客户等级删除后不可恢复,确认删除吗？", function () {
                            Global.post("/System/DeleteClientMemberLevel", { levelid: _thisnow.data("id") }, function (data) {
                                if (data.result=="") {
                                    _self.getMemberLevelList();
                                } else {
                                    alert("此客户等级已被使用，删除失败");
                                }
                            });
                        });
                    }
                });

                $("#updateObject").unbind().click(function () {
                    var _thisnow = $(this);
                    if (_this.data("id") == "sourceList") {
                        ObjectJS.SourceEdit($(this));
                    } else if (_this.data("id") == "industryList") {
                        _self.Industry = {};
                        _self.Industry.ClientIndustryID = _thisnow.data('id');
                        _self.Industry.Name = _thisnow.data('name');
                        _self.Industry.Description = _thisnow.data('description');
                        _self.createIndustry();
                    } else if (_this.data("id") == "memberList") {
                        _self.MemberLevel = {};
                        _self.MemberLevel.LevelID = _thisnow.data('id');
                        _self.MemberLevel.Name = _thisnow.data('name');
                        _self.MemberLevel.DiscountFee = _thisnow.data('discountfee'); 
                        _self.MemberLevel.IntegFeeMore = _thisnow.data('integfeemore');
                        _self.MemberLevel.ImgUrl = _thisnow.data('imgurl');
                        _self.createMemberLevel();
                    }
                });
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
            $("#sourceList .tr-header").after("<tr><td colspan='6'><div class='nodata-txt' >暂无数据!<div></td></tr>");
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
                    urlItem += '<li data-id="' + item.ColorID + '" data-value="' + item.ColorValue + '" data-name="' + item.ColorName + '"  class="color-item"><div class="left color-leftzuoyou" style=" border-right:18px solid ' + item.ColorValue + '; "></div><div class="left colordiv" style="background-color:' + item.ColorValue + '";>' + item.ColorName + '</div></li>';
                }
            } 
            $("#colorList").html(urlItem);
            $("#colorList").find(".color-item").click(function () {
                var _this = $(this);
                var position = _this.position(); 
                $(".colordrop li").data("id", _this.data("id"));
                $(".colordrop").css({ "top": position.top+45, "left": position.left+30 }).show().mouseleave(function () {
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

    /*客户行业列表*/
    ObjectJS.getIndustryList = function () { 
        $("#industryList .table-list").nextAll().remove();
        $("#industryList .table-list").after("<tr><td colspan='5'><div class='data-loading' ><div></td></tr>");
        Global.post("/System/GetClientIndustry", {}, function (data) {
            $("#industryList .tr-header").nextAll().remove();
            var items = data.items;
            if (items.length > 0) {
                var innerhtml = "";
                for (var i = 0; i < items.length; i++) {
                    innerhtml += "<h1 class='industrylist left' data-id='" + items[i].ClientIndustryID + "' data-name='" + items[i].Name + "' data-description='" + items[i].Description + "' title='创建人:" + (items[i].CreateUser ? items[i].CreateUser.Name : "--") + "' >" + items[i].Name + "</h1>";
                } 
                $("#industryList .table-list").after(innerhtml);
                $(".industrylist").click(function () {
                    var _this = $(this);
                    var position = _this.position(); 
                    $("#ddlSource li").data("id", _this.data("id"));
                    $("#ddlSource li").data("name", _this.data("name"));
                    $("#ddlSource li").data("description", _this.data("description"));
                    $("#ddlSource").css({ "top": position.top +130, "left": position.left+5 }).show().mouseleave(function () {
                        $(this).hide();
                    }); 
                });
            } else {
                $("#industryList .table-list").after("<tr><td colspan='5'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
        });
    }

    /*客户行业弹窗*/
    ObjectJS.createIndustry = function () {
        var _self = this;
        doT.exec("template/system/clientindustry-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !_self.Industry.ClientIndustryID ? "新建行业" : "编辑行业",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        _self.Industry.Name = $("#Name").val();
                        _self.Industry.Description = $("#Description").val();
                        _self.saveIndustry();
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
            $("#Name").focus();
            $("#Name").val(_self.Industry.Name);
            $("#Description").val(_self.Industry.Description);
        });
    }

    /*客户行业保存*/
    ObjectJS.saveIndustry = function () {
        var _self = this;
        Global.post("/System/SaveClientIndustry", { clientindustry: JSON.stringify(_self.Industry) }, function (data) {
            if (data.result == 1) {
                _self.getIndustryList();
            } else if (data.result == 2) {
                alert("保存失败,行业已存在!");
            } else{
                alert(result);
            }
        });
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
                    /* style='background-image:url(" + items[i].ImgUrl + "); background-size:contain;'*/
                    innnerHtml += "<li><div class='levelitem left' data-imgurl='" + items[i].ImgUrl + "'  data-integfeemore='" + items[i].IntegFeeMore + "' data-name='" + items[i].Name + "' data-discountfee='" + items[i].DiscountFee + "' data-id='" + items[i].LevelID + "' title='创建人:" + (items[i].CreateUser ? items[i].CreateUser.Name : "--") + "' ></tr>" +
                        "<div class='mTop5 mLeft5 mBottom5'> <table width='100%'><tbody><tr> <td colspan='2' style='width:100%;text-align:center;font-size:14px;'><span style='width:25px;height:25px;border-radius:50%;' class='left'>" +
                        "<img style='width:100% ;height:100%;border-radius:50%;' src='" +( items[i].ImgUrl != '' ? items[i].ImgUrl : '/Content/menuico/custom.png' )+ "' alt=''></span><span class='left mLeft5 pTop3'>" + items[i].Name + "</span></td></tr>" +
                        "<tr class='mTop5'><td class='color999 width50'>升级条件:</td><td> " + (i==0?"0":items[i-1].IntegFeeMore) + "<会员积分<="+items[i].IntegFeeMore+ "</td>" +
                        "<tr><td class='color999'>折扣:</td><td > " + items[i].DiscountFee + "</td></tr>" +
                        "</tbody></table> </div></div></li>";
                }
                $(".memberlevelul").html(innnerHtml);
                $(".levelitem").click(function () {
                    var _this = $(this);
                    var position = _this.position(); 
                    $("#ddlSource li").data("id", _this.data("id"));
                    $("#ddlSource li").data("name", _this.data("name"));
                    $("#ddlSource li").data("discountfee", _this.data("discountfee")); 
                    $("#ddlSource li").data("integfeemore", _this.data("integfeemore"));
                    $("#ddlSource li").data("imgurl", _this.data("imgurl"));
                    $("#ddlSource").css({ "top": position.top , "left": position.left + 210 }).show().mouseleave(function () {
                        $(this).hide();
                    });
                });
            } else {
                $(".memberlevelul").html("<h1><div class='nodata-txt' >暂无数据!<div></h1>");
            }
        });
    }
    /*客户会员等级弹窗*/
    ObjectJS.createMemberLevel = function () {
        var _self = this;
        doT.exec("template/system/clientmemberlevel-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !_self.MemberLevel.LeveID ? "新建等级" : "编辑等级",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        _self.MemberLevel.Name = $("#Name").val();
                        _self.MemberLevel.IntegFeeMore = $("#IntegFeeMore").val(); 
                        _self.MemberLevel.DiscountFee = $("#DiscountFee").val();
                        _self.MemberLevel.ImgUrl= $("#ImgUrl").attr("src"),
                        _self.saveMemberLevel();
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
            $("#Name").focus();
            $("#Name").val(_self.MemberLevel.Name);
            $("#IntegFeeMore").val(_self.MemberLevel.IntegFeeMore); 
            $("#DiscountFee").val(_self.MemberLevel.DiscountFee);
            $("#ImgUrl").attr("src", _self.MemberLevel.ImgUrl);
            Upload.createUpload({
                element: "#ImgUrlUpload",
                buttonText: "选择等级图标",
                className: "",
                data: { folder: '', action: 'add', oldPath: _self.MemberLevel.ImgUrl },
                success: function (data, status) {
                    if (data.Items.length > 0) {
                        $("#ImgUrl").attr("src", data.Items[0]);
                    } else {
                        alert("只能上传jpg/png/gif类型的图片，且大小不能超过1M！");
                    }
                }
            });
        });
    }
    /*客户行业保存*/
    ObjectJS.saveMemberLevel = function () {
        var _self = this;
        var reg = /^\d+(\.\d+)?$/;
        //var reg2 = /^\d[0]+(\.\d+)?$/;
        var reg2 = /^(((\d[0]\.\d+))|\+?0\.\d*|\+?1)$/;
        if (!reg2.test(_self.MemberLevel.DiscountFee) && parseFloat(_self.MemberLevel.DiscountFee)>1) {
            alert('折扣格式输入有误,请修改后在提交');
            return false;
        }
        if (!reg.test(_self.MemberLevel.IntegFeeMore)) {
            alert('满足条件格式输入有误!请修改后在提交');
            return false;
        } 
        Global.post("/System/SaveClientMemberLevel", { clientmemberlevel: JSON.stringify(_self.MemberLevel) }, function (data) {
            if (data.result == "") {
                _self.getMemberLevelList();
            } else {
                alert(data.result);
            }
        });
    }

    module.exports = ObjectJS;
});