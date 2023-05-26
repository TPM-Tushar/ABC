using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomModels.Models.Alerts;

namespace ECDataAPI.Areas.Alerts.Interfaces
{
    interface ISendSMSDetails
    {
        string SendSMS(SendSMSRequestModel SMSRequest);
        SendSMSResponseModel SendOTP(SendOTPRequestModel SMSRequest);
        ValidateOTPResponseModel ValidateOTP(OTPValidationModel otpValidationModel);
        bool IsMobileNumberVerified(long userID);
       SendSMSResponseModel ResendOTP(SendOTPRequestModel SMSRequest);
    }
}
