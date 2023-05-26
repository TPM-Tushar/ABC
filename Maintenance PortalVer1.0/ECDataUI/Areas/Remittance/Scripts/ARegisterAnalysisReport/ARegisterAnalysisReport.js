
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

    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true,

    }).datepicker("setDate", FromDate);


    $('#divFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),

        pickerPosition: "bottom-left"
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

function EXCELDownloadFun() {
    BlockUI();
    //alert($("#SROOfficeListID").val());
    FromDate = $("#txtFromDate").val();
    //alert(FromDate);

    var SROfficeID = $("#SROOfficeListID").val();
    var ARegister = $("input[id='ARegister']:checked").val();
    var AnyWhereECARegister = $("input[id='AnyWhereEC_ARegister']:checked").val();
    var KOS_ARegister = $("input[id='KOS_ARegister']:checked").val();

    //alert(ARegister);
    //alert(AnyWhereECARegister);
    //alert(KOS_ARegister);
    $.ajax({
        type: "POST",
        url: "/Remittance/ARegisterAnalysisReport/ValidateSearchParameters",
        data: {
            "FromDate": FromDate,
            "SROfficeID": SROfficeID,
            "ARegister": ARegister,
            "AnyWhereECARegister": AnyWhereECARegister,
            "KOS_ARegister": KOS_ARegister
        },
        success: function (data) {

            var IsReceiptsSynchronized = data.ReceiptsSynchronized;
            var IsARegisterGenerated = data.ARegisterGenerated;

            //alert("IsReceiptsSynchronized" + IsReceiptsSynchronized);
           // alert("IsARegisterGenerated" + IsARegisterGenerated);
            $.unblockUI();
            if (data.success == false && data.serverError == false) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                    });
            }
            else if (data.success == false) {
              

                $.unblockUI();
                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'>  " + data.errorMessage + " Do you want to Download as Excel? " + " </span>",

                    buttons: {
                        cancel: {
                            label: '<i class="fa fa-times"></i> No',
                            className: 'pull-right margin-left-NoBtn'
                        },
                        confirm: {
                            label: '<i class="fa fa-check"></i> Yes'
                        }
                    },
                    callback: function (result) {
                        BlockUI();
                        if (result) {
                            //alert("IsReceiptsSynchronized 2" + IsReceiptsSynchronized);
                           // alert("IsARegisterGenerated 2 " + IsARegisterGenerated);
                            window.location.href = '/Remittance/ARegisterAnalysisReport/GetARegisterAnalysisReportDetails?FromDate=' + FromDate + "&SROfficeID=" + SROfficeID + "&ARegister=" + ARegister + "&AnyWhereECARegister=" + AnyWhereECARegister + "&KOS_ARegister=" + KOS_ARegister + "&IsReceiptsSynchronized=" + IsReceiptsSynchronized + "&IsARegisterGenerated=" + IsARegisterGenerated;
                           $.unblockUI();
                        }
                        $.unblockUI();
                    }
                });




                //
            }
            else {
                BlockUI();
                window.location.href = '/Remittance/ARegisterAnalysisReport/GetARegisterAnalysisReportDetails?FromDate=' + FromDate + "&SROfficeID=" + SROfficeID + "&ARegister=" + ARegister + "&AnyWhereECARegister=" + AnyWhereECARegister + "&KOS_ARegister=" + KOS_ARegister + "&IsReceiptsSynchronized=" + IsReceiptsSynchronized + "&IsARegisterGenerated=" + IsARegisterGenerated;
                $.unblockUI();
                
            }

        },
        error: function (err) {
            $.unblockUI();
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                callback: function () {

                }
            });
            //bootbox.alert("Error: " + err);
        }
    });

}


function BlockUI() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}
/////