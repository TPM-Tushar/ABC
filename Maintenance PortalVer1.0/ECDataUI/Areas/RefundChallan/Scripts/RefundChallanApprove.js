var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    
    $('#RefundChallanDROrderDetailsCollapse').click(function () {

        var classToRemove = $('#RefundChallanApproveToggleIcon').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#RefundChallanApproveToggleIcon').removeClass(classToRemove).addClass(classToSet);
    });

    $('#RefundChallanApprovePanelAllTabs').hide();

    $("#btncloseAbortPopup").click(function () {

        $("#objPDFViewer").attr('data', '');
        $('#divViewAbortModal').css('display', 'none');
        $('#divViewAbortModal').addClass('modal fade');
    });


    $('#DROOfficeOrderListID').change(function () {
        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/RefundChallan/RefundChallanApprove/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROOfficeOrderListID').val() },
            datatype: "json",
            type: "GET",
            success: function (data) {
                unBlockUI();
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    $('#SROOfficeOrderListID').empty();
                    $.each(data.SROOfficeList, function (i, SROOfficeList) {
                        SROOfficeList
                        $('#SROOfficeOrderListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });


    //$('#DROOfficeOrderListID').change(function () {
    //    blockUI('loading data.. please wait...');

    //    alert("District:" + $('#DROOfficeOrderListID').val());


    //    $.ajax({
    //        url: '/RefundChallan/RefundChallanApprove/GetSROOfficeListByDistrictID',
    //        data: { "DistrictID": $('#DROOfficeOrderListID').val() },
    //        datatype: "json",
    //        type: "GET",
    //        success: function (data) {
    //            if (data.serverError == true) {
    //                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
    //                    function () {
    //                        window.location.href = "/Home/HomePage"
    //                    });
    //            }
    //            else {
    //                $('#SROOfficeOrderListID').empty();
    //                alert('data' + $('#SROOfficeOrderListID').val());
    //                $.each(data.SROOfficeList, function (i, SROOfficeList) {
    //                    SROOfficeList
    //                   // alert("value " + SROfficeOrderList.Value + "Text" + SROfficeOrderList.Text);
    //                    $('#SROOfficeOrderListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
    //                });
    //            }
    //            unBlockUI();
    //        },
    //        error: function (xhr) {
    //            unBlockUI();
    //        }
    //    });
    //});
    
    SearchRefundChallanOrder();

});

function SearchRefundChallanOrder() {
    $("#RefundChallanApproveTableDiv").show();
    RefundChallanApproveTable();

}

function RefundChallanApproveTable() {

    //alert('DroCode: ' + $("#DROOfficeOrderListID").val());
    blockUI('loading data.. please wait...');
    var DetailsTable = $('#RefundChallanApproveDtlsTable').DataTable({
        ajax: {
            url: '/RefundChallan/RefundChallanApprove/LoadRefundChallanApproveTable',
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
                                var classToRemove = $('#RefundChallanApproveToggleIcon').attr('class');
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
                    unBlockUI();
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
            //  { "className": "dt-center ShorterTextClass", "targets": [4] },
            { "className": "dt-center", "targets": "_all" },
            { "orderable": false, "targets": "_all" },
            { "bSortable": false, "aTargets": "_all" },

            { "width": "2px", "targets": [0] },
            { "width": "70px", "targets": [1] },
            { "width": "70px", "targets": [2] },
            { "width": "70px", "targets": [3] },
            { "width": "82px", "targets": [4] },
            { "width": "85px", "targets": [5] },
            { "width": "75px", "targets": [6] },
            { "width": "95px", "targets": [7] },
            { "width": "90px", "targets": [8] },
            { "width": "90px", "targets": [9] },
            { "width": "120px", "targets": [10] },
            { "width": "80px", "targets": [11] },
            { "width": "15px", "targets": [12] },

        ],

        columns: [
            { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
            { data: "DROName", "searchable": true, "visible": true, "name": "DROName" },
            { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
            { data: "DROrderNumber", "searchable": true, "visible": true, "name": "DROrderNumber" },
            { data: "DROrderDate", "searchable": true, "visible": true, "name": "DROrderDate" },
            { data: "InstrumentNumber", "searchable": true, "visible": true, "name": "InstrumentNumber" },
            { data: "InstrumentDate", "searchable": true, "visible": true, "name": "InstrumentDate" },
            { data: "ChallanAmount", "searchable": true, "visible": true, "name": "ChallanAmount" },
            { data: "RefundAmount", "searchable": true, "visible": true, "name": "RefundAmount" },
            { data: "PartyName", "searchable": true, "visible": true, "name": "PartyName" },
            { data: "PartyMobileNumber", "searchable": true, "visible": true, "name": "PartyMobileNumber" },
            { data: "ViewDROrder", "searchable": true, "visible": true, "name": "ViewDROrder" },
            { data: "Action", "searchable": true, "visible": true, "name": "Action" },
            { data: "DRApprovalStatus", "searchable": true, "visible": true, "name": "DRApprovalStatus" },

        ],
        fnInitComplete: function (oSettings, json) {
            //if (json.isDrLogin) {
            //    $("#AddOrderSPANID").show();
            //}
            //else {
            //    $("#AddOrderSPANID").hide();
            //}
            //$("#AddOrderSPANID").show();
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
    
function RefundChallanApproveAddEditOrder(RowId) {

    blockUI('loading data.. please wait...');

    $.ajax({
        url: '/RefundChallan/RefundChallanApprove/LoadAddNewRefundChallanApproveView',
        headers: header,
        data: { RowId },
        datatype: "json",
        type: "POST",
        success: function (data) {

            if (data.serverError == true) {

                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {

                $('#RefundChallanApprovePanelAllTabs').show();

                $('#RefundChallanApproveSaveViewDivID').html('');

                $('#RefundChallanApproveSaveViewDivID').html(data);

                $('#RefundChallanDROrderDetailsCollapse').trigger('click');

            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });

}

function ExcelRefundChallanOrderDetails() {

    window.location.href = '/RefundChallan/RefundChallanApprove/ExportRefundChallanOrderDetailsToExcel?SroID=' + $("#SROOfficeOrderListID").val() + "&DistrictID=" + $("#DROOfficeOrderListID").val() + "&DroName=" + $("#DROOfficeOrderListID option:selected").text() + "&SroName=" + $("#SROOfficeOrderListID option:selected").text();

    
}

function ViewBtnClickOrderTable(RowId) {
    blockUI();
    $('#divViewAbortModal').css('display', 'block');
    $('#divViewAbortModal').addClass('modal fade in');
    $("#objPDFViewer").attr('data', '');
    $("#objPDFViewer").attr('data', '/RefundChallan/RefundChallanApprove/ViewBtnClickOrderTable?RowId=' + RowId);
    unBlockUI();
}


