var token = '';
var header = {};
var detailsTable = null;

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    //$("#btnChangePassword").click(function () {
    //    if ($("#changePasswordFormID").valid()) {
    //        if ($('#txtNewPassword').val() != '' && $('#txtConfirmPassword') != '') {
    //            if ($('#txtNewPassword').val() == $('#txtConfirmPassword').val()) {
    //                $.ajax({
    //                    url: "/AnywhereEC/AnywhereEC/GetSessionSalt",
    //                    type: "GET",
    //                    async: true,
    //                    cache: false,
    //                    success: function (data) {
    //                        if (data.success == true) {
    //                            if (data.serverError == true && data.success == false) {
    //                                if (data.message != null || data.message != "") {
    //                                    $.unblockUI();

    //                                    bootbox.alert(data.message, function (confirmed) {
    //                                        if (confirmed) {
    //                                            window.location.href = "/Error/Index";
    //                                        }
    //                                    });
    //                                }
    //                            }
    //                            else {
    //                                if (data.dataMessage != null) {
    //                                    //$('#SessionSalt').val(data.dataMessage);
    //                                    //  var salt = data.SENCData;//$('#SessionSalt').val();
    //                                    var salt = data.dataMessage;
    //                                    var hash2 = hex_md5($("#txtNewPassword").val());
    //                                    //var concatHash2 = hash2 + rstr2hex(salt).toUpperCase();
    //                                    //var doublehash2 = hex_md5(concatHash2);

    //                                    var hash3 = hex_md5($("#txtConfirmPassword").val());
    //                                    //var concatHash3 = hash3 + rstr2hex(salt).toUpperCase();
    //                                    //var doublehash3 = hex_md5(concatHash3);

                                        
    //                                    $("#PasswordTxtId").val($("#txtNewPassword").val());
    //                                    $("#txtNewPassword").val(hash2);
    //                                    $("#txtConfirmPassword").val(hash3);


    //                                    $.ajax({
    //                                        url: "/AnywhereEC/AnywhereEC/ForgotPassword",
    //                                        type: "POST", //HTTP POST Method
    //                                        headers: header,
    //                                        data: $("#changePasswordFormID").serialize(),
    //                                        dataType: "json",
    //                                        success: function (data) {
    //                                            if (data.success == true) {
    //                                                bootbox.alert({
    //                                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
    //                                                    callback: function () {
    //                                                        window.location.href = '/Home/HomePage';
    //                                                    }
    //                                                });

    //                                            }
    //                                            else {
    //                                                bootbox.alert({
    //                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
    //                                                    callback: function () {
    //                                                        $("#txtNewPassword").val('');
    //                                                        $("#txtConfirmPassword").val('');
    //                                                    }
    //                                                });

    //                                            }
    //                                            unBlockUI();
    //                                        },
    //                                        error: function (err) {
    //                                            unBlockUI();
    //                                            alert("Error in Processing" + err);
    //                                        },
    //                                        beforeSend: function () {
    //                                            blockUI('loading data.. please wait...');

    //                                        }
    //                                    });

    //                                }
    //                            }
    //                            unBlockUI();
    //                        }
    //                    },
    //                    error: function () {
    //                        window.location.href = "/Error/Index";
    //                        unBlockUI();
    //                    },
    //                    beforeSend: function () {
    //                        blockUI('loading data.. please wait...');

    //                    }
    //                });
    //            }
    //        }
    //    }
    //});



    $("#btnChangePassword").click(function () {
        if ($("#changePasswordFormID").valid()) {
            if ($('#txtNewPassword').val() != '' && $('#txtConfirmPassword') != '') {
                if ($('#txtNewPassword').val() == $('#txtConfirmPassword').val()) {
                    var hash2 = hex_md5($("#txtNewPassword").val());

                    var hash3 = hex_md5($("#txtConfirmPassword").val());
                    
                    $("#PasswordTxtId").val($("#txtNewPassword").val());
                    $("#txtNewPassword").val(hash2);
                    $("#txtConfirmPassword").val(hash3);


                    $.ajax({
                        url: "/AnywhereEC/AnywhereEC/ForgotPassword",
                        type: "POST", //HTTP POST Method
                        headers: header,
                        data: $("#changePasswordFormID").serialize(),
                        dataType: "json",
                        success: function (data) {
                            if (data.success == true) {
                                bootbox.alert({
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function () {
                                        window.location.href = '/Home/HomePage';
                                    }
                                });

                            }
                            else {
                                bootbox.alert({
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function () {
                                        $("#txtNewPassword").val('');
                                        $("#txtConfirmPassword").val('');
                                    }
                                });

                            }
                            unBlockUI();
                        },
                        error: function (err) {
                            unBlockUI();
                            alert("Error in Processing" + err);
                        },
                        beforeSend: function () {
                            blockUI('loading data.. please wait...');

                        }
                    });
                }
            }
        }
    });


    $("#btnReset").click(function () {

        $("#txtNewPassword").val("");
        $("#txtConfirmPassword").val("");
    });

    
    //var myRedirect = function (redirectUrl, arg, value, arg2, value2) {
    //    var form = $('<form action="' + redirectUrl + '" method="post">' +
    //        '<input type="hidden" name="' + arg + '" value="' + value + '"></input>' + '<input type="hidden" name="' + arg2 + '" value="' + value2 + '"></input>' + '</form>');
    //    $('body').append(form);
    //    $(form).submit();
    //};

    //myRedirect('http://localhost:4128/', 'LoginName', 'dr.RJR', 'Password', 'Igrs@2020');
        //token = $('[name=__RequestVerificationToken]').val();
        //header["__RequestVerificationToken"] = token;

        //$.redirect("http://localhost:4128/",
        //    {
        //        LoginName: "dr.RJR",
        //        Password: "kaskflsaflsaf"
        //});

        //$.ajax({
        //    url: "http://localhost:4128/",
        //    data: { LoginName: 'dr.RJR', Password: 'jafjsaf' },
        //    cache: false,
        //    type: "POST",
        //    success: function (data) {
        //        console.log(data);
        //    },
        //    error: function (err) {
        //        bootbox.alert({
        //            //size: 'small',
        //            //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
        //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
        //        });

        //    }
        //})
});


//function to perform client side validation
function validate() {

    var flag = true;
    var password_check = /^.*(?=.{6,})(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$/;


    if ($("#txtNewPassword").val() == "") {
        alert("Please Enter New Password");
        $('#txtNewPassword').focus();
        return false;
    }
    else if ($("#txtConfirmPassword").val() == "") {
        alert("Please Enter Retype New Password");
        $('#txtConfirmPassword').focus();
        return false;
    }
    else if ($("#txtNewPassword").val() != $("#txtConfirmPassword").val()) {
        alert("New password and retype new password does not match");
        $('#txtNewPassword').focus();
        return false;
    }
    else if (!password_check.test($("#txtNewPassword").val())) {
        alert("Please choose strong password(one upper case letter, one lower case letter, one special symbol[e.g.@#$%^&+=] minimum length must be 6 characters and maximum length must be 32 characters)");
        $('#txtNewPassword').focus();
        return false;
    }
    else if ($("#txtNewPassword").val().trim() == $("#loginname").val().trim()) {
        alert("User name and password should not be same, Please Choose password different from user name");
        $('#txtNewPassword').focus();
        return false;
    }
    else if ($("#txtNewPassword").val().length > 32) {
        alert("new password must be up to 32 characters.");
        $('#txtNewPassword').focus();
        return false;
    }
    else if ($("#txtConfirmPassword").val().length > 32) {
        alert("Retype password must be up to 32 characters.");
        $('#txtConfirmPassword').focus();
        return false;
    }
    return flag;
}