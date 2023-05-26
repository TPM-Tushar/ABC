
var token = '';
var header = {};
var selectedApplicationTypeText;
var txtFromDate;
var txtToDate;
var SroID;
var statusTosend;
var paymentPendingsince;
var Day;


// BELOW FUNCTION PARAMATERS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
var days;
var daysForExcel = 0;
var LongestPaymentPendingSinceDate;
// ABOVE FUNCTION PARAMATERS ADDED BY SHUBHAM BHAGAT ON 14-12-2020

$(document).ready(function () {


    // alert("KOS");
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    //SelectedApplicationTypeText = $('#DROOfficeListID').val();

    $('#txtFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"


    });

    $('#txtFromDate').datepicker()
        .on("input change", function (e) {
            // alert($(this).val());
            var changedFromdate = ($(this).val());

            $('#txtToDate').datepicker({

                autoclose: true,
                format: 'dd/mm/yyyy',
                endDate: '+0d',
                minDate: changedFromdate,
                pickerPosition: "bottom-left"


            });

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


    $('#DROOfficeListID').change(function () {
        
        blockUI('loading data.. please wait...');
        //$("#KOSPaymentStatusReportDetailsTable").DataTable().clear().destroy();
        $('#tableToBeLaded').empty();
        $("#datatable2").hide();
        $.ajax({
            url: '/MISReports/KOSPaymentStatusReport/GetSROOfficeListByDistrictID',
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

    $('#SROOfficeListID').change(function () {
        
        blockUI('loading data.. please wait...');
        //$("#KOSPaymentStatusReportDetailsTable").DataTable().clear().destroy();
        $('#tableToBeLaded').empty();
        $("#datatable2").hide();
        unBlockUI();
    });


    $("#SearchBtn").click(function () {

        $("#datatable2").hide();


        // alert($('#ApplicationTypeIdListID option:selected').val());


        if ($.fn.DataTable.isDataTable("#KOSPaymentStatusReportDetailsTable")) {
            $("#KOSPaymentStatusReportDetailsTable").DataTable().clear().destroy();
        }

        if ($.fn.DataTable.isDataTable("#KOSPaymentStatusReportDataTable")) {
            $("#KOSPaymentStatusReportDataTable").DataTable().clear().destroy();
        }

        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        selectedApplicationTypeText = $("#ApplicationTypeListID option:selected").val();

        if (Date.parse(txtFromDate) > Date.parse(txtToDate)) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">From Date should be less than To Date.</span>');

            $('#tableToBeLaded').hide();

            return;
        }
        else {
            $('#tableToBeLaded').show();

        }


        if (txtFromDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
            return;
        }
        else if (txtToDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;

        }
        var _SroCode = $("#SROOfficeListID").val();
        var _DistrictCode = $("#DROOfficeListID").val();
       
        blockUI('Loading data please wait.');
        $.ajax({
            type: "POST",
            url: '/MISReports/KOSPaymentStatusReport/KOSPaymentStatusReportDetails',
            cache: false,
            headers: header,
            data: {
                'FromDate': txtFromDate, 'ToDate': txtToDate, 'ApplicationTypeId': selectedApplicationTypeText,'SroCode':_SroCode,'DistrictCode':_DistrictCode
            },

            success: function (data) {
                unBlockUI();
                $('#tableToBeLaded').html(data);

            },
            error: function (xhr, status, err) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while proccessing your request : " + err + '</span>',
                    callback: function () {

                    }
                });
                unBlockUI();
            }
        });
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');


    });


    //Document.Ready finishes below:   
});

// BELOW FUNCTION PARAMATERS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
//function LoadKOSPaymentStatusReportDataTable(status) {
function LoadKOSPaymentStatusReportDataTable(status, days, date) {
    //alert("Hyperlink clicked");


    if ($.fn.DataTable.isDataTable("#KOSPaymentStatusReportDataTable")) {
        $("#KOSPaymentStatusReportDataTable").DataTable().clear().destroy();
    }

    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    SelectedApplicationTypeText = $("#ApplicationTypeListID option:selected").val();
    //alert(status);
    statustosend = status;

    // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
    // BECAUSE FOR STATUS 6 WE HAVE TO SEND DAYS AND FOR STATUS 7 WE HAVE TO SEND DAYS AND
    // IN FROMDATE AND TODATE WE HAVE TO PASS "Longest Payment Pending Since" COLUMN DATE VALUE
    if (status == 6) {
        days = days;
        daysForExcel = days;
    }
    if (status == 7) {
        days = days;
        daysForExcel = days;
        txtFromDate = date;
        txtToDate = date;
        LongestPaymentPendingSinceDate = date;
    }   
    // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020

    if (txtFromDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
        return;
    }
    else if (txtToDate == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
        return;

    }
    var _SroCode = $("#SROOfficeListID").val();
    var _DistrictCode = $("#DROOfficeListID").val();

    var KOSPaymentStatusReportDataTable = $('#KOSPaymentStatusReportDataTable').DataTable({


        ajax: {

            url: '/MISReports/KOSPaymentStatusReport/LoadKOSPaymentStatusReportDataTable',
            type: "POST",
            headers: header,
            data: {
                'FromDate': txtFromDate, 'ToDate': txtToDate, 'ApplicationTypeId': SelectedApplicationTypeText, 'status': statustosend,
                // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
                'Days': days, 'SROfficeID': _SroCode, 'DROfficeID': _DistrictCode
                // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
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


                                var classToRemove = $('#DtlsToggleIconSearchPara2').attr('class');
                                if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse2').trigger('click');

                                $("#KOSPaymentStatusReportDataTable").DataTable().clear().destroy();
                            }
                        }
                    });
                }
                else {

                    $("#datatable2").show();

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

        },



        "pageLength": 10,
        //"iDisplayLength": 10,
        bserverSide: true,
        "scrollY": "300px",
        "scrollCollapse": true,
        //bPaginate: true,
        //bLengthChange: true,
        //bInfo: true,
        //info: true,
        bFilter: false,
        //searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        //Added by Madhusoodan on 10/08/2003
        columnDefs: [

            { targets: "_all", orderable: false, "className": "text-center" }

        ],

        columns: [

            { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
            { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
            { data: "ApplicationType", "searchable": true, "visible": true, "name": "ApplicationType" },
            { data: "ApplicationNumber", "searchable": true, "visible": true, "name": "ApplicationNumber" },
            { data: "TransactionDate", "searchable": true, "visible": true, "name": "TransactionDate" },
            { data: "ChallanReferencenNumber", "searchable": true, "visible": true, "name": "ChallanReferencenNumber" },
            { data: "PaymentStatus", "searchable": true, "visible": true, "name": "PaymentStatus" },
            { data: "PaymentRealizedInDays", "searchable": true, "visible": true, "name": "PaymentRealizedInDays" },


        ],
        fnInitComplete: function (oSettings, json) {

            $("#PDFSPANID2").html(json.PDFDownloadBtn);

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


function PaymentPendingsinceChanged(value) {

    // alert($('#paymentpendingsince option:selected').val());
    $("#datatable2").hide();


    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    selectedApplicationTypeText = $("#ApplicationTypeListID option:selected").val();


    paymentPendingsince = $("#paymentpendingsince option:selected").val();
    var _SroCode = $("#SROOfficeListID").val();
    var _DistrictCode = $("#DROOfficeListID").val();

    blockUI('Loading data please wait.');

    $.ajax({
        type: "POST",
        url: '/MISReports/KOSPaymentStatusReport/PaymentPendingSince',
        cache: false,
        headers: header,
        data: {
            'FromDate': txtFromDate, 'ToDate': txtToDate, 'ApplicationTypeId': selectedApplicationTypeText, 'paymentPendingsince': paymentPendingsince, 'SROfficeID': _SroCode, 'DROfficeID': _DistrictCode
        },

        success: function (json) {
            unBlockUI();

            for (i = 0; i < json.data.length; i++) {

                var days = JSON.parse(json.data[i].PaymentPendingSince);

                document.getElementById("paymentpendingsincetd").innerHTML = days;

                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-12-2020 
                //alert(days);
                //alert(document.getElementById("paymentpendingsincetd"));
                //$('#paymentpendingsincetd').off('click');
                $('#paymentpendingsincetd').attr("onclick", "LoadKOSPaymentStatusReportDataTable(6," + days + ")");
                //document.getElementById("paymentpendingsincetd").onclick = LoadKOSPaymentStatusReportDataTable(6, days); 
                //alert(document.getElementById("paymentpendingsincetd"));
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-12-2020 

            }



        },
        error: function (xhr, status, err) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while proccessing your request : " + err + '</span>',
                callback: function () {

                }
            });
            unBlockUI();
        }
    });










}



function EXCELDownloadFun() {

    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    selectedApplicationTypeText = $("#ApplicationTypeListID option:selected").val();

    //alert(selectedApplicationTypeText);
    var _SroCode = $("#SROOfficeListID").val();
    var _DistrictCode = $("#DROOfficeListID").val();
    // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
    //alert($("#paymentpendingsincetd").text());
    //window.location.href = '/MISReports/KOSPaymentStatusReport/KOSPaymentStatusReportToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&ApplicationTypeId=" + selectedApplicationTypeText;
    window.location.href = '/MISReports/KOSPaymentStatusReport/KOSPaymentStatusReportToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&ApplicationTypeId=" + selectedApplicationTypeText + "&PaymentPendingSinceDays=" + $("#paymentpendingsincetd").text() + "&DistrictCode=" + _DistrictCode + "&SroCode=" + _SroCode;
    // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020

}


function DownloadExel() {

    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    selectedApplicationTypeText = $("#ApplicationTypeListID option:selected").val();
    //statustosend = status;

    // alert(selectedApplicationTypeText);

    // alert(statustosend);
    var _SroCode = $("#SROOfficeListID").val();
    var _DistrictCode = $("#DROOfficeListID").val();
    // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
    // BECAUSE FOR STATUS 6 WE HAVE TO SEND DAYS AND FOR STATUS 7 WE HAVE TO SEND DAYS AND
    // IN FROMDATE AND TODATE WE HAVE TO PASS "Longest Payment Pending Since" COLUMN DATE VALUE
    if (statustosend == 6) {
        days = daysForExcel;
    }
    else if (statustosend == 7) {
        days = daysForExcel;
        txtFromDate = LongestPaymentPendingSinceDate;
        txtToDate = LongestPaymentPendingSinceDate;
    }
    else {
        days = 0;
    }
    // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020

    // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
    //window.location.href = '/MISReports/KOSPaymentStatusReport/KOSPaymentStatusReportTableToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&ApplicationTypeId=" + selectedApplicationTypeText;
    window.location.href = '/MISReports/KOSPaymentStatusReport/KOSPaymentStatusReportTableToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&ApplicationTypeId=" + selectedApplicationTypeText + "&status=" + statustosend + "&days=" + days + "&DistrictCode=" + _DistrictCode + "&SroCode=" + _SroCode;
    // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
}

