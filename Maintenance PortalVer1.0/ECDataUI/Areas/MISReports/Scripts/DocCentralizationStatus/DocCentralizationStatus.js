//Global variables.
var token = '';
var header = {};
var SROName;
var Date;
$(document).ready(function () {

    $('#txtFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,

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

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });
    $("#SearchBtn").click(function () {
        SROName = $("#SROOfficeListID option:selected").text();
        RegDate = $("#txtFromDate").val();
        blockUI('Loading data please wait.');
        $.ajax({
            type: "POST",
            url: "/MISReports/DocCentralizationStatus/LoadDocCentralizationStatusDataTable/",
            cache: false,
            headers: header,
            data: $("#SearchParametersForm").serialize(),
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