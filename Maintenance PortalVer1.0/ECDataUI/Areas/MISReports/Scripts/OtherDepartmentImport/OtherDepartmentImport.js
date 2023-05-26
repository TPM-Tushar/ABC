var token = '';
var header = {};
//var SelectedSROText;
var txtFromDate;
var txtToDate;
var SroID;
var DistrictID;
var ReportListID;
//var LogTypeID;
//var SelectedSROText;
//var SelectedDistrictText;
//var SelectedLogTypeText;
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DROOfficeListID').change(function () {
        if ($('#DROOfficeListID').val() === "0") {
            $('#SROOfficeListID').empty();
            $('#SROOfficeListID').append('<option value="0">All</option>');
        }
        else {
            blockUI('Loading data please wait.');

            $.ajax({
                url: '/MISReports/OtherDepartmentImport/GetSROOfficeListByDistrictID',
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


    $("#SearchBtn").click(function () {
        if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
            $("#AnywhereECTable").DataTable().clear().destroy();
        }
        //To Get Same Data in PDF and Excel
        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        SroID = $("#SROOfficeListID option:selected").val();
        DistrictID = $("#DROOfficeListID option:selected").val();
        ReportListID = $("#ReportListID option:selected").val();

        //LogTypeID = $("#LogTypeId option:selected").val();
        //SelectedSROText = $("#SROOfficeListID option:selected").text();
        //SelectedDistrictText = $("#DROOfficeListID option:selected").text();
        //SelectedLogTypeText = $("#LogTypeId option:selected").text();

        if ($('#DROOfficeListID').val() < "0" || $('#DROOfficeListID').val() === null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select District.</span>');
            return;
        }
        if ($('#SROOfficeListID').val() < "0" || $('#SROOfficeListID').val() === null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select SRO Name.</span>');
            return;
        }
        if ($('#ReportListID').val() === null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Report Name.</span>');
            return;
        }

        if (txtFromDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
            return;
        }
        else if (txtToDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;
        }

        var AnywhereECTable = $('#AnywhereECTable').DataTable({
            ajax: {
                url: '/MISReports/OtherDepartmentImport/OtherDepartmentImportDetails',
                type: "POST",
                headers: header,
                data: {
                    'FromDate': txtFromDate,
                    'ToDate': txtToDate,
                    'DistrictID': DistrictID,
                    'SroID': SroID,
                    'ReportNameID': ReportListID
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
                                    $("#AnywhereECTable").DataTable().clear().destroy();
                                    //$("#PDFSPANID").html('');
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
                    unBlockUI();
                },
                beforeSend: function () {
                    blockUI('Loading data please wait.');
                    var searchString = $('#AnywhereECTable_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#AnywhereECTable_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                AnywhereECTable.search('').draw();
                                $("#AnywhereECTable_filter input").prop("disabled", false);
                            });
                            unBlockUI();
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
                { orderable: false, targets: [6] },
                { orderable: false, targets: [7] },
                { orderable: false, targets: [8] },
                { orderable: false, targets: [9] }//,
                //{ orderable: false, targets: [10] }//,
                //{ orderable: false, targets: [11] },
                //{ orderable: false, targets: [12] }
            ],
            //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration

            columns: [
                { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
                { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
                { data: "ArticleNameE", "searchable": true, "visible": true, "name": "ArticleNameE" },
                { data: "PropertyID", "searchable": true, "visible": true, "name": "PropertyID" },
                { data: "ActionDate", "searchable": true, "visible": true, "name": "ActionDate" },
                { data: "SketchNumber", "searchable": true, "visible": true, "name": "SketchNumber" },
                { data: "ImportedXML", "searchable": true, "visible": true, "name": "ImportedXML" },
                { data: "ExportedXML", "searchable": true, "visible": true, "name": "ExportedXML" },
                { data: "ReferenceNumber", "searchable": true, "visible": true, "name": "ReferenceNumber" },
                { data: "BtnViewSummary", "searchable": true, "visible": true, "name": "BtnViewSummary" },
                { data: "UploadDate", "searchable": true, "visible": true, "name": "UploadDate" },
                //{ data: "WhetherDocumentRegistered", "searchable": true, "visible": true, "name": "WhetherDocumentRegistered" },
                { data: "DateofRegistration", "searchable": true, "visible": true, "name": "DateofRegistration" },
                //Added by mayank on 05/01/2022
                { data: "KaigrRegInsertDate", "searchable": true, "visible": true, "name": "KaigrRegInsertDate" }
                //End of Comment by mayank on 05/01/2022


                //,
                //{ data: "DateofUpload", "searchable": true, "visible": true, "name": "DateofUpload" },
                //{ data: "ArchivedinCD", "searchable": true, "visible": true, "name": "ArchivedinCD" },
                //{ data: "DocumentDeliveryDate", "searchable": true, "visible": true, "name": "DocumentDeliveryDate" },
                //{ data: "DateofRegistration", "searchable": true, "visible": true, "name": "DateofRegistration" },

            ],
            fnInitComplete: function (oSettings, json) {
                //$("#PDFSPANID").html(json.PDFDownloadBtn);
                $("#EXCELSPANID").html('');
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
        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration

        if ($('#ReportListID').val() === "mojini") {
            AnywhereECTable.columns([4]).visible(false);
            AnywhereECTable.columns([3]).visible(false);
            AnywhereECTable.columns([5]).visible(false);
            AnywhereECTable.columns([10]).visible(false);
            $(AnywhereECTable.column(4).header()).html('Property Identification Number');
            $(AnywhereECTable.column(6).header()).html('Sketch Number');
            $(AnywhereECTable.column(9).header()).html('Reference Number');
            //Added by mayank on 05/01/2022
            AnywhereECTable.columns([13]).visible(false);
            //End of Comment by mayank on 05/01/2022
        }
        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
        else if ($('#ReportListID').val() === "fruits") {
            //alert("fruits");
            AnywhereECTable.columns([3]).visible(true);
            AnywhereECTable.columns([4]).visible(true);
            AnywhereECTable.columns([5]).visible(true);
            AnywhereECTable.columns([6]).visible(true);
            AnywhereECTable.columns([10]).visible(true);
            $(AnywhereECTable.column(4).header()).html('Action');
            $(AnywhereECTable.column(6).header()).html('Reference No.');
            $(AnywhereECTable.column(9).header()).html('Form III');
             //Added by mayank on 05/01/2022
            AnywhereECTable.columns([13]).visible(true);
            //End of Comment by mayank on 05/01/2022
        }
        else {
            AnywhereECTable.columns([6]).visible(false);
            //AnywhereECTable.columns([7]).visible(false);
            //AnywhereECTable.columns([9]).visible(false);     
            //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
            $(AnywhereECTable.column(4).header()).html('Property Identification Number');
            $(AnywhereECTable.column(6).header()).html('Sketch Number');
            $(AnywhereECTable.column(9).header()).html('Reference Number');
            AnywhereECTable.columns([3]).visible(false);
            AnywhereECTable.columns([5]).visible(false);
            AnywhereECTable.columns([10]).visible(false);
             //Added by mayank on 05/01/2022
            AnywhereECTable.columns([13]).visible(false);
            //End of Comment by mayank on 05/01/2022
        }


        //tableIndexReports.columns.adjust().draw();
        AnywhereECTable.columns.adjust().draw();
        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration

        if ($('#ReportListID').val() === "eswathu") { $('#reportHeadingID').html("ESwathu Import Report"); }
        else if ($('#ReportListID').val() === "upor") { $('#reportHeadingID').html("UPOR Import Report"); }
        else if ($('#ReportListID').val() === "eaasthi") { $('#reportHeadingID').html("EAASTHI Import Report"); }
        else if ($('#ReportListID').val() === "mojini") { $('#reportHeadingID').html("Mojini Import Report"); }
        else if ($('#ReportListID').val() === "fruits") { $('#reportHeadingID').html("FRUITS Import Report"); }


    });

    $('#ReportListID').change(function () {
        if ($('#ReportListID').val() === "fruits") {
            $("#FruitsdateMsgID").html('<label style="color:red"><b>Note:</b></label><label style="color:black">From Date and To Date regarding FRUITS Report is related to Application recieved date at Registration office</label>');
            $("#FruitsdateMsgID").html('<label style="color:red"><b>Note:</b></label><label style="color:black">From Date/To Date - date on which FRUITS transactions received at Sub Registrar office.</label>');

        }
        else {
            $("#FruitsdateMsgID").html('');
        }
    });

    $("#btncloseAbortPopup").click(function () {
        $('#divViewAbortModal').modal('hide');
        $("#divLoadAbortView").html('');
    });

    $("#btnCloseModal").click(function () {
        $('#divViewAbortModal').modal('hide');
        $("#divLoadAbortView").html('');
    });
       
    
});

//function PDFDownloadFun() {
//    window.location.href = '/MISReports/DocumentReferences/ExportAnywhereECRptToPDF?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + SroID + "&DistrictID=" + DistrictID + "&LogTypeID=" + LogTypeID + "&SelectedDist=" + SelectedDistrictText + "&SelectedSRO=" + SelectedSROText + "&SelectedLogType=" + SelectedLogTypeText;

//}


// BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 31-12-2020
//function EXCELDownloadFun(INTDistrictId, INTSroId, FromDate, FromDate, STRReportNameID) {
function EXCELDownloadFun(INTDistrictId, INTSroId, FromDate, ToDate, STRReportNameID) {
    // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 31-12-2020

    //alert('FromDate-' +FromDate);
    //alert('ToDate-'+ToDate);


    window.location.href = '/MISReports/OtherDepartmentImport/ExportOtherDepartmentImportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SroID=" + INTSroId + "&DistrictID=" + INTDistrictId + "&ReportName=" + STRReportNameID;
}


function XMLDownloadFun(LogId, SROCode, ReportName, XMLType) {

    window.location.href = '/MISReports/OtherDepartmentImport/ExportToXML?LogId=' + LogId + "&SROCode=" + SROCode + "&ReportName=" + ReportName + "&XMLType=" + XMLType;
}
//Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration

function FormIIIDownloadFun(LogId, SROCode, ReportName, XMLType) {

    window.location.href = '/MISReports/OtherDepartmentImport/FormIIIDownloadFun?LogId=' + LogId + "&SROCode=" + SROCode + "&ReportName=" + ReportName + "&XMLType=" + XMLType;
}

function ViewTransXMLFun(LogId, SROCode, ReportName, XMLType) {

    //window.location.href = '/MISReports/OtherDepartmentImport/ViewTransXMLFun?LogId=' + LogId + "&SROCode=" + SROCode + "&ReportName=" + ReportName + "&XMLType=" + XMLType;

    $.ajax({
        url: '/MISReports/OtherDepartmentImport/ViewTransXMLFun',
        data: { "LogId": LogId, "SROCode": SROCode, "ReportName": ReportName, "XMLType": XMLType },
        datatype: "json",
        type: "GET",
        success: function (data) {
            //if (data.serverError == true) {
            //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
            //        function () {
            //            window.location.href = "/Home/HomePage"
            //        });
            //}
            //else {
            //    if (data.serverError == false && data.success == false) {
            //        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
            //    }
            //    else {
            //console.log(data);
            $('#divViewAbortModal').modal('show');
            $("#divLoadAbortView").html( $.parseHTML(data));
            //$("#divLoadAbortViewMainPage").html(data);
            $("#modalheaderspanid").html("KAVERI FRUITS Integration Imported Data");
            //    }
            //}
            unBlockUI();
        },
        error: function (xhr) {
            console.log(xhr);
            unBlockUI();
        }
    });
}