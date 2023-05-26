

$(document).ready(function () {
    //var SROOfficeListID = $("#SROOfficeListID option:selected").val();
    //var DROOfficeListID = $("#DROOfficeListID option:selected").val();
    //var Date = $("#txtStamp5DateID").val();
    //var ToDate = $("#ToDateID").val();



 



});

function PDFDownloadFun(DROOfficeListID, SROOfficeListID, Date, ToDate) {

    window.location.href = '/MISReports/TodaysDocumentsRegistered/ExportTodaysTotalDocsRegReportToPDF?SROOfficeListID=' + SROOfficeListID + "&DROOfficeListID=" + DROOfficeListID + "&Date=" + Date + "&ToDate=" + ToDate + "&MaxDate=" + MaxDate;
}


function EXCELDownloadFun(DROOfficeListID, SROOfficeListID, Date, ToDate) {
    
    window.location.href = '/MISReports/TodaysDocumentsRegistered/ExportTodaysDocumentsRegisteredReportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROOfficeListID=" + DROOfficeListID + "&Date=" + Date + "&ToDate=" + ToDate + "&MaxDate=" + MaxDate + "&DocTypeID=" + DocTypeID + "&DocTypeText=" + DocTypeText;
}