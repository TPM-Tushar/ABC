//Global variables.
var token = '';
var header = {};

$(document).ready(function () {
    
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    //Added by Madhusoodan on 21-09-2020
    $('#btnSearch').show();
    $('#btnDistinctFRN').hide();


    $('#CCConversionLogTable').hide();

    $('#CCConversionCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove === "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
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

    
    $('#txtFromDate').datepicker({
        format: 'dd-mm-yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", dFromDate);

    $('#txtToDate').datepicker({
        format: 'dd-mm-yyyy'
    }).datepicker("setDate", dToDate);


    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    $('#btnSearch').click(function () {
        var FromDate = $('#txtFromDate').val();
        var ToDate = $('#txtToDate').val();

        $('#CCConversionLogTable').show();

        //Added by Madhusoodan on 21-09-2020
        var DistinctLogs = false;       //To show all distinct records in datatable

        GetCCConversionLogs(DistinctLogs);

        $('#btnSearch').hide();
        $('#btnDistinctFRN').show();
    });

    $('#btnDistinctFRN').click(function () {
        $('#CCConversionLogTable').show();
        var DistinctLogs = true;       //To show only distinct records in datatable
        
        GetCCConversionLogs(DistinctLogs);
    });

    $('#txtFromDate').change(function () {
        $('#btnSearch').show();
        $('#btnDistinctFRN').hide();

    });

    $('#txtToDate').change(function () {
        $('#btnSearch').show();
        $('#btnDistinctFRN').hide();
    });

    $('#DocTypeID').change(function () {
        $('#btnSearch').show();
        $('#btnDistinctFRN').hide();
    });
    
});

function GetCCConversionLogs(DistinctLogs) {
    var FromDate = $('#txtFromDate').val();
    var ToDate = $('#txtToDate').val();
    
    //Added by Madhusoodan on 18-09-2020
    var DocumentTypeId = $('#DocTypeID option:selected').val();

    $("#CCConversionLogTable").DataTable().destroy();

    var CCConversionLogTable = $('#CCConversionLogTable').DataTable({
        "ajax": {
            "url": "/Remittance/CCConversionLog/CCConversionLogDetails",
            "type": "POST",
            headers: header,
            data: {
                'fromDate': FromDate,
                'ToDate': ToDate,
                'DocTypeId': DocumentTypeId,
                'DistinctLogs': DistinctLogs
            },
            datatype: "json",
            //"datatype": "json",
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError !== undefined) {
                                window.location.href = "/Home/HomePage";
                            }
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                                    $('#DtlsToggleIconSearchPara').trigger('click');

                                }
                                $("#CCConversionLogTable").DataTable().clear().destroy();

                                //Added by Madhusoodan on 07-10-2020
                                $('#btnSearch').show();
                                $('#btnDistinctFRN').hide();
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
                var searchString = $('#CCConversionLogTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#CCConversionLogTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            CCConversionLogTable.search('').draw();
                            $("#CCConversionLogTable_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },
        "iDisplayLength": 50,
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
        "bSort": true,
        columnDefs: [
            
            { targets: "_all", orderable: true, "className": "text-center" }
            
        ],
        columns: [
            { "data": "SrNo", "name": "SrNo", "autoWidth": true, "visible": true },
            { "data": "LogID", "name": "LogID", "searchable": true, "visible": true, "autoWidth": true },
            { "data": "CCID", "name": "CCID", "autoWidth": true, "searchable": true, "visible": true },
            { "data": "UserID", "name": "UserID", "searchable": true, "visible": true, "autoWidth": true },
            { "data": "UserName", "name": "UserName", "searchable": true, "visible": true, "autoWidth": true },
            { "data": "SROCode", "name": "SROCode", "searchable": true, "autoWidth": true },
            { "data": "DocumentID", "name": "DocumentID", "searchable": true, "visible": true, "autoWidth": true },
            { "data": "FinalRegistrationNumber", "name": "FinalRegistrationNumber", "searchable": true, "autoWidth": true },
            {
                "data": "LogDateTime", "name": "LogDateTime", "autoWidth": true, "searchable": true,
                "render": function (data) {
                    if (data === null) return "";
                    return data;
                }
            },
            { "data": "IsConvertedUsingImgMagick", "name": "IsConvertedUsingImgMagick", "autoWidth": true, "searchable": true }
            
        ],
        fnInitComplete: function (oSettings, json) {
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
        }

    });
}

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
