var token = '';
var header = {};
//var SelectedSROText;
var txtFromDate;
var txtToDate;
var SroID;
var DistrictID;
//var LogTypeID;
//var SelectedSROText;
//var SelectedDistrictText;
//var SelectedLogTypeText;
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DROOfficeListID').change(function () {
        if ($('#DROOfficeListID').val() === "0") {
            $('#SROOfficeListID').empty();
            $('#SROOfficeListID').append('<option value="0">All</option>');
        }
        else {
            blockUI('Loading data please wait.');

            $.ajax({
                url: '/MISReports/DocumentReferences/GetSROOfficeListByDistrictID',
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
                            //SROOfficeList
                            $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                        });
                    }
                    unBlockUI();
                },
                error: function (xhr) {
                    unBlockUI();
                }
            });
        }
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


    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    $("#SearchBtn").click(function () {
        if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
            $("#AnywhereECTable").DataTable().clear().destroy();
        }
        //To Get Same Data in PDF and Excel
        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        SroID = $("#SROOfficeListID option:selected").val();
        DistrictID = $("#DROOfficeListID option:selected").val();
        //LogTypeID = $("#LogTypeId option:selected").val();
        //SelectedSROText = $("#SROOfficeListID option:selected").text();
        //SelectedDistrictText = $("#DROOfficeListID option:selected").text();
        //SelectedLogTypeText = $("#LogTypeId option:selected").text();

        if ($('#DROOfficeListID').val() < "0" || $('#DROOfficeListID').val() === null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select District.</span>');
            return;
        }
        if ($('#SROOfficeListID').val() < "0" || $('#SROOfficeListID').val() === null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select SRO Name.</span>');
            return;
        }

        if (txtFromDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
            return;
        }
        else if (txtToDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;
        }

        var AnywhereECTable = $('#AnywhereECTable').DataTable({
            ajax: {
                url: '/MISReports/DocumentReferences/DocumentReferencesDetails',
                type: "POST",
                headers: header,
                data: {
                    'FromDate': txtFromDate,
                    'ToDate': txtToDate,
                    'DistrictID': DistrictID,
                    'SroID': SroID
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
                                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                        $('#DtlsSearchParaListCollapse').trigger('click');
                                    $("#AnywhereECTable").DataTable().clear().destroy();
                                    //$("#PDFSPANID").html('');
                                    $("#EXCELSPANID").html('');
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
                { orderable: false, targets: [8] }
                //,
                //{ orderable: false, targets: [9] },
                //{ orderable: false, targets: [10] },
                //{ orderable: false, targets: [11] },
                //{ orderable: false, targets: [12] }
            ],
            columns: [
                { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
                { data: "DocumentType", "searchable": true, "visible": true, "name": "DocumentType" },
                { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
                { data: "ReferenceText", "searchable": true, "visible": true, "name": "ReferenceText" },
                { data: "ThroghType", "searchable": true, "visible": true, "name": "ThroghType" },
                { data: "RevenueOfficerNo_CourtNo", "searchable": true, "visible": true, "name": "RevenueOfficerNo_CourtNo" },
                { data: "RevenueOfficerDate_CourtOrderDate", "searchable": true, "visible": true, "name": "RevenueOfficerDate_CourtOrderDate" },
                { data: "DateofRegistration", "searchable": true, "visible": true, "name": "DateofRegistration" }
                //,
                //{ data: "DateofUpload", "searchable": true, "visible": true, "name": "DateofUpload" },
                //{ data: "ArchivedinCD", "searchable": true, "visible": true, "name": "ArchivedinCD" },
                //{ data: "DocumentDeliveryDate", "searchable": true, "visible": true, "name": "DocumentDeliveryDate" },
                //{ data: "DateofRegistration", "searchable": true, "visible": true, "name": "DateofRegistration" },

            ],
            fnInitComplete: function (oSettings, json) {
                //$("#PDFSPANID").html(json.PDFDownloadBtn);
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



        //tableIndexReports.columns.adjust().draw();
        //AnywhereECTable.columns.adjust().draw();



    });




});

//function PDFDownloadFun() {
//    window.location.href = '/MISReports/DocumentReferences/ExportAnywhereECRptToPDF?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + SroID + "&DistrictID=" + DistrictID + "&LogTypeID=" + LogTypeID + "&SelectedDist=" + SelectedDistrictText + "&SelectedSRO=" + SelectedSROText + "&SelectedLogType=" + SelectedLogTypeText;

//}


function EXCELDownloadFun(DistrictId, SroId, FromDate, ToDate) {

    window.location.href = '/MISReports/DocumentReferences/ExportDocumentReferencesToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SroID=" + SroId + "&DistrictID=" + DistrictId;
}