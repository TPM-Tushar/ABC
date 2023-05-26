var token = '';
var header = {};
isValid = false;
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    //var SROOfficeID;
    //var DROfficeID;
    //var DocumentNumber;
    //var BookTypeID;
    //var FinancialYear;


    //Added by Madhusoodan on 11/08/2021 to load order filename in edit mode
    if (filename != null || filename != "") {
        document.getElementById('spnOrderFileName').innerHTML = filename;
    }

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
    $("#idOrdeNo").on("paste", function (e) {
        e.preventDefault();
        return false;
    });

    //$("#idOrdeNo").bind("keydown", function (e) {
    //    if (!((e.which <= 90 && e.which >= 65) || (e.which <= 57 && e.which >= 48) || e.shiftKey || e.which == 188 || e.which == 190 || e.which == 191 || e.which == 220 || e.which == 219 || e.which == 51 || e.which == 221 || e.which == 32 || e.which == 8 || e.which == 173)) {
    //        e.preventDefault();
    //        return false;
    //    }
    //    else {
    //        //alert(e.which);
    //        if (e.shiftKey) {
    //            if (e.which == 51 || e.which == 55 || e.which == 51 || e.which == 56 || e.which == 51 || e.which == 173 || e.which == 51 || e.which == 219 || e.which == 221 || (e.which <= 90 && e.which >= 65)) {

    //            }
    //            else {
    //                e.preventDefault();
    //                return false;
    //            }
    //        }

    //    }
    //});

    var tabs_items = document.querySelectorAll(".tabs");
    tabs_items.forEach(function (tabs) {
        // Set variable
        var controls = tabs.querySelector(".tabs__control");
        var tab = controls.querySelectorAll(".tabs__tab");
        var contents = tabs.querySelector(".tabs__contents");
        var content = contents.querySelectorAll(".tabs__content");

        // Loop through all tabs
        tab.forEach(function (item) {
            item.onclick = function (e) {
                e.preventDefault();

                // Get clicked tab ID
                var tabId = item.dataset.tab;

                // Set current tab
                tab.forEach(function (item) {
                    if (tabId == item.dataset.tab) {
                        item.classList.add("tabs__tab--current");
                        item.setAttribute('aria-selected', 'true');
                        item.removeAttribute('tabindex', '-1');
                    } else {
                        item.classList.remove("tabs__tab--current");
                        item.setAttribute('aria-selected', 'false');
                        item.setAttribute('tabindex', '-1');
                    }
                });

                // Set current content
                content.forEach(function (item) {
                    if (tabId == item.dataset.tabContent) {
                        item.classList.add("tabs__content--current");
                        item.removeAttribute('hidden', 'hidden');
                    } else {
                        item.classList.remove("tabs__content--current");
                        item.setAttribute('hidden', 'hidden');
                    }
                });
            };
        });
    });


    //Remove
    //$('#DtlsSearchParaListCollapseOrderDetail').click(function () {
    //    var classToRemove = $('#DtlsToggleIconSearchParaOrderDetail').attr('class');
    //    var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
    //    $('#DtlsToggleIconSearchParaOrderDetail').removeClass(classToRemove).addClass(classToSet);
    //});

    //$('#DivCollapseButton').click(function () {
    //    var classToRemove = $('#CollapseButtonIcon').attr('class');
    //    var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
    //    $('#CollapseButtonIcon').removeClass(classToRemove).addClass(classToSet);
    //});


    $('#DivCollapseButton').click(function () {
        var classToRemove = $('#CollapseButtonIcon').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#CollapseButtonIcon').removeClass(classToRemove).addClass(classToSet);
    });

    $('#txtOrderDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        startDate: new Date('01/01/2003'),
        maxDate: '0',
        minDate: new Date('01/01/2003'),
        pickerPosition: "bottom-left"//,
    });

    $('#divFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        startDate: new Date('01/01/2003'),
        maxDate: new Date(),
        minDate: new Date('01/01/2003'),
        pickerPosition: "bottom-left"
    });


    $('#txtOrderDate').focusout(function () {
        //alert('rgegreg');

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        //if ($.fn.DataTable.isDataTable("#OrderTableID")) {
        //    $("#OrderTableID").DataTable().clear().destroy();

        //}
    });

    //Modified by Madhusoodan on 16/08/2021 for Save order operation
    $("#SaveOrderBtn").click(function () {
        if ($("#SROOfficeListID").val() == '0' || $("#SROOfficeListID").val() == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Enter Select Sro name  </span>'
            });
        }
       else if ($('#idOrdeNo').val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Enter Valid Order No</span>'
            });
        }
        else if ($('#txtOrderDate').val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Enter Valid Order Date</span>'
            });
        }
        else if (isValid) {
            SaveOrderDetails();
        }
        else {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select DR Order PDF file to upload.</span>'
            });
        }
    });

    //Added by Madhusoodan on 16/08/2021 for Update Order operation
    $("#UpdateOrderBtn").click(function () {

        if ($('#idOrdeNo').val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Enter Valid Order No</span>'
            });
        }
        else if ($('#txtOrderDate').val() == '') {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Enter Valid Order Date</span>'
            });
        }
        else {
            SaveOrderDetails();
        }
    });

    $('#btnDeleteCurrentOrderFile').click(function () {
        DeleteCurrentOrderFile();
    });

});

function loadOrderDetailsTable() {
    var DetailsTable = $('#DetailTableID').DataTable({
        ajax: {
            url: '/DataEntryCorrection/DataEntryCorrection/LoadPropertyDetailsData',
            type: "POST",
            headers: header,
            data: {
                'SROOfficeID': SROOfficeID, 'DROfficeID': DROfficeID, 'DocumentNumber': DocumentNumber, 'BookTypeID': BookTypeID, 'FinancialYear': FinancialYear,
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {


                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            } else {
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#DetailTableID").DataTable().clear().destroy();
                            }
                        }
                    });
                    return;
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchParaDetail').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsToggleIconSearchParaDetail').trigger('click');
                }
                unBlockUI();
                return json.data;
            },
            error: function () {
                unBlockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');
                var searchString = $('#DetailTableID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;

                    if (!regexToMatch.test(searchString)) {
                        $("#DetailTableID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DetailsTable.search('').draw();
                            $("#DetailTableID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        //"scrollX": true,
        //"scrollY": "300px",
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

        columnDefs: [
            { "className": "dt-center", "targets":"_all" },
           
            //{ orderable: false, targets: [1] },
            //{ orderable: false, targets: [2] },
            //{ orderable: false, targets: [3] },
            //{ orderable: false, targets: [4] }
        ],

        columns: [{ data: "Select", "searchable": true, "visible": true, "name": "Select" },
        { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
        { data: "PropertyDescription", "searchable": true, "visible": true, "name": "PropertyDescription" },
        { data: "PropertyNumberDetail", "searchable": true, "visible": true, "name": "PropertyNumberDetail" },
        { data: "ExecutionDate", "searchable": true, "visible": true, "name": "ExecutionDate" },
        { data: "NatureofDocument", "searchable": true, "visible": true, "name": "NatureofDocument" },
        { data: "Marketvalue", "searchable": true, "visible": true, "name": "Marketvalue" },
        { data: "Consideration", "searchable": true, "visible": true, "name": "Consideration" },
        { data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" },
        { data: "CDNumber", "searchable": true, "visible": true, "name": "CDNumber" },
        { data: "ViewBtn", "searchable": true, "visible": true, "name": "ViewBtn" }
        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
            //$("#AddOrderSPANID").html(json.btnAddOrder);
            //$("#EncryptedDocumentId").val(json.EncrytptedDocumetID);
            //$("#EncryptedDocumentId").val();
            //console.log(json.EncrytptedDocumetID);
            //console.log($("#EncryptedDocumentId").val());

        },
        preDrawCallback: function () {
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {

            //if (ModuleID == 1) {
            //    fnSetColumnVis(2, false);
            //}
            //else if (ModuleID == 2) {
            //    fnSetColumnVis(0, false);
            //    fnSetColumnVis(1, false);
            //}

            unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
            unBlockUI();
        },
    });
}

function SaveOrderDetails() {

    //alert("In SaveOrder Details");

    //Check if input tag is there, If yes then prompt user to select file. If not then Update Order No and Order Date only
    if (($("input[type='file']")).length == 1) {

        //($("input[type='file']")).length;
        //alert("Upload file and save");

        var formData = new FormData();
        var filesArray = null;

        //if ($("input[type='file']").val() == '') {
        //    alert("File is empty");
        //}

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
                formData.append("OrderNo", $("#idOrdeNo").val());
                formData.append("OrderDate", $("#txtOrderDate").val());

                var form = $("#OrderDetailForm");
                form.removeData('validator');
                form.removeData('unobtrusiveValidation');
                $.validator.unobtrusive.parse(form);

                if ($("#OrderDetailForm").valid()) {

                    blockUI();
                    $.ajax({
                        type: "POST",
                        url: "/DataEntryCorrection/DataEntryCorrection/UploadOrdersFile",
                        data: formData,
                        headers: header,
                        processData: false,
                        contentType: false,
                        success: function (data) {
                            if (data.success) {

                                $("#hdnfileContent").val(data.filePath);

                                $("#hdnRelativeFilePath").val(data.relativeFilePath);
                                $("#hdnNewOrderID").val(data.orderID);
                                $("#hdnExistingFileName").val(data.orderFileName);

                                //relativeFilePath
                                //var orderData = $("#OrderDetailForm").serializeArray();
                                //orderData.push({ name: 'relativeFilePath', value: data.relativeFilePath });
                                //

                                //alert("FilePath: " + $("#hdnfileContent").val());
                                //alert("RelativeFilePath: " + $("#hdnRelativeFilePath").val());
                                //alert("NwwOrderID: " + $("#hdnNewOrderID").val());

                                //Uncomment from here
                                blockUI();

                                $.ajax({
                                    type: "POST",
                                    url: "/DataEntryCorrection/DataEntryCorrection/SaveOrderDetails",
                                    //url: "/DataEntryCorrection/DataEntryCorrection/NewSaveOrderDetails",
                                    data: $("#OrderDetailForm").serialize(),
                                    //data: orderData,
                                    headers: header,
                                    //  processData: false,
                                    // contentType: false,
                                    success: function (data) {
                                        unBlockUI();
                                        if (data.success) {

                                            bootbox.alert({
                                                message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                                callback: function () {

                                                    //window.location.href = "/SROScriptManager/SROScriptManager/SROScriptManagerView";
                                                    //}
                                                    //alert("Sending to SelectDocument tab");

                                                    //For opening Select document tab
                                                    var TableName = "SelectDocument";
                                                    //Code commented by mayank on 29/11/2021
                                                    window.location.reload();
                                                    //$.ajax({
                                                    //    url: '/DataEntryCorrection/DataEntryCorrection/LoadInsertUpdateDeleteView',
                                                    //    headers: header,
                                                    //    type: "POST",
                                                    //    data: {
                                                    //        'TableViewName': TableName,
                                                    //        //'DocumentID': DocumentIDVar,
                                                    //        //'SROCode': SROCodeVar,
                                                    //    },
                                                    //    success: function (data) {

                                                    //        // REMOVE PREVIOUS ADDED ACTIVE CLASS
                                                    //        $('#AddEditOrderTabDivID').removeClass('active');
                                                    //        $('#SelectDocumentTabDivID').removeClass('active');
                                                    //        //$('#PropertyNumberDetailsTabDivID').removeClass('active');  //Not in use 
                                                    //        //$('#PartyDetailsMenuTabDivID').removeClass('active');

                                                    //        // ADD ACTIVE CLASS By Table Name
                                                    //        if (TableName == "AddEditOrder") {

                                                    //            // alert("In Order (AddEditOrder Tab)");

                                                    //            $('#AddEditOrderTabDivID').addClass('active');
                                                    //        }
                                                    //        else if (TableName == "SelectDocument") {
                                                    //            //alert("In (Select Document Tab)");

                                                    //            $('#SelectDocumentTabDivID').addClass('active');
                                                    //        }
                                                    //        //Not in use now
                                                    //        //else if (TableName == "PropertyNumberDetails") {
                                                    //        //    $('#PropertyNumberDetailsTabDivID').addClass('active');
                                                    //        //}
                                                    //        //else if (TableName == "PartyDetails") {
                                                    //        //    $('#PartyDetailsMenuTabDivID').addClass('active');
                                                    //        //}

                                                    //        $('#LoadDECInsertUpdateDeleteViewDivID').html('');
                                                    //        $('#LoadDECInsertUpdateDeleteViewDivID').html(data);

                                                    //        unBlockUI();
                                                    //    },
                                                    //    error: function (xhr) {

                                                    //        bootbox.alert({
                                                    //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                                    //            callback: function () {
                                                    //            }
                                                    //        });
                                                    //        unBlockUI();
                                                    //    },
                                                    //    beforeSend: function () {
                                                    //        blockUI('Loading Data please wait');
                                                    //    }
                                                    //});
                                                }
                                            });
                                            //Added by Madhusoodan on 08/08/2021 to refresh DR Orders grid after saving new DR Order
                                            DocDetailsTable();
                                            //
                                            //});
                                            //}
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
                            else {
                                unBlockUI();
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
            }
            else {
                //alert("In else ...");


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
        $.ajax({
            type: "POST",
            url: "/DataEntryCorrection/DataEntryCorrection/SaveOrderDetails",
            //url: "/DataEntryCorrection/DataEntryCorrection/NewSaveOrderDetails",
            data: $("#OrderDetailForm").serialize(),
            //data: orderData,
            headers: header,
            //  processData: false,
            // contentType: false,
            success: function (data) {
                unBlockUI();
                if (data.success) {

                    bootbox.alert({
                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                        callback: function () {

                            //window.location.href = "/SROScriptManager/SROScriptManager/SROScriptManagerView";
                            //}
                            //alert("Sending to SelectDocument tab");
                            window.location.reload();
                            //For opening Select document tab
                            //var TableName = "SelectDocument";

                            //$.ajax({
                            //    url: '/DataEntryCorrection/DataEntryCorrection/LoadInsertUpdateDeleteView',
                            //    headers: header,
                            //    type: "POST",
                            //    data: {
                            //        'TableViewName': TableName,
                            //        //'DocumentID': DocumentIDVar,
                            //        //'SROCode': SROCodeVar,
                            //    },
                            //    success: function (data) {

                            //        // REMOVE PREVIOUS ADDED ACTIVE CLASS
                            //        $('#AddEditOrderTabDivID').removeClass('active');
                            //        $('#SelectDocumentTabDivID').removeClass('active');
                            //        //$('#PropertyNumberDetailsTabDivID').removeClass('active');  //Not in use 
                            //        //$('#PartyDetailsMenuTabDivID').removeClass('active');

                            //        // ADD ACTIVE CLASS By Table Name
                            //        if (TableName == "AddEditOrder") {

                            //            // alert("In Order (AddEditOrder Tab)");

                            //            $('#AddEditOrderTabDivID').addClass('active');
                            //        }
                            //        else if (TableName == "SelectDocument") {
                            //            //alert("In (Select Document Tab)");

                            //            $('#SelectDocumentTabDivID').addClass('active');
                            //        }
                            //        //Not in use now
                            //        //else if (TableName == "PropertyNumberDetails") {
                            //        //    $('#PropertyNumberDetailsTabDivID').addClass('active');
                            //        //}
                            //        //else if (TableName == "PartyDetails") {
                            //        //    $('#PartyDetailsMenuTabDivID').addClass('active');
                            //        //}

                            //        $('#LoadDECInsertUpdateDeleteViewDivID').html('');
                            //        $('#LoadDECInsertUpdateDeleteViewDivID').html(data);

                            //        unBlockUI();
                            //    },
                            //    error: function (xhr) {

                            //        bootbox.alert({
                            //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            //            callback: function () {
                            //            }
                            //        });
                            //        unBlockUI();
                            //    },
                            //    beforeSend: function () {
                            //        blockUI('Loading Data please wait');
                            //    }
                            //});
                        }
                    });
                    //Added by Madhusoodan on 08/08/2021 to refresh DR Orders grid after saving new DR Order
                    DocDetailsTable();
                    //
                    //});
                    //}
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

        ////
    }
}

function DeleteCurrentOrderFile() {
    //alert("In Del");

    blockUI('Deleting DR Order File... please wait...');

    $.ajax({
        url: '/DataEntryCorrection/DataEntryCorrection/DeleteCurrentOrderFile',
        data: "",
        headers: header,
        type: "POST",
        success: function (data) {
            unBlockUI();
            if (data.success) {

                bootbox.alert({
                    message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                });

                if (data.currentOrderID != 0) {
                    AddEditOrder(data.currentOrderID);
                    $('#DROrderDetailsCollapse').trigger('click');
                    $('#spnOrderFileName').val('');
                }
            }
            else {

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