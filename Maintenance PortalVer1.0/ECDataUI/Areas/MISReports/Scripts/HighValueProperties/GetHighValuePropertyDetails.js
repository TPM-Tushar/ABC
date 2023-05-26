
$(document).ready(function () {

    var RangeID = $("#RagneID option:selected").val();
    var FinYearListID = $("#FinYearListID option:selected").val();
    
});

function EXCELDownloadFun(RangeID, FinYearListID) {
    window.location.href = '/MISReports/HighValueProperties/ExportTodaysHighValPropToExcel?RangeID=' + RangeID + '&FinYearListID=' + FinYearListID+'&MaxDate='+MaxDate;

}