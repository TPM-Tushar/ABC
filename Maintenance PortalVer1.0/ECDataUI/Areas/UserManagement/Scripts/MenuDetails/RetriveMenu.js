//alert("in retriveMenu.js");
$(document).ready(function () {
    //alert("In Ready");
    retriveMenusFromDB();
   
    
    $("#panelMenuDetailsForm").hide();
    $('#menuDetailsListCollapse').trigger('click');

    $('#menuDetailsListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    //$("#ShowAddMenuFormID").click(function () {
    //    $("#panelMenuDetailsForm").show();
    //    createMenuFormMethod();
    //    CollapseMenuDetailsList();
    //    //$("#ShowAddMenuFormID").hide();
    //});

    $("#buttonToOpenAddMenuForm").click(function () {
        $("#panelMenuDetailsForm").show();
        createMenuFormMethod();
        CollapseMenuDetailsList();
        document.documentElement.scrollTop = 90;
        //$("#ShowAddMenuFormID").hide();
    });

    //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    //    $($.fn.dataTable.tables(true)).DataTable()
    //       .columns.adjust();
    //});
});

function CollapseMenuDetailsList() {
    $('#menuDetailsListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-plus-square-o fa-pull-left fa-2x');
}

function OpenMenuDetailsList() {
    $('#menuDetailsListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-minus-square-o fa-pull-left fa-2x');
}

//Retrive Menus
function retriveMenusFromDB() {
    $("#editMenuHeadingId").hide();
    $("#menuDetailsListTable").DataTable().destroy();

    var t=$("#menuDetailsListTable").DataTable({
        //"bJQueryUI": true,
        //"bServerSide": true,
        "responsive": true,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        language: { search: "Search" },
        //Commented by Shubham Bhagat on 29-10-2018
        //"scrollX": true,
        "scrollY":"30vh",
        scrollCollapse: true,
        //fixedHeader: false,
        ScrollXInner: "100%",
        //"order": [[ 1, 'asc' ]],
        //"deferRender": true,
        ajax: {
            url: "/UserManagement/MenuDetails/LoadData",
            type: "POST",
            beforeSend: function () {
                //Added by Akash(24-08-2018) to validate search string.
                var searchString = $('#menuDetailsListTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#menuDetailsListTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                        //    $('#menuDetailsListTable_filter input').val('');

                            t.search('').draw();
                            $("#menuDetailsListTable_filter input").prop("disabled", false);
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
                $.unblockUI();
                return json.data;
            }
        },

       
        columnDefs: [
                    { orderable: false, targets: [0] },
                   { orderable: false, targets: [6] },
                   { orderable: false, targets: [8] },
                   { orderable: false, targets: [9] },
                   { orderable: false, targets: [10] }
                   //,{ orderable: false, targets: [11] }
                   //,{ orderable: false, targets: [13] }
        ],
        //fnInitComplete: function (oSettings, json) {
        //    alert('in fnInitComplete');

           
        //},

        "columns": [
              //{ "data": "MenuID", "name": "MenuID", "autoWidth": true },
              // For Showing Sr No
               { "data": "MenuName", "name": "MenuName", "autoWidth": true },
              { "data": "MenuName", "name": "MenuName", "autoWidth": true },
              //{ "data": "MenuNameR", "name": "MenuNameR", "autoWidth": true },
              { "data": "ParentMenu", "name": "ParentMenu", "autoWidth": true },
              { "data": "Sequence", "name": "Sequence", "autoWidth": true },
              { "data": "VerticalLevel", "name": "VerticalLevel", "autoWidth": true },
              { "data": "HorizontalSequence", "name": "HorizontalSequence", "autoWidth": true },
              //{ "data": "IsActive", "name": "isactive", "autowidth": true },
              {
                  "data": "IsActive", render: function (data, type, row) {
                      return (data == true) ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";

                  }
              },
              //{ "data": "IsActiveString", "name": "IsActiveString", "autoWidth": true },
              { "data": "LevelGroupCode", "name": "LevelGroupCode", "autoWidth": true },
              //{ "data": "IsMenuIDParameter", "name": "IsMenuIDParameter", "autoWidth": true },
              {
                  "data": "IsMenuIDParameter", render: function (data, type, row) {
                      return (data == true) ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";

                  }
              },
              //{ "data": "IsHorizontalMenu", "name": "IsHorizontalMenu", "autoWidth": true },
              {
                  "data": "IsHorizontalMenu", render: function (data,type,row) {
                      return (data == true) ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";

                  }
              },
                { "data": "MenuActionMappingButton", "name": "MenuActionMappingButton", "autoWidth": true }
                ,
              {
                  "render": function (data, type, full, meta)
                   { return "<a href='#' onclick=editMenuFormMethod('" + full.EncryptedID + "');><i class='fa fa-pencil  fa-2x' aria-hidden='true' style='color:black;' ></i></a>"; }
            },

              { "data": "ActionAssigned", "name": "ActionAssigned", "autoWidth": true }

               //,
               //{
               //    "render": function (data, type, row)
               //    { return "<a href='#' onclick=deleteMenuFormMethod('" + row.EncryptedID + "'); ><i class='fa fa-trash fa-2x' aria-hidden='true' style='color:black;'></i></a>"; }
               //}

        ]
    });
    //table.on('order.dt search.dt', function () {
    //    table.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
    //        cell.innerHTML = i + 1+ PageInfo.start;;
    //    });
    //}).draw();
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit').css({ "width": "100%" });;//.css('width', 'none !important').css('width', '100% !important');

    //$('.dataTables_scrollHeadInner').css('width', '100% !important');
    //$('.dataTables_scrollHeadInner').addClass("xyz");
    t.on('draw.dt', function () {
        var PageInfo = $('#menuDetailsListTable').DataTable().page.info();
        t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + PageInfo.start;
        });
    });
    //$('#menuDetailsListTable_info').addClass('col-md-4');
    //$('#menuDetailsListTable_info').css('margin-left', '-80%');
    //$('#menuDetailsListTable_info').addClass('style=margin-left:0px;');

    //$(".dataTables_scrollHeadInner").css({ "width": "100%" });

    //$(".table ").css({ "width": "100%" });

    $('#menuDetailsListTable_info').css('margin-left', '-2%');
    $('#menuDetailsListTable_info').css('font-size', '120%');

    
    
}
//Menu Add Function

function createMenuFormMethod() {
    //alert("createMenuFormMethod");
    BlockUI();
    $.ajax({
        url: '/UserManagement/MenuDetails/AddMenu',
        datatype: "text",
        type: "GET",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        success: function (data) {
            //bootbox.alert(data);

            $("#addEditDivId").show();
            //$("#createPartialViewId").show();

            $("#ShowAddMenuFormID").hide();
            $("#addMenuHeadingId").show();
            $("#editMenuHeadingId").hide();
            $("#menuActionMappingHeadingId").hide();
            $("#addEditDivId").html(data);
            //$("#createPartialViewId").html(data);
            $.unblockUI();
        },
        error: function (xhr) {
            //bootbox.alert('error');
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            $.unblockUI();
        }
    });
  
    //$("#editPartialViewId").hide();

}

//Menu Edit Function
function editMenuFormMethod(EncryptedID) {
    //alert("edit");
    //error
    $("#panelMenuDetailsForm").show();
    CollapseMenuDetailsList();
    
    $("#addEditDivId").show();
    //$("#editPartialViewId").show();
    $("#ShowAddMenuFormID").hide();
    $("#addMenuHeadingId").hide();
    $("#editMenuHeadingId").show();
    $("#menuActionMappingHeadingId").hide();
    document.documentElement.scrollTop = 90;
    BlockUI();
    $.ajax({
        url: '/UserManagement/MenuDetails/EditMenu/',
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
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            $.unblockUI();
        }
    });
    //$("#createPartialViewId").hide();
}

//Menu Delete Function
function deleteMenuFormMethod(EncryptedID) {
    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Do you want to delete Menu Details?</span>",

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
                    url: '/UserManagement/MenuDetails/DeleteMenu/',
                    data: { "EncryptedID": EncryptedID },
                    datatype: "json",
                    type: "POST",

                    success: function (data) {
                        //alert(data);
                        //  alert(data.menuDetailsResponseModel.Message);
                        //if (data.menuDetailsResponseModel.Result) {
                        //bootbox.alert(data.menuDetailsResponseModel.Message);
                        bootbox.alert({
                            //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.menuDetailsResponseModel.Message + '</span>',
                            callback: function () {
                            }
                        });
                        retriveMenusFromDB();
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
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            callback: function () {
                            }
                        });
                        $.unblockUI();
                    }
                });
            }
        }
    });
    //-------------------------------
    //$.confirm({
    //    title: 'Do you want to delete?',
    //    content: '',
    //    buttons: {
    //        confirm: function () {
    //            $.ajax({
    //                url: '/UserManagement/MenuDetails/DeleteMenu/',
    //                data: { "EncryptedID": EncryptedID },
    //                datatype: "text",
    //                type: "POST",
    //                contenttype: 'application/json; charset=utf-8',
    //                async: true,
    //                success: function (data) {
    //                    if (data.success) {
    //                        bootbox.alert("Data Deleted");
    //                        retriveMenusFromDB();
    //                    }
    //                    else {
    //                        bootbox.alert("Error in deleting");
    //                    }
    //                    // if(data==true)
    //                    // $("#createPartialViewId").html(data);
    //                },
    //                error: function (xhr) {
    //                    alert('error');
    //                }
    //            });
    //            // $.alert('Confirmed!');
    //        },
    //        cancel: function () {
    //            bootbox.alert('Canceled!');
    //        }//,
    //        //somethingElse: {
    //        //    text: 'Something else',
    //        //    btnClass: 'btn-blue',
    //        //    keys: ['enter', 'shift'],
    //        //    action: function () {
    //        //        $.alert('Something else?');
    //        //    }
    //        //}
    //    }
    //});

    //  bootboxConfirm
   // $("#createPartialViewId").hide();
   //$("#editPartialViewId").hide();

    $("addEditDivId").hide();
}

//RoleMenuMapping
//RoleMenuMapping Function to show Menu List To map to role
function MenuActionMapping(EncryptedID) {

    $("#panelMenuDetailsForm").show();
    CollapseMenuDetailsList();

    $("#addEditDivId").show();
    //$("#editPartialViewId").show();
    $("#ShowAddMenuFormID").hide();
    $("#addMenuHeadingId").hide();
    $("#editMenuHeadingId").hide();
    $("#menuActionMappingHeadingId").show();

    BlockUI();
    $.ajax({
        url: '/UserManagement/MenuDetails/MenuActionMapping/',
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
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
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