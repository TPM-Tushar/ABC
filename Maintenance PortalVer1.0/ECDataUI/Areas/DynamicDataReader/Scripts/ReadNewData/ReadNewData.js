
var token = '';
var header = {};


$(document).ready(function () {

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;




    //$(".divMonthWise").hide();

    $("#ExecuteBtn").click(function () {

        DBName = $("#dbList option:selected").val();
        Purpose = $("#txtPurpose").val();
        QueryText = $("#txtQueryText").val();
        //QueryIdTxt = $("#QueryIdTxt").val();


        if (DBName == 0) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please select database.</span>');
            return;
        }
        if (Purpose == "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter purpose.</span>');
            return;
        }
        if (QueryText === "") {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Please enter Query Text.</span>');
            return;
        }
        if (QueryText.toLowerCase().includes("select * from")) {
            bootbox.alert('<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Select * from query are not allowed.</span>');
            return;
        }
        if (DBName !== 0 && Purpose !== "" && QueryText !== "") {
            $('#MasterTableModel').modal('show');
        }


    });

    //$("#confmsg1").click(function () {
    //    $("#btnAlertYes").attr("disabled", !this.checked);
    //});

    var checkBoxes = $('input.compulsory'),
        submitButton = $('#btnAlertYes');

    checkBoxes.change(function () {
        submitButton.attr("disabled", checkBoxes.is(":not(:checked)"));
        if (checkBoxes.is(":not(:checked)")) {
            submitButton.addClass('disabled');
        } else {
            submitButton.removeClass('disabled');
        }
    });


    $("#btnAlertYes").click(function () {

        $.ajax({
            url: '/DynamicDataReader/ReadNewData/GetSSRSReportData',
            headers: header,
            type: "POST",
            data: {
                'DatabaseName': DBName, 'Purpose': Purpose, 'QueryText': QueryText
            },
            success: function (data) {
                if (data.errorMessage != null) {
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + data.errorMessage + '</span>',
                        callback: function () {
                            if (data.serverError != undefined) {
                                window.location.href = "/Home/HomePage"
                            }
                        }
                    });
                }
                //console.log(data.data);
                //$('#modalValue').html(data.data);
                //$("#exampleModalLong").modal('show');
                //$('#SearchResult').html('');
                //$('#SearchResult').html(data); 
                //alert(data);

                $('#QueryDataView').html('');
                $('#QueryDataView').html(data);
                $('#MasterTableModel').modal('hide');
                unBlockUI();

            },
            error: function (xhr) {
                alert(xhr);
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });
                unBlockUI();
            },
            beforeSend: function () {
                blockUI('loading data.. please wait...');

            }
        });

    });

});