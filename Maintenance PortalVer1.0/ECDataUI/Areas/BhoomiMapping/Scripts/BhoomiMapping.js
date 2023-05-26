//const { Alert } = require("bootstrap");

var token = '';
var header = {};

$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;


    $("#DownloadExcel").click(function (e) {
        var loc = window.location.pathname;
        e.preventDefault();
        window.location.href = "/content/BhoomiMappingSampleExcel/SampleBhoomiMappingInputFile.xlsx";
    });
});


function Upload() {
    blockUI("Loading");

    var SRO = $('#SROOfficeOrderListID').val();
    if (SRO == "") {
        bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Please select SRO.</span>');
        unBlockUI();
        return false;
    }

    var ext = $('#excel_input').val().split('.').pop().toLowerCase();
    if (ext !== 'xls' && ext !== 'xlsx') {
        bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Please select excel file.</span>');
        unBlockUI();
        return false;
    }

    var fileUpload = $("#excel_input").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();

    // can be used in case of multiple file uploads
    //for (var i = 0; i < files.length; i++) {
    //    fileData.append(files[i].name, files[i]);
    //}


    fileData.append("ExcelFile", files[0]);
    fileData.append('SROCode', SRO);
    $.ajax({
        url: '/BhoomiMapping/BhoomiMapping/Upload',
        contentType: false, 
        processData: false, 
        data: fileData,  
        type: "POST",
        headers: header,
        success: function (data) {
            unBlockUI();
            if (data.includes("Row_Exceed_Error")) {
                var r = data.split(".")[0];
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">There are more than ' + r + ' rows in the excel sheet, Please check.</span>');
            }
            else if (data == "Col_Mismatch_error") {
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Column Header Name Mismatch, Please Check Excel File.</span>');
            }
            else if (data == "SRO_error") {
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Selected SRO doesn\'t match SRO in Excel File.</span>');
            }
            else if (data == "Empty_Error_Excel") {
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Selected excel file is empty.</span>');
            }
            else if (data == "Null_Value_Error") {
                bootbox.alert('<i class="fa fa-exclamation-triangle boot-icon boot-icon" style="color:#dd0000;"></i><span class="boot-alert-txt">Selected excel file has some empty/null value. Please Check.</span>');
            }
            else {
                $("#StatusDiv").html(data);
            }
        },
        error: function (xhr) {
            unBlockUI();
            alert(xhr);
        }
    });

    

}



