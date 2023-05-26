$(document).ready(function () {
   
    $("#closeWorkFlowActionForm").click(function () {
        OpenOfficeList();
        $("#DivWorkFlowActionDetailsWrapper").fadeOut(500);
        $("#panelNewWorkFlowAction").fadeIn();

        document.documentElement.scrollTop = 0;
    });
    $("#btnCancel").click(function () {
        OpenOfficeList();
        $("#DivWorkFlowActionDetailsWrapper").fadeOut(500);
        $("#panelNewWorkFlowAction").fadeIn();
        document.documentElement.scrollTop = 0;
    });

    if ($("#drpDistrictList").val() == 0)
        $("#drpOfficeNameList").prop('disabled', true);

    //for inserting he record
    $("#btnSave").click(function () {
        var form = $("#WorkFlowActionDetailsForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);

        if ($("#WorkFlowActionDetailsForm").valid()) {
            $.ajax({
                url: "/UserManagement/WorkFlowActionDetails/CreateUpdateNewWorkFlowAction",
                type: "POST",
                data: $("#WorkFlowActionDetailsForm").serialize(),
                success: function (data) {
                    //  bootbox.alert(data.message);
                    if (data.success) {
                        bootbox.alert({
                            //   size: 'small',
                            //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                      
                        callback: function () {
                            GetJsonData();
                            OpenOfficeList();
                            $("#DivWorkFlowActionDetailsWrapper").fadeOut(500);
                            $("#panelNewWorkFlowAction").fadeIn();
                            document.documentElement.scrollTop = 0;
                        }
                        });
                    } else {
                        bootbox.alert({
                            //   size: 'small',
                            //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                        });
                    }
                },
                error: function () {
                    bootbox.alert({
                        //   size: 'small',
                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>',

                    });
                }
            });
        } else {
            return;
        }
    });
    $("#btnUpdate").click(function () {
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
                    var form = $("#WorkFlowActionDetailsForm");
                    form.removeData('validator');
                    form.removeData('unobtrusiveValidation');
                    $.validator.unobtrusive.parse(form);
                    var fun = 5;
                    if ($("#WorkFlowActionDetailsForm").valid()) {
                        $.ajax({
                            url: "/UserManagement/WorkFlowActionDetails/UpdateWorkflowAction",
                            type: "POST",
                            data: $("#WorkFlowActionDetailsForm").serialize(),
                            success: function (data) {
                                if (data.success) {
                                    bootbox.alert({
                                        //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                        callback: function () {
                                            GetJsonData();
                                            OpenOfficeList();
                                            $("#DivWorkFlowActionDetailsWrapper").fadeOut(500);
                                            $("#panelNewWorkFlowAction").fadeIn();
                                            document.documentElement.scrollTop = 0;
                                        }
                                    });
                                }
                                else {
                                    bootbox.alert({
                                        //   size: 'small',
                                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in Updating</span>',

                                    });
                                }
                            },
                            error: function () {
                                bootbox.alert({
                                    //   size: 'small',
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>',

                                });
                            }
                        });
                    }
                    else { return; }
                }
            }
        });

    });
});
