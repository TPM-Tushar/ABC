
$(document).ready(function () {
    $("ul#crumbs li:nth-child(1)").trigger('click');

    GetJsonData();



    $('#officeListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    $('#officeListCollapse').trigger('click');


    $("#btnAddOfficeDetails").click(function () {
        BlockUI();
        $.ajax({
            type: "Get",
            url: "/UserManagement/OfficeDetails/CreateNewOffice",
            success: function (data) {
                CollapseOfficeList();
                $("#panelNewOffice").hide();
                $('#DivOfficeDetailsWrapper').html(data);
                $('#DivOfficeDetailsWrapper').fadeIn(500);
                $("#OfficeNametxt").focus();
                document.documentElement.scrollTop = 0;
                $.unblockUI();
            },
            error: function () {
                bootbox.alert({
                    //   size: 'small',
                    //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>',
                });
            }
        });

        //CollapseOfficeList();
        //$("#panelNewOffice").hide();
        //$('#DivOfficeDetailsWrapper').load('/UserManagement/OfficeDetails/CreateNewOffice');
        //$('#DivOfficeDetailsWrapper').fadeIn(500);
        //$("#OfficeNametxt").focus();
    });




});
//function FillCreateNewOfficeDiv() {
//    $.ajax({
//        type: "Get",
//        url: "/UserManagement/OfficeDetails/CreateNewOffice",
//        success: function (data) {
//            $("#btnCreateNewOffice").fadeOut(300);
//            $('#CreateUpdateOfficeFormDiv').html(data)
//            $("#CreateUpdateOfficeFormDiv").fadeIn(500);
//            $("#OfficeNametxt").focus();
//            },
//        error: function () {
//            bootbox.alert("error");
//        }
//    })

//}

// To arrange Table header with Table body when left toggle button is clicked Added By Shubham Bhagat
$('#togglebtnId').click(function () {
    setTimeout(function () { $('thead').children().children().eq(1).click(); $('thead').children().children().eq(1).click(); }, 275);
});
//--------Getting Data for DataTable----------------

function GetJsonData() {

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/UserManagement/OfficeDetails/LoadOfficeDetailsGridData",
        success: function (jsonData) {

            FillDataTable(jsonData);
            $.unblockUI();

        },
        error: function (xhr, status, err) {
            bootbox.alert({
                //   size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>',

            });
            $.unblockUI();

        }
    });

}

function UpdateOfficeDetailsData(EncryptedId) {
    BlockUI();
    $.ajax({
        type: "Get",
        url: "/UserManagement/OfficeDetails/UpdateOffice/",
        data: { "EncryptedID": EncryptedId },
        success: function (data) {




            CollapseOfficeList();
            $("#panelNewOffice").hide();
            $('#DivOfficeDetailsWrapper').html(data);

            $('#DivOfficeDetailsWrapper').fadeIn(500);
            $("#OfficeNametxt").focus();
            $.unblockUI();

        },
        error: function () {
            bootbox.alert({
                //   size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in Updating</span>',

            });
            $.unblockUI();
        }
    })

}
function DeleteOfficeDetailsData(EncryptedId) {
    bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Are you sure you want to delete?</span>",
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
                    url: "/UserManagement/OfficeDetails/DeleteOffice/",
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
    $('#officeListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-plus-square-o fa-pull-left fa-2x');
}
function OpenOfficeList() {
    $('#officeListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-minus-square-o fa-pull-left fa-2x');
}

function FillDataTable(jsonData) {
    if ($.fn.DataTable.isDataTable("#OfficeDetailsGrid")) {
        $('#OfficeDetailsGrid').remove();
        $('.clsTblWrapper').html('<table id="OfficeDetailsGrid" class="display table table-responsive table-striped table-bordered" cellspacing="0" width="100%"></table>');
    }
    var table = $('#OfficeDetailsGrid').DataTable({

        destroy: true,
        paging: true,
        pageLength: 10,
        searching: true,
        responsive: true,
        "scrollY": "30vh",
        scrollCollapse: true,
        language: { search: "Search" },
        ordering: true,//sorting..   
        //Commented by Shubham Bhagat on 29-10-2018
        //"scrollX": true,
        columnDefs: [{ orderable: false, targets: [0] }, { orderable: false, targets: [7] }],
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
    //$("#OfficeDetailsGrid_info").css('margin-left', "-80%");
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');

    $('#OfficeDetailsGrid_info').css('margin-left', '-2%');
    $('#OfficeDetailsGrid_info').css('font-size', '120%');



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


