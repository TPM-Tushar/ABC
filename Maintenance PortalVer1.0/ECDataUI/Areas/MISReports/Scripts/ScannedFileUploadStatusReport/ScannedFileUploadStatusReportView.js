
var token = '';
var header = {};
var SROOfficeListID;
var DROOfficeListID;
var DistrictText;
var SROText;
var OfficeType;
//Added by Madhusoodan on 30-04-2020
var DocTypeID;
var DocTypeText;

$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    $('#DocTypeDropDownListID').hide();   //Added by Madhusoodan on 28-08-2020
    $('input[type=radio][name=RDOBtnFilter]').change(function () {
        $('#EXCELSPANID').html('');

        if ($.fn.DataTable.isDataTable("#AnywhereECTable")) {
            $("#AnywhereECTable").DataTable().clear().destroy();
            $("#AnywhereECTable").hide();
        }
        if ($.fn.DataTable.isDataTable("#DocScannedAndDeliveryTableID")) {
            $("#DocScannedAndDeliveryTableID").DataTable().clear().destroy();
            $("#DocScannedAndDeliveryTableID").hide();
        }
        SelectedType = $('input[name="RDOBtnFilter"]:checked').val();

        if (SelectedType == "SR") {

            $('#ID_DR_Lbl').html("District");
            $('#DocTypeDropDownListID').show();     //Added by Madhusoodan on 28-08-2020

            $('#DRODropDownListID').removeClass("col-md-12").addClass("col-md-4");
            $('#ID_DR_Lbl').css('padding-left','0%');
            $('#LISTDRO').css('margin-left', '0%');
            $('#LBLDRO').css('margin-top', '1.5%');

            if ($('#LBLDRO').hasClass("col-md-8"))
                $('#LBLDRO').removeClass("col-md-8").addClass("col-md-4");
            if ($('#LISTDRO').hasClass("col-md-3"))
                $('#LISTDRO').removeClass("col-md-3").addClass("col-md-8");

            if (IsDrLogin == "True") {
                $('#SRODropDownListID').show();
                $('#DRODropDownListID').show();
               
                if ($('#FirstDivId').hasClass("col-md-4") == true) {
                    $('#FirstDivId').removeClass("col-md-4").addClass("col-md-2");
                }
                if ($('#FourthDivId').hasClass("col-md-4") == true) {
                    $('#FourthDivId').removeClass("col-md-4").addClass("col-md-2");
                }
            }
            else if (IsSrLogin == "True") {
                $('#RdoBtnId').hide();
                $('#SRODropDownListID').show();
                $('#DRODropDownListID').hide();
                if ($('#FirstDivId').hasClass("col-md-2") == true) {
                    $('#FirstDivId').removeClass("col-md-2").addClass("col-md-4");
                }
                if ($('#FourthDivId').hasClass("col-md-2") == true) {
                    $('#FourthDivId').removeClass("col-md-2").addClass("col-md-4");
                }
            }
            else {

                $('#SRODropDownListID').show();
                $('#DRODropDownListID').show();
                $('')
                if ($('#FirstDivId').hasClass("col-md-4") == true) {
                    $('#FirstDivId').removeClass("col-md-4").addClass("col-md-2");
                }
                if ($('#FourthDivId').hasClass("col-md-4") == true) {
                    $('#FourthDivId').removeClass("col-md-4").addClass("col-md-2");
                }
            }


            //$('#DRODropDownListID').show();
            //$('#SRODropDownListID').show();
        }
        else if (SelectedType == "DR") {
            $('#ID_DR_Lbl').css('padding-left', '29%');
            $('#LISTDRO').css('margin-left', '-18%');
            $('#LBLDRO').css('margin-top', '1%');
            //alert(IsDrLogin);
            $('#ID_DR_Lbl').html("DRO");

            if ($('#LBLDRO').hasClass("col-md-4"))
                $('#LBLDRO').removeClass("col-md-4").addClass("col-md-8");
            if ($('#LISTDRO').hasClass("col-md-8"))
                $('#LISTDRO').removeClass("col-md-8").addClass("col-md-3");

            $('#DocTypeDropDownListID').hide();   //Added by Madhusoodan on 28-08-2020

            //$('#DRODropDownListID').css("col-md-12 text-center");
            //$('#DRODropDownListID').addClass("col-md-12 text-center");
            $('#DRODropDownListID').removeClass("col-md-4").addClass("col-md-12 text-center");
            //$('#DRODropDownListID').css("col-md-4col-md-push-4");
            //$('#DRODropDownListID').removeClass("col-md-4").addClass("col-md-12 col-md-push-4");

            //$('#ID_DR_Lbl').css("col-md-2");

            //ID_DR_Lbl   -- for span
            //DROOfficeListID  -- for dropdown


            if (IsDrLogin == "True") {
                //alert("In DrLogin Dr");
                $('#DRODropDownListID').show();
                $('#SRODropDownListID').hide();

                //Commented by Madhusoodan on 29-04-2020
                //if ($('#FirstDivId').hasClass("col-md-2") == true) {
                //    $('#FirstDivId').removeClass("col-md-2").addClass("col-md-4");
                //}
                //if ($('#FourthDivId').hasClass("col-md-2") == true) {
                //    $('#FourthDivId').removeClass("col-md-2").addClass("col-md-4");
                //}
            }
            else {
                $('#DRODropDownListID').show();
                $('#SRODropDownListID').hide();

                //Commented by Madhusoodan on 29-04-2020
                //alert($('#FirstDivId').hasClass("col-md-2"));
                if ($('#FirstDivId').hasClass("col-md-4") == true) {
                    //alert("In DR Inner");
                    $('#FirstDivId').removeClass("col-md-4").addClass("col-md-4");
                }
                if ($('#FourthDivId').hasClass("col-md-4") == true) {
                    $('#FourthDivId').removeClass("col-md-4").addClass("col-md-4");
                }
            }


            $('#DRRowId').show();
            $('#SRLoginViewId').hide();
            $('#SRRowId').hide();
            //$('#DRODropDownListID').show();
            //$('#SRODropDownListID').show();
        }
    });

    if (IsDrLogin == "True") {
        $('input:radio[name=RDOBtnFilter][value=DR]').trigger('click');
    }
    else if (IsSrLogin == "True") {
        $('input:radio[name=RDOBtnFilter][value=SR]').trigger('click');

    }
    else {
        $('input:radio[name=RDOBtnFilter][value=DR]').trigger('click');
    }




    $('#DROOfficeListID').change(function () {
        blockUI('Loading data please wait.');
        $.ajax({
            url: '/MISReports/ScannedFileUploadStatusReport/GetSROOfficeListByDistrictID',
            data: { "DistrictID": $('#DROOfficeListID').val() },
            datatype: "json",
            type: "GET",
            success: function (data) {
                if (data.serverError == true) {
                    bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        function () {
                            window.location.href = "/Home/HomePage"
                        });
                }
                else {
                    $('#SROOfficeListID').empty();
                    $.each(data.SROOfficeList, function (i, SROOfficeList) {
                        SROOfficeList
                        $('#SROOfficeListID').append('<option value="' + SROOfficeList.Value + '">' + SROOfficeList.Text + '</option>');
                    });
                }
                unBlockUI();
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    });

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    $("#SearchBtn").click(function () {
        SelectedType = $('input[name="RDOBtnFilter"]:checked').val();
       
        document.getElementById("OfficeTypeId").value = SelectedType;
        SROOfficeListID = $("#SROOfficeListID option:selected").val();
        DROOfficeListID = $("#DROOfficeListID option:selected").val();
        DistrictText = $("#DROOfficeListID option:selected").text();

        //Added by Madhusoodan on 29-04-2020
        DocTypeID = $("#DocTypeID option:selected").val();
        //alert(DocTypeID);
        DocTypeText = $("#DocTypeID option:selected").text();
        //alert(DocTypeText);

        //Added by Madhusoodan on 06-05-2020
        //To Change to span tag of Collapsale heading above Datatable on SRO Radio Btn selection
        var selectedRadioBtn = $('input[name="RDOBtnFilter"]:checked').val();
        if (selectedRadioBtn == "SR") {
            $('#InnerDocTypeDivID').show();
            var selectedDocType = $("#DocTypeID option:selected").text();
            $('#InnerDocTypeDivID').html("/ Registration Type: " + selectedDocType);
        }

        if (selectedRadioBtn == "DR")
        {
            $('#InnerDocTypeDivID').html("");
            $('#InnerDocTypeDivID').hide();
           
        }

        SROText = $("#SROOfficeListID option:selected").text();
        if ($('#DROOfficeListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any District.</span>');
        }
        else if ($('#SROOfficeListID').val() < "0") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select Any SRO</span>');
        }
        else {

            blockUI('Loading data please wait.');
            $.ajax({
                type: "POST",
                url: "/MISReports/ScannedFileUploadStatusReport/LoadScannedFileUploadStatusReportTable/",
                cache: false,
                headers: header,
                data: $("#ScannedFileUploadStatusFormID").serialize() + '&OfficeTypeId=' + SelectedType,
                success: function (data) {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');
                    unBlockUI();
                    $('#tableToBeLaded').html(data);
                },
                error: function (xhr, status, err) {
                    //bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i> <span class="boot-alert-txt">'+"Error occured while proccessing your request : " + err+'</span>');
                    //alert("asd");
                    //window.location.href = "/Error/Index";
                    bootbox.alert({
                        //   size: 'small',
                        //title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + "Error occured while proccessing your request : " + err + '</span>',
                        callback: function () {

                        }
                    });

                    unBlockUI();
                    //$.unblockUI();
                }
            });

        }


    });
});



