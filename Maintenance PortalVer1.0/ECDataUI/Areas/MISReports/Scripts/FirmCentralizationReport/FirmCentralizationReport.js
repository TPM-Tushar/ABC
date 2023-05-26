var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    var txtFromDate;
    var txtToDate;

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
    }).datepicker("setDate", txtToDate);


    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", txtFromDate);



   
    $("#BtnGenerateReport").click(function () {
        GenerateReport();
        
    });

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $('input:radio[name=RDOBtnFilter][value=FileExist]').trigger('click');
    $('input[type=radio][name=RDOBtnFilter]').change(function () {

        //var CurrentSearchClass = $('#DtlsToggleIconSearchPara').attr('class');
        var CurrentDetailClass = $('#DtlsToggleIconSearchParaDetail').attr('class');
        //if (CurrentSearchClass == "fa fa-minus-square-o fa-pull-left fa-2x")
        //    $('#DtlsSearchParaListCollapse').trigger('click');
        if (CurrentDetailClass == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapseDetail').trigger('click');
    });
});

function GenerateReport() {
    if ($.fn.DataTable.isDataTable("#DetailTableID")) {
        $("#DetailTableID").DataTable().clear().destroy();
    }
    DistrictID = $("#DROOfficeListID option:selected").val();
    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    if (txtFromDate == null || txtFromDate == "") {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
        return;
    }
    else if (txtToDate == null || txtToDate == "") {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
        return;
    }
    else if (DistrictID == 0) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select District.</span>');
        return;
    }
    var SearchBy = $('input[name="RDOBtnFilter"]:checked').val();
    //console.log(SearchBy);
    if (SearchBy != undefined) {
        var _SearchByParameter = "";
        if (SearchBy == "FileExist")
            _SearchByParameter = "FileExist";

        if (SearchBy == "FileReadable")
            _SearchByParameter = "FileReadable";
    }
    else {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Select any one filter' + '</span>',
            callback: function () {
            }
        });
        return;
    }
    var DROfficeWiseSummaryTableID = $('#DetailTableID').DataTable({
        ajax: {
            url: '/MISReports/FirmCentralizationReport/GetFirmCentralizationDetails/',
            type: "POST",
            headers: header,
            data: { 'DistrictID': DistrictID, "FromDate": txtFromDate, "ToDate": txtToDate, "SearchByParameter": _SearchByParameter },
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
                                $("#DetailTableID").DataTable().clear().destroy();
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
                var searchString = $('#DetailTableID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#DetailTableID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DROfficeWiseSummaryTableID.search('').draw();
                            $("#DetailTableID_filter input").prop("disabled", false);
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
            { targets: "_all", orderable: false, "className": "text-center" }
        ],

        columns: [
            { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
            { data: "RegistrationID", "searchable": true, "visible": true, "name": "RegistrationID" },
            { data: "FirmNumber", "searchable": true, "visible": true, "name": "FirmNumber" },
            { data: "DateOfRegistration", "searchable": true, "visible": true, "name": "DateOfRegistration" },
            { data: "CDNumber", "searchable": true, "visible": true, "name": "CDNumber" },
            { data: "IsLocalFirmDataCentralized", "searchable": true, "visible": true, "name": "IsLocalFirmDataCentralized" },
            { data: "IsLocalScanDocumentUpload", "searchable": true, "visible": true, "name": "IsLocalScanDocumentUpload" },
            { data: "IsCDWriting", "searchable": true, "visible": true, "name": "IsCDWriting" },
            { data: "IsFirmDataCentralized", "searchable": true, "visible": true, "name": "IsFirmDataCentralized" },
            { data: "IsScanDocumentUploaded", "searchable": true, "visible": true, "name": "IsScanDocumentUploaded" },
            { data: "IsUploadedScanDocumentPresent", "searchable": true, "visible": true, "name": "IsUploadedScanDocumentPresent" },
            { data: "IsFilePresent", "searchable": true, "visible": true, "name": "IsFilePresent" },
            { data: "IsFileReadable", "searchable": true, "visible": true, "name": "IsFileReadable" },
            { data: "FilePath", "searchable": true, "visible": true, "name": "FilePath" },


        ],
        fnInitComplete: function (oSettings, json) {
            //$("#SROSpanID").html(json.SROName);
            $("#EXCELSPANID").html('');
            $("#EXCELSPANID").html(json.ExcelDownloadBtn);
            
            if (SearchBy == "FileExist")
                DROfficeWiseSummaryTableID.columns([12]).visible(false);

            if (SearchBy == "FileReadable")
                DROfficeWiseSummaryTableID.columns([12]).visible(true);
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

function EXCELDownloadFun(DistrictID, FromDate, ToDate) {
    window.location.href = '/MISReports/FirmCentralizationReport/ExportOrderDetailsToExcel?DistrictID=' + DistrictID + "&FromDate=" + FromDate + "&ToDate=" + ToDate + "&Searchby=" + $('input[name="RDOBtnFilter"]:checked').val();
}