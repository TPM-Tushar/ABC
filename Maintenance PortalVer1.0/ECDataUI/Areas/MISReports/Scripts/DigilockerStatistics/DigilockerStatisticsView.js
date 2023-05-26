
var token = '';
var header = {};
var selectedApplicationTypeText;
var txtFromDate;
var txtToDate;
var SroID;
var statusTosend;
var paymentPendingsince;
var Day;


var days;
var daysForExcel = 0;
var LongestPaymentPendingSinceDate;

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

    $('#txtFromDate').datepicker()
        .on("input change", function (e) {

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





    $("#SearchBtn").click(function () {



        if ($.fn.DataTable.isDataTable("#DigilockerStatisticsReportDetailsTable")) {
            $("#DigilockerStatisticsReportDetailsTable").DataTable().clear().destroy();
        }

        if ($.fn.DataTable.isDataTable("#DigilockerStatisticsReportDataTable")) {
            $("#DigilockerStatisticsReportDataTable").DataTable().clear().destroy();
        }

        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        

        //if (Date.parse(txtFromDate) < Date.parse(txtToDate)) {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">From Date should be less than To Date.</span>');

        //    $('#tableToBeLaded').hide();

        //    return;
        //}
        //else {
        //    $('#tableToBeLaded').show();

        //}


        if (txtFromDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
            return;
        }
        else if (txtToDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;

        }
        
        blockUI('Loading data please wait.');
        $.ajax({
            type: "POST",
            url: '/MISReports/DigilockerStatistics/DigilockerStatisticsReportDetails',
            cache: false,
            headers: header,
            data: {
                'FromDate': txtFromDate, 'ToDate': txtToDate
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

 
});



function EXCELDownloadFun() {

    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    window.location.href = '/MISReports/DigilockerStatistics/DigilockerStatisticsReportToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate ;
    
}


