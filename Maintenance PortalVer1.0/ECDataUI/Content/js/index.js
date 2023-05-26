$(document).ready(function () {
    $("#font-min").click(function () {
        $("body").css("font-size", "13px");
        $(".navbar-default .navbar-nav > li > a").css("font-size", "12px");
        $(".navbar .dropdown-menu li a").css("font-size", "11px");
        $(".breadcrumb-wrap h4").css("font-size", "12px");
        $("ol.breadcrumb li a").css("font-size", "11px");
        $(".container-4").css("font-size", "11px");
        $(".info-data").css("font-size", "11px");
        $(".data-block").css("font-size", "12px");
    });

    $("#font-normal").click(function () {
        $("body").css("font-size", "14px");
        $(".navbar-default .navbar-nav > li > a").css("font-size", "13px");
        $(".navbar .dropdown-menu li a").css("font-size", "12px");
        $(".breadcrumb-wrap h4").css("font-size", "13px");
        $("ol.breadcrumb li a").css("font-size", "12px");
        $(".container-4").css("font-size", "12px");
        $(".info-data").css("font-size", "12px");
        $(".data-block").css("font-size", "13px");
    });


    $("#font-max").click(function () {
        $("body").css("font-size", "15px");
        $(".navbar-default .navbar-nav > li > a").css("font-size", "14px");
        $(".navbar .dropdown-menu li a").css("font-size", "13px");
        $(".breadcrumb-wrap h4").css("font-size", "14px");
        $("ol.breadcrumb li a").css("font-size", "13px");
        $(".container-4").css("font-size", "13px");
        $(".info-data").css("font-size", "13px");
        $(".data-block").css("font-size", "14px");
    });
});