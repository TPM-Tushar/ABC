//const { Alert } = require("bootstrap");

var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

   

    $('#DROOfficeOrderListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/Blockchain/Blockchain/GetSroCodebyDistrict',
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



});


function DetailsTable() {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $("#ApprovalListTable").DataTable().clear().destroy();
    var dro = $("#DROOfficeOrderListID").val();
    var sro = $("#SROOfficeOrderListID").val();

    var DetailsTable = $('#ApprovalListTable').DataTable({
        ajax: {
            url: "/Blockchain/Blockchain/LoadDetailsTable",
            type: "POST",
            headers: header,
            data: {
                "DroCode": dro, "SroCode": sro
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
                                $("#ApprovalListTable").DataTable().clear().destroy();
                            }
                        }
                    });
                    return;
                }
                else {
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
        searching: false,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [
            { "className": "dt-center", "targets": "_all" },
            { "visible": false, "searchable": false, "targets": "[7]" },
            { "visible": false, "searchable": false, "targets": "[8]" }

        ],

        columns: [
            { data: "SNo", "searchable": true, "visible": true, "name": "SNo" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
            { data: "Stamp5DateTime", "searchable": true, "visible": true, "name": "Stamp5DateTime" },
            { data: "RequestDate", "searchable": true, "visible": true, "name": "RequestDate" },
            { data: "NatureOfDocument", "searchable": true, "visible": true, "name": "NatureOfDocument" },
            { data: "ReasonDesc", "searchable": true, "visible": true, "name": "ReasonDesc" },
            { data: "Action", "searchable": true, "visible": true, "name": "Action" },
            { data: "DocumentID", "searchable": false, "visible": false, "name": "DocumentID" },
            { data: "SROCode", "searchable": false, "visible": false, "name": "SROCode" },
        ],
        fnInitComplete: function (oSettings, json) {
            //Added by madhur on 22-10-2021
            if ($('#ApprovalListTable').DataTable().page.info().recordsDisplay <= 0) {
                $('#btnApproval').css('display', 'none');
            }
            else {
                $('#btnApproval').css('display', 'inline-block');
            }

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

function View() {
    $("#ApprovalTableDIV").show();
    DetailsTable();
}

function Approval() {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    var appList = [];
    $("input:checkbox[name=chkbxBCA]:checked").each(function () {
        appList.push($(this).val());
    });
    //Added by madhur on 22-10-2021
    if (appList.length <= 0) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select some rows before approval.</span>');
    }
    else {
        $.ajax({
            url: '/Blockchain/Blockchain/Approval',
            data: { "appList": appList },
            type: "POST",
            headers: header,
            success: function (data) {
                bootbox.alert('<i class="fa fa-check boot-icon boot-icon"></i><span class="boot-alert-txt">' + data + '</span>');
                unBlockUI();
                DetailsTable();
            },
            error: function (xhr) {
                alert(xhr);
                unBlockUI();
            }
        });
    }
}