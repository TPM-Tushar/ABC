/*
  
    * File Name         :   MarriageAnalysisReport.js
    * Author Name       :   Tushar Mhaske
    * Description       :    JS for Marriage Analysis Report.

*/
var FromDate;
var ToDate;
var SROOfficeListID;
//Global variables.
var token = '';
var header = {};
var SelectedSRO;
var SelectedDistrict;
var DROOfficeListID;
var SROOfficeListID;
var FrommDate;
var ToDate;
var cdate = new Date();
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $('#SevaSindhuReportTableID').DataTable({
        "autoWidth": false,
        "columnDefs": [
            { "width": "50%", "targets": 10 }
        ]
    });





    $('#DROOfficeListID').change(function () {
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/Remittance/MarriageAnalysisReport/GetSROOfficeListByDistrictID',
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


    $('#DROOfficeListID').focus();
    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });
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


    const CurrentDate = new Date();

    $('#divFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        startDate: new Date(CurrentDate.setMonth(CurrentDate.getMonth() - 6)).toLocaleDateString('en-GB'),
       minDate: new Date(CurrentDate.setMonth(CurrentDate.getMonth() - 6)).toLocaleDateString('en-GB'),

        pickerPosition: "bottom-left"
    });

    const CurrentDate2 = new Date();

    $('#divToDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',


        startDate: new Date(CurrentDate2.setMonth(CurrentDate2.getMonth() - 6)).toLocaleDateString('en-GB'),
        minDate: new Date(CurrentDate2.setMonth(CurrentDate2.getMonth() - 6)).toLocaleDateString('en-GB'),

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
});

function GetSevaSindhuDetails() {
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();
    //alert("FromDate" + FromDate);
    //alert("ToDate" + ToDate);
    SROfficeID = $("#SROOfficeListID option:selected").val();
    DROfficeID = $("#DROOfficeListID option:selected").val();


    var tableARegisterReportDetails = $('#SevaSindhuReportTableID').DataTable({
        ajax: {

            url: '/MISReports/SevaSindhuApplicationDetails/GetSevaSindhuApplicationDetails',
            type: "POST",
            headers: header,
            data: {
                'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DROfficeID': DROfficeID
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
                                $("#SevaSindhuReportTableID").DataTable().clear().destroy();

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
                var searchString = $('#SevaSindhuReportTableID input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#SevaSindhuReportTableID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DROfficeWiseSummaryTableID.search('').draw();
                            $("#SevaSindhuReportTableID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }
        },
        serverSide: true,

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


 

        columns: [
          
        
                            


            { data: "SrNo", "searchable": true, "visible": true, "name": "srNo", "width": '5%' },
            { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName", "width": '20%' },
            { data: "RefNo", "searchable": true, "visible": true, "name": "RefNo", "width": '5%' },
            { data: "Akg", "searchable": true, "visible": true, "name": "Akg"},
            { data: "AppRecivedDate", "searchable": true, "visible": true, "name": "AppRecivedDate","width": '40%' },
            { data: "AppointmentDateTime", "searchable": true, "visible": true, "name": "AppointmentDateTime", "width": '40%' },
            { data: "MarrigeType", "searchable": true, "visible": true, "name": "MarrigeType", "width": '10%' },
            { data: "CaseNo", "searchable": true, "visible": true, "name": "CaseNo" },
            { data: "RegDate", "searchable": true, "visible": true, "name": "RegDate", "width": '40%' },
            { data: "ApplicationStatus", "searchable": true, "visible": true, "name": "ApplicationStatus" },
            { data: "AcceptDateTime", "searchable": true, "visible": true, "name": "AcceptDateTime" },
            { data: "RejectDateTime", "searchable": true, "visible": true, "name": "RejectDateTime" },
            { data: "RejectionReason", "searchable": true, "visible": true, "name": "RejectionReason" },




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



function EXCELDownloadFun(FromDate,ToDate,SROfficeID) {   
    //FromDate = $("#txtFromDate").val();
    //ToDate = $("#txtToDate").val();
    ////alert("FromDate" + FromDate);
    ////alert("ToDate" + ToDate);
    //SROfficeID = $("#SROOfficeListID option:selected").val();
    //DROfficeID = $("#DROOfficeListID option:selected").val();
    ////alert("SROfficeID" + SROfficeID);
    //BRIDEPersonID = $("input[name='BRIDEPersonID']:checked").val();

    //BRIDEGroomPersonID = $("input[name='BRIDEGroomPersonID']:checked").val();
    //WitnessCount = $("input[name='WitnessCount']:checked").val();
    //ReceiptCount = $("input[name='ReceiptCount']:checked").val();

    //window.location.href = '/Remittance/MasterData/ExportSummaryReportToExcel?TableId=' + TableId;

    window.location.href = '/MISReports/SevaSindhuApplicationDetails/ExportReportToExcel?FromDate=' + FromDate + '&ToDate=' + ToDate + '&SROfficeID=' + SROfficeID ;


   


}