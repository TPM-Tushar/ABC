//Global variables.
var token = '';
var header = {};


$(document).ready(function () {
    
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    
    $("#txtticketNumber").val('');
            

    $("#SQLScriptManagerFile").change(function () {
        //alert('regre')
        var fileExtension = ['sql'];//, 'xls'

        if ($.inArray($(this).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Only SQL file is allowed ." + '</span>',
                callback: function () { }
            });

            $(this).val('');
            return false;
        }

        var file = document.getElementById(this.id).files.item(0);
        var docid = $(this).attr('id');
        //alert(file.size);
        //alert(MaxFileSizeToUpload);
        if (file.size > MaxFileSizeToUpload) {

            alert(MaxFileSizeToUpload);
            var MaxFilesizeInMB = MaxFileSizeToUpload / 1048576;

            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "File size is greater than " + MaxFilesizeInMB + ' MB. ' + 'Please upload another file</span>',
                callback: function () { }
            });
            $(this).val('');
            return false;

        }
        //alert(docid);
    });

    $("#btnInsertscript").click(function () {
        $("#hdnSQLScriptManagerFile").val($("#SQLScriptManagerFile").val());
        InsertInScriptManger();
    });
    

    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });



});

function InsertInScriptManger() {


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

    var form = $("#ScriptManagerInsertForm");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    //url: "/SROScriptManager/SROScriptManager/InsertScriptManager",
    //    data: $("#ScriptManagerInsertForm").serialize(),

    if ($("#ScriptManagerInsertForm").valid()) {
        
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

                    $("#hdnSQLScriptContent").val(data.FileDataStr);
                    

                    BlockUI();
                    $.ajax({
                        type: "POST",
                        url: "/SROScriptManager/SROScriptManager/InsertScriptManager",
                        //data: { sqlFilePath: data.filePath, TicketNumber: vTicketNumber },
                        data: $("#ScriptManagerInsertForm").serialize(),
                        headers: header,

                        //  processData: false,
                        // contentType: false,
                        success: function (data) {
                            if (data.success) {

                                bootbox.alert({
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function () {

                                        window.location.href = "/SROScriptManager/SROScriptManager/SROScriptManagerView";
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
                        error: function (error) {
                            //alert(JSON.stringify(error));
                            //alert('3 else');
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                            });
                            $.unblockUI();

                        }
                    });


                }
                else {

                    //alert('1 else');
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                    });
                    $.unblockUI();

                }

            },
            error: function () {
                //alert('2 else');
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                });
                $.unblockUI();

            }
        });

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