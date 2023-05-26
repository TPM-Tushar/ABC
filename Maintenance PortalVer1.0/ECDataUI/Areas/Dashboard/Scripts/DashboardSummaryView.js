var Target_DataSet;
var Achieved_DataSet;
var Document_DataSet;
var Revenue_DataSet;
var DistrictCode_;
var selectedType;
var DistrictText;
var DistrictTxt_;
var DistrictTextForExcel;
//Added by RamanK on 18-06-2020
var FinYear;
//var FinYearVal;
// BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
var IsCurrentFinYear = true;
// ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020

// BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
//var RevenueTargetVsAchievedTargetPopUp;
//var RevenueTargetVsAchievedAchievedPopUp;
var RevenueTargetVsAchievedFinYearsPopUp;
var IsMonthsProgressChartVisible;
var FinYearForProgressMonthsChart;
var CurrentVsPrevFinYrFinYearsPopUp;
var CurrentVsPrevFinYrMonthsPopUp;
var ProgressBarChartForModalPopup;
var RevenueTargetVsAchievedForModalPopup;
// ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020

$(document).ready(function () {



	 // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        

    $('#RevenueTargetVsAchievedPopUpId').on('shown.bs.modal', function (event) {

        //$('#RevenueTargetVsAchievedPopUpTitleID').html("Revenue : Target Vs Achieved");
        //alert($('#spnOfficeDesc').text());
        var DistOrStatetext = "";
        if (DistrictText == undefined) {
            // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020
            //DistOrStatetext = "State Wide View";
            DistOrStatetext = $('#spnOfficeDesc').text();
            // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020
        } else {
            DistOrStatetext = DistrictText;
        }

        $('#RevenueTargetVsAchievedPopUpTitleID').text("Revenue : Target Vs Achieved (" + DistOrStatetext + ")");

        //if (selectedType == "F") {
        //    $('#RevenueTargetVsAchievedPopUpTitleID').text("Revenue : Target Vs Achieved For Financial Year: " + $("#FinYearListId option:selected").text() + "(" + DistOrStatetext + ")");
        //}
        //else if (selectedType == "M") {
        //    $('#RevenueTargetVsAchievedPopUpTitleID').text("Revenue : Target Vs Achieved For Current Month" + "(" + DistOrStatetext + ")");
        //}
        //else if (selectedType == "D") {
        //    $('#RevenueTargetVsAchievedPopUpTitleID').text("Revenue : Target Vs Achieved For Today" + "(" + DistOrStatetext + ")");
        //}

        var link = $(event.relatedTarget);
        // get data source
        var source = link.attr('data-source').split(',');
        // get title
        var title = link.html();
        // get labels
        var table = link.parents('table');
        var labels = [];
        $('#' + table.attr('id') + '>thead>tr>th').each(function (index, value) {
            // without first column
            if (index > 0) { labels.push($(value).html()); }
        });
        // get target source
        var target = [];
        $.each(labels, function (index, value) {
            target.push(link.attr('data-target-source'));
        });
        // Chart initialisieren
        var modal = $(this);
        var canvas = modal.find('.modal-body canvas');
        //var canvas = document.getElementById('cnvsSalesStatisticsLineChart');
        //modal.find('.modal-title').html("Sales Statistics");
        //var ctx = canvas[0].getContext("2d");
        //canvas.clientHeight = 2000;
        
        //canvas.attr('width', '863').attr('height', '455');
        //canvas.width = 2000;
        //canvas.height = 600;
        //canvas.width = 1200;
        //alert('1');
        //canvas.attr('width', '863').attr('height', '455');
        //alert('2');
       

        //if (SalesStatisticsLineChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        //    SalesStatisticsLineChart.destroy();
        //}
        var data = {
            labels: RevenueTargetVsAchievedFinYearsPopUp,
            datasets: [

                Target_DataSet,
                Achieved_DataSet
            ]

        };

        var barChartOptions = {
            //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
            scaleBeginAtZero: true,
            responsive: true,
            maintainAspectRatio: true,
            // To stop removal of dataset on click data set icon
            //legend: {
            //    onClick: function (e) {
            //        e.stopPropagation();
            //    }

            //}
            legend: {
                display: true,
                position: "top",
                labels: {
                    fontColor: "Black",

                },
                fontSize: 25,
                boxWidth: 40


            }

        }

        //////ADDED BY PANKAJ SAKHARE ON 05-10-2020 FOR LABEL IN POPUP
        //var opts = {
        //    scales: {
        //        yAxes: [{
        //            scaleLabel: {
        //                display: true,
        //                labelString: ($('input[name="rdoDBSaleStatistics"]:checked').val() === 'Revenue') ? 'Rupees in Crore' : ''
        //            }
        //        }]
        //    }
        //};

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
        var opts = {
            scales: {
                yAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: 'Rupees in Crore'
                    }
                }]
            }
        };
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
        if (window.RevenueTargetVsAchievedForModalPopup != undefined) {
            // alert('hjbjhjf');
            window.RevenueTargetVsAchievedForModalPopup.destroy();
        }

        RevenueTargetVsAchievedForModalPopup = Chart.Line(canvas, {
            data: data,
            //options: barChartOptions
            options: opts
        });
        
        //var ext1 = document.getElementById('canvasRevenueTargetVsAchievedPopUp');
        //var ext2 = ext1.getContext("2d");
        //ext2.scales(2,2);
        //document.getElementById('canvasRevenueTargetVsAchievedPopUp').width = 300;
        //document.getElementById('canvasRevenueTargetVsAchievedPopUp').offsetHeight = 4550;
        //document.getElementById('canvasRevenueTargetVsAchievedPopUp').css('height', '450px');

        // BELOW CODE IS WORKING FOR SAME SIZE POUP BUT MODAL ALIGNMENT IS NOT PROPER 
        var hmjbgm = document.getElementById('canvasRevenueTargetVsAchievedPopUp');
        hmjbgm.style.width = "1100px";
        hmjbgm.style.height = "600px";
        // ABOVE CODE IS WORKING FOR SAME SIZE POUP BUT MODAL ALIGNMENT IS NOT PROPER 


    }).on('hidden.bs.modal', function (event) {
        // reset canvas size
        var modal = $(this);
        var canvas = modal.find('.modal-body canvas');
        //canvas.attr('width', '863').attr('height', '455');
        //canvas.attr('width', '568px').attr('height', '300px');
        //canvas.css("font-size", "500%");
        //canvas.style.canvas.style.width = 863 + 'px';
        //canvas.style.canvas.style.height = 455 + 'px';
        // destroy modal
        //alert('ghnyj');
        $(this).data('bs.modal', null);
    });

    //var hmjbgm = document.getElementById('canvasRevenueTargetVsAchievedPopUp').getContext("2d");
    //hmjbgm.canvas.clientHeight = 1000;
   
    //alert('gbhjgmjh');
    //document.getElementById('canvasRevenueTargetVsAchievedPopUp').width = 160;
    //document.getElementById('canvasRevenueTargetVsAchievedPopUp').offsetHeight = 100;
    //$('#canvasRevenueTargetVsAchievedPopUp').css("font-size", "500%");
    //$('#canvasRevenueTargetVsAchievedPopUp').height("450");
    //$('#canvasRevenueTargetVsAchievedPopUp').css('height', 450);
    //$('#canvasRevenueTargetVsAchievedPopUp').css('height', '450px');
    
    //$('#canvasRevenueTargetVsAchievedPopUp').css("width", "807px");
    
     // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020

    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        

    $('#CurrentVsPrevFinYrPopUpId').on('shown.bs.modal', function (event) {

        //$('#RevenueTargetVsAchievedPopUpTitleID').html("Revenue : Target Vs Achieved");

        var DistOrStatetext = "";
        if (DistrictText == undefined) {
            // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020
            //DistOrStatetext = "State Wide View";
            DistOrStatetext = $('#spnOfficeDesc').text();
            // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020
        } else {
            DistOrStatetext = DistrictText;
        }

        $('#CurrentVsPrevFinYrPopUpTitleID').text("Progress : Current Vs Previous Fin. Years (" + DistOrStatetext + ")");

        //if (selectedType == "F") {
        //    $('#CurrentVsPrevFinYrPopUpTitleID').text("Progress : Current Vs Previous Fin. Years For Financial Year: " + $("#FinYearListId option:selected").text() + "(" + DistOrStatetext + ")");
        //}
        //else if (selectedType == "M") {
        //    $('#CurrentVsPrevFinYrPopUpTitleID').text("Progress : Current Vs Previous Fin. Years For Current Month" + "(" + DistOrStatetext + ")");
        //}
        //else if (selectedType == "D") {
        //    $('#CurrentVsPrevFinYrPopUpTitleID').text("Progress : Current Vs Previous Fin. Years For Today" + "(" + DistOrStatetext + ")");
        //}

        var link = $(event.relatedTarget);
        // get data source
        var source = link.attr('data-source').split(',');
        // get title
        var title = link.html();
        // get labels
        var table = link.parents('table');
        var labels = [];
        $('#' + table.attr('id') + '>thead>tr>th').each(function (index, value) {
            // without first column
            if (index > 0) { labels.push($(value).html()); }
        });
        // get target source
        var target = [];
        $.each(labels, function (index, value) {
            target.push(link.attr('data-target-source'));
        });
        // Chart initialisieren
        var modal = $(this);
        var canvas = modal.find('.modal-body canvas');
        //var canvas = document.getElementById('cnvsSalesStatisticsLineChart');
        //modal.find('.modal-title').html("Sales Statistics");
        //var ctx = canvas[0].getContext("2d");
        canvas.clientHeight = 2000;
        //alert('1');
        //canvas.attr('width', '863').attr('height', '455');
        //alert('2');

        //if (SalesStatisticsLineChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        //    SalesStatisticsLineChart.destroy();
        //}

        //alert(CurrentVsPrevFinYrFinYearsPopUp);
        //alert(JSON.stringify(Document_DataSet));
        //alert(JSON.parse(Document_DataSet));
        //alert(IsMonthsProgressChartVisible);
        //var data = {
        //    labels: CurrentVsPrevFinYrFinYearsPopUp,
        //    datasets: [
        //        Document_DataSet,
        //        Revenue_DataSet
        //    ]

        //};
        var data;
        // FOR SHOWING MONTHS PROGRESS CHART IN MODAL POPUP
        if (IsMonthsProgressChartVisible) {
            //alert('in if');
            data = {
                labels: CurrentVsPrevFinYrMonthsPopUp,
                datasets: [
                    Document_DataSet,
                    Revenue_DataSet
                ]

            };
        }
        // FOR SHOWING FIN YEAR PROGRESS CHART IN MODAL POPUP
        else {
           // alert('in else');
            data = {
                labels: CurrentVsPrevFinYrFinYearsPopUp,
                datasets: [
                    Document_DataSet,
                    Revenue_DataSet
                ]

            };
        }


        //var barChartOptions = {
        //    //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
        //    scaleBeginAtZero: true,
        //    responsive: true,
        //    maintainAspectRatio: true,
        //    // To stop removal of dataset on click data set icon
        //    //legend: {
        //    //    onClick: function (e) {
        //    //        e.stopPropagation();
        //    //    }

        //    //}
        //    legend: {
        //        display: true,
        //        position: "top",
        //        labels: {
        //            fontColor: "Black",

        //        },
        //        fontSize: 25,
        //        boxWidth: 40


        //    }

        //}

        //////ADDED BY PANKAJ SAKHARE ON 05-10-2020 FOR LABEL IN POPUP
        //var opts = {
        //    scales: {
        //        yAxes: [{
        //            scaleLabel: {
        //                display: true,
        //                labelString: ($('input[name="rdoDBSaleStatistics"]:checked').val() === 'Revenue') ? 'Rupees in Crore' : ''
        //            }
        //        }]
        //    }
        //};

        //RevenueTargetVsAchievedChart = Chart.Line(canvas, {
        //    data: data,
        //    //options: barChartOptions
        //    //options: opts
        //});
        //options
        var options = {
            responsive: true,
            title: {
                display: true,
                position: "top",
                //text: "Bar Chart",
                fontSize: 0,
                fontColor: "#111"
            },
            legend: {
                display: true,
                position: "top",
                labels: {
                    fontColor: "#333",
                    fontSize: 12
                },
                onClick: function (event, legendItem) {
                    //get the index of the clicked legend
                    //alert('dfdfad');
                    //var xLabel = this.scales['x-axis-0'].getValueForPixel(e.x);
                    ////console.log(xLabel.format('MMM YYYY'));
                    //alert("clicked x-axis area: " + xLabel);
                    var index = legendItem.datasetIndex;
                    //toggle chosen dataset's visibility
                    ProgressBarChartForModalPopup.data.datasets[index].hidden =
                        !ProgressBarChartForModalPopup.data.datasets[index].hidden;
                    //toggle the related labels' visibility
                    ProgressBarChartForModalPopup.options.scales.yAxes[index].display =
                        !ProgressBarChartForModalPopup.options.scales.yAxes[index].display;
                    ProgressBarChartForModalPopup.update();
                }

            },
            scales: {
                yAxes: [{
                    stacked: false,
                    position: "left",
                    id: "y-axis-0",
                    type: 'linear',
                    ticks: {
                        beginAtZero: true,

                    },
                    scaleLabel: {
                        display: true,
                        labelString: 'No of Documents'
                    },
                },

                {
                    stacked: false,
                    position: "right",
                    id: "y-axis-1",

                    type: 'linear',
                    ticks: {
                        beginAtZero: true,

                    },
                    scaleLabel: {
                        display: true,
                        labelString: 'Revenue : Rupees in Crore'
                    },
                }
                ]
            },
            layout: {
                padding: {
                    left: 0,
                    right: 0,
                    top: 0,
                    bottom: 0
                }
            }//,
            // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 13-10-2020
            //onClick: function (e) {

            //    var activePoints = ProgressBarChartForModalPopup.getElementsAtEvent(e);
            //    if (activePoints[0]) {
            //        var chartData = activePoints[0]['_chart'].config.data;
            //        var idx = activePoints[0]['_index'];

            //        var label = chartData.labels[idx];
            //        //var value = chartData.datasets[0].data[idx];
            //        //var color = chartData.datasets[0].backgroundColor[idx]; //Or any other data you wish to take from the clicked slice

            //        //alert(label + ' ' + value + ' ' + color); //Or any other function you want to execute. I sent the data to the server, and used the response i got from the server to create a new chart in a Bootstrap modal.
            //        //alert(label + ' ' + value); //Or any other function you want to execute. I sent the data to the server, and used the response i got from the server to create a new chart in a Bootstrap modal.
            //        alert(label); //Or any other function you want to execute. I sent the data to the server, and used the response i got from the server to create a new chart in a Bootstrap modal.
            //        PopulateProgressChart("2", "1", selectedType, DistrictCode_);
            //        IsMonthsProgressChartVisible = false;
            //        alert('IsMonthsProgressChartVisible : ' + IsMonthsProgressChartVisible);
            //        // CALL MAIN METHOD TO POPULATE ALL FIN YEAR CHART
            //    }

            //}

            // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 13-10-2020
        }
        //if (ProgressBarChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        //    ProgressBarChart.destroy();
        //    alert('in destroy');
        //}
        //var ctxLine = document.getElementById("canvasCurrentVsPrevFinYrPopUp").getContext("2d");
        //if (window.bar != undefined) {
        //    alert('in destroy');
        //    window.bar.destroy();
        //}
        //window.bar = new Chart(ctxLine, {});
        //create Chart class object
        if (window.ProgressBarChartForModalPopup != undefined) {
           // alert('hjbjhjf');
            window.ProgressBarChartForModalPopup.destroy();
        }
        ProgressBarChartForModalPopup = new Chart(canvas, {
            type: "bar",
            data: data,
            options: options
        });

        // BELOW CODE IS WORKING FOR SAME SIZE POUP BUT MODAL ALIGNMENT IS NOT PROPER 
        var hmjbgm = document.getElementById('canvasCurrentVsPrevFinYrPopUp');
        hmjbgm.style.width = "1100px";
        hmjbgm.style.height = "600px";
        // ABOVE CODE IS WORKING FOR SAME SIZE POUP BUT MODAL ALIGNMENT IS NOT PROPER 


    }).on('hidden.bs.modal', function (event) {
        // reset canvas size
        var modal = $(this);
        var canvas = modal.find('.modal-body canvas');
        //canvas.attr('width', '863').attr('height', '455');
        //canvas.attr('width', '568px').attr('height', '300px');
        //canvas.css("font-size", "500%");
        //canvas.style.canvas.style.width = 863 + 'px';
        //canvas.style.canvas.style.height = 455 + 'px';
        // destroy modal
        //alert('ghnyj');
        $(this).data('bs.modal', null);
    });

    //$('#canvasRevenueTargetVsAchievedPopUp').css("font-size", "500%");
    //$('#canvasRevenueTargetVsAchievedPopUp').height("450");
    //$('#canvasRevenueTargetVsAchievedPopUp').css('height', 450);
    //$('#canvasRevenueTargetVsAchievedPopUp').css('height', '450px');

    //$('#canvasRevenueTargetVsAchievedPopUp').css("width", "807px");

     // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020



    //alert('tab 1');//sb
    //alert(IsCurrentFinYear);
    //Added by Raman Kalegaonkar on 24-06-2020
    $("#FinYearListId").change(function () {
        //alert("Hiii");
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
        if ($("#FinYearListId option:selected").index() == "0") {
            IsCurrentFinYear = true;
            //alert('IsCurrentFinYear=' + IsCurrentFinYear);
        }
        else {
            IsCurrentFinYear = false;
            //alert('IsCurrentFinYear=' + IsCurrentFinYear);
        }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
        PopulateTiles();
        //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
        PopulateAvgRegTimeFun();
        //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
    });
    $('#FinYearListId').show();
    DistrictCode_ = FirstElemOfDistrictList; //First DistrictCode from the list should be passed for the first time
    DistrictTxt_ = FirstElemOfDistrictListTxt;
    // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
    DistrictTextForExcel = FirstElemOfDistrictListTxt;
    // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
    $('input:radio[name=rdoDBSummary][value=F]').trigger('click');
    selectedType = $('input[name="rdoDBSummary"]:checked').val();
    PopulateTiles();
    //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
    PopulateAvgRegTimeFun();
    //ADDED BY SHUBHAM BHAGAT ON 21-09-2020


    $('input[type=radio][name=rdoDBSummary]').change(function () {
        selectedType = $('input[name="rdoDBSummary"]:checked').val();
        PopulateTiles();
        //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
        PopulateAvgRegTimeFun();
        //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
        if (DistrictCode_ == "0") {
            //alert(DistrictCode_);
            if (selectedType == "F") {
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
                if ($("#FinYearListId option:selected").index() == "0") {
                    IsCurrentFinYear = true;
                    //alert('IsCurrentFinYear=' + IsCurrentFinYear);
                }
                else {
                    IsCurrentFinYear = false;
                    //alert('IsCurrentFinYear=' + IsCurrentFinYear);
                }
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020

                $("#OfficeStartTimeId").text("Average office start time for " + $("#FinYearListId option:selected").text() + " Fin Year");
                //$("#OfficeStartTimeId").text("Office Start time Indications on today (District Wise Office Start Time)");

                $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 Districts in Revenue Collection (Target Vs Achieved) in " + $("#FinYearListId option:selected").text() + " Fin year");
                $("#HighlightsId").text("State : " + $("#FinYearListId option:selected").text() + " Fin Year's Registration Highlights");
                $('#FinYearListId').show();

            }
            else if (selectedType == "M") {
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
                IsCurrentFinYear = true;
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020

                $("#OfficeStartTimeId").text("Average office start time for Current Month");
                $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 Districts in Revenue Collection (Contribution Percentage) in Current Month");
                $("#HighlightsId").text("State : Current Month's Registration Highlights");
                $('#FinYearListId').hide();

            }
            else if (selectedType == "D") {
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
                IsCurrentFinYear = true;
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020

                $("#OfficeStartTimeId").text("Office Start time Indications as on today");
                //$("#OfficeStartTimeId").text("District wise Office start time for Today");
                $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 Districts in Revenue Collection (Contribution Percentage) as on Today");
                $("#HighlightsId").text("State : Today's Registration Highlights");
                $('#FinYearListId').hide();

            }

        }
        else {
            //alert(DistrictCode_);
            if (selectedType == "F") {

                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
                if ($("#FinYearListId option:selected").index() == "0") {
                    IsCurrentFinYear = true;
                    //alert('IsCurrentFinYear=' + IsCurrentFinYear);
                }
                else {
                    IsCurrentFinYear = false;
                    //alert('IsCurrentFinYear=' + IsCurrentFinYear);
                }
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020

                $("#OfficeStartTimeId").text("Average office start time for " + $("#FinYearListId option:selected").text() + " Fin Year");
                $("#HighlightsId").text(DistrictText + " : " + $("#FinYearListId option:selected").text() + " Fin Years Registration Highlights");
                $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Target Vs achieved) in " + $("#FinYearListId option:selected").text() + " Fin year");
                $('#FinYearListId').show();

            }
            else if (selectedType == "M") {
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
                IsCurrentFinYear = true;
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020

                $("#OfficeStartTimeId").text("Average office start time for Current Month");
                $("#HighlightsId").text(DistrictText + " : Current Month's Registration Highlights");
                $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Contribution Percentage) in Current Month");
                $('#FinYearListId').hide();

            }
            else if (selectedType == "D") {
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
                IsCurrentFinYear = true;
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020

                $("#OfficeStartTimeId").text("Office start time indications as on Today");
                $("#HighlightsId").text(DistrictText + " : Today's Registration Highlights");
                $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Contribution Percentage) as on Today");
                $('#FinYearListId').hide();

            }

        }



    });

    LoadAllCharts();

    $(".rdoToggleChartTable").change(function () {
        var _val = $(this).val();
        var splittedArray = _val.split("_");
        ToggleGraphAndTable(splittedArray[0], splittedArray[1])
    });


});



//$("#cnvsProgressBarChart").click(
//    function (evt) {
//        alert('1');
//        var ctx = document.getElementById("cnvsProgressBarChart").getContext("2d");
//        // from the endPoint we get the end of the bars area
//        var base = myBar.scale.endPoint;
//        var height = myBar.chart.height;
//        var width = myBar.chart.width;
//        // only call if event is under the xAxis
//        if (evt.pageY > base) {
//            alert('2');
//            // how many xLabels we have
//            var count = myBar.scale.valuesCount;
//            var padding_left = myBar.scale.xScalePaddingLeft;
//            var padding_right = myBar.scale.xScalePaddingRight;
//            // calculate width for each label
//            var xwidth = (width - padding_left - padding_right) / count;
//            // determine what label were clicked on AND PUT IT INTO bar_index 
//            var bar_index = (evt.offsetX - padding_left) / xwidth;
//            // don't call for padding areas
//            if (bar_index > 0 & bar_index < count) {
//                bar_index = parseInt(bar_index);
//                // either get label from barChartData
//                console.log("barChartData:" + barChartData.labels[bar_index]);
//                // or from current data
//                var ret = [];
//                for (var i = 0; i < myBar.datasets[0].bars.length; i++) {
//                    ret.push(myBar.datasets[0].bars[i].label)
//                };
//                console.log("current data:" + ret[bar_index]);
//                // based on the label you can call any function
//            }
//        }
//    }
//);

function OnclickOfHref(DistrictCode, DistrictName) {
    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 19-10-2020
    $('#RevTartVsAch_Graph_btnID').addClass('active');
    $('#RevTartVsAch_DT_btnID').removeClass('active');
    $('#ProgCurrVsPrev_Graph_btnID').addClass('active');
    $('#ProgCurrVsPrev_DT_btnID').removeClass('active');
    $('#RevenueTargetVsAchievedMaxBtnId').show();
    $('#CurrentVsPrevFinYrMaxBtnId').show();    
    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 19-10-2020


    //alert(DistrictCode + " " + DistrictName);
    // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
    DistrictTextForExcel = DistrictName;
    // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
    DistrictText = DistrictName;
    DistrictCode_ = DistrictCode;
    $('#spnOfficeDesc').text(DistrictName);
    PopulateTiles();
    //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
    PopulateAvgRegTimeFun();
    //ADDED BY SHUBHAM BHAGAT ON 21-09-2020
    $("nav.navbar.bootsnav").on("mouseleave", function () {
        $('li.dropdown', this).removeClass("on");
        $(".dropdown-menu", this).stop().fadeOut();
        $(".dropdown-menu", this).removeClass(getIn);
        $(".col-menu", this).removeClass("on");
        $(".col-menu .content", this).stop().fadeOut();
        $(".col-menu .content", this).removeClass(getIn);
    });
    LoadRevenueTargetVsAchieved("1", "1", selectedType, DistrictCode_);
    PopulateProgressChart("2", "1", selectedType, DistrictCode_);
    //alert(selectedType+"   ,   "+DistrictCode_);
    //if (selectedType == "F") {
    //    $("#OfficeStartTimeId").text("SRO wise Average office start time indication for Current Fin Year");
    //    $("#HighlightsId").text("Current Fin Years Highlights :" + DistrictText);
    //    $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Contribution Percentage) in Current Fin year");
    //}   
    //else if (selectedType == "M") {
    //    $("#OfficeStartTimeId").text("SRO wise Average office start time indication for Current Month");
    //    $("#HighlightsId").text("Current Month's Highlights : " + DistrictText);
    //    $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Contribution Percentage) in Current Month");
    //}
    //else if (selectedType == "D") {
    //    $("#OfficeStartTimeId").text("SRO wise Office start time indication for Today");
    //    $("#HighlightsId").text("Today's Highlights :"+ DistrictText);
    //    $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Contribution Percentage) as on Today");
    //}
    if (DistrictCode_ == "0") {

        if (selectedType == "F") {
            $("#OfficeStartTimeId").text("Average office start time for Fin Year " + $("#FinYearListId option:selected").text());
            //$("#OfficeStartTimeId").text("Office Start time Indications on today (District Wise Office Start Time)");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 Districts in Revenue Collection (Target Vs Achieved) in Fin year " + $("#FinYearListId option:selected").text());
            $("#HighlightsId").text("State : Current Fin Years Registration Highlights");
        }
        else if (selectedType == "M") {
            $("#OfficeStartTimeId").text("Average office start time for Current Month");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 Districts in Revenue Collection (Contribution Percentage) in Current Month");
            $("#HighlightsId").text("State : Current Month's Registration Highlights");
        }
        else if (selectedType == "D") {
            //$("#OfficeStartTimeId").text("District wise Office start time for Today");
            $("#OfficeStartTimeId").text("Office Start time Indications as on today");

            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 Districts in Revenue Collection (Contribution Percentage) as on Today");
            $("#HighlightsId").text("State : Today's Registration Highlights");
        }

    }
    else {
        if (selectedType == "F") {
            $("#OfficeStartTimeId").text("Average office start time for Fin Year " + $("#FinYearListId option:selected").text());
            $("#HighlightsId").text(DistrictText + " : " + $("#FinYearListId option:selected").text() + " Fin Year's Registration Highlights");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Target Vs Achieved) in " + $("#FinYearListId option:selected").text() + " Fin year");
        }
        else if (selectedType == "M") {
            $("#OfficeStartTimeId").text("Average office start time for Current Month");
            $("#HighlightsId").text(DistrictText + " : Current Month's Registration Highlights");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Contribution Percentage) in Current Month");
        }
        else if (selectedType == "D") {
            $("#OfficeStartTimeId").text("Office start time indications as on Today");
            $("#HighlightsId").text(DistrictText + " : Today's Registration Highlights");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Contribution Percentage) as on Today");
        }

    }
}

function LoadAllCharts() {
    //alert("On LoadAllCharts");
    LoadRevenueTargetVsAchieved("1", "1", selectedType, DistrictCode_);
    PopulateProgressChart("2", "1", selectedType, DistrictCode_);

}
function ToggleGraphAndTable(GraphId, toggleBtnId) {
    //alert("In ToggleGraphAndTable:GraphId:" + GraphId + "toggleBtnId:" + toggleBtnId)
    switch (GraphId) {
        case "1":
            	 // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
            if (toggleBtnId == "2") {
                $('#RevenueTargetVsAchievedMaxBtnId').hide();
            }
            else {
                $('#RevenueTargetVsAchievedMaxBtnId').show();

            }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020

            LoadRevenueTargetVsAchieved(GraphId, toggleBtnId, selectedType, DistrictCode_);
            break;

        case "2":
            // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
            if (toggleBtnId == "2") {
                $('#CurrentVsPrevFinYrMaxBtnId').hide();
            }
            else {
                $('#CurrentVsPrevFinYrMaxBtnId').show();

            }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020

             // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
            // BELOW CODE IS COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 15-10-2020
      
            // PopulateProgressChart(GraphId, toggleBtnId, selectedType, DistrictCode_);

            // IF THE MONTHS PROGRESS CHART IS VISIBLE THEN CALL PopulateProgressChartMonthWise METHOD TO 
            // POPULATE GRAPH OT DATATABLE FOR MONTHS WISE OTHERWISE CALL PopulateProgressChart TO POPULATE
            // GRAPH OT DATATABLE FOR all Finyear
            if (IsMonthsProgressChartVisible) {
                PopulateProgressChartMonthWise(GraphId, toggleBtnId, selectedType, DistrictCode_, FinYearForProgressMonthsChart);
            }
            else {
                PopulateProgressChart(GraphId, toggleBtnId, selectedType, DistrictCode_);
            }
              // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
            break;
        default:
    }
}

//region for Revenue target vs achieved
function LoadRevenueTargetVsAchieved(GraphId, toggleBtnId, selectedType, DistrictCode_) {
    //alert(selectedType + "FFFF"+DistrictCode_);
    blockUI('loading data.. please wait...');
    $.ajax({
        url: "/Dashboard/Dashboard/LoadRevenueTargetVsAchieved",
        type: "POST",
        data: { 'toggleBtnId': toggleBtnId, 'selectedType': selectedType, 'DistrictCode': DistrictCode_ },

        success: function (jsonData) {
            //alert(DistrictText);
            if (toggleBtnId == 1) {
                $("#tbl_1_Wrapper").remove();
                //$("#dv_1_RevenueTargetVsAchieved_Wrapper").html('<div id="dv_1_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart"><canvas id="cnvsRevenueTargetVsAchievedChart" style="height:300px; width: 807px;"></canvas></div></div></div>');
                $("#dv_1_RevenueTargetVsAchieved_Wrapper").html('<div id="dv_1_Graph"><div class="box-body chat" id="chat-box" style="width: auto; overflow: hidden;"><div class="chart"><canvas id="cnvsRevenueTargetVsAchievedChart" height="188"></canvas></div></div></div>');

                RevenueTargetVsAchievedDataSet(jsonData);
                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                $("#EXCELRevenueTargetVsAchievedSpanID").html('');
                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                unBlockUI();

            }
            if (toggleBtnId == 2) {
                $("#dv_1_Graph").remove();
                $("#dv_1_RevenueTargetVsAchieved_Wrapper").html('<div id="tbl_1_Wrapper"><table id="tbl_1_RevenueTargetVsAchieved" class="table table-striped table-bordered table-condensed table-hover" style="text-align:center !important; vertical-align: middle;background-color: aliceblue; overflow: hidden; border: solid;width: 96%;"></table></div>');
                //alert("Last");
                FillDataTable(jsonData.TableColumns, jsonData.TableData, "tbl_1_RevenueTargetVsAchieved", "tbl_1_Wrapper")
                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                if (jsonData.TableData.length != undefined) {
                    $("#EXCELRevenueTargetVsAchievedSpanID").html('');
                    $("#EXCELRevenueTargetVsAchievedSpanID").html(jsonData.ExcelBtn);
                }
                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                unBlockUI();

            }

        },
        error: function (xhr, status, err) {
            bootbox.alert("Error occured while proccessing your request : " + err);

            unBlockUI();

        }
    });





}
function RevenueTargetVsAchievedDataSet(jsonData) {

    //alert(jsonData.LineChart.Lbl_Target);
    //alert(jsonData.LineChart.Lbl_Achieved);

    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
    //RevenueTargetVsAchievedTargetPopUp=jsonData.LineChart.Target;
    //RevenueTargetVsAchievedAchievedPopUp=jsonData.LineChart.Achieved;
    RevenueTargetVsAchievedFinYearsPopUp = jsonData.LineChart.FinYears;
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
   

    Target_DataSet =
        {
            label: jsonData.LineChart.Lbl_Target,
            // lineTension: 0.1,
            backgroundColor: "#F39C12",
            borderColor: "#F39C12",
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: "rgba(75,192,192,1)",
            pointBackgroundColor: "#fff",
            pointBorderWidth: 1,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: "rgba(75,192,192,1)",
            pointHoverBorderColor: "rgba(220,220,220,1)",
            pointHoverBorderWidth: 2,
            pointRadius: 5,
            pointHitRadius: 10,
            fill: false,
            data: jsonData.LineChart.Target,
        };

    Achieved_DataSet =
        {
            label: jsonData.LineChart.Lbl_Achieved,
            //lineTension: 0.1,
            backgroundColor: "#3C8DBC",
            borderColor: "#3C8DBC",
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: "rgba(75,192,192,1)",
            pointBackgroundColor: "#fff",
            pointBorderWidth: 1,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: "rgba(75,192,192,1)",
            pointHoverBorderColor: "rgba(220,220,220,1)",
            pointHoverBorderWidth: 2,
            pointRadius: 5,
            pointHitRadius: 10,
            fill: false,
            data: jsonData.LineChart.Achieved,
        };



    ReloadRevenueTargetVsAchievedChart(jsonData.LineChart.FinYears);


}
function ReloadRevenueTargetVsAchievedChart(FinYear) {
    var canvas = document.getElementById('cnvsRevenueTargetVsAchievedChart');
    if (RevenueTargetVsAchievedChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        RevenueTargetVsAchievedChart.destroy();
    }
    var data = {
        labels: FinYear,
        datasets: [

            Target_DataSet,
            Achieved_DataSet

        ]

    };
    var option = {
        responsive: true,
        title: {
            display: true,
            position: "top",
            //text: "Bar Chart",
            fontSize: 0,
            fontColor: "#111"
        },
        legend: {
            display: true,
            position: "top",
            labels: {
                fontColor: "#333",
                fontSize: 12
            }
        },
        scales: {
            yAxes: [{
                ticks: {
                    min: 0,
                    //padding: 0,
                }
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
                ,
                scaleLabel: {
                    display: true,
                    labelString:  'Rupees in Crore' 
                }
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
            }]
        },
        layout: {
            padding: {
                left: 0,
                right: 0,
                top: 0,
                bottom: 0
            }
        }
    }

    var barChartOptions = {
        //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
        scaleBeginAtZero: true,
        responsive: true,
        maintainAspectRatio: true,
        // To stop removal of dataset on click data set icon
        legend: {
            onClick: function (e) {
                e.stopPropagation();
            }
        }

    }

    var RevenueTargetVsAchievedChart = Chart.Line(canvas, {
        data: data,
        options: option
    });

}

//Region for Progress CHART
function PopulateProgressChart(GraphId, toggleBtnId, selectedType, DistrictCode_) {
    //alert("In PopulateProgressChart : " + "selectedType : " + selectedType + "DistrictCode_ : " + DistrictCode_);
    blockUI('loading data.. please wait...');
    $.ajax({
        url: "/Dashboard/Dashboard/PopulateProgressChart",
        type: "POST",
        data: { 'toggleBtnId': toggleBtnId, 'selectedType': selectedType, 'DistrictCode': DistrictCode_ },

        success: function (jsonData) {

            if (toggleBtnId == 1) {
                $("#dv_2_ProgressChart_Wrapper").remove();
                //$("#dv_1_CurrentVsPrevFinYr_Wrapper").html('<div id="dv_2_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart" style="border-radius:10px;"><canvas id="cnvsProgressBarChart" style="height:300px; width: 807px;"></canvas></div></div></div>');
                $("#dv_1_CurrentVsPrevFinYr_Wrapper").html('<div id="dv_2_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart" style="border-radius:10px;"><canvas id="cnvsProgressBarChart" height="188"></canvas></div></div></div>');
                PopulateProgressChartDataSet(jsonData);
                PopulatePregresschart(jsonData);
                // ADDED BY PANKAJ ON 17-09-2020
                $("#EXCELCurrentVsPrevFinYearSpanID").html('');
                // ADDED BY PANKAJ ON 17-09-2020
            }
            if (toggleBtnId == 2) {
                $("#dv_2_Graph").remove();
                $("#dv_1_CurrentVsPrevFinYr_Wrapper").html('<div id="dv_2_ProgressChart_Wrapper"><table id="tbl_2_PregressChart" class="table table-striped table-bordered table-condensed table-hover" style="text-align:center !important; vertical-align: middle;background-color: aliceblue; overflow: hidden; border: solid;width: 50%;"></table></div>');
                FillDataTable(jsonData.TableColumns, jsonData.TableData, "tbl_2_PregressChart", "dv_2_ProgressChart_Wrapper")
                $("#dv_2_Table").show();
                $("#dv_2_Graph").hide();
                // ADDED BY PANKAJ ON 17-09-2020
                if (jsonData.TableData.length != undefined) {
                    $("#EXCELCurrentVsPrevFinYearSpanID").html('');
                    $("#EXCELCurrentVsPrevFinYearSpanID").html(jsonData.ExcelBtn);
                }
                // ADDED BY PANKAJ ON 17-09-2020
            }

            unBlockUI();
        },
        error: function (xhr, status, err) {
            //alert("in PopulateSurchargeCessBarChart");
            bootbox.alert("Error occured while proccessing your request : " + err);
            unBlockUI();

        }
    });




}
function PopulateProgressChartDataSet(jsonData) {
    //alert('in PopulateProgressChartDataSet(jsonData) fadefhdsjkj');

    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
    CurrentVsPrevFinYrFinYearsPopUp = jsonData.BarChart.FinYear;
    //alert(CurrentVsPrevFinYrFinYearsPopUp)
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
    Document_DataSet = {
        //datalabels: {
        //    listeners: {
        //        click: function (context) {
        //            alert(context.datasetIndex)
        //        },

        //    }
        //},

        label: jsonData.BarChart.Lbl_Documents,
        data: jsonData.BarChart.Documents,
        yAxisID: "y-axis-0",
        backgroundColor: "#3C8DBC",
        borderColor: "#3C8DBC",
        borderCapStyle: 'butt',
        borderDash: [],
        borderDashOffset: 0.0,
        borderJoinStyle: 'miter',
        pointBorderColor: "rgba(75,192,192,1)",
        pointBackgroundColor: "#fff",
        pointBorderWidth: 1,
        pointHoverRadius: 5,
        pointHoverBackgroundColor: "rgba(75,192,192,1)",
        pointHoverBorderColor: "rgba(220,220,220,1)",
        pointHoverBorderWidth: 2,
        pointRadius: 5,
        pointHitRadius: 10,
        fill: false,
        // Changes this dataset to become a line
        type: 'line'
    },

        Revenue_DataSet =
        {
            //datalabels: {
            //    listeners: {
            //        click: function (context) {
            //            alert(context.datasetIndex)
            //        }
            //    }
            //},
            label: jsonData.BarChart.Lbl_Revenue,
            data: jsonData.BarChart.Revenue,
            yAxisID: "y-axis-1",
            backgroundColor: [
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
            ],
            borderColor: [
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)"

            ],
            borderWidth: 1,
            //// Changes this dataset to become a line
            //type: 'line'

        };

}
function PopulatePregresschart(jsonData) {
    //alert('in PopulatePregresschart(jsonData)');
    //$('#CollapseSearchDiv').trigger('click');
    //bar chart data
    var ctx = document.getElementById("cnvsProgressBarChart");
    if (ProgressBarChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        ProgressBarChart.destroy();
    }
    var data = {
        labels: jsonData.BarChart.FinYear,
        datasets: [
            Document_DataSet,
            Revenue_DataSet
        ]
    };

    //options
    var options = {
        responsive: true,
        title: {
            display: true,
            position: "top",
            //text: "Bar Chart",
            fontSize: 0,
            fontColor: "#111"
        },
        legend: {
            display: true,
            position: "top",
            labels: {
                fontColor: "#333",
                fontSize: 12
            },
            onClick: function (event, legendItem) {
                //get the index of the clicked legend
                //alert('dfdfad');
                //var xLabel = this.scales['x-axis-0'].getValueForPixel(e.x);
                ////console.log(xLabel.format('MMM YYYY'));
                //alert("clicked x-axis area: " + xLabel);
                var index = legendItem.datasetIndex;
                //toggle chosen dataset's visibility
                ProgressBarChart.data.datasets[index].hidden =
                    !ProgressBarChart.data.datasets[index].hidden;
                //toggle the related labels' visibility
                ProgressBarChart.options.scales.yAxes[index].display =
                    !ProgressBarChart.options.scales.yAxes[index].display;
                ProgressBarChart.update();
            }

        },
        scales: {
            yAxes: [{
                stacked: false,
                position: "left",
                id: "y-axis-0",
                type: 'linear',
                ticks: {
                    beginAtZero: true,

                },
                scaleLabel: {
                    display: true,
                    labelString: 'No of Documents'
                }
            },

            {
                stacked: false,
                position: "right",
                id: "y-axis-1",

                type: 'linear',
                ticks: {
                    beginAtZero: true,

                },
                scaleLabel: {
                    display: true,
                    labelString: 'Revenue : Rupees in Crore'
                },
            }
            ]
        },
        layout: {
            padding: {
                left: 0,
                right: 0,
                top: 0,
                bottom: 0
            }
        },
        // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 13-10-2020
        //onClick: function (e) {

        //    var activePoints = ProgressBarChart.getElementsAtEvent(e);
        //    if (activePoints[0]) {
        //        var chartData = activePoints[0]['_chart'].config.data;
        //        var idx = activePoints[0]['_index'];

        //        var label = chartData.labels[idx];
              
        //        ProgressBarChartMonthWise(label);

        //        FinYearForProgressMonthsChart=label;
                
        //        IsMonthsProgressChartVisible = true;

        //        //alert('IsMonthsProgressChartVisible : ' + IsMonthsProgressChartVisible);
        //    }



        //    //var activePoints = ProgressBarChart.getElementsAtEvent(e);

        //    //var selectedIndex = activePoints[0]._index;
        //    //alert(selectedIndex);
        //    //alert(this.data.datasets[0].data[selectedIndex]);
        //    //alert(this.data.datasets[1].data[selectedIndex]);

        //    //var clickedDatasetIndex = activePoints[1]._datasetIndex;
        //    //alert(clickedDatasetIndex);
        //    //var clickedDatasetPoint = ProgressBarChart.data.datasets[clickedDatasetIndex];
        //    //alert(clickedDatasetPoint);
        //    //var label = clickedDatasetPoint.label;
        //    //alert(label);
        //    //var value = clickedDatasetPoint.data[selectedIndex]["x"];
        //    //alert(value);


        //    //var clickedDatasetIndex = activePoints[0]._datasetIndex;
        //    //var clickedElementIndex = activePoints[0]._index;
        //    //var clickedDatasetPoint = ctx.data.datasets[clickedDatasetIndex];
        //    //var label = clickedDatasetPoint.label;
        //    //var value = clickedDatasetPoint.data[clickedElementIndex]["y"];

        //    //alert("Clicked: " + label + " - " + value);   




        //    //alert(this.data.datasets[2].data[selectedIndex]);
        //    //alert(this.data.datasets[3].data[selectedIndex]);
        //}
       
        // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 13-10-2020
        // BELOW CODE IS COMMENTED TO STOP LABLE CLICK ON Progress : Current Vs prev fin year BY SHUBHAM BHAGAT ON 26-11-2020
    }
    //create Chart class object
    var ProgressBarChart = new Chart(ctx, {
        type: "bar",
        data: data,
        options: options
    });

      // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 13-10-2020
    //document.getElementById("cnvsProgressBarChart").onclick = function (evt) {
    //    var activePoints = ProgressBarChart.getElementsAtEventForMode(evt, 'point', ProgressBarChart.options);
    //    var firstPoint = activePoints[0];
    //    var label = ProgressBarChart.data.labels[firstPoint._index];
    //    var value = ProgressBarChart.data.datasets[firstPoint._datasetIndex].data[firstPoint._index];
    //    alert(label + ": " + value);
    //};
    // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 13-10-2020
    unBlockUI();


}

function FillDataTable(TableColumns, TableData, HtmlTableID, WrapperDivID) {

    //|| jsonData.data.length == 0
    if (TableData.length === undefined) {

        bootbox.alert({
            size: 'small',
            message: '<i class="fa fa-exclamation-triangle text-warning boot-icon"></i> <span class="boot-alert-txt"> Invalid option selected.</span>',
            callback: function () { /* your callback code */ }
        });
        unBlockUI();
    }
    else {
        if ($.fn.DataTable.isDataTable("#" + HtmlTableID)) {
            if ($('#' + HtmlTableID).length > 0)
                $('#' + HtmlTableID).remove();
            $('#' + WrapperDivID).html('<table id="' + HtmlTableID + '" class="table table-striped table-bordered table-condensed table-hover" style="text-align:center !important;vertical-align: middle;background-color: aliceblue; overflow: hidden; border: solid;width: 96%;"></table>');
        }
        else {
            $('#' + WrapperDivID).html('<table id="' + HtmlTableID + '" class="table table-striped table-bordered table-condensed table-hover" style="text-align:center !important;vertical-align: middle;background-color: aliceblue; overflow: hidden; border: solid;width: 96%;"></table>');
        }


        var table = $('#' + HtmlTableID).DataTable({
            dom: "Bfrtip",
            destroy: true,
            // "bPaginate": false,
            "bInfo": false,
            "bPaginate": false,
            "bFilter": false,
            responsive: true,
            "scrollY": "240px",
            //"scrollX": true,

            "scrollCollapse": true,
            searching: false,
            //   language: { search: "Search" },
            ordering: false,//sorting...//akash
            buttons: [
                // 'copy', 'csv', 'excel', 'pdf'
            ],
            columnDefs: [{
                "searchable": true,
                "orderable": false,
                "className": "text-center", //added by akash
                "width": 50,
                "targets": 0 //index of column to be searched
            },
            { "className": "text-center", "targets": "_all" }
            ],
            order: [[1, 'asc']],
            data: TableData,
            columns: TableColumns,
            fnInitComplete: function (oSettings, json) {

            }
        });

    }
    unBlockUI();

}

//Populates Tiles
function PopulateTiles() {
    if (DistrictCode_ == "0") {

        if (selectedType == "F") {
            $("#OfficeStartTimeId").text("Average office start time for Fin Year " + $("#FinYearListId option:selected").text());
            //$("#OfficeStartTimeId").text("Office Start time Indications on today (District Wise Office Start Time)");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 Districts in Revenue Collection (Target Vs Achieved) in Fin year " + $("#FinYearListId option:selected").text());
            $("#HighlightsId").text("State :" + $("#FinYearListId option:selected").text() + " Fin Year's Registration Highlights");
            $('#FinYearListId').show();
            $('#DurationDescID').html("For Fin. Year " + $("#FinYearListId option:selected").text());
        }
        else if (selectedType == "M") {
            $("#OfficeStartTimeId").text("Average office start time for Current Month");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 Districts in Revenue Collection (Contribution Percentage) in Current Month");
            $("#HighlightsId").text("State : Current Month's Registration Highlights");
            $('#FinYearListId').hide();
            $('#DurationDescID').html("For Current Month");


        }
        else if (selectedType == "D") {
            //$("#OfficeStartTimeId").text("District wise Office start time for Today");
            //$("#OfficeStartTimeId").text("Office Start time Indications on today (District Wise Office Start Time)");
            $("#OfficeStartTimeId").text("Office Start time Indications as on today");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 Districts in Revenue Collection (Contribution Percentage) as on Today");
            $("#HighlightsId").text("State : Today's Registration Highlights");
            $('#FinYearListId').hide();
            $('#DurationDescID').html("For Fin. Year " + $("#FinYearListId option:selected").text());
            $('#DurationDescID').html("For Today");

        }

    }
    else {
        if (selectedType == "F") {
            $("#OfficeStartTimeId").text("Average office start time for " + $("#FinYearListId option:selected").text() + " Fin Year");
            $("#HighlightsId").text(DistrictTxt_ + " : " + $("#FinYearListId option:selected").val() + " Fin Year's Registration Highlights");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Target Vs Achieved) in " + $("#FinYearListId option:selected").val() + " Fin year");
            $('#FinYearListId').show();
            // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 05-08-2020 AT 5:00 PM
            //$('#DurationDescID').html("For Fin. Year " + $("#FinYearListId option:selected").val());
            $('#DurationDescID').html("For Fin. Year " + $("#FinYearListId option:selected").text());

        }
        else if (selectedType == "M") {
            $("#OfficeStartTimeId").text("Average office start time for Current Month");
            $("#HighlightsId").text(DistrictTxt_ + " : Current Month's Registration Highlights");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Contribution Percentage) in Current Month");
            $('#FinYearListId').hide();
            $('#DurationDescID').html("For Current Month");


        }
        else if (selectedType == "D") {
            $("#OfficeStartTimeId").text("Office start time indications as on Today");
            $("#HighlightsId").text(DistrictTxt_ + " : Today's Registration Highlights");
            $("#TopAndBottomOfficesId").text("Top 3 and bottom 3 SROs in Revenue Collection (Contribution Percentage) as on Today");
            $('#FinYearListId').hide();
            $('#DurationDescID').html("For Today");

        }
    }

    blockUI('loading data.. please wait...');
    // BELOW CODE IS COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 17-10-2020
    // BECAUSE IN CASE OF CURRENT MONTH AND TODAY WE HAVE TO SEND CURRENT FINANCIAL YEAR
    //FinYear = $("#FinYearListId option:selected").val();
  
    if (selectedType == "F") {
        FinYear = $("#FinYearListId option:selected").val();
    } else {
        FinYear = $("#FinYearListId option:first").val();     
    }
    //alert('FinYear :' + FinYear);
    // BELOW CODE IS COMMENTED AND ADDED BY SHUBHAM BHAGAT ON 17-10-2020

    $.ajax({
        type: "GET",
        url: "/Dashboard/Dashboard/PopulateTiles",
        data: { 'selectedType': selectedType, 'SelectedOffice': DistrictCode_, 'FinYearId': FinYear },
        success: function (data) {
            if (data.success) {
                for (var i = 0; i < data.TilesModel.Tiles.length; i++) {
                    //$("#T_" + (i + 1) + "_Amt").remove();
                    //$("#T_" + (i + 1) + "_Amt").remove();
                    //$("#T_" + (i + 1) + "_Amt").remove();

                    //$("#T_" + (i + 1) + "_Amt").text(data.TilesModel.Tiles[i].Amount);
                    //$("#T_" + (i + 1) + "_Title").text(data.TilesModel.Tiles[i].Title);
                    //$("#T_" + (i + 1) + "_Desc").text(data.TilesModel.Tiles[i].Description);
                    if (i == 0 || i == 1 || i == 2 || i == 5) {
                        if (data.IsStateWise == "1")//For State wise Data
                        {
                            $("#T_" + (i + 1) + "_Amt").html('<span style="font-size:38px;white-space:nowrap;font-weight: bold;">' + data.TilesModel.Tiles[i].Amount + '</span style="font-size: 300px;font-weight: bold;"><span style="font-size: 30px;font-weight: bold;"> Cr</span>');
                        }
                        else {
                            $("#T_" + (i + 1) + "_Amt").html('<span style="font-size:38px;white-space:nowrap;font-weight: bold;">' + data.TilesModel.Tiles[i].Amount + '</span style="font-size: 300px;font-weight: bold;"><span style="font-size: 25px;font-weight: bold;"> Cr</span>');
                        }
                    }
                    else {
                        $("#T_" + (i + 1) + "_Amt").html('<span style="font-size:38px;white-space:nowrap;font-weight: bold;">' + data.TilesModel.Tiles[i].Amount + '</span style="font-size: 300px;font-weight: bold;"><span style="font-size: 30px;font-weight: bold;"> </span>');
                    }
                    $("#T_" + (i + 1) + "_Title").text(data.TilesModel.Tiles[i].Title);
                    $("#T_" + (i + 1) + "_Desc").html('<span style="font-size:18px;white-space:nowrap;font-weight: bold;">' + data.TilesModel.Tiles[i].DescPercentage + '</span style="font-size: 30px;font-weight: bold !important;"><span style="font-size: 14px;font-weight: bold;">' + data.TilesModel.Tiles[i].Description + '</span>');
                }
                PopulateRevenueList(data.TilesModel._RevenueCollectionWrapperModel);
                PopulateCurrentAchievements(data.TilesModel.CurrentAchievementsModel);
                fillStartTimeIndications(data.TilesModel.StartTimeIndicationTop, data.TilesModel.StartTimeIndicationBottom);
                PopulateProgressBarTop(data.TilesModel._ProgressBarTargetVsAchieved);
            }
            else {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error Occured while processing request </span>'
                });
            }
            unBlockUI();

        },
        error: function () {
            bootbox.alert({
                //   size: 'small',
                //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
        }
    });
}

//To Populate Revenue List
function PopulateRevenueList(RevenueCollectionWrapperModel) {


    $("#sectionRevenueCollectedTop").html(RevenueCollectionWrapperModel.UpperRevenueList);
    $("#sectionRevenueCollectedBottom").html(RevenueCollectionWrapperModel.LowerRevenueList);

}

//To Populate Current Achievements
function PopulateCurrentAchievements(CurrentAchievementsModel) {
    var index;
    $("#UlAchivements").html('');
    //alert("in PopulateCurrentAchievements:" + CurrentAchievementsModel.CurrentAchievementsList.length + "    " + CurrentAchievementsModel.CurrentAchievementsList[0]);
    for (index = 0; index < CurrentAchievementsModel.CurrentAchievementsList.length; index++) {
        //alert("in ForEach" + "index % 4" + (index+1)%4);
        //$("#achivementID ul").append('<li class="list-group-item list-group-item-secondary">' + CurrentAchievementsModel.CurrentAchievementsList[index] + '</li>');

        switch ((index + 1) % 2) {
            case 1:
                $("#UlAchivements").append('<li class="list-group-item list-group-item-secondary">' + CurrentAchievementsModel.CurrentAchievementsList[index] + '</li>');
                break;
            //case 2:
            //    $("#achivementID ul").append('<li class="list-group-item list-group-item-info">' + CurrentAchievementsModel.CurrentAchievementsList[index] + '</li>');
            //    break;
            //case 3:
            //    $("#achivementID ul").append('<li class="list-group-item list-group-item-warning">' + CurrentAchievementsModel.CurrentAchievementsList[index] + '</li>');
            //    break;
            case 0:
                $("#UlAchivements").append('<li class="list-group-item list-group-item-secondary">' + CurrentAchievementsModel.CurrentAchievementsList[index] + '</li>');
                break;
        }






    }
    var $allLists = $("#UlAchivements")

    FadeLists($allLists, 0);

}

//To Populate Target Vs Achieved 
//function PopulateTargetVsAchieved()
//{

//    $('#TargetBarID').
//}


function LoadPopup(PopupType) {
    blockUI('loading data.. please wait...');
    FinYear = $("#FinYearListId option:selected").val();

    $('#divLoadPopupView').load("/Dashboard/Dashboard/LoadPopup", // url 
        {
            'PopupType': PopupType,
            'selectedType': selectedType,
            'SelectedOffice': DistrictCode_,
            'FinYear': FinYear

        },    // data 
        function (data, status, jqXGR) {  // callback function 
            $("#divLoadPopup").modal("show");
            unBlockUI();
            if (selectedType == "F") {
                $('#IDPercentContributed').text("TARGET ACHIEVED");

            }
            //ADDED BY PANKAJ SAKHARE ON 05-10-2020 FOR HEADING IN POPUP MODEL
            var DistOrStatetext = "";
            if (DistrictText == undefined) {
                // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020
                //DistOrStatetext = "State Wide View";
                DistOrStatetext = $('#spnOfficeDesc').text();
                // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 18-12-2020               

            } else {
                DistOrStatetext = DistrictText;
            }

            if (selectedType == "F") {
                $('#spnModalHeadingSelectedVal').text("For Financial Year: " + $("#FinYearListId option:selected").text() + "(" + DistOrStatetext + ")");
            }
            else if (selectedType == "M") {
                $('#spnModalHeadingSelectedVal').text("For Current Month" + "(" + DistOrStatetext + ")");
            }
            else if (selectedType == "D") {
                $('#spnModalHeadingSelectedVal').text("For Today" + "(" + DistOrStatetext + ")");
            }
        });

}


function fillStartTimeIndications(StartTimeIndicationTop, StartTimeIndicationBottom) {

    $("#sectionAvgTimeTop").html(StartTimeIndicationTop);
    $("#sectionAvgTimeBottom").html(StartTimeIndicationBottom);

}


function PopulateProgressBarTop(ProgressBarTopModel) {
    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 22-10-2020
    // FOR MAKING LABEL DYNAMIC PIN POINT ON LEFT SIDE OR RIGHT SIDE ACCORDING TO PERCENTAGE
    //alert('TargetPercentage_Bar:' + ProgressBarTopModel.TargetPercentage_Bar);
    //if (ProgressBarTopModel.TargetPercentage_Bar != null)
    //{
        
    //    alert(ProgressBarTopModel.TargetPercentage_Bar.substring(0, ProgressBarTopModel.TargetPercentage_Bar.indexOf("%")));
    //    var target_INT = ProgressBarTopModel.TargetPercentage_Bar.substring(0, (ProgressBarTopModel.TargetPercentage_Bar.indexOf("%")));
    //    alert(target_INT);

    //    //alert(ProgressBarTopModel.TargetPercentage_Bar.indexOf("%"));
    //    if (target_INT >= 70) {
    //        alert('target if');
    //        $('#TargetId').removeClass('progress-value').addClass('demoRight');
    //        //$('.progress').css('border-right', '5px solid #3177b4');
    //        //$('.progress-value').css('border-right', '5px solid #3177b4');

    //        //$('.progress').removeProp('border-left');
    //        //$('.progress-value').removeProp('border-left');
    //        //$('.progress').removeProp('left');
    //        //$('.progress-value').removeProp('left');

    //        //$('.progress').css('right', '0%');
    //        //$('.progress-value').css('right', '0%');
    //        //$('#TargetId').removeProp('border-left');
    //        //$('#TargetId').css('border-right', '5px solid #3177b4');
    //        //$('#TargetId' ).css('right', '0%');
    //    }
    //    else {
    //        alert('target else');
    //        //$('#TargetId').css('border-left', '5px solid #3177b4');
    //        //$('#TargetId').css('left', '0%');
    //    }
    //}
    //else {

    //}
    ////alert('ForeCastPercentage_Bar:'+ProgressBarTopModel.ForeCastPercentage_Bar);
    //if (ProgressBarTopModel.ForeCastPercentage_Bar >= 70) {
    //    //alert('Forecast is');
    //    $('#ForecastId').css('border-right', '5px solid #3177b4');
    //    $('#ForecastId').css('right', '0%');
    //}
    //else {
    //    //alert('Forecast else');
    //    $('#ForecastId').css('border-left', '5px solid #3177b4');
    //    $('#ForecastId').css('left', '0%');
    //}
    //alert(ProgressBarTopModel.AchievedPercentage_Bar);
    //if (ProgressBarTopModel.AchievedPercentage_Bar >= 70) {
    //    //alert('Achieved if');
    //    $('#AchievedId').css('border-right', '5px solid #3177b4');
    //    $('#AchievedId').css('right', '0%');
    //}
    //else {
    //    //alert('Achieved else');
    //    $('#AchievedId').css('border-left', '5px solid #3177b4');
    //    $('#AchievedId').css('left', '0%');
    //}
    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 22-10-2020


    //$('#TargetId').css('border-right','5px solid #3177b4');
    // alert('in PopulateProgressBarTop');
    //alert(ProgressBarTopModel.TargetValue);
    //alert(ProgressBarTopModel.TargetValue==="");
    TargetBar = document.getElementById("TargetBarID");
    //alert(ProgressBarTopModel.TargetPercentage_Bar);
    TargetBar.style["width"] = ProgressBarTopModel.TargetPercentage_Bar;
    //alert('ProgressBarTopModel.TargetValue.split()[1]' + ProgressBarTopModel.TargetValue.split(':')[1]);
    //alert('ProgressBarTopModel.AchievedValue.split()[1]' + ProgressBarTopModel.AchievedValue.split(':')[1]);
    //alert('ProgressBarTopModel.ForeCastValue.split()[1]' + ProgressBarTopModel.ForeCastValue.split(':')[1]);
    //alert("ProgressBarTopModel.TargetPercentage_Bar-"+ProgressBarTopModel.TargetPercentage_Bar);
    //alert("ProgressBarTopModel.TargetValue-" + ProgressBarTopModel.TargetValue);
    //alert("ProgressBarTopModel.ForeCastPercentage_Bar-" + ProgressBarTopModel.ForeCastPercentage_Bar);
    //alert("ProgressBarTopModel.ForeCastPercentage-" + ProgressBarTopModel.ForeCastPercentage);
    //alert("ProgressBarTopModel.AchievedPercentage_Bar-" + ProgressBarTopModel.AchievedPercentage_Bar);
    //alert("ProgressBarTopModel.AchievedPercentage-" + ProgressBarTopModel.AchievedPercentage);
    // BELOW CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR 
    //$("#TargetSpanId").text((ProgressBarTopModel.TargetValue === "" ? "0" : ProgressBarTopModel.TargetValue.split(':')[1]) + " (" + ProgressBarTopModel.TargetPercentage + ")");
    $("#TargetSpanId").text(ProgressBarTopModel.TargetPercentage);
    $("#TargetValueID").html("(" + (ProgressBarTopModel.TargetValue === "" ? "0.00 Cr" : ProgressBarTopModel.TargetValue.split(':')[1]) +" Cr)");
    // ABOVE CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR 

    //alert("Target val (" + ProgressBarTopModel.TargetValue+")");
    TargetAchievedBar = document.getElementById("ForecastBarID");
    //alert(ProgressBarTopModel.ForeCastPercentage_Bar);
    TargetAchievedBar.style["width"] = ProgressBarTopModel.ForeCastPercentage_Bar;
    // BELOW CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR 
    //$("#ForecastSpanId").text((ProgressBarTopModel.ForeCastValue === "" ? "0" : ProgressBarTopModel.ForeCastValue.split(':')[1]) + " (" + ProgressBarTopModel.ForeCastPercentage + ")");
    $("#ForecastSpanId").text(ProgressBarTopModel.ForeCastPercentage);
    $("#ForecastValueID").html("(" + (ProgressBarTopModel.ForeCastValue === "" ? "0.00 Cr" : ProgressBarTopModel.ForeCastValue.split(':')[1]) + " Cr)");

    // ABOVE CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR 


    TargetBar = document.getElementById("AchievedBarID");
    //alert(ProgressBarTopModel.AchievedPercentage_Bar);
    TargetBar.style["width"] = ProgressBarTopModel.AchievedPercentage_Bar;
    // BELOW CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR 
    //$("#AchievedSpanId").text((ProgressBarTopModel.AchievedValue === "" ? "0" : ProgressBarTopModel.AchievedValue.split(':')[1]) + " (" + ProgressBarTopModel.AchievedPercentage + ")");
    $("#AchievedSpanId").text(ProgressBarTopModel.AchievedPercentage);
    $("#AchievedValueID").html("(" + (ProgressBarTopModel.AchievedValue === "" ? "0.00 Cr" : ProgressBarTopModel.AchievedValue.split(':')[1]) + " Cr)");

    // ABOVE CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR 

    //alert(ProgressBarTopModel.AchievedValue);
    //alert(ProgressBarTopModel.TargetValue);
    //alert('ForeCastPercentage =' + ProgressBarTopModel.ForeCastPercentage);
    //alert('AchievedPercentage ='+ProgressBarTopModel.AchievedPercentage);
    //alert(ProgressBarTopModel.AchievedValue);
    //alert(ProgressBarTopModel.TargetValue);

    document.getElementById("TargetBarID").setAttribute("title", ProgressBarTopModel.TargetValue);
    document.getElementById("AchievedBarID").setAttribute("title", ProgressBarTopModel.AchievedValue);
    document.getElementById("ForecastBarID").setAttribute("title", ProgressBarTopModel.ForeCastValue);

    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
    if (IsCurrentFinYear) {
        //alert('in if');
        //$('#AchievedForecastTextID').text('Achieved and Forecast');
        $('#ForecastBarID').show();
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 20-10-2020
        $('#ForecastTextID').show();
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 20-10-2020
        // BELOW CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR
        $('#ForecastValueID').show();
        // ABOVE CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR 


        
    }
    else {
        //alert('in else');
        //$('#AchievedForecastTextID').text('Achieved');
        $('#ForecastBarID').hide();
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 20-10-2020
        $('#ForecastTextID').hide();
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 20-10-2020
        // BELOW CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR
        $('#ForecastValueID').hide();
        // ABOVE CODE IS COMMENTED & CHANGED BY SHUBHAM BHAGAT BY SHUBHAM BHAGAT ON 22-12-2020 AFTER DISCUSSION WITH SIR 
    }
    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 13-08-2020
}


function AnimateList($listItems, index, callback) {


    $listItems.eq(index).animate({ left: 0, opacity: 1 }, function () {
        AnimateList($listItems, index + 1, callback)
    });
}

function FadeLists($lists, index) {
    if (index >= $lists.length) index = 0;

    var $currentList = $lists.eq(index);
    $currentList.fadeIn(2000, function () {
        AnimateList($currentList.find("li"), 0, function () { FadeLists($lists, index + 1) });
    })
}

// ADDED BY SHUBHAM BHAGAT ON 17-09-2020
function EXCELRevenueTargetVsAchieved(toggleBtnId, selectedType, DistrictCode) {
    window.location.href = '/Dashboard/Dashboard/EXCELRevenueTargetVsAchieved?toggleBtnId=' + toggleBtnId + "&selectedType=" + selectedType + "&DistrictCode=" + DistrictCode + "&DistrictTextForExcel=" + DistrictTextForExcel;
}
// ADDED BY SHUBHAM BHAGAT ON 17-09-2020

//ADDED BY PANKAJ ON 17-09-2020
function EXCELProgressCurrentVsPreviousFinYear(toggleBtnId, selectedType, DistrictCode) {
    window.location.href = '/Dashboard/Dashboard/EXCELProgressCurrentVsPreviousFinYear?toggleBtnId=' + toggleBtnId + "&selectedType=" + selectedType + "&DistrictCode=" + DistrictCode + "&DistrictTextForExcel=" + DistrictTextForExcel;
}


//ADDED BY SHUBHAM BHAGAT ON 21-09-2020 FOR AVG_REG_TIME PANEL
function PopulateAvgRegTimeFun() {
    if (DistrictCode_ == "0") {

        if (selectedType == "F") {
            $("#AvgRegTimeHeadingId").text("Average Registration time for Fin Year " + $("#FinYearListId option:selected").text());
        }
        else if (selectedType == "M") {
            $("#AvgRegTimeHeadingId").text("Average Registration time for Current Month");


        }
        else if (selectedType == "D") {
            $("#AvgRegTimeHeadingId").text("Average Registration time as on today");

        }

    }
    else {
        if (selectedType == "F") {
            $("#AvgRegTimeHeadingId").text("Average Registration time for Fin Year " + $("#FinYearListId option:selected").text() + " Fin Year");

        }
        else if (selectedType == "M") {
            $("#AvgRegTimeHeadingId").text("Average Registration time for Current Month");


        }
        else if (selectedType == "D") {
            $("#AvgRegTimeHeadingId").text("Average Registration time as on today");

        }
    }

    blockUI('loading data.. please wait...');
    FinYear = $("#FinYearListId option:selected").val();

    $.ajax({
        type: "GET",
        url: "/Dashboard/Dashboard/PopulateAvgRegTime",
        data: { 'selectedType': selectedType, 'SelectedOffice': DistrictCode_, 'FinYearId': FinYear },
        success: function (data) {
            if (data.success) {

                fillAvgRegTime(data.TilesModel.Top3AvgRegTime, data.TilesModel.Bottom3AvgRegTime, data.TilesModel.AVG_REGISTRASTION_TIME_FYWISE);

            }
            else {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error Occured while processing request </span>'
                });
            }
            unBlockUI();

        },
        error: function () {
            bootbox.alert({
                //   size: 'small',
                //  title: "<span class=' boot-alert-title'><i class='fa fa-exclamation-triangle text-danger boot-icon boot-icon'></i>&nbsp;&nbsp;Error</span>",
                message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">Error in processing</span>'
            });
        }
    });
}

function fillAvgRegTime(Top3AvgRegTime, Bottom3AvgRegTime, AVG_REGISTRASTION_TIME_FYWISE) {
    $("#sectionAvgRegTimeTop").html(Top3AvgRegTime);
    $("#sectionAvgRegTimeBottom").html(Bottom3AvgRegTime);
    $("#OverAllRegTimeHeadingId").html(" : " + AVG_REGISTRASTION_TIME_FYWISE + " Minutes");


}



function ProgressBarChartMonthWise(label) {
   // alert('function:' + label)
    PopulateProgressChartMonthWise("2", "1", selectedType, DistrictCode_, label);
}

 // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020 
//Region for Progress CHART Month Wise
function PopulateProgressChartMonthWise(GraphId, toggleBtnId, selectedType, DistrictCode_, FinYear) {
    //alert("In PopulateProgressChart : " + "selectedType : " + selectedType + "DistrictCode_ : " + DistrictCode_);
    blockUI('loading data.. please wait...');
    $.ajax({
        url: "/Dashboard/Dashboard/PopulateProgressChartMonthWise",
        type: "POST",
        data: {
            'toggleBtnId': toggleBtnId, 'selectedType': selectedType, 'DistrictCode': DistrictCode_, 'FinYear': FinYear
        },

        success: function (jsonData) {

            if (toggleBtnId == 1) {
                $("#dv_2_ProgressChart_Wrapper").remove();
                //$("#dv_1_CurrentVsPrevFinYr_Wrapper").html('<div id="dv_2_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart" style="border-radius:10px;"><canvas id="cnvsProgressBarChart" style="height:300px; width: 807px;"></canvas></div></div></div>');
                $("#dv_1_CurrentVsPrevFinYr_Wrapper").html('<div id="dv_2_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart" style="border-radius:10px;"><canvas id="cnvsProgressBarChart" height="188"></canvas></div></div></div>');
                PopulateProgressChartDataSetMonthWise(jsonData);
                PopulatePregresschartMonthWise(jsonData);
                // ADDED BY PANKAJ ON 17-09-2020
                $("#EXCELCurrentVsPrevFinYearSpanID").html('');
                // ADDED BY PANKAJ ON 17-09-2020
            }
            if (toggleBtnId == 2) {
                $("#dv_2_Graph").remove();
                $("#dv_1_CurrentVsPrevFinYr_Wrapper").html('<div id="dv_2_ProgressChart_Wrapper"><table id="tbl_2_PregressChart" class="table table-striped table-bordered table-condensed table-hover" style="text-align:center !important; vertical-align: middle;background-color: aliceblue; overflow: hidden; border: solid;width: 50%;"></table></div>');
                FillDataTable(jsonData.TableColumns, jsonData.TableData, "tbl_2_PregressChart", "dv_2_ProgressChart_Wrapper")
                $("#dv_2_Table").show();
                $("#dv_2_Graph").hide();
                // ADDED BY PANKAJ ON 17-09-2020
                if (jsonData.TableData.length != undefined) {
                    $("#EXCELCurrentVsPrevFinYearSpanID").html('');
                    $("#EXCELCurrentVsPrevFinYearSpanID").html(jsonData.ExcelBtn);
                }
                // ADDED BY PANKAJ ON 17-09-2020
            }

            unBlockUI();
        },
        error: function (xhr, status, err) {
            //alert("in PopulateSurchargeCessBarChart");
            bootbox.alert("Error occured while proccessing your request : " + err);
            unBlockUI();

        }
    });




}

function PopulateProgressChartDataSetMonthWise(jsonData) {
    //alert('in PopulateProgressChartDataSet(jsonData) fadefhdsjkj');
    CurrentVsPrevFinYrMonthsPopUp = jsonData.BarChart.Months;
    Document_DataSet = {
      
        label: jsonData.BarChart.Lbl_Documents,
        data: jsonData.BarChart.Documents,
        yAxisID: "y-axis-0",
        backgroundColor: "#3C8DBC",
        borderColor: "#3C8DBC",
        borderCapStyle: 'butt',
        borderDash: [],
        borderDashOffset: 0.0,
        borderJoinStyle: 'miter',
        pointBorderColor: "rgba(75,192,192,1)",
        pointBackgroundColor: "#fff",
        pointBorderWidth: 1,
        pointHoverRadius: 5,
        pointHoverBackgroundColor: "rgba(75,192,192,1)",
        pointHoverBorderColor: "rgba(220,220,220,1)",
        pointHoverBorderWidth: 2,
        pointRadius: 5,
        pointHitRadius: 10,
        fill: false,
        // Changes this dataset to become a line
        type: 'line'
    },

        Revenue_DataSet =
        {
            label: jsonData.BarChart.Lbl_Revenue,
            data: jsonData.BarChart.Revenue,
            yAxisID: "y-axis-1",
            backgroundColor: [
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5",
                "#E5E5E5"
            ],
            borderColor: [
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)",
                "rgba(10,20,30,1)"

            ],
            borderWidth: 1,
            //// Changes this dataset to become a line
            //type: 'line'

        };

}

function PopulatePregresschartMonthWise(jsonData) {
    //alert('in PopulatePregresschart(jsonData)');
    //$('#CollapseSearchDiv').trigger('click');
    //bar chart data
    var ctx = document.getElementById("cnvsProgressBarChart");
    if (ProgressBarChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        ProgressBarChart.destroy();
    }
    var data = {
        labels: jsonData.BarChart.Months,
        datasets: [
            Document_DataSet,
            Revenue_DataSet
        ]
    };

    //options
    var options = {
        responsive: true,
        title: {
            display: true,
            position: "top",
            //text: "Bar Chart",
            fontSize: 0,
            fontColor: "#111"
        },
        legend: {
            display: true,
            position: "top",
            labels: {
                fontColor: "#333",
                fontSize: 12
            },
            onClick: function (event, legendItem) {
                //get the index of the clicked legend
                //alert('dfdfad');
                //var xLabel = this.scales['x-axis-0'].getValueForPixel(e.x);
                ////console.log(xLabel.format('MMM YYYY'));
                //alert("clicked x-axis area: " + xLabel);
                var index = legendItem.datasetIndex;
                //toggle chosen dataset's visibility
                ProgressBarChart.data.datasets[index].hidden =
                    !ProgressBarChart.data.datasets[index].hidden;
                //toggle the related labels' visibility
                ProgressBarChart.options.scales.yAxes[index].display =
                    !ProgressBarChart.options.scales.yAxes[index].display;
                ProgressBarChart.update();
            }

        },
        scales: {
            yAxes: [{
                stacked: false,
                position: "left",
                id: "y-axis-0",
                type: 'linear',
                ticks: {
                    beginAtZero: true,

                },
                scaleLabel: {
                    display: true,
                    labelString: 'No of Documents'
                },
            },

            {
                stacked: false,
                position: "right",
                id: "y-axis-1",

                type: 'linear',
                ticks: {
                    beginAtZero: true,

                },
                scaleLabel: {
                    display: true,
                    labelString: 'Revenue : Rupees in Crore'
                },
            }
            ]
        },
        layout: {
            padding: {
                left: 0,
                right: 0,
                top: 0,
                bottom: 0
            }
        },
        // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 13-10-2020
        onClick: function (e) {

            var activePoints = ProgressBarChart.getElementsAtEvent(e);
            if (activePoints[0]) {
                var chartData = activePoints[0]['_chart'].config.data;
                var idx = activePoints[0]['_index'];

                var label = chartData.labels[idx];
                //var value = chartData.datasets[0].data[idx];
                //var color = chartData.datasets[0].backgroundColor[idx]; //Or any other data you wish to take from the clicked slice

                //alert(label + ' ' + value + ' ' + color); //Or any other function you want to execute. I sent the data to the server, and used the response i got from the server to create a new chart in a Bootstrap modal.
                //alert(label + ' ' + value); //Or any other function you want to execute. I sent the data to the server, and used the response i got from the server to create a new chart in a Bootstrap modal.
               // alert(label); //Or any other function you want to execute. I sent the data to the server, and used the response i got from the server to create a new chart in a Bootstrap modal.
                PopulateProgressChart("2", "1", selectedType, DistrictCode_);
                IsMonthsProgressChartVisible = false;
               // alert('IsMonthsProgressChartVisible : ' + IsMonthsProgressChartVisible);
                // CALL MAIN METHOD TO POPULATE ALL FIN YEAR CHART
            }

        }
      
        // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 13-10-2020
    }
    //create Chart class object
    var ProgressBarChart = new Chart(ctx, {
        type: "bar",
        data: data,
        options: options
    });
       
    unBlockUI();


}



function EXCELProgressCurrentVsPreviousMonths(toggleBtnId, selectedType, DistrictCode, FinYear) {
    window.location.href = '/Dashboard/Dashboard/EXCELProgressCurrentVsPreviousMonths?toggleBtnId=' + toggleBtnId + "&selectedType=" + selectedType + "&DistrictCode=" + DistrictCode + "&DistrictTextForExcel=" + DistrictTextForExcel + "&FinYear=" + FinYear;
}


// ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020