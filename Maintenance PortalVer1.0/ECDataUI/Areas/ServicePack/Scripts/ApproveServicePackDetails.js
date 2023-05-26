/*----------------------------------------------------------------------------------------
    * Project Id    :KARIGR [ IN-KA-IGR-02-05 ]
    * Project Name  :Kaveri Maintainance Portal
    * Name          :ApproveServicePackDetails.js
    * Description   :     
    * Author        :Harshit   
    * Creation Date :01/06/2019
    * Modified By   :   
    * Updation Date :17 May 2019
    * ECR No : 300
----------------------------------------------------------------------------------------*/
$(document).ready(function () {

    DisplayApproveServicePackDetailsList();
    $("#btnSubmitReleaseNotes").click(AddReleaseNotes);
    $('#divFromDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        //yearRange: '2018:2019',
        //minDate: '07/12/2018',
        // minDate: new Date(2018, 12, 7),
        pickerPosition: "bottom-left"

    });


    $("#DateRequired").change(function () {        
        //do something
        if ($("#DateRequired").val() == "") {
            $('#ReleaseDateValID').text("Please Select Release Date");            
        }
        else {
            $('#ReleaseDateValID').text("");
        }
    });

    $("#txtReleaseNotes").change(function () {
        //do something
        if ($("#txtReleaseNotes").val() != "") {
            var patt = new RegExp("^[a-zA-Z0-9-/., ]+$");
            var ReleaseNotesbool = patt.test($('#txtReleaseNotes').val());
            if (!ReleaseNotesbool) {
                $('#ReleaseNotesValID').text("Invalid Release Note");
            } else
            {
                $('#ReleaseNotesValID').text("");
            }
        }
        else {
            $('#ReleaseNotesValID').text("");
        }
    });
});


function DisplayApproveServicePackDetailsList() {
    var tableElement = $('#tblServicePackGrid');
    tableElement.dataTable().fnDestroy();
    tableElement.DataTable({
        ajax: {
            url: "/ServicePack/ServicePackDetails/GetServicePackDetailsList",
            type: "POST",
            data: { IsRequestForApprovalsList: "true" },
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
            { data: "ReferenceID", className: "text-center", "width": "1%" },
            //{
            //    data: null,
            //    className: "center",
            //    "width": "1%",
            //    render: function (data, type, row) {
            //        return "<div class='text-center'><span style='cursor:pointer;font-size:16px;color:green;' class='fa fa-check' title='Click here to Approve Service Pack' onClick ='ApproveServicePack(\"" + data.EncryptedId.toString() + "\")' /></div>";
            //    },
            //    bSortable: false
            //},
            { data: "ApproveBTn", className: "text-center", "width": "1%" },
            { data: "ReleaseType", "width": "1%" },
            { data: "IsTestOrFinal", "width": "1%" },
            { data: "MajorVersion", "width": "1%" },
            { data: "MinorVersion", "width": "1%" },
            //Added By Tushar on 2 Jan 2023 for Upload DateTime
            { data: "ServicePackAddedDateTime", "width": "1%", className: "text-center" },
            //End By Tushar on 2 Jan 2023
            { data: "SPDescription", "width": "5%" },
            { data: "InstallationProc", "width": "5%" },
            {
                data: null,
                className: "center",
                sorting: "false",
                "width": "5%",
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
            }
        ]
        ,
        autoWidth: false

    });

    $('#tblServicePackGrid_info').css('font-size', '120%');
    $('.dataTables_length').css('float', 'left');
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

function ApproveServicePack(EncryptedID) {
    $.ajax({
        type: 'POST',
        datatype: 'json',
        url: '/ServicePack/ServicePackDetails/GetServicePackDetailsForApprovalAndRelease',
        async: false,
        cache: false,
        data: { EncryptedID: EncryptedID },
        success: function (data) {
            $("#dvServicePackDetailsSummaryModalBody").html(data);
            $("#dvServicePackDetailsSummaryMain").show();
            $("#dvServicePackDetailsSummaryModal").modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

$('#dvServicePackDetailsSummaryModal').on('shown.bs.modal', function () {
    $('.input-group.date').datepicker({
        format: "dd/mm/yyyy",
        startDate: new Date(),
        todayBtn: "linked",
        autoclose: true,
        todayHighlight: true,
    });
});


function AddReleaseNotes() {
    if ($("#DateRequired").val() == "") {
        $('#ReleaseDateValID').text("Please Select Release Date");
        //bootbox.alert("Please Select Release Date");
        //$("#dvServicePackDetailsSummaryModal").modal('hide');
        return;
    }

    //Added by shubham bhagat on 07-10-2019
    //commented below check and added in MVC Controller 
    //Added by shubham bhagat on 05-10-2019
    if ($("#txtReleaseNotes").val() != "") {
        var patt = new RegExp("^[a-zA-Z0-9-/., ]+$");

        var ReleaseNotesbool = patt.test($('#txtReleaseNotes').val());
        if (!ReleaseNotesbool) {
            //alert('fgre');
            $('#ReleaseNotesValID').text("Invalid Release Note");
            //bootbox.alert("Invalid Release Notes");
            //$("#dvServicePackDetailsSummaryModal").modal('hide');
            return;
            //window.location.reload();
        }
    }

    event.preventDefault();
    var formData = new FormData();

    //if ($('#txtReleaseNotes').val().trim()=="") {
    //    bootbox.alert("Please enter Release note.");
    //    return;
    //}



    formData.append("ReleaseNotes", $('#txtReleaseNotes').val());
    formData.append("SpID", $('#hdnEncryptedID').val());
    formData.append("ReleaseDate", $("#DateRequired").val());

    var RequestToken = $('[name=__RequestVerificationToken]').val();
    formData.append("__RequestVerificationToken", RequestToken);

    $.ajax({
        url: "/ServicePack/ServicePackDetails/AddReleaseNotes",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.serverError == true && response.success == false) {
                if (response.message != null || response.message != "") {
                    bootbox.alert({
                        message: response.message,
                        callback: function () {
                            //Added by shubham bhagat on 18-09-2019
                            window.location.href = "/Home/HomePage"
                            //window.location.href = "/Login/Error";
                        }
                    });
                }
            }
            else {
                if (response.success) {
                    bootbox.alert({
                        message: response.message,
                        callback: function () {
                            window.location.reload();
                        }
                    });
                }
                else {
                    if (response.errorType == 1)
                    {
                        $('#ReleaseDateValID').text(response.message);
                    }
                    else if (response.errorType == 2)
                    {
                        $('#ReleaseNotesValID').text(response.message);
                    }
                    else {
                        bootbox.alert(response.message);
                        //$("#dvServicePackDetailsSummaryMain").show();                    
                        //$("#dvServicePackDetailsSummaryModal").modal('show');                    
                        // ADDED BY SHUBHAM BHAGAT ON 07-10-2019
                        $("#dvServicePackDetailsSummaryModal").modal('hide');
                        return;
                    }
                }
            }
        },
        error: function (xhr, status, err) {
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
            });

        }
    });

}