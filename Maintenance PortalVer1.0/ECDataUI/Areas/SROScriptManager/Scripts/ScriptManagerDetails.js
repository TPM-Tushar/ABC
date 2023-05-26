//Global variables.
var token = '';
var header = {};


$(document).ready(function () {

    //alert("In document ready 1");
    //$.validator.unobtrusive.parse(form);
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#scriptManagerListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    $("#SearchBtn").click(function () {

        if ($.fn.DataTable.isDataTable("#scriptManagerDetailsListTable")) {
            $("#scriptManagerDetailsListTable").DataTable().clear().destroy();
        }

        if ($.fn.DataTable.isDataTable("#scriptManagerDetailsListTable")) {
            $("#scriptManagerDetailsListTable").DataTable().clear().destroy();
        }
        ServicePack = $("#txtSrchServicePackNumber").val();
        
        var ReScanningDetailsTable = $('#scriptManagerDetailsListTable').DataTable({
            ajax: {
                url: '/SROScriptManager/SROScriptManager/LoadScriptManagerTable',
                type: "POST",
                headers: header,
                data: {
                    ServicePack : ServicePack
                },
                dataSrc: function (json) {
                    unBlockUI();
                    if (json.errorMessage != null) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            callback: function () {
                                if (json.serverError != undefined) {
                                    window.location.href = "/Home/HomePage"
                                } else {
                                    var classToRemove = $('#ToggleIconID').attr('class');
                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                        $('#scriptManagerListCollapse').trigger('click');
                                    $("#scriptManagerDetailsListTable").DataTable().clear().destroy();
                                    $("#PDFSPANID").html('');
                                    $("#EXCELSPANID").html('');
                                }
                            }
                        });
                    }
                    else {
                        var classToRemove = $('#ToggleIconID').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                            $('#scriptManagerListCollapse').trigger('click');
                    }
                    unBlockUI();
                    return json.data;
                },
                error: function () {
                    unBlockUI();
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                },
                beforeSend: function () {
                    blockUI('loading data.. please wait...');
                    var searchString = $('#scriptManagerDetailsListTable_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;

                        if (!regexToMatch.test(searchString)) {
                            $("#scriptManagerDetailsListTable_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                ReScanningDetailsTable.search('').draw();
                                $("#scriptManagerDetailsListTable_filter input").prop("disabled", false);
                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
            },
            serverSide: true,
            //"scrollX": true,
            //"scrollY": "300px",
            scrollCollapse: true,
            bPaginate: true,
            bLengthChange: true,
            bInfo: true,
            info: true,
            bFilter: false,
            searching: true,
            "destroy": true,
            "bAutoWidth": true,
            "bScrollAutoCss": true,

            columnDefs: [
                { "className": "dt-center", "targets": "_all" },
                { width: '9%', targets: [0] },
                //Chaged by Omkar on 04092020 start

                { width: '9%', targets: [1] },
                { width: '34%', targets: [2] },
                { width: '8%', targets: [3] },
                { width: '16%', targets: [4] },
                { width: '8%', targets: [5] },
                { width: '8%', targets: [6] },
                { width: '8%', targets: [7] },


               //Chaged by Omkar on 04092020 end


                //{ orderable: false, targets: [1] },
                //{ orderable: false, targets: [2] },
                //{ orderable: false, targets: [3] },
                //{ orderable: false, targets: [4] }
            ],
            order: [[5, 'desc']],   

            columns: [
                {
                    data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo"
                },
                                    //Added by Omkar on 04092020

                {
                    data: "ScriptID", "searchable": true, "visible": true, "name": "ScriptID"
                },
                {
                    data: "Script", "searchable": true, "visible": true, "name": "Script"
                },
                {
                    data: "ServicePack", "searchable": true, "visible": true, "name": "ServicePack"
                },
                {
                    data: "Description", "searchable": true, "visible": true, "name": "Description"
                },
                {
                    data: "DateOfScript", "searchable": true, "visible": true, "name": "DateOfScript"
                },
                {
                    data: "IsActive", "searchable": true, "visible": true, "name": "IsActive",
                    "render": function (data, type, row) {
                        return (data == true) ? '<span class="glyphicon glyphicon-ok" style="color:green" title="Active"> </span > ' : '<span class="glyphicon glyphicon-remove" style="color:red" title="Not Active"></span>';
                    }
                },
                {
                    data: "Action", "searchable": true, "visible": true, "name": "Action"
                }
            ],
            fnInitComplete: function (oSettings, json) {
                //$("#PDFSPANID").html(json.PDFDownloadBtn);
              //  $("#EXCELSPANID").html(json.ExcelDownloadBtn);
            },
            preDrawCallback: function () {
                unBlockUI();
            },
            fnRowCallback: function (nRow, aData, iDisplayIndex) {

                //if (ModuleID == 1) {
                //    fnSetColumnVis(2, false);
                //}
                //else if (ModuleID == 2) {
                //    fnSetColumnVis(0, false);
                //    fnSetColumnVis(1, false);
                //}

                unBlockUI();
                return nRow;
            },
            drawCallback: function (oSettings) {
                unBlockUI();
            },
        }).order([5, 'desc']).draw();
    });

    //$("#btnUpdateScript").click(function () {
    //    UpdateInScriptManger();
    //});


    //$("#btnDownloadcript").click(function () {

    //    alert("#hdnScriptID" + $("#hdnScriptID").val());

    //    window.location.href = "/SROScriptManager/SROScriptManager/DownLoadScriptFile?ScriptID=" + $("#hdnScriptID").val();
    //});

    //$('#dvScriptManagerModal').modal({
    //    backdrop: 'static',
    //    keyboard: false
    //})

});


function EditSROScriptManagerDetails(id) {


    $.ajax({
        type: 'POST',
        datatype: 'json',
        headers: header,
        url: '/SROScriptManager/SROScriptManager/EditScriptsByID',
        async: false,
        cache: false,
        data: { ScriptID: id },
        success: function (data) {
            $("#dvScriptManagerModalBody").html(data);
            $("#dvScriptManagerMain").show();
            $("#dvScriptManagerModal").modal('show'); 
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}


//function UpdateInScriptManger() {


//    var formData = new FormData();
//    var filesArray = null;
//    $.each($("input[type='file']"), function () {
//        var id = $(this).attr('id');
//        var fileData = document.getElementById(id).files[0];
//        if (fileData != undefined) {
//            formData.append(id, fileData);
//            if (filesArray == null) {
//                filesArray = id + ",";
//            }
//            else {
//                filesArray += id + ",";
//            }
//        }
//    });

//    formData.append("filesArray", filesArray);

//    var form = $("#ScriptManagerUpdateForm");
//    form.removeData('validator');
//    form.removeData('unobtrusiveValidation');
//    $.validator.unobtrusive.parse(form);

//    //url: "/SROScriptManager/SROScriptManager/InsertScriptManager",
//    //    data: $("#ScriptManagerInsertForm").serialize(),

//    if ($("#ScriptManagerInsertForm").valid()) {

//        BlockUI();
//        $.ajax({
//            type: "POST",
//            url: "/SROScriptManager/SROScriptManager/UploadSQLScriptFile",
//            data: formData,
//            headers: header,
//            processData: false,
//            contentType: false,
//            success: function (data) {
//                if (data.success) {

//                    $("#hdnUpdateSQLScriptContent").val(data.FileDataStr);


//                    BlockUI();
//                    $.ajax({
//                        type: "POST",
//                        url: "/SROScriptManager/SROScriptManager/UpdateScriptManager",
//                        data: $("#ScriptManagerUpdateForm").serialize(),
//                        headers: header,

//                        //  processData: false,
//                        // contentType: false,
//                        success: function (data) {
//                            if (data.success) {

//                                bootbox.alert({
//                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
//                                    callback: function () {

//                                        window.location.href = "/SROScriptManager/SROScriptManager/SROScriptManagerView";
//                                    }
//                                });
//                            }
//                            else {
//                                bootbox.alert({
//                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                                });
//                            }
//                            $.unblockUI();
//                        },
//                        error: function () {
//                            bootbox.alert({
//                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                            });
//                            $.unblockUI();

//                        }
//                    });


//                }
//                else {


//                    bootbox.alert({
//                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                    });
//                    $.unblockUI();

//                }

//            },
//            error: function () {
//                bootbox.alert({
//                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                });
//                $.unblockUI();

//            }
//        });

//    }



//}


function EXCELDownloadFun(STRServicePackNumber) {

    window.location.href = '/SROScriptManager/SROScriptManager/ExportOtherDepartmentImportToExcel';
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