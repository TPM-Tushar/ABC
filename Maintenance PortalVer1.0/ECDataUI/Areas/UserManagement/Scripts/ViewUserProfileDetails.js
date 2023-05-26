
//Global variables.
var token = '';
var header = {};

$(document).ready(function () {



    $("#UpdateBtn").hide();
    $("#EditBtn").hide();


    token = $('[name=__RequestVerificationToken]').val();
    header["__RequestVerificationToken"] = token;

    // $("#LoadUserProfile").html(data);

    //$('#LoadUserProfile').load('/UserManagement/UserProfileDetails/PViewUserProfileDetails');
    $("#LoadUserProfile").load('/UserManagement/UserProfileDetails/PViewUserProfileDetails', function (data) {
        return $("#LoadUserProfile").html(data);
    });




    $("#EditBtn").show();


    $("#Country").change(function () {

    });

    $('#ddlIDProof').change(function () {
        // alert("YES");
        alert($('#ddlIDProof').val());
        $('#IDProofNumber').val('');
        $('#IDProofNumber').unbind();

        if ($('#ddlIDProof').val() == 1) {
            $("#IDProofNumber").attr("placeholder", "PAN Number"); //Uppercase in case of PAN.
            $('#IDProofNumber').keyup(function () {
                this.value = this.value.toUpperCase();
            });
        }
        else if ($('#ddlIDProof').val() == 2) {
            $("#IDProofNumber").attr("placeholder", "Passport Number");
        }
        else if ($('#ddlIDProof').val() == 3) {
            $("#IDProofNumber").attr("placeholder", "Driving License Number");
        }
        else if ($('#ddlIDProof').val() == 4) {
            $("#IDProofNumber").attr("placeholder", "Bank Passbook Number");
        }
        else if ($('#ddlIDProof').val() == 5) {
            $("#IDProofNumber").attr("placeholder", "School Leaving Certificate Number");
        }
        else if ($('#ddlIDProof').val() == 6) {
            $("#IDProofNumber").attr("placeholder", "Matriculation Certificate Number");
        }
        else if ($('#ddlIDProof').val() == 9) {
            $("#IDProofNumber").attr("placeholder", "Water Bill Number");
        }
        else if ($('#ddlIDProof').val() == 10) {
            $("#IDProofNumber").attr("placeholder", "Ration Card Number");
        }
        else if ($('#ddlIDProof').val() == 12) {
            $("#IDProofNumber").attr("placeholder", "Voting Card Number");
        }
        else if ($('#ddlIDProof').val() == 13) {
            $("#IDProofNumber").attr("placeholder", "Aadhar Card Number");
        }
        else {
            $("#IDProofNumber").attr("placeholder", "ID Proof Number");
        }
    });

    $('#goToHome').click(function () {

        window.location.href = '/Home/HomePage';
    }


    );


    $('#EditUserProfile').click(function () {
        $("#EditBtn").hide();
        $("#UpdateBtn").show();

        $('#LoadUserProfile').load('/UserManagement/UserProfileDetails/PEditUserProfileDetails');


    }


    );






});


//Block UI
function BlockUI() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}