var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;



    //$('#iconSideBar').trigger('click');

    // $('#divLoadAbortViewForPropertyPopup').hide();

    $('#SRAndDRDropdown').hide();

    $('#RefundChallanDetailsCollapse').click(function () {
        
        var classToRemove = $('#RefundChallanToggleIcon').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#RefundChallanToggleIcon').removeClass(classToRemove).addClass(classToSet);
    });
    
    $('#RefundChallanEntryPanelAllTabs').hide();
    
    $("#btncloseAbortPopup").click(function () {

        $("#objPDFViewer").attr('data', '');
        $('#divViewAbortModal').css('display', 'none');
        $('#divViewAbortModal').addClass('modal fade');
    });


    $('#btnAddNewRefundChallanDetails').click(function () {
    });

    //$('#DROOfficeListID').change(function () {
    //    blockUI('loading data.. please wait...');

    //    $.ajax({
    //        url: '/RefundChallan/RefundChallanApprove/GetSROOfficeListByDistrictID',
    //        data: { "DistrictID": $('#DROOfficeListID').val() },
    //        datatype: "json",
    //        type: "GET",
    //        success: function (data) {
    //            unBlockUI();
    //            if (data.serverError == true) {
    //                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
    //                    function () {
    //                        window.location.href = "/Home/HomePage"
    //                    });
    //            }
    //            else {
    //                $('#SROOfficeListID').empty();
    //                $.each(data.SROOfficeList, function (i, SROOfficeList) {
    //                    SROOfficeList
    //                    $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
    //                });
    //            }
    //            unBlockUI();
    //        },
    //        error: function (xhr) {
    //            unBlockUI();
    //        }
    //    });
    //});

    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/RefundChallan/RefundChallan/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROOfficeListID').val() },
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




    SearchRefundChallanEntry();
    
});

function SearchRefundChallanEntry()
{
    $("#RefundChallanTableDiv").show();
    RefundChallanDetailsTable();
}

function RefundChallanDetailsTable() {

    //alert("DrO : "+ $("#DROOfficeListID").val());
    blockUI('loading data.. please wait...');

    var DetailsTable = $('#RefundChallanDtlsTable').DataTable({
        ajax: {
            url: '/RefundChallan/RefundChallan/LoadRefundChallanDetailsTable',
            type: "POST",
            headers: header,
            data: {
                //'DroCode': $("#idDROCode").val(), 'SroCode': $("#idSROCode").val(),
                //'DroCode': $("#DROOfficeOrderListID").val(), 'SroCode': $("#SROOfficeOrderListID").val(),
                'DroCode': $("#DROOfficeListID").val(), 'SroCode': $("#SROOfficeListID").val(),

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
                                var classToRemove = $('#RefundChallanToggleIcon').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#DetailTableID").DataTable().clear().destroy();
                               
                            }
                        }
                    });
                    return;
                }
                else
                {
                    var classToRemove = $('#DtlsToggleIconSearchParaDetail').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsToggleIconSearchParaDetail').trigger('click');
                    unBlockUI();
                }
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
            { "className": "dt-center", "targets": "_all" },
            { "orderable": false, "targets": "_all" },
            { "bSortable": false, "aTargets": "_all" },

            { "width": "2px", "targets": [0] },
            { "width": "64px", "targets": [1] },
            { "width": "61px", "targets": [2] },
            { "width": "70px", "targets": [3] },
            { "width": "82px", "targets": [4] },
            { "width": "80px", "targets": [5] },
            { "width": "75px", "targets": [6] },
            { "width": "90px", "targets": [7] },
            { "width": "85px", "targets": [8] },
            { "width": "90px", "targets": [9] },
            { "width": "90px", "targets": [10] },
            { "width": "70px", "targets": [11] },
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
            { data: "ChallanEntryStatus", "searchable": true, "visible": true, "name": "ChallanEntryStatus" },
            { data: "DRApprovalStatus", "searchable": true, "visible": true, "name": "DRApprovalStatus" },

        ],
        fnInitComplete: function (oSettings, json) {
            //if (json.isDrLogin) {
            //    $("#AddRefundChallanSPANID").show();
            //}
            //else {
            //    $("#AddRefundChallanSPANID").hide();
            //}
            //$("#AddRefundChallanSPANID").show();

            //alert("IsSROrDRLogin : " + json.IsSROrDRLogin)
            //$("#IsSROrDRLogin").html(json.IsSROrDRLogin);

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

function RefundChallanAddEditDetails(RowId) {


    blockUI('loading data.. please wait...');
    $.ajax({
        url: '/RefundChallan/RefundChallan/LoadAddNewRefundChallanView',
        headers: header,
        data: { RowId },
        datatype: "json",
        type: "POST",
        success: function (data) {
            
            if (data.serverError == true) {

                unBlockUI();

                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });

            }
            else {

                $('#RefundChallanEntryPanelAllTabs').show();

                $('#LoadRefundChallanEntryUpdateViewDivID').html('');

                $('#LoadRefundChallanEntryUpdateViewDivID').html(data);
                
                $('#RefundChallanDetailsCollapse').trigger('click');
              
            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });

}

function ExcelRefundChallanDetails() {
    //alert('SROOfficeListID' + $('#SROOfficeListID').val());
    //alert('dROOfficeListID' + $('#DROOfficeListID').val());
    //alert('DROOfficeListID' + $("#DROOfficeListID option:selected").text());
    //alert('SROOfficeListID' + $("#SROOfficeListID option:selected").text());

    //window.location.href = '/RefundChallan/RefundChallan/ExportRefundChallanOrderDetailsToExcel?SroID=' + $("#idSROCode").val() + "&DistrictID=" + $("#idDROCode").val() + "&DroName=" + $("#idDROName").val() + "&SroName=" + $("#idSROName").val();
    window.location.href = '/RefundChallan/RefundChallan/ExportRefundChallanOrderDetailsToExcel?SroID=' + $("#SROOfficeListID").val() + "&DistrictID=" + $("#DROOfficeListID").val() + "&DroName=" + $("#DROOfficeListID option:selected").text() + "&SroName=" + $("#SROOfficeListID option:selected").text();
    //window.location.href = '/DataEntryCorrection/DataEntryCorrection/ExportOrderDetailsToExcel?SroID=' + $("#SROOfficeOrderListID").val() + "&DistrictID=" + $("#DROOfficeOrderListID").val() + "&DroName=" + $("#DROOfficeOrderListID option:selected").text() + "&SroName=" + $("#SROOfficeOrderListID option:selected").text();
    
}

function ViewBtnClickOrderTable(RowId) {
    blockUI();
    $('#divViewAbortModal').css('display', 'block');
    $('#divViewAbortModal').addClass('modal fade in');
    $("#objPDFViewer").attr('data', '');
    $("#objPDFViewer").attr('data', '/RefundChallan/RefundChallan/ViewBtnClickOrderTable?RowId=' + RowId);
    unBlockUI();
}



