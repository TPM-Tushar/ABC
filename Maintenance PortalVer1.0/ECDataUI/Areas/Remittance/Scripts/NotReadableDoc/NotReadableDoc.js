
//Global variables.
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
 //Added By Tushar on 14 Nov 2022
    $('#DROOfficeListID').change(function () {
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/Remittance/NotReadableDoc/GetSROOfficeListByDistrictID',
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
 //End By Tushar on 14 Nov 2022

});



function SearchNotReadableDocDetails() {
    let SROfficeID = $("#SROOfficeListID").val();
    let DistrictID = $('#DROOfficeListID').val()

    let tableNotReadableDocDetails = $('#NotReadableDocDetailsID').DataTable({
        ajax: {

            url: '/Remittance/NotReadableDoc/GetNotReadableDocDetails',
            type: "POST",
            headers: header,
            data: {
               
                'SROfficeID': SROfficeID,
                'DistrictID': DistrictID
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
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#NotReadableDocDetailsID").DataTable().clear().destroy();
                                $("#EXCELSPANID").html('');
                            }
                        }
                    });
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
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });

                unBlockUI();
            },
    
            beforeSend: function () {
                blockUI('loading data.. please wait...');
              
                var searchString = $('#NotReadableDocDetailsID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#NotReadableDocDetailsID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                           
                            tableNotReadableDocDetails.search('').draw();
                            $("#NotReadableDocDetailsID_filter input").prop("disabled", false);
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


        columns: [

            //{ data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width": "2%" },

            //{ data: "Registration_Number", "searchable": true, "visible": true, "name": "Registration_Number", "width": "7%" },
            //{ data: "Log_Date", "searchable": true, "visible": true, "name": "Log_Date", "width": "8%" },
            //{ data: "CD_Number", "searchable": true, "visible": true, "name": "CD_Number", "width": "10%" },
            //{ data: "Document_Type", "searchable": true, "visible": true, "name": "Document_Type", "width": "10%" },
            //{ data: "Logged_By", "searchable": true, "visible": true, "name": "Logged_By", "width": "7%" },
            // { data: "Stamp5DateTime", "searchable": true, "visible": true, "name": "Stamp5DateTime", "width": "6%" }
            { data: "srNo", "searchable": true, "visible": true, "name": "srNo", "width": "2%" },

            { data: "Registration_Number", "searchable": true, "visible": true, "name": "Registration_Number", "width": "10%" },
            { data: "Stamp5DateTime", "searchable": true, "visible": true, "name": "Stamp5DateTime", "width": "7%" },
            { data: "CD_Number", "searchable": true, "visible": true, "name": "CD_Number", "width": "8%" },
            { data: "Document_Type", "searchable": true, "visible": true, "name": "Document_Type", "width": "8%" },
            { data: "Logged_By", "searchable": true, "visible": true, "name": "Logged_By", "width": "7%" },
            { data: "Log_Date", "searchable": true, "visible": true, "name": "Log_Date", "width": "8%" },
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


function EXCELDownloadFun(SroCodeEx, DROfficeIDEx) {
   // alert(" in EXCELDownloadFun");

    window.location.href = '/Remittance/NotReadableDoc/ExportNotReadableDocToExcel?SROfficeID=' + SroCodeEx + "&DROfficeID=" + DROfficeIDEx;

}

