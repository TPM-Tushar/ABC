
//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    $('#txtCurrentPassword').focus();


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#txtCurrentPassword').keypress(function (e) {
        var key = e.which;
        if (key == 13) // the enter key code
        {
            $("#btnCheckCurrentPassword").trigger('click');
        }
    });

    $('#btnGoToHome').click(function () {

        window.location.href = '/Home/HomePage';
    });

    $('#btnReset').click(function () {
        $('#txtNewPassword').val('');
        //$('#OTPID').val(''); 
        $('#txtConfirmPassword').val('');
        $('#txtCurrentPassword').val('');
    });

    $('#btnChangePassword').click(function () {

        if ($("#changePasswordFormID").valid()) {

            if ($('#txtNewPassword').val() != '' && $('#txtConfirmPassword') != '' && $('#txtCurrentPassword') != '') {
                if ($('#txtNewPassword').val() == $('#txtConfirmPassword').val()) {

                    //var EncryptedPassword = hex_sha512($('#txtCurrentPassword').val()); // to encrypt password.
                    //$('#txtCurrentPassword').val(EncryptedPassword);

                    //var EncryptedPassword = hex_sha512($('#txtNewPassword').val()); // to encrypt password.
                    //$('#txtNewPassword').val(EncryptedPassword);

                    //var EncryptedConfirmPassword = hex_sha512($('#txtConfirmPassword').val()); // to encrypt password.
                    //$('#txtConfirmPassword').val(EncryptedConfirmPassword);


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
                                        var singleEncryptedPWD = hex_sha512($('#txtNewPassword').val());
                                        var concatHash = singleEncryptedPWD + rstr2hex(data.SENCData).toUpperCase();
                                        var doubleEncryptedPWD = hex_sha512(concatHash);
                                        $('#txtNewPassword').val(doubleEncryptedPWD);

                                        var singleEncryptedPWD = hex_sha512($('#txtConfirmPassword').val());
                                        var concatHash = singleEncryptedPWD + rstr2hex(data.SENCData).toUpperCase();
                                        var doubleEncryptedPWD = hex_sha512(concatHash);
                                        $('#txtConfirmPassword').val(doubleEncryptedPWD);


                                        
                                        var singleEncryptedPWD = hex_sha512($('#txtCurrentPassword').val());
                                        var concatHash = singleEncryptedPWD + rstr2hex(data.SENCData).toUpperCase();
                                        var doubleEncryptedPWD = hex_sha512(concatHash);
                                        $('#txtCurrentPassword').val(doubleEncryptedPWD);


                                        $.ajax(
                                            {

                                                url: "/UserManagement/UserProfileDetails/SaveChangedPassword", // Controller/View
                                                type: "POST", //HTTP POST Method
                                                headers: header,
                                                data: $("#changePasswordFormID").serialize(),
                                                success: function (data) {

                                                    if (data.success) {

                                                        bootbox.alert({
                                                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

                                                            callback: function () {

                                                                $("#btnLoggOffID").trigger('click');
                                                                //  window.location.href = '/Login/UserLogin';
                                                                //window.location.href = '/Home/HomePage';

                                                            }
                                                        });

                                                    }
                                                    else {

                                                        bootbox.alert({

                                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                                            callback: function () {

                                                                $('#txtNewPassword').val('');
                                                                $('#txtConfirmPassword').val('');
                                                                $('#txtCurrentPassword').val('');

                                                            }
                                                        });

                                                    }
                                                },
                                                error: function (err) {
                                                    alert("Error in Processing" + err);
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
                else {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Passwords are not matching ' + '</span>')
                    $('#txtNewPassword').val('');
                    $('#txtConfirmPassword').val('');
                }

            }
            else {

                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Enter all required fields' + '</span>');
                $('#txtNewPassword').val('');
                $('#txtConfirmPassword').val('');
                $('#txtCurrentPassword').val('');

                //if ($('#txtConfirmPassword').val() == '' && $('#txtNewPassword').val() == '')
                //{
                //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Enter all required fields' + '</span>');
                //}


                //else if ($('#txtNewPassword').val() == '') {
                //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Enter New Password' + '</span>');
                //    $('#txtNewPassword').val('');
                //    // $('#OTPID').val(''); 
                //    $('#txtConfirmPassword').val('');

                //}
                //else if ($('#txtConfirmPassword').val() == '') {
                //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Enter Confirm Password' + '</span>');
                //    $('#txtNewPassword').val('');
                //    $('#txtConfirmPassword').val('');
                //}

            }

        }
    });







});






