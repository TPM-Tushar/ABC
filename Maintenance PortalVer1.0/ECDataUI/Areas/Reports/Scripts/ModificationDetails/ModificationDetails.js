$(document).ready(function () {
    $('#Captcha').focus();
    $(window).keydown(function (event) {
        var hasFocus = $('#Captcha').is(':focus');

        if (event.keyCode == 13) {
            if ($("#Captcha").val() == "") {
                event.preventDefault();
                return false;
            }
            else {
                if (hasFocus) {
                    $("#btnSearch").trigger("click");
                    $('#Captcha').blur();
                }
            }
        }
    });
    $('#txtFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

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
        minDate: new Date(),
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

    }).datepicker("setDate", 'now');

    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',

    }).datepicker("setDate", 'now');//'01/12/2018'
    $("#btnSearch").click(function () {
        ShowSROModificationDetailsData();
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


function ShowSROModificationDetailsData() {
    var programs = $("#ddProgramList").val() + "";
    if (programs == "") {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select application name" + '</span>',
        });

    }
    if (!($("#SearchParametersForm").valid())) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please enter all required fields" + '</span>',
        });
        return;
    }
    var fromDate = $("#txtFromDate").val();
    var ToDate = $("#txtToDate").val();
    var selectedOfc = $("#ddOfficeList option:selected").val();
    var selectedOfficeName = $("#ddOfficeList option:selected").text();
    var tableElement = $('#StatTableDataID');
    if ($.fn.DataTable.isDataTable("#StatTableDataID")) {
        tableElement.dataTable().fnDestroy();
    }
    tableElement.DataTable({
        ajax: {
            url: "/ModificationDetails/GetModificationDetailsList",
            type: "POST",
            data: {
                'fromDate': fromDate, 'ToDate': ToDate, 'selectedOfc': selectedOfc, 'programs': programs, 'captcha': $("#Captcha").val()
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
                            window.location.reload(true);
                        }
                    });
                }
                else {
                unBlockUI();
                return json.data;
                }
            },
            error: function () {
                unBlockUI();
                refreshCaptcha();
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Something Went Wrong!" + '</span>');
                window.location.reload(true);
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');
            }
        },
        buttons: [
            {
                extend: 'pdf',
                text: '<i class="fa fa-file-pdf-o"></i> PDF',
                exportOptions: {
                    columns: ':not(.no-print)'
                },
                action:
                function (e, dt, node, config) {
                    this.disable();
                    window.location = '/ModificationDetails/ExportModificationDetailsLogList?FromDate=' + fromDate + "&ToDate=" + ToDate + "&SelectedOfc=" + selectedOfc + "&Programs=" + programs + "&OfficeName=" + selectedOfficeName;
                }
            },
            {
                extend: 'excel',
                action: function (e, dt, node, config) {
                    this.disable();
                    window.location = '/ModificationDetails/ExportModificationDetailsLogListToExcel?FromDate=' + fromDate + "&ToDate=" + ToDate + "&SelectedOfc=" + selectedOfc + "&Programs=" + programs + "&OfficeName=" + selectedOfficeName;
                }
            }

        ],
        scrollY: "400px",
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        "pageLength": -1,
        sScrollXInner: "130%",
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        dom: 'lBfrtip',
        columns: [
            { data: 'SroName', bSortable: true },
            { data: 'DateOfEvent', bSortable: true },
            { data: 'ModificationType', bSortable: true },
            { data: 'Loginname', bSortable: true },
            { data: 'IPAddress', bSortable: true },
            { data: 'HostName', bSortable: true },
            { data: 'PhysicalAddress', bSortable: true },
            { data: 'ApplicationName', bSortable: true },
            {
                data: null,
                render: function (data, type, row) {
                    return "<div class='text-center'><span style='cursor:pointer;' class='glyphicon glyphicon-search' title='Click here to View details' onClick ='ViewDetails(\"" + data.EncryptedMasterID + "\",\"" + data.EncryptedDetailsID + "\")' /><div>";
                },
                bSortable: false
            }
        ]//Columns Ends Here
        ,
        autoWidth: false,
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
        fnInitComplete: function (oSettings, json) {
            unBlockUI();
            if (json.status == "0") {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                    callback: function () {
                        refreshCaptcha();
                    }
                });
            }
            else {
                refreshCaptcha();
            }
            $(".dt-button").addClass("btn btn-primary dt-button buttons-pdf buttons-html5");
            $('.dataTables_scrollBody thead tr').css({ visibility: 'collapse' });
        }
    });

}
function ViewDetails(EncryptedMasterID, EncryptedDetailsID) {
    $.ajax({
        type: 'POST',
        datatype: 'html',
        url: '/ModificationDetails/GetModificationTypeDetails',
        async: false,
        cache: false,
        data: { 'EncryptedMasterID': EncryptedMasterID, 'EncryptedDetailsID': EncryptedDetailsID },
        success: function (data) {
            $("#ModificationsMasterTableModel").html(data);
            $("#ModificationsMasterTableModel").modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            window.location.href = "/ECDataAuditDetails/Error";
        }
    });
}


function refreshCaptcha() {
    $('#Captcha').val('');
    $("#dvCaptcha input").val('');
    $(".newCaptcha").trigger('click');
}
