



//Global variables.
var token = '';
var header = {};

$(document).ready(function () {



    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    
    $("#FirstName").focus();


    $('#Captcha').attr("placeholder", "Captcha");
    $('#Captcha').attr('tabindex', 12);



    //added by akash on (16-05-2018) to change placeholder of IDProofNumber and set IdProofNumber to Uppercase in case Of PAN.
    $('#ddlIDProof').change(function () {
        // alert($('#ddlIDProof').val());
        $('#IDProofNumber').val('');
        $('#IDProofNumber').unbind();

        if ($('#ddlIDProof').val() == 1) {
            $("#IDProofNumber").attr("placeholder", "PAN Number"); //Uppercase in case of PAN.
            $('#IDProofNumber').keyup(function () {
                this.value = this.value.toUpperCase();
            });
        }
        else if ($('#ddlIDProof').val() == 2) {
            $("#IDProofNumber").attr("placeholder", "Passport Number");
        }
        else if ($('#ddlIDProof').val() == 3) {
            $("#IDProofNumber").attr("placeholder", "Driving License Number");
        }
        else if ($('#ddlIDProof').val() == 4) {
            $("#IDProofNumber").attr("placeholder", "Bank Passbook Number");
        }
        else if ($('#ddlIDProof').val() == 5) {
            $("#IDProofNumber").attr("placeholder", "School Leaving Certificate Number");
        }
        else if ($('#ddlIDProof').val() == 6) {
            $("#IDProofNumber").attr("placeholder", "Matriculation Certificate Number");
        }
        else if ($('#ddlIDProof').val() == 9) {
            $("#IDProofNumber").attr("placeholder", "Water Bill Number");
        }
        else if ($('#ddlIDProof').val() == 10) {
            $("#IDProofNumber").attr("placeholder", "Ration Card Number");
        }
        else if ($('#ddlIDProof').val() == 12) {
            $("#IDProofNumber").attr("placeholder", "Voting Card Number");
        }
        else if ($('#ddlIDProof').val() == 13) {
            $("#IDProofNumber").attr("placeholder", "Aadhar Card Number");
        }
        else {
            $("#IDProofNumber").attr("placeholder", "ID Proof Number");
        }
    });


    
    $('#btnGotoLoginPage').click(function () {
        window.location.href = "/Login/UserLogin";
    });


    // To check availability of "Firm Name".
    $("#txtUserName").focusout(function () {


        if (isEmailAddress($("#txtUserName").val())) {

            //    alert(isEmailAddress($("#txtUserName").val()));

            var userName = $("#txtUserName").val();
            if (userName.length != 0) {
                $.ajax({
                    type: "GET",
                    url: "/Account/CheckUserNameAvailability?userName=" + userName,

                    success: function (data) {

                        if (data.success) {
                            $("#InValidUserNameID").show();
                            $("#txtUserName").focus();
                        }
                        else {

                            ////   alert(data.errorMessage);
                            //   if (data.errorMessage.length != 0) { //To display error msg of Web Api.//added by akash 
                            //       alert("In....");
                            //       bootbox.alert(data.errorMessage);
                            //   }
                            $("#InValidUserNameID").hide();

                            // $("#ValidUserNameID").show();
                        }
                    },
                    error: function (xhr, status, err) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while proccessing your request" + '</span>',
                            callback: function () {

                            }
                        });
                    }
                });
            }
        }
        else {

            $("#InValidUserNameID").hide();

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
  


    $('#btnSubmit').click(function () {

        if ($("#RegistrationFormId").valid()) {

            //var EncryptedPassword = hex_sha512($('#txtPassword').val()); // to encrypt password.
            //$('#txtPassword').val(EncryptedPassword);

            //var EncryptedConfirmPassword = hex_sha512($('#txtConfirmPassword').val()); // to encrypt password.
            //$('#txtConfirmPassword').val(EncryptedConfirmPassword);

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
                              //  var salt = data.SENCData;//$('#SessionSalt').val();
                                var singleEncryptedPWD = hex_sha512($('#txtPassword').val());
                                var concatHash = singleEncryptedPWD + rstr2hex(data.SENCData).toUpperCase();
                                var doubleEncryptedPWD = hex_sha512(concatHash);
                                $('#txtPassword').val(doubleEncryptedPWD);

                                var singleEncryptedPWD = hex_sha512($('#txtConfirmPassword').val());
                                var concatHash = singleEncryptedPWD + rstr2hex(data.SENCData).toUpperCase();
                                var doubleEncryptedPWD = hex_sha512(concatHash);
                                $('#txtConfirmPassword').val(doubleEncryptedPWD);


                                $.ajax({
                                    url: "/Account/UserRegistration",
                                    type: "POST",
                                    headers: header,
                                    data: $("#RegistrationFormId").serialize(),
                                    dataType: "json",
                                    success: function (data) {


                                        refreshCaptcha();

                                        if (data.success) {
                                            bootbox.alert({
                                                message: data.responseMsg,
                                                callback: function () {

                                                   //window.location.href = "/Login/UserLogin";

                                                    $.ajax({
                                                        type: "GET",
                                                        url: "/Alerts/SendSMSDetails/VerifyCaptchaAfterRegistrationForOTP",
                                                        data: {
                                                            "IsOTPSent": 0,
                                                            "EncryptedUId": data.EncryptedUId,
                                                            "OTPTypeId": 1
                                                        },
                                                        contentType: "application/json; charset=utf-8",
                                                        datatype: "json",
                                                        success: function (data) {
                                                            // refreshOTPCaptcha();
                                                            $('#divViewOTPValidationModal').modal({
                                                                backdrop: 'static',
                                                                keyboard: false
                                                            });
                                                            $('#divLoadOTPValidationView').html(data);
                                                            // $('#OTPID').focus();
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

                                                            bootbox.alert("Error: " + err);
                                                        }
                                                    });


                                                  
                                                }
                                            });

                                            $('#RegistrationFormId')[0].reset();
                                            $("#btnReset").trigger('click');
                                            $.unblockUI();
                                        }
                                        else {

                                            if (data.errorMessage == undefined) {
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
                                                if (data.errorMessage.length != 0) {
                                                    bootbox.alert({
                                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                                                        callback: function () {
                                                            $('#txtPassword').val('');
                                                            $('#txtConfirmPassword').val('');
                                                            $('#txtCaptcha').val('');
                                                            refreshCaptcha();
                                                        }
                                                    });
                                                    $.unblockUI();
                                                }
                                                else {


                                                    bootbox.alert({
                                                        message: data.responseMsg
                                                    });


                                                    $('#txtPassword').val('');
                                                    $('#txtConfirmPassword').val('');
                                                    $('#txtCaptcha').val('');
                                                    //refreshCaptcha();
                                                }
                                            }


                                            $.unblockUI();
                                        }

                                    },
                                    error: function () {
                                        bootbox.alert("Error occured while processing your request.");
                                        $.unblockUI();

                                    }
                                });



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
    });



    $('#txtPAN').keyup(function () {
        this.value = this.value.toUpperCase();
    });

    $('.newCaptcha').click(function () {
        $('#Captcha').val('');
    });

    $("#btnReset").click(function () {
        refreshCaptcha();
        $('.text-danger').text(""); //to reset error messages
    });
});



function isEmailAddress(str) {
    var pattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    return pattern.test(str);  // returns a boolean 
}





function refreshCaptcha() {
  //  alert("In registration captcha")

    $('#Captcha').val('');
    $("#dvCaptcha input").val('');
    $(".newCaptcha").trigger('click');
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
