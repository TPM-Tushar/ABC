
//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $("#btnSaveAbortDesc").click(function () {
        SaveAbortDesc();
    });

});



function SaveAbortDesc() {
    var IsAbortDescContainsSpecialSymbols;
    var form = $("#FormSaveAbortDescID");
    //form.removeData('validator');
    //form.removeData('unobtrusiveValidation');
    //$.validator.unobtrusive.parse(form);

    var regexToMatchForAbortDesc = /^[^<>&]+$/;
    if (!regexToMatchForAbortDesc.test($('#AbortDescriptionID').val())) {
        IsAbortDescContainsSpecialSymbols = true;
        bootbox.alert('Special characters like <, >, &  is not allowed.', function () {
        });
        //unBlockUI();
        return false;
    }
    if (!IsAbortDescContainsSpecialSymbols) {
        bootbox.confirm({
            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
            message: "<span class='boot-alert-txt'>Are you sure you want to abort?</span>",
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> No',
                    className: 'pull-right margin-left-NoBtn'
                }, confirm: {
                    label: '<i class="fa fa-check"></i> Yes'
                }
            },
            callback: function (result) {

                if (result) {

                    blockUI('Loading data please wait.');
                    $.ajax({
                        url: "/MISReports/DataRestorationReport/SaveAbortData/",
                        type: "POST",
                        data: $("#FormSaveAbortDescID").serialize(),
                        headers: header,
                        success: function (data) {
                            //alert('1');
                            if (data.success) {
                                unBlockUI();
                                bootbox.alert({
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function (result) {
                                        //alert('2');
                                        // RELOAD DATATABLE
                                        //LoadControllerActionDetailsData();

                                        // UNBLOCK SHIFTED TO ABOVE BOOTBOX.ALERT BECAUSE IT IS HEARING IS BLOCKING 
                                        // BECAUSE IT IS IN CALLBACK OF BOOTBOX.ALERT AND SCREEN IS BLOCKED SO WE ARE UNABLE
                                        // CLICK OK
                                        //unBlockUI();

                                        $('#divViewAbortModal').modal('hide');
                                        // TRIGGER SEARCH BTN FOR REFREASHING THE DATATABLE TO REMOVE THE ABORT BTN
                                        //$("#SearchBtn").trigger("click");
                                        window.location.href = "/MISReports/DataRestorationReport/DataRestorationReport"

                                    }
                                });
                            }
                            else {
                                if (data.success == false && data.IsServerError == true) {
                                    bootbox.alert({
                                        //   size: 'small',
                                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + ' </span>',
                                        callback: function () {
                                            window.location.href = "/Home/HomePage"
                                        }
                                    });
                                }
                                else {
                                    bootbox.alert({
                                        //   size: 'small',
                                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + ' </span>',
                                        callback: function () {

                                        }
                                    });

                                }

                                unBlockUI();
                            }
                        },
                        error: function () {
                            bootbox.alert({
                                //   size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing Abort details. </span>',
                            });
                            unBlockUI();
                        }
                    });

                }
            }
        });
    }
    // }



}