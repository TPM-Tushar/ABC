
//Global variables.
var token = '';
var header = {};



$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;



    $('#txtToDate').datepicker({
        format: "mm/yyyy",
        viewMode: "months",
        minViewMode: "months",


        endYear: new Date().getMonth(),

        autoclose: true,

        pickerPosition: "bottom-left"

    });


    $('#divToDate').datepicker({
        format: "mm/yyyy",
        viewMode: "months",
        minViewMode: "months",

        //: January,
        endYear: new Date().getMonth(),

        autoclose: true,

        pickerPosition: "bottom-left"
    });

    $('#txtToDate').datepicker({
        format: "mm   /   yyyy",
        changeMonth: false,
        changeYear: false,

    }).datepicker("setDate", ToDate);



    //

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });
    //


    $('#DROOfficeListID').change(function () {
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/ScanningStatisticsConsolidated/GetSROOfficeListByDistrictID',
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


});

function SearchScanningStatisticsConsolidatedDetails() {
    let SROfficeID = $("#SROOfficeListID").val();
    let DistrictID = $('#DROOfficeListID').val();
    
    let Date = $("#txtToDate").val();
  
    //alert("ToDate" + Date);

    let IsValidate = ValidateParameters(DistrictID, SROfficeID);

    if (IsValidate) {
        var tableScanningStatisticsConsolidatedDetails = $('#ScanningStatisticsID').DataTable({
            ajax: {

                url: '/MISReports/ScanningStatisticsConsolidated/GetScanningStatisticsConsolidatedDetails',
                type: "POST",
                headers: header,
                data: {

                     'Date': Date, 'SROfficeID': SROfficeID, 'DistrictID': DistrictID
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
                                    $("#ScanningStatisticsID").DataTable().clear().destroy();
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
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });

                    unBlockUI();
                },
                beforeSend: function () {
                    blockUI('Loading data please wait.');
                    var searchString = $('#RegistrationNoVerificationDetailsID_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#ScanningStatisticsID_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                tableScanningStatisticsConsolidatedDetails.search('').draw();
                                $("#ScanningStatisticsID_filter input").prop("disabled", false);
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
            "responsive": true,
            scrollCollapse: true,
            bPaginate: true,
            bLengthChange: true,
            bInfo: true,
            info: true,
            bFilter: false,
            searching: true,
            "destroy": true,
            "bAutoWidth": true,


            columns: [


                { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width": "5%" },

                { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName", "width": "10%" },
                { data: "SROName", "searchable": true, "visible": true, "name": "SROName", "width": "10%" },
                { data: "Month", "searchable": true, "visible": true, "name": "Month", "width": "13%" },
                { data: "Total_scanned_page", "searchable": true, "visible": true, "name": "Total_scanned_page", "width": "10%" },
                { data: "DocType", "searchable": true, "visible": true, "name": "DocType", "width": "10%" },
     



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
                unBlockUI();
            },
        });
    }

}

function ValidateParameters(DistrictID, SROfficeID) {

    //Validations
    if (DistrictID == undefined) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please select District' + '</span>',
            callback: function () {

            }
        }
        )
        return false;
    } else if (SROfficeID == undefined) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please select SRO Name' + '</span>',
            callback: function () {

            }
        })
        return false;
    } else {
        return true;
    }
    //End Validations

}
function EXCELDownloadFun(DateEX, DROfficeIDEx, SroCodeEx) {
    //alert("In EXCELDownloFun");
    window.location.href = '/MISReports/ScanningStatisticsConsolidated/ExportScanningStatisticsConsolidatedDetailsToExcel?DateEx=' + DateEX + "&DROfficeID=" + DROfficeIDEx + "&SroCode=" + SroCodeEx;
}