define(function (require, exports, module) {
    var Global = require("global");

    var ObjectJS = {};

    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function () { 
        //头像设置
        var options =
	    {
	        thumbBox: '.thumbBox',
	        spinner: '.spinner',
	        imgSrc: ''
	    }

        var cropper = $('.imageBox').cropbox(options);

        $('#upload-file').on('change', function() {
            var reader = new FileReader();
            reader.onload = function(e) {
                options.imgSrc = e.target.result;
                cropper = $('.imageBox').cropbox(options);
            }
            reader.readAsDataURL(this.files[0]);
            this.files = [];
        });

        $('#btnZoomIn').on('click', function () {
            cropper.zoomIn();
        });

        $('#btnZoomOut').on('click', function() {
            cropper.zoomOut();
        });
        
        $('#btnCrop').on('click', function () { 
            if ($(".imageBox").css("background-image") == "none") {
                return;
            } else {
                var img = cropper.getDataURL();
                $('.cropped3').css('box-shadow', 'none').css('width', '500px').css('height', '160px').html('').show();
                $('.cropped3').append('<div class="left mLeft15"><img src="' + img + '" align="absmiddle" style="width:160px;margin-top:4px;border-radius:160px;box-shadow:0px 0px 12px #7E7E7E;"><p>160px*160px</p></div>');
                $('.cropped3').append('<div class="left mLeft15"><img src="' + img + '" align="absmiddle" style="width:128px;margin-top:4px;border-radius:128px;box-shadow:0px 0px 12px #7E7E7E;"><p>128px*128px</p></div>');
                $('.cropped3').append('<div class="left mLeft15"><img src="' + img + '" align="absmiddle" style="width:64px;margin-top:4px;border-radius:64px;box-shadow:0px 0px 12px #7E7E7E;" ><p>64px*64px</p></div><div class="clear"></div>');
                

                Global.post("/MyAccount/SaveAccountAvatar", { avatar: img }, function (data) {
                    if (data.Result == 1) {
                        $(".avatar", parent.document).attr("src", data.Avatar + "?t=" + new Date().toLocaleString());
                    }
                    else {
                        alert("保存失败");
                    }
                });
            }
        });
    }

    module.exports = ObjectJS;
});