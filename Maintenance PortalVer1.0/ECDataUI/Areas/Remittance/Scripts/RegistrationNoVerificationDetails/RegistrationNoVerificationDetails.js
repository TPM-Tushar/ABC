
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
//Added By Rushikesh on 16 Feb 2023
    var newLabel = '';
    $('#DocumentTypeId').on('change', function () {
        $('.replacetxt').text(newLabel); //Change the text before changing the value

        switch ($('#DocumentTypeId').val()) {
            case '0':
               // newLabel = 'Transaction';
                newLabel = 'Document';
                $('.replacetxt').text(newLabel);
                break;

            case '1':
                //newLabel = 'Document Master';
               newLabel = 'Document';
                $('.replacetxt').text(newLabel);
                break;
            case '2':
                newLabel = 'Marriage';
                $('.replacetxt').text(newLabel);
                break;

        }

    }).trigger('change');
//End By Rushikesh on 16 Feb 2023
    
});

//function SearchRegistrationNoVerificationDetails() {
//    alert("Hello");
//    var SROfficeID = $("#SROOfficeListID").val();
//    FromDate = $("#txtFromDate").val();
//    ToDate = $("#txtToDate").val();

//    alert("SROfficeID" + SROfficeID);
//    alert("FromDate" + FromDate);
//    alert("ToDate"+ToDate);

//}

function SearchRegistrationNoVerificationDetails() {
    var SROfficeID = $("#SROOfficeListID").val();
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();
    DateNullCheck = $("input[name='NullCheckBox']:checked").val();
    var DocumentTypeId = $("#DocumentTypeId").val();
    // Added By Tushar on 14 Oct 2022
    var FRNCheck = $("input[name='FRNCheckBox']:checked").val();
    var SFNCheck = $("input[name='SFNCheckBox']:checked").val();
    var IsRefresh = "false";
    //End By Tushar on 14 Oct 2022
    //Added By Tushar on 1 Nov 2022
    var FileNACheck = $("input[id='FileNACheckBox']:checked").val();
    var CNull = $("input[id='C_NullCheckBox']:checked").val();
    var LNull = $("input[id='L_NullCheckBox']:checked").val();
     //End By Tushar on 1 Nov 2022
    //Added By Tushar on 2 Nov 2022
    let MisMatch = $("input[id='MisMatch']:checked").val(); 
    let Deleted = $("input[id='Deleted']:checked").val();
    let Added = $("input[id='Added']:checked").val();
    let C_NA_L_A = $("input[id='C_NA_L_A']:checked").val();
    let C_NA_L_NA = $("input[id='C_NA_L_NA']:checked").val();
    let C_A_L_NA = $("input[id='C_A_L_NA']:checked").val();
    //Added By Tushar on 9 Nov 2022
    let SM_M = $("input[id='SM_M']:checked").val();
    //End By Tushar on 9 Nov 2022
    //Added By Tushar on 29 Nov 2022
    let IsDuplicate = $("input[id='IsDuplicate']:checked").val();
    //End By Tushar on 29 Nov 2022
    //End By Tushar on 2 Nov 2022
    //Added By Tushar on 3 Jan 2023
    let PropertyAreaDetailsErrorType = $("input[name='PropertyErrorType']:checked").val();
    //End By Tushar on 3 Jan 2023
  //Added By Rushikesh on 6 Feb 2023
    let DateDetailsErrorType = $("input[name='DateErrorType']:checked").val();
    //End By Rushikesh on 6 Feb 2023
    //Added bY Rushikesh on 13-02-2023
    let DateErrorType_DateDetails = $("input[name='DateErrorType_DateDetails']:checked").val();
    //End By Rushikesh on 13-02-2023
    var tableRegistrationNoVerificationDetails = $('#RegistrationNoVerificationDetailsID').DataTable({
        ajax: {

            url: '/Remittance/RegistrationNoVerificationDetails/GetRegistrationNoVerificationDetails',
            type: "POST",
            headers: header,
            data: {
                //Commented and added By Tushar on 14 Oct 2022
                //'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DocumentTypeId': DocumentTypeId, 'DateNullCheck': DateNullCheck
                //'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DocumentTypeId': DocumentTypeId, 'DateNullCheck': DateNullCheck, 'FRNCheck': FRNCheck, 'SFNCheck': SFNCheck, 'IsRefresh': IsRefresh, 'FileNACheck': FileNACheck, 'CNull': CNull, 'LNull': LNull, 'MisMatch': MisMatch, 'Deleted': Deleted, 'Added': Added, 'C_NA_L_A': C_NA_L_A, 'C_NA_L_NA': C_NA_L_NA, 'C_A_L_NA': C_A_L_NA, 'SM_M': SM_M, 'IsDuplicate': IsDuplicate
                //Commented and Added By Tushar on 3 Jan 2023

                //Updated by Rushikesh 6 Feb 2023
                'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DocumentTypeId': DocumentTypeId, 'DateNullCheck': DateNullCheck, 'FRNCheck': FRNCheck, 'SFNCheck': SFNCheck, 'IsRefresh': IsRefresh, 'FileNACheck': FileNACheck, 'CNull': CNull, 'LNull': LNull, 'MisMatch': MisMatch, 'Deleted': Deleted, 'Added': Added, 'C_NA_L_A': C_NA_L_A, 'C_NA_L_NA': C_NA_L_NA, 'C_A_L_NA': C_A_L_NA, 'SM_M': SM_M, 'IsDuplicate': IsDuplicate, 'PropertyAreaDetailsErrorType': PropertyAreaDetailsErrorType, 'DateDetailsErrorType': DateDetailsErrorType, 'DateErrorType_DateDetails': DateErrorType_DateDetails
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
                                $("#RegistrationNoVerificationDetailsID").DataTable().clear().destroy();
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
                        $("#RegistrationNoVerificationDetailsID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DROfficeWiseSummaryTableID.search('').draw();
                            $("#RegistrationNoVerificationDetailsID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }
        },
        serverSide: true,
        //scrollY: true,
        //scrollX: true,
       // "scrollX": true,
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

            //Updated by Rushikesh 6 Feb 2023
            //Updated Sequence of Central and Local fields

            /*
                { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width": "2%" },
    
                { data: "DocumentID", "searchable": true, "visible": true, "name": "DocumentID", "width": "3%" },
                { data: "SROCode", "searchable": true, "visible": true, "name": "SROCode", "width": "2%" },
                { data: "C_Stamp5DateTime", "searchable": true, "visible": true, "name": "C_Stamp5DateTime", "width": "4%" },
                { data: "C_FRN", "searchable": true, "visible": true, "name": "C_FRN", "width": "10%" },
                { data: "C_ScannedFileName", "searchable": true, "visible": true, "name": "C_ScannedFileName", "width": "10%" },
                { data: "L_Stamp5DateTime", "searchable": true, "visible": true, "name": "L_Stamp5DateTime", "width": "10%" },
                { data: "L_FRN", "searchable": true, "visible": true, "name": "L_FRN", "width": "10%" },
                { data: "L_ScannedFileName", "searchable": true, "visible": true, "name": "L_ScannedFileName", "width": "10%" },
                { data: "BatchID", "searchable": true, "visible": true, "name": "BatchID", "width": "4%" },
                { data: "C_CDNumber", "searchable": true, "visible": true, "name": "C_CDNumber", "width": "4%" },
                { data: "L_CDNumber", "searchable": true, "visible": true, "name": "L_CDNumber", "width": "4%" },
                { data: "ErrorType", "searchable": true, "visible": true, "name": "ErrorType", "width": "5%" },
                { data: "DocumentTypeID", "searchable": true, "visible": true, "name": "DocumentTypeID", "width": "5%" },
                { data: "BatchDateTime", "searchable": true, "visible": true, "name": "BatchDateTime", "width": "5%" },
                { data: "C_ScanFileUploadDateTime", "searchable": true, "visible": true, "name": "C_ScanFileUploadDateTime", "width": "5%" },
                { data: "L_ScanDate", "searchable": true, "visible": true, "name": "L_ScanDate", "width": "5%" }
            ], */
            { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width": "2%" },

            { data: "DocumentID", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "DocumentID", "width": "3%" },
            { data: "SROCode", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "SROCode", "width": "2%" },
            //{ data: "L_Stamp5DateTime", "searchable": true, "visible": true, "name": "L_Stamp5DateTime", "width": "10%" },
            //{ data: "L_FRN", "searchable": true, "visible": true, "name": "L_FRN", "width": "10%" },
            //{ data: "L_ScannedFileName", "searchable": true, "visible": true, "name": "L_ScannedFileName", "width": "10%" },
            //{ data: "BatchID", "searchable": true, "visible": true, "name": "BatchID", "width": "4%" },
            //{ data: "C_CDNumber", "searchable": true, "visible": true, "name": "C_CDNumber", "width": "4%" },
            //{ data: "L_StartTime", "searchable": true, "visible": true, "name": "L_StartTime", "width": "5%" }


            //{ data: "C_ScanFileUploadDateTime", "searchable": true, "visible": true, "name": "C_ScanFileUploadDateTime", "width": "5%" },
            //Added By Tushar on 3 Jan 2023
            //{ data: "L_SROCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "L_SROCode", "width": "5%" },
            //{ data: "PropertyID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "PropertyID", "width": "5%" },
            //{ data: "VillageCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "VillageCode", "width": "5%" },
            //{ data: "TotalArea", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "TotalArea", "width": "5%" },
            //{ data: "MeasurementUnit", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "MeasurementUnit", "width": "5%" },
            //{ data: "P_DocumentID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "P_DocumentID", "width": "5%" },
            { data: "C_Stamp5DateTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_Stamp5DateTime", "width": "4%" },
            { data: "L_Stamp5DateTime_1", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp5DateTime_1", "width": "5%" },
            { data: "L_Stamp5DateTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_Stamp5DateTime", "width": "10%" },

            { data: "C_FRN", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_FRN", "width": "10%" },
            { data: "L_FRN", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_FRN", "width": "10%" },

            { data: "C_ScannedFileName", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_ScannedFileName", "width": "10%" },
            { data: "L_ScannedFileName", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_ScannedFileName", "width": "10%" },

            { data: "C_CDNumber", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_CDNumber", "width": "4%" },
            { data: "L_CDNumber", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_CDNumber", "width": "4%" },

            { data: "C_ScanFileUploadDateTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_ScanFileUploadDateTime", "width": "5%" },
            { data: "L_ScanDate", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_ScanDate", "width": "5%" },

            { data: "C_SROCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_SROCode", "width": "5%" },
            { data: "L_SROCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "L_SROCode", "width": "5%" },

            { data: "C_PropertyID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_PropertyID", "width": "5%" },
            { data: "PropertyID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "PropertyID", "width": "5%" },

            { data: "C_VillageCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_VillageCode", "width": "5%" },
            { data: "VillageCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "VillageCode", "width": "5%" },

            { data: "C_TotalArea", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_TotalArea", "width": "5%" },
            { data: "TotalArea", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "TotalArea", "width": "5%" },

            { data: "C_MeasurementUnit", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_MeasurementUnit", "width": "5%" },
            { data: "MeasurementUnit", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "MeasurementUnit", "width": "5%" },

            { data: "C_DocumentID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_DocumentID", "width": "5%" },
            { data: "P_DocumentID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "P_DocumentID", "width": "5%" },

            { data: "C_Stamp1DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_Stamp1DateTime", "width": "5%" },
            { data: "L_Stamp1DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp1DateTime", "width": "5%" },

            { data: "C_Stamp2DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_Stamp2DateTime", "width": "5%" },
            { data: "L_Stamp2DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp2DateTime", "width": "5%" },

            { data: "C_Stamp3DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_Stamp3DateTime", "width": "5%" },
            { data: "L_Stamp3DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp3DateTime", "width": "5%" },

            { data: "C_Stamp4DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_Stamp4DateTime", "width": "5%" },
            { data: "L_Stamp4DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp4DateTime", "width": "5%" },

            { data: "C_PresentDateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_PresentDateTime", "width": "5%" },
            { data: "L_PresentDateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_PresentDateTime", "width": "5%" },

            { data: "C_ExecutionDateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_ExecutionDateTime", "width": "5%" },
            { data: "L_ExecutionDateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_ExecutionDateTime", "width": "5%" },

            { data: "C_DateOfStamp", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_DateOfStamp", "width": "5%" },
            { data: "L_DateOfStamp", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_DateOfStamp", "width": "5%" },

            { data: "C_WithdrawalDate", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_WithdrawalDate", "width": "5%" },
            { data: "L_WithdrawalDate", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_WithdrawalDate", "width": "5%" },

            { data: "C_RefusalDate", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_RefusalDate", "width": "5%" },
            { data: "L_RefusalDate", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_RefusalDate", "width": "5%" },

            //End By Tushar on 3 Jan 2023


            //Added By Tushar on 29 Nov 2022
            { data: "L_StartTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_StartTime", "width": "5%" },
            { data: "L_EndTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_EndTime", "width": "5%" },
            { data: "L_Filesize", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_Filesize", "width": "5%" },
            { data: "L_Pages", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_Pages", "width": "5%" },
            { data: "L_Checksum", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_Checksum", "width": "5%" },
            { data: "IsDuplicate", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "IsDuplicate", "width": "5%" },
            //End By Tushar on 29 Nov 2022
            { data: "BatchID", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "BatchID", "width": "4%" },
            { data: "ErrorType", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "ErrorType", "width": "5%" },
            { data: "DocumentTypeID", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "DocumentTypeID", "width": "5%" },
            { data: "BatchDateTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "BatchDateTime", "width": "5%" },
            { data: "P_BatchID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "P_BatchID", "width": "4%" },
            { data: "P_ErrorType", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "P_ErrorType", "width": "5%" },

            //End by Rushikesh 7 Feb 2023

            //Added by rushikesh 9 Feb 2023
            { data: "DocumentID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "DocumentID", "width": "3%" },
            { data: "SROCode", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "SROCode", "width": "3%" },
            { data: "TableName", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "TableName", "width": "3%" },
            { data: "ReceiptID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "ReceiptID", "width": "3%" },
            { data: "StampDetailsID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "StampDetailsID", "width": "3%" },
            { data: "PartyID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "PartyID", "width": "3%" },
            { data: "L_DateOfReceipt", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_DateOfReceipt", "width": "3%" },
            { data: "C_DateOfReceipt", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_DateOfReceipt", "width": "3%" },
            { data: "L_DateOfStamp", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_DateOfStamp", "width": "3%" },
            { data: "C_DateOfStamp", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_DateOfStamp", "width": "3%" },
            { data: "L_DDChalDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_DDChalDate", "width": "3%" },
            { data: "C_DDChalDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_DDChalDate", "width": "3%" },
            { data: "L_StampPaymentDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_StampPaymentDate", "width": "3%" },
            { data: "C_StampPaymentDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_StampPaymentDate", "width": "3%" },
            { data: "L_DateOfReturn", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_DateOfReturn", "width": "3%" },
            { data: "C_DateOfReturn", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_DateOfReturn", "width": "3%" },
            { data: "L_AdmissionDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_AdmissionDate", "width": "3%" },
            { data: "C_AdmissionDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_AdmissionDate", "width": "3%" },
            { data: "BatchID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "BatchID", "width": "3%" },
            { data: "ErrorType", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "ErrorType", "width": "3%" },
            //End by rushikesh 9 Feb 2023
        ],

        fnInitComplete: function (oSettings, json) {
            //console.log(json);
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

function RefreshDetails() {
    var IsRefresh = "true";
    var SROfficeID = $("#SROOfficeListID").val();
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();
    DateNullCheck = $("input[name='NullCheckBox']:checked").val();
    var DocumentTypeId = $("#DocumentTypeId").val();
    // Added By Tushar on 14 Oct 2022
    var FRNCheck = $("input[name='FRNCheckBox']:checked").val();
    var SFNCheck = $("input[name='SFNCheckBox']:checked").val();
    //Added By Tushar on 1 Nov 2022
    var FileNACheck = $("input[name='FileNACheckBox']:checked").val();
     //End By Tushar on 1 Nov 2022
    /*
    $.ajax({
        url: '/Remittance/RegistrationNoVerificationDetails/GetRegistrationNoVerificationDetails',
        type: "POST",
        headers: header,
        data: {
            //Commented and added By Tushar on 14 Oct 2022
            //'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DocumentTypeId': DocumentTypeId, 'DateNullCheck': DateNullCheck
            'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DocumentTypeId': DocumentTypeId, 'DateNullCheck': DateNullCheck, 'FRNCheck': FRNCheck, 'SFNCheck': SFNCheck, 'IsRefresh': IsRefresh
        },
        success: function (data) {
           // $("#dvCCApplicationDetails").html(data);
        },
        error: function () {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });

            unBlockUI();
        },
    });
    */
    //Added By Tushar on 3 Jan 2023
    let PropertyAreaDetailsErrorType = $("input[name='PropertyErrorType']:checked").val();
    //End By Tushar on 3 Jan 2023

    //Added By Rushikesh on 6 Feb 2023
    let DateDetailsErrorType = $("input[name='DateErrorType']:checked").val();
    //End By Rushikesh on 6 Feb 2023
    //Added bY Rushikesh on 13-02-2023
    let DateErrorType_DateDetails = $("input[name='DateErrorType_DateDetails']:checked").val();
    //End bY Rushikesh on 13-02-2023


    var tableRegistrationNoVerificationDetails = $('#RegistrationNoVerificationDetailsID').DataTable({
        ajax: {

            url: '/Remittance/RegistrationNoVerificationDetails/GetRegistrationNoVerificationDetails',
            type: "POST",
            headers: header,
            data: {
                //Commented and added By Tushar on 14 Oct 2022
                //'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DocumentTypeId': DocumentTypeId, 'DateNullCheck': DateNullCheck
                'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DocumentTypeId': DocumentTypeId, 'DateNullCheck': DateNullCheck, 'FRNCheck': FRNCheck, 'SFNCheck': SFNCheck, 'IsRefresh': IsRefresh
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
                                $("#RegistrationNoVerificationDetailsID").DataTable().clear().destroy();
                           
                            }
                        }
                    });
                }
                else {
                    if (json.RefreshMessage != null || json.RefreshMessage != undefined || json.RefreshMessage != "") {
                        bootbox.alert({
                            message: '<i class="fa fa-check text-success boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.RefreshMessage + '</span>',
                            callback: function () {
                            }
                        });
                    } else {
                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                            $('#DtlsSearchParaListCollapse').trigger('click');
                    }
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
                        $("#RegistrationNoVerificationDetailsID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DROfficeWiseSummaryTableID.search('').draw();
                            $("#RegistrationNoVerificationDetailsID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }
        },
        serverSide: true,
        //scrollY: true,
        //scrollX: true,
        // "scrollX": true,
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

            //Updated by Rushikesh 6 Feb 2023
            //Updated Sequence of Central and Local fields

            /*
                { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width": "2%" },
    
                { data: "DocumentID", "searchable": true, "visible": true, "name": "DocumentID", "width": "3%" },
                { data: "SROCode", "searchable": true, "visible": true, "name": "SROCode", "width": "2%" },
                { data: "C_Stamp5DateTime", "searchable": true, "visible": true, "name": "C_Stamp5DateTime", "width": "4%" },
                { data: "C_FRN", "searchable": true, "visible": true, "name": "C_FRN", "width": "10%" },
                { data: "C_ScannedFileName", "searchable": true, "visible": true, "name": "C_ScannedFileName", "width": "10%" },
                { data: "L_Stamp5DateTime", "searchable": true, "visible": true, "name": "L_Stamp5DateTime", "width": "10%" },
                { data: "L_FRN", "searchable": true, "visible": true, "name": "L_FRN", "width": "10%" },
                { data: "L_ScannedFileName", "searchable": true, "visible": true, "name": "L_ScannedFileName", "width": "10%" },
                { data: "BatchID", "searchable": true, "visible": true, "name": "BatchID", "width": "4%" },
                { data: "C_CDNumber", "searchable": true, "visible": true, "name": "C_CDNumber", "width": "4%" },
                { data: "L_CDNumber", "searchable": true, "visible": true, "name": "L_CDNumber", "width": "4%" },
                { data: "ErrorType", "searchable": true, "visible": true, "name": "ErrorType", "width": "5%" },
                { data: "DocumentTypeID", "searchable": true, "visible": true, "name": "DocumentTypeID", "width": "5%" },
                { data: "BatchDateTime", "searchable": true, "visible": true, "name": "BatchDateTime", "width": "5%" },
                { data: "C_ScanFileUploadDateTime", "searchable": true, "visible": true, "name": "C_ScanFileUploadDateTime", "width": "5%" },
                { data: "L_ScanDate", "searchable": true, "visible": true, "name": "L_ScanDate", "width": "5%" }
            ], */
            { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width": "2%" },

            { data: "DocumentID", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "DocumentID", "width": "3%" },
            { data: "SROCode", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "SROCode", "width": "2%" },
            //{ data: "L_Stamp5DateTime", "searchable": true, "visible": true, "name": "L_Stamp5DateTime", "width": "10%" },
            //{ data: "L_FRN", "searchable": true, "visible": true, "name": "L_FRN", "width": "10%" },
            //{ data: "L_ScannedFileName", "searchable": true, "visible": true, "name": "L_ScannedFileName", "width": "10%" },
            //{ data: "BatchID", "searchable": true, "visible": true, "name": "BatchID", "width": "4%" },
            //{ data: "C_CDNumber", "searchable": true, "visible": true, "name": "C_CDNumber", "width": "4%" },
            //{ data: "L_StartTime", "searchable": true, "visible": true, "name": "L_StartTime", "width": "5%" }


            //{ data: "C_ScanFileUploadDateTime", "searchable": true, "visible": true, "name": "C_ScanFileUploadDateTime", "width": "5%" },
            //Added By Tushar on 3 Jan 2023
            //{ data: "L_SROCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "L_SROCode", "width": "5%" },
            //{ data: "PropertyID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "PropertyID", "width": "5%" },
            //{ data: "VillageCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "VillageCode", "width": "5%" },
            //{ data: "TotalArea", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "TotalArea", "width": "5%" },
            //{ data: "MeasurementUnit", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "MeasurementUnit", "width": "5%" },
            //{ data: "P_DocumentID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "P_DocumentID", "width": "5%" },
            { data: "C_Stamp5DateTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_Stamp5DateTime", "width": "4%" },
            { data: "L_Stamp5DateTime_1", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp5DateTime_1", "width": "5%" },
            { data: "L_Stamp5DateTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_Stamp5DateTime", "width": "10%" },

            { data: "C_FRN", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_FRN", "width": "10%" },
            { data: "L_FRN", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_FRN", "width": "10%" },

            { data: "C_ScannedFileName", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_ScannedFileName", "width": "10%" },
            { data: "L_ScannedFileName", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_ScannedFileName", "width": "10%" },

            { data: "C_CDNumber", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_CDNumber", "width": "4%" },
            { data: "L_CDNumber", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_CDNumber", "width": "4%" },

            { data: "C_ScanFileUploadDateTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "C_ScanFileUploadDateTime", "width": "5%" },
            { data: "L_ScanDate", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_ScanDate", "width": "5%" },

            { data: "C_SROCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_SROCode", "width": "5%" },
            { data: "L_SROCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "L_SROCode", "width": "5%" },

            { data: "C_PropertyID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_PropertyID", "width": "5%" },
            { data: "PropertyID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "PropertyID", "width": "5%" },

            { data: "C_VillageCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_VillageCode", "width": "5%" },
            { data: "VillageCode", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "VillageCode", "width": "5%" },

            { data: "C_TotalArea", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_TotalArea", "width": "5%" },
            { data: "TotalArea", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "TotalArea", "width": "5%" },

            { data: "C_MeasurementUnit", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_MeasurementUnit", "width": "5%" },
            { data: "MeasurementUnit", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "MeasurementUnit", "width": "5%" },

            { data: "C_DocumentID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "C_DocumentID", "width": "5%" },
            { data: "P_DocumentID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "P_DocumentID", "width": "5%" },

            { data: "C_Stamp1DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_Stamp1DateTime", "width": "5%" },
            { data: "L_Stamp1DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp1DateTime", "width": "5%" },

            { data: "C_Stamp2DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_Stamp2DateTime", "width": "5%" },
            { data: "L_Stamp2DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp2DateTime", "width": "5%" },

            { data: "C_Stamp3DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_Stamp3DateTime", "width": "5%" },
            { data: "L_Stamp3DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp3DateTime", "width": "5%" },

            { data: "C_Stamp4DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_Stamp4DateTime", "width": "5%" },
            { data: "L_Stamp4DateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_Stamp4DateTime", "width": "5%" },

            { data: "C_PresentDateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_PresentDateTime", "width": "5%" },
            { data: "L_PresentDateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_PresentDateTime", "width": "5%" },

            { data: "C_ExecutionDateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_ExecutionDateTime", "width": "5%" },
            { data: "L_ExecutionDateTime", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_ExecutionDateTime", "width": "5%" },

            { data: "C_DateOfStamp", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_DateOfStamp", "width": "5%" },
            { data: "L_DateOfStamp", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_DateOfStamp", "width": "5%" },

            { data: "C_WithdrawalDate", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_WithdrawalDate", "width": "5%" },
            { data: "L_WithdrawalDate", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_WithdrawalDate", "width": "5%" },

            { data: "C_RefusalDate", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "C_RefusalDate", "width": "5%" },
            { data: "L_RefusalDate", "searchable": true, "visible": SetvisiblityForDateDetails(DateDetailsErrorType), "name": "L_RefusalDate", "width": "5%" },

            //End By Tushar on 3 Jan 2023


            //Added By Tushar on 29 Nov 2022
            { data: "L_StartTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_StartTime", "width": "5%" },
            { data: "L_EndTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_EndTime", "width": "5%" },
            { data: "L_Filesize", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_Filesize", "width": "5%" },
            { data: "L_Pages", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_Pages", "width": "5%" },
            { data: "L_Checksum", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "L_Checksum", "width": "5%" },
            { data: "IsDuplicate", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "IsDuplicate", "width": "5%" },
            //End By Tushar on 29 Nov 2022
            { data: "BatchID", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "BatchID", "width": "4%" },
            { data: "ErrorType", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "ErrorType", "width": "5%" },
            { data: "DocumentTypeID", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "DocumentTypeID", "width": "5%" },
            { data: "BatchDateTime", "searchable": true, "visible": Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails), "name": "BatchDateTime", "width": "5%" },
            { data: "P_BatchID", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "P_BatchID", "width": "4%" },
            { data: "P_ErrorType", "searchable": true, "visible": SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType), "name": "P_ErrorType", "width": "5%" },

            //End by Rushikesh 7 Feb 2023

            //Added by rushikesh 9 Feb 2023
            { data: "DocumentID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "DocumentID", "width": "3%" },
            { data: "SROCode", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "SROCode", "width": "3%" },
            { data: "TableName", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "TableName", "width": "3%" },
            { data: "ReceiptID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "ReceiptID", "width": "3%" },
            { data: "StampDetailsID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "StampDetailsID", "width": "3%" },
            { data: "PartyID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "PartyID", "width": "3%" },
            { data: "L_DateOfReceipt", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_DateOfReceipt", "width": "3%" },
            { data: "C_DateOfReceipt", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_DateOfReceipt", "width": "3%" },
            { data: "L_DateOfStamp", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_DateOfStamp", "width": "3%" },
            { data: "C_DateOfStamp", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_DateOfStamp", "width": "3%" },
            { data: "L_DDChalDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_DDChalDate", "width": "3%" },
            { data: "C_DDChalDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_DDChalDate", "width": "3%" },
            { data: "L_StampPaymentDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_StampPaymentDate", "width": "3%" },
            { data: "C_StampPaymentDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_StampPaymentDate", "width": "3%" },
            { data: "L_DateOfReturn", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_DateOfReturn", "width": "3%" },
            { data: "C_DateOfReturn", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_DateOfReturn", "width": "3%" },
            { data: "L_AdmissionDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "L_AdmissionDate", "width": "3%" },
            { data: "C_AdmissionDate", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "C_AdmissionDate", "width": "3%" },
            { data: "BatchID", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "BatchID", "width": "3%" },
            { data: "ErrorType", "searchable": true, "visible": SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails), "name": "ErrorType", "width": "3%" },
            //End by rushikesh 9 Feb 2023
        ],
        
        fnInitComplete: function (oSettings, json) {
          
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


//Added by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 
function RefreshPropertyAreaDetailsDetails() {
    var IsRefreshPropertyAreaDetailsEnable = "true";
    var SROfficeID = $("#SROOfficeListID").val();


    blockUI('loading data.. please wait...');

    $.ajax({
        url: '/Remittance/RegistrationNoVerificationDetails/RefreshPropertyAreaDetails',
        data: {
             'SROfficeID': SROfficeID
        },
        datatype: "json",
        type: "GET",
        success: function (data) {
            unBlockUI();
            if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                 function () {
                     window.location.href = "/Home/HomePage"
                 });
            }
            else
            {
                if (data.success == true) {
                    bootbox.alert({
                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                    });
                }
                else
                {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                    });
                }         
            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });
}
//Ended by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 





function EXCELDownloadFun() {
    //alert(" in EXCELDownloadFun");
    var SROfficeID = $("#SROOfficeListID").val();
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();
    window.location.href = '/Remittance/RegistrationNoVerificationDetails/ExportDocRegNoCLBatchDetailsToExcel?SROfficeID=' + SROfficeID + "&FromDate=" + FromDate + "&ToDate=" + ToDate;

}

//function EXCELDownloadFunForTable(SROfficeID, FromDate, ToDate, DocumentTypeIdEx, IsFRNCheckEx, IsSFNCheckEx, IsRefreshEx, IsDateNullEx, IsFileNAEX, IsCNullEx, IsLNullEx, IsErrorTypecheckEx, ErrorCodeEx, IsDuplicateEx) {
//Commented and Added By Tushar on 3 Jan 2023
function EXCELDownloadFunForTable(SROfficeID, FromDate, ToDate, DocumentTypeIdEx, IsFRNCheckEx, IsSFNCheckEx, IsRefreshEx, IsDateNullEx, IsFileNAEX, IsCNullEx, IsLNullEx, IsErrorTypecheckEx, ErrorCodeEx, IsDuplicateEx, IsPropertyAreaDetailsErrorType, IsDateDetailsErrorType, IsDateErrorType_DateDetails) {


    //window.location.href = '/Remittance/RegistrationNoVerificationDetails/ExportRegistrationNoVerificationDetailsToExcel?SroCode=' + SROfficeID + "&fromDate=" + FromDate + "&ToDate=" + ToDate + "&DocumentTypeId=" + DocumentTypeIdEx + "&IsFRNCheck=" + IsFRNCheckEx + "&IsSFNCheck=" + IsSFNCheckEx + "&IsRefresh=" + IsRefreshEx + "&IsDateNull=" + IsDateNullEx;
    //window.location.href = '/Remittance/RegistrationNoVerificationDetails/ExportRegistrationNoVerificationDetailsToExcel?SroCode=' + SROfficeID + "&fromDate=" + FromDate + "&ToDate=" + ToDate + "&DocumentTypeId=" + DocumentTypeIdEx + "&IsFRNCheck=" + IsFRNCheckEx + "&IsSFNCheck=" + IsSFNCheckEx + "&IsRefresh=" + IsRefreshEx + "&IsDateNull=" + IsDateNullEx + "&IsFileNA=" + IsFileNAEX + "&IsCNull=" + IsCNullEx + "&IsLNull=" + IsLNullEx + "&IsErrorTypecheck=" + IsErrorTypecheckEx + "&ErrorCode=" + ErrorCodeEx + "&IsDuplicate=" + IsDuplicateEx; IsDateDetailsErrorType
    //Commented and Added By Tushar on 3 Jan 2023
    window.location.href = '/Remittance/RegistrationNoVerificationDetails/ExportRegistrationNoVerificationDetailsToExcel?SroCode=' + SROfficeID + "&fromDate=" + FromDate + "&ToDate=" + ToDate + "&DocumentTypeId=" + DocumentTypeIdEx + "&IsFRNCheck=" + IsFRNCheckEx + "&IsSFNCheck=" + IsSFNCheckEx + "&IsRefresh=" + IsRefreshEx + "&IsDateNull=" + IsDateNullEx + "&IsFileNA=" + IsFileNAEX + "&IsCNull=" + IsCNullEx + "&IsLNull=" + IsLNullEx + "&IsErrorTypecheck=" + IsErrorTypecheckEx + "&ErrorCode=" + ErrorCodeEx + "&IsDuplicate=" + IsDuplicateEx + "&IsPropertyAreaDetailsErrorType=" + IsPropertyAreaDetailsErrorType + "&IsDateDetailsErrorType=" + IsDateDetailsErrorType + "&IsDateErrorType_DateDetails=" + IsDateErrorType_DateDetails;

}
//Added By Tushar on 7 Nov 2022
function EXCELDownloadFunForCount() {
    //alert("In EXCELDownloadFunForCount ");
    var SROfficeID = $("#SROOfficeListID").val();
    var DocumentTypeId = $("#DocumentTypeId").val();
if (DocumentTypeId == null || DocumentTypeId == "0") {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please select Document Type' + '</span>',
            callback: function () {
            }
    })
} else
    window.location.href = '/Remittance/RegistrationNoVerificationDetails/ExportScannedFileDetailsToExcel?SROfficeID=' + SROfficeID + "&DocumentTypeId=" + DocumentTypeId;
}

function EXCELDownloadFunForFRN() {
    var SROfficeID = $("#SROOfficeListID").val();
    var DocumentTypeId = $("#DocumentTypeId").val();
    if (DocumentTypeId == null || DocumentTypeId == "0") {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please select Document Type' + '</span>',
            callback: function () {
            }
        })
    } else {
        window.location.href = '/Remittance/RegistrationNoVerificationDetails/ExportFinalRegistrationNumberDetailsToExcel?SROfficeID=' + SROfficeID + "&DocumentTypeId=" + DocumentTypeId;
    }
   
}
//End By Tushar on 7 nov 2022

//Added BY Tushar on 3 jan 2023
function SetvisiblityForPropertyAreaDetails(PropertyAreaDetailsErrorType) {
    if (PropertyAreaDetailsErrorType != null && PropertyAreaDetailsErrorType != undefined) {
        return true
    }
    return false
}

function Setvisiblity(PropertyAreaDetailsErrorType, DateErrorType_DateDetails) {
    if ((PropertyAreaDetailsErrorType == null || PropertyAreaDetailsErrorType == undefined) && (DateErrorType_DateDetails == null || DateErrorType_DateDetails == undefined)) {
       // console.log("property log call")
        return true
    }
   
    return false
}
//End By Tushar on 3 Jan 2023

//Added by Rushikesh 6 Feb 2023
function SetvisiblityForDateDetails(DateDetailsErrorType) {
    if (DateDetailsErrorType != null && DateDetailsErrorType != undefined) {
        return true
    }
    return false
}
/*
function Setvisiblity(DateDetailsErrorType) {
    if ((DateDetailsErrorType == null || DateDetailsErrorType == undefined)) {
        return true
    }
    return false
}
*/
//Added by Rushikesh 6 Feb 2023


//added by rushikesh 9 feb 2023
function SetvisiblityForDateErrorType_DateDetails(DateErrorType_DateDetails) {
    if ((DateErrorType_DateDetails != null || DateErrorType_DateDetails != undefined)) {
        console.log("hello");
        return true
    }

    return false

}
//end by rushikesh 9 feb 2023

//Added By Tushar on 8 Feb 2023
function EXCELDownloadFunForDateDetails() {
    var SROfficeID = $("#SROOfficeListID").val();
    //var DocumentTypeId = $("#DocumentTypeId").val();
    
    window.location.href = '/Remittance/RegistrationNoVerificationDetails/ExportDateDetailsToExcel?SROfficeID=' + SROfficeID ;
  
}
//End By Tushar on 8 Feb 2023