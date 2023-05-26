var FromDate;
var ToDate;

//Global variables.
var token = '';
var header = {};
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    var SROOfficeListID;  
  
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var TodaysDate = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();

    //$('#DROOfficeListID').focus();
    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });

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


    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    var SROOfficeListID = $("#SROOfficeListID option:selected").val();
    var IntegrationtypeID = $("#IntegrationtypeID option:selected").val();

    $("#SearchBtn").click(function () {
        if ($.fn.DataTable.isDataTable("#IndexIIReportsID")) {
            $("#IndexIIReportsID").DataTable().clear().destroy();
        }

        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        IntegrationtypeID = $("#IntegrationtypeID option:selected").val();
        FromDate = $("#txtFromDate").val();
        ToDate = $("#txtToDate").val();

        if ($('#SROOfficeListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Select Any SRO.</span>');
            return;
        }
        else if ($('#IntegrationtypeID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Select Any Integration type.</span>');
            return;
        }
        //alert(SROOfficeListID + "-" + IntegrationtypeID + "-" + FromDate + "-" + ToDate);

        var tableIndexReports = $('#IndexIIReportsID').DataTable({
            ajax: {
                url: '/MISReports/OtherDepartmentStatus/OtherDepartmentStatusDetails',
                type: "POST",
                headers: header,
                data: {
                    'SROOfficeListID': SROOfficeListID, 'IntegrationtypeID': IntegrationtypeID, 'FromDate': FromDate, 'ToDate': ToDate,
                },
                dataSrc: function (json) {
                    unBlockUI();
                    unBlockUI();
                    if (json.errorMessage != null) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            callback: function () {
                                window.location.href = "/Home/HomePage"
                            }
                        });
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
                    //$.unblockUI();
                    unBlockUI();
                },
                beforeSend: function () {
                    blockUI('loading data.. please wait...');
                    // Added by SB on 22-3-2019 at 11:06 am
                    var searchString = $('#IndexIIReportsID_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#IndexIIReportsID_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                //    $('#menuDetailsListTable_filter input').val('');
                                tableIndexReports.search('').draw();
                                $("#IndexIIReportsID_filter input").prop("disabled", false);
                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
            },
            serverSide: true,
            // pageLength: 100,
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
                { data: "Column1", "searchable": true, "visible": true, "name": "Column1" },
                { data: "Column2", "searchable": true, "visible": true, "name": "Column2" },
                { data: "Column3", "searchable": true, "visible": true, "name": "Column3" },
                { data: "Column4", "searchable": true, "visible": true, "name": "Column4" },
                { data: "Column5", "searchable": true, "visible": true, "name": "Column5" },
                { data: "Column6", "searchable": true, "visible": true, "name": "Column6" },
                { data: "Column7", "searchable": true, "visible": true, "name": "Column7" }
            ],
            fnInitComplete: function (oSettings, json) {
                //alert('in fnInitComplete');
                $("#PDFSPANID").html(json.PDFDownloadBtn);
                $("#EXCELSPANID").html(json.ExcelDownloadBtn);
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

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');
    });

});

function PDFDownloadFun(SROOfficeListID, IntegrationtypeID, FromDate, ToDate) {
    window.location.href = '/MISReports/OtherDepartmentStatus/ExportReportToPDF?SROOfficeListID=' + SROOfficeListID + "&IntegrationtypeID=" + IntegrationtypeID + "&FromDate=" + FromDate + "&ToDate=" + ToDate;
}

function EXCELDownloadFun(SROOfficeListID, IntegrationtypeID, FromDate, ToDate) {
    window.location.href = '/MISReports/OtherDepartmentStatus/ExportToExcel?SROOfficeListID=' + SROOfficeListID + "&IntegrationtypeID=" + IntegrationtypeID + "&FromDate=" + FromDate + "&ToDate=" + ToDate;
}