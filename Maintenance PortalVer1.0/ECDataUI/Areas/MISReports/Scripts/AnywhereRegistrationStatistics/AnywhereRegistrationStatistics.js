var token = '';
var header = {};
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    var txtFromDate;
    var txtToDate;
    var DistrictID;
    var txtDistrict;
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


    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });



    $("#SearchBtn").click(function () {
        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        DistrictID = $("#DROOfficeListID option:selected").val();
        txtDistrict = $("#DROOfficeListID option:selected").text();

        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/AnywhereRegistrationStatistics/LoadAnywhereRegStatTable/',
            datatype: "json",
            type: "POST",
            headers: header,
            data: {
                'FromDate': txtFromDate, 'ToDate': txtToDate, 'DistrictID': DistrictID
            },
            success: function (data) {
                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                    $('#DtlsSearchParaListCollapse').trigger('click');
                unBlockUI();
                $('#test').html(data);

                //if (data.errorMessage != null) {
                //    if (data.errorMessage.length != 0) {
                //        bootbox.alert({
                //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                //            callback: function () {
                //            }
                //        });
                //    }
                //}
                $.unblockUI();
            },

            error: function () {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                unBlockUI();
            }
        });
    });

  
});