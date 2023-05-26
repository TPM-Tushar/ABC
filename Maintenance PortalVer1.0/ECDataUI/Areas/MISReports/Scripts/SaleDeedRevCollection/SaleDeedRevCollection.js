
//Global variables.
var token = '';
var header = {};
var SelectedDistrictText;
var SelectedSROText;
var SROOfficeListID;
var DROfficeID;
var FinancialID;
var PropertyValueID;
var PropertyTypeID;
$(document).ready(function () {
    //$('.NoteClass').hide();
    $('#MonthListID').hide();
    //$('#BuildTypeListID').attr("disabled", true);
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    //$('#HiddedRow').hide();
 
   

    $('#DROOfficeListID').focus();
    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $('#SROOfficeListID').change(function () {

        $('#MonthListID').show();

    });

    //Added by Ramank on 25-07-2019
    //$('#PropertyTypeListID').change(function () {
    
    //    var PropertyTypeListVal = $("#PropertyTypeListID option:selected").val();
    //    if (PropertyTypeListVal == "2") {
    //        $('#BuildTypeListID').prop("disabled", false);
    //    } else if (PropertyTypeListVal == "1")
    //    {
    //        $('#BuildTypeListID').prop("disabled", true);
    //    } else if (PropertyTypeListVal == "0")
    //    {
    //        $('#BuildTypeListID').prop("disabled", true);
    //    }

    //});
    
    $('#DROOfficeListID').change(function () {

        if ($("#DROOfficeListID option:selected").val() == 0)
        {
            $('#MonthListID').hide();

        }
            $.ajax({
            url: '/MISReports/SaleDeedRevCollection/GetSROOfficeListByDistrictID',
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
    var SROOfficeListID = $("#SROOfficeListID option:selected").val();
    var DROOfficeListID = $("#DROOfficeListID option:selected").val();


    //$('#DROOfficeListID').trigger("change");

    $("#SearchBtn").click(function () {
        SelectedDistrictText = $("#DROOfficeListID option:selected").text();
        SelectedSROText = $("#SROOfficeListID option:selected").text();
        //$('#NoteID').show();
        if ($.fn.DataTable.isDataTable("#IndexIIReportsID")) {
            $("#IndexIIReportsID").DataTable().clear().destroy();
        }

        // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
        // TO GET SAME DATA IN PDF AND EXCEL 
        //var SROOfficeListID = $("#SROOfficeListID option:selected").val();
        //var DROfficeID = $("#DROOfficeListID option:selected").val();
        //var FinancialID = $("#FinancialYearListID option:selected").val();

        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        DROfficeID = $("#DROOfficeListID option:selected").val();
        FinancialID = $("#FinancialYearListID option:selected").val();
        PropertyTypeID = $("#PropertyTypeListID option:selected").val();
        //BuildTypeID = $("#BuildTypeListID option:selected").val();
        PropertyValueID = $("#PropertyValueListID option:selected").val();
        if ($('#DROOfficeListID').val() < "0" || $('#DROOfficeListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any District.</span>');
            return;
        }
        else if ($('#SROOfficeListID').val() < "0" || $('#SROOfficeListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any SRO.</span>');
            return;

        }
        else if ($('#FinancialYearListID').val() == "" || $('#FinancialYearListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Financial Year.</span>');
            return;
        }
        else if ($('#PropertyTypeListID').val() == "" || $('#PropertyTypeListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Property Type.</span>');
            return;
        }
        else if ($('#PropertyValueListID').val() == "" || $('#PropertyValueListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Property Value.</span>');
            return;
        }
        //else if ($('#BuildTypeListID').val() == "" || $('#BuildTypeListID').val() < "0") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Build Type.</span>');
        //    return;
        //}

        //if (SROOfficeListID == "0") {

        //    var tableIndexReports = $('#IndexIIReportsID').DataTable({
        //        ajax: {
        //            url: '/MISReports/SaleDeedRevCollection/GetSaleDeedRevCollectionDetails',
        //            type: "POST",
        //            headers: header,
        //            data: {
        //                'SROOfficeListID': SROOfficeListID, 'DROfficeID': DROfficeID, 'FinancialID': FinancialID, 'PropertyTypeID': PropertyTypeID, 'PropertyValueID': PropertyValueID
        //            },
        //            dataSrc: function (json) {
        //                unBlockUI();
        //                unBlockUI();
        //                if (json.errorMessage != null) {
        //                    bootbox.alert({
        //                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
        //                        callback: function () {
        //                            if (json.serverError != undefined) {
        //                                window.location.href = "/Home/HomePage"
        //                            }
        //                            else {
        //                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        //                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        //                                    $('#DtlsSearchParaListCollapse').trigger('click');
        //                                $("#IndexIIReportsID").DataTable().clear().destroy();
        //                                $("#PDFSPANID").html('');
        //                                $("#EXCELSPANID").html('');
        //                            }
        //                        }
        //                    });
        //                }
        //                else {
        //                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        //                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
        //                        $('#DtlsSearchParaListCollapse').trigger('click');
        //                }
        //                unBlockUI();
        //                return json.data;
        //            },
        //            error: function () {
        //                //unBlockUI();                   
        //                bootbox.alert({
        //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
        //                    callback: function () {
        //                    }
        //                });
        //                //$.unblockUI();
        //                unBlockUI();
        //            },
        //            beforeSend: function () {
        //                blockUI('loading data.. please wait...');
        //                // Added by SB on 22-3-2019 at 11:06 am
        //                var searchString = $('#IndexIIReportsID_filter input').val();
        //                if (searchString != "") {
        //                    var regexToMatch = /^[^<>]+$/;
        //                    if (!regexToMatch.test(searchString)) {
        //                        $("#IndexIIReportsID_filter input").prop("disabled", true);
        //                        bootbox.alert('Please enter valid Search String ', function () {
        //                            //    $('#menuDetailsListTable_filter input').val('');
        //                            tableIndexReports.search('').draw();
        //                            $("#IndexIIReportsID_filter input").prop("disabled", false);
        //                        });
        //                        unBlockUI();
        //                        return false;
        //                    }
        //                }
        //            }
        //        },
        //        serverSide: true,
        //        // pageLength: 100,
        //        "scrollX": true,
        //        "scrollY": "300px",
        //        scrollCollapse: true,
        //        bPaginate: true,
        //        bLengthChange: true,
        //        // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        //        // "pageLength": -1,
        //        //sScrollXInner: "150%",
        //        bInfo: true,
        //        info: true,
        //        bFilter: false,
        //        searching: true,
        //        // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
        //        // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
        //        //dom: 'lBfrtip',
        //        "destroy": true,
        //        "bAutoWidth": true,
        //        "bScrollAutoCss": true,
        //        //buttons: [
        //        //    {
        //        //        extend: 'pdf',
        //        //        text: '<i class="fa fa-file-pdf-o"></i> PDF',
        //        //        exportOptions: {
        //        //            columns: ':not(.no-print)'
        //        //        },
        //        //        action:
        //        //            function (e, dt, node, config) {
        //        //                //this.disable();
        //        //                window.location = '/MISReports/SaleDeedRevCollection/ExportReportToPDF?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
        //        //            }
        //        //    },
        //        //    {
        //        //        extend: 'excel',
        //        //        text: '<i class="fa fa-file-pdf-o"></i> Excel',
        //        //        exportOptions: {
        //        //            columns: ':not(.no-print)'
        //        //        },
        //        //        action:
        //        //            function (e, dt, node, config) {
        //        //                // BECAUSE OF BELOW CODE EXCEL BUTTON IS WORKING ONLY ONE TIME IF WE COMMENT 
        //        //                // BELOW this.disable(); so it will start working many times
        //        //                this.disable();
        //        //                window.location = '/MISReports/SaleDeedRevCollection/ExportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
        //        //            }
        //        //    }
        //        //],


        //        columnDefs: [
        //            { orderable: false, targets: [0] },
        //            { orderable: false, targets: [1] },
        //            { orderable: false, targets: [2] },
        //            { orderable: false, targets: [3] },
        //            { orderable: false, targets: [4] },
        //            { orderable: false, targets: [5] }
                    
        //        ],
        //        columns: [

        //            { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
        //            { data: "MonthName", "searchable": true, "visible": false, "name": "MonthName" },
        //            { data: "DocumentsRegistered", "searchable": true, "visible": true, "name": "DocumentsRegistered" },
        //            { data: "StampDuty", "searchable": true, "visible": true, "name": "StampDuty" },
        //            { data: "RegistrationFee", "searchable": true, "visible": true, "name": "RegistrationFee" },
        //            { data: "Total", "searchable": true, "visible": true, "name": "Total" }
        //        ],
        //        fnInitComplete: function (oSettings, json) {
        //            //alert('in fnInitComplete');
        //            $("#PDFSPANID").html(json.PDFDownloadBtn);
        //            $("#EXCELSPANID").html(json.ExcelDownloadBtn);
        //        },
        //        preDrawCallback: function () {
        //            unBlockUI();
        //        },
        //        fnRowCallback: function (nRow, aData, iDisplayIndex) {
        //            unBlockUI();
        //            return nRow;
        //        },
        //        drawCallback: function (oSettings) {
        //            //responsiveHelper.respond();
        //            unBlockUI();
        //        },
        //    });

        //}
        //else
        //{
            var tableIndexReports = $('#IndexIIReportsID').DataTable({
                ajax: {
                    url: '/MISReports/SaleDeedRevCollection/GetSaleDeedRevCollectionDetails',
                    type: "POST",
                    headers: header,
                    data: {
                        'SROOfficeListID': SROOfficeListID, 'DROfficeID': DROfficeID, 'FinancialID': FinancialID, 'PropertyTypeID': PropertyTypeID, 'PropertyValueID': PropertyValueID/*, "BuildTypeID": BuildTypeID*/
                    },
                    dataSrc: function (json) {
                        unBlockUI();
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
                                        $("#IndexIIReportsID").DataTable().clear().destroy();
                                        //$("#PDFSPANID").html('');
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
                // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
                // "pageLength": -1,
                //sScrollXInner: "150%",
                bInfo: true,
                info: true,
                bFilter: false,
                searching: true,
                // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
                // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
                //dom: 'lBfrtip',
                "destroy": true,
                "bAutoWidth": true,
                "bScrollAutoCss": true,
                 //aLengthMenu: [
                 //       [25, 50, 100, 200, -1],
                 //       [25, 50, 100, 200, "All"]
                 //   ],
                 //   iDisplayLength: -1,

                //buttons: [
                //    {
                //        extend: 'pdf',
                //        text: '<i class="fa fa-file-pdf-o"></i> PDF',
                //        exportOptions: {
                //            columns: ':not(.no-print)'
                //        },
                //        action:
                //            function (e, dt, node, config) {
                //                //this.disable();
                //                window.location = '/MISReports/SaleDeedRevCollection/ExportReportToPDF?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
                //            }
                //    },
                //    {
                //        extend: 'excel',
                //        text: '<i class="fa fa-file-pdf-o"></i> Excel',
                //        exportOptions: {
                //            columns: ':not(.no-print)'
                //        },
                //        action:
                //            function (e, dt, node, config) {
                //                // BECAUSE OF BELOW CODE EXCEL BUTTON IS WORKING ONLY ONE TIME IF WE COMMENT 
                //                // BELOW this.disable(); so it will start working many times
                //                this.disable();
                //                window.location = '/MISReports/SaleDeedRevCollection/ExportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
                //            }
                //    }
                //],

                //columnDefs: [{
                //    targets: [5],
                //    render: $.fn.dataTable.render.number(',', '.', 2)
                //}],
                columnDefs: [
                    { orderable: false, targets: [0] },
                    { orderable: false, targets: [1] },
                    { orderable: false, targets: [2] },
                    { orderable: false, targets: [3] },
                    { orderable: false, targets: [4] },
                    {
                        orderable: false, targets: [5]
                        
                    }
                   
                ],
                //"language": {
                //    "decimal": ",",
                //    "thousands": "."
                //},
                columns: [

                    { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                    { data: "MonthName", "searchable": true, "visible": true, "name": "MonthName" },
                    { data: "DocumentsRegistered", "searchable": true, "visible": true, "name": "DocumentsRegistered" },
                    { data: "StampDuty", "searchable": true, "visible": true, "name": "StampDuty" },
                    { data: "RegistrationFee", "searchable": true, "visible": true, "name": "RegistrationFee" },
                    { data: "Total", "searchable": true, "visible": true, "name": "Total" }
                ],
                fnInitComplete: function (oSettings, json) {
                    //alert('in fnInitComplete');
                    //$("#PDFSPANID").html(json.PDFDownloadBtn);
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


       // }
        //tableIndexReports.columns.adjust().draw();
        //tableIndexReports.columns.adjust().draw();



    });


    //$("#PdfDownload").click(function () {
    //    //alert('DROfficeID::' + DROfficeID);
    //    //alert('SROOfficeListID::' + SROOfficeListID);
    //    //alert('FinancialID::' + FinancialID);
    //    if (DROfficeID == undefined) { }
    //    else if (SROOfficeListID == "") { }
    //    else if (FinancialID == undefined) { }
    //    else {
    //        window.location.href = '/MISReports/SaleDeedRevCollection/ExportReportToPDF?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID;
    //    }
    //});

    //$("#excelDownload").click(function () { 
    //    //alert('DROfficeID::' + DROfficeID);
    //    //alert('SROOfficeListID::' + SROOfficeListID);
    //    //alert('FinancialID::' + FinancialID);
    //    if (DROfficeID == undefined) { }
    //    else if (SROOfficeListID == "") { }
    //    else if (FinancialID == undefined) { }
    //    else {
    //    window.location.href = '/MISReports/SaleDeedRevCollection/ExportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID;
    //    }
    //});
    $('#IndexIIReportsID').dataTable({
        //"aLengthMenu": [[25, 50, 75, -1], [25, 50, 75, "All"]],
        "iDisplayLength": 100
    });
});

function PDFDownloadFun(DROfficeID, SROOfficeListID, FinancialID) {
    var PropertyValueID;
    var PropertyTypeID;
    window.location.href = '/MISReports/SaleDeedRevCollection/ExportReportToPDF?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID + "&SelectedDistrict=" + SelectedDistrictText + "&SelectedSRO=" + SelectedSROText + "&MaxDate=" + MaxDate + "&PropertyValueID=" + PropertyValueID + "&PropertyTypeID=" + PropertyTypeID ;
}


function EXCELDownloadFun(DROfficeID, SROOfficeListID, FinancialID) {

    window.location.href = '/MISReports/SaleDeedRevCollection/ExportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID + "&SelectedDistrict=" + SelectedDistrictText + "&SelectedSRO=" + SelectedSROText + "&MaxDate=" + MaxDate + "&PropertyValueID=" + PropertyValueID + "&PropertyTypeID=" + PropertyTypeID;;

}