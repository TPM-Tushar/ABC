$(document).ready(function () {



    //$("#MobileNo").change(function () {
        // To check availability of "Mobile No".

    //});
    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });


    $('#ddlIDProof').change(function () {

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

    // commented by shubham bhagat on 18-04-2019 
    //$("#MobileNo").focusout(function () {


    //    if (($("#MobileNo").val()) != null) {

    //        //alert($("#MobileNo").val());

    //        var mobileNo = $("#MobileNo").val();
    //        if (mobileNo.length != 0) {
    //            $.ajax({
    //                type: "GET",
    //                url: "/UserManagement/UserProfileDetails/CheckMobileNoAvailability?mobileNo=" + mobileNo,

    //                success: function (data) {

    //                    if (data.success) {


    //                        $("#ErrorMessageID").html(data.message);

    //                        $("#MobileNo").focus();
    //                    }
    //                    else {
    //                        $("#ErrorMessageID").html("");

    //                        // $("#InValidUserNameID").hide();


    //                    }
    //                },
    //                error: function (xhr, status, err) {
    //                    bootbox.alert("Error " + err);
    //                }
    //            });
    //        }
    //    }
    //    else {
    //        $("#ErrorMessageID").html("");

    //        //$("#InValidUserNameID").hide();

    //    }
    //});


    $('#EditUserProfileDetails').click(function () {

        // commented by shubham bhagat on 18-04-2019
        //if (($("#Email").val()) != oldUserEmail) {
        //    bootbox.alert({
        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "User can not update registered email address." + '</span>',
        //        callback: function () {
        //            //    return ;
        //            window.location.href = "/UserManagement/UserProfileDetails/ViewUserProfileDetails";
        //        }
        //    });
        //}
        //else
        //{

        var NewMobileNo = $('#MobileNo').val();


        // alert("Old == " + vOldMobileNumber);
        //alert("New == " + NewMobileNo);

        var MobileVerificationStatus = 0;
        if (vOldMobileNumber != NewMobileNo) {
            MobileVerificationStatus = 1;
        }
        else {
            MobileVerificationStatus = 0;

        }
        // return false;
        $.validator.unobtrusive.parse('#EditFormId');


        if ($("#EditFormId").valid()) {
            $.ajax(
                {

                    url: "/UserManagement/UserProfileDetails/UpdateUserProfileDetails", // Controller/View
                    type: "POST", //HTTP POST Method
                    headers: header,
                    data: $("#EditFormId").serialize(),
                    success: function (data) {


                        if (data.success) {
                            bootbox.alert({
                                message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                callback: function () {

                                    if (MobileVerificationStatus == 1) {

                                        BlockUI();

                                        bootbox.confirm({
                                            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                                            message: '<span class="boot-alert-txt">Please verify your updated mobile number.<br />Do you want to verify it now?</span>',
                                            buttons: {
                                                cancel: {
                                                    label: 'No',
                                                    className: 'pull-right btn-default margin-left-NoBtn'
                                                },
                                                confirm: {
                                                    label: 'Yes'
                                                }
                                            },
                                            callback: function (result) {
                                                if (result) {
                                                    //$.ajax({
                                                    //    type: "GET",
                                                    //    url: "/Alerts/SendSMSDetails/InputOTPFromUser",
                                                    //    data: {
                                                    //        "IsOTPSent": 0,
                                                    //        "OTPTypeId": 1
                                                    //    },
                                                    //    success: function (data) {
                                                    //        if (typeof (data) == "string") {
                                                    //            $('#divViewOTPValidationModal').modal({
                                                    //                backdrop: 'static',
                                                    //                keyboard: false
                                                    //            });
                                                    //            $('#divLoadOTPValidationView').html(data);
                                                    //            $('#OTPID').focus();
                                                    //            $('#divViewOTPValidationModal').modal('show');
                                                    //            $("#btnCloseOTPModal").off("click").click(function () {
                                                    //                $('#divViewOTPValidationModal').modal('hide');
                                                    //            });
                                                    //        }
                                                    //        else {
                                                    //            bootbox.alert({
                                                    //                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                                    //                callback: function () { }
                                                    //            });
                                                    //        }
                                                    //        $.unblockUI();

                                                    //    },
                                                    //    error: function (err) {
                                                    //        $.unblockUI();

                                                    //        bootbox.alert("Error: " + err);
                                                    //    }
                                                    //});

                                                    $.ajax({
                                                        type: "GET",
                                                        url: "/Alerts/SendSMSDetails/VerifyCaptchaForOTP",
                                                        success: function (data) {

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
                                                    // Show OTP pop up -- shrinivas
                                                }
                                                $.unblockUI();

                                            }
                                        });
                                    }
                                    else {
                                        window.location.href = '/UserManagement/UserProfileDetails/ViewUserProfileDetails';

                                    }


                                }
                            });



                        }
                        else {

                            bootbox.alert(data.message);


                        }
                    },
                    error: function (err) {
                        alert("Error in Processing" + err);


                    }

                });
        }
        // commented by shubham bhagat on 18-04-2019
        //}



    });



});