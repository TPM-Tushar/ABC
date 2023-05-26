//akash


//Global variables.
var token = '';
var header = {};

$(document).ready(function () {
    $("#txtEmail").focus();

    $('#Captcha').attr("placeholder", "Captcha");

  //  $('#Captcha').css("font-size", "18px");

    $('#Captcha').attr('tabindex', 3);


    $('#txtEmail').on("cut copy paste", function (e) {
        e.preventDefault();
    });

    $('#txtPassword').on("cut copy paste", function (e) {
        e.preventDefault();
    });


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;



    //$(window).keydown(function (event) {
    //    var hasFocus = $('#txtPassword').is(':focus');

    //    if (event.keyCode == 13) {
    //        if ($("#txtEmail").val() == "") {
    //            event.preventDefault();
    //            return false;
    //        }
    //        else {
    //            if (hasFocus) {
    //                $("#btnlogin").trigger("click");
    //            }
    //        }
    //    }
    //});



    $(window).keydown(function (event) {
        var hasFocus = $('#Captcha').is(':focus');

        if (event.keyCode == 13) {
            if ($("#txtEmail").val() == "") {
                event.preventDefault();
                return false;
            }
            else {
                if (hasFocus) {
                    $("#btnlogin").trigger("click");
                }
            }
        }
    });





    $('#btnlogin').click(function () {


        if ($("#LoginformId").valid()) {
            //$('#btnlogin').prop('disabled', true);

            //    BlockUI();
            if ($.trim($('#txtPassword').val()) != '') {
                BlockUI();
                $.ajax({
                    url: "/Login/GetSessionSalt",
                    type: "GET",
                    async: false,
                    cache: false,
                    success: function (data) {
                        //  console.log(data.success);
                        if (data.success == true) {

                            if (data.dataMessage != null) {
                                //$('#SessionSalt').val(data.dataMessage);

                                var salt = data.dataMessage;//$('#SessionSalt').val();
                                var singleEncryptedPWD = hex_sha512($('#txtPassword').val());
                                var concatHash = singleEncryptedPWD + rstr2hex(data.SENCData).toUpperCase();
                                var doubleEncryptedPWD = hex_sha512(concatHash);


                                var concatThird = doubleEncryptedPWD + rstr2hex(salt).toUpperCase();
                                var ThirdEncryptedPWD = hex_sha512(concatThird);
                                $('#txtPassword').val(ThirdEncryptedPWD);

                                //  $('#Captcha').val('');
                                //   BlockUI();

                                $.ajax({
                                    url: "/Login/UserLogin",
                                    type: "POST",
                                    headers: header,
                                    data: $("#LoginformId").serialize(),
                                    dataType: "json",
                                    success: function (data) {
                                        refreshCaptcha();
                                        if (data.success) {
                                            window.location.href = data.URL;
                                        }
                                        else {
                                            $("#txtPassword").val('');


                                            bootbox.alert({
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.responseMsg + '</span>',
                                                callback: function () {
                                                    $("#txtPassword").focus();
                                                }
                                            });

                                        }
                                        $.unblockUI();

                                    },
                                    error: function () {
                                        bootbox.alert({
                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while proccessing your request"+ '</span>',
                                            callback: function () {

                                            }
                                        });
                                        $.unblockUI();
                                    }
                                });

                            }

                        }
                        //   $.unblockUI();

                    },
                    error: function () {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while proccessing your request" + '</span>',
                            callback: function () {

                            }
                        });

                        $.unblockUI();

                    }
                });
            }
        }
    });



    $('.newCaptcha').click(function () {
        //alert($('#Captcha').val(''));
        $('#Captcha').val('');
        //RefreshCaptcha();
    });



    $("#btnReset").click(function () {
        refreshCaptcha();

        $("#LoginformId")[0].reset();
        $('.text-danger').text(" "); //to reset error messages
        $("#txtEmail").focus();
    });




    $('#CaptchaDiv').find('br').remove();



});

function refreshCaptcha() {
    $('#Captcha').val('');
    $("#dvCaptcha input").val('');
    $(".newCaptcha").trigger('click');
    //alert("Refresh captch");
}


function validate() {

    if ($.trim($('#txtPassword').val()) != '') {

        BlockUI();
        $.ajax({
            url: "/Login/GetSessionSalt",
            type: "GET",
            async: true,
            cache: false,
            success: function (data) {
                if (data.success == true) {

                    if (data.serverError == true && data.success == false) {
                        if (data.message != null || data.message != "") {
                            $.unblockUI();

                            bootbox.alert(data.message, function (confirmed) {
                                if (confirmed) {
                                    window.location.href = "/Error/Index";
                                }
                            });
                        }
                    }
                    else {
                        if (data.dataMessage != null) {
                            //$('#SessionSalt').val(data.dataMessage);
                            var salt = data.dataMessage;//$('#SessionSalt').val();
                            var singleEncryptedPWD = hex_sha512($('#txtPassword').val());
                            var concatHash = singleEncryptedPWD + rstr2hex(data.SENCData).toUpperCase();
                            var doubleEncryptedPWD = hex_sha512(concatHash);

                            alert(doubleEncryptedPWD);

                            var concatThird = doubleEncryptedPWD + rstr2hex(salt).toUpperCase();
                            var ThirdEncryptedPWD = hex_sha512(concatThird);


                            //$('#EncryptedPassword').val('');
                            //$('#EncryptedPassword').val(doubleEncryptedPWD);
                            $('#txtPassword').val(ThirdEncryptedPWD);
                            $('#frmLogin').submit();
                            $('#Captcha').val('');

                        }
                    }
                    $.unblockUI();

                }
            },
            error: function () {
                window.location.href = "/Error/Index";
                $.unblockUI();

            }
        });
    }

}//validate() ends here

function resetAll() {

    $("#Email").val('');
    $("#Password").val('');
}


//function RefreshCaptcha() {
//    alert("Hiiiii")
//        $.ajax({
//            url: "/Login/GetCaptchaImage",
//            type: "GET",
//            async: true,
//            cache: false,
//            success: function (data) {
//                //$('#CaptchaImage').src = '"' + data + '"';
//                $('#CaptchaImage').attr("src", data);
//            },
//            error: function () {
//                window.location.href = "/Login/Error";
//            }
//        });



//    }


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
