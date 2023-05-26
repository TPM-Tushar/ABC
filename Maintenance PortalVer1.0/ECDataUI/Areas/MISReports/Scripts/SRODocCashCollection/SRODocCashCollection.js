var FromDate;
var ToDate;
var SROOfficeListID;
var DROfficeID;

//Global variables.
var token = '';
var header = {};
var SelectedDistrict;
var SelectedSRO;

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    //$("#AmountID").val(' ');
    $('#DROOfficeListID').focus();
    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
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



    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/SRODocCashCollection/GetSROOfficeListByDistrictID',
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
                //window.location.href = '/Error/SessionExpire';
                unBlockUI();
            }
        });
    });
    var SROOfficeListID = $("#SROOfficeListID option:selected").val();
    var DROOfficeListID = $("#DROOfficeListID option:selected").val();

    //$('#DROOfficeListID').trigger("change");

    //$("#SearchBtn").click(function () {

    //    if ($.fn.DataTable.isDataTable("#IndexIIReportsID")) {
    //        $("#IndexIIReportsID").DataTable().clear().destroy();
    //    }
    //    FromDate = $("#txtFromDate").val();
    //    ToDate = $("#txtToDate").val();
    //    SROOfficeListID = $("#SROOfficeListID option:selected").val();
    //    DROfficeID = $("#DROOfficeListID option:selected").val();
    //    SelectedDistrict = $("#DROOfficeListID option:selected").text();
    //    SelectedSRO = $("#SROOfficeListID option:selected").text();
    //    //if ($('#DROOfficeListID').val() < "0") {
    //    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any District.</span>');
    //    //    return;
    //    //}
    //    //else if ($('#SROOfficeListID').val() < "0") {
    //    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any SRO</span>');
    //    //    return;

    //    //}
    //    var tableIndexReports = $('#IndexIIReportsID').DataTable({
    //        ajax: {
    //            url: '/MISReports/SRODocCashCollection/GetSRODocCashCollectionReportsDetails',
    //            type: "POST",
    //            headers: header,

    //            data: {
    //                'fromDate': FromDate, 'ToDate': ToDate, 'SROOfficeListID': SROOfficeListID, 'DROfficeID': DROfficeID
    //            },
    //            dataSrc: function (json) {
    //                unBlockUI();
    //                unBlockUI();
    //                if (json.errorMessage != null) {

    //                    bootbox.alert({
    //                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
    //                        callback: function () {
    //                            if (json.serverError != undefined) {
    //                                window.location.href = "/Home/HomePage"
    //                            } else {
    //                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
    //                                    $('#DtlsSearchParaListCollapse').trigger('click');
    //                                $("#IndexIIReportsID").DataTable().clear().destroy();
    //                                //$("#PDFSPANID").html('');
    //                                $("#EXCELSPANID").html('');
    //                            }
    //                        }
    //                    });

    //                }
    //                else {
    //                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
    //                        $('#DtlsSearchParaListCollapse').trigger('click');
    //                }
    //                unBlockUI();
    //                return json.data;


    //            },
    //            error: function () {
    //                //  unBlockUI();
    //                //window.location.href = '/Error/SessionExpire';
    //                bootbox.alert({
    //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
    //                    callback: function () {
    //                    }
    //                });
    //                $.unblockUI();
    //            },
    //            beforeSend: function () {
    //                blockUI('loading data.. please wait...');
    //                // Added by SB on 22-3-2019 at 11:06 am
    //                var searchString = $('#IndexIIReportsID_filter input').val();
    //                if (searchString != "") {
    //                    var regexToMatch = /^[^<>]+$/;
    //                    if (!regexToMatch.test(searchString)) {
    //                        $("#IndexIIReportsID_filter input").prop("disabled", true);
    //                        bootbox.alert('Please enter valid Search String ', function () {
    //                            //    $('#menuDetailsListTable_filter input').val('');

    //                            tableIndexReports.search('').draw();
    //                            $("#IndexIIReportsID_filter input").prop("disabled", false);

    //                        });
    //                        unBlockUI();
    //                        return false;
    //                    }
    //                }
    //            }
    //        },



    //        "language": {
    //            "decimal": ",",
    //            "thousands": "."
    //        },
    //        serverSide: true,
    //        // pageLength: 100,
    //        "scrollX": true,
    //        "scrollY": "300px",
    //        scrollCollapse: true,
    //        bPaginate: true,
    //        bLengthChange: true,
    //        // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
    //        // "pageLength": -1,
    //        //sScrollXInner: "150%",
    //        bInfo: true,
    //        info: true,
    //        bFilter: false,
    //        searching: true,
    //        // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
    //        // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
    //        //dom: 'lBfrtip',
    //        "destroy": true,
    //        "bAutoWidth": true,
    //        "bScrollAutoCss": true,



    //        //buttons: [
    //        //    {
    //        //        extend: 'pdf',
    //        //        text: '<i class="fa fa-file-pdf-o"></i> PDF',
    //        //        exportOptions: {
    //        //            columns: ':not(.no-print)'
    //        //        },
    //        //        action:
    //        //            function (e, dt, node, config) {
    //        //                //this.disable();
    //        //                window.location = '/MISReports/SRODocCashCollection/ExportReportToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&DROfficeID=" + DROfficeID 
    //        //            }


    //        //    },
    //        //    {
    //        //        extend: 'excel',
    //        //        text: '<i class="fa fa-file-pdf-o"></i> Excel',
    //        //        exportOptions: {
    //        //            columns: ':not(.no-print)'
    //        //        },
    //        //        action:
    //        //            function (e, dt, node, config) {
    //        //                this.disable();
    //        //                window.location = '/MISReports/SRODocCashCollection/ExportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&DROfficeID=" + DROfficeID
    //        //            }


    //        //    }

    //        //],


    //        columnDefs: [

    //            { orderable: false, targets: [0] },
    //            { orderable: false, targets: [1] },
    //            { orderable: false, targets: [2] },
    //            { orderable: false, targets: [3] },
    //            { orderable: false, targets: [4] },
    //            { orderable: false, targets: [5] },
    //            { orderable: false, targets: [6] },
    //            { orderable: false, targets: [7] },




    //        ],

    //        columns: [
    //            { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
    //            { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
    //            { data: "DocumentNumber", "searchable": true, "visible": true, "name": "DocumentNumber" },

    //            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
    //            { data: "PresentDatetime", "searchable": true, "visible": true, "name": "PresentDatetime" },
    //            { data: "StampDuty", "searchable": true, "visible": true, "name": "StampDuty" },
    //            { data: "ReceiptFee", "searchable": true, "visible": true, "name": "ReceiptFee" },
    //            { data: "Total", "searchable": true, "visible": true, "name": "Total" },
    //            { data: "ReceiptNumber", "searchable": true, "visible": true, "name": "ReceiptNumber" }


    //        ],
    //        fnInitComplete: function (oSettings, json) {
    //            //alert('in fnInitComplete');
    //            //$("#PDFSPANID").html(json.PDFDownloadBtn);
    //            $("#EXCELSPANID").html(json.ExcelDownloadBtn);
    //        },



    //        preDrawCallback: function () {

    //            unBlockUI();
    //        },
    //        fnRowCallback: function (nRow, aData, iDisplayIndex) {


    //            unBlockUI();
    //            return nRow;
    //        },
    //        drawCallback: function (oSettings) {
    //            //responsiveHelper.respond();
    //            unBlockUI();
    //        },
    //    });
    //});

    $("#excelDownload").click(function () {

        FromDate = $("#txtFromDate").val();
        ToDate = $("#txtToDate").val();
        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        DROfficeID = $("#DROOfficeListID option:selected").val();
        SelectedDistrict = $("#DROOfficeListID option:selected").text();
        SelectedSRO = $("#SROOfficeListID option:selected").text();
        //blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/SRODocCashCollection/ValidateSearchParameters',
            data: { "fromDate": FromDate, "ToDate": ToDate, "SROOfficeListID": SROOfficeListID, "DROfficeID": DROfficeID },
            type: "GET",
            success: function (data) {
                if (data.success == true) {
                    window.location.href = '/MISReports/SRODocCashCollection/ExportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&SelectedDistrict=" + SelectedDistrict + "&SelectedSRO=" + SelectedSRO;
                    //unBlockUI();
                }
                else {
                    //unBlockUI();
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            //window.location.href = "/MISReports/SRODocCashCollection/SRODocCashCollectionView"

                        });
                }
            },
            error: function (xhr) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Error in processing</span>',
                    function () {
                        //window.location.href = "/MISReports/SRODocCashCollection/SRODocCashCollectionView"

                    });

                //window.location.href = '/Error/SessionExpire';
                //unBlockUI();
            }
        });
    });
});

//function PDFDownloadFun(FromDate, ToDate, SROOfficeListID, DROfficeID) {

//    window.location.href = '/MISReports/SRODocCashCollection/ExportReportToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&SelectedDistrict=" + SelectedDistrict + "&SelectedSRO=" + SelectedSRO;
//}


//function EXCELDownloadFun(FromDate, ToDate, SROOfficeListID, DROfficeID) {

//    window.location.href = '/MISReports/SRODocCashCollection/ExportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&SelectedDistrict=" + SelectedDistrict + "&SelectedSRO=" + SelectedSRO;

//}