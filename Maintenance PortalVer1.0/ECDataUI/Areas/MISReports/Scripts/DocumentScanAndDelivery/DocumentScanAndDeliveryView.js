var token = '';
var header = {};
//var SelectedSROText;
var txtFromDate;
var txtToDate;
var SroID;
var DistrictID;
var SelectedType;
var DocumentTypeText;
var DocumentTypeId;
//var SelectedDistrictText;
//var SelectedLogTypeText;

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    //var asd = Document.getElementById('DROOfficeListID');
    //asd.value='DRO';
    $('#ID_DR_Lbl').html("DRO");
    $('input[type=radio][name=RDOBtnFilter]').change(function () {

        $('#EXCELSPANID').html('');

        if ($.fn.DataTable.isDataTable("#AnywhereECTable"))
        {
            $("#AnywhereECTable").DataTable().clear().destroy();
            $("#AnywhereECTable").hide();
        }
        if ($.fn.DataTable.isDataTable("#DocScannedAndDeliveryTableID"))
        {
            $("#DocScannedAndDeliveryTableID").DataTable().clear().destroy();
            $("#DocScannedAndDeliveryTableID").hide();
        }
        SelectedType = $('input[name="RDOBtnFilter"]:checked').val();

        if (SelectedType == "SR")
        {
            $('#DocTypeDropDownListID').show();
            if (IsSr == "True")
            {
                $('#RdoBtnId').hide();
                $('#DRRowId').hide();
                $('#SRRowId').hide();
                $('#SRLoginViewId').show();
            }
            else
            {
                $('#DRRowId').hide();
                $('#SRRowId').show();
                $('#SRLoginViewId').hide();
            }
           
            //$('#DRODropDownListID').show();
            //$('#SRODropDownListID').show();
        }
        else if (SelectedType == "DR")
        {
            $('#DocTypeDropDownListID').hide();
            $('#DRRowId').show();
            $('#SRLoginViewId').hide();
            $('#SRRowId').hide();
            //$('#DRODropDownListID').show();
            //$('#SRODropDownListID').show();
        }
    });

    //Added by RamanK to hide rdo btns in case of SR and DR
    if (IsSr == 'True')
    {
        //$('input:radio[name=RDOBtnFilter][value=SR]').trigger('click');        
        $('#RdoBtnId').hide();
        $('#DRRowId').hide();
        $('#SRRowId').hide();
        $('#SRLoginViewId').show();
    }
    else if (IsDr == 'True')
    {
        $('input:radio[name=RDOBtnFilter][value=DR]').trigger('click');
    }
    else
    {
        $('input:radio[name=RDOBtnFilter][value=DR]').trigger('click');
    }

    $('#DROOfficeListID').change(function () {
        $('#EXCELSPANID').html('');

        if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
            $("#AnywhereECTable").DataTable().clear().destroy();
            $("#AnywhereECTable").hide();
        }
        if ($.fn.DataTable.isDataTable("#DocScannedAndDeliveryTableID"))
        {
            $("#DocScannedAndDeliveryTableID").DataTable().clear().destroy();
            $("#DocScannedAndDeliveryTableID").hide();
        }

      
    });

    $('#DROOfficeListIDForSR').change(function () {
        $('#EXCELSPANID').html('');

        if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
            $("#AnywhereECTable").DataTable().clear().destroy();
            $("#AnywhereECTable").hide();
        }
        if ($.fn.DataTable.isDataTable("#DocScannedAndDeliveryTableID")) {
            $("#DocScannedAndDeliveryTableID").DataTable().clear().destroy();
            $("#DocScannedAndDeliveryTableID").hide();
        }
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/DocumentScanAndDelivery/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROOfficeListIDForSR').val() },
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
                    $('#SROOfficeListIDForSR').empty();
                    $.each(data.SROOfficeList, function (i, SROOfficeList) {
                        SROOfficeList
                        $('#SROOfficeListIDForSR').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });

    });
    $('#SROOfficeListID').change(function () {
        $('#EXCELSPANID').html('');

        if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
            $("#AnywhereECTable").DataTable().clear().destroy();
            $("#AnywhereECTable").hide();
        }
        if ($.fn.DataTable.isDataTable("#DocScannedAndDeliveryTableID")) {

            $("#DocScannedAndDeliveryTableID").hide();
            $("#DocScannedAndDeliveryTableID").DataTable().clear().destroy();

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
    }).datepicker("setDate", ToDate);
    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", FromDate);

    //***********************************************************************

    $('#txtFromDateForSRO').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,

    });
    $('#txtToDateForSro').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

    });
    $('#divFromDateForSro').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),

        pickerPosition: "bottom-left"

    });
    $('#divToDateForSro').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });
    $('#txtToDateForSro').datepicker({
        format: 'dd/mm/yyyy',
    }).datepicker("setDate", ToDate);
    $('#txtFromDateForSRO').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", FromDate);
    //***********************************************************

    $('#txtFromDateForSrLogin').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,

    });
    $('#txtToDateForSrLogin').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

    });
    $('#divFromDateForSrLogin').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),

        pickerPosition: "bottom-left"

    });
    $('#divToDateForSrLogin').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });
    $('#txtToDateForSrLogin').datepicker({
        format: 'dd/mm/yyyy',
    }).datepicker("setDate", ToDate);
    $('#txtFromDateForSrLogin').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", FromDate);
    //***********************************************************

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });
    $("#SearchBtn").click(function () {
        SelectedType = $('input[name="RDOBtnFilter"]:checked').val();
        if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
            $("#AnywhereECTable").DataTable().clear().destroy();
        }
        if ($.fn.DataTable.isDataTable("#DocScannedAndDeliveryTableID")) {
            $("#DocScannedAndDeliveryTableID").DataTable().clear().destroy();
            //$("#DocScannedAndDeliveryTableID").hide();
        }
        //To Get Same Data in PDF and Excel
        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();
        SroID = $("#SROOfficeListID option:selected").val();
        DistrictID = $("#DROOfficeListID option:selected").val();
        DocumentTypeId = $("#DocTypeID option:selected").val();
        DocumentTypeText = $("#DocTypeID option:selected").text();
        //LogTypeID = $("#LogTypeId option:selected").val();
        //SelectedSROText = $("#SROOfficeListID option:selected").text();
        //SelectedDistrictText = $("#DROOfficeListID option:selected").text();
        //SelectedLogTypeText = $("#LogTypeId option:selected").text();

        if ($('#DROOfficeListID').val() < "0" || $('#DROOfficeListID').val() === null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select District.</span>');
            return;
        }
        if ($('#SROOfficeListID').val() < "0" || $('#SROOfficeListID').val() === null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select SRO Name.</span>');
            return;
        }

        if (txtFromDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
            return;
        }
        else if (txtToDate == null) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;
        }
        if (IsSr == "True") {
            txtFromDateForSro = $("#txtFromDateForSrLogin").val();
            txtToDateForSro = $("#txtToDateForSrLogin").val();
            if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
                $("#AnywhereECTable").DataTable().clear().destroy();
            }
            $("#AnywhereECTable").hide();
            $("#DocScannedAndDeliveryTableID").show();
            var DocScannedAndDeliveryTable = $('#DocScannedAndDeliveryTableID').DataTable({
                ajax: {
                    url: '/MISReports/DocumentScanAndDelivery/DocumentScanAndDeliveryDetailsForSRO',
                    type: "POST",
                    headers: header,
                    data: {
                        'FromDate': txtFromDateForSro,
                        'ToDate': txtToDateForSro,
                        'DistrictID': DistrictID,
                        'SroID': SroID,
                        'IsSrLogin': IsSr,
                        'DocumentTypeId': DocumentTypeId
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
                                        $("#AnywhereECTable").DataTable().clear().destroy();
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
                        unBlockUI();
                    },
                    beforeSend: function () {
                        blockUI('Loading data please wait.');
                        var searchString = $('#DocScannedAndDeliveryTableID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;
                            if (!regexToMatch.test(searchString)) {
                                $("#DocScannedAndDeliveryTableID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    DocScannedAndDeliveryTable.search('').draw();
                                    $("#DocScannedAndDeliveryTableID_filter input").prop("disabled", false);
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
                    { orderable: false, targets: [12] }
                ],
                columns: [
                    { data: "SerialNumber", "searchable": true, "visible": true, "name": "SerialNumber" },
                    { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
                    { data: "DocumentType", "searchable": true, "visible": true, "name": "DocumentType" },
                    { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
                    { data: "LocalServerStoragePath", "searchable": true, "visible": true, "name": "LocalServerStoragePath" },
                    { data: "FileUploadedToCentralServer", "searchable": true, "visible": true, "name": "FileUploadedToCentralServer" },
                    { data: "StateDataCentreStoragePath", "searchable": true, "visible": true, "name": "StateDataCentreStoragePath" },
                    { data: "SizeoftheFile", "searchable": true, "visible": true, "name": "SizeoftheFile" },
                    { data: "DateofScan", "searchable": true, "visible": true, "name": "DateofScan" },
                    { data: "DateofUpload", "searchable": true, "visible": true, "name": "DateofUpload" },
                    { data: "ArchivedinCD", "searchable": true, "visible": true, "name": "ArchivedinCD" },
                    { data: "DocumentDeliveryDate", "searchable": true, "visible": true, "name": "DocumentDeliveryDate" },
                    { data: "DateofRegistration", "searchable": true, "visible": true, "name": "DateofRegistration" },

                ],
                fnInitComplete: function (oSettings, json) {
                    //$("#PDFSPANID").html(json.PDFDownloadBtn);
                    $("#EXCELSPANID").html('');
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
        }
        if (SelectedType == "SR")
        {
            var SROIdForSRORdoBtn = $("#SROOfficeListIDForSR option:selected").val();
            var DROIdForSRORdoBtn = $("#DROOfficeListIDForSR option:selected").val();
            txtFromDateForSro = $("#txtFromDateForSRO").val();
            txtToDateForSro = $("#txtToDateForSRO").val();
            DocumentTypeId = $("#DocTypeID option:selected").val();
            DocumentTypeText = $("#DocTypeID option:selected").text();

            if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
                $("#AnywhereECTable").DataTable().clear().destroy();
            }
            $("#AnywhereECTable").hide();
            $("#DocScannedAndDeliveryTableID").show();
            var DocScannedAndDeliveryTable = $('#DocScannedAndDeliveryTableID').DataTable({
                ajax: {
                    url: '/MISReports/DocumentScanAndDelivery/DocumentScanAndDeliveryDetailsForSRO',
                    type: "POST",
                    headers: header,
                    data: {
                        'FromDate': txtFromDateForSro,
                        'ToDate': txtToDateForSro,
                        'DistrictID': DROIdForSRORdoBtn,
                        'SroID': SROIdForSRORdoBtn, 
                        'IsSrLogin': IsSr,
                        'DocumentTypeId': DocumentTypeId
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
                                        $("#AnywhereECTable").DataTable().clear().destroy();
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
                        unBlockUI();
                    },
                    beforeSend: function () {
                        blockUI('Loading data please wait.');
                        var searchString = $('#DocScannedAndDeliveryTableID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;
                            if (!regexToMatch.test(searchString)) {
                                $("#DocScannedAndDeliveryTableID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    DocScannedAndDeliveryTable.search('').draw();
                                    $("#DocScannedAndDeliveryTableID_filter input").prop("disabled", false);
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
                    { orderable: false, targets: [12] }
                ],
                columns: [
                    { data: "SerialNumber", "searchable": true, "visible": true, "name": "SerialNumber" },
                    { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
                    { data: "DocumentType", "searchable": true, "visible": true, "name": "DocumentType" },
                    { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
                    { data: "LocalServerStoragePath", "searchable": true, "visible": true, "name": "LocalServerStoragePath" },
                    { data: "FileUploadedToCentralServer", "searchable": true, "visible": true, "name": "FileUploadedToCentralServer" },
                    { data: "StateDataCentreStoragePath", "searchable": true, "visible": true, "name": "StateDataCentreStoragePath" },
                    { data: "SizeoftheFile", "searchable": true, "visible": true, "name": "SizeoftheFile" },
                    { data: "DateofScan", "searchable": true, "visible": true, "name": "DateofScan" },
                    { data: "DateofUpload", "searchable": true, "visible": true, "name": "DateofUpload" },
                    { data: "ArchivedinCD", "searchable": true, "visible": true, "name": "ArchivedinCD" },
                    { data: "DocumentDeliveryDate", "searchable": true, "visible": true, "name": "DocumentDeliveryDate" },
                    { data: "DateofRegistration", "searchable": true, "visible": true, "name": "DateofRegistration" },

                ],
                fnInitComplete: function (oSettings, json) {
                    //$("#PDFSPANID").html(json.PDFDownloadBtn);
                    $("#EXCELSPANID").html('');
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
        }
        else if (SelectedType == "DR")
        {
            if (
                $.fn.DataTable.isDataTable("#DocScannedAndDeliveryTableID")) 
                {
                    $("#DocScannedAndDeliveryTableID").DataTable().clear().destroy();
                }
                $("#DocScannedAndDeliveryTableID").hide();
                $("#AnywhereECTable").show();
                $("#AnywhereECTable").show();
                $("#AnywhereECTable").show();

            var AnywhereECTable = $('#AnywhereECTable').DataTable({
                ajax: {
                    url: '/MISReports/DocumentScanAndDelivery/DocumentScanAndDeliveryDetails',
                    type: "POST",
                    headers: header,
                    data: {
                        'FromDate': txtFromDate,
                        'ToDate': txtToDate,
                        'DistrictID': DistrictID,
                        'SroID': SroID,
                        'DocumentTypeId': DocumentTypeId

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
                                        $("#AnywhereECTable").DataTable().clear().destroy();
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
                        unBlockUI();
                    },
                    beforeSend: function () {
                        blockUI('Loading data please wait.');
                        var searchString = $('#AnywhereECTable_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;
                            if (!regexToMatch.test(searchString)) {
                                $("#AnywhereECTable_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    AnywhereECTable.search('').draw();
                                    $("#AnywhereECTable_filter input").prop("disabled", false);
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
                    { orderable: false, targets: [12] }
                ],
                columns: [
                    { data: "SerialNumber", "searchable": true, "visible": true, "name": "SerialNumber" },
                    { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
                    { data: "DocumentType", "searchable": true, "visible": true, "name": "DocumentType" },
                    { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
                    { data: "LocalServerStoragePath", "searchable": true, "visible": true, "name": "LocalServerStoragePath" },
                    { data: "FileUploadedToCentralServer", "searchable": true, "visible": true, "name": "FileUploadedToCentralServer" },
                    { data: "StateDataCentreStoragePath", "searchable": true, "visible": true, "name": "StateDataCentreStoragePath" },
                    { data: "SizeoftheFile", "searchable": true, "visible": true, "name": "SizeoftheFile" },
                    { data: "DateofScan", "searchable": true, "visible": true, "name": "DateofScan" },
                    { data: "DateofUpload", "searchable": true, "visible": true, "name": "DateofUpload" },
                    { data: "ArchivedinCD", "searchable": true, "visible": true, "name": "ArchivedinCD" },
                    { data: "DocumentDeliveryDate", "searchable": true, "visible": true, "name": "DocumentDeliveryDate" },
                    { data: "DateofRegistration", "searchable": true, "visible": true, "name": "DateofRegistration" },

                ],
                fnInitComplete: function (oSettings, json) {
                    //$("#PDFSPANID").html(json.PDFDownloadBtn);
                    $("#EXCELSPANID").html('');
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
        }



        //tableIndexReports.columns.adjust().draw();
        //AnywhereECTable.columns.adjust().draw();



    });

    



});

//function PDFDownloadFun() {
//    window.location.href = '/MISReports/DocumentScanAndDelivery/ExportAnywhereECRptToPDF?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&SroID=" + SroID + "&DistrictID=" + DistrictID + "&LogTypeID=" + LogTypeID + "&SelectedDist=" + SelectedDistrictText + "&SelectedSRO=" + SelectedSROText + "&SelectedLogType=" + SelectedLogTypeText;

//}


function EXCELDownloadFun(DistrictId, SroId, FromDate, ToDate) {


    if (SelectedType == "SR") {
        
        window.location.href = '/MISReports/DocumentScanAndDelivery/ExportDocumentScanToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SroID=" + SroId + "&DistrictID=" + DistrictId + "&Type=" + SelectedType + "&IsSROLogin=" + IsSr + "&DocumentType=" + DocumentTypeText + "&DocumentTypeId=" + DocumentTypeId;
    }
    else if (SelectedType == "DR") {
        window.location.href = '/MISReports/DocumentScanAndDelivery/ExportDocumentScanToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SroID=" + SroId + "&DistrictID=" + DistrictId + "&Type=" + SelectedType + "&IsSROLogin=" + IsSr + "&DocumentType=" + DocumentTypeText + "&DocumentTypeId=" + DocumentTypeId;
    }
    else if (IsDr == "True") {
        window.location.href = '/MISReports/DocumentScanAndDelivery/ExportDocumentScanToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SroID=" + SroId + "&DistrictID=" + DistrictId + "&Type=" + SelectedType + "&IsSROLogin=" + IsSr + "&DocumentType=" + DocumentTypeText + "&DocumentTypeId=" + DocumentTypeId;

    }
    else if (IsSr == "True") {
        SelectedType = "SR";
        window.location.href = '/MISReports/DocumentScanAndDelivery/ExportDocumentScanToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SroID=" + SroId + "&DistrictID=" + DistrictId + "&Type=" + SelectedType + "&IsSROLogin=" + IsSr + "&DocumentType=" + DocumentTypeText + "&DocumentTypeId=" + DocumentTypeId;

    }
}
