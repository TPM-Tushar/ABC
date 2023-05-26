var token = '';
var header = {};
isValid = false;
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $("#idApproveOrderData").hide();
    $("#idRejectOrderData").hide();


    $('#divDROrderDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        startDate: new Date('01/01/2003'),
        maxDate: new Date(),
        minDate: new Date('01/01/2003'),
        pickerPosition: "bottom-left"
    });

    $("#idChallanPurposeList").multiselect({
        includeSelectAllOption: false

    });

    $(function () {

        if ($('#idRejectionReason').val() == "") {
            radiobtn = document.getElementById("idApproveRadioBtn");
            radiobtn.checked = true;
            $('#idRejectOrderData').hide();
            $('#idApproveOrderData').show();

        }
        else {
            btnRadio = document.getElementById("idApproveRadioBtn");
            btnRadio.checked = false;

            radiobtn = document.getElementById("idRejectRadioBtn");
            radiobtn.checked = true;

            $('#idApproveOrderData').hide();
            $('#idRejectOrderData').show();
        }

    });

    $('#closeRefundChallanApproveForm').click(function () {
        $('#RefundChallanApproveToggleIcon').trigger('click');
        $('#RefundChallanApprovePanelAllTabs').hide();
    });


    if (filename != null || filename != "") {
        document.getElementById('spnOrderFileName').innerHTML = filename;
    }


    CheckRadioButtonSelection();


    $("#decOrderFile").change(function () {
        var fup = document.getElementById('decOrderFile');
        var fileName = fup.value;
        var ext = fileName.substring(fileName.lastIndexOf('.') + 1);
        if (ext == "pdf") {
            isValid = true;
        }
        else {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Select PDF file only.</span>'
            });
            fup.focus();
            isValid = false;
        }
    });


    $("#SaveRefundChallanApproveBtn").click(function () {
        
        if ($('input[name="RefundChallanOrderOption"]:checked').val() == undefined) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Approve or Reject button</span>'
            });
        }

        else if ($('input[name="RefundChallanOrderOption"]:checked').val() == "ApproveOrderSelected") {

            if ($("#idDROrderNumber").val() == '') {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter DR Order Number  </span>'
                });
            }
            else if ($("#idDROrderDate").val() == 0 || $("#idDROrderDate").val() == '') {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter DR Order Date  </span>'
                });
            }
            else if (isValid == false && $('#spnOrderFileName').text() == '')
            {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select DR Order PDF file to upload.</span>'
                });        
            }

            else if ($("#IdRejectionReasonHidden").val() != '') {
                
                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'>This Challan Refund Entry is already rejected. On 'Approve' Rejected Details will be deleted. Do you want to continue ?</span>",

                    buttons: {
                        cancel: {
                            label: '<i class="fa fa-times"></i> No',
                            className: 'pull-right margin-left-NoBtn'
                        },
                        confirm: {
                            label: '<i class="fa fa-check"></i> Yes'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            SaveRefundChallanApproveDetails();
                        }
                    }
                });
            }
            else
            {
                 SaveRefundChallanApproveDetails();
            }
            
        }

        else if ($('input[name="RefundChallanOrderOption"]:checked').val() == "RejectOrderSelected")
        {
            if ($("#idRejectionReason").val() == '') {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Rejection Reason  </span>'
                });
            }

            else if ($("#idDROrderNumberHidden").val() != '') {
                
                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'>This Challan Refund Entry is already approved. On 'Reject' Approved details with DR Order will be deleted. Do you want to continue ?</span>",

                    buttons: {
                        cancel: {
                            label: '<i class="fa fa-times"></i> No',
                            className: 'pull-right margin-left-NoBtn'
                        },
                        confirm: {
                            label: '<i class="fa fa-check"></i> Yes'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            SaveRefundChallanRejectionDetails();
                        }
                    }
                });
            }
            else
            {
                SaveRefundChallanRejectionDetails();
            }
        }
    });


    $('#btnFinalizeOrder').click(function () {

        if ($('input[name="RefundChallanOrderOption"]:checked').val() == undefined) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select approve or reject button</span>'
            });
        }

        else if ($('input[name="RefundChallanOrderOption"]:checked').val() == "ApproveOrderSelected") {

            if ($("#idDROrderNumber").val() == '') {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter DR Order Number  </span>'
                });
            }
            else if ($("#idDROrderDate").val() == 0 || $("#idDROrderDate").val() == '') {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter DR Order Date  </span>'
                });
            }

            else if (isValid == false && $('#spnOrderFileName').text() == '') {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select DR Order PDF file to upload.</span>'
                });
            }
            //Added By Shivam B on 20-07-2022 for taking confirmation message that order is already saved with rejection and finalize the order with approve rejection data will be deleted
            else if ($("#IdRejectionReasonHidden").val() != '') {

                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'>This Challan Refund Entry is already rejected. On 'Approve' Rejected Details will be deleted. Do you want to continue ?</span>",

                    buttons: {
                        cancel: {
                            label: '<i class="fa fa-times"></i> No',
                            className: 'pull-right margin-left-NoBtn'
                        },
                        confirm: {
                            label: '<i class="fa fa-check"></i> Yes'
                        }
                    },
                    callback: function (result) {
                        if (result) {

                            var bootboxConfirm = bootbox.confirm({
                                title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                                message: "<span class='boot-alert-txt'>Please verify all Details of Challan to be refunded before finalizing. After finalizing this entry will be non editable. Are you sure to finalize ?</span>",

                                buttons: {
                                    cancel: {
                                        label: '<i class="fa fa-times"></i> No',
                                        className: 'pull-right margin-left-NoBtn'
                                    },
                                    confirm: {
                                        label: '<i class="fa fa-check"></i> Yes'
                                    }
                                },
                                callback: function (result) {
                                    if (result) {
                                        FinalizeApproveDROrder();
                                    }
                                }
                            });
                        }
                    }
                });
            }
            //Added By Shivam B on 20-07-2022 for taking confirmation message that order is already saved with rejection and finalize the order with approve rejection data will be deleted

            else {
                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'>Please verify all Details of Challan to be refunded before finalizing. After finalizing this entry will be non editable. Are you sure to finalize ?</span>",

                    buttons: {
                        cancel: {
                            label: '<i class="fa fa-times"></i> No',
                            className: 'pull-right margin-left-NoBtn'
                        },
                        confirm: {
                            label: '<i class="fa fa-check"></i> Yes'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            FinalizeApproveDROrder();
                        }
                    }
                });
                
            }
        }


        else if ($('input[name="RefundChallanOrderOption"]:checked').val() == "RejectOrderSelected") {

            if ($('#idRejectionReason').val() == '') {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Rejection Reason  </span>'
                });
            }
         //Added By Shivam B on 20-07-2022 for taking confirmation message that order is already saved with Approve Details and finalize the order with rejection, the approve data will be deleted
            else if ($("#idDROrderNumberHidden").val() != '') {

                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'>This Challan Refund Entry is already approved. On 'Reject' Approved details with DR Order will be deleted. Do you want to continue ?</span>",

                    buttons: {
                        cancel: {
                            label: '<i class="fa fa-times"></i> No',
                            className: 'pull-right margin-left-NoBtn'
                        },
                        confirm: {
                            label: '<i class="fa fa-check"></i> Yes'
                        }
                    },
                    callback: function (result) {
                        if (result) {

                            var bootboxConfirm = bootbox.confirm({
                                title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                                message: "<span class='boot-alert-txt'>Please verify all Details of Challan to be refunded before finalizing. After finalizing this entry will be non editable. Are you sure to finalize ?</span>",

                                buttons: {
                                    cancel: {
                                        label: '<i class="fa fa-times"></i> No',
                                        className: 'pull-right margin-left-NoBtn'
                                    },
                                    confirm: {
                                        label: '<i class="fa fa-check"></i> Yes'
                                    }
                                },
                                callback: function (result) {
                                    if (result) {
                                        FinalizeRejectDROrder();
                                    }
                                }
                            });
                        }
                    }
                });
            }
           //Added By Shivam B on 20-07-2022 for taking confirmation message that order is already saved with Approve Details and finalize the order with rejection, the approve data will be deleted

            else {
                
                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'>Please verify all Details of Challan to be refunded before finalizing. After finalizing this entry will be non editable. Are you sure to finalize ?</span>",

                    buttons: {
                        cancel: {
                            label: '<i class="fa fa-times"></i> No',
                            className: 'pull-right margin-left-NoBtn'
                        },
                        confirm: {
                            label: '<i class="fa fa-check"></i> Yes'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            FinalizeRejectDROrder();
                        }
                    }
                });
            }
        }
    })

    
    $("#idApproveOrReject").change(function () {

        var selectedRefundChallanOrderOption = $('input[name="RefundChallanOrderOption"]:checked').val();
        
        if (selectedRefundChallanOrderOption == "ApproveOrderSelected") {
            $('#idRejectOrderData').hide();
            $('#idApproveOrderData').show();
            
        }
        else if (selectedRefundChallanOrderOption == "RejectOrderSelected") {
            $('#idApproveOrderData').hide();
            $('#idRejectOrderData').show();
        }
    });


    $('#idIsChallanNoExists').click(function () {
        IsChallanNoExists();
    })

    $('#btnDeleteCurrentOrderFile').click(function () {
        DeleteCurrentOrderFile();
    });

});


function CheckRadioButtonSelection() {
    if ($("#idDROrderNumber").val() != '') {
        ApproveRadiobtn = document.getElementById("idApproveRadioBtn");
        ApproveRadiobtn.checked = true;
    }
    else {
        ApproveRadiobtn = document.getElementById("idApproveRadioBtn");
        ApproveRadiobtn.checked = false;

        RejectRadiobtn = document.getElementById("idRejectRadioBtn");
        RejectRadiobtn.checked = true;
    }
}


function SaveRefundChallanApproveDetails() {

    var decOrderFile = $('#decOrderFile').val();
    var radiobtn = $('input[name="RefundChallanOrderOption"]:checked').val();

    //alert("In SaveOrder Details");

    //Check if input tag is there, If yes then prompt user to select file. If not then Update Order No and Order Date only
    if (($("input[type='file']")).length == 1) {

        
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
                formData.append("filesArray", filesArray);
                formData.append("RowId", $("#idRowId").val());
                formData.append("InstrumentNumber", $("#idChallanNumber").val());
                formData.append("InstrumentDate", $("#idChallanDate").val());
                formData.append("DROrderNumber", $("#idDROrderNumber").val());
                formData.append("DROrderDate", $("#idDROrderDate").val());
                formData.append("ChallanAmount", $("#idChallanAmount").val());
                formData.append("RefundAmount", $("#idRefundAmount").val());
                formData.append("PartyName", $("#idPartyName").val());
                formData.append("ApplicationDateTime", $("#idApplicationDateTime").val());
                formData.append("PartyMobileNumber", $("#idPartyMobileNumber").val());
                formData.append("IsOrderInEditMode", $("#IsOrderInEditMode").val());

                //formData.append("filesArray", filesArray);
                //formData.append("OrderNo", $("#idOrdeNo").val());
                //formData.append("OrderDate", $("#txtOrderDate").val());

                var form = $("#RefundChallanApproveForm");
                form.removeData('validator');
                form.removeData('unobtrusiveValidation');
                $.validator.unobtrusive.parse(form);

                if ($("#RefundChallanApproveForm").valid()) {

                    blockUI();
                    $.ajax({
                        type: "POST",
                        url: "/RefundChallan/RefundChallanApprove/UploadOrdersFile",
                        data: formData,
                        headers: header,
                        processData: false,
                        contentType: false,
                        success: function (data) {
                            if (data.success) {
                                $("#hdnfileContent").val(data.filePath);
                                $("#hdnRelativeFilePath").val(data.relativeFilePath);
                                $("#hdnExistingFileName").val(data.orderFileName);
                                
                                blockUI();
                                $.ajax({
                                    type: "POST",
                                    url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanApproveDetails",
                                    data: $("#RefundChallanApproveForm").serialize(),
                                    headers: header,
                                    success: function (data) {
                                        if (data.success) {
                                            unBlockUI();
                                            bootbox.alert({
                                                message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                                callback: function () {
                                                    
                                                    window.location.reload();
                                                    
                                                }
                                            });
                                            DocDetailsTable();
                                        }
                                        else {
                                            unBlockUI();
                                            bootbox.alert({
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                            });
                                        }
                                    },
                                    error: function () {
                                        unBlockUI();

                                        bootbox.alert({
                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                                        });
                                    }
                                });
                            }
                            else {
                                unBlockUI();
                                bootbox.alert({
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                });
                                $.unblockUI();
                            }
                        },
                        error: function () {
                            unBlockUI();
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                            });
                            $.unblockUI();
                        }
                    });
                }
            }
            else
            {
                //alert("In else ...");
                unBlockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select DR Order PDF file to upload.</span>'
                });

            }
        });
    }
    else {
        //Update Order No. and order date
        //alert("Update Order No. and order date only.");
        ////

            blockUI();
            $.ajax({
                type: "GET",
                url: "/RefundChallan/RefundChallanApprove/CheckifOrderNoExist",
                data: { "OrderNo": $('#idDROrderNumber').val(), "RowId": $("#idRowId").val() },
                headers: header,
                success: function (data) {
                    if (data.success) {
                        blockUI();
                        $.ajax({
                            type: "POST",
                            url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanApproveDetails",
                            data: $("#RefundChallanApproveForm").serialize(),
                            //data: orderData,
                            headers: header,
                            //  processData: false,
                            // contentType: false,
                            success: function (data) {
                                if (data.success) {
                                    unBlockUI();
                                    bootbox.alert({
                                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                        callback: function () {

                                            window.location.reload();
                                        }
                                    });
                                    //window.location.reload();

                                }
                                else {
                                    unBlockUI();
                                    bootbox.alert({
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                    });
                                }
                            },
                            error: function () {
                                unBlockUI();

                                bootbox.alert({
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                                });
                            }
                        });
                    }
                    else {
                        unBlockUI();
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                        });
                    }
                },
                error: function () {
                    unBlockUI();

                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                    });
                }
            });
        
    }
    
}


function SaveRefundChallanRejectionDetails() {

    blockUI();
    $.ajax({
        type: "POST",
        url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanRejectionDetails",
        data: { "RejectionReason": $("#idRejectionReason").val(), "RowId": $("#idRowId").val(), "InstrumentNumber": $("#idChallanNumber").val() },
        headers: header,
        success: function (data) {
            unBlockUI();
            if (data.success) {
                bootbox.alert({
                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                    callback: function () {

                        window.location.reload();
                    }
                });
                DocDetailsTable();

            }
            else {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                });
            }
        },
        error: function () {
            unBlockUI();
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
        }
    });

}


function IsChallanNoExists() {
    blockUI();
    $.ajax({
        type: "POST",
        url: "/RefundChallan/RefundChallanApprove/IsChallanNoExists",
        data: { 'Instrumentnumber': $("#idChallanNumber").val(), 'InstrumentDate': $("#idChallanDate").val(), 'RowId': $("#idRowId").val(), },
        headers: header,
        success: function (data) {
            unBlockUI();
            if (data.success) {

                bootbox.alert({
                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                });
                
            }
            else {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                });
            }
        },
        error: function () {
            unBlockUI();
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
        }
    });

}


function FinalizeApproveDROrder() {

    if (($("input[type='file']")).length == 1) {


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
                formData.append("filesArray", filesArray);
                formData.append("RowId", $("#idRowId").val());
                formData.append("InstrumentNumber", $("#idChallanNumber").val());
                formData.append("InstrumentDate", $("#idChallanDate").val());
                formData.append("DROrderNumber", $("#idDROrderNumber").val());
                formData.append("DROrderDate", $("#idDROrderDate").val());
                formData.append("ChallanAmount", $("#idChallanAmount").val());
                formData.append("RefundAmount", $("#idRefundAmount").val());
                formData.append("PartyName", $("#idPartyName").val());
                formData.append("ApplicationDateTime", $("#idApplicationDateTime").val());
                formData.append("PartyMobileNumber", $("#idPartyMobileNumber").val());
                formData.append("IsOrderInEditMode", $("#IsOrderInEditMode").val());

                //formData.append("filesArray", filesArray);
                //formData.append("OrderNo", $("#idOrdeNo").val());
                //formData.append("OrderDate", $("#txtOrderDate").val());

                var form = $("#RefundChallanApproveForm");
                form.removeData('validator');
                form.removeData('unobtrusiveValidation');
                $.validator.unobtrusive.parse(form);

                if ($("#RefundChallanApproveForm").valid()) {

                    blockUI();
                    $.ajax({
                        type: "GET",
                        url: "/RefundChallan/RefundChallanApprove/CheckifOrderNoExist",
                        data: { "OrderNo": $("#idDROrderNumber").val(), "RowId": $("#idRowId").val() },
                        headers: header,
                        success: function (data) {
                            if (data.success) {

                                blockUI();
                                $.ajax({
                                    type: "POST",
                                    url: "/RefundChallan/RefundChallanApprove/UploadOrdersFile",
                                    data: formData,
                                    headers: header,
                                    processData: false,
                                    contentType: false,
                                    success: function (data) {

                                        if (data.success) {

                                            $("#hdnfileContent").val(data.filePath);
                                            $("#hdnRelativeFilePath").val(data.relativeFilePath);
                                            $("#hdnExistingFileName").val(data.orderFileName);

                                            blockUI();
                                            $.ajax({
                                                type: "POST",
                                                url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanApproveDetails",
                                                data: $("#RefundChallanApproveForm").serialize(),
                                                headers: header,
                                                success: function (data) {

                                                    if (data.success) {

                                                        blockUI('Finalizing DR Order... please wait...');
                                                        $.ajax({
                                                            url: '/RefundChallan/RefundChallanApprove/FinalizeApproveDROrder',
                                                            headers: header,
                                                            datatype: "json",
                                                            data: { "RowId": $('#idRowId').val() },
                                                            type: "POST",
                                                            success: function (data) {
                                                                if (data.success) {
                                                                    unBlockUI();
                                                                    bootbox.alert({
                                                                        message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

                                                                        callback: function () {
                                                                            window.location.href = '/RefundChallan/RefundChallanApprove/RefundChallanApproveView';
                                                                        }
                                                                    });
                                                                }
                                                                else {
                                                                    unBlockUI();
                                                                    bootbox.alert({
                                                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                                                    });
                                                                }
                                                            },
                                                            error: function (xhr) {
                                                                unBlockUI();
                                                                bootbox.alert({
                                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                                                                });
                                                            }
                                                        });

                                                    }
                                                    else {
                                                        unBlockUI();
                                                        bootbox.alert({
                                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                                        });
                                                    }
                                                },
                                                error: function () {
                                                    unBlockUI();
                                                    bootbox.alert({
                                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                                                    });
                                                }
                                            });


                                        }
                                        else {
                                            unBlockUI();
                                            bootbox.alert({
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                            });
                                        }
                                    },
                                    error: function () {
                                        $.unblockUI();
                                        bootbox.alert({
                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                                        });

                                    }
                                });
                            }
                            else {
                                unBlockUI();
                                bootbox.alert({
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                });
                            }
                        },
                        error: function () {
                            $.unblockUI();
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                            });
                        }
                    });
                }

            }
        });
    }
    else {

        blockUI();
        $.ajax({
            type: "GET",
            url: "/RefundChallan/RefundChallanApprove/CheckifOrderNoExist",
            data: { "OrderNo": $("#idDROrderNumber").val(), "RowId": $("#idRowId").val() },
            headers: header,
            success: function (data) {
                if (data.success) {
                    blockUI();
                    $.ajax({
                        type: "POST",
                        url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanApproveDetails",
                        data: $("#RefundChallanApproveForm").serialize(),
                        headers: header,
                        success: function (data) {
                            if (data.success) {

                                blockUI('Finalizing DR Order... please wait...');
                                $.ajax({
                                    url: '/RefundChallan/RefundChallanApprove/FinalizeApproveDROrder',
                                    headers: header,
                                    datatype: "json",
                                    data: { "RowId": $('#idRowId').val() },
                                    type: "POST",
                                    success: function (data) {
                                        unBlockUI();
                                        if (data.success) {

                                            bootbox.alert({
                                                message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

                                                callback: function () {
                                                    window.location.href = '/RefundChallan/RefundChallanApprove/RefundChallanApproveView';
                                                }
                                            });
                                        }
                                        else {
                                            unBlockUI();
                                            bootbox.alert({
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                            });
                                        }
                                    },
                                    error: function (xhr) {
                                        unBlockUI();
                                        bootbox.alert({
                                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                                        });
                                    }
                                });

                            }
                            else {
                                unBlockUI();
                                bootbox.alert({
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                });
                            }
                        },
                        error: function () {
                            unBlockUI();
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                            });
                        }
                    });  
                }
                else {
                    unBlockUI();
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                    });
                }
            },
            error: function () {
                unBlockUI();
                $.unblockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                });
            }
        });
    }
           
}


function FinalizeRejectDROrder() {

    blockUI();
    $.ajax({
        type: "POST",
        url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanRejectionDetails",
        data: { 'RowId': $("#idRowId").val(), 'RejectionReason': $('#idRejectionReason').val(), "InstrumentNumber": $("#idChallanNumber").val()},
        headers: header,
        success: function (data) {
            if (data.success) {
                blockUI('Finalizing DR Order... please wait...');
                $.ajax({
                    url: '/RefundChallan/RefundChallanApprove/FinalizeRejectDROrder',
                    headers: header,
                    datatype: "json",
                    data: { "RowId": $('#idRowId').val()},
                    type: "POST",
                    success: function (data) {
                        if (data.success) {
                            unBlockUI();
                            bootbox.alert({
                                message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

                                callback: function () {
                                    window.location.href = '/RefundChallan/RefundChallanApprove/RefundChallanApproveView';
                                }
                            });
                        }
                        else {
                            unBlockUI();
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                            });
                        }
                    },
                    error: function (xhr) {
                        unBlockUI();
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                        });
                    }
                });
                
            }
            else {
                unBlockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                });
            }
        },
        error: function () {
            unBlockUI();
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
        }
    });

}


//Added on 27/05/2022 for delete DR order pdf on click of delete button in Refund Challan by ShivamB
function DeleteCurrentOrderFile() {
    //alert("In Del");

    blockUI('Deleting DR Order File... please wait...');

    $.ajax({
        url: '/RefundChallan/RefundChallanApprove/DeleteCurrentOrderFile',
        data: { 'RowId': $("#idRowId").val() },
        headers: header,
        type: "POST",
        success: function (data) {
            if (data.success)
            {
                unBlockUI();
                bootbox.alert({
                    message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                });

                if (data.RowId != 0) {
                    RefundChallanApproveTable()
                    RefundChallanApproveAddEditOrder(data.RowId)
                    $('#spnOrderFileName').val('');
                    $('#RefundChallanDROrderDetailsCollapse').trigger('click');
                }
            }
            else {
                unBlockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                });
            }
        },
        error: function (xhr) {
            unBlockUI();
            //alert(xhr);
        }
    });
}

//Added on 27/05/2022 for delete DR order pdf on click of delete button in Refund Challan by ShivamB




//Commented By Shivam B Because of Delete DR Order Pdf Functionality on 27/05/2022 

//function SaveRefundChallanApproveDetails() {

//    var decOrderFile = $('#decOrderFile').val();
//    var radiobtn = $('input[name="RefundChallanOrderOption"]:checked').val();

//    if (decOrderFile == '') {

//        if (document.getElementById('spnOrderFileName').innerHTML == '') {
//            bootbox.alert({
//                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select PDF File</span>'
//            });
//        }
//        else {
//            blockUI();
//            $.ajax({
//                type: "GET",
//                url: "/RefundChallan/RefundChallanApprove/CheckifOrderNoExist",
//                data: { "OrderNo": $('#idDROrderNumber').val(), "RowId": $("#idRowId").val() },
//                headers: header,
//                success: function (data) {
//                    if (data.success) {

//                        $("#hdnfileContent").val(data.filePath);
//                        $("#hdnRelativeFilePath").val(data.relativeFilePath);
//                        $("#hdnExistingFileName").val(data.orderFileName);

//                        var formData = new FormData();
//                        formData.append("RowId", $("#idRowId").val());
//                        formData.append("InstrumentNumber", $("#idChallanNumber").val());
//                        formData.append("InstrumentDate", $("#idChallanDate").val());
//                        formData.append("DROrderNumber", $("#idDROrderNumber").val());
//                        formData.append("DROrderDate", $("#idDROrderDate").val());
//                        formData.append("ChallanAmount", $("#idChallanAmount").val());
//                        formData.append("RefundAmount", $("#idRefundAmount").val());
//                        formData.append("PartyName", $("#idPartyName").val());
//                        formData.append("ApplicationDateTime", $("#idApplicationDateTime").val());
//                        formData.append("PartyMobileNumber", $("#idPartyMobileNumber").val());
//                        formData.append("IsOrderInEditMode", $("#IsOrderInEditMode").val());
//                        formData.append("IsApproveRadioBtn", radiobtn);

//                        var form = $("#RefundChallanApproveForm");
//                        form.removeData('validator');
//                        form.removeData('unobtrusiveValidation');
//                        $.validator.unobtrusive.parse(form);

//                        blockUI();
//                        $.ajax({
//                            type: "POST",
//                            url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanApproveDetails",
//                            data: $("#RefundChallanApproveForm").serialize(),
//                            headers: header,
//                            success: function (data) {
//                                unBlockUI();
//                                if (data.success) {
//                                    bootbox.alert({
//                                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
//                                        callback: function () {

//                                            var TableName = "SelectDocument";
//                                            window.location.reload();
//                                        }
//                                    });
//                                }
//                                else {
//                                    unBlockUI();
//                                    bootbox.alert({
//                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                                    });
//                                }
//                            },
//                            error: function () {
//                                unBlockUI();
//                                bootbox.alert({
//                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                                });
//                            }
//                        });
//                    }
//                    else {
//                        unBlockUI();
//                        bootbox.alert({
//                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                        });
//                    }
//                },
//                error: function () {
//                    $.unblockUI();
//                    bootbox.alert({
//                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                    });
//                }
//            });
//        }


//    }
//    else {

//        if (($("input[type='file']")).length == 1) {
//            var formData = new FormData();
//            var filesArray = null;

//            $.each($("input[type='file']"), function () {

//                var id = $(this).attr('id');
//                var fileData = document.getElementById(id).files[0];

//                if (fileData != undefined) {
//                    formData.append(id, fileData);
//                    if (filesArray == null) {
//                        filesArray = id + ",";
//                    }
//                    else {
//                        filesArray += id + ",";
//                    }
//                    formData.append("RowId", $("#idRowId").val());
//                    formData.append("InstrumentNumber", $("#idChallanNumber").val());
//                    formData.append("InstrumentDate", $("#idChallanDate").val());
//                    formData.append("DROrderNumber", $('#idDROrderNumber').val());
//                    formData.append("DROrderDate", $("#idDROrderDate").val());
//                    formData.append("ChallanAmount", $("#idChallanAmount").val());
//                    formData.append("RefundAmount", $("#idRefundAmount").val());
//                    formData.append("PartyName", $("#idPartyName").val());
//                    formData.append("ApplicationDateTime", $("#idApplicationDateTime").val());
//                    formData.append("PartyMobileNumber", $("#idPartyMobileNumber").val());
//                    formData.append("IsOrderInEditMode", $("#IsOrderInEditMode").val());
//                    formData.append("filesArray", filesArray);


//                    var form = $("#RefundChallanApproveForm");
//                    form.removeData('validator');
//                    form.removeData('unobtrusiveValidation');
//                    $.validator.unobtrusive.parse(form);

//                    if ($("#RefundChallanApproveForm").valid()) {

//                        blockUI();
//                        $.ajax({
//                            type: "POST",
//                            url: "/RefundChallan/RefundChallanApprove/UploadOrdersFile",
//                            data: formData,
//                            headers: header,
//                            processData: false,
//                            contentType: false,
//                            success: function (data) {
//                                if (data.success) {

//                                    $("#hdnfileContent").val(data.filePath);

//                                    $("#hdnRelativeFilePath").val(data.relativeFilePath);
//                                    $("#hdnExistingFileName").val(data.orderFileName);


//                                    //Uncomment from here
//                                    blockUI();

//                                    $.ajax({
//                                        type: "POST",
//                                        url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanApproveDetails",
//                                        data: $("#RefundChallanApproveForm").serialize(),
//                                        headers: header,
//                                        success: function (data) {
//                                            unBlockUI();
//                                            if (data.success) {

//                                                bootbox.alert({
//                                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
//                                                    callback: function () {
//                                                        var TableName = "SelectDocument";
//                                                        window.location.reload();
//                                                    }
//                                                });

//                                            }
//                                            else {
//                                                bootbox.alert({
//                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                                                });
//                                            }
//                                        },
//                                        error: function () {
//                                            unBlockUI();

//                                            bootbox.alert({
//                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                                            });
//                                        }
//                                    });
//                                }
//                                else {
//                                    unBlockUI();
//                                    bootbox.alert({
//                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                                    });
//                                    $.unblockUI();
//                                }
//                            },
//                            error: function () {
//                                bootbox.alert({
//                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                                });
//                                $.unblockUI();
//                            }
//                        });
//                    }
//                }
//                else {
//                    bootbox.alert({
//                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select DR Order PDF file to upload.</span>'
//                    });
//                }
//            });
//        }
//        else {
//            blockUI();
//            $.ajax({
//                type: "POST",
//                url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanApproveDetails",
//                data: $("#RefundChallanApproveForm").serialize(),
//                headers: header,
//                success: function (data) {
//                    unBlockUI();
//                    if (data.success) {

//                        bootbox.alert({
//                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
//                            callback: function () {

//                                window.location.reload();
//                            }
//                        });
//                        DocDetailsTable();

//                    }
//                    else {
//                        bootbox.alert({
//                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                        });
//                    }
//                },
//                error: function () {
//                    unBlockUI();

//                    bootbox.alert({
//                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                    });
//                }
//            });
//        }
//    }

//}

//function SaveRefundChallanRejectionDetails() {

//    blockUI();
//    $.ajax({
//        type: "POST",
//        url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanRejectionDetails",
//        data: { "RejectionReason": $("#idRejectionReason").val(), "RowId": $("#idRowId").val(), "InstrumentNumber": $("#idChallanNumber").val() },
//        headers: header,
//        success: function (data) {
//            unBlockUI();
//            if (data.success) {
//                bootbox.alert({
//                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
//                    callback: function () {

//                        window.location.reload();
//                    }
//                });
//                DocDetailsTable();

//            }
//            else {
//                bootbox.alert({
//                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                });
//            }
//        },
//        error: function () {
//            unBlockUI();
//            bootbox.alert({
//                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//            });
//        }
//    });

//}

//function FinalizeApproveDROrder() {

//    var decOrderFile = $('#decOrderFile').val();


//    if (decOrderFile == '') {

//        if ($('#spnOrderFileName').val == '') {
//            bootbox.alert({
//                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select PDF File </span>'
//            });
//        }
//        else {

//            blockUI();
//            $.ajax({
//                type: "GET",
//                url: "/RefundChallan/RefundChallanApprove/CheckifOrderNoExist",
//                data: { "OrderNo": $("#idDROrderNumber").val(), "RowId": $("#idRowId").val() },
//                headers: header,
//                success: function (data) {
//                    if (data.success) {

//                        $("#hdnfileContent").val(data.filePath);

//                        $("#hdnRelativeFilePath").val(data.relativeFilePath);
//                        $("#hdnNewOrderID").val(data.orderID);
//                        $("#hdnExistingFileName").val(data.orderFileName);


//                        var formData = new FormData();
//                        formData.append("RowId", $("#idRowId").val());
//                        formData.append("InstrumentNumber", $("#idChallanNumber").val());
//                        formData.append("InstrumentDate", $("#idChallanDate").val());
//                        formData.append("DROrderNumber", $("#idDROrderNumber").val());
//                        formData.append("DROrderDate", $("#idDROrderDate").val());
//                        formData.append("ChallanAmount", $("#idChallanAmount").val());
//                        formData.append("RefundAmount", $("#idRefundAmount").val());
//                        formData.append("PartyName", $("#idPartyName").val());
//                        formData.append("ApplicationDateTime", $("#idApplicationDateTime").val());
//                        formData.append("PartyMobileNumber", $("#idPartyMobileNumber").val());
//                        formData.append("IsOrderInEditMode", $("#IsOrderInEditMode").val());

//                        var form = $("#RefundChallanApproveForm");
//                        form.removeData('validator');
//                        form.removeData('unobtrusiveValidation');
//                        $.validator.unobtrusive.parse(form);

//                        blockUI();
//                        $.ajax({
//                            type: "POST",
//                            url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanApproveDetails",
//                            data: $("#RefundChallanApproveForm").serialize(),
//                            headers: header,
//                            success: function (data) {
//                                if (data.success) {
//                                    blockUI('Finalizing DR Order... please wait...');
//                                    $.ajax({
//                                        url: '/RefundChallan/RefundChallanApprove/FinalizeApproveDROrder',
//                                        headers: header,
//                                        datatype: "json",
//                                        data: { "RowId": $('#idRowId').val() },
//                                        type: "POST",
//                                        success: function (data) {
//                                            unBlockUI();
//                                            if (data.success) {

//                                                bootbox.alert({
//                                                    message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

//                                                    callback: function () {
//                                                        window.location.href = '/RefundChallan/RefundChallanApprove/RefundChallanApproveView';
//                                                    }
//                                                });
//                                            }
//                                            else {
//                                                bootbox.alert({
//                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                                                });
//                                            }
//                                        },
//                                        error: function (xhr) {
//                                            unBlockUI();
//                                        }
//                                    });

//                                }
//                                else {
//                                    unBlockUI();
//                                    bootbox.alert({
//                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                                    });
//                                }
//                            },
//                            error: function () {
//                                unBlockUI();
//                                bootbox.alert({
//                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                                });
//                            }
//                        });
//                    }
//                    else {
//                        unBlockUI();
//                        bootbox.alert({
//                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                        });
//                    }
//                },
//                error: function () {
//                    $.unblockUI();
//                    bootbox.alert({
//                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                    });

//                }
//            });
//        }

//    }
//    else {
//        if (($("input[type='file']")).length == 1) {


//            var formData = new FormData();
//            var filesArray = null;


//            $.each($("input[type='file']"), function () {

//                var id = $(this).attr('id');
//                var fileData = document.getElementById(id).files[0];


//                if (fileData != undefined) {
//                    formData.append(id, fileData);
//                    if (filesArray == null) {
//                        filesArray = id + ",";
//                    }
//                    else {
//                        filesArray += id + ",";
//                    }
//                    formData.append("RowId", $("#idRowId").val());
//                    formData.append("InstrumentNumber", $("#idChallanNumber").val());
//                    formData.append("InstrumentDate", $("#idChallanDate").val());
//                    formData.append("DROrderNumber", $("#idDROrderNumber").val());
//                    formData.append("DROrderDate", $("#idDROrderDate").val());
//                    formData.append("ChallanAmount", $("#idChallanAmount").val());
//                    formData.append("RefundAmount", $("#idRefundAmount").val());
//                    formData.append("PartyName", $("#idPartyName").val());
//                    formData.append("ApplicationDateTime", $("#idApplicationDateTime").val());
//                    formData.append("PartyMobileNumber", $("#idPartyMobileNumber").val());
//                    formData.append("IsOrderInEditMode", $("#IsOrderInEditMode").val());
//                    formData.append("filesArray", filesArray);


//                    var form = $("#RefundChallanApproveForm");
//                    form.removeData('validator');
//                    form.removeData('unobtrusiveValidation');
//                    $.validator.unobtrusive.parse(form);

//                    if ($("#RefundChallanApproveForm").valid()) {

//                        blockUI();
//                        $.ajax({
//                            type: "POST",
//                            url: "/RefundChallan/RefundChallanApprove/UploadOrdersFile",
//                            data: formData,
//                            headers: header,
//                            processData: false,
//                            contentType: false,
//                            success: function (data) {
//                                if (data.success) {

//                                    $("#hdnfileContent").val(data.filePath);

//                                    $("#hdnRelativeFilePath").val(data.relativeFilePath);
//                                    $("#hdnExistingFileName").val(data.orderFileName);

//                                    blockUI();
//                                    $.ajax({
//                                        type: "POST",
//                                        url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanApproveDetails",
//                                        data: $("#RefundChallanApproveForm").serialize(),
//                                        headers: header,
//                                        success: function (data) {
//                                            if (data.success) {
//                                                blockUI('Finalizing DR Order... please wait...');
//                                                $.ajax({
//                                                    url: '/RefundChallan/RefundChallanApprove/FinalizeApproveDROrder',
//                                                    headers: header,
//                                                    datatype: "json",
//                                                    data: { "RowId": $('#idRowId').val() },
//                                                    type: "POST",
//                                                    success: function (data) {
//                                                        unBlockUI();
//                                                        if (data.success) {

//                                                            bootbox.alert({
//                                                                message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

//                                                                callback: function () {
//                                                                    window.location.href = '/RefundChallan/RefundChallanApprove/RefundChallanApproveView';
//                                                                }
//                                                            });
//                                                        }
//                                                        else {
//                                                            unBlockUI();
//                                                            bootbox.alert({
//                                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                                                            });
//                                                        }
//                                                    },
//                                                    error: function (xhr) {
//                                                        unBlockUI();
//                                                    }
//                                                });

//                                            }
//                                            else {
//                                                unBlockUI();
//                                                bootbox.alert({
//                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                                                });
//                                            }
//                                        },
//                                        error: function () {
//                                            unBlockUI();
//                                            bootbox.alert({
//                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                                            });
//                                        }
//                                    });
//                                }
//                                else {
//                                    unBlockUI();
//                                    bootbox.alert({
//                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                                    });
//                                }
//                            },
//                            error: function () {
//                                $.unblockUI();
//                                bootbox.alert({
//                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                                });
//                            }
//                        });
//                    }
//                }
//                else {
//                    $.unblockUI();
//                    bootbox.alert({
//                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select DR Order PDF file to upload.</span>'
//                    });

//                }
//            });
//        }
//    }
//}

//function FinalizeRejectDROrder() {

//    blockUI();
//    $.ajax({
//        type: "POST",
//        url: "/RefundChallan/RefundChallanApprove/SaveRefundChallanRejectionDetails",
//        data: { 'RowId': $("#idRowId").val(), 'RejectionReason': $('#idRejectionReason').val(), "InstrumentNumber": $("#idChallanNumber").val() },
//        headers: header,
//        success: function (data) {
//            if (data.success) {
//                blockUI('Finalizing DR Order... please wait...');
//                $.ajax({
//                    url: '/RefundChallan/RefundChallanApprove/FinalizeRejectDROrder',
//                    headers: header,
//                    datatype: "json",
//                    data: { "RowId": $('#idRowId').val() },
//                    type: "POST",
//                    success: function (data) {
//                        unBlockUI();
//                        if (data.success) {

//                            bootbox.alert({
//                                message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

//                                callback: function () {
//                                    window.location.href = '/RefundChallan/RefundChallanApprove/RefundChallanApproveView';
//                                }
//                            });
//                        }
//                        else {
//                            bootbox.alert({
//                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                            });
//                        }
//                    },
//                    error: function (xhr) {
//                        unBlockUI();
//                    }
//                });

//            }
//            else {
//                unBlockUI();
//                bootbox.alert({
//                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                });
//            }
//        },
//        error: function () {
//            unBlockUI();
//            bootbox.alert({
//                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//            });
//        }
//    });

//}

//Commented By Shivam B  Because of Delete DR Order Pdf Functionality on 27/05/2022