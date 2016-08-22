/// <reference path="jquery-3.1.0.intellisense.js" />
$(".btn-choose-paper").click(function () {
    $(".btn-choose-paper").removeClass("active-paper");
    $(this).addClass("active-paper");

    console.log("choose paper click");
});



$(".template").click(function () {
    $(".template").removeClass("active-template");
    $(this).addClass("active-template");

    console.log("template click");
});

$("#btnAddImage").click(function () {
    var templateName = $(".active-template").attr("data-templatename");
    var paperSizeName = $(".active-paper").attr("data-papersizename");

    console.log("template click " + templateName);
    if (templateName == null) {
        $("#wrapper").overhang({
            type: "warn",
            message: "You have to choose template.",
            duration: 2,
            upper: true
        });
        return;
    }
    if (paperSizeName == null) {
        $("#wrapper").overhang({
            type: "warn",
            message: "You have to choose Paper Size.",
            duration: 2,
            upper: true
        });
        return;
    }
    $.get('Home/GetAddImagesModal', { TemplateName: templateName, PaperSize: paperSizeName }, function (data) {
        $("#AddImagesModal .modal-content").html("");

        $("#AddImagesModal .modal-content").append(data);
          $("#AddImagesModal").modal("show");
    });

});
function AddImageToPlaceHolder(elem) {

    if ($(elem).hasClass('active-box')) {
        return;
    };
    $('.btn-add-image').hide();

    $(".template-box").removeClass("active-box");
    $(elem).addClass("active-box");


    var attr = $(".active-box").find(".btn-add-image").attr('style');
    console.log(attr);

    if (attr == "display: none;") {
        $('.active-box .btn-add-image').show();
    } else {
        $('.active-box .btn-add-image').hide();
    }

    //$(".active-box .panel-body").html("<a href='#' class='btn btn-default btn-add-image'>Add image to this place </a> ")
};


function PrintInBrowser(elem) {
    $('.btn-add-image').hide();
    $(".template-box").removeClass("active-box");
    $("#page-for-printing").printElement({
        printMode: 'popup',
        importStyle: false,
        printContainer: false,
        debug: true
    });
};

var ajx = {
    config: {
        url: ''
    },
    invoke: function (method, data, successCB, failureCB) {
        successCB = successCB || "";
        failureCB = failureCB || "";

        $.ajax({
            type: "POST",
            url: this.config.url + method,
            data: data,
            async: true,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                successCB(response);
            },
            error: failureCB
        });
    }

};