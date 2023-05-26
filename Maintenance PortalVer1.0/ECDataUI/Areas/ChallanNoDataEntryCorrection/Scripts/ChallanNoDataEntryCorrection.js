
var token = '';
var header = {};
var SelectedType;
var txtDate;
var InstrumentNumber;
var SROOfficeListID;

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    $(".input-group-addon").hide();
    //document.getElementById("idIsChallanDateSelectedCheckBox").checked = false;


    $("#gfg").datepicker("disable");


    var MinChallanMonthOfChallanNoDataEntryCorrection = $('#IdMinChallanMonthOfChallanNoDataEntryCorrection').val();
    var MinChallanYearOfChallanNoDataEntryCorrection = $('#IdMinChallanYearOfChallanNoDataEntryCorrection').val();

   // var MinChallanYearTwoDigitOfChallanNoDataEntryCorrection = MinChallanMonthOfChallanNoDataEntryCorrection.substring(4,2);
  
    
    var newDate = 01 + "/" + MinChallanMonthOfChallanNoDataEntryCorrection + "/" + MinChallanYearOfChallanNoDataEntryCorrection;


    $('input[type="checkbox"]').click(function () {
        var yes = document.getElementById("idIsChallanDateSelectedCheckBox");
        if (yes.checked == true) {
            var NewInstrumentDate = document.getElementById("idNewInstrumentDate");
            var ReEnterInstrumentDate = document.getElementById("idReEnterInstrumentDate");
            NewInstrumentDate.disabled = false;
            ReEnterInstrumentDate.disabled = false;

            $(".input-group-addon").show();

            $('#divNewInstrumentDate').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                endDate: '+0d',
                startDate: newDate,
                maxDate: new Date(),
                minDate: newDate,
                pickerPosition: "bottom-left",
            });

            $('#divReEnterInstrumentDate').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                endDate: '+0d',
                startDate: newDate,
                maxDate: new Date(),
                minDate: newDate,
                pickerPosition: "bottom-left",
            });

        }
        else {
            var NewInstrumentDate = document.getElementById("idNewInstrumentDate");
            var ReEnterInstrumentDate = document.getElementById("idReEnterInstrumentDate");
            NewInstrumentDate.disabled = true;
            ReEnterInstrumentDate.disabled = true;

            $(".input-group-addon").hide();
            $('#idNewInstrumentDate').val('');
            $('#idReEnterInstrumentDate').val('');
        }
    });


    $("#panelEditChallanNoDetails").hide();


    $('#DtlsEditChallanNoDetailsCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconEditChallanNo').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconEditChallanNo').removeClass(classToRemove).addClass(classToSet);
    });



    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    $("#SearchBtn").click(function () {

        LoadChallanDetailsTable()
        $('#IdInstrumenNoRemarkMessage').hide();


    });


    


    $("#SaveBtn").click(function () {

        //gets table
        var oTable = document.getElementById('ChallanDetailsTable');

        //gets rows of table

        var oCells = oTable.rows.item(1).cells;
        var oldChallanNo = oCells.item(4).innerHTML;
        var oldChallanDate = oCells.item(5).innerHTML;

        var ChallanMessage = "Please verify the challan number and challan date as per the format. <br/><br/>" +
            "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
            "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
            "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
            "&nbsp &nbsp &nbsp CR<span style=\"color:red;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/>";


        var NewChallanNo = $("#idNewChallanNumber").val().trim();
        var ReEnterChallanNo = $("#idReEnterChallanNumber").val().trim();
        var NewInstrumentDate = $('#idNewInstrumentDate').val().trim();
        var ReEnterInstrumentDate = $('#idReEnterInstrumentDate').val().trim();
        var Reason = $('#idReason').val().trim();



        var Month = NewInstrumentDate.substring(5, 3);
        var year = NewInstrumentDate.substring(10, 8);

        var oTable = document.getElementById('ChallanDetailsTable');
        var oCells = oTable.rows.item(1).cells;
        var idReceipt_StampPayDate = oCells.item(10).innerHTML;
        var IsPaymentDoneAtDROOffice = oCells.item(2).innerHTML;
        var SROName = oCells.item(1).innerHTML;
        var DROName = oCells.item(3).innerHTML;

        document.getElementById('idReceipt_StampPayDate').value = idReceipt_StampPayDate;
        

        if (NewChallanNo == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter new Challan Number.</span>'
            });
        }
        else if (ReEnterChallanNo == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Re-Enter Challan Number.</span>'
            });
        }
        else if (NewChallanNo != ReEnterChallanNo) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">New Challan Number and Re-Enter challan Number must be same.</span>'
            });
        }
        else if (Reason == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Reason.</span>'
            });
        }
        else if (Reason.length < 25) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Reason should contain at least 25 characters.</span>'
            });
        }
        else if (Reason.length > 250) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Reason should not contain more than 250 characters.</span>'
            });
        }
        else if (NewChallanNo.length < 18 || NewChallanNo.substring(2, 0) != "CR") {
            if (NewChallanNo.substring(2, 0) != "IG") {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + ChallanMessage + '</span>'
                });
            }
            else {


                if (IsPaymentDoneAtDROOffice == "No")
                {
                    var bootboxConfirm = bootbox.confirm({
                        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                        message: "<span class='boot-alert-txt'>Challan No " + oldChallanNo + " will be updated as " + NewChallanNo + " in SRO " + SROName + "<br/> Please confirm the correction request, After confirmation the details are non editable <br/> Do you want to save it ? </span>",


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
                                SaveChallanDetails();
                            }
                        }
                    });
                }
                else
                {
                    var bootboxConfirm = bootbox.confirm({
                        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                        message: "<span class='boot-alert-txt'> Challan No " + oldChallanNo + " will be updated as " + NewChallanNo + " in DRO " + DROName + "br/> Please confirm the correction request, After confirmation the details are non editable <br/> Do you want to save it ? </span>",


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
                                SaveChallanDetails();
                            }
                        }
                    });
                }
                   
            }
        }
        else
        {
            if (IsPaymentDoneAtDROOffice == "No")
            {
                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'>Challan No " + oldChallanNo + " will be updated as " + NewChallanNo + " in SRO " + SROName + "<br/> Please confirm the correction request, After confirmation the details are non editable <br/> Do you want to save it ? </span>",


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
                            SaveChallanDetails();
                        }
                    }
                });
            }
            else
            {
                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    message: "<span class='boot-alert-txt'> Challan No " + oldChallanNo + " will be updated as " + NewChallanNo + " in DRO " + DROName + "br/> Please confirm the correction request, After confirmation the details are non editable <br/> Do you want to save it ? </span>",


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
                            SaveChallanDetails();
                        }
                    }
                });
            }

            

        }
    });


});








function LoadChallanDetailsTable() {
    if ($.fn.DataTable.isDataTable("#ChallanDetailsTable")) {
        $("#ChallanDetailsTable").DataTable().clear().destroy();
    }


    InstrumentNumber = $("#txtInstrumentNo").val().trim();
    SROOfficeListID = $("#SROOfficeListID").val();


    if (InstrumentNumber == "") {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Challan Number.</span>');
        return;
    }

    var ChallanDetailsRptTable = $('#ChallanDetailsTable').DataTable({
        ajax: {
            url: '/ChallanNoDataEntryCorrection/ChallanNoDataEntryCorrection/GetChallanReportDetails',
            type: "POST",
            headers: header,
            data: {
                'InstrumentNumber': InstrumentNumber, 'SROOfficeListID': $("#SROOfficeListID").val()
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError == true) {
                                window.location.href = "/Home/HomePage"
                            }
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#ChallanDetailsTable").DataTable().clear().destroy();
                                $("#panelEditChallanNoDetails").hide();
                            }
                        }
                    });
                }
                else {

                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                    $("#panelEditChallanNoDetails").hide();


                    if (json.data.length > 0) {
                        var classToRemove = $('#DtlsToggleIconEditChallanNo').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                            $('#DtlsEditChallanNoDetailsCollapse').trigger('click');
                        $("#panelEditChallanNoDetails").show();

                        $('#idOldChallanDate').val(json.data[0].ChallanDate);

                        if (json.UpdateBtn == null) {
                            

                            var SaveBtn = document.getElementById("SaveBtn");
                            SaveBtn.disabled = false;
                            $('#SaveBtn').show();

                            var NewChallanNumber = document.getElementById("idNewChallanNumber");
                            var ReEnterChallanNumber = document.getElementById("idReEnterChallanNumber");
                            var IsChallanDateSelectedCheckBox = document.getElementById("idIsChallanDateSelectedCheckBox");
                            var NewInstrumentDate = document.getElementById("idNewInstrumentDate");
                            var ReEnterInstrumentDate = document.getElementById("idReEnterInstrumentDate");
                            var Reason = document.getElementById("idReason");
                            NewChallanNumber.disabled = false;
                            ReEnterChallanNumber.disabled = false;
                            IsChallanDateSelectedCheckBox.disabled = false;
                            
                            Reason.disabled = false;


                            $('#idNewChallanNumber').val('');
                            ($('#idReEnterChallanNumber').val(''))
                            $('#idReason').val('');


                            var yes = document.getElementById("idIsChallanDateSelectedCheckBox");
                            if (yes.checked == false) {
                                $('#idNewInstrumentDate').val('');
                                $('#idReEnterInstrumentDate').val('');
                                $(".input-group-addon").hide();
                                NewInstrumentDate.disabled = true;
                                ReEnterInstrumentDate.disabled = true;
                            }
                            else
                            {
                                NewInstrumentDate.disabled = false;
                                ReEnterInstrumentDate.disabled = false;
                            }


                        }

                        else if (json.UpdateBtn == "<button class='btn btn-success' disabled data-toggle='tooltip'  title='Update'>Update</button>") {
                            var SaveBtn = document.getElementById("SaveBtn");
                            SaveBtn.disabled = true;
                            $('#SaveBtn').hide();

                            $('#idNewChallanNumber').val(json.data[0].NewInstrumentNumber);
                            $('#idReEnterChallanNumber').val(json.data[0].ReEnterInstrumentNumber);
                            $('#idNewInstrumentDate').val(json.data[0].NewInstrumentDate);
                            $('#idReEnterInstrumentDate').val(json.data[0].ReEnterInstrumentDate);
                            $('#idReason').val(json.data[0].Reason);


                            var NewChallanNumber = document.getElementById("idNewChallanNumber");
                            var ReEnterChallanNumber = document.getElementById("idReEnterChallanNumber");
                            var IsChallanDateSelectedCheckBox = document.getElementById("idIsChallanDateSelectedCheckBox");
                            var NewInstrumentDate = document.getElementById("idNewInstrumentDate");
                            var ReEnterInstrumentDate = document.getElementById("idReEnterInstrumentDate");
                            var Reason = document.getElementById("idReason");
                            NewChallanNumber.disabled = true;
                            ReEnterChallanNumber.disabled = true;
                            Reason.disabled = true;
                            IsChallanDateSelectedCheckBox.disabled = true;
                            NewInstrumentDate.disabled = true;
                            ReEnterInstrumentDate.disabled = true;

                            
                            $(".input-group-addon").hide();



                        }
                        else
                        {
                            var SaveBtn = document.getElementById("SaveBtn");
                            SaveBtn.disabled = true;
                            $('#SaveBtn').hide();
                            $('#idNewChallanNumber').val(json.data[0].NewInstrumentNumber);
                            $('#idReEnterChallanNumber').val(json.data[0].ReEnterInstrumentNumber);
                            $('#idNewInstrumentDate').val(json.data[0].NewInstrumentDate);
                            $('#idReEnterInstrumentDate').val(json.data[0].ReEnterInstrumentDate);
                            $('#idReason').val(json.data[0].Reason);
                            $('#idRemarkMessage').val(json.RemarkMessage);

                            $(".input-group-addon").hide();
                            var NewChallanNumber = document.getElementById("idNewChallanNumber");
                            var ReEnterChallanNumber = document.getElementById("idReEnterChallanNumber");
                            var IsChallanDateSelectedCheckBox = document.getElementById("idIsChallanDateSelectedCheckBox");
                            var NewInstrumentDate = document.getElementById("idNewInstrumentDate");
                            var ReEnterInstrumentDate = document.getElementById("idReEnterInstrumentDate");
                            var Reason = document.getElementById("idReason");
                            NewChallanNumber.disabled = true;
                            ReEnterChallanNumber.disabled = true;
                            IsChallanDateSelectedCheckBox.disabled = true;
                            NewInstrumentDate.disabled = true;
                            ReEnterInstrumentDate.disabled = true;
                            Reason.disabled = true;




                        }

                    }

                }
                unBlockUI();
                return json.data;
            },
            error: function () {
                //unBlockUI();                   
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                $("#panelEditChallanNoDetails").hide();
                //$.unblockUI();
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');
                // Added by SB on 22-3-2019 at 11:06 am
                var searchString = $('#DailyReceiptDetailsTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#DailyReceiptDetailsTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            //    $('#menuDetailsListTable_filter input').val('');
                            ChallanDetailsRptTable.search('').draw();
                            $("#ChallanDetailsTable_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        scrollX: true,
        "scrollY": "300px",
        "scrollCollapse": true,
        bPaginate: true,
        bLengthChange: true,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,

        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [

        ],


        columns: [

            { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
            { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
            { data: "IsPayDoneAtDROffice", "searchable": true, "visible": true, "name": "IsPayDoneAtDROffice" },
            { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName" },
            { data: "ChallanNumber", "searchable": true, "visible": true, "name": "ChallanNumber" },
            { data: "ChallanDate", "searchable": true, "visible": true, "name": "ChallanDate" },
            { data: "Amount", "searchable": true, "visible": true, "name": "Amount" },
            { data: "IsStampPayment", "searchable": true, "visible": true, "name": "IsStampPayment" },
            { data: "IsReceiptPayment", "searchable": true, "visible": true, "name": "IsReceiptPayment" },
            { data: "ReceiptNumber", "searchable": true, "visible": true, "name": "ReceiptNumber" },
            { data: "Receipt_StampPayDate", "searchable": true, "visible": true, "name": "Receipt_StampPayDate" },
            { data: "ServiceName", "searchable": true, "visible": true, "name": "ServiceName" },
            { data: "DocumentPendingNumber", "searchable": true, "visible": true, "name": "DocumentPendingNumber" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
        ],
        fnInitComplete: function (oSettings, json) {
            $("#UpdateButtonID").html(json.UpdateBtn);
            $("#idRemarkMessage").html(json.RemarkMessage);
        },
        preDrawCallback: function () {
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
            //responsiveHelper.respond();
            unBlockUI();
        },

    });

}




function SaveChallanDetails() {



    //gets table
    var oTable = document.getElementById('ChallanDetailsTable');

    //gets rows of table

    var oCells = oTable.rows.item(1).cells;
    var idHiddenInstrumentNo = oCells.item(4).innerHTML;
    var cellval2;


    //gets rows of table
    var rowLength = oTable.rows.length;

    //loops through rows    
    for (i = 1; i < rowLength; i++) {

        //gets cells of current row  
        var oCells = oTable.rows.item(i).cells;

        //gets amount of cells of current row
        var cellLength = oCells.length;

        //loops through each cell in current row
        //for (var j = 0; j < cellLength; j++) {

        // get your cell info here

        var cellVal = oCells.item(4).innerHTML;
        cellval2 = cellVal + ",";
    }

    document.getElementById('idHiddenInstrumentNo').value = idHiddenInstrumentNo;


    let formData = $("#ChallanDetailsForm").serialize();

    blockUI();
    $.ajax({
        type: "POST",
        url: "/ChallanNoDataEntryCorrection/ChallanNoDataEntryCorrection/SaveChallanDetails",
        data: formData,
        //headers: header,
        success: function (data) {
            unBlockUI();
            if (data.success) {


                bootbox.alert({
                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                    callback: function () {
                        
                        LoadChallanDetailsTable();
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




function updateChallanDetails(ChallanDetails) {

    var ChallanDetailsArray = ChallanDetails.split(',');
    var ChallanNumber = ChallanDetailsArray[0];
    var oldChallanNumber = ChallanDetailsArray[1];
    var ChallanDate = ChallanDetailsArray[2];
    var SROCode = ChallanDetailsArray[3];
    var DistrictCode = ChallanDetailsArray[4];
    var ChallanCorrectionID1 = ChallanDetailsArray[5];
    

    blockUI();
    $.ajax({
        type: "POST",
        url: "/ChallanNoDataEntryCorrection/ChallanNoDataEntryCorrection/UpdateChallanDetails",
        data: { 'ChallanDetails': ChallanDetails },
        success: function (data) {
            unBlockUI();
            window.stop();
            if (data.success) {
                
                BootBoxCall(data.message, data.success);
                
            }
            else {
                BootBoxCall(data.message, data.success);
            }
        },
        error: function () {
            window.stop();
            BootBoxCall(data.message, data.success);
        }
    });

    

}



function BootBoxCall(message, status) {
    if (message != null && status) {
        bootbox.alert({
            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + message + '</span>',
            callback: function () {
                window.location.reload();
            }
        });
    }
    else {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + message + '</span>',
            callback: function () {
                window.location.reload();
            }
        });
    }
}













