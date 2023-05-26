$(document).ready(function () {    
    DisplayReleasedServicePackDetailsList();
});
function DisplayReleasedServicePackDetailsList() {
    var tableElement = $('#tblServicePackGrid');
    tableElement.dataTable().fnDestroy();
    tableElement.DataTable({
        ajax: {
            url: "/ServicePack/ServicePackDetails/GetServicePackDetailsList",
            type: "POST",
            data: { IsRequestForReleasedServicePacksList: "true" },
            dataSrc: function (json) {
                return json.data;
            },
            error: function () {
                bootbox.alert("Something Went Wrong");
            }
        },
        serverSide: true,
        scrollY: "400px",
        scrollCollapse: true,
        sScrollX: false,
        bInfo: true,
        //sDom: 'TRrt<"col-md-4 pad-0"f><"col-md-1 pad-0"l><"col-md-2 pad-0"i><"col-md-5 pad-0"p>',
        columns: [
            { data: "ReferenceID", width: "1%" },
            { data: "ReleaseType", width: "2%"  },
            { data: "IsTestOrFinal" },
            { data: "MajorVersion" },
            { data: "MinorVersion" },
             //Added By Tushar on 2 Jan 2023 for Upload DateTime
            { data: "ServicePackAddedDateTime", "width": "1%", className: "text-center" },
            //End By Tushar on 2 Jan 2023
            { data: "SPDescription" },
            { data: "InstallationProc" },
            { data: "ReleaseDetails" },
            {
                data: null,
                className: "center",
                sorting: "false",
                render: function (data, type, row) {
                    return data.ChangesList;
                },
            },
            {
                data: null,
                render: function (data, type, row) {
                    return data.VirtualPath;
                },
                bSortable: false
            }, 
            { data: "ServicePackReleaseDateTime" }
        ]
        ,
        autoWidth: false,
        fnInitComplete: function (oSettings, json) {
            $('div#tblServicePackGrid_info').css('padding-left', '0px');
        }

    });

    $('#tblServicePackGrid_info').css('font-size', '120%');
    $('.dataTables_length').css('float', 'left');
}

function DownloadServicePack(VirtualPath) {
    if (VirtualPath == "ForseeableRestriction") {
        bootbox.alert("❌ File download permission denied.Please check release date of service pack.");
        return;
    }

    $.ajax({
        url: '/ServicePack/ServicePackDetails/CheckIfFileExists',
        data: { "VirtualPath": VirtualPath },
        type: "GET",
        success: function (data) {
            if (data.success == true) {
                window.location.href = "/ServicePack/ServicePackDetails/DownloadServicePackFile?virtualPath=" + VirtualPath;
            }
            else {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                    function () { });
            }
        },
        error: function (xhr) {
        }
    });

}