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