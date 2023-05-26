//Global variables.
var token = '';
var header = {};

$(document).ready(function () {



    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $("#txtticketNumber").val('');



    $("#zipFile").change(function () {

        var fileExtension = ['zip', 'ZIP'];//, 'xls'

        if ($.inArray($(this).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Only ZIP file is allowed ." + '</span>',
                callback: function () { }
            });

            $(this).val('');
            return false;
        }

        //var file = document.getElementById(this.id).files.item(0);
        //var docid = $(this).attr('id');
        //if (file.size > MaxFileSizeToUpload) {

        //    var MaxFilesizeInMB = MaxFileSizeToUpload / 1048576;

        //    bootbox.alert({
        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "File size is greater than " + MaxFilesizeInMB + ' MB. ' + 'Please upload another file</span>',
        //        callback: function () { }
        //    });
        //    $(this).val('');
        //    return false;

        //}
    });

    $("#SQLPatchFile").change(function () {

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
        if (file.size > MaxFileSizeToUpload) {

            var MaxFilesizeInMB = MaxFileSizeToUpload / 1048576;

            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "File size is greater than " + MaxFilesizeInMB + ' MB. ' + 'Please upload another file</span>',
                callback: function () { }
            });
            $(this).val('');
            return false;

        }
    });


    $("#btnGeneratekeyPair").click(function () {
        GenerateSaveKeyPair();
    });

    $("#btnDecryptFile").click(function () {
        $("#hdnFilePath").val($("#zipFile").val());
        DecryptEnclosure();
    });

    $("#btnEncryptPatchFile").click(function () {
        $("#hdnSQLPatchFile").val($("#SQLPatchFile").val());
        EncryptPatchFile();
    });

    $("#btnRegister").click(function () {

        RegisterTicketDetailsAndGenerateKeyPair();
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

function GenerateSaveKeyPair() {

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/KaveriSupport/KaveriSupport/GenerateKeyPair",
        success: function (data) {
            if (data.success) {
                bootbox.alert({
                    message: '<i class="fa-check text-success boot-icon boot-icon"></i><span class="boot-alert-txt"> Public Key generated and uploaded successully</span>'
                });
            }
            else {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Failed to generate and upload public key </span>'
                });
            }
            $.unblockUI();
        },
        error: function () {
            bootbox.alert({
                //   size: 'small',
                //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
        }
    });
}

function DecryptEnclosure() {

    var vTicketNumber = $("#txtticketNumber").val();
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
    var other_data = vTicketNumber;
    formData.append("TicketNumber", other_data);

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/KaveriSupport/KaveriSupport/DecryptEnclosureFile",
        data: formData,
        headers: header,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.success) {

                $("#txtticketNumber").val("");
                $("#zipFile").val("");
                // alert("Ticket ::::" + data.TicketNumber);

                window.location.href = "/KaveriSupport/KaveriSupport/DownloadFile?Filepath=" + data.Filepath + "&TicketNumber=" + data.TicketNumber;//+ data.FileBytes;
            }
            else {


                if (data.message == undefined) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured. because " + ' <ul style="list-style-type: disc;font-weight:normal">' + '<li> Either Duplicate Session Found</li>' + '<li>Invalid Request</li>' + ' <li> Poor network connectivity</li>' + '</span>',
                        callback: function () {

                            window.location.href = '/Error/SessionExpire';
                            //    $('#btnLoggOffID').trigger('click');
                            $.unblockUI();
                            //   return false;
                        }
                    });
                }
                else {

                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                    });
                }
            }
            $.unblockUI();
        },
        error: function () {
            bootbox.alert({
                //   size: 'small',
                //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
        }
    });
}


function EncryptPatchFile() {

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

    var vTicketNumber = $("#txtticketNumber").val();


    BlockUI();
    $.ajax({
        type: "POST",
        url: "/KaveriSupport/KaveriSupport/UploadSQLPatchFile",
        data: formData,
        headers: header,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.success) {

                $("#hdnSQLPatchFile").val(data.filePath);

                //    window.location.href = "/KaveriSupport/KaveriSupport/EncryptSQLPatchFile?sqlFilePath=" + data.filePath + "&TicketNumber=" + vTicketNumber;
                BlockUI();
                $.ajax({
                    type: "POST",
                    url: "/KaveriSupport/KaveriSupport/EncryptSQLPatchFile",
                    data: { sqlFilePath: data.filePath, TicketNumber: vTicketNumber },
                    headers: header,

                    //  processData: false,
                    // contentType: false,
                    success: function (data) {
                        if (data.success) {

                            bootbox.alert({
                                message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                callback: function () {

                                    window.location.href = "/KaveriSupport/KaveriSupport/EncryptSQLPatch";
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


function RegisterTicketDetailsAndGenerateKeyPair() {


    var form = $("#TicketRegForm");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
    if ($("#TicketRegForm").valid()) {


        $.ajax({
            type: "POST",
            url: "/KaveriSupport/KaveriSupport/RegisterTicketDetailsAndGenerateKeyPair",
            data: $("#TicketRegForm").serialize(),
            headers: header,
            success: function (data) {
                if (data.success) {

                    bootbox.alert({
                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                        callback: function () {
                            //Reload Page
                            window.location.href = "/KaveriSupport/KaveriSupport/KaveriSupport";
                        }
                    });
                }
                else {

                    if (data.message == undefined) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured. because " + ' <ul style="list-style-type: disc;font-weight:normal">' + '<li> Either Duplicate Session Found</li>' + '<li>Invalid Request</li>' + ' <li> Poor network connectivity</li>' + '</span>',
                            callback: function () {

                                window.location.href = '/Error/SessionExpire';
                                //    $('#btnLoggOffID').trigger('click');
                                $.unblockUI();
                                //   return false;
                            }
                        });
                    }
                    else {

                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                        });
                    }
                }
                $.unblockUI();
            },
            error: function () {
                bootbox.alert({
                    //   size: 'small',
                    //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                });
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