/*----------------------------------------------------------------------------------------
    * Project Id    :KARIGR [ IN-KA-IGR-02-05 ]
    * Project Name  :Kaveri Maintainance Portal
    * Name          :AddServicePackDetails
    * Description   :
    * Author        :Harshit   
    * Creation Date :01/05/2019
    * Modified By   :   
    * Updation Date :17 May 2019
    * ECR No : 300
----------------------------------------------------------------------------------------*/
var bugsList = [];
var enhancementsList = [];
var SuuportAnalysisList = [];

// Added by Shubham bhagat on 23-10-2019
//var enhancementsFixedBugsInputRules = /^[a-zA-Z0-9_ ]*$/;
var enhancementsFixedBugsInputRules = /^[a-zA-Z0-9-/., ]+$/;

var majorversionInput = new RegExp('^(?:[1-9] | 0[1-9]|10)$');
var minorversionInput = new RegExp('^(?:[1-9]|1[1-9]|20)$');
var changesModificationslist = "<select id='abc'>" +
    "<option value=\"1\">Bug<\/option>" +
    "<option value=\"2\">Enhancements<\/option>" +
    "<\/select>";

//Added by shubham bhagat on 05-10-2019
var FileSizeForValidation = 0;
$(document).ready(function () {  
    
    $(".ClsServicePackReleaseFiles").change(function () {
        $('span.spnClear').hide();
        var fileExtension = ['zip', 'ZIP'];
        if ($.inArray($(this).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
            bootbox.alert("Only Zip Files Are Allowed!!");
            $(this).val('');
            $('span.spnClear').hide();
            return false;
        }
        var file = document.getElementById(this.id).files.item(0)
        var iSize = file.size / 1048576; //In KB
        iSize = iSize.toFixed(2); //In MB
        $('span.spnClear').show();
        $("#FileSize").addClass('fa fa-file');
        if (iSize == 0) {
            iSize = file.size / 1024;            
            $("#spn").text("File Size: " + iSize.toFixed(2) + " KB    ");
        } else {            
            $("#spn").text("File Size: " + iSize + " MB    ");
        }
        // added by shubham bhagat on 10-10-2019 at 7:08 PM
        //$("#spn").text("File Size: " + iSize + " MB");
        //alert("File Size: " + iSize + " MB");

        //Added by shubham bhagat on 05-10-2019
        FileSizeForValidation = iSize;
        $("#divFileSize").show();
    });

    $('[data-toggle="tooltip"]').tooltip();
    var actions = $("table td:last-child").html();
    // Append table with add row form on add new button click
    $(".add-new").click(function () {
        $(this).attr("disabled", "disabled");
        var index = $("table tbody tr:last-child").index();
        var row = '<tr>' +
            '<td style="word-wrap: anywhere;"><input type="text" class="form-control" name="name" id="name"></td>' +
            '<td>' + actions + '</td>' +
            '</tr>';
        $("table").append(row);
        //$("table tbody tr").eq(index + 1).find(".add, .edit").toggle();
        $('[data-toggle="tooltip"]').tooltip();
    });
    // Add row on add button click
    $(document).on("click", ".add", function () {
        var empty = false;
        var input = $(this).parents("tr").find('input[type="text"]');
        //alert("input.val() :: "+input.val()+"::");        
        if ($('#lstChangeType').val() == 0) {
           bootbox.alert('Please Select Correspoding Modification Type to continue');
            return;
        }

        // ADDED BY SHUBHAM BHAGAT ON 04-10-2019
        //if (!input.val())
        if (!input.val().trim()) {
            bootbox.alert('Please enter Changes/Modification type description');
            return;
        }

        if (enhancementsFixedBugsInputRules.test(input.val())) {
            if ($('#lstChangeType').val() == 1) {
                bugsList.push(input.val());
            }
            if ($('#lstChangeType').val() == 2) {
                enhancementsList.push(input.val());
            }
            if ($('#lstChangeType').val() == 3) {
                SuuportAnalysisList.push(input.val());
            }
        }
        else {
           bootbox.alert('Invalid Input');
            return;
        }

        input.each(function () {
            if (!$(this).val()) {
                $(this).addClass("error");
                empty = true;
            } else {
                $(this).removeClass("error");
            }
        });
        $(this).parents("tr").find(".error").first().focus();
        if (!empty && $('#lstChangeType').val() == "2") {
            input.each(function () {
                //$(this).parent("td").html($(this).val());
                $(this).parent("td").html("<label style='color:green;word-break:break-word;'>Enhancement : </label>" + $(this).val());
            });
            $(this).parents("tr").find(".add, .edit").toggle();
            $(".add-new").removeAttr("disabled");
        }
        else if (!empty && $('#lstChangeType').val() == "1") {
            input.each(function () {
                //$(this).parent("td").html($(this).val());
                $(this).parent("td").html("<label style='color:red;word-break:break-word;'>Bug : </label>" + $(this).val());
            });
            $(this).parents("tr").find(".add, .edit").toggle();
            $(".add-new").removeAttr("disabled");
        }
        else if (!empty && $('#lstChangeType').val() == "3") {
            input.each(function () {
                //$(this).parent("td").html($(this).val());
                $(this).parent("td").html("<label style='color:blue;word-break:break-word;'>Support Analysis : </label>" + $(this).val());
            });
            $(this).parents("tr").find(".add, .edit").toggle();
            $(".add-new").removeAttr("disabled");
        }

    });
    // Edit row on edit button click
    //$(document).on("click", ".edit", function () {
    //    $(this).parents("tr").find("td:not(:last-child)").each(function () {
    //        $(this).html('<input type="text" class="form-control" value="' + $(this).text() + '">');
    //    });
    //    $(this).parents("tr").find(".add, .edit").toggle();
    //    $(".add-new").attr("disabled", "disabled");
    //});
    // Delete row on delete button click
    $(document).on("click", ".delete", function () {

        $(this).parents("tr").find("td:not(:last-child)").each(function () {
            var vCurrentVal = ($(this).text());
            //alert("vCurrentVal::"+vCurrentVal);
            //alert("(vCurrentVal.includes(bugs))::" + (vCurrentVal.includes("Bug")));
            //alert("(vCurrentVal.includes(Enhancement))::" + (vCurrentVal.includes("Enhancement")));
            //if ($('#lstChangeType').val() == 1) {
            if (vCurrentVal.includes("Bug")) {
                //alert("inside bug");
                bugsList.splice(bugsList.indexOf($(this).text()), 1);
            }


            //if ($('#lstChangeType').val() == 2) {

            if (vCurrentVal.includes("Enhancement")) {
                //alert("inside Enhancement");

                enhancementsList.splice(enhancementsList.indexOf($(this).text()), 1);
            }

        });
        $(this).parents("tr").remove();


        $(".add-new").removeAttr("disabled");
    });


    $("#onlyMajorNumber").keypress(function (e) {
        //if not number display error message
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            $("#errmsg").html("only number allowed").show().fadeOut("slow");
            return false;
        }

        if (majorversionInput.test($("#onlyMajorNumber").val())) {

        }
        else {
            $("#errmsg").html("❌ Input between 1 to 10").show().fadeOut("slow");
        }


        if ($("#onlyMajorNumber").val() < 1) {
            $("#errmsg").html("❌ Negative values are not allowed").show().fadeOut("slow");
        }
    });


    $("#onlyMinorNumber").keypress(function (e) {
        //if not number display error message
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            $("#errmsg").html("❌ only number allowed").show().fadeOut("slow");
            return false;
        }
        if (minorversionInput.test($("#onlyMajorNumber").val())) {

        }
        else {
            $("#errmsg").html("❌ Input between 1 to 20").show().fadeOut("slow");
        }


        if ($("#onlyMinorNumber").val() < 0) {
            $("#errmsg").html("❌ Negative values are not allowed").show().fadeOut("slow");
        }
    });


    $('#btnSubmitServicePackDetails').click(function () {
        AddServicePackDetails();
    });
    $('#btnClose').click(function () {
        window.location.href = "/Home/HomePage";
    });

    //--------------Reset Button ----------------//
    $('#btnReset').click(function () {
        $(':input', '#frmAddEditServicePackDetails').each(function () {
            var type = this.type;
            var tag = this.tagName.toLowerCase(); // normalize case
            // to reset the value attr of text inputs,
            // fileUpload and textareas
            if (type == 'text' || tag == 'textarea' || type == 'file')
                this.value = "";
            // select elements need to have their 'selectedIndex' property set to -1
            // (this works for both single and multiple select elements)
            else if (tag == 'select')
                this.selectedIndex = 0;
            $("#onlyMajorNumber").val(' ');
            $("#onlyMinorNumber").val(' ');
        });
    });

    //Added and Changed by mayank on 14/09/2021 for Support Exe Release
    $("#lstReleaseTypes").change(function () {
        if ($("#lstReleaseTypes").val() == 3) {
            $("#rdoNFinal").attr('disabled', 'disabled');
            $("#rdoYTest").attr('disabled', 'disabled');
            $("#rdoNSupport").prop('checked', true);
            $("#rdoNSupport").removeAttr('disabled');

        }
        else {

            $("#rdoNFinal").removeAttr('disabled');
            $("#rdoYTest").removeAttr('disabled');
            $("#rdoNSupport").attr('disabled', 'disabled');
            $("#rdoNSupport").prop('checked', false);
            $("#rdoYTest").prop('checked', true);
        }
    });
    //End
});

function AddServicePackDetails() {

    if ($("#lstReleaseTypes").val() == 0) {
        bootbox.alert("Please Select Release Type");
        return;
    }


    if ($('#onlyMajorNumber').val() == "") {
        bootbox.alert("Major Version is needed");
        return;
    }

    if ($('#onlyMinorNumber').val() == "") {
        bootbox.alert("Minor Version is needed");
        return;
    }


    if ($('#txtServicePackDesc').val() == "") {
        bootbox.alert("Service Pack Description is needed");
        return;
    }

    if ($('#txtInstallationProcedure').val() == "") {
        bootbox.alert("Installation Procedure is needed");
        return;
    }

    //disable the default form submission
    //event.preventDefault();
    var formData = new FormData();
    var fileOfficeOrder = document.getElementById("UploadReleaseFile").files[0];

    if (fileOfficeOrder == null) {
        bootbox.alert("Please upload Release File");
        return;
    }

    // BELOW CODE CHANGED FROM 20 MB TO 40 MB BY OMKAR ON 12-08-2020
    //Added by shubham bhagat on 05-10-2019
    //Added by mayank on 06-12-2021
    if (FileSizeForValidation > 150) {
        bootbox.alert("Please Upload file size less than 150 MB!");
        return;
    }

    //Added by shubham bhagat on 05-10-2019
    var patt = new RegExp("^[a-zA-Z0-9-/., ]+$");

    //Commented by mayank on 06/12/2021
    //var ServicePackDescbool = patt.test($('#txtServicePackDesc').val());    
    //if (!ServicePackDescbool)
    //{        
    //    bootbox.alert("Invalid Description");
    //    return;
    //}

    ////Added by shubham bhagat on 05-10-2019
    //var InstallationProcedurebool = patt.test($('#txtInstallationProcedure').val());
    //if (!InstallationProcedurebool) {
    //    bootbox.alert("Invalid Installation Procedure");
    //    return;
    //}

    
    //if ($("#lstChangeType").val() == 0) {
    //    bootbox.alert("Please Select Changes/Modification Type!");
    //    return;
    //}

    formData.append("ReleaseType", $("#lstReleaseTypes").val());
    formData.append("File", fileOfficeOrder);
    formData.append("Major", $('#onlyMajorNumber').val());
    formData.append("Minor", $('#onlyMinorNumber').val());
    formData.append("ChangesType", $('#lstChangeType').val());
    formData.append("FinalTestValue", $('[name="ServicePackDetails.IsTestOrFinal"]:checked').val());
    //formData.append("IsActiveValue", $('[name="ServicePackDetails.IsActive"]:checked').val());
    formData.append("ServicePackDescription", $('#txtServicePackDesc').val());
    formData.append("InstallationProcedure", $('#txtInstallationProcedure').val());
    var bugsListValues = bugsList.join("|");
    formData.append("BugsList", bugsListValues);
    var enhancementsListValues = enhancementsList.join("|");
    formData.append("EnhancementList", enhancementsListValues);
    var SupportListValues = SuuportAnalysisList.join("|");
    formData.append("SupportAnalysisList", SupportListValues);
    var RequestToken = $('[name=__RequestVerificationToken]').val();
    formData.append("__RequestVerificationToken", RequestToken);
    $.ajax({
        url: "/ServicePack/ServicePackDetails/SaveServicePackDetails",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.serverError == true && response.success == false) {
                if (response.message != null || response.message != "") {
                    bootbox.alert({
                        message: response.message,
                        callback: function () {
                            //Added by shubham bhagat on 18-09-2019
                            window.location.href = "/Home/HomePage"
                            //window.location.href = "/Login/Error";                            
                        }
                    });
                }
            }
            else {
                if (response.success) {
                    bootbox.alert({
                        message: response.message,
                        callback: function () {
                            window.location.href = "/ServicePack/ServicePackDetails/AddServicePackDetailsHome";
                        }
                    });
                }
                else if (response.success == false) {
                    bootbox.alert(response.message);
                }
                //Added by shubham bhagat on 18-09-2019
                // Commented to optimise code
                //else if (response.success == false && response.errormsgType == 1) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 2) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 3) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 4) {
                //    bootbox.alert(response.message);
                //    $.unblockUI();
                //}
                //else if (response.success == false && response.errormsgType == 5) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 6) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 7) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 8) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 9) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 10) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 11) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 12) {
                //    bootbox.alert(response.message);
                //}
                //else if (response.success == false && response.errormsgType == 13) {
                //    bootbox.alert(response.message);
                //}
                //else {
                //    bootbox.alert("Error While Adding Service Pack Details!");
                //}
            }
        },
        error: function (xhr, status, err) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>'
            });
        }

    });
}

function DownloadServicePack(VirtualPath) {
    window.location.href = "/ServicePack/ServicePackDetails/DownloadServicePackFile?virtualPath=" + VirtualPath;
}

function EditServicePackDetails(encryptedID) {
    window.location.href = "/ServicePack/ServicePackDetails/EditServicePackDetails?id=" + encryptedID;
}
