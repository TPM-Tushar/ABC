//Global variables.
var token = '';
var header = {};


$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $("#rdnIsSROOffice").attr('checked', true).trigger('click');
    $("#rdnIsSROOfficeDtls").attr('checked', true).trigger('click');
    $("#hdnIsDROOffice").val("1");
    $("#SROOfficeListID").prop("disabled", false);
    //$("#dtLastDateForPatchUpdate").prop("disabled", false);
    $("#dtSPExecutionDateTime").prop("disabled", true);


    $('#scriptManagerListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    if ($("#txtAppMajor").val() == 0)
        $("#txtAppMajor").val('');
    if ($("#txtAppMinor").val() == 0)
        $("#txtAppMinor").val('');

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var TodaysDate = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();
    var FromDate;
    var ToDate;
    var SROOfficeID;
    var DROfficeID;
    var ModuleID;

    $('#dtReleaseDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,

    });

    $('#dtLastDateForPatchUpdate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+15d',
        pickerPosition: "bottom-left"

    });

    $('#divReleaseDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        
       maxDate: new Date(),

        pickerPosition: "bottom-left"

    });

    $('#divLastDateForPatchUpdate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+15d',

        pickerPosition: "bottom-left"

    });


    $('#dtnLastDateForPatchUpdate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+15d',

        pickerPosition: "bottom-left"

    });



    $('#dtReleaseDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", TodaysDate);


    $('#dtSPExecutionDateTime').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

    });

    $('#dtSPExecutionDateTime').datepicker({
        format: 'dd/mm/yyyy',
        endDate: '+7d',
        maxDate: new Date(),

    }).datepicker("setDate", TodaysDate);


    $('#divSPExecutionDateTime').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });



    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/SROScriptManager/SROScriptManager/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROOfficeListID').val() },
            datatype: "json",
            type: "GET",
            success: function (data) {
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    $('#SROOfficeListID').empty();
                    $.each(data.SROOfficeList, function (i, SROOfficeList) {
                        SROOfficeList
                        $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });


    $("#btnAddAppVersion").click(function () {

        var officeTypestr;
        if ($('#rdnIsSROOffice').is(':checked'))
            officeTypestr = 'SR Office';
        if ($('#rdnIsDROOffice').is(':checked'))
            officeTypestr = 'DR Office';
        bootbox.confirm({
            message: "Application Version Details will be added for " + officeTypestr,
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                //console.log('This was logged in the callback: ' + result);
                if (result == true) {
                    AddApplicationVersionDetails();
                }
                else {
                    return;
                }
            }
        });

        //

    });

    //$("#projectKey").change(function () {
    //    alert($('option:selected', this).text());
    //});
    LoadAppVersionDetailsList(1);


    $('input[type=radio]').change(function () {

        if (this.value == '1') {
            $("#hdnIsDROOffice").val("1");
            $("#SROOfficeListID").prop("disabled", false);
            //$("#dtLastDateForPatchUpdate").prop("disabled", false);
            $("#dtSPExecutionDateTime").prop("disabled", true);
        }
        if (this.value == '2') {
            $("#hdnIsDROOffice").val("2");
            $("#SROOfficeListID").prop("disabled", true);
            //$("#dtLastDateForPatchUpdate").prop("disabled", true);
            $("#dtSPExecutionDateTime").prop("disabled", false);
        }

        if (this.value == '10') {
            LoadAppVersionDetailsList(10);
        }
        if (this.value == '20') {
            LoadAppVersionDetailsList(20);
        }

    });



});


function LoadAppVersionDetailsList(OfficeTypeID) {

    if ($.fn.DataTable.isDataTable("#AppVersionDtlsID")) {
        $("#AppVersionDtlsID").DataTable().clear().destroy();
    }

    if ($.fn.DataTable.isDataTable("#AppVersionDtlsID")) {
        $("#AppVersionDtlsID").DataTable().clear().destroy();
    }

    var ReScanningDetailsTable = $('#AppVersionDtlsID').DataTable({
        ajax: {
            url: '/SROScriptManager/SROScriptManager/LoadAppVersionDetails',
            type: "POST",
            headers: header,
            data: {
                OfficeTypeID: OfficeTypeID
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            } else {
                                var classToRemove = $('#ToggleIconID').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#scriptManagerListCollapse').trigger('click');
                                $("#AppVersionDtlsID").DataTable().clear().destroy();
                                $("#PDFSPANID").html('');
                                $("#EXCELSPANID").html('');
                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#ToggleIconID').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#scriptManagerListCollapse').trigger('click');
                }
                unBlockUI();
                return json.data;
            },
            error: function () {
                unBlockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');
                var searchString = $('#AppVersionDtlsID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;

                    if (!regexToMatch.test(searchString)) {
                        $("#AppVersionDtlsID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            ReScanningDetailsTable.search('').draw();
                            $("#AppVersionDtlsID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        //"scrollX": true,
        //"scrollY": "300px",
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [
            { "className": "dt-center", "targets": "_all" },
            { width: '5%', targets: [0] },
            { width: '14%', targets: [1] },
            { width: '10%', targets: [2] },
            { width: '7%', targets: [3] },
            { width: '7%', targets: [4] },
            { width: '10%', targets: [5] },
            { width: '10%', targets: [6] },
            { width: '8%', targets: [7] },
            { width: '8%', targets: [8] },
            { width: '10%', targets: [9] },
        ],

        columns: [
            {
                data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo"
            },
            {
                data: "AppName", "searchable": true, "visible": true, "name": "AppName"
            },
            {
                data: "SROOfficeName", "searchable": true, "visible": true, "name": "SROOfficeName"
            },
            {
                data: "AppMajor", "searchable": true, "visible": true, "name": "AppMajor"
            },
            {
                data: "AppMinor", "searchable": true, "visible": true, "name": "AppMinor"
            },
            {
                data: "ReleaseDate", "searchable": true, "visible": true, "name": "ReleaseDate"
            },
            {
                data: "LastDateForPatch", "searchable": true, "visible": true, "name": "LastDateForPatch"
            },
            {
                data: "IsDROOffice", "searchable": true, "visible": true, "name": "IsDROOffice"
            },
            {
                data: "DROOfficeName", "searchable": true, "visible": true, "name": "DROOfficeName"
            },
            {
                data: "SPExecutionDate", "searchable": true, "visible": true, "name": "SPExecutionDate"
            },
            {
                data: "Action", "searchable": true, "visible": true, "name": "Action"
            }
        ],
        fnInitComplete: function (oSettings, json) {
            //$("#PDFSPANID").html(json.PDFDownloadBtn);
            //$("#EXCELSPANID").html(json.ExcelDownloadBtn);
        },
        preDrawCallback: function () {
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {

            //if (ModuleID == 1) {
            //    fnSetColumnVis(2, false);
            //}
            //else if (ModuleID == 2) {
            //    fnSetColumnVis(0, false);
            //    fnSetColumnVis(1, false);
            //}

            unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
            unBlockUI();
        },
    });
}

function EditAppVersionDetails() {

    $.ajax({
        type: 'POST',
        datatype: 'json',
        headers: header,
        url: '/SROScriptManager/SROScriptManager/EditAppVersionDetailsByAppName',
        async: false,
        cache: false,
        data: { ScriptID: id },
        success: function (data) {


            $("#dvAppVersionModalBody").html(data);
            $("#dvAppVersionMain").show();
            $("#dvAppVersionModal").modal('show');

            //if (data.success != 'undefined') {
            //    if (data.success) {

            //        bootbox.alert({
            //            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
            //            callback: function () {

            //                window.location.href = "/SROScriptManager/SROScriptManager/SROScriptManagerView";
            //            }
            //        });
            //    }
            //    else {
            //        bootbox.alert({
            //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
            //        });
            //    }
            //    $.unblockUI();
            //}
            //else {
            //}
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

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

function AddApplicationVersionDetails() {

    //$.validator.unobtrusive.parse(form);

    //if ($("#dvAppVersionForm").valid()) {
    console.log($("#dvAppVersionForm").valid());



    BlockUI();
    $.ajax({
        type: "POST",
        url: "/SROScriptManager/SROScriptManager/AddAppVersionDetails",
        data: $("#dvAppVersionForm").serialize(),
        headers: header,
        success: function (data) {
            if (data.success) {
                bootbox.alert({
                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                    callback: function () {
                        window.location.href = "/SROScriptManager/SROScriptManager/AppVersionView";
                    }
                });
            }
            else {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                });
            }
            $.unblockUI();
        },
        error: function () {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
            $.unblockUI();

        }
    });

}

function EditAppVersionDetails(VersionID, OfficeID, IDO) {


    $.ajax({
        type: 'POST',
        datatype: 'json',
        headers: header,
        url: '/SROScriptManager/SROScriptManager/EditAppVersionDetailsByAppName',
        async: false,
        cache: false,
        data: {
            VersionID: VersionID,
            OfficeCode: OfficeID,
            IDO: IDO
        },
        success: function (data) {
            if (data.message != undefined) {
                bootbox.alert(data.message);
            }
            else {
                $("#dvAppVersionModalBody").html(data);
                $("#dvAppVersionMain").show();
                $("#dvAppVersionModal").modal('show');
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}