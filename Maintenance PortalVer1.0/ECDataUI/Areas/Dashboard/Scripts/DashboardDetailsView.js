//Raman Kalegaonkar 16-02-2020

var SalesStatisticsLineChart;
var HighValPropLineChart;
var AgriGreaterThanTenLakhs_DataSet;
var AgriLessThanTenLakhs_DataSet;
var FaltsApartments_DataSet;
var Lease_DataSet;
var NonAgriGreaterThanTenLakhs_DataSet;
var NonAgriLessThanTenLakhs_DataSet;
var SurchargeCollected_DataSet;
var CessCollested_DataSet;
var OneLakhToTenLakhs_DataSet;
var TenLakhsToOneCrore_DataSet;
var OneCroreToFiveCrore_DataSet;
var FiveCroreToTenCrore_DataSet;
var AboveTenCrore_DataSet;
var SelectedValForSalesStatistics;
var SelectedValForHighValProp;
var Total_DataSet;
var FinYearForSaleRevenuePopUp;
var FinYearForSurchargeCessPopUp;
var SurchargeForPopUp;
var CessForPopUp;
var TotalForPopUp;
var FinYearForHighValPropPopUp;
//Added by RamanK on 22-06-2020
var FinYear;

// ADDED BY SHUBHAM BHAGAT ON 17-09-2020
var DistrictTextForExcel;
var SROTextForExcel;

//ADDED BY PANKAJ SAKHARE ON 06-10-2020 FOR GRAPH POPUP TEXT
var DistTxtForGraphPopup;
var SroTxtForGraphPopup;

$(document).ready(function () {
    //alert('tab 2');//sb
    //alert($('#FinYearListID').val());

    //$("#FinYearListID").change(function () {
    //    //alert("Hiii");
    //   $('#SearchBtn').trigger('click');

    //});
    //ADDED BY SHUBHAM BHAGAT 09-04-2020
    $('input[name=natureOfArticleTypeName][value=ALL]').prop('checked', true);
    $('input:radio[name="natureOfArticleTypeName"]').change(function () {
        //if ($(this).val() == 'ALL') {
        //    alert("You selected the first option and deselected the second one");       
        //} else {
        //    alert("You selected the second option and deselected the first one");
        //}
        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/Dashboard/Dashboard/NatureOfDocumentByRadioType',
            data: { "radioType": $(this).val() },
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
                    $('#ddArticleNameList').empty();
                    // alert('1');
                    $.each(data.natureOfDocument, function (i, natureOfDocument) {
                        //SROOfficeList
                        $('#ddArticleNameList').append('<option value="' + natureOfDocument.Value + '">' + natureOfDocument.Text + '</option>');
                    });
                    // alert('2');
                    $("#ddArticleNameList option:selected").removeAttr("selected");// working 1 //step 1
                    // alert('3');
                    $("#ddArticleNameList").multiselect('refresh');// working 1 //step 2
                    // alert('4');
                    var i;
                    for (i = 0; i < data.natureOfDocumentArr.length; ++i) {
                        // do something with `substr[i]`
                        //alert(data.natureOfDocumentArr[i]);
                        $('#ddArticleNameList  option').each(function () { //step 3 // delete > 
                            //alert($(this).text() + ' ' + $(this).val());
                            $("#ddArticleNameList option[value='" + data.natureOfDocumentArr[i] + "']").attr('selected', 'selected');
                        });
                    }
                    // alert('5');
                    $("#ddArticleNameList").multiselect('refresh'); //step 4
                    // alert('6');
                }
                unBlockUI();
                // alert('7');
            },
            error: function (xhr) {
                unBlockUI();
            }
        });

        //alert('1');

        //$("#ddArticleNameList option:selected").removeAttr("selected");// working 1 //step 1
        //alert('2');
        //$("#ddArticleNameList").multiselect('refresh');// working 1 //step 2
        //alert('3');
        //$('#ddArticleNameList  option').each(function () { //step 3 // delete > 
        //    //alert($(this).text() + ' ' + $(this).val());
        //    $("#ddArticleNameList option[value='" + $(this).val() + "']").attr('selected', 'selected');
        //});
        //alert('4');
        //$("#ddArticleNameList").multiselect('refresh'); //step 4


        //$.each($("#ddArticleNameList option:selected"), function () {
        //    //countries.push($(this).val());
        //    //alert($(this).val());
        //    //$(this).prop('selected', false); // <-- HERE
        //    //alert('dsfdsafsd');
        //    //$("#ddArticleNameList option[value= 106]").attr('selected', false);
        //    $(this).removeAttr("selected");
        //});
        //alert('2222');
        //$("#ddArticleNameList option:selected").removeAttr("selected");// working 1//step 1
        //$("#ddArticleNameList").multiselect('refresh');// working 1 //step 2
        //$("#ddArticleNameList option[value='" + myValue + "']").attr('selected', 'selected');

        //$('#ddArticleNameList > option').each(function () {
        //    //alert($(this).text() + ' ' + $(this).val());
        //    $("#ddArticleNameList option[value='" + $(this).val() + "']").attr('selected', 'selected');
        //});
        //$("#ddArticleNameList").multiselect('refresh');



        //$('#ddArticleNameList').multiSelect('deselect_all'); //error
        //$("#ddArticleNameList option:selected").each(function () {
        //    $(this).removeAttr("selected");
        //});
        //$("#ddArticleNameList option:selected").attr('selected', false);
        //$('#ddArticleNameList > option').each(function () {
        //    //alert($(this).text() + ' ' + $(this).val());
        //    if ()
        //        $("#ddArticleNameList option[value=3]").attr('selected', 'selected');
        //    //else
        //    //    $("#ddArticleNameList option[value=3]").attr('selected', false);
        //});
    });
    //ADDED BY SHUBHAM BHAGAT 09-04-2020

    $('#modChart').on('shown.bs.modal', function (event) {
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


        //var chart = Chart.Line(canvas, {
        //    responsive: true,
        //    labels: labels,
        //    datasets: [{
        //        fillColor: "rgba(151,187,205,0.2)",
        //        strokeColor: "rgba(151,187,205,1)",
        //        pointColor: "rgba(151,187,205,1)",
        //        pointStrokeColor: "#fff",
        //        pointHighlightFill: "#fff",
        //        pointHighlightStroke: "rgba(151,187,205,1)",
        //        data: source
        //    }, {
        //        fillColor: "rgba(220,220,220,0.2)",
        //        strokeColor: "#F7464A",
        //        pointColor: "#FF5A5E",
        //        pointStrokeColor: "#FF5A5E",
        //        pointHighlightFill: "#fff",
        //        pointHighlightStroke: "red",
        //        data: target
        //    }]
        //}

        //if (SalesStatisticsLineChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        //    SalesStatisticsLineChart.destroy();
        //}
        var data = {
            labels: FinYearForSaleRevenuePopUp,
            datasets: [

                NonAgriLessThanTenLakhs_DataSet,
                NonAgriGreaterThanTenLakhs_DataSet,
                AgriLessThanTenLakhs_DataSet,
                AgriGreaterThanTenLakhs_DataSet,
                FaltsApartments_DataSet,
                Lease_DataSet,
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

        ////ADDED BY PANKAJ SAKHARE ON 05-10-2020 FOR LABEL IN POPUP
        var opts = {
            scales: {
                yAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: ($('input[name="rdoDBSaleStatistics"]:checked').val() === 'Revenue') ? 'Rupees in Crore' : ''
                    }
                }]
            }
        };

        SalesStatisticsLineChart = Chart.Line(canvas, {
            data: data,
            //options: barChartOptions
            options: opts
        });



    }).on('hidden.bs.modal', function (event) {
        // reset canvas size
        var modal = $(this);
        var canvas = modal.find('.modal-body canvas');
        canvas.attr('width', '568px').attr('height', '300px');
        // destroy modal
        $(this).data('bs.modal', null);
    });

    $('#SurchargeCessPopUpId').on('shown.bs.modal', function (event) {
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

        //modal.find('.modal-title').html("Surcharge And Cess Collected");
        if (DistTxtForGraphPopup === undefined) {
            DistTxtForGraphPopup = "All";
        }
        if (SroTxtForGraphPopup === undefined) {
            SroTxtForGraphPopup = "All";
        }
        // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 22-12-2020
        //modal.find('.modal-title').html("Surcharge And Cess Collected" + " For District: " + DistTxtForGraphPopup + "&nbsp; &nbsp; SRO: " + SroTxtForGraphPopup);
        modal.find('.modal-title').html("Surcharge - Cess Collected" + " For District: " + DistTxtForGraphPopup + "&nbsp; &nbsp; SRO: " + SroTxtForGraphPopup);
        // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 22-12-2020

        //var ctx = canvas[0].getContext("2d");
        canvas.clientHeight = 2000;

        var ctx = document.getElementById("canvasSurchargeCessPopUp");
        //if (SurchargeCessBarChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        //    SurchargeCessBarChart.destroy();
        //}

        var data = {
            labels: FinYearForSurchargeCessPopUp,
            datasets: [
                {
                    label: "Surcharge",
                    backgroundColor: "#F56954",
                    data: SurchargeForPopUp,
                    //data: [100, 233, 45, 734,111]
                },
                {
                    label: "Cess",
                    backgroundColor: "#F39C12",
                    data: CessForPopUp,
                    //data: [200, 45, 23, 734, 111]
                },
                {
                    label: "Total",
                    backgroundColor: "#449D44",
                    data: TotalForPopUp,
                    //data: [300, 287, 23, 734, 111]
                }
                //SurchargeCollected_DataSet,
                //CessCollested_DataSet
            ]
        };

        

        //options
        var options = {
            responsive: true,
            title: {
                display: true,
                position: "top",
                //text: "Bar Chart",
                fontSize: 12,
                fontColor: "#111"
            },
            legend: {
                display: true,
                position: "top",
                labels: {
                    fontColor: "Black",
                    fontSize: 14
                }
            },
            //scales: {
            //    //yAxes: [{
            //    //    ticks: {
            //    //        min: 0
            //    //    }
            //    //}]
            //    xAxes: [{ stacked: true }],
            //    yAxes: [{ stacked: true }]
            //}
            scales: {
                //yAxes: [{
                //    ticks: {
                //        min: 0
                //    }
                //}]
                xAxes: [{ stacked: true }],
                //yAxes: [{ stacked: true }],
                //ADDED BY PANKAJ ON 21-09-2020 FOR LABEL ON Y-AXIS
                yAxes: [{
                    stacked: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Rupees in Crore'
                    }
                }]
            }
        }
        //create Chart class object
        var SurchargeCessBarChart = new Chart(ctx, {
            type: "bar",
            data: data,
            options: options
        });
        unBlockUI();




    }).on('hidden.bs.modal', function (event) {
        // reset canvas size
        var modal = $(this);
        var canvas = modal.find('.modal-body canvas');
        canvas.attr('width', '568px').attr('height', '300px');
        // destroy modal
        $(this).data('bs.modal', null);
    });

    $('#HighValuePopUpId').on('shown.bs.modal', function (event) {
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
        //modal.find('.modal-title').html("High Value Properties");
        //var ctx = canvas[0].getContext("2d");
        canvas.clientHeight = 2000;
        //console.log(canvas);
        var canvas = document.getElementById('canvasHighValuePopUp');
        //if (HighValPropLineChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        //    HighValPropLineChart.destroy();
        //}

        var data = {
            
            labels: FinYearForHighValPropPopUp,
            datasets: [

                OneLakhToTenLakhs_DataSet,
                TenLakhsToOneCrore_DataSet,
                OneCroreToFiveCrore_DataSet,
                FiveCroreToTenCrore_DataSet,
                AboveTenCrore_DataSet

            ]

        };

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

        ////ADDED BY PANKAJ SAKHARE ON 05-10-2020 FOR LABEL IN POPUP
        var optsHighValProp = {
            scales: {
                yAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: ($('input[name="rdoDBHighValueProp"]:checked').val() === 'Revenue') ? 'Rupees in Crore' : ''
                    }
                }]
            }
        };

        HighValPropLineChart = Chart.Line(canvas, {
            data: data,
            //options: barChartOptions
            options: optsHighValProp
        });



    }).on('hidden.bs.modal', function (event) {
        // reset canvas size
        var modal = $(this);
        var canvas = modal.find('.modal-body canvas');
        canvas.attr('width', '568px').attr('height', '300px');
        // destroy modal
        $(this).data('bs.modal', null);
    });

    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);
    });
    //$('#DashboardSummaryID').click();
    //$('#DashboardSummaryID').addClass('active');
    $("#ddArticleNameList").multiselect({
        includeSelectAllOption: true
    });
    $('.multiselect').addClass('minimal');
    $('.multiselect').addClass('btn-sm');
    //LoadAllCharts();

    //$('#DtlsToggleIconSearchPara').trigger('click');
    $('#DROOfficeListID').change(function () {

        blockUI('loading data.. please wait...');
        $.ajax({
            url: '/Dashboard/Dashboard/GetSROOfficeListByDistrictID',
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
    //$('#SROOfficeListID').change(function () {
    //    LoadAllCharts();
    //});

    $("#SearchBtn").click(function () {
        //$('#DtlsToggleIconSearchPara').trigger('click');
        LoadAllCharts();
        if ($.fn.DataTable.isDataTable("#DashboardSummaryTblID")) {
            //alert("DashboardSummaryTable Exists");
            $("#DashboardSummaryTable").DataTable().clear().destroy();
        }
        $('#IdTDCurrentFinYr').html($("#FinYearListID option:selected").text() + " Fin. Year");
        $('#IdTDPrevFinYr').html(($("#FinYearListID option:selected").val() - 1) + "-" + (($("#FinYearListID option:selected").val() % 2000)) + " Fin. Year");
        $('#IdCumulative').html("Upto " + $("#FinYearListID option:selected").text() + " Fin. Year");

        SroID = $("#SROOfficeListID option:selected").val();
        SelectedSROText = $("#SROOfficeListID option:selected").text();
        SelectedDistrictText = $("#DROOfficeListID option:selected").text();
        DistrictID = $("#DROOfficeListID option:selected").val();
        SelectedNatureOfDocText = $("#ddArticleNameList option:selected").text();
        NatureOfDocID = $("#ddArticleNameList").val();
        var NatureOfDocIdJoined = NatureOfDocID.join();

        //alert(NatureOfDocIdJoined);
        //$('#SummaryRptFilterId').html("( Fin Year : " + $("#FinYearListID option:selected").text() + ", District : " + SelectedDistrictText + ",  SRO : " + SelectedSROText + " )");
        //ADDED BY PANKAJ ON 06-10-2020 FOR GRAPH POPUP HEADING
        $('#SummaryRptFilterId').html("( District : " + SelectedDistrictText + ",  SRO : " + SelectedSROText + " )");
        DistTxtForGraphPopup = $("#DROOfficeListID option:selected").text();
        SroTxtForGraphPopup = $("#SROOfficeListID option:selected").text();
       

        SaleStatisticsRdoClick();
        HighValPropRdoClick();
        $('#spanSalesStatistics').html("( District : " + SelectedDistrictText + ",  SRO : " + SelectedSROText + " )");
        $('#spanSurchargeAndCess').html("( District : " + SelectedDistrictText + ",  SRO : " + SelectedSROText + " )");
        $('#spanHighValProp').html("( District : " + SelectedDistrictText + ",  SRO : " + SelectedSROText + " )");
        

        FinYear = $("#FinYearListID option:selected").val();

        var DashboardSummaryTable = $('#DashboardSummaryTblID').DataTable({
            ajax: {
                url: '/Dashboard/Dashboard/LoadDashboardSumaryTable',
                type: "POST",
                data: {
                    'SroID': SroID, 'DistrictID': DistrictID, 'NatureOfDocID': NatureOfDocIdJoined, 'FinYearId': FinYear
                },
                dataSrc: function (json) {
                    //alert('2');
                    //unBlockUI();
                    if (json.errorMessage != null) {
                        bootbox.alert({
                            message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + json.errorMessage + '</span>',
                            callback: function () {
                                if (json.serverError != undefined) {
                                    window.location.href = "/Home/HomePage"
                                }
                                else {
                                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                    if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                        $('#DtlsSearchParaListCollapse').trigger('click');
                                    $("#DashboardSummaryTblID").DataTable().clear().destroy();
                                    $("#PDFSPANID").html('');
                                    $("#EXCELSPANID").html('');
                                }
                            }
                        });
                    }
                    else {
                        //alert("In Else");
                        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                        //alert(classToRemove);
                        if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x") {
                            //alert("It Is Plus***************************************************************8")
                            $('#DtlsSearchParaListCollapse').trigger('click');
                        }
                    }
                    unBlockUI();
                    return json.data;
                },
                error: function () {
                    //unBlockUI();                   
                    bootbox.alert({
                        message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                        callback: function () {
                        }
                    });
                    //$.unblockUI();
                    unBlockUI();
                },
                beforeSend: function () {
                    //alert('1');
                    blockUI('loading data.. please wait...');

                    var searchString = $('#DashboardSummaryTblID_filter input').val();
                    if (searchString != "") {
                        var regexToMatch = /^[^<>]+$/;
                        if (!regexToMatch.test(searchString)) {
                            $("#DashboardSummaryTblID_filter input").prop("disabled", true);
                            bootbox.alert('Please enter valid Search String ', function () {
                                //    $('#menuDetailsListTable_filter input').val('');
                                DashboardSummaryTable.search('').draw();
                                $("#DashboardSummaryTblID_filter input").prop("disabled", false);
                            });
                            unBlockUI();
                            return false;
                        }
                    }
                }
            },
            serverSide: true,
            // pageLength: 100,
            //"scrollX": true,
            //"scrollY": true,
            //"scrollY": "300px",
            "scrollCollapse": true,
            bPaginate: false,
            bLengthChange: true,
            // "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
            // "pageLength": -1,
            //sScrollXInner: "150%",
            bInfo: false,
            info: false,
            bFilter: false,
            searching: true,
            // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
            // TO HIDE 5 DATA TABLE BUTTON BY DEFAULT
            //dom: 'lBfrtip',
            "destroy": true,
            "bAutoWidth": true,
            "bScrollAutoCss": true,
            //buttons: [
            //    {
            //        extend: 'pdf',
            //        text: '<i class="fa fa-file-pdf-o"></i> PDF',
            //        exportOptions: {
            //            columns: ':not(.no-print)'
            //        },
            //        action:
            //            function (e, dt, node, config) {
            //                //this.disable();
            //                window.location = '/MISReports/SaleDeedRevCollection/ExportReportToPDF?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
            //            }
            //    },
            //    {**
            //        extend: 'excel',
            //        text: '<i class="fa fa-file-pdf-o"></i> Excel',
            //        exportOptions: {
            //            columns: ':not(.no-print)'
            //        },
            //        action:
            //            function (e, dt, node, config) {
            //                // BECAUSE OF BELOW CODE EXCEL BUTTON IS WORKING ONLY ONE TIME IF WE COMMENT 
            //                // BELOW this.disable(); so it will start working many times
            //                this.disable();
            //                window.location = '/MISReports/SaleDeedRevCollection/ExportToExcel?SROOfficeListID=' + SROOfficeListID + "&DROfficeID=" + DROfficeID + "&FinancialID=" + FinancialID
            //            }
            //    }
            //],

            //columnDefs: [{
            //    targets: [5],
            //    render: $.fn.dataTable.render.number(',', '.', 2)
            //}],
            columnDefs: [
                //{ orderable: false, targets: [2] },
                //{ orderable: false, targets: [4] },
                //{
                //    orderable: false, targets: [5]

                //},    
                //{ orderable: false, targets: [6] },
                //{ orderable: false, targets: [7] }
                { "width": "18%", "targets": 0 },
                { "width": "11%", "targets": 1 },
                { "width": "11%", "targets": 2 },
                { "width": "11%", "targets": 3 },
                { "width": "11%", "targets": 4 },
                { "width": "12%", "targets": 5 },
                { "width": "12%", "targets": 6 },
                { "width": "14%", "targets": 7 }
                //{ "width": "20%", "targets": 0 }

            ],
            //"language": {
            //    "decimal": ",",
            //    "thousands": "."
            //},
            columns: [

                { data: "Description", "searchable": true, "visible": true, "name": "Description" },
                { data: "Today", "searchable": true, "visible": true, "name": "Today" },
                { data: "Yesterday", "searchable": true, "visible": true, "name": "Yesterday" },
                { data: "CurrentMonth", "searchable": true, "visible": true, "name": "CurrentMonth" },
                { data: "PreviousMonth", "searchable": true, "visible": true, "name": "PreviousMonth" },
                { data: "CurrentFinYear", "searchable": true, "visible": true, "name": "CurrentFinYear" },
                { data: "PrevFinYear", "searchable": true, "visible": true, "name": "PrevFinYear" },
                { data: "UptoCurrentFinYear", "searchable": true, "visible": true, "name": "UptoCurrentFinYear" },
                //{ data: "UptoPrevFinYear", "searchable": true, "visible": true, "name": "UptoPrevFinYear" }

            ],
            fnInitComplete: function (oSettings, json) {
                //alert('3:fnInitComplete');
                //alert('in fnInitComplete');
                //$("#PDFSPANID").html(json.PDFDownloadBtn);
                $("#EXCELSPANID").html(json.ExcelDownloadBtn);
                //ADDED BY PANKAJ SAKHARE ON 18-09-2020
                $("#ExcelDivId").html(json.ExcelBtn);
                DistrictTextForExcel = $('#DROOfficeListID option:selected').text();
                SROTextForExcel = $('#SRODropDownListID option:selected').text();

            },
            preDrawCallback: function () {
                //alert('4:preDrawCallback');
                 // below code commented by shubham bhagat on 11-09-2020  
                //unBlockUI();
            },
            //fnRowCallback: function (nRow, aData, iDisplayIndex) {
            //    unBlockUI();
            //    return nRow;
            //},
            //"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            //    if (aData[3] == "5") {
            //        $('td', nRow).css('background-color', 'Red');
            //    }
            //    else if (aData[3] == "4") {
            //        $('td', nRow).css('background-color', 'Orange');
            //    }
            //},
            "fnRowCallback": function (nRow, row, data, index) {
                //alert(data);
                //$(row).find('td:eq(0)').css('backgroung', 'red');
                $('td:eq(0)', nRow).css('background-color', '#E8E8E8');
                $('td:eq(0)', nRow).css('color', 'black');
                $('td:eq(0)', nRow).css('border-right', '1px solid #73a9d9');
                $('td:eq(2)', nRow).css('border-right', '1px solid #73a9d9');
                $('td:eq(4)', nRow).css('border-right', '1px solid #73a9d9');
                $('td:eq(6)', nRow).css('border-right', '1px solid #73a9d9');
                $('td:eq(8)', nRow).css('border-right', '1px solid #73a9d9');
                $('td:eq(0)', nRow).css('font-weight', 'bold');

                //$('td:eq(1)', nRow).css('border', '0.5px solid black');
                //$('td:eq(2)', nRow).css('border', '0.5px solid black');
                //$('td:eq(3)', nRow).css('border', '0.5px solid black');
                //$('td:eq(4)', nRow).css('border', '0.5px solid black');
                //$('td:eq(5)', nRow).css('border', '0.5px solid black');
                //$('td:eq(6)', nRow).css('border', '0.5px solid black');
                //$('td:eq(7)', nRow).css('border', '0.5px solid black');
                //$('td:eq(8)', nRow).css('border', '0.5px solid black');

                $('td:eq(1)', nRow).css('background-color', '#E0EFF7');
                $('td:eq(2)', nRow).css('background-color', '#E0EFF7');
                $('td:eq(3)', nRow).css('background-color', '#C1DFEF');
                $('td:eq(4)', nRow).css('background-color', '#C1DFEF');

                $('td:eq(5)', nRow).css('background-color', '#A0C8E6');
                $('td:eq(6)', nRow).css('background-color', '#A0C8E6');
                $('td:eq(7)', nRow).css('background-color', '#7EB3DC');
                $('td:eq(8)', nRow).css('background-color', '#7EB3DC');


            },
            drawCallback: function (oSettings) {
                //responsiveHelper.respond();
                //alert('5:drawCallback');
                // below code commented by shubham bhagat on 11-09-2020                
                //unBlockUI();
            },
        });



        //tableIndexReports.columns.adjust().draw();

        
        DashboardSummaryTable.columns.adjust().draw();



    });


    $(".rdoToggleChartTable").change(function () {
        var _val = $(this).val();
        var splittedArray = _val.split("_");
        //alert(splittedArray[0] + splittedArray[1]);
        ToggleGraphAndTable(splittedArray[0], splittedArray[1])
    });

    $('#SearchBtn').trigger('click');

    //  $('#dv_1_Graph').show();

    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-08-2020 AT 10:23 AM
    $('#FinYearListDivID').hide();
    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-08-2020 AT 10:23 AM
});

function SaleStatisticsRdoClick() {
    
    // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
    $("#EXCELSaleStatisticsSpanID").html('');

    SelectedValForSalesStatistics = $('input[name="rdoDBSaleStatistics"]:checked').val();
    //alert(SelectedValForSalesStatistics);
    if (SelectedValForSalesStatistics == "DocRegistered") {
        //LoadDocumentRegisteredChart("4", "1");
        //LoadRevenueCollectedChart("1", "1");
        $('input:radio[name=options][value=4_1]').trigger('click');
        //COMMENTED AND ADDED BY PANKAJ ON 05-10-2020 FOR GRAPH POPUP HEADING
        //$('#TitleSaleStatId').html("Sale Statistics ( Document Registered )");
        if (DistTxtForGraphPopup === undefined) {
            DistTxtForGraphPopup = "All";
        }
        if (SroTxtForGraphPopup === undefined) {
            SroTxtForGraphPopup = "All";
        }
        $('#TitleSaleStatId').html("Sale Statistics ( Document Registered )" + " For District: " + DistTxtForGraphPopup + "&nbsp; &nbsp; SRO: " + SroTxtForGraphPopup);

    }
    else {
        $('input:radio[name=options][value=1_1]').trigger('click');
         //COMMENTED AND ADDED BY PANKAJ ON 05-10-2020 FOR GRAPH POPUP HEADING
        //$('#TitleSaleStatId').html("Sale Statistics ( Revenue Collected )");
        if (DistTxtForGraphPopup === undefined) {
            DistTxtForGraphPopup = "All";
        }
        if (SroTxtForGraphPopup === undefined) {
            SroTxtForGraphPopup = "All";
        }
        $('#TitleSaleStatId').html("Sale Statistics ( Revenue Collected )" + " For District: " + DistTxtForGraphPopup + "&nbsp; &nbsp; SRO: " + SroTxtForGraphPopup);
        //LoadRevenueCollectedChart("1", "1");
    }
    $('#SalesStatMaxBtnId').show();

}
function HighValPropRdoClick() {
    SelectedValForHighValProp = $('input[name="rdoDBHighValueProp"]:checked').val();
    //alert("SelectedValForHighValProp: " + SelectedValForHighValProp + " and DocRegistered");

    if (SelectedValForHighValProp == "DocRegistered") {
        //ADDED AND COMMENTED BY PANKAJ SAKHARE ON 06-10-2020 FOR GRAPH POPUP
        //$('#TitleHighValPropId').html("High Value Properties ( Document Registered )");
        var localVar;
        if (DistTxtForGraphPopup === undefined) {
            DistTxtForGraphPopup = "All";
        }
        if (SroTxtForGraphPopup === undefined) {
            SroTxtForGraphPopup = "All";
        }
       
        $('#TitleHighValPropId').html("High Value Properties ( Document Registered )" + " For District: " + DistTxtForGraphPopup + "&nbsp; &nbsp; SRO: " + SroTxtForGraphPopup);

        //alert("In Document Registered High Val Prop RDO BUTTON CHANGE");
        //LoadHighValPropChartDataForDocs("5", "1");
        $('input:radio[name=options][value=5_1]').trigger('click');

        //ToggleGraphAndTable("5", "1");
    }
    else {
         //ADDED AND COMMENTED BY PANKAJ SAKHARE ON 06-10-2020 FOR GRAPH POPUP
        //$('#TitleHighValPropId').html("High Value Properties ( Revenue Collected )");
        if (DistTxtForGraphPopup === undefined) {
            DistTxtForGraphPopup = "All";
        }
        if (SroTxtForGraphPopup === undefined) {
            SroTxtForGraphPopup = "All";
        }
        $('#TitleHighValPropId').html("High Value Properties ( Revenue Collected )" + " For District: " + DistTxtForGraphPopup + "&nbsp; &nbsp; SRO: " + SroTxtForGraphPopup);


        //alert("In Revenue Collected RDO BUTTON CHANGE");
        //LoadHighValPropChart("3", "1");
        $('input:radio[name=options][value=3_1]').trigger('click');

        //ToggleGraphAndTable("3", "1");
    }
    $('#HighValPropMaxBtnId').show();

}
//first function to be called on load
function LoadAllCharts() {
  
    blockUI('loading data.. please wait...');
    $('input:radio[name=rdoDBSaleStatistics][value=Revenue]').trigger('click');
    $('input:radio[name=rdoDBHighValueProp][value=Revenue]').trigger('click');

    //LoadRevenueCollectedChart("1", "1");
    PopulateSurchargeCessBarChart("2", "1");
    LoadHighValPropChart("3", "1");

}


//On change of graph and table toggle
function ToggleGraphAndTable(GraphId, toggleBtnId) {
    //alert(GraphId+toggleBtnId);
    //alert("GraphId " + GraphId);
    //alert("toggleBtnId " + toggleBtnId);
    switch (GraphId) {
        case "1":
            if (toggleBtnId == "2") {
                $('#SalesStatMaxBtnId').hide();
            }
            else {
                $('#SalesStatMaxBtnId').show();

            }
            LoadRevenueCollectedChart(GraphId, toggleBtnId);
            break;

        case "2":
            if (toggleBtnId == "2") {
                $('#SurchargeCessMaxBtnId').hide();
            }
            else {
                $('#SurchargeCessMaxBtnId').show();

            }
            PopulateSurchargeCessBarChart(GraphId, toggleBtnId);
            break;

        case "3":
            if (toggleBtnId == "2") {
                $('#HighValPropMaxBtnId').hide();
            }
            else {
                $('#HighValPropMaxBtnId').show();

            }
            LoadHighValPropChart(GraphId, toggleBtnId);
            break;

        case "4":
            if (toggleBtnId == "2") {
                $('#SalesStatMaxBtnId').hide();
            }
            else {
                $('#SalesStatMaxBtnId').show();

            }
            LoadDocumentRegisteredChart(GraphId, toggleBtnId);
            break;
        case "5":
            //alert("High Val Docs Regd");
            if (toggleBtnId == "2") {
                $('#HighValPropMaxBtnId').hide();
            }
            else {
                $('#HighValPropMaxBtnId').show();

            }
            LoadHighValPropChartDataForDocs(GraphId, toggleBtnId);
            break;

        default:
    }
}

//region for Revenue collected chart
function LoadRevenueCollectedChart(GraphId, toggleBtnId) {


    $('#IDRdoDocRegistered').hide();
    $('#IDRdoRevenueCollected').show();
    //$('#IDRdoRevenueCollected').hide();
    //blockUI('loading data.. please wait...');
    $.ajax({
        url: "/Dashboard/Dashboard/LoadRevenueCollectedChartData",
        type: "POST",
        // data: $("#DashboardDetailsFormId").serialize(),
        data: { 'toggleBtnId': toggleBtnId, 'DistrictCode': $("#DROOfficeListID option:selected").val(), SROCode: $("#SROOfficeListID option:selected").val() },

        success: function (jsonData) {

            if (toggleBtnId == 1) {

                $("#dv_1_Table").remove();
                $("#dv_1_Wrapper").html('<div id="dv_1_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart"><canvas id="cnvsSalesStatisticsLineChart" style="height:300px; width: 807px;"></canvas></div></div></div>');

                PopulateLineChartDataSet(jsonData);

                 // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                $("#EXCELSaleStatisticsSpanID").html('');

                // $("#dv_1_Graph").show();
            }


            if (toggleBtnId == 2) {
                //$('input:radio[name=options][value=1_2]').trigger('click');


                $("#dv_1_Graph").remove();

                $("#dv_1_Wrapper").html('<div id="dv_1_Table"> <div id= "dv_1_RevenueCollected_Wrapper"> </div> </div>');

                FillDataTableDetails(jsonData.TableColumns, jsonData.TableData, "tbl_1_RevenueCollected", "dv_1_RevenueCollected_Wrapper")

                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                if (jsonData.TableData.length != undefined) {
                    $("#EXCELSaleStatisticsSpanID").html('');
                    $("#EXCELSaleStatisticsSpanID").html(jsonData.ExcelBtn);
                }
                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
            }

            // below code commented and changed from "$.unblockUI();" to "unBlockUI();" of unblock by shubham bhagat on 11-09-2020
            //$.unblockUI();
            // below code commented by shubham bhagat on 11-09-2020  
            //unBlockUI();


        },
        error: function (xhr, status, err) {
            //alert("in LoadRevenueCollectedChartData");
            bootbox.alert("Error occured while proccessing your request : " + err);

            //$.unblockUI();
            unBlockUI();
        }
    });
}

//region for Document Registered chart
function LoadDocumentRegisteredChart(GraphId, toggleBtnId) {
    $('#IDRdoRevenueCollected').hide();
    $('#IDRdoDocRegistered').show();
    //alert("In Document Registeredddd");
    //blockUI('loading data.. please wait...');
    $.ajax({
        //url: "/Dashboard/Dashboard/LoadRevenueCollectedChartData",
        url: "/Dashboard/Dashboard/LoadDocumentRegisteredChartData",
        type: "POST",
        // data: $("#DashboardDetailsFormId").serialize(),
        data: { 'toggleBtnId': toggleBtnId, 'DistrictCode': $("#DROOfficeListID option:selected").val(), SROCode: $("#SROOfficeListID option:selected").val() },

        success: function (jsonData) {
            if (toggleBtnId == 1) {
                $("#dv_1_Table").remove();
                $("#dv_1_Graph").remove();
                $("#dv_1_Wrapper").html('<div id="dv_1_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart"><canvas id="cnvsSalesStatisticsLineChart" style="height:300px; width: 807px;"></canvas></div></div></div>');
                $("#SalesSatatRevenueModalId").html('<div id="dv_1_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart"><canvas id="cnvsSalesStatisticsLineChart" style="height:300px; width: 807px;"></canvas></div></div></div>');

                PopulateLineChartDataSet(jsonData);

                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                $("#EXCELSaleStatisticsSpanID").html('');

                // $("#dv_1_Graph").show();
            }


            if (toggleBtnId == 2) {

                $("#dv_1_Graph").remove();

                $("#dv_1_Wrapper").html('<div id="dv_1_Table"> <div id= "dv_1_RevenueCollected_Wrapper"> </div> </div>');

                FillDataTableDetails(jsonData.TableColumns, jsonData.TableData, "tbl_1_RevenueCollected", "dv_1_RevenueCollected_Wrapper")

                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                if (jsonData.TableData.length != undefined) {
                    $("#EXCELSaleStatisticsSpanID").html('');
                    $("#EXCELSaleStatisticsSpanID").html(jsonData.ExcelBtn);
                }
                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
            }


           // below code commented and changed from "$.unblockUI();" to "unBlockUI();" of unblock by shubham bhagat on 11-09-2020
            //$.unblockUI();
            // below code commented by shubham bhagat on 11-09-2020  
            //unBlockUI();


        },
        error: function (xhr, status, err) {
            //alert("in LoadRevenueCollectedChartData");
            bootbox.alert("Error occured while proccessing your request : " + err);

            //$.unblockUI();
            unBlockUI();
        }
    });

}
function PopulateLineChartDataSet(jsonData) {
    //alert(jsonData.LineChart.AgriGreaterThanTenLakhs[4]);
   // alert(jsonData.LineChart.Lbl_AgriGreaterThanTenLakhs);
    AgriGreaterThanTenLakhs_DataSet =
        {
            label: jsonData.LineChart.Lbl_AgriGreaterThanTenLakhs,
            // lineTension: 0.1,
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
            data: jsonData.LineChart.AgriGreaterThanTenLakhs,
        };

    AgriLessThanTenLakhs_DataSet =
        {
            label: jsonData.LineChart.Lbl_AgriLessThanTenLakhs,
            //lineTension: 0.1,
            backgroundColor: "#F56954",
            borderColor: "#F56954",
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
            data: jsonData.LineChart.AgriLessThanTenLakhs,
        };

    FaltsApartments_DataSet =
        {
            label: jsonData.LineChart.Lbl_FaltsApartments,
            //lineTension: 0.1,
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
            data: jsonData.LineChart.FaltsApartments,
        };


    Lease_DataSet =
        {
            label: jsonData.LineChart.Lbl_Lease,
            // lineTension: 0.1,
            backgroundColor: "#3ed685",
            borderColor: "#3ed685",
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
            data: jsonData.LineChart.Lease,
        };

    NonAgriGreaterThanTenLakhs_DataSet =
        {
            label: jsonData.LineChart.Lbl_NonAgriGreaterThanTenLakhs,
            //lineTension: 0.1,
            backgroundColor: "#A52A2A",
            borderColor: "#A52A2A",
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
            data: jsonData.LineChart.NonAgriGreaterThanTenLakhs,
        };

    NonAgriLessThanTenLakhs_DataSet =
        {
            label: jsonData.LineChart.Lbl_NonAgriLessThanTenLakhs,
            //lineTension: 0.1,
            backgroundColor: "#b03dd3",
            borderColor: "#b03dd3",
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
            data: jsonData.LineChart.NonAgriLessThanTenLakhs,
        };
    FinYearForSaleRevenuePopUp = jsonData.LineChart.FinYear;

    ReloadSalesStatisticsLineChart(jsonData.LineChart.FinYear);


}
function ReloadSalesStatisticsLineChart(FinYear) {
    //alert('ReloadSalesStatisticsLineChart');
    var canvas = document.getElementById('cnvsSalesStatisticsLineChart');
    if (SalesStatisticsLineChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        SalesStatisticsLineChart.destroy();
    }
    var data = {
        labels: FinYear,
        datasets: [

            NonAgriLessThanTenLakhs_DataSet,
            NonAgriGreaterThanTenLakhs_DataSet,
            AgriLessThanTenLakhs_DataSet,
            AgriGreaterThanTenLakhs_DataSet,
            FaltsApartments_DataSet,
            Lease_DataSet,
        ]

    };

    //ADDED BY PANKAJ ON 21-09-2020 FOR LABEL ON Y-AXIS
    var SalesChartOption = {
        scales: {
            yAxes: [{
                scaleLabel: {
                    display: true,
                    labelString: ($('input[name="rdoDBSaleStatistics"]:checked').val() === 'Revenue') ? 'Rupees in Crore' : ''
                }
            }]
        }
    }
    //var SalesStasticsYaxisRadioText = $('input[name="rdoDBSaleStatistics"]:checked').val();
    //alert(SalesStasticsYaxisRadioText === 'Revenue');
    //ADDED BY PANKAJ ON 21-09-2020 FOR LABEL ON Y-AXIS


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

    SalesStatisticsLineChart = Chart.Line(canvas, {
        data: data,
        //options: barChartOptions
        //ADDED BY PANKAJ ON 21-09-2020 FOR LABEL ON Y-AXIS

        options: SalesChartOption
    });

}

//Region for SurchargeCessBarChart
function PopulateSurchargeCessBarChart(GraphId, toggleBtnId) {
    //blockUI('loading data.. please wait...');
    NatureOfDocID = $("#ddArticleNameList").val();
    var NatureOfDocIdJoined = NatureOfDocID.join();
    $.ajax({
        url: "/Dashboard/Dashboard/PopulateSurchargeCessBarChart",
        type: "POST",
        //data: { 'toggleBtnId': toggleBtnId , 'DistrictCode': $('#DROOfficeListID').val(), SROCode: $('#SROOfficeListID').val() },
        data: { 'toggleBtnId': toggleBtnId, 'DistrictCode': $("#DROOfficeListID option:selected").val(), 'SROCode': $("#SROOfficeListID option:selected").val(), 'NatureOfDoc': NatureOfDocIdJoined },

        success: function (jsonData) {

            if (toggleBtnId == 1) {
                $("#dv_2_Table").remove();
                $("#dv_2_Wrapper").html('<div id="dv_2_Graph" style="padding- top:2%;">    <div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart" style="border-radius:10px;"><canvas id="cnvsBarChart" style="height:500px !important;"></canvas></div></div></div>');
                //$("#cnvsBarChart").css('height', '1000px');
                //        $('td', nRow).css('background-color', 'Orange');

                PopulateSurchargeCessBarChartDataSet(jsonData);
                //ADDED BY PANKAJ SAKHARE ON 18-09-2020
                $("#EXCELSurchargeCessSpanId").html('');
            }
            if (toggleBtnId == 2) {
                //alert(jsonData.TableColumns);
                //alert(jsonData.TableData);
                $("#dv_2_Graph").remove();
                $("#dv_2_Wrapper").html('<div id="dv_2_Table"><div id="dv_2_BarChart_Wrapper"></div></div>');
                FillDataTableDetailsForSurchargeAndCess(jsonData.TableColumns, jsonData.TableData, "tbl_2_BarChart", "dv_2_BarChart_Wrapper")
                //ADDED BY PANKAJ SAKHARE ON 18-09-2020
                $("#EXCELSurchargeCessSpanId").html('');
                $("#EXCELSurchargeCessSpanId").html(jsonData.ExcelBtn);
            }
           // below code commented and changed from "$.unblockUI();" to "unBlockUI();" of unblock by shubham bhagat on 11-09-2020
            //$.unblockUI();
            // below code commented by shubham bhagat on 11-09-2020  
            //unBlockUI();
        },
        error: function (xhr, status, err) {
            //alert("in PopulateSurchargeCessBarChart");
            bootbox.alert("Error occured while proccessing your request : " + err);
            //$.unblockUI();
            unBlockUI();
        }
    });

   // below code commented and changed from "$.unblockUI();" to "unBlockUI();" of unblock by shubham bhagat on 11-09-2020
   //$.unblockUI();
   // below code commented by shubham bhagat on 11-09-2020  
   //unBlockUI();



}
function PopulateSurchargeCessBarChartDataSet(jsonData) {
    //alert(jsonData.BarChart.CessCollested);
    SurchargeCollected_DataSet = {
        label: jsonData.BarChart.Lbl_CessCollected,
        data: jsonData.BarChart.CessCollested,

        backgroundColor: [
            "#3C8DBC",
            "#3C8DBC",
            "#3C8DBC",
            "#3C8DBC",
            "#3C8DBC",
            "#3C8DBC",
            "#3C8DBC",
            "#3C8DBC"

        ],
        borderColor: [
            "rgba(30,20,10,1)",
            "rgba(10,20,30,1)",
            "rgba(10,20,30,1)",
            "rgba(10,20,30,1)",
            "rgba(10,20,30,1)",
            "rgba(10,20,30,1)",
            "rgba(10,20,30,1)",
            "rgba(10,20,30,1)"
        ],

        borderWidth: 1
    },

        Total_DataSet =
        {
            label: jsonData.BarChart.Lbl_SurchargeCollected,
            data: jsonData.BarChart.SurchargeCollected,

            backgroundColor: [
                "F56954",
                "#F56954",
                "3ED685",
                "3ED685",
                "3ED685",
                "3ED685",
                "3ED685",
                "3ED685",
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
            borderWidth: 1
        }; CessCollested_DataSet =
            {
                label: jsonData.BarChart.Lbl_SurchargeCollected,
                data: jsonData.BarChart.SurchargeCollected,

                backgroundColor: [
                    "Green",
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
                borderWidth: 1
            };
    PopulateBarchart(jsonData);

}
function PopulateBarchart(jsonData) {
    //alert(jsonData.BarChart.SurchargeCollected[0]);
    //alert(jsonData.BarChart.SurchargeCollected[1]);
    //$('#CollapseSearchDiv').trigger('click');
    FinYearForSurchargeCessPopUp = jsonData.BarChart.FinYear;
    SurchargeForPopUp = jsonData.BarChart.SurchargeCollected;
    CessForPopUp = jsonData.BarChart.CessCollested;
    TotalForPopUp = jsonData.BarChart.Total;
    //alert(SurchargeForPopUp);
    //alert(CessForPopUp);
    //alert(TotalForPopUp);
    //bar chart data
    var ctx = document.getElementById("cnvsBarChart");
    if (SurchargeCessBarChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        SurchargeCessBarChart.destroy();
    }
    var data = {
        labels: jsonData.BarChart.FinYear,
        datasets: [
            {
                label: jsonData.BarChart.Lbl_SurchargeCollected,
                backgroundColor: "#F56954",
                data: jsonData.BarChart.SurchargeCollected,
                //data: [100, 233, 45, 734,111]
            },
            {
                label: jsonData.BarChart.Lbl_CessCollected,
                backgroundColor: "#F39C12",
                data: jsonData.BarChart.CessCollested,
                //data: [200, 45, 23, 734, 111]
            },
            {
                label: jsonData.BarChart.Lbl_Total,
                backgroundColor: "#449D44",
                data: jsonData.BarChart.Total,
                //data: [300, 287, 23, 734, 111]
            }
            //SurchargeCollected_DataSet,
            //CessCollested_DataSet
        ]
    };

    //options
    var options = {
        responsive: true,
        title: {
            display: true,
            position: "top",
            //text: "Bar Chart",
            fontSize: 12,
            fontColor: "#111"
        },
        legend: {
            display: true,
            position: "top",
            labels: {
                fontColor: "Black",
                fontSize: 14
            }
        },
        scales: {
            //yAxes: [{
            //    ticks: {
            //        min: 0
            //    }
            //}]
            xAxes: [{ stacked: true }],
            //yAxes: [{ stacked: true }],
             //ADDED BY PANKAJ ON 21-09-2020 FOR LABEL ON Y-AXIS
            yAxes: [{
                stacked: true,
                scaleLabel: {
                    display: true,
                    labelString:'Rupees in Crore'
                }
            }]
        }
    }
    //create Chart class object
    var SurchargeCessBarChart = new Chart(ctx, {
        type: "bar",
        data: data,
        options: options
    });

   // below code commented by shubham bhagat on 11-09-2020  
   //unBlockUI();


}

//Region for High Value properties Chart
function LoadHighValPropChart(GraphId, toggleBtnId) {
   
    $('#IDRdoHighValDocRegd').hide();
    $('#IDRdoHighValRevenue').show();
    //  blockUI('loading data.. please wait...');
    $.ajax({
        url: "/Dashboard/Dashboard/LoadHighValPropChartData",
        type: "POST",
        //data: { 'toggleBtnId': toggleBtnId, 'DistrictCode': $('#DROOfficeListID').val(), SROCode: $('#SROOfficeListID').val()  },
        data: { 'toggleBtnId': toggleBtnId, 'DistrictCode': $("#DROOfficeListID option:selected").val(), SROCode: $("#SROOfficeListID option:selected").val() },

        success: function (jsonData) {

            if (toggleBtnId == 1) {
                if ($("#dv_3_Table").length > 0)
                    $("#dv_3_Table").remove();
                $("#dv_3_Wrapper").html('<div id="dv_3_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart"><canvas id="cnvsHighValPropLineChart" style="height:300px; width: 807px;"></canvas></div></div></div>');
                PopulateHighValPropDataSet(jsonData);

                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                $("#EXCELHighValPropSpanID").html('');

            }


            if (toggleBtnId == 2) {
                if ($("#dv_3_Table").length > 0)

                    $("#dv_3_Graph").remove();

                $("#dv_3_Wrapper").html('<div id="dv_3_Table"><div id="dv_3_HighValPropWrapper"></div></div>');
                FillDataTableDetails(jsonData.TableColumns, jsonData.TableData, "tbl_3_HighValProp", "dv_3_HighValPropWrapper")

                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                if (jsonData.TableData.length != undefined) {
                    $("#EXCELHighValPropSpanID").html('');
                    $("#EXCELHighValPropSpanID").html(jsonData.ExcelBtn);
                }
                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020


            }


           // below code commented and changed from "$.unblockUI();" to "unBlockUI();" of unblock by shubham bhagat on 11-09-2020
            //$.unblockUI();
            // below code commented by shubham bhagat on 11-09-2020  
            //unBlockUI();
        },
        error: function (xhr, status, err) {
            bootbox.alert("Error occured while proccessing your request : " + err);

            //$.unblockUI();
            unBlockUI();
        }
    });





}
//Region High Value Property Chart for Documents Registered
function LoadHighValPropChartDataForDocs(GraphId, toggleBtnId) {

    $('#IDRdoHighValDocRegd').show();
    $('#IDRdoHighValRevenue').hide();
    //  blockUI('loading data.. please wait...');
    $.ajax({
        url: "/Dashboard/Dashboard/LoadHighValPropChartDataForDocs",
        type: "POST",
        //data: { 'toggleBtnId': toggleBtnId, 'DistrictCode': $('#DROOfficeListID').val(), SROCode: $('#SROOfficeListID').val()  },
        data: { 'toggleBtnId': toggleBtnId, 'DistrictCode': $("#DROOfficeListID option:selected").val(), SROCode: $("#SROOfficeListID option:selected").val() },

        success: function (jsonData) {

            if (toggleBtnId == 1) {
                if ($("#dv_3_Table").length > 0)
                    $("#dv_3_Table").remove();
                $("#dv_3_Wrapper").html('<div id="dv_3_Graph"><div class="box-body chat" id="chat-box" style="width: auto;  overflow: hidden;"><div class="chart"><canvas id="cnvsHighValPropLineChart" style="height:300px; width: 807px;"></canvas></div></div></div>');
                PopulateHighValPropDataSet(jsonData);

                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                $("#EXCELHighValPropSpanID").html('');
            }


            if (toggleBtnId == 2) {
                if ($("#dv_3_Table").length > 0)

                    $("#dv_3_Graph").remove();

                $("#dv_3_Wrapper").html('<div id="dv_3_Table"><div id="dv_3_HighValPropWrapper"></div></div>');
                FillDataTableDetails(jsonData.TableColumns, jsonData.TableData, "tbl_3_HighValProp", "dv_3_HighValPropWrapper")

                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                if (jsonData.TableData.length != undefined) {
                    $("#EXCELHighValPropSpanID").html('');
                    $("#EXCELHighValPropSpanID").html(jsonData.ExcelBtn);
                }
                // ADDED BY SHUBHAM BHAGAT ON 17-09-2020

            }


          // below code commented and changed from "$.unblockUI();" to "unBlockUI();" of unblock by shubham bhagat on 11-09-2020
            //$.unblockUI();
            // below code commented by shubham bhagat on 11-09-2020  
            //unBlockUI();
        },
        error: function (xhr, status, err) {
            bootbox.alert("Error occured while proccessing your request : " + err);

            //$.unblockUI();
            unBlockUI();

        }
    });





}
function PopulateHighValPropDataSet(jsonData) {



    OneLakhToTenLakhs_DataSet =
        {
            label: jsonData.LineChart.Lbl_OneLakhToTenLakhs,
            // lineTension: 0.1,
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
            data: jsonData.LineChart.OneLakhToTenLakhs,
        };

    TenLakhsToOneCrore_DataSet =
        {
            label: jsonData.LineChart.Lbl_TenLakhsToOneCrore,
            //lineTension: 0.1,
            backgroundColor: "#F56954",
            borderColor: "#F56954",
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
            data: jsonData.LineChart.TenLakhsToOneCrore,
        };

    OneCroreToFiveCrore_DataSet =
        {
            label: jsonData.LineChart.Lbl_OneCroreToFiveCrore,
            //lineTension: 0.1,
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
            data: jsonData.LineChart.OneCroreToFiveCrore,
        };


    FiveCroreToTenCrore_DataSet =
        {
            label: jsonData.LineChart.Lbl_FiveCroreToTenCrore,
            // lineTension: 0.1,
            backgroundColor: "#3ed685",
            borderColor: "#3ed685",
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
            data: jsonData.LineChart.FiveCroreToTenCrore,
        };

    AboveTenCrore_DataSet =
        {
            label: jsonData.LineChart.Lbl_AboveTenCrore,
            //lineTension: 0.1,
            backgroundColor: "#dbc130",
            borderColor: "#dbc130",
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
            data: jsonData.LineChart.AboveTenCrore,
        };


    ReloadHighValPropChart(jsonData.LineChart.FinYear);
    FinYearForHighValPropPopUp = jsonData.LineChart.FinYear;


}
function ReloadHighValPropChart(FinYear) {

    var canvas = document.getElementById('cnvsHighValPropLineChart');
    if (HighValPropLineChart != undefined) {   // destroy previous chart else it shows previous values also if you hover on bars
        HighValPropLineChart.destroy();
    }
    var data = {
        labels: FinYear,
        datasets: [

            OneLakhToTenLakhs_DataSet,
            TenLakhsToOneCrore_DataSet,
            OneCroreToFiveCrore_DataSet,
            FiveCroreToTenCrore_DataSet,
            AboveTenCrore_DataSet

        ]

    };

    //ADDED BY PANKAJ ON 21-09-2020 FOR LABEL ON Y-AXIS
    var HighValuePropertyChartOption = {
        scales: {
            yAxes: [{
                scaleLabel: {
                    display: true,
                    labelString: ($('input[name="rdoDBHighValueProp"]:checked').val() === 'Revenue') ? 'Rupees in Crore' : ''
                }
            }]
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

    HighValPropLineChart = Chart.Line(canvas, {
        data: data,
        //options: barChartOptions
        options: HighValuePropertyChartOption
    });

}

//To fill Datatable
function FillDataTableDetails(TableColumns, TableData, HtmlTableID, WrapperDivID) {



    //|| jsonData.data.length == 0
    if (TableData.length === undefined) {

        bootbox.alert({
            size: 'small',
            message: '<i class="fa fa-exclamation-triangle text-warning boot-icon"></i> <span class="boot-alert-txt"> Invalid option selected.</span>',
            callback: function () { /* your callback code */ }
        });

    }
    else {
        if ($.fn.DataTable.isDataTable("#" + HtmlTableID)) {
            if ($('#' + HtmlTableID).length > 0)
                $('#' + HtmlTableID).remove();

            $('#' + WrapperDivID).html('<table id="' + HtmlTableID + '" class="table table-striped table-bordered table-condensed table-hover" style="text-align:center !important;vertical-align: middle;background-color: aliceblue; overflow: hidden; border: solid;"></table>');



        }
        else {
            $('#' + WrapperDivID).html('<table id="' + HtmlTableID + '" class="table table-striped table-bordered table-condensed table-hover" style="text-align:center !important;vertical-align: middle;background-color: aliceblue; overflow: hidden; border: solid;"></table>');
        }



        var table = $('#' + HtmlTableID).DataTable({
            dom: "Bfrtip",
            destroy: true,
            // "bPaginate": false,
            "bInfo": false,
            "bPaginate": false,
            "bFilter": false,
            responsive: true,
            //Added By Raman Kalegaonkar on 30-03-2020
            "scrollY": "200px",
            "scrollCollapse": true,
            //Uncommented by shubham on 07-04-2020
            "scrollX": true,
            //paging: true,
            //  pageLength: 10,
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


}

//To fill Datatable
//Created by shubham bhagat on 07-04-2020
function FillDataTableDetailsForSurchargeAndCess(TableColumns, TableData, HtmlTableID, WrapperDivID) {



    //|| jsonData.data.length == 0
    if (TableData.length === undefined) {

        bootbox.alert({
            size: 'small',
            message: '<i class="fa fa-exclamation-triangle text-warning boot-icon"></i> <span class="boot-alert-txt"> Invalid option selected.</span>',
            callback: function () { /* your callback code */ }
        });

    }
    else {
        if ($.fn.DataTable.isDataTable("#" + HtmlTableID)) {
            if ($('#' + HtmlTableID).length > 0)
                $('#' + HtmlTableID).remove();

            $('#' + WrapperDivID).html('<table id="' + HtmlTableID + '" class="table table-striped table-bordered table-condensed table-hover" style="text-align:center !important;vertical-align: middle;background-color: aliceblue; overflow: hidden; border: solid;"></table>');



        }
        else {
            $('#' + WrapperDivID).html('<table id="' + HtmlTableID + '" class="table table-striped table-bordered table-condensed table-hover" style="text-align:center !important;vertical-align: middle;background-color: aliceblue; overflow: hidden; border: solid;"></table>');
        }



        var table = $('#' + HtmlTableID).DataTable({
            dom: "Bfrtip",
            destroy: true,
            // "bPaginate": false,
            "bInfo": false,
            "bPaginate": false,
            "bFilter": false,
            responsive: true,
            //Added By Raman Kalegaonkar on 30-03-2020
            "scrollY": "200px",
            "scrollCollapse": true,
            "scrollX": false,

            //paging: true,
            //  pageLength: 10,
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


}



// ADDED BY SHUBHAM BHAGAT ON 17-09-2020
function EXCELRevenueCollectedChartData(toggleBtnId, DistrictCode, SROCode) {
    //DistrictTextForExcel = $('#DROOfficeListID option:selected').text();
    //SROTextForExcel = $('#SRODropDownListID option:selected').text();
    window.location.href = '/Dashboard/Dashboard/EXCELRevenueCollectedChartData?toggleBtnId=' + toggleBtnId + "&DistrictCode=" + DistrictCode + "&SROCode=" + SROCode + "&DistrictTextForExcel=" + DistrictTextForExcel + "&SROTextForExcel=" + SROTextForExcel;
}
// ADDED BY SHUBHAM BHAGAT ON 17-09-2020


// ADDED BY SHUBHAM BHAGAT ON 17-09-2020
function EXCELDocumentRegisteredChartData(toggleBtnId, DistrictCode, SROCode) {
    //DistrictTextForExcel = $('#DROOfficeListID option:selected').text();
    //SROTextForExcel = $('#SRODropDownListID option:selected').text();
    window.location.href = '/Dashboard/Dashboard/EXCELDocumentRegisteredChartData?toggleBtnId=' + toggleBtnId + "&DistrictCode=" + DistrictCode + "&SROCode=" + SROCode + "&DistrictTextForExcel=" + DistrictTextForExcel + "&SROTextForExcel=" + SROTextForExcel;
}
// ADDED BY SHUBHAM BHAGAT ON 17-09-2020

// ADDED BY SHUBHAM BHAGAT ON 17-09-2020
function EXCELHighValPropChartData(toggleBtnId, DistrictCode, SROCode) {
    //DistrictTextForExcel = $('#DROOfficeListID option:selected').text();
    //SROTextForExcel = $('#SRODropDownListID option:selected').text();
    window.location.href = '/Dashboard/Dashboard/EXCELHighValPropChartData?toggleBtnId=' + toggleBtnId + "&DistrictCode=" + DistrictCode + "&SROCode=" + SROCode + "&DistrictTextForExcel=" + DistrictTextForExcel + "&SROTextForExcel=" + SROTextForExcel;
}
// ADDED BY SHUBHAM BHAGAT ON 17-09-2020

// ADDED BY SHUBHAM BHAGAT ON 17-09-2020
function EXCELHighValPropChartDataForDocs(toggleBtnId, DistrictCode, SROCode) {
    //DistrictTextForExcel = $('#DROOfficeListID option:selected').text();
    //SROTextForExcel = $('#SRODropDownListID option:selected').text();
    window.location.href = '/Dashboard/Dashboard/EXCELHighValPropChartDataForDocs?toggleBtnId=' + toggleBtnId + "&DistrictCode=" + DistrictCode + "&SROCode=" + SROCode + "&DistrictTextForExcel=" + DistrictTextForExcel + "&SROTextForExcel=" + SROTextForExcel;
}
// ADDED BY SHUBHAM BHAGAT ON 17-09-2020

//ADDED BY PANKAJ SAKHARE ON 17-09-2020
function EXCELDashboardSumaryTable(DistrictID, SroID, NatureOfDocID, FinYear) {    
    window.location.href = '/Dashboard/Dashboard/EXCELDashboardSummaryTable?DistrictId=' + DistrictID + "&SroId=" + SroID + "&NatureOfDocId=" + NatureOfDocID + "&FinYearID=" + FinYear + "&DistrictText=" + DistrictTextForExcel + "&SroText=" + SROTextForExcel;
}

//ADDED BY PANKAJ SAKHARE ON 18-09-2020
function EXCELSurchargeCessBarChart(toggleBtnId, DistrictCode, SROCode, NatureOfDoc) {
    window.location.href = '/Dashboard/Dashboard/EXCELSurchargeCess?toggleBtnId=' + toggleBtnId + "&DistrictCode=" + DistrictCode + "&SROCode=" + SROCode + "&NatureOfDoc=" + NatureOfDoc + "&DistrictText=" + DistrictTextForExcel + "&SroText=" + SROTextForExcel;
}


