//Global variables.
var token = '';
var header = {};


$(document).ready(function () {
    
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    

    $('#scriptManagerListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    $("#DROSearchBtn").click(function () {

        if ($.fn.DataTable.isDataTable("#DROscriptManagerDetailsListTable")) {
            $("#DROscriptManagerDetailsListTable").DataTable().clear().destroy();
        }

        if ($.fn.DataTable.isDataTable("#DROscriptManagerDetailsListTable")) {
            $("#DROscriptManagerDetailsListTable").DataTable().clear().destroy();
        }
        ServicePack = $("#txtSrchDROServicePackNumber").val();

        var ReScanningDetailsTable = $('#DROscriptManagerDetailsListTable').DataTable({
            ajax: {
                url: '/SROScriptManager/SROScriptManager/LoadDROScriptManagerTable',
                type: "POST",
                headers: header,
                data: {
                    ServicePack: ServicePack
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
                                    $("#DROscriptManagerDetailsListTable").DataTable().clear().destroy();
                                  //  $("#PDFSPANID").html('');
                                 //   $("#EXCELSPANID").html('');
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
                    var searchString = $('#DROscriptManagerDetailsListTable_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;

                        if (!regexToMatch.test(searchString)) {
                            $("#DROscriptManagerDetailsListTable_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                ReScanningDetailsTable.search('').draw();
                                $("#DROscriptManagerDetailsListTable_filter input").prop("disabled", false);
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
               // $("#EXCELSPANID").html(json.ExcelDownloadBtn);
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

    $("#txtticketNumber").val('');            

    $("#SQLScriptManagerFile").change(function () {

        var fileExtension = ['sql'];//, 'xls'

        if ($.inArray($(this).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Only SQL file is allowed ." + '</span>',
                callback: function () { }
            });

            $(this).val('');
            return false;
        }

        var file = document.getElementById(this.id).files.item(0);
        var docid = $(this).attr('id');
        if (file.size > MaxFileSizeToUpload) {

            var MaxFilesizeInMB = MaxFileSizeToUpload / 1048576;

            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "File size is greater than " + MaxFilesizeInMB + ' MB. ' + 'Please upload another file</span>',
                callback: function () { }
            });
            $(this).val('');
            return false;

        }
    });

    $("#btnInsertDROScript").click(function () {
        $("#hdnSQLScriptManagerFile").val($("#SQLScriptManagerFile").val());
        InsertInDROScriptManger();
    });
    

    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });



});

function InsertInDROScriptManger() {


    var formData = new FormData();
    var filesArray = null;
    $.each($("input[type='file']"), function () {
        var id = $(this).attr('id');
        var fileData = document.getElementById(id).files[0];
        if (fileData != undefined) {
            formData.append(id, fileData);
            if (filesArray == null) {
                filesArray = id + ",";
            }
            else {
                filesArray += id + ",";
            }
        }
    });

    formData.append("filesArray", filesArray);

    var form = $("#DROScriptManagerInsertForm");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    //url: "/SROScriptManager/SROScriptManager/InsertScriptManager",
    //    data: $("#DROScriptManagerInsertForm").serialize(),

    if ($("#DROScriptManagerInsertForm").valid()) {
        
        BlockUI();
        $.ajax({
            type: "POST",
            url: "/SROScriptManager/SROScriptManager/UploadSQLScriptFile",
            data: formData,
            headers: header,
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.success) {

                    $("#hdnSQLScriptContent").val(data.FileDataStr);
                    

                    BlockUI();
                    $.ajax({
                        type: "POST",
                        url: "/SROScriptManager/SROScriptManager/AddDROScriptManager",
                        //data: { sqlFilePath: data.filePath, TicketNumber: vTicketNumber },
                        data: $("#DROScriptManagerInsertForm").serialize(),
                        headers: header,

                        //  processData: false,
                        // contentType: false,
                        success: function (data) {
                            if (data.success) {

                                bootbox.alert({
                                    message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                    callback: function () {

                                        window.location.href = "/SROScriptManager/SROScriptManager/DROScriptManagerView";
                                    }
                                });
                            }
                            else {
                                bootbox.alert({
                                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                });
                            }
                            $.unblockUI();
                        },
                        error: function () {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                            });
                            $.unblockUI();

                        }
                    });


                }
                else {


                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                    });
                    $.unblockUI();

                }

            },
            error: function () {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                });
                $.unblockUI();

            }
        });

    }



}

function EditSROScriptManagerDetails(id) {


    $.ajax({
        type: 'POST',
        datatype: 'json',
        headers: header,
        url: '/SROScriptManager/SROScriptManager/EditDROScriptsByID',
        async: false,
        cache: false,
        data: { ScriptID: id },
        success: function (data) {
            $("#dvDROScriptManagerModalBody").html(data);
            $("#dvDROScriptManagerMain").show();
            $("#dvDROScriptManagerModal").modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}



function EXCELDownloadFun( INTSroId, FromDate, FromDate, STRReportNameID) {

    window.location.href = '/MISReports/OtherDepartmentImport/ExportOtherDepartmentImportToExcel?FromDate=' + FromDate + "&ToDate=" + ToDate + "&SroID=" + INTSroId + "&DistrictID=" + INTDistrictId + "&ReportName=" + STRReportNameID;
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