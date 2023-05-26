

var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;




    //to be added on js page for all view pages inside modules where otp is required
    $(document).bind("contextmenu", function (e) {
        e.preventDefault();
    });
    $(document).keydown(function (e) {
        if (e.which === 123) {
            return false;
        }
    });

    if (sessionStorage.getItem('IsValidated') == 1) {
        sessionStorage.setItem('IsValidated', '2');
    }
    else if (performance.navigation.type == performance.navigation.TYPE_RELOAD || sessionStorage.getItem('IsValidated') == 2) {

    }
    //else {
    //    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">OTP Validation is Required.</span>',
    //        function () {
    //            sessionStorage.setItem('IsValidated', '0');
    //            window.location.href = "/Home/HomePage";
    //        });
    //}


    $('#btnReset').click(function () {
        //$('#txtOrderDate').datepicker('setDate', null);
        //$('#SROfficeListID').prop('selectedIndex', 0);
        //$('#FinancialYearListID').prop('selectedIndex', 0);
        //$('#BookTypeListID').prop('selectedIndex', 0);
        //$("#txtDocumentNumber").val(0);
        //$("#NewPageCount").val('');
        //$("#oldPageCount").val('');
        //$("#txtOrderNumber").val('');
        //$('#DROrderDetailsList').show();
        //$('#UploadOrderDetails').hide();


        location.reload();


    });


    $('input[type=radio][name=DType]').change(function () {

        var cv = $('input[type=radio][name=DType]:checked').val();
        if (cv == 'DOC') {
            document.getElementById('DynamicLabel').innerHTML = "Document Number";
            $("#txtDocumentNumber").val(0);
            $("#BDIV").css('display', 'block');
            $("#FDIV").css('display', 'block');
        }
        else if (cv == 'MARR') {
            document.getElementById('DynamicLabel').innerHTML = "Marriage Number";
            $("#txtDocumentNumber").val(0);
            $("#FinancialYearListID").find('option:eq(0)').prop('selected', true);
            $("#BookTypeListID").find('option:eq(0)').prop('selected', true);
            $("#BDIV").css('display', 'none');
            $("#FDIV").css('display', 'none');
        }
    });

    $('input[type=radio][name=NewPC]').change(function () {

        var cv = $('input[type=radio][name=NewPC]:checked').val();
        if (cv == '0') {

            $("#NewPageCount").attr('disabled', 'disabled');
            $("#NewPageCount").val('');
        }
        else if (cv == '1') {
            $("#NewPageCount").removeAttr('disabled');
            $("#NewPageCount").val('');
        }
    });


    $("#NewPageCount").keyup(function () {

        if ($("#NewPageCount").val() > 999) {

            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Max Value for Page Count is 999.</span>',
                function () {
                    $("#NewPageCount").val('');
                });
        }

    });


    $('#txtOrderDate').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        endDate: '+0d',
        startDate: new Date('01/01/2003'),
        maxDate: '0',
        minDate: new Date('01/01/2003'),
        pickerPosition: "bottom-left"//,
    });

    DocDetailsTable();




    $('#DROfficeListID').change(function () {
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/DataEntryCorrection/ReScanningApplication/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROfficeListID').val() },
            type: "GET",
            success: function (data) {
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    $('#SROfficeListID').empty();
                    $.each(data.SROfficeList, function (i, SROfficeList) {
                        SROfficeList
                        $('#SROfficeListID').append('<option value="' + SROfficeList.Value + '">' + SROfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });




    $("#btncloseAbortPopup").click(function () {
        $('#ErrorDetailsModal').modal('hide');
    });




    $("#SROfficeListID").on("change", function () {
        PageCount();
    });


    $("#txtDocumentNumber").focusout(function () {
        PageCount();
    });


    $("#BookTypeListID").on("change", function () {
        PageCount();
    });


    $("#FinancialYearListID").on("change", function () {
        PageCount();
    });

});





function PageCount() {
    var SROfficeID = $("#SROfficeListID option:selected").val();
    var DocNo = $("#txtDocumentNumber").val();
    var Type = $('input[type=radio][name=DType]:checked').val();
    var BType = $('#BookTypeListID').val();

    if (Type == "DOC") {
        var BType = $('#BookTypeListID').val();
        var FYear = $('#FinancialYearListID').val();
        if (SROfficeID != 0 && DocNo != "" && DocNo != 0 && FYear != 0 && BType != 0) {

            $.ajax({
                type: "GET",
                url: '/DataEntryCorrection/ReScanningApplication/PCount?SROCode=' + SROfficeID + '&DocNo=' + DocNo + '&Fyear=' + FYear + '&BType=' + BType,
                headers: header,
                processData: false,
                contentType: false,
                success: function (data) {
                    unBlockUI();
                    $('#oldPageCount').val(data);
                },
                error: function (xhr) {
                    unBlockUI();
                }

            });


        }


    }
    else {
        var BType = "0";
        var FYear = "0";
        if (SROfficeID != 0 && DocNo != "" && DocNo != 0) {


            $.ajax({
                type: "GET",
                url: '/DataEntryCorrection/ReScanningApplication/PCount?SROCode=' + SROfficeID + '&DocNo=' + DocNo + '&Fyear=' + FYear + '&BType=' + BType,
                headers: header,
                processData: false,
                contentType: false,
                success: function (data) {
                    unBlockUI();
                    $('#oldPageCount').val(data);

                },
                error: function (xhr) {
                    unBlockUI();
                }

            });



        }
    }





}


function DocDetailsTable() {
    //$('#RSADtlsTable').DataTable().clear();
    var DetailsTable = $('#RSADtlsTable').DataTable({
        ajax: {
            url: '/DataEntryCorrection/ReScanningApplication/LoadDocDetailsTable',
            type: "POST",
            headers: header,
            data: {
                'DroCode': $("#DROfficeListID option:selected").val()
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
                                $("#DetailTableID").DataTable().clear().destroy();
                            }
                        }
                    });
                    return;
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchParaDetail').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsToggleIconSearchParaDetail').trigger('click');
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
            //{ "className": "dt-center ShorterTextClass", "targets": [4] },
            { "className": "dt-center", "targets": "_all" },
            { "orderable": false, "targets": "_all" },
            { "bSortable": false, "aTargets": "_all" },
            //{ "width": "200px", "targets": [6] },
            //{ orderable: false, targets: [1] },
            //{ orderable: false, targets: [2] },
            //{ orderable: false, targets: [3] },
            //{ orderable: false, targets: [4] }
        ],

        columns: [
            { data: "SNo", "searchable": true, "visible": true, "name": "SNo" },
            { data: "DROName", "searchable": true, "visible": true, "name": "DROName" },
            { data: "SROName", "searchable": true, "visible": true, "name": "SROName" },
            { data: "EnteredBY", "searchable": true, "visible": true, "name": "EnteredBY" },
            { data: "EnteryDate", "searchable": true, "visible": true, "name": "EnteryDate" },
            { data: "DROrderNumber", "searchable": true, "visible": true, "name": "DROrderNumber" },
            { data: "OrderDate", "searchable": true, "visible": true, "name": "OrderDate" },
            { data: "RegistrationNumber", "searchable": true, "visible": true, "name": "RegistrationNumber" },
            { data: "isActive", "searchable": false, "visible": true, "name": "isActive" }, 
            { data: "isRescanCompleted", "searchable": false, "visible": true, "name": "isRescanCompleted" }, 
            { data: "ViewBtn", "searchable": true, "visible": true, "name": "ViewBtn" },
            { data: "DocTypeID", "searchable": false, "visible": false, "name": "DocTypeID" }, 
        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
            //if (json.isDrLogin) {
            //    $("#AddOrderSPANID").show();
            //}
            //else {
            //    $("#AddOrderSPANID").hide();
            //}

        },
        preDrawCallback: function () {
            unBlockUI();
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {

            //if (ModuleID == 1) {
            //    fnSetColumnVis(2, false);
            //}
            //else if (ModuleID == 2) {
            //    fnSetColumnVis(0, false);
            //    fnSetColumnVis(1, false);
            //}

            unBlockUI();
            return nRow;
        },
        drawCallback: function (oSettings) {
            unBlockUI();
        },
    });
    //Commented by mayank on 17/09/2021 uncommneted after sp execution
    //DetailsTable.columns([1]).visible(false);
    //DetailsTable.columns([3]).visible(false);
}



function ViewBtnClickOrderTable(path) {

    blockUI('loading data.. please wait...');
    $.ajax({
        type: "GET",
        url: "/DataEntryCorrection/ReScanningApplication/ViewBtnClickOrderTable?path=" + path, 
        data: null, 
        success: function (result) {
            // do something with result

            if (result.includes("Could not find file")) {

                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-check">  ' + result + '</span>',
                    function() {
                    }
                });

            }
            else {
                window.location.href = "/DataEntryCorrection/ReScanningApplication/Download?path=" + path;
            }

        },
        error: function (req, status, error) {
            // do something with error   
        }
    });

    //window.location.href = "/DataEntryCorrection/ReScanningApplication/ViewBtnClickOrderTable?path=" + path;
   


    unBlockUI();
}






function upFunct(DRO, SRO, DocNo, OrderNo, OrderDT, DType, DocID, Tri, FRN, NPC, isMissingDocument) {

    var formData = new FormData();
    formData.append('File', $('input[type=file][name=OrderFile]')[0].files[0]);
    formData.append('SRO', SRO);
    formData.append('DRO', DRO);
    formData.append('DocNo', DocNo);
    formData.append('OrderNo', OrderNo);
    formData.append('OrderDT', OrderDT);
    formData.append('DType', DType);
    formData.append('DocID', DocID);
    formData.append('Tri', Tri);
    formData.append('FRN', FRN);
    formData.append('NPC', NPC);
    formData.append('isMissingDocument', isMissingDocument);

    $.ajax({
        type: "POST",
        url: "/DataEntryCorrection/ReScanningApplication/Upload",
        data: formData,
        headers: header,
        processData: false,
        contentType: false,
        success: function (data) {
            unBlockUI();
            if (!data.serverError && data.status == "Success") {
                $('#RSADtlsTable').DataTable().draw();
                bootbox.alert({
                    message: '<i class="fa fa-check text-danger boot-icon boot-icon"></i><span class="boot-check">' + data.Message + '</span>',
                    function() {
                    }
                });
                

            }
            else if (!data.serverError) {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>'
                });
            }
            else {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                    function() {
                        window.location.href = "/Home/HomePage"
                    }
                });

            }
        },
        error: function (xhr) {
            unBlockUI();
        }

    });
}




function TrigDP() {
    $("#txtOrderDate").focus();
}

function Show() {
    document.getElementById("OrderFile").value = "";
    var fileDownload = false;
    var SROfficeID = $("#SROfficeListID option:selected").val();
    var DROfficeID = $("#DROfficeListID option:selected").val();
    var OrderNo = $("#txtOrderNumber").val();
    var OrderDate = $("#txtOrderDate").val();
    var DocNo = $("#txtDocumentNumber").val();
    var Type = $('input[type=radio][name=DType]:checked').val();
    var PageCountUpdationFlag = $('input[type=radio][name=NewPC]:checked').val();
    var NPC = 0;
    if (PageCountUpdationFlag == 1) {
        NPC = $('#NewPageCount').val();
    }
    else {
        NPC = -1;
    }
    if (Type == "DOC") {
        var BType = $('#BookTypeListID').val();
        var FYear = $('#FinancialYearListID').val();
    }
    else {
        var BType = "0";
        var FYear = "0";
    }
    //Validation
    if (DROfficeID == 0) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid DRO Details</span>'
        });
        return;
    }


    if (SROfficeID == 0) {
        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select valid SRO Details</span>'
        });
        return;
    }

    if (NPC == 0 || NPC == "" || isNaN(NPC)) {

        bootbox.alert({
            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter valid page count.</span>'
        });
        return;

    }
    blockUI('loading data.. please wait...');
    //$('#PDFModal').css('display', 'block');
    //$('#PDFModal').addClass('modal fade in');
    //$("#objPDFViewer").attr('data', '');
    //$("#objPDFViewer").attr('data', '/DataEntryCorrection/ReScanningApplication/btnShowClick?DRO=' + DROfficeID + '&SRO=' + SROfficeID + '&OrderNo=' + OrderNo + '&Date=' + OrderDate + '&DocNo=' + DocNo + '&DType=' + Type);
    //unBlockUI();

    $.ajax({
        url: '/DataEntryCorrection/ReScanningApplication/btnShowClick',
        data: {
            "DRO": DROfficeID, "SRO": SROfficeID, "OrderNo": OrderNo, "Date": OrderDate, "DocNo": DocNo, "DType": Type, "BType": BType, "FYear": FYear, "NPC": NPC
        },
        datatype: "json",
        type: "GET",
        success: function (data) {
            if (data.serverError == true) {
                bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                    function () {
                        //window.location.href = "/Home/HomePage"
                    });
            }
            else {
                if (data.status == "Found") {
                    $("#uploadBtnWrapper").css("display", "grid");
                    $('#DROrderDetailsList').hide();
                    bootbox.alert('<i class="fa fa-check text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Details are verified. Please proceed to upload the order.</span>',
                        function () {
                            $("#btnUpload").attr("onclick", data.OnClickData);

                        });
                }
                else if (data.status == "NotFoundFile") {
                    $("#uploadBtnWrapper").css("display", "none");

                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                        function () {
                            $("#btnUpload").attr("onclick", "");
                        });

                }
                else if (data.status != "") {
                    $('#RSADtlsTable_filter label input').focus();
                    $('#RSADtlsTable_filter label input').val(data.status);
                    var e = jQuery.Event("keypress");
                    e.which = 13; //enter keycode
                    e.keyCode = 13;
                    $("#RSADtlsTable_filter label input").trigger(e);
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.Message + '</span>',
                        function () {
                            $("#btnUpload").attr("onclick", "");
                        });
                }

                else {
                    $("#uploadBtnWrapper").css("display", "none");

                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Something went wrong. Try Again.</span>',
                        function () {
                            $("#btnUpload").attr("onclick", "");
                        });
                }
            }
            unBlockUI();
        },
        error: function (xhr) {
            unBlockUI();
        }
    });
}


