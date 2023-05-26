/*----------------------------------------------------------------------------------------
    * Project Id    :KARIGR [ IN-KA-IGR-02-05 ]
    * Project Name  :Kaveri Maintainance Portal
    * Name          :
    * Description   :     
    * Author        :Harshit   
    * Creation Date :01/05/2019
    * Modified By   :   
    * Updation Date :17 May 2019
    * ECR No : 300
----------------------------------------------------------------------------------------*/
$(document).ready(function () {
    DisplayServicePackDetailsList();
});


function DisplayServicePackDetailsList() {
    var tableElement = $('#tblServicePackGrid');
    tableElement.dataTable().fnDestroy();
    tableElement.DataTable({
        ajax: {
            url: "/ServicePack/ServicePackDetails/GetServicePackDetailsList",
            type: "POST",
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
        //sDom: 'Lfrtlip' ,
        //sDom: 'TRrt<"col-md-4 pad-0"f><"col-md-1 pad-0"l><"col-md-2 pad-0"i><"col-md-5 pad-0"p>',
        columns: [
            { data: "ReferenceID", width: "1%",className: "text-center"},
            {
                data: null,
                className: "text-center",
                sorting: "false",
                "width": "1%",
                render: function (data, type, row) {
                    return data.ActiveDeactiveMenu;
                },
            },
            {
                data: null,
                className: "text-center",
                sorting: "false",
                "width": "1%",
                render: function (data, type, row) {
                    return data.EditServicePackIfNotReleased;
                },
            },
            { data: "ReleaseType","width": "2%" },
            { data: "IsTestOrFinal", "width": "2%" },
            { data: "MajorVersion", "width": "1%" },
            { data: "MinorVersion", "width": "1%"},
             //Added By Tushar on 2 Jan 2023 for Upload DateTime
            { data: "ServicePackAddedDateTime", "width": "1%", className: "text-center"},
            //End By Tushar on 2 Jan 2023
            { data: "SPDescription", "width": "5%" },
            { data: "InstallationProc", "width": "5%" },
            {
                data: null,
                sorting: "false",
                 "width": "10%",
                render: function (data, type, row) {
                    return data.ChangesList;
                },
            },
            {
                data: null,
                "width": "1%",
                render: function (data, type, row) {
                    return data.VirtualPath;
                },
                bSortable: false
            },
            { data: "ServicePackStatus", "width": "5%" }
        ]
        ,
        autoWidth: true

    });


    //$('#tblServicePackGrid_info').css('margin-left', '-285%');
    $('#tblServicePackGrid_info').css('font-size', '120%');
    $('.dataTables_length').css('float', 'left');
    
}

//Update Details
function EditServicePackDetails(encryptedID) {
   window.location.href = "/ServicePack/ServicePackDetails/EditServicePackDetails?id=" + encryptedID;
}

function DownloadServicePack(VirtualPath) {
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

//DeactivateServicePack 
function DeactivateServicePack(EncryptedID) {
    var flag = confirm('Are you sure,you want to deactivate service pack ??');
    if (EncryptedID == "" || flag == false) {
        return;
    }
    else {
        $.ajax({
            url: "/ServicePack/ServicePackDetails/DeactivateServicePack",
            type: 'POST',
            datatype: 'json',
            async: false,
            cache: false,
            data: { id: EncryptedID },
            success: function (json) {
                        //if the call is made to delete the relations
                        if (json.serverError == true && json.success == false) {
                            if (json.message != null || json.message != "") {
                                bootbox.alert(json.message, "Alert", function (confirmed) {
                                    if (confirmed) {
                                        //Added by shubham bhagat on 18-09-2019
                                        window.location.href = "/Home/HomePage"
                                        //window.location.href = "/Login/Error";
                                    }
                                });
                            }
                        }
                        else {
                            if (json.success == true) {
                                bootbox.alert(json.message, function () {
                                    DisplayServicePackDetailsList();
                                });
                            }
                        }
            },
            error: function (xhr, status, error) {
                bootbox.alert(error);
            }
        });//ajax call ends here
    }
    
}

function ActivateServicePack(EncryptedID) {
    var flag = confirm('Are you sure,you want to Activate service pack ??');
    if (EncryptedID == "" || flag == false) {
        return;
    }
    else {
        $.ajax({
            url: "/ServicePack/ServicePackDetails/ActivateServicePackDetailsEntry",
            type: 'POST',
            datatype: 'json',
            async: false,
            cache: false,
            data: { id: EncryptedID },
            success: function (json) {
                //if the call is made to delete the relations
                if (json.serverError == true && json.success == false) {
                    if (json.message != null || json.message != "") {
                        bootbox.alert(json.message, "Alert", function (confirmed) {
                            if (confirmed) {
                                //Added by shubham bhagat on 18-09-2019
                                window.location.href = "/Home/HomePage"
                                //window.location.href = "/Login/Error";
                            }
                        });
                    }
                }
                else {
                    if (json.success == true) {
                        bootbox.alert(json.message, function () {
                            DisplayServicePackDetailsList();
                        });
                    }
                }
            },
            error: function (xhr, status, error) {
                bootbox.alert(error);
            }
        });//ajax call ends here
    }
}
