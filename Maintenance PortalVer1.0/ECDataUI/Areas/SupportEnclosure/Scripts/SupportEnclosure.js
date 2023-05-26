/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosure.js
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   JS for Support Enclosure
*/


var token = '';
var header = {};

$(document).ready(function () {




    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $('#DtlsSearchPartyEnclosureListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPartyEnclosure').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPartyEnclosure').removeClass(classToRemove).addClass(classToSet);
    });


    var SROOfficeID;
    var DROfficeID;
    var DocumentNumber;
    var BookTypeID;
    var FinancialYear;


    $('#DROOfficeListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/SupportEnclosure/SupportEnclosureDetails/GetSROOfficeListByDistrictID',
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


    $("#SearchBtn").click(function () {

        var form = $("#SupportEnclosureForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);


        //alert($("#SupportEnclosureForm").serialize());

        //alert($("#SupportEnclosureForm").valid());

        if ($("#SupportEnclosureForm").valid()) {

            SROOfficeID = $("#SROOfficeListID option:selected").val();
            DROfficeID = $("#DROOfficeListID option:selected").val();
            DocumentNumber = $("#txtDocumentNumber").val();
            BookTypeID = $("#BookTypeListID option:selected").val();
            FinancialYear = $("#FinancialYearListID option:selected").val();

            if ($.fn.DataTable.isDataTable("#EnclosureDocumentID")) {
                $("#EnclosureDocumentID").DataTable().clear().destroy();
            }

            if ($.fn.DataTable.isDataTable("#EnclosureDocumentID")) {
                $("#EnclosureDocumentID").DataTable().clear().destroy();
            }

            if ($.fn.DataTable.isDataTable("#EnclosurePartyID")) {
                $("#EnclosurePartyID").DataTable().clear().destroy();
            }

            if ($.fn.DataTable.isDataTable("#EnclosurePartyID")) {
                $("#EnclosurePartyID").DataTable().clear().destroy();
            }


            if (SROOfficeID == 0 || DROfficeID == 0) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Office Details</span>'
                });
                return;
            }

            if (DocumentNumber == "" || DocumentNumber == 0) {
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
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Book Type</span>'
                });
                return;
            }

            if (FinancialYear == "" || FinancialYear == "Select") {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid Financial Year</span>'
                });
                return;
            }


            var DocumentEnclosureDetailsTable = $('#EnclosureDocumentID').DataTable({
                ajax: {
                    url: '/SupportEnclosure/SupportEnclosureDetails/LoadSupportDocumentEnclosureTableData',
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
                                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                            $('#DtlsSearchParaListCollapse').trigger('click');
                                        $("#EnclosureDocumentID").DataTable().clear().destroy();
                                    }
                                }
                            });
                            return;
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
                        unBlockUI();
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                            callback: function () {
                            }
                        });
                    },
                    beforeSend: function () {
                        blockUI('loading data.. please wait...');
                        var searchString = $('#EnclosureDocumentID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;

                            if (!regexToMatch.test(searchString)) {
                                $("#EnclosureDocumentID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    DocumentEnclosureDetailsTable.search('').draw();
                                    $("#EnclosureDocumentID_filter input").prop("disabled", false);
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
                    //{ orderable: false, targets: [1] },
                    //{ orderable: false, targets: [2] },
                    //{ orderable: false, targets: [3] },
                    //{ orderable: false, targets: [4] }
                ],

                columns: [
                    {
                        data: "SrNo", "searchable": true, "visible": true, "name": "SrNo"
                    },
                    {
                        data: "OfficeName", "searchable": true, "visible": false, "name": "OfficeName"
                    },
                    {
                        data: "DocumentID", "searchable": true, "visible": false, "name": "DocumentID"
                    },
                    {
                        data: "DocumentNumber", "searchable": true, "visible": true, "name": "DocumentNumber"
                    },
                    {
                        data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber"
                    },
                    {
                        data: "SupportDocumentTypeID", "searchable": true, "visible": false, "name": "SupportDocumentTypeID"
                    },
                    {
                        data: "SupportDocumentType", "searchable": true, "visible": true, "name": "SupportDocumentType"
                    },
                    {
                        data: "UploadDateTime", "searchable": true, "visible": true, "name": "UploadDateTime"
                    },
                    {
                        data: "FileName", "searchable": true, "visible": true, "name": "FileName"
                    },
                    {
                        data: "DownloadEnclosureButton", "searchable": false, "visible": true, "name": "DownloadEnclosureButton"
                    }
                ],
                fnInitComplete: function (oSettings, json) {
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

            var PartyEnclosureDetailsTable = $('#EnclosurePartyID').DataTable({
                ajax: {
                    url: '/SupportEnclosure/SupportEnclosureDetails/LoadSupportPartyEnclosureTableData',
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
                                        var classToRemove = $('#DtlsToggleIconSearchPartyEnclosure').attr('class');
                                        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                            $('#DtlsSearchPartyEnclosureListCollapse').trigger('click');
                                        $("#EnclosurePartyID").DataTable().clear().destroy();
                                    }
                                }
                            });
                            return;
                        }
                        else {
                            var classToRemove = $('#DtlsToggleIconSearchPartyEnclosure').attr('class');
                            if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                                $('#DtlsSearchPartyEnclosureListCollapse').trigger('click');
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
                        var searchString = $('#EnclosurePartyID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;

                            if (!regexToMatch.test(searchString)) {
                                $("#EnclosurePartyID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    PartyEnclosureDetailsTable.search('').draw();
                                    $("#EnclosurePartyID_filter input").prop("disabled", false);
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
                    //{ orderable: false, targets: [1] },
                    //{ orderable: false, targets: [2] },
                    //{ orderable: false, targets: [3] },
                    //{ orderable: false, targets: [4] }
                ],

                columns: [
                    {
                        data: "SrNo", "searchable": true, "visible": true, "name": "SrNo"
                    },
                    {
                        data: "OfficeName", "searchable": true, "visible": false, "name": "OfficeName"
                    },
                    {
                        data: "PartyID", "searchable": true, "visible": false, "name": "PartyID"
                    },
                    {
                        data: "PartyName", "searchable": true, "visible": true, "name": "PartyName"
                    },
                    {
                        data: "DocumentID", "searchable": true, "visible": false, "name": "DocumentID"
                    },
                    {
                        data: "DocumentNumber", "searchable": true, "visible": true, "name": "DocumentNumber"
                    },
                    {
                        data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber"
                    },
                    {
                        data: "SupportDocumentTypeID", "searchable": true, "visible": false, "name": "SupportDocumentTypeID"
                    },
                    {
                        data: "SupportDocumentType", "searchable": true, "visible": true, "name": "SupportDocumentType"
                    },
                    {
                        data: "UploadDateTime", "searchable": true, "visible": true, "name": "UploadDateTime"
                    },
                    {
                        data: "FileName", "searchable": true, "visible": true, "name": "FileName"
                    },
                    {
                        data: "DownloadEnclosureButton", "searchable": false, "visible": true, "name": "DownloadEnclosureButton"
                    }
                ],
                fnInitComplete: function (oSettings, json) {
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
        //else {
        //    bootbox.alert({
        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter required details</span>'
        //    });
        //    return;
        //}
    });


});

function DownLoadEnclosureFile(FilePath, FileName) {
    window.location.href = "/SupportEnclosure/SupportEnclosureDetails/DownLoadEnclosureFile?Path=" + FilePath + "&FileName=" + FileName;
}


