using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.Alerts;
using ECDataAPI.Areas.Alerts.Interfaces;
using ECDataAPI.Areas.Alerts.DAL;

namespace ECDataAPI.Areas.Alerts.BAL
{
    public class SendSMSDetailsBAL : ISendSMSDetails
    {
        ISendSMSDetails dalObject = new SendSMSDetailsDAL();

        public bool IsMobileNumberVerified(long userID)
        {
            return dalObject.IsMobileNumberVerified(userID);
        }

        public SendSMSResponseModel SendOTP(SendOTPRequestModel SMSRequest)
        {
            return dalObject.SendOTP(SMSRequest);
        }

        public string SendSMS(SendSMSRequestModel SMSRequest)
        {
            return dalObject.SendSMS(SMSRequest);
        }

        public ValidateOTPResponseModel ValidateOTP(OTPValidationModel otpValidationModel)
        {
            return dalObject.ValidateOTP(otpValidationModel);
        }

        public SendSMSResponseModel ResendOTP(SendOTPRequestModel SMSRequest)
        {
            return dalObject.ResendOTP(SMSRequest);
        }
    }
}