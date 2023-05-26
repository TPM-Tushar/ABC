//Global variables.
var token = '';
var header = {};
var SROOfficeListID;
var DROOfficeListID;
var Stamp5Date;
var ToDate;
var SpecialSP;

//Added by Madhusoodan on 30-04-2020
var DocTypeID;
var DocTypeText;

$(document).ready(function () {
    $('#txtStamp5DateID').change(function () {
        //alert("The text has been changed.");
        //alert($('#txtStamp5DateID').val());
        //$('#ToDateID').val($('#txtStamp5DateID').val());
        $('#ToDateID').datepicker({
            format: 'dd/mm/yyyy',
            changeMonth: true,
            changeYear: true
        }).datepicker("setDate", $('#txtStamp5DateID').val());
//        var date = new Date();
//        if (date.getMonth() + 1 <= 9) {
//            var month = "0" + (date.getMonth() + 1);
//        }
//        else {
//            var month = (date.getMonth() + 1);
//        }
//        if (date.getDate() < 10) {
//            var dtString = '0' + date.getDate() + '/' + month + '/' + date.getFullYear();
//        }
//        else {
//            var dtString = date.getDate() + '/' + month + '/' + date.getFullYear();
//}
//        if ($('#txtStamp5DateID').val() == $('#ToDateID').val() && $('#txtStamp5DateID').val() == dtString) {
//            $('.disapp').css("display", "block");
//        }
//        else {
//            $('#SpecialSP').prop('checked', false); 
//            $('.disapp').css("display", "none");
//        }
    });

    //$('#ToDateID').change(function () {
    //    var date = new Date();
    //    if (date.getMonth() + 1 <= 9) {
    //        var month = "0" + (date.getMonth() + 1);
    //    }
    //    else {
    //        var month = (date.getMonth() + 1);
    //    }
    //    if (date.getDate() < 10) {
    //        var dtString = '0' + date.getDate() + '/' + month + '/' + date.getFullYear();
    //    }
    //    else {
    //        var dtString = date.getDate() + '/' + month + '/' + date.getFullYear();
    //    }
    //    if ($('#txtStamp5DateID').val() == $('#ToDateID').val() && $('#txtStamp5DateID').val() == dtString) {
    //        $('.disapp').css("display", "block");
    //    }
    //    else {
    //        $('#SpecialSP').prop('checked', false);
    //        $('.disapp').css("display", "none");
    //    }


    //});
    
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DROOfficeListID').focus();
    $('#RegDate').Text = "asgdsg";
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var TodaysDate = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();

    $('#txtStamp5DateID').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,
    });

    $('#divStamp5Date').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,
    });

    $('#divToDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,
    });

    $('#txtStamp5DateID').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", FromDate);

    $('#ToDateID').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,
    });

    $('#ToDateID').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", ToDate);

    $('#DROOfficeListID').change(function () {
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/TodaysDocumentsRegistered/GetSROOfficeListByDistrictID',
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

    //$('#DROOfficeListID').trigger("change");

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $("#SearchBtn").click(function () {
        blockUI('loading data.. please wait...');
        if ($('#SpecialSP').is(":checked")) {
            SpecialSP = "true";
        }
        else {
            SpecialSP = "false";
        }
        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        DROOfficeListID = $("#DROOfficeListID option:selected").val();
        Stamp5Date = $("#txtStamp5DateID").val();

        //Added by Madhusoodan on 30-04-2020
        DocTypeID = $("#DocTypeID option:selected").val();
        //alert(DocTypeID);
        DocTypeText = $("#DocTypeID option:selected").text();
        //alert(DocTypeText);

        //Added by Madhusoodan on 06-05-2020
        //To Change to span tag of Collapsale heading above Datatable
        var selectedDocType = $("#DocTypeID option:selected").text();
        $('#InnerDocTypeDivID').html(selectedDocType); 

        //// 05-05-2020

        if (DocTypeID == '1') {
            //alert("Selected DocTypeID :  In if" + DocTypeID);
            $("#ReportInfoID").show();
           // $("#TotalContainerID").show();
        }
        else {
            //alert("Selected DocTypeID : " + DocTypeID);
            $("#ReportInfoID").hide();
           // $("#TotalContainerID").hide();
        }


        ToDate = $("#ToDateID").val();
        if (Stamp5Date == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
        }
        else if (ToDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
        }
        else if ($('#DROOfficeListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid District.</span>');
        }
        else if ($('#SROOfficeListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid SRO</span>');
        }
        else {
            $.ajax({
                url: '/MISReports/TodaysDocumentsRegistered/GetTodaysTotalDocumentsRegisteredSummary',
                data: {
                    'FromDate': Stamp5Date, 'ToDate': ToDate, 'SroID': SROOfficeListID, 'DistrictID': DROOfficeListID, 'DocTypeID': DocTypeID, 'SpecialSP': SpecialSP
                },
                datatype: "json",
                headers: header,
                type: "POST",
                success: function (data) {
                    if (data.errorMessage == undefined) {
                        $('#SummaryTableID').html('');
                        $('#SummaryTableID').html(data);

                        //Added by mayank on 13/09/2021 for Firm registered report
                        if ($("#DocTypeID").val() == 4)
                            $('#DROfficeWiseSummaryTableID tr').eq(0).find('th').eq(2).html('Total Other ( in Rs. )');
                        else
                            $('#DROfficeWiseSummaryTableID tr').eq(0).find('th').eq(2).html('Total Stamp Duty ( in Rs. )');

                        var ToDaysDocsRegdDataTable = $('#ToDaysDocsRegdDataTable').DataTable({
                            ajax: {
                                url: '/MISReports/TodaysDocumentsRegistered/GetTodaysTotalDocumentsRegisteredDetails',
                                type: "POST",
                                headers: header,
                                data: {
                                    'FromDate': Stamp5Date, 'ToDate': ToDate, 'SroID': SROOfficeListID, 'DistrictID': DROOfficeListID, 'DocTypeID': DocTypeID, 'SpecialSP': SpecialSP
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
                                                    $("#ToDaysDocsRegdDataTable").DataTable().clear().destroy();
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
                                    //MergeGridCells();

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
                                    var searchString = $('#ToDaysDocsRegdDataTable_filter input').val();
                                    if (searchString != "") {
                                        var regexToMatch = /^[^<>]+$/;
                                        if (!regexToMatch.test(searchString)) {
                                            $("#ToDaysDocsRegdDataTable_filter input").prop("disabled", true);
                                            bootbox.alert('Please enter valid Search String ', function () {
                                                ToDaysDocsRegdDataTable.search('').draw();
                                                $("#ToDaysDocsRegdDataTable_filter input").prop("disabled", false);
                                            });
                                            unBlockUI();
                                            return false;
                                        }
                                    }
                                }
                            },
                            serverSide: true,
                            "scrollY": "300px",
                            "scrollCollapse": true,
                            bPaginate: true,
                            bLengthChange: true,
                            bInfo: true,
                            info: true,
                            bFilter: false,
                            searching: true,
                            "destroy": true,
                            "bAutoWidth": true,
                            "bScrollAutoCss": true,
                            "lengthMenu": [[10, 25, 50, 350], [10, 25, 50, "All"]],
                            "pageLength": 350,
                            "bSort": true,
                            columnDefs: [

                                { "orderable": false, targets: [0] }
                                //{ "orderable": false, targets: [1] }
                            ],

                            columns: [

                                { data: "SRNo", "searchable": true, "visible": true, "name": "SRNo" },
                                { data: "District", "searchable": true, "visible": true, "name": "District" },
                                { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
                                { data: "Documents", "searchable": true, "visible": true, "name": "Documents" },
                                { data: "RegistrationFee", "searchable": true, "visible": true, "name": "RegistrationFee" },
                                { data: "StampDuty", "searchable": true, "visible": true, "name": "StampDuty" },
                                { data: "Total", "searchable": true, "visible": true, "name": "Total" }

                            ],
                            fnInitComplete: function (oSettings, json) {
                                $("#PDFSPANID").html(json.PDFDownloadBtn);
                                $("#EXCELSPANID").html(json.ExcelDownloadBtn);
                                // ADDED BY SHUBHAM BHAGAT ON 23-09-2020
                                $("#NoteSpanID").html(json.ReportInfo);
                                
                                //MergeGridCells();


                            },
                            preDrawCallback: function () {
                                unBlockUI();

                            },
                            fnRowCallback: function (nRow, aData, iDisplayIndex) {
                                unBlockUI();
                                return nRow;
                                //MergeGridCells();

                            },
                            drawCallback: function (oSettings) {
                                //responsiveHelper.respond();
                                unBlockUI();
                                MergeGridCells();

                            },
                        });
                        //Added by mayank on 13/09/2021 for Firm registered report
                        if ($("#DocTypeID").val() == 4) {
                            $(ToDaysDocsRegdDataTable.column(5).header()).html('Others ( in Rs.)');
                            ToDaysDocsRegdDataTable.columns([2]).visible(false);
                            ToDaysDocsRegdDataTable.columns([5]).visible(false);
                        }
                        else {
                            $(ToDaysDocsRegdDataTable.column(5).header()).html('Stamp Duty ( in Rs.)');
                            ToDaysDocsRegdDataTable.columns([2]).visible(true);
                            ToDaysDocsRegdDataTable.columns([5]).visible(true);
                        }

                    }
                    else {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            callback: function () {
                            }
                        });
                        unBlockUI();
                    }
                },
                error: function (xhr) {

                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    unBlockUI();
                }
            });

            ////var form = $("#TotalDocsRegFormId");
            ////form.removeData('validator');
            ////form.removeData('unobtrusiveValidation');
            ////$.validator.unobtrusive.parse(form);
            ////if ($("#TotalDocsRegFormId").valid()) {
            //blockUI('Loading data please wait.');
            //$.ajax({
            //    type: "POST",  /",
            //    cache: false,
            //    headers: header,
            //    data: $("#TotalDocsRegFormId").serialize(),
            //    success: function (data) {
            //        unBlockUI();
            //        //if (data.success) {
            //        $('#tableToBeLaded').html(data);

            //        //}
            //        //else
            //        //{
            //        //if (data.Message.length != 0) {
            //        //if (data.success == false) {
            //        //    bootbox.alert({
            //        //        //   size: 'small',
            //        //        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
            //        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
            //        //        callback: function () {

            //        //        }
            //        //    });
            //        //}
            //        //bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i> <span class="boot-alert-txt">' + data.errorMessage + '</span>');
            //        //alert('in fhruieqfguire');

            //        //}
            //        //}

            //        //$.unblockUI();
            //    },
            //    error: function (xhr, status, err) {
            //        //bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i> <span class="boot-alert-txt">'+"Error occured while proccessing your request : " + err+'</span>');
            //        //alert("asd");
            //        //window.location.href = "/Error/Index";
            //        bootbox.alert({
            //            //   size: 'small',
            //            //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
            //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while proccessing your request : " + err + '</span>',
            //            callback: function () {

            //            }
            //        });

            //        unBlockUI();
            //        //$.unblockUI();
            //    }
            //});
            ////}
            ////else {
            ////    return;
            ////}


        }
        
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

        ////06-05-2020
        //if (DocTypeID == 1) {
        //    alert("Selected DocTypeID : 1 ");
        //    //$("#ReportInfoID").show();
        //    $("#TotalContainerID").show();
        //}
        //else {
        //    alert("Selected DocTypeID : 2 || 3 ");
        //    //$("#ReportInfoID").hide();
        //    $("#TotalContainerID").hide();
        //}
        
    });

    //$('#txtStamp5DateID').change(function () {
    //    //alert("A"); 
    //    $('#RegDate').text($('#txtStamp5DateID').val());
    //});
    //$('#ToDaysDocsRegdDataTable').dataTable();


    //Added by mayank on 13/09/2021 for Firm registered report
    $("#DocTypeID").change(function () {
        if ($("#DocTypeID").val() == 4) {
            $("#SROOfficeListID").val(0);
            $("#SROOfficeListID").attr('disabled', 'disabled');
            $("#divSpecialSP").hide();
        }
        else {
            $("#SROOfficeListID").removeAttr('disabled');
            $("#divSpecialSP").show();

        }

    });

    //End

});


function PDFDownloadFun() {
    //alert("SRO:" + SROOfficeListID + "DRO:" + DROOfficeListID + "Date:" + Stamp5Date + "ToDate:" + ToDate + "MaxDate:" + MaxDate);
    window.location.href = '/MISReports/TodaysDocumentsRegistered/ExportTodaysTotalDocsRegReportToPDF?SROOfficeListID=' + SROOfficeListID + "&DROOfficeListID=" + DROOfficeListID + "&Date=" + Stamp5Date + "&ToDate=" + ToDate + "&MaxDate=" + MaxDate;
}


function EXCELDownloadFun() {
    //alert("aaaaa");
    //alert("aaaaa");

    //blockUI('loading data.. please wait...');

    //$.fileDownload('/MISReports/TodaysDocumentsRegistered/ExportTodaysDocumentsRegisteredReportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROOfficeListID=" + DROOfficeListID + "&Date=" + Stamp5Date + "&ToDate=" + ToDate + "&MaxDate=" + MaxDate)
    //    .done(function () {
    //        unBlockUI();
    //    })
    //    .fail(function () {
    //        unBlockUI();

    //        alert('Some error occured while processing!');
    //    });

    //return;

    //Added by Madhusoodan on 30-04-2020
    //Added DocTypeId & DocTypeText in it.
    window.location.href = '/MISReports/TodaysDocumentsRegistered/ExportTodaysDocumentsRegisteredReportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROOfficeListID=" + DROOfficeListID + "&Date=" + Stamp5Date + "&ToDate=" + ToDate + "&MaxDate=" + MaxDate + "&DocTypeID=" + DocTypeID + "&DocTypeText=" + DocTypeText + "&SpecialSP=" + SpecialSP;
}

function MergeGridCells() {

    var dimension_cells = new Array();
    var dimension_col = null;
    var columnCount = $("#ToDaysDocsRegdDataTable tr:first th").length;
    //for (dimension_col = 0; dimension_col < columnCount; dimension_col++) {
    // first_instance holds the first instance of identical td
    var first_instance = null;
    var firstInstanceOfFirstCol = null;
    var rowspan = 1;
    var srNoCount = 0;
    // iterate through rows
    $("#ToDaysDocsRegdDataTable").find('tr').each(function () {

        // find the td of the correct column (determined by the dimension_col set above)
        var dimension_td = $(this).find('td:nth-child(' + 2 + ')');
        var dimension_td1 = $(this).find('td:nth-child(' + 1 + ')');

        if (first_instance == null) {
            // must be the first row
            first_instance = dimension_td;
            firstInstanceOfFirstCol = dimension_td1;
            firstInstanceOfFirstCol.text = srNoCount++;

        } else if (dimension_td.text() == first_instance.text()) {
            // the current td is identical to the previous
            // remove the current td
            dimension_td.remove();
            dimension_td1.remove();
            ++rowspan;
            // increment the rowspan attribute of the first instance
            first_instance.attr('rowspan', rowspan);
            firstInstanceOfFirstCol.attr('rowspan', rowspan);

        } else {
            // this cell is different from the last

            first_instance = dimension_td;
            firstInstanceOfFirstCol = dimension_td1;
            dimension_td1.html(srNoCount++);
            //firstInstanceOfFirstCol.data(srNoCount++).draw();
            //alert(srNoCount);
            rowspan = 1;
        }
    });
    //}
}
