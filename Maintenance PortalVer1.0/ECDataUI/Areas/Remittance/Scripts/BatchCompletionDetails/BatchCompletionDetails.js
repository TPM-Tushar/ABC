
//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DtlsSearchParaListCollapse').click(function () {

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    $('#divFromDate').datepicker({
        autoclose: true,
        format: 'mm/dd/yyyy',
        endDate: '+0d',
        startDate: new Date('01/01/2000'),
        maxDate: new Date(),
        minDate: new Date('01/01/2022'),
        pickerPosition: "bottom-left"

    });
});

function GetBatchCompletionDetails() {

   
   // var FromDate = $("#txtFromDate").val();

    var DocType = $("#DocType").val();


    var tableRegistrationNoVerificationSummary = $('#MasterDataID').DataTable({


        ajax: {

            url: '/Remittance/BatchCompletionDetails/GetBatchCompletionDetails',
            data: {

              //  'FromDate': FromDate,
                'Doctype': DocType,

            },
            type: "POST",
            headers: header,

            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            }
                            //else if (!json.status ) {
                            //    bootbox.alert({
                            //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>'
                            //    });
                            //}
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#MasterDataID").DataTable().clear().destroy();
                                $("#EXCELSPANID").html('');
                            }
                        }
                    });
                }

                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                    $('.input-sm').attr('placeholder', 'SROCode / SRO Name');
                    unBlockUI();
                }
                return json.data;


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
                var searchString = $('#MasterDataID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#MasterDataID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableRegistrationNoVerificationSummary.search('').draw();
                            $("#MasterDataID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }
        },
        serverSide: true,

        //"scrollX": "300px",
        scrollX: true,
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
        "bSort": true,
        "bSort": true,


        columns: [

            { data: "srNo", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "srNo", "width": "2%" },
            { data: "SROCode", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "SROCode" },
            { data: "SroName", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "SroName" },
 //Added by Rushikesh 27 Feb 2023
            { data: "L_Stamp5DateTime", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "L_Stamp5DateTime" },

           //added by vijay 
            { data: "BatchDateTime", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "BatchDateTime" },
            { data: "Isverified", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "Isverified" },


            //End by Rushikesh 27 Feb 2023

            /*
            { data: "documentmaster_MaxDocID", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "documentmaster_MaxDocID" },
            { data: "MaxDMStamp5Datetime", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "RPT_DocReg_NoCLBatchDetails_MaxToDocID1" },
            { data: "RPT_DocReg_NoCLBatchDetails_MaxToDocID", "searchable": setConditionFor_DM(DocType), "visible": setConditionFor_DM(DocType), "name": "RPT_DocReg_NoCLBatchDetails_MaxToDocID" },
            { data: "MaxBatchStamp5Datetime", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "documentmaster_MaxDocID" },
            { data: "DataStatus", "searchable": true, "visible": setConditionFor_DM(DocType), "name": "DataStatus" },
            */
            { data: "srNo", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "srNo", "width": "2%" },
            { data: "SROCode", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "SROCode" },
            { data: "SroName", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "SroName" },

            //Added by Rushikesh 27 Feb 2023
            { data: "L_Stamp5DateTime", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "L_Stamp5DateTime" },
            //End by Rushikesh 27 Feb 2023
            { data: "BatchDateTime", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "BatchDateTime" },
            { data: "Isverified", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "Isverified" },


            /*
            { data: "MaxRegID", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "MaxRegID" },
            { data: "MaxMRDateofReg", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "MaxMRDateofReg" },
            { data: "RPT_DocReg_NoCLBatchDetails_MaxToDocID", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "RPT_DocReg_NoCLBatchDetails_MaxToDocID2" },
            { data: "MaxBatchStamp5Datetime", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "documentmaster_MaxDocID" },
            { data: "DataStatus", "searchable": true, "visible": setConditionFor_MR(DocType), "name": "DataStatus" },
            */

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
function setConditionFor_DM(DocType) {
    if (DocType == 0) return true;
    return false;
}


function setConditionFor_MR(DocType) {
    if (DocType == 1) return true;
    return false;
}









