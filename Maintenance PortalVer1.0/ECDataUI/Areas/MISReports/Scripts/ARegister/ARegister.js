var token = '';
var header = {};
var test = '';
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    var txtFromDate;
    var txtToDate;

    $('#txtForDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"
    });

    $('#divForDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });

    $('#txtForDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    });

    if (IsSRLogin == 'false' || IsSRLogin == 'FALSE' || IsSRLogin == 'False') {
        $("#SROOfficeListID").removeAttr('disabled');
    }
    else {
        $("#SROOfficeListID").attr('disabled', 'disabled');

    }

    $("#BtnGenerateReport").click(function () {
        //alert('btn click');
        $("#objPDFViewer").attr('data', '');
        GenerateReport();
    });

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });
});

function GenerateReport() {
    //alert('fun call');

    blockUI('loading data.. please wait...');

    if ($("#SROOfficeListID option:selected").val() == '0') {
        unBlockUI();
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select SRO Office</span>'
            
        });
        return;
        $.unblockUI();
    }

    $.ajax({
        url: '/MISReports/ARegister/GenerateReport',
        data: { 'SROOfficeID': $("#SROOfficeListID option:selected").val(), 'ForDate': $("#txtForDate").val() },
        headers: header,
        type: "POST",
        dataType: "json",
        success: function (data) {
            unBlockUI();
            console.log(data);
            if (!data.success) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                });
            }

            else {
                //bootbox.alert({
                //    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                //});
                console.log(data);
                //if (data.messageGeneration != "" || data.messageGeneration != undefined || data.messageGeneration != null)
                if (data.messageGeneration != "" )
                {
                    var bootboxConfirm = bootbox.alert({
                        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                        message: "<span class='boot-alert-txt'>" + data.messageGeneration + "</span>",

                        //buttons: {
                        //    cancel: {
                        //        label: '<i class="fa fa-times"></i> No',
                        //        className: 'pull-right margin-left-NoBtn'
                        //    },
                        //    confirm: {
                        //        label: '<i class="fa fa-check"></i> Yes'
                        //    }
                        //},
                        callback: function (result) {
                            //$("#objPDFViewer").attr('data', '');
                            //$("#objPDFViewer").attr('data', data.URL);
                            //var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                            //if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                            //    $('#DtlsSearchParaListCollapse').trigger('click');
                            //}
                        }
                    });
                }
                else {
                    $("#objPDFViewer").attr('data', '');
                    $("#objPDFViewer").attr('data', data.URL);
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                        $('#DtlsSearchParaListCollapse').trigger('click');
                    }
                }
               
            }
        },
        error: function (xhr) {
            //alert("error " + xhr);
            unBlockUI();
            test = xhr;
            //var err = eval("(" + xhr.responseText + ")");
            alert(xhr);
        }
    });
}