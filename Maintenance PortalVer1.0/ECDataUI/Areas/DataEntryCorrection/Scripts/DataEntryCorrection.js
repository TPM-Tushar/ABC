var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $(document).bind("contextmenu", function (e) {
        e.preventDefault();
    });
    $(document).keydown(function (e) {
        if (e.which === 123) {
            return false;
        }
    });


    //changed 'and' to 'or' condition in else if statement by madhur on 22-08-2022
    if (sessionStorage.getItem('IsValidated') == 1) {
        sessionStorage.setItem('IsValidated', '2');
    }

    else if (performance.navigation.type == performance.navigation.TYPE_RELOAD || sessionStorage.getItem('IsValidated') == 2) {

    }
    else {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">OTP Validation is Required.</span>',
            function () {
                sessionStorage.setItem('IsValidated', '0');
                window.location.href = "/Home/HomePage";
            });
    }



    //if (sessionStorage.getItem('IsValidated') == 1) {
    //    sessionStorage.setItem('IsValidated', '2');
    //}
    //else if (performance.navigation.type == performance.navigation.TYPE_RELOAD && sessionStorage.getItem('IsValidated') == 2) {

    //}
    //else {
    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">OTP Validation is Required.</span>',
    //        function () {
    //            sessionStorage.setItem('IsValidated', '0');
    //            window.location.href = "/Home/HomePage";
    //        });
    //}


    $('#iconSideBar').trigger('click');

    //Added by Madhusoodan on 20/08/2021 to hide popup of Index II report for finalized orders
    $('#divLoadAbortViewForPropertyPopup').hide();


    $('#DROrderDetailsCollapse').click(function () {


        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $('#panelAllTabs').hide();

    //DocDetailsTable();

    //Added by Madhur on 26/07/2021
    $("#btncloseAbortPopup").click(function () {

        $("#objPDFViewer").attr('data', '');
        $('#divViewAbortModal').css('display', 'none');
        $('#divViewAbortModal').addClass('modal fade');
        //$('#divViewAbortModal').modal('show');
      


        //$('#divViewAbortModal').modal('hide');
        //$("#divViewAbortModal").modal('hide')
    });

    $('#DtlsSearchParaListCollapse').click(function () {
        //alert('click 1');

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $('#DtlsSearchParaListCollapseDetail').click(function () {
        //alert('click');
        var classToRemove = $('#DtlsToggleIconSearchParaDetail').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchParaDetail').removeClass(classToRemove).addClass(classToSet);
    });

    //$('#DROrderDetailsCollapse').click(function () {
    //    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    //    var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
    //    $('#DtlsToggleIconSearchParaDtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    //});

    //LoadTabView();
    $('#btnAddNewOrder').click(function () {

        //Commented and added by Madhusoodan on 08/08/2021 to collapse DR Order Details table on Add new order click
        //$('#DROrderDetailsCollapse').trigger('click');

        //var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');

        //var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        //$('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
        //$('#DROrderDetailsCollapse').removeClass("row well-custom").addClass("row well-custom collapsed");
        //$('#DROrderDetailsList').removeClass("collapse in").addClass("collapse");
    });

    $('#btnFinalizeDEC').click(function () {
        FinalizeDEC();
    });

    //$("#").change(function () {

    //});
    //Added by mayank on 16/08/2021 Populate Village on SRO Change
    $('#DROOfficeOrderListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/DataEntryCorrection/DataEntryCorrection/GetSroCodebyDistrict',
            data: { "DROCode": $('#DROOfficeOrderListID').val() },
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
                    $('#SROOfficeOrderListID').empty();
                    $.each(data.SroList, function (i, SroList) {
                        //SROOfficeList
                        $('#SROOfficeOrderListID').append('<option value="' + SroList.Value + '">' + SroList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });

    $('#btncloseAbortPopup2').click(function () {

        //$('#divViewAbortModalForIndexIIPopup').css('display', 'none');
        //$('#divViewAbortModalForIndexIIPopup').addClass('modal fade');
        $('#divViewAbortModalForIndexIIPopup').modal('hide');

    });
    //Added by mayank on 16/08/2021
    $('#btnclosePropertyPopup').click(function () {

        //$('#divViewAbortModalForIndexIIPopup').css('display', 'none');
        //$('#divViewAbortModalForIndexIIPopup').addClass('modal fade');
        $('#divViewAbortModalForIndexIIPopup').modal('hide');
    });

    $("#DROrderTableDiv").hide();

    SearchOrder();
});



function ViewBtnClickOrderTable(OrderID) {

    //Added by shivam b on 04/05/2022 for Virtual Directory on Section 68 Note Data Entry
    blockUI();
    $('#divViewAbortModal').css('display', 'block');
    $('#divViewAbortModal').addClass('modal fade in');
    $("#objPDFViewer").attr('data', '');
    $("#objPDFViewer").attr('data', '/DataEntryCorrection/DataEntryCorrection/ViewBtnClickOrderTable?OrderID=' + OrderID);
    unBlockUI();
    //Added by shivam b on 04/05/2022 for Virtual Directory on Section 68 Note Data Entry


    //$.ajax({
    //    url: '/DataEntryCorrection/DataEntryCorrection/ViewBtnClickOrderTable',
    //    data: { "OrderID": OrderID },
    //    type: "GET",
    //    success: function (data) {
    //        if (data != "Could not find file") {
    //            $('#divViewAbortModal').css('display', 'block');
    //            $('#divViewAbortModal').addClass('modal fade in');
    //            //$('#divViewAbortModal').modal('show');
    //            $("#objPDFViewer").attr('data', '');
    //            $("#objPDFViewer").load(data);
    //            $("#objPDFViewer").attr('data', '/DataEntryCorrection/DataEntryCorrection/ViewBtnClickOrderTable?OrderID=' + OrderID);

    //        }
    //        else {
    //            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">File not exist at path.</span>');
    //        }
    //        unBlockUI();
    //    },
    //    error: function (xhr) {
    //        alert(xhr);
    //        unBlockUI();
    //    }
    //});
}




//Added by Madhur
function DocDetailsTable() {

    var DetailsTable = $('#DROrderDtlsTable').DataTable({
        ajax: {
            url: '/DataEntryCorrection/DataEntryCorrection/LoadDocDetailsTable',
            type: "POST",
            headers: header,
            data: {
                'DroCode': $("#DROOfficeOrderListID").val(), 'SroCode': $("#SROOfficeOrderListID").val(),
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
            { "className": "dt-center ShorterTextClass", "targets": [4] },
            { "className": "dt-center", "targets": "_all" },
            { "orderable": false, "targets": "_all" },
            { "bSortable": false, "aTargets": "_all" },
            { "width": "200px", "targets": [6] },
            //{ orderable: false, targets: [1] },
            //{ orderable: false, targets: [2] },
            //{ orderable: false, targets: [3] },
            //{ orderable: false, targets: [4] }
        ],

        columns: [
            { data: "SNo", "searchable": true, "visible": true, "name": "SNo" },
            { data: "DROName", "searchable": true, "visible": true, "name": "DROName" },
            { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
            { data: "EnteredBY", "searchable": true, "visible": true, "name": "EnteredBY" },
            { data: "DROrderNumber", "searchable": true, "visible": true, "name": "DROrderNumber" },
            { data: "OrderDate", "searchable": true, "visible": true, "name": "OrderDate" },
            //Commented and added by Madhusoodan on 10/08/2021 (to send tooltip code from DAL)
            //{ data: "Section68Note", "searchable": true, "visible": true, "name": "Section68Note", render: function (data, type, full, meta) { return data + '<span data-toggle="tooltip" class="tooltiptext" title="' +  data + '"></span>';} },
            { data: "Section68Note", "searchable": true, "visible": true, "name": "Section68Note" },
            { data: "RegistrationNumber", "searchable": true, "visible": true, "name": "RegistrationNumber" },
            { data: "Status", "searchable": true, "visible": true, "name": "Status" },
            { data: "ViewBtn", "searchable": true, "visible": true, "name": "ViewBtn" },
            { data: "Action", "searchable": true, "visible": true, "name": "Action" },
        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
            if (json.isDrLogin) {
                $("#AddOrderSPANID").show();
            }
            else {
                $("#AddOrderSPANID").hide();
            }

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
       //Commented by mayank on 17/09/2021 uncommneted after sp execution
    //DetailsTable.columns([1]).visible(false);
    //DetailsTable.columns([3]).visible(false);
}

//alert("In DocDetailsTable");
//Modified by Madhusodan on 12/08/2021
//Added by Madhur
//function DeleteBtnClickOrderTable(orderID) {
function DeleteDROrder(orderID) {

    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        //Commented and aded by Madhusoodan on 15/12/2021
        //message: "<span class='boot-alert-txt'>Do you want to delete this order ?</span>",
        message: "<span class='boot-alert-txt'>Do you want to reset this order ?</span>",

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
                blockUI('Deleting DR Order... please wait...');

                $.ajax({
                    url: '/DataEntryCorrection/DataEntryCorrection/DeleteDECOrder',
                    data: { "orderID": orderID },
                    type: "GET",
                    success: function (data) {
                        unBlockUI();
                        if (data.success) {
                            //bootbox.alert({
                            //    message: '<i class="fa fa-trash" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.message + '</span>',

                            //});

                            //Keep this alert **** as it showsDelete message 
                            //alert(data.message);   //Do not comment

                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                callback: function () {
                                  
                                }
                            });
                            //Commented by mayank
                            //window.location.href = "http://localhost:8888/DataEntryCorrection/DataEntryCorrection/DataEntryCorrectionView";
                            window.location.reload();
                            //refresh DR Order datatable
                            //DocDetailsTable();
                            //Instead of refresh load entire page

                            ////
                            //var bootboxConfirm2 = bootbox.confirm({
                            //    //title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                            //    message: "<span class='boot-alert-txt'>" + data.message + "</span>",

                            //    buttons: {
                            //        //cancel: {
                            //        //    label: '<i class="fa fa-times"></i> No',
                            //        //    className: 'pull-right margin-left-NoBtn'
                            //        //},
                            //        confirm: {
                            //            label: '<i class="fa fa-check"></i> OK'
                            //        }
                            //    },
                            //    callback: function (result) {
                            //        if (result) {
                            //            window.location.href = "http://localhost:8888/DataEntryCorrection/DataEntryCorrection/DataEntryCorrectionView";
                            //        }
                            //    }
                            //});
                            ////
                        }
                        else {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                            });
                        }
                    },
                    error: function (xhr) {
                        unBlockUI();
                        alert("in XHR error");
                        alert(xhr);
                    }
                });
            }
        }
    });



}



//function AddNewOrder() {
function AddEditOrder(orderID) {

    //alert("OrderID: " + orderID);

    blockUI('loading data.. please wait...');

    $.ajax({
        url: '/DataEntryCorrection/DataEntryCorrection/LoadAddNewOrderView',
        headers: header,
        data: { orderID },
        datatype: "json",
        type: "POST",
        success: function (data) {

            //alert("In data");

            if (data.serverError == true) {

                //alert("In data.ServerError= true");

                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                //alert("In else.. Loading Partial View Order");

                $('#panelAllTabs').show();

                $('#LoadDECInsertUpdateDeleteViewDivID').html('');

                $('#LoadDECInsertUpdateDeleteViewDivID').html(data);
                $('#AddEditOrderTabDivID').addClass('active');
                //Added by Madhusoodan on 12/08/2021
                $('#SelectDocumentTabDivID').removeClass('active');

                //Added by Madhusoodan on 08/08/2021 to collapse DR Order Details table on edit click
                $('#DROrderDetailsCollapse').trigger('click');

            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });

}


//Old AddOrder() (Reuse it)
function AddOrder() {

    //alert("In Add order js func");

    blockUI('loading data.. please wait...');
    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        $('#DtlsToggleIconSearchPara').trigger('click');

    var EncryptedPropertyId = $('input[type=radio]:checked').val();
    //alert("Add button click " + value);
    //alert("Add button click " + value);

    if (EncryptedPropertyId == undefined) {
        unBlockUI();

        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select property for which order needs to be added</span>');
        return;

    }

    var EncryptedDocumentID = $("#EncryptedDocumentId").val();
    if (EncryptedDocumentID == undefined || EncryptedDocumentID == '' || EncryptedDocumentID == "") {
        unBlockUI();

        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Document Information not set.Please contact admin</span>');
        return;

    }
    var _SroCode = $("#SROOfficeListID").val();
    if (_SroCode == 0) {
        unBlockUI();
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Sro Office and submit form again</span>');
        return;
    }
    //alert(EncryptedDocumentId);"EncryptedDocumentId": EncryptedDocumentId 
    $.ajax({
        url: '/DataEntryCorrection/DataEntryCorrection/LoadOrderDetailsView',
        data: { "EncrytedPropertyId": EncryptedPropertyId, "EncryptedDocumentID": EncryptedDocumentID, "SroCode": _SroCode },
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
                //alert("In else.. Loading Partial View Order");
                //$('#SROOfficeListID').empty();
                //$.each(data.SROOfficeList, function (i, SROOfficeList) {
                //    SROOfficeList
                //    $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                //});
                $("#PartialViewLoader").html(data);
                //$("#SpanHeader").html("Order Details");
            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });
    //console.log(value);

}

//Check qand remove if not in use
function ViewSection68Note(EncryptedPropertyId) {
    //alert(EncryptedPropertyId);
    var EncryptedDocumentID = $("#EncryptedDocumentId").val();
    if (EncryptedDocumentID == undefined || EncryptedDocumentID == '' || EncryptedDocumentID == "") {
        unBlockUI();

        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Document Information not set.Please contact admin</span>');
        return;

    }
    var _SroCode = $("#SROOfficeListID").val();
    if (_SroCode == 0) {
        unBlockUI();
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Sro Office and submit form again</span>');
        return;
    }
    $.ajax({
        url: '/DataEntryCorrection/DataEntryCorrection/LoadOrderDetailsView',
        data: { "EncrytedPropertyId": EncryptedPropertyId, "EncryptedDocumentID": EncryptedDocumentID, "SroCode": _SroCode },
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
                //$('#SROOfficeListID').empty();
                //$.each(data.SROOfficeList, function (i, SROOfficeList) {
                //    SROOfficeList
                //    $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                //});
                $("#PartialViewLoader").html(data);
                //$("#SpanHeader").html("Order Details");
            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });
}



function LoadTabView(TableName) {

    //alert("In Load Tab View: " + TableName);

    ////
    $.ajax({
        url: '/DataEntryCorrection/SelectDocument/CheckOrderInSession',
        headers: header,
        type: "GET",
        data: {
        },
        success: function (data) {

            if (data.success) {
                ////
                $.ajax({
                    url: '/DataEntryCorrection/DataEntryCorrection/LoadInsertUpdateDeleteView',
                    headers: header,
                    type: "POST",
                    data: {
                        'TableViewName': TableName,
                    },
                    success: function (data) {

                        if (data != null) {


                            // REMOVE PREVIOUS ADDED ACTIVE CLASS
                            $('#AddEditOrderTabDivID').removeClass('active');
                            $('#SelectDocumentTabDivID').removeClass('active');
                            $('#PropertyNumberDetailsTabDivID').removeClass('active');
                            $('#PartyDetailsMenuTabDivID').removeClass('active');

                            // ADD ACTIVE CLASS By Table Name
                            if (TableName == "AddEditOrder") {

                                $('#AddEditOrderTabDivID').addClass('active');
                            }
                            else if (TableName == "SelectDocument") {
                                //alert("In (Select Document Tab)");

                                $('#SelectDocumentTabDivID').addClass('active');
                            }
                            else if (TableName == "PropertyNumberDetails") {
                                $('#PropertyNumberDetailsTabDivID').addClass('active');
                            }
                            else if (TableName == "PartyDetails") {
                                $('#PartyDetailsMenuTabDivID').addClass('active');
                            }

                            $('#LoadDECInsertUpdateDeleteViewDivID').html('');
                            $('#LoadDECInsertUpdateDeleteViewDivID').html(data);

                            unBlockUI();
                        }
                        else {
                            unBlockUI();
                            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon">' + data.message + '</i><span class="boot-alert-txt"></span>');
                        }
                    },
                    error: function (xhr) {

                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            callback: function () {
                            }
                        });
                        unBlockUI();
                    },
                    beforeSend: function () {
                        blockUI('Loading Data please wait');
                    }
                });
                ////
            }
            else {
                unBlockUI();
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon">' + data.message + '</i><span class="boot-alert-txt"></span>');
            }
        },
        error: function (xhr) {

            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            unBlockUI();
        },
        beforeSend: function () {
            blockUI('Loading Data please wait');
        }
    });

    ////

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

    //        if (data != null) {


    //            // REMOVE PREVIOUS ADDED ACTIVE CLASS
    //            $('#AddEditOrderTabDivID').removeClass('active');
    //            $('#SelectDocumentTabDivID').removeClass('active');
    //            $('#PropertyNumberDetailsTabDivID').removeClass('active');
    //            $('#PartyDetailsMenuTabDivID').removeClass('active');

    //            // ADD ACTIVE CLASS By Table Name
    //            if (TableName == "AddEditOrder") {

    //                $('#AddEditOrderTabDivID').addClass('active');
    //            }
    //            else if (TableName == "SelectDocument") {
    //                //alert("In (Select Document Tab)");

    //                $('#SelectDocumentTabDivID').addClass('active');
    //            }
    //            else if (TableName == "PropertyNumberDetails") {
    //                $('#PropertyNumberDetailsTabDivID').addClass('active');
    //            }
    //            else if (TableName == "PartyDetails") {
    //                $('#PartyDetailsMenuTabDivID').addClass('active');
    //            }

    //            $('#LoadDECInsertUpdateDeleteViewDivID').html('');
    //            $('#LoadDECInsertUpdateDeleteViewDivID').html(data);

    //            unBlockUI();
    //        }
    //        else {
    //            unBlockUI();
    //            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon">' + data.message + '</i><span class="boot-alert-txt"></span>');
    //        }
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


function ViewIndexIIReport(OrderID) {

    //$.ajax({     
    //    url: '/DataEntryCorrection/DataEntryCorrection/LoadIndexIIData',
    //    data: { 'PropertyID': PropertyID },
    //    headers: header,
    //    type: "GET",
    //    success: function (data) {

    //$('#divLoadAbortViewForPropertyPopup').html('');
    ////$('#divLoadAbortViewForPropertyPopup').html(data);
    //$('#divLoadAbortViewForPropertyPopup').show();

    ////LoadIndexIIDatatable(DocumentID, PropertyID);
    //LoadIndexIIDatatable(133249, 277073);

    //$('#divViewAbortModalForPropertyPopup').css('display', 'block');
    //$('#divViewAbortModalForPropertyPopup').addClass('modal fade in');

    //$("#divViewAbortModalForIndexIIPopup").show();
    //$('#divViewAbortModalForIndexIIPopup').css('display', 'block');
    //$('#divViewAbortModalForIndexIIPopup').addClass('modal fade in');
    $("#divViewAbortModalForIndexIIPopup").modal('show');

    LoadIndexIIDatatable(OrderID)

    //unBlockUI();
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

function LoadIndexIIDatatable(OrderID) {

    //blockUI('Loading Index II Details... Please wait...');



    $('#IndexIITableID').DataTable({
        ajax: {
            url: '/DataEntryCorrection/DataEntryCorrection/LoadIndexIIDetails',
            type: "POST",
            headers: header,
            data: {
                'OrderID': OrderID
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {

                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',

                    });
                    return;
                }
                else {
                }
                //unBlockUI();
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
            }
        },
        serverSide: true,
        scrollCollapse: true,
        bPaginate: false,
        bLengthChange: false,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: false,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [
            //{ "className": "dt-center", "targets": "_all" },
            { "className": "dt-center", "targets": [0, 2, 3, 4, 5, 6, 7, 8] },
            { "className": "dt-left", "targets": [1] },
            { "orderable": false, "targets": "_all" },
            { "bSortable": false, "aTargets": "_all" },

        ],
        columns: [
            { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
            { data: "Description", "searchable": true, "visible": true, "name": "Description" },
            { data: "Stamp5Datetime", "searchable": true, "visible": true, "name": "Stamp5Datetime" },
            { data: "ArticleNameE", "searchable": true, "visible": true, "name": "ArticleNameE" },
            { data: "Executant", "searchable": true, "visible": true, "name": "Executant" },
            { data: "Claimant", "searchable": true, "visible": true, "name": "Claimant" },
            { data: "CDNumber", "searchable": true, "visible": true, "name": "CDNumber" },
            { data: "PageCount", "searchable": true, "visible": true, "name": "PageCount" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },

        ],
        fnInitComplete: function (oSettings, json) {

            //var form = $('#PropertyNumberDetailsForm');
            //form.append('<input type="hidden" id="PNDDocumentID" name="PNDDocumentID" value ="' + DocumentID + '" />');
            //form.append('<input type="hidden" id="PNDPropertyID" name="PNDPropertyID" value ="' + PropertyID + '" />');
            $("#IndexIIDetailsPopUp").html("Index II Entry for Document No: " + json.finalRegistrationNo + " of " + json.SroName + " SRO");
            $("#ExcelIndexIISPANID").html('');
            $("#ExcelIndexIISPANID").html("<button class='btn btn-success' name='btnExcelIndexII' id='btnExcelOrderIndexII' OnClick='ExcelIndexIIDetails(" + OrderID + ")'>Download as Excel</button>");
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
        },
        drawCallback: function (oSettings) {
            unBlockUI();
        },
    });
}

function SearchOrder() {
    $("#DROrderTableDiv").show();
    DocDetailsTable();
}

function ScrollToBottom() {
    //alert("Height : " + $('#LoadDECInsertUpdateDeleteViewDivID')[0].scrollHeight);
    $('#LoadDECInsertUpdateDeleteViewDivID').scrollTop($('#LoadDECInsertUpdateDeleteViewDivID')[0].scrollHeight);

    //$('#LoadDECInsertUpdateDeleteViewDivID').scrollTo({ left: 0, top: $('#LoadDECInsertUpdateDeleteViewDivID')[0].scrollHeight, behavior: "smooth" });

}

function ExcelOrderDetails() {
    window.location.href = '/DataEntryCorrection/DataEntryCorrection/ExportOrderDetailsToExcel?SroID=' + $("#SROOfficeOrderListID").val() + "&DistrictID=" + $("#DROOfficeOrderListID").val() + "&DroName=" + $("#DROOfficeOrderListID option:selected").text() + "&SroName=" + $("#SROOfficeOrderListID option:selected").text();

}

function ExcelIndexIIDetails(OrderID) {
    window.location.href = '/DataEntryCorrection/DataEntryCorrection/ExportindexIIDetailsToExcel?OrderID=' + OrderID;

}

function OpenDocumentTab(OrderID) {
    var TableName = "SelectDocument";
    //Changed by mayank on 05/12/2021 for DEC changes
    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        $('#DtlsToggleIconSearchPara').trigger('click');

                            $.ajax({
                                url: '/DataEntryCorrection/DataEntryCorrection/LoadInsertUpdateDeleteView',
                                headers: header,
                                type: "POST",
                                data: {
                                    'TableViewName': TableName,
                                    'OrderID': OrderID,
                                    //'DocumentID': DocumentIDVar,
                                    //'SROCode': SROCodeVar,
                                },
                                success: function (data) {
                                    //alert("great success");

                                    //console.log(data);
                                    // REMOVE PREVIOUS ADDED ACTIVE CLASS
                                    $('#panelAllTabs').show();
                                    $('#AddEditOrderTabDivID').removeClass('active');
                                    $('#SelectDocumentTabDivID').removeClass('active');
                                    //$('#PropertyNumberDetailsTabDivID').removeClass('active');  //Not in use 
                                    //$('#PartyDetailsMenuTabDivID').removeClass('active');

                                    // ADD ACTIVE CLASS By Table Name
                                    if (TableName == "AddEditOrder") {

                                        // alert("In Order (AddEditOrder Tab)");

                                        $('#AddEditOrderTabDivID').addClass('active');
                                    }
                                    else if (TableName == "SelectDocument") {
                                        //alert("In (Select Document Tab)");

                                        $('#SelectDocumentTabDivID').addClass('active');
                                    }
                                    //Not in use now
                                    //else if (TableName == "PropertyNumberDetails") {
                                    //    $('#PropertyNumberDetailsTabDivID').addClass('active');
                                    //}
                                    //else if (TableName == "PartyDetails") {
                                    //    $('#PartyDetailsMenuTabDivID').addClass('active');
                                    //}

                                    $('#LoadDECInsertUpdateDeleteViewDivID').html('');
                                    $('#LoadDECInsertUpdateDeleteViewDivID').html(data);

                                    unBlockUI();
                                },
                                error: function (xhr) {

                                    bootbox.alert({
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                        callback: function () {
                                        }
                                    });
                                    unBlockUI();
                                },
                                beforeSend: function () {
                                    blockUI('Loading Data please wait');
                                }
                            });
}