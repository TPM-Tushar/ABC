﻿

$(document).ready(function () {


    $('#EditUserProfile').click(function () {
        $("#EditBtn").hide();
        $("#UpdateBtn").show();

        $('#LoadUserProfile').load('/UserManagement/UserProfileDetails/PEditUserProfileDetails');


    });



});


