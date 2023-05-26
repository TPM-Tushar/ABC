var token = '';
var header = {};
var test = 0;
var pages;
var p;
var resp;
var DataForCorrectedPDF;
var IDs = [];
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    LoadDT();

    $('#DROfficeListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/Utilities/ScannedFileDownload/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROfficeListID').val() },
            type: "GET",
            success: function (data) {
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    $('#SROfficeListID').empty();
                    $.each(data.SROfficeList, function (i, SROfficeList) {
                        SROfficeList
                        $('#SROfficeListID').append('<option value="' + SROfficeList.Value + '">' + SROfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });





    $("#MarriageTypeListID").hide();
    $("#NoticeTypeListID").hide();

    //Commented and added By Tushar on 1 april 2022 for add Document Type Notice

    //$('#DocumentTypeId').change(function () {
    //    if ($("#DocumentTypeId").val() == '1') {
    //        $("#MarriageTypeListID").hide();
    //        $("#BookTypeListID").show();
    //        $("#lblDocNo").html('<label class="PaddingTop10" for="DocumentNumber" style="padding-left:4px;">Document Number</label><label style="color:red">*</label>\n')
    //    }
    //    else {
    //        $("#MarriageTypeListID").show();
    //        $("#BookTypeListID").hide();
    //        $("#lblDocNo").html('<label class="PaddingTop10" for="DocumentNumber" style="padding-left:4px;">Marriage Case No</label><label style="color:red">*</label>\n')
    //    }
    //});
       $('#DocumentTypeId').change(function () {
           if ($("#DocumentTypeId").val() == '1') {
               $("#MarriageTypeListID").hide();
               $("#NoticeTypeListID").hide();
               $("#BookTypeListID").show();
               $("#lblDocNo").html('<label class="PaddingTop10" for="DocumentNumber" style="padding-left:4px;">Document Number</label><label style="color:red">*</label>\n')
           }
           else if ($("#DocumentTypeId").val() == '2') {
               $("#MarriageTypeListID").show();
               $("#BookTypeListID").hide();
               $("#NoticeTypeListID").hide();
               $("#lblDocNo").html('<label class="PaddingTop10" for="DocumentNumber" style="padding-left:4px;">Marriage Case No</label><label style="color:red">*</label>\n')
           } else {
               $("#BookTypeListID").hide();
               $("#MarriageTypeListID").hide();
               $("#NoticeTypeListID").show();
               $("#lblDocNo").html('<label class="PaddingTop10" for="DocumentNumber" style="padding-left:4px;">Notice Number</label><label style="color:red">*</label>\n')
           }
    });

    //End By Tushar on 1 April 2022
});
function LoadDT() {

    var tableScannedFileDownload = $('#ScannedFileLogTableID').DataTable({
        ajax: {

            url: '/Utilities/ScannedFileDownload/LoadScannedFileDownloadLogTable',
            type: "POST",
            headers: header,
            data: {

            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            }
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#ScannedFileLogTableID").DataTable().clear().destroy();

                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                }
                unBlockUI();
                return json.data;
            },
            error: function () {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                //$.unblockUI();
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');
                // Added by SB on 22-3-2019 at 11:06 am
                var searchString = $('#ScannedFileLogTableID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#ScannedFileLogTableID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            //    $('#menuDetailsListTable_filter input').val('');
                            tableScannedFileDownload.search('').draw();
                            $("#ScannedFileLogTableID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        "scrollX": true,
        "scrollY": "300px",
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,
        //"ordering": false,

        columnDefs: [
            //{ orderable: false, targets: [0] },
            //{ orderable: false, targets: [1] },
            //{ orderable: false, targets: [2] },
            //{ orderable: false, targets: [3] },
            //{ orderable: false, targets: [4] },
            //{ orderable: false, targets: [5] },
            //{ orderable: true, targets: [6] },
        ],

        columns: [

            { data: "FRN", "searchable": true, "visible": true, "name": "FRN" },
            { data: "SroName", "searchable": true, "visible": true, "name": "SroName" },
            { data: "FileName", "searchable": true, "visible": true, "name": "FileName" },
            { data: "Filepath", "searchable": true, "visible": true, "name": "Filepath" },
            { data: "DownloadedBY", "searchable": true, "visible": true, "name": "DownloadedBY" },
            { data: "DownloadReason", "searchable": true, "visible": true, "name": "DownloadReason" },
            { data: "DownloadDateTime", "searchable": true, "visible": true, "name": "DownloadDateTime" }

        ],
        fnInitComplete: function (oSettings, json) {
            //$("#PDFSPANID").html(json.PDFDownloadBtn);
            //$("#EXCELSPANID").html(json.ExcelDownloadBtn);
        },
        preDrawCallback: function () {
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
            unBlockUI();
        },
    });

}

//Added By Tushar on 24 March 2022
function loadModal() {

    var validateParameter = ValidateParameters();
    //Added By Tushar on 24 March 2022 for validation
    if (validateParameter) {

        $.ajax({
            url: '/Utilities/ScannedFileDownload/ValidateParameters',
            data: {
                "SROfficeID": SROfficeID, "DROfficeID": DROfficeID, "DocumentNumber": DocumentNumber, "BookTypeID": BookTypeID, "FinancialYear": FinancialYear, "DType": DType, "MarriageTypeID": MarriageTypeID, "DownloadReasonID": DownloadReasonID, "DocumentTypeID": DocumentTypeID
            },
            type: "GET",
            success: function (data) {
                if (data.success == true) {

                    blockUI('loading.. please wait...');
                    $.ajax({
                        url: '/Home/IsMobileNumberVerified',
                        type: "GET",
                        success: function (data) {
                            if (data.success == true) {

                                //alert("ajax success");
                                $.ajax({
                                    type: "GET",
                                    url: "/Home/InputOTPFromUser",
                                    success: function (data1) {

                                        if (data1.success) {
                                            $.ajax({
                                                url: '/Home/LoadModal',
                                                type: "GET",
                                                success: function (data3) {

                                                    $('#divOTPValidationModal').modal({
                                                        backdrop: 'static',
                                                        keyboard: false
                                                    });
                                                    $("#divOTPInput").show();
                                                    $('#OTPID').focus();
                                                    $('#divLoadOTPValidation').html(data3);
                                                    $("#divForMessageResend").show();
                                                    $('#OTPValidationFormId').append('<input type="hidden" id="IsOTPSent" name="IsOTPSent" value="' + data1.IsOTPSent + '"><input type="hidden" id="OTPTypeId" name="OTPTypeId" value="' + data1.OTPTypeId + '"><input type="hidden" id="EncryptedUId" name="EncryptedUId" value="' + data1.EncryptedUId + '">');
                                                    $('#MobileNumbertoDisplayID').html(data1.MobileNumberToDisplay);
                                                    $('#divOTPValidationModal').modal('show');
                                                    unBlockUI();

                                                    $('#CLPP').click(function () {
                                                        $('#divOTPValidationModal').modal('hide');
                                                    })


                                                },
                                                error: function (err) {
                                                    bootbox.alert({
                                                        //   size: 'small',
                                                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                                                        callback: function () {

                                                        }
                                                    });
                                                    // bootbox.alert("Error: " + err);
                                                }
                                            });
                                        }
                                        else {
                                            unBlockUI();
                                            if (data.message == undefined) {
                                                //alert("ajax fail");
                                                bootbox.alert({
                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while processing your request ." + '</span>',
                                                    callback: function () {

                                                        window.location.href = '/Error/SessionExpire';
                                                        //    $('#btnLoggOffID').trigger('click');
                                                        //   return false;
                                                    }
                                                });
                                            }
                                            else {
                                                bootbox.alert({
                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                                    callback: function () {
                                                        $('#divOTPValidationModal').modal('hide');


                                                    }
                                                });
                                            }
                                        }

                                    },
                                    error: function (err) {
                                        bootbox.alert({
                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                                            callback: function () {

                                            }
                                        });
                                    }
                                });
                            }
                            else {
                                unBlockUI();
                                bootbox.alert('<span class="boot-alert-txt"><i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i>Mobile Number not verified. Please verify it first.</span>');
                            }
                        },
                        error: function (xhr) {
                            alert(xhr);
                        }
                    });
                }

                else {
                    unBlockUI();
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                        });
                }
            },
            error: function (xhr) {
                alert("Error Occured while processing...");
                unBlockUI();
            }
        });
    }
}

function ValidateOTP() {
    //alert("In Validate OTP");
    if ($.trim($('#OTPID').val()) == '') {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please Enter OTP " + '</span>',
            callback: function () {
            }
        });
    }
    if ($.trim($('#OTPID').val()) != '') {
        $.ajax({
            url: "/Home/GetSessionSalt",
            type: "GET",
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    // alert("in if 1");
                    if (data.dataMessage != null) {
                        //  alert("in if 2");
                        var salt = data.dataMessage;
                        var singleEncryptedOTP = hex_sha512($('#OTPID').val());
                        var concathash = singleEncryptedOTP + rstr2hex(salt).toUpperCase();
                        var doubleEncryptedOTP = hex_sha512(concathash);
                        $('#OTPID').val(doubleEncryptedOTP);
                        //  alert("before ajax call");
                        $.ajax({
                            type: "POST",
                            url: "/Home/ValidateOTP",
                            headers: header,
                            data: $("#OTPValidationFormId").serialize(),
                            dataType: "json",
                            success: function (data) {
                                if (data.success) {
                                    bootbox.alert({
                                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.msg + '</span>',
                                        callback: function (result) {
                                            //alert(result);
                                            $('#divOTPValidationModal').modal('hide');
                                            ScannedFileDownload();
                                        }
                                    });
                                   
                                    //setTimeout(function () {

                                    //    bootbox.hideAll();
                                    //}, 1500);
                                    //
                                    //
                                }


                                else {
                                    if (data.msg == undefined) {
                                        bootbox.alert({
                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something went wrong while processing your request ." + '</span>',
                                            callback: function () {

                                                window.location.href = '/Error/SessionExpire';
                                                $.unblockUI();
                                            }
                                        });
                                    }
                                    else {
                                        bootbox.alert({
                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.msg + '</span>',
                                            callback: function () {
                                                $('#divOTPValidationModal').css('display', 'block');

                                                $('#OTPID').val("");
                                                $('#OTPID').focus();
                                            }
                                        });
                                    }
                                }
                            },
                            error: function (err) {
                                bootbox.alert({
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                                    callback: function () {
                                        $('#divOTPValidationModal').modal('show');

                                    }
                                });
                            }
                        });
                    }
                }
            },
            error: function (err) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while processing your request" + '</span>',
                    callback: function () {

                    }
                });
            }
        });
    }
}

function ScannedFileDownload() {

    var fileDownload = false;
    SROfficeID = $("#SROfficeListID option:selected").val();
    DROfficeID = $("#DROfficeListID option:selected").val();
    DocumentNumber = $("#txtDocumentNumber").val();
    BookTypeID = $("#BookTypeListID option:selected").val();
    MarriageTypeID = $("#MarriageTypeListID option:selected").val();
    NoticeTypeListID = $("#NoticeTypeListID option:selected").val();
    FinancialYear = $("#FinancialYearListID option:selected").val();
    //Commented and Added By tushar on 23 march 2022 for Download reason dropDown selection
    //DownloadReason = $("#DownloadReasonId").val();
    //DownloadReason = $("#DownloadReasonId option:selected").text();
    DownloadReasonID = $("#DownloadReasonId option:selected").val();
    DocumentTypeID = $("#DocumentTypeId option:selected").val();
    //End By Tushar on 23 March 2022
    DType = $("input[name='DType']:checked").val();
    //var regexToMatch = /^[^<>]+$/;
    var regexToMatchDownloadReason = /^[a-zA-Z0-9-/., ]+$/;
    var regexToMatchDocumentNumber = new RegExp('^[0-9]*$');

    var validateParameter = ValidateParameters();


    //Added By Tushar on 24 March 2022 for validation
    if (validateParameter) {
  
                    blockUI('loading data.. please wait...');

        $.ajax({
            url: '/Utilities/ScannedFileDownload/DownloadScannedFile',
            data: {
                "SROfficeID": SROfficeID, "DROfficeID": DROfficeID, "DocumentNumber": DocumentNumber, "BookTypeID": BookTypeID, "FinancialYearStr": FinancialYear, "DType": DType, "ReasonID": DownloadReasonID, "DocumentTypeID": DocumentTypeID, "MarriageTypeID": MarriageTypeID, "NoticeTypeListID": NoticeTypeListID
            },
            type: "GET",
            success: function (data) {
                unBlockUI();

                if (data.success == false) {
                    $.unblockUI();
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                        function () {
                        });
                    return; LoadDT();
                }
                else {
                    if (data.TiffConversionError) {
                        GetCorrectedPDF(data.FinalRegistrationNumber, data.CompromisedTiffFilePath, data.ReferenceString);
                    }
                    else {
                        saveByteArray(data.FileName, base64ToArrayBuffer(data.FileContent));
                    }
                    //LoadDT();
                }
                $.unblockUI();
            },
            error: function (xhr) {
                unBlockUI();
                alert("Error Occured while processing...");
                unBlockUI();
            }
        });


    }

}

//End BY Tushar on 24 March 2022

//Added by mayank on 29/03/2022
function saveByteArray(reportName, byte) {
    var bytes = new Uint8Array(byte);
    //var blob = new Blob([bytes], { type: "application/pdf" });
    var blob = new Blob([bytes], { type:"" });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    var fileName = reportName;
    link.download = fileName;
    link.click();
    SaveDownloadDetails();
};

function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}

function ValidateParameters() {
    var fileDownload = false;
    SROfficeID = $("#SROfficeListID option:selected").val();
    DROfficeID = $("#DROfficeListID option:selected").val();
    DocumentNumber = $("#txtDocumentNumber").val();
    BookTypeID = $("#BookTypeListID option:selected").val();
    MarriageTypeID = $("#MarriageTypeListID option:selected").val();
    NoticeTypeListID = $("#NoticeTypeListID option:selected").val();
    FinancialYear = $("#FinancialYearListID option:selected").val();
    //Commented and Added By tushar on 23 march 2022 for Download reason dropDown selection
    //DownloadReason = $("#DownloadReasonId").val();
    //DownloadReason = $("#DownloadReasonId option:selected").text();
    DownloadReasonID = $("#DownloadReasonId option:selected").val();
    DocumentTypeID = $("#DocumentTypeId option:selected").val();
    //End By Tushar on 23 March 2022
    DType = $("input[name='DType']:checked").val();
    //var regexToMatch = /^[^<>]+$/;
    //var regexToMatchDownloadReason = /^[a-zA-Z0-9-/., ]+$/;
    var regexToMatchDocumentNumber = new RegExp('^[0-9]*$');



    //Validation
    if (DocumentTypeID == 0) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Document Type</span>'
        });
        return false;
    }

    if (SROfficeID == 0 || DROfficeID == 0) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Office Details</span>'
        });
        return false;
    }

    else if ((DocumentNumber == "" || DocumentNumber == 0) && DocumentTypeID == 1) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Document Number</span>'
        });
        return false;
    }
    else if ((DocumentNumber == "" || DocumentNumber == 0) && DocumentTypeID == 2) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Marriage Case No</span>'
        });
        return false;
    }
    else if ((DocumentNumber == "" || DocumentNumber == 0) && DocumentTypeID == 3) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Notice Number</span>'
        });
        return false;
    }

    else if (!regexToMatchDocumentNumber.test(DocumentNumber) && DocumentTypeID == 1) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Document Number</span>'
        });
        return false;
    }
    else if (!regexToMatchDocumentNumber.test(DocumentNumber) && DocumentTypeID == 2) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Marriage Case No</span>'
        });
        return false;
    }
    else if (!regexToMatchDocumentNumber.test(DocumentNumber) && DocumentTypeID == 3) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Notice Number</span>'
        });
        return false;
    }

    else if (BookTypeID == 0 && DocumentTypeID == 1) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Book Type</span>'
        });
        return false;
    }
    else if (MarriageTypeID == 0 && DocumentTypeID == 2) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Marriage Type</span>'
        });
        return false;
    } else if (NoticeTypeListID == 0 && DocumentTypeID == 3) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Notice Type</span>'
        });
        return false;
    }

    else if (FinancialYear == "" || FinancialYear == "Select") {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Financial Year</span>'
        });
        return false;
    }

    else if (DownloadReasonID == 0) {
        //Added By tushar on 23 March 2022
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please Select Download Reason" + '</span>',
            function () {
            });
        return false;
    }

    //else if (!regexToMatchDownloadReason.test(DownloadReason)) {
    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please enter valid Download Reason" + '</span>',
    //        function () {
    //        });

    //}
    else if (DType == null) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select Download type" + '</span>',
            function () {
            });
        return false;
    }
    else if (DocumentTypeID == 0 || DocumentTypeID == null || DocumentTypeID == undefined) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select Document type" + '</span>',
            function () {
            });
        return false;
    }
    else {
        return true;
    }
}

function GetCorrectedPDF(FinalRegistrationNumber, CompromisedTiffFilePath, ReferenceString) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $('#FinalRegistrationNumber').val(FinalRegistrationNumber);
    $('#ReferenceString').val(ReferenceString);
    //$('#CompromisedTiffFilePath').val(CompromisedTiffFilePath);
    var xhr = new XMLHttpRequest();
    xhr.open('GET', CompromisedTiffFilePath);
    xhr.responseType = 'arraybuffer';
    xhr.onload = imgLoaded;
    xhr.send();
}

function loadOne(e) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    try {
        UTIF.decodeImage(resp, pages[p]);
    } catch (error) {
        if (error == "JpegError: JPEG error: SOI not found") {
            ConvertToPDFUsingImageMagick();
            return;
        }
    }
    var rgba = UTIF.toRGBA8(pages[p]);
    var canvas = document.createElement('canvas');
    canvas.width = pages[p].width;
    canvas.height = pages[p].height;
    var ctx = canvas.getContext('2d');
    var imageData = ctx.createImageData(canvas.width, canvas.height);
    for (var i = 0; i < rgba.length; i++) {
        imageData.data[i] = rgba[i];
    }
    ctx.putImageData(imageData, 0, 0);
    IDs.push(canvas.toDataURL());
    var imgObj = document.createElement('img');
    imgObj.src = canvas.toDataURL('image/png');
    imgObj.setAttribute('width', canvas.width);
    imgObj.setAttribute('height', canvas.height);
    if (++p < pages.length) {
        imgObj.onload = loadOne;
    }
    if (p == pages.length) {
        ConvertToTIFF();
    }
}

function imgLoaded(e) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    resp = e.target.response;
    pages = UTIF.decode(resp);
    console.log(pages);
    p = 0;
    loadOne();
}

function ConvertToTIFF() {
    var stringArray = new Array();
    var i = 0;
    $.each(IDs, function (index, value) {
        stringArray[i] = value;
        i++;
    });    
    var postData = { BaseImageList: stringArray, FinalRegistrationNumber: $('#FinalRegistrationNumber').val() ,ReferenceString: $('#ReferenceString').val() };
    $.ajax({
        type: "POST",
        url: "/Utilities/ScannedFileDownload/GetCorrectedPDFFileName",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (data) {
            $.unblockUI();
            if (data.success == false) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                    function () {
                    });
                return;
            }   
            else {
                    saveByteArray(data.FileName, base64ToArrayBuffer(data.FileContent));
            }
        },
        traditional: true
    });
}

function ConvertToPDFUsingImageMagick() {
    var postData = { FinalRegistrationNumber: $('#FinalRegistrationNumber').val(), ReferenceString: $('#ReferenceString').val() };
    $.ajax({
        type: "POST",
        url: "/Utilities/ScannedFileDownload/ConvertToPDFUsingImageMagick",
        data: postData,
        dataType: "json",
        cache: false,
        success: function (data) {
            $.unblockUI();
            if (data.success) {
                saveByteArray(data.FileName, base64ToArrayBuffer(data.FileContent));
            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Some Error Occurred please try again later.</span>',
                    function () {
                    });
                $.unblockUI();
            }
        },
        traditional: true
    });
}

function SaveDownloadDetails() {
    var fileDownload = false;
    SROfficeID = $("#SROfficeListID option:selected").val();
    DROfficeID = $("#DROfficeListID option:selected").val();
    DocumentNumber = $("#txtDocumentNumber").val();
    BookTypeID = $("#BookTypeListID option:selected").val();
    MarriageTypeID = $("#MarriageTypeListID option:selected").val();
    NoticeTypeListID = $("#NoticeTypeListID option:selected").val();
    FinancialYear = $("#FinancialYearListID option:selected").val();
    DownloadReasonID = $("#DownloadReasonId option:selected").val();
    DocumentTypeID = $("#DocumentTypeId option:selected").val();
    //End By Tushar on 23 March 2022
    DType = $("input[name='DType']:checked").val();
    //var regexToMatch = /^[^<>]+$/;
    $.ajax({
        url: '/Utilities/ScannedFileDownload/SaveScannedFileDownloadDetails',
        data: {
            "SROfficeID": SROfficeID, "DROfficeID": DROfficeID, "DocumentNumber": DocumentNumber, "BookTypeID": BookTypeID, "FinancialYearStr": FinancialYear, "DType": DType, "ReasonID": DownloadReasonID, "DocumentTypeID": DocumentTypeID, "MarriageTypeID": MarriageTypeID, "NoticeTypeListID": NoticeTypeListID
        },
        type: "Post",
        success: function (data) {
            unBlockUI();
            LoadDT();
        },
        error: function (xhr) {
            unBlockUI();
            alert("Error Occured while processing...");
            unBlockUI();
        }
    });
}