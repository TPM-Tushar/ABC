
var token = '';
var header = {};
var SelectedSROText;
var txtFromDate;
var txtToDate;
var SroID;
var SelectedDistrictText;
var DistrictID;

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/MISReports/PendingDocumentsSummary/GetSROOfficeListByDistrictID',
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

    $("#SearchBtn").click(function () {
        if ($.fn.DataTable.isDataTable("#DailyReceiptDetailsTable")) {
            $("#DailyReceiptDetailsTable").DataTable().clear().destroy();
        }

        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        SroID = $("#SROOfficeListID option:selected").val();
        SelectedSROText = $("#SROOfficeListID option:selected").text();
        SelectedDistrictText = $("#DROOfficeListID option:selected").text();
        DistrictID = $("#DROOfficeListID option:selected").val();


        if (txtFromDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
            return;
        }
        else if (txtToDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;

        }

        var PendingDocsTable = $('#PendingDocsSummaryTable').DataTable({
            ajax: {
                url: '/MISReports/PendingDocumentsSummary/LoadPendingDocumentSummaryDataTable',
                type: "POST",
                headers: header,
                data: {
                    'FromDate': txtFromDate, 'ToDate': txtToDate, 'SroID': SroID, 'DistrictID': DistrictID
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
                                    $("#PendingDocsSummaryTable").DataTable().clear().destroy();
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
                    // Added by SB on 22-3-2019 at 11:06 am
                    var searchString = $('#PendingDocsSummaryTable_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#PendingDocsSummaryTable_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                //    $('#menuDetailsListTable_filter input').val('');
                                PendingDocsTable.search('').draw();
                                $("#PendingDocsSummaryTable_filter input").prop("disabled", false);
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
            "scrollX": true,
            "scrollY": "30vh",
            //"scrollY": "300px",
            "scrollCollapse": true,
            bPaginate: true,
            bLengthChange: true,
            "lengthMenu": [[350], ["All"]],
            "pageLength": 350,
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
                { orderable: false, targets: [0] }
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

                { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                { data: "District", "searchable": true, "visible": true, "name": "District" },
                { data: "SRO", "searchable": true, "visible": true, "name": "SRO" },
                { data: "NoOfDocsPresented", "searchable": true, "visible": true, "name": "NoOfDocsPresented" },
                { data: "NoOfDocsRegistered", "searchable": true, "visible": true, "name": "NoOfDocsRegistered" },
                { data: "Str_NoOfDocsKeptPending", "searchable": true, "visible": true, "name": "NoOfDocsKeptPending" },
                // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021        
                //{ data: "Str_DocsNotRegdNotPending", "searchable": true, "visible": true, "name": "DocsNotRegdNotPending" }
                { data: "Str_Number_of_Pending_later_finalized_Docs", "searchable": true, "visible": true, "name": "NoOfPendingLaterFinalizedDocs" }
                // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021			

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
                MergeGridCells();
                unBlockUI();
            },
        });



        PendingDocsTable.columns.adjust().draw();




    });




});

function GetPendingDocsSummaryDetails(columnName, SROCode, EncryptedSROName, EncryptedDistrict) {
    //$('#MasterTableModel').load('/KaveriIntegration/KaveriIntegration/OtherTableDetails', { columnName: columnName });
    if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
        $("#AnywhereECTable").DataTable().clear().destroy();
    }
    //To Get Same Data in PDF and Excel
    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    DistrictID = $("#DROOfficeListID option:selected").val();

    if (txtFromDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
        return;
    }
    else if (txtToDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
        return;
    }

    if ($.fn.DataTable.isDataTable("#PendingDocsdetailsTable")) {
        $("#PendingDocsdetailsTable").DataTable().clear().destroy();
    }

    var PendingDocsdetailsTable = $('#PendingDocsdetailsTable').DataTable({
        ajax: {
            url: '/MISReports/PendingDocumentsSummary/LoadPendingDocumentDetailsDataTable',
            type: "POST",
            headers: header,
            data: {
                'FromDate': txtFromDate,
                'ToDate': txtToDate,
                'DistrictID': DistrictID,
                'columnName': columnName,
                'SROCode': SROCode,
                'EncryptedSROName': EncryptedSROName,
                'EncryptedDistrict': EncryptedDistrict
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
                                var classToRemove = $('#DtlsToggleIconSearchPara1').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse1').trigger('click');
                                $("#PendingDocsdetailsTable").DataTable().clear().destroy();
                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara1').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse1').trigger('click');
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
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#PendingDocsdetailsTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#PendingDocsdetailsTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            PendingDocsdetailsTable.search('').draw();
                            $("#PendingDocsdetailsTable_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        $('#MasterTableModel').modal('hide');
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        "scrollX": true,
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
        columnDefs: [
            { orderable: false, targets: [0] },
            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] },
            { orderable: false, targets: [3] },
            { orderable: false, targets: [4] },
            { orderable: false, targets: [5] },

            // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021        
            //{ orderable: false, targets: [6] },

            // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021	

        ],

        columns: [
            { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
            { data: "SroName", "searchable": true, "visible": true, "name": "SroName" },
            { data: "PendingNumber", "searchable": true, "visible": true, "name": "PendingNumber" },
            { data: "PendingDate", "searchable": true, "visible": true, "name": "PendingDate" },
            { data: "ReasonOfPending", "searchable": true, "visible": true, "name": "ReasonOfPending" },
            //{ data: "IsCleared", "searchable": true, "visible": true, "name": "IsCleared" },
            //{ data: "WhetherRegistered", "searchable": true, "visible": true, "name": "WhetherRegistered" },
            // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021        
            //{
            //    "data": "IsCleared", render: function (data, type, row) {
            //        return (data == true) ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";
            //    }
            //},
            // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021	

            //{
            //    "data": "WhetherRegistered", render: function (data, type, row) {
            //        return (data == true) ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";
            //    }
            //} 
            { data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" }
        ],
        fnInitComplete: function (oSettings, json) {
            $("#SROSpanID1").html('');
            $("#SROSpanID1").html(json.SRO);
            $("#DistrictSpanID1").html('');
            $("#DistrictSpanID1").html(json.District);
            $("#EXCELSPANID1").html('');
            $("#EXCELSPANID1").html(json.ExcelDownloadBtn);
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

    //alert(columnName);


    //$('#DtlsSearchParaListCollapse1').on('shown.bs.collapse', function () {
    //    $($.fn.dataTable.tables(true)).DataTable()
    //        .columns.adjust();
    //});

    // Adjust Table Header but calling two times on first time it need to be changed 1-11-2019 
    //var table = $('#AnywhereECTable').DataTable();
    //table.columns.adjust().draw();
    PendingDocsdetailsTable.columns.adjust().draw();


    //For Displaying SRO name, From date and to date on model popup.   
    $("#FromDateSpanID1").html(txtFromDate);
    $("#ToDateSpanID1").html(txtToDate);




    // Show Model Popup
    $('#MasterTableModel').modal('show');
}


function GetPendingDocsDocsNotRegdNotPendingDetails(columnName, SROCode, EncryptedSROName, EncryptedDistrict) {
    //$('#MasterTableModel').load('/KaveriIntegration/KaveriIntegration/OtherTableDetails', { columnName: columnName });
    if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
        $("#AnywhereECTable").DataTable().clear().destroy();
    }
    //To Get Same Data in PDF and Excel
    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    DistrictID = $("#DROOfficeListID option:selected").val();

    if (txtFromDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
        return;
    }
    else if (txtToDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
        return;
    }

    if ($.fn.DataTable.isDataTable("#PendingDocsdetailsTable")) {
        $("#PendingDocsdetailsTable").DataTable().clear().destroy();
    }

    var DocsNotPendingNotRegddetailsTable = $('#DocsNotPendingNotRegdTable').DataTable({
        ajax: {
            url: '/MISReports/PendingDocumentsSummary/LoadPendingDocumentDetailsDataTable',
            type: "POST",
            headers: header,
            data: {
                'FromDate': txtFromDate,
                'ToDate': txtToDate,
                'DistrictID': DistrictID,
                'columnName': columnName,
                'SROCode': SROCode,
                'EncryptedSROName': EncryptedSROName,
                'EncryptedDistrict': EncryptedDistrict
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
                                var classToRemove = $('#DtlsToggleIconSearchPara1').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse1').trigger('click');
                                $("#DocsNotPendingNotRegdTable").DataTable().clear().destroy();
                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara1').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse1').trigger('click');
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
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#DocsNotPendingNotRegdTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#DocsNotPendingNotRegdTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            PendingDocsdetailsTable.search('').draw();
                            $("#DocsNotPendingNotRegdTable_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        $('#DocsNotRegdNotPendingModel').modal('hide');
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        "scrollX": true,
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
            { data: "SroName", "searchable": true, "visible": true, "name": "SroName" },
            { data: "RegArticle", "searchable": true, "visible": true, "name": "RegArticle" },
            { data: "DocumentNumber", "searchable": true, "visible": true, "name": "DocumentNumber" },
            { data: "PresentDate", "searchable": true, "visible": true, "name": "PresentDate" },
            { data: "ConsiderationAmount", "searchable": true, "visible": true, "name": "ConsiderationAmount" },
            { data: "WithdrawalDate", "searchable": true, "visible": true, "name": "WithdrawalDate" }

        ],
        fnInitComplete: function (oSettings, json) {
            $("#SROSpanID2").html('');
            $("#SROSpanID2").html(json.SRO);
            $("#DistrictSpanID2").html('');
            $("#DistrictSpanID2").html(json.District);
            $("#EXCELSPANID2").html('');
            $("#EXCELSPANID2").html(json.ExcelDownloadBtn);
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

    //alert(columnName);


    //$('#DtlsSearchParaListCollapse1').on('shown.bs.collapse', function () {
    //    $($.fn.dataTable.tables(true)).DataTable()
    //        .columns.adjust();
    //});

    // Adjust Table Header but calling two times on first time it need to be changed 1-11-2019 
    DocsNotPendingNotRegddetailsTable.columns.adjust().draw();

    //var table = $('#AnywhereECTable').DataTable();
    //table.columns.adjust().draw();

    //For Displaying SRO name, From date and to date on model popup.   
    $("#FromDateSpanID2").html(txtFromDate);
    $("#ToDateSpanID2").html(txtToDate);




    // Show Model Popup
    $('#DocsNotRegdNotPendingModel').modal('show');
}





function MergeGridCells() {
    var dimension_cells = new Array();
    var dimension_col = null;
    var columnCount = $("#PendingDocsSummaryTable tr:first th").length;
    //for (dimension_col = 0; dimension_col < columnCount; dimension_col++) {
    // first_instance holds the first instance of identical td
    var first_instance = null;
    var firstInstanceOfFirstCol = null;
    var rowspan = 1;
    var srNoCount = 0;
    // iterate through rows
    $("#PendingDocsSummaryTable").find('tr').each(function () {

        // find the td of the correct column (determined by the dimension_col set above)
        var dimension_td = $(this).find('td:nth-child(' + 2 + ')');
        var dimension_td1 = $(this).find('td:nth-child(' + 1 + ')');

        if (first_instance == null) {
            // must be the first row
            first_instance = dimension_td;
            firstInstanceOfFirstCol = dimension_td1;
            firstInstanceOfFirstCol.text = srNoCount++;

        } else if (dimension_td.text() == first_instance.text()) {
            // the current td is identical to the previous
            // remove the current td
            dimension_td.remove();
            dimension_td1.remove();
            ++rowspan;
            // increment the rowspan attribute of the first instance
            first_instance.attr('rowspan', rowspan);
            firstInstanceOfFirstCol.attr('rowspan', rowspan);

        } else {
            // this cell is different from the last

            first_instance = dimension_td;
            firstInstanceOfFirstCol = dimension_td1;
            dimension_td1.html(srNoCount++);
            //firstInstanceOfFirstCol.data(srNoCount++).draw();
            //alert(srNoCount);
            rowspan = 1;
        }
    });
    //}
}

function EXCELDownloadFun() {
    window.location.href = '/MISReports/PendingDocumentsSummary/ExportPendingDocumentSummaryToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + SroID + "&SelectedSRO=" + SelectedSROText + "&SelectedDistrict=" + SelectedDistrictText;
}

function PendingDocsDetailsEXCELDownloadFun(EncryptedColName, EncryptedSROCODE, EncryptedSROName, EncryptedDistrict) {
    window.location.href = '/MISReports/PendingDocumentsSummary/ExportPendingDocumentDetailsToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + EncryptedSROCODE + "&SelectedSRO=" + EncryptedSROName + "&SelectedDistrict=" + EncryptedDistrict + "&EncryptedColName=" + EncryptedColName;
}


function GetPendingLaterFinalizedDocs(columnName, SROCode, EncryptedSROName, EncryptedDistrict) {
    //$('#MasterTableModel').load('/KaveriIntegration/KaveriIntegration/OtherTableDetails', { columnName: columnName });
    if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
        $("#AnywhereECTable").DataTable().clear().destroy();
    }
    //To Get Same Data in PDF and Excel
    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    DistrictID = $("#DROOfficeListID option:selected").val();

    if (txtFromDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
        return;
    }
    else if (txtToDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
        return;
    }

    if ($.fn.DataTable.isDataTable("#PendingDocsdetailsTable")) {
        $("#PendingDocsdetailsTable").DataTable().clear().destroy();
    }

    var NumberOfPendingLaterFinalizedTable = $('#NumberOfPendingLaterFinalizedTable').DataTable({
        ajax: {
            url: '/MISReports/PendingDocumentsSummary/LoadPendingDocumentDetailsDataTable',
            type: "POST",
            headers: header,
            data: {
                'FromDate': txtFromDate,
                'ToDate': txtToDate,
                'DistrictID': DistrictID,
                'columnName': columnName,
                'SROCode': SROCode,
                'EncryptedSROName': EncryptedSROName,
                'EncryptedDistrict': EncryptedDistrict
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
                                var classToRemove = $('#DtlsToggleIconSearchPara1').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse1').trigger('click');
                                $("#NumberOfPendingLaterFinalizedTable").DataTable().clear().destroy();
                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara1').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse1').trigger('click');
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
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#NumberOfPendingLaterFinalizedTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#NumberOfPendingLaterFinalizedTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            PendingDocsdetailsTable.search('').draw();
                            $("#NumberOfPendingLaterFinalizedTable_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        $('#NumberOfPendingLaterFinalizedModel').modal('hide');
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        "scrollX": true,
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
            { data: "SroName", "searchable": true, "visible": true, "name": "SroName" },
            { data: "PendingNumber", "searchable": true, "visible": true, "name": "PendingNumber" },
            { data: "PendingDate", "searchable": true, "visible": true, "name": "PendingDate" },
            { data: "ReasonOfPending", "searchable": true, "visible": true, "name": "ReasonOfPending" },
            //{ data: "WhetherRegistered", "searchable": true, "visible": true, "name": "WhetherRegistered" },         

            {
                "data": "WhetherRegistered", render: function (data, type, row) {
                    return (data == true) ? "Yes" : "No";
                }
            },
            { data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" }

        ],
        fnInitComplete: function (oSettings, json) {
            $("#SROSpanID3").html('');
            $("#SROSpanID3").html(json.SRO);
            $("#DistrictSpanID3").html('');
            $("#DistrictSpanID3").html(json.District);
            $("#EXCELSPANID3").html('');
            $("#EXCELSPANID3").html(json.ExcelDownloadBtn);
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

    //alert(columnName);


    //$('#DtlsSearchParaListCollapse1').on('shown.bs.collapse', function () {
    //    $($.fn.dataTable.tables(true)).DataTable()
    //        .columns.adjust();
    //});

    // Adjust Table Header but calling two times on first time it need to be changed 1-11-2019 
    NumberOfPendingLaterFinalizedTable.columns.adjust().draw();

    //var table = $('#AnywhereECTable').DataTable();
    //table.columns.adjust().draw();

    //For Displaying SRO name, From date and to date on model popup.   
    $("#FromDateSpanID3").html(txtFromDate);
    $("#ToDateSpanID3").html(txtToDate);




    // Show Model Popup
    $('#NumberOfPendingLaterFinalizedModel').modal('show');
}

