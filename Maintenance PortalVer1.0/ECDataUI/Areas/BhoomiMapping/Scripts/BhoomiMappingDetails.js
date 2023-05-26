//const { Alert } = require("bootstrap");

var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;



    $("#btnDownload").click(function (e) {

    //    var loc = window.location.pathname;
    //    e.preventDefault();
    //    blockUI('loading data.. please wait...');
    //    window.location.href = $('#btnDownload').attr("data-src");
    //    bootbox.alert('<i class="fa fa-check boot-icon boot-icon" style="color:green;"></i><span class="boot-alert-txt">Downloaded Succesfully!</span>');
    //    unBlockUI();
        $.ajax({
            url: '/BhoomiMapping/BhoomiMappingDetails/DeleteReport',
            data: { "FileName": "ExcelReport" },
            datatype: "text",
            type: "GET",
            success: function (data) {
                if (data == "Successful")
                    console.log("Memory_freed_Succesfully.");
                else
                    console.log("Memory_freed_failure.");
                },
            error: function (xhr) {
                console.log("Memory_freed_failure.");
            }
        });

    });




});

function DownloadLink() {
    var d = $('#MappingDetailsTable').DataTable().row(':eq(0)').data();
    var name = "/Content/BhoomiUploads/" + d.FileName + ".xlsx";
    $('#btnDownload').attr("href", name);
    document.getElementById("btnDownload").type = "application/vnd.ms-excel";
}



function Upload() {
    blockUI("Loading");

    var SRO = $('#SROOfficeOrderListID').val();
    if (SRO == "") {
        bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Please select SRO.</span>');
        unBlockUI();
        return false;
    }

    var ext = $('#excel_input').val().split('.').pop().toLowerCase();
    if (ext !== 'xls' && ext !== 'xlsx') {
        bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Please select excel file.</span>');
        unBlockUI();
        return false;
    }

    var fileUpload = $("#excel_input").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();

    // can be used in case of multiple file uploads
    //for (var i = 0; i < files.length; i++) {
    //    fileData.append(files[i].name, files[i]);
    //}


    fileData.append("ExcelFile", files[0]);
    fileData.append('SROCode', SRO);
    $.ajax({
        url: '/BhoomiMapping/BhoomiMapping/Upload',
        contentType: false,
        processData: false,
        data: fileData,
        type: "POST",
        headers: header,
        success: function (data) {
            unBlockUI();
            if (data == "Row_Exceed_Error") {
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">There are more than 100 rows in the excel sheet, Please check.</span>');
            }
            else if (data == "Col_Mismatch_error") {
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Column Header Name Mismatch, Please Check Excel File.</span>');
            }
            else if (data == "SRO_error") {
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Selected SRO doesn\'t match SRO in Excel File.</span>');
            }
            else if (data == "Empty_Error_Excel") {
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Selected excel file is empty.</span>');
            }
            else if (data == "Null_Value_Error") {
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Selected excel file has some empty/null value. Please Check.</span>');
            }
            else {
                $("#StatusDiv").html(data);
            }
        },
        error: function (xhr) {
            unBlockUI();
            alert(xhr);
        }
    });



}

function DetailsTable() {
    $("#MappingDetailsTable").DataTable().clear().destroy();
    var sro = $("#SROOfficeOrderListID").val();

    if (sro == 0) {
        bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Please select SRO.</span>');
        return false;
    }

    var DetailsTable = $('#MappingDetailsTable').DataTable({
        ajax: {
            url: "/BhoomiMapping/BhoomiMappingDetails/LoadDetailsTable",
            type: "POST",
            headers: header,
            data: {
                "SroCode": sro
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

                $('#btnDownload').css("display", "none");
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
            { "className": "dt-center", "targets": "_all" },
            { "className": "fileLink", "targets": 15 }

        ],


        columns: [
            { data: "SNo", "searchable": true, "visible": true, "name": "SNo" },
            { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName" },
            { data: "KaveriSROCode", "searchable": true, "visible": true, "name": "KaveriSROCode" },
            { data: "KaveriSROName", "searchable": true, "visible": true, "name": "KaveriSROName" },
            { data: "KaveriVillageCode", "searchable": true, "visible": true, "name": "KaveriVillageCode" },
            { data: "KaveriVillageName", "searchable": true, "visible": true, "name": "KaveriVillageName" },
            { data: "KaveriHobiCode", "searchable": true, "visible": true, "name": "KaveriHobiCode" },
            { data: "KaveriHobiName", "searchable": true, "visible": true, "name": "KaveriHobiName" },
            { data: "BhoomiDistrictCode", "searchable": true, "visible": true, "name": "BhoomiDistrictCode" },
            { data: "BhoomiTalukCode", "searchable": true, "visible": true, "name": "BhoomiTalukCode" },
            { data: "BhoomiTalukName", "searchable": true, "visible": true, "name": "BhoomiTalukName" },
            { data: "BhoomiHobiCode", "searchable": true, "visible": true, "name": "BhoomiHobiCode" },
            { data: "BhoomiHobiName", "searchable": true, "visible": true, "name": "BhoomiHobiName" },
            { data: "BhoomiVillageCode", "searchable": true, "visible": true, "name": "BhoomiVillageCode" },
            { data: "BhoomiVillageName", "searchable": true, "visible": true, "name": "BhoomiVillageName" },
            { data: "FileName", "searchable": false, "visible": false, "name": "FileName" },
        ],
        fnInitComplete: function (oSettings, json) {
            if ($('#MappingDetailsTable').DataTable().rows().count() != 0) {
                $('#btnDownload').css("display", "block");
                DownloadLink();
            }
            else {

                $('#btnDownload').css("display", "none");
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

function Show() {
    $("#MappingDetailsTableDIV").show();
    DetailsTable();
}


