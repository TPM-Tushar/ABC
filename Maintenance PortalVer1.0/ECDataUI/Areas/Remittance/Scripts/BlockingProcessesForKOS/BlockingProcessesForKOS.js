var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });



    if ($.fn.DataTable.isDataTable("#BlockingProcessesTable")) {
        $("#BlockingProcessesTable").DataTable().clear().destroy();
    }


    var BlockingProcessesRptTable = $('#BlockingProcessesTable').DataTable({
        ajax: {
            url: '/Remittance/BlockingProcessesForKOS/GetBlockingProcessForKOSDetails',
            type: "POST",
            headers: header,
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
                                $("#BlockingProcessesTable").DataTable().clear().destroy();
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
                            BlockingProcessesRptTable.search('').draw();
                            $("#BlockingProcessesTable_filter input").prop("disabled", false);
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
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,
        columnDefs: [
            //{ orderable: false, targets: [2] },
            //{ orderable: false, targets: [4] },
            //{
            //    orderable: false, targets: [5]

            //},
            //{ orderable: false, targets: [6] },
            //{ orderable: false, targets: [7] }

        ],
            
        columns: [

            { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
            { data: "session_id", "searchable": true, "visible": true, "name": "session_id" },
            { data: "command", "searchable": true, "visible": true, "name": "command" },
            { data: "blocking_session_id", "searchable": true, "visible": true, "name": "blocking_session_id" },
            { data: "wait_type", "searchable": true, "visible": true, "name": "wait_type" },
            { data: "wait_time", "searchable": true, "visible": true, "name": "wait_time" },
            { data: "wait_resource", "searchable": true, "visible": true, "name": "wait_resource" },
            { data: "TEXT", "searchable": true, "visible": true, "name": "TEXT" },
            { data: "DateTime", "searchable": true, "visible": true, "name": "DateTime" },
           
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

function EXCELDownloadFun() {

    window.location.href = '/Remittance/BlockingProcessesForKOS/DownloadBlockingProcessForKOSDetails';
}