define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");
    var Model = {};

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.bindElement($(".stages-item"));
        _self.bingStyle();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $(window).resize(function () {
            _self.bingStyle();
        });

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).hasClass("operatestage")) {
                $("#ddlStage").hide();
            }

            if (!$(e.target).hasClass("operateitem")) {
                $("#ddlItem").hide();
            }
        });
        //添加新阶段
        $("#addObject").click(function () {

            _self.showLayer("", "", $(this).data("sort") * 1 + 1, 0);
        });

        //删除阶段
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("机会阶段删除后不可恢复,确认删除吗？", function () {
                _self.deleteModel(_this.data("id"), function (status) {
                    if (status) {
                        alert("阶段删除成功", function () {
                            location.href = location.href;
                        });
                    } else {
                        alert("此阶段存在关联机会，删除失败");
                    }
                });
            });
        });

        //编辑阶段名称
        $("#editObject").click(function () {
            var _this = $(this);
            _self.showLayer(_this.data("id"), $("#" + _this.data("id")).html(), _this.data("sort"), _this.data("probability"));
        });

        //编辑项目名称
        $("#editItem").click(function () {

            var _this = $(this), item = $("#" + _this.data("id"));

            item.hide();
            item.parent().next().show();

            item.parent().next().find("textarea").data("stageid", _this.data("stageid")).data("id", _this.data("id")).focus();

            item.parent().next().find("textarea").val(item.find(".itemname").html());
            
        });

        //删除阶段行为
        $("#deleteItem").click(function () {
            var _this = $(this);
            confirm("阶段行为删除后不可恢,确认删除吗？", function () {
                Global.post("/System/DeleteStageItem", {
                    id: _this.data("id"),
                    stageid: _this.data("stageid")
                }, function (data) {
                    if (data.status) {
                        $("#" + _this.data("id")).remove();
                    } else {
                        alert("系统异常!");
                    }
                })
            });
        });

    }

    //添加、编辑浮层
    ObjectJS.showLayer = function (id, name, sort, probability) {
        var _self = this;
        doT.exec("template/system/opportunitystage-detail.html", function (template) {
            var innerText = template([]);
            Easydialog.open({
                container: {
                    id: "addOrderStage",
                    header: id ? "编辑阶段" : "添加阶段",
                    content: innerText,
                    yesFn: function () {
                        if (!$("#stagename").val().trim()) {
                            alert("阶段名称不能为空");
                            return false;
                        }
                        if (!$("#probability").val().trim().isDouble() || $("#probability").val().trim() < 0 || $("#probability").val().trim() > 100) {
                            alert("成交概率只能是大于0小于等于100的数字");
                            return false;
                        }
                        var model = {
                            StageID: id,
                            StageName: $("#stagename").val().trim(),
                            Probability: $("#probability").val().trim() / 100,
                            Sort: sort
                        };
                        _self.saveModel(model);
                    },
                    callback: function () {

                    }
                }
            });

            if (id) {
                $("#stagename").val(name);
                $("#probability").val(probability);
            }

            $("#addOrderStage .stage-item").click(function () {
                var _this = $(this);
                if (_this.hasClass("hover")) {
                    _this.removeClass("hover");
                } else {
                    _this.siblings().removeClass("hover");
                    _this.addClass("hover");
                }
            });
        });
    }

    //元素绑定事件
    ObjectJS.bindElement = function (items) {
        var _self = this;
        //下拉事件
        items.find(".operatestage").click(function () {
            var _this = $(this);
            if (_this.data("type") != 0) {
                $("#deleteObject").hide();
            } else {
                $("#deleteObject").show();
            }
            var offset = _this.offset();
            $("#ddlStage li").data("id", _this.data("id")).data("sort", _this.data("sort")).data("probability", _this.data("probability"));
            var left = offset.left;
            if (left > document.documentElement.clientWidth - 150) {
                left = left - 150;
            }
            $("#ddlStage").css({ "top": offset.top + 20, "left": left }).show().mouseleave(function () {
                $(this).hide();
            });
        });

        //添加行为项
        items.find(".create-child").click(function () {
            var _this = $(this);
            _this.prev().show();
            _this.prev().find("textarea").data("stageid", _this.data("id")).data("id", "").val("").focus();
        });

        //行为文本改变事件
        items.find(".create-action textarea").blur(function () {
            var _this = $(this);
            if (!_this.val().trim()) {
                _this.parent().hide();
                return;
            }
            if (_this.data("id") && _this.val().trim() == $("#" + _this.data("id")).find(".itemname").html().trim()) {
                _this.parent().hide();
                $("#" + _this.data("id")).show();
                return;
            }
            var model = {
                ItemID: _this.data("id"),
                ItemName: _this.val().trim(),
                StageID: _this.data("stageid")
            };
            Global.post("/System/SaveStageItem", { entity: JSON.stringify(model) }, function (data) {
                if (data.model.ItemID) {
                    if (model.ItemID) {
                        $("#" + _this.data("id")).find(".itemname").html(model.ItemName);
                        $("#" + _this.data("id")).show();
                    } else {
                        var ele = $('<li id="' + data.model.ItemID + '">' +
                                        '<span class="itemname width200 long">' + model.ItemName + '</span>' +
                                        '<span data-id="' + data.model.ItemID + '" data-stageid="' + model.StageID + '" class="ico-dropdown operateitem"></span>' +
                                    '</li> ');
                        _this.parent().prev(".child-items").append(ele);

                        _self.bindElement(ele);
                    }
                    _this.parent().hide();
                } else {
                    alert("系统异常!");
                }
            });

        });

        //行为项下拉事件
        items.find(".operateitem").click(function () {
            var _this = $(this);
            var offset = _this.offset();
            $("#ddlItem li").data("id", _this.data("id")).data("stageid", _this.data("stageid"));
            var left = offset.left;
            if (left > document.documentElement.clientWidth - 150) {
                left = left - 150;
            }
            $("#ddlItem").css({ "top": offset.top + 20, "left": left }).show().mouseleave(function () {
                $(this).hide();
            });
        });
    }

    //保存阶段实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/System/SaveStage", { entity: JSON.stringify(model) }, function (data) {
            if (data.status == 1) {
                alert("保存成功", function () {
                    location.href = location.href;
                });
                
            } else {
                alert("系统异常，请稍后重试");
            }
        });
    }

    //删除
    ObjectJS.deleteModel = function (id, callback) {
        Global.post("/System/DeleteStage", { id: id }, function (data) {
            !!callback && callback(data.status);
        })
    }

    //高度控制
    ObjectJS.bingStyle = function () {
        var height = document.documentElement.clientHeight;
        $(".child-items").css("max-height", height - 330);
        $(".stages-box").css("height", height - 230);
    }

    module.exports = ObjectJS;
});