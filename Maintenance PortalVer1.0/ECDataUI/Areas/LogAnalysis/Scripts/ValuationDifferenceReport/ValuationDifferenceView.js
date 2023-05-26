var FromDate;
var ToDate;
var SROOfficeListID;
//var DROfficeID;
//Global variables.
var token = '';
var PropertyTypeListID;

var header = {};
var PropertyTypeName;
$(document).ready(function () {

    //$('[data-toggle="popover"]').popover();

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    $('#PropertyTypeListID').change(function () {
        $("#tableValuationRptTableID").DataTable().clear().destroy();
        $("#SummaryBtnSpanID").hide();
        $("#TblPanelID").css('display', 'none');
        $('#EXCELSPANID').hide();
        $("#spnDetailTblHeader").html('');


        $(".dt-tableClass").DataTable().clear().destroy();
        $(".dt-tableClass").css('display', 'none');


        var selectedVal=($(this).val());
        if (selectedVal == 3) {
            $("#spn-Aprtmnt-DutyLabel").text("Duty Payable @ 5.6% as conveyance");
            $("#spn-Aprtmnt-DutyPaidLabel").text("Stamp Duty Payable @ 5.6%");
        }
        if (selectedVal == 4) {

            $("#spn-Aprtmnt-DutyLabel").text("Duty Payable @ 5.65% as conveyance");
            $("#spn-Aprtmnt-DutyPaidLabel").text("Stamp Duty Payable @ 5.65%");
        }


    });


    //********** To allow only Numbers in textbox **************
    $(".AllowOnlyNumber").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            //    $("#errmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });


    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });

    $("#SearchBtn").click(function () {
        $("#SummaryBtnSpanID").show();

        PropertyTypeListID = $("#PropertyTypeListID option:selected").val();
        PropertyTypeName = $("#PropertyTypeListID option:selected").text();

        var RegArticleIdArr = $("#ddRegArticleList").val().toString();;
     
        $("#TblPanelID").css('display', 'block');
        $("#tableValRptDetailsTableID").hide();

        var tableValuationRptTable = $('#tableValuationRptTableID').DataTable({
            ajax: {
                url: '/LogAnalysis/ValuationDifferenceReport/GetValuationDiffRptData',
                type: "POST",
                headers: header,
                data: {
                    'RegArticleIdArr': RegArticleIdArr,
                    'PropertyTypeListID': PropertyTypeListID
                },
                dataSrc: function (json) {
                    unBlockUI();
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
                                    $("#tableValuationRptTableID").DataTable().clear().destroy();
                                    //$("#PDFSPANID").html('');
                                    $("#EXCELSPANID").html('');
                                    // Added by shubham bhagat on 18-07-2019
                                    $('#jurisdictionalSummaryTableID').html('');
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
                    unBlockUI();
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                },
                beforeSend: function () {
                    blockUI('Loading data please wait.');
                    // Added by SB on 22-3-2019 at 11:06 am
                    var searchString = $('#tableValuationRptTableID_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;

                        if (!regexToMatch.test(searchString)) {
                            $("#tableValuationRptTableID_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                tableValuationRptTable.search('').draw();
                                $("#tableValuationRptTableID_filter input").prop("disabled", false);
                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
            },
            serverSide: true,
            // pageLength: 100,
            "scrollX": false,
            "scrollY": "300px",
            //sScrollXInner: "50%",
            scrollCollapse: true,
            bPaginate: false,
            bLengthChange: true,
            // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
            // "pageLength": -1,
            bInfo: true,
            info: true,
            bFilter: false,
            searching: true,
            "destroy": true,
            "bAutoWidth": true,
            "bScrollAutoCss": true,
            "lengthMenu": [[350], ["All"]],
            //"pageLength": 350,
            "bScrollAutoCss": true,
            columnDefs: [
                { orderable: false, targets: [0] },
                { orderable: false, targets: [1] },
                { orderable: false, targets: [2] },
                { orderable: false, targets: [3] },
                { orderable: false, targets: [4] },
                { orderable: false, targets: [5] },



            ],
            columns: [
                { data: "SerialNo", "searchable": true, "visible": true, "name": "SerialNo" },
                { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
                { data: "TansactionsDone", "searchable": true, "visible": true, "name": "TansactionsDone" },
                { data: "StampDutyRecovery", "searchable": true, "visible": true, "name": "StampDutyRecovery" },
                { data: "Registration_Fees_Recovery__Probable_", "searchable": true, "visible": true, "name": "Registration_Fees_Recovery__Probable_" },
                { data: "Total", "searchable": true, "visible": true, "name": "Total" }

            ],
            fnInitComplete: function (oSettings, json) {
                //$("#PDFSPANID").html(json.PDFDownloadBtn);
                $("#SummaryBtnSpanID").html(json.ExcelDownloadBtn);
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

    );


    $('#mdl-Reg-dump').on('hidden.bs.modal', function (e) {
        $("#spn-mdl-Reg-dump-Text").html('');

    });
    $("#ddRegArticleList").multiselect({
        includeSelectAllOption: true,
        onSelectAll: function () {
            alert("select-all-nonreq");
        },
        maxHeight: 400,
        nonSelectedText: 'Choose...',
        onDeselectAll: function () {
            alert("deselect-all-nonreq");
        }
    });

    $('.multiselect').addClass('minimal');
    $('.multiselect').addClass('btn-sm');


});
function PopulateDetailsTable(SROCode, SROName) {

    $("#spnDetailTblHeader").html(SROName);

    //$.scrollTo($('#tableValRptDetailsTableID'), 500);
    PropertyTypeListID = $("#PropertyTypeListID option:selected").val();
    
    switch (PropertyTypeListID) {

        case "1": //Open Built Rate
             

            GetValuationDiffForOpenBuiltRate(PropertyTypeListID, SROName, SROCode);
            break;
        case "2"://Agriculture
            GetValuationDiffForAgriculture(PropertyTypeListID, SROName, SROCode);  // change here
            break;

        case "3"://Apartment
            GetValuationDiffForApartment(PropertyTypeListID, SROName, SROCode); //change here
            break;
        case "4"://Apartment
            GetValuationDiffForApartment(PropertyTypeListID, SROName, SROCode); //change here
            break;
        default:
            break;



    }

}
function EXCELDownloadFun(SROCode, PropertyTypeListID, PropertyType, SROName, RegArticleIdArr)
{
    //alert("Report : "+RegArticleIdArr);
    window.location.href = '/LogAnalysis/ValuationDifferenceReport/ExportValuationDiffRptToExcel?SROCode=' + SROCode + "&PropertyTypeID=" + PropertyTypeListID + "&PropertyType=" + PropertyType + "&SROName=" + SROName + "&RegArticleIdArr=" + RegArticleIdArr;

}
function GetValuationDocumentPopup(EncryptedId) {

    $.ajax
        ({
            type: "GET",
            url: '/LogAnalysis/ValuationDifferenceReport/GetValuationDocumentPopup',
            data: { 'EncryptedId': EncryptedId },
            success: function (data) {

                $("#divViewDocument").html(data);
                $("#divViewDocumentModal").modal('show');
                $.unblockUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                bootbox.alert("error");
                $.unblockUI();
            }
        });

}
function ShowBuildingMeasrDetails(id, FRegNum) {
    $("#spn-mdl-Reg-dump-title").html("Registration Details for Registration number : <i>" + FRegNum + " </i>");

    $("#spn-mdl-Reg-dump-Text").html($("#spnTblBldng_" + id).html());
    $('#mdl-Reg-dump').modal('show');
}

function GetValuationDiffForOpenBuiltRate(PropertyTypeListID, SROName, SROCode) {

    $("#spnDetailTblHeader").html(SROName);

    PropertyTypeName = $("#PropertyTypeListID option:selected").text();
    var RegArticleIdArr = $("#ddRegArticleList").val().toString();

    //$("#dv-tblOpenBuiltRate,#tblOpenBuiltRate").css('display', 'block');
    $("#tblOpenBuiltRate").css('display', 'block');
    $("#tblAgriculture").css('display','none');
    $("#tblApartment").css('display', 'none');
    $('#EXCELSPANID').show();

    //alert(PropertyTypeListID);
    $("#divDetailsValReport").css('display', 'block');
    var tableValuationRptDetailsTable = $('#tblOpenBuiltRate').DataTable({
        ajax: {
            url: '/LogAnalysis/ValuationDifferenceReport/GetValuationDiffDetailedData',
            type: "POST",
            headers: header,
            data: {
                'PropertyTypeListID': PropertyTypeListID, 'SROCode': SROCode, 'PropertyTypeName': PropertyTypeName,
                'RegArticleIdArr': RegArticleIdArr,
                'SROName': SROName
            },
            dataSrc: function (json) {
                unBlockUI();
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
                                
                                //$("#PDFSPANID").html('');
                                $("#EXCELSPANID").html('');
                                $('#tableValuationRptDetailsTable').html('');
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
                unBlockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#tblOpenBuiltRate_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;

                    if (!regexToMatch.test(searchString)) {
                        $("#tblOpenBuiltRate_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableValuationRptDetailsTable.search('').draw();
                            $("#tblOpenBuiltRate_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }
                }
            }
        },
        //serverSide: true,
        //// pageLength: 100,
        //"scrollX": true,
        ////"scrollX": "200px",
        //"scrollY": "350px",//sb
        ////"scrollY": "30vh",
        
        ////sScrollXInner: "50%",
        //scrollCollapse: true,
        //bPaginate: true,
        //bLengthChange: true,
        //// "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        //// "pageLength": -1,
        //bInfo: true,
        //info: true,
        //bFilter: false,
        //searching: true,
        //"destroy": true,
        //"bAutoWidth": true, // changed//sb
        ////"bAutoWidth": false,
        ////shubham 7-3-2020
        //"bScrollAutoCss": true, //commented
        // //shubham 7-3-2020
        ////autoWidth:true,

        serverSide: true,
        "scrollX": true,
        "scrollY": "250px",
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        //dom: 'lBfrtip',
        "destroy": true,
        columnDefs: [
            { orderable: false, targets: [0] },
            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] },
            { orderable: false, targets: [3] },
            { orderable: false, targets: [4] },
            { orderable: false, targets: [5] },
            { orderable: false, targets: [6] },
            { orderable: false, targets: [7] },
            { orderable: false, targets: [8] },
            { orderable: false, targets: [9] },
            { orderable: false, targets: [10] },
            { orderable: false, targets: [11] },
            { orderable: false, targets: [12] },
            { orderable: false, targets: [13] }

            //,{ orderable: false, targets: [11] },

        ],
        columns: [
            { data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
            { data: "NatureOfDocument", "searchable": true, "visible": true, "name": "NatureOfDocument" },
            { data: "AreaName", "searchable": true, "visible": true, "name": "AreaName" },
            { data: "GuidancePerSquareFeetRate", "searchable": true, "visible": true, "name": "GuidancePerSquareFeetRate" },
            { data: "Measurement__Square_Feet_", "searchable": true, "visible": true, "name": "Measurement__Square_Feet_" },
            { data: "Registered_Per_Square_Feet_Rate", "searchable": true, "visible": true, "name": "Registered_Per_Square_Feet_Rate" },
            { data: "Registration_dump", "searchable": true, "visible": true, "name": "Registration_dump" },
            //{ data: "Measurement", "searchable": true, "visible": true, "name": "Measurement" },
            //{ data: "RegisteredPerSquareFeetRate", "searchable": true, "visible": true, "name": "RegisteredPerSquareFeetRate" },
            { data: "Consideration", "searchable": true, "visible": true, "name": "Consideration" },
            { data: "RegisteredGuidanceValue", "searchable": true, "visible": true, "name": "RegisteredGuidanceValue" },

            { data: "PayableStampDuty", "searchable": true, "visible": true, "name": "PayableStampDuty" },
            { data: "payableRegFee", "searchable": true, "visible": true, "name": "payableRegFee" },

            { data: "PaidStampDuty", "searchable": true, "visible": true, "name": "PaidStampDuty" },
            { data: "RegFeePaid", "searchable": true, "visible": true, "name": "RegFeePaid" },

            { data: "StampDutyDifference", "searchable": true, "visible": true, "name": "StampDutyDifference" },
            { data: "RegFeeDifference", "searchable": true, "visible": true, "name": "RegFeeDifference" },

            { data: "TotalDifference", "searchable": true, "visible": true, "name": "TotalDifference" },
            //{ data: "Result", "searchable": true, "visible": true, "name": "Result" },
            { data: "ClickToViewDocument", "searchable": true, "visible": true, "name": "ClickToViewDocument" }


        ],
        //"aoColumns": [
        //    { "sWidth": "400px" }, // 1st column width 
        //    { "sWidth": "50px" }, // 2nd column width 
        //    { "sWidth": "60px" } // 3rd column width and so on 
        //],
        fnInitComplete: function (oSettings, json) {
            //$("#PDFSPANID").html(json.PDFDownloadBtn);
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

    //tableValuationRptDetailsTable.columns.adjust().draw();    // Adjust Table Header but calling two times on first time it need to be changed 1-11-2019 
    var table = $('#tblOpenBuiltRate').DataTable();
    table.columns.adjust().draw();
   
}

function GetValuationDiffForAgriculture(PropertyTypeListID, SROName, SROCode) {
    $("#spnDetailTblHeader").html(SROName);

    //$.scrollTo($('#tableValRptDetailsTableID'), 500);
    PropertyTypeListID = $("#PropertyTypeListID option:selected").val();
    PropertyTypeName = $("#PropertyTypeListID option:selected").text();
    var RegArticleIdArr = $("#ddRegArticleList").val().toString();

  

    $("#tblOpenBuiltRate").css('display', 'none');
    $("#tblAgriculture").css('display', 'block');
    $("#tblApartment").css('display', 'none');
    $('#EXCELSPANID').show();

    //alert(PropertyTypeListID);
    $("#divDetailsValReport").css('display', 'block');
    var tableValuationRptDetailsTable = $('#tblAgriculture').DataTable({
        ajax: {
            url: '/LogAnalysis/ValuationDifferenceReport/GetValuationDiffDetailedData',
            type: "POST",
            headers: header,
            data: {
                'PropertyTypeListID': PropertyTypeListID, 'SROCode': SROCode, 'PropertyTypeName': PropertyTypeName,
                'RegArticleIdArr': RegArticleIdArr,
                'SROName': SROName
            },
            dataSrc: function (json) {
                unBlockUI();
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
                                //$("#PDFSPANID").html('');
                                $("#EXCELSPANID").html('');
                                $('#tableValuationRptDetailsTable').html('');
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
                unBlockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#tblAgriculture_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;

                    if (!regexToMatch.test(searchString)) {
                        $("#tblAgriculture_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableValuationRptDetailsTable.search('').draw();
                            $("#tblAgriculture_filter input").prop("disabled", false);
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
        //sScrollXInner: "50%",
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        // "pageLength": -1,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bScrollAutoCss": true,
        columnDefs: [
            { orderable: false, targets: [0] },
            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] },
            { orderable: false, targets: [3] },
            { orderable: false, targets: [4] },
            { orderable: false, targets: [5] },
            { orderable: false, targets: [6] },
            { orderable: false, targets: [7] },
            { orderable: false, targets: [8] },
            { orderable: false, targets: [9] },
            { orderable: false, targets: [10] },
            { orderable: false, targets: [11] },
            { orderable: false, targets: [12] },
            { orderable: false, targets: [13] }


            //,{ orderable: false, targets: [11] },

        ],
        columns: [
            { data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
            { data: "NatureOfDocument", "searchable": true, "visible": true, "name": "NatureOfDocument" },
            { data: "AreaName", "searchable": true, "visible": true, "name": "AreaName" },
            { data: "GuidancePerSquareFeetRate", "searchable": true, "visible": true, "name": "GuidancePerSquareFeetRate" },


            { data: "Measurement__Guntas", "searchable": true, "visible": true, "name": "Measurement__Guntas" },
            { data: "Registered_Per_Gunta_Rate", "searchable": true, "visible": true, "name": "Registered_Per_Gunta_Rate" },
            //{ data: "Measurement", "searchable": true, "visible": true, "name": "Measurement" },
            //{ data: "RegisteredPerSquareFeetRate", "searchable": true, "visible": true, "name": "RegisteredPerSquareFeetRate" },
            { data: "Consideration", "searchable": true, "visible": true, "name": "Consideration" },
            { data: "PaidStampDuty", "searchable": true, "visible": true, "name": "PaidStampDuty" },
            { data: "RegFeePaid", "searchable": true, "visible": true, "name": "RegFeePaid" },
            { data: "RegisteredGuidanceValue", "searchable": true, "visible": true, "name": "RegisteredGuidanceValue" },
            { data: "PayableStampDuty", "searchable": true, "visible": true, "name": "PayableStampDuty" },
            { data: "StampDutyDifference", "searchable": true, "visible": true, "name": "StampDutyDifference" },
            { data: "payableRegFee", "searchable": true, "visible": true, "name": "payableRegFee" },
            { data: "RegFeeDifference", "searchable": true, "visible": true, "name": "RegFeeDifference" },
            { data: "TotalDifference", "searchable": true, "visible": true, "name": "TotalDifference" },
            //{ data: "Result", "searchable": true, "visible": true, "name": "Result" },
            { data: "ClickToViewDocument", "searchable": true, "visible": true, "name": "ClickToViewDocument" }


        ],
        fnInitComplete: function (oSettings, json) {
            //$("#PDFSPANID").html(json.PDFDownloadBtn);
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

function GetValuationDiffForApartment(PropertyTypeListID, SROName, SROCode) {
    $("#spnDetailTblHeader").html(SROName);

    //$.scrollTo($('#tblApartment'), 500);
    PropertyTypeListID = $("#PropertyTypeListID option:selected").val();
    PropertyTypeName = $("#PropertyTypeListID option:selected").text();
    var RegArticleIdArr = $("#ddRegArticleList").val().toString();

    $("#tblOpenBuiltRate").css('display', 'none');
    $("#tblAgriculture").css('display', 'none');
    $("#tblApartment").css('display', 'block');
    $('#EXCELSPANID').show();

    //alert(PropertyTypeListID);
    $("#divDetailsValReport").css('display', 'block');
    var tableValuationRptDetailsTable = $('#tblApartment').DataTable({
        ajax: {
            url: '/LogAnalysis/ValuationDifferenceReport/GetValuationDiffDetailedData',
            type: "POST",
            headers: header,
            data: {
                'PropertyTypeListID': PropertyTypeListID, 'SROCode': SROCode, 'PropertyTypeName': PropertyTypeName,
                'RegArticleIdArr': RegArticleIdArr,
                'SROName': SROName
            },
            dataSrc: function (json) {
                unBlockUI();
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
                                //$("#PDFSPANID").html('');
                                $("#EXCELSPANID").html('');
                                $('#tableValuationRptDetailsTable').html('');
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
                unBlockUI();
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#tblApartment_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;

                    if (!regexToMatch.test(searchString)) {
                        $("#tblApartment_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableValuationRptDetailsTable.search('').draw();
                            $("#tblApartment_filter input").prop("disabled", false);
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
        //shubham 7-3-2020

        "scrollY": "300px",
        //"scrollY": "30vh",
        //shubham 7-3-2020

        //sScrollXInner: "50%",
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        // "pageLength": -1,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        "destroy": true,
        //shubham 7-3-2020
        "bAutoWidth": true, // changed
        //"bAutoWidth": false,
        //autoWidth: true,// added
        //shubham 7-3-2020

         //shubham 7-3-2020
         "bScrollAutoCss": true, //commented
         //shubham 7-3-2020


        columnDefs: [
            { orderable: false, targets: [0] },
            { orderable: false, targets: [1] },
            { orderable: false, targets: [2] },
            { orderable: false, targets: [3] },
            { orderable: false, targets: [4] },
            { orderable: false, targets: [5] },
            { orderable: false, targets: [6] },
            { orderable: false, targets: [7] },
            { orderable: false, targets: [8] },
            { orderable: false, targets: [9] },
            { orderable: false, targets: [10] },
            { orderable: false, targets: [11] },
            { orderable: false, targets: [12] },
            { orderable: false, targets: [13] }


            //,{ orderable: false, targets: [11] },

        ],
        columns: [
            { data: "RegistrationDate", "searchable": true, "visible": true, "name": "RegistrationDate" },
            { data: "FinalRegistrationNumber", "searchable": true, "visible": true, "name": "FinalRegistrationNumber" },
            { data: "NatureOfDocument", "searchable": true, "visible": true, "name": "NatureOfDocument" },
            { data: "Consideration", "searchable": true, "visible": true, "name": "Consideration" },

            { data: "AreaName", "searchable": true, "visible": true, "name": "AreaName" },

            { data: "Apartment_Name", "searchable": true, "visible": true, "name": "Apartment_Name" },


            { data: "Super_Builtup_Area_shown_in_Document", "searchable": true, "visible": true, "name": "Super_Builtup_Area_shown_in_Document" },


            { data: "Rate_as_per_G_V_notification_01_01_2019", "searchable": true, "visible": true, "name": "Rate_as_per_G_V_notification_01_01_2019" },
            { data: "Total_Value_on_Super_Builtup_Area", "searchable": true, "visible": true, "name": "Total_Value_on_Super_Builtup_Area" },

            { data: "PayableStampDuty", "searchable": true, "visible": true, "name": "PayableStampDuty" },

            { data: "payableRegFee", "searchable": true, "visible": true, "name": "payableRegFee" },


            { data: "TotalPayable", "searchable": true, "visible": true, "name": "TotalPayable" },
            { data: "Market_Value_calculated_as_per_document_at_the_time_of_Registration", "searchable": true, "visible": true, "name": "Market_Value_calculated_as_per_document_at_the_time_of_Registration" },
            { data: "PaidStampDuty", "searchable": true, "visible": true, "name": "PaidStampDuty" },

            { data: "RegFeePaid", "searchable": true, "visible": true, "name": "RegFeePaid" },


            { data: "TotalPaid", "searchable": true, "visible": true, "name": "TotalPaid" },
            { data: "Difference_between_the_Two", "searchable": true, "visible": true, "name": "Difference_between_the_Two" },
            //{ data: "Result", "searchable": true, "visible": true, "name": "Result" },
            { data: "ClickToViewDocument", "searchable": true, "visible": true, "name": "ClickToViewDocument" }


        ],
        fnInitComplete: function (oSettings, json) {
            //$("#PDFSPANID").html(json.PDFDownloadBtn);
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

    //$(window).on('resize', function () {
    //    table.fnAdjustColumnSizing();
    //});
}

function DownloadEXCELSummary(RegArticleIdArr, PropertyTypeListID) {
    //alert("Report : "+RegArticleIdArr);
    window.location.href = '/LogAnalysis/ValuationDifferenceReport/ExportValuationDiffSummaryToExcel?RegArticleIdArr=' + RegArticleIdArr + "&PropertyTypeListID=" + PropertyTypeListID + "&PropertyTypeName=" + PropertyTypeName ;


    //$.ajax(
    //    {

    //        url: "/LogAnalysis/ValuationDifferenceReport/ExportValuationDiffSummaryToExcel", // Controller/View
    //        type: "POST", //HTTP POST Method
    //        headers: header,
    //        data: {
    //            'PropertyTypeListID': PropertyTypeListID, 'PropertyTypeName': PropertyTypeName,
    //            'RegArticleIdArr': RegArticleIdArr,
    //        },
    //        success: function (data) {

    //            if (data.success) {

    //            }
    //            else {
    //            }
    //        },
    //        error: function (err) {
    //            alert("Error in Processing" + err);
    //        }

    //    });






}


