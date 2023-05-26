//Global variables.
var token = '';
var header = {};
$(document).ready(function () {
    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    $('#RagneID').focus();

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });


    $("#SearchBtn").click(function () {

        var RagneID = $("#RagneID option:selected").val();
        var FinYearListID = $("#FinYearListID option:selected").val();
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/MISReports/HighValueProperties/GetHighValuePropertyDetails/',
            data: { "RagneID": RagneID, "FinYearListID": FinYearListID },
            datatype: "json",
            headers: header,
            type: "POST",
            success: function (data) {

                $('#tableToBeLaded').html(data);
                unBlockUI();
            },
            error: function (xhr) {
               
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                unBlockUI();
            }
        });
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
            $('#DtlsSearchParaListCollapse').trigger('click');

    });
   

});