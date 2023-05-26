

$(document).ready(function () {
    SelectedType = $('input[name="RDOBtnFilter"]:checked').val();
    if (SelectedType == "SR") {
        $('#IdOffice').html("Sub Registrar Office");
    }
    else if (SelectedType == "DR") {
        $('#IdOffice').html("District Registrar Office");
    }
   
});

function PDFDownloadFun(DROOfficeListID, SROOfficeListID) {

    window.location.href = '/MISReports/ScannedFileUploadStatusReport/ExportScannedFileUploadStatusReportToPDF?DistrictCode=' + DROOfficeListID + "&SROCode=" + SROOfficeListID + "&DistrictText=" + DistrictText + "&SROText=" + SROText;
}


function EXCELDownloadFun(DROOfficeListID, SROOfficeListID, OfficeType) {
    
    window.location.href = '/MISReports/ScannedFileUploadStatusReport/ExportScannedFileUploadStatusRptToExcel?DistrictCode=' + DROOfficeListID + "&SROCode=" + SROOfficeListID + "&DistrictText=" + DistrictText + "&SROText=" + SROText + "&OfficeType=" + OfficeType + "&DocTypeID=" + DocTypeID + "&DocTypeText=" + DocTypeText;
}