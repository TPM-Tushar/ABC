var myLineChart;
var StampDutyDataSet;
var RegistrationFeeDataSet;
//Raman 20-03-2020
var IsTabLoaded1 = false;
var IsTabLoaded2 = false;
//ADDED BY PANKAJ ON 13-10-2020
var Tab1Data;
var Tab2Data;

// BELOW CODE COMMENTED AND ADDED IN $(document).ready(function (){}); BY SHUBHAM BHAGAT ON 18-12-2020
//$('#DashboardSummaryID').trigger('click');
 // ABOVE CODE COMMENTED AND ADDED IN $(document).ready(function (){}); CHANGED BY SHUBHAM BHAGAT ON 18-12-2020


function LoadDashboardSummaryView() {
    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-08-2020 AT 3:00 PM
    blockUI('loading data.. please wait...');
    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-08-2020 AT 3:00 PM

    $('#DashboardDetailsDivID').hide();
    $('#DashboardSummaryDivID').show();
    //$('#DashboardDetailsID').removeClass('active');
    //$('#DashboardSummaryDivID').addClass('active');
    //$('#DashboardSummaryID a').addClass('active');
    //$('#DashboardSummaryID a').addClass('active');

    var AllActiveItems = $('#TabListDivID').find('.active');
    if (AllActiveItems.length > 0) {
        AllActiveItems.removeClass('active');
    }
    $('#DashboardSummaryID').addClass('active');

    if (IsTabLoaded1 == false) {
        $.ajax({
            url: '/Dashboard/Dashboard/DashboardSummaryView',
            data: {},
            datatype: "json",
            type: "GET",
            success: function (data) {
                unBlockUI();
                $('#DashboardSummaryDivID').html(data);
                //Techadmin
                Tab1Data = data;
                IsTabLoaded1 = true;
            },
            error: function (xhr) {
                unBlockUI();
            }
        });
    } else {
        $('#DashboardSummaryDivID').html(Tab1Data);
    }

}
function LoadDashboardDetailsView() {

    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-08-2020 AT 3:00 PM
    blockUI('loading data.. please wait...');
    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-08-2020 AT 3:00 PM

    $('#DashboardSummaryDivID').hide();
    $('#DashboardDetailsDivID').show();
    //$('#DashboardSummaryDivID').removeClass('active');
    //$('#DashboardDetailsID').addClass('active');


    // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020
    // BECAUSE FOR SR THERE IS ONLY 1 TAB SO IT IS THROWING JAVASCRIPT ERROR SO WE WILL CHECK CONDITION IT THERE IS SR SO WE WILL NOT DEACTIVE ANOTHER TAB
    //var AllActiveItems = $('#TabListDivID').find('.active');
    //if (AllActiveItems.length > 0) {        
    //    AllActiveItems.removeClass('active');        
    //}
    if (currentRoleIDVAR != srRoleIDVAR) {
        var AllActiveItems = $('#TabListDivID').find('.active');
        //alert(AllActiveItems.length);
        if (AllActiveItems.length > 0) {
            //alert('bef');
            AllActiveItems.removeClass('active');
            //alert('aft');
        }
    }
    // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020


    if (IsTabLoaded2 == false) {


        $.ajax({
            url: '/Dashboard/Dashboard/DashboardDetailsView',
            data: {},
            datatype: "json",
            type: "GET",
            success: function (data) {
                unBlockUI();
                $('#DashboardDetailsDivID').html(data);
                //Techadmin
                Tab2Data = data;
                IsTabLoaded2 = true;
            },
            error: function (xhr) {
                unBlockUI();
            }
        });

    } else {
        $('#DashboardDetailsDivID').html(Tab2Data);
    }
}

$(document).ready(function () {
    //alert('1');
    //alert('main');//sb

    // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020
    //$('#DashboardSummaryID a').click();
    //alert(srRoleIDVAR);
    if (currentRoleIDVAR == srRoleIDVAR) {
        //alert('2');
        $('#DashboardDetailsID2').trigger('click');
        //alert('3');

        //$('#DashboardDetailsID2 a').click();
        //alert('6');
    }
    else {
        //alert('4');

        $('#DashboardSummaryID').trigger('click');
        //alert('5');


        //$('#DashboardSummaryID a').click();
    }
    
 // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020

    //$('#DashboardSummaryID a').click();

    // $('#DashboardSummaryID a').addClass('active');
    //$('#DashboardSummaryID').click();
    //LoadDashboardSummaryView();

    //$('#DashboardSummaryID').addClass('active');

    $('#iconSideBar').trigger('click');




    // LoadDashboardSummaryView();
    //  $('#DashboardSummaryID').trigger('click');
});