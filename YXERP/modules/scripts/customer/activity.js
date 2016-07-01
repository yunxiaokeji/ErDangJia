//活动模块
//MU
//2015-11-29

define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Upload = require("upload"), PosterIco, editor,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser"),
        moment = require("moment");
        require("daterangepicker");
        require("pager");

    var Model = {};

    var ObjectJS = {};

    ObjectJS.Params = {
        PageSize: 10,
        PageIndex:1,
        KeyWords: "",
        IsAll: 0,//0：我的活动；1：所有活动
        Stage:-1,
        BeginTime: "",
        EndTime: "",
        FilterType: 0,
        UserID:'',
        DisplayType: 1,//1：列表；2：卡片
        OrderBy: "CreateTime desc"
    };

    ////初始化 列表
    ObjectJS.init = function (isAll) {
        var _self = this;
        _self.Params.IsAll = isAll;
        _self.Params.PageSize = Math.floor(($(".activityList").width() - 20) / 290) * 3;
        _self.getList();
        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

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
            ObjectJS.Params.PageIndex = 1;
            ObjectJS.Params.BeginTime = start ? start.format("YYYY-MM-DD") : "";
            ObjectJS.Params.EndTime = end ? end.format("YYYY-MM-DD") : "";
            _self.getList();
        });

        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.KeyWords = keyWords;
                ObjectJS.getList();
            });
        });

        //加载下属
        if (_self.Params.IsAll != 0) {
            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "人员-",
                    defaultText: "全部",
                    defaultValue: "",
                    userid: "-1",
                    isTeam: false,
                    width: "180",
                    onChange: function (data) {
                        ObjectJS.Params.PageIndex = 1;
                        ObjectJS.Params.UserID = data.userid;
                        ObjectJS.getList();
                    }
                });
            });
        }

        //切换阶段
        $(".search-type li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.FilterType = _this.data("id");
                ObjectJS.getList();
            }
        });

        //删除活动
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("确认删除活动吗?", function () {
                Global.post("/Activity/DeleteActivity", { activityID: _this.data("id") }, function (data) {
                    if (data.Result == 1) {
                        if (ObjectJS.Params.IsAll == 0)
                            location.href = "/Activity/MyActivity";
                        else
                            location.href = "/Activity/Activitys";
                    }
                    else {
                        alert("删除失败");
                    }
                });
            });
        });

        //编辑活动
        $("#setObjectRole").click(function () {
            var _this = $(this);
            location.href = "/Activity/Operate/" + _this.data("id");
        });

        //显示模式切换
        $(".display-tab").click(function () {
            var type = parseInt($(this).data("type"));
            ObjectJS.Params.DisplayType = type;

            if (type == 1) {
                $(this).find("img").attr("src", "/modules/images/ico-list-blue.png");
                $(this).next().find("img").attr("src", "/modules/images/ico-card-gray.png");
                $(".activityList").html('');

                $(".activityList").show();
                $(".activityCardList").hide();
            }
            else {
                $(this).find("img").attr("src", "/modules/images/ico-card-blue.png");
                $(this).prev().find("img").attr("src", "/modules/images/ico-list-gray.png");
                $(".activityCardList").html('');

                $(".activityList").hide();
                $(".activityCardList").show();
            }

            ObjectJS.getList();
        });

        //切换阶段
        $(".search-stages li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.Stage = _this.data("id");
                ObjectJS.getList();
            }
        });

        //排序
        $(".sort-item").click(function () {
            var _this = $(this);
            if (_this.hasClass("hover")) {
                if (_this.find(".asc").hasClass("hover")) {
                    _this.find(".asc").removeClass("hover");
                    _this.find(".desc").addClass("hover");
                    ObjectJS.Params.OrderBy = _this.data("column") + " desc ";
                } else {
                    _this.find(".desc").removeClass("hover");
                    _this.find(".asc").addClass("hover");
                    ObjectJS.Params.OrderBy = _this.data("column") + " asc ";
                }
            } else {
                _this.addClass("hover").siblings().removeClass("hover");
                _this.siblings().find(".hover").removeClass("hover");
                _this.find(".desc").addClass("hover");
                ObjectJS.Params.OrderBy = _this.data("column") + " desc ";
            }
            _self.getList();
        });

        //隐藏下拉
        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (ObjectJS.Params.DisplayType == 1)
        {
            $(".activityList").html("<tr><td><div class='data-loading' ><div></td></tr>");
        }
        else {
            $(".activityCardList").html("<li ><div class='data-loading'></div></li>");
        }

        Global.post("/Activity/GetActivityList",
            {
                pageSize: ObjectJS.Params.PageSize,
                pageIndex: ObjectJS.Params.PageIndex,
                keyWords: ObjectJS.Params.KeyWords,
                isAll: ObjectJS.Params.IsAll,
                beginTime: ObjectJS.Params.BeginTime,
                endTime: ObjectJS.Params.EndTime,
                stage: ObjectJS.Params.Stage,
                filterType: ObjectJS.Params.FilterType,
                userID: ObjectJS.Params.UserID,
                orderBy: ObjectJS.Params.OrderBy
            },
            function (data) {
                _self.bindList(data.Items);

                $("#pager").paginate({
                    total_count: data.TotalCount,
                    count: data.PageCount,
                    start: _self.Params.PageIndex,
                    display: 5,
                    images: false,
                    mouse: 'slide',
                    onChange: function (page) {
                        _self.Params.PageIndex = page;
                        _self.getList();
                    }
                });
            }
        );
    }

    //加载列表
    ObjectJS.bindList = function (items) {
        if (items.length > 0)
        {
            if (ObjectJS.Params.DisplayType == 1) 
            {
                doT.exec("template/activity/activity_list.html", function (template) {
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);

                    //操作
                    innerhtml.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown-white").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));

                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 39 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    });

                    $(".activityList").html(innerhtml);

                    require.async("businesscard", function () {
                        $(".activitymember").businessCard();
                    });

                });
            }
            else
            {
                doT.exec("template/activity/activity_card_list.html", function (template) {
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);

                    //操作
                    innerhtml.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown-white").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));

                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 30 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    });

                    $(".activityCardList").html(innerhtml);

                    require.async("businesscard", function () {
                        $(".activitymember").businessCard();
                    });
                });
            }
        }
        else
        {
            if (ObjectJS.Params.DisplayType == 1)
            {
                $(".activityList").html("<tr><td><div class='nodata-txt'>暂无数据!<div></td></tr>");
            }
            else 
            {
                $(".activityCardList").html("<li><div class='nodata-txt'>暂无数据!</div></li>");
            }
        }
    }

    ////初始化操作 编辑、新增
    ObjectJS.initOperate = function (Editor, id) {
        var _self = this;
        editor = Editor;

        _self.bindOperateEvent();

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        if (id) {
            $(".header-title").html("编辑活动");
            _self.getDetail(1);
        }
        else
            ObjectJS.getUserDetail();

        setTimeout(function () { $(".edui-container").css("z-index", "600") }, 1000);
            
    }

    //绑定事件
    ObjectJS.bindOperateEvent = function () {
        var _self = this;

        //选择海报图片
        PosterIco = Upload.createUpload({
            element: "#Poster",
            buttonText: "选择活动海报图片",
            className: "",
            data: { folder: '/Content/tempfile/', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0)
                {
                    _self.IcoPath = data.Items[0];
                    $("#PosterDisImg").show().attr("src", data.Items[0]);
                    $("#PosterImg").val(data.Items[0]);
                }
            }
        });

        //添加负责人
        $("#addOwner").click(function () {
            ChooseUser.create({
                title: "添加负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    for (var i = 0; i < items.length; i++) {
                        _self.createMember(items[i], "OwnerIDs",true);
                    }
                }
            });
        });

        //添加成员
        $("#addMember").click(function () {
            ChooseUser.create({
                title: "添加成员",
                type: 1,
                single: false,
                callback: function (items) {
                    for (var i = 0; i < items.length; i++) {
                        _self.createMember(items[i], "MemberIDs",false);
                    }

                }
            });
        });

        //保存活动
        $("#btnSaveActivity").click(function () {
            if (!VerifyObject.isPass()) {
                return;
            };

            var OwnerID='', MemberID='';
            $("#OwnerIDs .member").each(function () {
                OwnerID += $(this).data("id")+"|";
            });
            $("#MemberIDs .member").each(function () {
                MemberID += $(this).data("id") + "|";
            });
            if (OwnerID == '')
            {
                alert("请设置任务负责人");
                return;
            }

            var BeginTime = '', EndTime = '';
            BeginTime=$("#BeginTime").val();
            EndTime=$("#EndTime").val();
            if (BeginTime == '' || EndTime == '')
            {
                alert("请选择开始时间和结束时间");
                return;
            }

            var model = {
                ActivityID: $("#ActivityID").val(),
                Name: $("#Name").val(),
                Poster: $("#PosterImg").val(),
                OwnerID: OwnerID,
                MemberID: MemberID,
                BeginTime: BeginTime,
                EndTime: EndTime,
                Address: $("#Address").val(),
                Remark: encodeURI(editor.getContent())
            };

            _self.saveModel(model);
        });
        
    }

    //获取详情//option=1 编辑页；option=2 详情页
    ObjectJS.getDetail = function (option) {
        var _self = this;
        Global.post("/Activity/GetActivityDetail", { activityID: $("#ActivityID").val() }, function (data) {
            if (data.Item) {
                var item = data.Item;

                if (option == 1) {
                    $("#Name").val(item.Name);
                    $("#EndTime").val(item.EndTime.toDate("yyyy-MM-dd"));
                    $("#BeginTime").val(item.BeginTime.toDate("yyyy-MM-dd"));
                    $("#Address").val(item.Address);
                    $("#PosterImg").val(item.Poster);
                    if (item.Poster != '') {
                        $("#PosterDisImg").attr("src", item.Poster).show();
                    }

                    ObjectJS.createMemberDetail(item.Owner, "OwnerIDs");
                    for (var i = 0; i < item.Members.length; i++) {
                        ObjectJS.createMemberDetail(item.Members[i], "MemberIDs");
                    }

                    editor.ready(function () {
                        editor.setContent(decodeURI(item.Remark));
                    });

                } else {
                    $("#Name").html(item.Name);
                    if (item.Owner) {
                        $("#OwnerName").html(item.Owner.Name);
                        $("#OwnerName").data("id", item.Owner.UserID);
                    }
                    $("#EndTime").html(item.EndTime.toDate("yyyy-MM-dd"));
                    $("#BeginTime").html(item.BeginTime.toDate("yyyy-MM-dd"));
                    $("#Address").html(item.Address);
                    $("#Remark").html(decodeURI(item.Remark));

                    if (item.Poster != '') {
                        $("#Poster").attr("src", item.Poster);
                    }

                    if (item.Members) {
                        for (var i = 0; i < item.Members.length; i++) {
                            var m = item.Members[i];
                            $("#MemberList").append("<li class='member' data-id='" + m.UserID + "'>" + m.Name + "</li>");
                        }
                    }
                    require.async("sharemingdao", function () {
                        $("#btn_shareMD").sharemingdao({
                            post_pars: {
                                content: $("#Name").html(),
                                groups: [],
                                share_type: 0
                            },
                            task_pars: {
                                name: $("#Name").html(),
                                end_date: $("#EndTime").html(),
                                charger: item.Owner,
                                members: item.Members,
                                des: '',
                                url: "/Activity/Detail?id=" + $("#ActivityID").val() + "&source=md"
                            },
                            schedule_pars: {
                                name: $("#Name").html(),
                                start_date: $("#BeginTime").html(),
                                end_date: $("#EndTime").html(),
                                members: item.Members,
                                address: $("#Address").html(),
                                des: '',
                                url: "/Activity/Detail?id=" + $("#ActivityID").val() + "&source=md"
                            },
                            callback: function (type, url) {
                                if (type == "Calendar") {
                                    url = "<a href='" + url + "' target='_blank'>分享明道日程，点击查看详情</a>";
                                } else if (type == "Task") {
                                    url = "<a href='" + url + "' target='_blank'>分享明道任务，点击查看详情</a>";
                                }
                                var entity = {
                                    ActivityID: $("#ActivityID").val(),
                                    Msg: encodeURI(url),
                                    FromReplyID: "",
                                    FromReplyUserID: "",
                                    FromReplyAgentID: ""
                                };

                                ObjectJS.SaveActivityReply(entity);
                            }
                        });
                    });
                }

                $("#OwnerID").val(item.OwnerID);
                $("#MemberID").val(item.MemberID);

                //require.async("businesscard", function () {
                //    $(".member").businessCard();
                //});
            }
        });
    }

    //获取当前用户实体
    ObjectJS.getUserDetail = function () {
        Global.post("/Activity/GetUserDetail", null, function (data) {
            ObjectJS.createMemberDetail(data.Item, "OwnerIDs");
        });
    }

    //拼接一个用户成员
    ObjectJS.createMember = function (item, id, isSingle) {
        if ($("#" + id + " div[data-id='" + item.id + "']").html())
            return false;

        var html = '<div class="member left" data-id="' + item.id + '">';
        html += '    <div class="left pRight5">';
        html += '          <span>' + item.name + '</span>';
        html += '     </div>';
        html += '      <div class="left mRight10 pLeft5"><a href="javascript:void(0);" onclick="$(this).parents(\'.member\').remove();">×</a></div>';
        html += '      <div class="clear"></div>';
        html += '   </div>';

        if (isSingle)
            $("#" + id).html(html);
        else
            $("#" + id).append(html);

        //require.async("businesscard", function () {
        //    $("div.member").businessCard();
        //});
    }

    //拼接一个用户成员
    ObjectJS.createMemberDetail = function (item, id) {
        if (item.Avatar == '')
            item.Avatar = "/modules/images/defaultavatar.png";

        var html = '<div class="member left" data-id="' + item.UserID + '">';
        html += '    <div class="left pRight5">';
        html += '          <span>' + item.Name + '</span>';
        html += '     </div>';
        html += '      <div class="left mRight10 pLeft5"><a href="javascript:void(0);" onclick="$(this).parents(\'.member\').remove();">×</a></div>';
        html += '      <div class="clear"></div>';
        html += '   </div>';

        $("#" + id).append(html); 
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/Activity/SavaActivity", { entity: JSON.stringify(model) }, function (data) {
            if (data.ID.length > 0) {
                location.href = "/Activity/Detail/" + data.ID;
            }
        })
    }

    ////初始化 详情
    ObjectJS.initDetail = function (activityid) {

        var _self = this;
        _self.activityid = activityid;
        _self.bindDetailEvent();
        _self.getDetail(2);
    }

    //绑定事件
    ObjectJS.bindDetailEvent = function () {
        var _self = this;
      
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            if ($(this).data("id") == "activityCustoms" && (!$(this).data("first") || $(this).data("first") == 0)){
                $(this).data("first", "1");
                _self.getCustomersByActivityID();
            }
        });

        require.async("replys", function () {
            $("#navRemark").getObjectReplys({
                guid: _self.activityid,
                type: 3, /*1 客户 2订单 3活动 4产品 5员工 7机会 */
                pageSize: 10
            });
        });
    }

    //获取活动对应客户列表
    ObjectJS.getCustomersByActivityID = function () {
        var _self = this;
        $(".box-header").nextAll().remove();
        $(".box-header").after("<div class='data-loading' ><div>");

        Global.post("/Activity/GetCustomersByActivityID", {
            pageSize: ObjectJS.Params.PageSize,
            pageIndex: ObjectJS.Params.PageIndex,
            activityID: $("#ActivityID").val()
        }, function (data) {
            $(".box-header").nextAll().remove();

            if (data.Items.length > 0) {
                doT.exec("template/activity/activity_customers.html", function (template) {
                    var innerhtml = template(data.Items);
                    innerhtml = $(innerhtml);

                    $(".box-header").after(innerhtml);
                });
            } else {
                $(".box-header").after("<div class='nodata-box' >暂无数据!<div>");
            }

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: _self.Params.PageIndex,
                display: 5,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    $(".tr-header").nextAll().remove();
                    _self.Params.PageIndex = page;
                    _self.getCustomersByActivityID();
                }
            });
        });
    }

    module.exports = ObjectJS;
});