

var token = '';
var header = {};



function Show() {
    SROfficeID = $("#SROfficeListID option:selected").val();



    //Validation
    if (SROfficeID == 0) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid SRO.</span>'
        });
        return;
    }

   

    //$("#PhotoThumbDIV").show();
    DetailsTable(SROfficeID);
}








function DetailsTable(SROfficeID) {

    blockUI("Loading...");
    $("#PendingDocumentsTable").DataTable().clear().destroy();

    var DetailsTable = $('#PendingDocumentsTable').DataTable({
        ajax: {
            url: "/PendingDocuments/PendingDocuments/PendingDocumentAvailaibility",
            type: "POST",
            headers: header,
            data: {
                "SROfficeID": SROfficeID
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
                                $("#PendingDocumentsTable").DataTable().clear().destroy();
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

                //$('#btnDownload').css("display", "none");
            },
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
            { "orderable": false, "targets":"_all" }
        ],


        columns: [
            { data: "SNo", "searchable": true, "visible": true, "name": "SNo" },
            { data: "PendingNumber", "searchable": true, "visible": true, "name": "PendingNumber" },
            { data: "PresentationDate", "searchable": true, "visible": true, "name": "PresentationDate" },
            { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
            { data: "PendingReason", "searchable": true, "visible": true, "name": "PendingReason" },
            { data: "DateOfPending", "searchable": true, "visible": true, "name": "DateOfPending" },
        ],
        fnInitComplete: function (oSettings, json) {
            //if ($('#PhotoThumbTable').DataTable().rows().count() != 0) {
            //    $('#btnDownload').css("display", "block");
            //}
            //else {

            //$('#btnDownload').css("display", "none");
            //}
            unBlockUI();


            //$("tr td:nth-child(5)").addClass('img-center');
            //$("tr td:nth-child(6)").addClass('img-center');
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