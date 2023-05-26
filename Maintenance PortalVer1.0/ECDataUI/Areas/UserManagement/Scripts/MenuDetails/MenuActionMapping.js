
//Global variables.
var token = '';
var header = {};


$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#ControllerActionDetails_AreaList').prop("disabled", false);

    $('#ControllerActionDetails_ControllerList').prop("disabled", true);

    $('#ControllerActionDetails_ActionList').prop("disabled", true);



    $("#closeMenuDetailsForm").click(function () {
        $("#panelMenuDetailsForm").hide();
        $("#ShowAddMenuFormID").show();
        OpenMenuDetailsList();
        //$("#ShowAddMenuFormID").hide();
    });
    $("#MAS_Modules_ModuleList").change(function () {
        //if ($('#MAS_Modules_ModuleList').val() == -1) {
        //    $('#ControllerActionDetails_AreaList').prop("disabled", true);
        //    $('#ControllerActionDetails_ControllerList').prop("disabled", true);

        //    $('#ControllerActionDetails_ActionList').prop("disabled", true);
        //}
        //else {
        //    if ($('#ControllerActionDetails_AreaList').val() == "Select") {
        //        $('#ControllerActionDetails_AreaList').prop("disabled", false);
        //        $('#ControllerActionDetails_ActionList').prop("disabled", true);
        //        $('#ControllerActionDetails_ControllerList').prop("disabled", true);
        //    } else if ($('#ControllerActionDetails_ControllerList').val() == "Select") {
        //        $('#ControllerActionDetails_ActionList').prop("disabled", true);
        //    } else {

        //        $('#ControllerActionDetails_AreaList').prop("disabled", false);

        //        $('#ControllerActionDetails_ControllerList').prop("disabled", false);

        //        $('#ControllerActionDetails_ActionList').prop("disabled", false);
        //    }


        //}

        $('#MapUnmapMenuActionButtonId').html('');
        $('#ControllerActionDetails_AreaList').val("Select");
        $('#ControllerActionDetails_ControllerList').empty();
        $('#ControllerActionDetails_AreaList').trigger("change");
    })

    // For Getting the controller names according to area
    $('#ControllerActionDetails_AreaList').change(function () {
        // Added By Shubham Bhagat on 29-10-2018
        $('#MapUnmapMenuActionButtonId').html('');
        if ($('#ControllerActionDetails_AreaList').val() == "Select") {
            // bootbox.alert("Please Select Area Name.");
            $('#ControllerActionDetails_ControllerList').empty();
            $('#ControllerActionDetails_ActionList').empty();
            $('#ControllerActionDetails_ControllerList').prop("disabled", true);
            $('#ControllerActionDetails_ActionList').prop("disabled", true);
        }
        else {
            BlockUI();
            $.ajax({
                url: '/UserManagement/MenuDetails/ControllerList/',
                data: { "EncryptedID": $('#EncryptedID').val(), "ControllerActionDetails_AreaListId": $('#ControllerActionDetails_AreaList').val() },
                datatype: "json",
                type: "POST",
                success: function (data) {
                    //alert(data.serverError);
                    if (data.serverError == true) {
                        //alert("if"+data.serverError);
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            function () {
                                window.location.href = "/Home/HomePage"
                            });
                    }
                    else {
                        //alert("else" + data.serverError);
                        $('#ControllerActionDetails_ControllerList').empty();
                        $('#ControllerActionDetails_ActionList').empty();

                        $.each(data.ControllerActionDetails_ControllerList, function (i, ControllerActionDetails_Controller) {
                            $('#ControllerActionDetails_ControllerList').append('<option value="' + ControllerActionDetails_Controller.Value + '">' + ControllerActionDetails_Controller.Text + '</option>');
                        });

                        $('#ControllerActionDetails_ControllerList').prop("disabled", false);
                        $('#ControllerActionDetails_ActionList').prop("disabled", true);
                    }
                    $.unblockUI();
                },
                error: function (xhr) {
                    //alert('error');
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

    // For Getting the controller names according to area
    $('#ControllerActionDetails_ControllerList').change(function () {
        // Added By Shubham Bhagat on 29-10-2018
        $('#MapUnmapMenuActionButtonId').html('');
        if ($('#ControllerActionDetails_ControllerList').val() == "Select") {
            bootbox.alert("Please Select Controller Name.");
            $('#ControllerActionDetails_ActionList').empty();
            $('#ControllerActionDetails_ActionList').prop("disabled", true);
        }
        else {
            BlockUI();
            $.ajax({
                url: '/UserManagement/MenuDetails/ActionList/',
                data: { "EncryptedID": $('#EncryptedID').val(), "ControllerActionDetails_ControllerListId": $('#ControllerActionDetails_ControllerList').val() },
                datatype: "json",
                type: "POST",
                success: function (data) {
                    if (data.serverError == true) {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            function () {
                                window.location.href = "/Home/HomePage"
                            });
                    }
                    else {
                        $('#ControllerActionDetails_ActionList').empty();

                        $.each(data.ControllerActionDetails_ActionList, function (i, ControllerActionDetails_Action) {
                            $('#ControllerActionDetails_ActionList').append('<option value="' + ControllerActionDetails_Action.Value + '">' + ControllerActionDetails_Action.Text + '</option>');
                        });

                        $('#ControllerActionDetails_ActionList').prop("disabled", false);
                    }
                    $.unblockUI();
                },
                error: function (xhr) {
                    //alert('error');
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

    $('#ControllerActionDetails_ActionList').change(function () {

        if ($('#MAS_Modules_ModuleList').val() == -1) {
            bootbox.alert("Please Select Module Name.");
        }
        else if ($('#ControllerActionDetails_AreaList').val() == "Select") {
            bootbox.alert("Please Select Area Name.");
        } else if ($('#ControllerActionDetails_ControllerList').val() == "Select") {
            bootbox.alert("Please Select Controller Name.");
        }
        else if ($('#ControllerActionDetails_ActionList').val() == "Select") {
            bootbox.alert("Please Select Action Name.");
            $('#MapUnmapMenuActionButtonId').html('');
        }
        else {
            BlockUI();
            $.ajax({
                url: '/UserManagement/MenuDetails/MapUnmapMenuActionButton/',
                data: {
                    "EncryptedID": $('#EncryptedID').val(),
                    "MAS_Modules_ModuleListId": $('#MAS_Modules_ModuleList').val(),
                    "ControllerActionDetails_AreaListId": $('#ControllerActionDetails_AreaList').val(),
                    "ControllerActionDetails_ControllerListId": $('#ControllerActionDetails_ControllerList').val(),
                    "ControllerActionDetails_ActionListId": $('#ControllerActionDetails_ActionList').val()
                },
                datatype: "json",
                type: "POST",
                success: function (data) {
                    if (data.serverError == true) {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            function () {
                                window.location.href = "/Home/HomePage"
                            });
                    }
                    else {
                        $('#MapUnmapMenuActionButtonId').html('');

                        $('#MapUnmapMenuActionButtonId').html(data.MapUnmapMenuActionButton);
                    }
                    $.unblockUI();
                },
                error: function (xhr) {
                    //alert('error');
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
});

function MapMenuToAction() {

    if ($('#MAS_Modules_ModuleList').val() == -1) {
        bootbox.alert("Please Select Module Name.");
    }
    else if ($('#ControllerActionDetails_AreaList').val() == "Select") {
        bootbox.alert("Please Select Area Name.");
    } else if ($('#ControllerActionDetails_ControllerList').val() == "Select") {
        bootbox.alert("Please Select Controller Name.");
    }
    else if ($('#ControllerActionDetails_ActionList').val() == "Select") {
        bootbox.alert("Please Select Action Name.");
    }
    else {
        var bootboxConfirm = bootbox.confirm({
            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
            message: "<span class='boot-alert-txt'>Do you want to Map Menu to Action?</span>",

            buttons: {
                confirm: {
                    label: '<i class="fa fa-check"></i> Yes'
                },
                cancel: {
                    label: '<i class="fa fa-times"></i> No',
                    className: 'pull-right margin-left-NoBtn'
                }
            },
            callback: function (result) {
                if (result) {
                    BlockUI();
                    $.ajax({
                        url: '/UserManagement/MenuDetails/MapMenuToAction/',
                        data: {
                            "EncryptedID": $('#EncryptedID').val(),
                            "MAS_Modules_ModuleListId": $('#MAS_Modules_ModuleList').val(),
                            "ControllerActionDetails_AreaListId": $('#ControllerActionDetails_AreaList').val(),
                            "ControllerActionDetails_ControllerListId": $('#ControllerActionDetails_ControllerList').val(),
                            "ControllerActionDetails_ActionListId": $('#ControllerActionDetails_ActionList').val()
                        },
                        datatype: "json",
                        type: "POST",
                        headers: header,
                        success: function (data) {
                            if (data.status === true) {
                                //bootbox.alert(data.message);

                                bootbox.alert({
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function () {
                                        $("#panelMenuDetailsForm").hide();
                                        $("#ShowAddMenuFormID").show();
                                        OpenMenuDetailsList();
                                    }
                                });
                                //$("#panelMenuDetailsForm").hide();
                                //$("#ShowAddMenuFormID").show();
                                //OpenMenuDetailsList();
                            }
                            else {
                                //bootbox.alert(data.message);
                                bootbox.alert({
                                    //   size: 'small',
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function () {
                                        window.location.href = "/Home/HomePage"
                                    }
                                });
                            }
                            $.unblockUI();
                        },
                        error: function (xhr) {
                            //alert('error');
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                callback: function () {
                                }
                            });
                            $.unblockUI();
                        }
                    });

                }
            }
        });


    }
}

function UnmapMenuToAction() {
    if ($('#MAS_Modules_ModuleList').val() == -1) {
        bootbox.alert("Please Select Module Name.");
    }
    else if ($('#ControllerActionDetails_AreaList').val() == "Select") {
        bootbox.alert("Please Select Area Name.");
    } else if ($('#ControllerActionDetails_ControllerList').val() == "Select") {
        bootbox.alert("Please Select Controller Name.");
    }
    else if ($('#ControllerActionDetails_ActionList').val() == "Select") {
        bootbox.alert("Please Select Action Name.");
    }
    else {
        var bootboxConfirm = bootbox.confirm({
            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
            message: "<span class='boot-alert-txt'>Do you want to Unmap Menu to Action ?</span>",

            buttons: {
                confirm: {
                    label: '<i class="fa fa-check"></i> Yes'
                },
                cancel: {
                    label: '<i class="fa fa-times"></i> No',
                    className: 'pull-right margin-left-NoBtn'
                }
            },
            callback: function (result) {
                if (result) {
                    BlockUI();
                    $.ajax({
                        url: '/UserManagement/MenuDetails/UnmapMenuToAction/',
                        data: {
                            "EncryptedID": $('#EncryptedID').val(),
                            "MAS_Modules_ModuleListId": $('#MAS_Modules_ModuleList').val(),
                            "ControllerActionDetails_AreaListId": $('#ControllerActionDetails_AreaList').val(),
                            "ControllerActionDetails_ControllerListId": $('#ControllerActionDetails_ControllerList').val(),
                            "ControllerActionDetails_ActionListId": $('#ControllerActionDetails_ActionList').val()
                        },
                        datatype: "json",
                        type: "POST",
                        headers: header,
                        success: function (data) {
                            if (data.status === true) {
                                //bootbox.alert(data.message);

                                bootbox.alert({
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function () {
                                        $("#panelMenuDetailsForm").hide();
                                        $("#ShowAddMenuFormID").show();
                                        OpenMenuDetailsList();
                                    }
                                });

                                //$("#panelMenuDetailsForm").hide();
                                //$("#ShowAddMenuFormID").show();
                                //OpenMenuDetailsList();
                            }
                            else {
                                //bootbox.alert(data.message);
                                bootbox.alert({
                                    //   size: 'small',
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function () {
                                        window.location.href = "/Home/HomePage"
                                    }
                                });
                            }
                            $.unblockUI();
                        },
                        error: function (xhr) {
                            //alert('error');
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                callback: function () {
                                }
                            });
                            $.unblockUI();
                        }
                    });
                }
            }
        });
    }
}