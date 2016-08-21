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