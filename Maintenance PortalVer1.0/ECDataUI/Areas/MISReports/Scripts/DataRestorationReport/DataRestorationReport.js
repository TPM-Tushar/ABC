var token = '';
var header = {};
var SroID = 0;
//var SROIDForScriptDownload = 0;

// ADDED BY SHUBHAM BHAGAT ON 05-08-2020
var DT_SROCodeForDownloadFile = 0;

// ADDED BY SHUBHAM BHAGAT ON  30-07-2020
// PREVIOUS CONDITION FIRST CLICK ON 'INITIATE NEW RESTORATION REQUEST' BTN AND THEN
// CLICK ON 'CLICK HERE' BTN TO INITIATE PROCESS NOW CHANGING WHEN CLICKED ON 
// 'INITIATE NEW RESTORATION REQUEST' BTN THEN UPDATE THE FLAG AND AFTER LOADING PARTIAL VIEW 
// IN PARTIAL VIEW CHECK FLAG AND TRIGGER ON GenerateKeyFun() AND CHANGE THE FLAG
var IsInitaiteNewProcess = false;

// BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
var CurrentRoleID = 0;
// ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

// BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-04-2020
var IsAbort = false;
// ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-04-2020

$(document).ready(function () {
    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 11-08-2020
    $('#NoteForRowClickID').hide();
    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 11-08-2020
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    //alert('1');
    //$("#DailyReceiptDetailsTable tr").click(function () {
    //    alert('2');
    //    var selected = $(this).hasClass("table_row_highlight");
    //    $("#DailyReceiptDetailsTable tr").removeClass("table_row_highlight");
    //    if (!selected)
    //        $(this).addClass("table_row_highlight");
    //});

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    $("#SearchBtn").click(function () {

        $('#InformationPanedID').html('');

        if ($.fn.DataTable.isDataTable("#DailyReceiptDetailsTable")) {
            $("#DailyReceiptDetailsTable").DataTable().clear().destroy();
        }

        SroID = $("#SROOfficeListID option:selected").val();

        var DailyReceiptRptTable = $('#DailyReceiptDetailsTable').DataTable({
            ajax: {
                url: '/MISReports/DataRestorationReport/LoadInitiateMasterTable',
                type: "POST",
                headers: header,
                data: {
                    'SroID': SroID
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
                                    //$("#PDFSPANID").html('');
                                    //$("#EXCELSPANID").html('');
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
                    // Added by SB on 22-3-2019 at 11:06 am
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
            //"scrollX": true,
            //"scrollY": true,
            "scrollY": "300px",
            "scrollCollapse": true,
            bPaginate: true,
            bLengthChange: true,
            // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
            // "pageLength": -1,
            //sScrollXInner: "150%",
            bInfo: true,
            info: true,
            bFilter: false,
            searching: true,
            // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
            // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
            //dom: 'lBfrtip',
            "destroy": true,
            "bAutoWidth": true,
            "bScrollAutoCss": true,
            //buttons: [
            //    {
            //        extend: 'pdf',
            //        text: '<i class="fa fa-file-pdf-o"></i> PDF',
            //        exportOptions: {
            //            columns: ':not(.no-print)'
            //        },
            //        action:
            //            function (e, dt, node, config) {
            //                //this.disable();
            //                window.location = '/MISReports/SaleDeedRevCollection/ExportReportToPDF?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
            //            }
            //    },
            //    {
            //        extend: 'excel',
            //        text: '<i class="fa fa-file-pdf-o"></i> Excel',
            //        exportOptions: {
            //            columns: ':not(.no-print)'
            //        },
            //        action:
            //            function (e, dt, node, config) {
            //                // BECAUSE OF BELOW CODE EXCEL BUTTON IS WORKING ONLY ONE TIME IF WE COMMENT 
            //                // BELOW this.disable(); so it will start working many times
            //                this.disable();
            //                window.location = '/MISReports/SaleDeedRevCollection/ExportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
            //            }
            //    }
            //],

            //columnDefs: [{
            //    targets: [5],
            //    render: $.fn.dataTable.render.number(',', '.', 2)
            //}],
            columnDefs: [
                //{ orderable: false, targets: [2] },
                //{ orderable: false, targets: [4] },
                //{
                //    orderable: false, targets: [5]

                //},
                //{ orderable: false, targets: [6] },
                //{ orderable: false, targets: [7] }

            ],
            //"language": {
            //    "decimal": ",",
            //    "thousands": "."
            //},
            columns: [

                { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
                { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
                { data: "InitiationDateTime", "searchable": true, "visible": true, "name": "InitiationDateTime" },
                {
                    data: "STATUS_DESCRIPTION", "searchable": true, "visible": true, "name": "STATUS_DESCRIPTION"
                    , render: function (data, type, row) {
                        //alert(data);
                        var textwithTooltip = "<div class='tooltip'>" + data + "<span class='tooltiptext'>Click here</span></div>";
                        //alert(textwithTooltip);
                        return textwithTooltip;
                    }
                },
                { data: "Is_Completed_STR", "searchable": true, "visible": true, "name": "Is_Completed_STR" },
                { data: "CompleteDateTime", "searchable": true, "visible": true, "name": "CompleteDateTime" },
                { data: "ConfirmDateTime", "searchable": true, "visible": true, "name": "ConfirmDateTime" },
                { data: "INIT_ID", "searchable": false, "visible": true, "name": "INIT_ID" },
                { data: "SroCode", "searchable": false, "visible": true, "name": "SroCode" },
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                { data: "AbortBtn", "searchable": false, "visible": true, "name": "AbortBtn" }
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
            ],
            fnInitComplete: function (oSettings, json) {
                //alert('in fnInitComplete');
                //$("#PDFSPANID").html(json.PDFDownloadBtn);
                //$("#EXCELSPANID").html(json.ExcelDownloadBtn);

                // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 8:00 PM
                $("#InitiateBTNForSR").html('');
                $("#InitiateBTNForSR").html(json.InitiateBTNForSR);

                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 31-12-2020
                // IF AND ELSE BLOCK IS ADDED IF THERE ARE NO ROWS IN DATA TABLE
                if (DailyReceiptRptTable.data().length > 0) {
                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 11-08-2020
                    $('#NoteForRowClickID').show();
                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 11-08-2020
                }
                else {
                    $('#NoteForRowClickID').hide();
                }
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 31-12-2020



                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

                // FOR SHOWING ABORT COLUMN TO ONLY SR AND HIDE FOR ALL OTHERS
                //alert('json.CurrentRoleID :'+json.CurrentRoleID);
                CurrentRoleID = json.CurrentRoleID
                //alert('CurrentRoleID :'+CurrentRoleID);
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                //alert(vSRRoleID == CurrentRoleID);
                // IF IS FOR SR ROLE TO DISPLAY ABORT COLUMN 
                if (vSRRoleID == CurrentRoleID) {
                    //alert('1 in if');
                    DailyReceiptRptTable.columns([9]).visible(true);
                    //$("td:nth-last-child(5)").css("text-align", "left");
                    //$("td:nth-last-child(7)").css("text-align", "left");
                }
                // ELSE IS FOR TECHADMIN AND AIGR COMP ROLE TO HIDE ABORT COLUMN 
                else {
                    //alert('1 in else');
                    DailyReceiptRptTable.columns([9]).visible(false);
                    //$("td:nth-last-child(4)").css("text-align", "left");
                    //$("td:nth-last-child(6)").css("text-align", "left");

                }
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
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
                unBlockUI();
                // // IF IS FOR SR ROLE TO DISPLAY ABORT COLUMN 
                //if (vSRRoleID == CurrentRoleID) {
                //    alert('in if');

                //    $("td:nth-last-child(5)").css("text-align", "left");
                //    $("td:nth-last-child(7)").css("text-align", "left");
                //}
                //// ELSE IS FOR TECHADMIN AND AIGR COMP ROLE TO HIDE ABORT COLUMN 
                //else {
                //    alert('in else');

                //    $("td:nth-last-child(4)").css("text-align", "left");
                //    $("td:nth-last-child(6)").css("text-align", "left");

                //}
            },
        });



        //tableIndexReports.columns.adjust().draw();
        //DailyReceiptRptTable.columns.adjust().draw();
        //alert('1');
        DailyReceiptRptTable.columns([7, 8]).visible(false);
        //alert('2');

        //// BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
        //alert(vSRRoleID == CurrentRoleID);
        //if (vSRRoleID == CurrentRoleID) {
        //    alert('in if');
        //    DailyReceiptRptTable.columns([9]).visible(true);
        //}
        //else {
        //    alert('in else');
        //    DailyReceiptRptTable.columns([9]).visible(false);

        //}
        //// ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

        $('#DailyReceiptDetailsTable tbody').off().on('click', 'tr', function () {

            //alert($('#DailyReceiptDetailsTable tbody').value);
            //alert('aaya');
            //alert('3');
            DailyReceiptRptTable.$('tr.selected').removeClass('selected');
            //alert('4');

            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            }

            else {
                DailyReceiptRptTable.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
            //alert('5');

            var CurrentIndex = DailyReceiptRptTable.row(this).index();
            //alert('6');
            // WORKING SOLUTION 1
            var DT_InitID = 0;
            var DT_SROCode = 0;
            //alert('7');
            DailyReceiptRptTable
                .column(7)
                .data()
                .each(function (value, index) {
                    if (index == CurrentIndex) {

                        DT_InitID = value;
                        //alert(DT_InitID);
                    }
                });
            //alert('8');
            DailyReceiptRptTable
                .column(8)
                .data()
                .each(function (value, index) {
                    if (index == CurrentIndex) {

                        DT_SROCode = value;
                        // ADDED BY SHUBHAM BHAGAT ON 05-08-2020
                        DT_SROCodeForDownloadFile = DT_SROCode;
                        //alert(DT_SROCode);
                    }
                });
            
            //alert('9');
            //alert("INIT_ID-" + DT_InitID + "Srocode-" + DT_SROCode);


            //SroID = $("#SROOfficeListID option:selected").val();
            // load partial view
            // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 31-12-2020
            if (DT_SROCode == 0 && DT_InitID == 0) { return; }
            // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 31-12-2020

            //alert('bhjvj');
            //alert(DT_InitID + "," + DT_SROCodeForDownloadFile + "," + DT_SROCode);
             // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 30-04-2020
            // FOR STOPING STATUS LOADING CALL IN CASE OF ABORT BUTTON CLICKED           
            if (IsAbort === true) { return; }
            // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 30-04-2020

            //alert("after");
            blockUI('Loading data please wait.');

            $.ajax({
                url: '/MISReports/DataRestorationReport/DataRestorationReportStatus',
                // BELOW CODE IS COMMENETED AND CHANGED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:00 PM
                //data: { "SroID": SroID },
                data: { "SroID": DT_SROCode, "INIT_ID": DT_InitID },

                datatype: "json",
                headers: header,
                type: "POST",
                success: function (data) {
                    //add validations
                    if (data.errorMessage == undefined) {
                        //alert('10');
                        $('#InformationPanedID').html('');
                        $('#InformationPanedID').html(data);
                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 11-08-2020
                        $('#NoteForRowClickID').hide();
                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 11-08-2020

                        //alert('11');
                        unBlockUI();
                    }
                    else if (data.serverError == true) {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            function () {
                                window.location.href = "/Home/HomePage"
                            });
                    }
                    else {
                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                    }
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

            // BELOW CODE STOPS CLICK EVENT AFTER WE CLICK ONE TIME
            //$("#DailyReceiptDetailsTable tbody").off("click");

            //alert('CurrentIndex-' + CurrentIndex);

            // WORKING SOLUTION 2
            //console.log(DailyReceiptRptTable.cells({ row: CurrentIndex, column: 7 }).data()[0]);

            // EXTRA
            //for (var i = 0; i < 10; i++) {
            //    console.log(DailyReceiptRptTable.cells({ row: i, column: 7 }).data()[0]);
            //}   

            // REMOVED SUPPORT BY JQUERY VERION
            //var position = DailyReceiptRptTable.fnGetPosition(this);
            //var hiddenColumnValue = DailyReceiptRptTable.fnGetData(position)[7];
            //alert(hiddenColumnValue);

            // WORKING SOLUTION 1
            //var DT_InitID = 0;
            //var DT_SROCode = 0;

            //DailyReceiptRptTable
            //    .column(7)
            //    .data()
            //    .each(function (value, index) {
            //        if (index == CurrentIndex) {

            //            DT_InitID = value;
            //            //alert(InitID);
            //        }
            //    });

            //DailyReceiptRptTable
            //    .column(8)
            //    .data()
            //    .each(function (value, index) {
            //        if (index == CurrentIndex) {

            //            DT_SROCode = value;
            //            //alert(InitID);
            //        }
            //    });

            //alert("INIT_ID-" + DT_InitID + "Srocode-" + DT_SROCode);




        });

    });


    // BELOW CODE COPIED AND COMMENETED BY SHUBHAM BHAGAT ON 24-07-2020 AT 12:40 PM
    // BELOW CODE IS BACKUP BEFORE CALLING DATATABLE METHOD 
    // BELOW CODE IS DIRECTLY CALLING FOR CURRENT PROCESS METHOD I.E. DataRestorationReportStatus
    //$("#SearchBtn").click(function () {

    //    if ($("#SROOfficeListID option:selected").val() == "0") {
    //        bootbox.alert("Please select SRO Name");
    //    }
    //    else {  //add block ui
    //        blockUI('Loading data please wait.');

    //        SroID = $("#SROOfficeListID option:selected").val();
    //        // load partial view
    //        $.ajax({
    //            url: '/MISReports/DataRestorationReport/DataRestorationReportStatus',
    //            data: { "SroID": SroID },
    //            datatype: "json",
    //            headers: header,
    //            type: "POST",
    //            success: function (data) {
    //                //add validations
    //                if (data.errorMessage == undefined) {
    //                    $('#InformationPanedID').html('');
    //                    $('#InformationPanedID').html(data);
    //                    unBlockUI();
    //                }
    //                else if (data.serverError == true) {
    //                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
    //                        function () {
    //                            window.location.href = "/Home/HomePage"
    //                        });
    //                }
    //                else {
    //                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
    //                }
    //            },
    //            error: function (xhr) {
    //                bootbox.alert({
    //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
    //                    callback: function () {
    //                    }
    //                });
    //                unBlockUI();
    //            }
    //        });

    //        //$.ajax({
    //        //    url: '/MISReports/DataRestorationReport/DataRestorationReportStatus',
    //        //    type: "POST",
    //        //    headers: header,
    //        //    data: { "SroID": SroID},
    //        //    datatype: "json",// it is required ot not doubt on 03-06-2020           
    //        //    success: function (data) {
    //        //        if (data.serverError == true) {
    //        //            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
    //        //                function () {
    //        //                    window.location.href = "/Home/HomePage"
    //        //                });
    //        //        }
    //        //        else {

    //        //        }
    //        //        unBlockUI();
    //        //    },
    //        //    error: function (xhr) {
    //        //        unBlockUI();
    //        //    }
    //        //});
    //    }

    //});

    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
    $("#btncloseAbortPopup").click(function () {
        $('#divViewAbortModal').modal('hide');
    });
    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020	

});

function InitiateNewProcess(SROCode) {
    bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
        //message: "<span class='boot-alert-txt'>This action will initiate process of data restoration from Central database to SR database. Are you sure ?</span>",
        message: "<span class='boot-alert-txt'>This action will initiate process of data restoration. Are you sure ?</span>",
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
        buttons: {
            cancel: {
                label: '<i class="fa fa-times"></i> No',
                className: 'pull-right margin-left-NoBtn'
            }, confirm: {
                label: '<i class="fa fa-check"></i> Yes'
            }
            //confirm: {
            //    label: 'Yes',
            //    className: 'btn-success'                
            //},
            //cancel: {
            //    label: 'No',
            //    className: 'btn-danger pull-right'
            //}
        },
        callback: function (result) {
            //console.log('This was logged in the callback: ' + result);
            if (result == true) {
                //alert('done');
                blockUI('Loading data please wait.');

                //SroID = $("#SROOfficeListID option:selected").val();
                // load partial view
                $.ajax({
                    url: '/MISReports/DataRestorationReport/DataRestorationReportStatus',
                    // BELOW CODE IS COMMENETED AND CHANGED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:00 PM
                    //data: { "SroID": SroID },
                    data: { "SroID": SROCode, "INIT_ID": "0" },
                    // INIT_ID IS SET HARDCODED BECAUSE IT IS CHECKED AT DAL LAYER FOR INITIATING NEW REQUEST

                    datatype: "json",
                    headers: header,
                    type: "POST",
                    success: function (data) {
                        //add validations
                        if (data.errorMessage == undefined) {
                            $('#InformationPanedID').html('');
                            $('#InformationPanedID').html(data);
                            // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 8:00 PM
                            // HIDE INITIATE BUTTON FOR SR IF CLICK IT TO INITIATE NEW DATABASE RESTORATION PROCESS
                            $("#InitiateBTNForSR").html('');
                            $("#InitiateBTNForSR").hide();
                            //$("#SearchBtn").trigger("click");

                            // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 7:25 PM
                            IsInitaiteNewProcess = true;

                            // ADDING BELOW CODE HERE GIVING ERROR BECAUSE CALL IS NOT GOING IN SEQUENCE
                            // TRING TO SHIFT AFTER PROCESS IS INITIATED
                            // ADDED BY SHUBHAM BHAGAT ON 31-07-2020 AT 12:25 PM
                            //$('#DailyReceiptDetailsTable').DataTable().draw();
                            //$("#SearchBtn").trigger("click");
                            unBlockUI();
                        }
                        else if (data.serverError == true) {
                            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                                function () {
                                    window.location.href = "/Home/HomePage"
                                });
                        }
                        else {
                            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
                        }
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
            else {
                return;
            }
        }
    });          
}


// BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
function AbortProcess(INIT_ID) {
    //alert('AbortProcess');
    IsAbort = true;
    $('#divLoadAbortView').load('/MISReports/DataRestorationReport/AbortView',
        { INIT_ID: INIT_ID }, function () {

            $('#divViewAbortModal').modal('show');

        }

    );
}
// ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

//BELOW CODE ADDED BY SHUBHAM BHAGAT ON 06-05-2021 ADD BUTTON TO GENERATE SCRIPT
function GenerateScriptBySR(sroID, InitID) {
    blockUI('Loading data please wait.');

    $.ajax({
        url: '/MISReports/DataRestorationReport/DataRestorationReportStatusForScript',
        // BELOW CODE IS COMMENETED AND CHANGED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:00 PM
        //data: { "SroID": SroID },
        data: { "SroID": sroID, "INIT_ID": InitID },

        datatype: "json",
        headers: header,
        type: "POST",
        success: function (data) {
            //add validations
            if (data.errorMessage == undefined) {
                //alert('10');
                $('#InformationPanedID').html('');
                $('#InformationPanedID').html(data);
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 11-08-2020
                $('#NoteForRowClickID').hide();
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 11-08-2020

                //alert('11');
                unBlockUI();
            }
            else if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>');
            }
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
//ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 06-05-2021 ADD BUTTON TO GENERATE SCRIPT