var token = '';
var header = {};
var SelectedSRO;

$(document).ready(function () {

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var TodaysDate = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();
    var SROOfficeID;
    var FromDate;
    var ToDate;

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


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

    //$('#txtToDate').datepicker({
    //    format: 'dd/mm/yyyy',
    //}).datepicker("setDate", TodaysDate);


    //$('#txtFromDate').datepicker({
    //    format: 'dd/mm/yyyy',
    //    changeMonth: true,
    //    changeYear: true
    //}).datepicker("setDate", TodaysDate);



    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    if ($.fn.DataTable.isDataTable("#AuditSpecificationDtlsTblID")) {
        $("#AuditSpecificationDtlsTblID").DataTable().clear().destroy();
    }


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

    $("#SearchBtn").click(function () {

        SelectedSRO = $("#SROOfficeListID option:selected").text();
        var form = $("#AuditSpecificationDetailsForm");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);


        SROOfficeID = $("#SROOfficeListID option:selected").val();
        FromDate = $("#txtFromDate").val();
        ToDate = $("#txtToDate").val();

        //alert(SROOfficeID + "   " + FromDate + "   " + ToDate);

        if (SROOfficeID == 0) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select SRO Office</span>',
                callback: function () {
                    return;
                }
            });
        }
        else if ($("#AuditSpecificationDetailsForm").valid()) {
            //alert(SROOfficeID);
            var OfficeType = $("input[name='OfficeTypeToGetDropDown']:checked").val();


            var DocumentEnclosureDetailsTable = $('#AuditSpecificationDtlsTblID').DataTable({
                ajax: {
                    url: '/XELFiles/XELFilesDetails/LoadAuditSpecificationDetails',
                    type: "POST",
                    headers: header,
                    data: {
                        'fromDate': FromDate, 'ToDate': ToDate, 'SROOfficeCode': SROOfficeID, 'OfficeType': OfficeType
                    },
                    dataSrc: function (json) {
                        //unBlockUI();
                        if (json.errorMessage != null) {
                            //alert('if');

                            unBlockUI();
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                callback: function () {
                                    if (json.serverError != undefined) {
                                        window.location.href = "/Home/HomePage";
                                    } else {
                                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                            $('#DtlsSearchParaListCollapse').trigger('click');
                                        $("#AuditSpecificationDtlsTblID").DataTable().clear().destroy();
                                    }
                                    //unBlockUI();
                                }
                            });
                            return;
                        }
                        else {
                            //alert('else');
                            var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                            if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                                $('#DtlsSearchParaListCollapse').trigger('click');
                            unBlockUI();
                            return json.data;
                        }
                        //unBlockUI();
                        //return json.data;                        
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
                        var searchString = $('#AuditSpecificationDtlsTblID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;
                            //alert('rgewerg');
                            //unBlockUI();
                            if (!regexToMatch.test(searchString)) {
                                $("#AuditSpecificationDtlsTblID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    DocumentEnclosureDetailsTable.search('').draw();
                                    $("#AuditSpecificationDtlsTblID_filter input").prop("disabled", false);
                                });
                                unBlockUI();
                                return false;
                            }
                        }
                    }
                },
                serverSide: true,
                "scrollX": true,
                "scrollY": "300px",
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
                        data: "OfficeName", "searchable": true, "visible": true, "name": "OfficeName"

                    },
                    {
                        data: "ServerName", "searchable": true, "visible": true, "name": "ServerName"

                    },
                    {
                        data: "DatabaseName", "searchable": true, "visible": true, "name": "DatabaseName"

                    },
                    {
                        data: "LoginName", "searchable": true, "visible": true, "name": "LoginName"

                    },
                    {
                        data: "HostName", "searchable": true, "visible": true, "name": "HostName"

                    },
                    {
                        data: "ApplicationName", "searchable": true, "visible": true, "name": "ApplicationName"
                    },

                    {
                        data: "EventTime", "searchable": true, "visible": true, "name": "EventTime"

                    },
                    {
                        data: "Statement", "searchable": true, "visible": true, "name": "Statement"

                    }, {
                        data: "OfficeType", "searchable": true, "visible": true, "name": "OfficeType"

                    }
                ]
                ,
                fnInitComplete: function (oSettings, json) {
                    //unBlockUI();     
                    //unBlockUI();     
                    $("#EXCELSPANID").html(json.ExcelDownloadBtn);

                }
                //preDrawCallback: function () {
                //    //unBlockUI();                   
                //},
                //fnRowCallback: function (nRow, aData, iDisplayIndex) {                   
                //    //if (ModuleID == 1) {
                //    //    fnSetColumnVis(2, false);
                //    //}
                //    //else if (ModuleID == 2) {
                //    //    fnSetColumnVis(0, false);
                //    //    fnSetColumnVis(1, false);
                //    //}

                //    //unBlockUI();
                //    return nRow;
                //},
                //drawCallback: function (oSettings) {
                //    //unBlockUI();                    
                //},
            });

            //DocumentEnclosureDetailsTable.columns.adjust().draw();
        }
    });


});

function EXCELDownloadFun(FromDate, ToDate, SROOfficeID) {
    var OfficeType = $("input[name='OfficeTypeToGetDropDown']:checked").val();

    window.location.href = '/XELFiles/XELFilesDetails/ExportToExcel?SROOfficeID=' + SROOfficeID + "&FromDate=" + FromDate + "&ToDate=" + ToDate + "&SelectedSRO=" + SelectedSRO + "&OfficeType=" + OfficeType;

}
