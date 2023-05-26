//Global variables.
var token = '';
var header = {};


$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    if ($("#hdnIsActive").val() == 'True')
        $('#rdnEnable').prop('checked', true);    
    else 
        $('#rdnDisable').prop('checked', true);
       
    $("#btnDownloadcript").click(function () {
        window.location.href = "/SROScriptManager/SROScriptManager/DownLoadScriptFile?ScriptID=" + $("#hdnScriptID").val();
    });

    $("#btnUpdateScript").click(function () {
        if ($('#rdnEnable').is(':checked')) 
            $("#hdnIsActive").val('True');        
        else 
            $("#hdnIsActive").val('False');
        UpdateInScriptManger();
    });


    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/SROScriptManager/SROScriptManager/GetSROOfficeListByDistrictID',
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

});



function UpdateInScriptManger() {

    //console.log("in UpdateInScriptManger");

    var formData = new FormData();
    var filesArray = null;
    $.each($("input[type='file']"), function () {
        var id = $(this).attr('id');
        var fileData = document.getElementById(id).files[0];
        if (fileData != undefined) {
            formData.append(id, fileData);
            if (filesArray == null) {
                filesArray = id + ",";
            }
            else {
                filesArray += id + ",";
            }
        }
    });

    formData.append("filesArray", filesArray);
    var form = $("#ScriptManagerUpdateForm");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    //$.validator.unobtrusive.parse(form);

    if ($("#ScriptManagerUpdateForm").valid()) {
        //console.log("in ScriptManagerInsertForm valid");
        BlockUI();
        $.ajax({
            type: "POST",
            url: "/SROScriptManager/SROScriptManager/UploadSQLScriptFile",
            data: formData,
            headers: header,
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.success) {
                    $("#hdnUpdateSQLScriptContent").val(data.FileDataStr);
                    BlockUI();
                    $.ajax({
                        type: "POST",
                        url: "/SROScriptManager/SROScriptManager/UpdateScriptManager",
                        data: $("#ScriptManagerUpdateForm").serialize(),
                        headers: header,
                        success: function (data) {
                            if (data.success) {
                                bootbox.alert({
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function () {
                                        window.location.href = "/SROScriptManager/SROScriptManager/SROScriptManagerDetailsView";
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
                else {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                    });
                    $.unblockUI();
                }
            },
            error: function () {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                });
                $.unblockUI();
            }
        });
    }
    else {
        //console.log("in ScriptManagerInsertForm NOTvalid");
    }



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