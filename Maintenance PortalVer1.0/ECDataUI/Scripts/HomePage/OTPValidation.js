

var ResendOTPCounter = 0;
var MaxValueForOTPAttempt = 3;

var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $(document).bind("contextmenu", function (e) {
        e.preventDefault();
    });
    $(document).keydown(function (e) {
        if (e.which === 123) {
            return false;
        }
    });



    $("#divForMessageResendOTPAttempt").hide();


    $('#divViewOTPValidationModal').on('keypress', function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            $("#btnValidateOTP").trigger('click');
        }
    });

    $(".AllowOnlyNumber").keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {

            return false;
        }
    });




    $("#btnResendOTP").click(function () {

        $.ajax({
            type: "GET",
            url: "/Home/InputOTPFromUser",
            success: function (data1) {

                if (data1.success) {
                    $.ajax({
                        url: '/Home/LoadModal',
                        type: "GET",
                        success: function (data3) {


                            $("#divOTPSentMsg").hide();
                            $('#OTPID').focus();
                            $('#OTPID').html('');
                            $("#messageforResend").show();
                            document.getElementById("IsOTPSent").outerHTML = "";
                            document.getElementById("OTPTypeId").outerHTML = "";
                            document.getElementById("EncryptedUId").outerHTML = "";
                            $('#OTPValidationFormId').append('<input type="hidden" id="IsOTPSent" name="IsOTPSent" value="' + data1.IsOTPSent + '"><input type="hidden" id="OTPTypeId" name="OTPTypeId" value="' + data1.OTPTypeId + '"><input type="hidden" id="EncryptedUId" name="EncryptedUId" value="' + data1.EncryptedUId + '">');
                            $('#MobileNumbertoDisplayID').html(data1.MobileNumberToDisplay);
                            $('#divOTPValidationModal').modal('show');
                        },
                        error: function (err) {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                                callback: function () {

                                }
                            });
                        }
                    });
                }
                else {
                    if (data.message == undefined) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while processing your request ." + '</span>',
                            callback: function () {

                                window.location.href = '/Error/SessionExpire';
                            }
                        });
                    }
                    else {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                            callback: function () {
                                $('#divOTPValidationModal').modal('hide');


                            }
                        });
                    }
                }

            },
            error: function (err) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                    callback: function () {

                    }
                });
            }
        });

    });


});


function OpenCaptchaForVerification(encryptedUId, IsOTPSent) {

     
    $.ajax({
        type: "GET",
        url: "/DataEntryCorrection/DataEntryCorrection/VerifyCaptchaAfterRegistrationForOTP",
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
        }
    });
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
