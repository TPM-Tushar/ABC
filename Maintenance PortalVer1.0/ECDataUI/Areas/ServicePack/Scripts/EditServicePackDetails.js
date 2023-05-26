/*----------------------------------------------------------------------------------------
    * Project Id    :KARIGR [ IN-KA-IGR-02-05 ]
    * Project Name  :Kaveri Maintainance Portal
    * Name          :
    * Description   :     
    * Author        :Harshit   
    * Creation Date :01/05/2019
    * Modified By   :   
    * Updation Date :17 May 2019
    * ECR No : 300
----------------------------------------------------------------------------------------*/
var bugsList = [];
// Added by Shubham bhagat on 23-10-2019
//var enhancementsFixedBugsInputRules = /^[a-zA-Z0-9_ ]*$/;
var enhancementsFixedBugsInputRules = /^[a-zA-Z0-9-/., ]+$/;
var majorversionInput = new RegExp('^(?:[1-9] | 0[1-9]|10)$');
var minorversionInput = new RegExp('^(?:[1-9]|1[1-9]|20)$');
var changesModificationslist = "<select class = 'form-control minimal dropdown-toggle' id='abc'>" +
    "<option value=\"1\">Bugs<\/option>" +
    "<option value=\"2\">Enhancements<\/option>" +
    "<\/select>";
var _griddata;
var countofGrid = 0;
//Added by shubham bhagat on 05-10-2019
var FileSizeForValidation = 0;
$(function () {
    //Added by shubham bhagat on 18-09-2019
    //alert('in edit js');
    //alert($('#IsFileToUpdateID').val());
   

    //alert($("#ddlChangeType").val());   

    //alert(sfds.length);
    //for (i = 0; i < sfds.length; i++) {
    //    alert(sfds[i]);
    //}
    //alert("tagregrehgrtehs :: "+tagregrehgrtehs);
    //var persons = JSON.parse(tagregrehgrtehs);
    

    //Below code is right but extra
    //var ModificationTypeListInJsonOBJ = JSON.parse(ModificationTypeListInJsonStr);
    //var i;
    //for (i = 0; i < ModificationTypeListInJsonOBJ.length; i++) {
    //    //alert("Id :: " + ModificationTypeListInJsonOBJ[i].Id);
    //    //alert("Value :: " + ModificationTypeListInJsonOBJ[i].Value);
    //}

    //jQuery('#WebGrid > tbody > tr').each(function (index, value) {
    //    //$('td:eq(2)', this).css('background-color', 'red');
    //    var tdObject = $(this).find('td:eq(2)'); //locate the <td> holding select;
    //    //alert(tdObject);
    //    var selectObject = tdObject.find("select"); //grab the <select> tag assuming that there will be only single select box within that <td> 
    //    //alert(selectObject);
    //    var selCntry = selectObject.val(); // get the selected country from current <tr>
    //    alert(selCntry);
    //    selectObject.val('2');
    //});
    //Above code is right but extra

    //var ModificationTypeListInJsonOBJ = JSON.parse(ModificationTypeListInJsonStr);
    //var listCount=0;
    //var rowCount = 0;
    //for (listCount = 0; listCount < ModificationTypeListInJsonOBJ.length; listCount++) {                
    //    rowCount = 0;
    //    jQuery('#WebGrid > tbody > tr').each(function (index, value) {            
    //        var tdObject = $(this).find('td:eq(2)'); //locate the <td> holding select;
    //        var selectObject = tdObject.find("select"); //grab the <select> tag assuming that there will be only single select box within that <td> 
    //        //var selCntry = selectObject.val(); // get the selected country from current <tr>
    //        //alert(selCntry);
    //        if (listCount == rowCount) {
    //            selectObject.val(ModificationTypeListInJsonOBJ[listCount].Value);
    //        }
    //        rowCount = rowCount + 1;
    //    });
    //}

    





    //alert("JSON.parse(tagregrehgrtehs) :: " + persons);
    //alert("JSON.stringify(persons) ::" + JSON.stringify(persons));
    //alert("persons[0] :: " + persons[0]);
    //alert("JSON.stringify(persons[0]) :: " + JSON.stringify(persons[0]));
    //alert("JSON.stringify(persons[0]) :: " + JSON.stringify(persons[0]));
    //var i;
    //for (i = 0; i < persons.length; i++) {
    //    alert("Id :: " + persons[i].Id);
    //    alert("Value :: " + persons[i].Value);
    //}

    //var ewfwregrew = JSON.stringify(persons);
    //alert(ewfwregrew[0]);
    //for (var i = 0; i < tagregrehgrtehs.length; i++) {
    //alert(tagregrehgrtehs[i]);
    //}

    var count = $('#WebGrid tr').length;
    $('#add').on('click', function () {
        
        $('table').append('<tr id=' + count++ + '>' +
            '<td>N/A</td>' +
            '<td><input name=\'title\' id=\'Name\' style="width: -moz-available;"/></td>' +
            '<td id=\'ddlChangeType\' >' + changesModificationslist + '</td>' +
            '<td><button class="btn btn-danger theButton" title="Click here to delete Changes/Modification Type entry">Delete</button></td>' +
            + '</tr>');
    });

    $('.edit-mode').hide();
    $('.edit-user, .cancel-user').on('click', function () {
       
        var tr = $(this).parents('tr:first');        
        tr.find('.edit-mode, .display-mode').toggle();
        // ADDED BY SHUBHAM BHAGAT ON 09-10-2019
        //alert($(this).parents('tr:first').siblings());
        //tr.find('td:eq(2)').find("select").removeAttr("disabled");
        if ($(this).hasClass('edit-user'))
        {
            //alert('edit-user');
            tr.find('td:eq(2)').find("select").removeAttr("disabled");            
        }
        if ($(this).hasClass('cancel-user')) {
            //alert('cancel-user');
            tr.find('td:eq(2)').find("select").prop("disabled","disabled");
        }
        //$(this).parents('tr:first').find('td:eq(2)').find("select").css('disabled', 'false');
        //$(this).parents('tr:first').find('td:eq(2)').find("select").disabled = 'false';
        //$(this).parents('tr:first').find('td:eq(2)').find("select").removeAttr("disabled");
        //alert($(this).parents('tr:first').find('td:eq(2)').find("select").css('disabled', 'false'));

        //alert(tr.find('#ddlChangeType').);
        //tr.find('#ddlChangeType').css('disabled','false');
        //alert($(this).closest('tr').find('td:nth-child(3)').attr("id"));
        // below is correct code
        //alert($(this).parents('tr:first').find('td:eq(2)').find("select").css('background-color','red'));
        //$(this).find('td:eq(2)').find("select").css('background-color','red'));
        //alert($(this).parents('tr:first').find('td:nth-child(3)').css('font-color','red'));
    });
    $('body').on('click', function () {
    }).on('click', 'button.theButton', function (e) {

        $(this).parents("tr").remove();
    });
    //The Dynamically Populated Ones
    //$('body').on('click',  function () {
    //}).on('click', 'button.theButton', function (e) {
    //    var tr = $(this).parents('tr:first');
    //    var Name = tr.find("#Name").val();
    //    var SurName = tr.find("#abc").val();
    //    //var UserID = tr.find("#UserID").html();
    //    var UserModel =
    //        {
    //            "Name": Name,
    //            "SurName": SurName
    //        };
    //    alert(JSON.stringify(UserModel));
    //    });

    //The Server Populated Ones
    $('.save-user').on('click', function () {
        var tr = $(this).parents('tr:first');
        var Name = tr.find("#Name").val();
        var SurName = tr.find("#ddlChangeType").val();
        if (!Name) {
            bootbox.alert('Invalid Input');
            return;
        }

        if (enhancementsFixedBugsInputRules.test(Name)) {
            tr.find("#lblName").text(Name);
            tr.find("#lblSurName").text(SurName);
            tr.find('.edit-mode, .display-mode').toggle();
        }
        else {
            bootbox.alert('Invalid Input');
            return;
        }
        // ADDED BY SHUBHAM BHAGAT ON 09-10-2019
        tr.find('td:eq(2)').find("select").prop("disabled", "disabled");
    });

    $('#btnClose').click(function () {
        window.location.href = "/ServicePack/ServicePackDetails/AddServicePackDetailsHome";
    });


    
    $('#btnEditServicePackDetails').click(function () {
        _griddata = gridTojson();
        countofGrid = $('#WebGrid tr').length - 1;
        UpdateServicePackDetails(_griddata);

    });

    $('.delete').click(function () {
        var str = $(this).attr("id").split("_");
        id = str[1];
        var flag = confirm('Are you sure to delete ??');
        if (id == "" || flag == false) {
            return;
        }
        else {
            DeleteServicePackChangeEntry(id);
        }
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

    $(".ClsServicePackReleaseFiles").change(function () {
               
        $('span.spnClear').hide();
        var fileExtension = ['zip', 'ZIP'];

        //Added by shubham bhagat on 18-09-2019
        if ($(".ClsServicePackReleaseFiles").val() == "")
            return false;

        if ($.inArray($(this).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
            bootbox.alert("Only Zip Files Are Allowed!!");
            $(this).val('');
            $('span.spnClear').hide();

            //Added by shubham bhagat on 18-09-2019
            $('#IsFileToUpdateID').val("false");
            //alert($('#IsFileToUpdateID').val());
            return false;
        }

        var vSelectedFileName = $(this).val();        
        $("#Upload-filename").text(vSelectedFileName.substring(vSelectedFileName.lastIndexOf('\\')+1, vSelectedFileName.length));
        //alert("vSelectedFileName::" + vSelectedFileName);

        var file = document.getElementById(this.id).files.item(0);
        var iSize = file.size / 1048576; //In KB
        iSize = iSize.toFixed(2); //In MB
        $('span.spnClear').show();
        //$("#FileSize").addClass('fa fa-file');
        //$("#spn").text("File Size: " + iSize + " MB");

        //Added by shubham bhagat on 18-09-2019
        $('#IsFileToUpdateID').val("true");
        //alert($('#IsFileToUpdateID').val());

        //Added by shubham bhagat on 05-10-2019
        FileSizeForValidation = iSize;
        //alert(file.size);
        if (iSize == 0) {
            iSize = file.size / 1024;
            //bootbox.alert("File Size: " + iSize.toFixed(2) + " KB");
            $("#spn").text("File Size: " + iSize.toFixed(2) + " KB    ");
        }
        else {
            //bootbox.alert("File Size: " + iSize + " MB");
            $("#spn").text("File Size: " + iSize + " MB    ");
        }
    });

    //Added and Changed by mayank on 14/09/2021 for Support Exe Release
    if (ReleaseTypeID == 3) {
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
        //$("#rdoYTest").prop('checked', true);
        if (ReleaseTypeID == 1)
            $("#rdoYTest").prop('checked', true);
        if (ReleaseTypeID == 2)
            $("#rdoNFinal").prop('checked', true);

    }
    //End
})

//All Inputted Text
function gridTojson() {
    var json = '{';
    var otArr = [];
    var tbl2 = $('#WebGrid tbody tr').each(function (i) {
        if ($(this)[0].rowIndex != 0) {
            x = $(this).children();
            var itArr = [];
            x.each(function () {
                if ($(this).children('input').length > 0) {
                    if ($(this).children('input').val() == "") {
                        //console.log(itArr.push('"' + $(this).children('input').val() + '"'));
                        //alert("❌ One of the Values in the Modification Details are empty.");                                              
                    }
                    if (enhancementsFixedBugsInputRules.test($(this).children('input').val())) {
                        itArr.push('"' + $(this).children('input').val() + '"');
                    }
                    else {
                        bootbox.alert("❌ One of the Values in the Modification Details is invalid.");
                    }
                }
                if ($(this).children('select').length > 0) {
                    itArr.push('"' + $(this).children('select').val() + '"');
                }
            });
            otArr.push('"' + i + '": [' + itArr.join(',') + ']');
        }
    })
    json += otArr.join(",") + '}'
    return json;
}

function UpdateServicePackDetails(_griddata) {

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

    //Added by shubham bhagat on 05-10-2019
    var patt = new RegExp("^[a-zA-Z0-9-/., ]+$");

    var ServicePackDescbool = patt.test($('#txtServicePackDesc').val());
    if (!ServicePackDescbool) {
        bootbox.alert("Invalid Description");
        return;
    }

    //Added by shubham bhagat on 05-10-2019
    var InstallationProcedurebool = patt.test($('#txtInstallationProcedure').val());
    if (!InstallationProcedurebool) {
        bootbox.alert("Invalid Installation Procedure");
        return;
    }

    //disable the default form submission
    event.preventDefault();
    var formData = new FormData();
    var fileOfficeOrder = document.getElementById("UploadReleaseFile").files[0];

    //Added by shubham bhagat on 18-09-2019
    // commented below file check
    //if (fileOfficeOrder == null) {
    //    bootbox.alert("Please upload Release File");
    //    return;
    //}

    //if ($("#lstChangeType").val() == 0) {
    //    bootbox.alert("Please Select Changes/Modification Type!");
    //    return;
    //}

    //Added by shubham bhagat on 05-10-2019
    if ($('#IsFileToUpdateID').val()=="true") {
        //alert("alert($('#IsFileToUpdateID').val()); in updating :: "+$('#IsFileToUpdateID').val());
        if (FileSizeForValidation > 40) {
            bootbox.alert("Please Upload file size less than 40 MB!");
            return;
        }
    }

    
    //Added by shubham bhagat on 18-09-2019
    formData.append("IsFileToUpdateID", $('#IsFileToUpdateID').val());

    formData.append("ReleaseType", $("#lstReleaseTypes").val());
    formData.append("File", fileOfficeOrder);
    formData.append("SpID", $('#spID').val());
    formData.append("Major", $('#onlyMajorNumber').val());
    formData.append("Minor", $('#onlyMinorNumber').val());
    //Added by shubham bhagat on 18-09-2019
    //Commented and changed on 25-09-2019 because it is sending undefined value.
    //formData.append("FinalTestValue", $('input[name=radioName]:checked', '#frmAddEditServicePackDetails').val());
    formData.append("FinalTestValue", $('[name="ServicePackDetails.IsTestOrFinal"]:checked').val());    
    formData.append("IsActiveValue", $('[name="ServicePackDetails.IsActive"]:checked').val());
    formData.append("ServicePackDescription", $('#txtServicePackDesc').val());
    formData.append("InstallationProcedure", $('#txtInstallationProcedure').val());
    formData.append("BugsList", _griddata);
    formData.append("GridCount", countofGrid);
    var RequestToken = $('[name=__RequestVerificationToken]').val();
    formData.append("__RequestVerificationToken", RequestToken);
    $.ajax({
        url: "/ServicePack/ServicePackDetails/UpdateServicePackDetails",
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
                $('.ClsServicePackReleaseFiles').val('');
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
                //else if (response.success == false && response.errormsgType == 14) {
                //    bootbox.alert(response.message);
                //}
                //else {
                //    bootbox.alert("Error While Adding Service Pack Details!");
                //}
            }
        },
        error: function (xhr, status, err) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
            });
        }

    });
}



function DeleteServicePackChangeEntry(id) {
    $.ajax({
        url: "/ServicePack/ServicePackDetails/ServicePackChangesDetailsEntry",
        type: 'POST',
        datatype: 'json',
        async: false,
        cache: false,
        data: { id: id },
        success: function (json) {
            if (json.serverError == true && json.success == false) {
                if (json.message != null || json.message != "") {
                    bootbox.alert(json.message, function (confirmed) {
                        if (confirmed) {
                            //Added by shubham bhagat on 18-09-2019
                            window.location.href = "/Home/HomePage"
                            //window.location.href = "/Login/Error";
                        }
                    });
                }
            }
            else {
                if (json.success == true) {
                    bootbox.alert(json.message, function () {
                        window.location.reload();
                    });
                }
                else {
                    bootbox.alert(json.message);
                }
            }
        },
        error: function (xhr, status, error) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
            });
        }
    });
}

//Added by shubham bhagat on 18-09-2019
function clearFile()
{ 
    $(".ClsServicePackReleaseFiles").val('');
    $("#Upload-filename").text('');

    $('#IsFileToUpdateID').val("false");
    $('span.spnClear').hide();
    //$('span.spnClear').text('');
    $('#spn').html('');
    //alert("clearFile()");
}