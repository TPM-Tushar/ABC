//Global variables.
var token = '';
var header = {};


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
});

function SearchFirmCentralizationDetails() {
    //alert("In SearchFirmCentralizationDetails");

    DistrictID = $("#DROOfficeListID option:selected").val();
    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();

    //alert("DistrictID" + DistrictID);
    //alert("txtFromDate" + txtFromDate);
    //alert("txtToDate" + txtToDate);
    // Commented by Rushikesh Chaudhari
    /*
    if (DistrictID == "0") {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Select District' + '</span>',
            callback: function () {
            }
        });
        return;
    }
    */
    //
    var SearchBy = $('input[name="SearchType"]:checked').val();
    //console.log(SearchBy);
    //alert("SearchBy" + SearchBy);
    if (SearchBy != undefined) {
        var _SearchByParameter = "";
        if (SearchBy == "LA_CNA")
            _SearchByParameter = "LA_CNA";

        else if (SearchBy == "CA_LNA")
            _SearchByParameter = "CA_LNA";
        else if (SearchBy == "FN_Miss")
            _SearchByParameter = "FN_Miss";
        else if (SearchBy == "SC_LA_CNA")
            _SearchByParameter = "SC_LA_CNA";
        else if (SearchBy =="SC_CA_LNA")
            _SearchByParameter = "SC_CA_LNA";
        else if (SearchBy == "SC_FN_Miss")
            _SearchByParameter = "SC_FN_Miss";
    }
    else {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Select Any One Filter' + '</span>',
            callback: function () {
            }
        });
        return;
    }
    //
    //
    var FirmCentralizationDetailsTable = $('#DetailTableID').DataTable({
        ajax: {
            url: '/Remittance/FirmCentralization/GetFirmCentralizationDetails/',
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
                var searchString = $('#DetailTableID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#DetailTableID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            FirmCentralizationDetailsTable.search('').draw();
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
        //columnDefs: [
        //    { targets: "_all", orderable: false, "className": "text-center" }
        //],

        columns: [
            { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
            { data: "RegistrationID", "searchable": true, "visible": true, "name": "RegistrationID" },
            { data: "L_FirmNumber", "searchable": true, "visible": true, "name": "L_FirmNumber" },
            { data: "L_DateOfRegistration", "searchable": true, "visible": true, "name": "L_DateOfRegistration" },
            { data: "L_CDNumber", "searchable": true, "visible": true, "name": "L_CDNumber" },
            { data: "L_ScanFileName", "searchable": true, "visible": true, "name": "L_ScanFileName" },
            //{ data: "IsLocalFirmDataCentralized", "searchable": true, "visible": true, "name": "IsLocalFirmDataCentralized" },
            //{ data: "IsLocalScanDocumentUpload", "searchable": true, "visible": true, "name": "IsLocalScanDocumentUpload" },
            //{ data: "IsCDWriting", "searchable": true, "visible": true, "name": "IsCDWriting" },
            { data: "C_FirmNumber", "searchable": true, "visible": true, "name": "C_FirmNumber" },
            { data: "C_DateOfRegistration", "searchable": true, "visible": true, "name": "C_DateOfRegistration" },
            { data: "C_CDNumber", "searchable": true, "visible": true, "name": "C_CDNumber" },
          
            { data: "C_ScanFileName", "searchable": true, "visible": true, "name": "C_ScanFileName" },
            //{ data: "C_DateOfRegistration", "searchable": true, "visible": true, "name": "C_DateOfRegistration" },
            { data: "UploadDateTime", "searchable": true, "visible": true, "name": "UploadDateTime" },
         


        ],
        fnInitComplete: function (oSettings, json) {
        
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
           
            unBlockUI();
        },
    });
    //
}

function EXCELDownloadFun(DistrictID, FromDate, ToDate, SearchBy) {
    window.location.href = '/Remittance/FirmCentralization/ExportFirmCentralizationDetailsToExcel?DistrictID=' + DistrictID + "&FromDate=" + FromDate + "&ToDate=" + ToDate + "&SearchBy=" + SearchBy;
}

function EXCELDownloadFunForLocal() {
    let DistrictID = $("#DROOfficeListID option:selected").val();
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();

    // Commented by Rushikesh
    /*
    if (DistrictID == "0") {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Select District' + '</span>',
            callback: function () {
            }
        });
        return;
    }
    else {
        */
        window.location.href = '/Remittance/FirmCentralization/ExportLocalFirmDetailsToExcel?DistrictID=' + DistrictID + "&FromDate=" + FromDate + "&ToDate=" + ToDate;
   // }

}

function EXCELDownloadFunForCentral() {
    let DistrictID = $("#DROOfficeListID option:selected").val();
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();

    //Commented by Rushikesh
    /*
    if (DistrictID == "0") {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Select District' + '</span>',
            callback: function () {
            }
        });
        return;
    }
    else {
    */
        window.location.href = '/Remittance/FirmCentralization/ExportCentralFirmDetailsToExcel?DistrictID=' + DistrictID + "&FromDate=" + FromDate + "&ToDate=" + ToDate;
    //}

}

function EXCELDownloadFunForCentral() {
    let DistrictID = $("#DROOfficeListID option:selected").val();
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();

    //Commented by Rushikesh
    /*
    if (DistrictID == "0") {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Select District' + '</span>',
            callback: function () {
            }
        });
        return;
    }
    else { 
    */
    window.location.href = '/Remittance/FirmCentralization/ExportCentralFirmDetailsToExcel?DistrictID=' + DistrictID + "&FromDate=" + FromDate + "&ToDate=" + ToDate;
//}

}