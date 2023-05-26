$(document).ready(function () {
    var txtFromDate;
    var txtToDate;
    var DistrictID;
    var txtDistrict;
    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();
    DistrictID = $("#DROOfficeListID option:selected").val();
    txtDistrict = $("#DROOfficeListID option:selected").text();
    //$("#TableAnywhereRegStat").find("tr:first-child").children("td").css("background-color", "#99CFF7");
    $("#AnywhereRegStatPdfId").click(function () {
        window.location.href = '/MISReports/AnywhereRegistrationStatistics/ExportAnywhereRegStatToExcel?FromDate=' + txtFromDate + "&ToDate=" + txtToDate + "&DistrictID=" + DistrictID + "&txtDistrict=" + txtDistrict;
    });

});