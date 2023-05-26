var token = '';
var header = {};
var txtFromDate;
var txtToDate;
var DistrictID;
var SroID;
var FinancialYear;
var Month;
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#SROOfficeListID').change(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        if ($.fn.DataTable.isDataTable("#DROfficeWiseSummaryTableID")) {
            $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
        }
        $("#EXCELSPANID").html('');
    });

    $('#DROOfficeListID').change(function () {
        
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        if ($.fn.DataTable.isDataTable("#DROfficeWiseSummaryTableID")) {
            $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
        }
        $("#EXCELSPANID").html('');

        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/MISReports/FRUITSIntegration/GetSROOfficeListByDistrictID',
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
                    //alert('3');
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
        //alert('2');
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

    //$('#txtToDate').datepicker({
    //    format: 'dd/mm/yyyy',
    //}).datepicker("setDate", ToDate);

    //$('#txtFromDate').datepicker({
    //    format: 'dd/mm/yyyy',
    //    changeMonth: true,
    //    changeYear: true
    //}).datepicker("setDate", FromDate);

    $('#txtFromDate').focusout(function () {
        //alert('rgegreg');

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        if ($.fn.DataTable.isDataTable("#DROfficeWiseSummaryTableID")) {
            $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
        }
        $("#EXCELSPANID").html('');
    });

    $('#txtToDate').focusout(function () {
        //    alert('fgregregrthrtjhrth');.

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        if ($.fn.DataTable.isDataTable("#DROfficeWiseSummaryTableID")) {
            $("#DROfficeWiseSummaryTableID").DataTable().clear().destroy();
        }
        $("#EXCELSPANID").html('');
    });


    $("#SearchBtn").click(function () {
        if ($.fn.DataTable.isDataTable("#DetailTableID")) {
            $("#DetailTableID").DataTable().clear().destroy();
        }


        SroID = $("#SROOfficeListID option:selected").val();
        DistrictID = $("#DROOfficeListID option:selected").val();
        FinancialYear = $("#financialYearListID option:selected").val();
        Month = $("#MonthListID option:selected").val();

        if (FinancialYear == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Please select Financial Year to view Pending FRUITS report.</span>',
                callback: function () {
                    return;
                }
            });
            return;
        }

        if (Month == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Please select Month to view Pending FRUITS report.</span>',
                callback: function () {
                    return;
                }
            });
            return;
        }



        var DROfficeWiseSummaryTableID = $('#DetailTableID').DataTable({
            ajax: {
                url: '/MISReports/FRUITSIntegration/GetFruitsRecvDetails/',
                type: "POST",
                headers: header,
                data: { 'FromDate': txtFromDate, 'ToDate': txtToDate, 'DistrictID': DistrictID, 'SroID': SroID, "FinancialYear": FinancialYear, "Month": Month },
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
                                    $("#DetailTableID").DataTable().clear().destroy();
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
                    var searchString = $('#DetailTableID_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#DetailTableID_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                DROfficeWiseSummaryTableID.search('').draw();
                                $("#DetailTableID_filter input").prop("disabled", false);
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
                { targets: "_all", orderable: false, "className": "text-center" }
            ],

            columns: [
                { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
                { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
                { data: "ReferenceNo", "searchable": true, "visible": true, "name": "ReferenceNo" },
                { data: "AcknowledgementNo", "searchable": true, "visible": true, "name": "AcknowledgementNo" },
                { data: "DataReceivedDate", "searchable": true, "visible": true, "name": "DataReceivedDate" },
                { data: "Form3", "searchable": true, "visible": true, "name": "Form3" },
                { data: "TranXML", "searchable": true, "visible": true, "name": "TranXML" },
                //{ data: "ActionDateTime", "searchable": true, "visible": true, "name": "ActionDateTime" },
                //{ data: "ResponseDateTime", "searchable": true, "visible": true, "name": "ResponseDateTime" },
                //{ data: "Form3", "searchable": true, "visible": true, "name": "Form3" },
                //{ data: "TranXML", "searchable": true, "visible": true, "name": "TranXML" },
                //{ data: "ResponseXML", "searchable": true, "visible": true, "name": "ResponseXML" },


            ],
            fnInitComplete: function (oSettings, json) {
                //$("#SROSpanID").html(json.SROName);
                $("#EXCELSPANID").html('');
                $("#EXCELSPANID").html(json.ExcelDownloadBtn);

                $('#SummaryTableID').DataTable().destroy();


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

    });

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });
    $('#DtlsSearchParaListCollapseDetail').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchParaDetail').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchParaDetail').removeClass(classToRemove).addClass(classToSet);
    });
});


function DownloadForm3(ReferenceNo, SroCode) {
    //console.log(ReferenceNo);
    //console.log(SroCode);
    window.location.href = '/MISReports/FRUITSIntegration/DownloadFormIII?ReferenceNo=' + ReferenceNo + "&SroCode=" + SroCode;
}

function DownloadTransXML(ReferenceNo, SroCode) {
    //console.log(ReferenceNo);
    //console.log(SroCode);
    window.location.href = '/MISReports/FRUITSIntegration/DownloadTransXML?ReferenceNo=' + ReferenceNo + "&SroCode=" + SroCode;




}

