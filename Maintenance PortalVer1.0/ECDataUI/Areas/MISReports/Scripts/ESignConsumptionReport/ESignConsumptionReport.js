
var token = '';
var header = {};
var SelectedAppStatusType;
var txtFromDate;
var txtToDate;
var FinancialyearCode;
var monID;
var yearID;



$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#totlESignCountDiv').hide();
    $('#dvESignDataTable').hide();

    $('#txtFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"


    });

    $('#txtFromDate').datepicker()
        .on("input change", function (e) {
            var changedFromdate = ($(this).val());

            $('#txtToDate').datepicker({

                autoclose: true,
                format: 'dd/mm/yyyy',
                endDate: '+0d',
                minDate: changedFromdate,
                pickerPosition: "bottom-left"


            });

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
    }).datepicker("setDate", ToDate);


    $('#txtFromDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", FromDate);


    //document.getElementById('ApplicationStatusTypeListID').onchange = function () {

    //    //SelectedAppStatusType = $('#ApplicationStatusTypeListID option:selected').val();
    //    SelectedAppStatusType = $("#dvAppStatusTypeID option:selected").val();

    //    // alert("Month: " + monID + "  Year: " + yearID);

    //    //LoadESignDetailsDataTable(monID, yearID, SelectedAppStatusType);
    //    txtFromDate = $("#txtFromDate").val();
    //    txtToDate = $("#txtToDate").val();
    //    LoadESignDetailsDataTable(txtFromDate, txtToDate, SelectedAppStatusType);
    //};


    if (ViewESignDetailsDataTable == "True") {
        //alert("ViewESignDetailsDataTable: " + ViewESignDetailsDataTable);
        $("#dvESignDataTable").show();
    }



    $("#BtnViewReport").click(function () {

        //txtFromDate = $("#txtFromDate").val();
        //txtToDate = $("#txtToDate").val();

        FinancialyearCode = $("#financialYearListID").val();

        //if (Date.parse(txtFromDate) > Date.parse(txtToDate)) {
        //if (false) {
        //alert("From Date: " + txtFromDate);
        //alert("To Date: " + txtToDate);

        //bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">From Date should be less than To Date.</span>');

        //hide below datatable if this is run
        //return;
        //}
        //else {
        //alert("1234");

        $('#dvLoadTotalESignConsView').show();

        if ($.fn.DataTable.isDataTable("#tblESignDetailsDataTable")) {
            $("#tblESignDetailsDataTable").DataTable().clear().destroy();
        }
        //}


        //if (txtFromDate == "") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select From Date.</span>');
        //    return;
        //}
        //else if (txtToDate == "") {
        //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select To Date.</span>');
        //    return;

        //}

        if (FinancialyearCode == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Financial Year.</span>');
            return;
        }


        blockUI('Loading data please wait.');
        $.ajax({
            type: "POST",
            url: '/MISReports/ESignConsumptionReport/GetTotalESignConsumedCount',
            cache: false,
            headers: header,
            data: {
                'FinancialyearCode': FinancialyearCode
            },

            success: function (data) {
                unBlockUI();
                if (data.serverError == true) {

                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {

                    if (data.success == false) {

                        bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                            function () { });
                    }
                    else {


                        $('#totlESignCountDiv').show();

                        $('#dvLoadTotalESignConsView').html(data);
                    }
                }



                //SelectedAppStatusType = 0;         //To load all records (Success/Failed)
                ////SelectedAppStatusType = $("#dvAppStatusTypeID option:selected").val();
                //document.getElementById("ApplicationStatusTypeListID").value = "0";  //When View Report is cliked,it should show all records (Success/Falied)
                //LoadESignDetailsDataTable(SelectedAppStatusType);
            },
            error: function (xhr, status, err) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while proccessing your request : " + err + '</span>',
                    callback: function () {

                    }
                });
                unBlockUI();
            }
        });
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');


    });

    //Added by Madhusoodan on 12/10/2021
    $("#BtnViewESignDetails").click(function () {

        txtFromDate = $("#txtFromDate").val();
        txtToDate = $("#txtToDate").val();

        SelectedAppStatusType = $("#dvAppStatusTypeID option:selected").val();
        LoadESignDetailsDataTable(txtFromDate, txtToDate, SelectedAppStatusType);

        //var tblESignDetailsDataTable = $('#tblESignDetailsDataTable').DataTable({

        //    ajax: {

        //        url: '/MISReports/ESignConsumptionReport/LoadESignDetailsDataTable',
        //        type: "POST",
        //        headers: header,
        //        data: {
        //            'FromDate': txtFromDate, 'ToDate': txtToDate, 'ApplicationStatusTypeID': SelectedAppStatusType

        //            //'MonthCode': monthID, 'FinancialYearCode': year, 'ApplicationStatusTypeID': SelectedAppStatusType
        //        },
        //        dataSrc: function (json) {
        //            unBlockUI();
        //            if (json.errorMessage != null) {
        //                bootbox.alert({
        //                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
        //                    callback: function () {
        //                        if (json.serverError != undefined) {
        //                            window.location.href = "/Home/HomePage"
        //                        }
        //                        else {
        //                            var classToRemove = $('#DtlsToggleIconSearchPara2').attr('class');
        //                            if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
        //                                $('#DtlsSearchParaListCollapse2').trigger('click');

        //                            $("#tblESignDetailsDataTable").DataTable().clear().destroy();
        //                        }
        //                    }
        //                });
        //            }
        //            else {

        //                $('#dvESignDataTable').show();
        //                $('#dtESignDetails').show();
        //            }
        //            unBlockUI();
        //            return json.data;
        //        },
        //        error: function () {

        //            bootbox.alert({
        //                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
        //                callback: function () {
        //                }
        //            });

        //            unBlockUI();
        //        },
        //    },



        //    //"pageLength": 10,
        //    //"iDisplayLength": 10,
        //    bserverSide: true,
        //    "scrollY": "300px",
        //    "scrollCollapse": true,
        //    bPaginate: true,
        //    bLengthChange: true,
        //    //bInfo: true,
        //    //info: true,
        //    bFilter: false,
        //    //searching: true,
        //    "destroy": true,
        //    "bAutoWidth": true,
        //    "bScrollAutoCss": true,

        //    columnDefs: [

        //        { targets: "_all", orderable: false, "className": "text-center" }

        //    ],

        //    columns: [

        //        { data: "SerialNo", "searchable": false, "visible": true, "name": "SerialNo" },
        //        { data: "ApplicationNumber", "searchable": false, "visible": true, "name": "ApplicationNumber" },
        //        { data: "ApplicationType", "searchable": false, "visible": true, "name": "ApplicationType" },
        //        { data: "ApplicationDate", "searchable": false, "visible": true, "name": "ApplicationDate" },
        //        { data: "ApplicationStatus", "searchable": false, "visible": true, "name": "ApplicationStatus" },
        //        { data: "ApplicationSubmitDate", "searchable": false, "visible": true, "name": "ApplicationSubmitDate" },
        //        { data: "ESignRequestDate", "searchable": false, "visible": true, "name": "ESignRequestDate" },
        //        { data: "ESignRequestTransactionNo", "searchable": false, "visible": true, "name": "ESignRequestTransactionNo" },
        //        { data: "ESignResponseDate", "searchable": false, "visible": true, "name": "ESignResponseDate" },
        //        { data: "ESignResponseTransactionNo", "searchable": false, "visible": true, "name": "ESignResponseTransactionNo" },
        //        { data: "ESignResponseCode", "searchable": false, "visible": true, "name": "ESignResponseCode" },
        //        { data: "Status", "searchable": false, "visible": true, "name": "Status" },
        //        { data: "ResponseErrorCode", "searchable": false, "visible": true, "name": "ResponseErrorCode" },
        //        { data: "ResponseErrorMessage", "searchable": false, "visible": true, "name": "ResponseErrorMessage" }

        //    ],
        //    fnInitComplete: function (oSettings, json) {

        //    },
        //    preDrawCallback: function () {
        //        unBlockUI();
        //    },
        //    fnRowCallback: function (nRow, aData, iDisplayIndex) {
        //        unBlockUI();
        //        return nRow;
        //    },
        //    drawCallback: function (oSettings) {

        //        unBlockUI();
        //    },
        //});
    });
});


//function LoadESignDetailsDataTable(selAppStatusType) {
//function LoadESignDetailsDataTable(monthID, year, selAppStatusType) {
function LoadESignDetailsDataTable(txtFromDate, txtToDate, selAppStatusType) {

    if ($.fn.DataTable.isDataTable("#tblESignDetailsDataTable")) {
        $("#tblESignDetailsDataTable").DataTable().clear().destroy();
    }

    //txtFromDate = $("#txtFromDate").val();
    //txtToDate = $("#txtToDate").val();

    SelectedAppStatusType = selAppStatusType;
    //SelectedAppStatusType = $("#dvAppStatusTypeID option:selected").val();
    
    //alert("Sel: " + SelectedAppStatusType);
    
    var tblESignDetailsDataTable = $('#tblESignDetailsDataTable').DataTable({

        ajax: {

            url: '/MISReports/ESignConsumptionReport/LoadESignDetailsDataTable',
            type: "POST",
            headers: header,
            data: {
                'FromDate': txtFromDate, 'ToDate': txtToDate, 'ApplicationStatusTypeID': SelectedAppStatusType
                //'MonthCode': monthID, 'FinancialYearCode': year, 'ApplicationStatusTypeID': SelectedAppStatusType
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
                            else {
                                var classToRemove = $('#DtlsToggleIconSearchPara2').attr('class');
                                if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse2').trigger('click');

                                $("#tblESignDetailsDataTable").DataTable().clear().destroy();
                            }
                        }
                    });
                }
                else {
                    
                    $('#dvESignDataTable').show();
                    $('#dtESignDetails').show();
                }
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
        },



        //"pageLength": 10,
        //"iDisplayLength": 10,
        bserverSide: true,
        "scrollY": "300px",
        "scrollCollapse": true,
        bPaginate: true,
        bLengthChange: true,
        //bInfo: true,
        //info: true,
        bFilter: false,
        //searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,

        columnDefs: [

            { targets: "_all", orderable: false, "className": "text-center" }

        ],

        columns: [

            { data: "SerialNo", "searchable": false, "visible": true, "name": "SerialNo" },
            { data: "ApplicationNumber", "searchable": false, "visible": true, "name": "ApplicationNumber" },
            { data: "ApplicationType", "searchable": false, "visible": true, "name": "ApplicationType" },
            { data: "ApplicationDate", "searchable": false, "visible": true, "name": "ApplicationDate" },
            { data: "ApplicationStatus", "searchable": false, "visible": true, "name": "ApplicationStatus" },
            { data: "ApplicationSubmitDate", "searchable": false, "visible": true, "name": "ApplicationSubmitDate" },
            { data: "ESignRequestDate", "searchable": false, "visible": true, "name": "ESignRequestDate" },
            { data: "ESignRequestTransactionNo", "searchable": false, "visible": true, "name": "ESignRequestTransactionNo" },
            { data: "ESignResponseDate", "searchable": false, "visible": true, "name": "ESignResponseDate" },
            { data: "ESignResponseTransactionNo", "searchable": false, "visible": true, "name": "ESignResponseTransactionNo" },
            { data: "ESignResponseCode", "searchable": false, "visible": true, "name": "ESignResponseCode" },
            { data: "Status", "searchable": false, "visible": true, "name": "Status" },
            { data: "ResponseErrorCode", "searchable": false, "visible": true, "name": "ResponseErrorCode" },
            { data: "ResponseErrorMessage", "searchable": false, "visible": true, "name": "ResponseErrorMessage" }

        ],
        fnInitComplete: function (oSettings, json) {

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


function LoadMonthWiseDatatable(monthID, year) {
    //alert("IN LoadMonthWiseDatatable with monthID: " + monthID + "  Year: " + year);
    //alert(monthID);
    //alert(year);
    monID = monthID;
    yearID = year;
    //alert("monID: " + monID + "  yearID: " + yearID);
    SelectedAppStatusType = 0; 
    document.getElementById("ApplicationStatusTypeListID").value = "0";  //When View Report is cliked,it should show all records (Success/Falied)
    LoadESignDetailsDataTable(monthID, year, SelectedAppStatusType);
}


function DownloadTotalCountExcel() {

    //txtFromDate = $("#txtFromDate").val();
    //txtToDate = $("#txtToDate").val();

    FinancialyearCode = $("#financialYearListID").val();

    window.location.href = '/MISReports/ESignConsumptionReport/TotalESignConsumptionCountToExcel?FinancialyearCode=' + FinancialyearCode;


}


function DownloadESignDetailsExcel() {

    txtFromDate = $("#txtFromDate").val();
    txtToDate = $("#txtToDate").val();

    SelectedAppStatusType = $("#dvAppStatusTypeID option:selected").val();

    //window.location.href = '/MISReports/ESignConsumptionReport/ESignStatusDetailsDatatableToExcel?monthID=' + monID + "&year=" + yearID + "&SelectedAppStatusType=" + SelectedAppStatusType;

    window.location.href = '/MISReports/ESignConsumptionReport/ESignStatusDetailsDatatableToExcel?fromDate=' + txtFromDate + "&toDate=" + txtToDate + "&SelectedAppStatusType=" + SelectedAppStatusType;

}

