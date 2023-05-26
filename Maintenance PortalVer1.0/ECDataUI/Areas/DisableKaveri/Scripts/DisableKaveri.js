//$(document).ready(function () {
//    $('#DisableKaveriTable').DataTable({
//        ajax: {
//            url: '/DisableKaveri/DisableKaveri/DisableKaveriView1/',
//            dataSrc: 'data'
//        },

//        "scrollY": "300px",

//        "bAutoWidth": true,
//        "bScrollAutoCss": true,
//        "paging": false,
//       // ordering: false,
//        //"info": false,

//        columns: [
//            {
//                data: null,
//                orderable: false,
//                className: 'select-checkbox',
//                defaultContent: ''
//            },
//            {
//                data: "srNo", "searchable": true, "visible": true, "name": "srNo"

//            },
//            { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
//            { data: "IsDisabled", "searchable": true, "visible": true, "name": "IsDisabled" },
//            { data: "DisableDate", "searchable": true, "visible": true, "name": "DisableDate" },
//            { data: "Kaveri1Code", "searchable": true, "visible": false, "name": "Kaveri1Code" },
//        ],

//        select: {
//            style: 'multi',
//            selector: 'td:first-child'
//        },
//    });
//});
$(document).ready(function () {
    blockUI('Loading data please wait.');
    $('#DisableKaveriTable').DataTable({
        ajax: {
            url: '/DisableKaveri/DisableKaveri/DisableKaveriViewDetails/',
            dataSrc: 'data',
            //dataSrc: function (json) {
            //    unBlockUI();
            //    if (json.message != null) {
            //        bootbox.alert({
            //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.message + '</span>',
            //            callback: function () {
            //                if (json.serverError != undefined) {
            //                    window.location.href = "/Home/HomePage"
            //                }
            //                else {


            //                }
            //            }
            //        });
            //    }
            //},
            error: function () {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });

                unBlockUI();
            },
        },

        "scrollY": "500px",

        //"bAutoWidth": true,
        "bScrollAutoCss": true,
        "paging": false,
        // ordering: false,
        //"info": false,

        columns: [
            {
                data: null,
                orderable: false,
                className: 'select-checkbox',
                defaultContent: '',
                createdCell: function (td, cellData, rowData, row, col) {
                    unBlockUI();
                    if (rowData.DisableDate !== "--") {
                        $(td).closest('tr').find('.select-checkbox').prop('disabled', true);
                        //$(td).closest('tr').addClass('disable-row');
                        //$('.disable-row').attr('id', 'id-disable-row');
                        $(td).closest('tr').find('.select-checkbox').removeClass('select-checkbox');
                    }
                }
            },
            { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "className": "text-center" },
            { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
            { data: "IsDisabled", "searchable": true, "visible": true, "name": "IsDisabled" },
            { data: "DisableDate", "searchable": true, "visible": true, "name": "DisableDate" },
            { data: "Kaveri1Code", "searchable": true, "visible": false, "name": "Kaveri1Code" },
        ],

        select: {
            style: 'multi',
            selector: 'td:first-child'
        },
    });
    $('tr.disable-row').css('color', 'gray');
});






function SubmitDetails() {

    var selectedRows = $('#DisableKaveriTable').DataTable().rows('.selected').data();

    // Loop through the selected rows and get the "OfficeName" value
    //var Kaveri1Codes = [];
    //for (var i = 0; i < selectedRows.length; i++) {
    //    var rowData = selectedRows[i];
    //    var Kaveri1Code = rowData.Kaveri1Code;
    //    Kaveri1Codes.push(Kaveri1Code);
    //}
    if (selectedRows.length == 0) {
        return;
    }
    var OfficeName = GetOfficeName(selectedRows);
    var KaveriCode = null;
    //
    bootbox.confirm({
        message: '<span class="boot-alert-txt">Do you want to disable Kaveri for office(s) ' + OfficeName + '?</span>',
        buttons: {
            confirm: {
                label: 'Yes',
                //className: 'btn-success'
            },
            cancel: {
                label: 'No',
               // className: 'btn-danger'
            }
        },
        callback: function (result) {
            //alert('This was logged in the callback: ' + result);
            if (result) {
                KaveriCode = GetKaveriCode(selectedRows);
               // alert(KaveriCode);
                blockUI('Loading data please wait.');
                $.ajax({
                    type: "POST",
                    url: '/DisableKaveri/DisableKaveri/UpdateDisableKaveriDetails/',
                    data: { 'KaveriCode': KaveriCode, },
                    dataType: "json",
                    success: function (data) {
                        if (data.success && !data.serverError) {
                            bootbox.alert({
                                message: '<i class="fa fa-check text-success boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                callback: function () {
                                }
                            })
                            $('#DisableKaveriTable').DataTable().ajax.reload();
                        } else {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                                callback: function () {
                                }
                            })
                        }
                        unBlockUI();
                    },
                    error: function () {
                        unBlockUI();
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            callback: function () {
                            }
                        })
                    },
                });
            }
        }
    });
    //
    //alert(KaveriCode);
    // Display the "OfficeName" values
    //console.log(KaveriCode);
  
}
function GetKaveriCode(selectedRows) {
    var Kaveri1Codes = [];
    for (var i = 0; i < selectedRows.length; i++) {
        var rowData = selectedRows[i];
        var Kaveri1Code = rowData.Kaveri1Code;
        Kaveri1Codes.push(Kaveri1Code);
    }
    return Kaveri1Codes;
}
function GetOfficeName(selectedRows) {
    var OfficeNames = [];
    for (var i = 0; i < selectedRows.length; i++) {
        var rowData = selectedRows[i];
        var OfficeName = rowData.OfficeName;
        OfficeNames.push(OfficeName);
    }
    return OfficeNames;
}