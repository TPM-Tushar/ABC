using CustomModels.Models.Alerts;
using CustomModels.Models.HomePage;
using CustomModels.Models.MenuHelper;
using ECDataAPI.DAL;
using ECDataAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.BAL
{
    public class HomePageBAL : IHomePage
    {
        IHomePage objDAL = new HomePageDAL();


        public HomePageModel GetHomePageDetails(LoadMenuModel model)
        {
            return objDAL.GetHomePageDetails(model);

        }


        public MenuItems GetHomePageSideBarStatistics(LoadMenuModel model)
        {
            return objDAL.GetHomePageSideBarStatistics(model);

        }

        public PasswordDetailsModel GetUserPasswordDetails(long UserID, short RoleID)
        {
            return objDAL.GetUserPasswordDetails(UserID, RoleID);
        }

        //Added by Madhur on 15-02-2022 for OTP
        public bool IsMobileNumberVerified(long userID)
        {
            return objDAL.IsMobileNumberVerified(userID);
        }

        public SendSMSResponseModel SendOTP(SendOTPRequestModel SMSRequest)
        {
            return objDAL.SendOTP(SMSRequest);
        }

        public string SendSMS(SendSMSRequestModel SMSRequest)
        {
            return objDAL.SendSMS(SMSRequest);
        }

        public ValidateOTPResponseModel ValidateOTP(OTPValidationModel otpValidationModel)
        {
            return objDAL.ValidateOTP(otpValidationModel);
        }

        public SendSMSResponseModel ResendOTP(SendOTPRequestModel SMSRequest)
        {
            return objDAL.ResendOTP(SMSRequest);
        }
        //
    }
}