
//Global variables.
var token = '';
var header = {};
var IsFRNCheckEx = "";
var IsSFNCheckEx = "";
var IsRefreshEx = "";
var IsDateNullEx = "";

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#txtFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',

        maxDate: '0',
        pickerPosition: "bottom-left"
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
        changeYear: true,

    }).datepicker("setDate", FromDate);

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });
    //
    $("input:checkbox").on('click', function () {

        var $box = $(this);
        if ($box.is(":checked")) {

            var group = "input:checkbox[name='" + $box.attr("name") + "']";

            $(group).prop("checked", false);
            $box.prop("checked", true);
        } else {
            $box.prop("checked", false);
        }
    });
    //
    //
    $('#DocumentTypeId').change(function () {
        if ($("#DocumentTypeId").val() == '1') {
   
     
            $("#C_Frn").html('Central_FinalRegistrationNumber');
            $("#C_Stamp5Date").html('Central_Stamp5DateTime');
            $("#L_Frn").html('Local_FinalRegistrationNumber');
            $("#L_Stamp5Date").html('Local_Stamp5DateTime');
            
        }
        else if ($("#DocumentTypeId").val() == '2') {
   
            $("#C_Frn").html('Central_MarriageCaseNo');
            $("#C_Stamp5Date").html('Central_DateOfRegistration');
            $("#L_Frn").html('Local_MarriageCaseNo');
            $("#L_Stamp5Date").html('Local_DateOfRegistration');

        }
    });
    //
});



function SearchMissingScanDocumentDetails() {
  //  alert("IN SearchMissingSacnDocumentDetails");
    var SROfficeID = $("#SROOfficeListID").val();
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();
    DateNullCheck = $("input[name='NullCheckBox']:checked").val();
    var DocumentTypeId = $("#DocumentTypeId").val();
  

    let C_NA_L_A = $("input[id='C_NA_L_A']:checked").val();
    let C_NA_L_NA = $("input[id='C_NA_L_NA']:checked").val();
    let C_A_L_NA = $("input[id='C_A_L_NA']:checked").val();


    var tableMissingScanDocumentDetails = $('#MissingSacnDocumentDetailsID').DataTable({
        ajax: {

            url: '/Remittance/MissingScanDocument/GetMissingScanDocumentDetails',
            type: "POST",
            headers: header,
            data: {
      
                'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DocumentTypeId': DocumentTypeId, 'C_NA_L_A': C_NA_L_A, 'C_NA_L_NA': C_NA_L_NA, 'C_A_L_NA': C_A_L_NA
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
                                $("#MissingSacnDocumentDetailsID").DataTable().clear().destroy();
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
                var searchString = $('#MissingSacnDocumentDetailsID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#MissingSacnDocumentDetailsID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableMissingScanDocumentDetails.search('').draw();
                            $("#MissingSacnDocumentDetailsID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }
        },
        serverSide: true,

        "scrollX": "300px",
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

            { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width": "2%" },
            { data: "SRO_Name", "searchable": true, "visible": true, "name": "SRO_Name", "width": "11%" },
            { data: "Central_FinalRegistrationNumber", "searchable": true, "visible": true, "name": "Central_FinalRegistrationNumber", "width": "13%" },
            { data: "Central_ScannedFileName", "searchable": true, "visible": true, "name": "Central_ScannedFileName", "width": "11%" },
            { data: "Central_Stamp5DateTime", "searchable": true, "visible": true, "name": "Central_Stamp5DateTime", "width": "9%" },
            { data: "Central_CDNumber", "searchable": true, "visible": true, "name": "Central_CDNumber", "width": "10%" },
            { data: "Local_FinalRegistrationNumber", "searchable": true, "visible": true, "name": "Local_FinalRegistrationNumber", "width": "13%" },
            { data: "Local_ScannedFileName", "searchable": true, "visible": true, "name": "Local_ScannedFileName", "width": "11%" },
            { data: "Local_Stamp5DateTime", "searchable": true, "visible": true, "name": "Local_Stamp5DateTime", "width": "9%" },
            { data: "Local_CDNumber", "searchable": true, "visible": true, "name": "Local_CDNumber", "width": "10%" }
          
          
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



function EXCELDownloadFun(SroCodeEx, fromDate, ToDate, DocumentTypeIdEx, IsErrorTypecheckEx, ErrorCodeEx) {
    //alert("in excel fun");

    //window.location.href = '/Remittance/RegistrationNoVerificationDetails/ExportRegistrationNoVerificationDetailsToExcel?SroCode=' + SROfficeID + "&fromDate=" + FromDate + "&ToDate=" + ToDate + "&DocumentTypeId=" + DocumentTypeIdEx + "&IsFRNCheck=" + IsFRNCheckEx + "&IsSFNCheck=" + IsSFNCheckEx + "&IsRefresh=" + IsRefreshEx + "&IsDateNull=" + IsDateNullEx;
    window.location.href = '/Remittance/MissingScanDocument/ExportMissingScanDocumentDetailsToExcel?SroCode=' + SroCodeEx + "&fromDate=" + fromDate + "&ToDate=" + ToDate + "&DocumentTypeId=" + DocumentTypeIdEx + "&IsErrorTypecheck=" + IsErrorTypecheckEx + "&ErrorCode=" + ErrorCodeEx;

}