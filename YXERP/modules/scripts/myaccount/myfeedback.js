

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot");

    var FeedBack = {};

    FeedBack.Params = {
        pageIndex: 1,
        type: -1,
        status: -1,
        beginDate: '',
        endDate: '',
        keyWords: '',
        id: ''
    };

    //列表初始化
    FeedBack.init = function () {
        FeedBack.bindEvent();
        FeedBack.bindData();
    };

    //绑定事件
    FeedBack.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                if (FeedBack.Params.keyWords != keyWords) {
                    FeedBack.Params.pageIndex = 1;
                    FeedBack.Params.keyWords = keyWords;
                    FeedBack.bindData();
                }
            });
        });

        //下拉状态、类型查询
       /* require.async("dropdown", function () {
            var Types = [
                {
                    ID: "1",
                    Name: "问题"
                },
                {
                    ID: "2",
                    Name: "建议"
                },
                {
                    ID: "3",
                    Name: "需求"
                }
            ];
            $("#FeedTypes").dropdown({
                prevText: "意见类型-",
                defaultText: "所有",
                defaultValue: "-1",
                data: Types,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    FeedBack.Params.pageIndex = 1;
                    FeedBack.Params.type = parseInt(data.value);
                    FeedBack.Params.beginDate = $("#BeginTime").val();
                    FeedBack.Params.endDate = $("#EndTime").val();
                    FeedBack.bindData();
                }
            });

            $(".search-tab li").click(function () {
                $(this).addClass("hover").siblings().removeClass("hover");
                var index = $(this).data("index");
                $(".content-body div[name='navContent']").hide().eq(parseInt(index)).show();
                FeedBack.Params.pageIndex = 1;
                FeedBack.Params.status = index == 0 ? -1 : index;
                FeedBack.Params.beginDate = $("#BeginTime").val();
                FeedBack.Params.endDate = $("#EndTime").val();
                FeedBack.bindData();
            });

        });
        */
        //时间段查询
        $("#SearchFeedBacks").click(function () {
            if ($("#BeginTime").val() != '' || $("#EndTime").val() != '') {
                FeedBack.Params.pageIndex = 1;
                FeedBack.Params.beginDate = $("#BeginTime").val();
                FeedBack.Params.endDate = $("#EndTime").val();
                FeedBack.bindData();
            }
        });

    };

    //绑定数据列表
    FeedBack.bindData = function () {
        $(".tr-header").nextAll().remove();
        Global.post("/MyAccount/GetFeedBacks", FeedBack.Params, function (data) {
            doT.exec("template/myaccount/myfeedback-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
                $(".a").bind("click", function () { FeedBack.getFeedBackDetail($(this).data("id"));  });
            });

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: FeedBack.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    FeedBack.Params.pageIndex = page;
                    FeedBack.bindData();
                }
            });

        });
    }

    
    //详情
    FeedBack.getFeedBackDetail = function (id) {
        Global.post("/MyAccount/GetFeedBackDetail", { id: id }, function (data) {
            $("#show-contact-detail").empty();
            doT.exec("template/myaccount/myfeedback-detail.html?3", function (templateFun) {
                var innerText = templateFun(data.Item);
                Easydialog.open({
                    container: {
                        id: "show-model-detail",
                        header: "反馈详情",
                        content: innerText,
                        yesFn: function () {
                        },
                        callback: function () {
                        }
                    }
                });
                $(".edit-company").hide();
            });
        });
    };
    module.exports = FeedBack;
});