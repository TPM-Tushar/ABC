
var token = '';
var header = {};

var txtFromDate;
var txtToDate;
var SroID;
var TableID;

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;



    $("input[name='OfficeTypeToGetDropDown']").click(function () {
        var radioValue = $("input[name='OfficeTypeToGetDropDown']:checked").val();
        blockUI('PLEASE WAIT');
        $.ajax({
            url: '/XELFiles/XELFilesDetails/GetOfficeList',
            datatype: "text",
            type: "GET",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            data: { "OfficeType": radioValue },
            success: function (data) {
                // ADDED BY SHUBHAM BHAGAT ON 21-05-2019 AT 6:36 PM
                if (data.errorMessage == undefined) {
                    $('#SROOfficeListID').empty();
                    $.each(data.OfficeList, function (i, OfficeListItem) {
                        $('#SROOfficeListID').append('<option value="' + OfficeListItem.Value + '">' + OfficeListItem.Text + '</option>');
                    });
                }
                else {
                    bootbox.alert({
                        //size: 'small',
                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    });
                }


                unBlockUI();
            },
            error: function (xhr) {
                bootbox.alert({
                    //size: 'small',
                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                });
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


    $('#ddTableNameList').change(function () {
         $("#SROOfficeListID").val("0");
        if ($.fn.DataTable.isDataTable("#DailyReceiptDetailsTable")) {
            $("#DailyReceiptDetailsTable").DataTable().clear().destroy();
        }

    });

    $("#SearchBtn").click(function () {

        if ($.fn.DataTable.isDataTable("#DailyReceiptDetailsTable")) {
            $("#DailyReceiptDetailsTable").DataTable().clear().destroy();
        }
        var OfficeType = $("input[name='OfficeTypeToGetDropDown']:checked").val();
        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        SroID = $("#SROOfficeListID option:selected").val();
        TableID = $("#ddTableNameList option:selected").val();


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

                url: '/XELFiles/XELFilesDetails/LoadXELLogDetails',
                type: "POST",
                headers: header,
                data: {
                    'FromDate': txtFromDate, 'ToDate': txtToDate, 'SroID': SroID, 'TableID': TableID, 'OfficeType': OfficeType
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
            "scrollX": true,
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

            "destroy": true,
            "bAutoWidth": true,
            "bScrollAutoCss": true,

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
                { data: "sroCode", "searchable": true, "visible": true, "name": "sroCode" },
                { data: "AbsolutePath", "searchable": true, "visible": true, "name": "AbsolutePath" },
                { data: "fileName", "searchable": true, "visible": true, "name": "fileName" },
                { data: "Year", "searchable": true, "visible": true, "name": "Year" },
                { data: "Month", "searchable": true, "visible": true, "name": "Month" },
                { data: "TransmissionInitateDateTime", "searchable": true, "visible": true, "name": "TransmissionInitateDateTime" },
                { data: "TransmissionCompleteDateTime", "searchable": true, "visible": true, "name": "TransmissionCompleteDateTime" },
                { data: "FileSize", "searchable": true, "visible": true, "name": "FileSize" },
                { data: "FileReadDateTime", "searchable": true, "visible": true, "name": "FileReadDateTime" },

                { data: "IsSuccessfullUpload", "searchable": true, "visible": true, "name": "IsSuccessfullUpload" },
             
                { data: "IsFileReadSuccessful", "searchable": true, "visible": true, "name": "IsFileReadSuccessful" },
                { data: "EventStartDate", "searchable": true, "visible": true, "name": "EventStartDate" },
                { data: "EventEndDate", "searchable": true, "visible": true, "name": "EventEndDate" },
                { data: "LogDate", "searchable": true, "visible": true, "name": "LogDate" },
                { data: "sExceptionType", "searchable": true, "visible": true, "name": "sExceptionType" },
                { data: "InnerExceptionMsg", "searchable": true, "visible": true, "name": "InnerExceptionMsg" },
                { data: "ExceptionMsg", "searchable": true, "visible": true, "name": "ExceptionMsg" },
                { data: "ExceptionStackTrace", "searchable": true, "visible": true, "name": "ExceptionStackTrace" },
                { data: "ExceptionMethodName", "searchable": true, "visible": true, "name": "ExceptionMethodName" },
                { data: "SchedulerName", "searchable": true, "visible": true, "name": "SchedulerName" },
            ],
            fnInitComplete: function (oSettings, json) {
                //alert('in fnInitComplete');

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

        if (TableID == 1) {
            DailyReceiptRptTable.columns([14]).visible(false);
            DailyReceiptRptTable.columns([15]).visible(false);
            DailyReceiptRptTable.columns([16]).visible(false);
            DailyReceiptRptTable.columns([17]).visible(false);
            DailyReceiptRptTable.columns([18]).visible(false);
            DailyReceiptRptTable.columns([19]).visible(false);
            DailyReceiptRptTable.columns([20]).visible(false);


        }

        else if (TableID == 2) {
 
            DailyReceiptRptTable.columns([2]).visible(false);
            DailyReceiptRptTable.columns([3]).visible(false);
            DailyReceiptRptTable.columns([4]).visible(false);
            DailyReceiptRptTable.columns([5]).visible(false);
            DailyReceiptRptTable.columns([6]).visible(false);
            DailyReceiptRptTable.columns([7]).visible(false);
            DailyReceiptRptTable.columns([8]).visible(false);
            DailyReceiptRptTable.columns([9]).visible(false);
            DailyReceiptRptTable.columns([10]).visible(false);
            DailyReceiptRptTable.columns([11]).visible(false);
            DailyReceiptRptTable.columns([12]).visible(false);
            DailyReceiptRptTable.columns([13]).visible(false);


        }
    });




});

