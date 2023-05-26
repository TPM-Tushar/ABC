
$(document).ready(function () {

    GetJsonData();

    $('#WorkFlowActionListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    $('#WorkFlowActionListCollapse').trigger('click');

    $("#btnAddWorkFlowActionDetails").click(function () {
        $.ajax({
            type: "Get",
            url: "/UserManagement/WorkFlowActionDetails/CreateUpdateNewWorkFlowAction",
            success: function (data) {
                CollapseOfficeList();
                $("#panelNewWorkFlowAction").hide();
                $('#DivWorkFlowActionDetailsWrapper').html(data);
                $('#DivWorkFlowActionDetailsWrapper').fadeIn(500);
                $("#Discriptiontxt").focus();
                document.documentElement.scrollTop = 0;

            },
            error: function () {
                //bootbox.alert("error in retriving");
                bootbox.alert({
                    //   size: 'small',
                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                });
            }
        });

        //CollapseOfficeList();
        //$("#panelNewWorkFlowAction").hide();
        //$('#DivWorkFlowActionDetailsWrapper').load('/UserManagement/OfficeDetails/CreateNewOffice');
        //$('#DivWorkFlowActionDetailsWrapper').fadeIn(500);
        //$("#OfficeNametxt").focus();
    });


});

//--------Getting Data for DataTable----------------

function GetJsonData() {

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/UserManagement/WorkFlowActionDetails/LoadWorkflowActionGridData",
        success: function (jsonData) {

            FillDataTable(jsonData);
            $.unblockUI();

        },
        error: function (xhr, status, err) {
            bootbox.alert({
                //   size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
            $.unblockUI();

        }
    });

}

function UpdateWorkflowActionData(EncryptedId) {
    $.ajax({
        type: "Get",
        url: "/UserManagement/WorkFlowActionDetails/UpdateWorkflowAction/",
        data: { "EncryptedID": EncryptedId },
        success: function (data) {
            CollapseOfficeList();
            $("#panelNewWorkFlowAction").hide();
            $('#DivWorkFlowActionDetailsWrapper').html(data);
            $('#DivWorkFlowActionDetailsWrapper').fadeIn(500);
            $("#Discriptiontxt").focus();

        },
        error: function () {
            //bootbox.alert("error in updating");
            bootbox.alert({
                //   size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in Updating</span>'
            });
        }
    })
}
function DeleteWorkflowActionData(EncryptedId) {
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
                $.ajax({
                    type: "Post",
                    url: "/UserManagement/WorkFlowActionDetails/DeleteWorkflowAction/",
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
                        }
                        else {
                            bootbox.alert({
                                //   size: 'small',
                                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in Deleting</span>'
                            });
                        }
                    },
                    error: function () {
                        bootbox.alert({
                            //   size: 'small',
                            //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                        });
                    }
                });
            }
        }
    });
}
function CollapseOfficeList() {
    $('#WorkFlowActionListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-plus-square-o fa-pull-left fa-2x');
}
function OpenOfficeList() {
    $('#WorkFlowActionListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-minus-square-o fa-pull-left fa-2x');
}

function FillDataTable(jsonData) {
    if ($.fn.DataTable.isDataTable("#WorkFlowActionGrid")) {
        $('#WorkFlowActionGrid').remove();
        $('.clsTblWrapper').html('<table id="WorkFlowActionGrid" class="display table table-responsive table-striped table-bordered" cellspacing="0" width="100%"></table>');
    }
    var table = $('#WorkFlowActionGrid').DataTable({
       
        destroy: true,
        paging: true,       
        "filter": true,
        "pageLength": 10,
        searching: true,
        responsive: true,
        "scrollY": "30vh",
        scrollCollapse: true,
        language: { search: "Search" },
        ordering: true,//sorting..       
        columnDefs: [{ orderable: false, targets: [0] }, { orderable: false, targets: [5] }],

        //columnDefs: [{ orderable: false, targets: [0] },{ orderable: false, targets: [5] }, { orderable: false, targets: [7] }, { orderable: false, targets: [8] }],
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
    //$("#WorkFlowActionGrid_info").css('margin-left', "-80%");
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
    $('#WorkFlowActionGrid_info').css('margin-left', '-2%');
    $('#WorkFlowActionGrid_info').css('font-size', '120%');




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


