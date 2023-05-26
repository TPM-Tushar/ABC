var txtFromDate;
var txtToDate;
var selectedDB;

var token = '';
var header = {};
var DataReadingHistRptTable;

$(document).ready(function () {

    ////TO CHANGE TO DATE SAME AS TO FORM DATE
    //$("#txtFromDate").change(function () {
    //    $('#txtToDate').val($('#txtFromDate').val());

    //});


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which !== 8 && e.which !== 0 && (e.which < 48 || e.which > 57)) {
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

    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", FromDate);

    $('#txtToDate').datepicker({
        format: 'dd/mm/yyyy'
    }).datepicker("setDate", ToDate);

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove === "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    //$(".divMonthWise").hide();

    $("#SearchBtn").click(function () {

        //$('#SearchResult').css('display', 'block');
        //$('#InformationPanedID').html('');
        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        selectedDB = $("#DatabaseList option:selected").val();
        topRows = $("#txtTopRows").val();
        selectedReplica = $("input[name='rdoReplicaType']:checked").val();

        //console.log(txtFromDate + " " + txtToDate + " " + selectedDB + " " + topRows);

        if (selectedDB == 0) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select database.</span>');
            return;
        }
        if (txtFromDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
            return;
        }
        else if (txtToDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;

        }

        DataReadingHistRptTable = $('#DataReadingHistoryDetailTable').DataTable({
            ajax: {
                url: '/DynamicDataReader/DataReadingHistory/GetDataReadingHistoryReport',
                type: "POST",
                headers: header,
                data: {
                    'FromDate': txtFromDate, 'ToDate': txtToDate, 'SelectedDB': selectedDB
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

            // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
            //dom: 'lBfrtip',
            "destroy": true,
            "bAutoWidth": true,
            "bScrollAutoCss": true,
            "bSort": true,
            columnDefs: [
                //{ width: 50, targets: "_all" }
                //{ width: 50, targets: 2 }
                { orderable: false, targets: [2] },
                { orderable: false, targets: [3] },
                { orderable: false, targets: [4] },
                { orderable: false, targets: [5] },
                { orderable: false, targets: [6] },
                { orderable: false, targets: [7] }

            ],
            //"language": {
            //    "decimal": ",",
            //    "thousands": "."
            //},
            //{
            //    data: "STATUS_DESCRIPTION", "searchable": true, "visible": true, "name": "STATUS_DESCRIPTION"
            //    , render: function (data, type, row) {
            //        //alert(data);
            //        var textwithTooltip = "<div class='tooltip'>" + data + "<span class='tooltiptext'>Click here</span></div>";
            //        //alert(textwithTooltip);
            //        return textwithTooltip;
            //    }
            //},
            columns: [

                { data: "SrNo", "searchable": false, "visible": true, "name": "SrNo" },
                { data: "QueryID", "searchable": false, "visible": true, "name": "QueryID" },
                { data: "Purpose", "searchable": false, "visible": true, "name": "Purpose" },
                { data: "DatabaseName", "searchable": true, "visible": true, "name": "DatabaseName" },
                { data: "Date", "searchable": false, "visible": true, "name": "Date" },
                { data: "LoginName", "searchable": false, "visible": true, "name": "LoginName" },
                { data: "DBUserName", "searchable": false, "visible": true, "name": "DBUserName" },
                { data: "QueryResult", "searchable": false, "visible": true, "name": "QueryResult" },
                { data: "QuerySqlText", "searchable": false, "visible": false, "name": "QuerySqlText" },
                //{ data: "NoOfRows", "searchable": false, "visible": true, "name": "NoOfRows" },
               

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

    });

    $("#NewQueryBtn").click(function () {

        $('#SearchResult').css('display', 'none');
        $("#QueryAnalyserDetailTable").DataTable().clear().destroy();
        //$('#DtlsSearchParaListCollapse').addClass('row well-custom collapsed');

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsToggleIconSearchPara').trigger('click');

        $.ajax({
            url: '/Remittance/QueryAnalyser/QueryAnalyserNewQueryView',
            headers: header,
            type: "GET",
            success: function (data) {
                //console.log(data.data);
                //$('#modalValue').html(data.data);
                //$("#exampleModalLong").modal('show');
                //$('#SearchResult').html('');
                //$('#SearchResult').html(data); 

                $('#InformationPanedID').html('');
                $('#InformationPanedID').html(data);
            },
            error: function (xhr) {

                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                unBlockUI();
            }
        });

    });

});

function showmodelpopup(srno) {
    console.log(srno);
    //console.log(QueryExecutionStatusRptTable.rows(srno).data()[0]["QuerySqlText"]);
    var index = ((srno % 10) - 1) >= 0 ? ((srno % 10) - 1) : 9;
    var data = DataReadingHistRptTable.rows(index).data()[0]["QuerySqlText"];
    $('#modalValue').html(data);

}

function ViewData(queryid) {

    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        $('#DtlsToggleIconSearchPara').trigger('click');

    $.ajax({
        url:  '/DynamicDataReader/DataReadingHistory/GetSSRSReportData',
        headers: header,
        type: "POST",
        data: {
            'QueryId': queryid
        },
        success: function (data) {

            $('#InformationPanedID').html('');
            $('#InformationPanedID').html(data);
            unBlockUI();
        },
        error: function (xhr) {

            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            unBlockUI();
        },
        beforeSend: function () {
            blockUI('loading data.. please wait...');

        }
    });
}

function EXCELDownloadFun() {

    window.location.href = '/DynamicDataReader/DataReadingHistory/ExportDataReadingHistoryToExcel?SelectedDB=' + selectedDB + "&FromDate=" + txtFromDate + "&ToDate=" + txtToDate;
}
