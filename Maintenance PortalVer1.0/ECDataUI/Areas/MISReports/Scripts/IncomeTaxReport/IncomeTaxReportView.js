//Global variables.
var token = '';
var header = {};

$(document).ready(function () {



    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DtlsToggleIconSearchParaListCollapse').trigger('click');

    $('#DROfficeListID').change(function () {
        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/IncomeTaxReport/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROfficeListID').val() },
            datatype: "json",
            type: "GET",
            success: function (data) {

                if (data.serverError == true) {
                    unBlockUI();
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    unBlockUI();
                    $('#SROfficeListID').empty();
                    $.each(data.SROOfficeList, function (i, SROOfficeList) {
                        SROOfficeList
                        $('#SROfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });


    $('#DROfficeListID').focus();


    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchParaListCollapse').removeClass(classToRemove).addClass(classToSet);
    });

    $('#SearchParaDtls').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    $("#SearchBtn").click(function () {


        if ($.fn.DataTable.isDataTable("#IncomeTaxReportsID")) {
            $("#IncomeTaxReportsID").DataTable().clear().destroy();
        }

        SROfficeID = $("#SROfficeListID option:selected").val();
        DROfficeID = $("#DROfficeListID option:selected").val();
        FinYearID = $("#FinYearListID option:selected").val();
        var DatabaseNameListID = $("#DatabaseNameListID option:selected").val();



        if (DROfficeID == "0") {
            if ($.fn.DataTable.isDataTable("#IncomeTaxReportsID")) {
                $("#IncomeTaxReportsID").DataTable().clear().destroy();


            }
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any District" + '</span>',
                callback: function () {
                    var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                }
            });
        }
        else if (SROfficeID == "0") {
            if ($.fn.DataTable.isDataTable("#IncomeTaxReportsID")) {
                $("#IncomeTaxReportsID").DataTable().clear().destroy();


            }
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any SRO" + '</span>',
                callback: function () {
                    var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                }
            });
        }
        else if (FinYearID == "0") {
            if ($.fn.DataTable.isDataTable("#IncomeTaxReportsID")) {
                $("#IncomeTaxReportsID").DataTable().clear().destroy();
                
            }
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any Financial Year" + '</span>',
                callback: function () {
                    var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                }
            });
        }

        else {
            if ($.fn.DataTable.isDataTable("#IncomeTaxReportsID")) {

                $("#IncomeTaxReportsID").DataTable().clear().destroy();
            }

            //$("#SearchParaDtls").trigger('click');

            var tableIndexReports = $('#IncomeTaxReportsID').DataTable({

                ajax: {
                    url: '/MISReports/IncomeTaxReport/GetIncomeTaxReportDetails',
                    type: "POST",
                    headers: header,
                    data: {
                        'DROfficeID': DROfficeID, 'SROfficeID': SROfficeID, 'FinYearID': FinYearID
                    },
                    dataSrc: function (json) {
                        unBlockUI();
                        if (json.errorMessage != null) {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                callback: function () {
                                    if (json.serverError != undefined) {
                                        window.location.href = "/Home/HomePage"
                                    } else {
                                        var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
                                        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                            $('#DtlsSearchParaListCollapse').trigger('click');
                                        $("#SearchParaDtls").trigger('click');
                                        $("#IncomeTaxReportsID").DataTable().clear().destroy();
                                        $("#PDFSPANID").html('');
                                        $("#EXCELSPANID").html('');
                                    }
                                    //$("#DtlsToggleIconSearchPara").trigger('click');
                                }
                            });
                        }
                        else {
                            var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
                            if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                                $('#DtlsSearchParaListCollapse').trigger('click');

                            var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                $("#SearchParaDtls").trigger('click');
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
                        var searchString = $('#IncomeTaxReportsID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;

                            if (!regexToMatch.test(searchString)) {
                                $("#IncomeTaxReportsID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    //    $('#menuDetailsListTable_filter input').val('');

                                    tableIndexReports.search('').draw();
                                    $("#IncomeTaxReportsID_filter input").prop("disabled", false);

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
                "destroy": true,
                "bAutoWidth": true,
                "bScrollAutoCss": true,

                columnDefs: [

                    { orderable: false, targets: [1] },
                    { orderable: false, targets: [3] },
                    { orderable: false, targets: [4] },
                    { orderable: false, targets: [5] },
                    { orderable: false, targets: [6] },
                    { orderable: false, targets: [7] },
                    { orderable: false, targets: [8] },
                    { orderable: false, targets: [9] },
                    { orderable: false, targets: [10] },
                    { orderable: false, targets: [11] },
                    { orderable: false, targets: [12] },
                    { orderable: false, targets: [13] },
                    { orderable: false, targets: [14] },
                    { orderable: false, targets: [15] },
                    { orderable: false, targets: [16] },
                    { orderable: false, targets: [17] },
                    { orderable: false, targets: [18] },
                    { orderable: false, targets: [19] },
                    { orderable: false, targets: [20] },
                    { orderable: false, targets: [21] },
                    { orderable: false, targets: [22] },
                    { orderable: false, targets: [23] },
                    { orderable: false, targets: [24] },
                    { orderable: false, targets: [25] },
                    { orderable: false, targets: [26] },
                    { orderable: false, targets: [27] },
                    { orderable: false, targets: [28] },
                    { orderable: false, targets: [29] }
                ],

                columns: [
                    { data: "ReportSrNo", "searchable": true, "visible": true, "name": "ReportSrNo" },
                    { data: "OriginalReportSrNo", "searchable": true, "visible": true, "name": "OriginalReportSrNo" },
                    { data: "CustomerID", "searchable": true, "visible": true, "name": "CustomerID" },
                    { data: "PersonName", "searchable": true, "visible": true, "name": "PersonName" },
                    { data: "DateOfBirth", "searchable": true, "visible": true, "name": "DateOfBirth" },
                    { data: "FatherName", "searchable": true, "visible": true, "name": "FatherName" },
                    { data: "PanAckNo", "searchable": true, "visible": true, "name": "PanAckNo" },
                    { data: "AadharNumber", "searchable": true, "visible": true, "name": "AadharNumber" },
                    { data: "IdentificationType", "searchable": true, "visible": true, "name": "IdentificationType" },
                    { data: "IdentificationNumber", "searchable": true, "visible": true, "name": "IdentificationNumber" },
                    { data: "FlatDoorBuilding", "searchable": true, "visible": true, "name": "FlatDoorBuilding" },
                    { data: "NameOfPrem", "searchable": true, "visible": true, "name": "NameOfPrem" },
                    { data: "RoadStreet", "searchable": true, "visible": true, "name": "RoadStreet" },
                    { data: "AreaLocality", "searchable": true, "visible": true, "name": "AreaLocality", "autoWidth": false },
                    { data: "CityTown", "searchable": true, "visible": true, "name": "CityTown", "autoWidth": false },
                    { data: "PostalCode", "searchable": true, "visible": true, "name": "PostalCode", "autoWidth": false },
                    { data: "StateCode", "searchable": true, "visible": true, "name": "StateCode", "autoWidth": true },
                    { data: "CountryCode", "searchable": true, "visible": true, "name": "CountryCode" },
                    { data: "MobileNo", "searchable": true, "visible": true, "name": "MobileNo" },
                    { data: "STDCode", "searchable": true, "visible": true, "name": "STDCode" },
                    { data: "TelephoneNo", "searchable": true, "visible": true, "name": "TelephoneNo" },
                    { data: "AgriIncome", "searchable": true, "visible": true, "name": "AgriIncome" },
                    { data: "NonAgriIncome", "searchable": true, "visible": true, "name": "NonAgriIncome" },
                    { data: "Remarks", "searchable": true, "visible": true, "name": "Remarks" },
                    { data: "Form60AckNo", "searchable": true, "visible": true, "name": "Form60AckNo" },
                    { data: "TransactionDate", "searchable": true, "visible": true, "name": "TransactionDate" },
                    { data: "TransactionID", "searchable": true, "visible": true, "name": "TransactionID" },
                    { data: "TransactionType", "searchable": true, "visible": true, "name": "TransactionType" },
                    { data: "TransactionAmount", "searchable": true, "visible": true, "name": "TransactionAmount" },
                    { data: "TransactionMode", "searchable": true, "visible": true, "name": "TransactionMode" },
                    
                ],
                fnInitComplete: function (oSettings, json) {
                    console.log(json);
                    //alert('in fnInitComplete');
                    $("#PDFSPANID").html(json.PDFDownloadBtn);
                    $("#EXCELSPANID").html(json.ExcelDownloadBtn);
                    $("#DroName").html(json.DroName);
                    $("#SroName").html(json.SroName);
                    $("#FinYearName").html(json.FYName);
                    
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

    });
    


});

function PDFDownloadFun(DROfficeID, SROfficeID, FinYearID) {

    window.location.href = '/MISReports/IncomeTaxReport/ExportIncomeTaxReportToPDF?DROfficeID=' + DROfficeID + "&SROfficeID=" + SROfficeID + "&FinYearID=" + FinYearID;

}


function EXCELDownloadFun(DROfficeID, SROfficeID, FinYearID) {

    window.location.href = '/MISReports/IncomeTaxReport/ExportIncomeTaxReportToExcel?DROfficeID=' + DROfficeID + "&SROfficeID=" + SROfficeID + "&FinYearID=" + FinYearID;

}


