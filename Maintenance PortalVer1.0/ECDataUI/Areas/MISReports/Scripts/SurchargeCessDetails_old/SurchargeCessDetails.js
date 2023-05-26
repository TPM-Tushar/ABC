var FromDate;
var ToDate;
var SROOfficeListID;
var NatureOfDocumentListID;
var SROSelected;
var DistrictSelected;
//Global variables.
var token = '';
var header = {};
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DROOfficeListID').change(function () {
        //if ($('#DROOfficeListID').val() == "0") {
        //    if ($.fn.DataTable.isDataTable("#SurchargeCessDetailsID")) {
        //        $("#SurchargeCessDetailsID").DataTable().clear().destroy();
        //    }
        //    bootbox.alert({
        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any District." + '</span>',
        //        callback: function () {
        //            var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        //            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        //                $('#DtlsSearchParaListCollapse').trigger('click');

        //            // Added by shubham bhagat on 18-07-2019
        //            $("#PDFSPANID").html('');
        //            $("#EXCELSPANID").html('');
        //        }
        //    });
        //}
        //else {
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/SurchargeCessDetails/GetSROOfficeListByDistrictID',
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
                        // Added by shubham bhagat on 18-07-2019
                        // commented below line
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
        //}
    });

    $('#DROOfficeListID').focus();
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

    //$("#SearchBtn").click(function () {
    //    if ($.fn.DataTable.isDataTable("#SurchargeCessDetailsID")) {
    //        $("#SurchargeCessDetailsID").DataTable().clear().destroy();
    //    }

    //    FromDate = $("#txtFromDate").val();
    //    ToDate = $("#txtToDate").val();
    //    SROOfficeListID = $("#SROOfficeListID option:selected").val();
    //    NatureOfDocumentListID = $("#NatureOfDocumentListID option:selected").val();
    //    DROfficeID = $("#DROOfficeListID option:selected").val();
    //    SROSelected = $("#SROOfficeListID option:selected").text();
    //    DistrictSelected = $("#DROOfficeListID option:selected").text();
    //    //if (DROfficeID == "0") {
    //    //    if ($.fn.DataTable.isDataTable("#SurchargeCessDetailsID")) {
    //    //        $("#SurchargeCessDetailsID").DataTable().clear().destroy();
    //    //    }
    //    //    bootbox.alert({
    //    //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any District." + '</span>',
    //    //        callback: function () {
    //    //            var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //    //            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
    //    //                $('#DtlsSearchParaListCollapse').trigger('click');

    //    //            // Added by shubham bhagat on 18-07-2019
    //    //            $("#PDFSPANID").html('');
    //    //            $("#EXCELSPANID").html('');
    //    //        }
    //    //    });
    //    //}
    //    //else if (SROOfficeListID == "0") {
    //    //    if ($.fn.DataTable.isDataTable("#SurchargeCessDetailsID")) {
    //    //        $("#SurchargeCessDetailsID").DataTable().clear().destroy();
    //    //    }
    //    //    bootbox.alert({
    //    //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any SRO." + '</span>',
    //    //        callback: function () {
    //    //            var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //    //            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
    //    //                $('#DtlsSearchParaListCollapse').trigger('click');

    //    //            // Added by shubham bhagat on 18-07-2019
    //    //            $("#PDFSPANID").html('');
    //    //            $("#EXCELSPANID").html('');
    //    //        }
    //    //    });
    //    //}
    //    //else {
    //        if ($.fn.DataTable.isDataTable("#SurchargeCessDetailsID")) {              
    //            $("#SurchargeCessDetailsID").DataTable().clear().destroy();
    //        }
    //        var tableIndexReports = $('#SurchargeCessDetailsID').DataTable({

    //            ajax: {
    //                url: '/MISReports/SurchargeCessDetails/SurchargeCessDetails',
    //                type: "POST",
    //                headers: header,
    //                data: {
    //                    'fromDate': FromDate, 'ToDate': ToDate, 'SROOfficeListID': SROOfficeListID, 'NatureOfDocumentListID': NatureOfDocumentListID, 'DROfficeID': DROfficeID
    //                },
    //                dataSrc: function (json) {
    //                    unBlockUI();
    //                    unBlockUI();
    //                    if (json.errorMessage != null) {
    //                        bootbox.alert({
    //                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
    //                            callback: function () {
    //                                if (json.serverError != undefined) {
    //                                    window.location.href = "/Home/HomePage"
    //                                } else {
    //                                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
    //                                        $('#DtlsSearchParaListCollapse').trigger('click');
    //                                    $("#SurchargeCessDetailsID").DataTable().clear().destroy();
    //                                    //$("#PDFSPANID").html('');
    //                                    $("#EXCELSPANID").html('');
    //                                }
    //                            }
    //                        });
    //                    }
    //                    else {
    //                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
    //                            $('#DtlsSearchParaListCollapse').trigger('click');
    //                    }
    //                    unBlockUI();
    //                    return json.data;
    //                },
    //                error: function () {
    //                    unBlockUI();
    //                    bootbox.alert({
    //                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
    //                        callback: function () {
    //                        }
    //                    });
    //                },
    //                beforeSend: function () {
    //                    blockUI('Loading data please wait.');
    //                    // Added by SB on 22-3-2019 at 11:06 am
    //                    var searchString = $('#SurchargeCessDetailsID_filter input').val();
    //                    if (searchString != "") {
    //                        var regexToMatch = /^[^<>]+$/;

    //                        if (!regexToMatch.test(searchString)) {
    //                            $("#SurchargeCessDetailsID_filter input").prop("disabled", true);
    //                            bootbox.alert('Please enter valid Search String ', function () {
    //                                tableIndexReports.search('').draw();
    //                                $("#SurchargeCessDetailsID_filter input").prop("disabled", false);
    //                            });
    //                            unBlockUI();
    //                            return false;
    //                        }
    //                    }
    //                }
    //            },
    //            serverSide: true,
    //            // pageLength: 100,
    //            "scrollX": true,
    //            "scrollY": "300px",
    //            scrollCollapse: true,
    //            bPaginate: true,
    //            bLengthChange: true,
    //            // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
    //            // "pageLength": -1,
    //            //sScrollXInner: "150%",
    //            bInfo: true,
    //            info: true,
    //            bFilter: false,
    //            searching: true,
    //            "destroy": true,
    //            "bAutoWidth": true,
    //            "bScrollAutoCss": true,
    //            columnDefs: [
    //                { orderable: false, targets: [0] },
    //                { orderable: false, targets: [1] },
    //                { orderable: false, targets: [2] },
    //                { orderable: false, targets: [3] },
    //                { orderable: false, targets: [4] },
    //                { orderable: false, targets: [5] },
    //                { orderable: false, targets: [6] },
    //                { orderable: false, targets: [7] },
    //                { orderable: false, targets: [8] },
    //                { orderable: false, targets: [9] },
    //                { orderable: false, targets: [10] },
    //                { orderable: false, targets: [11] },
    //                { orderable: false, targets: [12] },
    //                { orderable: false, targets: [13] },
    //                { orderable: false, targets: [14] }

    //            ],
    //            columns: [
    //                { data: "serialNo", "searchable": true, "visible": true, "name": "serialNo" },
    //                { data: "SroName", "searchable": true, "visible": true, "name": "SroName" },
    //                { data: "ArticleNameE", "searchable": true, "visible": true, "name": "ArticleNameE" },
    //                { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
    //                { data: "PropertyDetails", "searchable": true, "visible": true, "name": "PropertyDetails" },
    //                { data: "VillageNameE", "searchable": true, "visible": true, "name": "VillageNameE" },
    //                { data: "Executant", "searchable": true, "visible": true, "name": "Executant" },
    //                { data: "Claimant", "searchable": true, "visible": true, "name": "Claimant" },
    //                { data: "PropertyValue", "searchable": true, "visible": true, "name": "PropertyValue" },
    //                { data: "GovtDuty", "searchable": true, "visible": true, "name": "GovtDuty", "autoWidth": false },
    //                { data: "AdditionalDuty", "searchable": true, "visible": true, "name": "AdditionalDuty", "autoWidth": false },
    //                { data: "CessDuty", "searchable": true, "visible": true, "name": "CessDuty", "autoWidth": true },
    //                { data: "TotalStumpDuty", "searchable": true, "visible": true, "name": "TotalStumpDuty" },
    //                { data: "PaidStumpDuty", "searchable": true, "visible": true, "name": "PaidStumpDuty" },
    //                { data: "RegisteredDatetime", "searchable": true, "visible": true, "name": "RegisteredDatetime" }
    //            ],
    //            fnInitComplete: function (oSettings, json) {
    //                //$("#PDFSPANID").html(json.PDFDownloadBtn);
    //                $("#EXCELSPANID").html(json.ExcelDownloadBtn);
    //            },
    //            preDrawCallback: function () {
    //                unBlockUI();
    //            },
    //            fnRowCallback: function (nRow, aData, iDisplayIndex) {
    //                unBlockUI();
    //                return nRow;
    //            },
    //            drawCallback: function (oSettings) {
    //                unBlockUI();
    //            },
    //        });
    //  //  }
    //});

    //$("#excelDownload").click(function () {
    //    FromDate = $("#txtFromDate").val();
    //    ToDate = $("#txtToDate").val();
    //    SROOfficeListID = $("#SROOfficeListID option:selected").val();
    //    NatureOfDocumentListID = $("#NatureOfDocumentListID option:selected").val();
    //    DROfficeID = $("#DROOfficeListID option:selected").val();
    //    SROSelected = $("#SROOfficeListID option:selected").text();
    //    DistrictSelected = $("#DROOfficeListID option:selected").text();
    //    window.location.href = '/MISReports/SurchargeCessDetails/ExportSurchargeCessDetailsToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&DROOfficeListID=" + DROfficeID + "&SROSelected=" + SROSelected + "&DistrictSelected=" + DistrictSelected;
    //});
    $("#excelDownload").click(function () {
        FromDate = $("#txtFromDate").val();
        ToDate = $("#txtToDate").val();
        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        NatureOfDocumentListID = $("#NatureOfDocumentListID option:selected").val();
        DROfficeID = $("#DROOfficeListID option:selected").val();
        SROSelected = $("#SROOfficeListID option:selected").text();
        DistrictSelected = $("#DROOfficeListID option:selected").text();
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/SurchargeCessDetails/ValidateSearchParameters',
            data: { "FromDate": FromDate, "ToDate": ToDate, "SROOfficeListID": SROOfficeListID, "DROfficeID": DROfficeID, "NatureOfDocumentListID": NatureOfDocumentListID, "DROOfficeListID": DROfficeID },
            type: "GET",
            success: function (data) {
                if (data.success == true) {
                    window.location.href = '/MISReports/SurchargeCessDetails/ExportSurchargeCessDetailsToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&DROOfficeListID=" + DROfficeID + "&SROSelected=" + SROSelected + "&DistrictSelected=" + DistrictSelected;

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

//function PDFDownloadFun(FromDate, ToDate, SROOfficeListID, NatureOfDocumentListID, DROfficeID) {
//    window.location.href = '/MISReports/SurchargeCessDetails/ExportSurchargeCessDetailsToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&DROOfficeListID=" + DROfficeID + "&SROSelected=" + SROSelected + "&DistrictSelected=" + DistrictSelected;
//}

//function EXCELDownloadFun(FromDate, ToDate, SROOfficeListID, NatureOfDocumentListID, DROfficeID) {
//    window.location.href = '/MISReports/SurchargeCessDetails/ExportSurchargeCessDetailsToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&DROOfficeListID=" + DROfficeID + "&SROSelected=" + SROSelected + "&DistrictSelected=" + DistrictSelected;
//}

