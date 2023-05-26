
$(document).ready(function () {


    $("#lblModuleName").text(DefaultModuleName);

    $("#txtModuleNo").val(DefaultModuleID);

    loadSideBarStatistics(DefaultModuleID, DefaultModuleName);

    $("#btnRefreshStats").click(function () {
        if ($("#frmStatistics").valid()) {

            $.ajax({
                type: "POST",
                url: "/Home/GetHomePageSideBarStatistics",
                data: $("#frmStatistics").serialize(),

                success: function (data) {

                    $("#ULsubModuleStatsList").empty();

                    var strdata = "";
                    $.each(data.subModuleList, function () {
                        strdata += '<li><a href="#"><i class="fa fa-dot-circle-o"></i>&nbsp;&nbsp;' + this.SubModuleName + '<span class="pull-right badge bg-blue" style="min-width:40px;">' + this.Count + "</span></a></li>";
                    });

                    $("#ULsubModuleStatsList").append(strdata);
                },
                error: function (xhr, status, err) {
                    bootbox.alert("Error " + err);
                }
            });
        }
    });


    //LoggOFF onClick
    $("#btnLoggOffID").click(function () {
        //  BlockUI();
        $.ajax({
            type: "Get",
            url: "/Login/Logout",
            //data: { "encryptedID": encryptedID },
            success: function (data) {

                if (data.success == true) {

                    window.location.href = data.URL;

                }
            },
            error: function () {
                bootbox.alert("error");
                //  $.unblockUI();
            }
        });
    });


    //********************** To change "Month & Year" in Side bar stats panel. *****************
    $('.DropDownInStats').change(function () {
        CopyMonthAndYear();
    });
    $('#ddlmonths').trigger('change');
});

function CopyMonthAndYear() {
    var SelectedMonthId = $('#ddlmonths :selected');
    var SelectedYearId = $('#ddlyears :selected');
    var Month = SelectedMonthId.val();
    var Year = SelectedYearId.val();

    //alert(Month);
    // alert(Month.length);

    if (Year != 0 && (Month != 0 && Month.length < 12)) {


        if (Month == 1) {
            $("#MonthInStatBar").text("");
            $("#YearInStatBar").text("Year - " + SelectedYearId.text());
        }
        else {
            $("#MonthInStatBar").text(SelectedMonthId.text());
            $("#YearInStatBar").text("- " + SelectedYearId.text());
        }



        //******** To refresh stats on change of DropDowns of Month or Year **********
        $("#btnRefreshStats").trigger("click");
    }


}

function loadSideBarStatistics(ModuleId) {

    ToggleSubModules(ModuleId);

    //****************** To set Module Name in "Side Bar Statistics" ********************************
    $("#lblModuleName").text(arguments[1]);
    $("#txtModuleNo").val(ModuleId);

    //*************** To add and remove "Icon of Module" in side bar statistics *************************
    var IconID = "#ModuleIcon_" + ModuleId;
    var iconClassToSet = $(IconID).attr('class');
    var iconClassToRemove = $(ModuleSideBarStatsIcon).attr('class');
    $(ModuleSideBarStatsIcon).removeClass(iconClassToRemove).addClass(iconClassToSet);

    //*************** To add and remove "Background Color of Module Title Panel" in side bar statistics *************************
    var divPanelID = "#divModuleColorPanel_" + ModuleId;
    var colorClassToSet = $(divPanelID).attr('class');
    var colorClassToRemove = $(divSideBarModulePanelColor).attr('class');
    $(divSideBarModulePanelColor).removeClass(colorClassToRemove).addClass(colorClassToSet);

    $('#ddlmonths').trigger('change');

}

function ToggleSubModules(ModuleID) {

    CloseRemainingModules();

    $('#div_SubModules_' + ModuleID).slideToggle();
    $('#div_SubModules_' + ModuleID).addClass('DropDownModuleList');

    var ToggleIconID = "#ToggleIcon_" + ModuleID;
    var classToRemove = $(ToggleIconID).attr('class');
    var classToSet = (classToRemove == "fa fa-plus" ? "fa fa-minus" : "fa fa-plus");
    $(ToggleIconID).removeClass(classToRemove).addClass(classToSet);
}

function CloseRemainingModules() {
    // alert($(".btnModuleToggle > i").length);
    //alert($(".btnModuleToggle > i[class='fa fa-minus']").length);

    $(".btnModuleToggle > i[class='fa fa-minus']").removeClass('fa fa-minus').addClass('fa fa-plus');
    $(".DropDownModuleList").slideToggle();
    $(".DropDownModuleList").removeClass('DropDownModuleList');

}


function loadMenus(menuId, moduleId, modulename) {

    window.location.href = "/Home/RedirectToMenuPage/" + menuId + "$" + moduleId + "$" + modulename;;
}


