#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   UserDetailDAL.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   14-04-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :  DAL layer to save/update/delete/retreive User details.

*/
#endregion


#region References
using ECDataAPI.Areas.UserManagement.Interface;
using CustomModels.Models.UserManagement;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomModels.Models.Common;
using Security;
using System.Transactions;
using ECDataAPI.Common;
using System.Security;
using CustomModels.Models.Alerts;
using ECDataAPI.Areas.Alerts.DAL;
#endregion

namespace ECDataAPI.Areas.UserManagement.DAL
{
    public class UserDetailDAL : IUserDetail, IDisposable
    {
        #region Properties
        private KaveriEntities dbContext = null;
        private Dictionary<String, String> decryptedParameters = null;
        private String[] encryptedParameters = null;
        #endregion

        ///// <summary>
        ///// This Method returns UserActivationModel object if PartnerDetails added successfully in DB.
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public UserActivationModel RegisterUser(UserModel model)
        //{
        //    KaveriEntities dbcontext = null;
        //    try
        //    {
        //        dbcontext = new KaveriEntities();

        //        UMG_UserProfile userProfileObj = new UMG_UserProfile();

        //        UMG_UserDetails userDetailsObj = new UMG_UserDetails();

        //        UserActivationModel useractivationmodel = new UserActivationModel();


        //        bool IsEmailExists = CheckEmailAlreadyExists(model.Email); //method returns true if emailID is alredy registered.
        //        bool isMobileNumExists = CheckMobileAlreadyExists(model.MobileNumber); //method returns true if MobileNumber is alredy registered.

        //        if (!IsEmailExists)
        //        {
        //            if (isMobileNumExists)
        //            {
        //                useractivationmodel.ResponseMessage = "Mobile Number is already registered.";

        //                return useractivationmodel; //MobileNumber already exists.

        //            }
        //            using (TransactionScope ts = new TransactionScope())
        //            {

        //                long UserId = (dbcontext.UMG_UserDetails.Any() ? dbcontext.UMG_UserDetails.Max(a => a.UserID) : 0) + 1;
        //                userDetailsObj.CreationDate = DateTime.Now;
        //                userDetailsObj.DefaultRoleID = Convert.ToInt16(ECDataAPI.Common.ApiCommonEnum.RoleDetails.OnlineUser);
        //                userDetailsObj.IsActive = false;
        //                userDetailsObj.IsLocked = false;
        //                userDetailsObj.IsFirstLogin = false;
        //                userDetailsObj.LevelID = Convert.ToInt16(ECDataAPI.Common.ApiCommonEnum.LevelDetails.Online);
        //                userDetailsObj.OfficeID = Convert.ToInt16(ECDataAPI.Common.ApiCommonEnum.OfficeIdEnum.OnlinePortal); 
        //                userDetailsObj.Password = model.Password; 
        //                userDetailsObj.UserName = model.Email;
        //                userDetailsObj.UserID = UserId;


        //                //Need to remove(02-07-18) akash
        //                userDetailsObj.IsActive = true;

        //                dbcontext.UMG_UserDetails.Add(userDetailsObj);

        //                userProfileObj.FirstName = model.FirstName;
        //                userProfileObj.LastName = model.LastName;
        //                userProfileObj.Address1 = model.Address1;
        //                userProfileObj.MobileNumber = model.MobileNumber;


        //                userProfileObj.IDProofTypeID = model.IDProofID;
        //                userProfileObj.IDProofNumber= model.IDProofNumber;

        //                userProfileObj.Pincode = model.Pincode;
        //                userProfileObj.Email = model.Email;
        //                userProfileObj.UserProfileID = (dbcontext.UMG_UserProfile.Any() ? dbcontext.UMG_UserProfile.Max(a => a.UserID) : 0) + 1;
        //                userProfileObj.UserID = UserId;
        //                userProfileObj.CountryID = 1; //need to change
        //                dbcontext.UMG_UserProfile.Add(userProfileObj);

        //                dbcontext.SaveChanges();
        //                ts.Complete();

        //                //  if user registered successfully  then return "UserActivationModel" for E-mail activation link.

        //                useractivationmodel.Email = userProfileObj.Email;
        //                useractivationmodel.EncryptedId = URLEncrypt.EncryptParameters(new string[] { "UserId=" + UserId });
        //                useractivationmodel.UserID = UserId;
        //                useractivationmodel.IsSuccessfullyInserted = true;


        //                useractivationmodel.ResponseMessage = "Account activation link has been sent to your registered email address.";

        //                return useractivationmodel;
        //            }
        //        }
        //        else
        //        {
        //            useractivationmodel.ResponseMessage = "Email address is already registered.";

        //            return useractivationmodel; //Email already exists.

        //        }
        //    }
        //    catch (Exception exception)
        //    {

        //        throw exception;
        //    }
        //    finally
        //    {
        //        if (dbcontext != null)
        //            dbcontext.Dispose();
        //    }
        //}


        /// <summary>
        /// This Method returns UserActivationModel object if PartnerDetails added successfully in DB.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserActivationModel RegisterUser(UserModel model)
        {
            KaveriEntities dbcontext = null;
            SendSMSDetailsDAL sendSMSDalObj = null; // shrinivas
            SendOTPRequestModel OTPRequest = null; // shrinivas
            try
            {
                dbcontext = new KaveriEntities();

                UMG_UserProfile userProfileObj = new UMG_UserProfile();

                UMG_UserDetails userDetailsObj = new UMG_UserDetails();

                UserActivationModel useractivationmodel = new UserActivationModel();
                UMG_PasswordHistory passwordHistoryObj = new UMG_PasswordHistory();//Added by Raman...


                bool IsEmailExists = CheckEmailAlreadyExists(model.Email); //method returns true if emailID is alredy registered.
                bool isMobileNumExists = CheckMobileAlreadyExists(model.MobileNumber); //method returns true if MobileNumber is alredy registered.

                if (!IsEmailExists)
                {
                    if (isMobileNumExists)
                    {
                        useractivationmodel.ResponseMessage = "Mobile Number is already registered.";

                        return useractivationmodel; //MobileNumber already exists.

                    }
                    //Added By Raman Kalegaonkar on 04-01-2019
                    if (dbcontext.UMG_UserProfile.Any(u => u.IDProofTypeID == model.IDProofID && u.IDProofNumber == model.IDProofNumber))
                    {
                        useractivationmodel.ResponseMessage = "User is present for this IDProof. Please try another IDProof Number";

                        return useractivationmodel; //IdProofID and IDProofNumber already exists.
                    }

                    using (TransactionScope ts = new TransactionScope())
                    {

                        long UserId = (dbcontext.UMG_UserDetails.Any() ? dbcontext.UMG_UserDetails.Max(a => a.UserID) : 0) + 1;
                        userDetailsObj.CreationDate = DateTime.Now;
                        userDetailsObj.DefaultRoleID = Convert.ToInt16(ECDataAPI.Common.ApiCommonEnum.RoleDetails.OnlineUser);
                        userDetailsObj.IsActive = false;
                        userDetailsObj.IsLocked = false;
                        userDetailsObj.IsFirstLoginDone = false;
                        userDetailsObj.LevelID = Convert.ToInt16(ECDataAPI.Common.ApiCommonEnum.LevelDetails.Others);
                        userDetailsObj.OfficeID = Convert.ToInt16(ECDataAPI.Common.ApiCommonEnum.OfficeIdEnum.OnlinePortal);
                        userDetailsObj.Password = model.Password;
                        userDetailsObj.UserName = model.Email;
                        userDetailsObj.PasswordChangeDate = DateTime.Now;
                        userDetailsObj.UserID = UserId;


                        //Need to remove(02-07-18) akash
                        userDetailsObj.IsActive = true;

                        dbcontext.UMG_UserDetails.Add(userDetailsObj);

                        userProfileObj.FirstName = model.FirstName;
                        userProfileObj.LastName = model.LastName;
                        userProfileObj.Address1 = model.Address1;
                        userProfileObj.MobileNumber = model.MobileNumber;


                        userProfileObj.IDProofTypeID = model.IDProofID;
                        userProfileObj.IDProofNumber = model.IDProofNumber;

                        userProfileObj.Pincode = model.Pincode;
                        userProfileObj.Email = model.Email;
                        userProfileObj.UserProfileID = (dbcontext.UMG_UserProfile.Any() ? dbcontext.UMG_UserProfile.Max(a => a.UserID) : 0) + 1;
                        userProfileObj.UserID = UserId;
                        userProfileObj.CountryID = (short)model.CountryID; //need to change
                        userProfileObj.IsMobileNumVerified = false; //Need to change (Changed on 13-02-2019)
                        dbcontext.UMG_UserProfile.Add(userProfileObj);

                        //Added by Raman...
                        passwordHistoryObj.Password = model.Password;
                        passwordHistoryObj.ChangeDateTime = DateTime.Now;
                        passwordHistoryObj.UserID = UserId;
                        passwordHistoryObj.ID = dbcontext.UMG_PasswordHistory.Max(ad => (Int64?)ad.ID) == null ? 1 : dbcontext.UMG_PasswordHistory.Max(ad => (Int64)ad.ID) + 1;
                        dbcontext.UMG_PasswordHistory.Add(passwordHistoryObj);


                        dbcontext.SaveChanges();
                        ts.Complete();

                        //  if user registered successfully  then return "UserActivationModel" for E-mail activation link.

                        useractivationmodel.Email = userProfileObj.Email;
                        useractivationmodel.EncryptedId = URLEncrypt.EncryptParameters(new string[] { "UserId=" + UserId });
                        useractivationmodel.UserID = UserId;
                        useractivationmodel.IsSuccessfullyInserted = true;

                        useractivationmodel.ResponseMessage = "Account activation link has been sent to your registered email address.";


                    }

                    // sending OTP -- shrinivas

                    sendSMSDalObj = new SendSMSDetailsDAL();
                    OTPRequest = new SendOTPRequestModel();

                    OTPRequest.OTPRequestEncryptedUId = useractivationmodel.EncryptedId;
                    OTPRequest.messageToBeSent = "OTP to verify your mobile number is ";
                    OTPRequest.OTPTypeId = 1;

                    //SendSMSResponseModel OTPResponse = sendSMSDalObj.SendOTP(OTPRequest);

                    //if (OTPResponse.errorCode.Equals("0"))
                    //{
                    //    useractivationmodel.IsOTPSent = 1;
                    //}
                    //else
                    //{
                    //    useractivationmodel.IsOTPSent = 2;
                    //}

                    // sending OTP -- shrinivas
                }
                else
                {
                    useractivationmodel.ResponseMessage = "Email address is already registered.";



                }
                return useractivationmodel; //Email already exists.


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (dbcontext != null)
                    dbcontext.Dispose();
            }
        }


        /// <summary>
        /// This method returns boolean value TRUE if UserName alredy exists.
        /// </summary>
        /// <param name="encryptedID"></param>
        /// <returns></returns>
        public Boolean CheckUserNameAvailability(string encryptedID)
        {
            try
            {
                encryptedParameters = encryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                string userName = (decryptedParameters["userName"].ToString().Trim()).ToString();

                dbContext = new KaveriEntities();
                //  bool result=dbContext.UMG_UserDetails.Where(m => m.UserName.ToLower() == userName.ToLower()).Any();

                return dbContext.UMG_UserDetails.Where(m => m.UserName.ToLower() == userName.ToLower()).Any();
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



        /// <summary>
        /// This method returns LoginResponseModel object if logged in successfully.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public LoginResponseModel UserLogin(LoginViewModel model)
        {
            KaveriEntities dbcontext = null;
            try
            {
                dbcontext = new KaveriEntities();

                LoginResponseModel responseObject = new LoginResponseModel();
                UMG_UserDetails userDetailObject = null;

                try
                {
                    userDetailObject = dbcontext.UMG_UserDetails.Where(a => a.UserName.Equals(model.EmailId)).FirstOrDefault();

                }
                catch (Exception e)
                {
                    ApiCommonFunctions.WriteDebugLogInDB((int)ApiCommonEnum.FunctionalityMaster.Login, e.GetBaseException().Message, "UserDetailDAL", "UserLogin");
                    ApiExceptionLogs.LogError(e);
                    responseObject.ResponseMessage = "Error occured while fetching user details.";
                    return responseObject;
                }

                //UMG_UserDetails userDetailObject = dbcontext.UMG_UserDetails.Where(a => a.UserName==(model.EmailId) && a.Password==(model.Password)).FirstOrDefault();

                //UMG_UserDetails userDetailObject = dbcontext.UMG_UserDetails.Where(a => a.UserName==(model.EmailId) && string.Equals(  a.Password,(model.Password),StringComparison.OrdinalIgnoreCase)).FirstOrDefault();





                if (userDetailObject != null)
                {
                    bool isRoleActive = dbcontext.UMG_RoleDetails.Where(a => a.RoleID == userDetailObject.DefaultRoleID).Select(x => x.IsActive).FirstOrDefault();

                    //if (userDetailObject.IsActive  )
                    // changed by m rafe on 24-12-19
                    if (userDetailObject.IsActive && isRoleActive)

                    {

                        #region Commented by shubham bhagat on 10-4-2019 requirement change
                        //if (userDetailObject.DefaultRoleID == (short)ApiCommonEnum.RoleDetails.SR)
                        //{
                        //    MAS_RegisteredResourcesMacAddress resourceObject = dbcontext.MAS_RegisteredResourcesMacAddress.Where(a => a.UserID == userDetailObject.UserID).FirstOrDefault();

                        //    if (resourceObject == null)
                        //    {
                        //        new Exception("Resource details doesn't exist for this user. ");
                        //    }
                        //    else
                        //    {
                        //        responseObject.ResourceID = resourceObject.ResourceID; // ask mam, what if their are multiple resources.
                        //    }

                        //}
                        #endregion

                        UMG_UserProfile userProfileObject = dbcontext.UMG_UserProfile.Where(a => a.UserID == userDetailObject.UserID).FirstOrDefault();


                        responseObject.DefaultRoleId = userDetailObject.DefaultRoleID;
                        responseObject.LevelId = userDetailObject.LevelID;
                        responseObject.OfficeId = userDetailObject.OfficeID;
                        responseObject.UserID = userDetailObject.UserID;
                        responseObject.Password = userDetailObject.Password;
                        responseObject.DefaultRoleId = userDetailObject.DefaultRoleID;
                        responseObject.UserFullName = userProfileObject.FirstName + " " + userProfileObject.LastName;
                        //responseObject.IsFirstLogin = userDetailObject.IsFirstLogin;

                        responseObject.ResponseMessage = string.Empty;
                    }
                    else
                    {

                        responseObject.ResponseMessage = "Your account is not activated.";

                    }
                }
                else
                {
                    responseObject.ResponseMessage = "Invalid credentials.";

                }
                return responseObject;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                if (dbcontext != null)
                    dbcontext.Dispose();
            }
        }

        /// <summary>
        /// This method returns UserActivationModel object if email address present in DB and user is not Activated.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserActivationModel AccountActivation(UserActivationModel model)
        {
            KaveriEntities dbcontext = null;
            try
            {
                dbcontext = new KaveriEntities();
                UserActivationModel useractivationmodel = new UserActivationModel();

                UMG_UserProfile userObject = dbcontext.UMG_UserProfile.Where(x => x.Email == model.Email).FirstOrDefault();

                if (userObject == null)
                {

                    useractivationmodel.ResponseMessage = "Email address is not registered.";
                    return useractivationmodel; // if emailID is not registered.
                }
                else
                {
                    UMG_UserDetails userDetailObject = dbcontext.UMG_UserDetails.Where(x => x.UserID == userObject.UserID).FirstOrDefault();

                    if (userDetailObject.IsActive)
                    {
                        useractivationmodel.ResponseMessage = "Your account is already activated.";
                        return useractivationmodel;//if account is already activated.
                    }
                    else
                    {
                        useractivationmodel.Email = userObject.Email;
                        useractivationmodel.EncryptedId = URLEncrypt.EncryptParameters(new string[] { "UserId=" + userDetailObject.UserID });
                        useractivationmodel.UserID = userDetailObject.UserID;
                        useractivationmodel.IsSuccessfullyInserted = true;
                        useractivationmodel.ResponseMessage = "Account activation link has been sent to your registered email address. ";
                    }
                }


                return useractivationmodel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbcontext != null)
                    dbcontext.Dispose();
            }
        }

        /// <summary>
        /// This method returns UserActivationModel object if email is correct in case of ForgotPassword.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserActivationModel ForgotPassword(UserActivationModel model)
        {
            KaveriEntities dbcontext = null;
            UserActivationModel useractivationmodel = new UserActivationModel();
            try
            {
                dbcontext = new KaveriEntities();


                UMG_UserDetails userObject = dbcontext.UMG_UserDetails.Where(x => x.UserName == model.Email).FirstOrDefault();
                if (userObject == null)
                {
                    useractivationmodel.ResponseMessage = "Invalid username , Please enter valid username.";
                    useractivationmodel.IsRegisteredEmail = false;
                    return useractivationmodel; // if emailID is not registered.
                }
                else
                {
                    UMG_UserDetails userDetailObject = dbcontext.UMG_UserDetails.Where(x => x.UserID == userObject.UserID).FirstOrDefault();
                    useractivationmodel.Email = userObject.UserName;
                    useractivationmodel.IsRegisteredEmail = true;
                    useractivationmodel.EncryptedId = URLEncrypt.EncryptParameters(new string[] { "UserId=" + userDetailObject.UserID });
                    useractivationmodel.UserID = userDetailObject.UserID;
                    useractivationmodel.IsSuccessfullyInserted = true;

                    useractivationmodel.ResponseMessage = "To change password a link has been sent to your registered email address.";
                }
                return useractivationmodel;
            }
            catch (Exception e)
            {
                ApiCommonFunctions.WriteDebugLogInDB((int)ApiCommonEnum.FunctionalityMaster.ForgotPassword, e.GetBaseException().Message, "UserDetailDAL", "ForgotPassword");
                ApiExceptionLogs.LogError(e);
                useractivationmodel.ResponseMessage = "Error occured while fetching user details.";
                useractivationmodel.IsRegisteredEmail = false;
                return useractivationmodel;
            }
            finally
            {
                if (dbcontext != null)
                    dbcontext.Dispose();
            }
        }


        /// <summary>
        /// This method returns empty string if e-mail has been sent successfully.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SendEmail(EmailModel model)
        {
            KaveriEntities dbcontext = null;
            try
            {

                string responseMessage = string.Empty;

                if (model != null)
                {
                    dbcontext = new KaveriEntities();

                    //    UMG_UserProfile userObject = dbcontext.UMG_UserProfile.Where(x => x.Email == model.RecepientAddress).FirstOrDefault();

                    //   bool IsEmailExists = dbcontext.UMG_UserProfile.Any(x => x.Email == model.RecepientAddress);



                    //                    var userdetails = dbcontext.UMG_UserDetails.Where(x => x.UserName == model.RecepientAddress).FirstOrDefault();



                    long UserID = dbcontext.UMG_UserDetails.Where(x => x.UserName == model.RecepientAddress).Select(x => x.UserID).FirstOrDefault();


                    var userProfile = dbcontext.UMG_UserProfile.Where(x => x.UserID == UserID).FirstOrDefault();






                    if (userProfile != null)
                    {
                        model.RecepientAddress = userProfile.Email;

                        string result = ApiCommonFunctions.SendMail(model); //Static Method for sending user activation link.

                        if (string.IsNullOrEmpty(result))
                        {
                            responseMessage = string.Empty;//success...!
                        }
                        else
                        {
                            responseMessage = result; // "Unable to send email"; //Error
                        }
                    }
                    else
                    {
                        responseMessage = "Email address (username) is not registered.";
                    }
                    return responseMessage;
                }
                else
                {
                    return responseMessage = "Invalid Parameters."; //need to change
                }

            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// This method returns string messages depending on  user is activated or not.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string AccountActivationByLink(string Id)
        {
            KaveriEntities dbcontext = null;
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    dbcontext = new KaveriEntities();

                    String[] encryptedParameters = null;

                    encryptedParameters = Id.Split('/');

                    if (encryptedParameters.Length != 3)
                        throw new Exception("URL tempered.");



                    Dictionary<string, string> dict = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });


                    string decryptedId = dict["UserId"];

                    long UserId = Convert.ToInt64(decryptedId);

                    UMG_UserDetails userObject = dbcontext.UMG_UserDetails.Where(x => x.UserID == UserId).FirstOrDefault();

                    if (userObject == null)
                    {
                        return " <div class='alert alert-danger fade in'><h3> <strong> Error!</strong> Email address is not registered. </h3></div>";
                    }
                    else
                    {
                        if (userObject.IsActive == true)
                        {
                            return " <div class='alert alert-info fade in'><h3> <strong> Success!</strong> Your account is already activated. </h3></div>";
                        }
                        userObject.IsActive = true;

                        dbcontext.Entry(userObject).State = System.Data.Entity.EntityState.Modified;
                        dbcontext.SaveChanges();
                    }

                    return " <div class='alert alert-success fade in'><h3> <strong> Success!</strong>Your account has been successfully activated. </h3></div>";
                }
                else
                {
                    return " <div class='alert alert-danger fade in'><h3> <strong> Error!</strong>  Account activation failed due to Invalid parameters. </h3></div>";
                }

            }
            catch (Exception)
            {
                throw;
            }


        }

        /// <summary>
        /// This method returns string messages depending on password has been changed or  Not.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string ForgotPasswordByLink(ChangePasswordModel model)
        {
            KaveriEntities dbcontext = null;
            try
            {
                if (model != null)
                {
                    dbcontext = new KaveriEntities();

                    String[] encryptedParameters = null;

                    encryptedParameters = model.Id.Split('/');

                    if (encryptedParameters.Length != 3)
                        throw new Exception("URL tempered.");



                    Dictionary<string, string> dict = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });


                    string decryptedId = dict["UserId"];

                    long UserId = Convert.ToInt64(decryptedId);

                    UMG_UserDetails userObject = dbcontext.UMG_UserDetails.Where(x => x.UserID == UserId).FirstOrDefault();

                    if (userObject == null)
                    {
                        return " <div class='alert alert-danger fade in'><h3> <strong> Error!</strong> Given Id is not registered. </h3></div>";
                    }
                    else
                    {

                        userObject.Password = model.Password; //Change password.

                        dbcontext.Entry(userObject).State = System.Data.Entity.EntityState.Modified;
                        dbcontext.SaveChanges();

                        return " <div class='alert alert-success fade in'><h3> <strong> Success!</strong>Your password has been successfully changed. </h3></div>";

                    }

                }
                else
                {
                    return " <div class='alert alert-danger fade in'><h3> <strong> Error!</strong> Change Password failed due to Invalid parameters  </h3></div>";
                }
                // return " <div class='alert alert-success fade in'><h3> <strong> Success!</strong>Your password has been successfully changed. </h3></div>";

            }
            catch (Exception)
            {
                throw;
            }


        }

        /// <summary>
        /// This method returns true if email is alredy registered.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckEmailAlreadyExists(string email)
        {
            KaveriEntities dbcontext = null;
            bool IsEmailExists = false;
            try
            {
                dbcontext = new KaveriEntities();
                IsEmailExists = dbcontext.UMG_UserProfile.Any(c => c.Email == email); //Ckeck if email alredy registered or not.
                                                                                      // if()
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (dbcontext != null)
                    dbcontext.Dispose();
            }
            return IsEmailExists;
        }

        /// <summary>
        /// This method returns true if mobile number is alredy registered.
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool CheckMobileAlreadyExists(string mobile)
        {
            KaveriEntities dbcontext = null;
            bool isExist = false;
            try
            {
                dbcontext = new KaveriEntities();
                if (!string.IsNullOrEmpty(mobile))
                {
                    isExist = dbcontext.UMG_UserProfile.Any(c => c.MobileNumber == mobile); //Ckeck if email alredy registered or not.
                    return isExist;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (dbcontext != null)
                    dbcontext.Dispose();
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