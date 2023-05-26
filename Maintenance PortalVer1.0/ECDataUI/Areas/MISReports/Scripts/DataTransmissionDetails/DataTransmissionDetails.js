
var token = '';
var header = {};
var DataBaseListID;
var SROOfficeListID;



$(document).ready(function () {
    //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    //    $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
    //});
    

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

   

    $("#SearchBtn").click(function () {
        //if ($('#DROOfficeListID').val() < "0" || $('#DROOfficeListID').val() < "0") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any District.</span>');
        //    return;
        //}
        //else if ($('#SROOfficeListID').val() < "0" || $('#SROOfficeListID').val() < "0") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any SRO.</span>');
        //    return;

        //}
        //else if ($('#ModuleNameListID').val() < "0") {
        //    alert($('#ModuleNameListID').val());
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Module.</span>');
        //    return;
        //}
        //else if ($('#FeeTypeListID').val() == "" || $('#FeeTypeListID').val() < "0") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Fee Type.</span>');
        //    return;
        //}


        //if ($.fn.DataTable.isDataTable("#DailyReceiptsID")) {
        //    $("#DailyReceiptsID").DataTable().clear().destroy();
        //}


        //if ($.fn.DataTable.isDataTable("#DailyReceiptsID")) {
        //    //$("#DailyReceiptsTableID").dataTable().fnDestroy();
        //    //$("#DtlsSearchParaListCollapse").trigger('click');
        //    $("#DailyReceiptsID").DataTable().clear().destroy();
        //}
      
        DataBaseListID = $("#DataBaseListID option:selected").val();
        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        var DailyReceiptDetailsTable = $('#DailyReceiptsID').DataTable({
            ajax: {
                url: '/MISReports/DataTransmissionDetails/LoadDataTransmissionDetails',
                type: "POST",
                headers: header,
                "autoWidth": true,
                data: {
                    'DBName': DataBaseListID,
                    'SROOfficeListID': SROOfficeListID
                },
                dataSrc: function (json) {
                    unBlockUI();
                    if (json.errorMessage != null) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            callback: function () {
                                //alert('1');
                                if (json.serverError != undefined) {
                                    //alert('2');
                                    window.location.href = "/Home/HomePage"

                                } else {
                                    //alert('3');
                                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                        $('#DailyReceiptsID').trigger('click');

                                    $("#DailyReceiptsID").DataTable().clear().destroy();
                                   
                                    $("#EXCELSPANID").html('');
                                }
                                //$("#DtlsSearchParaListCollapse").trigger('click');
                            }
                        });
                    }
                    else {
                        //alert('4');

                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                            $('#DtlsSearchParaListCollapse').trigger('click');
                        //DailyReceiptDetailsTable.columns.adjust().draw();
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
                    //$.unblockUI();
                },
                beforeSend: function () {
                    blockUI('loading data.. please wait...');
                    // Added by SB on 22-3-2019 at 11:06 am
                    var searchString = $('#DailyReceiptsID_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;

                        if (!regexToMatch.test(searchString)) {
                            $("#DailyReceiptsID_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                //    $('#menuDetailsListTable_filter input').val('');

                                DailyReceiptDetailsTable.search('').draw();
                                $("#DailyReceiptsID_filter input").prop("disabled", false);

                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
            },

            serverSide: true,
            // pageLength: 100,
            "scrollX": "800px",
            "scrollY": "300px",
            scrollCollapse: true,
            bPaginate: true,
            bLengthChange: true,
            // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
            //"pageLength": 10,
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

            columnDefs: [

                { targets: [0], orderable: false },
                { targets: [1], orderable: false },
                { targets: [2], orderable: false }
              
            ],

            columns: [
                { data: "SerialNumber", "searchable": true, "visible": true, "name": "SerialNumber" },
                { data: "TableName", "searchable": true, "visible": true, "name": "TableName" },
                { data: "rowCount", "searchable": true, "visible": true, "name": "rowCount" }                
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
            // DailyReceiptDetailsTable.columns.adjust().draw();

        });

        
    });

});

//function PDFDownloadFun(FromDate, ToDate, SROOfficeID, DROfficeID, ModuleID, FeeTypeID) {
//    window.location.href = '/MISReports/DailyReceiptDetails/ExportDailyReceiptDetailsToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeID + "&DROfficeID=" + DROfficeID + "&ModuleID=" + ModuleID + "&FeeTypeID=" + FeeTypeID + "&selectedDistrict=" + selectedDistrictText + "&SelectedSRO=" + SelectedSROText;
//}

function EXCELDownloadFun(DBName, SROID) {
    window.location.href = '/MISReports/DataTransmissionDetails/ExportDataTransmissionDetailsToExcel?DBName=' + DBName +"&SROID=" + SROID;
}