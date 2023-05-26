$(document).ready(function () {
    $("ul#crumbs li:nth-child(1)").trigger('click');

    retriveRoleDetailsList();


    $("#panelRoleDetailsForm").hide();
    $('#roleDetailsListCollapse').trigger('click');

    $('#roleDetailsListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    //$("#ShowAddEditRoleDetailsFormID").click(function () {
    //    $("#panelRoleDetailsForm").show();
    //    showAddRoleDetailForm();
    //    //createMenuFormMethod();
    //    CollapseRoleDetailsList();
    //    //$("#ShowAddMenuFormID").hide();
    //});

    $("#buttonToOpenAddRoleForm").click(function () {
        $("#panelRoleDetailsForm").show();
        showAddRoleDetailForm();
        //createMenuFormMethod();
        CollapseRoleDetailsList();
        //$("#ShowAddMenuFormID").hide();
    });

    //debugger;
    //$('div.dataTables_scrollHeadInner').attr('style', 'background-color: aliceblue; overflow: hidden; text-align: center; width: 100%; margin-left: 0px;');
    //$('table.datatable').attr('style', "background-color: aliceblue; overflow: hidden; text-align: center; width: 100%; margin-left: 0px;");
    //$('#roleDetailsListTable').css('width', '100% !important');
    //$('body').on('load', function () {
    //    alert('here');
    //    fun();
    //});


});


function CollapseRoleDetailsList() {
    $('#roleDetailsListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-plus-square-o fa-pull-left fa-2x');
}

function OpenRoleDetailsList() {
    $('#roleDetailsListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-minus-square-o fa-pull-left fa-2x');
}


function retriveRoleDetailsList() {
    $("#editRoleDetailsHeadingId").hide();
    $("#roleDetailsListTable").DataTable().destroy();

    var isTechAdmin = 'false';

    var t = $("#roleDetailsListTable").DataTable({
        //"bJQueryUI": true,
        //"bServerSide": true,
        "responsive": true,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        language: { search: "Search" },

        //"scrollCollapse": true,
        "scrollX": true,
        "scrollY": "30vh",
        scrollCollapse: true,
        //"scrollY": true,

        //"ordering": false,
        //autoWidth:true,
        //"deferRender": true,
        ajax: {
            url: "/UserManagement/RoleDetails/LoadRoleDetailsList",
            type: "POST",
            beforeSend: function () {
                //Added by Akash(24-08-2018) to validate search string.
                var searchString = $('#roleDetailsListTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#roleDetailsListTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            //     $('#roleDetailsListTable_filter input').val('');
                            t.search('').draw();
                            $("#roleDetailsListTable_filter input").prop("disabled", false);
                            //                            $('#roleDetailsListTable_filter input').attr("value", "");
                        });
                        return false;
                    }
                }
            },
            datatype: "json",
            dataSrc: function (json) {
                $.unblockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            window.location.href = "/Home/HomePage"
                        }
                    });
                }
                isTechAdmin = json.isTechAdmin == 1 ? 'true' : 'false';

                $.unblockUI();
                return json.data;
            }
        },
        columnDefs: [

            //    { orderable: false, targets: [0] },
            //{ orderable: false, targets: [2] },
            //{ orderable: false, targets: [3] },
            //{ orderable: false, targets: [4] }
            //    //,
            //{ orderable: false, targets: [6] }
        ],

        "columns": [
            // For Showing Sr No
            { "data": "RoleName", "name": "RoleName", "autoWidth": true },
            { "data": "RoleName", "name": "RoleName", "autoWidth": true },
            //{ "data": "RoleNameR", "name": "RoleNameR", "autoWidth": true },
            //{ "data": "IsActive", "name": "isactive", "autowidth": true },
            {
                "data": "IsActive", render: function (data, type, row) {
                    return (data == true) ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";
                }
            },
            { "data": "MapMenuButton", "name": "MapMenuButton", "autoWidth": true },
            { "data": "EditRoleButton", "name": "EditRoleButton", "autoWidth": true },
            { "data": "AssignedMenus", "name": "AssignedMenus", "autoWidth": true },
             

        ],
        fnInitComplete: function (oSettings, json) {

            if (isTechAdmin != 'true') {
                t.columns([4]).visible(false);
              //  t.columns([5]).visible(false);


            }

        }
    });
    t.on('draw.dt', function () {
        var PageInfo = $('#roleDetailsListTable').DataTable().page.info();
        t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + PageInfo.start;
        });
    });



    //$('#roleDetailsListTable_info').addClass('col-md-4');
    $('#roleDetailsListTable_info').css('margin-left', '-2%');
    $('#roleDetailsListTable_info').css('font-size', '120%');
    //$('#roleDetailsListTable_paginate').css('margin', '-20px');
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
    //debugger;
    //table.responsive.recalc();
    //table.columns.adjust().draw();
}

function showAddRoleDetailForm() {
    //alert("createMenuFormMethod");
    BlockUI();
    $.ajax({
        url: '/UserManagement/RoleDetails/AddRoleDetails',
        datatype: "text",
        type: "GET",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        success: function (data) {
            //bootbox.alert(data);

            $("#addEditDivId").show();
            //$("#createPartialViewId").show();

            $("#ShowAddEditRoleDetailsFormID").hide();
            $("#addRoleDetailsHeadingId").show();
            $("#editRoleDetailsHeadingId").hide();
            $("#roleMenuMappingHeadingId").hide();

            $("#addEditDivId").html(data);
            $.unblockUI();
            //$("#createPartialViewId").html(data);
        },
        error: function (xhr) {
            //bootbox.alert('error');
            bootbox.alert({
                //size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
            });
            $.unblockUI();
        }
    });
    //$("#editPartialViewId").hide();

}

//Role Edit Function
function showEditRoleDetailForm(EncryptedID) {
    //alert("edit");
    //error
    $("#panelRoleDetailsForm").show();
    CollapseRoleDetailsList();

    $("#addEditDivId").show();
    //$("#editPartialViewId").show();
    $("#ShowAddEditRoleDetailsFormID").hide();
    $("#addRoleDetailsHeadingId").hide();
    $("#editRoleDetailsHeadingId").show();
    $("#roleMenuMappingHeadingId").hide();

    BlockUI();
    $.ajax({
        url: '/UserManagement/RoleDetails/EditRoleDetails/',
        data: { "EncryptedID": EncryptedID },
        datatype: "text",
        type: "GET",
        //datatype: "json",
        //type: "POST",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        success: function (data) {

            //$("#panelMenuDetailsForm").show();
            //CollapseMenuDetailsList();
            //$("#editPartialViewId").show();
            //$("#ShowAddMenuFormID").hide();
            //$("#addMenuHeadingId").hide();
            //$("#editMenuHeadingId").show();

            //if (data.menuDetailsModel!=null)
            //{
            $("#addEditDivId").html(data);
            $.unblockUI();
            //$("#editPartialViewId").html(data);
            //}
            //else {
            //    bootbox.alert(data);
            //}
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
    //$("#createPartialViewId").hide();
}

//Role Delete Function
function deleteRoleDetails(EncryptedID, IsActive) {
 
    var sText = (IsActive == "1" ? "DeActivate" : "Activate");
    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Do you want to " + sText + " Role?</span>",

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
                    url: '/UserManagement/RoleDetails/DeleteRoleDetails/',
                    data: { "EncryptedID": EncryptedID },
                    datatype: "json",
                    type: "POST",

                    success: function (data) {
                        //alert(data);
                        //  alert(data.menuDetailsResponseModel.Message);
                        //if (data.menuDetailsResponseModel.Result) {
                        //bootbox.alert(data.roleDetailsResponseModel.Message);
                        bootbox.alert({
                            //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.roleDetailsResponseModel.Message + '</span>',
                            callback: function () {
                            }
                        });
                        retriveRoleDetailsList();
                        $.unblockUI();
                        //}
                        //else {
                        //    bootbox.alert(data.menuDetailsResponseModel.Message);
                        //}
                        //// if(data==true)
                        // $("#createPartialViewId").html(data);
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

    $("addEditDivId").hide();
}

RoleMenuMapping
//RoleMenuMapping Function to show Menu List To map to role
function RoleMenuMapping(EncryptedID) {
    $("#panelRoleDetailsForm").show();
    CollapseRoleDetailsList();

    $("#addEditDivId").show();
    //$("#editPartialViewId").show();
    $("#ShowAddEditRoleDetailsFormID").hide();
    $("#addRoleDetailsHeadingId").hide();
    $("#editRoleDetailsHeadingId").hide();
    $("#roleMenuMappingHeadingId").show();

    BlockUI();
    $.ajax({
        url: '/UserManagement/RoleDetails/RoleMenuMapping/',
        data: { "EncryptedID": EncryptedID },
        datatype: "text",
        type: "GET",
        //datatype: "json",
        //type: "POST",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        success: function (data) {

            //$("#panelMenuDetailsForm").show();
            //CollapseMenuDetailsList();
            //$("#editPartialViewId").show();
            //$("#ShowAddMenuFormID").hide();
            //$("#addMenuHeadingId").hide();
            //$("#editMenuHeadingId").show();

            //if (data.menuDetailsModel!=null)
            //{
            $("#addEditDivId").html(data);
            $.unblockUI();
            //$("#editPartialViewId").html(data);
            //}
            //else {
            //    bootbox.alert(data);
            //}
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
    //$("#createPartialViewId").hide();
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




// To arrange Table header with Table body when left toggle button is clicked Added By Shubham Bhagat
$('#togglebtnId').click(function () {
    setTimeout(function () {
        $('thead').children().children().eq(1).click();
        $('thead').children().children().eq(1).click();
    }, 275);
});
