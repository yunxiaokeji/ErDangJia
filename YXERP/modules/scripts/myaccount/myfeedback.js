

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Global = require("global"),
        Easydialog = require("easydialog"),
        doT = require("dot"),
        moment = require("moment");
    require("daterangepicker");

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
            FeedBack.Params.PageIndex = 1;
            FeedBack.Params.beginDate = start ? start.format("YYYY-MM-DD") : "";
            FeedBack.Params.endDate = end ? end.format("YYYY-MM-DD") : "";
            FeedBack.bindData();
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