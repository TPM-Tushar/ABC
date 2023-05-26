//Global variables.
var token = '';
var header = {};
var AppVersionDtlsID;
var selectedlist = [];



$(document).ready(function () {
   
    $('#rdnIsSROOffice').prop('checked', true)

    // alert('ApplyAppVersionReady');
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


   

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var TodaysDate = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();
    var FromDate;
    var ToDate;
    var SROOfficeID;
    var DROfficeID;
 
    var ApplicationNameListID;
    // var SelectedOffice = new Array();
    //var SelectedOffice;

    // var selectedOfc;

    $('#dtReleaseDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',

        pickerPosition: "bottom-left"//,

    });


    $('#dtLastDateForPatchUpdate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+15d',
        pickerPosition: "bottom-left",
        minDate: '0',
        startDate: '+0d',
    });

    $('#divReleaseDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',

        maxDate: new Date(),

        pickerPosition: "bottom-left"

    });

    $('#divLastDateForPatchUpdate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+15d',
        minDate: '0',
        pickerPosition: "bottom-left",
        startDate: '+0d',


    });


    $('#dtnLastDateForPatchUpdate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+15d',

        pickerPosition: "bottom-left"

    });



    $('#dtReleaseDate').datepicker({
        format: 'dd/mm/yyyy',
        changeMonth: true,
        changeYear: true
    }).datepicker("setDate", TodaysDate);


    $('#dtSPExecutionDateTime').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: '0',
        pickerPosition: "bottom-left"

    });

    $('#dtSPExecutionDateTime').datepicker({
        format: 'dd/mm/yyyy',
        endDate: '+7d',
        maxDate: new Date(),

    }).datepicker("setDate", TodaysDate);


    $('#divSPExecutionDateTime').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        maxDate: new Date(),
        pickerPosition: "bottom-left"

    });



    $("#selectall").click(function () {
      

        $('input:checkbox').prop('checked', this.checked);
    });



    $('input[type=radio]').change(function () {

      //  alert('Radio button changed');

        if (this.value == '1') {
           // alert('SR');

            SRDRList();

        }

        if (this.value == '2') {
           // alert('DR');

            DRList();

        }



    });


    

    document.getElementById("ApplicationNameListID").onchange = function () {

        $('#rdnIsSROOffice').prop('checked', true)
        //  alert('Go Clicked');
        var classToRemove = $('#ToggleIconID').attr('class');
        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
            $('#scriptManagerListCollapse').trigger('click');


        ApplicationNameListID = $("#ApplicationNameListID option:selected").val();
        //alert(ApplicationNameListID);


        if (ApplicationNameListID == "Select Application name" || ApplicationNameListID == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select application name from dropdown</span>');
            return;

        }


        document.getElementById("selectedAppname").innerHTML = ApplicationNameListID;

      

        SRDRList();


        $("#ApplyAppVersionTable").show();

    }

    $("#btnApplyAppVersion").click(function () {

        var value = $('input[type=radio]:checked').val();
      //  alert(value);

        if (value == 1) {

            if (ApplicationNameListID == "Select Application name" || ApplicationNameListID == "") {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select application name from dropdown</span>');
                return;

            }
            // alert('Apply Clicked');
            AppVersionDtlsID.search('').draw();

            // Append String with /n

            // var items = document.getElementsByName('SROId');

            var items = document.getElementsByClassName('messageCheckbox');


            selectedlist = [];

            for (var i = 0; i < items.length; i++) {
                if (items[i].type == 'checkbox' && items[i].checked == true)
                    //selectedlist += items[i].value + "-";
                    selectedlist = selectedlist + items[i].value + ",";

            }

           // alert(selectedlist);



            // alert(selectedlist.length);


            if (($("#txtAppMajor").val() == "") || ($("#txtAppMajor").val() == 0)) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">App Major is required</span>');
                return;

            }
            if ($("#txtAppMinor").val() == "") {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">App Minor is required</span>');
                return;

            }

            var txtReleaseDate = $("#dtReleaseDate").val();
            var txtLastDate = $("#dtLastDateForPatchUpdate").val();
            var txtAppMajor = $("#txtAppMajor").val();
            var txtAppMinor = $("#txtAppMinor").val();
            var txtAppName = ApplicationNameListID;
            // alert(ApplicationNameListID);
            //var selectedOfc = JSON.stringify(SelectedOffice);
            if (txtReleaseDate == "") {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Release Date is required</span>');
                return;

            }

            if (txtLastDate == "") {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Last date for patch update is required</span>');
                return;

            }

            if (selectedlist.length == 0) {

                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select atleast one SRO from list</span>');
                return;

            }



            bootbox.confirm({
                message: "Update application version " + " App Major : " + txtAppMajor + "   App Minor : " + txtAppMinor + "  for Application  " + ApplicationNameListID + "?",
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-success'
                    },
                    cancel: {
                        label: 'No',
                        className: 'btn-danger'
                    }
                },
                callback: function (result) {
                    if (result == true) {


                        //BlockUI();
                        // blockUI('updating application version..please wait');

                        // Ajax Call
                        $.ajax({
                            type: 'POST',
                            datatype: 'json',
                            headers: header,
                            url: '/SROScriptManager/SROScriptManager/ApplyAppVersion',
                            async: false,
                            traditional: true,


                            cache: false,
                            data: {
                                txtAppName: txtAppName, txtReleaseDate: txtReleaseDate, txtLastDate: txtLastDate, txtAppMajor: txtAppMajor, txtAppMinor: txtAppMinor, selectedOfc: selectedlist
                            },
                            success: function (data) {
                                if (data.success) {
                                    bootbox.alert({
                                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                        callback: function () {
                                            window.location.href = "/SROScriptManager/SROScriptManager/ApplyAppVersionView";
                                        }
                                    });
                                    unblockUI();

                                }
                                else {
                                    bootbox.alert({
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                    });
                                    unblockUI();

                                }


                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                            }
                        });


                    }
                    else {
                        return;
                    }
                }
            });



        }

        if (value == 2) {

            if (ApplicationNameListID == "Select Application name" || ApplicationNameListID == "") {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select application name from dropdown</span>');
                return;

            }
            // alert('Apply Clicked');
            AppVersionDtlsID.search('').draw();

            // Append String with /n

            // var items = document.getElementsByName('SROId');

            var items = document.getElementsByClassName('messageCheckbox');


            selectedlist = [];

            for (var i = 0; i < items.length; i++) {
                if (items[i].type == 'checkbox' && items[i].checked == true)
                    //selectedlist += items[i].value + "-";
                    selectedlist = selectedlist + items[i].value + ",";

            }


          //  alert(selectedlist);



            // alert(selectedlist.length);


            if (($("#txtAppMajor").val() == "") || ($("#txtAppMajor").val() == 0)) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">App Major is required</span>');
                return;

            }
            if ($("#txtAppMinor").val() == "") {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">App Minor is required</span>');
                return;

            }

            var txtReleaseDate = $("#dtReleaseDate").val();
            var txtLastDate = $("#dtLastDateForPatchUpdate").val();
            var txtAppMajor = $("#txtAppMajor").val();
            var txtAppMinor = $("#txtAppMinor").val();
            var txtAppName = ApplicationNameListID;
            // alert(ApplicationNameListID);
            //var selectedOfc = JSON.stringify(SelectedOffice);
            if (txtReleaseDate == "") {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Release Date is required</span>');
                return;

            }

            if (txtLastDate == "") {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt"> Last date for patch update is required</span>');
                return;

            }

            if (selectedlist.length == 0) {

                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select atleast one SRO from list</span>');
                return;

            }



            bootbox.confirm({
                message: "Update application version " + " App Major : " + txtAppMajor + "   App Minor : " + txtAppMinor + "  for Application  " + ApplicationNameListID + "?",
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-success'
                    },
                    cancel: {
                        label: 'No',
                        className: 'btn-danger'
                    }
                },
                callback: function (result) {
                    if (result == true) {


                        //BlockUI();
                        // blockUI('updating application version..please wait');

                        // Ajax Call
                        $.ajax({
                            type: 'POST',
                            datatype: 'json',
                            headers: header,
                            url: '/SROScriptManager/SROScriptManager/ApplyAppVersionDR',
                            async: false,
                            traditional: true,


                            cache: false,
                            data: {
                                txtAppName: txtAppName, txtReleaseDate: txtReleaseDate, txtLastDate: txtLastDate, txtAppMajor: txtAppMajor, txtAppMinor: txtAppMinor, selectedOfc: selectedlist
                            },
                            success: function (data) {
                                if (data.success) {
                                    bootbox.alert({
                                        message: '<i class="fa fa-check text-success boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>',
                                        callback: function () {
                                            window.location.href = "/SROScriptManager/SROScriptManager/ApplyAppVersionView";
                                        }
                                    });
                                    unblockUI();

                                }
                                else {
                                    bootbox.alert({
                                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.message + '</span>'
                                    });
                                    unblockUI();

                                }


                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                            }
                        });


                    }
                    else {
                        return;
                    }
                }
            });



        }


    });



  

//Document ready finishes below
});




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


function SRDRList() {
   // alert('SRDRListcall');


    if ($.fn.DataTable.isDataTable("#AppVersionDtlsID")) {
        $("#AppVersionDtlsID").DataTable().clear().destroy();
    }

   

     AppVersionDtlsID = $('#AppVersionDtlsID').DataTable({
        order: [[1, 'asc']],
      
        ajax: {

            url: '/SROScriptManager/SROScriptManager/SRDRList',
            type: "POST",
            headers: header,
            data: {
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                              //  window.location.href = "/Home/HomePage"
                            }
                            else {


                                $("#AppVersionDtlsID").DataTable().clear().destroy();
                            }
                        }
                    });
                }
                else {

                    $("#AppVersionDtlsID").show();

                }
                unBlockUI();
                return json.data;
            },
            error: function () {

                bootbox.alert({
                   message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    
                });

                unBlockUI();
            },

        },
        bserverSide: true,
        //"pageLength": 10,
        "scrollY": "300px",
        "scrollCollapse": true,
        bPaginate: false,
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
            { width: '10%', targets: [0] },
          

            { width: '20%', targets: [1] },
            { width: '20%', targets: [2] },

           
        ],

        columns: [
            {
                data: "SROId", "searchable": true, "visible": true, "name": "SROId",
                render: function (data, type, full, meta) {

                    return '<input type="checkbox" id="box"  class="messageCheckbox" value=" '+ data +'">';

                },
               
            },

           // { data: "SROId", "searchable": true, "visible": true, "name": "SROId" },
            { data: "DROOfficeName", "searchable": true, "visible": true, "name": "DROOfficeName" },
            { data: "SROOfficeName", "searchable": true, "visible": true, "name": "SROOfficeName" },



        ],
        fnInitComplete: function (oSettings, json) {


        },
        preDrawCallback: function () {
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
           
        },
        drawCallback: function (oSettings) {

            unBlockUI();
        },
    });





}


function DRList() {
   


    if ($.fn.DataTable.isDataTable("#AppVersionDtlsID")) {
        $("#AppVersionDtlsID").DataTable().clear().destroy();
    }



    AppVersionDtlsID = $('#AppVersionDtlsID').DataTable({
        order: [[1, 'asc']],

        ajax: {

            url: '/SROScriptManager/SROScriptManager/DRList',
            type: "POST",
            headers: header,
            data: {
            },
            dataSrc: function (json) {
                unBlockUI();
                if (json.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            if (json.serverError != undefined) {
                                //  window.location.href = "/Home/HomePage"
                            }
                            else {


                                $("#AppVersionDtlsID").DataTable().clear().destroy();
                            }
                        }
                    });
                }
                else {

                    $("#AppVersionDtlsID").show();

                }
                unBlockUI();
                return json.data;
            },
            error: function () {

                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',

                });

                unBlockUI();
            },

        },
        bserverSide: true,
        //"pageLength": 10,
        "scrollY": "300px",
        "scrollCollapse": true,
        bPaginate: false,
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
            { width: '10%', targets: [0] },


            { width: '20%', targets: [1] },
            { width: '20%', targets: [2] },


        ],

        columns: [
            {
                data: "DROId", "searchable": true, "visible": true, "name": "DROId",
                render: function (data, type, full, meta) {

                    return '<input type="checkbox" id="box"  class="messageCheckbox" value=" ' + data + '">';

                },

            },

            // { data: "SROId", "searchable": true, "visible": true, "name": "SROId" },
            { data: "DROOfficeName", "searchable": true, "visible": true, "name": "DROOfficeName" },
            { data: "SROOfficeName", "searchable": true, "visible": true, "name": "SROOfficeName" },



        ],
        fnInitComplete: function (oSettings, json) {


        },
        preDrawCallback: function () {
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {

        },
        drawCallback: function (oSettings) {

            unBlockUI();
        },
    });





}



