
$(document).ready(function () {

    //Added by madhur on 15-02-2022
    $(document).bind("contextmenu", function (e) {
        e.preventDefault();
    });
    $(document).keydown(function (e) {
        if (e.which === 123) {
            return false;
        }
    });

    //Added by Madhur on 22-08-2022
    sessionStorage.setItem("IsValidated", "0");

    if (IsMobileNumberVerfied == 0) {

        // Bootbox confirmation dialog for sending OTP
        BlockUI();

        bootbox.confirm({
            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
            message: '<span class="boot-alert-txt">You have not verified your mobile number.<br />Do you want to verify it now?</span>',
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
                    $.ajax({
                        type: "GET",
                        url: "/Alerts/SendSMSDetails/VerifyCaptchaForOTP",
                        success: function (data) {

                                $('#divViewOTPValidationModal').modal({
                                    backdrop: 'static',
                                    keyboard: false
                            });
                          //  console.log(data);
                                $('#divLoadOTPValidationView').html(data);
                               // $('#OTPID').focus();
                            $('#divViewOTPValidationModal').modal('show');
                            $('#divCaptcha').show();
                            $("#divOTPInput").hide();
                                $("#btnCloseOTPModal").off("click").click(function () {
                                    $('#divViewOTPValidationModal').modal('hide');
                                });
                            //$('#divOTPValidationModal').css('display', 'none!important');
                            //Added by mayank to relaod and get user details on 16-02-2022
                            $('#divOTPValidationModal').attr("style", "display: none!important");
                            //
                            $.unblockUI();

                        },
                        error: function (err) {
                            $.unblockUI();

                            bootbox.alert("Error: " + err);
                        }
                    });

       
                }
                $.unblockUI();

            }
        });

        // Bootbox confirmation dialog for sending OTP
    }

    // checking if user mobile no is verified -- shrinivas




    $("#lblModuleName").text(DefaultModuleName);

    $("#txtModuleNo").val(DefaultModuleID);
   
   loadSideBarStatistics(DefaultModuleID, DefaultModuleName);
     
    $("#btnRefreshStats").click(function () {
        if ($("#frmStatistics").valid()) {

            $.ajax({
                type: "POST",
                url: "/Home/GetHomePageSideBarStatistics",
                data: $("#frmStatistics").serialize(),

                success: function (data) {

                    $("#ULsubModuleStatsList").empty();

                    var strdata = "";
                    $.each(data.subModuleList, function () {
                        strdata += '<li><a href="#"><i class="fa fa-dot-circle-o"></i>&nbsp;&nbsp;' + this.SubModuleName + '<span class="pull-right badge bg-blue" style="min-width:40px;">' + this.Count + "</span></a></li>";
                    });

                    $("#ULsubModuleStatsList").append(strdata);
                },
                error: function (xhr, status, err) {
                    bootbox.alert("Error " + err);
                }
            });
        }
    });


    //LoggOFF onClick
    $("#btnLoggOffID").click(function () {
        //  BlockUI();
        $.ajax({
            type: "Get",
            url: "/Login/Logout",
            //data: { "encryptedID": encryptedID },
            success: function (data) {

                if (data.success == true) {

                    window.location.href = data.URL;

                }
            },
            error: function () {
                bootbox.alert("error");
                //  $.unblockUI();
            }
        });
    });


    //********************** To change "Month & Year" in Side bar stats panel. *****************
    $('.DropDownInStats').change(function () {
        CopyMonthAndYear();
    });
    $('#ddlmonths').trigger('change');
});

function CopyMonthAndYear() {
    var SelectedMonthId=$('#ddlmonths :selected');
    var SelectedYearId=$('#ddlyears :selected');
    var Month = SelectedMonthId.val();
    var Year = SelectedYearId.val();

    //alert(Month);
    // alert(Month.length);

    if (Year != 0 && (Month != 0 && Month.length < 12)) {


        if (Month == 1) {
            $("#MonthInStatBar").text("");
            $("#YearInStatBar").text("Year - " + SelectedYearId.text());
        }
        else
        {
            $("#MonthInStatBar").text(SelectedMonthId.text());
            $("#YearInStatBar").text("- " + SelectedYearId.text());
        }

       

        //******** To refresh stats on change of DropDowns of Month or Year **********
        $("#btnRefreshStats").trigger("click");
    }


}

function loadSideBarStatistics(ModuleId) {

    ToggleSubModules(ModuleId);

    //****************** To set Module Name in "Side Bar Statistics" ********************************
    $("#lblModuleName").text(arguments[1]);
    $("#txtModuleNo").val(ModuleId);

    //*************** To add and remove "Icon of Module" in side bar statistics *************************
    var IconID = "#ModuleIcon_" + ModuleId;
    var iconClassToSet = $(IconID).attr('class');
    //var iconClassToRemove = $(ModuleSideBarStatsIcon).attr('class');
    //$(ModuleSideBarStatsIcon).removeClass(iconClassToRemove).addClass(iconClassToSet);

    //*************** To add and remove "Background Color of Module Title Panel" in side bar statistics *************************
    var divPanelID = "#divModuleColorPanel_" + ModuleId;
    var colorClassToSet = $(divPanelID).attr('class');
    //var colorClassToRemove = $(divSideBarModulePanelColor).attr('class');
    //$(divSideBarModulePanelColor).removeClass(colorClassToRemove).addClass(colorClassToSet);

    $('#ddlmonths').trigger('change');

}

function ToggleSubModules(ModuleID) {

    CloseRemainingModules();

    $('#div_SubModules_' + ModuleID).slideToggle();
    $('#div_SubModules_' + ModuleID).addClass('DropDownModuleList');

    var ToggleIconID = "#ToggleIcon_" + ModuleID;
    var classToRemove = $(ToggleIconID).attr('class');
    var classToSet = (classToRemove == "fa fa-plus" ? "fa fa-minus" : "fa fa-plus");
    $(ToggleIconID).removeClass(classToRemove).addClass(classToSet);
}

function CloseRemainingModules() {
    // alert($(".btnModuleToggle > i").length);
    //alert($(".btnModuleToggle > i[class='fa fa-minus']").length);

    $(".btnModuleToggle > i[class='fa fa-minus']").removeClass('fa fa-minus').addClass('fa fa-plus');
    $(".DropDownModuleList").slideToggle();
    $(".DropDownModuleList").removeClass('DropDownModuleList');

}


function loadMenus(menuId, moduleId, modulename) {
    //Added if else condition by Madhur on 15-02-2022 for showing otp popup if menuID matches if condition
    //Added By Tushar on 31 jan 2023
    var MenuDisabledList = [];
    var MenuDisabledarrayString = $("#MenuEnabledList").val();
   
    MenuDisabledList = MenuDisabledarrayString.split(',');

   
   var Result= GetMenuDisabledDetails(menuId,MenuDisabledList);

    if (Result) {
    //End By Tushar on 31 jan 2023
    var arr = [];
    var arrayString = $("#ForOTPString").val();
    arr = arrayString.split(',');
    if (arr.includes(menuId.toString())) {
        $.ajax({
            url: '/Home/IsMobileNumberVerified',
            type: "GET",
            success: function (data) {
                if (data.success == true) {

                    $.ajax({
                        type: "GET",
                        url: "/Home/InputOTPFromUser",
                        success: function (data1) {

                            if (data1.success) {
                                $.ajax({
                                    url: '/Home/LoadModal',
                                    type: "GET",
                                    success: function (data3) {

                                        $('#divOTPValidationModal').modal({
                                            backdrop: 'static',
                                            keyboard: false
                                        });
                                        $("#divOTPInput").show();
                                        $('#OTPID').focus();
                                        $('#divLoadOTPValidation').html(data3);
                                        $("#divForMessageResend").show();
                                        $('#OTPValidationFormId').append('<input type="hidden" id="IsOTPSent" name="IsOTPSent" value="' + data1.IsOTPSent + '"><input type="hidden" id="OTPTypeId" name="OTPTypeId" value="' + data1.OTPTypeId + '"><input type="hidden" id="EncryptedUId" name="EncryptedUId" value="' + data1.EncryptedUId + '">');
                                        $('#MobileNumbertoDisplayID').html(data1.MobileNumberToDisplay);
                                        $('#divOTPValidationModal').modal('show');
                                        sessionStorage.setItem("menuId", menuId);
                                        sessionStorage.setItem("moduleId", moduleId);
                                        sessionStorage.setItem("modulename", modulename);

                                        $('#CLPP').click(function () {
                                            $('#divOTPValidationModal').modal('hide');
                                        })


                                    },
                                    error: function (err) {
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
                            else {
                                if (data.message == undefined) {
                                    bootbox.alert({
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while processing your request ." + '</span>',
                                        callback: function () {

                                            window.location.href = '/Error/SessionExpire';
                                            //    $('#btnLoggOffID').trigger('click');
                                            //   return false;
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
                }
                else {
                    bootbox.alert('<span class="boot-alert-txt"><i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i>Mobile Number not verified. Please verify it first.</span>');
                }
            },
            error: function (xhr) {
                alert(xhr);
            }
        });
    }
    else {
        //added by madhur on 12-08-2022 for removing session if redirected to different module
        sessionStorage.setItem("IsValidated", "0");
        window.location.href = "/Home/RedirectToMenuPage/" + menuId + "$" + moduleId + "$" + modulename;
         }
    }
}



var request = 1;
function CheckClientStatus() {

    $.ajax({
        url: "/Home/KeepSessionAlive?request=" + request,
        cache: false,
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data.success) {
                request++;
                setTimeout("CheckClientStatus()", 3000);//4seconds
            }
            else {

            }
        },
        error: function () {
            console.log('Errror............!!!');
            $.unblockUI();
        }
    });

}


//Added by madhur on 15-02-2022
function ValidateOTP() {

        if ($.trim($('#OTPID').val()) != '') {
            $.ajax({
                url: "/Home/GetSessionSalt",
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
                                url: "/Home/ValidateOTP",
                                headers: header,
                                data: $("#OTPValidationFormId").serialize(),
                                dataType: "json",
                                success: function (data) {
                                    if (data.success) {
                                        bootbox.alert({
                                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.msg + '</span>',
                                            callback: function () {
                                            }
                                        });
                                        var menuID = sessionStorage.getItem("menuId");
                                        var moduleId = sessionStorage.getItem("moduleId");
                                        var modulename = sessionStorage.getItem("modulename");
                                        sessionStorage.setItem("IsValidated", "1");
                                        window.location.href = "/Home/RedirectToMenuPage/" + menuID + "$" + moduleId + "$" + modulename;
                                    }


                                    else {
                                        if (data.msg == undefined) {
                                            bootbox.alert({
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while processing your request ." + '</span>',
                                                callback: function () {

                                                    window.location.href = '/Error/SessionExpire';
                                                    $.unblockUI();
                                                }
                                            });
                                        }
                                        else {
                                            bootbox.alert({
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.msg + '</span>',
                                                callback: function () {
                                                    $('#divOTPValidationModal').css('display','block');

                                                    $('#OTPID').val("");
                                                    $('#OTPID').focus();
                                                }
                                            });
                                        }
                                    }
                                },
                                error: function (err) {
                                    bootbox.alert({
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                                        callback: function () {
                                            $('#divOTPValidationModal').modal('show');

                                        }
                                    });
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
        }
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

//Added By Tushar on 31 Jan 2023
function GetMenuDisabledDetails(menuId, MenuDisabledList) {
    var Result = false;

    if (MenuDisabledList.includes(menuId.toString())) {

        BlockUI();
        $.ajax({
            url: '/Home/GetMenuDisabledDetails',
            datatype: "json",
            type: "GET",
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == false) {
                    $.unblockUI();
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                        function () {
                            
                        });
    
                    Result = data.success;
                }
             
                Result= data.success;
              
            },
            error: function (xhr) {
                $.unblockUI();
                Result = false;
            }
        });
   
        $.unblockUI();
    } else {
  

        Result = true;
    }
    return Result;
}

//End By Tushar on 31 jan 2023