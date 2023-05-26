var FromDate;
var ToDate;
var SROOfficeListID;
//var DROfficeID;
//Global variables.
var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    //$('#DROOfficeListID').change(function () {
    //    blockUI('Loading data please wait.');
    //    $.ajax({
    //        url: '/MISReports/JurisdictionalWise/GetSROOfficeListByDistrictID',
    //        data: { "DistrictID": $('#DROOfficeListID').val() },
    //        datatype: "json",
    //        type: "GET",
    //        success: function (data) {
    //            if (data.serverError == true) {
    //                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
    //                    function () {
    //                        window.location.href = "/Home/HomePage"
    //                    });
    //            }
    //            else {
    //                $('#SROOfficeListID').empty();
    //                $.each(data.SROOfficeList, function (i, SROOfficeList) {
    //                    SROOfficeList
    //                    $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
    //                });
    //            }
    //            unBlockUI();
    //        },
    //        error: function (xhr) {
    //            unBlockUI();
    //        }
    //    });
    //});


    $('#SROOfficeListID').focus();
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

    $("#SearchBtn").click(function () {
        if ($.fn.DataTable.isDataTable("#jurisdictionalDetailsID")) {
            $("#jurisdictionalDetailsID").DataTable().clear().destroy();
        }

        FromDate = $("#txtFromDate").val();
        ToDate = $("#txtToDate").val();
        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        //DROfficeID = $("#DROOfficeListID option:selected").val();

        //if (DROfficeID == "0") {
        //    if ($.fn.DataTable.isDataTable("#jurisdictionalDetailsID")) {
        //        $("#jurisdictionalDetailsID").DataTable().clear().destroy();
        //    }
        //    bootbox.alert({
        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any District." + '</span>',
        //        callback: function () {
        //            var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        //            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        //                $('#DtlsSearchParaListCollapse').trigger('click');
        //        }
        //    });
        //}
        //else
        //if (SROOfficeListID == "0") {
        //    if ($.fn.DataTable.isDataTable("#jurisdictionalDetailsID")) {
        //        $("#jurisdictionalDetailsID").DataTable().clear().destroy();
        //    }
        //    bootbox.alert({
        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any Jurisdictional Office." + '</span>',
        //        callback: function () {
        //            var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        //            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        //                $('#DtlsSearchParaListCollapse').trigger('click');

        //            // Added by shubham bhagat on 18-07-2019
        //            $('#jurisdictionalSummaryTableID').html('');
        //            $("#PDFSPANID").html('');
        //            $("#EXCELSPANID").html('');
        //        }
        //    });
        //}
        //else {
            // Jurisdictional Summary Table
            //$.ajax({
            //    url: '/MISReports/JurisdictionalWise/JurisdictionalWiseSummary/',
            //    data: {
            //        'fromDate': FromDate, 'ToDate': ToDate, 'SROOfficeListID': SROOfficeListID//, 'DROfficeID': DROfficeID
            //    },
            //    datatype: "json",
            //    headers: header,
            //    type: "POST",
            //    success: function (data) {
            //        $('#jurisdictionalSummaryTableID').html('');
            //        $('#jurisdictionalSummaryTableID').html(data);
            //        //unBlockUI();
            //    },
            //    error: function (xhr) {

            //        bootbox.alert({
            //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
            //            callback: function () {
            //            }
            //        });
            //        unBlockUI();
            //    }
            //});

            // Jurisdictional Details Table
            if ($.fn.DataTable.isDataTable("#jurisdictionalDetailsID")) {              
                $("#jurisdictionalDetailsID").DataTable().clear().destroy();
            }
            var tableIndexReports = $('#jurisdictionalDetailsID').DataTable({

                ajax: {
                    url: '/MISReports/JurisdictionalWise/JurisdictionalWiseDetail',
                    type: "POST",
                    headers: header,
                    data: {
                        'fromDate': FromDate, 'ToDate': ToDate, 'SROOfficeListID': SROOfficeListID//, 'DROfficeID': DROfficeID
                    },
                    dataSrc: function (json) {
                        unBlockUI();
                        unBlockUI();
                        if (json.errorMessage != null) {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                callback: function () {
                                    if (json.serverError != undefined) {
                                        window.location.href = "/Home/HomePage"
                                    } else {
                                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                            $('#DtlsSearchParaListCollapse').trigger('click');
                                        $("#jurisdictionalDetailsID").DataTable().clear().destroy();
                                        //$("#PDFSPANID").html('');
                                        $("#EXCELSPANID").html('');
                                        // Added by shubham bhagat on 18-07-2019
                                        $('#jurisdictionalSummaryTableID').html('');
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
                        unBlockUI();
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            callback: function () {
                            }
                        });
                    },
                    beforeSend: function () {
                        blockUI('Loading data please wait.');
                        // Added by SB on 22-3-2019 at 11:06 am
                        var searchString = $('#jurisdictionalDetailsID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;

                            if (!regexToMatch.test(searchString)) {
                                $("#jurisdictionalDetailsID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    tableIndexReports.search('').draw();
                                    $("#jurisdictionalDetailsID_filter input").prop("disabled", false);
                                });
                                unBlockUI();
                                return false;
                            }
                        }
                    }
                },
                serverSide: true,
                // pageLength: 100,
                "scrollX": true,
                "scrollY": "300px",
                scrollCollapse: true,
                bPaginate: true,
                bLengthChange: true,
                // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
                // "pageLength": -1,
                //sScrollXInner: "150%",
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
                    { orderable: false, targets: [6] }

                ],
                columns: [
                    { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                    { data: "JurisdictionalOffice", "searchable": true, "visible": true, "name": "JurisdictionalOffice" },
                    { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
                    { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
                    { data: "StumpDuty", "searchable": true, "visible": true, "name": "StumpDuty" },
                    { data: "RegistrationFees", "searchable": true, "visible": true, "name": "RegistrationFees" },
                    { data: "Total", "searchable": true, "visible": true, "name": "Total" }
                ],
                fnInitComplete: function (oSettings, json) {
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
                    unBlockUI();
                },
            });
                
    });
});

function PDFDownloadFun(FromDate, ToDate, SROOfficeListID) {
    window.location.href = '/MISReports/JurisdictionalWise/JurisdictionalWiseToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&MaxDate=" + MaxDate ;
}

function EXCELDownloadFun(FromDate, ToDate, SROOfficeListID) {
    window.location.href = '/MISReports/JurisdictionalWise/ExportJurisdictionalWiseToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&MaxDate=" + MaxDate;
}