using CustomModels.Models.Alerts;
using CustomModels.Models.HomePage;
using CustomModels.Models.MenuHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Interface
{
    public interface IHomePage
    {


        HomePageModel GetHomePageDetails(LoadMenuModel model);
        MenuItems GetHomePageSideBarStatistics(LoadMenuModel model);

        PasswordDetailsModel GetUserPasswordDetails(long UserID, short RoleID);

        //added by madhur on 15-02-2022 for OTP

        string SendSMS(SendSMSRequestModel SMSRequest);
        SendSMSResponseModel SendOTP(SendOTPRequestModel SMSRequest);
        ValidateOTPResponseModel ValidateOTP(OTPValidationModel otpValidationModel);
        bool IsMobileNumberVerified(long userID);
        SendSMSResponseModel ResendOTP(SendOTPRequestModel SMSRequest);
        //
    }
}
