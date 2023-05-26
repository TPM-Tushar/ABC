using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.Alerts;
using ECDataAPI.Areas.Alerts.Interfaces;
using System.Net;
using System.IO;
using System.Text;
using System.Web.Script;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using ECDataAPI.DAL;
 
using CustomModels.Security;
using System.Security;
using System.Configuration;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;

namespace ECDataAPI.Areas.Alerts.DAL
{
    public class SendSMSDetailsDAL : ISendSMSDetails,IDisposable
    {
        KaveriEntities dbContext = null;
        private String[] encryptedParameters = null;
        private Dictionary<String, String> decryptedParameters = null;
        //string error, status, smsLogId, queue, toContact, errorString;

        public bool IsMobileNumberVerified(long userID)
        {
            try
            {
                dbContext = new KaveriEntities();

                UMG_UserProfile userProfileDbObj = dbContext.UMG_UserProfile.FirstOrDefault(x => x.UserID == userID);

                if (userProfileDbObj != null)
                {
                    return userProfileDbObj.IsMobileNumVerified;
                }

                return false; // needs to be fixed
            }
            catch (Exception)
            {
                throw ;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public SendSMSResponseModel SendOTP(SendOTPRequestModel OTPRequest)
        {
            long otpVerificationId = 0;
            SendSMSResponseModel OTPResponse = null;
            UMG_OTP_VerificationDetails otpVerificationDbObject = null;

            try
            {
                dbContext = new KaveriEntities();
                OTPResponse = new SendSMSResponseModel();
                //DateTime otpSentDateTime = new DateTime();

                // code to check if OTP sent within last 10 mins -- commented

                //UMG_OTP_VerificationDetails otpVerificationDbObjectForValidity = dbContext.UMG_OTP_VerificationDetails.OrderByDescending(x => x.OtpVerificationID).FirstOrDefault(x => x.UserId == OTPRequest.OTPRequestUserId);

                //TimeSpan validMinutes = new TimeSpan(0, 10, 0);

                //if (otpVerificationDbObjectForValidity != null)
                //    otpSentDateTime = otpVerificationDbObjectForValidity.OTPSentDateTime;

                //if (otpVerificationDbObjectForValidity == null || otpSentDateTime < DateTime.Now.Subtract(validMinutes))
                //{

                encryptedParameters = OTPRequest.OTPRequestEncryptedUId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                OTPRequest.OTPRequestUserId = Convert.ToInt64(decryptedParameters["UserId"].ToString().Trim());


                CommonDAL commonDalObject = new CommonDAL();

                string OTP = commonDalObject.GenerateOTP();
                OTPRequest.messageToBeSent = OTPRequest.messageToBeSent + OTP;

                UMG_UserProfile userProfileDbObj = dbContext.UMG_UserProfile.FirstOrDefault(x => x.UserID == OTPRequest.OTPRequestUserId);

                if (userProfileDbObj != null)
                {
                    OTPRequest.toContact = userProfileDbObj.MobileNumber;
                }
                else
                {
                    OTPResponse.errorCode = "1";
                    return OTPResponse;
                }

                SendSMSRequestModel SMSRequest = new SendSMSRequestModel();

                SMSRequest.messageToBeSent = OTPRequest.messageToBeSent;
                SMSRequest.toContact = OTPRequest.toContact;

                string result = SendSMS(SMSRequest);

                if (string.IsNullOrEmpty(result))
                {
                    // Encrypt OTP and insert entry into Database 

                    otpVerificationDbObject = new UMG_OTP_VerificationDetails();
                    otpVerificationId = (dbContext.UMG_OTP_VerificationDetails.Any() ? dbContext.UMG_OTP_VerificationDetails.Max(x => x.OtpVerificationID) : 0) + 1;
                    otpVerificationDbObject.OtpVerificationID = otpVerificationId;
                    otpVerificationDbObject.UserId = OTPRequest.OTPRequestUserId;
                    UMG_UserDetails userDetailsObj = dbContext.UMG_UserDetails.FirstOrDefault(x => x.UserID == OTPRequest.OTPRequestUserId);
                    if (userDetailsObj != null)
                        otpVerificationDbObject.UMG_UserDetails = userDetailsObj;

                    string encryptedOTP = SHA512Checksum.CalculateSHA512Hash(OTP);
                    otpVerificationDbObject.OTPToSend = encryptedOTP;
                    otpVerificationDbObject.OTPSentDateTime = DateTime.Now; // needs to change to response datetime

                    otpVerificationDbObject.OtpTypeId = OTPRequest.OTPTypeId;

                    UMG_OTP_VerificationDetails otpVerificationReturnObject = dbContext.UMG_OTP_VerificationDetails.Add(otpVerificationDbObject);

                    if (otpVerificationDbObject != null)
                        dbContext.SaveChanges();

                    // Encrypt OTP and insert entry into Database 

                    OTPResponse.errorCode = "0";

                }
                else
                {
                    OTPResponse.errorCode = "1";
                }
                //}
                //else
                //{
                //    OTPResponse.errorCode = "0";
                //}

                // code to check if OTP sent within last 30 mins

                // Added by Shubham Bhagat on 20-04-2019 to show mobile number 
                OTPResponse.MobileNumber = "XXXXXXX" + userProfileDbObj.MobileNumber.Substring(7, 3);

                return OTPResponse;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }




        public string SendSMS(SendSMSRequestModel SMSRequest)
        {
           // SendSMSResponseModel SMSResponse = new SendSMSResponseModel();

            try
            { 

                ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService service = new PreRegApplicationDetailsService.ApplicationDetailsService();
                bool result;
                try
                {
                     result = service.SendSMS(SMSRequest.messageToBeSent, SMSRequest.toContact);

                }
                catch (Exception e)
                {
                    ApiExceptionLogs.LogError(e);
                    return "Something went wrong while connecting to OTP service";

                }

                if (result)
                    return string.Empty; //success
                else
                    return "Unable to send OTP"; //Error



            }
            catch (Exception e)
            {
                ApiExceptionLogs.LogError(e);
                return "Unable to send OTP";

            }
        }



        //public SendSMSResponseModel SendSMS(SendSMSRequestModel SMSRequest)
        //{
        //    SendSMSResponseModel SMSResponse = new SendSMSResponseModel();

        //    try
        //    {

        //        PreRegApplicationDetailsService.ApplicationDetailsService service = new PreRegApplicationDetailsService.ApplicationDetailsService();
        //       bool result= service.SendSMS(SMSRequest.messageToBeSent, SMSRequest.toContact);

        //        return result;


        //    }
        //    catch (Exception e)
        //    {
        //        ApiExceptionLogs.LogError(e);
        //        SMSResponse.errorCode = "1";
        //        return SMSResponse;
        //        //throw;
        //    }
        //}

        public ValidateOTPResponseModel ValidateOTP(OTPValidationModel otpValidationModel)
        {
            ValidateOTPResponseModel responseModel = null;
            //UMG_OTP_UserMobile_Verification userMobileVerifyDbObj = new UMG_OTP_UserMobile_Verification();
            try
            {
                dbContext = new KaveriEntities();
                responseModel = new ValidateOTPResponseModel();

                encryptedParameters = otpValidationModel.EncryptedUId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                long userID = Convert.ToInt64(decryptedParameters["UserId"].ToString().Trim());

                UMG_OTP_VerificationDetails userMobileVerifyDbObj = dbContext.UMG_OTP_VerificationDetails.Where(x => x.UserId == userID && x.OtpTypeId == otpValidationModel.OTPTypeId).OrderByDescending(x => x.OtpVerificationID).FirstOrDefault();

                if (userMobileVerifyDbObj != null)
                {
                    string OTPHash = SHA512ChecksumWrapper.ComputeHash(userMobileVerifyDbObj.OTPToSend, otpValidationModel.SessionSalt);
                    if (OTPHash.Equals(otpValidationModel.EncryptedOTP.ToUpper()))
                    {
                        TimeSpan validMinutes = new TimeSpan(0, 10, 0);

                        DateTime otpSentDateTime = userMobileVerifyDbObj.OTPSentDateTime;

                        if (otpSentDateTime > DateTime.Now.Subtract(validMinutes))
                        {
                            userMobileVerifyDbObj.IsOTPVerified = true;
                            userMobileVerifyDbObj.OTPVerifiedDateTime = DateTime.Now;
                            dbContext.Entry(userMobileVerifyDbObj).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            responseModel.responseStatus = true;
                            responseModel.responseMessage = "Your mobile number has been successfully verified.";

                            UMG_UserProfile userProfileDbObj = dbContext.UMG_UserProfile.FirstOrDefault(x => x.UserID == userID);
                            if (userProfileDbObj != null)
                            {
                                userProfileDbObj.IsMobileNumVerified = true;
                                dbContext.Entry(userProfileDbObj).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                responseModel.responseStatus = false;
                                responseModel.responseMessage = "The entered OTP is invalid.";
                            }
                        }
                        else
                        {
                            responseModel.responseStatus = false;
                            responseModel.responseMessage = "The entered OTP is invalid.";
                        }
                    }
                    else
                    {
                        responseModel.responseStatus = false;
                        responseModel.responseMessage = "The entered OTP is invalid.";
                    }
                }
                else
                {
                    responseModel.responseStatus = false;
                    responseModel.responseMessage = "The entered OTP is invalid.";
                }
                return responseModel; // needs to be fixed
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }



        public SendSMSResponseModel ResendOTP(SendOTPRequestModel OTPRequest)
        {


            long otpVerificationId = 0;
            SendSMSResponseModel OTPResponse = null;
            UMG_OTP_VerificationDetails otpVerificationDbObject = null;

            try
            {
                dbContext = new KaveriEntities();
                OTPResponse = new SendSMSResponseModel();

                CommonDAL commonDalObject = new CommonDAL();

                string OTP = commonDalObject.GenerateOTP();
                OTPRequest.messageToBeSent = OTPRequest.messageToBeSent + OTP;

                UMG_UserProfile userProfileDbObj = dbContext.UMG_UserProfile.FirstOrDefault(x => x.UserID == OTPRequest.OTPRequestUserId);

                if (userProfileDbObj != null)
                {
                    OTPRequest.toContact = userProfileDbObj.MobileNumber;
                }
                else
                {
                    OTPResponse.errorCode = "1";
                    return OTPResponse;
                }

                SendSMSRequestModel SMSRequest = new SendSMSRequestModel();

                SMSRequest.messageToBeSent = OTPRequest.messageToBeSent;
                SMSRequest.toContact = OTPRequest.toContact;

                string result = SendSMS(SMSRequest);

                if (string.IsNullOrEmpty(result))
                {

                    // CHETAN - CODE TO UPDATE PREVIOUS ISNULLIFIED ENTRIES TO TRUE
                    var Obj_OTPVerificationDetail = dbContext.UMG_OTP_VerificationDetails.Where(x => x.UserId == OTPRequest.OTPRequestUserId).ToList();

                    foreach (var item in Obj_OTPVerificationDetail)
                    {
                        item.IsNullfied = true;
                        dbContext.Entry(userProfileDbObj).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                    }


                    // Encrypt OTP and insert entry into Database 

                    otpVerificationDbObject = new UMG_OTP_VerificationDetails();

                    otpVerificationId = (dbContext.UMG_OTP_VerificationDetails.Any() ? dbContext.UMG_OTP_VerificationDetails.Max(x => x.OtpVerificationID) : 0) + 1;
                    otpVerificationDbObject.OtpVerificationID = otpVerificationId;
                    otpVerificationDbObject.UserId = OTPRequest.OTPRequestUserId;



                    UMG_UserDetails userDetailsObj = dbContext.UMG_UserDetails.FirstOrDefault(x => x.UserID == OTPRequest.OTPRequestUserId);
                    if (userDetailsObj != null)
                        otpVerificationDbObject.UMG_UserDetails = userDetailsObj;




                    string encryptedOTP = SHA512Checksum.CalculateSHA512Hash(OTP);

                    otpVerificationDbObject.OTPToSend = encryptedOTP;
                    otpVerificationDbObject.OTPSentDateTime = DateTime.Now; // needs to change to response datetime

                    otpVerificationDbObject.OtpTypeId = OTPRequest.OTPTypeId;

                    UMG_OTP_VerificationDetails otpVerificationReturnObject = dbContext.UMG_OTP_VerificationDetails.Add(otpVerificationDbObject);

                    if (otpVerificationDbObject != null)
                        dbContext.SaveChanges();

                    // Encrypt OTP and insert entry into Database 
                }
                else
                {
                    OTPResponse.errorCode = "1";
                }


                // code to check if OTP sent within last 30 mins

                return OTPResponse;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}