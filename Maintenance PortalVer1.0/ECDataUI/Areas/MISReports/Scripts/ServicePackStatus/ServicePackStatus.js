
//Global variables.
var token = '';
var header = {};

var DROOfficeListID;
var SROOfficeListID;
var SoftwareReleaseTypeListID;
var ServicePackChangeTypeListID;
var ReleasedStatusListID;
var IsSRDRFlag;

var IsSRO_Dropdown_Visible;

var DROOfficeSelText;
var SROOfficeSelText;
var SoftwareReleaseTypeSelText;
var ServicePackChangeTypeSelText;
var ReleasedStatusSelText;


$(document).ready(function () {

   

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    // COMMENTED AND SHIFTED BY SHUBHAM BHAGAT ON 23-09-20202
    // VARIABLE DECLARATION SFIFTED TO GLOBAL AREA FROM DOCUMENT READY TO BE USED GLOBALLY

    //var DROOfficeListID;
    //var SROOfficeListID;
    //var SoftwareReleaseTypeListID;
    //var ServicePackChangeTypeListID;
    //var ReleasedStatusListID;
    //var IsSRDRFlag;

    //var IsSRO_Dropdown_Visible;


    //$('#ServicePackListID').focus();

    //********** To allow only Numbers in textbox **************
    //$(".AllowOnlyNumber").keypress(function (e) {
    //    //if the letter is not digit then display error and don't type anything
    //    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
    //        //display error message
    //        //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
    //        return false;
    //    }
    //});

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    // BELOW FUNCTION IS WORKING BUT NO EFFECT ON DATATABLE
    //$(function () {
    //    $('.collapse').on('shown.bs.collapse', function () {
    //        alert('gbhjn');
    //        $($.fn.dataTable.tables(true)).DataTable()
    //            .columns.adjust();
    //    });
    //});

    // ADDED BY SHUBHAM BHAGAT ON 08-09-2020
    $('#DROOfficeListID').change(function () {
        if (IsSRO_Dropdown_Visible) {
            //alert('1');
            $.ajax({
                url: '/MISReports/ServicePackStatus/GetSROOfficeListByDistrictID',
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
                            //SROOfficeList
                            $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                        });
                    }
                    unBlockUI();
                },
                error: function (xhr) {
                    unBlockUI();
                }
            });
        }
    });



    //$('#DROOfficeListID').trigger("change");

    // ADDED BY SHUBHAM BHAGAT ON 08-09-2020
    // TO SET DEFAULT RADIO BUTTON OF  WHEN PAGE LOADS
    $("input[name=RDOBtnFilter][value=DR]").attr('checked', 'checked');
    // TO HIDE SRO DROPDOWN WHEN PAGE LOAD
    $('#SRODropDownListID').hide();
    // SET FLAG TO FALSE WHEN PAGE LOAD
    IsSRO_Dropdown_Visible = false;

    $('input[type=radio][name=RDOBtnFilter]').change(function () {
        $("#EXCELSPANID").html('');
        //$('#EXCELSPANID').html('');

        //if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
        //    $("#AnywhereECTable").DataTable().clear().destroy();
        //    $("#AnywhereECTable").hide();
        //}

        if ($.fn.DataTable.isDataTable("#IndexIIReportsID")) {
            $("#IndexIIReportsID").DataTable().clear().destroy();
            //$("#IndexIIReportsID").show();
        }

        if ($.fn.DataTable.isDataTable("#ServicePackStatusTableID")) {
            $("#ServicePackStatusTableID").DataTable().clear().destroy();
            //$("#ServicePackStatusTableID").show();
            //$("#IndexIIReportsID").hide();
        }
        $("#IndexIIReportsID").show();
        $("#ServicePackStatusTableID").show();

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
            $('#DtlsSearchParaListCollapse').trigger('click');
        }

        SelectedType = $('input[name="RDOBtnFilter"]:checked').val();
        //alert(SelectedType);
        if (SelectedType == "SR") {
            $('#DRODropDownListID').show();
            $('#SRODropDownListID').show();
            IsSRO_Dropdown_Visible = true;
            // SET SRO DROPDOWN TO EMPTY
            $('#SROOfficeListID').empty();

            $('#SROOfficeListID').append('<option value="0">All</option>');
        }
        else if (SelectedType == "DR") {
            $('#DRODropDownListID').show();
            $('#SRODropDownListID').hide();
            IsSRO_Dropdown_Visible = false;
        }
        //if (SelectedType == "SR") {
        //    $('#DocTypeDropDownListID').show();
        //    if (IsSr == "True") {
        //        $('#RdoBtnId').hide();
        //        $('#DRRowId').hide();
        //        $('#SRRowId').hide();
        //        $('#SRLoginViewId').show();
        //    }
        //    else {
        //        $('#DRRowId').hide();
        //        $('#SRRowId').show();
        //        $('#SRLoginViewId').hide();
        //    }

        //    //$('#DRODropDownListID').show();
        //    //$('#SRODropDownListID').show();
        //}
        //else if (SelectedType == "DR") {
        //    $('#DocTypeDropDownListID').hide();
        //    $('#DRRowId').show();
        //    $('#SRLoginViewId').hide();
        //    $('#SRRowId').hide();
        //    //$('#DRODropDownListID').show();
        //    //$('#SRODropDownListID').show();
        //}
    });

    $("#SearchBtn").click(function () {
        if ($.fn.DataTable.isDataTable("#IndexIIReportsID")) {
            $("#IndexIIReportsID").DataTable().clear().destroy();
        }
        if ($.fn.DataTable.isDataTable("#ServicePackStatusTableID")) {
            $("#ServicePackStatusTableID").DataTable().clear().destroy();
        }

        if (IsSRO_Dropdown_Visible) {
            IsSRDRFlag = "S";
        } else {
            IsSRDRFlag = "D";
        }

        DROOfficeListID = $("#DROOfficeListID option:selected").val();
        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        SoftwareReleaseTypeListID = $("#SoftwareReleaseTypeListID option:selected").val();
        ServicePackChangeTypeListID = $("#ServicePackChangeTypeListID option:selected").val();
        ReleasedStatusListID = $("#ReleasedStatusListID option:selected").val();

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 23-09-2020
        DROOfficeSelText = $("#DROOfficeListID option:selected").text();
        SROOfficeSelText = $("#SROOfficeListID option:selected").text();
        SoftwareReleaseTypeSelText = $("#SoftwareReleaseTypeListID option:selected").text();
        ServicePackChangeTypeSelText = $("#ServicePackChangeTypeListID option:selected").text();
        ReleasedStatusSelText = $("#ReleasedStatusListID option:selected").text();
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 23-09-2020
        //alert("StatusListID:"+StatusListID)


        //if (DROfficeListID <= "0") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select District.</span>');
        //    //return;
        //}
        //else if (SROOfficeListID < "0") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select SRO name.</span>');
        //    //return;
        //}
        //else if (ServicePackListID < "0") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select service pack.</span>');
        //    //return;
        //}

        //else {
        //var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        //if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
        //    $('#DtlsSearchParaListCollapse').trigger('click');

        // #region DATATABLE  CODE COMMENTED AND SHIFTED IN IF/ELSE BY SHUBHAM BHAGAT ON 24-09-2020 
        // BECAUSE OF DATATABLE COLUMN AND DATA WIDTH ISSUE



        //var tableIndexReports = $('#IndexIIReportsID').DataTable({
        //    ajax: {
        //        url: '/MISReports/ServicePackStatus/ServicePackStatusDetails',
        //        type: "POST",
        //        headers: header,
        //        data: {
        //            //'ServicePackListID': ServicePackListID, 'SROOfficeListID': SROOfficeListID, 'StatusListID': StatusListID
        //            'IsSRDRFlag': IsSRDRFlag,
        //            'DROOfficeListID': DROOfficeListID,
        //            'SROOfficeListID': SROOfficeListID,
        //            'SoftwareReleaseTypeListID': SoftwareReleaseTypeListID,
        //            'ServicePackChangeTypeListID': ServicePackChangeTypeListID,
        //            'ReleasedStatusListID': ReleasedStatusListID
        //        },
        //        dataSrc: function (json) {
        //            unBlockUI();
        //            unBlockUI();
        //            if (json.errorMessage != null) {
        //                bootbox.alert({
        //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
        //                    callback: function () {
        //                        if (json.serverError != undefined) {
        //                            window.location.href = "/Home/HomePage"
        //                        }
        //                        else {
        //                            window.location.reload();
        //                        }

        //                    }
        //                });
        //            } else {
        //                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        //                if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
        //                    $('#DtlsSearchParaListCollapse').trigger('click');
        //            }
        //            unBlockUI();
        //            return json.data;
        //        },
        //        error: function () {
        //            //unBlockUI();                   
        //            bootbox.alert({
        //                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
        //                callback: function () {
        //                }
        //            });
        //            //$.unblockUI();
        //            unBlockUI();
        //        },
        //        beforeSend: function () {
        //            blockUI('loading data.. please wait...');
        //            // Added by SB on 22-3-2019 at 11:06 am
        //            var searchString = $('#IndexIIReportsID_filter input').val();
        //            if (searchString != "") {
        //                var regexToMatch = /^[^<>]+$/;
        //                if (!regexToMatch.test(searchString)) {
        //                    $("#IndexIIReportsID_filter input").prop("disabled", true);
        //                    bootbox.alert('Please enter valid Search String ', function () {
        //                        //    $('#menuDetailsListTable_filter input').val('');
        //                        tableIndexReports.search('').draw();
        //                        $("#IndexIIReportsID_filter input").prop("disabled", false);
        //                    });
        //                    unBlockUI();
        //                    return false;
        //                }
        //            }
        //        }
        //    },
        //    serverSide: true,
        //    // pageLength: 100,
        //    "scrollX": true,
        //    "scrollY": "300px",
        //    scrollCollapse: true,
        //    bPaginate: true,
        //    bLengthChange: true,
        //    // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        //    // "pageLength": -1,
        //    //sScrollXInner: "150%",
        //    bInfo: true,
        //    info: true,
        //    bFilter: false,
        //    searching: true,
        //    "destroy": true,
        //    "bAutoWidth": true,
        //    "bScrollAutoCss": true,
        //    columnDefs: [
        //        { orderable: false, targets: [0] },
        //        { orderable: false, targets: [1] },
        //        { orderable: false, targets: [2] },
        //        { orderable: false, targets: [3] },
        //        { orderable: false, targets: [4] },
        //        { orderable: false, targets: [5] },
        //        { orderable: false, targets: [6] },
        //        { orderable: false, targets: [7] },
        //        { orderable: false, targets: [8] },
        //        { orderable: false, targets: [9] },
        //        { orderable: false, targets: [10] },
        //        { orderable: false, targets: [11] },
        //        { orderable: false, targets: [12] },
        //        { orderable: false, targets: [13] }
        //    ],
        //    columns: [
        //        { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
        //        { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName" },
        //        { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
        //        { data: "SoftwareReleaseType", "searchable": true, "visible": true, "name": "SoftwareReleaseType" },
        //        { data: "ReleaseMode", "searchable": true, "visible": true, "name": "ReleaseMode" },
        //        { data: "Major", "searchable": true, "visible": true, "name": "Major" },
        //        { data: "Minor", "searchable": true, "visible": true, "name": "Minor" },
        //        { data: "Description", "searchable": true, "visible": true, "name": "Description" },
        //        { data: "InstallationProcedure", "searchable": true, "visible": true, "name": "InstallationProcedure" },
        //        { data: "ChangeType", "searchable": true, "visible": true, "name": "ChangeType" },
        //        { data: "Status", "searchable": true, "visible": true, "name": "Status" },
        //        { data: "ReleaseInstruction", "searchable": true, "visible": true, "name": "ReleaseInstruction" },
        //        { data: "AddedDate", "searchable": true, "visible": true, "name": "AddedDate" },
        //        { data: "ReleaseDate", "searchable": true, "visible": true, "name": "ReleaseDate" }

        //    ],
        //    fnInitComplete: function (oSettings, json) {
        //        //alert("fnInitComplete");
        //        //$("#PDFSPANID").html(json.PDFDownloadBtn);
        //        $("#EXCELSPANID").html('');
        //        $("#EXCELSPANID").html(json.ExcelDownloadBtn);
        //        //tableIndexReports.columns.adjust().draw();
        //    },
        //    preDrawCallback: function () {
        //        //alert("preDrawCallback");
        //        unBlockUI();
        //    },
        //    fnRowCallback: function (nRow, aData, iDisplayIndex) {
        //        unBlockUI();
        //        return nRow;
        //    },
        //    drawCallback: function (oSettings) {
        //        //alert("drawCallback");
        //        unBlockUI();
        //    },
        //});
        // #endregion

        if (IsSRDRFlag == "D") {
            $('#IndexIIReportsID').show();
            $('#ServicePackStatusTableID').hide();
            var tableIndexReports = $('#IndexIIReportsID').DataTable({
                ajax: {
                    url: '/MISReports/ServicePackStatus/ServicePackStatusDetails',
                    type: "POST",
                    headers: header,
                    data: {
                        //'ServicePackListID': ServicePackListID, 'SROOfficeListID': SROOfficeListID, 'StatusListID': StatusListID
                        'IsSRDRFlag': IsSRDRFlag,
                        'DROOfficeListID': DROOfficeListID,
                        'SROOfficeListID': SROOfficeListID,
                        'SoftwareReleaseTypeListID': SoftwareReleaseTypeListID,
                        'ServicePackChangeTypeListID': ServicePackChangeTypeListID,
                        'ReleasedStatusListID': ReleasedStatusListID
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
                                        window.location.reload();
                                    }

                                }
                            });
                        } else {
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
                "destroy": true,
                // CHANGED BY SHUBHAM BHAGAT  ON 24-09-2020
                "bAutoWidth": true,
                //"bAutoWidth": false,
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
                    { orderable: false, targets: [8] },
                    { orderable: false, targets: [9] },
                    { orderable: false, targets: [10] },
                    { orderable: false, targets: [11] },
                    { orderable: false, targets: [12] }//,
                    //{ orderable: false, targets: [13] }
                ],
                columns: [
                    { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                    { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName" },
                    //{ data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
                    { data: "SoftwareReleaseType", "searchable": true, "visible": true, "name": "SoftwareReleaseType" },
                    { data: "ReleaseMode", "searchable": true, "visible": true, "name": "ReleaseMode" },
                    { data: "Major", "searchable": true, "visible": true, "name": "Major" },
                    { data: "Minor", "searchable": true, "visible": true, "name": "Minor" },
                    { data: "Description", "searchable": true, "visible": true, "name": "Description" },
                    { data: "InstallationProcedure", "searchable": true, "visible": true, "name": "InstallationProcedure" },
                    { data: "ChangeType", "searchable": true, "visible": true, "name": "ChangeType" },
                    { data: "Status", "searchable": true, "visible": true, "name": "Status" },
                    { data: "ReleaseInstruction", "searchable": true, "visible": true, "name": "ReleaseInstruction" },
                    { data: "AddedDate", "searchable": true, "visible": true, "name": "AddedDate" },
                    { data: "ReleaseDate", "searchable": true, "visible": true, "name": "ReleaseDate" }

                ],
                fnInitComplete: function (oSettings, json) {
                    //alert("fnInitComplete");
                    //$("#PDFSPANID").html(json.PDFDownloadBtn);
                    $("#EXCELSPANID").html('');
                    $("#EXCELSPANID").html(json.ExcelDownloadBtn);
                    //tableIndexReports.columns.adjust().draw();
                    
                },
                preDrawCallback: function () {
                    //alert("preDrawCallback");
                    unBlockUI();
                },
                fnRowCallback: function (nRow, aData, iDisplayIndex) {
                    unBlockUI();
                    return nRow;
                },
                drawCallback: function (oSettings) {
                    //alert("drawCallback");
                    unBlockUI();
                },
            });
            tableIndexReports.columns.adjust().draw();
        }
        else {
            $('#ServicePackStatusTableID').show();
            $('#IndexIIReportsID').hide();
            var ServicePackStatusTableID_VAR = $('#ServicePackStatusTableID').DataTable({
                ajax: {
                    url: '/MISReports/ServicePackStatus/ServicePackStatusDetails',
                    type: "POST",
                    headers: header,
                    data: {
                        //'ServicePackListID': ServicePackListID, 'SROOfficeListID': SROOfficeListID, 'StatusListID': StatusListID
                        'IsSRDRFlag': IsSRDRFlag,
                        'DROOfficeListID': DROOfficeListID,
                        'SROOfficeListID': SROOfficeListID,
                        'SoftwareReleaseTypeListID': SoftwareReleaseTypeListID,
                        'ServicePackChangeTypeListID': ServicePackChangeTypeListID,
                        'ReleasedStatusListID': ReleasedStatusListID
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
                                        window.location.reload();
                                    }

                                }
                            });
                        } else {
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
                        var searchString = $('#ServicePackStatusTableID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;
                            if (!regexToMatch.test(searchString)) {
                                $("#ServicePackStatusTableID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    //    $('#menuDetailsListTable_filter input').val('');
                                    ServicePackStatusTableID_VAR.search('').draw();
                                    $("#ServicePackStatusTableID_filter input").prop("disabled", false);
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
                // added by Shubham Bhagat on 24-09-2020 
                // for making column widt widher than they actually need
                //sScrollXInner: "102%",
                bInfo: true,
                info: true,
                bFilter: false,
                searching: true,
                "destroy": true,
                // CHANGED BY SHUBHAM BHAGAT  ON 24-09-2020
                "bAutoWidth": true,
                //"bAutoWidth":false,
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
                    { orderable: false, targets: [8] },
                    { orderable: false, targets: [9] },
                    { orderable: false, targets: [10] },
                    { orderable: false, targets: [11] },
                    { orderable: false, targets: [12] },
                    { orderable: false, targets: [13] }

                    // { "width": "6.25%", orderable: true, targets: [0] },
                    //{ "width": "6.25%", orderable: false, targets: [1] },
                    //{ "width": "6.25%", orderable: false, targets: [2] },
                    //{ "width": "6.25%", orderable: false, targets: [3] },
                    //{ "width": "6.25%", orderable: false, targets: [4] },
                    //{ "width": "6.25%", orderable: false, targets: [5] },
                    //{ "width": "6.25%", orderable: false, targets: [6] },
                    //{ "width": "6.25%", orderable: false, targets: [7] },
                    //{ "width": "6.25%", orderable: false, targets: [8] },
                    //{ "width": "18.75%", orderable: false, targets: [9] },
                    //{ "width": "6.25%", orderable: false, targets: [10] },
                    //{ "width": "6.25%", orderable: false, targets: [11] },
                    //{ "width": "6.25%", orderable: false, targets: [12] },
                    //{ "width": "6.25%", orderable: false, targets: [13] }
                ],
                columns: [
                    { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                    { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName" },
                    { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
                    { data: "SoftwareReleaseType", "searchable": true, "visible": true, "name": "SoftwareReleaseType" },
                    { data: "ReleaseMode", "searchable": true, "visible": true, "name": "ReleaseMode" },
                    { data: "Major", "searchable": true, "visible": true, "name": "Major" },
                    { data: "Minor", "searchable": true, "visible": true, "name": "Minor" },
                    { data: "Description", "searchable": true, "visible": true, "name": "Description" },
                    { data: "InstallationProcedure", "searchable": true, "visible": true, "name": "InstallationProcedure" },
                    { data: "ChangeType", "searchable": true, "visible": true, "name": "ChangeType" },
                    { data: "Status", "searchable": true, "visible": true, "name": "Status" },
                    { data: "ReleaseInstruction", "searchable": true, "visible": true, "name": "ReleaseInstruction" },
                    { data: "AddedDate", "searchable": true, "visible": true, "name": "AddedDate" },
                    { data: "ReleaseDate", "searchable": true, "visible": true, "name": "ReleaseDate" }

                ],
                fnInitComplete: function (oSettings, json) {
                    //alert("fnInitComplete");
                    //$("#PDFSPANID").html(json.PDFDownloadBtn);
                    $("#EXCELSPANID").html('');
                    $("#EXCELSPANID").html(json.ExcelDownloadBtn);
                    //tableIndexReports.columns.adjust().draw();
                    //alert($('#ServicePackStatusTableID').find('tbody tr:first th:first').trigger('click'));
                    //$('#ServicePackStatusTableID').find('tbody tr:first th:first').trigger('click');
                    //$('#ServicePackStatusTableID tr:first-child').find('th:eq(0)').trigger('click');
                },
                preDrawCallback: function () {
                    //alert("preDrawCallback");
                    unBlockUI();
                },
                fnRowCallback: function (nRow, aData, iDisplayIndex) {
                    unBlockUI();
                    return nRow;
                },
                drawCallback: function (oSettings) {
                    //alert("drawCallback");
                    unBlockUI();
                },
            });

            //$($.fn.dataTable.tables(true)).DataTable()
            //    .scroller.measure();
            ServicePackStatusTableID_VAR.columns.adjust().draw();
        }

        //}
        //alert('1');
        // ADDED BY SHUBHAM BHAGAT ON 24-09-2020
        //if (IsSRDRFlag == "D")
        //{
        //    tableIndexReports.columns([2]).visible(false);
        //}


        //tableIndexReports.columns.adjust().draw();
        //tableIndexReports.responsive.recalc();
        //$('#IndexIIReportsID').DataTable()
        //    .columns.adjust()
        //    .responsive.recalc();
        //alert('2');
    });

});

//function PDFDownloadFun(ServicePackListID, StatusListID, SROOfficeListID){
//    window.location.href = '/MISReports/ServicePackStatus/ExportReportToPDF?DROOfficeListID=' + DROOfficeListID + "&SROOfficeListID=" + SROOfficeListID + "&SoftwareReleaseTypeListID=" + SoftwareReleaseTypeListID + "&ServicePackChangeTypeListID=" + ServicePackChangeTypeListID + "&ReleasedStatusListID=" + ReleasedStatusListID;
//}


function EXCELDownloadFun(DROOfficeListID, SROOfficeListID, SoftwareReleaseTypeListID, ServicePackChangeTypeListID, ReleasedStatusListID, IsSRDRFlag) {
    //window.location.href = '/MISReports/ServicePackStatus/ExportToExcel?DROOfficeListID=' + DROOfficeListID + "&SROOfficeListID=" + SROOfficeListID + "&SoftwareReleaseTypeListID=" + SoftwareReleaseTypeListID + "&ServicePackChangeTypeListID=" + ServicePackChangeTypeListID + "&ReleasedStatusListID=" + ReleasedStatusListID + "&IsSRDRFlag=" + IsSRDRFlag;
    window.location.href = '/MISReports/ServicePackStatus/ExportToExcel?DROOfficeListID=' + DROOfficeListID + "&SROOfficeListID=" + SROOfficeListID + "&SoftwareReleaseTypeListID=" + SoftwareReleaseTypeListID + "&ServicePackChangeTypeListID=" + ServicePackChangeTypeListID + "&ReleasedStatusListID=" + ReleasedStatusListID + "&IsSRDRFlag=" + IsSRDRFlag + "&DROOfficeSelText=" + DROOfficeSelText + "&SROOfficeSelText=" + SROOfficeSelText + "&SoftwareReleaseTypeSelText=" + SoftwareReleaseTypeSelText + "&ServicePackChangeTypeSelText=" + ServicePackChangeTypeSelText + "&ReleasedStatusSelText=" + ReleasedStatusSelText;
}