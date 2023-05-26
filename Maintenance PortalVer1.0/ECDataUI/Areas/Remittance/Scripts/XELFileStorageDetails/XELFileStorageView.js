var token = '';
var header = {};
var SelectedType = '';
var OfficeCodeSelected = '';
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });
    $("#SearchBtn").click(function () {
        if ($.fn.DataTable.isDataTable("#XELFileListOfficeWiseTableID")) {
            $("#XELFileListOfficeWiseTableID").DataTable().clear().destroy();
        }
        $("#EXCELSPANIDOfficeList").html('');
        $("#EXCELSPANIDFileListOfficeWise").html('');

        SelectedType = $('input[name="OfficeTypeName"]:checked').val();
        //alert("rfgwre_" + SelectedType + "_rfgwre");
        //if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
        //    $("#AnywhereECTable").DataTable().clear().destroy();
        //}
        //if ($.fn.DataTable.isDataTable("#DocScannedAndDeliveryTableID")) {
        //    $("#DocScannedAndDeliveryTableID").DataTable().clear().destroy();
        //    //$("#DocScannedAndDeliveryTableID").hide();
        //}

        if ($.fn.DataTable.isDataTable("#XELFileOfficeListTableID")) {
            $("#XELFileOfficeListTableID").DataTable().clear().destroy();
        }

        if (SelectedType == undefined) {
            bootbox.alert("Please select office type.");
        }
        else {
            var XELFileOfficeListTableID = $('#XELFileOfficeListTableID').DataTable({
                ajax: {
                    url: '/Remittance/XELFileStorageDetails/XELFileOfficeList',
                    type: "POST",
                    headers: header,
                    data: {
                        'OfficeType': SelectedType
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
                                        $("#XELFileOfficeListTableID").DataTable().clear().destroy();
                                        //$("#PDFSPANID").html('');
                                        $("#EXCELSPANIDOfficeList").html('');
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
                        var searchString = $('#XELFileOfficeListTableID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;
                            if (!regexToMatch.test(searchString)) {
                                $("#XELFileOfficeListTableID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    XELFileOfficeListTableID.search('').draw();
                                    $("#XELFileOfficeListTableID_filter input").prop("disabled", false);
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
                "lengthMenu": [[350], ["All"]],
                "bAutoWidth": true,
                "bScrollAutoCss": true,
                columnDefs: [
                    { orderable: false, targets: [0] },
                    { orderable: false, targets: [1] },
                    { orderable: false, targets: [2] },
                    { orderable: false, targets: [3] },
                    { orderable: false, targets: [4] }
                ],
                columns: [
                    { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                    { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
                    { data: "NoOfFiles", "searchable": true, "visible": true, "name": "NoOfFiles" },
                    { data: "TotalSizeOnDiskInMB", "searchable": true, "visible": true, "name": "TotalSizeOnDiskInMB" },
                    { data: "LastCentralizedOn", "searchable": true, "visible": true, "name": "LastCentralizedOn" }

                ],
                fnInitComplete: function (oSettings, json) {
                    //$("#PDFSPANID").html(json.PDFDownloadBtn);
                    $("#EXCELSPANIDOfficeList").html('');
                    $("#EXCELSPANIDOfficeList").html(json.ExcelDownloadBtn);
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


        //$("#AnywhereECTable").hide();
        //$("#AnywhereECTable").show();



        //tableIndexReports.columns.adjust().draw();
        //AnywhereECTable.columns.adjust().draw();


        $.ajax({
            url: '/Remittance/XELFileStorageDetails/RootDirectoryTable',
            data: { "officeType": SelectedType },
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
                    $("#rootDirectoryDivID").html(data);
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });

    });


});

function XELFileListOfficeWise(Srocode) {
    OfficeCodeSelected = Srocode;
    //alert(Srocode);
    //alert(SelectedType);

    if ($.fn.DataTable.isDataTable("#XELFileListOfficeWiseTableID")) {
        $("#XELFileListOfficeWiseTableID").DataTable().clear().destroy();
    }

    //if (Srocode == undefined) {
    //    bootbox.alert("Please select office type.");
    //}
    //else {

    var XELFileListOfficeWiseTableID = $('#XELFileListOfficeWiseTableID').DataTable({
        ajax: {
            url: '/Remittance/XELFileStorageDetails/XELFileListOfficeWise',
            type: "POST",
            headers: header,
            data: {
                'SROCode': Srocode,
                'OfficeType': SelectedType
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
                                $("#XELFileListOfficeWiseTableID").DataTable().clear().destroy();
                                //$("#PDFSPANID").html('');
                                $("#EXCELSPANIDFileListOfficeWise").html('');
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
                var searchString = $('#XELFileListOfficeWiseTableID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#XELFileListOfficeWiseTableID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            XELFileListOfficeWiseTableID.search('').draw();
                            $("#XELFileListOfficeWiseTableID_filter input").prop("disabled", false);
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
            { orderable: false, targets: [7] }
        ],
        columns: [
            { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
            { data: "FileName", "searchable": true, "visible": true, "name": "FileName" },
            { data: "FileSizeInMB", "searchable": true, "visible": true, "name": "FileSizeInMB" },
            { data: "FileDateTime", "searchable": true, "visible": true, "name": "FileDateTime" },
            { data: "FilePath", "searchable": true, "visible": true, "name": "FilePath" },
            { data: "EventStartDate", "searchable": true, "visible": true, "name": "EventStartDate" },
            { data: "EventEndDate", "searchable": true, "visible": true, "name": "EventEndDate" },
            { data: "FileReadDateTime", "searchable": true, "visible": true, "name": "FileReadDateTime" }

        ],
        fnInitComplete: function (oSettings, json) {
            //$("#PDFSPANID").html(json.PDFDownloadBtn);
            $("#EXCELSPANIDFileListOfficeWise").html('');
            $("#EXCELSPANIDFileListOfficeWise").html(json.ExcelDownloadBtn);
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
    //}
}

function XELFileDownloadPathVerify(path) {
    $.ajax({
        url: '/Remittance/XELFileStorageDetails/XELFileDownloadPathVerify',
        data: { "path": path },
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
                if (data.serverError == false && data.success == false) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                }
                else {
                    if (data.IsFileExistAtDownloadPath) {
                        window.location.href = '/Remittance/XELFileStorageDetails/XELFileDownload?path=' + path + '&OfficeType=' + SelectedType+ '&OfficeCodeSelected=' + OfficeCodeSelected;
                    }
                    else {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">File not exist at path.</span>');
                    }
                }
            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });
}

function EXCELDownloadFun(OfficeType) {

    window.location.href = '/Remittance/XELFileStorageDetails/ExportXELFileCountOfficewiseToExcel?OfficeType=' + OfficeType;
}

function EXCELDownloadFunFileList(SROCode, OfficeType) {

    window.location.href = '/Remittance/XELFileStorageDetails/ExportXELFileListOfficewiseToExcel?SROCode=' + SROCode + '&OfficeType=' + OfficeType;
}