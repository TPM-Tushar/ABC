var token = '';
var header = {};
//var isInitiationDateBtnClicked = false;
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
       
    //alert($("#ShowInitDateAndGeneratedKeyMsgID").val());
    if ($("#ShowInitDateAndGeneratedKeyMsgID").val() === "True") {
        //alert("in if");   
        //$("#InitiationDateSpanID").show();
        //$("#GenerateKeyTextSpanID").show();
        $("#InitiationDateBTNSpanID").hide();
    }
    else {
        //alert("in else");
        $("#InitiationDateSpanID").hide();
        $("#GenerateKeyTextSpanID").hide();
    }

    //alert($('#dB_RES_TABLE_WISE_COUNT_ListCoundID').val());
    // ADDED ON 22-07-2020 AT 5:30 PM BY SHUBHAM BHAGAT TO HIDE MESSAGE
    if ($('#dB_RES_TABLE_WISE_COUNT_ListCoundID').val() > 0)
    {
        $('#ApproveBtnID').html('');
        $('#ApproveBtnID').hide();
    }

     // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 7:25 PM
    if (IsInitaiteNewProcess === true)
    {
        InitiateDateFun();
        IsInitaiteNewProcess = false;
    }

});

function InitiateDateFun() {
    $("#InitiationDateSpanID").show();
    $("#InitiationDateBTNSpanID").hide();
    //isInitiationDateBtnClicked = true;
    //alert("wergreg");
    GenerateKeyFun();
}

function GenerateKeyFun() {
    //alert($("#GenerateKeyValueSpanID").text());//gives value in span 
    //alert($("#GenerateKeyValueSpanID").val());// not gives anything gives 
    //alert($("#GenerateKeyValueSpanID").html());//gives value in span 

    //if (isInitiationDateBtnClicked)
    //{

    $.ajax({
        url: '/MISReports/DataRestorationReport/InitiateDatabaseRestoration',
        data: {
            "SroID": $("#SROOfficeListID option:selected").val(),
            "GenerateKeyValue": $("#GenerateKeyValueSpanID").text(),
            "InitiationDate": $("#InitiationDateSpanID").text()
        },
        datatype: "json",
        headers: header,
        type: "POST",
        success: function (data) {
            //add validations
            if (data.errorMessage == undefined) {
                $("#GenerateKeyTextSpanID").show();
                $("#GenerateKeyBtnSpanID").hide(); // hide btn or set empty space

                //$('#InformationPanedID').html('');
                //$('#InformationPanedID').html(data);
                unBlockUI();
            }
            else if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
            }
        },
        error: function (xhr) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            unBlockUI();
        }
    });
    //}
    //else {
    //    bootbox.alert("<h4>Please click on Initiate Date button.</h4>");
    //}

    // ADDED BY SHUBHAM BHAGAT ON 31-07-2020 AT 12:25 PM
    // TO DRAW DATATABLE AGAING AFTER INITIATING NEW PROCESS
    $('#DailyReceiptDetailsTable').DataTable().draw();
    //$("#SearchBtn").trigger("click");

}


function GenerateKeyAfterExpiration(initID, keyID) {
    $.ajax({
        url: '/MISReports/DataRestorationReport/GenerateKeyAfterExpiration',
        data: {
            "initID": initID,
            "keyID": keyID
        },
        datatype: "json",
        headers: header,
        type: "POST",
        success: function (data) {
            //add validations
            if (data.errorMessage == undefined) {
                $("#GenerateKeyTextSpanID").html(data.GeneratedKeyWithMsgAfterExpiration);

                //$("#GenerateKeyTextSpanID").show();
                //$("#GenerateKeyBtnSpanID").hide(); // hide btn or set empty space

                //$('#InformationPanedID').html('');
                //$('#InformationPanedID').html(data);
                unBlockUI();
            }
            else if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
            }
        },
        error: function (xhr) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            unBlockUI();
        }
    });
}


function ApproveScript(scriptID, InitID) {
    $.ajax({
        url: '/MISReports/DataRestorationReport/ApproveScript',
        data: {
            "scriptID": scriptID,
            "InitID": InitID
        },
        datatype: "json",
        headers: header,
        type: "POST",
        success: function (data) {
            //add validations
            if (data.errorMessage == undefined) {
                //alert("a"+data.IsScriptApprovedSuccefully+"b");
                if (data.IsScriptApprovedSuccefully === true) {
                    //alert('if')
                    $("#ApproveBtnID").html('');

                    // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                    // COMMENTED BELOW ALERT AS DISCUSSED WITH SIR
                    //bootbox.alert(data.ScriptApprovedMSG);

                    // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                    // DISPLAYED MESSAGE IN PLACE OF BUTTON AS DISCUSSED WITH SIR
                    $("#ApproveBtnID").html(data.ApproveBtnORMessage);

                    // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                    $("#NoteForApprovalForSRID").html('');

                    // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                    //$("#ApproveBtnID").html("<label style='font-size:initial;font-weight:bold;color: #3177b4;'>" + data.ApproveBtnORMessage + "</label>");
                    //$("#ApproveBtnID").css('font-size', 'initial');
                    //$("#ApproveBtnID").css('font-weight', 'bold');
                    //$("#ApproveBtnID").css('color', '#3177b4');
                    //$("#ApproveBtnID").text(data.ApproveBtnORMessage);


                    //alert('efwfew');

                    // CALL METHOD TO RELOAD DATA INSERTION DETAILS TABLE
                    token = $('[name=__RequestVerificationToken]').val();
                    header["__RequestVerificationToken"] = token;

                    // load partial view
                    $.ajax({
                        url: '/MISReports/DataRestorationReport/DataInsertionTable',
                        data: {
                            "scriptID": scriptID,
                            "InitID": InitID,
                            // FLAG ADDED ON 10-07-2020 AT 4:45 PM
                            "IsRectifiedScriptUploaded": "false",
                        },
                        datatype: "json",
                        headers: header,
                        type: "POST",
                        success: function (data) {
                            //add validations
                            if (data.errorMessage == undefined) {
                                $('#dataInsertionTableDivID').html('');
                                $('#dataInsertionTableDivID').html(data);
                                unBlockUI();
                            }
                            else if (data.serverError == true) {
                                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                                    function () {
                                        window.location.href = "/Home/HomePage"
                                    });
                            }
                            else {
                                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                            }
                        },
                        error: function (xhr) {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                callback: function () {
                                }
                            });
                            unBlockUI();
                        }
                    });


                }
                else {
                    //alert('else')
                    bootbox.alert(data.ScriptApprovedMSG);
                }

                //$("#GenerateKeyTextSpanID").html(data.GeneratedKeyWithMsgAfterExpiration);

                //$("#GenerateKeyTextSpanID").show();
                //$("#GenerateKeyBtnSpanID").hide(); // hide btn or set empty space

                //$('#InformationPanedID').html('');
                //$('#InformationPanedID').html(data);
                unBlockUI();
            }
            else if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
            }
        },
        error: function (xhr) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            unBlockUI();
        }
    });
}

function DownloadScriptForRectification(InitID, scriptID) {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $.ajax({
        url: '/MISReports/DataRestorationReport/DownloadScriptPathVerify',
        data: {
            "scriptID": scriptID,
            "InitID": InitID
        },
        datatype: "json",
        type: "GET",
        //headers: header,
        //type: "POST",
        success: function (data) {
            if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                if (data.serverError == false && data.success == false) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                }
                else {
                    if (data.IsFileExistAtDownloadPath) {
                        // ADDED BY SHUBHAM BHAGAT ON 05-08-2020                          
                        window.location.href = '/MISReports/DataRestorationReport/DownloadScriptForRectification?InitID=' + InitID + '&scriptID=' + scriptID + '&SroID=' + DT_SROCodeForDownloadFile;
                        //window.location.href = '/MISReports/DataRestorationReport/DownloadScriptForRectification?InitID=' + InitID + '&scriptID=' + scriptID + '&SroID=' + SroID;
                    }
                    else {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">File not exist at path.</span>');
                    }
                }
            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });
}


function UploadRectifiedScript(InitID, scriptID) {
    // CALL METHOD TO RELOAD DATA INSERTION DETAILS TABLE
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    // load partial view
    $.ajax({
        url: '/MISReports/DataRestorationReport/UploadRectifiedScriptView',
        data: {
            "scriptID": scriptID,
            "InitID": InitID
        },
        datatype: "json",
        headers: header,
        type: "POST",
        success: function (data) {
            //add validations
            if (data.errorMessage == undefined) {
                $('#UploadScriptPartialViewDivID').html('');
                $('#UploadScriptPartialViewDivID').html(data);
                unBlockUI();
            }
            else if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
            }
        },
        error: function (xhr) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            unBlockUI();
        }
    });

}




//pankaj for js work here
function ConfirmDataInsertion(InitId, officeID) {
    $.ajax({
        url: '/MISReports/DataRestorationReport/ConfirmDataInsertion',
        data: {
            "InitId": InitId,
            "officeID": officeID
        },

        datatype: "json",
        headers: header,
        type: "POST",
        success: function (data) {
            //console.log('inside func ' + data);
            if (data.errorMessage == undefined) {
                if (data.IsDataInsertionConfirmed == true) {
                    $('#ConfirmBtnAndMsgId').html('');

                    // COMMENTED AND ADDED ON 22-07-2020 BY SHUBHAM BHAGAT
                    //$('#ConfirmMsg').html(data.DataInsertionConfrimationMsg);
                    //$('#ConfirmMsg').html('');
                                       
                    $('#ConclusionMsgID').html(data.DataInsertionConfrimationMsg);

                    $('#ApproveBtnID').html('');
                    $('#ApproveBtnID').hide();

                    //test
                    //token = $('[name=__RequestVerificationToken]').val();
                    //header["__RequestVerificationToken"] = token;

                    //$.ajax({
                    //    url: '/MISReports/DataRestorationReport/GetConfirmationButtonMessage',
                    //    data: {
                    //        "InitId": InitId,
                    //        "officeID": officeID
                    //    },

                    //    datatype: "json",
                    //    headers: header,
                    //    type: "POST",
                    //    success: function (data) {
                    //        console.log('inside inner func ');
                    //        if (data.errorMessage == undefined) {
                    //            if (data.IsDataInsertionConfirmed == true) {
                    //                $('#ConfirmBtnAndMsgId').html('');
                    //                $('#ConfirmBtnAndMsgId').html(data.DataInsertionConfrimationMsg);

                    //                unBlockUI();
                    //            }
                    //        }
                    //        else if (data.serverError == true) {
                    //            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    //                function () {
                    //                    window.location.href = "/Home/HomePage"
                    //                });
                    //        }
                    //        else {
                    //            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                    //        }
                    //    },
                    //    error: function (xhr) {
                    //        bootbox.alert({
                    //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    //            callback: function () {
                    //            }
                    //        });
                    //        unBlockUI();
                    //    }

                    // })
                    //test end

                    unBlockUI();

                }
            }
            else if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
            }
        },
        error: function (xhr) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            unBlockUI();
        }
    })

}
//pankaj section end