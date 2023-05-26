
// Added By SHubham Bhagat on 15-12-2018
//Global variables.
var token = '';
var header = {};

//alert("create office user details");
$(document).ready(function () {
    if (vIsForUpdate == 0)
    {        
        $('#OfficeNamesDropDownId').prop('disabled', true);
        $('#RoleDropDownListId').prop('disabled', true);
    }
    // Added By SHubham Bhagat on 15-12-2018
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    // on 11 - 4 - 2019 Commented by SB  due to requirement change 
    //// Added By SHubham Bhagat on 15-12-2018
    //$('#Office_ShortName_DivId').hide();
    //$('#Office_OfficeType_DivID').hide();
    //$('#Office_District_DivID').hide();

    if (vIsForUpdate == 1) {
       // alert("asd");
        $("#txtUserName").prop("readonly", true);
    }
    
    $("#CloseOfficeUserForm").click(function () {
        window.location.href = "/UserManagement/OfficeUserDetails/ViewOfficeUserDetails";
    });

    $("#btnCancel").click(function () {
        //debugger;
        window.location.href = "/UserManagement/OfficeUserDetails/ViewOfficeUserDetails";
    });
      
    $("#btnUpdate").click(function () {
        var form = $("#RegistrationOfficeUserFormId");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        if ($("#RegistrationOfficeUserFormId").valid()) {
            if ($('#OfficeNamesDropDownId').val() == "0") {
                bootbox.alert('Please Select Office');
            }
            else if ($('#LevelDropDownListId').val() == "0")
            {
                bootbox.alert('Please Select Level');
            }
            else if ($('#RoleDropDownListId').val() == "0")
            {
                bootbox.alert('Please Select Role');
            }
            else {
                bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'>Are you sure you want to update?</span>",
                    buttons: {
                        cancel: {
                            label: '<i class="fa fa-times"></i> No',
                            className: 'pull-right margin-left-NoBtn'
                        },
                        confirm: {
                            label: '<i class="fa fa-check"></i> Yes'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            //if ($('#OfficeNamesDropDownId').val() == "0") {
                            //    //alert('in if');
                            //    bootbox.alert('Please Select Office');
                            //}
                            //else {
                            BlockUI();
                            $.ajax({

                                url: "/UserManagement/OfficeUserDetails/UpdateOfficeUser",
                                type: "POST",
                                data: $("#RegistrationOfficeUserFormId").serialize(),
                                headers: header,
                                success: function (data) {
                                    if (data.success) {
                                        bootbox.alert({
                                            //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                            callback: function () {
                                                GetJsonData();
                                                OpenOfficeList();
                                                $("#DivOfficeUserDetailsWrapper").fadeOut(500);
                                                $("#panelOfficeUser").fadeIn();
                                                document.documentElement.scrollTop = 0;
                                            }
                                        });
                                        $.unblockUI();
                                    }
                                    else {

                                        if (data.message == undefined) {
                                            bootbox.alert({
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while processing your request ." + '</span>',
                                                callback: function () {
                                                    //window.location.href = '/Error/SessionExpire';
                                                    $.unblockUI();
                                                }
                                            });
                                        }
                                        else {
                                            bootbox.alert({
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                            });
                                            $.unblockUI();
                                        }
                                    }
                                },
                                error: function () {
                                    bootbox.alert({
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Error in processing Office User details...</span>',
                                    });
                                    $.unblockUI();
                                }
                            });
                            //}
                            //else { $.unblockUI(); return; }
                            //}
                        }
                    }
                });
            }

           
        }
        else { $.unblockUI(); return; }
    });

    //amit user registration


    $("#FirstName").focus();
    //change placeholder of IDProofNumber and set IdProofNumber to Uppercase in case Of PAN.
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

    // To check availability of "User Name".
    $("#txtUserName").focusout(function () {
        //alert('1');
        // commented by shubham bhagat on 18-04-2019
        //if (isEmailAddress($("#txtUserName").val()))
        //{

            //    alert(isEmailAddress($("#txtUserName").val()));
            // Added by shubham bhagat on 20-04-2019 
        
            if (vIsForUpdate == 0) {
            
                var userName = $("#txtUserName").val();
                if (userName.length != 0) {

                    $.ajax({
                        type: "GET",
                        url: "/UserManagement/OfficeUserDetails/CheckUserNameAvailability?userName=" + userName,

                        success: function (data) {

                            if (data.success) {
                                $("#InValidUserNameID").show();
                                $("#txtUserName").focus();
                            }
                            else {
                                $("#InValidUserNameID").hide();
                            }
                        },
                        error: function (xhr, status, err) {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Error in processing Office User details...</span>',
                            });
                        }
                    });
                }
            }
         // commented by shubham bhagat on 18-04-2019
        //}
        //else
        //{
        //    $("#InValidUserNameID").hide();
        //}
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


    $('#btnCreate').click(function () {
        //if ($('#OfficeNamesDropDownId').val() == "0") {
        //    //alert('in if');
        //    bootbox.alert('Please Select Office');
        //}
        //else {
            //alert('in else');
            var form = $("#RegistrationOfficeUserFormId");
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);

            if ($("#RegistrationOfficeUserFormId").valid()) {

           

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
                                        url: "/UserManagement/OfficeUserDetails/OfficeUserRegistration",
                                        type: "POST",
                                        data: $("#RegistrationOfficeUserFormId").serialize(),
                                        dataType: "json",
                                        headers: header,
                                        success: function (data) {
                                            if (data.success) {
                                                bootbox.alert({
                                                    //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

                                                    callback: function () {
                                                        GetJsonData();
                                                        OpenOfficeList();
                                                        $("#DivOfficeUserDetailsWrapper").fadeOut(500);
                                                        $("#panelOfficeUser").fadeIn();
                                                        document.documentElement.scrollTop = 0;
                                                    }
                                                });
                                            }
                                            else {
                                                if (data.errorMessage == undefined) {
                                                    bootbox.alert({
                                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while processing your request ." + '</span>',
                                                        callback: function () {
                                                            //window.location.href = '/Error/SessionExpire';
                                                            $.unblockUI();
                                                        }
                                                    });
                                                }
                                                else {
                                                    if (data.errorMessage.length != 0) {

                                                        bootbox.alert({
                                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                                                        });
                                                        $('#txtPassword').val('');
                                                        $('#txtConfirmPassword').val('');
                                                        $.unblockUI();
                                                    }
                                                    else {
                                                        bootbox.alert({
                                                            //   size: 'small',
                                                            //   title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                                                        });
                                                        $('#txtPassword').val('');
                                                        $('#txtConfirmPassword').val('');

                                                    }
                                                }

                                                $.unblockUI();
                                            }

                                        },
                                        error: function () {
                                            bootbox.alert({
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error occured while processing your request.</span>',
                                            });

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
      //  }
    });

    $('#txtPAN').keyup(function () {
        this.value = this.value.toUpperCase();
    });

    $("#btnReset").click(function () {
      $('.text-danger').text(""); //to reset error messages
    });

    // on 11 - 4 - 2019 Commented by SB  due to requirement change
    //// Added by Shubham Bhagat on 15-12-2018
    //$('#OfficeNamesDropDownId').change(function () {
    //    //alert("in OfficeNamesDropDownId");
    //    //if ($('#firstChildMenuDetailsList').val() == 0) {
    //    //    $('#secondChildMenuListDivId').hide();
    //    //}
    //    //alert($('#firstChildMenuDetailsList').val());

    //    if ($('#OfficeNamesDropDownId').val() == 0) {
    //        // Added By SHubham Bhagat on 15-12-2018
    //        $('#verticalLine2Id').css('height', '60%');
    //        $('#Office_ShortName_DivId').hide();
    //        $('#Office_OfficeType_DivID').hide();
    //        $('#Office_District_DivID').hide();

    //        // Added by Shubham Bhagat on 8-4-2019
    //        bootbox.alert('Please select Office', function () {
    //            $('#LevelDropDownListId').prop('disabled', true);
    //            $('#RoleDropDownListId').prop('disabled', true);
    //            $('#LevelDropDownListId').empty();
    //            $("#LevelDropDownListId").append(new Option("-- Select Level --", "0"));
    //            $('#RoleDropDownListId').empty();
    //            $("#RoleDropDownListId").append(new Option("-- Select Role --", "0"));
    //        });
    //    }


    //    // Added by Shubham Bhagat on 5-1-2019
    //    //if ($('#OfficeNamesDropDownId').val() > 0) {
    //    if ($('#OfficeNamesDropDownId').val() != 0) {
    //        //alert('in office drop down');
    //        $('#verticalLine2Id').css('height','78%');
    //        BlockUI();
    //        $.ajax({
    //            url: '/UserManagement/OfficeUserDetails/GetOfficeDetailsInfo/',
    //            data: { "officeDetailId": $('#OfficeNamesDropDownId option:selected').val() },
    //            datatype: "json",
    //            type: "GET",
    //            success: function (data) {
    //                //alert(data.Office_OfficeType);
    //                //alert(data.Office_ShortName);
    //                $('#Office_ShortName_DivId').show();                    
    //                $('#Office_OfficeType_DivID').show();                    
    //                $('#Office_District_DivID').show();
    //                //$('#te').text(data.Office_OfficeType);
    //                //$('#Office_OfficeType_Id').val(data.Office_OfficeType);
    //                //$('#Office_District_Id').html(data.Office_ShortName);
    //                //$('#Office_ShortName_Id').value=data.Office_District;

    //                $('#Office_OfficeType_Id').text(data.Office_OfficeType);
    //                $('#Office_District_Id').text(data.Office_District);
    //                $('#Office_ShortName_Id').text( data.Office_ShortName);

    //                // commented by Shubham Bhagat on 8-4-2019 due to requirement change to not populate roleList on office change
    //                //// Added by Shubham Bhagat on 5-1-2019
    //                //$('#RoleDropDownListId').empty();
    //                //$.each(data.RoleDropDown, function (i, RoleDropDownListVariable) {
    //                //    $('#RoleDropDownListId').append('<option value="' + RoleDropDownListVariable.Value + '">' + RoleDropDownListVariable.Text + '</option>');
    //                //});

    //                // Added by Shubham Bhagat on 5-1-2019
    //                $('#LevelDropDownListId').empty();
    //                $.each(data.LevelDetailsDropDown, function (i, LevelDetailsDropDownVariable) {
    //                    $('#LevelDropDownListId').append('<option value="' + LevelDetailsDropDownVariable.Value + '">' + LevelDetailsDropDownVariable.Text + '</option>');
    //                });
    //                $('#LevelDropDownListId').prop('disabled', false);
                    
    //                $.unblockUI();
    //            },
    //            error: function (xhr, status, err) {
    //                //bootbox.alert("Error " + err);
    //                bootbox.alert({
    //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
    //                    callback: function () {
    //                    }
    //                });
    //                $.unblockUI();
    //            }
    //        });
    //    }
    //});

    
    //$('#parentMenuDetailsList').change(function () {
    //    //alert("in parent change");

    //    if ($('#parentMenuDetailsList').val() == 0) {
    //        $('#secondChildMenuListDivId').hide();
    //        $('#firstChildMenuListDivId').hide();
    //        $('#firstChildMenuDetailsList').empty();
    //        $('#secondChildMenuDetailsList').empty();
    //    }

    //    if ($('#parentMenuDetailsList').val() == -1) {
    //        bootbox.alert("Please Select Parent Menu.");
    //    }

    //    if ($('#parentMenuDetailsList').val() > 0) {
    //        $('#firstChildMenuListDivId').show();
    //        $('#secondChildMenuListDivId').hide();
    //        BlockUI();
    //        $.ajax({
    //            url: '/UserManagement/MenuDetails/GetFirstChildMenuDetailsList/',
    //            data: { "parentId": $('#parentMenuDetailsList option:selected').val() },
    //            datatype: "json",
    //            type: "POST",
    //            success: function (data) {
    //                $('#firstChildMenuDetailsList').empty();
    //                $('#secondChildMenuDetailsList').empty();
    //                $.each(data.firstChildMenuDetailsList, function (i, firstChildMenuDetails) {
    //                    $('#firstChildMenuDetailsList').append('<option value="' + firstChildMenuDetails.Value + '">' + firstChildMenuDetails.Text + '</option>');
    //                });
    //                $.unblockUI();
    //            },
    //            error: function (xhr, status, err) {
    //                //bootbox.alert("Error " + err);
    //                bootbox.alert({
    //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
    //                    callback: function () {
    //                    }
    //                });
    //                $.unblockUI();
    //            }
    //        });
    //    }
    //});

    // Added by Shubham Bhagat on 08-04-2019
    $('#LevelDropDownListId').change(function ()
    {      
        if ($('#LevelDropDownListId').val() == 0) {          
            bootbox.alert('Please select level', function () {
                $('#RoleDropDownListId').prop('disabled', true);
                $('#RoleDropDownListId').empty(); 
                $("#RoleDropDownListId").append(new Option("-- Select Role --", "0"));
                $('#OfficeNamesDropDownId').prop('disabled', true);
                $('#OfficeNamesDropDownId').empty();
                $("#OfficeNamesDropDownId").append(new Option("-- Select Office --", "0"));
            });
        }
       
        if ($('#LevelDropDownListId').val() != 0)
        {
            BlockUI();
            $.ajax({
                url: '/UserManagement/OfficeUserDetails/GetRoleListByLevel/',
                data: { "LevelID": $('#LevelDropDownListId option:selected').val() },
                datatype: "json",
                type: "GET",
                success: function (data) {
                    if (data.errorMessage != undefined) {
                        $.unblockUI();
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            callback: function () {
                                window.location.href = "/Home/HomePage"
                            }
                        });
                    }
                    else {
                        // Added by Shubham Bhagat on 08-04-2019
                        $('#RoleDropDownListId').empty();
                        $.each(data.RoleDropDown, function (i, RoleDropDownListVariable) {
                            $('#RoleDropDownListId').append('<option value="' + RoleDropDownListVariable.Value + '">' + RoleDropDownListVariable.Text + '</option>');
                        });
                        $('#RoleDropDownListId').prop('disabled', false);

                        // Added by Shubham Bhagat on 11-04-2019
                        $('#OfficeNamesDropDownId').empty();
                        $.each(data.OfficeNamesDropDown, function (i, OfficeDropDownListVariable) {
                            $('#OfficeNamesDropDownId').append('<option value="' + OfficeDropDownListVariable.Value + '">' + OfficeDropDownListVariable.Text + '</option>');
                        });
                        $('#OfficeNamesDropDownId').prop('disabled', false);
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    //bootbox.alert("Error " + err);
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    $.unblockUI();
                }
            });
        }
    });


    //Added by mayank for User Manager on 26/08/2021
    if (isDrLogin == 'True') {
        //alert("True");
        //$('select').prop("disabled", true);
        $('select option:not(:selected)').attr("disabled", "disabled")
        $("#RegistrationOfficeUserFormId :input").prop('readonly', true);
    }
    
});

function isEmailAddress(str) {
    var pattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    return pattern.test(str);  // returns a boolean 
}
