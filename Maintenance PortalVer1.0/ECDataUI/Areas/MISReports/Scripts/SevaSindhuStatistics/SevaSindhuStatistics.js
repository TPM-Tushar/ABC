var FinYearListID;
var FromDate;
var ToDate;
var isYearWise;
var selectedYear;
var selectedMonth;
var IsMonthWise;
var selectedYearText;
var token = '';
var header = {};

$(document).ready(function () {


    var selectedRdo = "";

    $("#SearchBtn").click(function () {
        //alert("In DocumentWise 0" + selectedRdo);
        if (selectedRdo == "DW") {
            FillSevaSindhuStatisticsReportYearWiseTable();

        }
        else if (selectedRdo == "PW") {

            FillSevaSindhuStatisticsReportTable();
        }
        else if (selectedRdo == "MW") {

            FillSevaSindhuStatisticsReportMonthWiseTable();
        }
        
    });

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $(".divYearWise").hide();
    $(".divMonthWise").hide();

    
    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });
    $('#ddDROfficeList').change(function () {
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/SevaSindhuStatistics/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#ddDROfficeList').val() },
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


    $('#ddDROfficeList').focus();

    $('#txtFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        startdate: new Date(),
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
        startdate: new Date(),

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
        startdate: new Date(),

        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", FromDate);

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    //$(".divMonthWise").hide();

    $('input[type=radio][name=rdoRevenueType]').on('change', function () {

        selectedRdo = $('input[name=rdoRevenueType]:checked').val();
        if (selectedRdo == "DW") {
            $(".divYearWise").show();
            $(".divPeriodWise").hide();
            $(".divMonthWise").hide();
            if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReport")) {
                $("#SevaSindhuStatisticsReport").DataTable().clear().destroy();

            }
            if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReportYearWise")) {
                $("#SevaSindhuStatisticsReportYearWise").DataTable().clear().destroy();
            }

        }
        else if (selectedRdo == "PW") {
            $(".divPeriodWise").show();
            $(".divYearWise").hide();
            $(".divMonthWise").hide();


            if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReportYearWise")) {
                //$("#IndexIIReportsID").dataTable().fnDestroy();
                //$("#DtlsSearchParaListCollapse").trigger('click');
                $("#SevaSindhuStatisticsReportYearWise").DataTable().clear().destroy();


            }
            if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReportYearWise")) {
                $("#SevaSindhuStatisticsReportYearWise").DataTable().clear().destroy();
            }

        }
        else if (selectedRdo == "MW") {
            $(".divPeriodWise").hide();
            $(".divYearWise").hide();
            $(".divMonthWise").show();


            if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReportYearWise")) {
                $("#SevaSindhuStatisticsReportYearWise").DataTable().clear().destroy();
            }

            if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReport")) {
                $("#SevaSindhuStatisticsReport").DataTable().clear().destroy();

            }


        }
        

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');
        $("#EXCELSPANID").html('');
    });

    $('input:radio[name=rdoRevenueType][value=PW]').click();




});


function FillSevaSindhuStatisticsReportTable() {

    $(".divRevenueYearWise").hide();

    $(".divRevenuePeriodWise").show();
    $(".divDocumentWise").hide();


    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();

    selectedDistrictText = $("#ddDROfficeList option:selected").text();
    DROfficeListID = $("#ddDROfficeList option:selected").val();

    selectedSROText = $("#SROOfficeListID option:selected").text();
    SROOfficeListID = $("#SROOfficeListID option:selected").val();

    var tableARegisterReportDetails = $('#SevaSindhuStatisticsReport').DataTable({
        ajax: {

            url: '/MISReports/SevaSindhuStatistics/GetSevaSindhuReportDetails/',
            type: "POST",
            headers: header,
            data: {
                'FromDate': FromDate, 'ToDate': ToDate,
                'SROOfficeListID': SROOfficeListID, 'DROfficeListID': DROfficeListID
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
                                $("#SevaSindhuStatisticsReport").DataTable().clear().destroy();

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
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });

                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#SevaSindhuStatisticsReport input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#SevaSindhuStatisticsReport_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DROfficeWiseSummaryTableID.search('').draw();
                            $("#SevaSindhuStatisticsReport_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }

        },
        serverSide: true,

        scrollX: true,
        "scrollY": "300px",
        "responsive": true,
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bSort": true,

        columns: [

            { data: "SRNo", "searchable": true, "visible": true, "name": "srNo", "width": '5%' },
          //  { data: "SROoffice", "searchable": true, "visible": true, "name": "SROoffice", "width": '20%' },
            { data: "Application_received_date", "searchable": true, "visible": true, "name": "Application_received_date", "width": '20%' },
            { data: "No_of_Application_Received", "searchable": true, "visible": true, "name": "No_of_Application_Received", "width": '20%' },
            { data: "No_of_Application_Processed", "searchable": true, "visible": true, "name": "No_of_Application_Processed", "width": '20%' },
            { data: "No_of_Application_Registered", "searchable": true, "visible": true, "name": "No_of_Application_Registered", "width": '20%' },
            { data: "No_of_Application_Rejected", "searchable": true, "visible": true, "name": "No_of_Application_Rejected", "width": '20%' },

            /*
            * SROoffice = IndexIIReportsDetailsModel.SROoffice,
              No_of_Application_Received = IndexIIReportsDetailsModel.No_of_Application_Received,
              No_of_Application_Processed = IndexIIReportsDetailsModel.No_of_Application_Processed,
              No_of_Application_Registered = IndexIIReportsDetailsModel.No_of_Application_Registered,
              No_of_Application_Rejected = IndexIIReportsDetailsModel.No_of_Application_Rejected
            */
        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
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
}


function FillSevaSindhuStatisticsReportMonthWiseTable() {
    $(".divRevenuePeriodWise").hide();
    $(".divRevenueYearWise").hide();

    IsMonthWise = "1";

    $(".divRevenueMonthWise").show();

    selectedYearText = $("#ddFinYearList option:selected").text();
    selectedYear = $("#ddFinYearList option:selected").val();

    selectedDistrictText = $("#ddDROfficeList option:selected").text();
    DROfficeListID = $("#ddDROfficeList option:selected").val();

    selectedSROText = $("#SROOfficeListID option:selected").text();
    SROOfficeListID = $("#SROOfficeListID option:selected").val();

    FinYearListID = $("#ddFinYearList").val();

    if (selectedYear == "0") {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Year</span>');

        return;
    }
    
    if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReportYearWise")) {
        $("#SevaSindhuStatisticsReportYearWise").DataTable().clear().destroy();
    }

    if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReport")) {
        $("#SevaSindhuStatisticsReport").DataTable().clear().destroy();
    }

    FinYearListID = $("#ddFinYearList").val();

    var tableSevaSindhuStatisticsMonthWiseReport = $('#SevaSindhuStatisticsMonthWiseReport').DataTable({

        ajax: {
            url: '/MISReports/SevaSindhuStatistics/LoadSevaSindhuStatisticsReportTblMonthWise',
            type: "POST",
            headers: header,
            data: {    
                'selectedYear': FinYearListID, 'IsMonthWise': IsMonthWise,
                'SROOfficeListID': SROOfficeListID, 'DROfficeListID': DROfficeListID
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            //alert("json.serverError111::" + json.serverError);
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            }
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#SevaSindhuStatisticsReportYearWise").DataTable().clear().destroy();
                                $("#EXCELSPANID").html('');

                            }
                        }
                    });

                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                    return json.data;
                }

                unBlockUI();



            },
            error: function () {
                //  unBlockUI();
                //window.location.href = '/Error/SessionExpire';
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                $.unblockUI();
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');
                // Added by SB on 22-3-2019 at 11:06 am
                var searchString = $('#SevaSindhuStatisticsMonthWiseReport_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        bootbox.alert('Please enter valid Search String ', function () {
                            //    $('#menuDetailsListTable_filter input').val('');

                            tableSevaSindhuStatisticsYearWiseReport.search('').draw();


                        });
                        unBlockUI();
                        return false;
                    }
                }

            }
        },




        serverSide: true,
        //pageLength: 100,
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
        //dom: 'lBfrtip',
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,
        
        columns: [
            { data: "SRNo", "searchable": true, "visible": true, "name": "srNo", "width": '5%' },
         //   { data: "SROoffice", "searchable": true, "visible": true, "name": "SROoffice", "width": '20%' },
            { data: "Application_Received_Month", "searchable": true, "visible": true, "name": "Application_Received_Month", "width": '20%' },
            { data: "No_of_Application_Received", "searchable": true, "visible": true, "name": "No_of_Application_Received", "width": '20%' },
            { data: "No_of_Application_Processed", "searchable": true, "visible": true, "name": "No_of_Application_Processed", "width": '20%' },
            { data: "No_of_Application_Registered", "searchable": true, "visible": true, "name": "No_of_Application_Registered", "width": '20%' },
            { data: "No_of_Application_Rejected", "searchable": true, "visible": true, "name": "No_of_Application_Rejected", "width": '20%' },
        ],

        fnInitComplete: function (oSettings, json) {
            //alert('in fnInitComplete');
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
}


function FillSevaSindhuStatisticsReportYearWiseTable() {

    $(".divRevenuePeriodWise").hide();
    $(".divRevenueYearWise").show();
    $(".divDocumentWise").hide();

    selectedYear = $("#ddYearList option:selected").val();
   // selectedMonth = $("#ddMonthList option:selected").val();


    if (selectedYear == "0") {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Year</span>');
        return;
    }

    if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReportYearWise")) {
        $("#SevaSindhuStatisticsReportYearWise").DataTable().clear().destroy();
    }
    

    
    isYearWise = "1";

    if ($.fn.DataTable.isDataTable("#SevaSindhuStatisticsReportYearWise")) {
        $("#SevaSindhuStatisticsReportYearWise").DataTable().clear().destroy();
    }
    var tableSevaSindhuStatisticsYearWiseReport = $('#SevaSindhuStatisticsReportYearWise').DataTable({

        ajax: {
            url: '/MISReports/SevaSindhuStatistics/SevaSindhuStatisticsReportDetailsYearWise',
            type: "POST",
            headers: header,
            data: {
                  'isYearWise': isYearWise
                , 'selectedYear': selectedYear
               // , 'selectedMonth': selectedMonth
               

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
                                $("#SevaSindhuStatisticsReportYearWise").DataTable().clear().destroy();
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
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                $.unblockUI();
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');
                var searchString = $('#SevaSindhuStatisticsReportYearWise_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableSevaSindhuStatisticsYearWiseReport.search('').draw();
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },

        serverSide: true,
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

        columns: [

            { data: "SRNo", "searchable": true, "visible": true, "name": "srNo", "width": '5%' },
            { data: "SROoffice", "searchable": true, "visible": true, "name": "SROoffice", "width": '20%' },
            { data: "Application_Received_Year", "searchable": true, "visible": true, "name": "Application_Received_Year", "width": '20%' },
            { data: "No_of_Application_Received", "searchable": true, "visible": true, "name": "No_of_Application_Received", "width": '20%' },
            { data: "No_of_Application_Processed", "searchable": true, "visible": true, "name": "No_of_Application_Processed", "width": '20%' },
            { data: "No_of_Application_Registered", "searchable": true, "visible": true, "name": "No_of_Application_Registered", "width": '20%' },
            { data: "No_of_Application_Rejected", "searchable": true, "visible": true, "name": "No_of_Application_Rejected", "width": '20%' },

        ],
        fnInitComplete: function (oSettings, json) {
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
}



function EXCELDownloadFun(FromDate, ToDate, IsYearWise, selectedYear, IsMonthWise, selectedDistrictText, selectedSROText, SROOfficeListID, DROfficeListID) {
    var selectedRdo = $('input[name=rdoRevenueType]:checked').val();
    if (IsYearWise == 1) {

        window.location.href = '/MISReports/SevaSindhuStatistics/ExportSevaSindhuDtlYearWiseToExcel?isYearWise=' + IsYearWise + "&selectedYear=" + selectedYear;
    }
    else if (IsMonthWise == 1) {
        window.location.href = '/MISReports/SevaSindhuStatistics/ExportSevaSindhuDtlMonthWiseToExcel?FinYearListID=' + selectedYear + "&IsMonthWise=" + IsMonthWise + "&selectedDistrictText=" + selectedDistrictText + "&selectedSROText=" + selectedSROText + "&SROOfficeListID="+ SROOfficeListID+"&DROfficeListID="+ DROfficeListID;
    }
    else {
        window.location.href = '/MISReports/SevaSindhuStatistics/SevaSindhuStatisticsReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&selectedDistrictText=" + selectedDistrictText + "&selectedSROText=" + selectedSROText + "&SROOfficeListID=" + SROOfficeListID + "&DROfficeListID=" + DROfficeListID;
    }
}
