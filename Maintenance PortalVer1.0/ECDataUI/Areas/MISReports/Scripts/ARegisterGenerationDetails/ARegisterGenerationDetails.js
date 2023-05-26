var FromDate;
var ToDate;
var SROOfficeListID;
//Global variables.
var token = '';
var header = {};
var SelectedSRO;
var SelectedDistrict;
var DROOfficeListID;
var SROOfficeListID;
var FrommDate;
var ToDate;
$(document).ready(function () {
  
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DROOfficeListID').change(function () {
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/ARegisterGenerationDetails/GetSROOfficeListByDistrictID',
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
    }).datepicker("setDate", ToDate);


    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true,

    }).datepicker("setDate", FromDate);

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });
});


function SearchARegisterGenDet() {
   // alert("In SearchARegisterGenDet ");
    FromDate = $("#txtFromDate").val();
    ToDate = $("#txtToDate").val();
    //alert("FromDate" + FromDate);
    //alert("ToDate" + ToDate);
    SROfficeID = $("#SROOfficeListID option:selected").val();
    DROfficeID = $("#DROOfficeListID option:selected").val();
    NGReport = $("input[name='NGReport']:checked").val();
    //alert("NGReport__" + NGReport);
   // alert("SROfficeID" + SROfficeID);
   // alert("DROfficeID" + DROfficeID);
    var tableARegisterReportDetails = $('#ARegRepGenTableID').DataTable({
        ajax: {

            url: '/MISReports/ARegisterGenerationDetails/GetARegisterGenerationReportsDetails',
            type: "POST",
            headers: header,
            data: {
                'FromDate': FromDate, 'ToDate': ToDate, 'SROfficeID': SROfficeID, 'DROfficeID': DROfficeID, 'NGReport': NGReport
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
                                $("#ARegRepGenTableID").DataTable().clear().destroy();

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
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
            
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#ARegRepGenTableID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#ARegRepGenTableID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DROfficeWiseSummaryTableID.search('').draw();
                            $("#ARegRepGenTableID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
        
}
                }
        },
        serverSide: true,
        "scrollX": false,
        "scrollY": "250px",
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        "destroy": true,
        "bAutoWidth": true,
    
   
        columns: [

            { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width": "2%"},
            { data: "DistrictName", "searchable": true, "visible": true, "name": "DistrictName", "width": "5%" },
            { data: "SroName", "searchable": true, "visible": true, "name": "SroName", "width": "5%" },
            { data: "Receipt_Date", "searchable": true, "visible": true, "name": "Receipt_Date", "width": "5%"},
            { data: "IsReceiptsSynchronized", "searchable": true, "visible": true, "name": "IsReceiptsSynchronized", "width": "4%" },
            { data: "File_Gen_Date", "searchable": true, "visible": true, "name": "File_Gen_Date", "width": "5%" },
            { data: "File_Path", "searchable": true, "visible": true, "name": "File_Path", "width": "40%" }

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
        },
    });
}