//Global variables.
var token = '';
var header = {};


$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $('#DROOfficeListnID').change(function () {

        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/SROScriptManager/SROScriptManager/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROOfficeListnID').val() },
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
                    $('#SROOfficeListnID').empty();
                    $.each(data.SROOfficeList, function (i, SROOfficeList) {
                        SROOfficeList
                        $('#SROOfficeListnID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                    });
                    $('#SROOfficeListnID').val($('#hdnSROfficeID').val());
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });

    $('#dtnReleaseDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,

    });

    $('#divnReleaseDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),

        pickerPosition: "bottom-left"

    });

    $('#dtnLastDateForPatchUpdate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        maxDate: '+15d',
        endDate: '+15d',

        pickerPosition: "bottom-left"

    });

    $('#divnLastDateForPatchUpdate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        maxDate: '+15d',
        endDate: '+15d',


        pickerPosition: "bottom-left"

    });

    $('#dtnSPExecutionDateTime').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

    });
    
    $('#divnSPExecutionDateTime').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });



    $('#DROOfficeListnID').val($('#hdnDROfficeID').val());
    
    if ($("#hdnIsDRO").val() == 'False') {
        //alert("SRO Office");
        $("#rdoSROOffice").attr('checked', true).trigger('click');
        $("#hdnIsDROOfficestr").val("1");
        $("#SROOfficeListnID").prop("disabled", false);
        $("#dtnSPExecutionDateTimen").prop("disabled", true);
        //console.log($('#hdnSROfficeID').val());
        //$('#SROOfficeListnID').val($('#hdnSROfficeID').val());
        $('#DROOfficeListnID').trigger("change");
    }
    else {
        //alert("DRO Office");
        $("#rdoDROOffice").attr('checked', true).trigger('click');
        $("#hdnIsDROOfficestr").val("2");
        $("#SROOfficeListnID").prop("disabled", true);
        $("#dtnSPExecutionDateTimen").prop("disabled", false);
    }

    $('input[type=radio]').change(function () {
        //alert(this.value);
        if (this.value == '11') {
            $("#hdnIsDROOfficestr").val("11");
            $("#SROOfficeListnID").prop("disabled", false);
            $("#dtSPExecutionDateTimen").prop("disabled", true);
        }
        if (this.value == '22') {
            $("#hdnIsDROOfficestr").val("22");
            $("#SROOfficeListnID").prop("disabled", true);
            $("#dtSPExecutionDateTimen").prop("disabled", false);
        }

    });


    $("#btnUpdateAppVersion").click(function () {

        var officeTypestr;
        if ($('#rdoSROOffice').is(':checked'))
            officeTypestr = 'SR Office';
        if ($('#rdoDROOffice').is(':checked'))
            officeTypestr = 'DR Office';
        bootbox.confirm({
            message: "Application Version Details will be Updated for " + officeTypestr,
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                //console.log('This was logged in the callback: ' + result);
                if (result == true) {
                   // UpdateAppVersionDetails();
                   UpdateApplicationVersionDetails();
                }
                else {
                    return;
                }
            }
        });

    });


    


});

function UpdateApplicationVersionDetails() {

    //$.validator.unobtrusive.parse(form);

    //if ($("#dvAppVersionForm").valid()) {
    console.log($("#dvEditAppVersionForm").valid());
    
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/SROScriptManager/SROScriptManager/UpdateAppVersionDetails",
        data: $("#dvEditAppVersionForm").serialize(),
        headers: header,
        success: function (data) {
            if (data.success) {
                bootbox.alert({
                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                    callback: function () {
                        window.location.href = "/SROScriptManager/SROScriptManager/AppVersionView";
                    }
                });
            }
            else {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                });
            }
            $.unblockUI();
        },
        error: function () {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
            $.unblockUI();

        }
    });

}