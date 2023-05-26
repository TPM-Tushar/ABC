var txtFromDate;
var txtToDate;
var selectedDB;
var topRows;
var selectedReplica;
var token = '';
var header = {};
var QueryExecutionStatusRptTable;



$(document).ready(function () {

    ////TO CHANGE TO DATE SAME AS TO FORM DATE
    //$("#txtFromDate").change(function () {
    //    $('#txtToDate').val($('#txtFromDate').val());

    //});


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    var selectedRdo = "";

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

    $('input[type=radio][name=rdoRevenueType]').on('change', function () {

        selectedRdo = $('input[name=rdoReplicaType]:checked').val();

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove === "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');
        $("#EXCELSPANID").html('');


    });

    $('input:radio[name=rdoReplicaType][value=PR]').click();

    $("#SearchBtn").click(function () {


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
        if (topRows == "" || topRows == undefined) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Top Rows Value.</span>');
            return;
        }

        QueryExecutionStatusRptTable = $('#QueryExecutionStatusReportTable').DataTable({
            ajax: {
                url: '/Remittance/QueryExecutionStatusReport/GetQueryExecutionStatusReport',
                type: "POST",
                headers: header,
                data: {
                    'FromDate': txtFromDate, 'ToDate': txtToDate, 'SelectedDB': selectedDB, 'TopRows': topRows, 'ReplicaType': selectedReplica
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
                //{ orderable: false, targets: [2] },
                { orderable: false, targets: [3] },
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
                //{ data: "DatabaseName", "searchable": true, "visible": true, "name": "DatabaseName" },
                { data: "QuerySqlText", "searchable": false, "visible": false, "name": "QuerySqlText" },
                { data: "Query", "searchable": false, "visible": true, "name": "Query" },
                //{
                //    data: "QuerySqlText", "searchable": false, "visible": true, "name": "QuerySqlText",
                //    render: function (data, type, row) {

                //        var renderResult = "<a onclick='ShowModelPopup(" + data + ")'>" + 'aaaaaaaaaaaaaa..' + "</a>";
                //        //var textwithtooltip = "<div class='tooltip'>Click<span class= 'tooltiptext'>" + "here" + "</span></div>";
                //        //console.log(textwithtooltip);
                //        return "hello...........................................";
                //    }
                //},

                //{ data: "ObjectName", "searchable": true, "visible": true, "name": "ObjectName" },
                //{ data: "ReplicaName", "searchable": true, "visible": true, "name": "ReplicaName" },
                { data: "QueryPlanXML", "searchable": false, "visible": false, "name": "QueryPlanXML" },
                { data: "QueryPlanXMLButton", "searchable": false, "visible": true, "name": "QueryPlanXMLButton" },
                { data: "LastExecutionTime", "searchable": true, "visible": true, "name": "LastExecutionTime" },
                { data: "CountExecutions", "searchable": true, "visible": true, "name": "CountExecutions" },

                { data: "total_worker_time", "searchable": true, "visible": true, "name": "total_worker_time" },
                { data: "max_worker_time", "searchable": true, "visible": true, "name": "max_worker_time" },
                { data: "last_worker_time", "searchable": true, "visible": true, "name": "last_worker_time" },

                { data: "total_elapsed_time", "searchable": true, "visible": true, "name": "total_elapsed_time" },
                { data: "max_elapsed_time", "searchable": true, "visible": true, "name": "max_elapsed_time" },
                { data: "last_elapsed_time", "searchable": true, "visible": true, "name": "last_elapsed_time" },

                { data: "total_physical_reads", "searchable": true, "visible": true, "name": "total_physical_reads" },
                { data: "max_physical_reads", "searchable": true, "visible": true, "name": "max_physical_reads" },
                { data: "last_physical_reads", "searchable": true, "visible": true, "name": "last_physical_reads" },

                { data: "total_logical_reads", "searchable": true, "visible": true, "name": "total_logical_reads" },
                { data: "max_logical_reads", "searchable": true, "visible": true, "name": "max_logical_reads" },
                { data: "last_logical_reads", "searchable": true, "visible": true, "name": "last_logical_reads" },

                { data: "total_logical_writes", "searchable": true, "visible": true, "name": "total_logical_writes" },
                { data: "max_logical_writes", "searchable": true, "visible": true, "name": "max_logical_writes" },
                { data: "last_logical_writes", "searchable": true, "visible": true, "name": "last_logical_writes" },

                //{ data: "MaxTempdbSpaceUsed", "searchable": true, "visible": true, "name": "MaxTempdbSpaceUsed" },
                //{ data: "AvgTempdbSpaceUsed", "searchable": true, "visible": true, "name": "AvgTempdbSpaceUsed" },
                //{ data: "LastTempdbSpaceUsed", "searchable": true, "visible": true, "name": "LastTempdbSpaceUsed" },
                //{ data: "MaxQueryMaxUsedMemory8kPages", "searchable": true, "visible": true, "name": "MaxQueryMaxUsedMemory8kPages" },
                //{ data: "AvgQueryMaxUsedMemory8kPages", "searchable": true, "visible": true, "name": "AvgQueryMaxUsedMemory8kPages" },
                //{ data: "LastQueryMaxUsedMemory8kPages", "searchable": true, "visible": true, "name": "LastQueryMaxUsedMemory8kPages" },
                //{ data: "MaxRowCount", "searchable": true, "visible": true, "name": "MaxRowCount" },
                //{ data: "AvgRowCount", "searchable": true, "visible": true, "name": "AvgRowCount" },
                //{ data: "LastRowCount", "searchable": true, "visible": true, "name": "LastRowCount" }
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



});


function EXCELDownloadFun() {

    window.location.href = '/Remittance/QueryExecutionStatusReport/ExportQueryStatusReportToExcel?SelectedReplica=' + selectedReplica + "&SelectedDB=" + selectedDB + "&FromDate=" + txtFromDate + "&ToDate=" + txtToDate + "&TopRows=" + topRows;
}

function showmodelpopup(srno) {
    console.log(srno);
    //console.log(QueryExecutionStatusRptTable.rows(srno).data()[0]["QuerySqlText"]);
    var index = ((srno % 10) - 1) >= 0 ? ((srno % 10) - 1) : 9;
    var data = QueryExecutionStatusRptTable.rows(index).data()[0]["QuerySqlText"];
    $('#modalValue').html(data);
    //return "<div class='modal fade' id='exampleModalLong' tabindex='-1' role='dialog' aria-labelledby='exampleModalLongTitle' aria-hidden='true'>" +
    //    "<div class='modal-dialog' role = 'document' > <div class='modal-content'>" +
    //            "<div class='modal-header'>" +
    //                "<h5 class='modal-title' id='exampleModalLongTitle'>Modal title</h5>" +
    //                "<button type='button' class='close' data-dismiss='modal' aria-label='Close'>" +
    //                    "<span aria-hidden='true'>&times;</span>" +
    //                "</button>" +
    //            "</div>" +
    //            "<div class='modal-body'>" +
                    
    //            "</div>" +
    //            "<div class='modal-footer'>" +
    //                "<button type='button' class='btn btn-secondary' data-dismiss='modal'>Close</button>" +
    //                "<button type='button' class='btn btn-primary'>Save changes</button>" +
    //            "</div>"+
    //        "</div>" +
    //    "</div >"+
    //"</div >";
}

function DownloadQueryPlanXML(srno) {
    var index = ((srno % 10) - 1) >= 0 ? ((srno % 10) - 1) : 9;
    var resdata = QueryExecutionStatusRptTable.rows(index).data()[0]["QueryPlanXML"];
    //string.Format("QueryPlanXML" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".sqlplan");
    var d = new Date();
    var file = "QueryPlanXML-" + d.getDate() + "-" + (d.getMonth() + 1) + "-" + d.getFullYear() + "-" + d.getHours() + "_" + d.getMinutes() + "_" + d.getSeconds();
    var filename = file + ".sqlplan";
    var blob = new Blob([resdata], { type: "text/plain;charset=utf-8" });
    saveAs(blob, filename);


    //window.location.href = '/Remittance/QueryExecutionStatusReport/DownloadQueryPlanXML?Data=' + data;
    //$.ajax({
    //    url: '/Remittance/QueryExecutionStatusReport/DownloadQueryPlanXML',
    //    data: resdata,
    //    datatype: "json",
    //    //contentType: "application/json; charset=utf-8",
    //    headers: header,
    //    type: "POST",
    //    success: function (data) {

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
}