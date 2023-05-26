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

    $('#DROfficeListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/PhotoThumb/PhotoThumbFailed/GetSROOfficeListByDistrictID',
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



    $("#btncloseAbortPopup").click(function () {
        $('#ErrorDetailsModal').modal('hide');
    });


});




function DetailsTable(SROfficeID) {
    $("#PhotoThumbFailedTable").DataTable().clear();
    blockUI('loading data.. please wait...');
    var DetailsTable = $('#PhotoThumbFailedTable').DataTable({
        ajax: {
            url: "/PhotoThumb/PhotoThumbFailed/PhotoThumbFailed",
            type: "POST",
            headers: header,
            data: {
                "SROfficeID": SROfficeID
            },
            dataSrc: function (json) {
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
                                $("#PhotoThumbFailedTable").DataTable().clear().destroy();
                            }
                        }
                    });
                    return;
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
            { data: "FRN", "searchable": true, "visible": true, "name": "FRN" },
            { data: "PartyName", "searchable": true, "visible": true, "name": "PartyName" },
            { data: "Type", "searchable": true, "visible": true, "name": "Type" },
            { data: "Action", "searchable": true, "visible": true, "name": "Action" },
            { data: "FileName", "searchable": false, "visible": false, "name": "FileName" },
        ],
        fnInitComplete: function (oSettings, json) {
            if ($('#PhotoThumbFailedTable').DataTable().rows().count() != 0) {
                $('#btnDownload').css("display", "block");
                DownloadLink();
            }
            else {
            
                $('#btnDownload').css("display", "none");
            }

            unBlockUI();

            //$("tr td:nth-child(5)").addClass('img-center');
            //$("tr td:nth-child(6)").addClass('img-center');
        },
        preDrawCallback: function () {
            //unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {

            //unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
            //unBlockUI();
        },
    });
    unBlockUI();


}

function DownloadLink() {
    var d = $('#PhotoThumbFailedTable').DataTable().row(':eq(0)').data();
    var name = "/Content/TempPhotoThumbFiles/" + d.FileName + ".xlsx";
    $('#btnDownload').attr("href", name);
    document.getElementById("btnDownload").type = "application/vnd.ms-excel";
}

function Detail(PartyID, SROCode, IsPhoto, IsThumb) {
    blockUI('loading data.. please wait...');

    $("#PhotoThumbFailedDetails").DataTable().clear().destroy();
    var DetailTable = $('#PhotoThumbFailedDetails').DataTable({
        ajax: {
            url: "/PhotoThumb/PhotoThumbFailed/PhotoThumbFailedDetail",
            type: "POST",
            headers: header,
            data: {
                "PartyID": PartyID, "SROCode": SROCode, "IsPhoto": IsPhoto, "IsThumb": IsThumb
            },
            dataSrc: function (json) {
                //unBlockUI();
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
                                $("#PhotoThumbFailedDetails").DataTable().clear().destroy();
                            }
                        }
                    });
                    return;
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
        searching: false,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [
            { "className": "dt-center", "targets": "_all" },
        ],


        columns: [
            { data: "SNo", "searchable": true, "visible": true, "name": "SNo" },
            { data: "CDNumber", "searchable": true, "visible": true, "name": "CDNumber" },
            { data: "Date", "searchable": true, "visible": true, "name": "Date" },
            { data: "Error", "searchable": true, "visible": true, "name": "Error" },
        ],
        fnInitComplete: function (oSettings, json)
        {
            $('#ErrorDetailsModal').modal('show');

            unBlockUI();


            //$("tr td:nth-child(5)").addClass('img-center');
            //$("tr td:nth-child(6)").addClass('img-center');
        },
        preDrawCallback: function () {
           // unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {

          //  unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
          //  unBlockUI();
        },
    });

}

function Show() {
    var fileDownload = false;
    SROfficeID = $("#SROfficeListID option:selected").val();
    DROfficeID = $("#DROfficeListID option:selected").val();


    //Validation
    if (SROfficeID == 0 || DROfficeID == 0) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Office Details</span>'
        });
        return;
    }

  
    DetailsTable(SROfficeID);
}


