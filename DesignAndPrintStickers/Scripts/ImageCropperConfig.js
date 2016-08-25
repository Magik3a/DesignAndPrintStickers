function getRoundedCanvas(sourceCanvas) {
    var canvas = document.createElement('canvas');
    var context = canvas.getContext('2d');
    var width = sourceCanvas.width;
    var height = sourceCanvas.height;
    canvas.width = width;
    canvas.height = height;
    context.beginPath();
    context.arc(width / 2, height / 2, Math.min(width, height) / 2, 0, 2 * Math.PI);
    context.strokeStyle = 'rgba(0,0,0,0)';
    context.stroke();
    context.clip();
    context.drawImage(sourceCanvas, 0, 0, width, height);
    return canvas;
};



var ImageCropper = {
    config: {
        cropX: 0,
        cropY: 0,
        cropHeight: 0,
        cropWidth: 0
    },
    init: function () {
        this.BindEvents();
    },
    BindEvents: function () {
        $('.btnCrop').on('click', function () {
            var objImage = $('#image').cropper('getCropBoxData');
            ImageCropper.Crop(objImage);
        });
        $('.btnCropAndAddToAll').on('click', function () {
            var objImage = $('#imageForAll').cropper('getCropBoxData');
            ImageCropper.CropForAll(objImage);
        });



    },
    InitCrop: function () {
        //initialize the cropper method
        $('#image').cropper({
            aspectRatio: ($(".active-template").attr("data-templatename") != "Credit card size (3.370x2.125'') ")?1:43/27,
            dragMode: 'move', //enabling dragging of image
            center: true,
            autoCrop: true, //use this to just zoom and pan image around
            autoCropArea: 0.8, //the size of the crop box
            zoomable: true,
            zoomOnWheel: true,
            crop: function (e) {
                // Output the result data for cropping image.
                ImageCropper.SetSizeandCoordinates(e);
            }
        });

    },

    InitCropForAll: function () {
        //initialize the cropper method
        $('#imageForAll').cropper({
            aspectRatio: ($(".active-template").attr("data-templatename") != "Credit card size (3.370x2.125'') ")?1:43/27,
            dragMode: 'move', //enabling dragging of image
            center: true,
            autoCrop: true, //use this to just zoom and pan image around
            autoCropArea: 0.8, //the size of the crop box
            zoomable: true,
            zoomOnWheel: true,
            crop: function (e) {
                // Output the result data for cropping image.
                ImageCropper.SetSizeandCoordinates(e);
            }
        });

    },

    SetSizeandCoordinates: function (e) {
        ImageCropper.config.cropX = e.x;
        ImageCropper.config.cropY = e.y;
        ImageCropper.config.cropHeight = e.height;
        ImageCropper.config.cropWidth = e.width;
    },
    Crop: function () {
        var param = {
            imagePath: $('#image').attr("src").split("?")[0],
            cropPointX: ImageCropper.config.cropX,
            cropPointY: ImageCropper.config.cropY,
            imageCropWidth: ImageCropper.config.cropWidth,
            imageCropHeight: ImageCropper.config.cropHeight,
            templateName: $(".active-template").attr("data-templatename")
        };
        ajx.invoke('/Home/CropImage', JSON.stringify(param), function (data) {
            //can refresh the image path in the original location
            //destroy the crop instance
            $('#image').cropper('destroy');
            //replace the original image with new image
            //timestamp is used to avoid caching
            //$('.active-box').attr("style","background-image: url("+ data.photoPath + "?t=" + new Date().getTime()+")");

            var img = $('#AddImageModalBody .active-box img').length;
            if (img) {
                $('#AddImageModalBody .active-box img').remove();
            }
            $('#AddImageModalBody .active-box').append("<img src='" + data.photoPath + "?t=" + new Date().getTime() + "' style='width: 100%; height: 100%; float: left;' />");
            $('#AddImageModalBody .active-box').find('.btn-remove-image').show();
            $('#AddImageModalBody .active-box').find('.btn-add-image').hide();

            $('#uploaderForOne').show();
            $('#image').attr("src", "");
            $('#image').hide();


            $('.add-picture-modal').modal('hide');
        });
    },
    CropForAll: function () {
        var param = {
            imagePath: $('#imageForAll').attr("src").split("?")[0],
            cropPointX: ImageCropper.config.cropX,
            cropPointY: ImageCropper.config.cropY,
            imageCropWidth: ImageCropper.config.cropWidth,
            imageCropHeight: ImageCropper.config.cropHeight,
            templateName: $(".active-template").attr("data-templatename")
        };


        ajx.invoke('/Home/CropImage', JSON.stringify(param), function (data) {
            //can refresh the image path in the original location
            //destroy the crop instance
            $('#imageForAll').cropper('destroy');
            //replace the original image with new image
            //timestamp is used to avoid caching
            //$('.active-box').attr("style","background-image: url("+ data.photoPath + "?t=" + new Date().getTime()+")");
            $("#AddImageModalBody .template-box").each(function (index, element) {
                var img = $(element).children("img").length;
                console.log(img > 0);
                if (img) {
                    $(element).children("img").remove();
                    console.log("have image");
                }

                $(element).append("<img src='" + data.photoPath + "?t=" + new Date().getTime() + "' style='width: 100%; height: 100%; float: left;' />");
                $(element).find('.btn-remove-image').show();
                  $(element).find('.btn-add-image').hide();   
                console.log("appended image");
            });


            $('#uploaderForAll').show();
            $('#imageForAll').attr("src", "");
            $('#imageForAll').hide();
            $('.add-picture-modal-toall').modal('toggle');
        });
    },

};