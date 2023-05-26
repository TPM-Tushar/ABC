//const { Alert } = require("bootstrap");

var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $('#DROfficeListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/PhotoThumb/PhotoThumb/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROfficeListID').val() },
            type: "GET",
            success: function (data) {
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    $('#SROfficeListID').empty();
                    $.each(data.SROfficeList, function (i, SROfficeList) {
                        SROfficeList
                        $('#SROfficeListID').append('<option value="' + SROfficeList.Value + '">' + SROfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });

    //$("#btnDownload").click(function (e) {

    ////    var loc = window.location.pathname;
    ////    e.preventDefault();
    ////    blockUI('loading data.. please wait...');
    ////    window.location.href = $('#btnDownload').attr("data-src");
    ////    bootbox.alert('<i class="fa fa-check boot-icon boot-icon" style="color:green;"></i><span class="boot-alert-txt">Downloaded Succesfully!</span>');
    ////    unBlockUI();
    //    $.ajax({
    //        url: '/BhoomiMapping/BhoomiMappingDetails/DeleteReport',
    //        data: { "FileName": "ExcelReport" },
    //        datatype: "text",
    //        type: "GET",
    //        success: function (data) {
    //            if (data == "Successful")
    //                console.log("Memory_freed_Succesfully.");
    //            else
    //                console.log("Memory_freed_failure.");
    //            },
    //        error: function (xhr) {
    //            console.log("Memory_freed_failure.");
    //        }
    //    });

    //});




});





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
        url: '/PhotoThumb/PhotoThumb/Upload',
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

function DetailsTable(SROfficeID, DROfficeID, DocumentNumber, BookTypeID, FinancialYear) {
    $("#PhotoThumbTable").DataTable().clear().destroy();

    var DetailsTable = $('#PhotoThumbTable').DataTable({
        ajax: {
            url: "/PhotoThumb/PhotoThumb/PhotoThumbAvailaibilityTable",
            type: "POST",
            headers: header,
            data: {
                "SROfficeID": SROfficeID, "DROfficeID": DROfficeID, "DocumentNumber": DocumentNumber, "BookTypeID": BookTypeID, "FinancialYear": FinancialYear
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
                                $("#PhotoThumbTable").DataTable().clear().destroy();
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

                //$('#btnDownload').css("display", "none");
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
        ],


        columns: [
            { data: "SNo", "searchable": true, "visible": true, "name": "SNo" },
            { data: "PartyID", "searchable": true, "visible": false, "name": "PartyID" },
            { data: "DocumentID", "searchable": true, "visible": false, "name": "DocumentID" },
            { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
            { data: "Fname", "searchable": true, "visible": true, "name": "Fname" },
            { data: "Lname", "searchable": true, "visible": true, "name": "Lname" },
            { data: "Photo", "searchable": true, "visible": true, "name": "Photo" },
            { data: "Thumb", "searchable": true, "visible": true, "name": "Thumb" },
        ],
        fnInitComplete: function (oSettings, json) {
            //if ($('#PhotoThumbTable').DataTable().rows().count() != 0) {
            //    $('#btnDownload').css("display", "block");
            //}
            //else {

                //$('#btnDownload').css("display", "none");
            //}


            $("tr td:nth-child(5)").addClass('img-center');
            $("tr td:nth-child(6)").addClass('img-center');
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
    var fileDownload = false;
    SROfficeID = $("#SROfficeListID option:selected").val();
    DROfficeID = $("#DROfficeListID option:selected").val();
    DocumentNumber = $("#txtDocumentNumber").val();
    BookTypeID = $("#BookTypeListID option:selected").val();
    FinancialYear = $("#FinancialYearListID option:selected").val();
    //var regexToMatch = /^[^<>]+$/;
    var regexToMatchDocumentNumber = new RegExp('^[0-9]*$');



    //Validation
    if (SROfficeID == 0 || DROfficeID == 0) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Office Details</span>'
        });
        return;
    }

    else if (DocumentNumber == "" || DocumentNumber == 0) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Document Number</span>'
        });
        return;
    }

    else if (!regexToMatchDocumentNumber.test(DocumentNumber)) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Document Number</span>'
        });
        return;
    }

    else if (BookTypeID == 0) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Book Type</span>'
        });
        return;
    }

    else if (FinancialYear == "" || FinancialYear == "Select") {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Financial Year</span>'
        });
        return;
    }

    $("#PhotoThumbDIV").show();
    DetailsTable(SROfficeID, DROfficeID, DocumentNumber, BookTypeID, FinancialYear);
}


