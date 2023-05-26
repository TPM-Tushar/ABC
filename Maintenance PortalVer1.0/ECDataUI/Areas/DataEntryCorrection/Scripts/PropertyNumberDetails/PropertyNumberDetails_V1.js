var token = '';
var header = {};
var test = '';
var form = '';
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    DocumentID = null;
    PropertyID = null;

    //previousPropertyDetails(4, 0, "2368", 1, 2004);
    //Check if working or not
    //previousPropertyDetails();


    $('.CurrentNoHideShowClass').show();
    $('.SurveyNoDetailsHideShowClass').hide();


    $('#CurrentPropertyTypeListID').change(function () {
        if ($('#CurrentPropertyTypeListID').val() != 0 && $('#CurrentPropertyTypeListID').val() != 1) {
            $('.SurveyNoDetailsHideShowClass').hide();
            $('.CurrentNoHideShowClass').show();
            $("#txtCurrentNumber").prop('required', true);
            $("#txtCurrentSurveyNumber").prop('required', false);
            $("#txtCurrentSurveyNoChar").prop('required', false);
            $("#txtCurrentHissaNo").prop('required', false);
            $('#txtCurrentSurveyNumber').val('');
            $('#txtCurrentSurveyNoChar').val('');
            $('#txtCurrentHissaNo').val('');

        }

        else if ($('#CurrentPropertyTypeListID').val() == 1) {
            //alert("")
            $('.SurveyNoDetailsHideShowClass').show();
            $('.CurrentNoHideShowClass').hide();
            $("#txtCurrentNumber").prop('required', false);
            $('#txtCurrentNumber').val('');

            $("#txtCurrentSurveyNumber").prop('required', true);
            $("#txtCurrentSurveyNoChar").prop('required', true);
            $("#txtCurrentHissaNo").prop('required', true);
        }
        else {
            $('.SurveyNoDetailsHideShowClass').hide();
            $('.CurrentNoHideShowClass').show();
            $('#txtCurrentNumber').val('');
            $('#txtCurrentSurveyNumber').val('');
            $('#txtCurrentSurveyNoChar').val('');
            $('#txtCurrentHissaNo').val('');

            $("#txtCurrentNumber").prop('required', false);
            $("#txtCurrentSurveyNumber").prop('required', false);
            $("#txtCurrentSurveyNoChar").prop('required', false);
            $("#txtCurrentHissaNo").prop('required', false);
        }
    });


    $('#OldPropertyTypeListID').change(function () {
        if ($('#OldPropertyTypeListID').val() != 0) {
            $("#txtOldNumber").prop('required', true);
        }
        else {
            $('#txtOldNumber').val('');
            $("#txtOldNumber").prop('required', false);
        }
    });

    //Save btn click
    $('#AddPNDetailsBtn').click(function () {
        DocumentID = $('#PNDDocumentID').val();
        PropertyID = $('#PNDPropertyID').val();


        if (DocumentID == null || PropertyID == null) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select one Property from Property Details.</span>'
            });
        }
        else {


          //  alert("SRO Selected :" + $('#SROListID option:selected').val());

            //var dataToSend = $("#PropertyNumberDetailsForm").serializeArray();
            //dataToSend.push({ name: 'DocumentID', value: DocumentID }, { name: 'PropertyID', value: PropertyID }, { name: '#SROListID', value: SROListID }, { name: '#VillageListID', value: VillageListID });


            //Added by Shivam B on 10/05/2022 For Validation of Sub Registrar 
            if ($('#SROListID').val() == "0") {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Sub Registrar</span>'
                });
                return;
            }

            if ($("#VillageListID").val() == "0") {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Village/Area</span>'
                });
                return;
            }
            //Added by Shivam B on 10/05/2022 For Validation of Sub Registrar 


            if ($("#CurrentPropertyTypeListID").val() == "0" &&
                $("#OldPropertyTypeListID").val() == "0") {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Current Property or Old Property type</span>'
                });
                return;
            }

            var trimmedCurrentNostr = $("#txtCurrentNumber").val().trim();


            if ($("#CurrentPropertyTypeListID").val() != "0") {

                if (trimmedCurrentNostr.length == 0 && $("#CurrentPropertyTypeListID").val() != "1") {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Current Property Number</span>'
                    });
                    return;
                }
            }

            if ($("#CurrentPropertyTypeListID").val() == "1") {
                $("#txtCurrentSurveyNoChar").removeAttr('required');
                $("#txtCurrentHissaNo").removeAttr('required');
            }

            var trimmedOldNostr = $("#txtOldNumber").val().trim();


            if ($("#OldPropertyTypeListID").val() != "0") {

                if (trimmedOldNostr.length == 0) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Old Property Number</span>'
                    });
                    return;
                }
            }

            if (trimmedOldNostr.length != 0) {
                if ($("#OldPropertyTypeListID").val() == "0") {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Old Property Type</span>'
                    });
                    return;
                }

            }


            if (trimmedCurrentNostr.length != 0) {
                if ($("#CurrentPropertyTypeListID").val() == "0") {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Current Property Type</span>'
                    });
                    return;
                }

            }


            //Added by Shivam B on 10-May-2022 for Checking Selected District Registrar is equal to Current Session District Registrar UserID

            if ($('#idHiddenDROCode').val() != $('#DROListID option:selected').val()) {

                var bootboxConfirm = bootbox.confirm({
                    title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
                    //message: "<span class='boot-alert-txt'>This Challan Refund Entry is already rejected. On 'Approve' Rejected Details will be deleted. Do you want to continue ?</span>",
                    //message: "<span class='boot-alert-txt'>You have selected district " + $('#DROListID option:selected').text() + " Please Note SRO " + $('#idHiddenSROName').val() + " district is " + $('#idHiddenDROName').val() + "</span>",
                    //message: "<span class='boot-alert-txt'> Please Note SRO " + $('#idHiddenSROName').val() + " district is " + $('#idHiddenDROName').val() + ", you have selected district " + $('#DROListID option:selected').text() + ".</span>",
                    message: "<span class='boot-alert-txt'> Please note you have selected " + $('#DROListID option:selected').text() + " district, SRO " + $('#idHiddenSROName').val() + " district is " + $('#idHiddenDROName').val() + ".</span>",
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
                            AddPropertyNoDetails(DocumentID, PropertyID);
                        }
                    }
                });
            }
            else {
                AddPropertyNoDetails(DocumentID, PropertyID);
            }
        }
        //Added by Shivam B on 10-May-2022 for Checking Selected District Registrar is equal to Current Session District Registrar UserID
            
        //}







        // //Added by Shivam B on 10-May-2022 for Checking Selected District Registrar is equal to Current Session District Registrar UserID

        //if ($('#idHiddenDROCode').val() != $('#DROListID option:selected').val()) {
            
        //    var bootboxConfirm = bootbox.confirm({
        //        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        //        //message: "<span class='boot-alert-txt'>This Challan Refund Entry is already rejected. On 'Approve' Rejected Details will be deleted. Do you want to continue ?</span>",
        //        message: "<span class='boot-alert-txt'>You have selected district " + $('#DROListID option:selected').text() + " Please Note SRO " + $('#idHiddenSROName').val() + " district is " + $('#idHiddenDROName').val() + "</span>",

        //        buttons: {
        //            cancel: {
        //                label: '<i class="fa fa-times"></i> No',
        //                className: 'pull-right margin-left-NoBtn'
        //            },
        //            confirm: {
        //                label: '<i class="fa fa-check"></i> Yes'
        //            }
        //        },
        //        callback: function (result) {
        //            if (result) {
        //                AddPropertyNoDetails(DocumentID, PropertyID);
        //            }
        //        }
        //    });
        //}
        //else
        //{ 
        //    AddPropertyNoDetails(DocumentID, PropertyID);
        //}

        ////Added by Shivam B on 10-May-2022 for Checking Selected District Registrar is equal to Current Session District Registrar UserID


        //AddPropertyNoDetails(DocumentID, PropertyID);
    });


    //Added by Shivam B on 10/05/2022 Populate Village on DRO Change
    $('#DROListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/DataEntryCorrection/PropertyNumberDetails/GetSROOfficeListByDistrictID',
            data: { "DroCode": $('#DROListID').val() },
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
                    $('#SROListID').empty();

                    //Added By Shivam B For DRO 
                    $('#VillageListID').empty();
                    var VillageListId = document.getElementById("VillageListID");
                    var option = document.createElement('option');
                    option.text ="Select", option.value = 0 ;
                    VillageListId.add(option, 0);


                    $.each(data.SROOfficeList, function (i, SROOfficeList) {
                        //SROOfficeList
                        $('#SROListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });
    //Added by Shivam B on 10/05/2022 Populate Village on DRO Change




    //Added by mayank on 16/08/2021 Populate Village on SRO Change
    $('#SROListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/DataEntryCorrection/PropertyNumberDetails/GetVillageBySROCode',
            data: { "SROCode": $('#SROListID').val() },
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
                    $('#VillageListID').empty();
                    $.each(data.VillageList, function (i, VillageList) {
                        //SROOfficeList
                        $('#VillageListID').append('<option value="' + VillageList.Value + '">' + VillageList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });
    //Added by mayank on 17/08/2021 Populate Village on SRO Change
    $('#VillageListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/DataEntryCorrection/PropertyNumberDetails/GetHobliDetailsOnVillageSroCode',
            data: { "SroCode": $('#SROListID').val(), "VillageCode": $("#VillageListID").val() },
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
                        //$('#VillageListID').empty();
                        //$.each(data.VillageList, function (i, VillageList) {
                        //    //SROOfficeList
                        //    $('#VillageListID').append('<option value="' + VillageList.Value + '">' + VillageList.Text + '</option>');
                        //});
                    $("#HobliNameDivID").html(data.HobliName);
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });






});
//Added by Madhur

//Commented by Madhusoodan on 02/08/2021 to remove hardcoded values
//function previousPropertyDetails(SROOfficeID, DROfficeID, DocumentNumber, BookTypeID, FinancialYear) {
function previousPropertyDetails() {

    var PropertyDetailTableID = $('#PropertyDetailTableID').DataTable({
        ajax: {
            url: '/DataEntryCorrection/PropertyNumberDetails/LoadPropertyDetailsData',
            type: "POST",
            headers: header,
            //Commented by Madhusoodan on 02/08/2021 to remove hardcoded values
            //data: {
            //    'SROOfficeID': SROOfficeID, 'DROfficeID': DROfficeID, 'DocumentNumber': DocumentNumber, 'BookTypeID': BookTypeID, 'FinancialYear': FinancialYear,
            //},
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {


                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            } else {

                                $("#PropertyDetailTableID").DataTable().clear().destroy();
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
                blockUI('loading data.. please wait...');
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

        ],


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




//Added By Madhur 29-7-21


function UnSelectBtnClick(ID) {
    var SELEID = '#SELE' + ID;
    // var flag = $(SELEID).closest('tr');
    $("#SelectTableID").DataTable().clear().destroy();
    //$("#PropertyDetailTableID").DataTable().cell(flag).data("<button type ='button' class='btn btn-group-md btn-primary selection' id='SELE" + ID + "' data-toggle='tooltip' data-placement='top' onclick='SelectBtnClick(" + ID +")' title='Click here'>Select</ button>");
    $(SELEID).closest(".SelToUnsel").html("<button type ='button' class='btn btn-group-md btn-primary selection' id='SELE" + ID + "' data-toggle='tooltip' data-placement='top' onclick='SelectBtnClick(" + ID + ")' title='Click here'>Select</ button>");
    var classToRemove6 = $('#DtlsToggleIconSearchParaDetail6').attr('class');
    if (classToRemove6 == "fa fa-minus-square-o fa-pull-left fa-2x") {
        $('#DtlsSearchParaListCollapseDetail6').click();
    }

    DocumentID = null;
    PropertyID = null;
}


function SelectBtnClick(ID) {
    $("#SelectTableID").DataTable().clear().destroy();
    $('.unselection').each(function () {
        var SID = $(this).attr('id');
        var SFID = SID.replace('SELE', '');
        $(this).closest('.SelToUnsel').html("<button type ='button' class='btn btn-group-md btn-primary selection' id='" + SID + "' data-toggle='tooltip' data-placement='top' onclick='SelectBtnClick(" + SFID + ")' title='Click here'>Select</ button>");
    });

    var flag = $('#SELE' + ID).closest('tr');

    //Commented and added by Madhusoodan on 03/08/2021
    //var DocumentID = $("#PropertyDetailTableID").DataTable().row(flag).data().DocumentID;
    //var PropertyID = $("#PropertyDetailTableID").DataTable().row(flag).data().PropertyID;

    DocumentID = $("#PropertyDetailTableID").DataTable().row(flag).data().DocumentID;
    PropertyID = $("#PropertyDetailTableID").DataTable().row(flag).data().PropertyID;

    SelectBtnPopulation(DocumentID, PropertyID, ID);
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
        ordering: false,


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


            $("#PropertyDetailTableID").DataTable().cell($('#SELE' + ID).closest('.SelToUnsel')).data("<button type ='button' class='btn btn-group-md btn-primary unselection' id='SELE" + ID + "' onclick='UnSelectBtnClick(" + ID + ")' data-toggle='tooltip' data-placement='top' title='Click here'>Unselect</ button>");


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

function DeletePropertyNoDetails(keyId) {

    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        //message: "<span class='boot-alert-txt'>Do you want to Delete <br>Village Name : " + VillageName + " <br>  Property Type :" + CurrentPropertyType + "<br> Property Number :" + PropertyNo + "?</span>",
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
                    url: '/DataEntryCorrection/PropertyNumberDetails/DeletePropertyNoDetails',
                    data: { "KeyId": keyId },
                    type: "GET",
                    contentType: "json",
                    success: function (data) {
                        //test = data;
                        //alert(data);
                        //alert(data.serverError);
                        //alert(data.success);
                        //alert(data.Message);
                        unBlockUI();
                        if (!data.serverError) {
                            if (data.success) {
                                bootbox.alert('<i class="fa fa-trash" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                                LoadPropertyPopup($('#PNDDocumentID').val(), $('#PNDPropertyID').val());
                                //bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');
                            }
                            //bootbox.alert('<i class="fa fa-bin" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                        }
                        else {

                            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');

                        }
                    },
                    error: function (xhr) {
                        //alert("error " + xhr);
                        unBlockUI();
                        test = xhr;
                        var err = eval("(" + xhr.responseText + ")");
                        alert(err.Message);
                    }
                });
            }
        }
    });
}

function EditBtnClickOrderTable(DROrderNumber) {
    $.ajax({
        url: '/DataEntryCorrection/PropertyNumberDetails/EditBtnClickOrderTable',
        data: { "DROrderNumber": DROrderNumber },
        type: "GET",
        success: function (data) {
            if (data != "Could not find file") {
                $('#divViewAbortModal').modal('show');
                $("#objPDFViewer").attr('data', '');
                $("#objPDFViewer").load(data);
                $("#objPDFViewer").attr('data', '/DataEntryCorrection/PropertyNumberDetails/ViewBtnClickOrderTable?DROrderNumber=' + DROrderNumber);

            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">File not exist at path.</span>');
            }
            unBlockUI();
        },
        error: function (xhr, status, err) {
            var err = eval("(" + xhr.responseText + ")");
            alert(err.Message);
        }
    });
}


//Added by Madhusoodan on 03/08/2021
function AddPropertyNoDetails(DocumentID, PropertyID) {

    //alert("hello");

    var dataToSend = $("#PropertyNumberDetailsForm").serializeArray();
    dataToSend.push({ name: 'DocumentID', value: DocumentID }, { name: 'PropertyID', value: PropertyID }),

    
        form = $("#PropertyNumberDetailsForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);

        //alert("9");


        //if ($("#PropertyNumberDetailsForm").valid()) {
        if (form.valid()) {

            //alert("In PropertyNumberDetailsForm.valid()");
            //alert($("#PropertyNumberDetailsForm").serialize());
            //alert("DataToSend: " + dataToSend);
            //alert("10");


            blockUI();
            //alert("before ajax call to save")
            $.ajax({
                url: "/DataEntryCorrection/PropertyNumberDetails/AddUpdatePropertyNoDetails",
                type: "POST",
                //data: $("#PropertyNumberDetailsForm").serialize(),
                data: dataToSend,
                dataType: "json",
                headers: header,
                success: function (data) {
                    unBlockUI();
                    if (data.success) {
                        //alert("data.success = true");

                        //Clear all text boxes and dropdowns after successfully saved
                        $(':input', '#PropertyNumberDetailsForm')
                            .not(':button, :submit, :reset, :hidden')
                            .val('');

                        $('#VillageListID').val(0);


                        //Commented by Madhusoodna on 09/08/2021

                        ////
                        //Changed by mayank on 10-08-2021
                        //PropertyPopupPopulation(DocumentID, PropertyID);
                        LoadPropertyPopup(DocumentID, PropertyID);

                        bootbox.alert({
                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                            callback: function () { }
                        });



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

////Added by Madhusoodan on 03/08/2021
//function AddPropertyNoDetails(DocumentID, PropertyID) {

//    //alert("1");
//    if (DocumentID == null || PropertyID == null) {
//        bootbox.alert({
//            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select one Property from Property Details.</span>'
//        });
//    }
//    else {

//        //alert("2");

//        var dataToSend = $("#PropertyNumberDetailsForm").serializeArray();
//        dataToSend.push({ name: 'DocumentID', value: DocumentID }, { name: 'PropertyID', value: PropertyID }, { name: '#SROListID', value: SROListID }, { name: '#VillageListID', value: VillageListID });


//        //Added by Shivam B on 10/05/2022 For Validation of Sub Registrar 
//        if ($('#SROListID').val() == "0") {
//            bootbox.alert({
//                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Sub Registrar</span>'
//            });
//            return;
//        }
        
//        if ($("#VillageListID").val() == "0") {
//            bootbox.alert({
//                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Village/Area</span>'
//            });
//            return;
//        }
//        //Added by Shivam B on 10/05/2022 For Validation of Sub Registrar 


//        if ($("#CurrentPropertyTypeListID").val() == "0" &&
//            $("#OldPropertyTypeListID").val() == "0") {
//            bootbox.alert({
//                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Current Property or Old Property type</span>'
//            });
//            return;
//        }
//        //alert("3");

//        var trimmedCurrentNostr = $("#txtCurrentNumber").val().trim();

//        //alert("4");

//        if ($("#CurrentPropertyTypeListID").val() != "0") {

//            if (trimmedCurrentNostr.length == 0 && $("#CurrentPropertyTypeListID").val() != "1") {
//                bootbox.alert({
//                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Current Property Number</span>'
//                });
//                return;
//            }
//        }

//        if ($("#CurrentPropertyTypeListID").val() == "1") {
//            $("#txtCurrentSurveyNoChar").removeAttr('required');
//            $("#txtCurrentHissaNo").removeAttr('required');
//        }
//        //alert("5");

//        var trimmedOldNostr = $("#txtOldNumber").val().trim();


//        if ($("#OldPropertyTypeListID").val() != "0") {

//            if (trimmedOldNostr.length == 0) {
//                bootbox.alert({
//                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Old Property Number</span>'
//                });
//                return;
//            }
//        }


//        //alert("6");

//        if (trimmedOldNostr.length != 0) {
//            if ($("#OldPropertyTypeListID").val() == "0") {
//                bootbox.alert({
//                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Old Property Type</span>'
//                });
//                return;
//            }

//        }
//        //alert("7");


//        if (trimmedCurrentNostr.length != 0) {
//            if ($("#CurrentPropertyTypeListID").val() == "0") {
//                bootbox.alert({
//                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Current Property Type</span>'
//                });
//                return;
//            }

//        }
        




//        //if (!/^\s*$/.test($("#txtCurrentNumber").val()) && $("#CurrentPropertyTypeListID").val() == "0") {
//        //    bootbox.alert({
//        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Current or Old Property type</span>'
//        //    });
//        //    return;
//        //}

//        //if ($("#txtCurrentNumber").val().length != 0) {
//        //    if (!(/^\s*$/.test($("#txtCurrentNumber").val()) && $("#CurrentPropertyTypeListID").val() == "0")) {
//        //        bootbox.alert({
//        //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Current Property type</span>'
//        //        });
//        //        return;
//        //    }
//        //}
//        //if ($("#txtOldNumber").val().length != 0) {
//        //    if (!(/^\s*$/.test($("#txtOldNumber").val()) && $("#OldPropertyTypeListID").val() == "0")) {
//        //        bootbox.alert({
//        //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Select Old Property type</span>'
//        //        });
//        //        return;
//        //    }
//        //}
//        //if ($("#CurrentPropertyTypeListID").val() == "1") {
//        //    if (!/^\s*$/.test($("#txtCurrentSurveyNumber").val()) && $("#CurrentPropertyTypeListID").val() == "1") {
//        //        bootbox.alert({
//        //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Survey Number</span>'
//        //        });
//        //        return;
//        //    }
//        //    else if(!isNaN($("#txtCurrentSurveyNumber").val())){
//        //        bootbox.alert({
//        //            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please Enter Survey Number in numeric form</span>'
//        //        });
//        //        return;
//        //    }
//        //}

//        ////if()

//        //if ($("#OldPropertyTypeListID").val() == "1")
//        ////









//         form = $("#PropertyNumberDetailsForm");
//        form.removeData('validator');
//        form.removeData('unobtrusiveValidation');
//        $.validator.unobtrusive.parse(form);

//        //alert("9");

        
//        //if ($("#PropertyNumberDetailsForm").valid()) {
//            if (form.valid()) {

//            //alert("In PropertyNumberDetailsForm.valid()");
//            //alert($("#PropertyNumberDetailsForm").serialize());
//            //alert("DataToSend: " + dataToSend);
//            //alert("10");


//            blockUI();
//            //alert("before ajax call to save")
//            $.ajax({
//                url: "/DataEntryCorrection/PropertyNumberDetails/AddUpdatePropertyNoDetails",
//                type: "POST",
//                //data: $("#PropertyNumberDetailsForm").serialize(),
//                data: dataToSend,
//                dataType: "json",
//                headers: header,
//                success: function (data) {
//                    unBlockUI();
//                    if (data.success) {
//                        //alert("data.success = true");

//                        //Clear all text boxes and dropdowns after successfully saved
//                        $(':input', '#PropertyNumberDetailsForm')
//                            .not(':button, :submit, :reset, :hidden')
//                            .val('');

//                        $('#VillageListID').val(0);


//                        //Commented by Madhusoodna on 09/08/2021

//                        ////
//                        //Changed by mayank on 10-08-2021
//                        //PropertyPopupPopulation(DocumentID, PropertyID);
//                        LoadPropertyPopup(DocumentID, PropertyID);

//                        bootbox.alert({
//                            message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
//                            callback: function () { }
//                        });



//                    }
//                    else {
//                        //alert("data.success = false");

//                        bootbox.alert({
//                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
//                        });
//                    }
//                },
//                error: function () {

//                    unBlockUI();

//                    //alert("Last error func");

//                    bootbox.alert({
//                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
//                    });
//                }
//            });
//        }
//        //else {
//        //    alert("11");
//        //}
//    }
//}

//Added By Madhur 29-7-21

$('#DtlsSearchParaListCollapseDetail5').click(function () {
    var classToRemove = $('#DtlsToggleIconSearchParaDetail5').attr('class');
    var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
    $('#DtlsToggleIconSearchParaDetail5').removeClass(classToRemove).addClass(classToSet);
});

$('#DtlsSearchParaListCollapseDetail6').click(function () {
    var classToRemove = $('#DtlsToggleIconSearchParaDetail6').attr('class');
    var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
    $('#DtlsToggleIconSearchParaDetail6').removeClass(classToRemove).addClass(classToSet);
});

//Added by Madhusoodan on 05/08/2021
function EditSelPropertyNoDetails() {

    //alert("In EditSelPropertyNoDetails OrderID: " + orderID);

    blockUI('Loading Property Number Details for seleted Property.. please wait...');

    $.ajax({
        url: '/DataEntryCorrection/PropertyNumberDetails/LoadAddNewOrderView',
        headers: header,
        data: { orderID },
        datatype: "json",
        type: "POST",
        success: function (data) {

            //alert("In data");

            if (data.serverError == true) {

                //alert("In data.ServerError= true");

                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                //alert("In else.. Loading Partial View Order");

                $('#panelAllTabs').show();

                $('#LoadDECInsertUpdateDeleteViewDivID').html('');

                $('#LoadDECInsertUpdateDeleteViewDivID').html(data);
                $('#AddEditOrderTabDivID').addClass('active');
            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });
}

function EditBtnProperty(orderID) {

    blockUI('loading data.. please wait...');

    $.ajax({
        url: '/DataEntryCorrection/PropertyNumberDetails/EditBtnProperty',
        headers: header,
        data: { 'OrderID': orderID },
        datatype: "json",
        type: "Get",
        success: function (data) {

            if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        window.location.href = "/Home/HomePage"
                    });
            }
            else {
                //alert("In else.. Loading Partial View Order");
                //console.log(data);
                //if (data != "Error") {

                //}
                //else if( data)
            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });

}

function DeactivatePropertyNoDetails(KeyId) {
    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        //message: "<span class='boot-alert-txt'>Do you want to Delete <br>Village Name : " + VillageName + " <br>  Property Type :" + CurrentPropertyType + "<br> Property Number :" + PropertyNo + "?</span>",
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
                    url: '/DataEntryCorrection/PropertyNumberDetails/DeactivatePropertyNoDetails',
                    data: { "keyID": KeyId },
                    type: "GET",
                    contentType: "json",
                    headers: header,
                    success: function (data) {
                        //test = data;
                        //alert(data);
                        //alert(data.serverError);
                        //alert(data.success);
                        //alert(data.Message);
                        unBlockUI();
                        if (!data.serverError) {
                            if (data.success) {
                                bootbox.alert('<i class="fa fa-ban" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                                LoadPropertyPopup($('#PNDDocumentID').val(), $('#PNDPropertyID').val());
                                //bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');
                            }
                            //bootbox.alert('<i class="fa fa-bin" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                        }
                        else {

                            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');

                        }
                    },
                    error: function (xhr) {
                        //alert("error " + xhr);
                        unBlockUI();
                        test = xhr;
                        var err = eval("(" + xhr.responseText + ")");
                        alert(err.Message);
                    }
                });
            }
        }
    });
}

//Added by Madhusoodan on 18/08/2021 to activate PropertyNoDetails
function ActivatePropertyNoDetails(KeyId) {

    var bootboxConfirm = bootbox.confirm({
        title: "<span class='boot-alert-title'><i class='fa fa-question-circle'></i>&nbsp;&nbsp;Confirm</span>",
        message: "<span class='boot-alert-txt'>Do you want to activate this record?</span>",

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
                    url: '/DataEntryCorrection/PropertyNumberDetails/ActivatePropertyNoDetails',
                    data: { "KeyId": KeyId },
                    type: "GET",
                    contentType: "json",
                    success: function (data) {
                        
                        unBlockUI();
                        if (!data.serverError) {
                            if (data.success) {
                                bootbox.alert('<i class="fa fa-ban" style="font-size: 15px;margin: 0 8px;"></i><span class="boot-alert-txt" style="font-size: 16px;">' + data.Message + '</span>');
                                LoadPropertyPopup($('#PNDDocumentID').val(), $('#PNDPropertyID').val());   
                            }
                        }
                        else {
                            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>');
                        }
                    },
                    error: function (xhr) {
                        unBlockUI();
                        var err = eval("(" + xhr.responseText + ")");
                        //alert(err.Message);
                    }
                });
            }
        }
    });
}
//function PropertyPopupPopulation(DocumentID, PropertyID) {
//    $('#SelectTableID').DataTable({
//        ajax: {
//            url: '/DataEntryCorrection/PropertyNumberDetails/SelectBtnClick',
//            type: "POST",
//            headers: header,
//            data: {
//                'DocumentID': DocumentID, 'PropertyID': PropertyID,
//            },
//            dataSrc: function (json) {
//                unBlockUI();
//                if (json.errorMessage != null) {

//                    bootbox.alert({
//                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',

//                    });
//                    return;
//                }
//                else {
//                }
//                unBlockUI();
//                return json.data;
//            },
//            error: function () {
//                unBlockUI();
//                bootbox.alert({
//                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
//                    callback: function () {
//                    }
//                });
//            },
//            beforeSend: function () {
//                blockUI('loading data.. please wait...');
//            }
//        },
//        serverSide: true,
//        scrollCollapse: true,
//        bPaginate: true,
//        bLengthChange: true,
//        bInfo: true,
//        info: true,
//        bFilter: false,
//        searching: true,
//        "destroy": true,
//        "bAutoWidth": true,
//        "bScrollAutoCss": true,

//        columnDefs: [
//            { "className": "dt-center", "targets": "_all" },
//        ],
//        columns: [
//            { data: "Sno", "searchable": true, "visible": true, "name": "Sno" },
//            { data: "DROrderNumber", "searchable": true, "visible": true, "name": "DROrderNumber" },
//            { data: "OrderDate", "searchable": true, "visible": true, "name": "OrderDate" },
//            { data: "Village", "searchable": true, "visible": true, "name": "Village" },
//            { data: "CurrentPropertyType", "searchable": true, "visible": true, "name": "CurrentPropertyType" },
//            { data: "CurrentNumber", "searchable": true, "visible": true, "name": "CurrentNumber" },
//            { data: "OldPropertyType", "searchable": true, "visible": true, "name": "OldPropertyType" },
//            { data: "OldNumber", "searchable": true, "visible": true, "name": "OldNumber" },
//            { data: "Survey_No", "searchable": true, "visible": true, "name": "Survey_No" },
//            { data: "Surnoc", "searchable": true, "visible": true, "name": "Surnoc" },
//            { data: "Hissa_No", "searchable": true, "visible": true, "name": "Hissa_No" },
//            { data: "CorrectionNote", "searchable": true, "visible": true, "name": "CorrectionNote" },
//            { data: "Action", "searchable": true, "visible": true, "name": "Action" },

//        ],
//        fnInitComplete: function (oSettings, json) {

//        },
//        preDrawCallback: function () {
//            unBlockUI();
//        },
//        fnRowCallback: function (nRow, aData, iDisplayIndex) {

//            //if (ModuleID == 1) {
//            //    fnSetColumnVis(2, false);
//            //}
//            //else if (ModuleID == 2) {
//            //    fnSetColumnVis(0, false);
//            //    fnSetColumnVis(1, false);
//            //}

//            unBlockUI();
//        },
//        drawCallback: function (oSettings) {
//            unBlockUI();
//        },
//    });
//}

