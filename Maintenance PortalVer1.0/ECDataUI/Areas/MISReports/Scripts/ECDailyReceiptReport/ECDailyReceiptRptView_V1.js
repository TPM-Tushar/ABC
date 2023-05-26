
var token = '';
var header = {};
var SelectedSROText;
var txtFromDate;
var txtToDate;
var SroID;
var SelectedDistrictText;
var DistrictID;

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/ECDailyReceiptReport/GetSROOfficeListByDistrictID',
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
       
        if ($.fn.DataTable.isDataTable("#DailyReceiptDetailsTable")) {
            $("#DailyReceiptDetailsTable").DataTable().clear().destroy();
        }

        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        SroID = $("#SROOfficeListID option:selected").val();
        SelectedSROText = $("#SROOfficeListID option:selected").text();
        SelectedDistrictText = $("#DROOfficeListID option:selected").text();
        DistrictID = $("#DROOfficeListID option:selected").val();


        if (txtFromDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
            return;
        }
        else if (txtToDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;

        }
               
        var DailyReceiptRptTable = $('#DailyReceiptDetailsTable').DataTable({
                ajax: {
                    url: '/MISReports/ECDailyReceiptReport/LoadECDailyReceiptRptTable',
                    type: "POST",
                    headers: header,
                    data: {
                        'FromDate': txtFromDate, 'ToDate': txtToDate, 'SroID': SroID, 'DistrictID': DistrictID
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
                                        $("#DailyReceiptDetailsTable").DataTable().clear().destroy();
                                        $("#PDFSPANID").html('');
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
                        //$.unblockUI();
                        unBlockUI();
                    },
                    beforeSend: function () {
                        blockUI('loading data.. please wait...');
                        // Added by SB on 22-3-2019 at 11:06 am
                        var searchString = $('#DailyReceiptDetailsTable_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;
                            if (!regexToMatch.test(searchString)) {
                                $("#DailyReceiptDetailsTable_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    //    $('#menuDetailsListTable_filter input').val('');
                                    DailyReceiptRptTable.search('').draw();
                                    $("#DailyReceiptDetailsTable_filter input").prop("disabled", false);
                                });
                                unBlockUI();
                                return false;
                            }
                        }
                    }
                },
                serverSide: true,
                // pageLength: 100,
                //"scrollX": true,
                //"scrollY": true,
                "scrollY": "300px",
                "scrollCollapse": true,
                bPaginate: true,
                bLengthChange: true,
                // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
                // "pageLength": -1,
                //sScrollXInner: "150%",
                bInfo: true,
                info: true,
                bFilter: false,
                searching: true,
                // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
                // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
                //dom: 'lBfrtip',
                "destroy": true,
                "bAutoWidth": true,
                "bScrollAutoCss": true,
                //buttons: [
                //    {
                //        extend: 'pdf',
                //        text: '<i class="fa fa-file-pdf-o"></i> PDF',
                //        exportOptions: {
                //            columns: ':not(.no-print)'
                //        },
                //        action:
                //            function (e, dt, node, config) {
                //                //this.disable();
                //                window.location = '/MISReports/SaleDeedRevCollection/ExportReportToPDF?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
                //            }
                //    },
                //    {
                //        extend: 'excel',
                //        text: '<i class="fa fa-file-pdf-o"></i> Excel',
                //        exportOptions: {
                //            columns: ':not(.no-print)'
                //        },
                //        action:
                //            function (e, dt, node, config) {
                //                // BECAUSE OF BELOW CODE EXCEL BUTTON IS WORKING ONLY ONE TIME IF WE COMMENT 
                //                // BELOW this.disable(); so it will start working many times
                //                this.disable();
                //                window.location = '/MISReports/SaleDeedRevCollection/ExportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
                //            }
                //    }
                //],

                //columnDefs: [{
                //    targets: [5],
                //    render: $.fn.dataTable.render.number(',', '.', 2)
                //}],
                columnDefs: [
                    //{ orderable: false, targets: [2] },
                    //{ orderable: false, targets: [4] },
                    //{
                    //    orderable: false, targets: [5]

                    //},
                    //{ orderable: false, targets: [6] },
                    //{ orderable: false, targets: [7] }

                ],
                //"language": {
                //    "decimal": ",",
                //    "thousands": "."
                //},
                columns: [

                    { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
                    { data: "ReceiptNo", "searchable": true, "visible": true, "name": "ReceiptNo" },
                    { data: "ReceiptType", "searchable": true, "visible": true, "name": "ReceiptType" },
                    //Added By ShivamB to view this columns in the ECDailyRecieptDetails Grid Table on 07-09-2022
                    { data: "ModeOfPayment", "searchable": true, "visible": true, "name": "ModeOfPayment" },
                    { data: "ChallanNumber", "searchable": true, "visible": true, "name": "ChallanNumber" },
                    //Added By ShivamB to view this columns in the ECDailyRecieptDetails Grid Table on 07-09-2022
                    { data: "ReceiptDate", "searchable": true, "visible": true, "name": "ReceiptDate" },
                    { data: "SROfficeAppNo", "searchable": true, "visible": true, "name": "SROfficeAppNo" },
                    { data: "AppName", "searchable": true, "visible": true, "name": "AppName" },
                    { data: "IssuedBy", "searchable": true, "visible": true, "name": "IssuedBy" },
                    { data: "PeriodOfSearch", "searchable": true, "visible": true, "name": "PeriodOfSearch" },
                    { data: "Amount", "searchable": true, "visible": true, "name": "Amount" },
                ],
                fnInitComplete: function (oSettings, json) {
                    //alert('in fnInitComplete');
                    //$("#PDFSPANID").html(json.PDFDownloadBtn);
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
        //DailyReceiptRptTable.columns.adjust().draw();



    });




});

function PDFDownloadFun() {
    window.location.href = '/MISReports/ECDailyReceiptReport/ExportECDailyReceiptRptToPDF?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + SroID + "&SelectedSRO=" + SelectedSROText + "&SelectedDistrict=" + SelectedDistrictText;
}


function EXCELDownloadFun()
{
    window.location.href = '/MISReports/ECDailyReceiptReport/ExportECDailyReceiptRptToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + SroID + "&SelectedSRO=" + SelectedSROText + "&SelectedDistrict=" + SelectedDistrictText;
}