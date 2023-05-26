

$(document).ready(function () {
    $('#btnSubmit').click(function () {
        if ($("#ForgotPasswordByLinkFormId").valid()) {


            //var EncryptedPassword = hex_sha512($('#txtPassword').val()); // to encrypt password.
            //$('#txtPassword').val(EncryptedPassword);

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
                                var singleEncryptedPWD = hex_sha512($('#txtPassword').val());
                                var concatHash = singleEncryptedPWD + rstr2hex(data.SENCData).toUpperCase();
                                var doubleEncryptedPWD = hex_sha512(concatHash);
                                $('#txtPassword').val(doubleEncryptedPWD);

                                var singleEncryptedPWD = hex_sha512($('#txtConfirmPassword').val());
                                var concatHash = singleEncryptedPWD + rstr2hex(data.SENCData).toUpperCase();
                                var doubleEncryptedPWD = hex_sha512(concatHash);
                                $('#txtConfirmPassword').val(doubleEncryptedPWD);



                                BlockUI();
                                $.ajax({
                                    url: "/Account/ForgotPasswordByLink",
                                    type: "POST",
                                    data: $("#ForgotPasswordByLinkFormId").serialize(),
                                    //  dataType: "json",
                                    success: function (data) {

                                        $("#responseDivID").html("");
                                        $("#responseDivID").html(data);
                                        $.unblockUI();

                                        refreshCaptcha();
                                        if (data.success) {
                                            bootbox.alert({
                                                message: data.responseMsg,
                                            });
                                            $.unblockUI();

                                        }
                                        else {
                                            bootbox.alert({
                                                message: data.errorMsg,
                                            });
                                            $.unblockUI();

                                        }
                                        //   $.unblockUI();


                                    },
                                    error: function () {
                                        console.log('Errror............!!!');
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


    function refreshCaptcha() {

        $("#dvCaptcha input").val('');
        $("#dvCaptcha a").trigger('click');
    }



    $("#btnReset").click(function () {
        clearAll();
        refreshCaptcha();
        $('.text-danger').text(""); //to reset error messages
    });
});

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