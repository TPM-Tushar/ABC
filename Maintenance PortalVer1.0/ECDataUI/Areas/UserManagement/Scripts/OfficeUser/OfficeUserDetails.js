
//Global variables.
var token = '';
var header = {};
$(document).ready(function () {


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $("ul#crumbs li:nth-child(2)").trigger('click');
   
    GetJsonData();

    

    $('#OfficeUserListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    $('#OfficeUserListCollapse').trigger('click');


    $("#btnOfficeUserDetails").click(function () {
        BlockUI();
        $.ajax({
            type: "Get",
            url: "/UserManagement/OfficeUserDetails/OfficeUserRegistration",
            success: function (data) {
             
                CollapseOfficeList();
                $("#panelOfficeUser").hide();
                $('#DivOfficeUserDetailsWrapper').html(data);
                $('#DivOfficeUserDetailsWrapper').fadeIn(500);
                $("#firstNameID").focus();
                document.documentElement.scrollTop = 80;
                $.unblockUI();

            },
            error: function () {
                bootbox.alert({
                    //   size: 'small',
                    //   title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Error in updating details... </span>',
                });
                $.unblockUI();
               
            }
        });

      
    });


});


// To arrange Table header with Table body when left toggle button is clicked
$('#togglebtnId').click(function () {
    setTimeout(function () { $('thead').children().children().eq(1).click(); $('thead').children().children().eq(1).click(); }, 275);
});

//--------Getting Data for DataTable----------------

function GetJsonData() {

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/UserManagement/OfficeUserDetails/LoadOfficeUserDetailsGridData",
        //headers: header,

        success: function (jsonData) {
            if (jsonData.errorMessage != undefined) {
                $.unblockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + jsonData.errorMessage + '</span>',
                    callback: function () {
                        window.location.href = "/Home/HomePage"
                    }
                });
            }
            else {
                FillDataTable(jsonData);
            }
            $.unblockUI();

        },
        error: function (xhr, status, err) {
            bootbox.alert({
                //   size: 'small',
                //   title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>',

            });
            $.unblockUI();

        }
    });

}

function UpdateOfficeUserDetailsData(EncryptedId) {
    BlockUI();
    $.ajax({
        type: "Get",
        url: "/UserManagement/OfficeUserDetails/UpdateOfficeUser/",
        data: { "EncryptedId": EncryptedId },
        success: function (data) {
            $.unblockUI();
            CollapseOfficeList();
            $("#panelOfficeUser").hide();
            $('#DivOfficeUserDetailsWrapper').html(data);
            $('#DivOfficeUserDetailsWrapper').fadeIn(500);
            $("#firstNameID").focus();
            document.documentElement.scrollTop = 80;


        },
        error: function () {
            $.unblockUI();
            bootbox.alert({
                //   size: 'small',
                //   title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in Updating</span>',

            });
        }
    })
}
function DeleteOfficeUserDetailsData(EncryptedId) {
    bootbox.confirm({
        title: "Confirm",
        message: "Are you sure you want to delete?",
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
                    type: "Post",
                    url: "/UserManagement/OfficeUserDetails/DeleteOfficeUser/",
                    data: { "EncryptedId": EncryptedId },
                    success: function (data) {
                        if (data.success) {
                            bootbox.alert({
                                //title: "<span class=' boot-alert-title'><i class='fa fa-check text-success boot-icon boot-icon'></i>&nbsp;&nbsp;Success</span>",
                                message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                callback: function () {
                                    GetJsonData();
                                }
                            });
                            $.unblockUI();
                        }
                        else {
                            bootbox.alert({
                                //   size: 'small',
                                //   title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in Deleting</span>',

                            });
                            $.unblockUI();
                        }
                    },
                    error: function () {
                        bootbox.alert({
                            //   size: 'small',
                            //   title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>',

                        });
                        $.unblockUI();

                    }
                });
            }
        }
    });
}
function CollapseOfficeList() {
    $('#OfficeUserListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-plus-square-o fa-pull-left fa-2x');
}
function OpenOfficeList() {
    $('#OfficeUserListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-minus-square-o fa-pull-left fa-2x');
}

function FillDataTable(jsonData) {
    if ($.fn.DataTable.isDataTable("#OfficeUserGrid")) {
        $('#OfficeUserGrid').remove();
        $('.clsTblWrapper').html('<table id="OfficeUserGrid" class="display table table-responsive table-striped table-bordered" cellspacing="0" width="100%"></table>');
    }
    var table = $('#OfficeUserGrid').DataTable({
       
        destroy: true,
        paging: true,
        //Commented by Shubham Bhagat on 29-10-2018
        //scrollX: true,
        pageLength: 10,
        searching: true,
        responsive: true,
        "scrollY": "30vh",
        scrollCollapse: true,
        language: { search: "Search" },
        ordering: true,//sorting..       
        columnDefs: [{ orderable: false, targets: [0] }, { orderable: false, targets: [9] }, { orderable: false, targets: [10] }],
        order: [[1, 'asc']],
        data: jsonData.data,
        columns: jsonData.columns,
        fnInitComplete: function (oSettings, json) {
            unBlockUI();
        }
    });

    table.on('order.dt search.dt', function () {
        table.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
    //$("#OfficeUserGrid_info").css('margin-left', "-80%");
    $('.dataTables_scrollHeadInner').css('box-sizing', 'border-box');
    $('#OfficeUserGrid_info').css('margin-left', '-2%');
    $('#OfficeUserGrid_info').css('font-size', '120%');



}
//----------Block UI-------------------------
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




