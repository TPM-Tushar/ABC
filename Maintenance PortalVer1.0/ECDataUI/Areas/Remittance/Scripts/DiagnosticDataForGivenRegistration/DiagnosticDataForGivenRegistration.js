//Global variables.
var SelectedRegistrationModule;
var FinalRegNumber;
var token = '';
var header = {};


$(document).ready(function () {


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;



    $("#DownloadBtn").click(function () {
       


        FinalRegNumber = $("#txtFinalRegNo").val();
        SelectedRegistrationModule = $("#RegistrationModuleDropDownID option:selected").val();


        if (FinalRegNumber.trim() == "" || FinalRegNumber == undefined) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Final Registration Number.</span>');
            return;
        }
        //window.location.href = "/Remittance/DiagnosticDataForGivenRegistration/DownloadDiagnosticDataInsertScript?RegistrationModuleCode=" + SelectedRegistrationModule + "&FinalRegistrationNumber=" + FinalRegNumber;



        $.ajax({
            url: '/Remittance/DiagnosticDataForGivenRegistration/DownloadDiagnosticDataInsertScript',
            data: {
                "RegistrationModuleCode": SelectedRegistrationModule,
                "FinalRegistrationNumber": FinalRegNumber
            },
            datatype: "json",
            type: "GET",
            headers: header,
            //type: "POST",
            success: function (data) {
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage";
                        });
                }
                else {
                    if (data.serverError == false && data.status == false) {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                    }
                    else {

                        window.location.href = "/Remittance/DiagnosticDataForGivenRegistration/DownloadDiagnosticDataInsertScript?RegistrationModuleCode=" + SelectedRegistrationModule + "&FinalRegistrationNumber=" + FinalRegNumber;

                        //var blob = new Blob([data.FileContent], { type: "text/plain;charset=utf-8" });
                        //saveAs(blob, data.FileName);

                        //if (data.IsFileExistAtDownloadPath) {                        
                        //    //window.location.href = '/MISReports/DataRestorationReport/DownloadScriptForRectification?InitID=' + InitID + '&scriptID=' + scriptID + '&SroID=' + DT_SROCodeForDownloadFile;
                        //}
                        //else {
                        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error.</span>');
                        //}
                    }
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');

            }
        });




    });

});


//Block UI
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