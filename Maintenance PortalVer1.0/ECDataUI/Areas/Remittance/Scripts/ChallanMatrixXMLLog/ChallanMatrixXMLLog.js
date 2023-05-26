


//Global variables.
var token = '';
var header = {};

$(document).ready(function () {


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $("input[name='OfficeTypeToGetDropDown']").click(function () {
        var radioValue = $("input[name='OfficeTypeToGetDropDown']:checked").val();
        blockUI('PLEASE WAIT');
        $.ajax({
            url: '/Remittance/ChallanMatrixXMLLog/GetOfficeList',
            datatype: "text",
            type: "GET",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            data: { "OfficeType": radioValue },
            success: function (data) {
                // ADDED BY SHUBHAM BHAGAT ON 21-05-2019 AT 6:36 PM
                if (data.errorMessage == undefined) {
                    $('#OfficeTypeDropDownID').empty();
                    $.each(data.OfficeList, function (i, OfficeListItem) {
                        $('#OfficeTypeDropDownID').append('<option value="' + OfficeListItem.Value + '">' + OfficeListItem.Text + '</option>');
                    });
                }
                else {
                    bootbox.alert({
                        //size: 'small',
                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    });
                }

                // DataTable Clear and Destroy and DataTable Collapse.
                if ($('#ToggleIconID').attr('class') === "fa fa-minus-square-o fa-pull-left fa-2x") {
                    $('#challanMatrixCollapse').trigger('click');
                    $("#challanMatrixTable").DataTable().clear().destroy();
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

    $('#btnSearch').click(function () {



        var fromDate = $("#txtFromDate").val();
        var ToDate = $("#txtToDate").val();

   


        


        var radioValue = $("input[name='OfficeTypeToGetDropDown']:checked").val();

        if (radioValue == undefined) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select office type.</span>');
        }
        else if ($('#OfficeTypeDropDownID').val() === "0" || $('#OfficeTypeDropDownID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select office.</span>');
        }
        else {
          //  $('#ModelOfficeType').val(radioValue);
            $("#challanMatrixTable").DataTable().destroy();
            //BlockUI();
           
            var t = $("#challanMatrixTable").DataTable({
                ////"bJQueryUI": true,
                ////"bServerSide": true,
                "responsive": true,
                "processing": true, // for show progress bar
                "serverSide": true, // for process server side
                "filter": true, // this is for disable filter (search box)
                "orderMulti": false, // for disable multiple column at once
                "pageLength": 10,
                language: { search: "Search" },
                ////"scrollCollapse": true,
                "scrollX": true,
                "scrollY": "30vh",
                scrollCollapse: true,
                ////"scrollY": true,
                ////"ordering": false,
                ////autoWidth:true,
                ////"deferRender": true,
                ajax: {
                    url: '/Remittance/ChallanMatrixXMLLog/ChallanMatrixDetails',
                    type: "POST",
                    datatype: "json",
                    headers: header,
                    //data: $('#MatrixFormID').serialize(),
                    beforeSend: function () {
                        blockUI('PLEASE WAIT');
                        //Added by Akash(24-08-2018) to validate search string.
                        var searchString = $('#challanMatrixTable_filter input').val();
                        if (searchString !== "") {
                            var regexToMatch = /^[^<>]+$/;
                            
                            if (!regexToMatch.test(searchString)) {
                                unBlockUI();
                                $("#challanMatrixTable_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    //     $('#challanMatrixTable_filter input').val('');
                                    t.search('').draw();
                                    $("#challanMatrixTable_filter input").prop("disabled", false);
                                    //$('#challanMatrixTable_filter input').attr("value", "");
                                });
                                return false;
                            }
                        }
                    },
                    dataSrc: function (json) {
                        if (json.errorMessage != undefined) {
                            unBlockUI();
                            bootbox.alert({
                                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                                callback: function () {
                                    $("#challanMatrixTable").DataTable().clear().destroy();
                                    var classToRemove = $('#ToggleIconID').attr('class');
                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                                        $('#challanMatrixCollapse').trigger('click');
                                    }
                                }
                            });
                        }
                        else {
                            var classToRemove = $('#ToggleIconID').attr('class');
                            if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                                $('#challanMatrixCollapse').trigger('click');
                            }
                            unBlockUI();
                            return json.data;
                        }
                    },
                    data: {
                        "OfficeType": radioValue,
                        "OfficeTypeID": $('#OfficeTypeDropDownID').val(),
                        'fromDate': fromDate,
                        'ToDate': ToDate,
                        'RequestTxt': $("#RequestTxtID").val(),
                        'ResponseTxt': $("#ResponseTxtID").val()
                    }
                },
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
                    { orderable: false, targets: [9] }
                    //,
                    //{ orderable: false, targets: [10] },
                    //{ orderable: false, targets: [11] }
                ],
                "columns": [
                    // For Showing Sr No
                    { "data": "RequestXMLID", "name": "RequestXMLID", "autoWidth": true },
                    { "data": "RequestXMLID", "name": "RequestXMLID", "autoWidth": true },
                    { "data": "SROCode", "name": "SROCode", "autoWidth": true },
                    { "data": "RequestDateTime", "name": "RequestDateTime", "autoWidth": true },
                    { "data": "ResponseDateTime", "name": "ResponseDateTime", "autoWidth": true },
                    { "data": "IsExceptionInRequest", "name": "IsExceptionInRequest", "autoWidth": true },
                    { "data": "RequestExceptionDetails", "name": "RequestExceptionDetails", "autoWidth": true },
                    { "data": "IsExceptionInResponse", "name": "IsExceptionInResponse", "autoWidth": true },
                    { "data": "ResponseExceptionDetails", "name": "ResponseExceptionDetails", "autoWidth": true },
                    //{ "data": "IsDRO", "name": "IsDRO", "autoWidth": true },
                    //{ "data": "DROCode", "name": "DROCode", "autoWidth": true },
                    { "data": "DownloadXmlBtn", "name": "DownloadXmlBtn", "autoWidth": true }
                ]
                //,
                //fnInitComplete: function (oSettings, json) {
                //    alert('in fnInitComplete complete');
                //    $.unblockUI();
                //}
            });

            t.on('draw.dt', function () {
                var PageInfo = $('#challanMatrixTable').DataTable().page.info();
                t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
                    cell.innerHTML = i + 1 + PageInfo.start;
                });
            });

            ////$('#exceptionDetailsListTable_info').addClass('col-md-4');
            $('#challanMatrixTable_info').css('margin-left', '-2%');
            $('#challanMatrixTable_info').css('font-size', '120%');
            ////$('#challanMatrixTable_paginate').css('margin', '-20px');
            //$('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
            ////debugger;
            ////table.responsive.recalc();
            ////table.columns.adjust().draw();                

            //$.unblockUI();
        }
    });


    $('#challanMatrixCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    $('#ResetID').click(function () {
        window.location.reload();
    });

    $('#OfficeTypeDropDownID').change(function () {
        // DataTable Clear and Destroy and DataTable Collapse.
        if ($('#ToggleIconID').attr('class') === "fa fa-minus-square-o fa-pull-left fa-2x") {
            $('#challanMatrixCollapse').trigger('click');
            $("#challanMatrixTable").DataTable().clear().destroy();

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
    }).datepicker("setDate", output);





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

// DOWNLOAD ZIPPED FILE
function DownloadChallanMatrixXML(RequestXMLID) {
    window.location.href = "/Remittance/ChallanMatrixXMLLog/DownloadChallanMatrixXML?RequestXMLID=" + RequestXMLID;
}