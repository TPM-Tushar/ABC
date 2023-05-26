
var token = '';
var header = {};
var SelectedSROText;
var txtFromDate;
var txtToDate;
var SroID;
var SelectedDistrictText;
var DistrictID;
var FinYearText;
var PaymentModeText;
var ReceiptTypeID;
var ReceiptTypeText;

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/PaymentModewiseCollectionSummary/GetSROOfficeListByDistrictID',
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

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $("#SearchBtn").click(function () {

        if ($.fn.DataTable.isDataTable("#PaymentModeWiseSummaryTable")) {
            $("#PaymentModeWiseSummaryTable").DataTable().clear().destroy();
        }

       
        SroID = $("#SROOfficeListID option:selected").val();
        SelectedSROText = $("#SROOfficeListID option:selected").text();
        DistrictID = $("#DROOfficeListID option:selected").val();
        SelectedDistrictText = $("#DROOfficeListID option:selected").text();
        FinYearID = $("#FinYearID option:selected").val();
        FinYearText = $("#FinYearID option:selected").text();
        PaymentModeID = $("#PaymentModeID option:selected").val();
        PaymentModeText = $("#PaymentModeID option:selected").text();
        ReceiptTypeID = $("#ReceiptTypeID option:selected").val();
        ReceiptTypeText = $("#ReceiptTypeID option:selected").text();
       
        var PaymentModeWiseSummaryTable = $('#PaymentModeWiseSummaryTable').DataTable({
            ajax: {
                url: '/MISReports/PaymentModewiseCollectionSummary/GetPaymentModeWiseRPTTableData',
                type: "POST",
                headers: header,
                data: {
                    'SroID': SroID, 'DistrictID': DistrictID, 'FinYearID': FinYearID, 'PaymentModeID': PaymentModeID, 'ReceiptTypeID': ReceiptTypeID
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
                                    $("#PaymentModeWiseSummaryTable").DataTable().clear().destroy();
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
                    var searchString = $('#PaymentModeWiseSummaryTable_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#PaymentModeWiseSummaryTable_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                //    $('#menuDetailsListTable_filter input').val('');
                                PaymentModeWiseSummaryTable.search('').draw();
                                $("#PaymentModeWiseSummaryTable_filter input").prop("disabled", false);
                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
            },
            serverSide: true,
            "scrollY": "300px",
            "scrollCollapse": true,
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
              

            ],
           
            columns: [

                { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
                { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName" },
                { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
                { data: "NoOfReceipts", "searchable": true, "visible": true, "name": "NoOfReceipts" },
                { data: "RegistrationFeeCollected", "searchable": true, "visible": true, "name": "RegistrationFeeCollected" },
                { data: "NoOfStampDuty", "searchable": true, "visible": true, "name": "NoOfStampDuty" },
                { data: "StampDutyCollected", "searchable": true, "visible": true, "name": "StampDutyCollected" },
                { data: "TotalNoofReceiptsandStampDuty", "searchable": true, "visible": true, "name": "TotalNoofReceiptsandStampDuty" },
                { data: "TotalCollection", "searchable": true, "visible": true, "name": "TotalCollection" }
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
                //responsiveHelper.respond();
                unBlockUI();
            },
        });



        //tableIndexReports.columns.adjust().draw();
        //DailyReceiptRptTable.columns.adjust().draw();



    });




});
//SroID = $("#SROOfficeListID option:selected").val();
//SelectedSROText = $("#SROOfficeListID option:selected").text();
//DistrictID = $("#DROOfficeListID option:selected").val();
//SelectedDistrictText = $("#DROOfficeListID option:selected").text();
//FinYearID = $("#FinYearID option:selected").val();
//FinYearText = $("#FinYearID option:selected").text();
//PaymentModeID = $("#PaymentModeID option:selected").val();
//PaymentModeText = $("#PaymentModeID option:selected").text();
//ReceiptTypeID = $("#ReceiptTypeID option:selected").val();
//ReceiptTypeText = $("#ReceiptTypeID option:selected").text();
function EXCELDownloadFun()
{
    window.location.href = '/MISReports/PaymentModewiseCollectionSummary/ExportPaymentModeWiseRPTToExcel?DistrictID=' + DistrictID + "&SelectedDistrictText=" + SelectedDistrictText + "&SroID=" + SroID + "&SelectedSROText=" + SelectedSROText + "&FinYearID=" + FinYearID + "&FinYearText=" + FinYearText + "&PaymentModeID=" + PaymentModeID + "&PaymentModeText=" + PaymentModeText + "&ReceiptTypeID=" + ReceiptTypeID + "&ReceiptTypeText=" + ReceiptTypeText;
}