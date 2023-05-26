var token = '';
var header = {};
var SelectedSROText;
var txtFromDate;
var txtToDate;
var SroID;
var DistrictID;
var LogTypeID;
var SelectedSROText;
var SelectedDistrictText;
var SelectedLogTypeText;
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/AnywhereECLog/GetSROOfficeListByDistrictID',
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

        if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
            $("#AnywhereECTable").DataTable().clear().destroy();
        }
        //To Get Same Data in PDF and Excel
        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        SroID = $("#SROOfficeListID option:selected").val();
        DistrictID = $("#DROOfficeListID option:selected").val();
        LogTypeID = $("#LogTypeId option:selected").val();
        SelectedSROText = $("#SROOfficeListID option:selected").text();
        SelectedDistrictText = $("#DROOfficeListID option:selected").text();
        SelectedLogTypeText = $("#LogTypeId option:selected").text();

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
                url: '/MISReports/AnywhereECLog/LoadAnywhereECLogTable',
                type: "POST",
                headers: header,
                data: {
                    'FromDate': txtFromDate, 'ToDate': txtToDate, 'DistrictID': DistrictID, 'SroID': SroID,
                    'LogTypeID': LogTypeID
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
                    unBlockUI();
                },
                beforeSend: function () {
                    blockUI('loading data.. please wait...');
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
              
            ],
         
            columns: [

                { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                { data: "ApplicationNo", "searchable": true, "visible": true, "name": "ApplicationNo" },
                { data: "SROfficeAppNo", "searchable": true, "visible": true, "name": "SROfficeAppNo" },
                { data: "ApplicationFilingDate", "searchable": true, "visible": true, "name": "ApplicationFilingDate" },
                { data: "UserName", "searchable": true, "visible": true, "name": "UserName" },
                { data: "Desc", "searchable": true, "visible": true, "name": "Desc" },
                { data: "LogDateTime", "searchable": true, "visible": true, "name": "LogDateTime" },
            ],
            fnInitComplete: function (oSettings, json) {
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
        //AnywhereECTable.columns.adjust().draw();



    });




});

function PDFDownloadFun() {
    window.location.href = '/MISReports/AnywhereECLog/ExportAnywhereECRptToPDF?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + SroID + "&DistrictID=" + DistrictID + "&LogTypeID=" + LogTypeID + "&SelectedDist=" + SelectedDistrictText + "&SelectedSRO=" + SelectedSROText + "&SelectedLogType=" + SelectedLogTypeText;

}


function EXCELDownloadFun() {
    
    window.location.href = '/MISReports/AnywhereECLog/ExportAnywhereECRptToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + SroID + "&DistrictID=" + DistrictID + "&LogTypeID=" + LogTypeID + "&SelectedDist=" + SelectedDistrictText + "&SelectedSRO=" + SelectedSROText + "&SelectedLogType=" + SelectedLogTypeText;
}