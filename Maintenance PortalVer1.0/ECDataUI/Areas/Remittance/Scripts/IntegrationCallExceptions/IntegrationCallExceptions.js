
//Global variables.
var token = '';
var header = {};
$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $("input[name='OfficeTypeToGetDropDown']").click(function () {
        var radioValue = $("input[name='OfficeTypeToGetDropDown']:checked").val();
        BlockUI();
        $.ajax({
            url: '/Remittance/IntegrationCallExceptions/GetOfficeList',
            datatype: "text",
            type: "GET",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            data: { "OfficeType": radioValue },
            success: function (data) {
                $('#OfficeTypeDropDownID').empty();
                $.each(data.OfficeList, function (i, OfficeListItem) {
                    $('#OfficeTypeDropDownID').append('<option value="' + OfficeListItem.Value + '">' + OfficeListItem.Text + '</option>');
                });
                // Datatable Destroy
                //var table=$("#exceptionDetailsListTable").DataTable();
                //table.clear().draw();
                //dataTable.fnDraw();
                //dataTable.fnDestroy();
                //$("#exceptionDetailsListTable").DataTable().destroy();

                //Collapse Datatable
                //CloseExceptionDetailsList();

                // DataTable Clear and Destroy and DataTable Collapse.
                if ($('#ToggleIconID').attr('class') === "fa fa-minus-square-o fa-pull-left fa-2x") {
                    $('#exceptionDetailsListCollapse').trigger('click');
                    $("#exceptionDetailsListTable").DataTable().clear().destroy();
                }

                $.unblockUI();

            },
            error: function (xhr) {
                bootbox.alert({
                    //size: 'small',
                    //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                });
                $.unblockUI();
            }
        });
        //if (radioValue) {
        //    alert("Your are a - " + radioValue);
        //}
    });

    $('#btnSearch').click(function () {
        BlockUI();
        var radioValue = $("input[name='OfficeTypeToGetDropDown']:checked").val();

        $("#exceptionDetailsListTable").DataTable().destroy();

        var t = $("#exceptionDetailsListTable").DataTable({
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
                url: '/Remittance/IntegrationCallExceptions/GetExceptionsDetails',
                type: "POST",
                headers: header,

                beforeSend: function () {
                    //Added by Akash(24-08-2018) to validate search string.
                    var searchString = $('#exceptionDetailsListTable_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                      
                        if (!regexToMatch.test(searchString)) {
                            $("#exceptionDetailsListTable_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                //     $('#exceptionDetailsListTable_filter input').val('');
                                t.search('').draw();
                                $("#exceptionDetailsListTable_filter input").prop("disabled", false);
                                //$('#exceptionDetailsListTable_filter input').attr("value", "");
                            });
                            return false;
                            $.unblockUI();
                        }
                    }
                },
                // Added on 15-05-2019 by shubham
                dataSrc: function (json) {
                    if (json.errorMessage != undefined) {
                        unBlockUI();
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            callback: function () {
                                $("#exceptionDetailsListTable").DataTable().clear().destroy();
                            }
                        });
                    }
                    else {
                        var classToRemove = $('#ToggleIconID').attr('class');
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                            $('#exceptionDetailsListCollapse').trigger('click');
                        }
                        unBlockUI();
                        return json.data;
                    }
                },
                data: {
                    "OfficeType": radioValue, "OfficeTypeID": $('#OfficeTypeDropDownID').val()
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
                { orderable: false, targets: [9] },
                { orderable: false, targets: [10] }
            ],

            "columns": [
                // For Showing Sr No
                { "data": "Logid", "name": "Logid", "autoWidth": true },
                { "data": "Logid", "name": "Logid", "autoWidth": true },
                { "data": "SROCode", "name": "SROCode", "autoWidth": true },
                { "data": "ExceptionType", "name": "ExceptionType", "autoWidth": true },
                { "data": "InnerExceptionMsg", "name": "InnerExceptionMsg", "autoWidth": true },
                { "data": "ExceptionMsg", "name": "ExceptionMsg", "autoWidth": true },
                { "data": "ExceptionStackTrace", "name": "ExceptionStackTrace", "autoWidth": true },
                { "data": "ExceptionMethodName", "name": "ExceptionMethodName", "autoWidth": true },
                { "data": "LogDate", "name": "LogDate", "autoWidth": true },
                { "data": "IsDRO", "name": "IsDRO", "autoWidth": true },
                { "data": "DRO", "name": "DRO", "autoWidth": true }
            ]
        });

        t.on('draw.dt', function () {
            var PageInfo = $('#exceptionDetailsListTable').DataTable().page.info();
            t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1 + PageInfo.start;
            });
        });


        ////$('#exceptionDetailsListTable_info').addClass('col-md-4');
        $('#exceptionDetailsListTable_info').css('margin-left', '-2%');
        $('#exceptionDetailsListTable_info').css('font-size', '120%');
        ////$('#exceptionDetailsListTable_paginate').css('margin', '-20px');
        //$('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
        ////debugger;
        ////table.responsive.recalc();
        ////table.columns.adjust().draw();      

        // Commented on 15-05-2019 by shubham
        //var classToRemove = $('#ToggleIconID').attr('class');
        //if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
        //    $('#exceptionDetailsListCollapse').trigger('click');

        $.unblockUI();
    });

    //$('#exceptionDetailsListCollapse').trigger('click');

    $('#exceptionDetailsListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });


    //$('#closeExceptionDetailsList').click(function () {
    //    var classToRemove = $('#ToggleIconID').attr('class');
    //    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
    //        $('#ToggleIconID').addClass(classToSet);
    //    }
    //});



    $('#ResetID').click(function () {
        window.location.reload();
    });

    $('#OfficeTypeDropDownID').change(function () {
        // DataTable Clear and Destroy and DataTable Collapse.
        if ($('#ToggleIconID').attr('class') === "fa fa-minus-square-o fa-pull-left fa-2x") {
            $('#exceptionDetailsListCollapse').trigger('click');
            $("#exceptionDetailsListTable").DataTable().clear().destroy();
        }
    });

});

//function CloseExceptionDetailsList() {
//    var classToRemove = $('#ToggleIconID').attr('class');
//    var classToSet = "fa fa-plus-square-o fa-pull-left fa-2x";
//    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
//        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
//    }
//}

//function OpenExceptionDetailsList() {
//    var classToRemove = $('#ToggleIconID').attr('class');
//    var classToSet = "fa fa-minus-square-o fa-pull-left fa-2x";
//    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
//        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
//    }
//}

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