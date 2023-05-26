


//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    $("#EmailID").focus();
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $('#Captcha').attr("placeholder", "Captcha");
    $('#Captcha').attr('tabindex', 3);


    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });



    $('#btnSubmit').click(function () {
        if ($("#ForgotPasswordFormId").valid()) {

            BlockUI();
            $.ajax({
                url: "/Account/ForgotPassword",
                cache: false,
                headers: header,
                type: "POST",
                data: $("#ForgotPasswordFormId").serialize(),
                dataType: "json",

                success: function (data) {
                    refreshCaptcha();
                    if (data.success) {

                        bootbox.alert({
                            message: data.responseMsg
                        });
                        $.unblockUI();

                    }
                    else {
                        bootbox.alert({
                            message: data.responseMsg
                        });
                        $("#ForgotPasswordFormId input").val('');

                        $.unblockUI();

                    }

                },
                error: function () {
                    console.log('Errror............!!!');
                    $.unblockUI();

                }
            });
        }
    });

    $('.newCaptcha').click(function () {
        $('#Captcha').val('');
    });


    $('#btnGotoLoginPage').click(function () {
        window.location.href = "/Login/UserLogin";
    });


    $("#btnReset").click(function () {
        refreshCaptcha();
        $('.text-danger').text(""); //to reset error messages
    });
});

function refreshCaptcha() {
    $('#Captcha').val('');
    $("#dvCaptcha input").val('');
    $("#dvCaptcha a").trigger('click');
}

//Block UI
function BlockUI() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}