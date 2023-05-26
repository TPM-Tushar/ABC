$(document).ready(function () {
    $("#btnUpload").click(function () {



        var fileUpload = $("#FileUpload1").get(0);
        var files = fileUpload.files;

        // Create FormData object  
        var formData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            formData.append(files[i].name, files[i]);
        }


        if ($("#ScriptRectificationHistoryID").val() === "")
            bootbox.alert("Please enter Corrective action");
        else {
            // Adding one more key to FormData object  
            formData.append('INITID_STR', $("#INITID_STR_ForUpload_ID").val());

            formData.append('SCRIPT_ID_STR', $("#SCRIPT_ID_STR_ForUpload_ID").val());

            formData.append('ScriptRectificationHistory_STR', $("#ScriptRectificationHistoryID").val());

            var RequestToken = $('[name=__RequestVerificationToken]').val();
            formData.append("__RequestVerificationToken", RequestToken);

            $.ajax({
                url: '/MISReports/DataRestorationReport/SaveUplodedRectifiedScript',
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data.errorMessage == undefined) {

                        // load partial view
                        $.ajax({
                            url: '/MISReports/DataRestorationReport/DataInsertionTable',
                            data: {
                                "scriptID": $("#SCRIPT_ID_STR_ForUpload_ID").val(),
                                "InitID": $("#INITID_STR_ForUpload_ID").val(),
                                // FLAG ADDED ON 10-07-2020 AT 4:45 PM 
                                "IsRectifiedScriptUploaded": "true",
                            },
                            datatype: "json",
                            headers: header,
                            type: "POST",
                            success: function (data) {
                                //add validations
                                if (data.errorMessage == undefined) {
                                    $('#dataInsertionTableDivID').html('');
                                    $('#dataInsertionTableDivID').html(data);
                                    unBlockUI();
                                }
                                else if (data.serverError == true) {
                                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                                        function () {
                                            window.location.href = "/Home/HomePage"
                                        });
                                }
                                else {
                                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                                }
                            },
                            error: function (xhr) {
                                bootbox.alert({
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                    callback: function () {
                                    }
                                });
                                unBlockUI();
                            }
                        });

                        bootbox.alert(data.RectifiedScriptUploadedMsg);
                        $("#DownloadScriptForTechAdminID").html('');
                        $("#UploadScriptForTechAdminID").html('');
                        $("#UploadScriptPartialViewDivID").html('');                        
                        unBlockUI();
                    }
                    else if (data.serverError == true) {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            function () {
                                window.location.href = "/Home/HomePage"
                            });
                    }
                    else {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                    }
                },
                error: function (xhr, status, err) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>'
                    });
                }

            });
        }
        
    });
});