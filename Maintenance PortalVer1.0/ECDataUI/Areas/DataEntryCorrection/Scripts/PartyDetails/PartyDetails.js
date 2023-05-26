var token = '';
var header = {};

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    DocumentID = null;
    PropertyID = null;

    $('#AddPartyDetailsBtn').click(function () {
        DocumentID = $('#PNDDocumentID').val();
        PropertyID = $('#PNDPropertyID').val();
        AddPartyDetails(DocumentID, PropertyID);
    });


    //Added By Madhur 29-7-21
    //previousPropertyDetails(4, 0, "2368", 1, 2004);
    //Added By Madhur 29-7-21
});

//Added By Madhur 29-7-21
function previousPropertyDetails(SROOfficeID, DROfficeID, DocumentNumber, BookTypeID, FinancialYear) {
    var PropertyDetailPartyTabTableID = $('#PropertyDetailPartyTabTableID').DataTable({
        ajax: {
            url: '/DataEntryCorrection/PartyDetails/LoadPropertyDetailsPartyTabData',
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

                                $("#PropertyDetailPartyPageTableID").DataTable().clear().destroy();
                            }
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
            { "className": "SelToUnsel dt-center", "targets": [0] },
            { "className": "dt-center", "targets": "_all" },

            //{ orderable: false, targets: [1] },
            //{ orderable: false, targets: [2] },
            //{ orderable: false, targets: [3] },
            //{ orderable: false, targets: [4] }
        ],

        //columns: [{ data: "Select", "searchable": true, "visible": true, "name": "Select" },
        //{ data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
        //{ data: "PropertyDescription", "searchable": true, "visible": true, "name": "PropertyDescription" },
        //{ data: "PropertyNumberDetail", "searchable": true, "visible": true, "name": "PropertyNumberDetail" },
        //{ data: "ExecutionDate", "searchable": true, "visible": true, "name": "ExecutionDate" },
        //{ data: "NatureofDocument", "searchable": true, "visible": true, "name": "NatureofDocument" },
        //{ data: "Marketvalue", "searchable": true, "visible": true, "name": "Marketvalue" },
        //{ data: "Consideration", "searchable": true, "visible": true, "name": "Consideration" },
        //{ data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" },
        //{ data: "CDNumber", "searchable": true, "visible": true, "name": "CDNumber" },
        //{ data: "ViewBtn", "searchable": true, "visible": true, "name": "ViewBtn" }
        //],

        columns: [
            { data: "Select", "searchable": true, "visible": true, "name": "Select" },
            { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
            { data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" },
            { data: "ExecutionDate", "searchable": true, "visible": true, "name": "ExecutionDate" },
            { data: "NatureOfDocument", "searchable": true, "visible": true, "name": "NatureOfDocument" },
            { data: "PropertyDetails", "searchable": true, "visible": true, "name": "PropertyDetails" },
            { data: "VillageName", "searchable": true, "visible": true, "name": "VillageName" },
            { data: "TotalArea", "searchable": true, "visible": true, "name": "TotalArea" },
            { data: "ScheduleDescription", "searchable": true, "visible": true, "name": "ScheduleDescription" },
            { data: "Marketvalue", "searchable": true, "visible": true, "name": "Marketvalue" },
            { data: "Consideration", "searchable": true, "visible": true, "name": "Consideration" },
            { data: "Claimant", "searchable": true, "visible": true, "name": "Claimant" },
            { data: "Executant", "searchable": true, "visible": true, "name": "Executant" }
        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
            $("#EncryptedDocumentId").val(json.EncrytptedDocumetID);
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


function UnSelectBtnClick(ID) {
    var SELEID = '#SELE' + ID;

    $("#SelectTablePartyTabID").DataTable().clear().destroy();
    $(SELEID).closest(".SelToUnsel").html("<button type ='button' class='btn btn-group-md btn-primary selection' id='SELE" + ID + "' data-toggle='tooltip' data-placement='top' onclick='SelectBtnClick(" + ID + ")' title='Click here'>Select</ button>");
    var classToRemove10 = $('#DtlsToggleIconSearchParaDetail10').attr('class');
    var classToSet10 = (classToRemove10 == "fa fa-minus-square-o fa-pull-left fa-2x" ? $('#DtlsSearchParaListCollapseDetail10').click() : "fa fa-plus-square-o fa-pull-left fa-2x");

    DocumentID = null;
    PropertyID = null;
}


function SelectBtnClick(ID) {
    $("#SelectTablePartyTabID").DataTable().clear().destroy();
    $('.SelToUnsel').each(function () {

        var SID = $(this).attr('id');
        if (SID != undefined) {
            $(".SelToUnsel").html("<button type ='button' class='btn btn-group-md btn-primary selection' id='" + SID + "' data-toggle='tooltip' data-placement='top' onclick='SelectBtnClick(" + SID + ")' title='Click here'>Select</ button>");
        }

    });

    var flag = $('#SELE' + ID).closest('tr');

    //Commented and added by Madhusoodan 
    //var DocumentID = $("#PropertyDetailPartyTabTableID").DataTable().row(flag).data().DocumentID;
    //var PropertyID = $("#PropertyDetailPartyTabTableID").DataTable().row(flag).data().PropertyID;
    DocumentID = $("#PropertyDetailPartyTabTableID").DataTable().row(flag).data().DocumentID;
    PropertyID = $("#PropertyDetailPartyTabTableID").DataTable().row(flag).data().PropertyID;

    SelectBtnPopulation(DocumentID, PropertyID, ID);
}


function SelectBtnPopulation(DocumentID, PropertyID, ID) {


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
            { data: "PartyType", "searchable": true, "visible": true, "name": "PartyType" },
            { data: "FirstName", "searchable": true, "visible": true, "name": "FirstName" },
            { data: "MiddleName", "searchable": true, "visible": true, "name": "MiddleName" },
            { data: "LastName", "searchable": true, "visible": true, "name": "LastName" },
            { data: "CorrectionNote", "searchable": true, "visible": true, "name": "CorrectionNote" },
            { data: "Action", "searchable": true, "visible": true, "name": "Action" },

        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
            $("#PropertyDetailPartyTabTableID").DataTable().cell($('.SelToUnsel')).data("<button type ='button' class='btn btn-group-md btn-primary unselection' id='SELE" + ID + "' onclick='UnSelectBtnClick(" + ID + ")' data-toggle='tooltip' data-placement='top' title='Click here'>Unselect</ button>");
            $('#DtlsSearchParaListCollapseDetail9').click();
            var classToRemove10 = $('#DtlsToggleIconSearchParaDetail10').attr('class');
            var classToSet10 = (classToRemove10 == "fa fa-plus-square-o fa-pull-left fa-2x" ? $('#DtlsSearchParaListCollapseDetail10').click() : "fa fa-plus-square-o fa-pull-left fa-2x");
            

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

function DeletePartyDetails(KeyId) {

    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Do you want to Delete this record?</span>",

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
                    url: '/DataEntryCorrection/PartyDetails/DeletePartyDetails',
                    data: { "KeyId": KeyId },
                    type: "GET",
                    success: function (data) {
                        unBlockUI();
                        if (!data.serverError) {
                            if (data.success) {
                                bootbox.alert('<i class="fa fa-trash" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                                LoadPartyPopup($('#PNDDocumentID').val(), $('#PNDPropertyID').val());
                                //bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');
                            }
                            //bootbox.alert('<i class="fa fa-bin" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                        }
                        else {

                            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');

                        }
                    },
                    Error: function () {
                        unBlockUI();

                    }
                });
            }
        }
    });

    
}
function DeactivatePartyDetails(KeyId) {

    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Do you want to Deactivate this record?</span>",

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
                    url: '/DataEntryCorrection/PartyDetails/DeactivatePartyDetails',
                    data: { "KeyId": KeyId },
                    type: "GET",
                    success: function (data) {
                        unBlockUI();
                        if (!data.serverError) {
                            if (data.success) {
                                bootbox.alert('<i class="fa fa-ban" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                                LoadPartyPopup($('#PNDDocumentID').val(), $('#PNDPropertyID').val());
                                //bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');
                            }
                            //bootbox.alert('<i class="fa fa-bin" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                        }
                        else {

                            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');

                        }
                    },
                    Error: function () {
                        unBlockUI();

                    }
                });
            }
        }
    });

    
}

function EditBtnClickOrderTable(DROrderNumber) {
    $.ajax({
        url: '/DataEntryCorrection/PartyDetails/EditBtnClickOrderTable',
        data: { "DROrderNumber": DROrderNumber },
        type: "GET",
        success: function (data) {
            if (data != "Could not find file") {
                $('#divViewAbortModal').modal('show');
                $("#objPDFViewer").attr('data', '');
                $("#objPDFViewer").load(data);
                $("#objPDFViewer").attr('data', '/DataEntryCorrection/PartyDetails/ViewBtnClickOrderTable?DROrderNumber=' + DROrderNumber);

            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">File not exist at path.</span>');
            }
            unBlockUI();
        },
        error: function (xhr) {
            alert(xhr);
            unBlockUI();
        }
    });
}

//Added by Madhusoodan on 03/08/2021
function AddPartyDetails(DocumentID, PropertyID) {


    if (DocumentID == null || PropertyID == null) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select one Property from Property Details.</span>'
        });
    }
    else {

        //alert("DocumentID: " + DocumentID + " ----   PropertyID: " + PropertyID);

        var dataForPartyDetails = $("#PartyDetailsForm").serializeArray();
        dataForPartyDetails.push({ name: 'DocumentID', value: DocumentID }, { name: 'PropertyID', value: PropertyID });


        var form = $("#PartyDetailsForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);

        if ($("#PartyDetailsForm").valid()) {

            //alert("In PartyDetailsForm.valid()");
            //alert($("#PartyDetailsForm").serialize());

            blockUI();

            $.ajax({
                url: "/DataEntryCorrection/PartyDetails/AddUpdatePartyDetails",
                type: "POST",
                //data: $("#PartyDetailsForm").serialize(),
                data: dataForPartyDetails,
                dataType: "json",
                headers: header,
                success: function (data) {
                    unBlockUI();
                    if (data.success) {
                        //alert("data.success = true");

                        //Clear all text boxes and dropdowns after successfully saved
                        $(':input', '#PartyDetailsForm')
                            .not(':button, :submit, :reset, :hidden')
                            .val('');
                        
                        bootbox.alert({
                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                            callback: function () { }
                        });
                        LoadPartyPopup(DocumentID, PropertyID)
                    }
                    else {
                        //alert("data.success = false");

                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                        });
                    }
                },
                error: function () {

                    unBlockUI();

                    //alert("Last error func");

                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                    });
                }
            });
        }
    }
}

//Added By Madhur 29-7-21


$('#DtlsSearchParaListCollapseDetail9').click(function () {
    var classToRemove = $('#DtlsToggleIconSearchParaDetail9').attr('class');
    var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
    $('#DtlsToggleIconSearchParaDetail9').removeClass(classToRemove).addClass(classToSet);
});

$('#DtlsSearchParaListCollapseDetail10').click(function () {
    var classToRemove = $('#DtlsToggleIconSearchParaDetail10').attr('class');
    var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
    $('#DtlsToggleIconSearchParaDetail10').removeClass(classToRemove).addClass(classToSet);
});


