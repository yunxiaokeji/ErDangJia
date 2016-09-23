define(function (require, exports, module) {
    var Global = require("global"),
        Dialog = require("dialog"),
        moment = require("moment");
    require("daterangepicker");
    require("pager");
    var Params = {
        pageIndex: 1,
        pageSize: 20, 
        customerid: "",
        keyWords: "",
        begintime: new Date().setMonth(new Date().getMonth() - 1).toString().toDate("yyyy-MM-dd"),
        endtime: Date.now().toString().toDate("yyyy-MM-dd"),
        orderBy: " SUM(b.Quantity-b.ReturnQuantity) desc "
    };

    var ObjectJS = {};

    ObjectJS.init = function (types) {
        var _self = this; 
        _self.bindEvent();
        _self.getOrderDetailRPT();
    }
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
            Params.begintime = start ? start.format("YYYY-MM-DD") : "";
            Params.endtime = end ? end.format("YYYY-MM-DD") : "";
            _self.getOrderDetailRPT();
        });
        $("#iptCreateTime").val(Params.begintime + ' 至 ' + Params.endtime);

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                if (Params.keyWords != keyWords) {
                    Params.pageIndex = 1;
                    Params.keyWords = keyWords;
                    _self.getOrderDetailRPT();
                }
            });
        });

        $(".td-span").click(function () {
            var _this = $(this); 
            if (_this.hasClass("hover")) { 
                if (_this.find(".asc").hasClass("hover")) {
                    $(".td-span").find(".asc").removeClass("hover");
                    $(".td-span").find(".desc").removeClass("hover");
                    _this.find(".desc").addClass("hover");
                    Params.orderBy = _this.data("column") + " desc ";
                } else {
                    $(".td-span").find(".desc").removeClass("hover");
                    $(".td-span").find(".asc").removeClass("hover");
                    _this.find(".asc").addClass("hover");
                    Params.orderBy = _this.data("column") + " asc ";
                }
            } else {
                $(".td-span").removeClass("hover");
                $(".td-span").find(".desc").removeClass("hover");
                $(".td-span").find(".asc").removeClass("hover");
                _this.addClass("hover");
                _this.find(".desc").addClass("hover");
                Params.orderBy = _this.data("column") + " desc ";
            }
            Params.pageIndex = 1;
            _self.getOrderDetailRPT();
        });



        $('#exportSaleRPT').click(function () {
            Params.filleName = '库存明细报表';
            Params.doctype = 1;
            Dialog.exportModel("/StockRPT/ExportStockDetailRPT", Params);
        }); 
    }
     
    ObjectJS.getOrderDetailRPT = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after('<tr class="item-tr"><td class="center" colspan="8"><div class="center data-loading"><div></td></tr>');
        Global.post("/StockRPT/GetStockDetailRPT", Params, function (data) {
            $(".tr-header").nextAll().remove();
            var innerhtml = '';
            if (data.items.length > 0) {
                for (var j = 0; j < data.items.length; j++) {
                    var item = data.items[j];
                    innerhtml += '<tr class="item-tr"><td class="tLeft">' + item.ProductCode + '</td>' +
                        '<td class="tLeft">' + item.ProductName + '</td>' +
                        '<td class="tLeft">' + item.Remark + '</td>' +
                        '<td class="tLeft">' + item.UnitName + '</td>' +
                        '<td class="center hide">' + item.QCQuantity + '</td>' +
                        '<td class="center ">' + item.InQuantity + '</td>' +
                        '<td class="center">' + item.OutQuantity + '</td>' + 
                        '<td class="center">' + (parseInt(item.StockIn) - parseInt(item.StockOut)) + '</td>' +
                        '</tr>';
                }
            } else {
                innerhtml = '<tr class="item-tr"><td class="center" colspan="8"><h2>暂无数据</h2></td></tr>';
            }
            $(".tr-header").after(innerhtml);

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Params.pageIndex,
                display: 5,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    $(".tr-header").nextAll().remove();
                    Params.pageIndex = page;
                    _self.getOrderDetailRPT();
                }
            });
        });
    } 
    module.exports = ObjectJS;
});