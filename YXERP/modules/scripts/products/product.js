
define(function (require, exports, module) {
    var Upload = require("upload"), ProductIco, ImgsIco,
        Global = require("global"),
        Verify = require("verify"), VerifyObject, DetailsVerify, editor,
        doT = require("dot"),
        Dialog = require("dialog"),
        Easydialog = require("easydialog");
    require("pager");
    require("switch");
    var $ = require('jquery');
    require("parser")($);
    require("form")($);
    var Params = {
        PageIndex: 1,
        keyWords: "",
        totalCount: 0,
        CategoryID: "",
        BeginPrice: "",
        EndPrice: "",
        OrderBy: "p.CreateTime desc",
        IsAsc: false
    };
    var CacheCategorys = [];
    var CacheChildCategorys = [];
    var Product = {};
    //添加页初始化
    Product.init = function (Editor) {
        var _self = this;
        editor = Editor;
        _self.bindEvent();
    }
    
    //绑定事件
    Product.bindEvent = function () {
        var _self = this;


        PosterIco = Upload.uploader({
            browse_button: 'productIco',
            file_path: "/Content/UploadFiles/Product/",
            picture_container: "orderImages", 
            multi_selection: false,
            maxSize: 1,
            successItems: '#productImg',
            fileType: 1,
            init: {}
        }); 
        $("#btnSaveProduct").on("click", function () {
            if (!VerifyObject.isPass()) {
                return;
            }
            Product.savaProduct();
        });

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        //编码是否重复
        $("#productCode").blur(function() {
            var _this = $(this);
            if (_this.val().trim() != "") {
                Global.post("/Products/IsExistsProductCode", {
                    code: _this.val(),
                    productid: ""
                }, function(data) {
                    if (data.Status) {
                        _this.val("");
                        alert("产品编码已存在,请重新输入");
                        _this.focus();
                    }
                });
            }
        });

        //条形码是否重复
        $("#shapeCode").blur(function () {
            var _this = $(this);
            if (_this.val().trim() != "") {
                Global.post("/Products/IsExistsShapeCode", {
                    code: _this.val(),
                    productid: ""
                }, function (data) {
                    if (data.status) {
                        _this.val("");
                        alert("条形码已存在,请重新输入");
                        _this.focus();
                    }
                });
            }
        });

        $("#productName").focus();

        //checkbox
        $(".checked").click(function () {
            var _this = $(this);
            if (_this.find(".checkbox").hasClass("hover")) {
                _this.find(".checkbox").removeClass("hover");
            } else {
                _this.find(".checkbox").addClass("hover");
            }
        });

        //更改价格同步子产品
        $("#price").change(function () {
            $(".child-product-table").find(".price,.bigprice").val($("#price").val());
        });

        //组合子产品
        $(".productsalesattr .attritem").click(function () {
            var _this = $(this), bl = false, details = [], isFirst = true;
            if (_this.find(".checkbox").hasClass("hover")) {
                _this.find(".checkbox").removeClass("hover");
            } else {
                _this.find(".checkbox").addClass("hover");
            }
            $(".productsalesattr").each(function () {
                bl = false;
                var _attr = $(this), attrdetail = details;
                //组合规格
                _attr.find(".checkbox.hover").each(function () {
                    bl = true;
                    var _value = $(this);
                    //首个规格
                    if (isFirst) {
                        var model = {};
                        model.ids = _attr.data("id") + ":" + _value.data("id");
                        model.saleAttr = _attr.data("id");
                        model.attrValue = _value.data("id");
                        model.names = "[" + _attr.data("text") + "：" + _value.data("text") + "]";
                        model.layer = 1;
                        model.guid = Global.guid();
                        details.push(model);
                    }else {
                        for (var i = 0, j = attrdetail.length; i < j; i++) {
                            if (attrdetail[i].ids.indexOf(_value.data("attrid")) < 0) {
                                var model = {};
                                model.ids = attrdetail[i].ids + "," + _attr.data("id") + ":" + _value.data("id");
                                model.saleAttr = attrdetail[i].saleAttr + "," + _attr.data("id");
                                model.attrValue = attrdetail[i].attrValue + "," + _value.data("id");
                                model.names = attrdetail[i].names + " [" + _attr.data("text") + "：" + _value.data("text") + "]";
                                model.layer = attrdetail[i].layer + 1;
                                model.guid = Global.guid();
                                details.push(model);
                            }
                        }
                    }
                });
                isFirst = false;
            });
            //选择所有属性
            if (bl) {
                var layer = $(".productsalesattr").length, items = [];
                for (var i = 0, j = details.length; i < j; i++) {
                    var model = details[i];
                    if (model.layer == layer) {
                        items.push(model);
                    }
                }
                $(".child-product-li").empty();
                //加载子产品
                doT.exec("template/products/product_child_add_list.html", function (templateFun) {
                    var innerText = templateFun(items);
                    innerText = $(innerText);
                    $(".child-product-li").append(innerText);

                    innerText.find(".upload-child-img").each(function () {
                        var _this = $(this);
                        Upload.createUpload({
                            element: "#" + _this.attr("id"),
                            buttonText: "选择图片",
                            className: "",
                            data: { folder: '', action: 'add', oldPath: "" },
                            success: function (data, status) {
                                if (data.Items.length > 0) {
                                    _this.siblings("img").attr("src", data.Items[0]).data("src", data.Items[0]);
                                } else {
                                    alert("只能上传jpg/png/gif类型的图片，且大小不能超过5M！");
                                }
                            }
                        });
                    })

                    innerText.find(".price,.bigprice").val($("#price").val());

                    //价格必须大于0的数字
                    innerText.find(".price,.bigprice").change(function () {
                        var _this = $(this);
                        if (!_this.val().isDouble() || _this.val() <= 0) {
                            _this.val($("#price").val());
                        }
                    });

                    //绑定启用插件
                    innerText.find(".ico-del").click(function () {
                        var _this = $(this);
                        confirm("确认删除此规格吗？", function () {
                            _this.parents("tr.list-item").remove();
                        });
                    });
                });
            }
        }); 
    }

    //保存产品
    Product.savaProduct = function () {
        var _self = this, attrlist = "", valuelist = "", attrvaluelist = "";
        var bl = true;
        $(".product-attr").each(function () {
            var _this = $(this);
            attrlist += _this.data("id") + ",";
            valuelist += _this.find("select").val() + ",";
            attrvaluelist += _this.data("id") + ":" + _this.find("select").val() + ",";
            if (!_this.find("select").val()) {
                bl = false;
            }
        });

        if (!bl) {
            alert("属性尚未设置值!");
            return false;
        }

        var Product = {
            ProductID: _self.ProductID,
            ProductCode: $("#productCode").val().trim(),
            ProductName: $("#productName").val().trim(),
            GeneralName: $("#generalName").val().trim(),
            IsCombineProduct: 0,
            ProviderID: $("#provider").val(),
            BrandID: $("#brand").val(),
            BigUnitID: $("#smallUnit").val().trim(),//$("#bigUnit").val().trim(),
            UnitID: $("#smallUnit").val().trim(),
            BigSmallMultiple: 1,//$("#bigSmallMultiple").val().trim(),
            CategoryID: $("#categoryID").val(),
            Status: $("#status").hasClass("hover") ? 1 : 0,
            AttrList: attrlist,
            ValueList: valuelist,
            AttrValueList: attrvaluelist,
            CommonPrice: $("#commonprice").val().trim(),
            Price: $("#price").val().trim(),
            Weight: $("#weight").val().trim(),
            WarnCount: $("#warnCount").val().trim(),
            IsNew: 0,//$("#isNew").prop("checked") ? 1 : 0,
            IsRecommend: 0,//$("#isRecommend").prop("checked") ? 1 : 0,
            IsAllow: $("#isAllow").hasClass("hover") ? 1 : 0,
            IsAutoSend: 0, //$("#isAutoSend").prop("checked") ? 1 : 0,
            EffectiveDays: $("#effectiveDays").val(),
            DiscountValue:1,
            ProductImage:$("#productImg").attr("src"),
            ShapeCode: $("#shapeCode").val().trim(),
            Description: encodeURI(editor.getContent())
        };

        //快捷添加子产品
        if (!_self.ProductID) {
            var details = [];
            $(".child-product-table .list-item").each(function () {
                var _this = $(this);
                var modelDetail = {
                    DetailsCode: _this.find(".code").val(),
                    ShapeCode: "",
                    ImgS: _this.find("img").data("src"),
                    SaleAttr: _this.data("attr"),
                    AttrValue: _this.data("value"),
                    SaleAttrValue: _this.data("attrvalue"),
                    Weight: 0,
                    Price: _this.find(".price").val(),
                    BigPrice: _this.find(".price").val(),//(Product.UnitID != Product.BigUnitID ? _this.find(".bigprice").val() : _this.find(".price").val()) * Product.BigSmallMultiple,
                    Remark: _this.data("desc"),
                    Description: ""
                };
                details.push(modelDetail);
            });
            Product.ProductDetails = details;
        }
        Global.post("/Products/SavaProduct", {
            product: JSON.stringify(Product)
        }, function (data) {
            if (data.result == 1) {
                if (!_self.ProductID) {
                    alert("产品保存成功", function () {
                        location.href = location.href;
                    });
                } else {
                    alert("保存成功", function () {
                        location.href = location.href;
                    });
                }
            } else if (data.result == 2) {
                alert("条形码已存在，保存失败");
            } else if (data.result == 3) {
                alert("产品编码已存在，保存失败");
            } else {
                alert("网络异常，操作失败");
            }
        });
    }

    //列表页初始化
    Product.initList = function () {
        var _self = this;
        _self.getChildCategory("");;
        _self.bindListEvent();
    }

    //获取分类信息和下级分类
    Product.getChildCategory = function (pid) {
        var _self = this;
        $("#category-child").empty();
        if (!CacheChildCategorys[pid]) {
            Global.post("/Products/GetChildCategorysByID", {
                categoryid: pid
            }, function (data) {
                CacheChildCategorys[pid] = data.items;
                _self.bindChildCagegory(pid);
            });
        } else {
            _self.bindChildCagegory(pid);
        }

        Params.CategoryID = pid;
        _self.getList();
    }

    //绑定下级分类
    Product.bindChildCagegory = function (pid) {
        var _self = this;
        var length = CacheChildCategorys[pid].length;
        if (length > 0) {
            for (var i = 0; i < length; i++) {
                var _ele = $(" <li data-id='" + CacheChildCategorys[pid][i].CategoryID + "'>" + CacheChildCategorys[pid][i].CategoryName + "</li>");
                _ele.click(function () {
                    //处理分类MAP
                    var _map = $(" <li data-id='" + $(this).data("id") + "'><a href='javascript:void(0);'>" + $(this).html() + "</a></li>");
                    _map.click(function () {
                        $(this).nextAll().remove();
                        _self.getChildCategory($(this).data("id"));
                    })
                    $(".category-map").append(_map);
                    _self.getChildCategory($(this).data("id"));
                });
                $("#category-child").append(_ele);
            }
        } else {
            $("#category-child").html("<li>无下级分类</li>");
        }
    }

    //绑定列表页事件
    Product.bindListEvent = function () {
        var _self = this;
        $(".category-map li").click(function () {
            $(this).nextAll().remove();
            _self.getChildCategory($(this).data("id"));
        });

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                Product.getList();
            });
        });

        //搜索价格区间
        $("#searchprice").click(function () {
            if (!!$("#beginprice").val() && !isNaN($("#beginprice").val())) {
                Params.BeginPrice = $("#beginprice").val();
                $("#attr-price .attrValues .price").removeClass("hover");
            } else if (!$("#beginprice").val()) {
                Params.BeginPrice = "";
            } else {
                $("#beginprice").val("");
            }

            if (!!$("#endprice").val() && !isNaN($("#endprice").val())) {
                Params.EndPrice = $("#endprice").val();
                $("#attr-price .attrValues .price").removeClass("hover");
            } else if (!$("#endprice").val()) {
                Params.EndPrice = "";
            } else {
                $("#endprice").val("");
            }

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

        $("#exportExcel").click(function () {
            _self.ShowExportExcel();
        });
        $('#exportProduct').click(function () {
            Dialog.exportModel("/Products/ExportFromProduct", { filter: JSON.stringify(Params), filleName: "产品导出" });
        });
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
    }

    //获取产品列表
    Product.getList = function () {
        var _self = this;
        $("#product-items").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='15'><div class='data-loading' ><div></td></tr>");

        Global.post("/Products/GetProductList", { filter: JSON.stringify(Params) }, function (data) {
            $("#product-items").nextAll().remove();

            if (data.Items.length > 0) {
                doT.exec("template/products/products.html", function (templateFun) {
                    var innerText = templateFun(data.Items);
                    innerText = $(innerText);
                    $("#product-items").after(innerText);

                    //绑定启用插件
                    innerText.find(".status").switch({
                        open_title: "点击上架",
                        close_title: "点击下架",
                        value_key: "value",
                        change: function (data, callback) {
                            _self.editStatus(data, data.data("id"), data.data("value"), callback);
                        }
                    });

                    //删除
                    innerText.find(".ico-del").click(function () {
                        var _this = $(this);
                        confirm("产品删除后不可恢复,确认删除吗？", function () {
                            Global.post("/Products/DeleteProduct", { productid: _this.data("id") }, function (data) {
                                if (data.result == 1) {
                                    alert("产品删除成功");
                                    _self.getList();
                                } else if (data.result == 10002) {
                                    alert("产品存在销售业务，删除失败");
                                } else {
                                    alert("删除失败！");
                                }
                            });
                        });
                    });
                   
                });
                $("#pager").paginate({
                    total_count: data.TotalCount,
                    count: data.PageCount,
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
            else
            {
                $(".tr-header").after("<tr><td colspan='15'><div class='nodata-txt' >暂无数据!</div></td></tr>");
            }
            
        });
    }

    //更改产品状态
    Product.editStatus = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/Products/UpdateProductStatus", {
            productid: id,
            status: status ? 0 : 1
        }, function (data) {
            !!callback && callback(data.Status);
        });
    }

    //更改产品是否新品
    Product.editIsNew = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/Products/UpdateProductIsNew", {
            productid: id,
            isnew: !status
        }, function (data) {
            !!callback && callback(data.Status);
        });
    }

    //更改产品是否推荐
    Product.editIsRecommend = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/Products/UpdateProductIsRecommend", {
            productid: id,
            isRecommend: !status
        }, function (data) {
            !!callback && callback(data.Status);
        });
    }

    //初始化编辑页数据
    Product.initEdit = function (productid,model, Editor) {
        var _self = this;
        editor = Editor;
        try
        {
            model = JSON.parse(model.replace(/&quot;/g, '"'));
            _self.bindDetailEvent(model);
            _self.bindDetail(model);
            _self.getChildList(model);
        } catch (err) {
            Global.post("/Products/GetProductByID", { productid: productid }, function(data) {
                var _model = data.Item;
                _self.bindDetailEvent(_model);
                _self.bindDetail(_model);
                _self.getChildList(_model);
            });
        }
    }

    //获取详细信息
    Product.bindDetail = function (model) {
        var _self = this;
        _self.ProductID = model.ProductID;
        $("#productName").val(model.ProductName);
        $("#productCode").val(model.ProductCode);
        $("#generalName").val(model.GeneralName);
        $("#shapeCode").val(model.ShapeCode);

        //截取绑定属性值
        var list = model.AttrValueList.split(',');
        for (var i = 0, j = list.length; i < j; i++) {
            $("#" + list[i].split(':')[0]).val(list[i].split(':')[1]);
        }
        model.ProviderID && $("#provider").val(model.ProviderID);
        model.BrandID && $("#brand").val(model.BrandID);
        $("#smallUnit").val(model.UnitID);
        $("#bigUnit").val(model.BigUnitID);
        $("#bigSmallMultiple").val(model.BigSmallMultiple);
        $("#commonprice").val(model.CommonPrice);
        $("#price").val(model.Price);
        $("#weight").val(model.Weight);
        $("#effectiveDays").val(model.EffectiveDays);
        $("#warnCount").val(model.WarnCount);

        model.Status != 1 || $("#status").addClass("hover");
        model.IsAllow != 1 || $("#isAllow").addClass("hover");
        //$("#isNew").prop("checked", model.IsNew == 1);
        //$("#isRecommend").prop("checked", model.IsRecommend == 1);
        //$("#isAutoSend").prop("checked", model.IsAutoSend == 1);
        $("#productImg").attr("src", model.ProductImage); 
        
        editor.ready(function () {
            editor.setContent(decodeURI(model.Description));
        });
    }

    //详情页事件
    Product.bindDetailEvent = function (model) {
        var _self = this;

        //保存产品信息
        $("#btnSaveProduct").on("click", function () {
            if (!VerifyObject.isPass()) {
                return;
            }
            Product.savaProduct();
        });

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
        //编辑图片
        PosterIco = Upload.uploader({
            browse_button: 'productIco',
            file_path: "/Content/UploadFiles/Product/",
            picture_container: "orderImages",
            //maxQuantity: 1,
            multi_selection: false,
            maxSize: 1,
            successItems: '#productImg',
            fileType: 1,
            init: {}
        }); 

        //编码是否重复
        $("#productCode").blur(function () {
            var _this = $(this);
            if (_this.val().trim() != "") {
                Global.post("/Products/IsExistsProductCode", {
                    code: _this.val(),
                    productid: model.ProductID
                }, function (data) {
                    if (data.Status) {
                        _this.val(model.ProductCode);
                        alert("产品编码已存在,请重新输入");
                    }
                });
            }
        });

        //条形码是否重复
        $("#shapeCode").blur(function () {
            var _this = $(this);
            if (_this.val().trim() != "") {
                Global.post("/Products/IsExistsShapeCode", {
                    code: _this.val(),
                    productid: model.ProductID
                }, function (data) {
                    if (data.status) {
                        _this.val(model.ShapeCode);
                        alert("条形码已存在,请重新输入");
                    }
                });
            }
        });

        //checkbox
        $(".checked").click(function () {
            var _this = $(this);
            if (_this.find(".checkbox").hasClass("hover")) {
                _this.find(".checkbox").removeClass("hover");
            } else {
                _this.find(".checkbox").addClass("hover");
            }
        });

        //切换内容
        $(".search-tab li").click(function () {
            var _this = $(this);
            _this.addClass("hover");
            _this.siblings().removeClass("hover");
            $("#productinfo").hide();
            $("#childproduct").hide();
            $("#" + _this.data("id") + "").removeClass("hide").show();
        });

        $("#addDetails").on("click", function () {
            $(".search-tab li").eq(0).removeClass("hover");
            $(".search-tab li").eq(1).addClass("hover");
            $("#productinfo").hide();
            $("#childproduct").removeClass("hide").show();
            _self.showTemplate(model, "");
        });
    }

    //子产品列表
    Product.getChildList = function (model) {
        var _self = this;
        $("#header-items").nextAll().remove();
        doT.exec("template/products/productdetails_list.html", function (templateFun) {
            var innerText = templateFun(model.ProductDetails);
            innerText = $(innerText);
            $("#header-items").after(innerText);

            //删除
            innerText.find(".ico-del").click(function () {
                var _this = $(this);
                confirm("子产品删除后不可恢复,确认删除吗？", function () {
                    Global.post("/Products/DeleteProductDetail", { productDetailID: _this.data("id") }, function (data) {
                        if (data.result == 1) {
                            alert("子产品删除成功");
                            _this.parent().parent().remove();
                        } else if (data.result == 10002) {
                            alert("子产品存在销售业务，删除失败");
                        } else {
                            alert("删除失败！");
                        }
                    });
                });
            });

            //绑定启用插件
            innerText.find(".status").switch({
                open_title: "点击启用",
                close_title: "点击禁用",
                value_key: "value",
                change: function (data, callback) {
                    _self.editDetailsStatus(data, data.data("id"), data.data("value"), callback);
                }
            });

            innerText.find(".ico-edit").click(function () {
                _self.showTemplate(model, $(this).data("id"));
            });
        });
    }

    //更改子产品状态
    Product.editDetailsStatus = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/Products/UpdateProductDetailsStatus", {
            productdetailid: id,
            status: status ? 0 : 1
        }, function (data) {
            !!callback && callback(data.Status);
        });
    }

    //添加/编辑子产品
    Product.showTemplate = function (model, id) {
        var _self = this, count = 1;
        doT.exec("template/products/productdetails_add.html", function (templateFun) {

            var html = templateFun(model.Category.SaleAttrs);

            Easydialog.open({
                container: {
                    id: "productdetails-add-div",
                    header: !id ? "添加产品规格" : "编辑产品规格",
                    content: html,
                    yesFn: function () {

                        if (!DetailsVerify.isPass()) {
                            return false;
                        }

                        var attrlist = "", valuelist = "", attrvaluelist = "", desc = $("#iptRemark").val().trim(), isNull = false;
                        $(".productattr").each(function () {
                            var _this = $(this);
                            attrlist += _this.data("id") + ",";
                            valuelist += _this.find("select").val() + ",";
                            attrvaluelist += _this.data("id") + ":" + _this.find("select").val() + ",";
                            //desc += "[" + _this.find(".attrname").html() + _this.find("select option:selected").text() + "]";
                            if (_this.find("select").val() == "|" && !_this.find("select").next().val()) {
                                isNull = true;
                                alert("请输入自定义规格", function () {
                                    _this.find("select").next().focus();
                                });
                                return false;
                            }
                        });
                        if (isNull) {
                            return false;
                        }
                        var Model = {
                            ProductDetailID: id,
                            ProductID: model.ProductID,
                            DetailsCode: $("#detailsCode").val().trim(),
                            ShapeCode: "",//$("#shapeCode").val().trim(),
                            UnitID: $("#unitid").val(),
                            SaleAttr: attrlist,
                            AttrValue: valuelist,
                            SaleAttrValue: attrvaluelist,
                            Price: $("#detailsPrice").val(),
                            BigPrice: $("#detailsPrice").val(),
                            Weight: 0,
                            ImgS: $("#imgS").attr("src"),
                            Remark: desc,
                            Description: ""
                        };

                        if (!desc) {
                            alert("规格不能为空！");
                            return false;
                        }

                        Global.post("/Products/SavaProductDetail", {
                            product: JSON.stringify(Model)
                        }, function (data) {
                            if (data.ID.length > 0) {
                                Easydialog.close();
                                Global.post("/Products/GetProductByID", {
                                    productid: model.ProductID,
                                }, function (data) {
                                    _self.getChildList(data.Item);
                                });
                            } else if (data.result == 2) {
                                alert("此规格已存在");
                            } else if (data.result == 3) {
                                alert("规格产品编码已存在");
                            } 
                        });
                        return false;
                    },
                    callback: function () {

                    }
                }
            });

            //设置有规格不能自动输入
            if ($(".productattr").length > 0) {
                $("#iptRemark").prop("disabled", "disabled").css({ "border": "none", "background-color": "#fff" });
            }

            //绑定单位
            $("#unitName").text(model.SmallUnit.UnitName);

            if (!id) {
                $("#detailsPrice").val(model.Price);
                var _desc = "";
                $(".productattr").each(function () {
                    var _this = $(this);
                    _desc += "[" + _this.find(".attrname").html() + _this.find("select option:selected").text() + "]";
                });
                $("#iptRemark").val(_desc);

            } else {
                var detailsModel;
                for (var i = 0, j = model.ProductDetails.length; i < j; i++) {
                    if (id == model.ProductDetails[i].ProductDetailID) {
                        detailsModel = model.ProductDetails[i];
                    }
                }
                $("#detailsPrice").val(detailsModel.Price);
                $("#detailsCode").val(detailsModel.DetailsCode);
                //_self.ImgS = detailsModel.ImgS;
                $("#imgS").attr("src", detailsModel.ImgS || "/modules/images/default.png");

                var list = detailsModel.SaleAttrValue.split(',');
                $(".productattr").each(function () {
                    var _this = $(this), bl = false;
                    for (var i = 0, j = list.length; i < j; i++) {
                        if (_this.find("select").attr("id") == list[i].split(':')[0] && list[i].split(':')[1] && list[i].split(':')[1] != "|") {
                            $("#" + list[i].split(':')[0]).val(list[i].split(':')[1]).prop("disabled", true);
                            bl = true;
                        }
                    }
                    if (!bl) {
                        _this.find("select").val("|");
                        _this.find("select").next().show();
                        var star = detailsModel.Remark.indexOf(_this.find(".attrname").html()) + _this.find(".attrname").html().length;
                        var len = 0;
                        
                        if (_this.next().hasClass("productattr")) {
                            len = detailsModel.Remark.indexOf(_this.next().find(".attrname").html()) - star - 2;
                        } else {
                            len = detailsModel.Remark.length - star - 1;
                        }
                        _this.find("select").next().val(detailsModel.Remark.substr(star, len));
                    }
                    
                });
                $("#iptRemark").val(detailsModel.Remark);
            }

            //选择规格
            $(".productattr select").change(function () {
                
                if ($(this).val() != "|") {
                    $(this).next().hide();
                } else {
                    $(this).next().show();
                }
                var _desc = "";
                $(".productattr").each(function () {
                    var _this = $(this);
                    if (_this.find("select").val() != "|") {
                        _desc += "[" + _this.find(".attrname").html() + _this.find("select option:selected").text() + "]";
                    } else {
                        _desc += "[" + _this.find(".attrname").html() + _this.find("select").next().val().trim() + "]";
                    }
                });
                $("#iptRemark").val(_desc);
            });

            //自定义文本
            $(".customize").keyup(function () {
                var _desc = "";
                $(".productattr").each(function () {
                    var _this = $(this);
                    if (_this.find("select").val() != "|") {
                        _desc += "[" + _this.find(".attrname").html() + _this.find("select option:selected").text() + "]";
                    } else {
                        _desc += "[" + _this.find(".attrname").html() + _this.find("select").next().val().trim() + "]";
                    }
                });
                $("#iptRemark").val(_desc);
            });
            ImgsIco = Upload.uploader({
                browse_button: 'imgSIco',
                file_path: "/Content/UploadFiles/PdtDetail/",
                picture_container: "orderImages", 
                multi_selection: false,
                maxSize: 1,
                successItems: '#imgS',
                fileType: 1,
                init: {}
            }); 

            DetailsVerify = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }
      
    Product.ShowExportExcel = function () {
        $('#show-product-export').empty();
        var guid = Global.guid() + "_";
        Dialog.open({
            container: {
                id: "show-product-export",
                header: "导入产品信息",
                importUrl: '/Products/ProductImport',
                yesFn: function () {
                    $('#upfileForm').form('submit', {
                        onSubmit: function () {
                            Dialog.setOverlay(guid, true);
                        },
                        success: function (data) {
                            Dialog.setOverlay(guid, false);
                            if (data == "操作成功") {
                                Dialog.close(guid);
                            }
                            alert(data);
                        }
                    });
                },
                docWidth: 450,
                exportUrl: '/Products/ExportFromProduct',
                exportParam: { test: true, model: 'Item' },
                herf: '/Products/ProductImport',
                noFn: true,
                yesText: '导入',
                callback: function () {

                }
            },
            guid: guid
        });

    }
    module.exports = Product;
})