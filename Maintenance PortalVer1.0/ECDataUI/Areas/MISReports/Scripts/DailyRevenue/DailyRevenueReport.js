var ArticleNameListID;
var FinYearListID;
var FromDate;
var ToDate;
var SROOfficeListID;
var DROfficeListID;
var isDayWise;
var selectedYear;
var selectedMonth;
var IsMonthWise;
var selectedYearText;
var selectedDistrictText;


    //$('#txtStamp5DateID').change(function () {
    //    //alert("The text has been changed.");
    //    $('#ToDateID').val($('#txtStamp5DateID').val());
    //});
//Global variables.
var token = '';
var header = {};
$(document).ready(function () {

    $("#txtFromDate").change(function () {
        $('#txtToDate').val($('#txtFromDate').val());

    });


    $('#DROOfficeListID').focus();

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    var selectedRdo = "";
    $(".divDayWise").hide();
    $(".divMonthWise").hide();

    //$('input:radio[name=rdoRevenueType][value=PW]').click();
    //  $('#DtlscollapseSearchPara').trigger('click');

    // $('#SROOfficeListID').focus();
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

    //$(".divMonthWise").hide();

    $('input[type=radio][name=rdoRevenueType]').on('change', function () {

        selectedRdo = $('input[name=rdoRevenueType]:checked').val();
        if (selectedRdo == "DW") {
            $(".divDayWise").show();
            $(".divPeriodWise").hide();
            $(".divMonthWise").hide();
            if ($.fn.DataTable.isDataTable("#DailyRevenueReport")) {
                $("#DailyRevenueReport").DataTable().clear().destroy();

            }
            if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
                $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
            }

        }
        else if (selectedRdo == "PW") {
            $(".divPeriodWise").show();
            $(".divDayWise").hide();
            $(".divMonthWise").hide();


            if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
                //$("#IndexIIReportsID").dataTable().fnDestroy();
                //$("#DtlsSearchParaListCollapse").trigger('click');
                $("#DailyrevenueReportDayWise").DataTable().clear().destroy();


            }
            if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
                $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
            }

        }
        else if (selectedRdo == "MW") {
            $(".divPeriodWise").hide();
            $(".divDayWise").hide();
            $(".divMonthWise").show();


            if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
                $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
            }

            if ($.fn.DataTable.isDataTable("#DailyRevenueReport")) {
                $("#DailyRevenueReport").DataTable().clear().destroy();

            }


        }
        else if (selectedRdo == "DocumentWise") {
            $(".divPeriodWise").hide();
            $(".divDayWise").hide();
            $(".divMonthWise").hide();
            $(".divDocumentWise").show();


            if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
                $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
            }

            if ($.fn.DataTable.isDataTable("#DailyRevenueReport")) {
                $("#DailyRevenueReport").DataTable().clear().destroy();

            }


        }



        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');
        $("#EXCELSPANID").html('');
    });

    $('input:radio[name=rdoRevenueType][value=PW]').click();

    $("#SearchBtn").click(function () {
        //alert("In DocumentWise 0" + selectedRdo);
        if (selectedRdo == "DW") {
            FillDailyRevenueReportDayWiseTable();

        }
        else if (selectedRdo == "PW") {

            FillDailyRevenueReportTable();
        }
        else if (selectedRdo == "MW") {

            FillDailyRevenueReportMonthWiseTable();
        }
        else if (selectedRdo == "DocumentWise") {
            //alert("In DocumentWise 1");
            FillDailyRevenueReportDocWiseTable();
        }

    });
    //$("#DtlsSearchParaListCollapse").trigger('click');

    $('#ddDROfficeList').change(function () {
        $.ajax({
            url: '/MISReports/TodaysDocumentsRegistered/GetSROOfficeListByDistrictID',
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

    //$('#ddDROfficeList').trigger("change");
    //$("#excelDownload").click(function () {
    //    if (isDayWise == "0") // isDayWise for =0
    //    {
    //        //alert(ArticleNameListID);
    //        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise;
    //    }
    //    else // isDayWise for =1
    //    { 
    //        //alert(ArticleNameListID);
    //        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
    //    }
    //});


});


function FillDailyRevenueReportTable() {
    $(".divRevenueDayWise").hide();
    $(".divRevenuePeriodWise").show();

    $(".divDocumentWise").hide();

    if ($.fn.DataTable.isDataTable("#DailyRevenueReport")) {
        $("#DailyRevenueReport").DataTable().clear().destroy();
    }

    //var ArticleNameListID = $('#ddArticleNameList').val();
    //var FromDate = $("#txtFromDate").val();
    //var ToDate = $("#txtToDate").val();
    //var SROOfficeListID = $("#SROOfficeListID option:selected").val();
    //var DROfficeListID = $("#ddDROfficeList option:selected").val();
    //var isDayWise = "0";

    ArticleNameListID = $('#ddArticleNameList').val();
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();
    SROOfficeListID = $("#SROOfficeListID option:selected").val();
    DROfficeListID = $("#ddDROfficeList option:selected").val();
    isDayWise = "0";
    selectedDistrictText = $("#ddDROfficeList option:selected").text();
    selectedSROText = $("#SROOfficeListID option:selected").text();
    if ($.fn.DataTable.isDataTable("#DailyRevenueReport")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyRevenueReport").DataTable().clear().destroy();


    }
    if ($.fn.DataTable.isDataTable("#DailyRevenueDocumentWiseReport")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyRevenueDocumentWiseReport").DataTable().clear().destroy();


    }

    //Added by Madhusoodan on 12-05-2020
    //To hide Processed Report Note when From Date - To Date are same
    if (FromDate == ToDate)
        $("#ReportInfoID").hide();
    else
        $("#ReportInfoID").show();



    var tableDailyRevenueReport = $('#DailyRevenueReport').DataTable({

        ajax: {
            url: '/MISReports/DailyRevenue/GetDailyRevenueReportDetails',
            type: "POST",
            headers: header,
            data: {
                'SROOfficeListID': SROOfficeListID, 'DROfficeListID': DROfficeListID, 'FromDate': FromDate, 'ToDate': ToDate,
                'ArticleNameListID': ArticleNameListID,
                'isDayWise': isDayWise
                , 'selectedDistrict': selectedDistrictText
                , 'selectedSRO': selectedSROText

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
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#DailyRevenueReport").DataTable().clear().destroy();
                                $("#EXCELSPANID").html('');

                                 
                            }
                            //$("#DtlsSearchParaListCollapse").trigger('click');
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
                var searchString = $('#DailyRevenueReport_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;

                    if (!regexToMatch.test(searchString)) {
                        $("#DailyRevenueReport_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            //    $('#menuDetailsListTable_filter input').val('');

                            tableDailyRevenueReport.search('').draw();
                            $("#DailyRevenueReport_filter input").prop("disabled", false);

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



        //buttons: [
        //{
        //    extend: 'pdf',
        //    text: '<i class="fa fa-file-pdf-o"></i> PDF',
        //    exportOptions: {
        //        columns: ':not(.no-print)'
        //    },
        //    action:
        //        function (e, dt, node, config) {
        //            //this.disable();
        //            window.location = '/MISReports/DailyRevenue/ExportDailyRevenueReportToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise;
        //        }


        //},
        //{
        //    extend: 'excel',
        //    text: '<i class="fa fa-file-pdf-o"></i> Excel',
        //    exportOptions: {
        //        columns: ':not(.no-print)'
        //    },
        //    action:
        //        function (e, dt, node, config) {
        //            this.disable();
        //            window.location = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise;
        //        }


        //}

        //],


        columnDefs: [

            { width: 30, targets: [0] },
            { width: 30, targets: [1] },
            { width: 30, targets: [2] },
            { width: 20, targets: [3] },
            { width: 30, targets: [4] },
            { width: 50, targets: [5] },
            { width: 50, targets: [6] },
            { width: 50, targets: [7] }
             

            //{ orderable: false, targets: [1] },
            //{ orderable: false, targets: [3] },
            //{ orderable: false, targets: [4] },
            //{ orderable: false, targets: [5] }

        ],

        columns: [
            { data: "SRNo", "searchable": true, "visible": true, "name": "SRNo" },
            { data: "districtName", "searchable": true, "visible": true, "name": "districtName" },
            { data: "officeName", "searchable": true, "visible": true, "name": "officeName" },
            { data: "ArticleName", "searchable": true, "visible": true, "name": "ArticleName" },
            { data: "Documents", "searchable": true, "visible": true, "name": "Documents" },
            { data: "StampDuty", "searchable": true, "visible": true, "name": "StampDuty" },
            { data: "RegistrationFee", "searchable": true, "visible": true, "name": "RegistrationFee" },
            { data: "TotalAmount", "searchable": true, "visible": true, "name": "TotalAmount" }


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
            //responsiveHelper.respond();
            unBlockUI();
        },
    });

    //Added by Madhusoodan on 11-05-2020
    //To Show District Name & Office Name columns only if a particular article is selected in 'Period Wise' radio btn is selected.
    if (ArticleNameListID == 0)
    {
        tableDailyRevenueReport.column(1).visible(false);
        tableDailyRevenueReport.column(2).visible(false);
    }
   






    //tableDailyRevenueReport.button(0).nodes().css('background', 'green');
    //tableDailyRevenueReport.button(1).nodes().css('background', 'green');




}


function FillDailyRevenueReportMonthWiseTable() {
    $(".divRevenuePeriodWise").hide();
    $(".divRevenueDayWise").hide();
    //By Madhusoodan on 22-05-2020
    //$(".divDocumentWise").hide();

    IsMonthWise = "1";
    $(".divRevenueMonthWise").show();
    selectedYearText = $("#ddFinYearList option:selected").text();
    selectedDistrictText = $("#ddDROfficeList option:selected").text();
    selectedSROText = $("#SROOfficeListID option:selected").text();
    var ArticleNameListID = $('#ddArticleNameList').val();
    var DROListID = $('ddDROfficeList').val();
    //if (ArticleNameListID == "0" && DROListID == "0") {
    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select specific Article</span>');

    //    return;
    //}

    //var selectedYear = $("#ddYearList option:selected").val();
    //var selectedMonth = $("#ddMonthList option:selected").val();
    selectedYear = $("#ddFinYearList option:selected").val();


    if (selectedYear == "0") {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Year</span>');

        return;
    }
    if ($.fn.DataTable.isDataTable("#DailyRevenueDocumentWiseReport")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyRevenueDocumentWiseReport").DataTable().clear().destroy();


    }
    if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
        $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
    }

    //var FromDate = $("#txtFromDate").val();
    //var ToDate = $("#txtToDate").val();
    //var SROOfficeListID = $("#SROOfficeListID option:selected").val();
    //var DROfficeListID = $("#ddDROfficeList option:selected").val();
    //var isDayWise = "1";



    //alert(ArticleNameListID);
    if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyrevenueReportDayWise").DataTable().clear().destroy();


    }
    if ($.fn.DataTable.isDataTable("#DailyRevenueReport")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyRevenueReport").DataTable().clear().destroy();


    }
    SROOfficeListID = $("#SROOfficeListID option:selected").val();
    DROfficeListID = $("#ddDROfficeList option:selected").val();
    ArticleNameListID = $("#ddArticleNameList option:selected").val();
    FinYearListID = $("#ddFinYearList").val();

    var tableDailyRevenueMonthWiseReport = $('#DailyRevenueMonthWiseReport').DataTable({

        ajax: {
            url: '/MISReports/DailyRevenue/LoadDailyRevenueReportTblMonthWise',
            type: "POST",
            headers: header,
            data: {
                'SROOfficeListID': SROOfficeListID, 'DROfficeListID': DROfficeListID, 'ArticleNameListID': ArticleNameListID,
                'selectedYear': FinYearListID, "IsMonthWise": IsMonthWise


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
                                $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
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
                var searchString = $('#DailyRevenueMonthWiseReport_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        bootbox.alert('Please enter valid Search String ', function () {
                            //    $('#menuDetailsListTable_filter input').val('');

                            tableDailyRevenueDayWiseReport.search('').draw();


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
        //"dom": '<"toolbar">frtip',


        //buttons: [
        //{
        //    extend: 'pdf',
        //    text: '<i class="fa fa-file-pdf-o"></i> PDF',
        //    exportOptions: {
        //        columns: ':not(.no-print)'                                                                                                     
        //    },                                                                                                                                 
        //    action:
        //        function (e, dt, node, config) {
        //            //this.disable();
        //            window.location = '/MISReports/DailyRevenue/ExportDailyRevenueReportToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
        //        }


        //},
        //{
        //    extend: 'excel',
        //    text: '<i class="fa fa-file-pdf-o"></i> Excel',
        //    exportOptions: {
        //        columns: ':not(.no-print)'
        //    },
        //    action:
        //        function (e, dt, node, config) {
        //            this.disable();
        //            window.location = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
        //        }


        //}

        //],


        columnDefs: [

            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] },
            { orderable: false, targets: [3] },
            { orderable: false, targets: [4] }

        ],

        //columns: [
        //    { data: "SRNo", "searchable": true, "visible": true, "name": "SRNo" },
        //    { data: "FinancialYear", "searchable": true, "visible": true, "name": "FinancialYear" },
        //    { data: "ArticleName", "searchable": true, "visible": true, "name": "ArticleName" },
        //    { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
        //    { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
        //    { data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" },
        //    { data: "PurchaseValue", "searchable": true, "visible": true, "name": "PurchaseValue" }
        //    { data: "StampDuty", "searchable": true, "visible": true, "name": "StampDuty" },
        //    { data: "RegistrationFee", "searchable": true, "visible": true, "name": "RegistrationFee" }

        //],
        columns: [
            { data: "SRNo", "searchable": true, "visible": true, "name": "SRNo" },
            { data: "Month", "searchable": true, "visible": true, "name": "Month" },
            { data: "ArticleName", "searchable": true, "visible": true, "name": "ArticleName" },
            { data: "Documents", "searchable": true, "visible": true, "name": "Documents" },
            { data: "StampDuty", "searchable": true, "visible": true, "name": "StampDuty" },
            { data: "RegistrationFee", "searchable": true, "visible": true, "name": "RegistrationFee" },
            { data: "TotalAmount", "searchable": true, "visible": true, "name": "TotalAmount" }

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
    //$("#excelDownload").click(function () {
    //    if (isDayWise == "0") // isDayWise for =0
    //    {
    //        //alert(ArticleNameListID);
    //        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise;
    //    }
    //    else // isDayWise for =1
    //    {
    //        //alert(ArticleNameListID);
    //        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
    //    }
    //});




    //tableDailyRevenueMonthWiseReport.columns.adjust().draw();



    //tableDailyRevenueDayWiseReport.button(0).nodes().css('background', 'green');
    //tableDailyRevenueDayWiseReport.button(1).nodes().css('background', 'green');




}




function FillDailyRevenueReportDocWiseTable() {
    //alert("In DocumentWise 2");
    $(".divRevenuePeriodWise").hide();
    $(".divRevenueDayWise").hide();
    //IsMonthWise = "1";
    $(".divRevenueMonthWise").hide();
    $(".divDocumentWise").show();
    selectedYearText = $("#ddFinYearList option:selected").text();
    selectedDistrictText = $("#ddDROfficeList option:selected").text();
    selectedSROText = $("#SROOfficeListID option:selected").text();
    var ArticleNameListID = $('#ddArticleNameList').val();
    var DROListID = $('ddDROfficeList').val();
    //if (ArticleNameListID == "0" && DROListID == "0") {
    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select specific Article</span>');

    //    return;
    //}

    //var selectedYear = $("#ddYearList option:selected").val();
    //var selectedMonth = $("#ddMonthList option:selected").val();
    selectedYear = $("#ddFinYearList option:selected").val();


    if (selectedYear == "0") {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Year</span>');

        return;
    }

    if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
        $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
    }

    //var FromDate = $("#txtFromDate").val();
    //var ToDate = $("#txtToDate").val();
    //var SROOfficeListID = $("#SROOfficeListID option:selected").val();
    //var DROfficeListID = $("#ddDROfficeList option:selected").val();
    //var isDayWise = "1";



    //alert(ArticleNameListID);
    if ($.fn.DataTable.isDataTable("#DailyRevenueMonthWiseReport")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyRevenueMonthWiseReport").DataTable().clear().destroy();


    }
    if ($.fn.DataTable.isDataTable("#DailyRevenueReport")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyRevenueReport").DataTable().clear().destroy();


    }
    if ($.fn.DataTable.isDataTable("#DailyRevenueDocumentWiseReport")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyRevenueDocumentWiseReport").DataTable().clear().destroy();


    }


    SROOfficeListID = $("#SROOfficeListID option:selected").val();
    DROfficeListID = $("#ddDROfficeList option:selected").val();
    ArticleNameListID = $("#ddArticleNameList option:selected").val();
    FinYearListID = $("#ddFinYearList").val();

    var tableDailyRevenueDocWiseReport = $('#DailyRevenueDocumentWiseReport').DataTable({

        ajax: {
            url: '/MISReports/DailyRevenue/LoadDailyRevenueReportTblDocWise',
            type: "POST",
            headers: header,
            data: {
                'SROOfficeListID': SROOfficeListID, 'DROfficeListID': DROfficeListID, 'ArticleNameListID': ArticleNameListID,
                'selectedYear': FinYearListID
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
                                $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
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
                var searchString = $('#DailyRevenueDocumentWiseReport_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        bootbox.alert('Please enter valid Search String ', function () {
                            //    $('#menuDetailsListTable_filter input').val('');

                            tableDailyRevenueDocWiseReport.search('').draw();


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
        //"dom": '<"toolbar">frtip',


        //buttons: [
        //{
        //    extend: 'pdf',
        //    text: '<i class="fa fa-file-pdf-o"></i> PDF',
        //    exportOptions: {
        //        columns: ':not(.no-print)'                                                                                                     
        //    },                                                                                                                                 
        //    action:
        //        function (e, dt, node, config) {
        //            //this.disable();
        //            window.location = '/MISReports/DailyRevenue/ExportDailyRevenueReportToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
        //        }


        //},
        //{
        //    extend: 'excel',
        //    text: '<i class="fa fa-file-pdf-o"></i> Excel',
        //    exportOptions: {
        //        columns: ':not(.no-print)'
        //    },
        //    action:
        //        function (e, dt, node, config) {
        //            this.disable();
        //            window.location = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
        //        }


        //}

        //],


        columnDefs: [

            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] },
            { orderable: false, targets: [3] },
            { orderable: false, targets: [4] }

        ],



        columns: [
            { data: "SRNo", "searchable": true, "visible": true, "name": "SRNo" },
            { data: "FinancialYear", "searchable": true, "visible": true, "name": "FinancialYear" },
            { data: "ArticleName", "searchable": true, "visible": true, "name": "ArticleName" },
            { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
            { data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" },
            { data: "PurchaseValue", "searchable": true, "visible": true, "name": "PurchaseValue" },
            { data: "StampDuty", "searchable": true, "visible": true, "name": "StampDuty" },
            { data: "RegistrationFee", "searchable": true, "visible": true, "name": "RegistrationFee" },
            { data: "Total_StampDuty_RegiFee", "searchable": true, "visible": true, "name": "Total_StampDuty_RegiFee" }

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
    //$("#excelDownload").click(function () {
    //    if (isDayWise == "0") // isDayWise for =0
    //    {
    //        //alert(ArticleNameListID);
    //        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise;
    //    }
    //    else // isDayWise for =1
    //    {
    //        //alert(ArticleNameListID);
    //        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
    //    }
    //});




    //tableDailyRevenueMonthWiseReport.columns.adjust().draw();



    //tableDailyRevenueDayWiseReport.button(0).nodes().css('background', 'green');
    //tableDailyRevenueDayWiseReport.button(1).nodes().css('background', 'green');




}


function FillDailyRevenueReportDayWiseTable() {

    $(".divRevenuePeriodWise").hide();
    $(".divRevenueDayWise").show();
    $(".divDocumentWise").hide();

    selectedDistrictText = $("#ddDROfficeList option:selected").text();
    selectedSROText = $("#SROOfficeListID option:selected").text();
    var ArticleNameListID = $('#ddArticleNameList').val();
    //if (ArticleNameListID == "0") {
    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select specific Article</span>');

    //    return;
    //}

    //var selectedYear = $("#ddYearList option:selected").val();
    //var selectedMonth = $("#ddMonthList option:selected").val();
    selectedYear = $("#ddYearList option:selected").val();
    selectedMonth = $("#ddMonthList option:selected").val();


    if (selectedYear == "0") {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Year</span>');

        return;
    }

    if (selectedMonth == "0") {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Month</span>');

        return;
    }

    if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
        $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
    }
    if ($.fn.DataTable.isDataTable("#DailyRevenueDocumentWiseReport")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyRevenueDocumentWiseReport").DataTable().clear().destroy();


    }
    //var FromDate = $("#txtFromDate").val();
    //var ToDate = $("#txtToDate").val();
    //var SROOfficeListID = $("#SROOfficeListID option:selected").val();
    //var DROfficeListID = $("#ddDROfficeList option:selected").val();
    //var isDayWise = "1";

    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();
    SROOfficeListID = $("#SROOfficeListID option:selected").val();
    DROfficeListID = $("#ddDROfficeList option:selected").val();
    isDayWise = "1";
    ArticleNameListID = $("#ddArticleNameList option:selected").val();
    if ($.fn.DataTable.isDataTable("#DailyrevenueReportDayWise")) {
        //$("#IndexIIReportsID").dataTable().fnDestroy();
        //$("#DtlsSearchParaListCollapse").trigger('click');
        $("#DailyrevenueReportDayWise").DataTable().clear().destroy();


    }
    var tableDailyRevenueDayWiseReport = $('#DailyrevenueReportDayWise').DataTable({

        ajax: {
            url: '/MISReports/DailyRevenue/GetDailyRevenueReportDetailsDayWise',
            type: "POST",
            headers: header,
            data: {
                'SROOfficeListID': SROOfficeListID, 'DROfficeListID': DROfficeListID, 'FromDate': FromDate, 'ToDate': ToDate, 'ArticleNameListID': ArticleNameListID
                , 'isDayWise': isDayWise
                , 'selectedYear': selectedYear
                , 'selectedMonth': selectedMonth
                , 'selectedDistrict': selectedDistrictText
                , 'selectedSRO': selectedSROText

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
                                $("#DailyrevenueReportDayWise").DataTable().clear().destroy();
                                $("#EXCELSPANID").html('');
                            }

                            //$("#DtlsSearchParaListCollapse").trigger('click');
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
                var searchString = $('#DailyrevenueReportDayWise_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        bootbox.alert('Please enter valid Search String ', function () {
                            //    $('#menuDetailsListTable_filter input').val('');

                            tableDailyRevenueDayWiseReport.search('').draw();


                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },




        serverSide: true,
        //"PageLength": 10,
        //"scrollX": true,
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



        //buttons: [
        //{
        //    extend: 'pdf',
        //    text: '<i class="fa fa-file-pdf-o"></i> PDF',
        //    exportOptions: {
        //        columns: ':not(.no-print)'                                                                                                     
        //    },                                                                                                                                 
        //    action:
        //        function (e, dt, node, config) {
        //            //this.disable();
        //            window.location = '/MISReports/DailyRevenue/ExportDailyRevenueReportToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
        //        }


        //},
        //{
        //    extend: 'excel',
        //    text: '<i class="fa fa-file-pdf-o"></i> Excel',
        //    exportOptions: {
        //        columns: ':not(.no-print)'
        //    },
        //    action:
        //        function (e, dt, node, config) {
        //            this.disable();
        //            window.location = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
        //        }


        //}

        //],


        columnDefs: [

            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] },
            { orderable: false, targets: [3] },
            { orderable: false, targets: [4] }

        ],

        columns: [
            { data: "SRNo", "searchable": true, "visible": true, "name": "SRNo" },
            { data: "DateValue", "searchable": true, "visible": true, "name": "SRNo" },
            { data: "ArticleName", "searchable": true, "visible": true, "name": "ArticleName" },
            { data: "Documents", "searchable": true, "visible": true, "name": "Documents" },
            { data: "StampDuty", "searchable": true, "visible": true, "name": "StampDuty" },
            { data: "RegistrationFee", "searchable": true, "visible": true, "name": "RegistrationFee" },
            { data: "TotalAmount", "searchable": true, "visible": true, "name": "TotalAmount" }

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

    //$("#excelDownload").click(function () {
    //    if (isDayWise == "0") // isDayWise for =0
    //    {
    //        //alert(ArticleNameListID);
    //        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise;
    //    }
    //    else // isDayWise for =1
    //    {
    //        //alert(ArticleNameListID);
    //        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + isDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth;
    //    }
    //});


    //tableDailyRevenueDayWiseReport.columns.adjust().draw();





    //tableDailyRevenueDayWiseReport.button(0).nodes().css('background', 'green');
    //tableDailyRevenueDayWiseReport.button(1).nodes().css('background', 'green');




}



function EXCELDownloadFun(FromDate, ToDate, SROOfficeListID, ArticleNameListID, DROfficeListID, IsDayWise, selectedYear = "", selectedMonth = "", IsMonthWise = "") {
    var selectedRdo = $('input[name=rdoRevenueType]:checked').val();
    if (IsDayWise == "1") {

        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + IsDayWise + "&selectedYear=" + selectedYear + "&selectedMonth=" + selectedMonth + "&SelectedDistrict=" + selectedDistrictText + "&SelectedSROText=" + selectedSROText + "&MaxDate=" + MaxDate;
    }
    else if (IsMonthWise == "1") {
        window.location.href = '/MISReports/DailyRevenue/ExportDailyRevDtlMonthWiseToExcel?DROfficeListID=' + DROfficeListID + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&FinYearListID=" + selectedYear + "&FinYear=" + selectedYearText + "&SelectedDistrict=" + selectedDistrictText + "&MaxDate=" + MaxDate;
    }
    else {
        window.location.href = '/MISReports/DailyRevenue/DailyRevenueReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&DROfficeListID=" + DROfficeListID + "&isDayWise=" + IsDayWise + "&SelectedDistrict=" + selectedDistrictText + "&SelectedSROText=" + selectedSROText + "&MaxDate=" + MaxDate;
    }

}

function EXCELDownloadMonthWiseFun(SROOfficeListID, DROfficeListID, ArticleNameListID, selectedYear) {
}


function EXCELDownloadDocumentWise(DROfficeListID, SROOfficeListID, ArticleNameListID, selectedYear) {
    window.location.href = '/MISReports/DailyRevenue/ExportDailyRevDtlDocumentWiseToExcel?DROfficeListID=' + DROfficeListID + "&SROOfficeListID=" + SROOfficeListID + "&ArticleNameListID=" + ArticleNameListID + "&FinYearListID=" + selectedYear + "&MaxDate=" + MaxDate + "&FinYear=" + selectedYearText + "&SelectedDistrict=" + selectedDistrictText;
}



    //$('#txtStamp5DateID').change(function () {
    //    //alert("The text has been changed.");
    //    $('#ToDateID').val($('#txtStamp5DateID').val());
    //});
