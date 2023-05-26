var token = '';
var header = {};
var txtFromDate;
var txtToDate;
var DistrictID;
var SroID;
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#SROOfficeListID').change(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        if ($.fn.DataTable.isDataTable("#DROfficeWiseSummaryTableID")) {
            $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
        }
        $("#EXCELSPANID").html('');
    });

    $('#DROOfficeListID').change(function () {
        //alert('1');
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        if ($.fn.DataTable.isDataTable("#DROfficeWiseSummaryTableID")) {
            $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
        }
        $("#EXCELSPANID").html('');

        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/PropertyWthoutImptBypassRDPR/GetSROOfficeListByDistrictID',
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
                    //alert('3');
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
        //alert('2');
    });


    $('#txtFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,
    });

    $('#txtToDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"
    });

    $('#divFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"
    });

    $('#divToDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"
    });

    $('#txtToDate').datepicker({
        format: 'dd/mm/yyyy',
    }).datepicker("setDate", ToDate);

    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", FromDate);

    $('#txtFromDate').focusout(function () {
        //alert('rgegreg');

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        if ($.fn.DataTable.isDataTable("#DROfficeWiseSummaryTableID")) {
            $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
        }
        $("#EXCELSPANID").html('');
    });

    $('#txtToDate').focusout(function () {
        //    alert('fgregregrthrtjhrth');.

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        if ($.fn.DataTable.isDataTable("#DROfficeWiseSummaryTableID")) {
            $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
        }
        $("#EXCELSPANID").html('');
    });


    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $("#SearchBtn").click(function () {
        if ($.fn.DataTable.isDataTable("#DROfficeWiseSummaryTableID")) {
            $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
        }

        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        SroID = $("#SROOfficeListID option:selected").val();
        DistrictID = $("#DROOfficeListID option:selected").val();

        if (txtFromDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
            return;
        }
        else if (txtToDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;
        }
        //alert("asdsa" + txtFromDate + " " + txtToDate + " " + SroID + " " + DistrictID);

        var DROfficeWiseSummaryTableID = $('#DROfficeWiseSummaryTableID').DataTable({
            ajax: {
                url: '/MISReports/PropertyWthoutImptBypassRDPR/LoadReportTable',
                type: "POST",
                headers: header,
                data: { 'FromDate': txtFromDate, 'ToDate': txtToDate, 'DistrictID': DistrictID, 'SroID': SroID },
                dataSrc: function (json) {
                    unBlockUI();
                    if (json.errorMessage != null) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            callback: function () {
                                if (json.serverError != undefined) {
                                    window.location.href = "/Home/HomePage"
                                }
                                else {
                                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                        $('#DtlsSearchParaListCollapse').trigger('click');
                                    $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
                                }
                            }
                        });
                    }
                    else {
                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                            $('#DtlsSearchParaListCollapse').trigger('click');
                    }
                    unBlockUI();
                    return json.data;
                },
                error: function () {
                    //unBlockUI();                   
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    unBlockUI();
                },
                beforeSend: function () {
                    blockUI('Loading data please wait.');
                    var searchString = $('#DROfficeWiseSummaryTableID_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#DROfficeWiseSummaryTableID_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                DROfficeWiseSummaryTableID.search('').draw();
                                $("#DROfficeWiseSummaryTableID_filter input").prop("disabled", false);
                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
            },
            serverSide: true,
            "scrollX": true,
            "scrollY": "300px",
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
                { orderable: false, targets: [0] },
                { orderable: false, targets: [1] },
                { orderable: false, targets: [2] },
                { orderable: false, targets: [3] }
                //{ orderable: false, targets: [4] },
                //{ orderable: false, targets: [5] },
                //{ orderable: false, targets: [6] },
                //{ orderable: false, targets: [7] },
                //{ orderable: false, targets: [8] }
            ],

            columns: [
                { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName" },
                { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
                //{ data: "TotalPropertiesRegistered", "searchable": true, "visible": true, "name": "TotalPropertiesRegistered" },
                //{ data: "Bhoomi", "searchable": true, "visible": true, "name": "Bhoomi" },
                //{ data: "E_Swathu", "searchable": true, "visible": true, "name": "E_Swathu" },
                //{ data: "UPOR", "searchable": true, "visible": true, "name": "UPOR" },
                //{ data: "Mojani", "searchable": true, "visible": true, "name": "Mojani" },
                { data: "Total_Without_Importing", "searchable": true, "visible": true, "name": "Total_Without_Importing" },
            ],
            fnInitComplete: function (oSettings, json) {
                //$("#SROSpanID").html(json.SROName);
                $("#EXCELSPANID").html('');
                $("#EXCELSPANID").html(json.ExcelDownloadBtn);
            },
            preDrawCallback: function () {
                unBlockUI();
            },
            fnRowCallback: function (nRow, aData, iDisplayIndex) {
                unBlockUI();
                return nRow;
            },
            drawCallback: function (oSettings) {
                //responsiveHelper.respond();
                unBlockUI();
            },
        });

    });
   
});

function GetOtherTableDetails(columnName, SROCode) {
    if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
        $("#AnywhereECTable").DataTable().clear().destroy();
    }
    //To Get Same Data in PDF and Excel
    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    DistrictID = $("#DROOfficeListID option:selected").val();

    if (txtFromDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
        return;
    }
    else if (txtToDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
        return;
    }

    if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
        $("#AnywhereECTable").DataTable().clear().destroy();
    }
    var AnywhereECTable = $('#AnywhereECTable').DataTable({
        ajax: {
            url: '/MISReports/PropertyWthoutImptBypassRDPR/OtherTableDetailsBypassRDPR',
            type: "POST",
            headers: header,
            data: {
                'FromDate': txtFromDate,
                'ToDate': txtToDate,
                'DistrictID': DistrictID,
                'columnName': columnName,
                'SROCode': SROCode
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            }
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara1').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse1').trigger('click');
                                $("#AnywhereECTable").DataTable().clear().destroy();
                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara1').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse1').trigger('click');
                }
                unBlockUI();
                return json.data;
            },
            error: function () {
                //unBlockUI();                   
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#AnywhereECTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#AnywhereECTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            AnywhereECTable.search('').draw();
                            $("#AnywhereECTable_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        $('#MasterTableModel').modal('hide');
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        "scrollX": true,
        "scrollY": "300px",
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
            { orderable: false, targets: [0] },
            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] },
            { orderable: false, targets: [3] },
            { orderable: false, targets: [4] },
            { orderable: false, targets: [5] },
            { orderable: false, targets: [6] },
            { orderable: false, targets: [7] },
            { orderable: false, targets: [8] },
            { orderable: false, targets: [9] },
            { orderable: false, targets: [10] }
        ],

        columns: [
            { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
            { data: "DocumentID", "searchable": true, "visible": true, "name": "DocumentID" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
            { data: "NatureOfDocument", "searchable": true, "visible": true, "name": "NatureOfDocument" },
            { data: "VillageName", "searchable": true, "visible": true, "name": "VillageName" },
            { data: "PropertyDetails", "searchable": true, "visible": true, "name": "PropertyDetails" },
            { data: "Executant", "searchable": true, "visible": true, "name": "Executant" },
            { data: "Claimant", "searchable": true, "visible": true, "name": "Claimant" },
            { data: "Reference_AcknowledgementNumber", "searchable": true, "visible": true, "name": "Reference_AcknowledgementNumber" },
            { data: "IntegrationDepartmentName", "searchable": true, "visible": true, "name": "IntegrationDepartmentName" },
            { data: "UploadDate", "searchable": true, "visible": true, "name": "UploadDate" }
        ],
        fnInitComplete: function (oSettings, json) {
            $("#SROSpanID").html('');
            $("#SROSpanID").html(json.SROName);
            $("#EXCELSPANID1").html('');
            $("#EXCELSPANID1").html(json.ExcelDownloadBtn);
        },
        preDrawCallback: function () {
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
            //responsiveHelper.respond();
            unBlockUI();
        },
    });

    //alert(columnName);
    if (columnName == 'B') {
        AnywhereECTable.columns([8]).visible(false);
        AnywhereECTable.columns([10]).visible(false);
    }

    if (columnName == 'G') {
        AnywhereECTable.columns([8]).visible(false);
        AnywhereECTable.columns([10]).visible(false);
    }




    //$('#DtlsSearchParaListCollapse1').on('shown.bs.collapse', function () {
    //    $($.fn.dataTable.tables(true)).DataTable()
    //        .columns.adjust();
    //});9

    // Adjust Table Header but calling two times on first time it need to be changed 1-11-2019 
    var table = $('#AnywhereECTable').DataTable();
    table.columns.adjust().draw();

    //For Displaying SRO name, From date and to date on model popup.   
    $("#FromDateSpanID").html(txtFromDate);
    $("#ToDateSpanID").html(txtToDate);

    switch (columnName) {
        case "B":
            $('#modelHeadingID').text('Total Properties Registered');
            break;
        case "C":
            $('#modelHeadingID').text('Bhoomi');
            break;
        case "D": $('#modelHeadingID').text('E-Swathu');
            break;
        case "E": $('#modelHeadingID').text('UPOR');
            break;
        case "F": $('#modelHeadingID').text('Mojini');
            break;
        case "G": $('#modelHeadingID').text('Total properties registered without importing');
            break;
        default: //alert('default');
    }

    // Show Model Popup
    $('#MasterTableModel').modal('show');
}


function EXCELDownloadFun(FromDate, ToDate, DistrictID, SroID) {
    window.location.href = '/MISReports/PropertyWthoutImptBypassRDPR/ExportToExcel?FromDate=' + FromDate + '&ToDate=' + ToDate + '&DistrictID=' + DistrictID + '&SroID=' + SroID;
}

function ExportToExcelDetails(FromDate, ToDate, SROCode, columnName) {
    window.location.href = '/MISReports/PropertyWthoutImptBypassRDPR/ExportToExcelDetails?FromDate=' + FromDate +
        '&ToDate=' + ToDate + '&SROCode=' + SROCode + '&columnName=' + columnName;
}