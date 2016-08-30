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
    var borderRaiudsPercent = $(".active-template").attr("data-borderradius");


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
    var isLogget = false;
    //$.get("Home/CheckIfUserIsLogged", function (data) {
    //    console.log(data);
    //    isLogget = data;
    //});
    $.ajax({
        async: false,
        type: 'GET',
        url: 'Home/CheckIfUserIsLogged',
        success: function (data) {
              console.log(data);
        isLogget = data;
        }
    });
    console.log(isLogget);
    if (!isLogget) {
        $("#wrapper").overhang({
            type: "warn",
            message: "You have to be logged in.",
            duration: 2,
            upper: true
        });

        $("#loginModal").modal("show");
        return;
    }
    $('#styletorender').html("");
    $('#styletorender').append("<style>.cropper-view-box{border-radius:" + borderRaiudsPercent + "%;}</style>");
    $.get('Home/GetAddImagesModal', { TemplateName: templateName, PaperSize: paperSizeName }, function (data) {
        $("#AddImagesModal .modal-content").html("");

        $("#AddImagesModal .modal-content").append(data);
        $("#AddImagesModal").modal("show");
    });

});


function AddImageToPlaceHolder(elem) {
    if ($(elem).children("img").length > 0) {
        console.log("Have image");
        $('.btn-remove-image').hide();
        $(elem).find('.btn-remove-image').show();
        return;
    }
    else {
        $('.btn-remove-image').hide();
    }
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

function RemoveImageFromPlaceHolder(elem) {
    $(elem).parent().parent().find("img").remove();
    $(elem).hide();
    $(".btn-add-image").hide();
    $(elem).parent().find(".btn-add-image").show();
    console.log("removed");
};
function PrintInBrowser(elem) {
    $('.btn-add-image').hide();
    $(".template-box").removeClass("active-box");
    $("#page-for-printing").printElement({
        printMode: 'popup',
        importStyle: true,
        printContainer: false
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

function DownloadPdf() {
    var param = {
        html: $("#page-for-printing").html()
    };

    //$.post('/Home/DownloadStickers', { html: $("#page-for-printing").html() } , function (data) {
    //    console.log(data);
    //});
    var paperSizeName = $(".active-paper").attr("data-papersizename");
    var templateName = $(".active-template").attr("data-templatename");
    $.ajax({
        cache: false,
        url: '/Home/DownloadStickers',
        data: JSON.stringify({ html: $("#page-for-printing").html(), pagesize: paperSizeName, templateName: templateName }),
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if (!data) {
                $("#wrapper").overhang({
                    type: "error",
                    message: "Whoops! You probably miss to add some images!",
                    duration: 2,
                    upper: true,
                });
                return;
            }
            console.log(data.fileGuid);
            window.location = '/Home/Download?fileGuid=' + data.fileGuid;
        }
    })
};



function SendViaEmail(elem) {
    $(elem).attr("id", "send-message");
    $(elem).removeClass("btn-danger");
    $(elem).addClass("btn-warning");
    $("#email-user-pdf").show(500);

    $(elem).attr("onclick", "SendMessageWithAtachment(this)");
    $(elem).html("Press again to sent!")
};

function SendMessageWithAtachment(elem) {
    if (!ValidateUserEmail($('#input-user-email-pdf').val())) {
        return;
    }
    $(elem).removeClass("btn-warning");
    $(elem).addClass("btn-success btn-lg");
    $("#email-user-pdf").hide(500);
    console.log("161 line in clickevent < put some ajax here");

    $(elem).html("Message is sent successfully! <br /> Click to send another.")
    $(elem).attr("id", "send-pdf-button");
    $(elem).attr("onclick", "SendAnotherMessageWIthAtachment(this)");
    //   $(this).unbind("click").click();

    var paperSizeName = $(".active-paper").attr("data-papersizename");
    var templateName = $(".active-template").attr("data-templatename");
    $.ajax({
        cache: false,
        url: '/Home/SendToEmailWithAtachmnent',
        data: JSON.stringify({ html: $("#page-for-printing").html(), pagesize: paperSizeName, templateName: templateName, reciever: $("#input-user-email-pdf").val() }),
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if (!data) {
                $("#wrapper").overhang({
                    type: "error",
                    message: "Whoops! You probably miss to add some images!",
                    duration: 2,
                    upper: true,
                });
                return;
            }
            else {
                $("#wrapper").overhang({
                    type: "success",
                    message: "Woohoo! Message is sent successfully!",
                    duration: 2,
                    upper: true,
                });
                return;
            }
        }
    });
};

function ValidateUserEmail(email) {
    var testEmail = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!testEmail.test(email)) {
        $("body").overhang({
            type: "warn",
            message: "This email is not appropriate, can you check what is going on!",
            duration: 2,
            upper: true
        });
        return false;
    }
    return true;
};


function SendAnotherMessageWIthAtachment(elem) {
    $(elem).attr("id", "send-message");
    $(elem).removeClass("btn-danger");
    $(elem).addClass("btn-warning");
    $("#email-user-pdf").show(500);


    $(elem).attr("onclick", "SendMessageWithAtachment(this)");
    $(elem).html("Press again to sent!")
};

function HideModalLogin(data) {

    console.log(data);


    if (data === true) {
        location.reload();
    }
};