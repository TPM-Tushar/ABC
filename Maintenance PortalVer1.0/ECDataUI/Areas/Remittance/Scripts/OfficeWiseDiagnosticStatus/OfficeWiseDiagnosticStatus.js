//Global variables.
//var txtFromDate;
var txtToDate;
var token = '';
var header = {};
var OfficeWiseDiagnosticStatusDetailTable;
var SelectedOfficeType;
//var SelectedAction;
var SelectedStatus;
var OfficeID;
var OfficeName;

$(document).ready(function () {

    SelectedStatus = "2";

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $("#todaydate").html($("#txtDate").val())

    $('#txtDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

    });



    $('#divDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });

    $('#txtDate').datepicker({
        format: 'dd/mm/yyyy'
    }).datepicker("setDate", ToDate);

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove === "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    $("#SearchBtn").click(function () {

        switch (SelectedStatus) {
            case '0':
                $("#actiontypeheading").html('(All Offices)')
                break;
            case '1':
                $("#actiontypeheading").html('(All Check Successful)')
                break;
            case '2':
                $("#actiontypeheading").html('(Issues Found)')
                break;
            case '3':
                $("#actiontypeheading").html('(Status Not Available)')
                break;
            case '4':
                $("#actiontypeheading").html('(Status Available)')
                break;
        }

        txtToDate = $("#txtDate").val();
        //OfficeID = $("#OfficeTypeDropDownID option:selected").val();
        //SelectedAction = $("#ActionDropDownID option:selected").val();
        //SelectedStatus = $("#StatusDropDownID option:selected").val();
        //SelectedStatus = "0";

        //SelectedOfficeType = $("input[name='OfficeTypeToGetDropDown']:checked").val();
        SelectedOfficeType = "SRO";
        //OfficeName = $("#OfficeTypeDropDownID option:selected").text();

        //console.log(txtFromDate + " " + txtToDate + " " + selectedDB + " " + topRows);


        $("#todaydate").html(txtToDate)

        if (txtToDate == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
            return;

        }

        OfficeWiseDiagnosticStatusDetailTable = $('#OfficeWiseDiagnosticStatusTable').DataTable({

            ajax: {
                url: '/Remittance/OfficeWiseDiagnosticStatus/GetOfficeWiseDiagnosticStatusDetail',
                type: "POST",
                headers: header,
                //data: {
                //    'FromDate': txtFromDate, 'ToDate': txtToDate, 'OfficeName': OfficeID, /*'SelectedAction': SelectedAction,*/ 'SelectedStatus': SelectedStatus, 'SelectedOfficeType': SelectedOfficeType
                //},
                data: {
                    'Date': txtToDate, 'SelectedStatus': SelectedStatus //'OfficeName': OfficeID, 'SelectedOfficeType': SelectedOfficeType, 
                },
                dataSrc: function (json) {
                    unBlockUI();
                    if (json.errorMessage != null) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            callback: function () {
                                if (json.serverError != undefined) {
                                    window.location.href = "/Home/HomePage"
                                }
                                else {
                                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                        $('#DtlsSearchParaListCollapse').trigger('click');
                                    $("#DailyReceiptDetailsTable").DataTable().clear().destroy();
                                    $("#EXCELSPANID").html('');
                                }
                            }
                        });
                    }
                    else {
                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                            $('#DtlsSearchParaListCollapse').trigger('click');
                    }
                    unBlockUI();
                    return json.data;
                },
                error: function () {
                    //unBlockUI();                   
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    //$.unblockUI();
                    unBlockUI();
                },
                beforeSend: function () {
                    blockUI('loading data.. please wait...');
                    var searchString = $('#DailyReceiptDetailsTable_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#DailyReceiptDetailsTable_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                //    $('#menuDetailsListTable_filter input').val('');
                                DailyReceiptRptTable.search('').draw();
                                $("#DailyReceiptDetailsTable_filter input").prop("disabled", false);
                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
            },
            serverSide: true,
            // pageLength: 100,
            "scrollX": true,
            //"scrollY": true,
            "scrollY": "300px",
            "scrollCollapse": true,
            bPaginate: true,
            bLengthChange: true,
            // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
            "lengthMenu": [252],
            // "pageLength": -1,
            //sScrollXInner: "150%",
            bInfo: true,
            info: true,
            bFilter: false,
            searching: true,

            // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
            //dom: 'lBfrtip',
            "destroy": true,
            "bAutoWidth": true,
            "bScrollAutoCss": true,
            "bSort": true,
            columnDefs: [
                //{ width: 50, targets: "_all" }
                //{ width: 50, targets: 2 }
                //{ orderable: false, targets: [0] },

                //{ orderable: false, targets: [1] },
                //{ orderable: false, targets: [2] },
                ////{ orderable: false, targets: [3] },
                ////{ orderable: false, targets: [4] },
                //{ orderable: false, targets: [5] },
                //{ orderable: false, targets: [6] },
                //{ orderable: false, targets: [7] },
                //{ orderable: false, targets: [8] },
                //{ orderable: false, targets: [9] },
                //{ orderable: false, targets: [10] },
                //{ orderable: false, targets: [11] },
                //{ orderable: false, targets: [12] }


                //targets: [4,7,10,13,16,19,22],
                //createdCell: function (td, data, type, row, meta) {
                //    //console.log(row);
                //    //console.log(type.AllActionStatus);
                //    //console.log(meta);
                //    //console.log(data);

                //    if (data === false && type.AllActionStatus === false) {
                //        $(td).css('background', 'red');
                //    }

                //data =  "<td style:'background: red'><i class='fa fa-times fa-lg' aria-hidden='true'></i></td>";
                //}
            
            ],

            columns: [

                { data: "SrNo", "searchable": false, "visible": true, "name": "SrNo" },
                { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
                { data: "DiagnosticDate", "searchable": true, "visible": true, "name": "DiagnosticDate" },
                {
                    data: "AllActionStatus", "searchable": false, "visible": true, "name": "AllActionStatus",
                    //render: function (data, type, row, full, meta) {
                    //    //console.log(row);
                    //    //console.log(meta);
                    //    //console.log(full);
                    //    if (data === false) {
                    //        var res = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                    //        return res;
                    //    } else {
                    //        var res2 = "<i class='fa fa-check fa-lg' aria-hidden='true'></i>";
                    //        return res2;
                    //    }
                    //}
                },
                { data: "DataFileSize", "searchable": true, "visible": true, "name": "DataFileSize" },
                { data: "LogFileSize", "searchable": true, "visible": true, "name": "LogFileSize" },
                { data: "DBDiskSpace", "searchable": true, "visible": true, "name": "DBDiskSpace" },
                //{ data: "SecureCodeHTML", "searchable": true, "visible": true, "name": "SecureCodeHTML" },
                
                {
                    data: "TimeZoneStatus", "searchable": false, "visible": true, "name": "TimeZoneStatus"
                    
                },
                {
                    data: "DBCCStatus", "searchable": false, "visible": true, "name": "DBCCStatus",
                    //render: function (data, type, row) {

                    //    if (data == false && row.DBCCErrorDesc == '') {
                    //        var res = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                    //        return res;
                    //    } else {
                    //        var res2 = "<i class='fa fa-check fa-lg' aria-hidden='true'></i>";
                    //        return res2;
                    //    }
                    //}
                },
                //{ data: "DBCCErrorDesc", "searchable": false, "visible": false, "name": "DBCCErrorDesc" },
                //{ data: "DBCCOutput", "searchable": false, "visible": false, "name": "DBCCOutput" },
                {
                    data: "ConstraintIntegrityStatus", "searchable": false, "visible": true, "name": "ConstraintIntegrityStatus",
                    //render: function (data, type, row) {

                    //    if (data == false && (row.ConstraintIntegrityErrorDesc == '' || row.LastFullBackupErrorDesc == null )) {
                    //        var res = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                    //        return res;
                    //    } else {
                    //        var res2 = "<i class='fa fa-check fa-lg' aria-hidden='true'></i>";
                    //        return res2;
                    //    }
                    //}
                },
                //{ data: "ConstraintIntegrityErrorDesc", "searchable": false, "visible": false, "ConstraintIntegrityErrorDesc": "Date" },
                //{ data: "ConstraintIntegrityOutput", "searchable": false, "visible": false, "name": "ConstraintIntegrityOutput" },
                {
                    data: "AuditEventStatus", "searchable": false, "visible": true, "name": "AuditEventStatus",
                    //render: function (data, type, row) {

                    //    if (data == false && (row.AuditEventErrorDesc == '' || row.LastFullBackupErrorDesc == null)) {
                    //        var res = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                    //        return res;
                    //    } else {
                    //        var res2 = "<i class='fa fa-check fa-lg' aria-hidden='true'></i>";
                    //        return res2;
                    //    }
                    //}
                },
                //{ data: "AuditEventErrorDesc", "searchable": false, "visible": false, "name": "AuditEventErrorDesc" },
                //{ data: "AuditEventOutput", "searchable": false, "visible": false, "name": "AuditEventOutput" },
                {
                    data: "Optimizer1Status", "searchable": false, "visible": true, "name": "Optimizer1Status",
                    //render: function (data, type, row) {
                    //    if (data == false && (row.Optimizer1ErrorDesc == '' || row.LastFullBackupErrorDesc == null)) {
                    //        alert('');
                    //        var res = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                    //        return res;
                    //    } else {
                    //        //alert('');
                    //        var res2 = "<i class='fa fa-check fa-lg' aria-hidden='true'></i>";
                    //        return res2;
                    //    }
                    //}
                },
                //{ data: "Optimizer1ErrorDesc", "searchable": false, "visible": false, "name": "Optimizer1ErrorDesc" },
                //{ data: "Optimizer1Output", "searchable": false, "visible": false, "name": "Optimizer1Output" },
                {
                    data: "Optimizer2Status", "searchable": false, "visible": true, "name": "Optimizer2Status",
                    //render: function (data, type, row) {
                    //    //console.log(row);
                    //    if (data == false && (row.Optimizer2ErrorDesc == '' || row.LastFullBackupErrorDesc == null)) {
                    //        var res = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                    //        return res;
                    //    } else {
                    //        var res2 = "<i class='fa fa-check fa-lg' aria-hidden='true'></i>";
                    //        return res2;
                    //    }
                    //}
                },
                //{ data: "Optimizer2ErrorDesc", "searchable": false, "visible": false, "name": "Optimizer2ErrorDesc" },
                //{ data: "Optimizer2Output", "searchable": false, "visible": false, "name": "Optimizer2Output" },
                {
                    data: "LastFullBackupStatus", "searchable": false, "visible": true, "name": "LastFullBackupStatus",
                    //render: function (data, type, row) {
                    //    if (data == false && (row.LastFullBackupErrorDesc == '' || row.LastFullBackupErrorDesc == null)) {
                    //        var res = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                    //        return res;
                    //    } else {
                    //        var res2 = "<i class='fa fa-check fa-lg' aria-hidden='true'></i>";
                    //        return res2;
                    //    }
                    //}
                },
                //{ data: "LastFullBackupErrorDesc", "searchable": false, "visible": false, "name": "LastFullBackupErrorDesc" },
                //{ data: "LastFullBackupOutput", "searchable": false, "visible": false, "name": "LastFullBackupOutput" },
                {
                    data: "LastDiffBackupStatus", "searchable": false, "visible": true, "name": "LastDiffBackupStatus",
                    //render: function (data, type, row) {
                    //    //console.log(row.LastDiffBackupErrorDesc);
                    //    if (data == false && (row.LastDiffBackupErrorDesc == '' || row.LastFullBackupErrorDesc == null)) {
                    //        var res = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                    //        return res;
                    //    } else {
                    //        var res2 = "<i class='fa fa-check fa-lg' aria-hidden='true'></i>";
                    //        return res2;
                    //    }
                    //}
                },
                //{ data: "LastDiffBackupErrorDesc", "searchable": false, "visible": false, "name": "LastDiffBackupErrorDesc" },
                //{ data: "LastDiffBackupOutput", "searchable": false, "visible": false, "name": "LastDiffBackupOutput" },

            ],
            fnInitComplete: function (oSettings, json) {
                //alert('in fnInitComplete');
                //$("#PDFSPANID").html(json.PDFDownloadBtn);
                //console.log(json);
                $(".action-error").parent().addClass("td-action");
                //$(".action-error").parent().css({ "background": "#73a9d9" });
                $(".action-error").parent().css({ "background": "#f08080" });
                //$("#EXCELSPANID").html(json.ExcelDownloadBtn);

                //FOR TILES DATA
                $("#T_1_Figure").html(json.tilesData.TotalNo);
                $("#T_2_Figure").html(json.tilesData.StatusAvailabelNo);
                $("#T_3_Figure").html(json.tilesData.StatusNotAvailable);
                $("#T_4_Figure").html(json.tilesData.AllOkNo);
                $("#T_5_Figure").html(json.tilesData.IssueFoundNo);
                $("#T_6_Figure").html(json.tilesData.TotalIssueFound);

                $("#T_2_Desc").html(json.tilesData.StatusAvailabelDesc);
                $("#T_3_Desc").html(json.tilesData.StatusNotAvailableDesc);
                $("#T_4_Desc").html(json.tilesData.AllOkDesc);
                $("#T_5_Desc").html(json.tilesData.IssueFoundNoDesc);

                //for dashboard table data
                
                $("#data_0").html(json.tilesData.ActionErrorsList[0].NumberOfErrors);
                $("#data_1").html(json.tilesData.ActionErrorsList[1].NumberOfErrors);
                $("#data_2").html(json.tilesData.ActionErrorsList[2].NumberOfErrors);
                $("#data_3").html(json.tilesData.ActionErrorsList[3].NumberOfErrors);
                $("#data_4").html(json.tilesData.ActionErrorsList[4].NumberOfErrors);
                $("#data_5").html(json.tilesData.ActionErrorsList[5].NumberOfErrors);
                $("#data_6").html(json.tilesData.ActionErrorsList[6].NumberOfErrors);
                $("#data_7").html(json.tilesData.ActionErrorsList[7].NumberOfErrors);
                $("#data_8").html(json.tilesData.ActionErrorsList[8].NumberOfErrors);
                $("#data_9").html(json.tilesData.ActionErrorsList[9].NumberOfErrors);


            },
            preDrawCallback: function () {
                unBlockUI();
            },
            fnRowCallback: function (nRow, aData, iDisplayIndex) {
                unBlockUI();
                return nRow;
            },
            drawCallback: function (oSettings) {
                //responsiveHelper.respond();
                $(".action-error").parent().addClass("td-action");
                //$(".action-error").parent().css({ "background": "#73a9d9" });
                $(".action-error").parent().css({ "background": "#f08080" });
                unBlockUI();
            }
        });

    });

    $('#OfficeWiseDiagnosticStatusTable').on('click', 'tbody tr', function () {
        //console.log('API row values : ', OfficeWiseDiagnosticStatusDetailTable.row(this).data());
        //console.log('API row values : ', OfficeWiseDiagnosticStatusDetailTable.row(this).data());
    });


    $('#OfficeWiseDiagnosticStatusTable').on('click', 'tbody td', function () {
        
        ////get textContent of the TD
        //console.log('TD cell textContent : ', this.textContent);

        ////get the value of the TD using the API
        //console.log('value by API : ', OfficeWiseDiagnosticStatusDetailTable.cell({ row: this.parentNode.rowIndex, column: this.cellIndex }).data());

        ////$('#modalValue').html(data);
    });


    $("#SearchBtn").click();

});


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

function GetActionData(ActionId, DetailId, MasterId) {
    //alert('data- ' + data);
    //console.log(ActionId + DetailId + MasterId);
    $.ajax({
        url: '/Remittance/OfficeWiseDiagnosticStatus/GetActionDetail',
        headers: header,
        type: "POST",
        data: {
            'ActionId': ActionId, 'DetailId': DetailId, 'MasterId': MasterId
        },
        success: function (data) {
            //console.log(data.data);
            $('#modalValue').html(data.data);
            $("#exampleModalLong").modal('show');
        },
        error: function (xhr) {

            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                callback: function () {
                }
            });
            unBlockUI();
        }
    });
}

function EXCELDownloadFun() {

    window.location.href = '/Remittance/OfficeWiseDiagnosticStatus/ExportOfficeWiseDiagnosticStatusToExcel?OfficeType=' + "&Status=" + SelectedStatus +  "&txtDate=" + txtToDate;
}

function CopyText() {
    /* Get the text field */
    var data = document.getElementById("modalValue");
    //console.log(data.innerHTML);
    containerid = 'modalValue';

    var elm = document.getElementById("modalValue");
    // for Internet Explorer

    if (document.body.createTextRange) {
        var range = document.body.createTextRange();
        range.moveToElementText(elm);
        range.select();
        document.execCommand("Copy");
    }
    else if (window.getSelection) {
        // other browsers

        var selection = window.getSelection();
        var range2 = document.createRange();
        range2.selectNodeContents(elm);
        selection.removeAllRanges();
        selection.addRange(range2);
        document.execCommand("Copy");
    }
   
}

function PopulateDataTable(statusCode) {
    SelectedStatus = statusCode;
    $("#SearchBtn").click();
}


function onDatechangeevent() {
    SelectedStatus = "2";
}

function GetDataByActionType(ActionID) {

    OfficeWiseDiagnosticStatusDetailTable = $('#OfficeWiseDiagnosticStatusTable').DataTable({

        ajax: {
            url: '/Remittance/OfficeWiseDiagnosticStatus/GetDiagnosticStatusDetailByActionType',
            type: "POST",
            headers: header,
            data: {
                'Date': txtToDate, 'SelectedStatus': SelectedStatus, 'ActionId': ActionID
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            }
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#DailyReceiptDetailsTable").DataTable().clear().destroy();
                                $("#EXCELSPANID").html('');
                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                }
                unBlockUI();
                return json.data;
            },
            error: function () {
                //unBlockUI();                   
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                //$.unblockUI();
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');
                var searchString = $('#DailyReceiptDetailsTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#DailyReceiptDetailsTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            //    $('#menuDetailsListTable_filter input').val('');
                            DailyReceiptRptTable.search('').draw();
                            $("#DailyReceiptDetailsTable_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        // pageLength: 100,
        "scrollX": true,
        //"scrollY": true,
        "scrollY": "300px",
        "scrollCollapse": true,
        bPaginate: true,
        bLengthChange: true,
        // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        "lengthMenu": [252],
        // "pageLength": -1,
        //sScrollXInner: "150%",
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,

        // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
        //dom: 'lBfrtip',
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,
        "bSort": true,
        columnDefs: [

        ],

        columns: [

            { data: "SrNo", "searchable": false, "visible": true, "name": "SrNo" },
            { data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName" },
            { data: "DiagnosticDate", "searchable": true, "visible": true, "name": "DiagnosticDate" },
            { data: "AllActionStatus", "searchable": false, "visible": true, "name": "AllActionStatus"},
            { data: "DataFileSize", "searchable": true, "visible": true, "name": "DataFileSize" },
            { data: "LogFileSize", "searchable": true, "visible": true, "name": "LogFileSize" },
            { data: "DBDiskSpace", "searchable": true, "visible": true, "name": "DBDiskSpace" },
            //{ data: "SecureCodeHTML", "searchable": true, "visible": true, "name": "SecureCodeHTML" },
           
            {
                data: "TimeZoneStatus", "searchable": false, "visible": true, "name": "TimeZoneStatus"

            },
            {
                data: "DBCCStatus", "searchable": false, "visible": true, "name": "DBCCStatus"
            },
            {
                data: "ConstraintIntegrityStatus", "searchable": false, "visible": true, "name": "ConstraintIntegrityStatus"
            },
            {
                data: "AuditEventStatus", "searchable": false, "visible": true, "name": "AuditEventStatus"
            },

            {
                data: "Optimizer1Status", "searchable": false, "visible": true, "name": "Optimizer1Status"
            },
            {
                data: "Optimizer2Status", "searchable": false, "visible": true, "name": "Optimizer2Status"
            },
            {
                data: "LastFullBackupStatus", "searchable": false, "visible": true, "name": "LastFullBackupStatus"
            },
            {
                data: "LastDiffBackupStatus", "searchable": false, "visible": true, "name": "LastDiffBackupStatus",
            }

        ],
        fnInitComplete: function (oSettings, json) {

            $(".action-error").parent().addClass("td-action");
            //$(".action-error").parent().css({ "background": "#73a9d9" });
            $(".action-error").parent().css({ "background": "#f08080" });
            //$("#EXCELSPANID").html(json.ExcelDownloadBtn);

        },
        preDrawCallback: function () {
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
            //responsiveHelper.respond();
            $(".action-error").parent().addClass("td-action");
            //$(".action-error").parent().css({ "background": "#73a9d9" });
            $(".action-error").parent().css({ "background": "#f08080" });
            unBlockUI();
        }
    });
}


//var txtFromDate;
//var txtToDate;
//var token = '';
//var header = {};
//var OfficeWiseDiagnosticStatusDetailTable;
//var SelectedOfficeType;
////var SelectedAction;
//var SelectedStatus;
//var OfficeName;


//switch (statusCode) {
//    case '0':
//        $("#actiontypeheading").html('(All Offices)')
//        break;
//    case '1':
//        $("#actiontypeheading").html('(All Check Successful)')
//        break;
//    case '2':
//        $("#actiontypeheading").html('(Issues Found)')
//        break;
//    case '3':
//        $("#actiontypeheading").html('(Status Not Available)')
//        break;
//    case '4':
//        $("#actiontypeheading").html('(Status Available)')
//        break;
//}