
$(document).ready(function () {
    $("ul#crumbs li:nth-child(3)").trigger('click');

    GetJsonData();

    $('#WorkFlowConfigurationListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    $('#WorkFlowConfigurationListCollapse').trigger('click');


    $("#btnAddWorkFlowConfigurationDetails").click(function () {
       
        $.ajax({
            type: "Get",
            url: "/UserManagement/WorkFlowConfigurationDetails/CreateWorkFlowConfiguration",
            success: function (data) {
                CollapseOfficeList();
                $("#panelNewWorkFlowConfiguration").hide();
                $('#DivWorkFlowConfigurationDetailsWrapper').html(data);
                $('#DivWorkFlowConfigurationDetailsWrapper').fadeIn(500);
                $("#ActionDropDwonList").focus();
                document.documentElement.scrollTop = 0;

            },
            error: function () {
                bootbox.alert({
                    //   size: 'small',
                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in Updating</span>',

                });
            }
        });


    });


});

//--------Getting Data for DataTable----------------

function GetJsonData() {

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/UserManagement/WorkFlowConfigurationDetails/LoadWorkFlowConfigurationGridData",
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

function UpdateWorkflowConfigurationData(EncryptedId) {
    $.ajax({
        type: "Get",
        url: "/UserManagement/WorkFlowConfigurationDetails/UpdateWorkFlowConfiguration/",
        data: { "EncryptedID": EncryptedId },
        success: function (data) {
            CollapseOfficeList();
            $("#panelNewWorkFlowConfiguration").hide();
            $('#DivWorkFlowConfigurationDetailsWrapper').html(data);
            $('#DivWorkFlowConfigurationDetailsWrapper').fadeIn(500);
            $("#ActionDropDwonList").focus();

        },
        error: function () {
            //bootbox.alert("error in updating");
            bootbox.alert({
                //   size: 'small',
                //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in updating</span>'
            });

        }
    })
}
function DeleteWorkflowConfigurationData(EncryptedId) {
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
                    url: "/UserManagement/WorkFlowConfigurationDetails/DeleteWorkFlowConfiguration/",
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
    $('#WorkFlowConfigurationListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-plus-square-o fa-pull-left fa-2x');
}
function OpenOfficeList() {
    $('#WorkFlowConfigurationListCollapse').trigger('click');
    var classToRemove = $('#ToggleIconID').attr('class');
    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-minus-square-o fa-pull-left fa-2x');
}

function FillDataTable(jsonData) {
    if ($.fn.DataTable.isDataTable("#WorkFlowConfigurationGrid")) {
        $('#WorkFlowConfigurationGrid').remove();
        $('.clsTblWrapper').html('<table id="WorkFlowConfigurationGrid" class="display table table-responsive table-striped table-bordered" cellspacing="0" width="100%"></table>');
    }
    var table = $('#WorkFlowConfigurationGrid').DataTable({
       
        destroy: true,
        paging: true,     
        pageLength: 10,
        searching: true,
        responsive: true,
        language: { search: "Search" },
        "scrollX": true,
        "scrollY": "30vh",
        scrollCollapse: true,
        ordering: true,//sorting..       
        columnDefs: [{ width: "10%", orderable: false, targets: [0] }, { orderable: false, targets: [4] }, { orderable: false, targets: [7] }],
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
    //$("#WorkFlowConfigurationGrid_info").css('margin-left', "-80%");
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
    $('#WorkFlowConfigurationGrid_info').css('margin-left', '-2%');
    $('#WorkFlowConfigurationGrid_info').css('font-size', '120%');




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