$(document).ready(function () {

   


    $("#btnFirmRegDetailsID").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/FirmRegistration/FirmRegistrationDetails");

    });


    $("#btnFirmRegGetEditID").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/FirmRegistration/EditFirmDetails");

    });


    $("#btnPartnerDetailsListID").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/PartnerDetails/PartnerDetailsList");

    });

    

    $("#btnFirmApplicationListIDForSR").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/FirmRegistration/LoadFirmDetails");

    });

    $("#btnAddEditPartnerDetailsID").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/PartnerDetails/AddEditPartnerDetails");

    });


    $("#btnFeePaymentID").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Accounts/AccountDetails/FeePaymentView");

    });



    $("#btnWitnessDetailsID").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/WitnessDetails/AddEditWitnessDetails");

    });



    $("#btnFirmApplicationListID").click(function () {

        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/FirmRegistration/LoadFirmDetails");

    });


    $("#"+sNavigationPageId).trigger("click");


    

    $("#btnCheckList").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/CheckSlip/CheckSlipsDetails");

    });


    $("#btnFirmSummaryID").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/FirmSummary/FirmSummaryView");

    });

    $("#btnFirmDocuments").click(function () {
        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/Firm/FirmDocuments/EditFirmSupportedDocuments");

    });


    $("#btnRegisterDSCID").click(function () {

        $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/UserManagement/RegisterDSC/RegisterDSC?id=" + UserID);

    });



    //LoggOFF onClick
    $("#btnLoggOffID").click(function () {
        BlockUI();
        $.ajax({
            type: "Get",
            url: "/Login/Logout",
            //data: { "encryptedID": encryptedID },
            success: function (data) {

                if (data.success == true) {

                    window.location.href = data.URL;

                }
                $.unblockUI();

             //   $("#dvPlacePage").load("/Login/UserLogin");
            },
            error: function () {
                bootbox.alert("error");
                $.unblockUI();
            }
        });
    });




});

function NavigateToPage(id) {
  
    $('#' + id).trigger('click');
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