

$(document).ready(function () {
    $("ul#crumbs li:nth-child(3)").trigger('click');

    $('#ControllerActionListCollapse').trigger('click');

    $('#ControllerActionListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });




    $("#btnAddControllerActionDetails").click(function () {


        CollapseControllerActionList();
        BlockUI();
        $.ajax({
            type: "Get",
            url: "/UserManagement/ActionDetails/InsertControllerActionData",
            success: function (data) {
                CollapseControllerActionList();
                $("#panelNewControllerAction").fadeOut(500);
                $("#panelDistrictForm").fadeIn(500);
                $("#DivControllerActionWrapper").fadeIn(500);
                $('#DivControllerActionWrapper').html(data);
                $('#controllerList').prop('disabled', true);
                $('#actionList').prop('disabled', true);
                $("#controllerList").empty();
                $("#actionList").empty();
                document.documentElement.scrollTop = 0;
                $.unblockUI();

            },
            error: function () {
                bootbox.alert({
                    //   size: 'small',
                    // tsle: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Error in Retriving</span>',
                });
                $.unblockUI();
            }
        });
    });
    $.unblockUI();

    // LoadControllerActionDetailsData();

    $('#btnFilterContActDetails').click(function () {
        LoadControllerActionDetailsData();
        //  OpenControllerActionList();
    });


    $("#btncloseRoleAuthPopup").click(function () {
        $('#divViewRoleModal').modal('hide');

    });


});


function GetRoleAuthView(sEncryptedID) {
    $('#divLoadRoleMappingView').load('/UserManagement/ActionDetails/GetRoleAuthView',
        { EncryptedID: sEncryptedID }, function () {

            $('#divViewRoleModal').modal('show');

        }

    );
}



function CollapseControllerActionList() {
    $('#ControllerActionListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-plus-square-o fa-pull-left fa-2x');
}
function OpenControllerActionList() {
    $('#ControllerActionListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-minus-square-o fa-pull-left fa-2x');
}

function LoadControllerActionDetailsData() {
    //alert("LoadControllerActionDetailsData");

    BlockUI();
    var isTechAdmin = 'false';
    var filterMenuId = $("#filterMenuDetailsList option:selected").val();
     
    $("#ControllerActionDetailsGrid").DataTable().destroy();
    var t = $("#ControllerActionDetailsGrid").DataTable({
        destroy: true,
        "responsive": true,
        "processing": false,//true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        "scrollX": true,
        "scrollY": "30vh",
        language: { search: "Search" },
        ajax: {
            url: "/UserManagement/ActionDetails/LoadControllerActionData",
            type: "POST",
            data: { "filterMenuId": filterMenuId },
            beforeSend: function () {
                //alert("BEFORE SEND");
                var searchString = $('#ControllerActionDetailsGrid_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#ControllerActionDetailsGrid_filter input").prop("disabled", true);

                        bootbox.alert('Please enter valid Search String ', function () {
                            //   $('#ControllerActionDetailsGrid_filter input').val('');
                            t.search('').draw();
                            $("#ControllerActionDetailsGrid_filter input").prop("disabled", false);
                        });
                        return false;
                    }
                }
            },
            datatype: "json",
            dataSrc: function (json) {
                //alert("json.isTechAdmin::" + json.isTechAdmin);
                isTechAdmin = json.isTechAdmin == 1 ? 'true' : 'false';

                $.unblockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError == true)
                                window.location.href = "/Home/HomePage"
                            else {
                                $("#ControllerActionDetailsGrid").DataTable().destroy();
                                return;

                            }
                        }

                    });
                }
                else {
                    $.unblockUI();
                    return json.data;
                }
                //$.unblockUI();
                //return json.data;
            }
        },

        columnDefs: [
            //{ orderable: false, targets: [0] }, { orderable: false, targets: [4] }
            //, { orderable: false, targets: [5] }

        ],
        "columns": [
            //{ "data": "SrNo", "name": "SrNo", "title": "Sr No", "autoWidth": true },
            //{ "data": "AreaNameId", "name": "AreaNameId", "title": "Area Name", "autoWidth": true, "bVisible": isTechAdmin  },

            //{ "data": "ControllerNameId", "name": "ControllerNameId", "autoWidth": true, "bVisible": isTechAdmin },
            //{ "data": "ActionNameId", "name": "ActionNameId", "autoWidth": true, "bVisible": isTechAdmin },
            //{ "data": "Description", "name": "Description", "autoWidth": true },
            //{ "data": "ForMenu", "name": "ForMenu", "autoWidth": true },
            //{ "data": "AssignedToRoles", "name": "AssignedToRoles", "autoWidth": true },
            //{ "data": "IsActiveStr", "name": "IsActiveStr", "autoWidth": true },
            //{ "data": "Edit", "name": "Edit", "autoWidth": true, "bVisible": isTechAdmin },
            //{ "data": "Delete", "name": "Delete", "autoWidth": true, "bVisible": isTechAdmin }


            { "data": "SrNo", "name": "SrNo", "title": "Sr No", "autoWidth": true },
            { "data": "AreaNameId", "name": "AreaNameId", "title": "Area Name", "autoWidth": true },

            { "data": "ControllerNameId", "name": "ControllerNameId", "autoWidth": true },
            { "data": "ActionNameId", "name": "ActionNameId", "autoWidth": true },
            { "data": "Description", "name": "Description", "autoWidth": true },
            { "data": "ForMenu", "name": "ForMenu", "autoWidth": true },
            { "data": "AssignedToRoles", "name": "AssignedToRoles", "autoWidth": true },
            { "data": "IsActiveStr", "name": "IsActiveStr", "autoWidth": true },
            { "data": "Edit", "name": "Edit", "autoWidth": true }
            //,{ "data": "Delete", "name": "Delete", "autoWidth": true }



            //{
            //    //"data": "ActionName", "name": "ActionName", "autoWidth": true ,
            //    "render": function (data, type, full, meta) {
            //        if (!full.IsActive) {
            //            return "<i class='fa fa-close  ' style='color:black'></i>";
            //        } else {
            //            return "<i class='fa fa-check  ' style='color:black'></i>";
            //        }
            //    }
            //},
            //{

            //    "render":

            //        function (data, type, full, meta) { return "<a href='#'  onclick=UpdateControllerActionData('" + full.EncryptedID + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>"; }
            //},
            //{
            //    data: null,
            //    "render": function (data, type, row) {
            //        return "<a href='#'  onclick=DeleteControllerActionData('" + row.EncryptedID + "'); ><i class='fa fa-trash fa-2x ' style='color:black'></i></a>";
            //    }
            //}

        ],


        fnInitComplete: function (oSettings, json) {

            //alert(isTechAdmin);
            if (isTechAdmin != 'true') {
                t.columns([1]).visible(false);
                t.columns([2]).visible(false);
                t.columns([3]).visible(false);
                t.columns([8]).visible(false);
              //  t.columns([9]).visible(false);

            }

        }
    });
    t.on('draw.dt', function () {
        //var PageInfo = $('#ControllerActionDetailsGrid').DataTable().page.info();
        //t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
        //    cell.innerHTML = i + 1 + PageInfo.start;
        //});
    });



    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');

    //  $('#ControllerActionDetailsGrid_info').addClass('col-md-5');
    //$("#ControllerActionDetailsGrid_info").css('margin-left', "-76%");
    $('#ControllerActionDetailsGrid_info').css('margin-left', '-2%');
    $('#ControllerActionDetailsGrid_info').css('font-size', '120%');

    $.unblockUI();

}
function UpdateControllerActionData(EncryptedID) {

    // RAFE CollapseControllerActionList();
    $("#panelNewControllerAction").fadeOut(500);
    $("#panelDistrictForm").fadeIn(500);
    $("#DivControllerActionWrapper").fadeIn(500);
    document.documentElement.scrollTop = 0;
    BlockUI();
    $.ajax({
        type: "Get",
        url: "/UserManagement/ActionDetails/UpdateControllerActionData/",
        data: { "EncryptedID": EncryptedID },
        success: function (data) {

            $('#DivControllerActionWrapper').html(data);
            $('#controllerList').prop('disabled', false);
            $('#actionList').prop('disabled', false);

            $.unblockUI();
        },
        error: function () {
            bootbox.alert({
                //   size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Error! Please try again!</span>',
            });
            $.unblockUI();

        }
    });
}

function DeleteControllerActionData(EncryptedID) {
    bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Are you sure you want to delete?</span>",
        buttons: {
            cancel: {
                label: '<i class="fa fa-times"></i> No ',
                className: 'pull-right margin-left-NoBtn'
            },
            confirm: {
                label: '<i class="fa fa-check"></i> Yes',

            }
        },
        callback: function (result) {

            if (result) {
                BlockUI();
                $.ajax({

                    type: "Post",
                    url: "/UserManagement/ActionDetails/DeleteControllerActionData/",
                    data: { "EncryptedID": EncryptedID },
                    success: function (data) {
                        if (data.success) {
                            bootbox.alert({
                                //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + 'Data Deleted Successfully!' + '</span>',
                                callback: function (result) {
                                    LoadControllerActionDetailsData();
                                }
                            });
                            $.unblockUI();
                        }
                        else {
                            bootbox.alert({
                                //   size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Error in deleting... </span>',
                            });
                            $.unblockUI();
                        }
                    },
                    error: function () {
                        bootbox.alert({
                            //   size: 'small',
                            //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> error in processing... </span>',
                        });
                        $.unblockUI();

                    }
                });
            }
        }
    });
}
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

