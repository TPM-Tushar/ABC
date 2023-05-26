//Global variables.
var token = '';
var header = {};

$(document).ready(function () {
    $(".KG").hide();


    console.log("DBID", $("#DBId").val());

    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;
    $('#DtlsSearchParaListCollapse').click(function () {
        var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
        var classToSet = (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x" ? "fa fa-minus-square-o fa-pull-left fa-2x" : "fa fa-plus-square-o fa-pull-left fa-2x");
        $('#DtlsToggleIconSearchPara').removeClass(classToRemove).addClass(classToSet);

    });

    $('#MasterDataID').DataTable({
        rowReorder: true,
        "orderable": true,
        aaSorting: [[9, 'desc']],

    });

    $(
        $('#DBId').change(function () {

            let DBId = $("#DBId").val();
            console.log(DBId)

            console.log("DBID", DBId);

            if (DBId == 1)
            {

                $(".EC").show();
                $(".KG").hide();
                 
            }

            if (DBId == 2)
            {
                $(".KG").show();
                $(".EC").hide();

            }
          }
        )
    );


});



function GetMasterData() {


    var TableIdEC = $(".EC").val();
    var TableIdKG = $(".KG").val();


    if (TableIdEC == 0 || TableIdEC == null) {
        var TableId = $(".KG").val();
    }
    else
        var TableId = $(".EC").val();

    $(".EC").val('');
    $(".KG").val('');
    var tableRegistrationNoVerificationSummary = $('#MasterDataID').DataTable({
        ajax: {

            url: '/Remittance/MasterData/GetMasterData',
            type: "POST",
            headers: header,
            data: {
            'TableId': TableId,
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
                                var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                                if (classToRemove == "fa fa-minus-square-o fa-pull-left fa-2x")
                                    $('#DtlsSearchParaListCollapse').trigger('click');
                                $("#MasterDataID").DataTable().clear().destroy();
                                $("#EXCELSPANID").html('');
                            }
                        }
                    });
                }
                else {
                    var classToRemove = $('#DtlsToggleIconSearchPara').attr('class');
                    if (classToRemove == "fa fa-plus-square-o fa-pull-left fa-2x")
                        $('#DtlsSearchParaListCollapse').trigger('click');









                    if (TableId == 1 && json.data.length <= 10 && json.IsRecordFilter == false && json.draw == "1") {

                        $("#FirstColSpan").attr("colspan", 2);



                    }
                }
                unBlockUI();
                return json.data;
            },
            error: function () {
                bootbox.alert({
                    message: '<i class="fa fa-exclamation-triangle text-danger boot-icon boot-icon"></i><span class="boot-alert-txt">' + 'Error in Processing' + '</span>',
                    callback: function () {
                    }
                });

                unBlockUI();
            },
            beforeSend: function () {
                blockUI('Loading data please wait.');
                var searchString = $('#MasterDataID_filter input').val();
                if (searchString != "") {
                    var regexToMatch = /^[^<>]+$/;
                    if (!regexToMatch.test(searchString)) {
                        $("#MasterDataID_filter input").prop("disabled", true);
                        bootbox.alert('Please enter valid Search String ', function () {
                            tableRegistrationNoVerificationSummary.search('').draw();
                            $("#MasterDataID_filter input").prop("disabled", false);
                        });
                        unBlockUI();
                        return false;
                    }

                }
            }
        },
        serverSide: true,

        //"scrollX": "300px",
        scrollX: true,
        "scrollY": "300px",
        "responsive": true,
        scrollCollapse: true,
        bPaginate: true,
        bLengthChange: true,
        bInfo: true,
        info: true,
        bFilter: false,
        searching: true,
        "destroy": true,
        "bAutoWidth": true,
        "bSort": true,
        //"lengthMenu": [[10, 25, 50, 100], [10, 25, 50, "All"]],


        columns: [

            { data: "SrNo", "searchable": true, "visible": true, "name": "SrNo" },
            { data: "VillageCode", "searchable": true, "visible": setConditions(TableId), "name": "VillageCode" },
            { data: "SROCode", "searchable": true, "visible": setConditions(TableId), "name": "SROCode", "width": "10%" },
            { data: "HobliCode", "searchable": true, "visible": setConditions(TableId), "name": "HobliCode" },
            { data: "CensusCode", "searchable": true, "visible": setConditions(TableId), "name": "CensusCode" },
            { data: "TalukCode", "searchable": true, "visible": setConditions(TableId), "name": "Taluk Code" },
            { data: "VillageNameK", "searchable": true, "visible": setConditions(TableId), "name": "VillageNameK" },
            { data: "VillageNameE", "searchable": true, "visible": setConditions(TableId), "name": "VillageNameE" },
            { data: "IsUrban", "searchable": true, "visible": setConditions(TableId), "name": "IsUrban" },
            { data: "BhoomiTalukCode", "searchable": true, "visible": setConditions(TableId), "name": "BhoomiTalukCode" },
            { data: "BhoomiVillageCode", "searchable": true, "visible": setConditions(TableId), "name": "BhoomiVillageCode" },
            { data: "BhoomiVillageName", "searchable": true, "visible": setConditions(TableId), "name": "BhoomiVillageName" },
            //hoblimaster
            { data: "HobliCode", "searchable": true, "visible": setConditionsForHobli(TableId), "name": "HobliCode" },
            { data: "TalukCode", "searchable": true, "visible": setConditionsForHobli(TableId), "name": "Taluk Code" },
            { data: "HobliNameK", "searchable": true, "visible": setConditionsForHobli(TableId), "name": "HobliNameK" },
            { data: "HobliNameE", "searchable": true, "visible": setConditionsForHobli(TableId), "name": "HobliNameE" },
            { data: "ShortNameK", "searchable": true, "visible": setConditionsForHobli(TableId), "name": "ShortNameK" },
            { data: "ShortNameE", "searchable": true, "visible": setConditionsForHobli(TableId), "name": "ShortNameE" },
            { data: "BhoomiHobliCode", "searchable": true, "visible": setConditionsForHobli(TableId), "name": "BhoomiHobliCode" },
            { data: "BhoomiHobliName", "searchable": true, "visible": setConditionsForHobli(TableId), "name": "BhoomiHobliName" },
            //bhoomimapping
            { data: "BhoomiMappingID", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "BhoomiMappingID" },
            { data: "DistrictCode", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "DistrictCode" },
            { data: "DistrictNameE", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "DistrictNameE" },
            { data: "KaveriSROCode", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "KaveriSROCode" },
            { data: "KaveriSROName", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "KaveriSROName" },
            { data: "KaveriVillageCode", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "KaveriVillageCode" },
            { data: "KaveriVillageName", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "KaveriVillageName" },
            { data: "kaveriHobliCode", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "KaveriHobliCode" },
            { data: "KaveriHobliName", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "KaveriHobliName" },
            { data: "BhoomiDistrictCode", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "BhoomiDistrictCode" },
            { data: "BhoomiTalukCode", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "BhoomiTalukCode" },
            { data: "BhoomiTalukName", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "BhoomiTalukName" },
            { data: "BhoomiHobliCode", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "BhoomiHobliCode" },
            { data: "BhoomiHobiName", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "BhoomiHobliName" },
            { data: "BhoomiVillageCode", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "BhoomiVillageCode" },
            { data: "BhoomiVillageName", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "BhoomiVillageName" },
            { data: "IsUpdated", "searchable": true, "visible": setConditionsForBhoomi(TableId), "name": "IsUpdated" },
            //district
            { data: "DistrictCode", "searchable": true, "visible": setConditionsForDistrict(TableId), "name": "DistrictCode" },
            { data: "DistrictNameK", "searchable": true, "visible": setConditionsForDistrict(TableId), "name": "DistrictNameK" },
            { data: "DistrictNameE", "searchable": true, "visible": setConditionsForDistrict(TableId), "name": "DistrictNameE" },
            { data: "ShortNameK", "searchable": true, "visible": setConditionsForDistrict(TableId), "name": "ShortNameK" },
            { data: "ShortNameE", "searchable": true, "visible": setConditionsForDistrict(TableId), "name": "ShortNameE" },
            { data: "DIGCode", "searchable": true, "visible": setConditionsForDistrict(TableId), "name": "DIGCode" },
            { data: "BhoomiDistrictCode", "searchable": true, "visible": setConditionsForDistrict(TableId), "name": "BhoomiDistrictCode" },
            //VILLAGE_MASTER_MERGING_MAPPING
            { data: "ID", "searchable": true, "visible": setConditionsForVM(TableId), "name": "ID" },
            { data: "SROCode", "searchable": true, "visible": setConditionsForVM(TableId), "name": "SROCode" },
            { data: "VillageCode", "searchable": true, "visible": setConditionsForVM(TableId), "name": "VillageCode" },
            { data: "MergedVillageCode", "searchable": true, "visible": setConditionsForVM(TableId), "name": "MergedVillageCode" },
            //SRomaster
            { data: "SROCode", "searchable": true, "visible": setConditionsforSRO(TableId), "name": "SROCode" },
            { data: "DistrictCode", "searchable": true, "visible": setConditionsforSRO(TableId), "name": "DistrictCode" },
            { data: "SRONameK", "searchable": true, "visible": setConditionsforSRO(TableId), "name": "SRONameK" },
            { data: "SRONameE", "searchable": true, "visible": setConditionsforSRO(TableId), "name": "SRONameE" },
            { data: "ShortNameK", "searchable": true, "visible": setConditionsforSRO(TableId), "name": "ShortNameK" },
            { data: "ShortNameE", "searchable": true, "visible": setConditionsforSRO(TableId), "name": "ShortameE" },
            { data: "GetBhoomiData", "searchable": true, "visible": setConditionsforSRO(TableId), "name": "GetBhoomiData" },
            { data: "IsVillageMatching", "searchable": true, "visible": setConditionsforSRO(TableId), "name": "IsVillageMatching" },
            //mas_office_master
            { data: "OfficeID", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "OfficeID" },
            { data: "OfficeTypeID", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "OfficeTypeID" },
            { data: "OfficeName", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "OfficeName" },
            { data: "OfficeNameR", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "OfficeNameR" },
            { data: "ShortName", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "ShortName" },
            { data: "ShortNameR", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "ShortNameR" },
            { data: "DistrictID", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "DistrictID" },
            { data: "ParentOfficeID", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "ParentOfficeID" },
            { data: "KaveriCode", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "KaveriCode" },
            { data: "BhoomiCensusCode", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "BhoomiCensusCode" },
            { data: "AnyWhereRegEnabled", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "AnyWhereRegEnabled" },
            { data: "OfficeAddress", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "OfficeAddress" },
            { data: "Landline", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "Landline" },
            { data: "Mobile", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "Mobile" },
            { data: "OnlineBookingEnabled", "searchable": true, "visible": setConditionsForMAS_office(TableId), "name": "OnlineBookingEnabled" },
            //mas_villages
            { data: "VillageID", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "VillageID" },
            { data: "OfficeID", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "OfficeID" },
            { data: "HobliID", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "HobliID" },
            { data: "TalukaID", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "TalukaID" },
            { data: "VillageName", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "VillageName" },
            { data: "VillageNameR", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "VillageNameR" },
            { data: "IsUrban", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "IsUrban" },
            { data: "CensusCode", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "CensusCode" },
            { data: "BhoomiTalukCode", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "BhoomiTalukCode" },
            { data: "BhoomiVillageCode", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "BhoomivillageCode" },
            { data: "BhoomiVillageName", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "BhoomivillageName" },
            { data: "BhoomiDistrictCode", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "BhoomiDistrictCode" },
            { data: "UPORTownID", "searchable": true, "visible": setConditionsForMAS_village(TableId), "name": "UPORTownUD" },
            //mas_hoblis

            { data: "HobliID", "searchable": true, "visible": setConditionsForMAS_Hoblis(TableId), "name": "HobliID" },
            { data: "TalukaID", "searchable": true, "visible": setConditionsForMAS_Hoblis(TableId), "name": "TalukaID" },
            { data: "HobliName", "searchable": true, "visible": setConditionsForMAS_Hoblis(TableId), "name": "HobliName" },
            { data: "HobliNameR", "searchable": true, "visible": setConditionsForMAS_Hoblis(TableId), "name": "HobliNameR" },
            { data: "BhoomiHobliCode", "searchable": true, "visible": setConditionsForMAS_Hoblis(TableId), "name": "BhoomiHobliCode" },
            { data: "BhoomiHobliName", "searchable": true, "visible": setConditionsForMAS_Hoblis(TableId), "name": "BhoomiHobliName" },
            { data: "BhoomiTalukCode", "searchable": true, "visible": setConditionsForMAS_Hoblis(TableId), "name": "BhoomiTalukCode" },
            { data: "BhoomiTalukName", "searchable": true, "visible": setConditionsForMAS_Hoblis(TableId), "name": "BhoomiTalukName" },
            { data: "BhoomiDistrictCode", "searchable": true, "visible": setConditionsForMAS_Hoblis(TableId), "name": "BhoomiDistrictCode" },







        ],
        fnInitComplete: function (oSettings, json) {
            //console.log(json);
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



function EXCELDownloadFun(TableId) {
    //alert("In EXCELDownloadFun");

    //   alert(fromDate + "fromDate");
    // alert("ToDate" + ToDate);
    //    alert("TableId" + TableId);

    window.location.href = '/Remittance/MasterData/ExportSummaryReportToExcel?TableId=' + TableId;

}
function setConditions(TableId) {
    if (TableId == 1) return true;
    //else { return false; }
    return false;
}
function setConditionsForHobli(TableId) {
    if (TableId == 2) return true;
    //else { return false; }
    return false;
}

function setConditionsForBhoomi(TableId) {
    if (TableId == 3) return true;
    //else { return false; }
    return false;
}
function setConditionsForDistrict(TableId) {
    if (TableId == 4) return true;
    //else { return false; }
    return false;
}
//testing
function setConditionsForVM(TableId) {
    if (TableId == 5) return true;
    //else { return false; }
    return false;
}
function setConditionsforSRO(TableId) {
    if (TableId == 6) return true;
    //else { return false; }
    return false;
}
function setConditionsForMAS_office(TableId) {
    if (TableId == 7) return true;
    //else { return false; }
    return false;
}
function setConditionsForMAS_village(TableId) {
    if (TableId == 8) return true;
    //else { return false; }
    return false;
}
function setConditionsForMAS_Hoblis(TableId) {
    if (TableId == 9) return true;
    //else { return false; }
    return false;
}