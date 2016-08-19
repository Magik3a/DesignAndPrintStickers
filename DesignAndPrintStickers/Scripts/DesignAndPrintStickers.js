
// Email Modal Sent Completed
function EmailSendComplete(data) {
    console.log(data);
    if (data.responseText === "true") {
        $("body").overhang({
            type: "success",
            message: "Woohoo! Thanks For your message!"
        });


        $("#modal-body-email form").hide(500);
          $("#modal-body-email .alert").delay(2000).show(500);
    }
    else {
         $("#wrapper").overhang({
            type: "warn",
            message: "Something is happening with the server, try again later!",
            duration: 2,
            upper: true
        });
        return false;
    }
}