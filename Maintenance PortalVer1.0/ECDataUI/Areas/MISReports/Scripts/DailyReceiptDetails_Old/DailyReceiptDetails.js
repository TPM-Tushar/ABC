
var token = '';
var header = {};
var SelectedDistrictText;
var SelectedSROText;
$(document).ready(function () {
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
    });



    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);

    });

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var TodaysDate = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();
    var FromDate;
    var ToDate;
    var SROOfficeID;
    var DROfficeID;
    var ModuleID;
    var FeeTypeID;
    var SelectedDistrict;
    var SelectedSRO;

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
    }).datepicker("setDate", TodaysDt);


    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", StartDtOfCurrentMonth);


    $('#DROOfficeListID').change(function () {
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/DailyReceiptDetails/GetSROOfficeListByDistrictID',
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
                        //SROOfficeList
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

    //$("#SearchBtn").click(function () {
    //    if ($('#DROOfficeListID').val() < "0" || $('#DROOfficeListID').val() < "0") {
    //        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any District.</span>');
    //        return;
    //    }
    //    else if ($('#SROOfficeListID').val() < "0" || $('#SROOfficeListID').val() < "0") {
    //        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any SRO.</span>');
    //        return;

    //    }
    //    //else if ($('#ModuleNameListID').val() < "0") {
    //    //    alert($('#ModuleNameListID').val());
    //    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Module.</span>');
    //    //    return;
    //    //}
    //    //else if ($('#FeeTypeListID').val() == "" || $('#FeeTypeListID').val() < "0") {
    //    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Fee Type.</span>');
    //    //    return;
    //    //}


    //    if ($.fn.DataTable.isDataTable("#DailyReceiptsID")) {
    //        $("#DailyReceiptsID").DataTable().clear().destroy();
    //    }


    //    if ($.fn.DataTable.isDataTable("#DailyReceiptsID")) {
    //        //$("#DailyReceiptsTableID").dataTable().fnDestroy();
    //        //$("#DtlsSearchParaListCollapse").trigger('click');
    //        $("#DailyReceiptsID").DataTable().clear().destroy();
    //    }
    //    FromDate = $("#txtFromDate").val();
    //    ToDate = $("#txtToDate").val();
    //    SROOfficeID = $("#SROOfficeListID option:selected").val();
    //    DROfficeID = $("#DROOfficeListID option:selected").val();
    //    ModuleID = $("#ModuleNameListID option:selected").val();
    //    FeeTypeID = $("#FeeTypeListID option:selected").val();
    //    selectedDistrictText = $("#DROOfficeListID option:selected").text();

    //    SelectedSROText = $("#SROOfficeListID option:selected").text();

    //    var DailyReceiptDetailsTable = $('#DailyReceiptsID').DataTable({
    //        ajax: {
    //            url: '/MISReports/DailyReceiptDetails/LoadDailyReceiptDetailsTable',
    //            type: "POST",
    //            headers: header,
    //            "autoWidth": true,
    //            data: {
    //                'fromDate': FromDate, 'ToDate': ToDate, 'SROOfficeID': SROOfficeID, 'DROfficeID': DROfficeID, 'ModuleID': ModuleID, 'FeeTypeID': FeeTypeID
    //            },
    //            dataSrc: function (json) {
    //                unBlockUI();
    //                if (json.errorMessage != null) {
    //                    bootbox.alert({
    //                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
    //                        callback: function () {
    //                            //alert('1');
    //                            if (json.serverError != undefined) {
    //                                //alert('2');
    //                                window.location.href = "/Home/HomePage"

    //                            } else {
    //                                //alert('3');
    //                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
    //                                    $('#DtlsSearchParaListCollapse').trigger('click');

    //                                $("#DailyReceiptsTableID").DataTable().clear().destroy();
    //                                //$("#PDFSPANID").html('');
    //                                $("#EXCELSPANID").html('');
    //                            }
    //                            //$("#DtlsSearchParaListCollapse").trigger('click');
    //                        }
    //                    });
    //                }
    //                else {
    //                    //alert('4');

    //                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
    //                        $('#DtlsSearchParaListCollapse').trigger('click');
    //                    //DailyReceiptDetailsTable.columns.adjust().draw();
    //                }
    //                unBlockUI();
    //                return json.data;
    //            },
    //            error: function () {
    //                unBlockUI();
    //                bootbox.alert({
    //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
    //                    callback: function () {
    //                    }
    //                });
    //                //$.unblockUI();
    //            },
    //            beforeSend: function () {
    //                blockUI('loading data.. please wait...');
    //                // Added by SB on 22-3-2019 at 11:06 am
    //                var searchString = $('#DailyReceiptsID_filter input').val();
    //                if (searchString != "") {
    //                    var regexToMatch = /^[^<>]+$/;

    //                    if (!regexToMatch.test(searchString)) {
    //                        $("#DailyReceiptsID_filter input").prop("disabled", true);
    //                        bootbox.alert('Please enter valid Search String ', function () {
    //                            //    $('#menuDetailsListTable_filter input').val('');

    //                            DailyReceiptDetailsTable.search('').draw();
    //                            $("#DailyReceiptsID_filter input").prop("disabled", false);

    //                        });
    //                        unBlockUI();
    //                        return false;
    //                    }
    //                }
    //            }
    //        },

    //        serverSide: true,
    //        // pageLength: 100,
    //        "scrollX": "800px",
    //        "scrollY": "300px",
    //        scrollCollapse: true,
    //        bPaginate: true,
    //        bLengthChange: true,
    //        // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
    //        //"pageLength": 10,
    //        //sScrollXInner: "150%",
    //        bInfo: true,
    //        info: true,
    //        bFilter: false,
    //        searching: true,
    //        // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
    //        // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
    //        //dom: 'lBfrtip',
    //        "destroy": true,
    //        "bAutoWidth": true,
    //        "bScrollAutoCss": true,

    //        columnDefs: [

    //            { targets: [0], orderable: false },
    //            { targets: [1], orderable: false, "className": "text-left" },
    //            { targets: [2], orderable: false },
    //            { orderable: false, targets: [3] },
    //            { targets: [4], "className": "text-left", orderable: false },
    //            { orderable: false, targets: [5], orderable: false },
    //            { orderable: false, targets: [6], orderable: false },
    //            { orderable: false, targets: [7], orderable: false },
    //            { orderable: false, targets: [8], orderable: false },
    //            { targets: [9], "className": "text-left", orderable: false },
    //            { targets: [10], orderable: false },
    //            { targets: [11], "className": "text-left", orderable: false },
    //            { targets: [12], "className": "text-left", orderable: false },
    //            { targets: [13],  orderable: false },
    //            { targets: [14], "className": "text-right", orderable: false },
    //            { targets: [15], "className": "text-right", orderable: false }
    //        ],

    //        columns: [
    //            { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
    //            { data: "SroName", "searchable": true, "visible": true, "name": "SroName" },
    //            { data: "PresentDateTime", "searchable": true, "visible": true, "name": "PresentDateTime" },
    //            { data: "DocumentNumber", "searchable": true, "visible": true, "name": "DocumentNumber" },
    //            { data: "ArticleName", "searchable": true, "visible": true, "name": "ArticleName" },
    //            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
    //            { data: "MarriageCaseNumber", "searchable": true, "visible": true, "name": "MarriageCaseNumber" },
    //            { data: "NoticeNumber", "searchable": true, "visible": true, "name": "NoticeNumber" },
    //            { data: "ReceiptNumber", "searchable": true, "visible": true, "name": "ReceiptNumber" },
    //            { data: "DescriptionEnglish", "searchable": true, "visible": true, "name": "DescriptionEnglish", "autoWidth": false },
    //            { data: "DateOfReceipt", "searchable": true, "visible": true, "name": "DateOfReceipt" },
    //            { data: "Description", "searchable": true, "visible": true, "name": "Description" },
    //            { data: "StampType", "searchable": true, "visible": true, "name": "StampType" },
    //            { data: "DDChallanNo", "searchable": true, "visible": true, "name": "DDChallanNo" },
    //            { data: "Amount", "searchable": true, "visible": true, "name": "Amount" },
    //            { data: "AmountPaid", "searchable": true, "visible": true, "name": "AmountPaid" },
    //        ],
    //        fnInitComplete: function (oSettings, json) {
    //            //alert('in fnInitComplete');
    //            //$("#PDFSPANID").html(json.PDFDownloadBtn);
    //            $("#EXCELSPANID").html(json.ExcelDownloadBtn);
    //        },
    //        preDrawCallback: function () {
    //            unBlockUI();
    //        },
    //        fnRowCallback: function (nRow, aData, iDisplayIndex) {
    //            unBlockUI();
    //            return nRow;
    //        },

    //        drawCallback: function (oSettings) {
    //            //responsiveHelper.respond();
    //            unBlockUI();
    //        },
    //        // DailyReceiptDetailsTable.columns.adjust().draw();

    //    });

    //    //This (if else if) block is added to hide some specific columns which are not needed for a specific module
    //    if (ModuleID == "1") {//Document Registration
    //        DailyReceiptDetailsTable.columns([2]).visible(false);
    //        DailyReceiptDetailsTable.columns([6]).visible(false);
    //        DailyReceiptDetailsTable.columns([7]).visible(false);
    //        DailyReceiptDetailsTable.columns([9]).visible(false);
    //        DailyReceiptDetailsTable.columns([14]).visible(false);
    //        DailyReceiptDetailsTable.columns([12]).visible(false);
    //        DailyReceiptDetailsTable.columns([13]).visible(false);
    //    }
    //    else if (ModuleID == "0") {//For ALL
    //        //DailyReceiptDetailsTable.columns([1]).visible(false);
    //        DailyReceiptDetailsTable.columns([2]).visible(false);
    //        DailyReceiptDetailsTable.columns([3]).visible(false);
    //        DailyReceiptDetailsTable.columns([4]).visible(false);
    //        DailyReceiptDetailsTable.columns([5]).visible(false);
    //        DailyReceiptDetailsTable.columns([6]).visible(false);
    //        DailyReceiptDetailsTable.columns([7]).visible(false);
    //        DailyReceiptDetailsTable.columns([14]).visible(false);
    //        DailyReceiptDetailsTable.columns([13]).visible(false);
    //        DailyReceiptDetailsTable.columns([12]).visible(false);
    //    }
    //    else if (ModuleID == "5") {//For Others
    //        DailyReceiptDetailsTable.columns([2]).visible(false);
    //        DailyReceiptDetailsTable.columns([4]).visible(false);
    //        DailyReceiptDetailsTable.columns([5]).visible(false);
    //        DailyReceiptDetailsTable.columns([6]).visible(false);
    //        DailyReceiptDetailsTable.columns([7]).visible(false);
    //        DailyReceiptDetailsTable.columns([3]).visible(false);
    //        DailyReceiptDetailsTable.columns([13]).visible(false);
    //        DailyReceiptDetailsTable.columns([14]).visible(false);
    //        DailyReceiptDetailsTable.columns([12]).visible(false);
    //    }
    //    else if (ModuleID == "4") { //Stamp Duty
    //        DailyReceiptDetailsTable.columns([4]).visible(false);
    //        DailyReceiptDetailsTable.columns([6]).visible(false);
    //        DailyReceiptDetailsTable.columns([7]).visible(false);
    //        DailyReceiptDetailsTable.columns([8]).visible(false);
    //        DailyReceiptDetailsTable.columns([9]).visible(false);
    //        DailyReceiptDetailsTable.columns([10]).visible(false);
    //        DailyReceiptDetailsTable.columns([11]).visible(false);
    //        DailyReceiptDetailsTable.columns([15]).visible(false);
    //    }
    //    //else if (ModuleID == "2") {
    //    //    DailyReceiptDetailsTable.columns([0]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([1]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([2]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([3]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([5]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([7]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([11]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([9]).visible(false);


    //    //}
    //    //else if (ModuleID == "3") {
    //    //    DailyReceiptDetailsTable.columns([0]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([1]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([2]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([3]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([4]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([7]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([11]).visible(false);
    //    //    DailyReceiptDetailsTable.columns([9]).visible(false);

    //    //}

    //    //var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //    //if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
    //    //    $('#DtlsSearchParaListCollapse').trigger('click');

    //    //DailyReceiptDetailsTable.columns.adjust().draw();

    //});

    $("#excelDownload").click(function () {
        FromDate = $("#txtFromDate").val();
        ToDate = $("#txtToDate").val();
        SROOfficeID = $("#SROOfficeListID option:selected").val();
        DROfficeID = $("#DROOfficeListID option:selected").val();
        ModuleID = $("#ModuleNameListID option:selected").val();
        FeeTypeID = $("#FeeTypeListID option:selected").val();
        selectedDistrictText = $("#DROOfficeListID option:selected").text();
        SelectedSROText = $("#SROOfficeListID option:selected").text();

        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/DailyReceiptDetails/ValidateSearchParameters',
            data: { "FromDate": FromDate, "ToDate": ToDate, "SROOfficeListID": SROOfficeID, "DROfficeID": DROfficeID, "ModuleID": ModuleID, "FeeTypeID": FeeTypeID },
            type: "GET",
            success: function (data) {
                if (data.success == true) {
                    window.location.href = '/MISReports/DailyReceiptDetails/ExportDailyReceiptDetailsToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeID + "&DROfficeID=" + DROfficeID + "&ModuleID=" + ModuleID + "&FeeTypeID=" + FeeTypeID + "&SelectedDistrict=" + selectedDistrictText + "&SelectedSRO=" + SelectedSROText;
                    unBlockUI();
                }
                else {
                    unBlockUI();
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {                           
                        });
                }
            },
            error: function (xhr) {
                 unBlockUI();
            }
        });
    });
});

//function PDFDownloadFun(FromDate, ToDate, SROOfficeID, DROfficeID, ModuleID, FeeTypeID) {
//    window.location.href = '/MISReports/DailyReceiptDetails/ExportDailyReceiptDetailsToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeID + "&DROfficeID=" + DROfficeID + "&ModuleID=" + ModuleID + "&FeeTypeID=" + FeeTypeID + "&selectedDistrict=" + selectedDistrictText + "&SelectedSRO=" + SelectedSROText;
//}

//function EXCELDownloadFun(FromDate, ToDate, SROOfficeID, DROfficeID, ModuleID, FeeTypeID) {
//    window.location.href = '/MISReports/DailyReceiptDetails/ExportDailyReceiptDetailsToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeID + "&DROfficeID=" + DROfficeID + "&ModuleID=" + ModuleID + "&FeeTypeID=" + FeeTypeID + "&SelectedDistrict=" + selectedDistrictText + "&SelectedSRO=" + SelectedSROText;
//}