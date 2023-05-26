
var token = '';
var header = {};

$(document).ready(function () {

    var SROOfficeID;
    var FromYear;
    var ToYear;
    var FromMonth;
    var ToMonth;

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $('#dvJobRegistrtionForm').hide();


    $("input[name='OfficeTypeToGetDropDown']").click(function () {
        var radioValue = $("input[name='OfficeTypeToGetDropDown']:checked").val();
        blockUI('PLEASE WAIT');
        $.ajax({
            url: '/XELFiles/XELFilesDetails/GetOfficeList',
            datatype: "text",
            type: "GET",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            data: { "OfficeType": radioValue },
            success: function (data) {
                // ADDED BY SHUBHAM BHAGAT ON 21-05-2019 AT 6:36 PM
                if (data.errorMessage == undefined) {
                    $('#SROOfficeListID').empty();
                    $.each(data.OfficeList, function (i, OfficeListItem) {
                        $('#SROOfficeListID').append('<option value="' + OfficeListItem.Value + '">' + OfficeListItem.Text + '</option>');
                    });
                }
                else {
                    bootbox.alert({
                        //size: 'small',
                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    });
                }


                unBlockUI();
            },
            error: function (xhr) {
                bootbox.alert({
                    //size: 'small',
                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                });
                unBlockUI();
            }
        });
    });
    $('#btnAddNewJobDetails').click(function () {

        $('#dvJobRegistrtionForm').show();
        $('#dvBtnAddNewJobReq').hide();

    });

    $('#btncloseJobRegistrationForm').click(function () {

        $('#dvJobRegistrtionForm').hide();
        $('#dvBtnAddNewJobReq').show();

    });





    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    if ($.fn.DataTable.isDataTable("#RegisteredJobsTblID")) {
        $("#RegisteredJobsTblID").DataTable().clear().destroy();
    }

    var DocumentEnclosureDetailsTable = $('#RegisteredJobsTblID').DataTable({
        ajax: {
            url: '/XELFiles/XELFilesDetails/LoadRegisteredJobsTableData',
            type: "POST",
            headers: header,
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
                                $("#RegisteredJobsTblID").DataTable().clear().destroy();
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
                        $("#RegisteredJobsTblID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            DocumentEnclosureDetailsTable.search('').draw();
                            $("#RegisteredJobsTblID_filter input").prop("disabled", false);
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
                data: "JobID", "searchable": true, "visible": true, "name": "JobID"
            },
            {
                data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName"
            },
            {
                data: "FromMonth", "searchable": true, "visible": true, "name": "FromMonth"
            },
            {
                data: "FromYear", "searchable": true, "visible": true, "name": "FromYear"
            },
            {
                data: "ToMonth", "searchable": true, "visible": true, "name": "ToMonth"
            },
            {
                data: "ToYear", "searchable": true, "visible": true, "name": "ToYear"
            },
            {
                data: "RegisteredDateTime", "searchable": true, "visible": true, "name": "RegisteredDateTime"
            },
            {
                data: "IsJobCompleted", "searchable": true, "visible": true, "name": "IsJobCompleted",
                "render": function (data, type, row) {
                    return (data == true) ? '<span class="glyphicon glyphicon-ok" style="color:green"> </span > ' : '<span class="glyphicon glyphicon-remove" style="color:red"></span>';
                }
            },
            {
                data: "CompletedDateTime", "searchable": true, "visible": true, "name": "CompletedDateTime"
            },
            {
                data: "Description", "searchable": true, "visible": true, "name": "Description"
            },
              {
                  data: "OfficeType", "searchable": true, "visible": true, "name": "OfficeType"
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

    $("#btnRegisterJob").click(function () {

        var form = $("#SupportEnclosureForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);


        SROOfficeID = $("#SROOfficeListID option:selected").val();
        FromYear = $("#FromYearListID option:selected").val();
        ToYear = $("#ToYearListID option:selected").val();
        FromMonth = $("#FromMonthListID option:selected").val();
        ToMonth = $("#ToMonthListID option:selected").val();

        if (SROOfficeID == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select SRO Office</span>',
                callback: function () {

                }
            });
        }
        else if (FromYear == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Year</span>',
                callback: function () {

                }
            });
        }
        else if (ToYear == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Year</span>',
                callback: function () {

                }
            });
        }
        else if (FromMonth == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Month</span>',
                callback: function () {

                }
            });
        }
        else if (ToMonth == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Month</span>',
                callback: function () {

                }
            });
        }
        else {
            RegisterJobsDetail();
        }

    });

});



function RegisterJobsDetail() {
    var radioValue = $("input[name='OfficeTypeToGetDropDown']:checked").val();

    $("#OfficeType").val(radioValue);
    var form = $("#RegisterJobsForm");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    if ($("#RegisterJobsForm").valid()) {

        $.ajax({
            type: "POST",
            url: "/XELFiles/XELFilesDetails/RegisterJobsDetails",
            data: $("#RegisterJobsForm").serialize(),
            headers: header,
            success: function (data) {
                if (data.success) {
                    bootbox.alert({
                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                        callback: function () {
                            //Reload Page
                            window.location.href = "/XELFiles/XELFilesDetails/RegisterJobs"
                        }
                    });
                }
                else {
                    if (data.message == undefined) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured. because " + ' <ul style="list-style-type: disc;font-weight:normal">' + '<li> Either Duplicate Session Found</li>' + '<li>Invalid Request</li>' + ' <li> Poor network connectivity</li>' + '</span>',
                            callback: function () {

                                window.location.href = '/Error/SessionExpire';
                                //    $('#btnLoggOffID').trigger('click');
                                $.unblockUI();
                                //   return false;
                            }
                        });
                    }
                    else {

                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                        });
                    }
                }
                $.unblockUI();
            },
            error: function () {
                bootbox.alert({
                    //   size: 'small',
                    //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
                });
            }
        });

    }





}

