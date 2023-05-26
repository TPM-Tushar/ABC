var token = '';
var header = {};
isValid = false;
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $('#divApplicationDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        startDate: new Date('01/01/2003'),
        maxDate: new Date(),
        minDate: new Date('01/01/2003'),
        pickerPosition: "bottom-left"
    });

    $('#divInstrumentDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        startDate: new Date('01/01/2003'),
        maxDate: new Date(),
        minDate: new Date('01/01/2003'),
        pickerPosition: "bottom-left"
    });


    if ($('#idIsOrderInEditMode').val() == 'False') {
        $('#idRefundChallanPurpose option').attr('selected', false);
    }

    $("#idRefundChallanPurpose").multiselect({
        includeSelectAllOption: true,
        enableFiltering: true,
        //numberDisplayed: 10,
        nonSelectedText: 'None Selected',
        //buttonWidth: '100%',
        enableCaseInsensitiveFiltering: true,
        maxHeight: 200


    });



    $('#closeRefundChallanEntryDetailsForm').click(function () {

        $('#RefundChallanToggleIcon').click();

        $('#RefundChallanEntryPanelAllTabs').hide();

        //var classToRemove = $('#RefundChallanToggleIcon').attr('class');
        //if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        //    $('#RefundChallanToggleIcon').trigger('click');

    });


    //$('#idRefundChallanPurpose').trigger('click');


    

    $('.multiselect').addClass('minimal');
    $('.multiselect').addClass('btn-sm');

    
    $('#UpdateRefundChallanDetailsBtn').click(function () {

        if ($("#idPartyName").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Party Name.</span>'
            });

        }
        else if ($("#idPartyMobileNumber").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Mobile Number.</span>'
            });

        }
        else if ($("#idChallanNumber").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Number.</span>'
            });

        }
        else if ($("#idReEnterChallanNumber").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Re-Enter Challan Number.</span>'
            });

        }
        else if ($('#idChallanDate').val() == '' || $("#idChallanDate").val() == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Date.</span>'
            });
        }
        else if ($('#idChallanAmount').val() == '' || $("#idChallanAmount").val() == 0 || $("#idChallanAmount").val() == '0') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Amount.</span>'
            });
        }
        else if ($('#idRefundAmount').val() == '' || $("#idRefundAmount").val() == 0 || $("#idChallanAmount").val() == '0') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Refund Amount.</span>'
            });
        }
        else if ($("#idApplicationDate").val() == 0 || $("#idApplicationDate").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Application Date.</span>'
            });
        }
        else {
            UpdateRefundChallanDetails();
        }
    });

    
    $("#SaveRefundChallanDetailsBtn").click(function () {

        if ($("#idPartyName").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Party Name.</span>'
            });

        }
        else if ($("#idPartyMobileNumber").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Mobile Number.</span>'
            });

        }
        else if ($("#idChallanNumber").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Number.</span>'
            });

        }
        else if ($("#idReEnterChallanNumber").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Re-Enter Challan Number.</span>'
            });

        }
        else if ($('#idChallanDate').val() == '' || $("#idChallanDate").val() == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Date.</span>'
            });
        }
        else if ($('#idChallanAmount').val() == '' || $("#idChallanAmount").val() == 0 || $("#idChallanAmount").val() == '0') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Amount.</span>'
            });
        }
        else if ($('#idRefundAmount').val() == '' || $("#idRefundAmount").val() == 0 || $("#idChallanAmount").val() == '0') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Refund Amount.</span>'
            });
        }
        else if ($("#idApplicationDate").val() == 0 || $("#idApplicationDate").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Application Date.</span>'
            });
        }

        else {
            SaveRefundChallanDetails();
        }
    });


    $('#FinalizeRefundChallanDetailsBtn').click(function () {

        if ($("#idPartyName").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Party Name.</span>'
            });

        }
        else if ($("#idPartyMobileNumber").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Mobile Number.</span>'
            });

        }
        else if ($("#idChallanNumber").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Number.</span>'
            });

        }
        else if ($("#idReEnterChallanNumber").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Re-Enter Challan Number.</span>'
            });

        }
        else if ($('#idChallanDate').val() == '' || $("#idChallanDate").val() == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Date.</span>'
            });
        }
        else if ($('#idChallanAmount').val() == '' || $("#idChallanAmount").val() == 0 || $("#idChallanAmount").val() == '0') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Amount.</span>'
            });
        }
        else if ($('#idRefundAmount').val() == '' || $("#idRefundAmount").val() == 0 || $("#idChallanAmount").val() == '0') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Refund Amount.</span>'
            });
        }
        else if ($("#idApplicationDate").val() == 0 || $("#idApplicationDate").val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Application Date.</span>'
            });
        }
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
                        FinalizeRefundChallanDetails();                                
                    }
                }
            });


        }

    });

});


function SaveRefundChallanDetails() {

    var formData = new FormData();

    formData.append("RowId", $("#idRowId").val());
    formData.append("InstrumentNumber", $("#idChallanNumber").val());
    formData.append("ReEnterInstrumentNumber", $("#idReEnterChallanNumber").val());
    formData.append("InstrumentDate", $("#idChallanDate").val());
    formData.append("ChallanAmount", $("#idChallanAmount").val());
    formData.append("RefundAmount", $("#idRefundAmount").val());
    formData.append("PartyName", $("#idPartyName").val());
    formData.append("ApplicationDateTime", $("#idApplicationDateTime").val());
    formData.append("RejectionReason", $("#idRejectionReason").val());
    formData.append("PartyMobileNumber", $("#idPartyMobileNumber").val());




    var form = $("#RefundChallanDetailForm");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    blockUI();
    $.ajax({
        type: "POST",
        url: "/RefundChallan/RefundChallan/SaveRefundChallanDetails",
        data: $("#RefundChallanDetailForm").serialize(),
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


function UpdateRefundChallanDetails() {

    var formData = new FormData();

    formData.append("RowId", $("#idRowId").val());
    formData.append("InstrumentNumber", $("#idChallanNumber").val());
    formData.append("ReEnterInstrumentNumber", $("#idReEnterChallanNumber").val());
    formData.append("InstrumentDate", $("#idChallanDate").val());
    formData.append("ChallanAmount", $("#idChallanAmount").val());
    formData.append("RefundAmount", $("#idRefundAmount").val());
    formData.append("PartyName", $("#idPartyName").val());
    formData.append("ApplicationDateTime", $("#idApplicationDateTime").val());
    formData.append("RejectionReason", $("#idRejectionReason").val());
    formData.append("PartyMobileNumber", $("#idPartyMobileNumber").val());




    var form = $("#RefundChallanDetailForm");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    blockUI();
    $.ajax({
        type: "POST",
        url: "/RefundChallan/RefundChallan/UpdateRefundChallanDetails",
        data: $("#RefundChallanDetailForm").serialize(),
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


function FinalizeRefundChallanDetails() {


    var formData = new FormData();

    formData.append("RowId", $("#idRowId").val());
    formData.append("InstrumentNumber", $("#idChallanNumber").val());
    formData.append("ReEnterInstrumentNumber", $("#idReEnterChallanNumber").val());
    formData.append("InstrumentDate", $("#idChallanDate").val());
    formData.append("ChallanAmount", $("#idChallanAmount").val());
    formData.append("RefundAmount", $("#idRefundAmount").val());
    formData.append("PartyName", $("#idPartyName").val());
    formData.append("ApplicationDateTime", $("#idApplicationDateTime").val());
    formData.append("RejectionReason", $("#idRejectionReason").val());
    formData.append("PartyMobileNumber", $("#idPartyMobileNumber").val());


    var form = $("#RefundChallanDetailForm");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    blockUI();
    $.ajax({
        type: "POST",
        url: "/RefundChallan/RefundChallan/UpdateRefundChallanDetails",
        data: $("#RefundChallanDetailForm").serialize(),
        headers: header,
        success: function (data) {
            if (data.success) {
                blockUI('Finalizing DR Order... please wait...');
                $.ajax({
                    url: '/RefundChallan/RefundChallan/FinalizeRefundChallanDetails',
                    headers: header,
                    datatype: "json",
                    data: { "RowId": $('#idRowId').val() },
                    type: "GET",
                    success: function (data) {
                        unBlockUI();
                        if (data.success) {

                            bootbox.alert({
                                //message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

                                callback: function () {
                                    window.location.href = '/RefundChallan/RefundChallan/RefundChallanView';
                                }
                            });
                        }
                        else {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                            });
                        }
                    },
                    error: function (xhr) {
                        unBlockUI();
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


function ChangeInstrumentNoType() {
    var x = document.getElementById("idChallanNumber");
    if (x.type == "text") {
        x.type = "password";
    }
}







