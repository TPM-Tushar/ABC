//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $("#btnSearch").click(function () {
        showData("1");
        // $("#OfficeWiseTblWrapper").css({ overflow: 'auto' });
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
        //yearRange: '2018:2019',
        //minDate: '07/12/2018',
        // minDate: new Date(2018, 12, 7),
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

    }).datepicker("setDate", '31/12/2018');

    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", '07/12/2018');


    $("#SearchParaListCollapse").click(function () {

        var classToRemoveSearchPara = $('#ToggleIconSearchPara').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#ToggleIconSearchPara').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

            $('#ToggleIconSearchPara').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

    });


    $("#DtlsSearchParaListCollapse").click(function () {

        var classToRemoveSearchPara = $('#DtlsToggleIconSearchPara').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsToggleIconSearchPara').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

            $('#DtlsToggleIconSearchPara').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

    });


    $("#ddProgramList").multiselect({
        includeSelectAllOption: true,
        numberDisplayed: 0,
        nSelectedText: 'selected',
        buttonText: function (options, select) {
            var numberOfOptions = $("#ddProgramList").children('option').length;

            if (options.length > 0) {
                if (options.length === numberOfOptions) {

                    return ' All Selected';
                }
                return options.length + ' Selected';
            }
            else {
                return 'None selected';
            }
        }

    });
    $('.multiselect').addClass('minimal');
    $('.multiselect').addClass('btn-sm');

});

//Block UI
function BlockUI() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}


function showData(PopulateOccurances) {
    var programs = $("#ddProgramList").val() + "";
    if (programs == "") {
        bootbox.alert({
            //   size: 'small',
            //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select application name" + '</span>',
            callback: function () {

            }
        });

    }
    if (PopulateOccurances == "1" && !($("#SearchParametersForm").valid())) {
        //bootbox.alert({
        //    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i> <span class="boot-alert-txt">'+"Please enter all required fields"+'</span>'
        //});
        bootbox.alert({
            //   size: 'small',
            //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please enter all required fields" + '</span>',
            callback: function () {
                refreshCaptcha();
            }
        });
        return;
    }
    if ($("#DtlsSearchParaListCollapse").hasClass("collapsed")) {

        $("#DtlsSearchParaListCollapse").trigger('click');

    }
    var fromDate = $("#txtFromDate").val();
    var ToDate = $("#txtToDate").val();
    var selectedOfc = $("#ddOfficeList option:selected").val();
    var selectedOfficeName = $("#ddOfficeList option:selected").text();
    var responsiveHelper;
    var breakpointDefinition = {
    };
    var tableElement = $('#StatTableDataID');
    if ($.fn.DataTable.isDataTable("#StatTableDataID")) {

        tableElement.dataTable().fnDestroy();
    }
    $("#PDFSPANID").show();

    tableElement.DataTable({

        ajax: {
            url: '/LogAnalysis/ECDataAuditDetails/GetECDataAuditDetailsList',
            type: "POST",
            headers: header,

            data: {
                'fromDate': fromDate, 'ToDate': ToDate, 'selectedOfc': selectedOfc, 'PopulateOccurances': PopulateOccurances, 'captcha': $("#Captcha").val()
                , 'programs': programs, 'selectedOfficeName': selectedOfficeName
            },
            dataSrc: function (json) {
                if ($("#DtlsSearchParaListCollapse").hasClass("collapsed")) {
                    $("#DtlsSearchParaListCollapse").trigger('click');
                }
                if (json.recordsFiltered == 0) {
                    unBlockUI();
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            //window.location.reload(true);
                            $("#OfficeWiseTbl").hide();
                            if ($("#DtlsSearchParaListCollapse").hasClass("collapsed")) {
                            }
                            else
                            {
                                $("#DtlsSearchParaListCollapse").trigger('click');
                                if ($.fn.DataTable.isDataTable("#StatTableDataID"))
                                {
                                    tableElement.dataTable().fnDestroy();
                                }
                            }

                            if ($("#SearchParaListCollapse").hasClass("collapsed"))
                            {
                            }
                            else
                            {
                                $("#SearchParaListCollapse").trigger('click');
                            }
                            $("#PDFSPANID").hide();
                        }
                    });
                }
                else {
                    unBlockUI();
                    return json.data;
                }
            },
            error: function (xhr, status, error) { //xhr, ajaxOptions, thrownError
                unBlockUI();
                refreshCaptcha();

                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something Went Wrong!" + '</span>');
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');
            }
        },
        serverSide: true,
        pageLength: 100,
        scrollY: "400px",
        scrollCollapse: true,
        bPaginate: true,
        //bLengthChange: true,
        // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        //"lengthMenu": [[10, 25, 50], [10, 25, 50]],
        bLengthChange: false,
        sScrollX: "100%",
        //"pageLength": -1,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: false,
        //dom: 'lBfrtip',
        //Commented and Modified by Harshit on 15 Jan 2018 to divert call to Server
        //buttons: [
        //   // { extend:'copy', attr: { id: 'allan' } }, 'csv', 'excel', 'pdf', 'print'
        // //'excel', 'pdf'

        //    {
        //        //extend: 'pdfHtml5',
        //        orientation: 'landscape',
        //        pageSize: 'LEGAL',


        //        extend: 'pdf',
        //        text: '<i class="fa fa-file-pdf-o"></i> PDF',
        //        title: $('h1').text(),
        //        exportOptions: {
        //            columns: ':not(.no-print)'
        //        },
        //        footer: true
        //    },
        //    {

        //        extend:'excel'
        //    }

        //],
        //buttons: [
        //    {
        //        extend: 'pdf',
        //        text: '<i class="fa fa-file-pdf-o"></i> PDF',
        //        exportOptions: {
        //            columns: ':not(.no-print)'
        //        },
        //        action:
        //        function (e, dt, node, config) {
        //            this.disable();
        //            window.location = '/LogAnalysis/ECDataAuditDetails/ExportECDataModificationInfoToPDF?FromDate=' + fromDate + "&ToDate=" + ToDate + "&SelectedOfc=" + selectedOfc + "&Programs=" + programs + "&OfficeName=" + selectedOfficeName;
        //        }
        //    }
        //    //,
        //    //{
        //    //    extend: 'excel',
        //    //    action: function (e, dt, node, config) {
        //    //        this.disable();
        //    //        window.location = '/LogAnalysis/ECDataAuditDetails/ExportECDataModificationInfoToExcel?FromDate=' + fromDate + "&ToDate=" + ToDate + "&SelectedOfc=" + selectedOfc + "&Programs=" + programs + "&OfficeName=" + selectedOfficeName;
        //    //    }
        //    //}

        //],
        columns: [
            { data: "SRONAME", "searchable": true, "orderable": false, "visible": true },
            { data: "FRN", "searchable": true, "orderable": true, "visible": true },
            { data: "DATEOFMODIFICATION", "searchable": true, "orderable": false, "visible": true },
            { data: "MODIFICATION_AREA", "searchable": true, "orderable": false, "visible": true },
            { data: "MODIFICATION_TYPE", "searchable": true, "orderable": false, "visible": true },
            {
                data: null,
                className: "center",
                sorting: "false",
                render: function (data, type, row) {
                    return data.MODIFICATION_DESCRIPTION;
                },
            },
            { data: "APPLICATION_NAME", "searchable": true, "orderable": false, "visible": true },
            { data: "IPADRESS", "searchable": true, "orderable": false, "visible": true },
            { data: "hostname", "searchable": true, "orderable": false, "visible": true }
        ],
        autoWidth: true,
        preDrawCallback: function () {
            //if (!responsiveHelper) {
            //    responsiveHelper = new ResponsiveDatatablesHelper(tableElement, breakpointDefinition);
            //}
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            //responsiveHelper.createExpandIcon(nRow);
            // $("td:first", nRow).html(iDisplayIndex + 1);
            //   $("td:first", nRow).css('text-align', 'center');

            unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
            //responsiveHelper.respond();
            unBlockUI();
        },
        fnInitComplete: function (oSettings, json) {
            unBlockUI();
            if (json.status == "0") {
                bootbox.alert({
                    //   size: 'small',
                    //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                    callback: function () {
                        refreshCaptcha();
                    }
                });
            }
            else {
                refreshCaptcha();
                $("#PDFSPANID").html(json.PDFDownloadBtn);
            }

            if (PopulateOccurances == "1" && json.status == "1") {
                $("#OfficeWiseTbl").show();
                // set officewise code here
                var trConcatinated = "";
                for (i = 0; i < json.OfficeWiseOccurance.length; i++) {
                    trConcatinated += "<tr><td style='cursor:pointer;' class='text-success' onclick='LoadOfficeWise(" + json.OfficeWiseOccurance[i].SROCode + ")'>" + json.OfficeWiseOccurance[i].SROName + "</td><td>" + json.OfficeWiseOccurance[i].NoOfOccurances + "</td><td>" + json.OfficeWiseOccurance[i].LastModifiedDateTime + "</td></tr>";
                }
                $("#OfficeWiseTbl tbody").empty();
                $("#OfficeWiseTbl tbody").append(trConcatinated);

                if ($("#SearchParaListCollapse").hasClass("collapsed")) {

                    $("#SearchParaListCollapse").trigger('click');

                }





                $("#TotModfctnOfcCount").html(json.OfficeWiseOccurance.length);
                $("#TotModfctnCount").html(json.TotModfctnCount);
            }

            $(".dt-button").addClass("btn btn-primary dt-button buttons-pdf buttons-html5");
            //$(".dt-button").css({ float: 'left' });
            $('.dataTables_scrollBody thead tr').css({ visibility: 'collapse' });
        }
    });




}

function refreshCaptcha() {
    $('#Captcha').val('');
    $("#dvCaptcha input").val('');
    $(".newCaptcha").trigger('click');
}



function LoadMasterTables(LogID, LOGTYPEID, SROCode, ItemID, SroName, ModificationArea, FRN) {
    // $('#MasterTableModel').load('/ECDataAuditDetails/LoadMasterTables?LogID=' + LogID + "&LOGTYPEID=" + LOGTYPEID + "&SROCode=" + SROCode + "&ItemID=" + ItemID + "&FRN=" + FRN + "&ModificationArea=" + ModificationArea);
    $('#MasterTableModel').load('/ECDataAuditDetails/LoadMasterTables', { LogID: LogID, LOGTYPEID: LOGTYPEID, SROCode: SROCode, ItemID: ItemID, SroName: SroName, ModificationArea: ModificationArea, FRN: FRN });
    $('#MasterTableModel').modal('show');
}


function LoadOfficeWise(officeId) {

    $("#ddOfficeList").val(officeId);

    showData("0");
}

function PDFDownloadFun(fromDate, ToDate, selectedOfc, programs, selectedOfficeName) {

    window.location.href = '/LogAnalysis/ECDataAuditDetails/ExportECDataModificationInfoToPDF?FromDate=' + fromDate + "&ToDate=" + ToDate + "&SelectedOfc=" + selectedOfc + "&Programs=" + programs + "&OfficeName=" + selectedOfficeName;
}
