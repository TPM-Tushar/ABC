var Amount;
var FromDate;
var ToDate;
var SROOfficeListID;
var NatureOfDocumentListID;
//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;




    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/RegistrationSummary/GetSROOfficeListByDistrictID',
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

    //$("#AmountID").val(' ');
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


    $("#SearchBtn").click(function () {

        //if ($('#SROOfficeListID').val() === "0" || $('#SROOfficeListID').val() < "0") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select office.</span>');
        //}
        //else if ($("#NatureOfDocumentListID option:selected").val() < "0") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Nature of Document.</span>');

        //}
        //else {

        if ($.fn.DataTable.isDataTable("#IndexIIReportsID")) {
            $("#IndexIIReportsID").DataTable().clear().destroy();
        }

        // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
        // TO GET SAME DATA IN PDF AND EXCEL 
        //var Amount = $("#AmountID").val();
        //var FromDate = $("#txtFromDate").val();
        //var ToDate = $("#txtToDate").val();
        //var SROOfficeListID = $("#SROOfficeListID option:selected").val();
        //var NatureOfDocumentListID = $("#NatureOfDocumentListID option:selected").val();

        Amount = $("#AmountID").val();
        FromDate = $("#txtFromDate").val();
        ToDate = $("#txtToDate").val();
        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        NatureOfDocumentListID = $("#NatureOfDocumentListID option:selected").val();
        DROfficeID = $("#DROOfficeListID option:selected").val();

        var DatabaseNameListID = $("#DatabaseNameListID option:selected").val();



        if (DROfficeID == "0") {
            if ($.fn.DataTable.isDataTable("#IndexIIReportsID")) {
                $("#IndexIIReportsID").DataTable().clear().destroy();


            }
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any District" + '</span>',
                callback: function () {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                }
            });
        }
        else if (SROOfficeListID == "0") {
            if ($.fn.DataTable.isDataTable("#IndexIIReportsID")) {
                $("#IndexIIReportsID").DataTable().clear().destroy();


            }
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any SRO" + '</span>',
                callback: function () {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                }
            });
        }
        else {
            if ($.fn.DataTable.isDataTable("#IndexIIReportsID")) {
                //$("#IndexIIReportsID").dataTable().fnDestroy();
                //$("#DtlsSearchParaListCollapse").trigger('click');
                $("#IndexIIReportsID").DataTable().clear().destroy();


            }
            var tableIndexReports = $('#IndexIIReportsID').DataTable({

                ajax: {
                    url: '/MISReports/RegistrationSummary/GetRegistrationSummaryDetails',
                    type: "POST",
                    headers: header,
                    data: {
                        'fromDate': FromDate, 'ToDate': ToDate, 'SROOfficeListID': SROOfficeListID, 'NatureOfDocumentListID': NatureOfDocumentListID, 'Amount': Amount, 'DROfficeID': DROfficeID
                    },
                    dataSrc: function (json) {
                        unBlockUI();
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
                                        $("#IndexIIReportsID").DataTable().clear().destroy();
                                        $("#PDFSPANID").html('');
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
                        var searchString = $('#IndexIIReportsID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;

                            if (!regexToMatch.test(searchString)) {
                                $("#IndexIIReportsID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    //    $('#menuDetailsListTable_filter input').val('');

                                    tableIndexReports.search('').draw();
                                    $("#IndexIIReportsID_filter input").prop("disabled", false);

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
                //        function (e, dt, node, config) {
                //            //this.disable();
                //            window.location = '/MISReports/IndexIIReports/ExportIndexIIReportToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&Amount=" + Amount;
                //        }


                //    },
                //    {
                //        extend: 'excel',
                //        text: '<i class="fa fa-file-pdf-o"></i> Excel',
                //        exportOptions: {
                //            columns: ':not(.no-print)'
                //        },
                //        action:
                //        function (e, dt, node, config) {
                //            this.disable();
                //            window.location = '/MISReports/IndexIIReports/ExportIndexIIReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&Amount=" + Amount;
                //        }


                //    }

                //],


                columnDefs: [

                    { orderable: false, targets: [1] },
                    { orderable: false, targets: [3] },
                    { orderable: false, targets: [4] },
                    { orderable: false, targets: [5] },
                    { orderable: false, targets: [6] },
                    { orderable: false, targets: [7] },
                    { orderable: false, targets: [8] }

                ],

                columns: [
                    { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
                    { data: "ArticleNameE", "searchable": true, "visible": true, "name": "ArticleNameE" },
                    { data: "Stamp5Datetime", "searchable": true, "visible": true, "name": "Stamp5Datetime" },
                    { data: "TotalArea", "searchable": true, "visible": true, "name": "TotalArea" },
                    { data: "Unit", "searchable": true, "visible": true, "name": "Unit" },
                    { data: "PropertyDetails", "searchable": true, "visible": true, "name": "PropertyDetails" },
                    { data: "Schedule", "searchable": true, "visible": true, "name": "Schedule", "autoWidth": false },
                    { data: "Executant", "searchable": true, "visible": true, "name": "Executant", "autoWidth": false },
                    { data: "Claimant", "searchable": true, "visible": true, "name": "Claimant", "autoWidth": true },
                    { data: "VillageNameE", "searchable": true, "visible": true, "name": "VillageNameE" },
                    { data: "marketvalue", "searchable": true, "visible": true, "name": "marketvalue" },
                    { data: "consideration", "searchable": true, "visible": true, "name": "consideration" },

                ],
                fnInitComplete: function (oSettings, json) {
                    //alert('in fnInitComplete');
                    $("#PDFSPANID").html(json.PDFDownloadBtn);
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




        }




        //    }

        //tableIndexReports.button(0).nodes().css('background', 'green');
        //tableIndexReports.button(1).nodes().css('background', 'green');




    });



    //$("#PdfDownload").click(function () {      
    //    window.location.href = '/MISReports/IndexIIReports/ExportIndexIIReportToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&Amount=" + Amount;

    //});

    //$("#excelDownload").click(function () { 
    //    window.location.href = '/MISReports/IndexIIReports/ExportIndexIIReportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&Amount=" + Amount;

    //});

    $("#btncloseAbortPopup").click(function () {
        $('#divViewAbortModal').modal('hide');
    });

});

function PDFDownloadFun(FromDate, ToDate, SROOfficeListID, NatureOfDocumentListID, Amount) {

    window.location.href = '/MISReports/RegistrationSummary/ExportRegistrationSummaryToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&Amount=" + Amount + "&DROOfficeListID=" + DROfficeID;
}


function EXCELDownloadFun(FromDate, ToDate, SROOfficeListID, NatureOfDocumentListID, Amount) {

    window.location.href = '/MISReports/RegistrationSummary/ExportRegistrationSummaryToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeListID + "&NatureOfDocumentListID=" + NatureOfDocumentListID + "&Amount=" + Amount + "&DROOfficeListID=" + DROfficeID;

}

//function ValidateParameter(FinalRegNo) {

//    $('#divViewAbortModal').modal('show');
//    $("#objPDFViewer").attr('data', '');
//    $("#objPDFViewer").attr('data', '/MISReports/RegistrationSummary/DisplayScannedFile?FRN=' + FinalRegNo);
//    $("#objPDFViewer").load('/MISReports/RegistrationSummary/DisplayScannedFile?FRN=' + FinalRegNo);
//}


function ValidateParameter(FinalRegNo) {
    //alert(FinalRegNo);
    $.ajax({
        url: '/MISReports/RegistrationSummary/ValidateParameters',
        data: { "FRN": FinalRegNo },
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
                if (data.serverError == false && data.success == false) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                }
                else {
                    if (data.IsFileExistAtDownloadPath) {
                        $('#divViewAbortModal').modal('show');
                        $("#objPDFViewer").attr('data', '');
                        $("#objPDFViewer").attr('data', '/MISReports/RegistrationSummary/DisplayScannedFile?FRN=' + FinalRegNo);
                        $("#objPDFViewer").load('/MISReports/RegistrationSummary/DisplayScannedFile?FRN=' + FinalRegNo);
                    }
                    else {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">File not exist at path.</span>');
                    }
                }
            }
            unBlockUI();
        },
        error: function (xhr) {
            alert(xhr);
            unBlockUI();
        }
    });
}