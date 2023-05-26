using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using CustomModels.Models.Alerts;
using ECDataAPI.Areas.Alerts.Interfaces;
using ECDataAPI.Areas.Alerts.BAL;

namespace ECDataAPI.Areas.Alerts.Controllers
{
    public class SendSMSDetailsAPIController : ApiController
    {
        ISendSMSDetails balObject = null;
        SendSMSResponseModel SMSResponse = null;
        ValidateOTPResponseModel validationResponse = null;

        [HttpPost]
        [Route("api/SendSMSDetailsAPIController/SendSMS")]
        public IHttpActionResult SendSMS(SendSMSRequestModel SMSRequest)
        {
            try
            {
                balObject = new SendSMSDetailsBAL();
                SMSResponse = new SendSMSResponseModel();

                string result = balObject.SendSMS(SMSRequest);

                return Ok(result);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        [HttpPost]
        [Route("api/SendSMSDetailsAPIController/SendOTP")]
        public IHttpActionResult SendOTP(SendOTPRequestModel OTPRequest)
        {
            try
            {
                balObject = new SendSMSDetailsBAL();
                SMSResponse = new SendSMSResponseModel();

                SMSResponse = balObject.SendOTP(OTPRequest);

                return Ok(SMSResponse);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/SendSMSDetailsAPIController/ValidateOTP")]
        public IHttpActionResult ValidateOTP(OTPValidationModel otpValidationModel)
        {
            try
            {
                balObject = new SendSMSDetailsBAL();
                validationResponse = new ValidateOTPResponseModel();

                validationResponse = balObject.ValidateOTP(otpValidationModel);

                return Ok(validationResponse);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/SendSMSDetailsAPIController/IsMobileNumberVerified")]
        public IHttpActionResult IsMobileNumberVerified(String userID)
        {
            try
            {
                balObject = new SendSMSDetailsBAL();

                bool response = balObject.IsMobileNumberVerified(Convert.ToInt64(userID));

                return Ok(response);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        [HttpPost]
        [Route("api/SendSMSDetailsAPIController/ResendOTP")]
        public IHttpActionResult ResendOTP(SendOTPRequestModel OTPRequest)
        {
            try
            {
                balObject = new SendSMSDetailsBAL();
                SMSResponse = new SendSMSResponseModel();

                SMSResponse = balObject.ResendOTP(OTPRequest);

                return Ok(SMSResponse);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}