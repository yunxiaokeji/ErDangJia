
/*基础配置*/
seajs.config({
    base: "/modules/",
    paths: {
        "echarts": 'plug/echarts/',
        "zrender": 'plug/echarts/zrender/'
    },
    alias: {
        "jquery": "/Scripts/jquery-1.11.1.js",
        "form": "/Scripts/jquery.form.js",
        "parser": "/Scripts/jquery.parser.js",
        //颜色选择器
        "color": "plug/choosecolor/spectrum.js",
        //全局JS
        "global": "scripts/global.js",
        //HTML模板引擎
        "dot": "plug/doT.js",
        //分页控件
        "pager": "plug/datapager/paginate.js",
        //报表底层
        'zrender': 'plug/echarts/zrender/zrender.js',
        //日期控件
        'moment': 'plug/daterangepicker/moment.js',
        'daterangepicker': 'plug/daterangepicker/daterangepicker.js',
        //拖动排序
        "sortable": "plug/sortable.js"
    },
    map: [
        //可配置版本号
        ['.css', '.css?v=20160622'],
        ['.js', '.js?v=20160622']
    ]
});


seajs.config({
    alias: {
        //数据验证
        "verify": "plug/verify.js",
        //城市地区
        "city": "plug/city.js",
        //上传
        "upload": "plug/upload/upload.js",
        //开关插件
        "switch": "plug/switch/switch.js",
        //标签插件
        "mark": "plug/mark/mark.js",
        //标签插件(取配系统置)
        "colormark": "plug/colormark/colormark.js",
        //弹出层插件
        "easydialog": "plug/easydialog/easydialog.js",
        //导入弹出层插件
        "dialog": "plug/dialog/dialog.js",
        //搜索插件
        "search": "plug/seachkeys/seachkeys.js",
        //购物车
        "cart": "plug/shoppingcart/shoppingcart.js",
        //选择员工
        "chooseuser": "plug/chooseuser/chooseuser.js",
        //选择客户
        "choosecustomer": "plug/choosecustomer/choosecustomer.js",
        //选择产品
        "chooseproduct": "plug/chooseproduct/chooseproduct.js",
        //选择下属
        "choosebranch": "plug/choosebranch/choosebranch.js",
        //下拉框
        "dropdown": "plug/dropdown/dropdown.js",
        //显示用户名片层
        "businesscard": "plug/businesscard/businesscard.js",
        //分享明道
        "sharemingdao": "plug/sharemingdao/sharemingdao.js"
    }
});

