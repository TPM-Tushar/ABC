
//Global variables.
var token = '';
var header = {};
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DRODropDownListID').hide();
    $('#SRODropDownListID').hide();
    
    var vIsDro = IsDro.toString();
    var iIsDro = vIsDro == 'True' ? 1 : 0;
    if (vIsDro == 'True') {
        
        $("input[name=DRO]").prop("checked", true).trigger("click");
        $('#DRODropDownListID').show();
    }
    else {
        $("input[name=SRO]").prop("checked", true).trigger("click");
        $('#SRODropDownListID').show();
    }

    $('#CheckBox').hide();


    $("input[name=DRO]")
        .on("click", EventOnClickOfDroRadioButton);
    $("input[name=SRO]")
        .on("click", EventOnClickOfSroRadioButton);

    function EventOnClickOfDroRadioButton(event) {

        document.getElementById('SROID').checked = false;
        $('#SRODropDownListID').hide();
        $('#DRODropDownListID').show();
        iIsDro = 1;

    }
    function EventOnClickOfSroRadioButton(event) {

        document.getElementById('DROID').checked = false;
        $('#DRODropDownListID').hide();
        $('#SRODropDownListID').show();
        iIsDro = 0;
    }

    $('#IsActiveId').change(function () {
        if ($('#IsActiveId').prop("checked") == true) {

            $('#SRODropDownList').hide();
        }
        if ($('#IsActiveId').prop("checked") == false) {
            $('#SRODropDownList').show();

        }
    });

    $("#DtlsSearchParaListCollapse").click(function () {
        var classToRemoveSearchPara = $('#DtlsToggleIconSearchPara').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#DtlsToggleIconSearchPara').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

            $('#DtlsToggleIconSearchPara').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

    });

    $("#RemittDetailsListCollapse").click(function () {
        var classToRemoveSearchPara = $('#RemittToggleIcon').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#RemittToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

            $('#RemittToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

    });

    $("#ChallanDetailsTableListCollapse").click(function () {
        var classToRemoveSearchPara = $('#ChallanToggleIcon').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#ChallanToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

            $('#ChallanToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

    });

    $("#VerificationDetailsTableListCollapse").click(function () {
        var classToRemoveSearchPara = $('#VerificationToggleIcon').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#VerificationToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

            $('#VerificationToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

    });


    $("#ChallanDetailsTransTableListCollapse").click(function () {
        var classToRemoveSearchPara = $('#ChallanTransDetailsToggleIcon').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#ChallanTransDetailsToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

            $('#ChallanTransDetailsToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

    });


    $("#BankTransactionAmountDetailsParaListCollapse").click(function () {
        var classToRemoveSearchPara = $('#BankTransactionAmountDeatilsToggleIcon').attr('class');

        if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
            $('#BankTransactionAmountDeatilsToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-plus-square-o fa-pull-left fa-2x");

        else if (classToRemoveSearchPara == "fa fa-plus-square-o fa-pull-left fa-2x")

            $('#BankTransactionAmountDeatilsToggleIcon').removeClass(classToRemoveSearchPara).addClass("fa fa-minus-square-o fa-pull-left fa-2x");

    });






    $("#btnSearch").click(function () {
        if ($("#SROOfficeListID option:selected").val() === "0" && document.getElementById('SROID').checked) {
            bootbox.alert("Please select SR Office", function () {
                var classToRemoveSearchPara = $('#DtlsToggleIconSearchPara').attr('class');
                if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
                    $('#DtlsSearchParaListCollapse').trigger('click');
            });
        }
        else if ($("#DROOfficeListID option:selected").val() === "0" && document.getElementById('DROID').checked) {
            bootbox.alert("Please select DR Office", function () {
                var classToRemoveSearchPara = $('#DtlsToggleIconSearchPara').attr('class');
                if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x")
                    $('#DtlsSearchParaListCollapse').trigger('click');
            });
        }
        else {
            var fromDate = $("#txtFromDate").val();
            var ToDate = $("#txtToDate").val();
            var DROOfficeListID = $("#DROOfficeListID option:selected").val();
            var SROOfficeListID = $("#SROOfficeListID option:selected").val();
            var IsActiveId = document.getElementById("IsActiveId").checked;
            var responsiveHelper;

            var BankTransactionDetailsTable = $('#StatTableDataID');
            var BankTransactionAmountDetailsTable = $('#BankTransactionAmountDetailsTableID');
            var DoubleVerificationDetailsTable = $('#DoubleVerificationDetailsTableID');
            var remittanceDetailsTable = $('#RemittanceDetailsTableID')
            var ChallanDetailsTable = $('#ChallanDetailsTableID');
            var ChallanDetailsTransTable = $('#ChallanTransDetailsTableID')
            var TransactionStatus = $("#TransactionStatusListID option:selected").val();


            if ($.fn.DataTable.isDataTable("#DoubleVerificationDetailsTableID")) {
                DoubleVerificationDetailsTable.dataTable().fnDestroy();

                var classToRemoveSearchPara = $('#VerificationToggleIcon').attr('class');

                if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x") {
                    $("#VerificationDetailsTableListCollapse").trigger('click');


                }

            }
            if ($.fn.DataTable.isDataTable("#RemittanceDetailsTableID")) {
                remittanceDetailsTable.dataTable().fnDestroy();

                var classToRemoveSearchPara = $('#RemittToggleIcon').attr('class');

                if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x") {
                    $("#RemittDetailsListCollapse").trigger('click');
                }


            }
            if ($.fn.DataTable.isDataTable("#ChallanDetailsTableID")) {
                ChallanDetailsTable.dataTable().fnDestroy();

                var classToRemoveSearchPara = $('#ChallanToggleIcon').attr('class');

                if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x") {


                    $("#ChallanDetailsTableListCollapse").trigger('click');
                }


            }

            if ($.fn.DataTable.isDataTable("#ChallanTransDetailsTableID")) {
                ChallanDetailsTransTable.dataTable().fnDestroy();

                var classToRemoveSearchPara = $('#ChallanTransDetailsToggleIcon').attr('class');

                if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x") {

                    $("#ChallanDetailsTransTableListCollapse").trigger('click');

                }


            }
            if ($.fn.DataTable.isDataTable("#StatTableDataID")) {

                //  BankTransactionDetailsTable.dataTable().fnDestroy();

            }

            if ($.fn.DataTable.isDataTable("#BankTransactionAmountDetailsTableID")) {
                $("#BankTransactionAmountDetailsTableID").dataTable().fnDestroy();
                var classToRemoveSearchPara = $('#BankTransactionAmountDeatilsToggleIcon').attr('class');

                if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x") {
                    $("#BankTransactionAmountDetailsParaListCollapse").trigger('click');

                }

            }

            //OnClick of Search BankTransactionDetails Should get populated
            var tableBankTransactiontails = $('#StatTableDataID').DataTable({

                ajax: {
                    url: '/Remittance/REMDaignostics/GetBankTransactionDetailsList',
                    type: "POST",
                    headers: header,

                    data: {
                        'fromDate': fromDate, 'ToDate': ToDate, 'DROOfficeListID': DROOfficeListID, 'IsActiveId': IsActiveId, 'SROOfficeListID': SROOfficeListID, 'TransactionStatus': TransactionStatus, 'IsDro': iIsDro
                    },
                    dataSrc: function (json) {
                        if ($("#DtlsSearchParaListCollapse").hasClass("collapsed")) {
                            $("#DtlsSearchParaListCollapse").trigger('click');
                        }
                        if (json.errorMessage != null) {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                callback: function () {
                                    $("#DtlsSearchParaListCollapse").trigger('click');
                                }
                            });
                        }

                        unBlockUI();
                        return json.data;
                    },
                    error: function () {
                        unBlockUI();
                        //window.location.href = '/Error/SessionExpire';
                    },
                    beforeSend: function () {
                        blockUI('loading data.. please wait...');
                        // Added by SB on 22-3-2019 at 11:06 am
                        var searchString = $('#StatTableDataID_filter input').val();

                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;

                            if (!regexToMatch.test(searchString)) {
                                unBlockUI();
                                $("#StatTableDataID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    //    $('#menuDetailsListTable_filter input').val('');

                                    tableBankTransactiontails.search('').draw();
                                    $("#StatTableDataID_filter input").prop("disabled", false);

                                });
                                return false;
                            }
                        }
                    }
                },

                bAutoWidth: false,
                serverSide: true,

                "scrollX": true,
                "scrollY": "30vh",
                scrollCollapse: true,
                bPaginate: true,
                bLengthChange: true,
                bInfo: true,
                info: true,
                bFilter: false,
                searching: true,
                dom: 'lBfrtip',
                "destroy": true,

                buttons: [
                    {

                    }
                ],

                columnDefs: [
                    { orderable: false, targets: [1] },
                    { orderable: false, targets: [2] },
                    { orderable: false, targets: [3] },
                    { orderable: false, targets: [4] },
                    { orderable: false, targets: [5] },
                    { orderable: false, targets: [6] },
                    { orderable: false, targets: [7] },
                    { orderable: false, targets: [9] },
                    { orderable: false, targets: [10] },
                    { orderable: false, targets: [11] },
                    { orderable: false, targets: [12] },
                    { orderable: false, targets: [13] },
                    { orderable: false, targets: [14] },
                    { orderable: false, targets: [15] },
                    { orderable: false, targets: [16] },
                    { orderable: false, targets: [17] },
                    { orderable: false, targets: [18] },
                    { orderable: false, targets: [19] },

                ],
                columns: [
                    { data: "TransactionID", "searchable": true, "visible": true, "name": "TransactionID" },
                    { data: "InstrumentBankIFSCCode", "searchable": true, "visible": true },
                    { data: "InstrumentBankMICRCode", "searchable": true, "visible": true },
                    { data: "InstrumentNumber", "searchable": true, "visible": true },
                    { data: "SROCode", "searchable": true, "visible": true },
                    { data: "IsReceipt", "searchable": true, "visible": true },
                    { data: "DocumentID", "searchable": true, "visible": true },
                    { data: "DateOfUpdate", "searchable": true, "visible": true },
                    { data: "Receipt_StampDate", "searchable": true, "visible": true, "name": "Receipt_StampDate" },
                    { data: "TotalAmount", "searchable": true, "visible": true },
                    { data: "InstrumentDate", "searchable": true, "visible": true },
                    { data: "ReceiptID", "searchable": true, "visible": true },
                    { data: "StampDetailsID", "searchable": true, "visible": true },
                    { data: "StampTypeID", "searchable": true, "visible": true },
                    { data: "ReceiptPaymentMode", "searchable": true, "visible": true },
                    { data: "ReceiptNumber", "searchable": true, "visible": true },
                    { data: "InstrumentBankName", "searchable": true, "visible": true },
                    { data: "SourceOfReceipt", "searchable": true, "visible": true },
                    { data: "DROCode", "searchable": true, "visible": true },
                    { data: "IsDRO", "searchable": true, "visible": true },
                    { data: "InsertDateTime", "searchable": true, "visible": true, "name": "InsertDateTime" },

                ],

                autoWidth: true,
                preDrawCallback: function () {

                    unBlockUI();
                },
                fnRowCallback: function (nRow, aData, iDisplayIndex) {


                    unBlockUI();
                    return nRow;
                },
                drawCallback: function (oSettings) {
                    unBlockUI();
                },
            });

            $(window).on('resize', function () {
                table.fnAdjustColumnSizing();
            });

            //onclick of any row on BankTransactionDetails table ,BankTransactionAmountDetails and RemittanceDetails tables should get populated
            $('#StatTableDataID tbody').on('click', 'tr', function () {


                if ($.fn.DataTable.isDataTable("#ChallanDetailsTableID")) {

                    ChallanDetailsTable.dataTable().fnDestroy();
                    var classToRemoveSearchPara = $('#ChallanToggleIcon').attr('class');

                    if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x") {
                        $("#ChallanDetailsTableListCollapse").trigger('click');

                    }

                }

                if ($.fn.DataTable.isDataTable("#ChallanTransDetailsTableID")) {
                    ChallanDetailsTransTable.dataTable().fnDestroy();
                    var classToRemoveSearchPara = $('#ChallanTransDetailsToggleIcon').attr('class');

                    if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x") {
                        $("#ChallanDetailsTransTableListCollapse").trigger('click');

                    }
                }

                if ($.fn.DataTable.isDataTable("#DoubleVerificationDetailsTableID")) {

                    DoubleVerificationDetailsTable.dataTable().fnDestroy();
                    var classToRemoveSearchPara = $('#VerificationToggleIcon').attr('class');

                    if (classToRemoveSearchPara == "fa fa-minus-square-o fa-pull-left fa-2x") {
                        $("#VerificationDetailsTableListCollapse").trigger('click');
                    }
                }
                tableBankTransactiontails.$('tr.selected').removeClass('selected');
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    tableBankTransactiontails.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }

                var CurrentIndex = tableBankTransactiontails.row(this).index();
                //var value = $(this).children('td:first').text();

                tableBankTransactiontails
                    .column(0)
                    .data()
                    .each(function (value, index) {
                        if (index == CurrentIndex) {

                            var TransactionId = value;


                            var BankTransactionAmountDetails = $('#BankTransactionAmountDetailsTableID').DataTable({
                                ajax: {
                                    url: '/Remittance/REMDaignostics/GetBankTransactionAmountDetailsList',
                                    type: "POST",
                                    headers: header,

                                    data: {
                                        'fromDate': fromDate, 'ToDate': ToDate, 'DROOfficeListID': DROOfficeListID, 'IsActiveId': IsActiveId, 'SROOfficeListID': SROOfficeListID, 'TransactionID': TransactionId,
                                    },
                                    dataSrc: function (json) {
                                        if ($("#BankTransactionAmountDetailsParaListCollapse").hasClass("collapsed")) {
                                            $("#BankTransactionAmountDetailsParaListCollapse").trigger('click');
                                        }
                                        //if (json.recordsFiltered == 0) {
                                        //    unBlockUI();
                                        //bootbox.alert({
                                        //    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                        //    callback: function () {
                                        //        $("#BankTransactionAmountDetailsParaListCollapse").trigger('click');


                                        //    }
                                        //});
                                        //}
                                        //else {
                                        unBlockUI();
                                        return json.data;
                                        // }
                                    },
                                    error: function () {
                                        unBlockUI();
                                        //window.location.href = '/Error/SessionExpire';
                                    },
                                    beforeSend: function () {
                                        blockUI('loading data.. please wait...');
                                        // Added by SB on 22-3-2019 at 11:06 am
                                        var searchString = $('#BankTransactionAmountDetailsTableID_filter input').val();
                                        if (searchString != "") {
                                            var regexToMatch = /^[^<>]+$/;

                                            if (!regexToMatch.test(searchString)) {
                                                unBlockUI();
                                                $("#BankTransactionAmountDetailsTableID_filter input").prop("disabled", true);
                                                bootbox.alert('Please enter valid Search String ', function () {
                                                    //    $('#menuDetailsListTable_filter input').val('');

                                                    BankTransactionAmountDetails.search('').draw();
                                                    $("#BankTransactionAmountDetailsTableID_filter input").prop("disabled", false);
                                                });
                                                return false;
                                            }
                                        }
                                    }
                                },

                                bAutoWidth: false,
                                serverSide: true,
                                "scrollX": true,
                                "scrollY": "30vh",
                                scrollCollapse: true,
                                bPaginate: true,
                                bLengthChange: true,
                                bInfo: true,
                                info: true,
                                bFilter: false,
                                searching: true,
                                dom: 'lBfrtip',
                                "destroy": true,


                                buttons: [
                                    {

                                    }
                                ],

                                columnDefs: [
                                    { orderable: false, targets: [0] },
                                    { orderable: false, targets: [1] },
                                    { orderable: false, targets: [2] },
                                    { orderable: false, targets: [3] },
                                    { orderable: false, targets: [4] },
                                    { orderable: false, targets: [5] },
                                ],

                                columns: [
                                    { data: "ID", "searchable": true, "visible": true, "name": "ID" },
                                    { data: "TransactionID", "searchable": true, "visible": true },
                                    { data: "Amount", "searchable": true, "visible": true },
                                    { data: "FeesRuleCode", "searchable": true, "visible": true },
                                    { data: "SROCode", "searchable": true, "visible": true },
                                    { data: "DeptSubPurpooseID", "searchable": true, "visible": true },
                                    { data: "InsertDateTime", "searchable": true, "visible": true, "name": "InsertDateTime" },

                                ],

                                autoWidth: true,
                                preDrawCallback: function () {

                                    unBlockUI();
                                },
                                fnRowCallback: function (nRow, aData, iDisplayIndex) {


                                    unBlockUI();
                                    return nRow;
                                },
                                drawCallback: function (oSettings) {
                                    unBlockUI();
                                },
                            });

                            var tableRemittanceDetails = remittanceDetailsTable.DataTable({

                                ajax: {
                                    url: '/Remittance/REMDaignostics/GetRemittanceDetailsList',
                                    type: "POST",
                                    headers: header,

                                    data: {
                                        'fromDate': fromDate, 'ToDate': ToDate, 'DROOfficeListID': DROOfficeListID, 'IsActiveId': IsActiveId, 'SROOfficeListID': SROOfficeListID, 'TransactionID': TransactionId,
                                    },
                                    dataSrc: function (json) {

                                        if ($("#RemittDetailsListCollapse").hasClass("collapsed")) {
                                            $("#RemittDetailsListCollapse").trigger('click');
                                        }
                                        //if (json.recordsFiltered == 0) {
                                        //    unBlockUI();
                                        //bootbox.alert({
                                        //    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                        //    callback: function () {
                                        //        $("#RemittDetailsListCollapse").trigger('click');

                                        //    }
                                        //});
                                        //}
                                        //else {
                                        unBlockUI();
                                        return json.data;
                                        // }
                                    },
                                    error: function () {
                                        unBlockUI();
                                        //window.location.href = '/Error/SessionExpire';
                                    },
                                    beforeSend: function () {
                                        blockUI('loading data.. please wait...');
                                        // Added by SB on 22-3-2019 at 11:06 am
                                        var searchString = $('#RemittanceDetailsTableID_filter input').val();
                                        if (searchString != "") {
                                            var regexToMatch = /^[^<>]+$/;

                                            if (!regexToMatch.test(searchString)) {
                                                unBlockUI();
                                                $("#RemittanceDetailsTableID_filter input").prop("disabled", true);
                                                bootbox.alert('Please enter valid Search String ', function () {
                                                    tableRemittanceDetails.search('').draw();
                                                    $("#RemittanceDetailsTableID_filter input").prop("disabled", false);
                                                });
                                                return false;
                                            }
                                        }
                                    }
                                },

                                serverSide: true,
                                "scrollX": true,
                                "scrollY": "250px",
                                scrollCollapse: true,
                                bPaginate: true,
                                bLengthChange: true,
                                bInfo: true,
                                info: true,
                                bFilter: false,
                                searching: true,
                                dom: 'lBfrtip',
                                "destroy": true,


                                buttons: [
                                    {

                                    }


                                ],

                                columnDefs: [
                                    { orderable: false, targets: [0] },
                                    { orderable: false, targets: [1] },
                                    { orderable: false, targets: [2] },
                                    { orderable: false, targets: [3] },
                                    { orderable: false, targets: [4] },
                                    { orderable: false, targets: [5] },
                                    { orderable: false, targets: [6] },
                                    { orderable: false, targets: [7] },
                                    { orderable: false, targets: [8] },
                                    { orderable: false, targets: [9] },
                                    { orderable: false, targets: [10] },
                                    { orderable: false, targets: [11] },
                                    { orderable: false, targets: [12] },
                                ],

                                columns: [
                                    { data: "ID", "searchable": true, "visible": true, "name": "ID" },
                                    { data: "TransactionID", "searchable": true, "visible": true },
                                    { data: "RemitterName", "searchable": true, "visible": true },
                                    { data: "DeptReferenceCode", "searchable": true, "visible": true },
                                    { data: "UIRNumber", "searchable": true, "visible": true },
                                    { data: "StatusCode", "searchable": true, "visible": true },
                                    { data: "StatusDesc", "searchable": true, "visible": true },
                                    { data: "TransactionStatus", "searchable": true, "visible": true },
                                    { data: "TransactionDateTime", "searchable": true, "visible": true },
                                    { data: "UserID", "searchable": true, "visible": true },
                                    { data: "IPAdd", "searchable": true, "visible": true },
                                    { data: "PaymentStatusCode", "searchable": true, "visible": true },
                                    { data: "DDOCode", "searchable": true, "visible": true },
                                    { data: "InsertDateTime", "searchable": true, "visible": true, "name": "InsertDateTime" },



                                ],
                                autoWidth: true,
                                preDrawCallback: function () {

                                    unBlockUI();
                                },
                                fnRowCallback: function (nRow, aData, iDisplayIndex) {

                                    unBlockUI();
                                    return nRow;
                                },
                                drawCallback: function (oSettings) {

                                    unBlockUI();
                                },
                                fnInitComplete: function (oSettings, json) {
                                    unBlockUI();


                                    //if (json.status == "0") {
                                    //    bootbox.alert({
                                    //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                    //        callback: function () {
                                    //        }
                                    //    });
                                    //}

                                    $('.dataTables_scrollBody thead tr').css({ visibility: 'collapse' });
                                }
                            });
                            $('.dt-button').hide();  //For Hiding Empty buttons on datatable

                            //onclick of any row on RemittanceDetailsTable ChallanDetails and ChallanDetailsTransactionDetails tables should get populated
                            $('#RemittanceDetailsTableID tbody').on('click', 'tr', function () {

                                tableRemittanceDetails.$('tr.selected').removeClass('selected');
                                if ($(this).hasClass('selected')) {
                                    $(this).removeClass('selected');
                                }
                                else {
                                    tableRemittanceDetails.$('tr.selected').removeClass('selected');
                                    $(this).addClass('selected');
                                }

                                var CurrentIndex = tableRemittanceDetails.row(this).index();
                                tableRemittanceDetails
                                    .column(3)//for DepartmentReferenceNumber Column
                                    .data()
                                    .each(function (value, index) {
                                        if (index == CurrentIndex) {

                                            var DepartmentReferenceNumber = value;
                                            var tableChallanDetails = ChallanDetailsTable.DataTable({

                                                ajax: {
                                                    url: '/Remittance/REMDaignostics/GetChallanDetailsList',
                                                    type: "POST",
                                                    headers: header,

                                                    data: {
                                                        'fromDate': fromDate, 'ToDate': ToDate, 'DROOfficeListID': DROOfficeListID, 'IsActiveId': IsActiveId, 'SROOfficeListID': SROOfficeListID, 'DepartmentReferenceNumber': DepartmentReferenceNumber,
                                                    },

                                                    dataSrc: function (json) {

                                                        if ($("#ChallanDetailsTableListCollapse").hasClass("collapsed")) {
                                                            $("#ChallanDetailsTableListCollapse").trigger('click');
                                                        }
                                                        //if (json.recordsFiltered == 0) {
                                                        //    unBlockUI();
                                                        //    bootbox.alert({
                                                        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                                        //        callback: function () {
                                                        //            $("#ChallanDetailsTableListCollapse").trigger('click');

                                                        //        }
                                                        //    });
                                                        //}
                                                        //else {
                                                        unBlockUI();
                                                        return json.data;
                                                        //}
                                                    },
                                                    error: function () {
                                                        unBlockUI();
                                                        // refreshCaptcha();
                                                        //window.location.href = '/Error/SessionExpire';
                                                    },
                                                    beforeSend: function () {
                                                        blockUI('loading data.. please wait...');
                                                        // Added by SB on 22-3-2019 at 11:06 am
                                                        var searchString = $('#ChallanDetailsTableID_filter input').val();
                                                        if (searchString != "") {
                                                            var regexToMatch = /^[^<>]+$/;

                                                            if (!regexToMatch.test(searchString)) {
                                                                unBlockUI();
                                                                $("#ChallanDetailsTableID_filter input").prop("disabled", true);
                                                                bootbox.alert('Please enter valid Search String ', function () {
                                                                    tableChallanDetails.search('').draw();
                                                                    $("#ChallanDetailsTableID_filter input").prop("disabled", false);
                                                                });
                                                                return false;
                                                            }
                                                        }
                                                    }
                                                },


                                                language: { search: "Search" },
                                                lengthChange: true,
                                                serverSide: true,
                                                "scrollX": true,
                                                "scrollY": "250px",
                                                scrollCollapse: true,
                                                bPaginate: true,
                                                bLengthChange: true,
                                                bInfo: true,
                                                info: true,
                                                bFilter: false,
                                                searching: true,
                                                dom: 'lBfrtip',
                                                "destroy": true,
                                                buttons: [
                                                    {

                                                    }


                                                ],
                                                columnDefs: [
                                                    { orderable: false, targets: [0] },
                                                    { orderable: false, targets: [1] },
                                                    { orderable: false, targets: [2] },
                                                    { orderable: false, targets: [3] },
                                                    { orderable: false, targets: [4] },
                                                    { orderable: false, targets: [5] },
                                                    { orderable: false, targets: [6] },
                                                    { orderable: false, targets: [7] },
                                                    { orderable: false, targets: [8] },
                                                    { orderable: false, targets: [9] },
                                                    { orderable: false, targets: [10] },
                                                    { orderable: false, targets: [11] },
                                                    { orderable: false, targets: [12] },
                                                    { orderable: false, targets: [13] },
                                                    { orderable: false, targets: [14] },
                                                    { orderable: false, targets: [15] },
                                                    { orderable: false, targets: [16] },
                                                ],
                                                columns: [
                                                    { data: "ChallanID", "searchable": true, "visible": true, "name": "ChallanID" },
                                                    { data: "ChallanRequestID", "searchable": true, "visible": true },
                                                    { data: "BatchID", "searchable": true, "visible": true },
                                                    { data: "ChallanDate", "searchable": true, "visible": true },
                                                    { data: "ChallanExpiryDate", "searchable": true, "visible": true },
                                                    { data: "ChallanTotalAmount", "searchable": true, "visible": true },
                                                    { data: "PaymentMode", "searchable": true, "visible": true },
                                                    { data: "ChallanAmount", "searchable": true, "visible": true },
                                                    { data: "CCNumber", "searchable": true, "visible": true },
                                                    { data: "CardType", "searchable": true, "visible": true },
                                                    { data: "InstrmntDate", "searchable": true, "visible": true },
                                                    { data: "InstrmntNumber", "searchable": true, "visible": true },
                                                    { data: "MICRCode", "searchable": true, "visible": true },
                                                    { data: "RemitterName", "searchable": true, "visible": true },
                                                    { data: "RmtncAgencyBank", "searchable": true, "visible": true },
                                                    { data: "ChallanRefNum", "searchable": true, "visible": true },
                                                    { data: "DepartmentRefNumber", "searchable": true, "visible": true },
                                                    { data: "InsertDateTime", "searchable": true, "visible": true, "name": "InsertDateTime" },

                                                ],
                                                autoWidth: true,
                                                preDrawCallback: function () {

                                                    unBlockUI();
                                                },
                                                fnRowCallback: function (nRow, aData, iDisplayIndex) {

                                                    unBlockUI();
                                                    return nRow;
                                                },
                                                drawCallback: function (oSettings) {
                                                    unBlockUI();
                                                },
                                                fnInitComplete: function (oSettings, json) {
                                                    unBlockUI();
                                                    //if (json.status == "0") {
                                                    //    bootbox.alert({
                                                    //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                                    //        callback: function () {
                                                    //        }
                                                    //    });
                                                    //}

                                                    $('.dataTables_scrollBody thead tr').css({ visibility: 'collapse' });
                                                }
                                            });

                                            $('.dt-button').hide();  //For Hiding Empty buttons on datatable
                                            //onclick of any row on ChallanDetails Table ,DoubleVerificationDetails table should get populated
                                            $('#ChallanDetailsTableID tbody').on('click', 'tr', function () {
                                                tableChallanDetails.$('tr.selected').removeClass('selected');
                                                if ($(this).hasClass('selected')) {
                                                    $(this).removeClass('selected');
                                                }
                                                else {
                                                    tableChallanDetails.$('tr.selected').removeClass('selected');
                                                    $(this).addClass('selected');
                                                }
                                                var CurrentIndex = tableChallanDetails.row(this).index();

                                                tableChallanDetails
                                                    .column(15)
                                                    .data()
                                                    .each(function (value, index) {
                                                        if (index == CurrentIndex) {

                                                            var ChallanRefNumber = value;
                                                            $('.dt-button').hide();  //For Hiding Empty buttons on datatable

                                                            var tableDoubleVerificationDetails = DoubleVerificationDetailsTable.DataTable({

                                                                ajax: {
                                                                    url: '/Remittance/REMDaignostics/GetDoubleVerificationDetailsList',
                                                                    type: "POST",
                                                                    headers: header,

                                                                    data: {
                                                                        'fromDate': fromDate, 'ToDate': ToDate, 'DROOfficeListID': DROOfficeListID, 'IsActiveId': IsActiveId, 'SROOfficeListID': SROOfficeListID, 'ChallanRefNumber': ChallanRefNumber,
                                                                    },

                                                                    dataSrc: function (json) {

                                                                        if ($("#VerificationDetailsTableListCollapse").hasClass("collapsed")) {
                                                                            $("#VerificationDetailsTableListCollapse").trigger('click');
                                                                        }

                                                                        unBlockUI();
                                                                        return json.data;

                                                                    },

                                                                    error: function () {
                                                                        unBlockUI();
                                                                        //window.location.href = '/Error/SessionExpire';
                                                                    },
                                                                    beforeSend: function () {
                                                                        blockUI('loading data.. please wait...');
                                                                        // Added by SB on 22-3-2019 at 11:06 am
                                                                        var searchString = $('#DoubleVerificationDetailsTableID_filter input').val();
                                                                        if (searchString != "") {
                                                                            var regexToMatch = /^[^<>]+$/;

                                                                            if (!regexToMatch.test(searchString)) {
                                                                                unBlockUI();
                                                                                $("#DoubleVerificationDetailsTableID_filter input").prop("disabled", true);
                                                                                bootbox.alert('Please enter valid Search String ', function () {
                                                                                    tableDoubleVerificationDetails.search('').draw();
                                                                                    $("#DoubleVerificationDetailsTableID_filter input").prop("disabled", false);
                                                                                });
                                                                                return false;
                                                                            }
                                                                        }
                                                                    }
                                                                },
                                                                serverSide: true,
                                                                "scrollX": true,
                                                                "scrollY": "250px",
                                                                scrollCollapse: true,
                                                                bPaginate: true,
                                                                bLengthChange: true,
                                                                bInfo: true,
                                                                info: true,
                                                                bFilter: false,
                                                                searching: true,
                                                                dom: 'lBfrtip',
                                                                "destroy": true,

                                                                buttons: [
                                                                    {

                                                                    }


                                                                ],
                                                                columnDefs: [
                                                                    { orderable: false, targets: [0] },
                                                                    { orderable: false, targets: [1] },
                                                                    { orderable: false, targets: [2] },
                                                                    { orderable: false, targets: [3] },
                                                                    { orderable: false, targets: [4] },
                                                                    { orderable: false, targets: [5] },
                                                                    { orderable: false, targets: [6] },
                                                                    { orderable: false, targets: [7] },
                                                                    { orderable: false, targets: [8] },
                                                                    { orderable: false, targets: [9] },
                                                                    { orderable: false, targets: [10] },
                                                                    { orderable: false, targets: [11] },
                                                                    { orderable: false, targets: [12] },
                                                                    { orderable: false, targets: [13] },

                                                                ],

                                                                columns: [

                                                                    { data: "ID", "searchable": true, "visible": true, "name": "ID" },
                                                                    { data: "ChallanRefNumber", "searchable": true, "visible": true },
                                                                    { data: "BankTransactionNumber", "searchable": true, "visible": true },
                                                                    { data: "BankName", "searchable": true, "visible": true },
                                                                    { data: "PaymentMode", "searchable": true, "visible": true },
                                                                    { data: "PaymentStatusCode", "searchable": true, "visible": true },
                                                                    { data: "PaidAmount", "searchable": true, "visible": true },
                                                                    { data: "TransactionTimeStamp", "searchable": true, "visible": true },
                                                                    { data: "UserID", "searchable": true, "visible": true },
                                                                    { data: "IPAdd", "searchable": true, "visible": true },
                                                                    { data: "TransactionID", "searchable": true, "visible": true },
                                                                    { data: "ServiceStatusCode", "searchable": true, "visible": true },
                                                                    { data: "ServiceStatusDesc", "searchable": true, "visible": true },
                                                                    { data: "SchedulerID", "searchable": true, "visible": true },
                                                                    { data: "InsertDateTime", "searchable": true, "visible": true, "name": "InsertDateTime" },

                                                                ],
                                                                autoWidth: true,
                                                                preDrawCallback: function () {

                                                                    unBlockUI();
                                                                },
                                                                fnRowCallback: function (nRow, aData, iDisplayIndex) {

                                                                    unBlockUI();
                                                                    return nRow;
                                                                },
                                                                drawCallback: function (oSettings) {
                                                                    unBlockUI();
                                                                },
                                                                fnInitComplete: function (oSettings, json) {
                                                                    unBlockUI();
                                                                    //if (json.status == "0") {
                                                                    //    bootbox.alert({
                                                                    //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                                                    //        callback: function () {

                                                                    //        }
                                                                    //    });
                                                                    //}

                                                                    //$(".dt-button").addClass("btn btn-primary dt-button buttons-pdf buttons-html5");
                                                                    $('.dataTables_scrollBody thead tr').css({ visibility: 'collapse' });
                                                                }
                                                            });


                                                            $('.dt-button').hide();  //For Hiding Empty buttons on datatable



                                                        }
                                                    });

                                            });


                                            $('.dt-button').hide();  //For Hiding Empty buttons on datatable


                                            var TableChallanDetailsTrans = ChallanDetailsTransTable.DataTable({

                                                ajax: {
                                                    url: '/Remittance/REMDaignostics/GetChallanMatrixTransactionDetails',
                                                    type: "POST",
                                                    headers: header,

                                                    data: {
                                                        'fromDate': fromDate, 'ToDate': ToDate, 'DROOfficeListID': DROOfficeListID, 'IsActiveId': IsActiveId, 'SROOfficeListID': SROOfficeListID, 'DepartmentReferenceNumber': DepartmentReferenceNumber, 'IsDro': iIsDro
                                                    },
                                                    dataSrc: function (json) {

                                                        if ($("#ChallanDetailsTransTableListCollapse").hasClass("collapsed")) {
                                                            $("#ChallanDetailsTransTableListCollapse").trigger('click');
                                                        }

                                                        unBlockUI();
                                                        return json.data;

                                                    },
                                                    error: function () {
                                                        unBlockUI();
                                                        // refreshCaptcha();
                                                        //window.location.href = '/Error/SessionExpire';
                                                    },
                                                    beforeSend: function () {
                                                        blockUI('loading data.. please wait...');
                                                        // Added by SB on 22-3-2019 at 11:06 am
                                                        var searchString = $('#ChallanTransDetailsTableID_filter input').val();
                                                        if (searchString != "") {
                                                            var regexToMatch = /^[^<>]+$/;


                                                            if (!regexToMatch.test(searchString)) {
                                                                unBlockUI();
                                                                $("#ChallanTransDetailsTableID_filter input").prop("disabled", true);
                                                                bootbox.alert('Please enter valid Search String ', function () {
                                                                    TableChallanDetailsTrans.search('').draw();
                                                                    $("#ChallanTransDetailsTableID_filter input").prop("disabled", false);

                                                                });
                                                                return false;
                                                            }
                                                        }
                                                    }
                                                },
                                                language: { search: "Search" },

                                                serverSide: true,
                                                "scrollX": true,
                                                "scrollY": "250px",
                                                scrollCollapse: true,
                                                bPaginate: true,
                                                bLengthChange: true,
                                                bInfo: true,
                                                info: true,
                                                bFilter: false,
                                                searching: true,
                                                dom: 'lBfrtip',
                                                "destroy": true,

                                                buttons: [
                                                    {

                                                    }

                                                ],
                                                columnDefs: [
                                                    { orderable: false, targets: [0] },
                                                    { orderable: false, targets: [1] },
                                                    { orderable: false, targets: [2] },
                                                    { orderable: false, targets: [3] },
                                                    { orderable: false, targets: [4] },
                                                    { orderable: false, targets: [5] },
                                                    { orderable: false, targets: [6] },
                                                    { orderable: false, targets: [7] },
                                                    { orderable: false, targets: [8] },
                                                    { orderable: false, targets: [9] },
                                                    { orderable: false, targets: [10] },
                                                    { orderable: false, targets: [11] },
                                                    { orderable: false, targets: [12] },
                                                    { orderable: false, targets: [13] },
                                                    { orderable: false, targets: [14] },
                                                    { orderable: false, targets: [15] },
                                                    { orderable: false, targets: [16] },
                                                    { orderable: false, targets: [17] },

                                                ],
                                                columns: [
                                                    { data: "ChallanReqID", "searchable": true, "visible": true, "name": "ChallanReqID" },
                                                    { data: "TransactionDateTime", "searchable": true, "visible": true },
                                                    { data: "SroCode", "searchable": true, "visible": true },
                                                    { data: "DDOCode", "searchable": true, "visible": true },
                                                    { data: "RemittanceBankName", "searchable": true, "visible": true },
                                                    { data: "ReceiptDate", "searchable": true, "visible": true },
                                                    { data: "UIRNumber", "searchable": true, "visible": true },
                                                    { data: "TransactionStatus", "searchable": true, "visible": true },
                                                    { data: "StatusCode", "searchable": true, "visible": true },
                                                    { data: "StatusDesc", "searchable": true, "visible": true },
                                                    { data: "UserID", "searchable": true, "visible": true },
                                                    { data: "IPAddress", "searchable": true, "visible": true },
                                                    { data: "BatchID", "searchable": true, "visible": true },
                                                    { data: "FirstPrintDate", "searchable": true, "visible": true },
                                                    { data: "ReqPaymentMode", "searchable": true, "visible": true },
                                                    { data: "IsDro", "searchable": true, "visible": true },
                                                    { data: "DroCode", "searchable": true, "visible": true },
                                                    { data: "SchedulerID", "searchable": true, "visible": true },
                                                    { data: "InsertDateTime", "searchable": true, "visible": true, "name": "InsertDateTime" },


                                                ],


                                                autoWidth: true,
                                                preDrawCallback: function () {

                                                    unBlockUI();
                                                },
                                                fnRowCallback: function (nRow, aData, iDisplayIndex) {

                                                    unBlockUI();
                                                    return nRow;
                                                },
                                                drawCallback: function (oSettings) {
                                                    unBlockUI();
                                                },
                                                fnInitComplete: function (oSettings, json) {
                                                    unBlockUI();
                                                    //if (json.status == "0") {
                                                    //    bootbox.alert({
                                                    //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                                    //        callback: function () {
                                                    //        }
                                                    //    });
                                                    //}

                                                    $('.dataTables_scrollBody thead tr').css({ visibility: 'collapse' });
                                                }
                                            });

                                            $('.dt-button').hide();  //For Hiding Empty buttons on datatable
                                        }
                                    });
                            });

                        }
                    });

                $('.dt-button').hide();  //For Hiding Empty buttons on datatable

            });





            $('.dt-button').hide();  //For Hiding Empty buttons on datatable
        }
    });

    var d = new Date();

    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();

    $('#txtFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"//,

    });

    $('#txtToDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

    });

    $('#divFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),

        pickerPosition: "bottom-left"

    });

    $('#divToDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });

    $('#txtToDate').datepicker({
        format: 'dd/mm/yyyy',
    }).datepicker("setDate", output);


    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", FromDate);

    $('#DROOfficeListID').change(function () {
        $('#SROOfficeListID').prop('disabled', false);

        $.ajax({
            url: '/Remittance/REMDaignostics/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROOfficeListID').val() },
            datatype: "json",
            type: "GET",
            success: function (data) {

                $('#SROOfficeListID').empty();
                $.each(data.SROOfficeList, function (i, SROOfficeList) {
                    $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                });
                unBlockUI();
            },
            error: function (xhr) {
                //window.location.href = '/Error/SessionExpire';
                unBlockUI();
            }
        });
    });
    $('.dt-button').hide();  //For Hiding Empty buttons on datatable

    $("#backToSummaryPage").click(function () {
        window.location.href = "/Remittance/REMDaignosticsSummary/GetOfficeListSummary";


    });

    if (IsForwardedFromSummaryLink == true) {
        $("#btnSearch").trigger('click');//to trigger when user clicks on link given on Summary table

    }
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



