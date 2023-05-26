var token = '';
var header = {};
var i = 0;
$(document).ready(function () {


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DROOfficeListID').change(function () {

        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/DataEntryCorrection/SelectDocument/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROOfficeListID').val() },
            datatype: "json",
            type: "GET",
            success: function (data) {
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    $('#SROOfficeListID').empty();
                    $.each(data.SROOfficeList, function (i, SROOfficeList) {
                        SROOfficeList
                        $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });

    //Added by Madhusoodan on 06/08/2021
    $('#dvSection68Note').hide();
    $('#btnUpdateSec68Note').hide();

    //Added by Madhusoodan on 13/08/2021 to lkoad finalize btn if Section 68 Note is added for Current Order ID
    $('#btnFinalizeOrder').show();
    LoadFinalizeButton();

    //Added by Madhur on 27-7-21

    $("#SearchBtn").click(function () {

        $("#PartialViewLoader").empty();
        var form = $("#DocumentSearchForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);


        if ($("#DocumentSearchForm").valid()) {
            SROOfficeID = $("#SROOfficeListID option:selected").val();
            DROfficeID = $("#DROOfficeListID option:selected").val();
            DocumentNumber = $("#txtDocumentNumber").val();
            BookTypeID = $("#BookTypeListID option:selected").val();
            FinancialYear = $("#FinancialYearListID option:selected").val();



            if (SROOfficeID == 0) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select SRO</span>'
                });
                return;
            }

            if (DocumentNumber == "" || DocumentNumber <= 0 || DocumentNumber > 99999) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Document Number</span>'
                });
                return;
            }
            var DocumentNumberRegex = new RegExp('^[0-9]*$');
            if (!DocumentNumberRegex.test(DocumentNumber)) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid Document Number</span>'
                });
                return;
            }


            if (BookTypeID == 0) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Book Type</span>'
                });
                return;
            }

            if (FinancialYear == "" || FinancialYear == "Select") {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Financial Year</span>'
                });
                return;
            }

            var classToRemove = $('#DtlsToggleIconSearchParaDetailExistingOrder').attr('class');
            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                $('#DtlsToggleIconSearchParaDetailExistingOrder').trigger('click');

            var classToRemove = $('#DtlsToggleIconSearchParaDocumentSearch').attr('class');
            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                $('#DtlsToggleIconSearchParaDocumentSearch').trigger('click');

            LoadPropertyDetailsData();



            //$('#DtlsSearchParaListCollapseDetail3').trigger('click');


            //$('#DtlsSearchParaListCollapse2').trigger('click');

        }

    });



    //Added by Madhur on 27-7-21



    //Changed by Madhusoodan on 06/08/2021 to save section 68 note, SROCode, DocumentID
    //$("#btnSelectCurrentDocument").click(function () {
    $("#btnSaveSec68Note").click(function () {

        //alert("In Select this Document btn");
        //It'll get DocumentID of first row, because DocumenID will be same in all rows
        //var CurrentDocumentID = $("#DetailTableID").DataTable().row().data().DocumentID;
        //alert("currentDocumentID: " + CurrentDocumentID + "   headers: " + header);
        SaveSection68Note();

    });

    //Added by Madhusoodan on 07/08/2021
    $("#btnUpdateSec68Note").click(function () {

        //This will update the entered/changed Section 68 Note
        SaveSection68Note();

    });

    //Added by Madhur on 07/08/2021
    $('#btncloseAbortPopup2').click(function () {

        $('#divLoadAbortViewForPropertyPopup').html('');

        $('#divViewAbortModalForPropertyPopup').css('display', 'none');
        $('#divViewAbortModalForPropertyPopup').addClass('modal fade');
        //$('#divViewAbortModalForPropertyPopup').modal('hide');

    });
    //Added by mayank on 16/08/2021
    $('#btnclosePropertyPopup').click(function () {

        $('#divLoadAbortViewForPropertyPopup').html('');

        $('#divViewAbortModalForPropertyPopup').css('display', 'none');
        $('#divViewAbortModalForPropertyPopup').addClass('modal fade');
        //$('#divViewAbortModalForPropertyPopup').modal('hide');


    });

    //Added by Madhusoodan on 13/08/2021 for Finalize button
    $('#btnFinalizeOrder').click(function () {
        FinalizeDEC();
    });


    $('#DtlsSearchParaListCollapseDocumentSearch').click(function () {
        //alert("2");
        var classToRemove = $('#DtlsToggleIconSearchParaDocumentSearch').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchParaDocumentSearch').removeClass(classToRemove).addClass(classToSet);


    });

    $('#DtlsSearchParaListCollapseDetailPropertyDetails').click(function () {
        //alert("3");
        var classToRemove = $('#DtlsToggleIconSearchParaDetailPropertyDetails').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchParaDetailPropertyDetails').removeClass(classToRemove).addClass(classToSet);



    });


    $('#DtlsSearchParaListCollapseDetailExistingOrder').click(function () {
        //alert("4");
        var classToRemove = $('#DtlsToggleIconSearchParaDetailExistingOrder').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchParaDetailExistingOrder').removeClass(classToRemove).addClass(classToSet);




    });

    //alert(isEditMode);
    if (IsDocumentSearched == "True") {
        $("#SROOfficeListID").attr("disabled", "disabled");
        $("#txtDocumentNumber").attr("disabled", "disabled");
        $("#BookTypeListID").attr("disabled", "disabled");
        $("#FinancialYearListID").attr("disabled", "disabled");
    }

    $("#btncloseAbortPopup").click(function () {

        $("#objPDFViewer").attr('data', '');
        $('#divViewAbortModal').css('display', 'none');
        $('#divViewAbortModal').addClass('modal fade');
        //$('#divViewAbortModal').modal('show');



        //$('#divViewAbortModal').modal('hide');
        //$("#divViewAbortModal").modal('hide')
    });

});

function LoadPropertyDetailsData() {

    var DetailsTable = $('#DetailTableID').DataTable({
        ajax: {
            url: '/DataEntryCorrection/SelectDocument/LoadPropertyDetailsData',
            type: "POST",
            headers: header,
            data: {
                'SROOfficeID': SROOfficeID, 'DROfficeID': DROfficeID, 'DocumentNumber': DocumentNumber, 'BookTypeID': BookTypeID, 'FinancialYear': FinancialYear,
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
                                var classToRemove = $('#DtlsToggleIconSearchParaDetailPropertyDetails').attr('class');
                                if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapseDetailPropertyDetails').trigger('click');
                                //var classToRemove = $('#DtlsToggleIconSearchPara2').attr('class');
                                //if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                //    $('#DtlsSearchParaListCollapse2').trigger('click');
                                $("#DetailTableID").DataTable().clear().destroy();
                            }
                        }
                    });
                    return;
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchParaDetailPropertyDetails').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapseDetailPropertyDetails').trigger('click');
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
                var searchString = $('#DetailTableID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;

                    if (!regexToMatch.test(searchString)) {
                        $("#DetailTableID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DetailsTable.search('').draw();
                            $("#DetailTableID_filter input").prop("disabled", false);
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
        bPaginate: false,
        bLengthChange: false,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: false,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [
            { "className": "SelToUnsel dt-center", "targets": [1] },
            //{ "className": "dt-center", "targets": "_all" },
            { "orderable": false, "targets": "_all" },
            { "bSortable": false, "aTargets": "_all" },
            { "className": "dt-center", "targets": [0, 1, 2, 3, 5, 6, 7, 8, 9, 10] },
            { "className": "dt-left", "targets": [4] },
        ],

        columns: [
            { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
            //Added by Madhusoodan on 06/08/2021
            { data: "SelectButton", "searchable": false, "visible": true, "name": "Select" },
            { data: "PNDTabButton", "searchable": false, "visible": true, "name": "PNDTabButton" },
            { data: "PartyTabButton", "searchable": false, "visible": true, "name": "PartyTabButton" },
            //
            { data: "ScheduleDescription", "searchable": true, "visible": true, "name": "ScheduleDescription" },
            { data: "ExecutionDate", "searchable": true, "visible": true, "name": "ExecutionDate" },
            { data: "NatureOfDocument", "searchable": true, "visible": true, "name": "NatureOfDocument" },
            { data: "Executant", "searchable": true, "visible": true, "name": "Executant" },
            { data: "Claimant", "searchable": true, "visible": true, "name": "Claimant" },
            { data: "CDNumber", "searchable": true, "visible": true, "name": "CDNumber" },
            { data: "PageCount", "searchable": true, "visible": true, "name": "PageCount" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" }


        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
            $("#EncryptedDocumentId").val(json.EncrytptedDocumetID);
            //console.log(json);
            if (json.recordsTotal >= 1 || IsDocumentSearched == "True") {
                $("#SROOfficeListID").attr("disabled", "disabled");
                $("#txtDocumentNumber").attr("disabled", "disabled");
                $("#BookTypeListID").attr("disabled", "disabled");
                $("#FinancialYearListID").attr("disabled", "disabled");
            }
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
    });
}

//On click of Get Details
function ViewBtnClickPreviousPropTable(OrderID) {
    $.ajax({
        url: '/DataEntryCorrection/SelectDocument/ViewBtnClickPreviousPropTable',
        data: { "OrderID": OrderID },
        type: "GET",
        success: function (data) {
            if (data != "Could not find file") {
                $('#divViewAbortModal').css('display', 'block');
                $('#divViewAbortModal').addClass('modal fade in');
                //$('#divViewAbortModal').modal('show');
                $("#objPDFViewer").attr('data', '');
                $("#objPDFViewer").load(data);
                $("#objPDFViewer").attr('data', '/DataEntryCorrection/SelectDocument/ViewBtnClickPreviousPropTable?OrderID=' + OrderID);

            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">File does not exist at path.</span>');
            }
            unBlockUI();
        },
        error: function (xhr) {
            //alert(xhr);
            unBlockUI();
        }
    });
}


//function SelectDocument(CurrentDocumentID) {
//Below function is used for add update both
function SaveSection68Note() {

    //alert("DocumentID: " + DocumentID + "   PropertyID: " + PropertyID);
    //alert("In SaveSection68NOte() -> DocumentID: " + docID + "   PropertyID: " + propID);

    //Added by Madhusoodan on 08/08/2021
    //$('#DtlsSearchParaListCollapseDetailPropertyDetails').trigger('click');

    //if (DocumentID == null || PropertyID == null) {
    if (docID == null || propID == null) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select a Property from Property Details.</span>'
        });
    }
    else {

        var dataToSave = $("#Section68NoteForm").serializeArray();
        //dataToSave.push({ name: 'DocumentID', value: DocumentID }, { name: 'PropertyID', value: PropertyID });
        dataToSave.push({ name: 'DocumentID', value: docID }, { name: 'PropertyID', value: propID });

        var form = $("#Section68NoteForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);

        if ($("#Section68NoteForm").valid()) {

            blockUI();

            ////
            $.ajax({
                //url: '/DataEntryCorrection/SelectDocument/SaveDocumentID',
                url: '/DataEntryCorrection/SelectDocument/SaveSectionNote',
                type: "POST",
                headers: header,
                data: dataToSave,
                dataType: "json",
                success: function (data) {
                    if (data.serverError) {

                        //alert("data.serverError == true");

                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            function () {
                                window.location.href = "/Home/HomePage"
                            });
                    }
                    else {
                        if (data.success) {

                            //alert("data.success == true");

                            bootbox.alert({
                                message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                callback: function () { }
                            });

                            //Toggle to update btn from save btn
                            $('#btnSaveSec68Note').hide();
                            $('#btnUpdateSec68Note').show();

                            //Added by Madhusoodan on 12/08/2021
                            //$('#DtlsSearchParaListCollapseDetailPropertyDetails').trigger('click');
                            //$('#DtlsSearchParaListCollapseDetailExistingOrder').trigger('click');

                            //Commented and added by Madhusoodan to load call function for same operation
                            //LoadPreviousPropertyDetails();
                            LoadPropertyDetailsData();

                            var classToRemove = $('#DtlsToggleIconSearchParaDetailExistingOrder').attr('class');
                            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                $('#DtlsToggleIconSearchParaDetailExistingOrder').trigger('click');

                            var classToRemove = $('#DtlsToggleIconSearchParaDetailPropertyDetails').attr('class');
                            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                $('#DtlsSearchParaListCollapseDetailPropertyDetails').trigger('click');
                            //                            $('#DtlsSearchParaListCollapseDetailPropertyDetails').trigger('click');
                            //$('#DtlsSearchParaListCollapseDetailExistingOrder').trigger('click');
                            //Load Finalize buton
                            LoadFinalizeButton();
                            //Added by Madhusoodan on 09/08/2021
                            //Reload EC Report Datatable
                            //var DetailsTable = $('#DetailTableID').DataTable({
                            //    ajax: {
                            //        url: '/DataEntryCorrection/SelectDocument/LoadPropertyDetailsData',
                            //        type: "POST",
                            //        headers: header,
                            //        data: {
                            //            'SROOfficeID': SROOfficeID, 'DROfficeID': DROfficeID, 'DocumentNumber': DocumentNumber, 'BookTypeID': BookTypeID, 'FinancialYear': FinancialYear,
                            //        },
                            //        dataSrc: function (json) {
                            //            unBlockUI();
                            //            if (json.errorMessage != null) {


                            //                bootbox.alert({
                            //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            //                    callback: function () {
                            //                        if (json.serverError != undefined) {
                            //                            window.location.href = "/Home/HomePage"
                            //                        } else {
                            //                            //alert("In class to remove");

                            //                            var classToRemove = $('#DtlsToggleIconSearchPara2').attr('class');
                            //                            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                            //                                $('#DtlsSearchParaListCollapse2').trigger('click');
                            //                            $("#DetailTableID").DataTable().clear().destroy();
                            //                        }
                            //                    }
                            //                });
                            //                return;
                            //            }
                            //            else {
                            //                var classToRemove = $('#DtlsToggleIconSearchParaDetail').attr('class');
                            //                if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                            //                    $('#DtlsToggleIconSearchParaDetail').trigger('click');
                            //            }
                            //            unBlockUI();
                            //            return json.data;
                            //        },
                            //        error: function () {
                            //            unBlockUI();
                            //            bootbox.alert({
                            //                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            //                callback: function () {
                            //                }
                            //            });
                            //        },
                            //        beforeSend: function () {
                            //            blockUI('loading data.. please wait...');
                            //            var searchString = $('#DetailTableID_filter input').val();
                            //            if (searchString != "") {
                            //                var regexToMatch = /^[^<>]+$/;

                            //                if (!regexToMatch.test(searchString)) {
                            //                    $("#DetailTableID_filter input").prop("disabled", true);
                            //                    bootbox.alert('Please enter valid Search String ', function () {
                            //                        DetailsTable.search('').draw();
                            //                        $("#DetailTableID_filter input").prop("disabled", false);
                            //                    });
                            //                    unBlockUI();
                            //                    return false;
                            //                }
                            //            }
                            //        }
                            //    },
                            //    serverSide: true,
                            //    //"scrollX": true,
                            //    //"scrollY": "300px",
                            //    scrollCollapse: true,
                            //    bPaginate: true,
                            //    bLengthChange: true,
                            //    bInfo: true,
                            //    info: true,
                            //    bFilter: false,
                            //    searching: true,
                            //    "destroy": true,
                            //    "bAutoWidth": true,
                            //    "bScrollAutoCss": true,

                            //    columnDefs: [
                            //        { "className": "SelToUnsel dt-center", "targets": [1] },
                            //        { "className": "dt-center", "targets": "_all" },
                            //        //{ orderable: false, targets: [1] },
                            //        //{ orderable: false, targets: [2] },
                            //        //{ orderable: false, targets: [3] },
                            //        //{ orderable: false, targets: [4] }
                            //    ],

                            //    columns: [
                            //        { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
                            //        //Added by Madhusoodan on 06/08/2021
                            //        { data: "SelectButton", "searchable": false, "visible": true, "name": "Select" },
                            //        { data: "PNDTabButton", "searchable": false, "visible": true, "name": "PNDTabButton" },
                            //        { data: "PartyTabButton", "searchable": false, "visible": true, "name": "PartyTabButton" },
                            //        //
                            //        { data: "ScheduleDescription", "searchable": true, "visible": true, "name": "ScheduleDescription" },
                            //        { data: "ExecutionDate", "searchable": true, "visible": true, "name": "ExecutionDate" },
                            //        { data: "NatureOfDocument", "searchable": true, "visible": true, "name": "NatureOfDocument" },
                            //        { data: "Executant", "searchable": true, "visible": true, "name": "Executant" },
                            //        { data: "Claimant", "searchable": true, "visible": true, "name": "Claimant" },
                            //        { data: "CDNumber", "searchable": true, "visible": true, "name": "CDNumber" },
                            //        { data: "PageCount", "searchable": true, "visible": true, "name": "PageCount" },
                            //        { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" }


                            //        //{ data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
                            //        //{ data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" },
                            //        //{ data: "ExecutionDate", "searchable": true, "visible": true, "name": "ExecutionDate" },
                            //        //{ data: "NatureOfDocument", "searchable": true, "visible": true, "name": "NatureOfDocument" },
                            //        //{ data: "PropertyDetails", "searchable": true, "visible": true, "name": "PropertyDetails" },
                            //        //{ data: "VillageName", "searchable": true, "visible": true, "name": "VillageName" },
                            //        //{ data: "TotalArea", "searchable": true, "visible": true, "name": "TotalArea" },
                            //        //{ data: "ScheduleDescription", "searchable": true, "visible": true, "name": "ScheduleDescription" },
                            //        //{ data: "Marketvalue", "searchable": true, "visible": true, "name": "Marketvalue" },
                            //        //{ data: "Consideration", "searchable": true, "visible": true, "name": "Consideration" },
                            //        //{ data: "Claimant", "searchable": true, "visible": true, "name": "Claimant" },
                            //        //{ data: "Executant", "searchable": true, "visible": true, "name": "Executant" }
                            //    ],
                            //    fnInitComplete: function (oSettings, json) {
                            //    },
                            //    preDrawCallback: function () {
                            //        unBlockUI();
                            //    },
                            //    fnRowCallback: function (nRow, aData, iDisplayIndex) {

                            //        //if (ModuleID == 1) {
                            //        //    fnSetColumnVis(2, false);
                            //        //}
                            //        //else if (ModuleID == 2) {
                            //        //    fnSetColumnVis(0, false);
                            //        //    fnSetColumnVis(1, false);
                            //        //}

                            //        unBlockUI();
                            //        return nRow;
                            //    },
                            //    drawCallback: function (oSettings) {
                            //        unBlockUI();
                            //    },
                            //});
                            //
                            //

                        }
                        else {
                            //alert("data.success == false");

                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>'
                            });
                        }
                    }
                    unBlockUI();
                },
                //error: function (xhr) {
                //    alert("In error : xhr");

                //    alert(xhr);
                //    unBlockUI();
                //}
                error: function (xhr, status, err) {
                    //alert("in LoadRevenueCollectedChartData");
                    //bootbox.alert("Error occured while proccessing your request : " + err);
                    //bootbox.alert("Error occured while proccessing your request : " + xhr);
                    //bootbox.alert("Error occured while proccessing your request : " + status);

                    //$.unblockUI();
                    unBlockUI();
                }
            });

            ////


        }
    }



}





//Added by Madhusoodan on 06/08/2021
//function SelectProperty(ID) {
function SelectProperty(ID, DocumentID, PropertyID) {

    //set documentID and PropertyID in gobal js variables
    docID = DocumentID;
    propID = PropertyID;

    //$('#DtlsSearchParaListCollapseDetail3').trigger('click');


    window.scrollTo({ left: 0, top: document.body.scrollHeight, behavior: "smooth" });


    //window.scrollTo({ left: 0, top: document.body.childre, behavior: "smooth" });
    //alert("before function")
    ScrollToBottom();
    //$('#LoadDECInsertUpdateDeleteViewDivID').scrollTop($('#LoadDECInsertUpdateDeleteViewDivID')[0].scrollHeight);
    //Madhur's Code
    $('.PNDactive').prop('disabled', true);
    $('.Partyactive').prop('disabled', true);
    var flag = $('#PNDTabButton' + ID).prop('disabled', false).removeClass('PNDdeactive').addClass('PNDactive');
    var flag1 = $('#PartyTabButton' + ID).prop('disabled', false).removeClass('Partydeactive').addClass('Partyactive');
    $('.unselection').each(function () {
        var SID = $(this).attr('id');
        var SFID = SID.replace('SELE', '');
        if (SID != undefined) {
            //alert("SID: " + SID + "SFID:" + SFID +"ID:"+ID);

            //var data1 = "<button type = 'button' class= 'btn btn-group-md btn-primary selection' id = '" + SID + "' onclick = 'SelectProperty(" + SFID + "," + DocumentID + "," + PropertyID + ")'>Select</ button >";
            $(this).attr('class', 'btn btn-group-md btn-primary selection');
            //$(this).closest('.SelToUnsel').html(data1);
        }//$('#' + lid).closest('.SelToUnsel').html('hii');        //$(this).html("<button type ='button' class='btn btn-group-md btn-primary selection' id='SELE" + ID + "' onclick='SelectProperty(" + ID + ")' data-toggle='tooltip' data-placement='top' title='Click here'>Select</ button>");
    });
    //var flag2 = $('#SELE' + ID).closest('td').html("<button type ='button' class='btn btn-group-md btn-primary unselection' id='SELE" + ID + "' onclick='UnSelectProperty(" + ID + "," + DocumentID + "," + PropertyID + ")' data-toggle='tooltip' data-placement='top' title='Click here'>Unselect</ button>");
    var flag2 = $('#SELE' + ID).closest('td').html("<button type ='button' class='btn btn-group-md btn-primary unselection' id='SELE" + ID + "' onclick='SelectProperty(" + ID + "," + DocumentID + "," + PropertyID + ")' data-toggle='tooltip' data-placement='top' title='Click here'>Select</ button>");

    LoadPreviousPropertyDetails();

    $('#dvSection68Note').show();

    //Load Previous Section 68 Note
    LoadPrevSection68Note();

    $('#txtSection68Note').focus();
    //Recurssion condition used to scroll to bottom of the page as partial view is used
    if (i < 1) {
        i = i + 1;
        SelectProperty(ID, DocumentID, PropertyID);
        if (i == 1)
            i = 0;
    }

}

//Added by Madhusoodan to load Previous Section68 Notes for Property
function LoadPreviousPropertyDetails() {

    //alert("In LoadPreviousPropertyDetails ->  SROOfficeID:" + SROOfficeID + "DocumentID: " + docID + "PropertyID: " + propID);

    var PreviousDetailsTable = $('#PreviousDetailTableID').DataTable({
        ajax: {
            url: '/DataEntryCorrection/SelectDocument/LoadPreviousPropertyDetailsData',
            type: "POST",
            headers: header,
            data: {
                'SROOfficeID': SROOfficeID, 'DROfficeID': DROfficeID, 'DocumentID': docID, 'PropertyID': propID,
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
                                var classToRemove = $('#DtlsToggleIconSearchPara1').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse1').trigger('click');
                                $("#PreviousDetailTableID").DataTable().clear().destroy();
                            }
                        }
                    });
                    return;
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchParaDetailExistingOrder').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsToggleIconSearchParaDetailExistingOrder').trigger('click');
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
                var searchString = $('#PreviousDetailTableID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;

                    if (!regexToMatch.test(searchString)) {
                        $("#PreviousDetailTableID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            PreviousDetailsTable.search('').draw();
                            $("#PreviousDetailTableID_filter input").prop("disabled", false);
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
            { "orderable": false, "targets": "_all" },
            { "bSortable": false, "aTargets": "_all" },
            //{ orderable: false, targets: [1] },
            //{ orderable: false, targets: [2] },
            //{ orderable: false, targets: [3] },
            //{ orderable: false, targets: [4] }
        ],


        columns: [
            { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
            { data: "DROrderNumber", "searchable": true, "visible": true, "name": "DROrderNumber" },
            { data: "OrderDate", "searchable": true, "visible": true, "name": "OrderDate" },
            { data: "OrderUploadDate", "searchable": true, "visible": true, "name": "OrderUploadDate" }, //Added by Madhusoodan on 11/08/2021 to add Order Upload Date column

            { data: "Section68Note", "searchable": true, "visible": true, "name": "Section68Note" },
            { data: "ViewDocument", "searchable": false, "visible": true, "name": "ViewDocument" },
            { data: "DeleteNoteBtn", "searchable": false, "visible": true, "name": "DeleteNoteBtn" } //Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
            //$("#DtlsSearchParaListCollapse2").click();
            //var className3 = $('#DtlsToggleIconSearchParaDetail3').attr('class');
            //var className4 = $('#DtlsToggleIconSearchParaDetail4').attr('class');
            //if (className3 != "fa fa-minus-square-o fa-pull-left fa-2x") {
            //    $('#DtlsSearchParaListCollapseDetail3').click();
            //}
            //if (className4 != "fa fa-minus-square-o fa-pull-left fa-2x") {
            //    $('#DtlsSearchParaListCollapseDetail4').click();
            //}
            var classToRemove = $('#DtlsToggleIconSearchParaDetailExistingOrder').attr('class');
            if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                $('#DtlsToggleIconSearchParaDetailExistingOrder').trigger('click');

            var classToRemove = $('#DtlsToggleIconSearchParaDetailPropertyDetails').attr('class');
            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                $('#DtlsToggleIconSearchParaDetailPropertyDetails').trigger('click');

            ScrollToBottom();
            $('#txtSection68Note').focus();

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
    });
}

function LoadPrevSection68Note() {

    //For edit mode show Section 68 Note in the same text area
    blockUI();

    $.ajax({
        //url: '/DataEntryCorrection/SelectDocument/SaveDocumentID',
        url: '/DataEntryCorrection/SelectDocument/LoadPreviousSecNote',
        type: "GET",
        headers: header,
        //data: { 'currentPropertyID': PropertyID },
        data: { 'currentPropertyID': propID },
        dataType: "Json",
        success: function (data) {
            if (data.serverError) {

                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                if (data.success) {
                    $('#txtSection68Note').val(data.PreviousSection68Note);

                    //Added by Madhusoodan on 17/08/2021
                    //$('#spnNotePreparedPart').val(data.Sec68NotePreparedPart);
                    document.getElementById('spnNotePreparedPart').innerHTML = data.Sec68NotePreparedPart;




                    if (data.IsUpdateButton) {
                        $('#btnSaveSec68Note').hide();
                        $('#btnUpdateSec68Note').show();
                    }
                    //}
                    else {
                        //$('#txtSection68Note').val('');  //clear any previous property's note

                        //alert("In IsUpdateButton == false");
                        $('#btnSaveSec68Note').show();
                        $('#btnUpdateSec68Note').hide();
                    }


                    //added by vijay on 08-03-2023
                    const inputField = document.getElementById('txtSection68Note');
                    const errorMessage = document.getElementById('txtSection68Note_error');
                    const invalidChar = document.getElementById('invalid-char');
                    //COMMENTED BY VIJAY ON 05-04-23 
                    //const regex = /^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()%#&*.,:; +]*$/;
                    //ADDED BY VIJAY ON 05-04-23 TO ACCEPT CHARACTERS LIKE "ತ್‌"
                    const regex = /^[\u0C80-\u0CFFa-zA-Z0-9_\u200C\u0CCD\/\\\-()%#&*.,:; +]*$/;

                    inputField.addEventListener('input', () => {
                        const input = inputField.value;
                        if (!regex.test(input)) {
                            errorMessage.style.display = 'block';
                            //const invalidIndex = input.search(/[^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()%#&*.,:; + ]/);
                            const invalidIndex = input.search(/[^[\u0C80-\u0CFFa-zA-Z0-9_\u200C\u0CCD\/\\\-()%#&*.,:; + ]/);

                            if (invalidIndex >= 0) {
                                const invalidCharValue = input.charAt(invalidIndex);
                                if (invalidCharValue === '\n') {
                                    invalidChar.textContent = 'Error:New line character is not allowed/Enter key was pressed';
                                } else {
                                    invalidChar.textContent = `Invalid character: ${invalidCharValue}`;
                                }
                                invalidChar.style.display = 'block';
                            }
                        } else {
                            errorMessage.style.display = 'none';
                            invalidChar.style.display = 'none';
                        }
                    });


                    inputField.addEventListener('keydown', (event) => {
                        if (event.keyCode === 13) {
                            event.preventDefault();
                            invalidChar.textContent = 'Error:New line character is not allowed/Enter key was pressed';
                            invalidChar.style.display = 'block';
                        }
                    });


                }
                else {
                    //alert("data.success == false");

                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>'
                    });
                }
            }
            unBlockUI();
        },
        error: function (xhr, status, err) {
            alert("In xhr error");
            alert(xhr);
            unBlockUI();
        }
    });
}


function SelectBtnPopulation(DocumentID, PropertyID, ID) {
    $('#SelectTableID').DataTable({
        ajax: {
            url: '/DataEntryCorrection/PropertyNumberDetails/SelectBtnClick',
            type: "POST",
            headers: header,
            data: {
                'DocumentID': DocumentID, 'PropertyID': PropertyID,
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {

                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',

                    });
                    return;
                }
                else {
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
            }
        },
        serverSide: true,
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
        ],
        columns: [
            { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
            { data: "DROrderNumber", "searchable": true, "visible": true, "name": "DROrderNumber" },
            { data: "OrderDate", "searchable": true, "visible": true, "name": "OrderDate" },
            { data: "Village", "searchable": true, "visible": true, "name": "Village" },
            { data: "CurrentPropertyType", "searchable": true, "visible": true, "name": "CurrentPropertyType" },
            { data: "CurrentNumber", "searchable": true, "visible": true, "name": "CurrentNumber" },
            { data: "OldPropertyType", "searchable": true, "visible": true, "name": "OldPropertyType" },
            { data: "OldNumber", "searchable": true, "visible": true, "name": "OldNumber" },
            { data: "Survey_No", "searchable": true, "visible": true, "name": "Survey_No" },
            { data: "Surnoc", "searchable": true, "visible": true, "name": "Surnoc" },
            { data: "Hissa_No", "searchable": true, "visible": true, "name": "Hissa_No" },
            { data: "CorrectionNote", "searchable": true, "visible": true, "name": "CorrectionNote" },
            { data: "Action", "searchable": true, "visible": true, "name": "Action" },

        ],
        fnInitComplete: function (oSettings, json) {


            $("#PropertyDetailTableID").DataTable().cell($('#SELE' + ID).closest('.SelToUnsel')).data("<button type ='button' class='btn btn-group-md btn-primary unselection' id='SELE" + ID + "' onclick='UnSelectBtnClick(" + ID + ")' data-toggle='tooltip' data-placement='top' title='Select this Property'>Unselect</ button>");


            var classToRemove6 = $('#DtlsToggleIconSearchParaDetail6').attr('class');
            if (classToRemove6 == "fa fa-plus-square-o fa-pull-left fa-2x") {
                $('#DtlsSearchParaListCollapseDetail6').click();
            }
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
        },
        drawCallback: function (oSettings) {
            unBlockUI();
        },
    });
}

function LoadPropertyPopup(DocumentID, PropertyID) {
    $.ajax({
        url: "/DataEntryCorrection/SelectDocument/CheckifOrderNoteExist",
        type: "GET",
        //data: $("#PropertyNumberDetailsForm").serialize(),
        data: { 'PropertyID': PropertyID },
        dataType: "json",
        headers: header,
        success: function (data) {
            unBlockUI();
            if (data.success) {
                $("#SelectTableID").DataTable().clear().destroy();
                $("#OrderDetailsPopUp").html("Add New Property Number Search Key");
                $.ajax({
                    //Changed by mayank on 16/08/2021
                    //url: '/DataEntryCorrection/SelectDocument/LoadInsertUpdateDeleteView?TableViewName=PropertyNumberDetails',
                    url: '/DataEntryCorrection/SelectDocument/LoadInsertUpdateDeleteView',
                    data: { 'TableViewNamee': 'PropertyNumberDetails', 'PropertyID': PropertyID },
                    headers: header,
                    type: "GET",
                    success: function (data) {
                        $('#divLoadAbortViewForPropertyPopupbody').html('');

                        $('#divLoadAbortViewForPropertyPopupbody').html(data);
                        PropertyPopupPopulation(DocumentID, PropertyID);
                        $('#divViewAbortModalForPropertyPopup').css('display', 'block');
                        $('#divViewAbortModalForPropertyPopup').addClass('modal fade in');
                        //$('#divViewAbortModalForPropertyPopup').modal('show');

                        unBlockUI();
                    },
                    error: function (xhr) {

                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            callback: function () {
                            }
                        });
                        unBlockUI();
                    },
                    beforeSend: function () {
                        blockUI('Loading Data please wait');
                    }
                });
            }
            else {
                //alert("data.success = false");

                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>'
                });
                return false;
            }

        },
        error: function (xhr) {
            test = xhr;
            unBlockUI();

            //alert("Last error func");

            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });

            return false;
        },

    });

}



function LoadPartyPopup(DocumentID, PropertyID) {



    $.ajax({
        url: "/DataEntryCorrection/SelectDocument/CheckifOrderNoteExist",
        type: "GET",
        data: { 'PropertyID': PropertyID },
        dataType: "json",
        headers: header,
        success: function (data) {
            unBlockUI();
            if (data.success) {
                //alert("return true");
                $("#SelectTablePartyTabID").DataTable().clear().destroy();
                $("#OrderDetailsPopUp").html("Add New Party Search Key");
                $("#UpdatePropertyNoDiv").hide();
                $.ajax({
                    url: '/DataEntryCorrection/SelectDocument/LoadInsertUpdateDeleteView?TableViewNamee=PartyNumberDetails',
                    data: {
                        'PropertyID': PropertyID, 'TableViewNamee': 'PartyNumberDetails'
                    },
                    headers: header,
                    type: "GET",
                    success: function (data) {
                        //$('#divLoadAbortViewForPropertyPopup').html('');
                        $('#divLoadAbortViewForPropertyPopupbody').empty();
                        //$('#divLoadAbortViewForPropertyPopup').show();

                        $('#divLoadAbortViewForPropertyPopupbody').html(data);
                        //$('#divViewAbortModalForPropertyPopup').modal('show');
                        PartyPopupPopulation(DocumentID, PropertyID);
                        $('#divViewAbortModalForPropertyPopup').css('display', 'block');
                        $('#divViewAbortModalForPropertyPopup').addClass('modal fade in');


                        unBlockUI();
                    },
                    error: function (xhr) {

                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            callback: function () {
                            }
                        });
                        unBlockUI();
                    },
                    beforeSend: function () {
                        blockUI('Loading Data please wait');
                    }
                });
            }
            else {
                //alert("data.success = false");

                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>'
                });
                return false;
            }

        },
        error: function (xhr) {
            test = xhr;
            unBlockUI();

            //alert("Last error func");

            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });

            return false;
        },

    });




}


function PartyPopupPopulation(DocumentID, PropertyID) {
    var SelectTablePartyTabID = $('#SelectTablePartyTabID').DataTable({
        ajax: {
            url: '/DataEntryCorrection/PartyDetails/SelectBtnPartyTabClick',
            type: "POST",
            headers: header,
            data: {
                'DocumentID': DocumentID, 'PropertyID': PropertyID,
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {


                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) { window.location.href = "/Home/HomePage" }
                            else { $("#SelectTablePartyTabID").DataTable().clear().destroy(); }
                        }
                    });
                    return;
                }
                else {
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
                var searchString = $('#SelectTablePartyTabID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;

                    if (!regexToMatch.test(searchString)) {
                        $("#SelectTablePartyTabID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DetailsTable.search('').draw();
                            $("SelectTablePartyTabID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },
        serverSide: true,
        scrollCollapse: true,
        bPaginate: false,
        bLengthChange: false,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: false,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [
            { "className": "dt-center", "targets": "_all" },


        ],
        columns: [
            { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
            { data: "DROrderNumber", "searchable": true, "visible": true, "name": "DROrderNumber" },
            { data: "OrderDate", "searchable": true, "visible": true, "name": "OrderDate" },
            { data: "PartyType", "searchable": true, "visible": true, "name": "PartyType" },
            { data: "FirstName", "searchable": true, "visible": true, "name": "FirstName" },
            { data: "MiddleName", "searchable": true, "visible": true, "name": "MiddleName" },
            { data: "LastName", "searchable": true, "visible": true, "name": "LastName" },
            { data: "CorrectionNote", "searchable": true, "visible": true, "name": "CorrectionNote" },
            { data: "Action", "searchable": true, "visible": true, "name": "Action" },

        ],
        fnInitComplete: function (oSettings, json) {
            var form = $('#PartyDetailsForm');
            form.append('<input type="hidden" id="PNDDocumentID" name="PNDDocumentID" value ="' + DocumentID + '" />');
            form.append('<input type="hidden" id="PNDPropertyID" name="PNDPropertyID" value ="' + PropertyID + '" />');

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
    });
}

//Added by Madhusoodan on 13/08/2021 to load Delete button for Section 68 Note
function DeleteSection68Note(NoteID) {

    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Do you want to Delete this Section 68(2) Note?</span>",

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
                blockUI();
                $.ajax({
                    url: '/DataEntryCorrection/SelectDocument/DeleteSectionNote',
                    data: { "NoteID": NoteID },
                    type: "GET",
                    contentType: "json",
                    success: function (data) {

                        unBlockUI();
                        if (!data.serverError) {
                            if (data.success) {
                                bootbox.alert('<i class="fa fa-trash" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                            }
                            //Refresh datatable after deleting file
                            LoadPreviousPropertyDetails();
                            //Check and load Finaliaze button
                            LoadFinalizeButton();
                            //Reload Section 68 Note
                            LoadPrevSection68Note();
                        }
                        else {
                            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');
                        }
                    },
                    error: function (xhr) {
                        unBlockUI();
                        test = xhr;
                        var err = eval("(" + xhr.responseText + ")");
                        //alert(err.Message);
                    }
                });
            }
        }
    });
}

//Added by Madhusoodan on 13/08/2021 to lkoad finalize btn if Section 68 Note is added for Current Order ID
function LoadFinalizeButton() {

    $.ajax({
        url: '/DataEntryCorrection/SelectDocument/IsSectionNoteAddedForOrderID',
        data: {},
        type: "GET",
        contentType: "json",
        success: function (data) {

            unBlockUI();
            if (!data.serverError) {
                if (data.success) {

                    //Show Finalize Button
                    if (data.showFinalizeButton) {
                        //Show
                        $('#btnFinalizeOrder').show();
                    }
                    else {
                        //hide
                        $('#btnFinalizeOrder').hide();
                    }

                }
            }
            else {
                //Hide Section 68 Note
            }
        },
        error: function (xhr) {
            unBlockUI();
            var err = eval("(" + xhr.responseText + ")");
            //alert(err.Message);
        }
    });

}

function FinalizeDEC() {

    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Please verify Correction Note in Property Description before finalizing. Are you sure to finalize ?</span>",

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
                blockUI('Finalizing DR Order... please wait...');

                //alert("Calling FinalizeDEC");
                $.ajax({
                    url: '/DataEntryCorrection/DataEntryCorrection/FinalizeDECOrder',
                    headers: header,
                    datatype: "json",
                    type: "GET",
                    success: function (data) {
                        unBlockUI();
                        if (data.success) {

                            bootbox.alert({
                                //message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                message: '<i class="fa fa-check fa-lg boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',

                                callback: function () {
                                    window.location.href = '/DataEntryCorrection/DataEntryCorrection/DataEntryCorrectionView';
                                }
                            });
                        }
                        else {
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                            });
                        }
                    },
                    error: function (xhr) {
                        unBlockUI();
                    }
                });
            }
        }
    });
}

function PropertyPopupPopulation(DocumentID, PropertyID) {
    $('#SelectTableID').DataTable({
        ajax: {
            url: '/DataEntryCorrection/PropertyNumberDetails/SelectBtnClick',
            type: "POST",
            headers: header,
            data: {
                'DocumentID': DocumentID, 'PropertyID': PropertyID,
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {

                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',

                    });
                    return;
                }
                else {
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
            }
        },
        serverSide: true,
        scrollCollapse: true,
        bPaginate: false,
        bLengthChange: false,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: false,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [
            { "className": "dt-center", "targets": "_all" },
            { "orderable": false, "targets": "_all" },
            { "bSortable": false, "aTargets": "_all" },

        ],
        columns: [
            { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
            { data: "DROrderNumber", "searchable": true, "visible": true, "name": "DROrderNumber" },
            { data: "OrderDate", "searchable": true, "visible": true, "name": "OrderDate" },
            { data: "Village", "searchable": true, "visible": true, "name": "Village" },
            { data: "CurrentPropertyType", "searchable": true, "visible": true, "name": "CurrentPropertyType" },
            { data: "CurrentNumber", "searchable": true, "visible": true, "name": "CurrentNumber" },
            { data: "OldPropertyType", "searchable": true, "visible": true, "name": "OldPropertyType" },
            { data: "OldNumber", "searchable": true, "visible": true, "name": "OldNumber" },
            { data: "Survey_No", "searchable": true, "visible": true, "name": "Survey_No" },
            { data: "Surnoc", "searchable": true, "visible": true, "name": "Surnoc" },
            { data: "Hissa_No", "searchable": true, "visible": true, "name": "Hissa_No" },
            { data: "CorrectionNote", "searchable": true, "visible": true, "name": "CorrectionNote" },
            { data: "Action", "searchable": true, "visible": true, "name": "Action" },

        ],
        fnInitComplete: function (oSettings, json) {

            var form = $('#PropertyNumberDetailsForm');
            form.append('<input type="hidden" id="PNDDocumentID" name="PNDDocumentID" value ="' + DocumentID + '" />');
            form.append('<input type="hidden" id="PNDPropertyID" name="PNDPropertyID" value ="' + PropertyID + '" />');

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
        },
        drawCallback: function (oSettings) {
            unBlockUI();
        },
    });
}