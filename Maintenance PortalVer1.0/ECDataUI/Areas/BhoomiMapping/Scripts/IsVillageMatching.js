//const { Alert } = require("bootstrap");

var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    var DetailsTable = $('#MatchingTable').DataTable({
        ajax: {
        url: "/BhoomiMapping/IsVillageMatching/IsVillageMatching",
            type: "POST",
            headers: header,
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
                                $("#MatchingTable").DataTable().clear().destroy();
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
        searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [
            { "className": "dt-center", "targets": "_all" }
        ],


        columns: [
            { data: "SNo", "searchable": true, "visible": true, "name": "SNo" },
            { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
            { data: "IsVillageMatching", "searchable": true, "visible": true, "name": "IsVillageMatching" },

        ],
        fnInitComplete: function (oSettings, json) {

            unBlockUI();
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
});






