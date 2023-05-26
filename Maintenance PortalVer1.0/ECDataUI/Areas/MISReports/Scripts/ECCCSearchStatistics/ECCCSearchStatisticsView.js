var token = '';
var header = {};
var api = "";
var detailsTable = "";
$(document).ready(function () {
    $("#DroListID").change(function () {
        var _DroCode = $('#DroListID').val();
        token = $('[name=__RequestVerificationToken]').val();
        header["__RequestVerificationToken"] = token;

        $.ajax({
            url: "/MISReports/ECCCSearchStatistics/GetSroList",
            data: { DroCode: _DroCode },
            cache: false,
            type: "POST",
            headers: header,
            success: function (data) {

                $("#SROOfficeListID").empty();
                $.each(data.SROfficeList, function (i, SROfficeList) {
                    $('#SROOfficeListID').append('<option value="' + SROfficeList.Value + '">' + SROfficeList.Text + '</option>')
                })
                $("#SRODropDownListID").show()
            },
            error: function (err) {
                bootbox.alert({
                    //size: 'small',
                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                });

            }
        })

    });
    $("#DroListID").change(function () {
        HideTable();
    });
    $("#SROOfficeListID").change(function () {
        HideTable();
    });
    $("#financialYearListID").change(function () {
        HideTable();
    });
    $("#MonthListID").change(function () {
        HideTable();
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
    $("#ExcelDivId").hide();
    //Added by Madhusoodan on 10/08/2020
    $("#SummaryTable").hide();
    $("#DetailTable").hide();
    //added by mayank content 23-06-2021 for ECCC Statistics
    $('input:radio[name=RDOBtnFilter][value=OfficeWise]').trigger('click');
    $('input[type=radio][name=RDOBtnFilter]').change(function () {
        //alert("radio change");
        HideTable();
        //var CurrentSearchClass = $('#DtlsToggleIconSearchPara').attr('class');
        var CurrentDetailClass = $('#DtlsToggleIconSearchParaDetail').attr('class');
        //if (CurrentSearchClass == "fa fa-minus-square-o fa-pull-left fa-2x")
        //    $('#DtlsSearchParaListCollapse').trigger('click');
        if (CurrentDetailClass == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapseDetail').trigger('click');
    });
    //23-06-2021 comment end
});
function LoadSummaryTable() {

    _SroCode = $("#SROOfficeListID").val();
    _DROCode = $("#DroListID").val();
    _FinancialyearCode = $("#financialYearListID").val();
    _MonthCode = $("#MonthListID").val();
    //added by mayank content 23-06-2021 for ECCC Statistics
    var SearchBy = $('input[name="RDOBtnFilter"]:checked').val();
    //console.log(SearchBy);
    if (SearchBy != undefined) {
        var _SearchByParameter = "";
        if (SearchBy == "DurationWise")
            _SearchByParameter = "SearchDurationWise";

        if (SearchBy == "OfficeWise")
            _SearchByParameter = "SearchOfficeWise";
    }
    else {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Select any one filter' + '</span>',
            callback: function () {
            }
        });
        return;
    }
    //end 23-06-2021 for ECCC Statistics 
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    if (_FinancialyearCode == 0) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Select Financial Year' + '</span>')
    }
    else {
        var summaryTable = $('#SummaryTable').DataTable({
            "ajax": {
                "url": "/MISReports/ECCCSearchStatistics/GetSummary",
                "type": "POST",
                headers: header,
                data: { SroCode: _SroCode, DROCode: _DROCode, FinancialyearCode: _FinancialyearCode, MonthCode: _MonthCode, SearchByParameter: _SearchByParameter },
                //"datatype": "json",
                dataSrc: function (json) {
                    unBlockUI();
                    if (json.errorMessage != null) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            callback: function () {
                                if (json.serverError != undefined) {
                                    window.location.href = "/Home/HomePage";
                                }
                                else {
                                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                                        $('#DtlsToggleIconSearchPara').trigger('click');

                                    }
                                    $("#SummaryTable").DataTable().clear().destroy();

                                }
                            }
                        });
                    }
                    else {
                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                            $('#DtlsToggleIconSearchPara').trigger('click');
                            $('#DtlsToggleIconSearchPara').attr('class', "fa fa-minus-square-o fa-pull-left fa-2x");
                        }
                    }
                    unBlockUI();
                    return json.data;
                },

                error: function () {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    unBlockUI();
                },
                beforeSend: function () {
                    blockUI('loading data.. please wait...');
                    //var searchString = $('#SummaryTable_filter input').val();
                    //if (searchString != "") {
                    //    var regexToMatch = /^[^<>]+$/;
                    //    if (!regexToMatch.test(searchString)) {
                    //        $("#SummaryTable_filter input").prop("disabled", true);
                    //        bootbox.alert('Please enter valid Search String ', function () {
                    //            summaryTable.search('').draw();
                    //            $("#SummaryTable_filter input").prop("disabled", false);
                    //        });
                    //        unBlockUI();
                    //        return false;
                    //    }
                    //}
                }
            },
            //"iDisplayLength": 10,
            serverSide: true,
            "scrollY": "300px",
            "scrollCollapse": true,
            //bPaginate: true,
            //bLengthChange: true,
            //bInfo: true,
            //info: true,
            bFilter: false,
            //searching: true,
            "destroy": true,
            "bAutoWidth": true,
            "bScrollAutoCss": true,
            columnDefs: [

                { targets: "_all", orderable: false, "className": "text-center" }

             ],
            columns: [

                { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
                { data: "MonthYear", "searchable": true, "visible": true, "name": "MonthYear" },
                { data: "TotalUserLogged", "searchable": true, "visible": true, "name": "TotalUserLogged" },
                { data: "TotalECSearched", "searchable": true, "visible": true, "name": "TotalECSearched" },
                { data: "TotalECSubmitted", "searchable": true, "visible": true, "name": "TotalECSubmitted" },
                { data: "TotalECSigned", "searchable": true, "visible": true, "name": "TotalECSigned" },
                { data: "TotalCCSearched", "searchable": true, "visible": true, "name": "TotalCCSearched" },
                { data: "TotalCCSubmitted", "searchable": true, "visible": true, "name": "TotalCCSubmitted" },
                { data: "TotalCCSigned", "searchable": true, "visible": true, "name": "TotalCCSigned" },
                //ADDED BY PANKAJ ON 11-06-2021
                { data: "AnywhereTotalECSigned", "searchable": true, "visible": true, "name": "AnywhereTotalECSigned" },
                { data: "LocalTotalECSigned", "searchable": true, "visible": true, "name": "LocalTotalECSigned" },
                { data: "AnywhereTotalCCSigned", "searchable": true, "visible": true, "name": "AnywhereTotalCCSigned" },
                { data: "LocalTotalCCSigned", "searchable": true, "visible": true, "name": "LocalTotalCCSigned" },



            ],
            fnInitComplete: function (oSettings, json) {
                $("#DroName").html(json.DroName); $("#SroName").html(json.SroName); $("#FYName").html(json.FYName);
                $("#ExcelDivId").show();
                colNameYear
                if (_MonthCode == 0)
                    $("#colNameYear").html("Year");
                else
                    $("#colNameYear").html("Month");
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
        summaryTable.columns.adjust().draw();
    }
}

function LoadDetailsTable() {
    _SroCode = $("#SROOfficeListID").val();
    _DROCode = $("#DroListID").val();
    _FinancialyearCode = $("#financialYearListID").val();
    _MonthCode = $("#MonthListID").val();
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    //added by mayank content 23-06-2021 for ECCC Statistics
    var SearchBy = $('input[name="RDOBtnFilter"]:checked').val();
    var _SearchByParameter = "";
    if (SearchBy != undefined) {
        if (SearchBy == "DurationWise")
            _SearchByParameter = "SearchDurationWise";

        if (SearchBy == "OfficeWise")
            _SearchByParameter = "SearchOfficeWise";
    }
    //end 23-06-2021 for ECCC Statistics 

    if (_FinancialyearCode == 0) {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Please Select Financial Year' + '</span>')
    }
    else {
        detailsTable = $('#DetailTable').DataTable({
            "ajax": {
                "url": "/MISReports/ECCCSearchStatistics/GetDetails",
                "type": "POST",
                headers: header,
                data: { SroCode: _SroCode, DROCode: _DROCode, FinancialyearCode: _FinancialyearCode, MonthCode: _MonthCode, SearchByParameter: _SearchByParameter },
                //"datatype": "json",
                dataSrc: function (json) {
                    unBlockUI();
                    if (json.errorMessage != null) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            callback: function () {
                                if (json.serverError != undefined) {
                                    window.location.href = "/Home/HomePage";
                                }
                                else {
                                    var classToRemove = $('#DtlsToggleIconSearchParaDetail').attr('class');
                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                        $('#DtlsToggleIconSearchParaDetail').trigger('click');
                                    $("#DetailTable").DataTable().clear().destroy();

                                }
                            }
                        });
                    }
                    else {
                        var classToRemove = $('#DtlsToggleIconSearchParaDetail').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                            $('#DtlsToggleIconSearchParaDetail').trigger('click');
                    }
                    unBlockUI();
                    return json.data;
                },

                error: function () {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    unBlockUI();
                },
                beforeSend: function () {
                    blockUI('loading data.. please wait...');
                    var searchString = $('#DetailTable_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#DetailTable_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                detailsTable.search('').draw();
                                $("#DetailTable_filter input").prop("disabled", false);
                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
            },
            "iDisplayLength": 10,
            serverSide: true,
            "scrollY": "300px",
            "scrollCollapse": true,
            //bPaginate: true,
            bPaginate: false,
            paging: false,
            bLengthChange: true,
            bInfo: true,
            info: true,
            bFilter: false,
            searching: true,
            "destroy": true,
            "bAutoWidth": true,
            "bScrollAutoCss": true,

            //Added by Madhusoodan on 10/08/2020
            columnDefs: [
                { targets: "_all", orderable: false, "className": "text-center" }
            ],

            columns: [

                { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
                { data: "MonthYear", "searchable": true, "visible": true, "name": "MonthYear" },
                { data: "TotalUserLogged", "searchable": true, "visible": true, "name": "TotalUserLogged" },
                { data: "TotalECSearched", "searchable": true, "visible": true, "name": "TotalECSearched" },
                { data: "TotalECSubmitted", "searchable": true, "visible": true, "name": "TotalECSubmitted" },
                { data: "TotalECSigned", "searchable": true, "visible": true, "name": "TotalECSigned" },
                { data: "TotalCCSearched", "searchable": true, "visible": true, "name": "TotalCCSearched" },
                { data: "TotalCCSubmitted", "searchable": true, "visible": true, "name": "TotalCCSubmitted" },
                { data: "TotalCCSigned", "searchable": true, "visible": true, "name": "TotalCCSigned" },
                //ADDED BY PANKAJ ON 11-06-2021
                { data: "AnywhereTotalECSigned", "searchable": true, "visible": true, "name": "AnywhereTotalECSigned" },
                { data: "LocalTotalECSigned", "searchable": true, "visible": true, "name": "LocalTotalECSigned" },
                { data: "AnywhereTotalCCSigned", "searchable": true, "visible": true, "name": "AnywhereTotalCCSigned" },
                { data: "LocalTotalCCSigned", "searchable": true, "visible": true, "name": "LocalTotalCCSigned" },

            ],


            fnInitComplete: function (oSettings, json) {
                $("#MonthName").html(json.MonthName);
                $("#DroName").html(json.DroName); $("#SroName").html(json.SroName); $("#FYName").html(json.FYName);
                //console.log(json);
                //$("#thTotalUserLoged").html(json.TotalUserLogged);
                //$("#thTotalEcSearch").html(json.TotalECSearched);
                //$("#thTotalEcSubmitted").html(json.TotalECSubmitted);
                //$("#thTotalEcSigned").html(json.TotalECSigned);
                //$("#thTotalCcSearch").html(json.TotalCCSearched);
                //$("#thTotalCcSubmitted").html(json.TotalCCSubmitted);
                //$("#thTotalCcSigned").html(json.TotalCCSigned);
                //$("#thTotalAnywhereEcSearch").html(json.AnywhereTotalECSigned);
                //$("#thTotalLocalEcSearch").html(json.LocalTotalECSigned);
                //$("#thTotalAnywhereCcSearch").html(json.AnywhereTotalCCSigned);
                //$("#thTotalLocalCcSearch").html(json.LocalTotalCCSigned);

                //var api=this.api()
                (detailsTable.columns(0).footer())[0].innerText = "Total";
                (detailsTable.columns(1).footer())[0].innerText = "Total";
                (detailsTable.columns(2).footer())[0].innerText = json.TotalUserLogged;
                (detailsTable.columns(3).footer())[0].innerText = json.TotalECSearched;
                (detailsTable.columns(4).footer())[0].innerText = json.TotalECSubmitted;
                (detailsTable.columns(5).footer())[0].innerText = json.TotalECSigned;
                (detailsTable.columns(6).footer())[0].innerText = json.TotalCCSearched;
                (detailsTable.columns(7).footer())[0].innerText = json.TotalCCSubmitted;
                (detailsTable.columns(8).footer())[0].innerText = json.TotalCCSigned;
                (detailsTable.columns(9).footer())[0].innerText = json.AnywhereTotalECSigned;
                (detailsTable.columns(10).footer())[0].innerText = json.LocalTotalECSigned;
                (detailsTable.columns(11).footer())[0].innerText = json.AnywhereTotalCCSigned;
                (detailsTable.columns(12).footer())[0].innerText = json.LocalTotalCCSigned;


                if (SearchBy == "DurationWise") {
                    if (_MonthCode == 0)
                        $("#colNameMonth").html("Month");
                    else
                        $("#colNameMonth").html("Date");
                }
                else {
                    $("#colNameMonth").html("Office Name");
                }
                // $("#SroName").html(json.SroName); $("#FYName").html(json.FYName);

                $("#ExcelDivId").show();
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
        detailsTable.columns.adjust().draw();
    }
}

function GetSummaryAndDetails() {


    if ($("#financialYearListID").val() != 0) {

        //Added by Madhusoodan on 10/08/2020
        //Commeneted by mayank on 23/06/2021
        //$("#SummaryTable").show();
        $("#DetailTable").show();
        //Commeneted by mayank on 23/06/2021
        //LoadSummaryTable();
        LoadDetailsTable();

    }
    else {
        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Select Financial Year' + '</span>')
    }
}

function EXCELDownloadFun() {
    _SroCode = $("#SROOfficeListID").val();
    _DROCode = $("#DroListID").val();
    _FinancialyearCode = $("#financialYearListID").val();
    _MonthCode = $("#MonthListID").val();

    //added by mayank content 23-06-2021 for ECCC Statistics
    var SearchBy = $('input[name="RDOBtnFilter"]:checked').val();
    var _SearchByParameter = "";
    if (SearchBy != undefined) {
        if (SearchBy == "DurationWise")
            _SearchByParameter = "SearchDurationWise";

        if (SearchBy == "OfficeWise")
            _SearchByParameter = "SearchOfficeWise";
    }
    //end 23-06-2021 for ECCC Statistics 

    window.location.href = '/MISReports/ECCCSearchStatistics/ExportSummaryToExcel?SroCode=' + _SroCode + "&DROCode=" + _DROCode + "&FinancialyearCode=" + _FinancialyearCode + "&MonthCode=" + _MonthCode + "&SearchByParameter=" + _SearchByParameter;
}

function HideTable() {
    $("#DetailTable").DataTable().clear().destroy();
    //Commeneted by mayank on 23/06/2021
    //$("#SummaryTable").DataTable().clear().destroy();
    $("#ExcelDivId").hide();
    $("#DroName").html("");
    $("#SroName").html("");
    $("#FYName").html("");
    $("#MonthName").html("");
    (detailsTable.columns(0).footer())[0].innerText = "Total";
    (detailsTable.columns(1).footer())[0].innerText = "Total";
    (detailsTable.columns(2).footer())[0].innerText = "0";
    (detailsTable.columns(3).footer())[0].innerText = "0";
    (detailsTable.columns(4).footer())[0].innerText = "0";
    (detailsTable.columns(5).footer())[0].innerText = "0";
    (detailsTable.columns(6).footer())[0].innerText = "0";
    (detailsTable.columns(7).footer())[0].innerText = "0";
    (detailsTable.columns(8).footer())[0].innerText = "0";
    (detailsTable.columns(9).footer())[0].innerText = "0";
    (detailsTable.columns(10).footer())[0].innerText = "0";
    (detailsTable.columns(11).footer())[0].innerText = "0";
    (detailsTable.columns(12).footer())[0].innerText = "0";
    //$("#thTotalUserLoged").html('');
    //$("#thTotalEcSearch").html('');
    //$("#thTotalEcSubmitted").html('');
    //$("#thTotalEcSigned").html('');
    //$("#thTotalCcSearch").html('');
    //$("#thTotalCcSubmitted").html('');
    //$("#thTotalCcSigned").html('');
    //$("#thTotalAnywhereEcSearch").html('');
    //$("#thTotalLocalEcSearch").html('');
    //$("#thTotalAnywhereCcSearch").html('');
    //$("#thTotalLocalCcSearch").html('');
}

