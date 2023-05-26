
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

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var TodaysDate = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();
    var FromDate;
    var ToDate;
    var SROOfficeID;
    var DROfficeID;
    var ModuleID;

    $('#txtFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,

    });

    $('#txtToDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

    });

    $('#divFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),

        pickerPosition: "bottom-left"

    });

    $('#divToDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });

    $('#txtToDate').datepicker({
        format: 'dd/mm/yyyy',
    }).datepicker("setDate", TodaysDate);


    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", TodaysDate);


    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/MISReports/ReScanningDetails/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROOfficeListID').val() },
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

    $("#SearchBtn").click(function () {

        if ($.fn.DataTable.isDataTable("#ReScanningTableID")) {
            $("#ReScanningTableID").DataTable().clear().destroy();
        }

        if ($.fn.DataTable.isDataTable("#ReScanningTableID")) {
            $("#ReScanningTableID").DataTable().clear().destroy();
        }
        FromDate = $("#txtFromDate").val();
        ToDate = $("#txtToDate").val();
        SROOfficeID = $("#SROOfficeListID option:selected").val();
        DROfficeID = $("#DROOfficeListID option:selected").val();
        ModuleID = $("#ModuleNameListID option:selected").val();

        var ReScanningDetailsTable = $('#ReScanningDtlsID').DataTable({
            ajax: {
                url: '/MISReports/ReScanningDetails/LoadReScanningDetailsTable',
                type: "POST",
                headers: header,
                data: {
                    'fromDate': FromDate, 'ToDate': ToDate, 'SROOfficeID': SROOfficeID, 'DROfficeID': DROfficeID, 'ModuleID': ModuleID
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
                                    $("#ReScanningTableID").DataTable().clear().destroy();
                                    $("#PDFSPANID").html('');
                                    $("#EXCELSPANID").html('');
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
                    unBlockUI();
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                },
                beforeSend: function () {
                    blockUI('loading data.. please wait...');
                    var searchString = $('#ReScanningTableID_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;

                        if (!regexToMatch.test(searchString)) {
                            $("#ReScanningTableID_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                ReScanningDetailsTable.search('').draw();
                                $("#ReScanningTableID_filter input").prop("disabled", false);
                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
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
                //{ orderable: false, targets: [1] },
                //{ orderable: false, targets: [2] },
                //{ orderable: false, targets: [3] },
                //{ orderable: false, targets: [4] }
            ],

            columns: [
                {
                    data: "DocumentNumber", "searchable": true, "visible": true, "name": "DocumentNumber"
                },
                {
                    data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber"
                },
                //{
                //    data: "MarriageCaseNo", "searchable": true, "visible": true, "name": "MarriageCaseNo"
                //},
                {
                    data: "SROOffice", "searchable": true, "visible": true, "name": "SROOffice"
                },
                {
                    data: "RescanEnableDateTime", "searchable": true, "visible": true, "name": "RescanEnableDateTime"
                },
                {
                    data: "IsFileUploaded", "searchable": true, "visible": true, "name": "IsFileUploaded",
                    "render": function (data, type, row) {
                        return (data == true) ? '<span class="glyphicon glyphicon-ok" style="color:green"> </span > ' : '<span class="glyphicon glyphicon-remove" style="color:red"></span>';
                    }
                }
            ],
            fnInitComplete: function (oSettings, json) {
                $("#PDFSPANID").html(json.PDFDownloadBtn);
                $("#EXCELSPANID").html(json.ExcelDownloadBtn);
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
    });

});



function PDFDownloadFun(FromDate, ToDate, SROOfficeID, DROfficeID, ModuleID) {

    window.location.href = '/MISReports/ReScanningDetails/ExportReScanningDetailsToPDF?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeID + "&DROfficeID=" + DROfficeID + "&ModuleID=" + ModuleID;
}


function EXCELDownloadFun(FromDate, ToDate, SROOfficeID, DROfficeID, ModuleID) {

    window.location.href = '/MISReports/ReScanningDetails/ExportReScanningDetailsToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SROOfficeListID=" + SROOfficeID + "&DROfficeID=" + DROfficeID + "&ModuleID=" + ModuleID;

}


