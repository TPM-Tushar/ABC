using CustomModels.Models.Alerts;
using CustomModels.Models.Common;
using CustomModels.Models.HomePage;
using CustomModels.Models.MenuHelper;
using ECDataAPI.BAL;
using ECDataAPI.Common;
using ECDataAPI.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ECDataAPI.Interface;

namespace ECDataAPI.Controllers
{
    public class HomeApiController : ApiController
    {

        [HttpPost]
        [Route("api/HomeApiController/GetHomePageDetails")]
        public IHttpActionResult GetHomePageDetails(LoadMenuModel model)
        {
            try
            {
                HomePageModel result = new HomePageDAL().GetHomePageDetails(model);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("api/HomeApiController/GetHomePageSideBarStatistics")]
        public IHttpActionResult GetHomePageSideBarStatistics(LoadMenuModel model)
        {
            try
            {
                MenuItems result = new HomePageDAL().GetHomePageSideBarStatistics(model);
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }



        [HttpPost]
        [Route("api/HomeApiController/GetUserPasswordDetails")]
        public IHttpActionResult GetUserPasswordDetails(PasswordDetailsModel model)
        {
            try
            {
                PasswordDetailsModel result = new HomePageBAL().GetUserPasswordDetails(model.UserID, model.RoleID);
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

		//Added by madhur on 15-02-2022
        SendSMSResponseModel SMSResponse = null;
        ValidateOTPResponseModel validationResponse = null;
        IHomePage balObject = null;
        [HttpPost]
        [Route("api/HomeApiController/SendSMS")]
        public IHttpActionResult SendSMS(SendSMSRequestModel SMSRequest)
        {
            try
            {
                SMSResponse = new SendSMSResponseModel();

                string result = new HomePageBAL().SendSMS(SMSRequest);

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/HomeApiController/SendOTP")]
        public IHttpActionResult SendOTP(SendOTPRequestModel OTPRequest)
        {
            try
            {
                SMSResponse = new SendSMSResponseModel();
                balObject = new HomePageBAL();
                SMSResponse = balObject.SendOTP(OTPRequest);

                return Ok(SMSResponse);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/HomeApiController/ValidateOTP")]
        public IHttpActionResult ValidateOTP(OTPValidationModel otpValidationModel)
        {
            try
            {
                balObject = new HomePageBAL();
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
        [Route("api/HomeApiController/IsMobileNumberVerified")]
        public IHttpActionResult IsMobileNumberVerified(String userID)
        {
            try
            {
                balObject = new HomePageBAL();

                bool response = balObject.IsMobileNumberVerified(Convert.ToInt64(userID));

                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/HomeApiController/ResendOTP")]
        public IHttpActionResult ResendOTP(SendOTPRequestModel OTPRequest)
        {
            try
            {
                balObject = new HomePageBAL();
                SMSResponse = new SendSMSResponseModel();

                SMSResponse = balObject.ResendOTP(OTPRequest);

                return Ok(SMSResponse);
            }
            catch (Exception)
            {
                throw;
            }
        }
		//Added till here
    }
}
