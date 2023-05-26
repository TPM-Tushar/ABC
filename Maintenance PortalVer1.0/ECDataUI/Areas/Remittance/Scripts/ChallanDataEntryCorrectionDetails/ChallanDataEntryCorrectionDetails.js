
//Global variables.
var token = '';
var header = {};

$(document).ready(function () {

    $('#MasterDataID').dataTable({
        "columnDefs": [
            {
                "width": "30%",
                "targets": 6
            }
        ]
    });


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;




    //$('#txtFromDate').datepicker({
    //    autoclose: true,
    //    format: 'dd/mm/yyyy',
    //    startDate: '01/01/2022',
    //    endDate: '+0d',
    //    minDate: '01/01/2022',
    //    maxDate: new Date(),
    //    pickerPosition: "bottom-left"
    //});

    //$('#txtFromDate').datepicker({
    //    format: 'dd/mm/yyyy',
    //    changeMonth: true,
    //    changeYear: true,

    //}).datepicker("setDate", FromDate);


    $('#divFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        startDate: new Date('01/01/2022'),
        maxDate: new Date(),
        minDate: new Date('01/01/2022'),
        pickerPosition: "bottom-left"
    });

    //$('#txtTODate').datepicker({
    //    autoclose: true,
    //    format: 'dd/mm/yyyy',
    //    endDate: '+0d',

    //    maxDate: '0',
    //    pickerPosition: "bottom-left"
    //});

    //$('#txtTODate').datepicker({
    //    format: 'dd/mm/yyyy',
    //    changeMonth: true,
    //    changeYear: true,

    //}).datepicker("setDate", TODate);


    $('#divToDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        startDate: new Date('01/01/2022'),
        maxDate: new Date(),
        minDate: new Date('01/01/2022'),
        pickerPosition: "bottom-left"
    });

    $('#DtlsSearchParaListCollapse').click(function () {

        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });




    $("input:checkbox").on('click', function () {

        var $box = $(this);
        if ($box.is(":checked")) {

            var group = "input:checkbox[name='" + $box.attr("name") + "']";

            $(group).prop("checked", false);
            $box.prop("checked", true);
        } else {
            $box.prop("checked", false);
        }
    });






});













function GetCdeData() {
    //var CU = $('#CU').val()
    var LU = $("input[name='LUpdate']:checked").val();
    var CU = $("input[name='CUpdate']:checked").val();
    // var LU = $('#LU').val();
   
    var SRO = $('#SROOfficeListID').val();
    var FromDate = $("#txtFromDate").val();
    var ToDate = $("#txtTODate").val();

    var tableRegistrationNoVerificationSummary = $('#MasterDataID').DataTable({


        ajax: {

            url: '/Remittance/ChallanDataEntryCorrectionDetails/GetCDECorrectionDetailsData',

            type: "POST",
            headers: header,
            data: {
                'IsCentrallyUpdated': CU,
                'IsLocalluUpdated': LU,
                'SRO': SRO,
                'FromDate': FromDate,
                'ToDate': ToDate

            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            }
                            //else if (!json.status ) {
                            //    bootbox.alert({
                            //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>'
                            //    });
                            //}
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#MasterDataID").DataTable().clear().destroy();
                                $("#EXCELSPANID").html('');
                            }
                        }
                    });
                }

                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                    unBlockUI();
                }
                return json.data;








                unBlockUI();
                return json.data;
            },
            error: function () {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });

                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#MasterDataID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#MasterDataID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableRegistrationNoVerificationSummary.search('').draw();
                            $("#MasterDataID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }
        },
        serverSide: true,

        //"scrollX": "300px",
        scrollX: true,
        "scrollY": "300px",
        "responsive": true,
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bSort": true,

        columnDefs: [
            { "className": "dt-center", "targets": "_all" },
            { "orderable": false, "targets": "_all" },
            { "bSortable": false, "aTargets": "_all" },

            { "width": "2px", "targets": [0] },
            { "width": "64px", "targets": [1] },
            { "width": "61px", "targets": [2] },
            { "width": "70px", "targets": [3] },
            { "width": "82px", "targets": [4] },
            { "width": "80px", "targets": [5] },
            { "width": "75px", "targets": [6] },
            { "width": "90px", "targets": [7] },
            { "width": "85px", "targets": [8] },


        ],

        columns: [

            { data: "SRONAME", "visible": true, "name": "SROCode" },
            { data: "ApplicationDate", "visible": true, "name": "ApplicationDate" },
            { data: "OldChallanNumber", "visible": true, "name": "OldChallanNumber" },
            { data: "OldChallanDate", "visible": true, "name": "OldChallanDate" },
            { data: "NewChallanNumber", "visible": true, "name": "NewChallanNumber" },
            { data: "NewChallanDate", "visible": true, "name": "NewChallanDate" },
            { data: "Reason", "visible": true, "name": "Reason" },
            { data: "IsLocallyUpdated", "visible": true, "name": "IsLocallyUpdated" },
            { data: "IsCentrallyUpdated", "visible": true, "name": "IsCentrallyUpdated" },




        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
            $("#EXCELSPANID").html(json.ExcelDownloadBtn);
        },
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


}











