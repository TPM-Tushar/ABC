
//Global variables.
var token = '';
var header = {};


//alert("in addEditMenu.js");
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#MenuNameId').focus();
    //alert($('#DropDownValuesCanChange').val());

    //var dd = $('#DropDownValuesCanChange').val();
    //alert(dd);
    if (($('#DropDownValuesCanChange').val() === true) || $('#DropDownValuesCanChange').val() === "True") {
        //if ($('#DropDownValuesCanChange').val()) {
        //alert("in if");

        //alert($('#DropDownValuesCanChange').val());
        //document.getElementById("parentMenuDetailsList").disabled = true;

        $('#parentMenuDetailsList').prop("disabled", false);

        $('#firstChildMenuDetailsList').prop("disabled", false);

        $('#secondChildMenuDetailsList').prop("disabled", false);

    }
    else {
        // alert("else ");

        //alert($('#DropDownValuesCanChange').val());
        //document.getElementById("parentMenuDetailsList").disabled = true;

        $('#parentMenuDetailsList').prop("disabled", true);

        $('#firstChildMenuDetailsList').prop("disabled", true);

        $('#secondChildMenuDetailsList').prop("disabled", true);

    }

    var abc = $('.listbox').multiselect({
        includeSelectAllOption: true
    });
    $('.multiselect').addClass('minimal');
    $('.multiselect').addClass('btn-sm');
    //$('#listbox').multiselect({
    //    includeSelectAllOption: true
    //});
    //$('#dd').multiselect({
    //      includeSelectAllOption:true
    //});
    // Trying to increase length of multiselect Drop down
    //abc.multiselect.width(400);


    $("#closeMenuDetailsForm").click(function () {
        $("#panelMenuDetailsForm").hide();
        $("#ShowAddMenuFormID").show();
        OpenMenuDetailsList();
        //$("#ShowAddMenuFormID").hide();
    });

    $("#btnCancelForAdd").click(function () {
        $("#panelMenuDetailsForm").hide();
        $("#ShowAddMenuFormID").show();
        OpenMenuDetailsList();
    });

    $("#btnCancelForEdit").click(function () {
        $("#panelMenuDetailsForm").hide();
        $("#ShowAddMenuFormID").show();
        OpenMenuDetailsList();
    });


    $('#parentMenuDetailsList').change(function () {
        //alert("in parent change");

        if ($('#parentMenuDetailsList').val() == 0) {
            $('#secondChildMenuListDivId').hide();
            $('#firstChildMenuListDivId').hide();
            $('#firstChildMenuDetailsList').empty();
            $('#secondChildMenuDetailsList').empty();
        }

        if ($('#parentMenuDetailsList').val() == -1) {
            bootbox.alert("Please Select Parent Menu.");
        }

        if ($('#parentMenuDetailsList').val() > 0) {
            $('#firstChildMenuListDivId').show();
            $('#secondChildMenuListDivId').hide();
            BlockUI();
            $.ajax({
                url: '/UserManagement/MenuDetails/GetFirstChildMenuDetailsList/',
                data: { "parentId": $('#parentMenuDetailsList option:selected').val() },
                datatype: "json",
                type: "POST",
                success: function (data) {
                    if (data.serverError == true) {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            function () {
                                window.location.href = "/Home/HomePage"
                            });
                    }
                    else {
                    $('#firstChildMenuDetailsList').empty();
                    $('#secondChildMenuDetailsList').empty();
                    $.each(data.firstChildMenuDetailsList, function (i, firstChildMenuDetails) {
                        $('#firstChildMenuDetailsList').append('<option value="' + firstChildMenuDetails.Value + '">' + firstChildMenuDetails.Text + '</option>');
                        });
                    } 
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    //bootbox.alert("Error " + err);
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    $.unblockUI();
                }
            });
        }
    });

    $('#firstChildMenuDetailsList').change(function () {
        //alert("in first child change");
        if ($('#firstChildMenuDetailsList').val() == 0) {
            $('#secondChildMenuListDivId').hide();
        }
        //alert($('#firstChildMenuDetailsList').val());
        if ($('#firstChildMenuDetailsList').val() == -1) {
            bootbox.alert("Please Select First Child Menu.");
        }

        if ($('#firstChildMenuDetailsList').val() > 0) {

            $('#firstChildMenuListDivId').show();
            $('#secondChildMenuListDivId').show();
            BlockUI();
            $.ajax({
                url: '/UserManagement/MenuDetails/GetSecondChildMenuDetailsList/',
                data: { "firstChildMenuDetailsId": $('#firstChildMenuDetailsList option:selected').val() },
                datatype: "json",
                type: "POST",
                success: function (data) {
                    if (data.serverError == true) {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            function () {
                                window.location.href = "/Home/HomePage"
                            });
                    }
                    else {
                        $('#secondChildMenuDetailsList').empty();
                        $.each(data.secondChildMenuDetailsList, function (i, secondChildMenuDetails) {
                            $('#secondChildMenuDetailsList').append('<option value="' + secondChildMenuDetails.Value + '">' + secondChildMenuDetails.Text + '</option>');
                        });
                    }
                    $.unblockUI();
                },
                error: function (xhr, status, err) {
                    //bootbox.alert("Error " + err);
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    $.unblockUI();
                }
            });
        }
    });


    $('#secondChildMenuDetailsList').change(function () {
        //alert("in second child change");
        if ($('#secondChildMenuDetailsList').val() != 0) {
            bootbox.alert("Please Select Self.");
        }
    });

    if ($('#parentMenuDetailsList').val() == -1 || $('#parentMenuDetailsList').val() == 0) {
        $('#firstChildMenuListDivId').hide();
        $('#secondChildMenuListDivId').hide();
    }

    if ($('#parentMenuDetailsList').val() >= 1 && $('#firstChildMenuDetailsList').val() == 0) {
        //alert($('#parentMenuDetailsList').val());
        //alert($('#firstChildMenuDetailsList').val());
        $('#secondChildMenuListDivId').hide();
    }

});

function addMenu() {

    var form = $("#MenuFormID");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);


    if ($('#parentMenuDetailsList').val() == -1) {
        bootbox.alert("Please Select Parent Menu.");
    }
    else if ($('#firstChildMenuDetailsList').val() == -1) {
        bootbox.alert("Please Select First Child Menu.");
    }
    else if ($('#parentMenuDetailsList').val() != 0 && $('#firstChildMenuDetailsList').val() != 0 && $('#secondChildMenuDetailsList').val() != 0) {
        bootbox.alert("Please Select Second Child Menu as Self.");
    }
    else if ($("#MenuFormID").valid()) {
        BlockUI();
        $.ajax({
            url: "/UserManagement/MenuDetails/AddMenu",
            type: "POST",
            data: $('#MenuFormID').serialize(),
            dataType: "json",
            headers: header,
            success: function (data) {
                //alert(data.menuDetailsResponseModel.Result);

                if (data.menuDetailsResponseModel.Result) {

                    //bootbox.alert(data.menuDetailsResponseModel.Message);
                    bootbox.alert({
                        //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.menuDetailsResponseModel.Message + '</span>',
                        callback: function () {
                            $("#addEditDivId").hide();
                            //$("#createPartialViewId").hide();
                            //menuTable.destroy();

                            retriveMenusFromDB();
                            $("#ShowAddMenuFormID").show();
                            OpenMenuDetailsList();
                            $("#panelMenuDetailsForm").hide();
                        }
                    });
                }
                else {
                    //.alert(data.menuDetailsResponseModel.Message);
                    bootbox.alert({
                        //   size: 'small',
                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.menuDetailsResponseModel.Message + '</span>',
                        function() {
                            window.location.href = "/Home/HomePage"
                        }
                    });
                }
                $.unblockUI();
            },
            error: function (xhr, status, err) {
                //bootbox.alert("Error " + err);
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                $.unblockUI();
            }

        });
    }
    else {
        return;
    }
}

function editMenu() {
    //alert("edit");

    //error
    bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Are you sure you want to update?</span>",
        buttons: {   
            cancel: {
                label: '<i class="fa fa-times"></i> No',
                className:'pull-right margin-left-NoBtn'
            }, confirm: {
                label: '<i class="fa fa-check"></i> Yes'
            }
        },
        callback: function (result) {

            if (result) {
                var form = $("#MenuFormID");
                form.removeData('validator');
                form.removeData('unobtrusiveValidation');
                $.validator.unobtrusive.parse(form);


                if ($('#parentMenuDetailsList').val() == -1) {
                    bootbox.alert("Please Select Parent Menu.");
                }
                else if ($('#firstChildMenuDetailsList').val() == -1) {
                    bootbox.alert("Please Select First Child Menu.");
                }
                else if ($('#parentMenuDetailsList').val() != 0 && $('#secondChildMenuDetailsList').val() != 0 && $('#firstChildMenuDetailsList').val() != 0) {
                    bootbox.alert("Please Select Second Child Menu as Self.");
                }
                else if ($("#MenuFormID").valid()) {
                    BlockUI();
                    $.ajax({
                        url: "/UserManagement/MenuDetails/UpdateMenu",
                        type: "POST",
                        data: $('#MenuFormID').serialize(),

                        dataType: "json",
                        headers: header,
                        success: function (data) {
                            if (data.menuDetailsResponseModel.Result) {
                                //bootbox.alert(data.menuDetailsResponseModel.Message);
                                bootbox.alert({
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.menuDetailsResponseModel.Message + '</span>',
                                    callback: function () {
                                        $("#addEditDivId").hide();
                                        //$("#editPartialViewId").hide();
                                        retriveMenusFromDB();
                                        $("#ShowAddMenuFormID").show();
                                        OpenMenuDetailsList();
                                        $("#panelMenuDetailsForm").hide();
                                    }
                                });

                            }
                            else {
                                //bootbox.alert(data.menuDetailsResponseModel.Message);
                                bootbox.alert({
                                    //   size: 'small',
                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.menuDetailsResponseModel.Message + '</span>',
                                    callback: function () {
                                        window.location.href = "/Home/HomePage"
                                        //$("#DtlsSearchParaListCollapse").trigger('click');
                                    }
                                });
                            }
                            $.unblockUI();
                        },
                        error: function (xhr, status, err) {
                            //bootbox.alert("Error " + err);
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                callback: function () {
                                }
                            });
                            $.unblockUI();
                        }
                    });
                }
            }
            else { return; }
        }
    });
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