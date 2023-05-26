
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

    $('#RegistrationNoVerificationSummaryID').DataTable({
        rowReorder: true,
        "orderable": true,
        aaSorting: [[9, 'desc']],
        
    });

});

function GetSummaryReportDetails() {
   let FromDate = $("#txtFromDate").val();
   let ToDate = $("#txtToDate").val();
    let DocumentTypeId = $("#DocumentTypeId").val();

   // alert("FromDate" + FromDate);
   // alert("ToDate" + ToDate);
//alert("DocumentTypeId" + DocumentTypeId);

    //

    var tableRegistrationNoVerificationSummary = $('#RegistrationNoVerificationSummaryID').DataTable({
        ajax: {

            url: '/Remittance/RegistrationNoVerificationSummaryReport/GetSummaryReportDetails',
            type: "POST",
            headers: header,
            data: {
      
                'FromDate': FromDate, 'ToDate': ToDate,  'DocumentTypeId': DocumentTypeId,
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
                                $("#RegistrationNoVerificationSummaryID").DataTable().clear().destroy();
                                $("#EXCELSPANID").html('');
                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                    //Added By Tushar on 28 Dec 2022
                    let DocumentTypeId = $("#DocumentTypeId").val();
                    if (DocumentTypeId == 4 && json.data.length <= 10 && json.IsRecordFilter == false && json.draw == "1") {

                        $("#FirstColSpan").attr("colspan", 2);
                       
                        $("#DataStatisticsGridId").append('<th colspan="3" style="border-right-color: black; ">Transactional Data Statistics</th>');
                        $("#DataStatisticsGridId").append('<th colspan="3">Scan Document Data Statistics</th>');
                    
                    }
                    //End By Tushar on 28 Dec 2022
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
                var searchString = $('#RegistrationNoVerificationSummaryID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#RegistrationNoVerificationSummaryID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableRegistrationNoVerificationSummary.search('').draw();
                            $("#RegistrationNoVerificationSummaryID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }
        },
        serverSide: true,
 
        //"scrollX": "300px",
        scrollX: false,
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
       //"lengthMenu": [[10, 25, 50, 100], [10, 25, 50, "All"]],
        columnDefs: [

            { "orderable": false, targets: [0] }
            
        ],
     
        
        //columns: [

        //    { data: "srNo", "searchable": true, "visible": true, "name": "srNo" },
        //    { data: "SROName", "searchable": true, "visible": true, "name": "SRO Name", "width":"10%" },
        //    { data: "M_M", "searchable": true, "visible": true, "name": "M_M" },
        //    { data: "L_Missing", "searchable": true, "visible": true, "name": "L_Missing" },
        //    { data: "L_Additional", "searchable": true, "visible": true, "name": "L_Additional" },
        //    { data: "Is_Duplicate", "searchable": true, "visible": true, "name": "Is Duplicate" },
        //    { data: "LP_CNP", "searchable": true, "visible": true, "name": "LP_CNP" },
        //    { data: "CNP_LNP", "searchable": true, "visible": true, "name": "CNP_LNP" },
        //    { data: "CP_LNP", "searchable": true, "visible": true, "name": "CP_LNP" },
        //    { data: "SM_M", "searchable": true, "visible": true, "name": "SM_M" },
       

          

        //],
          //Commented and Added By Tushar on 28 Dec 2022
        columns: [

            { data: "srNo", "searchable": true, "visible": true, "name": "srNo" },
            { data: "SROName", "searchable": true, "visible": setConditions(DocumentTypeId), "name": "SRO Name", "width":"10%" },
            { data: "M_M", "searchable": true, "visible": setConditions(DocumentTypeId), "name": "M_M" },
            { data: "L_Missing", "searchable": true, "visible": setConditions(DocumentTypeId), "name": "L_Missing" },
            { data: "L_Additional", "searchable": true, "visible": setConditions(DocumentTypeId), "name": "L_Additional" },
            { data: "Is_Duplicate", "searchable": true, "visible": setConditions(DocumentTypeId), "name": "Is Duplicate" },
            { data: "LP_CNP", "searchable": true, "visible": setConditions(DocumentTypeId), "name": "LP_CNP" },
            { data: "CNP_LNP", "searchable": true, "visible": setConditions(DocumentTypeId), "name": "CNP_LNP" },
            { data: "CP_LNP", "searchable": true, "visible": setConditions(DocumentTypeId), "name": "CP_LNP" },
            { data: "SM_M", "searchable": true, "visible": setConditions(DocumentTypeId), "name": "SM_M" },
            //
            { data: "DistrictName", "searchable": true, "visible": setConditionsForFirm(DocumentTypeId), "name": "DistrictName" ,"width": "10%" },
            { data: "FirmResult_LA_CNA_Count", "searchable": true, "visible": setConditionsForFirm(DocumentTypeId), "name": "FirmResult_LA_CNA_Count" },
            { data: "FirmResult_CA_LNA_Count", "searchable": true, "visible": setConditionsForFirm(DocumentTypeId), "name": "FirmResult_CA_LNA_Count" },
            { data: "FirmResult_FN_Miss_Count", "searchable": true, "visible": setConditionsForFirm(DocumentTypeId), "name": "FirmResult_FN_Miss_Count" },
            { data: "FirmResult_SC_LA_CNA_Count", "searchable": true, "visible": setConditionsForFirm(DocumentTypeId), "name": "FirmResult_SC_LA_CNA_Count" },
            { data: "FirmResult_SC_CA_LNA_Count", "searchable": true, "visible": setConditionsForFirm(DocumentTypeId), "name": "FirmResult_SC_CA_LNA_Count" },
            { data: "FirmResult_SC_FN_Miss_Count", "searchable": true, "visible": setConditionsForFirm(DocumentTypeId), "name": "FirmResult_SC_FN_Miss_Count" },
            //
            //End By Tushar on 28 Dec 2022

          

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

    //
}

function EXCELDownloadFun(fromDate, ToDate, DocumentTypeId) {
    //alert("In EXCELDownloadFun");

 //   alert(fromDate + "fromDate");
   // alert("ToDate" + ToDate);
//    alert("DocumentTypeId" + DocumentTypeId);

    window.location.href = '/Remittance/RegistrationNoVerificationSummaryReport/ExportSummaryReportToExcel?fromDate=' + fromDate + "&ToDate=" + ToDate + "&DocumentTypeId=" + DocumentTypeId;

}
//Added By Tushar on 28 Dec 2022
function setConditions(DocumentTypeId) {
    if (DocumentTypeId != 4) return true;
    //else { return false; }
    return false;
}
function setConditionsForFirm(DocumentTypeId) {
    if (DocumentTypeId == 4) return true;
    //else { return false; }
    return false;
} 
//End By Tushar on 28 Dec 2022