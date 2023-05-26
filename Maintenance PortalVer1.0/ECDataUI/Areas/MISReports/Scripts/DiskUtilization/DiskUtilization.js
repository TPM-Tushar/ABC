var token = '';
var header = {};
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;







    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $('#ServerToggleIconID').trigger('click');


  




});

//function PDFDownloadFun() {
//    window.location.href = '/MISReports/DocumentReferences/ExportAnywhereECRptToPDF?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + SroID + "&DistrictID=" + DistrictID + "&LogTypeID=" + LogTypeID + "&SelectedDist=" + SelectedDistrictText + "&SelectedSRO=" + SelectedSROText + "&SelectedLogType=" + SelectedLogTypeText;

//}


function LoadServerDriveDetails(ServerId) {

    //alert(ServerId)

    var driveInfoTable = $('#driveInfoTable').DataTable({
        ajax: {
            url: '/MISReports/DiskUtilization/DiskUtilizationDetails',
            type: "POST",
            headers: header,
            data: {
                'ServerId': ServerId
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            }
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#driveInfoTable").DataTable().clear().destroy();
                                //$("#PDFSPANID").html('');
                                //$("#EXCELSPANID").html('');
                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
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
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#driveInfoTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#driveInfoTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            driveInfoTable.search('').draw();
                            $("#driveInfoTable_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        "scrollX": true,
        "scrollY": "300px",
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
            { orderable: false, targets: [0] },
            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] },
            { orderable: false, targets: [3] },
            { orderable: false, targets: [4] },
            { orderable: false, targets: [5] },
            { orderable: false, targets: [6] },
            { orderable: false, targets: [7] },
        ],
        columns: [
            { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
            { data: "DriveName", "searchable": true, "visible": true, "name": "DriveName" },
            { data: "DriveType", "searchable": true, "visible": true, "name": "DriveType" },
            { data: "FileSystem", "searchable": true, "visible": true, "name": "FileSystem" },
            { data: "TotalSpace", "searchable": true, "visible": true, "name": "TotalSpace" },
            { data: "UsedSpace", "searchable": true, "visible": false, "name": "UsedSpace" },
            { data: "FreeSpace", "searchable": true, "visible": true, "name": "FreeSpace" },
            { data: "FreeSpacePercentage", "searchable": true, "visible": true, "name": "FreeSpacePercentage" }
        ],
        fnInitComplete: function (oSettings, json) {
            //$("#PDFSPANID").html(json.PDFDownloadBtn);
            //$("#EXCELSPANID").html('');
            //$("#EXCELSPANID").html(json.ExcelDownloadBtn);
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

