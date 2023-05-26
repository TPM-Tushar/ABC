

/* trigger validate button click event on pressing ENTER -- shrinivas */
var ResendOTPCounter = 0;
var MaxValueForOTPAttempt = 3;

//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

     



    $("#divForMessageResendOTPAttempt").hide();


    $('#divViewOTPValidationModal').on('keypress', function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            $("#btnValidateOTP").trigger('click');
        }
    });

    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });

    /* trigger validate button click event on pressing ENTER -- shrinivas */

    $("#btnValidateOTP").click(function () {
        if ($("#OTPValidationFormId").valid()) {
            $('#divViewOTPValidationModal').modal('hide');
          
            if ($.trim($('#OTPID').val()) != '') {
                $.ajax({
                    url: "/Login/GetSessionSalt",
                    type: "GET",
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data.success) {
                            if (data.dataMessage != null) {
                                var salt = data.dataMessage;
                                var singleEncryptedOTP = hex_sha512($('#OTPID').val());
                                var concathash = singleEncryptedOTP + rstr2hex(salt).toUpperCase();
                                var doubleEncryptedOTP = hex_sha512(concathash);
                                $('#OTPID').val(doubleEncryptedOTP);

                                $.ajax({
                                    type: "POST",
                                    url: "/SendSMSDetails/ValidateOTP",
                                    headers: header,
                                    data: $("#OTPValidationFormId").serialize(),
                                    dataType: "json",
                                    success: function (data) {
                                        if (data.success) {
                                            bootbox.alert({
                                                message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.msg + '</span>',
                                                callback: function () {
                                                    if (data.redirectToLoginPage) {
                                                        window.location.href = data.url;
                                                    }
                                                    //Added by mayank to relaod and get user details on 16-02-2022
                                                    else {
                                                        window.location.reload();
                                                    }
                                                    //end of comment added by mayank
                                                }
                                            });
                                        }
                                        else {
                                            if (data.msg == undefined) {
                                                bootbox.alert({
                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while processing your request ." + '</span>',
                                                    callback: function () {

                                                        window.location.href = '/Error/SessionExpire';
                                                        //    $('#btnLoggOffID').trigger('click');
                                                        $.unblockUI();
                                                        //   return false;
                                                    }
                                                });
                                            }
                                            else {
                                                bootbox.alert({
                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.msg + '</span>',
                                                    callback: function () {

                                                        $('#divViewOTPValidationModal').modal('show');
                                                        $('#OTPID').val("");
                                                        $('#OTPID').focus();
                                                    }
                                                });
                                            }
                                        }
                                    },
                                    error: function (err) {
                                        bootbox.alert({
                                            //   size: 'small',
                                            //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                                            callback: function () {

                                            }
                                        });
                                        //bootbox.alert("Error occured while processing your request");
                                    }
                                });
                            }
                        }
                    },
                    error: function (err) {
                        bootbox.alert({
                            //   size: 'small',
                            //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                            callback: function () {

                            }
                        });
                        //bootbox.alert("Error occured while processing your request");
                    }
                });
            }
            // OTP validation logic -- Shrini
        }

    });




    
    $("#btnVerifyCaptcha").click(function () {
        BlockUI();
     
        $.ajax({
            type: "POST",
            url: "/Alerts/SendSMSDetails/VerifyCaptchaForOTP",
            headers: header,
            data: $("#OTPValidationFormId").serialize(),
            success: function (data) {
                if (data.success) {
                    $.ajax({
                        type: "GET",
                        url: "/Alerts/SendSMSDetails/InputOTPFromUser",
                        data: {
                            "IsOTPSent": data.IsOTPSent,
                            "OTPTypeId": data.OTPTypeId,
                            "EncryptedUId": data.EncryptedUId
                        },
                        success: function (data) {

//                            refreshCaptcha();


                            if (data.success) {


                               // $(".newCaptcha").trigger('click');
                                $('#divViewOTPValidationModal').modal({
                                    backdrop: 'static',
                                    keyboard: false
                                });
                               // $('#divViewOTPValidationModal').modal('show');

                                $('#divCaptcha').hide();
                                $("#divOTPInput").show();
                                //$('#divLoadOTPValidationView').html(data);
                                $('#OTPID').focus();
                                //$('#divViewOTPValidationModal').modal('show');
                                $("#divForMessageResend").show();
                                // Added by shubham bhagat on 20-04-2019 to show mobile number
                                //alert(data.MobileNumberToDisplay)
                                $('#MobileNumbertoDisplayID').html(data.MobileNumberToDisplay);

                                
                                $("#btnCloseOTPModal").on("click").click(function () {
                                    //alert("asd");
                                    $.unblockUI();
                                    $('#divViewOTPValidationModal').modal('hide');
                                });
                                $.unblockUI();

                            }
                            else {
                                if (data.message == undefined) {
                                    bootbox.alert({
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while processing your request ." + '</span>',
                                        callback: function () {

                                            window.location.href = '/Error/SessionExpire';
                                            //    $('#btnLoggOffID').trigger('click');
                                            $.unblockUI();
                                            //   return false;
                                        }
                                    });
                                }
                                else {
                                    bootbox.alert({
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                        callback: function () {
                                            $('#divViewOTPValidationModal').modal('hide');

                                            $.unblockUI();

                                        }
                                    });
                                }
                            }
                            $.unblockUI();

                        },
                        error: function (err) {
                            $.unblockUI();
                            bootbox.alert({
                                //   size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                                callback: function () {

                                }
                            });
                           // bootbox.alert("Error: " + err);
                        }
                    });

                }
                else
                {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        callback: function () {
                            refreshCaptcha();

                            $('#divViewOTPValidationModal').modal('show');

                            $("#divViewOTPValidationModal").css("display", "block")

                           // $.unblockUI();

                        }
                    });
                }

                
                $.unblockUI();

            },
            error: function (err) {
                $.unblockUI();

                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while proccessing your request " + '</span>',
                    callback: function () {

                    }
                });
            }

        });

    });

    $("#btnResendOTP").click(function () {
  
        refreshCaptcha();
        $('#divCaptcha').show();
        $("#divOTPInput").hide();
        $('#OTPID').focus();

    
        $.unblockUI();

    });


});


function OpenCaptchaForVerification(encryptedUId, IsOTPSent) {

     
    $.ajax({
        type: "GET",
        url: "/Alerts/SendSMSDetails/VerifyCaptchaAfterRegistrationForOTP",
        data: {
            "IsOTPSent": IsOTPSent,
            "EncryptedUId": encryptedUId,
            "OTPTypeId": 1
        },
        success: function (data) {

            $('#divViewOTPValidationModal').modal({
                backdrop: 'static',
                keyboard: false
            });
            $('#divLoadOTPValidationView').html(' ');

            $('#divLoadOTPValidationView').html(data);
            $('#divViewOTPValidationModal').modal('show');
            $('#divCaptcha').show();

            $("#divOTPInput").hide();
            $("#btnCloseOTPModal").off("click").click(function () {
                $('#divViewOTPValidationModal').modal('hide');
            });

            $.unblockUI();

        },
        error: function (err) {
            $.unblockUI();
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                callback: function () {

                }
            });
            //bootbox.alert("Error: " + err);
        }
    });
}


function refreshCaptcha() {
  //  alert("new refresh..");
    $('#Captcha').val('');
    $("#dvCaptcha input").val('');
    $(".newCaptcha").trigger('click');
    $("#mydiv input").val('');
   // $("#mydiv a").trigger('click');


}




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
