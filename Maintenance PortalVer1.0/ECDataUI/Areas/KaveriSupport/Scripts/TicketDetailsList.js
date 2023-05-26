$(document).ready(function () {
  
    $("ul#crumbs li:nth-child(1)").trigger('click');

    retriveTicketDetailsList();

    LoadPrivateKeyDetailsList();
 
    $('#ticketDetailsListCollapse').trigger('click');

    $('#PrivateKeyDetailsListCollapse').trigger('click');


    $('#ticketDetailsListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });


    $('#PrivateKeyDetailsListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID2').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID2').removeClass(classToRemove).addClass(classToSet);
    });


});


//function CollapseTicketDetailsList() {
//    $('#ticketDetailsListCollapse').trigger('click');
//    var classToRemove = $('#ToggleIconID').attr('class');
//    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-plus-square-o fa-pull-left fa-2x');
//}

//function OpenRoleDetailsList() {
//    $('#ticketDetailsListCollapse').trigger('click');
//    var classToRemove = $('#ToggleIconID').attr('class');
//    $('#ToggleIconID').removeClass(classToRemove).addClass('fa fa-minus-square-o fa-pull-left fa-2x');
//}


function retriveTicketDetailsList() {
 //   $("#editRoleDetailsHeadingId").hide();
    $("#TicketDetailsListTable").DataTable().destroy();


    var t = $("#TicketDetailsListTable").DataTable({
        "ordering": false,
        "responsive": true,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        language: {search: "Search Ticket Number"},
        bProcessing: true,
        "scrollX": true,
        "scrollY": "30vh",
        //scrollCollapse: true,
    
        ajax: {
            url: "/KaveriSupport/KaveriSupport/LoadTicketDetailsList",
            type: "POST",
            beforeSend: function () {

                blockUI('Loading.. please wait...');

                var searchString = $('#TicketDetailsListTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        unBlockUI();

                        $("#TicketDetailsListTable_filter input").prop("disabled", true);

                        bootbox.alert('Please enter valid Search String ', function () {
                            t.search('').draw();
                            $("#TicketDetailsListTable_filter input").prop("disabled", false);
                        });
                        return false;

                    }
                }
                unBlockUI();

            },
            // Added on 15-05-2019 by shubham
            dataSrc: function (json) {
                if (json.errorMessage != undefined) {
                    unBlockUI();
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            $("#TicketDetailsListTable").DataTable().clear().destroy();

                            $('#ticketDetailsListCollapse').trigger('click');


                            //  var classToRemove = $('#ToggleIconID2').attr('class');
                            //if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                            //    $('#PrivateKeyDetailsListCollapse').trigger('click');
                            //}


                        }
                    });
                }
                else {
                    unBlockUI();
                    return json.data;
                }
            },
            datatype: "json"
        },
        columnDefs: [
           

            //{ orderable: false, targets: [0] },
            //{ orderable: false, targets: [1] },
            //{ orderable: false, targets: [2] },
            //{ orderable: false, targets: [3] },           
            //{ orderable: false, targets: [4] },
            //{ orderable: false, targets: [5] },
            //{ orderable: false, targets: [6] }
      
        ],
        "columns": [
            // For Showing Sr No
            { "data": "SrNo", "name": "SrNo", "autoWidth": true },
            { "data": "TicketNumber", "name": "TicketNumber", "autoWidth": true },
            { "data": "TicketDescription", "name": "TicketDescription", "autoWidth": false },
            { "data": "ModuleName", "name": "ModuleName", "autoWidth": true },
            { "data": "Office", "name": "Office", "autoWidth": true },
            { "data": "RegistrationDateTime", "name": "RegistrationDateTime", "autoWidth": true },
            {
                "data": "IsActive", render: function (data, type, row) {
                    return (data == true) ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";
                }
            }

            //{ "data": "SrNo", "name": "SrNo", "autoWidth": false },
            //{ "data": "TicketNumber", "name": "TicketNumber"},
            //{ "data": "TicketDescription", "name": "TicketDescription" },
            //{ "data": "ModuleName", "name": "ModuleName" },
            //{ "data": "Office", "name": "Office"},
            //{ "data": "RegistrationDateTime", "name": "RegistrationDateTime" },
            //{
            //    "data": "IsActive", render: function (data, type, row) {
            //        return (data == true) ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";
            //    }
            //}

        

        ]
    });
    t.on('draw.dt', function () {
        var PageInfo = $('#TicketDetailsListTable').DataTable().page.info();
        t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + PageInfo.start;
        });
    });



    //$('#roleDetailsListTable_info').addClass('col-md-4');
    $('#TicketDetailsListTable_info').css('margin-left', '-2%');
    $('#TicketDetailsListTable_info').css('font-size', '120%');
    //$('#roleDetailsListTable_paginate').css('margin', '-20px');
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
    //debugger;
    //table.responsive.recalc();
    //table.columns.adjust().draw();
}


function LoadPrivateKeyDetailsList() {
    //   $("#editRoleDetailsHeadingId").hide();
    $("#PrivateKeyDetailsListTable").DataTable().destroy();


    var t = $("#PrivateKeyDetailsListTable").DataTable({
        "ordering": false,
        "responsive": true,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        language: { search: "Search Ticket Number" },
       

        "scrollX": true,
        "scrollY": "30vh",
        scrollCollapse: true,

        ajax: {
            url: "/KaveriSupport/KaveriSupport/LoadPrivateKeyDetailsList",
            type: "POST",
            beforeSend: function () {

                blockUI('Loading.. please wait...');

                var searchString = $('#PrivateKeyDetailsListTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        unBlockUI();

                        $("#PrivateKeyDetailsListTable_filter input").prop("disabled", true);

                        bootbox.alert('Please enter valid Search String ', function () {
                            t.search('').draw();
                            $("#PrivateKeyDetailsListTable_filter input").prop("disabled", false);
                        });
                        return false;

                    }
                }
                unBlockUI();

            },
            // Added on 15-05-2019 by shubham
            dataSrc: function (json) {
                if (json.errorMessage != undefined) {
                    unBlockUI();
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                        callback: function () {
                            $("#PrivateKeyDetailsListTable").DataTable().clear().destroy();

                            $('#PrivateKeyDetailsListCollapse').trigger('click');


                          //  var classToRemove = $('#ToggleIconID2').attr('class');
                            //if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                            //    $('#PrivateKeyDetailsListCollapse').trigger('click');
                            //}


                        }
                    });
                }
                else {
                    unBlockUI();
                    return json.data;
                }
            },
            datatype: "json"
        },
        columnDefs: [
        //    { orderable: false, targets: [0] }
        //{ orderable: false, targets: [2] },
        //{ orderable: false, targets: [3] },
        //{ orderable: false, targets: [4] }

        ],



        "columns": [
            // For Showing Sr No
            { "data": "SrNo", "name": "SrNo", "autoWidth": true },
            { "data": "TicketNumber", "name": "TicketNumber", "autoWidth": true },
            { "data": "FileName", "name": "FileName", "autoWidth": false },
            { "data": "UploadDateTime", "name": "UploadDateTime", "autoWidth": true },
            {
                "data": "IsActive", render: function (data, type, row) {
                    return (data == true) ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";
                }
            }



        ]
    });
    t.on('draw.dt', function () {
        var PageInfo = $('#PrivateKeyDetailsListTable').DataTable().page.info();
        t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + PageInfo.start;
        });
    });



    //$('#roleDetailsListTable_info').addClass('col-md-4');
    $('#PrivateKeyDetailsListTable_info').css('margin-left', '-2%');
    $('#PrivateKeyDetailsListTable_info').css('font-size', '120%');
    //$('#roleDetailsListTable_paginate').css('margin', '-20px');
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
    //debugger;
    //table.responsive.recalc();
    //table.columns.adjust().draw();
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




// To arrange Table header with Table body when left toggle button is clicked Added By Shubham Bhagat
$('#togglebtnId').click(function () {
    setTimeout(function () {
        $('thead').children().children().eq(1).click();
        $('thead').children().children().eq(1).click();
    }, 275);
});