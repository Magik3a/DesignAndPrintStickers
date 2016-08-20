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
    var templateName = $(".active-tempalte").attr("data-templatename");
    var paperSizeName = $(".active-paper").attr("data-papersizename");
    $.get('Home/GetAddImagesModal', { TemplateName: templateName,  PaperSize: paperSizeName}, function (data) {


       });;

   console.log("template click");
});