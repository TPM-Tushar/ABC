$(document).ready(function () {
    $("#DROfficeWiseSummaryTableCollapse").trigger('click');
    $("#SROfficeWiseSummaryTableCollapse").trigger('click');


    $("#DROfficeWiseSummaryTableCollapse").click(function () {
        var classToRemoveSearchPara = $('#DtlsToggleIconDROfficeWiseSummaryTable').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsToggleIconDROfficeWiseSummaryTable').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

        $('#DtlsToggleIconDROfficeWiseSummaryTable').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

    });
    $("#SROfficeWiseSummaryTableCollapse").click(function () {
        var classToRemoveSearchPara = $('#DtlsToggleIconSROfficeWiseSummaryTable').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsToggleIconSROfficeWiseSummaryTable').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

            $('#DtlsToggleIconSROfficeWiseSummaryTable').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

    });

});

//To redirect to the OfficeDetails page , after clicking on any row of SROfficeWiseSummary table 
function RedirectToOfficeDetailsPage(EncryptedID)
{
    //alert("EncryptedID"+EncryptedID);
    window.location.href = "/Remittance/REMDaignostics/RemittanceDiagnosticsDetailsView?EncryptedID=" + EncryptedID;
}

