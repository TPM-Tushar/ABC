//Global variables.
var token = '';
var header = {};

$(document).ready(function () {



    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#divYear').datepicker({
        format: "yyyy",
        viewMode: "years",
        minViewMode: "years",
        //yearRange: '2003:2022',
        //startYear: '2003',
        //endYear: 'new Date().getFullYear()',
        //maxYear: 'new Date().getFullYear()',
        //minYear: '2003',
        startYear: 2003,
        endYear: new Date().getFullYear(),

        autoclose: true //to close picker once year is selected
    });

    if ($('#idIsSelectAll').val() == 'False') {
        $('#idRegArticle option').attr('selected', false);
    }

    $("#idRegArticle").multiselect({
        includeSelectAllOption: true,
        enableFiltering: true,
        numberDisplayed: 10,
        nonSelectedText: 'None Selected',
        buttonWidth: '100%',
        enableCaseInsensitiveFiltering: true,
        maxHeight: 200

    });


    $('#DtlsToggleIconSearchParaListCollapse').trigger('click');



    $('#DROfficeListID').change(function () {
        blockUI('loading data.. please wait...');

        $.ajax({
            url: '/Remittance/ScheduleAllocationAnalysis/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROfficeListID').val() },
            datatype: "json",
            type: "GET",
            success: function (data) {

                if (data.serverError == true) {
                    unBlockUI();
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    unBlockUI();
                    $('#SROfficeListID').empty();
                    $.each(data.SROOfficeList, function (i, SROOfficeList) {
                        SROOfficeList
                        $('#SROfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });

    $('#DROfficeListID').focus();



    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchParaListCollapse').removeClass(classToRemove).addClass(classToSet);
    });

    $('#SearchParaDtls').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });



    $('input[type="checkbox"]').click(function () {

        var yes = document.getElementById("idPartyIdCheckBox");
        if (yes.checked == true) {
            //$('#idPartyIdMessage').html.clear();
            $('#idPartyIdMessage').html("<b>!=N</b>");
        }
        else {
            //$('#idPartyIdMessage').html.clear();
            $('#idPartyIdMessage').html("<b>N</b>");
        }
    });

    //Added By ShivamB on 27-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page
    $('input[type="checkbox"]').click(function () {

        var yes = document.getElementById("idIsThroughVerifyCheckBox");
        if (yes.checked == true) {
            //$('#idIsThroughVerifyMessage').html.clear();
            $('#idIsThroughVerifyMessage').html("<b>IsThroughVerify != N</b>");

        }
        else {
            //$('#idIsThroughVerifyMessage').html.clear();
            $('#idIsThroughVerifyMessage').html("<b>IsThroughVerify == N</b>");
        }
    });

    $('input[type="checkbox"]').click(function () {

        var yes = document.getElementById("idIsSelectAllYearCheckBox");
        if (yes.checked == true) {
            //$('#idIsThroughVerifyMessage').html.clear();
            //$('#idIsThroughVerifyMessage').html("<b>IsThroughVerify != N</b>");
            //$(".ui-datepicker-trigger").addClass("hide");
            //$("divYear").attr("disabled", "disabled");
            //$("#divYear").datepicker("option", "disabled", true)
            //$('#divYear').datepicker('disabled');
            //$("#datepicker").datepicker("disable");
            //$("#idYear").attr("disabled", "disabled");
            //$("#divYear").prop('disabled', true);
            //$("#idYear").prop('disabled', true);
            //$("#divYear").datepicker("disable");
            //$("#divYear").datepicker().datepicker('disable');
            //$('#divYear').focusin(function () {
            //    $('.glyphicon glyphicon-calendar CalendarIcon').css("display", "none");
            //});
            //$(".date").css("display", "none");
            //$(".date").css("display", "block");
            $('.date').hide();
            //$("#dcacl").prop('disabled', true);
            //$("#date").attr('disabled', 'disabled');
        }
        else {
            //$('#idIsThroughVerifyMessage').html.clear();
            //$('#idIsThroughVerifyMessage').html("<b>IsThroughVerify == N</b>");
            $('.date').show();
            //$("#idYear").attr("enabled", "enabled");
            //$(".date").css("display", "unblock");
            //$('#divYear').datepicker('enabled');


        }
    });

    //$(".filtertype").click(function () {
    //    $(".filtertypeinput").toggleDisabled();
    //});

    //(function ($) {
    //    $.fn.toggleDisabled = function () {
    //        return this.each(function () {
    //            this.disabled = !this.disabled;
    //            if ($(this).datepicker("option", "disabled")) {
    //                $(this).datepicker("option", "disabled", false);
    //            } else {
    //                $(this).datepicker("option", "disabled", true);
    //            }

    //        });
    //    };
    //})(jQuery);


    //Ended By ShivamB on 27-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page


    //$('#idPartyIdCheckBox').click(function () {
    //    var yes = document.getElementById("idPartyIdCheckBox");
    //    if (yes.checked == true) {
    //        $('#idPartyIdMessage').html.clear();
    //        $('#idPartyIdMessage').html("<b>Party Id == Null</b>");
    //    }
    //    else {
    //        $('#idPartyIdMessage').html.clear();
    //        $('#idPartyIdMessage').html("<b>Party Id != Null</b>");
    //    }
    //    //var inputValue = $(this).attr("value");
    //    //$("." + inputValue).toggle();
    //    //$ idPartyIdMessage
    //});



    $("#SearchBtn").click(function () {

        if ($.fn.DataTable.isDataTable("#ScheduleAllocationAnalysisID")) {
            $("#ScheduleAllocationAnalysisID").DataTable().clear().destroy();
        }

        SROfficeID = $("#SROfficeListID option:selected").val();
        DROfficeID = $("#DROfficeListID option:selected").val();
        YearID = $("#idYear").val();
        SelectedRegArticleText = $("#idRegArticle option:selected").text();
        RegArticleID = $("#idRegArticle").val();
        var RegArticleIdJoined = RegArticleID.join();

        var idPartyIdCheckBox = document.getElementById("idPartyIdCheckBox");
        var IsPartyIdCheckBoxSelected = idPartyIdCheckBox.checked;

        //Added By ShivamB on 27-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page
        var idIsThroughVerifyCheckBox = document.getElementById("idIsThroughVerifyCheckBox");
        var IsThroughVerifyCheckBoxSelected = idIsThroughVerifyCheckBox.checked;

        var idIsSelectAllYearCheckBox = document.getElementById("idIsSelectAllYearCheckBox");
        var IsSelectAllYearCheckBoxSelected = idIsSelectAllYearCheckBox.checked;
        //Ended By ShivamB on 27-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page


        //Commented By ShivamB on 27-09-2022 for adding All options in District DropDown parameter.
        //if (DROfficeID == "0") {
        //    if ($.fn.DataTable.isDataTable("#ScheduleAllocationAnalysisID")) {
        //        $("#ScheduleAllocationAnalysisID").DataTable().clear().destroy();
        //    }
        //    bootbox.alert({
        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any District" + '</span>',
        //        callback: function () {
        //            var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
        //            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        //                $('#DtlsSearchParaListCollapse').trigger('click');
        //        }
        //    });
        //}
        //Ended By ShivamB on 27-09-2022 for adding All options in District DropDown parameter.

        //Commented By ShivamB on 27-09-2022 for adding All options in SRODropDown parameter on selection of District.
        //else if (SROfficeID == "0") {
        //    if ($.fn.DataTable.isDataTable("#ScheduleAllocationAnalysisID")) {
        //        $("#ScheduleAllocationAnalysisID").DataTable().clear().destroy();
        //    }
        //    bootbox.alert({
        //        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any SRO" + '</span>',
        //        callback: function () {
        //            var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
        //            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
        //                $('#DtlsSearchParaListCollapse').trigger('click');
        //        }
        //    });
        //}
        //Ended By ShivamB on 27-09-2022 for adding All options in SRODropDown parameter on selection of District.

        if (RegArticleIdJoined == "0" || RegArticleIdJoined == ' ' || RegArticleIdJoined == "" || RegArticleIdJoined == 'undefined' || RegArticleIdJoined == null) {
            if ($.fn.DataTable.isDataTable("#ScheduleAllocationAnalysisID")) {
                $("#ScheduleAllocationAnalysisID").DataTable().clear().destroy();

            }
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any Registration Article" + '</span>',
                callback: function () {
                    var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                }
            });
        }


        if (IsSelectAllYearCheckBoxSelected == false && YearID == 'undefined') {
            //alert("in IsSelectAllYearCheckBoxSelected == false");
            //alert("YearId :" + YearID);


            if ($.fn.DataTable.isDataTable("#ScheduleAllocationAnalysisID")) {
                $("#ScheduleAllocationAnalysisID").DataTable().clear().destroy();

            }
            bootbox.alert({
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Please select any Year" + '</span>',
                callback: function () {
                    var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                }
            });


        }


        else {
            if ($.fn.DataTable.isDataTable("#ScheduleAllocationAnalysisID")) {
                $("#ScheduleAllocationAnalysisID").DataTable().clear().destroy();
            }

            var tableIndexReports = $('#ScheduleAllocationAnalysisID').DataTable({

                ajax: {
                    url: '/Remittance/ScheduleAllocationAnalysis/GetScheduleAllocationAnalysisDetails',
                    type: "POST",
                    headers: header,
                    data: {
                        'DROfficeID': DROfficeID, 'SROfficeID': SROfficeID, 'YearID': YearID, 'RegArticleID': RegArticleIdJoined, 'IsPartyIdCheckBoxSelected': IsPartyIdCheckBoxSelected, 'IsThroughVerifyCheckBoxSelected': IsThroughVerifyCheckBoxSelected, 'IsSelectAllYearCheckBoxSelected': IsSelectAllYearCheckBoxSelected
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
                                        var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
                                        if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                            $('#DtlsSearchParaListCollapse').trigger('click');
                                        $("#SearchParaDtls").trigger('click');
                                        $("#ScheduleAllocationAnalysisID").DataTable().clear().destroy();
                                        $("#PDFSPANID").html('');
                                        $("#EXCELSPANID").html('');
                                    }
                                    //$("#DtlsToggleIconSearchPara").trigger('click');
                                }
                            });
                        }
                        else {
                            var classToRemove = $('#DtlsToggleIconSearchParaListCollapse').attr('class');
                            if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                                $('#DtlsSearchParaListCollapse').trigger('click');

                            var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                            if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                $("#SearchParaDtls").trigger('click');
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
                        //$.unblockUI();
                    },
                    beforeSend: function () {
                        blockUI('loading data.. please wait...');
                        var searchString = $('#ScheduleAllocationAnalysisID_filter input').val();
                        if (searchString != "") {
                            var regexToMatch = /^[^<>]+$/;

                            if (!regexToMatch.test(searchString)) {
                                $("#ScheduleAllocationAnalysisID_filter input").prop("disabled", true);
                                bootbox.alert('Please enter valid Search String ', function () {
                                    //    $('#menuDetailsListTable_filter input').val('');

                                    tableIndexReports.search('').draw();
                                    $("#ScheduleAllocationAnalysisID_filter input").prop("disabled", false);

                                });
                                unBlockUI();
                                return false;
                            }
                        }
                    }
                },




                serverSide: true,
                // pageLength: 100,
                "scrollX": true,
                "scrollY": "300px",
                scrollCollapse: true,
                bPaginate: true,
                bLengthChange: true,
                // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
                // "pageLength": -1,
                //sScrollXInner: "150%",
                bInfo: true,
                info: true,
                bFilter: false,
                searching: true,
                "destroy": true,
                "bAutoWidth": true,
                "bScrollAutoCss": true,

                columnDefs: [

                    { "className": "dt-center", "targets": "_all" },
                    { "orderable": false, "targets": "_all" },
                    { "bSortable": false, "aTargets": "_all" },

                    { orderable: false, targets: [0], width: "10px" },
                    { orderable: false, targets: [1], width: "40px" },
                    { orderable: false, targets: [2], width: "40px" }

                ],

                columns: [
                    { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
                    { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
                    { data: "Stamp5DateTime", "searchable": true, "visible": true, "name": "Stamp5DateTime" },


                ],
                fnInitComplete: function (oSettings, json) {
                    console.log(json);
                    $("#PDFSPANID").html(json.PDFDownloadBtn);
                    $("#EXCELSPANID").html(json.ExcelDownloadBtn);
                    $("#DroName").html(json.DroName);
                    $("#SroName").html(json.SroName);
                    $("#Year").html(json.Year);

                },



                preDrawCallback: function () {
                    unBlockUI();
                },

                fnRowCallback: function (nRow, aData, iDisplayIndex) {
                    unBlockUI();
                    return nRow;
                },
                drawCallback: function (oSettings) {
                    //responsiveHelper.respond();
                    unBlockUI();
                },
            });
        }

    });



});

function PDFDownloadFun(DROfficeID, SROfficeID, RegArticleID, YearID, IsPartyIdCheckBoxSelectedID, IsThroughVerifyCheckBoxSelectedID, IsSelectAllYearCheckBoxSelectedID) {

    window.location.href = '/Remittance/ScheduleAllocationAnalysis/ExportScheduleAllocationAnalysisToPDF?DROfficeID=' + DROfficeID + "&SROfficeID=" + SROfficeID + "&RegArticleID=" + RegArticleID + "&YearID=" + YearID + "&IsPartyIdCheckBoxSelectedID=" + IsPartyIdCheckBoxSelectedID + "&IsThroughVerifyCheckBoxSelectedID=" + IsThroughVerifyCheckBoxSelectedID + "&IsSelectAllYearCheckBoxSelectedID=" + IsSelectAllYearCheckBoxSelectedID;

}


function EXCELDownloadFun(DROfficeID, SROfficeID, RegArticleID, YearID, IsPartyIdCheckBoxSelectedID, IsThroughVerifyCheckBoxSelectedID, IsSelectAllYearCheckBoxSelectedID) {
    window.location.href = '/Remittance/ScheduleAllocationAnalysis/ExportScheduleAllocationAnalysisToExcel?DROfficeID=' + DROfficeID + "&SROfficeID=" + SROfficeID + "&RegArticleID=" + RegArticleID + "&YearID=" + YearID + "&IsPartyIdCheckBoxSelectedID=" + IsPartyIdCheckBoxSelectedID + "&IsThroughVerifyCheckBoxSelectedID=" + IsThroughVerifyCheckBoxSelectedID + "&IsSelectAllYearCheckBoxSelectedID=" + IsSelectAllYearCheckBoxSelectedID;

}



