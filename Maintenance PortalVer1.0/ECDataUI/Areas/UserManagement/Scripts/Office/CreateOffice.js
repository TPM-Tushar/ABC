
//Global variables.
var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    //Commented by shubham bhagat on 10 - 4 - 2019 requirement change
    //$("#talukaListDiv").hide();
    //if (talukaListShow) {
    //    if ($('#drpOfficeTypeList option:selected').val() == '3') {

    //        $("#talukaListDiv").show();
    //        $('#drpTalukaList').prop('disabled', true);
    //    }
    //}

     //Commented by shubham bhagat on 10 - 4 - 2019 requirement change
    ////24-12-2018 in case of update disable talukadropdown
    //if (IsUpdate) {
    //    $("#drpTalukaList").prop("disabled", false);
    //}






    if ($('#IsAnyFirmRegisteredForCurrentOffice').val() === "True") {
        //alert('vxfd');
        $('#OfficeNametxt').prop('disabled', true);
        $('#ShortNameEId').prop('disabled', true);
        $('#OfficeAddressId').prop('disabled', true);
        $('#drpOfficeTypeList').prop('disabled', true);
        $('#drpOfficeNameList').prop('disabled', true);
        $('#drpDistrictList').prop('disabled', true);
        //Commented by shubham bhagat on 10 - 4 - 2019 requirement change
        //$('#drpTalukaList').prop('disabled', true);

        $('#btnUpdate').hide();
    }

    $("#closeOfficeForm").click(function () {
        window.location.href = "/UserManagement/OfficeDetails/ShowOfficeView";
    });
    $("#btnCancel").click(function () {

        window.location.href = "/UserManagement/OfficeDetails/ShowOfficeView";
        //OpenOfficeList();
        //$("#DivOfficeDetailsWrapper").fadeOut(500);
        //$("#panelNewOffice").fadeIn();
        //document.documentElement.scrollTop = 0;
    });

    //if ($("#drpDistrictList").val() == 0)
    //    $("#drpOfficeNameList").prop('disabled', true);

    if ($("#drpOfficeTypeList").val() == 0)
        $("#drpOfficeNameList").prop('disabled', true);

    //for inserting he record
    $("#btnSave").click(function () {
        var form = $("#OfficeDetailsForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        if ($("#OfficeDetailsForm").valid()) {

            BlockUI();
            $.ajax({
                url: "/UserManagement/OfficeDetails/CreateNewOffice",
                type: "POST",
                data: $("#OfficeDetailsForm").serialize(),
                headers: header,
                success: function (data) {
                    //  bootbox.alert(data.message);
                    if (data.success) {
                        bootbox.alert({
                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                            callback: function () {
                                GetJsonData();
                                OpenOfficeList();
                                $("#DivOfficeDetailsWrapper").fadeOut(500);
                                $("#panelNewOffice").fadeIn();
                                document.documentElement.scrollTop = 0;
                            }
                        });
                        $.unblockUI();

                    } else {

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
                        }


                        $.unblockUI();
                    }
                },
                error: function () {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    $.unblockUI();
                }
            });

        }
        //Form.Valid Check
        //var form = $("#OfficeDetailsForm");
        //form.removeData('validator');
        //form.removeData('unobtrusiveValidation');
        //$.validator.unobtrusive.parse(form);

        //if ($("#OfficeDetailsForm").valid()) {
        //    BlockUI();
        //    $.ajax({
        //        url: "/UserManagement/OfficeDetails/CreateNewOffice",
        //        type: "POST",
        //        data: $("#OfficeDetailsForm").serialize(),
        //        headers: header,
        //        success: function (data) {
        //            //  bootbox.alert(data.message);
        //            if (data.success) {
        //                bootbox.alert({
        //                    //   size: 'small',
        //                    //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
        //                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
        //                    callback: function () {
        //                        GetJsonData();
        //                        OpenOfficeList();
        //                        $("#DivOfficeDetailsWrapper").fadeOut(500);
        //                        $("#panelNewOffice").fadeIn();
        //                        document.documentElement.scrollTop = 0;
        //                    }
        //                });
        //                $.unblockUI();

        //            } else {
        //                bootbox.alert({
        //                    //   size: 'small',
        //                    //   title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
        //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
        //                });
        //                $.unblockUI();
        //            }
        //        },
        //        error: function () {
        //            //bootbox.alert({
        //            //    //   size: 'small',
        //            //    title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
        //            //    message: '<span class="boot-alert-txt">Error in processing</span>',
        //            //});
        //            bootbox.alert({
        //                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
        //                callback: function () {
        //                }
        //            });
        //            $.unblockUI();
        //        }
        //    });
        //} else {
        //    $.unblockUI();
        //    return;
        //}
    });
    $("#btnUpdate").click(function () {

        var form = $("#OfficeDetailsForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        if ($("#OfficeDetailsForm").valid()) {

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

                        var fun = 5;
                        BlockUI();
                        $.ajax({
                            url: "/UserManagement/OfficeDetails/UpdateOffice",
                            type: "POST",
                            data: $("#OfficeDetailsForm").serialize(),
                            headers: header,
                            success: function (data) {
                                if (data.success) {
                                    bootbox.alert({
                                        //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                        callback: function () {
                                            GetJsonData();
                                            OpenOfficeList();
                                            $("#DivOfficeDetailsWrapper").fadeOut(500);
                                            $("#panelNewOffice").fadeIn();
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
                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"><span class="boot-alert-txt">' + data.message + '</span>',

                                        });
                                    }
                                 
                                    $.unblockUI();

                                }
                            },
                            error: function () {
                                bootbox.alert({
                                    //   size: 'small',
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>',
                                });
                                $.unblockUI();

                            }
                        });

                    }
                    else { $.unblockUI(); return; }
                }
            });
        }
        else { $.unblockUI(); return; }
    });
});



// Changes on 15-12-2018 Final Changes in User Management

function OfficeTypeChangeFun() {
   
    if ($('#drpOfficeTypeList option:selected').val() != 0)
    {
        BlockUI();
        $('#drpDistrictList').val(0).change();

        $("#talukaListDiv").hide();

        $.ajax({

            url: '/UserManagement/OfficeDetails/GetParentOfficeNameList?OfficeTypeId=' + $('#drpOfficeTypeList option:selected').val(),

            type: "GET",

            success: function (data) {
                if (data.success) {
                     //Commented by shubham bhagat on 10 - 4 - 2019 requirement change
                    //if ($('#drpOfficeTypeList option:selected').val() == '3') {
                    //    $("#talukaListDiv").show();
                    //    $('#drpTalukaList').prop('disabled', true);
                    //}

                    if ($('#drpOfficeTypeList option:selected').val() == 0) {
                        $('#drpOfficeNameList').prop('disabled', true);
                        $.unblockUI();
                    }
                    else {
                        $('#drpOfficeNameList').prop('disabled', false);
                        $.unblockUI();
                    }

                    $("#drpOfficeNameList").empty();

                    $.each(data.office, function (i, office) {
                        $("#drpOfficeNameList").append('<option value="' + office.Value + '">' +
                            office.Text + '</option>');
                    });
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
            },
            error: function () {
                bootbox.alert({
                    //   size: 'small',
                    //   title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>',
                });
                $.unblockUI();
            }
        });
    }
}

//Commented by shubham bhagat on 10 - 4 - 2019 requirement change
//function autoFillTalukaFun() {
//    if ($('#drpDistrictList option:selected').val() != 0) {
//        if ($('#drpOfficeTypeList option:selected').val() == '3') {
//            $("#talukaListDiv").show();
//            $.ajax({

//                url: '/UserManagement/OfficeDetails/GetTalukasByDistrictID?districtID=' + $('#drpDistrictList option:selected').val(),
//                type: "GET",
//                success: function (data) {
//                    $("#drpTalukaList").empty();
//                    $('#drpTalukaList').prop('disabled', false);
//                    $.each(data.Talukas, function (i, Taluka) {
//                        $("#drpTalukaList").append('<option value="' + Taluka.Value + '">' +
//                            Taluka.Text + '</option>');

//                    });
//                },
//                error: function () {
//                    $.alert("Error in List");
//                }
//            });
//        }
//    }
//}
