
//Global variables.
var token = '';
var header = {};
$(document).ready(function () {
    //alert("var :::"+PATH);


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    // BELOW CODE COMMENTED BY SHUBHAM BHAGAT ON 17-05-2019
    //// ADDED BY SHUBHAM BHAGAT ON 7-5-2019
    //// TO CHECK RADIO BUTTON ONLY FIRST TIME WHILE PAGE LOADING
    //var $radios = $('input:radio[name=ServiceName]');
    //if ($radios.is(':checked') === false) {
    //    $radios.filter('[value=ECDATA]').prop('checked', true);
    //}   

    //// Commented and added in dataSrc on 15-05-2019 by shubham bhagat
    //$('#driveinfocollapse').trigger('click');
    //LoadDriveInfoGrid();

    //// commented and added in datasrc on 15-05-2019 by shubham bhagat
    //$('#folderlistcollapse').trigger('click');
    //LoadFolderNameGrid();

    //// commented and added in datasrc on 15-05-2019 by shubham bhagat
    //$('#filelistcollapse').trigger('click');
    //LoadFileNameGrid();

    $('#folderListCollapse').click(function () {
        var classToRemove = $('#ToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#ToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    $('#fileListCollapse').click(function () {
        var classToRemove = $('#FileToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#FileToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    $('#driveInfoCollapse').click(function () {
        var classToRemove = $('#driveToggleIconID').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#driveToggleIconID').removeClass(classToRemove).addClass(classToSet);
    });

    // BELOW CODE COMMENTED BY SHUBHAM BHAGAT ON 17-05-2019
    //$("input[name='ServiceName']").click(function () {
    //    var radioValue = $("input[name='ServiceName']:checked").val();
    //    alert(radioValue);
    //    PATH = "root"; // FOR VIEWING ROOT FILES AND FOLDER OF BOTH SERVICES
    //    LoadDriveInfoGrid();
    //    LoadFolderNameGrid();
    //    LoadFileNameGrid();
    //});

    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 17-05-2019

    //alert($('#ApplicationNameID').val() != undefined);

    // TO LOAD ALL 3 DATA TABLE FIRST ON ON PAGE LOAD
    //if ($('#ApplicationNameID').val() != undefined)
    //{
    //    //alert('in !=undefined');
    //    //var dropDownValue = $('#ApplicationNameID').val();
    //    //alert('dropDownValue:::' + dropDownValue);
    //    LoadDriveInfoGrid();
    //    var driveclassToRemove = $('#driveToggleIconID').attr('class');
    //    if (driveclassToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
    //        $('#driveInfoCollapse').trigger('click');
    //    }

    //    LoadFolderNameGrid();
    //    var folderclassToRemove = $('#ToggleIconID').attr('class');
    //    if (folderclassToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
    //        $('#folderListCollapse').trigger('click');
    //    }

    //    LoadFileNameGrid();
    //    var fileclassToRemove = $('#FileToggleIconID').attr('class');
    //    if (fileclassToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
    //        $('#fileListCollapse').trigger('click');
    //    }
    //}

    $('#ErrorDirectoryNameID').change(function () {
        if ($('#ErrorDirectoryNameID').val() != "0") {
            //alert('IN DROP DOWN CHANGE');
            //var dropDownValue = $('#ApplicationNameID').val();
            //alert('dropDownValue:::' + dropDownValue);

            // Commented and changed on 18-05-2019 By shubham bhagat 
            //PATH = "root"; // FOR VIEWING ROOT FILES AND FOLDER OF BOTH SERVICES
            PATH = $('#ErrorDirectoryNameID').val(); // FOR VIEWING ROOT FILES AND FOLDER OF BOTH SERVICES

            LoadDriveInfoGrid();
            var driveclassToRemove = $('#driveToggleIconID').attr('class');
            if (driveclassToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                $('#driveInfoCollapse').trigger('click');
            }

            LoadFolderNameGrid();
            var folderclassToRemove = $('#ToggleIconID').attr('class');
            if (folderclassToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                $('#folderListCollapse').trigger('click');
            }

            LoadFileNameGrid();
            var fileclassToRemove = $('#FileToggleIconID').attr('class');
            if (fileclassToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                $('#fileListCollapse').trigger('click');
            }
        }
        else {
            //alert('in error directory change else');
            // TO CLEAR DATATABLE AND COLLAPSE

            $("#driveInfoTable").DataTable().clear().destroy();
            var driveclassToRemove = $('#driveToggleIconID').attr('class');
            if (driveclassToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                $('#driveInfoCollapse').trigger('click');
            }

            $("#folderDetailsTable").DataTable().clear().destroy();
            var folderclassToRemove = $('#ToggleIconID').attr('class');
            if (folderclassToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                $('#folderListCollapse').trigger('click');
            }
            $("#fileDetailsTable").DataTable().clear().destroy();

            var fileclassToRemove = $('#FileToggleIconID').attr('class');
            if (fileclassToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                $('#fileListCollapse').trigger('click');
            }

            $("#lblDisplayParentDir").text('');
            $("#lblDisplayParentDir1").text('');
            $("#backButtonSpanID").html('');
            $("#downloadAllButtonSpanID").html('');
        }
    });

    $('#ApplicationNameID').change(function () {
        //alert('fyuf');
        //alert($('#ApplicationNameID').val());
        if ($('#ApplicationNameID').val() != "0") {
            BlockUI();
            $.ajax({
                url: '/Remittance/ErrorLogFiles/GetErrorDirectoryList',
                datatype: "text",
                type: "GET",
                contenttype: 'application/json; charset=utf-8',
                async: true,
                data: { "ApplicationName": $('#ApplicationNameID').val() },
                success: function (data) {

                    if (data.errorMsg == undefined) {
                        $('#ErrorDirectoryNameID').empty();
                        $.each(data.ErrorDirectoryList, function (i, item) {
                            $('#ErrorDirectoryNameID').append('<option value="' + item.Value + '">' + item.Text + '</option>');
                        });

                        $("#driveInfoTable").DataTable().clear().destroy();
                        var driveclassToRemove = $('#driveToggleIconID').attr('class');
                        if (driveclassToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                            $('#driveInfoCollapse').trigger('click');
                        }

                        $("#folderDetailsTable").DataTable().clear().destroy();
                        var folderclassToRemove = $('#ToggleIconID').attr('class');
                        if (folderclassToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                            $('#folderListCollapse').trigger('click');
                        }
                        $("#fileDetailsTable").DataTable().clear().destroy();

                        var fileclassToRemove = $('#FileToggleIconID').attr('class');
                        if (fileclassToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                            $('#fileListCollapse').trigger('click');
                        }

                        $("#lblDisplayParentDir").text('');
                        $("#lblDisplayParentDir1").text('');
                        $("#backButtonSpanID").html('');
                        $("#downloadAllButtonSpanID").html('');
                    }
                    else {
                        bootbox.alert({
                            //size: 'small',
                            //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMsg + '</span>',
                            callback: function () {
                                window.location.href = "/Home/HomePage"
                            }
                        });
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
        }
        else {
            //alert('in aPPLICATION change else');
            // TO CLEAR DATATABLE AND COLLAPSE

            $("#driveInfoTable").DataTable().clear().destroy();
            var driveclassToRemove = $('#driveToggleIconID').attr('class');
            if (driveclassToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                $('#driveInfoCollapse').trigger('click');
            }

            $("#folderDetailsTable").DataTable().clear().destroy();
            var folderclassToRemove = $('#ToggleIconID').attr('class');
            if (folderclassToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                $('#folderListCollapse').trigger('click');
            }
            $("#fileDetailsTable").DataTable().clear().destroy();

            var fileclassToRemove = $('#FileToggleIconID').attr('class');
            if (fileclassToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                $('#fileListCollapse').trigger('click');
            }

            $("#lblDisplayParentDir").text('');
            $("#lblDisplayParentDir1").text('');
            $("#backButtonSpanID").html('');
            $("#downloadAllButtonSpanID").html('');

            $('#ErrorDirectoryNameID').empty();
            $("#ErrorDirectoryNameID").append(new Option("Select", "0"));

        }
    });

});

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

function LoadFolderNameGrid() {
    //alert('LoadFolderNameGrid'); 
    $("#folderDetailsTable").DataTable().destroy();

    var t = $("#folderDetailsTable").DataTable({
        //"bJQueryUI": true,
        //"bServerSide": true,
        "responsive": true,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        language: { search: "Search" },

        //"scrollCollapse": true,
        "scrollX": true,
        "scrollY": "30vh",
        scrollCollapse: true,
        //"scrollY": true,

        //"ordering": false,
        //autoWidth:true,
        //"deferRender": true,
        ajax: {
            url: "/Remittance/ErrorLogFiles/LoadFolderNameGrid",
            type: "POST",
            headers: header,

            beforeSend: function () {
                //Added by Akash(24-08-2018) to validate search string.
                var searchString = $('#folderDetailsTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                   
                    if (!regexToMatch.test(searchString)) {
                        $("#folderDetailsTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            t.search('').draw();
                            $("#folderDetailsTable_filter input").prop("disabled", false);
                        });
                        return false;
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
                            $("#folderDetailsTable").DataTable().clear().destroy();

                            var classToRemove = $('#ToggleIconID').attr('class');
                            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                                $('#folderListCollapse').trigger('click');
                            }
                        }
                    });
                }
                else {
                    unBlockUI();
                    return json.data;
                }
            },
            data: {
                "path": PATH,
                "ServiceName": $('#ApplicationNameID').val(),
                //"ServiceName": $("input[name='ServiceName']:checked").val(),
                "isBackward": IsBackward  //need to change               
            },
            datatype: "json"
        },
        columnDefs: [
            { orderable: false, targets: [0] },
            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] }
        ],

        "columns": [
            // For Showing Sr No
            { "data": "FileName", "name": "FileName", "autoWidth": true },
            { "data": "FileName", "name": "FileName", "autoWidth": true },
            { "data": "ActionBtn", "name": "ActionBtn", "autoWidth": true }

        ],
        fnInitComplete: function (oSettings, json) {
            //alert('in folder datatable complete');
            $("#lblDisplayParentDir").text(json.PresentDirectory);
            $("#lblDisplayParentDir1").text(json.PresentDirectory);
            $("#backButtonSpanID").html(json.BackButton);
            PATH = json.PresentDirectory;
        }
    });
    t.on('draw.dt', function () {
        var PageInfo = $('#folderDetailsTable').DataTable().page.info();
        t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + PageInfo.start;
        });
    });

    $('#folderDetailsTable_info').css('margin-left', '-2%');
    $('#folderDetailsTable_info').css('font-size', '120%');
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
}


function LoadFileNameGrid() {
    //alert('LoadFileNameGrid'); 
    $("#fileDetailsTable").DataTable().destroy();

    var t = $("#fileDetailsTable").DataTable({
        //"bJQueryUI": true,
        //"bServerSide": true,
        "responsive": true,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        language: { search: "Search" },

        //"scrollCollapse": true,
        "scrollX": true,
        "scrollY": "30vh",
        scrollCollapse: true,
        //"scrollY": true,

        //"ordering": false,
        //autoWidth:true,
        //"deferRender": true,
        ajax: {
            url: "/Remittance/ErrorLogFiles/LoadFileNameGrid",
            type: "POST",
            headers: header,
            beforeSend: function () {
                //Added by Akash(24-08-2018) to validate search string.
                var searchString = $('#fileDetailsTable_filter input').val();
               
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#fileDetailsTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            t.search('').draw();
                            $("#fileDetailsTable_filter input").prop("disabled", false);
                        });
                        return false;
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
                            $("#fileDetailsTable").DataTable().clear().destroy();

                            var classToRemove = $('#FileToggleIconID').attr('class');
                            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                                $('#fileListCollapse').trigger('click');
                            }
                        }
                    });
                }
                else {
                    unBlockUI();
                    return json.data;
                }
            },
            data: {
                "path": PATH,
                "ServiceName": $('#ApplicationNameID').val(),
                //"ServiceName": $("input[name='ServiceName']:checked").val(),
                "isBackward": IsBackward//need to change
            },
            datatype: "json"
        },
        columnDefs: [
            { orderable: false, targets: [0] },
            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] }
        ],

        "columns": [
            // For Showing Sr No
            { "data": "FileName", "name": "FileName", "autoWidth": true },
            { "data": "FileName", "name": "FileName", "autoWidth": true },
            { "data": "ActionBtn", "name": "ActionBtn", "autoWidth": true }

        ]
        ,
        fnInitComplete: function (oSettings, json) {
            //alert('DataTables has finished its initialisation.');    
            //alert('in file datatable complete');
            $("#downloadAllButtonSpanID").empty();
            if (json.setPresentDirectory) {
                $("#lblDisplayParentDir").text(json.PresentDirectory);
                $("#lblDisplayParentDir1").text(json.PresentDirectory);
                $("#backButtonSpanID").html(json.BackButton);
                PATH = json.PresentDirectory;
                //alert("File Complete Span value before empty :::" + $("#downloadAllButtonSpanID").html());

                //alert("File Complete Span value After empty:::" + $("#downloadAllButtonSpanID").html());
                $("#downloadAllButtonSpanID").html(json.DownloadAllButton);
                //alert("File Complete Span value After assign:::" + $("#downloadAllButtonSpanID").html());
            }
        }
    });
    t.on('draw.dt', function () {
        var PageInfo = $('#fileDetailsTable').DataTable().page.info();
        t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + PageInfo.start;
        });
    });

    $('#fileDetailsTable_info').css('margin-left', '-2%');
    $('#fileDetailsTable_info').css('font-size', '120%');
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
}


function OpenFolder(item) {
    //alert('OPEN FOLDER Before PATH value:::' + PATH);
    PATH = item;
    //alert('OPEN FOLDER After PATH value:::' + PATH);
    LoadFolderNameGrid();
    LoadFileNameGrid();
}

function BackFolder(path) {
    //alert('BACK FOLDER Before PATH value:::' + PATH);
    PATH = path;
    // alert('BACK FOLDER After PATH value:::' + PATH);
    IsBackward = true;
    LoadFolderNameGrid();
    LoadFileNameGrid();
}

// DOWNLOAD SINGLE FILE
function DownLoadFile(DownLoadFile) {
    //window.location.href = "/Remittance/ErrorLogFiles/DownLoadFile?FilePath=" + DownLoadFile + "&ServiceName=" + $("input[name='ServiceName']:checked").val();
    window.location.href = "/Remittance/ErrorLogFiles/DownLoadFile?FilePath=" + DownLoadFile + "&ServiceName=" + $('#ApplicationNameID').val();
}

// DOWNLOAD ZIPPED FILE
function DownLoadZippedFile(FolderPath) {
    //window.location.href = "/Remittance/ErrorLogFiles/DownLoadZippedFile?FolderPath=" + FolderPath + "&ServiceName=" + $("input[name='ServiceName']:checked").val();
    window.location.href = "/Remittance/ErrorLogFiles/DownLoadZippedFile?FolderPath=" + FolderPath + "&ServiceName=" + $('#ApplicationNameID').val();
}

function LoadDriveInfoGrid() {
    //alert('LoadDriveInfoGrid'); 

    $("#driveInfoTable").DataTable().destroy();

    var t = $("#driveInfoTable").DataTable({
        //"bJQueryUI": true,
        //"bServerSide": true,
        "responsive": true,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        language: { search: "Search" },

        //"scrollCollapse": true,
        "scrollX": true,
        "scrollY": "30vh",
        scrollCollapse: true,
        //"scrollY": true,

        //"ordering": false,
        //autoWidth:true,
        //"deferRender": true,
        ajax: {
            url: "/Remittance/ErrorLogFiles/LoadDriveInfoGrid",
            type: "POST",
            headers: header,
            beforeSend: function () {
                //Added by Akash(24-08-2018) to validate search string.
                var searchString = $('#driveInfoTable_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#driveInfoTable_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            t.search('').draw();
                            $("#driveInfoTable_filter input").prop("disabled", false);
                        });
                        return false;
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
                            $("#driveInfoTable").DataTable().clear().destroy();

                            var classToRemove = $('#driveToggleIconID').attr('class');
                            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x") {
                                $('#driveInfoCollapse').trigger('click');
                            }
                        }
                    });
                }
                else {
                    unBlockUI();
                    return json.data;
                }
            },
            data: {
                "ServiceName": $('#ApplicationNameID').val()
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
            { orderable: false, targets: [6] }
        ],

        "columns": [
            // For Showing Sr No
            { "data": "DriveName", "name": "DriveName", "autoWidth": true },
            { "data": "DriveName", "name": "DriveName", "autoWidth": true },
            { "data": "FreeSpace", "name": "FreeSpace", "autoWidth": true },
            { "data": "TotalSpace", "name": "TotalSpace", "autoWidth": true },
            { "data": "FileSystem", "name": "FileSystem", "autoWidth": true },
            { "data": "FreeSpacePercentage", "name": "FreeSpacePercentage", "autoWidth": true },
            { "data": "DriveType", "name": "DriveType", "autoWidth": true }

        ]
        //,
        //fnInitComplete: function (oSettings, json) {
        //    //alert('in drive datatable complete');
        //    PATH = json.PresentDirectory;
        //}
    });
    t.on('draw.dt', function () {
        var PageInfo = $('#driveInfoTable').DataTable().page.info();
        t.column(0, { page: 'current' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1 + PageInfo.start;
        });
    });

    $('#driveInfoTable_info').css('margin-left', '-2%');
    $('#driveInfoTable_info').css('font-size', '120%');
    $('.dataTables_scrollHeadInner').css('box-sizing', 'inherit');
}
