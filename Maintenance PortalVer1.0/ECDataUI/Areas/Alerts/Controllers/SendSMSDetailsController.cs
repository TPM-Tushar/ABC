using CustomModels.Models.Alerts;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ECDataUI.Areas.Alerts.Controllers
{
    public class SendSMSDetailsController : Controller
    {
        long userID = 0;
        ServiceCaller caller = null;
        //  OTPValidationModel otpValidationModel = null;
        private string errorMessage = String.Empty;





        [HttpGet]
        public ActionResult VerifyCaptchaAfterRegistrationForOTP(short IsOTPSent, string EncryptedUId, short OTPTypeId)
        {
            OTPValidationModelForCaptcha otpValidationModel = new OTPValidationModelForCaptcha();
            otpValidationModel.IsCaptchaVerified = false;
            otpValidationModel.EncryptedUId = EncryptedUId;
            otpValidationModel.IsOTPSent = IsOTPSent;
            otpValidationModel.OTPTypeId = (short)CommonEnum.OTPTypeId.MobileVerification;

            return RedirectToAction("VerifyCaptchaForOTP", new { EncryptedUId });

        }


        [HttpGet]
        public ActionResult VerifyCaptchaForOTP(string EncryptedUId = "")
        {
            OTPValidationModelForCaptcha otpValidationModel = new OTPValidationModelForCaptcha();
            otpValidationModel.IsCaptchaVerified = false;

            if (!string.IsNullOrEmpty(EncryptedUId))
                otpValidationModel.EncryptedUId = EncryptedUId;
            else
                otpValidationModel.EncryptedUId = URLEncrypt.EncryptParameters(new String[] { "UserId=" + KaveriSession.Current.UserID });

            otpValidationModel.IsOTPSent = (short)CommonEnum.IsOTPSent.OTPYetToBeSend;
            otpValidationModel.OTPTypeId = (short)CommonEnum.OTPTypeId.MobileVerification;

            return View("OTPValidation", otpValidationModel);

        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult VerifyCaptchaForOTP(OTPValidationModelForCaptcha model)
        {
            ModelState.Remove("EncryptedOTP");
            ;
            if (ModelState.IsValid)
            {
                return Json(new { success = true, model.IsOTPSent, model.OTPTypeId, model.EncryptedUId });

            }
            else
            {
                string errorMsg = ModelState.FormatErrorMessageInString();
                return Json(new { success = false, errorMessage = errorMsg });
            }

        }


        [HttpGet]
        [EventAuditLogFilter(Description = "Send OTP to Mobile Number of Online user")]
        public ActionResult InputOTPFromUser(short IsOTPSent, string EncryptedUId, short OTPTypeId)
        {
            if (IsOTPSent == (short)CommonEnum.IsOTPSent.OTPNotSent) // error case OTP was not sent to user -- shrininvas
            {
                return Json(new { success = false, message = "OTP could not be sent to your registered mobile number." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (IsOTPSent == (short)CommonEnum.IsOTPSent.OTPYetToBeSend)
                {
                    SendOTPRequestModel OTPRequest = new SendOTPRequestModel();
                    caller = new ServiceCaller("SendSMSDetailsAPIController");

                    //userID = KaveriSession.Current.UserID;

                    OTPRequest.OTPRequestEncryptedUId = EncryptedUId;
                    OTPRequest.OTPTypeId = OTPTypeId;

                    if (OTPRequest.OTPTypeId == (short)CommonEnum.OTPTypeId.MobileVerification)
                    {
                        OTPRequest.messageToBeSent = "OTP to verify your mobile number is ";
                    }

                    SendSMSResponseModel OTPResponse = caller.PostCall<SendOTPRequestModel, SendSMSResponseModel>("SendOTP", OTPRequest, out errorMessage);

                    //To bypass OTP service(SMS)
                    //SendSMSResponseModel OTPResponse = new SendSMSResponseModel();
                    //OTPResponse.errorCode = "0";
                    //OTPResponse.statusCode = "SUCCESS";

                    OTPValidationModelForCaptcha otpValidationModel = new OTPValidationModelForCaptcha();
                    otpValidationModel.EncryptedUId = URLEncrypt.EncryptParameters(new String[] { "UserId=" + userID });
                    otpValidationModel.IsOTPSent = IsOTPSent;
                    otpValidationModel.OTPTypeId = OTPTypeId;
                    // Added by shubham bhagat on 20-04-2019 to show mobile number
                    otpValidationModel.MobileNumber = OTPResponse.MobileNumber;
                    if (OTPResponse.statusCode != null)
                    {
                        otpValidationModel.Message = "OTP has been sent to your registered mobile number.";

                    }


                    if (OTPResponse.errorCode.Equals("0"))
                    {

                        //return View("OTPValidation", otpValidationModel);

                        // Added by shubham bhagat on 20-04-2019 to show mobile number
                        // Commented and changed by shubham bhagat on 20-04-2019 to show mobile number
                        //return Json(new { success = true}, JsonRequestBehavior.AllowGet);
                        return Json(new { success = true , MobileNumberToDisplay = OTPResponse.MobileNumber }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { success = false, message = "OTP couldn't sent to your mobile number , please contact help desk." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    OTPValidationModelForCaptcha otpValidationModel = new OTPValidationModelForCaptcha();
                    otpValidationModel.EncryptedUId = EncryptedUId;
                    otpValidationModel.IsOTPSent = IsOTPSent;
                    otpValidationModel.OTPTypeId = OTPTypeId;
                    otpValidationModel.Message = "OTP is sent to registered mobile number.";

                    return View("OTPValidation", otpValidationModel);
                }
            }
        }




        [HttpPost]
        public ActionResult SendSMS(SendSMSRequestModel SMSRequest)
        {
            caller = new ServiceCaller("SendSMSDetailsAPIController");

            SendSMSResponseModel response = caller.PostCall<SendSMSRequestModel, SendSMSResponseModel>("SendSMS", SMSRequest, out errorMessage);

            if (response.errorCode.Equals("0"))
                return Json(new { success = true, message = "SMS sent successfully." });
            else
                return Json(new { success = false, message = response.errorString });
        }

        //[HttpPost]
        //public ActionResult SendOTP(SendOTPRequestModel OTPRequest)
        //{
        //    caller = new ServiceCaller("SendSMSDetailsAPIController");

        //    userID = KaveriSession.Current.UserID;
        //    OTPRequest.OTPRequestUserId = userID;
        //    OTPRequest.messageToBeSent = "OTP to verify your mobile number is ";

        //    SendSMSResponseModel response = caller.PostCall<SendOTPRequestModel, SendSMSResponseModel>("SendOTP", OTPRequest, out errorMessage);

        //    if (response.errorCode.Equals("0"))
        //        return Json(new { success = true, message = "OTP sent successfully." });
        //    else
        //        return Json(new { success = false, message = "OTP could not be sent." });
        //}

        // OTP validation method call to AlertsAPIController -- Shrinivas

        [HttpPost]
        [EventAuditLogFilter(Description = "Validate OTP to verify user")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult ValidateOTP(OTPValidationModelForCaptcha otpValidationModel)
        {


            OTPValidationModel model = new OTPValidationModel();
            model.Id = otpValidationModel.Id;
            model.IsOTPSent = otpValidationModel.IsOTPSent;
            model.EncryptedOTP = otpValidationModel.EncryptedOTP;
            model.EncryptedUId = otpValidationModel.EncryptedUId;
            model.isToShowResponseMessage = otpValidationModel.isToShowResponseMessage;
            model.OTPTypeId = otpValidationModel.OTPTypeId;
            model.SessionSalt = KaveriSession.Current.SessionSalt;



            caller = new ServiceCaller("SendSMSDetailsAPIController");

            //otpValidationModel.SessionSalt = ;

            ValidateOTPResponseModel response = caller.PostCall<OTPValidationModel, ValidateOTPResponseModel>("ValidateOTP", model, out errorMessage);

            //add here errorMessage as out parameter to check error occured while 
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Json(new { success = false, msg = errorMessage });
            }

            if (response.responseStatus == true)
            {
                if (otpValidationModel.IsOTPSent == (short)CommonEnum.IsOTPSent.OTPAlreadySent)
                {
                    return Json(new { success = true, msg = response.responseMessage, redirectToLoginPage = true, url = "/Login/UserLogin" });
                }
                else
                {
                    return Json(new { success = true, msg = response.responseMessage });
                }
            }
            else
            {
                if (otpValidationModel.IsOTPSent == (short)CommonEnum.IsOTPSent.OTPAlreadySent)
                {
                    return Json(new { success = false, msg = response.responseMessage, redirectToLoginPage = true, url = "/Login/UserLogin" });
                }
                else
                {
                    return Json(new { success = false, msg = response.responseMessage });
                }
            }
            //return RedirectToAction("HomePage", new RouteValueDictionary(new { controller = "Home", action = "HomePage", area = "" }));
        }

        // OTP validation method call to AlertsAPIController -- Shrinivas

        [HttpGet]
        public ActionResult IsMobileNumberVerified()
        {
            caller = new ServiceCaller("SendSMSDetailsAPIController");

            userID = KaveriSession.Current.UserID;

            bool response = caller.GetCall<bool>("IsMobileNumberVerified", new { userID = userID.ToString() }, out errorMessage);

            if (response)
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }





        public ActionResult GetCaptchaImage()
        {
            return CaptchaLib.ControllerExtensions.Captcha(this, new CaptchaLib.CaptchaImage(), 100, 220, 70);

        }

    }


}