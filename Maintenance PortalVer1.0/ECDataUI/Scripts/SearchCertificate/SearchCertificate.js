

//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    $("#SearchCertResultPanel").hide();
   // $("#PdfDispPanel").hide();

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#txtCerificateNum').on("cut copy paste", function (e) {
        e.preventDefault();
    });

    $('#btnGotoLoginPage').click(function () {
        window.location.href = "/Login/UserLogin";
    })

    $("#btnSearch").click(function () {
        
        $.validator.unobtrusive.parse("#SearchCertificateForm");
        if ($("#SearchCertificateForm").valid()) {
           
            $.ajax({
                url: "/SearchCertificate/SearchCertificate",
                type: "POST",
                headers: header,
                data: $("#SearchCertificateForm").serialize(),
                
                success: function (data) {
                    if (data.responseMsg == null) {

                        $("#SearchCertResultPanel").show();
                        
                        $("#SearchCertResultPanel").html(data);
                        ////// call for diplaying pdf
                            //$.ajax({
                        //    url: "/SearchCertificate/DisplayCertificateView",
                            //    type: "GET",
                            //    data: $("#SearchCertificateForm").serialize(),


                            //    success: function (data) {
                            //        $("#PdfDispPanel").show();
                            //        $("#PdfDispPanel").html(data);
                            //    },
                            //    error: function () {

                            //        bootbox.alert(errorMessage);
                            //    }
                            //});
                        /////
                        refreshCaptcha();
                    }
                    else {
                        
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.responseMsg + '</span>',
                            callback: function () {
                            }
                        });

                        $("#SearchCertResultPanel").hide();
                        $("#PdfDispPanel").hide();
                        refreshCaptcha();
                    }
                },
                error: function () {
                    bootbox.alert("Error In Processing Request");
                    bootbox.alert(errorMessage);
                    refreshCaptcha();
                }
            });
        }
    });
    $("#btnReset").click(function () {
        refreshCaptcha();
        $('.text-danger').text(""); //to reset error messages
    });

});
function refreshCaptcha() {
    $('#Captcha').val('');
    $("#dvCaptcha input").val('');
    $(".newCaptcha").trigger('click');
}



