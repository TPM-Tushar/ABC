

//Global variables.
var token = '';
var header = {};
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    // COMMENTED BY SHUBHAM BHAGAT ON 17-05-2019
    //LoadDblVerification();   
    //$('#doubleVeriCollapse').trigger('click');

    $('#ResetID').click(function () {
        window.location.reload();
    });

    $('#doubleVeriCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
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

    $('#btnSearch').click(function () {
        var fromDate = $("#txtFromDate").val();
        var ToDate = $("#txtToDate").val();

        $("#doubleVeriTable").DataTable().destroy();



        var t = $("#doubleVeriTable").DataTable({
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
                url: '/Remittance/DblVerificationXMLLog/DblVeriXMLLogDetails',
                type: "POST",
                headers: header,
                beforeSend: function () {
                    blockUI('PLEASE WAIT');

                    //Added by Akash(24-08-2018) to validate search string.
                    var searchString = $('#doubleVeriTable_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                      
                        if (!regexToMatch.test(searchString)) {
                            unBlockUI();
                            $("#doubleVeriTable_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                //     $('#doubleVeriTable_filter input').val('');
                                t.search('').draw();
                                $("#doubleVeriTable_filter input").prop("disabled", false);
                                //$('#doubleVeriTable_filter input').attr("value", "");
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
                                $("#doubleVeriTable").DataTable().clear().destroy();
                                var classToRemove = $('#ToggleIconID').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                                    $('#doubleVeriCollapse').trigger('click');
                                }
                            }
                        });
                    }
                    else {
                        var classToRemove = $('#ToggleIconID').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                            $('#doubleVeriCollapse').trigger('click');
                        }
                        unBlockUI();
                        unBlockUI();
                        return json.data;
                    }
                },
                data: {
                    'fromDate': fromDate,
                    'ToDate': ToDate,
                    'RequestTxt': $("#RequestTxtID").val(),
                    'ResponseTxt': $("#ResponseTxtID").val(),
                },
                datatype: "json"
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
                { "data": "DownloadXmlBtn", "name": "DownloadXmlBtn", "autoWidth": true }
            ]
        });

        t.on('draw.dt', function () {
            var PageInfo = $('#doubleVeriTable').DataTable().page.info();
            t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1 + PageInfo.start;
            });
        });

        ////$('#exceptionDetailsListTable_info').addClass('col-md-4');
        $('#doubleVeriTable_info').css('margin-left', '-2%');
        $('#doubleVeriTable_info').css('font-size', '120%');
        ////$('#doubleVeriTable_paginate').css('margin', '-20px');
        //$('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
        ////debugger;
        ////table.responsive.recalc();
        ////table.columns.adjust().draw();          

        //  unBlockUI();
    });

});

// DOWNLOAD ZIPPED FILE
function DownloadDblVeriXML(RequestXMLID) {
    window.location.href = "/Remittance/DblVerificationXMLLog/DownloadDblVeriXML?RequestXMLID=" + RequestXMLID;
}

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