$(document).ready(function () {

    // $('#liDissolution').removeClass("sub-nav").addClass("sub-nav active");

    // alert("current active" + vCurrentActiveSubParentMenu + "current menu" + vRegistration);


    //Added by Tushar on 19 April 2022 for DeleteExpiredSessions
    $(document).ajaxError(function (xhr, props) {
        //alert("Ajax error props.status" + props.status);
        if (props.status === 401) {
            window.location.href = "/Error/SessionExpire"
        }
    });
    //end


    var RenderedLiItems = $("#dvHorizontalMenuBar > #crumbs li");

    //if (vRegistration == vCurrentActiveSubParentMenu) {
    //    $('#liRegistration').removeClass("sub-nav").addClass("sub-nav active");
    //}
    //else if (vAmendment == vCurrentActiveSubParentMenu) {
    //    $('#liAmendment').removeClass("sub-nav").addClass("sub-nav active");

    //}
    //else if (vDissolution == vCurrentActiveSubParentMenu) {
    //    $('#liDissolution').removeClass("sub-nav").addClass("sub-nav active");

    //}
    //else if (vRoleMenuActionDetails == vCurrentActiveSubParentMenu) {
    //    $('#liRoleMenuActionDetails').removeClass("sub-nav").addClass("sub-nav active");
    //}
    //else if (vOfficeUserDetails == vCurrentActiveSubParentMenu) {
    //    $('#liOfficeUserDetails').removeClass("sub-nav").addClass("sub-nav active");
    //}
    //else
    //alert(vSRODD_POCollectionDetails);
     
    if (vActiveMenuName == "") {


        if (vUpdateProfile == vCurrentActiveSubParentMenu) {
            $('#liUpdateProfile').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vChangePassword == vCurrentActiveSubParentMenu) {
            $('#liChangePassword').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vEcAuditDetails == vCurrentActiveSubParentMenu) {
            $('#liEcAuditDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vDiagnosticDetails == vCurrentActiveSubParentMenu) {
            $('#liDiagnosticDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vDiagnosticSummary == vCurrentActiveSubParentMenu) {
            $('#liDiagnosticSummary').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vUserDetail == vCurrentActiveSubParentMenu) {
            $('#liUserDetail').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vRoleMenuMapping == vCurrentActiveSubParentMenu) {
            $('#liRoleMenuMapping').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vOfficeDetail == vCurrentActiveSubParentMenu) {
            $('#liOfficeDetail').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vMenuActionDetails == vCurrentActiveSubParentMenu) {
            $('#liMenuActionDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vControllerActionDetails == vCurrentActiveSubParentMenu) {
            $('#liControllerActionDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vIntegrationCallExceptions == vCurrentActiveSubParentMenu) {
            $('#liIntegrationCallExceptions').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vErrorLogFiles == vCurrentActiveSubParentMenu) {
            $('#liErrorLogFiles').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vIndexIIReports == vCurrentActiveSubParentMenu) {
            $('#liIndexIIReport').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vRemittanceXMLLog == vCurrentActiveSubParentMenu) {
            $('#liRemittanceXMLLog').removeClass("sub-nav").addClass("sub-nav active");

        } else if (vChallanMatrixXMLLog == vCurrentActiveSubParentMenu) {
            $('#liChallanMatrixXMLLog').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vDoubleVeriXMLLog == vCurrentActiveSubParentMenu) {
            $('#liDoubleVeriXMLLog').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vTotalDocumentsRegistered == vCurrentActiveSubParentMenu) {
            $('#liTotaldocumentsregisteredandRevenuecollected').removeClass("sub-nav").addClass("sub-nav active");
        }//akash
        else if (vHighValueProperties == vCurrentActiveSubParentMenu) {
            $('#liHighValueProperties').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vDailyRevenueArticleWise == vCurrentActiveSubParentMenu) {
            $('#liDailyRevenueArticleWise').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vSRODocumentsCashCollectionDetails == vCurrentActiveSubParentMenu) {
            $('#liSRODocumentsCashCollectionDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vSaleDeedRegisteredandRevenueCollected == vCurrentActiveSubParentMenu) {
            $('#liSaleDeedRegisteredandRevenueCollected').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vSRODD_POCollectionDetails == vCurrentActiveSubParentMenu) {

            $('#liSRODDandPOCollection').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vTicketRegistration == vCurrentActiveSubParentMenu) {

            $('#liTicketRegistration').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vDecryptEnclosure == vCurrentActiveSubParentMenu) {

            $('#liDecryptEnclosure').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vUploadPatchFile == vCurrentActiveSubParentMenu) {

            $('#liUploadPatchFile').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vDailyReceiptsDetails == vCurrentActiveSubParentMenu) {

            $('#liDailyReceiptsDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vSurchargeandcessDetails == vCurrentActiveSubParentMenu) {
            $('#liSurchargeandcessDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vJurisdictionalWiseReport == vCurrentActiveSubParentMenu) {
            $('#liJurisdictionalWiseReport').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vExemptedDocument == vCurrentActiveSubParentMenu) {
            $('#liExemptedDocument').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vAnyWhereECLog == vCurrentActiveSubParentMenu) {


            $('#liAnywhereECLog').removeClass("sub-nav").addClass("sub-nav active");

        }
        else if (vScannedFileUploadStatus == vCurrentActiveSubParentMenu) {
            $('#liScannedFileUploadStatusreport').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vECDailyReceiptDetails == vCurrentActiveSubParentMenu) {
            $('#liECDailyReceiptsDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vReleasedServicePacks == vCurrentActiveSubParentMenu) {
            $('#liReleasedServicePacks').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vServicePacksDetails == vCurrentActiveSubParentMenu) {
            $('#liServicePacksDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vServicePackDetails == vCurrentActiveSubParentMenu) {
            $('#liServicePackDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vAddServicePackDetails == vCurrentActiveSubParentMenu) {
            $('#liAddServicePackDetails').removeClass("sub-nav").addClass("sub-nav active");
        }
        else if (vScannedFileDownload == vCurrentActiveSubParentMenu) {
            $('#liScannedFileDownload').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        }
        else if (vAnywhereRegistrationStatistics == vCurrentActiveSubParentMenu) {
            $('#liAnywhereRegistrationStatistics').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        }
        else if (vDocumentCentralizationStatus == vCurrentActiveSubParentMenu) {
            $('#liDocumentCentralizationStatus').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        }
        else if (vXELLog == vCurrentActiveSubParentMenu) {
            $('#liXELLog').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        }
        else if (vKaveriIntegrationReport == vCurrentActiveSubParentMenu) {
            $('#liKaveriIntegrationReport').removeClass("sub-nav").addClass("sub-nav active"); //Shubham
        }
        else if (vReScanningDetails == vCurrentActiveSubParentMenu) {
            $('#liReScanningDetails').removeClass("sub-nav").addClass("sub-nav active"); //Shubham
        }
        else if (vDataTransmissionReport == vCurrentActiveSubParentMenu) {
            $('#liDataTransmissionReport').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        }
        else if (vPendingDocumentsSummary == vCurrentActiveSubParentMenu) {
            $('#liPendingDocumentSummary').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        }
        else if (vBhoomiFileUploadReport == vCurrentActiveSubParentMenu) {
            $('#liBhoomiFileUploadReport').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        }
        else if (vCDWrittenReport == vCurrentActiveSubParentMenu) {
            $('#liCDWrittenReport').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        }
        else if (vJSlipUploadReport == vCurrentActiveSubParentMenu) {
            $('#liJslipUploadReport').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        } else if (vSAKALAUploadAndPendencyReport == vCurrentActiveSubParentMenu) {
            $('#liSAKALAUploadandPendencyReport').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        } else if (vDocumentScanandDelivery == vCurrentActiveSubParentMenu) {
            $('#liDocumentScanandDeliveryReport').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        } else if (vDocumentReferences == vCurrentActiveSubParentMenu) {
            $('#liDocumentReferencesReport').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        } else if (vOtherDepartmentImport == vCurrentActiveSubParentMenu) {
            $('#liOtherDepartmentImport').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        } else if (vDiskUtilizationReport == vCurrentActiveSubParentMenu) {
            $('#liDiskUtilizationReport').removeClass("sub-nav").addClass("sub-nav active"); //Ramank
        }
       
    }
    else {
        $('#' + vActiveMenuName).removeClass("sub-nav").addClass("sub-nav active"); //Ramank
         
    }


    $(".nav li").on("click", function () {
        // alert($(this).attr('class'));
        //alert($("#dvHorizontalMenuBar > #crumbs li").length);
        $("#dvHorizontalMenuBar > #crumbs li").removeClass("active");
        $(this).removeClass("treeview");
        $(this).addClass("active");

    });


    $("#dvHorizontalMenuBar a").on("click", function (e) {
        e.preventDefault();
        var hrefValue = $(this).attr('href');
        LoadPageAjax(hrefValue);
    });




    // Commented by SB on 9-4-2019 
    // below code is for toggle menu button
    var count = 0;
    $('#iconSideBar').click(function () {
        ++count;
        if (count % 2 == 0) {
            $(".SubMenuClass").css('padding-left', '28px');
            $('#iconSideBar').prop('title', 'Close Menu');
        }
        else {
            $(".SubMenuClass").css('padding-left', '1px');
            $('#iconSideBar').prop('title', 'Open Menu');
        }
    });

    // Added by SB on 9-4-2019 
    $('#iconSideBar').prop('title', 'Close Menu');
    // Commented by SB on 9-4-2019 
    //$('#iconSideBar').trigger('click');




    //LoggOFF onClick
    $("#btnLoggOffID").click(function () {
        //  BlockUI();
        $.ajax({
            type: "Get",
            url: "/Login/Logout",
            //data: { "encryptedID": encryptedID },
            success: function (data) {

                if (data.success == true) {

                    window.location.href = data.URL;

                }
                //   $.unblockUI();

                //   $("#dvPlacePage").load("/Login/UserLogin");
            },
            error: function () {
                bootbox.alert("error");
                //  $.unblockUI();
            }
        });
    });


    $("#btnRegisterDSCID").click(function () {
        //   var userID = 6;

        //  alert(UserID);

        //    $("#dvPlaceHolder").show();
        $("#dvPlacePage").load("/UserManagement/RegisterDSC/RegisterDSC?id=" + UserID);





        //window.location.href = "/UserManagement/RegisterDSC/RegisterDSC?id=" + userID;
        //  window.location.href = "/Firm/FirmSummary/DownloadDocumentForSign?EncryptedID=" + vEncryptedID;

    });


});




/// Checks for invalid session [JQUERY]  
function IsValidSession(response_data) {
    if (response_data != null) {
        if (response_data.match(/id="ajaxexpiry"/gi) != null)  // pattern to locate
        {
            window.location.replace("/Error/SessionExpire");
        }
    }
}


//function to load the page in maindiv
function LoadPage(urlParam) {


    var arrparam = urlParam.split('$');
    url = arrparam[0];
    //  alert(url);
    moduleName = arrparam[1];
    if (url != '#') {
        window.location.href = url;
    }

    return false;
}





function LoadPageAjax(urlParam) {
    //   alert(urlParam);
    isAjaxCall = true;

    var arrparam = urlParam.split('$');
    url = arrparam[0];
    moduleName = arrparam[1];
    if (url != '#') {
        //window.location.href = url;

        //Get the data from server
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
        $.ajax({
            url: url,
            type: "GET",
            cache: false,
            async: true,
            beforeSend: function () {
                //$.blockUI({
                //    css: {
                //        border: 'none',
                //        padding: '15px',
                //        backgroundColor: '#000',
                //        '-webkit-border-radius': '10px',
                //        '-moz-border-radius': '10px',
                //        opacity: .5,
                //        color: '#fff'
                //    }
                //});
            },
            error: function (xhr, status, error) {
                $.unblockUI();
                var errorMessage = $.parseJSON(xhr.responseText);
                if (errorMessage.error) {
                    alert(errorMessage.message);
                }
                else {
                    alert("Request can not be processed at this time, please try after some time!!!");
                }
                isAjaxCall = false;
                return false;
            },
            success: function (response) {
                //   alert("response success");

                isAjaxCall = false;
                IsValidSession(response);
                $('#dvPlacePage').html(response);

                $.unblockUI();
            }
        });
    }

    //VVIMP dont ever move return false in sucess method
    return false;
}

var request = 1;
function CheckClientStatus() {

    $.ajax({
        url: "/Home/KeepSessionAlive?request=" + request,
        cache: false,
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data.success) {
                request++;
                setTimeout("CheckClientStatus()", 3000);//4seconds
            }
            else {
                window.location.href = "/Error/Index";
            }
        },
        error: function () {
            console.log('Errror............!!!');
            $.unblockUI();
        }
    });



    //  $('div.feedback-box').html(feedback);
}




//var keepSessionAlive = false;
//var keepSessionAliveUrl = null;

//function SetupSessionUpdater(actionUrl) {

//    alert("Akash");

//    keepSessionAliveUrl = actionUrl;
//    var container = $("#body");
//    container.mousemove(function () { keepSessionAlive = true; });
//    container.keydown(function () { keepSessionAlive = true; });
//    CheckToKeepSessionAlive();
//}

//function CheckToKeepSessionAlive() {
//    setTimeout("KeepSessionAlive()", 3000);
//}

//function KeepSessionAlive() {
//    alert("KeepSessionAlive()");

//    if (keepSessionAlive && keepSessionAliveUrl != null) {
//        $.ajax({
//            type: "POST",
//            url: keepSessionAliveUrl,
//            success: function () { keepSessionAlive = false; }
//        });
//    }
//    alert("before CheckToKeepSessionAlive()");

//    CheckToKeepSessionAlive();
//}