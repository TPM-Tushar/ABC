

//Global variables.
var token = '';
var header = {};
var ActionNameIdHidden = "";
var ControllerNameIdHidden = "";
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#controllerList').prop('disabled', true);
    $('#actionList').prop('disabled', true);
    $("#closeControllerAction").click(function () {
        window.location.href = "/UserManagement/ActionDetails/ShowControllerActionData";

        //BlockUI();
        //$("#DivControllerActionWrapper").fadeOut(500);
        //$("#panelNewControllerAction").show();
        //$.unblockUI();
        //OpenControllerActionList();

    });
    $("#btnCancel").click(function () {

        window.location.href = "/UserManagement/ActionDetails/ShowControllerActionData";

        //BlockUI();
        //$("#DivControllerActionWrapper").fadeOut(500);
        //$("#panelNewControllerAction").show();
        //$.unblockUI();
        //OpenControllerActionList();

    });

    $("#listbox").multiselect({
        includeSelectAllOption: true
    });

    $("#ddMenuList").multiselect({
        includeSelectAllOption: true
    });

    $('.multiselect').addClass('minimal');
    $('.multiselect').addClass('btn-sm');

    $("#btnSave").click(function () {


        var form = $("#ControllerActionDetailsForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);

        if ($("#ControllerActionDetailsForm").valid()) {
            BlockUI();
            $.ajax({
                url: "/UserManagement/ActionDetails/InsertControllerActionData",
                type: "post",
                data: $("#ControllerActionDetailsForm").serialize(),
                headers: header,
                success: function (data) {
                    if (data.success) {
                        bootbox.alert({
                            //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;success</span>",
                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                            callback: function (result) {
                                $("#DivControllerActionWrapper").fadeOut(500);
                                $("#panelNewControllerAction").show();
                                LoadControllerActionDetailsData();
                                OpenControllerActionList();
                                $.unblockUI();
                                //window.location.href = data.url;
                            }
                        });
                    }
                    else {
                        if (data.role == true) {
                            $("#RoleError").html("<div><span class='text-danger'><span >Please select Role</span></span><div>");
                            $.unblockUI();
                        }
                        else {
                            bootbox.alert({
                                //   size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                            });
                            $.unblockUI();
                        }
                    }
                },
                error: function () {
                    bootbox.alert({
                        //   size: 'small',
                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing Controller Action details... </span>',
                    });
                    $.unblockUI();

                }
            });
        }
        else { return; }
    });
    $("#btnUpdate").click(function () {
        var form = $("#ControllerActionDetailsForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        if ($("#ControllerActionDetailsForm").valid()) {

            //if ($('#listbox > option:selected').length == 0) {
            //    //alert('in if');
            //    bootbox.alert('Please Select Role');
            //    //var options = $('#listbox > option:selected');
            //    //    if (options.length == 0) {
            //    //        bootbox.alert('Please Select Role');
            //    //        return false;
            //    //    }               
            //}
            //else {
            bootbox.confirm({
                title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                message: "<span class='boot-alert-txt'>Are you sure you want to update?</span>",
                buttons: {
                    cancel: {
                        label: '<i class="fa fa-times"></i> No',
                        className: 'pull-right margin-left-NoBtn'
                    }, confirm: {
                        label: '<i class="fa fa-check"></i> Yes'
                    }
                },
                callback: function (result) {
                    //alert("Controllerlist selected" + $('#controllerList option:selected').val());
                    ControllerNameIdHidden = $('#controllerList option:selected').val();
                    //alert("ControllerNameIdHidden" + ControllerNameIdHidden);
                    $('#ControllerNameId_HiddenId').val(ControllerNameIdHidden);
                    //alert("ActionList selected" + $('#actionList option:selected').val());
                    ActionNameIdHidden = $('#actionList option:selected').val();
                    //alert("ActionNameIdHidden" + ActionNameIdHidden);
                    $('#ActionNameId_HiddenId').val(ActionNameIdHidden);

                    if (result) {

                        $("#stateDropdownLIst").prop("disabled", false);
                        BlockUI();
                        $.ajax({
                            url: "/UserManagement/ActionDetails/UpdateControllerActionData/",
                            type: "POST",
                            data: $("#ControllerActionDetailsForm").serialize(),
                            headers: header,
                            success: function (data) {
                                if (data.success) {
                                    bootbox.alert({
                                        //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                        callback: function (result) {
                                            $("#DivControllerActionWrapper").fadeOut(500);
                                            $("#panelNewControllerAction").show();

                                            // RAFE   LoadControllerActionDetailsData();
                                            $.unblockUI();
                                            // RAFE  OpenControllerActionList();
                                            //window.location.href = data.url;
                                        }
                                    });
                                }
                                else {
                                    bootbox.alert({
                                        //   size: 'small',
                                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + ' </span>',
                                        callback: function () {
                                            window.location.href = "/Home/HomePage"
                                        }
                                    });
                                    $.unblockUI();
                                }
                            },
                            error: function () {
                                bootbox.alert({
                                    //   size: 'small',
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing Controller Action details... </span>',
                                });
                                $.unblockUI();
                            }
                        });

                    }
                }
            });
            // }
        }
        else { return; }
    });

    $("#btnUpdateRoleAuth").click(function () {
        updateRoleActionAuth();
    });

    $("#btnCancelRoleAuth").click(function () {
        $('#divViewRoleModal').modal('hide');

    });

    $("#closeRoleAuthPopup").click(function () {
        $('#divViewRoleModal').modal('hide');

    });



});



function updateRoleActionAuth() {

    var form = $("#FormRoleActionAuth");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    //if ($('#listbox > option:selected').length == 0) {
    //    //alert('in if');
    //    bootbox.alert('Please Select Role');
    //    //var options = $('#listbox > option:selected');
    //    //    if (options.length == 0) {
    //    //        bootbox.alert('Please Select Role');
    //    //        return false;
    //    //    }               
    //}
    //else {
        bootbox.confirm({
            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
            message: "<span class='boot-alert-txt'>Are you sure you want to update?</span>",
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

                    $("#stateDropdownLIst").prop("disabled", false);
                    BlockUI();
                    $.ajax({
                        url: "/UserManagement/ActionDetails/UpdateRoleActionAuth/",
                        type: "POST",
                        data: $("#FormRoleActionAuth").serialize(),
                        headers: header,
                        success: function (data) {
                            if (data.success) {
                                bootbox.alert({
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function (result) {
                                        
                                           LoadControllerActionDetailsData();
                                        $.unblockUI();
                                        // RAFE  OpenControllerActionList();
                                        //window.location.href = data.url;
                                        $('#divViewRoleModal').modal('hide');

                                    }
                                });
                            }
                            else {
                                bootbox.alert({
                                    //   size: 'small',
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + ' </span>',
                                    callback: function () {
                                        window.location.href = "/Home/HomePage"
                                    }
                                });
                                $.unblockUI();
                            }
                        },
                        error: function () {
                            bootbox.alert({
                                //   size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing Controller Action details... </span>',
                            });
                            $.unblockUI();
                        }
                    });

                }
            }
        });
   // }



}

function AreaChangeFun() {
    if ($('#areaList option:selected').val() == "0") {
        bootbox.alert("Please Select Area Name");
        $('#controllerList').prop('disabled', true);
        $("#controllerList").empty();
        $('#actionList').prop('disabled', true);
        $("#actionList").empty();
    }
    else {
        $.ajax({

            url: '/UserManagement/ActionDetails/GetControllerList?AreaName=' + $('#areaList option:selected').val(),

            type: "GET",

            success: function (data) {
                if (data.response != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.response + '</span>',
                        callback: function () {
                            window.location.href = "/Home/HomePage"
                        }
                    });
                }
                else {
                    $("#controllerList").empty();
                    $("#actionList").empty();
                    if ($('#areaList option:selected').val() != '0') {


                        $.each(data.ControllerList, function (i, ControllerList) {
                            $("#controllerList").append('<option value="' + ControllerList.Value + '">' +
                                ControllerList.Text + '</option>');
                            $('#controllerList').prop('disabled', false);
                            $('#actionList').prop('disabled', true);
                        });
                    }
                    else {
                        $("#controllerList").empty();
                        $("#actionList").empty();
                        $('#controllerList').prop('disabled', true);
                        $('#actionList').prop('disabled', true);
                    }
                }
            },
            error: function () {
                $.alert("Error in Processing");
            }
        });
    }

}

function ControllerChangeFun() {
    //alert('ControllerChangeFun');
    if ($('#controllerList option:selected').val() == "0") {
        bootbox.alert("Please Select Controller Name");
        //$('#controllerList').prop('disabled', true);
        //$("#controllerList").empty();
        $('#actionList').prop('disabled', true);
        $("#actionList").empty();
    }
    else {
        //alert($('#controllerList option:selected').val());
        ControllerNameIdHidden = $('#controllerList option:selected').val();
        //alert(ControllerNameIdHidden);
        $.ajax({


            url: '/UserManagement/ActionDetails/GetActionList',

            type: "GET",
            data: { "AreaName": $('#areaList option:selected').val(), "controllerName": $('#controllerList option:selected').val() },
            success: function (data) {
                if (data.response != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.response + '</span>',
                        callback: function () {
                            window.location.href = "/Home/HomePage"
                        }
                    });
                }
                else {
                    $("#actionList").empty();
                    if ($('#controllerList option:selected').val() != '0') {

                        $.each(data.ActionList, function (i, ActionList) {
                            $("#actionList").append('<option value="' + ActionList.Value + '">' +
                                ActionList.Text + '</option>');
                            $('#actionList').prop('disabled', false);
                        });
                    }
                    else {
                        $('#actionList').prop('disabled', true);
                    }
                }
            },
            error: function () {
                $.alert("Error in Processing");
            }
        });
    }

}

function ActionChangeFun() {
    //alert($('#actionList option:selected').val());
    ActionNameIdHidden = $('#actionList option:selected').val();
    //alert(ActionNameIdHidden);
}
