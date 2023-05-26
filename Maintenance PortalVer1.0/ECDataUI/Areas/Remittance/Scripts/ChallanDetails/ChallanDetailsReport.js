
var token = '';
var header = {};
var SelectedType;
var txtDate;
var InstrumentNumber;

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;




    $('#txtDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

    });

    $('#divDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });

    $('#txtDate').datepicker({
        format: 'dd/mm/yyyy'
    }).datepicker("setDate", ToDate);


    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $("#SearchBtn").click(function () {

        if ($.fn.DataTable.isDataTable("#ChallanDetailsTable")) {
            $("#ChallanDetailsTable").DataTable().clear().destroy();
        }

        //txtDate = $("#txtDate").val();
        //SelectedType = $("#TypeListID option:selected").val();
        InstrumentNumber = $("#txtInstrumentNo").val();

        if (InstrumentNumber == null || InstrumentNumber == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Instrument Number.</span>');
            return;
        }
        //if (txtDate == null || txtDate == "") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Date.</span>');
        //    return;
        //}
        //else if (SelectedType == null || SelectedType == "") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Type.</span>');
        //    return;
        //}

        var ChallanDetailsRptTable = $('#ChallanDetailsTable').DataTable({
            ajax: {
                url: '/Remittance/ChallanDetails/GetChallanReportDetails',
                type: "POST",
                headers: header,     
                data: {
                    'InstrumentNumber': InstrumentNumber,//'Date': txtDate,  'Type': SelectedType
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
                                    $("#ChallanDetailsTable").DataTable().clear().destroy();
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
                                ChallanDetailsRptTable.search('').draw();
                                $("#ChallanDetailsTable_filter input").prop("disabled", false);
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
                { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
                { data: "IsPayDoneAtDROffice", "searchable": true, "visible": true, "name": "IsPayDoneAtDROffice" },
                { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName" },
                { data: "ChallanNumber", "searchable": true, "visible": true, "name": "ChallanNumber" },
                { data: "ChallanDate", "searchable": true, "visible": true, "name": "ChallanDate" },
                { data: "Amount", "searchable": true, "visible": true, "name": "Amount" },
                { data: "IsStampPayment", "searchable": true, "visible": true, "name": "IsStampPayment" },
                { data: "IsReceiptPayment", "searchable": true, "visible": true, "name": "IsReceiptPayment" },
                { data: "ReceiptNumber", "searchable": true, "visible": true, "name": "ReceiptNumber" },
                { data: "Receipt_StampPayDate", "searchable": true, "visible": true, "name": "Receipt_StampPayDate" },
                { data: "InsertDateTime", "searchable": true, "visible": true, "name": "InsertDateTime" },
                { data: "ServiceName", "searchable": true, "visible": true, "name": "ServiceName" },
                { data: "DocumentPendingNumber", "searchable": true, "visible": true, "name": "DocumentPendingNumber" },
                { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
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

    function EXCELDownloadFun() {

        window.location.href = '/Remittance/ChallanDetails/ExportChallanDetailsToExcel?InstrumentNumber=' + InstrumentNumber; // + "&Date=" + txtDate + "&Type=" + SelectedType;
    }