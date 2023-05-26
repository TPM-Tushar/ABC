

//Global variables.
var token = '';
var header = {};


//alert("in addEditDetails.js");
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

  //  $('#IsActiveId').prop("disabled", true);//For disabling active status 

    $('#RoleNameId').focus();

    $("#closeRoleDetailsForm").click(function () {
        window.location.href = "/UserManagement/RoleDetails/RoleDetailsList";

        //$("#panelRoleDetailsForm").hide();
        //$("#ShowAddEditRoleDetailsFormID").show();
        //OpenRoleDetailsList();
        //$("#ShowAddMenuFormID").hide();
    });

    $("#btnCancelForAdd").click(function () {
        window.location.href = "/UserManagement/RoleDetails/RoleDetailsList";

        //$("#panelRoleDetailsForm").hide();
        //$("#ShowAddEditRoleDetailsFormID").show();
        //OpenRoleDetailsList();
    });

    $("#btnCancelForEdit").click(function () {
        window.location.href = "/UserManagement/RoleDetails/RoleDetailsList";

        //$("#panelRoleDetailsForm").hide();
        //$("#ShowAddEditRoleDetailsFormID").show();
        //OpenRoleDetailsList();
    });

});

function addRoleDetail() {
    var form = $("#RoleDetailsFormID");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
    if ($("#RoleDetailsFormID").valid()) {
        $.ajax({
            url: "/UserManagement/RoleDetails/AddRoleDetails",
            type: "POST",
            data: $('#RoleDetailsFormID').serialize(),
            dataType: "json",
            headers: header,
            success: function (data) {
                if (data.Status) {

                    //bootbox.alert(data.roleDetailsResponseModel.Message);
                    bootbox.alert({
                        //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                        callback: function () {
                            $("#addEditDivId").hide();
                            //$("#createPartialViewId").hide();
                            //menuTable.destroy();

                            retriveRoleDetailsList();
                            $("#ShowAddEditRoleDetailsFormID").show();
                            OpenRoleDetailsList();
                            $("#panelRoleDetailsForm").hide();
                        }
                    });


                }
                else {
                    //bootbox.alert(data.roleDetailsResponseModel.Message);
                    bootbox.alert({
                        //size: 'small',
                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                        callback: function () {
                            //window.location.href = "/Home/HomePage"
                        }
                    });
                }

            },
            error: function (xhr, status, err) {
                //bootbox.alert("Error " + err);
                bootbox.alert({
                    //size: 'small',
                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                });
            }

        });
    }
    else {
        return;
    }
}

function updateRoleDetail() {
    //alert("edit");

    //error
    var form = $("#RoleDetailsFormID");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    if ($("#RoleDetailsFormID").valid()) {

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

                    $.ajax({
                        url: "/UserManagement/RoleDetails/UpdateRoleDetails",
                        type: "POST",
                        data: $('#RoleDetailsFormID').serialize(),
                        dataType: "json",
                        headers: header,
                        success: function (data) {

                            if (data.Status) {
                                //bootbox.alert(data.roleDetailsResponseModel.Message);
                                //bootbox.alert(data.roleDetailsResponseModel.Message);
                                bootbox.alert({
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                    callback: function () {
                                        $("#addEditDivId").hide();
                                        //$("#editPartialViewId").hide();
                                        retriveRoleDetailsList();
                                        $("#ShowAddEditRoleDetailsFormID").show();
                                        OpenRoleDetailsList();
                                        $("#panelRoleDetailsForm").hide();

                                    }
                                });
                            }
                            else {
                                //bootbox.alert(data.roleDetailsResponseModel.Message);
                                bootbox.alert({
                                    //   size: 'small',
                                    //   title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                    callback: function () {
                                      //  window.location.href = "/Home/HomePage"
                                    }
                                });
                            }

                        },
                        error: function (xhr, status, err) {
                            //bootbox.alert("Error " + err);
                            bootbox.alert({
                                //size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            });
                        }

                    });


                }
                else { return; }
            }
        });
    }
}