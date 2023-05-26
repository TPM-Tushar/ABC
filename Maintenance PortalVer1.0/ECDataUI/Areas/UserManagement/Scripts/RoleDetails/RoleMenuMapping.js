

//Global variables.
var token = '';
var header = {};

//alert("Role Menu Mapping");

$(document).ready(function () {


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;



    //$("#roleMenuMappingHeadingId").Show();


    $("#closeRoleDetailsForm").click(function () {
        $("#panelRoleDetailsForm").hide();
        $("#ShowAddEditRoleDetailsFormID").show();
        OpenRoleDetailsList();
        //$("#ShowAddMenuFormID").hide();
    });

    $('#parentMenuDetailsDIVid').show();
    $('#parentMenuDetailsMapButtonId').show();

    $('#FirstChildMenuDetailsDIVid').hide();
    $('#firstChildMenuDetailsMapButtonId').hide();

    $('#SecondChildDetailsDIVid').hide();
    $('#secondChildMenuDetailsMapButtonId').hide();


    // For Getting First Child List and Map or Unmap Button of Parent menu and Role
    $('#parentMenuDetailsList').change(function () {
         
        if ($('#parentMenuDetailsList').val() == "0") {
            
            $('#parentMenuDetailsMapButtonId').hide();
            $('#FirstChildMenuDetailsDIVid').hide();
            $('#firstChildMenuDetailsMapButtonId').hide();
            $('#SecondChildDetailsDIVid').hide();
            $('#secondChildMenuDetailsMapButtonId').hide();
        } else

        //if ($('#parentMenuDetailsList').val() > 0)

        {
           
            //$('#parentMenuDetailsDIVid').show();
            //$('#secondChildMenuDetailsMapButtonId').show();

            parentMenuDetailsList();
        }
    });

    // For Getting Second Child List and Map or Unmap Button of First Child menu and Role
    $('#firstChildMenuDetailsList').change(function () {
        //alert("in first Child change");

        if ($('#firstChildMenuDetailsList').val() == 0) {
            $('#firstChildMenuDetailsMapButtonId').hide();
            $('#SecondChildDetailsDIVid').hide();
            $('#secondChildMenuDetailsMapButtonId').hide();
        } else

        // if ($('#firstChildMenuDetailsList').val() > 0) 
        {

            //$('#parentMenuDetailsDIVid').show();
            //$('#parentMenuDetailsMapButtonId').show();

            //$('#FirstChildMenuDetailsDIVid').show();
            $('#firstChildMenuDetailsMapButtonId').show();

            $('#SecondChildDetailsDIVid').show();
            $('#secondChildMenuDetailsMapButtonId').hide();

            firstChildMenuDetailsList();
        }
    });

    // For Getting Map or Unmap Button of Second Child menu and Role
    $('#secondChildMenuDetailsList').change(function () {
        //alert("in second Child change");

        if ($('#secondChildMenuDetailsList').val() == 0) {
            $('#secondChildMenuDetailsMapButtonId').hide();
        }

        if ($('#secondChildMenuDetailsList').val() > 0) {

            //$('#parentMenuDetailsDIVid').show();
            //$('#parentMenuDetailsMapButtonId').show();

            //$('#firstChildMenuListDivId').show();
            //$('#firstChildMenuDetailsMapButtonId').show();

            //$('#SecondChildDetailsDIVid').show();
            secondChildMenuDetailsList();
        }
    });
});

function parentMenuDetailsList() {
    BlockUI();
    $.ajax({
        url: '/UserManagement/RoleDetails/FirstChildMenuList/',
        //data: $('#RoleDetailsFormID').serialize(),
        //data: { "roleDetailsModel": roleDetailsModel },
        data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
        datatype: "json",
        type: "POST",
        success: function (data) {
            if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                $('#parentMenuDetailsMapButtonId').show();

                $('#FirstChildMenuDetailsDIVid').show();
                $('#firstChildMenuDetailsMapButtonId').hide();

                $('#SecondChildDetailsDIVid').hide();

                $('#firstChildMenuDetailsList').empty();
                $('#secondChildMenuDetailsList').empty();
                // MapUnmapButtonForParent = 
                //alert(MapUnmapButtonForParent);
                $('#parentMenuDetailsMapButtonId').html('');
                //alert(data.MapUnmapButtonForParent);
                $('#parentMenuDetailsMapButtonId').html(data.MapUnmapButtonForParent);
                $.each(data.firstChildMenuDetailsList, function (i, firstChildMenuDetails) {
                    $('#firstChildMenuDetailsList').append('<option value="' + firstChildMenuDetails.Value + '">' + firstChildMenuDetails.Text + '</option>');
                });
            }
            $.unblockUI();
        },
        error: function (xhr) {
            //alert('error');
            bootbox.alert({
                //size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
            });
            $.unblockUI();
        }
    });
}

function firstChildMenuDetailsList() {
    BlockUI();
    $.ajax({
        url: '/UserManagement/RoleDetails/SecondChildMenuList/',
        //data: $('#RoleDetailsFormID').serialize(),
        //data: { "roleDetailsModel": roleDetailsModel },
        data: { "EncryptedID": $('#EncryptedID').val(), "firstChildMenuDetailId": $('#firstChildMenuDetailsList').val() },
        datatype: "json",
        type: "POST",
        success: function (data) {
            if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                //alert(data.IsParentMenuMapped);
                if (data.IsParentMenuMapped == true) {
                    $('#firstChildMenuDetailsMapButtonId').show();

                    $('#SecondChildDetailsDIVid').show();
                    $('#secondChildMenuDetailsMapButtonId').hide();

                    $('#secondChildMenuDetailsList').empty();

                    $('#firstChildMenuDetailsMapButtonId').html('');
                    //alert(data.MapUnmapButtonForParent);
                    $('#firstChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForFirstChild);
                    $.each(data.secondChildMenuDetailsList, function (i, secondChildMenuDetails) {
                        $('#secondChildMenuDetailsList').append('<option value="' + secondChildMenuDetails.Value + '">' + secondChildMenuDetails.Text + '</option>');
                    });
                    $.unblockUI();
                }
                else {
                    $('#firstChildMenuDetailsMapButtonId').hide();
                    $('#SecondChildDetailsDIVid').hide();
                    $('#secondChildMenuDetailsMapButtonId').hide();
                    bootbox.alert("Please Map Parent Menu.");
                    $.unblockUI();
                }
            }
            $.unblockUI();
        },
        error: function (xhr) {
            //alert('error');
            bootbox.alert({
                //size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
            });
            $.unblockUI();
        }
    });

}

function secondChildMenuDetailsList() {
    BlockUI();
    $.ajax({
        url: '/UserManagement/RoleDetails/GetMapUnmapButtonForSecondChildMenu/',
        //data: $('#RoleDetailsFormID').serialize(),
        //data: { "roleDetailsModel": roleDetailsModel },
        data: { "EncryptedID": $('#EncryptedID').val(), "secondChildMenuDetailId": $('#secondChildMenuDetailsList').val() },
        datatype: "json",
        type: "POST",
        success: function (data) {
            if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                if (data.IsFirstChildMenuMapped == true) {
                    $('#secondChildMenuDetailsMapButtonId').show();
                    $('#secondChildMenuDetailsMapButtonId').html('');
                    //alert(data.MapUnmapButtonForSecondChild);
                    $('#secondChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForSecondChild);
                    $.unblockUI();
                }
                else {
                    $('#secondChildMenuDetailsMapButtonId').hide();
                    bootbox.alert("Please Map First Child Menu.");
                    $.unblockUI();
                }
            }
            $.unblockUI();
        },
        error: function (xhr) {
            //alert('error');
            bootbox.alert({
                //size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
            });
            $.unblockUI();
        }
    });
}

// MapParentMenu
function MapParentMenu(EncryptedID) {
    //alert("MapParentMenu");
    if ($('#parentMenuDetailsList').val() == 0) {
        bootbox.alert("Please Select Parent Menu");
    }
    else {
        //$.ajax({
        //    url: '/UserManagement/RoleDetails/MapParentMenu/',
        //    data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
        //    datatype: "json",
        //    type: "POST",
        //    success: function (data) {
        //        $('#parentMenuDetailsMapButtonId').html('');
        //        //alert(data.MapUnmapButtonForParent);
        //        $('#parentMenuDetailsMapButtonId').html(data.MapUnmapButtonForParent);
        //    }
        //});

        var bootboxConfirm = bootbox.confirm({
            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
            message: "<span class='boot-alert-txt'>Do you want to Map Parent menu ?</span>",

            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> No',
                    className: 'pull-right margin-left-NoBtn'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Yes'
                }
            },
            callback: function (result) {
                if (result) {
                    BlockUI();
                    $.ajax({
                        url: '/UserManagement/RoleDetails/MapParentMenu/',
                        data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
                        datatype: "json",
                        type: "POST",
                        headers: header,
                        success: function (data) {
                            if (data.serverError == true) {
                                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                    function () {
                                        window.location.href = "/Home/HomePage"
                                    });
                            }
                            else {
                                $('#parentMenuDetailsMapButtonId').html('');
                                //alert(data.MapUnmapButtonForParent);
                                $('#parentMenuDetailsMapButtonId').html(data.MapUnmapButtonForParent);
                                parentMenuDetailsList();
                            }
                            $.unblockUI();
                        },
                        error: function (xhr) {
                            //alert('error');
                            bootbox.alert({
                                //size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            });
                            $.unblockUI();
                        }
                    });
                }
            }
        });
    }
}

// UnmapParentMenu
function UnmapParentMenu(EncryptedID) {
    //debugger;
    //alert("UnmapParentMenu");
    if ($('#parentMenuDetailsList').val() == 0) {
        bootbox.alert("Please Select Parent Menu");
    }
    else {
        $.ajax({
            url: '/UserManagement/RoleDetails/FirstChildList_SecondChildList_BeforeParentUnmap/',
            data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
            datatype: "json",
            type: "POST",
            headers: header,
            success: function (data) {
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    //   if (data.IsFirstAndSecondChildListEmpty == true || data.IsFirstAndSecondChildListEmpty === "True") {
                    //if (data.IsFirstAndSecondChildListEmpty === "True") {
                    if (data.IsFirstAndSecondChildListEmpty == true && data.IsSecondChildListEmpty == true) {
                        //debugger;
                        ////alert("in IsFirstAndSecondChildListEmpty == true if");
                        var bootboxConfirm = bootbox.confirm({
                            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                            message: "<span class='boot-alert-txt'>Do you want to Unmap Parent menu ?</span>",

                            buttons: {
                                cancel: {
                                    label: '<i class="fa fa-times"></i> No',
                                    className: 'pull-right margin-left-NoBtn'
                                },
                                confirm: {
                                    label: '<i class="fa fa-check"></i> Yes'
                                }
                            },
                            callback: function (result) {
                                //debugger;
                                if (result) {
                                    BlockUI();
                                    $.ajax({
                                        url: '/UserManagement/RoleDetails/UnmapParentMenu/',
                                        data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
                                        datatype: "json",
                                        type: "POST",
                                        headers: header,
                                        success: function (data) {
                                            if (data.serverError == true) {
                                                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                                    function () {
                                                        window.location.href = "/Home/HomePage"
                                                    });
                                            }
                                            else {
                                                $('#parentMenuDetailsMapButtonId').html('');
                                                $('#parentMenuDetailsMapButtonId').html(data.MapUnmapButtonForParent);
                                                $('#firstChildMenuDetailsMapButtonId').html('');
                                                parentMenuDetailsList();
                                            }
                                            $.unblockUI();
                                        },
                                        error: function (xhr) {
                                            //alert('error');
                                            bootbox.alert({
                                                //size: 'small',
                                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                            });
                                            $.unblockUI();
                                        }
                                    });
                                }
                            }
                        });
                    }
                    else {

                        var submenuList, i, j, firstChildList = "</br><ul><font size='3'><b>Menu at First Level</b></font><font size='2'>", secondChildList = "<ul><font size='3'><b>Menu at Second Level</b></font><font size='2'>", subMenuList = "";

                        for (i in data.FirstChildListString) {
                            firstChildList += "<li>" + data.FirstChildListString[i] + "</li>";
                        }
                        firstChildList += "</font></ul>";
                        for (i in data.SecondChildListString) {
                            secondChildList += "<li>" + data.SecondChildListString[i] + "</li>";
                        }
                        secondChildList += "</font></ul>";


                        if (data.IsSecondChildListEmpty == true) {
                            //alert("if");
                            submenuList = firstChildList;
                        }
                        else {
                            //alert("else");
                            submenuList = firstChildList + secondChildList;
                        }

                        var checkbox = "<script>var IsCheckboxCheck=false; $(document).ready(function () { $('#RemoveSubmenus').change(function() { if($(this).is(':checked')) {  IsCheckboxCheck=true; }else{ IsCheckboxCheck=false;}      });});</script><input type='checkbox' name='RemoveSubmenus' id='RemoveSubmenus'  style='height:20px;width:20px;'><font size='3' style='top:-.25em;position:relative;'>&nbsp;To Unmap the Submenus please check.<br></font>";


                        var dialog = bootbox.dialog({
                            title: '<font size="4"><b>Confirm</b></font>',
                            message: "<i><font size='4' color='red'>The Menu \"<b>" + data.ParentMenuName + "</b>\" has the following submenus assaigned to the \"<b>" + data.RoleName + "\"</b> role</font></i>" + submenuList + checkbox,
                            closeButton: false,
                            buttons: {
                                cancel: {
                                    label: "Cancel",
                                    className: 'btn-danger pull-right margin-left-NoBtn marginRight',
                                    callback: function () {
                                        //Example.show('Custom cancel clicked');
                                    }
                                },
                                Unmap: {
                                    label: "Unmap",
                                    className: 'btn-info',
                                    callback: function () {


                                        if (IsCheckboxCheck) {
                                            var bootboxConfirm = bootbox.confirm({
                                                title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                                                message: "<span class='boot-alert-txt'>Do you want to Unmap Parent menu with its Submenu?</span>",

                                                buttons: {
                                                    cancel: {
                                                        label: '<i class="fa fa-times"></i> No',
                                                        className: 'pull-right margin-left-NoBtn'
                                                    },
                                                    confirm: {
                                                        label: '<i class="fa fa-check"></i> Yes'
                                                    }
                                                },
                                                callback: function (result) {
                                                    if (result) {
                                                        BlockUI();
                                                        $.ajax({
                                                            url: '/UserManagement/RoleDetails/UnmapParentMenuAndSubMenu/',
                                                            data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
                                                            datatype: "json",
                                                            type: "POST",
                                                            headers: header,
                                                            success: function (data) {
                                                                if (data.serverError == true) {
                                                                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                                                        function () {
                                                                            window.location.href = "/Home/HomePage"
                                                                        });
                                                                }
                                                                else {
                                                                    $('#parentMenuDetailsMapButtonId').html('');
                                                                    $('#parentMenuDetailsMapButtonId').html(data.MapUnmapButtonForParent);
                                                                    $('#firstChildMenuDetailsMapButtonId').html('');
                                                                    parentMenuDetailsList();
                                                                }
                                                                $.unblockUI();
                                                            },
                                                            error: function (xhr) {
                                                                //alert('error');
                                                                bootbox.alert({
                                                                    //size: 'small',
                                                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                                                });
                                                                $.unblockUI();
                                                            }
                                                        });
                                                    }
                                                }
                                            });
                                        }
                                        else {
                                            var bootboxConfirm = bootbox.confirm({
                                                title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                                                message: "<span class='boot-alert-txt'>Do you want to Unmap Parent menu ?</span>",

                                                buttons: {
                                                    cancel: {
                                                        label: '<i class="fa fa-times"></i> No',
                                                        className: 'pull-right margin-left-NoBtn'
                                                    },
                                                    confirm: {
                                                        label: '<i class="fa fa-check"></i> Yes'
                                                    }
                                                },
                                                callback: function (result) {
                                                    if (result) {
                                                        BlockUI();
                                                        $.ajax({
                                                            url: '/UserManagement/RoleDetails/UnmapParentMenu/',
                                                            data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
                                                            datatype: "json",
                                                            type: "POST",
                                                            headers: header,
                                                            success: function (data) {
                                                                if (data.serverError == true) {
                                                                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                                                        function () {
                                                                            window.location.href = "/Home/HomePage"
                                                                        });
                                                                }
                                                                else {
                                                                    $('#parentMenuDetailsMapButtonId').html('');
                                                                    $('#parentMenuDetailsMapButtonId').html(data.MapUnmapButtonForParent);
                                                                    $('#firstChildMenuDetailsMapButtonId').html('');
                                                                    parentMenuDetailsList();
                                                                }
                                                                $.unblockUI();
                                                            },
                                                            error: function (xhr) {
                                                                //alert('error');
                                                                bootbox.alert({
                                                                    //size: 'small',
                                                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                                                });
                                                                $.unblockUI();
                                                            }
                                                        });
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                            },
                            onEscape: function () {
                                dialog.modal("hide");
                            }

                        });
                    }
                }
                $.unblockUI();
            },
            error: function (xhr) {
                //alert('error');
                bootbox.alert({
                    //size: 'small',
                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                });
                $.unblockUI();
            }
        });

        //var dialog = bootbox.dialog({
        //    title: 'A custom dialog with buttons and callbacks',
        //    message: "<p>This dialog has buttons. Each button has it's own callback function.</p>",
        //    buttons: {
        //        cancel: {
        //            label: "I'm a custom cancel button!",
        //            className: 'btn-danger',
        //            callback: function () {
        //                //Example.show('Custom cancel clicked');
        //            }
        //        },
        //        noclose: {
        //            label: "I'm a custom button, but I don't close the modal!",
        //            className: 'btn-warning',
        //            callback: function () {
        //                //return false;                      
        //            }
        //        },
        //        noclose2: {
        //            label: "I'm a custom button, but I don't close the modal!",
        //            className: 'btn-warning',
        //            callback: function () {
        //                //return false;
        //            }
        //        },
        //        ok: {
        //            label: "I'm a custom OK button!",
        //            className: 'btn-info',
        //            callback: function () {
        //                //Example.show('Custom OK clicked');
        //            }
        //        }
        //    }
        //});

        //    var bootboxConfirm = bootbox.confirm({
        //        title: "Confirm",
        //        message: "Do you want to Unmap Parent menu ?",
        //        buttons: {
        //            cancel: {
        //                label: '<i class="fa fa-times"></i> No',
        //                className: 'pull-right margin-left-NoBtn'
        //            },
        //            confirm: {
        //                label: '<i class="fa fa-check"></i> Yes'
        //            }
        //        },
        //        callback: function (result) {
        //            if (result) {
        //                BlockUI();
        //                $.ajax({
        //                    url: '/UserManagement/RoleDetails/UnmapParentMenu/',
        //                    data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
        //                    datatype: "json",
        //                    type: "POST",
        //                    success: function (data) {
        //                        $('#parentMenuDetailsMapButtonId').html('');
        //                        $('#parentMenuDetailsMapButtonId').html(data.MapUnmapButtonForParent);
        //                        $('#firstChildMenuDetailsMapButtonId').html('');
        //                        parentMenuDetailsList();
        //                        $.unblockUI();
        //                    },
        //                    error: function (xhr) {
        //                        alert('error');
        //                        $.unblockUI();
        //                    }
        //                });
        //            }
        //        }
        //    });
    }
}

//// UnmapParentMenu
//function UnmapParentMenu(EncryptedID) {
//    //alert("UnmapParentMenu");
//    if ($('#parentMenuDetailsList').val() == 0) {
//        bootbox.alert("Please Select Parent Menu");
//    }
//    else {
//        //$.ajax({
//        //    url: '/UserManagement/RoleDetails/UnmapParentMenu/',
//        //    data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
//        //    datatype: "json",
//        //    type: "POST",
//        //    success: function (data) {
//        //        $('#parentMenuDetailsMapButtonId').html('');
//        //        //alert(data.MapUnmapButtonForParent);
//        //        $('#parentMenuDetailsMapButtonId').html(data.MapUnmapButtonForParent);
//        //    }
//        //});
//        var bootboxConfirm = bootbox.confirm({
//            title: "Confirm",
//            message: "Do you want to Unmap Parent menu ?",
//            buttons: {
//                cancel: {
//                    label: '<i class="fa fa-times"></i> No',
//                    className: 'pull-right margin-left-NoBtn'
//                },
//                confirm: {
//                    label: '<i class="fa fa-check"></i> Yes'
//                }
//            },
//            callback: function (result) {
//                if (result) {
//                    BlockUI();
//                    $.ajax({
//                        url: '/UserManagement/RoleDetails/UnmapParentMenu/',
//                        data: { "EncryptedID": $('#EncryptedID').val(), "parentMenuDetailId": $('#parentMenuDetailsList').val() },
//                        datatype: "json",
//                        type: "POST",
//                        success: function (data) {
//                            $('#parentMenuDetailsMapButtonId').html('');
//                            //alert(data.MapUnmapButtonForParent);
//                            $('#parentMenuDetailsMapButtonId').html(data.MapUnmapButtonForParent);
//                            $('#firstChildMenuDetailsMapButtonId').html('');
//                            parentMenuDetailsList();
//                            $.unblockUI();
//                        },
//                        error: function (xhr) {
//                            alert('error');
//                            $.unblockUI();
//                        }
//                    });
//                }
//            }
//        });
//    }
//}

// MapFirstChildMenu
function MapFirstChildMenu(EncryptedID) {
    // alert("MapFirstChildMenu");
    if ($('#firstChildMenuDetailsList').val() == 0) {
        bootbox.alert("Please Select First Child Menu");
    }
    else {
        //$.ajax({
        //    url: '/UserManagement/RoleDetails/MapFirstChildMenu/',
        //    data: { "EncryptedID": $('#EncryptedID').val(), "firstChildMenuDetailId": $('#firstChildMenuDetailsList').val() },
        //    datatype: "json",
        //    type: "POST",
        //    success: function (data) {

        //        $('#firstChildMenuDetailsMapButtonId').html('');
        //        //alert(data.MapUnmapButtonForFirstChild);
        //        $('#firstChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForFirstChild);
        //    }
        //});

        var bootboxConfirm = bootbox.confirm({
            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
            message: "<span class='boot-alert-txt'>Do you want to Map First Child menu ?</span>",

            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> No',
                    className: 'pull-right margin-left-NoBtn'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Yes'
                }
            },
            callback: function (result) {
                if (result) {
                    BlockUI();
                    $.ajax({
                        url: '/UserManagement/RoleDetails/MapFirstChildMenu/',
                        data: { "EncryptedID": $('#EncryptedID').val(), "firstChildMenuDetailId": $('#firstChildMenuDetailsList').val() },
                        datatype: "json",
                        type: "POST",
                        headers: header,
                        success: function (data) {
                            if (data.serverError == true) {
                                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                    function () {
                                        window.location.href = "/Home/HomePage"
                                    });
                            }
                            else {
                                $('#firstChildMenuDetailsMapButtonId').html('');
                                //alert(data.MapUnmapButtonForFirstChild);
                                $('#firstChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForFirstChild);
                                firstChildMenuDetailsList();
                            }
                            $.unblockUI();
                        },
                        error: function (xhr) {
                            //alert('error');
                            bootbox.alert({
                                //size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            });
                            $.unblockUI();
                        }
                    });
                }
            }
        });

    }
}


// UnmapFirstChildMenu
function UnmapFirstChildMenu(EncryptedID) {
    //alert("UnmapFirstChildMenu");
    if ($('#firstChildMenuDetailsList').val() == 0) {
        bootbox.alert("Please Select First Child Menu");
    }
    else {
        $.ajax({
            url: '/UserManagement/RoleDetails/SecondChildList_BeforeFirstChildUnmap/',
            data: { "EncryptedID": $('#EncryptedID').val(), "firstChildMenuDetailId": $('#firstChildMenuDetailsList').val() },
            datatype: "json",
            type: "POST",
            headers: header,
            success: function (data) {
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    if (data.IsSecondChildListEmpty == true || data.IsSecondChildListEmpty === "True") {
                        ////alert("in 1st If");
                        var bootboxConfirm = bootbox.confirm({
                            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                            message: "<span class='boot-alert-txt'>Do you want to Unmap First Child menu ?</span>",
                            buttons: {
                                cancel: {
                                    label: '<i class="fa fa-times"></i> No',
                                    className: 'pull-right margin-left-NoBtn'
                                },
                                confirm: {
                                    label: '<i class="fa fa-check"></i> Yes'
                                }
                            },
                            callback: function (result) {
                                if (result) {
                                    BlockUI();
                                    $.ajax({
                                        url: '/UserManagement/RoleDetails/UnmapFirstChildMenu/',
                                        data: { "EncryptedID": $('#EncryptedID').val(), "firstChildMenuDetailId": $('#firstChildMenuDetailsList').val() },
                                        datatype: "json",
                                        type: "POST",
                                        headers: header,
                                        success: function (data) {
                                            if (data.serverError == true) {
                                                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                                    function () {
                                                        window.location.href = "/Home/HomePage"
                                                    });
                                            }
                                            else {
                                                $('#firstChildMenuDetailsMapButtonId').html('');
                                                //alert(data.MapUnmapButtonForFirstChild);
                                                $('#firstChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForFirstChild);
                                                firstChildMenuDetailsList();
                                            }
                                            $.unblockUI();
                                        },
                                        error: function (xhr) {
                                            //alert('error');
                                            bootbox.alert({
                                                //size: 'small',
                                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                            });
                                            $.unblockUI();
                                        }
                                    });
                                }
                            }
                        });
                    }
                    else {
                        var j, submenuList = "<font size='3'><ul><b>Menu at Second Level</b></font><font size='2'>";

                        for (i in data.SecondChildListString) {
                            submenuList += "<li>" + data.SecondChildListString[i] + "</li>";
                        }
                        submenuList += "</font></ul>";
                        var checkbox = "<script>var IsCheckboxCheck=false; $(document).ready(function () { $('#RemoveSubmenus').change(function() { if($(this).is(':checked')) {  IsCheckboxCheck=true; }else{ IsCheckboxCheck=false;}      });});</script><input type='checkbox' name='RemoveSubmenus' id='RemoveSubmenus'  style='height:20px;width:20px;'><font size='3' style='top:-.25em;position:relative;'>&nbsp;To Unmap the Submenus please check.<br></font>";
                        var dialog = bootbox.dialog({
                            title: '<font size="4"><b>Confirm</b><font>',
                            message: "<font size='4' color='red'><i>The Menu \"<b>" + data.FirstChildMenuName + "\"</b> has the following submenus assigned to the \"<b>" + data.RoleName + "\"</b> role</i></font>" + submenuList + checkbox,
                            closeButton: false,

                            buttons: {
                                cancel: {
                                    label: "Cancel",
                                    className: 'btn-danger pull-right margin-left-NoBtn marginRight',

                                    callback: function () {
                                        //Example.show('Custom cancel clicked');
                                    }
                                },

                                Unmap: {
                                    label: "Unmap",
                                    className: 'btn-info',
                                    callback: function () {

                                        if (IsCheckboxCheck) {
                                            ////alert("unmap first child menu and sub menu");
                                            var bootboxConfirm = bootbox.confirm({
                                                title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                                                message: "<span class='boot-alert-txt'>Do you want to Unmap First Child menu with its Submenu?</span>",
                                                buttons: {
                                                    cancel: {
                                                        label: '<i class="fa fa-times"></i> No',
                                                        className: 'pull-right margin-left-NoBtn'
                                                    },
                                                    confirm: {
                                                        label: '<i class="fa fa-check"></i> Yes'
                                                    }
                                                },
                                                callback: function (result) {
                                                    if (result) {
                                                        BlockUI();
                                                        $.ajax({
                                                            url: '/UserManagement/RoleDetails/UnmapFirstChildMenuAndSubMenu/',
                                                            data: { "EncryptedID": $('#EncryptedID').val(), "firstChildMenuDetailId": $('#firstChildMenuDetailsList').val() },
                                                            datatype: "json",
                                                            type: "POST",
                                                            headers: header,
                                                            success: function (data) {
                                                                if (data.serverError == true) {
                                                                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                                                        function () {
                                                                            window.location.href = "/Home/HomePage"
                                                                        });
                                                                }
                                                                else {
                                                                    $('#firstChildMenuDetailsMapButtonId').html('');
                                                                    //alert(data.MapUnmapButtonForFirstChild);
                                                                    $('#firstChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForFirstChild);
                                                                    firstChildMenuDetailsList();
                                                                }
                                                                $.unblockUI();
                                                            },
                                                            error: function (xhr) {
                                                                //alert('error');
                                                                bootbox.alert({
                                                                    //size: 'small',
                                                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                                                });
                                                                $.unblockUI();
                                                            }
                                                        });
                                                    }
                                                }
                                            });

                                        } else {
                                            var bootboxConfirm = bootbox.confirm({
                                                title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                                                message: "<span class='boot-alert-txt'>Do you want to Unmap First Child menu ?</span>",
                                                buttons: {
                                                    cancel: {
                                                        label: '<i class="fa fa-times"></i> No',
                                                        className: 'pull-right margin-left-NoBtn'
                                                    },
                                                    confirm: {
                                                        label: '<i class="fa fa-check"></i> Yes'
                                                    }
                                                },
                                                callback: function (result) {


                                                    if (result) {
                                                        BlockUI();
                                                        $.ajax({
                                                            url: '/UserManagement/RoleDetails/UnmapFirstChildMenu/',
                                                            data: { "EncryptedID": $('#EncryptedID').val(), "firstChildMenuDetailId": $('#firstChildMenuDetailsList').val() },
                                                            datatype: "json",
                                                            type: "POST",
                                                            headers: header,
                                                            success: function (data) {
                                                                if (data.serverError == true) {
                                                                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                                                        function () {
                                                                            window.location.href = "/Home/HomePage"
                                                                        });
                                                                }
                                                                else {
                                                                    $('#firstChildMenuDetailsMapButtonId').html('');
                                                                    //alert(data.MapUnmapButtonForFirstChild);
                                                                    $('#firstChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForFirstChild);
                                                                    firstChildMenuDetailsList();
                                                                }
                                                                $.unblockUI();
                                                            },
                                                            error: function (xhr) {
                                                                //alert('error');
                                                                bootbox.alert({
                                                                    //size: 'small',
                                                                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                                                                });
                                                                $.unblockUI();
                                                            }
                                                        });

                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                            },
                            onEscape: function () {
                                dialog.modal("hide");
                            }
                        });
                    }
                }
                $.unblockUI();
            },
            error: function (xhr) {
                //alert('error');
                bootbox.alert({
                    //size: 'small',
                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                });
                $.unblockUI();
            }
        });
    }
}



// UnmapFirstChildMenu
//function UnmapFirstChildMenu(EncryptedID) {
//    // alert("UnmapFirstChildMenu");
//    if ($('#firstChildMenuDetailsList').val() == 0) {
//        bootbox.alert("Please Select First Child Menu");
//    }
//    else {
//        //$.ajax({
//        //    url: '/UserManagement/RoleDetails/UnmapFirstChildMenu/',
//        //    data: { "EncryptedID": $('#EncryptedID').val(), "firstChildMenuDetailId": $('#firstChildMenuDetailsList').val() },
//        //    datatype: "json",
//        //    type: "POST",
//        //    success: function (data) {
//        //        $('#firstChildMenuDetailsMapButtonId').html('');
//        //        //alert(data.MapUnmapButtonForFirstChild);
//        //        $('#firstChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForFirstChild);
//        //    }
//        //});
//        var bootboxConfirm = bootbox.confirm({
//            title: "Confirm",
//            message: "Do you want to Unmap First Child menu ?",
//            buttons: {
//                cancel: {
//                    label: '<i class="fa fa-times"></i> No',
//                    className: 'pull-right margin-left-NoBtn'
//                },
//                confirm: {
//                    label: '<i class="fa fa-check"></i> Yes'
//                }
//            },
//            callback: function (result) {
//                if (result) {
//                    BlockUI();
//                    $.ajax({
//                        url: '/UserManagement/RoleDetails/UnmapFirstChildMenu/',
//                        data: { "EncryptedID": $('#EncryptedID').val(), "firstChildMenuDetailId": $('#firstChildMenuDetailsList').val() },
//                        datatype: "json",
//                        type: "POST",
//                        success: function (data) {
//                            $('#firstChildMenuDetailsMapButtonId').html('');
//                            //alert(data.MapUnmapButtonForFirstChild);
//                            $('#firstChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForFirstChild);
//                            firstChildMenuDetailsList();
//                            $.unblockUI();
//                        },
//                        error: function (xhr) {
//                            alert('error');
//                            $.unblockUI();
//                        }
//                    });
//                }
//            }
//        });
//    }
//}

// MapSecondChildMenu
function MapSecondChildMenu(EncryptedID) {
    //alert("MapSecondChildMenu");
    if ($('#secondChildMenuDetailsList').val() == 0) {
        bootbox.alert("Please Select Second Child Menu");
    }
    else {
        //$.ajax({
        //    url: '/UserManagement/RoleDetails/MapSecondChildMenu/',
        //    data: { "EncryptedID": $('#EncryptedID').val(), "secondChildMenuDetailId": $('#secondChildMenuDetailsList').val() },
        //    datatype: "json",
        //    type: "POST",
        //    success: function (data) {

        //        $('#secondChildMenuDetailsMapButtonId').html('');
        //        //alert(data.MapUnmapButtonForFirstChild);
        //        $('#secondChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForSecondChild);
        //    }
        //});
        var bootboxConfirm = bootbox.confirm({
            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
            message: "<span class='boot-alert-txt'>Do you want to Map Second child menu ?</span>",

            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> No',
                    className: 'pull-right margin-left-NoBtn'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Yes'
                }
            },
            callback: function (result) {
                if (result) {
                    BlockUI();
                    $.ajax({
                        url: '/UserManagement/RoleDetails/MapSecondChildMenu/',
                        data: { "EncryptedID": $('#EncryptedID').val(), "secondChildMenuDetailId": $('#secondChildMenuDetailsList').val() },
                        datatype: "json",
                        type: "POST",
                        headers: header,
                        success: function (data) {
                            if (data.serverError == true) {
                                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                    function () {
                                        window.location.href = "/Home/HomePage"
                                    });
                            }
                            else {
                                $('#secondChildMenuDetailsMapButtonId').html('');
                                //alert(data.MapUnmapButtonForFirstChild);
                                $('#secondChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForSecondChild);
                            }
                            $.unblockUI();
                        },
                        error: function (xhr) {
                            //alert('error');
                            bootbox.alert({
                                //size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            });
                            $.unblockUI();
                        }
                    });
                }
            }
        });
    }
}

// UnmapSecondChildMenu
function UnmapSecondChildMenu(EncryptedID) {
    // alert("UnmapParentMenu");
    if ($('#secondChildMenuDetailsList').val() == 0) {
        bootbox.alert("Please Select Second Child Menu");
    }
    else {
        //$.ajax({
        //    url: '/UserManagement/RoleDetails/UnmapSecondChildMenu/',
        //    data: { "EncryptedID": $('#EncryptedID').val(), "secondChildMenuDetailId": $('#secondChildMenuDetailsList').val() },
        //    datatype: "json",
        //    type: "POST",
        //    success: function (data) {

        //        $('#secondChildMenuDetailsMapButtonId').html('');
        //        //alert(data.MapUnmapButtonForFirstChild);
        //        $('#secondChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForSecondChild);
        //    }
        //});

        var bootboxConfirm = bootbox.confirm({
            title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
            message: "<span class='boot-alert-txt'>Do you want to Unmap Second child menu ?</span>",

            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> No',
                    className: 'pull-right margin-left-NoBtn'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Yes'
                }
            },
            callback: function (result) {
                if (result) {
                    BlockUI();
                    $.ajax({
                        url: '/UserManagement/RoleDetails/UnmapSecondChildMenu/',
                        data: { "EncryptedID": $('#EncryptedID').val(), "secondChildMenuDetailId": $('#secondChildMenuDetailsList').val() },
                        datatype: "json",
                        type: "POST",
                        headers: header,
                        success: function (data) {
                            if (data.serverError == true) {
                                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                    function () {
                                        window.location.href = "/Home/HomePage"
                                    });
                            }
                            else {
                                $('#secondChildMenuDetailsMapButtonId').html('');
                                //alert(data.MapUnmapButtonForFirstChild);
                                $('#secondChildMenuDetailsMapButtonId').html(data.MapUnmapButtonForSecondChild);
                            }
                            $.unblockUI();
                        },
                        error: function (xhr) {
                            //alert('error');
                            bootbox.alert({
                                //size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            });
                            $.unblockUI();
                        }
                    });
                }
            }
        });
    }
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