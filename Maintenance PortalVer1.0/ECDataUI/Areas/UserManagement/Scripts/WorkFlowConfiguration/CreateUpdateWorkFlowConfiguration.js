//Global variables.
var token = '';
var header = {};

// Added by Shubham Bhagat on 17-12-2018
var HiddenFieldTosetToRoleId = 0;
var HiddenFieldTosetFromRoleId = 0;

var HiddenFieldTosetActionIdReverseOfficeConfiguration = 0;
var HiddenFieldTosetToRoleIdReverseOfficeConfiguration = 0;
var HiddenFieldTosetFromRoleIdReverseOfficeConfiguration = 0;
var HiddenFieldTosetOfficeIdReverseOfficeConfiguration = 0;
var HiddenFieldTosetServiceIdReverseOfficeConfiguration = 0;
var HiddenFieldTosetIsActiveReverseOfficeConfiguration = false;

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#OfficeReverseConfigurationDivIdTwo').hide();

    $("#closeWorkFlowConfigurationForm").click(function () {

        window.location.href = "/UserManagement/WorkFlowConfigurationDetails/ShowWorkFlowConfigurationDetails";
        //OpenOfficeList();
        //$("#DivWorkFlowConfigurationDetailsWrapper").fadeOut(500);
        //$("#panelNewWorkFlowConfiguration").fadeIn();

        //document.documentElement.scrollTop = 0;
    });
    $("#btnCancel").click(function () {
        window.location.href = "/UserManagement/WorkFlowConfigurationDetails/ShowWorkFlowConfigurationDetails";

        //$("ul#crumbs li:nth-child(3)").trigger('click');
        //OpenOfficeList();
        //$("#DivWorkFlowConfigurationDetailsWrapper").fadeOut(500);
        //$("#panelNewWorkFlowConfiguration").fadeIn();
        //document.documentElement.scrollTop = 0;
    });

    if ($("#drpDistrictList").val() == 0)
        $("#drpOfficeNameList").prop('disabled', true);

    //for inserting he record
    $("#btnSave").click(function () {
        if ($('#OfficeId').val() == 0) { bootbox.alert('Please Select office.'); }
        else {
            //alert('from role' + $('#FromRoleId_Hidden').val());
            //alert('to role' + $('#FromRoleId_Hidden').val());
            //alert('Office Id' + $('#OfficeId').val());
            $('#FromRoleId_Hidden').val(HiddenFieldTosetFromRoleId);
            $('#ToRoleId_Hidden').val(HiddenFieldTosetToRoleId);

            if ($('#ToAddReverseOfficeConfigurationId').val()) {
                $('#ActionId_ReverseofficeConfiguration_HiddenId').val(HiddenFieldTosetActionIdReverseOfficeConfiguration);
                $('#FromRoleId_ReverseofficeConfiguration_HiddenId').val(HiddenFieldTosetFromRoleIdReverseOfficeConfiguration);
                $('#ToRoleId_ReverseofficeConfiguration_HiddenId').val(HiddenFieldTosetToRoleIdReverseOfficeConfiguration);
                $('#OfficeId_ReverseofficeConfiguration_HiddenId').val(HiddenFieldTosetOfficeIdReverseOfficeConfiguration);
                $('#ServiceId_ReverseofficeConfiguration_HiddenId').val(HiddenFieldTosetServiceIdReverseOfficeConfiguration);
                $('#IsActive_ReverseofficeConfiguration_HiddenId').val(HiddenFieldTosetIsActiveReverseOfficeConfiguration);
            }

            //alert('from role' + $('#FromRoleId_Hidden').val());
            //alert('to role' + $('#FromRoleId_Hidden').val());
            var form = $("#WorkFlowConfigurationDetailsForm");
            // form.removeData('validator');
            //form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);

            if ($("#WorkFlowConfigurationDetailsForm").valid()) {

                $.ajax({
                    url: "/UserManagement/WorkFlowConfigurationDetails/CreateNewWorkFlowConfiguration",
                    type: "POST",
                    data: $("#WorkFlowConfigurationDetailsForm").serialize(),
                    headers: header,
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
                                    $("#DivWorkFlowConfigurationDetailsWrapper").fadeOut(500);
                                    $("#panelNewWorkFlowConfiguration").fadeIn();
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
                    $('#FromRoleId_Hidden').val(HiddenFieldTosetFromRoleId);
                    $('#ToRoleId_Hidden').val(HiddenFieldTosetToRoleId);
                    var form = $("#WorkFlowConfigurationDetailsForm");
                    form.removeData('validator');
                    form.removeData('unobtrusiveValidation');
                    $.validator.unobtrusive.parse(form);
                    var fun = 5;
                    if ($("#WorkFlowConfigurationDetailsForm").valid()) {
                        $.ajax({
                            url: "/UserManagement/WorkFlowConfigurationDetails/UpdateWorkFlowConfiguration",
                            type: "POST",
                            data: $("#WorkFlowConfigurationDetailsForm").serialize(),
                            headers: header,
                            success: function (data) {
                                if (data.success) {
                                    bootbox.alert({
                                        //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                        callback: function () {
                                            GetJsonData();
                                            OpenOfficeList();
                                            $("#DivWorkFlowConfigurationDetailsWrapper").fadeOut(500);
                                            $("#panelNewWorkFlowConfiguration").fadeIn();
                                            document.documentElement.scrollTop = 0;
                                        }
                                    });
                                }
                                else {
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
                    }
                    else { return; }
                }
            }
        });

    });
    // Added by Shubham Bhagat on 4-12-2018
    $("#ActionDropDwonList").change(function () { SetFromRoleAndToRole(); });

    $('#ActionDropDwonList').change(function () {

        if (vIsUpdate == 1) {

        }
        else {

            document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
            $('#ToAddReverseOfficeConfigurationId').val(false);
            $('#OfficeReverseConfigurationDivIdTwo').hide();
            //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());
        }
    });
    $('#FromRoleId').change(function () {
        if (vIsUpdate == 1) {

        }
        else {

            document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
            $('#ToAddReverseOfficeConfigurationId').val(false);
            $('#OfficeReverseConfigurationDivIdTwo').hide();
            //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());
        }
    });
    $('#ToRoleId').change(function () {
        if (vIsUpdate == 1) {

        }
        else {

            document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
            $('#ToAddReverseOfficeConfigurationId').val(false);
            $('#OfficeReverseConfigurationDivIdTwo').hide();
            //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());
        }
    });
    $('#OfficeId').change(function () {
        if (vIsUpdate == 1) {

        }
        else {

            document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
            $('#ToAddReverseOfficeConfigurationId').val(false);
            $('#OfficeReverseConfigurationDivIdTwo').hide();
            //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());
        }
    });
    $('#ServiceId').change(function () {
        if (vIsUpdate == 1) {

        }
        else {

            document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
            $('#ToAddReverseOfficeConfigurationId').val(false);
            $('#OfficeReverseConfigurationDivIdTwo').hide();
            //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());
        }
    });
    $('#IsActiveId').click(function () {
        if (vIsUpdate == 1) {

        }
        else {

            if ($("#IsActiveId").is(":checked")) {
                document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
                $('#ToAddReverseOfficeConfigurationId').val(false);
                $('#OfficeReverseConfigurationDivIdTwo').hide();
                //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());
            }
            else {
                document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
                $('#ToAddReverseOfficeConfigurationId').val(false);
                $('#OfficeReverseConfigurationDivIdTwo').hide();
                //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());
            }
        }
    });
    $('#btnReset').click(function () {
        // alert('in reset');
        $('#ToAddReverseOfficeConfigurationId').val(false);
        $('#OfficeReverseConfigurationDivIdTwo').hide();
    });

    // 26-12-2018
    if (vIsUpdate == 1) {
        //alert('From Role Id :' + $('#FromRoleId').val());
        //alert('To Role Id :' + $('#ToRoleId').val());
        HiddenFieldTosetFromRoleId = $('#FromRoleId').val();
        HiddenFieldTosetToRoleId = $('#ToRoleId').val();
        //alert('HiddenFieldTosetFromRoleId :' + HiddenFieldTosetFromRoleId);
        //alert('HiddenFieldTosetToRoleId :' + HiddenFieldTosetToRoleId);
        $('#FromRoleId').prop("disabled", true);
        $('#ToRoleId').prop("disabled", true);
    }
});


function onchangeToAdd() {
    //alert(" in onChangeToAdd")
    //var a = $("#ToAddReverseOfficeConfiguration2").is(":checked");
    //alert($("#ToAddReverseOfficeConfiguration2").is(":checked"));
    if ($("#ToAddReverseOfficeConfiguration2").is(":checked")) {
        // alert($("#ToAddReverseOfficeConfiguration2").is(":checked"))
        //alert($('#ActionDropDwonList').val())
        if ($('#ActionDropDwonList').val() != "0") {
            if ($('#FromRoleId').val() != "0") {
                if ($('#ToRoleId').val() != "0") {
                    if ($('#OfficeId').val() != "0") {
                        if ($('#ServiceId').val() != "0") {
                            $('#OfficeReverseConfigurationDivIdTwo').show();
                            ToAddReverseOfficeConfiguration();
                            //alert("in if")
                        }
                        else {
                            document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
                            bootbox.alert('Please Select Service / Module');
                        }
                    }
                    else {
                        document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
                        bootbox.alert('Please Select Office ');
                    }
                }
                else {
                    document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
                    bootbox.alert('Please Select To Role ');
                }
            }
            else {
                document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
                bootbox.alert('Please Select From Role ');
            }
        }
        else {
            document.getElementById("ToAddReverseOfficeConfiguration2").checked = false;
            bootbox.alert('Please Select Action ');
        }

    }
    else {
        // Code To Hide Reverse Office Congiguration To Be added Here
        //alert($("#ToAddReverseOfficeConfiguration2").is(":checked"))
        $('#OfficeReverseConfigurationDivIdTwo').hide();
        $('#ToAddReverseOfficeConfigurationId').val(false);
        //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());

    }
}

function ToAddReverseOfficeConfiguration() {
    //alert("In function");

    if ($('#ActionDropDwonList').val() == 1) {

        //---------------------------- To Set Reverse Office Configuration----------------------------------------
        $('#ActionId_ReverseofficeConfigurationId').val(2).change();
        //To set From Role
        $('#FromRoleId_ReverseofficeConfigurationId').val(1).change();
        //To set To Role
        $('#ToRoleId_ReverseofficeConfigurationId').val(2).change();

        $('#OfficeReverseOfficeConfigurationId').val($('#OfficeId').val()).change();

        $('#ServiceReverseOfficeConfigurationId').val($('#ServiceId').val()).change();

        if ($("#IsActiveId").is(":checked")) {
            // Commented By Shubham Bhagat on 28-12-2018
            //document.getElementById("IsActiveReverseOfficeConfigurationId").checked = true;

            // Added By Shubham Bhagat on 28-12-2018
            // To Display value on UI 
            document.getElementById("IsActiveReverseOfficeConfigurationId").checked = true;
            // To Origionally set value on Javascript level 
            $('#IsActiveReverseOfficeConfigurationId').val(true);
        }
        else {
            // Commented By Shubham Bhagat on 28-12-2018
            //document.getElementById("IsActiveReverseOfficeConfigurationId").checked = false;

            // Added By Shubham Bhagat on 28-12-2018
            // To Display value on UI 
            document.getElementById("IsActiveReverseOfficeConfigurationId").checked = false;
            // To Origionally set value on Javascript level
            $('#IsActiveReverseOfficeConfigurationId').val(false);

        }
        //alert($('#IsActive').val());
        //alert($("#IsActive").is(":checked"))
        // $('#IsActiveReverseOfficeConfigurationId').val($('#IsActive').val());

        //$('#IsActiveReverseOfficeConfigurationId').val()
        // $('#IsActiveReverseOfficeConfigurationId').prop("checked", $('#IsActive').val());

        //document.getElementById("IsActiveReverseOfficeConfigurationId").checked = true;//$('#IsActive').val();

        //-------------- To Set Value of Reverse Office Configuration to Javascript hidden field----------------------------------------
        HiddenFieldTosetActionIdReverseOfficeConfiguration = $('#ActionId_ReverseofficeConfigurationId').val();
        HiddenFieldTosetFromRoleIdReverseOfficeConfiguration = $('#FromRoleId_ReverseofficeConfigurationId').val();
        HiddenFieldTosetToRoleIdReverseOfficeConfiguration = $('#ToRoleId_ReverseofficeConfigurationId').val();
        HiddenFieldTosetOfficeIdReverseOfficeConfiguration = $('#OfficeReverseOfficeConfigurationId').val();
        HiddenFieldTosetServiceIdReverseOfficeConfiguration = $('#ServiceReverseOfficeConfigurationId').val();
        HiddenFieldTosetIsActiveReverseOfficeConfiguration = $('#IsActiveReverseOfficeConfigurationId').val();


        //------------To Disable all Reverse Office Configuration
        $('#ActionId_ReverseofficeConfigurationId').prop("disabled", true);
        $('#FromRoleId_ReverseofficeConfigurationId').prop("disabled", true);
        $('#ToRoleId_ReverseofficeConfigurationId').prop("disabled", true);
        $('#OfficeReverseOfficeConfigurationId').prop("disabled", true);
        $('#ServiceReverseOfficeConfigurationId').prop("disabled", true);
        $('#IsActiveReverseOfficeConfigurationId').prop("disabled", true);

        $('#ToAddReverseOfficeConfigurationId').val(true);
        //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());
    }
    else if ($('#ActionDropDwonList').val() == 2) {
        //alert('1');
        //alert("in else")
        $('#ActionId_ReverseofficeConfigurationId').val(1).change();
        //To set From Role
        $('#FromRoleId_ReverseofficeConfigurationId').val(2).change();
        //To set To Role
        $('#ToRoleId_ReverseofficeConfigurationId').val(1).change();

        $('#OfficeReverseOfficeConfigurationId').val($('#OfficeId').val()).change();

        $('#ServiceReverseOfficeConfigurationId').val($('#ServiceId').val()).change();

        if ($("#IsActiveId").is(":checked")) {
            // Commented By Shubham Bhagat on 28-12-2018
            //document.getElementById("IsActiveReverseOfficeConfigurationId").checked = true;

            // Added By Shubham Bhagat on 28-12-2018
            // To Display value on UI 
            document.getElementById("IsActiveReverseOfficeConfigurationId").checked = true;
            // To Origionally set value on Javascript level 
            $('#IsActiveReverseOfficeConfigurationId').val(true);
        }
        else {
            // Commented By Shubham Bhagat on 28-12-2018
            //document.getElementById("IsActiveReverseOfficeConfigurationId").checked = false;

            // Added By Shubham Bhagat on 28-12-2018
            // To Display value on UI 
            document.getElementById("IsActiveReverseOfficeConfigurationId").checked = false;
            // To Origionally set value on Javascript level
            $('#IsActiveReverseOfficeConfigurationId').val(false);

        }
        //alert($('#IsActive').val()+'vfdgy');
        //$('#IsActiveReverseOfficeConfigurationId').val($('#IsActive').val());
        //$('#IsActiveReverseOfficeConfigurationId').val()=$('#IsActive').val();

        HiddenFieldTosetActionIdReverseOfficeConfiguration = $('#ActionId_ReverseofficeConfigurationId').val();
        HiddenFieldTosetFromRoleIdReverseOfficeConfiguration = $('#FromRoleId_ReverseofficeConfigurationId').val();
        HiddenFieldTosetToRoleIdReverseOfficeConfiguration = $('#ToRoleId_ReverseofficeConfigurationId').val();
        HiddenFieldTosetOfficeIdReverseOfficeConfiguration = $('#OfficeReverseOfficeConfigurationId').val();
        HiddenFieldTosetServiceIdReverseOfficeConfiguration = $('#ServiceReverseOfficeConfigurationId').val();
        HiddenFieldTosetIsActiveReverseOfficeConfiguration = $('#IsActiveReverseOfficeConfigurationId').val();

        $('#ActionId_ReverseofficeConfigurationId').prop("disabled", true);
        $('#FromRoleId_ReverseofficeConfigurationId').prop("disabled", true);
        $('#ToRoleId_ReverseofficeConfigurationId').prop("disabled", true);
        $('#OfficeReverseOfficeConfigurationId').prop("disabled", true);
        $('#ServiceReverseOfficeConfigurationId').prop("disabled", true);
        $('#IsActiveReverseOfficeConfigurationId').prop("disabled", true);

        $('#ToAddReverseOfficeConfigurationId').val(true);
        //alert("ToAddReverseOfficeConfigurationId : " + $('#ToAddReverseOfficeConfigurationId').val());
    }


}


// Added by Shubham Bhagat on 4-12-2018
function SetFromRoleAndToRole() {

    if ($('#ActionDropDwonList').val() == 1) {
        //alert('1');
        //To set From Role
        $('#FromRoleId').val(2).change();
        //To set To Role
        $('#ToRoleId').val(1).change();

        // Added on 17-12-2018
        // Added on 17-12-2018
        //alert(HiddenFieldTosetFromRoleId);
        //alert(HiddenFieldTosetToRoleId);

        HiddenFieldTosetFromRoleId = 2;
        HiddenFieldTosetToRoleId = 1;

        //alert(HiddenFieldTosetFromRoleId);
        //alert(HiddenFieldTosetToRoleId);

        $('#FromRoleId').prop("disabled", true);
        $('#ToRoleId').prop("disabled", true);
    }

    if ($('#ActionDropDwonList').val() == 2) {
        //alert('2');
        //To set From Role
        $('#FromRoleId').val(1).change();

        //To set To Role
        $('#ToRoleId').val(2).change();

        // Added on 17-12-2018
        //alert(HiddenFieldTosetFromRoleId);
        //alert(HiddenFieldTosetToRoleId);

        HiddenFieldTosetFromRoleId = 1;
        HiddenFieldTosetToRoleId = 2;

        //alert(HiddenFieldTosetFromRoleId);
        //alert(HiddenFieldTosetToRoleId);

        $('#FromRoleId').prop("disabled", true);
        $('#ToRoleId').prop("disabled", true);

    }
    if ($('#ActionDropDwonList').val() == 0) {
        //alert('0');
        //To set From Role
        $('#FromRoleId').val(0).change();
        //To set To Role
        $('#ToRoleId').val(0).change();
    }
}