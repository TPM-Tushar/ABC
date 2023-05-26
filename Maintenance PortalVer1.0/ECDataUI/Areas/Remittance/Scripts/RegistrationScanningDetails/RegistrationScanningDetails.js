/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationScanningDetails.js
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :  JS for Registration Scanning Details Report .

*/



//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

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
    //
    $("input:checkbox").on('click', function () {

        var $box = $(this);
        if ($box.is(":checked")) {

            var group = "input:checkbox[name='" + $box.attr("name") + "']";

            $(group).prop("checked", false);
            $box.prop("checked", true);
        } else {
            $box.prop("checked", false);
        }
    });
    //Added By Tushar on 16 jan 2023 for DocumentType firm
    $('#DocumentTypeId').change(function () {
        if ($("#DocumentTypeId").val() == '4') {

            $("#DocType").html('<label class="PaddingTop10" for="DocumentTypeId" style="padding-left:4px;">District Name</label><label style="color:red">*</label>\n');
            
        } else {
            $("#DocType").html('<label class="PaddingTop10" for="DocumentTypeId" style="padding-left:4px;">SRO Name</label><label style="color:red">*</label>\n')
        }
        //

        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/Remittance/RegistrationScanningDetails/RegistrationScanningDetailsView',
            data: { "DocumentTypeId": $('#DocumentTypeId').val() },
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
                    $.each(data.OfficeList, function (i, OfficeList) {
                        OfficeList
                        $('#SROOfficeListID').append('<option value="' + OfficeList.Value + '">' + OfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });

            //
    });
    //End By Tushar on 16 Jan 2023

});

function SerachDetails() {
   // alert("In SerachDetails");
    let  SROfficeID = $("#SROOfficeListID").val();
    let FromDate = $("#txtFromDate").val();
    let ToDate = $("#txtToDate").val();
    let DocumentTypeId = $("#DocumentTypeId").val();
    let ScanFilterValue = $("input[name='ScanFilterValue']:checked").val();
    //alert("SROfficeID" + SROfficeID);
    //alert("FromDate" + FromDate);
    //alert("ToDate" + ToDate);
    //alert("DocumentTypeId" + DocumentTypeId);

    var tableRegistrationScanningDetails = $('#RegistrationScanningDetailsID').DataTable({
        ajax: {

            url: '/Remittance/RegistrationScanningDetails/GetRegistrationScanningDetails',
            type: "POST",
            headers: header,
            data: {

                //'FromDate': FromDate, 'ToDate': ToDate, 'DocumentTypeId': DocumentTypeId, 'SROfficeID': SROfficeID
                'FromDate': FromDate, 'ToDate': ToDate, 'DocumentTypeId': DocumentTypeId, 'SROfficeID': SROfficeID, 'ScanFilterValue': ScanFilterValue
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
                                $("#RegistrationScanningDetailsID").DataTable().clear().destroy();
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
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });

                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#RegistrationScanningDetailsID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#RegistrationScanningDetailsID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableRegistrationScanningDetails.search('').draw();
                            $("#RegistrationScanningDetailsID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }
        },
        serverSide: true,

    
        scrollX: false,
        "scrollY": "300px",
        "responsive": true,
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bSort": true,


        columns: [

            { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width":"3%" },
            { data: "SROCode", "searchable": true, "visible": SetVisibilityForSRO(DocumentTypeId), "name": "SROCode", "width": "3%"  },
            { data: "DRCode", "searchable": true, "visible": SetVisibilityForFirm(DocumentTypeId), "name": "DRCode", "width": "3%"   },
            { data: "MarriageCaseNo", "searchable": true, "visible": SetVisibilityForMarriage(DocumentTypeId), "name": "MarriageCaseNo", "width": "14%"  },
            { data: "NoticeNo", "searchable": true, "visible": SetVisibilityForNotice(DocumentTypeId), "name": "NoticeNo", "width": "14%"  },
            { data: "FirmNumber", "searchable": true, "visible": SetVisibilityForFirm(DocumentTypeId), "name": "FirmNumber", "width": "14%"  },
            { data: "DateOfRegistration", "searchable": true, "visible": SetVisibilityForDateOfRegistration(DocumentTypeId), "name": "DateOfRegistration", "width": "15%"  },
            { data: "NoticeIssuedDate", "searchable": true, "visible": SetVisibilityForNotice(DocumentTypeId), "name": "NoticeIssuedDate", "width": "15%"  },
            { data: "CDNumber", "searchable": true, "visible": true, "name": "CDNumber", "width": "10%"  },
            { data: "ScanMasterEntry", "searchable": true, "visible": true, "name": "ScanMasterEntry", "width": "10%"  },
            { data: "ScannedFileUploadDetailsEntry", "searchable": true, "visible": true, "name": "ScannedFileUploadDetailsEntry", "width": "15%"  },
            { data: "IsCDWritten", "searchable": true, "visible": true, "name": "IsCDWritten", "width": "10%"  },
            //Added By Tushar on 13 Jan 2023
            { data: "ScanFilePath", "searchable": true, "visible": true, "name": "ScanFilePath", "width": "20%"  },
            //End By Tushar on 13 Jan 2023

        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
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
            unBlockUI();
        },
    });
}
function SetVisibilityForMarriage(DocumentTypeId) {
    if (DocumentTypeId == 2) {
        return true;
    }
    return false;
}

function SetVisibilityForNotice(DocumentTypeId) {
    if (DocumentTypeId == 3) {
        return true;
    }
    return false;
}
//Added By Tushar on 16 Jan 2023
function SetVisibilityForFirm(DocumentTypeId) {
    if (DocumentTypeId == 4) {
        return true;
    }
    return false;
}

function SetVisibilityForSRO(DocumentTypeId) {
    if (DocumentTypeId != 4) {
        return true;
    }
    return false;
}
function SetVisibilityForDateOfRegistration(DocumentTypeId) {
    if (DocumentTypeId != 3) {
        return true;
    }
    return false;
}
//End By Tushar on 16 Jan 2023

function EXCELDownloadFun(fromDate, ToDate, DocumentTypeId, SROfficeID, ScanFilterValue) {
    window.location.href = '/Remittance/RegistrationScanningDetails/ExportRegistrationScanningDetailsToExcel?SROfficeID=' + SROfficeID + "&DocumentTypeId=" + DocumentTypeId + "&FromDate=" + fromDate + "&ToDate=" + ToDate + "&ScanFilterValue=" + ScanFilterValue;
}