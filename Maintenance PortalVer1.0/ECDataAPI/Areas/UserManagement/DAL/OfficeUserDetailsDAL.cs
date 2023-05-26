using CustomModels.Models.Common;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.DAL
{
    public class OfficeUserDetailsDAL : IOfficeUser, IDisposable
    {
        #region Properties
        private KaveriEntities dbContext = null;
        private Dictionary<String, String> decryptedParameters = null;
        private String[] encryptedParameters = null;
        #endregion

        /// <summary>
        /// This Method returns UserActivationModel object if PartnerDetails added successfully in DB.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></retulrns>
        public UserActivationModel RegisterUser(OfficeUserDetailsModel model)
        {
            KaveriEntities dbcontext = null;
            try
            {
                dbcontext = new KaveriEntities();

                UMG_UserProfile userProfileObj = new UMG_UserProfile();

                UMG_UserDetails userDetailsObj = new UMG_UserDetails();

                UserActivationModel useractivationmodel = new UserActivationModel();

                // commented by shubham bhagat on 18-04-2019
                //bool IsEmailExists = CheckEmailAlreadyExists(model.Email); //method returns true if emailID is alredy registered.

                // Added by shubham bhagat on 18-04-2019
                String encryptedIDForUsername = URLEncrypt.EncryptParameters(new String[] { "userName=" + model.Username });
                bool IsUserNameExists = CheckUserNameAvailability(encryptedIDForUsername);//method returns true if Username is already registered.

                // commented by shubham bhagat on 18-04-2019
                //bool isMobileNumExists = CheckMobileAlreadyExists(model.MobileNumber); //method returns true if MobileNumber is alredy registered.

                //if (!IsEmailExists)
                //{

                if (!IsUserNameExists)
                {
                    // commented by shubham bhagat on 18-04-2019
                    //if (isMobileNumExists)
                    //{
                    //    useractivationmodel.ResponseMessage = "Mobile Number is already registered.";

                    //    return useractivationmodel; //MobileNumber already exists.

                    //}
                    //Added by Raman Kalegaonkar on 04-01-2019
                    if (dbcontext.UMG_UserProfile.Any(u => u.IDProofTypeID == model.IDProofID && u.IDProofNumber == model.IDProofNumber))
                    {
                        useractivationmodel.ResponseMessage = "User is present for this IDProof. Please try another IDProof Number";

                        return useractivationmodel; //IdProofID and IDProofNumber already exists.
                    }


                    using (TransactionScope ts = new TransactionScope())
                    {
                        // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
                        encryptedParameters = model.OfficeID.Split('/');
                        if (!(encryptedParameters.Length == 3))
                            throw new SecurityException("URL Tempered");

                        decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        short officeEncryptId = Convert.ToInt16(decryptedParameters["EncryptOfficeID"].ToString().Trim());

                        // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
                        encryptedParameters = model.RoleID.Split('/');
                        if (!(encryptedParameters.Length == 3))
                            throw new SecurityException("URL Tempered");

                        decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        short roleEncryptId = Convert.ToInt16(decryptedParameters["EncryptRoleID"].ToString().Trim());

                        // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
                        encryptedParameters = model.LevelID.Split('/');
                        if (!(encryptedParameters.Length == 3))
                            throw new SecurityException("URL Tempered");

                        decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        short levelEncryptId = Convert.ToInt16(decryptedParameters["EncryptLevelID"].ToString().Trim());


                        #region Validation by shubham Bhagat on 5-4-2019
                        var officeExists = dbcontext.MAS_OfficeMaster.Where(x => x.OfficeID == officeEncryptId).Any();
                        if (!officeExists)
                        {
                            useractivationmodel.IsUpdatedSuccessfully = false;
                            useractivationmodel.ResponseMessage = "Office not found.";
                            return useractivationmodel;
                        }

                        var roleExists = dbcontext.UMG_RoleDetails.Where(x => x.RoleID == roleEncryptId).Any();
                        if (!roleExists)
                        {
                            useractivationmodel.IsUpdatedSuccessfully = false;
                            useractivationmodel.ResponseMessage = "Role not found.";
                            return useractivationmodel;
                        }
                        var levelExists = dbcontext.UMG_LevelDetails.Where(x => x.LevelID == levelEncryptId).Any();
                        if (!levelExists)
                        {
                            useractivationmodel.IsUpdatedSuccessfully = false;
                            useractivationmodel.ResponseMessage = "Level not found.";
                            return useractivationmodel;
                        }
                        #endregion


                        // Added by Shubham Bhagat on 5-1-2019
                        //if ()
                        //{

                        //}


                        long UserId = (dbcontext.UMG_UserDetails.Any() ? dbcontext.UMG_UserDetails.Max(a => a.UserID) : 0) + 1;
                        userDetailsObj.CreationDate = DateTime.Now;
                        userDetailsObj.DefaultRoleID = roleEncryptId;
                        userDetailsObj.IsActive = model.IsActive;
                        userDetailsObj.IsLocked = false;
                        userDetailsObj.IsFirstLoginDone = false;
                        userDetailsObj.LevelID = levelEncryptId;
                        userDetailsObj.OfficeID = officeEncryptId;
                        userDetailsObj.Password = model.Password;
                        userDetailsObj.UserName = model.Username;
                        userDetailsObj.UserID = UserId;
                        dbcontext.UMG_UserDetails.Add(userDetailsObj);

                        // For Activity Log                      
                        String messageForActivityLog = "User Details Added # " + model.Email + "- UserDetails Added. ";

                        userProfileObj.FirstName = model.FirstName;
                        userProfileObj.LastName = model.LastName;
                        userProfileObj.Address1 = model.Address1;
                        userProfileObj.MobileNumber = model.MobileNumber;
                        userProfileObj.IsMobileNumVerified = false;

                        userProfileObj.IDProofTypeID = model.IDProofID;
                        userProfileObj.IDProofNumber = model.IDProofNumber;

                        userProfileObj.Pincode = model.Pincode;
                        userProfileObj.Email = model.Email;
                        userProfileObj.UserProfileID = (dbcontext.UMG_UserProfile.Any() ? dbcontext.UMG_UserProfile.Max(a => a.UserID) : 0) + 1;
                        userProfileObj.UserID = UserId;
                        userProfileObj.CountryID = 1; //need to change
                        dbcontext.UMG_UserProfile.Add(userProfileObj);

                        // For Activity Log
                        messageForActivityLog = messageForActivityLog + "User Name :" + model.FirstName + " " + model.LastName + ".";

                        dbcontext.SaveChanges();
                        // For Activity Log
                        //String messageForActivityLog = model.OfficeNameE + " Office Detail Added.";
                        if (messageForActivityLog.Length < 1000)
                            ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.UserDetail), messageForActivityLog);
                        else
                        {
                            messageForActivityLog = messageForActivityLog.Substring(0, 999);
                            ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.UserDetail), messageForActivityLog);
                        }



                        ts.Complete();

                        //  if user registered successfully  then return "UserActivationModel" for E-mail activation link.

                        useractivationmodel.Email = userProfileObj.Email;
                        useractivationmodel.EncryptedId = URLEncrypt.EncryptParameters(new string[] { "UserID=" + UserId });
                        useractivationmodel.UserID = UserId;
                        useractivationmodel.IsSuccessfullyInserted = true;


                        useractivationmodel.ResponseMessage = "Account activation link has been sent to your registered email address.";

                        return useractivationmodel;
                    }
                }
                else
                {
                    useractivationmodel.ResponseMessage = "Username is already registered.";

                    return useractivationmodel; //Email already exists.

                }
                //}
                //else
                //{
                //    useractivationmodel.ResponseMessage = "Email address is already registered.";
                //    return useractivationmodel; //Email already exists.
                //}
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

                return dbContext.UMG_UserDetails.Where(m => m.UserName.ToLower() == userName.ToLower() && m.DefaultRoleID != 2).Any();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // commented by shubham bhagat on 18-04-2019
                //if (null != dbContext)
                //    dbContext.Dispose();
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

        public UserActivationModel UpdateOfficeUser(OfficeUserDetailsModel model)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {

                UMG_UserProfile userProfileObj = new UMG_UserProfile();
                UMG_UserDetails userDetailsObj = new UMG_UserDetails();
                UserActivationModel useractivationmodel = new UserActivationModel();

                encryptedParameters = model.EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int UserId = Convert.ToInt32(decryptedParameters["UserID"].ToString().Trim());
                //Added By Raman Kalegaonkar on 04-01-2019
                if (dbKaveriOnlineContext.UMG_UserProfile.Where(u => u.UserID != model.UserID).ToList().Any(u => u.IDProofTypeID == model.IDProofID && u.IDProofNumber == model.IDProofNumber))
                {
                    useractivationmodel.ResponseMessage = "User is present for this IDProof. Please try another IDProof Number";

                    return useractivationmodel; //IdProofID and IDProofNumber already exists.
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    //-------------- For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019--------------
                    // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
                    encryptedParameters = model.OfficeID.Split('/');
                    if (!(encryptedParameters.Length == 3))
                        throw new SecurityException("URL Tempered");

                    decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    short officeEncryptId = Convert.ToInt16(decryptedParameters["EncryptOfficeID"].ToString().Trim());

                    // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
                    encryptedParameters = model.RoleID.Split('/');
                    if (!(encryptedParameters.Length == 3))
                        throw new SecurityException("URL Tempered");

                    decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    short roleEncryptId = Convert.ToInt16(decryptedParameters["EncryptRoleID"].ToString().Trim());

                    // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
                    encryptedParameters = model.LevelID.Split('/');
                    if (!(encryptedParameters.Length == 3))
                        throw new SecurityException("URL Tempered");

                    decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    short levelEncryptId = Convert.ToInt16(decryptedParameters["EncryptLevelID"].ToString().Trim());
                    //-------------- For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019--------------


                    #region Validation by shubham Bhagat on 5-4-2019
                    var officeExists = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeEncryptId).Any();
                    if (!officeExists)
                    {
                        useractivationmodel.IsUpdatedSuccessfully = false;
                        useractivationmodel.ResponseMessage = "Office not found.";
                        return useractivationmodel;
                    }

                    var roleExists = dbKaveriOnlineContext.UMG_RoleDetails.Where(x => x.RoleID == roleEncryptId).Any();
                    if (!roleExists)
                    {
                        useractivationmodel.IsUpdatedSuccessfully = false;
                        useractivationmodel.ResponseMessage = "Role not found.";
                        return useractivationmodel;
                    }
                    var levelExists = dbKaveriOnlineContext.UMG_LevelDetails.Where(x => x.LevelID == levelEncryptId).Any();
                    if (!levelExists)
                    {
                        useractivationmodel.IsUpdatedSuccessfully = false;
                        useractivationmodel.ResponseMessage = "Level not found.";
                        return useractivationmodel;
                    }
                    #endregion

                    //-------------- For encrypting Id used in dropdown commented by Raman on 5-1-2019--------------
                    //Raman
                    userDetailsObj = dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.UserID == UserId).FirstOrDefault();
                    //Raman
                    userProfileObj = dbKaveriOnlineContext.UMG_UserProfile.Where(x => x.UserID == UserId).FirstOrDefault();

                    //Compare UMG_UserProfile object
                    OfficeUserDetailsModel UserProfileObjForComparison1 = new OfficeUserDetailsModel();

                    UserProfileObjForComparison1.FirstName = model.FirstName;
                    UserProfileObjForComparison1.LastName = model.LastName;
                    UserProfileObjForComparison1.MobileNumber = model.MobileNumber;
                    UserProfileObjForComparison1.Email = model.Email;
                    UserProfileObjForComparison1.Address1 = model.Address1;
                    UserProfileObjForComparison1.Pincode = model.Pincode;
                    UserProfileObjForComparison1.IDProofID = model.IDProofID;
                    UserProfileObjForComparison1.IDProofNumber = model.IDProofNumber;
                    UserProfileObjForComparison1.IsActive = model.IsActive;
                    UserProfileObjForComparison1.OfficeID = officeEncryptId.ToString();

                    OfficeUserDetailsModel UserProfileObjForComparison2 = new OfficeUserDetailsModel();

                    UserProfileObjForComparison2.FirstName = userProfileObj.FirstName;
                    UserProfileObjForComparison2.LastName = userProfileObj.LastName;
                    UserProfileObjForComparison2.MobileNumber = userProfileObj.MobileNumber;
                    UserProfileObjForComparison2.Email = userProfileObj.Email;
                    UserProfileObjForComparison2.Address1 = userProfileObj.Address1;
                    UserProfileObjForComparison2.Pincode = userProfileObj.Pincode;
                    UserProfileObjForComparison2.IDProofID = userProfileObj.IDProofTypeID ?? 0;
                    UserProfileObjForComparison2.IDProofNumber = userProfileObj.IDProofNumber;
                    UserProfileObjForComparison2.IsActive = userDetailsObj.IsActive;
                    UserProfileObjForComparison2.OfficeID = userDetailsObj.OfficeID.ToString();

                    bool IsObjectsSame = ApiCommonFunctions.CompareObjectsBeforeUpdate<OfficeUserDetailsModel>(UserProfileObjForComparison1, UserProfileObjForComparison2);
                    //**************


                    if (IsObjectsSame)
                    {
                        //Same obj
                        useractivationmodel.IsUpdatedSuccessfully = true;
                        useractivationmodel.ResponseMessage = "No change in office user record";
                        return useractivationmodel;
                    }
                    else
                    {


                        //Different object obj
                        // For Logging before update on 5-1-2019 By Shubham Bhagat  
                        UMG_UserProfile_Log uMG_UserProfile_Log = new UMG_UserProfile_Log();
                        uMG_UserProfile_Log.LogID = (dbKaveriOnlineContext.UMG_UserProfile_Log.Any() ? dbKaveriOnlineContext.UMG_UserProfile_Log.Max(x => x.LogID) : 0) + 1;
                        uMG_UserProfile_Log.UserProfileID = userProfileObj.UserProfileID;
                        uMG_UserProfile_Log.UserID = userProfileObj.UserID;
                        uMG_UserProfile_Log.FirstName = userProfileObj.FirstName;
                        uMG_UserProfile_Log.MiddleName = userProfileObj.MiddleName;
                        uMG_UserProfile_Log.LastName = userProfileObj.LastName;
                        uMG_UserProfile_Log.GenderID = userProfileObj.GenderID;
                        uMG_UserProfile_Log.Address1 = userProfileObj.Address1;
                        uMG_UserProfile_Log.Address2 = userProfileObj.Address2;
                        uMG_UserProfile_Log.CountryID = userProfileObj.CountryID;
                        uMG_UserProfile_Log.Pincode = userProfileObj.Pincode;
                        uMG_UserProfile_Log.MobileNumber = userProfileObj.MobileNumber;
                        uMG_UserProfile_Log.Email = userProfileObj.Email;
                        uMG_UserProfile_Log.PAN = userProfileObj.PAN;
                        uMG_UserProfile_Log.EPIC = userProfileObj.EPIC;
                        uMG_UserProfile_Log.UID = userProfileObj.UID;
                        uMG_UserProfile_Log.ThumbTemplate = userProfileObj.ThumbTemplate;
                        uMG_UserProfile_Log.ThumbMinutae = userProfileObj.ThumbMinutae;
                        uMG_UserProfile_Log.FingerID = userProfileObj.FingerID;
                        uMG_UserProfile_Log.ProfileLatestUpdateDate = userProfileObj.ProfileLatestUpdateDate;
                        uMG_UserProfile_Log.PhotoVirtualPath = userProfileObj.PhotoVirtualPath;
                        uMG_UserProfile_Log.ThumbVirtualPath = userProfileObj.ThumbVirtualPath;
                        uMG_UserProfile_Log.PhotoFilePath = userProfileObj.PhotoFilePath;
                        uMG_UserProfile_Log.ThumbFilePath = userProfileObj.ThumbFilePath;
                        uMG_UserProfile_Log.ResourceID = userProfileObj.ResourceID;
                        uMG_UserProfile_Log.IDProofTypeID = userProfileObj.IDProofTypeID;
                        uMG_UserProfile_Log.IDProofNumber = userProfileObj.IDProofNumber;
                        uMG_UserProfile_Log.IsMobileNumVerified = userProfileObj.IsMobileNumVerified;
                        uMG_UserProfile_Log.UpdateDateTime = System.DateTime.Now;
                        #region 5-4-2019 For Table LOG by SB
                        uMG_UserProfile_Log.UserIDLog = model.UserIdForActivityLogFromSession;
                        uMG_UserProfile_Log.UserIPAddress = model.UserIPAddress;
                        uMG_UserProfile_Log.ActionPerformed = "Update";
                        #endregion
                        dbKaveriOnlineContext.UMG_UserProfile_Log.Add(uMG_UserProfile_Log);




                        // For Logging before update on 5-1-2019 By Shubham Bhagat  
                        UMG_UserDetails_Log uMG_UserDetails_Log = new UMG_UserDetails_Log();

                        uMG_UserDetails_Log.LogID = (dbKaveriOnlineContext.UMG_UserDetails_Log.Any() ? dbKaveriOnlineContext.UMG_UserDetails_Log.Max(x => x.LogID) : 0) + 1;
                        uMG_UserDetails_Log.UserID = userDetailsObj.UserID;
                        uMG_UserDetails_Log.UserName = userDetailsObj.UserName;
                        uMG_UserDetails_Log.Password = userDetailsObj.Password;
                        uMG_UserDetails_Log.DefaultRoleID = (short)userDetailsObj.DefaultRoleID;
                        uMG_UserDetails_Log.LevelID = (short)userDetailsObj.LevelID;
                        uMG_UserDetails_Log.OfficeID = (short)userDetailsObj.OfficeID;
                        uMG_UserDetails_Log.IsActive = userDetailsObj.IsActive;
                        uMG_UserDetails_Log.IsLocked = userDetailsObj.IsLocked;
                        uMG_UserDetails_Log.IsFirstLoginDone = userDetailsObj.IsFirstLoginDone;
                        uMG_UserDetails_Log.FailedPasswordAttempts = userDetailsObj.FailedPasswordAttempts;
                        uMG_UserDetails_Log.PasswordChangeDate = userDetailsObj.PasswordChangeDate;
                        uMG_UserDetails_Log.LastLoginDate = userDetailsObj.LastLoginDate;
                        uMG_UserDetails_Log.CreationDate = userDetailsObj.CreationDate;
                        uMG_UserDetails_Log.Remarks = userDetailsObj.Remarks;
                        uMG_UserDetails_Log.UpdateDateTime = System.DateTime.Now;
                        #region 5-4-2019 For Table LOG by SB
                        uMG_UserDetails_Log.UserIDLog = model.UserIdForActivityLogFromSession;
                        uMG_UserDetails_Log.UserIPAddress = model.UserIPAddress;
                        uMG_UserDetails_Log.ActionPerformed = "Update";
                        #endregion
                        dbKaveriOnlineContext.UMG_UserDetails_Log.Add(uMG_UserDetails_Log);
                        //-------------- For encrypting Id used in dropdown commented by Raman on 5-1-2019--------------
                        String messageForActivityLog = "User Details Updated # " + model.Email + "- User Details Updated. ";

                        // userDetailsObj = dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.UserID == UserId).FirstOrDefault();
                        //userDetailsObj.CreationDate = DateTime.Now;
                        userDetailsObj.DefaultRoleID = roleEncryptId;
                        userDetailsObj.IsActive = model.IsActive;
                        userDetailsObj.IsLocked = false;
                        //On 9 - 4 - 2019 by Shubham Bhagat for not changing flag
                        //userDetailsObj.IsFirstLogin = false;
                        userDetailsObj.LevelID = levelEncryptId;
                        userDetailsObj.OfficeID = officeEncryptId;
                        // userDetailsObj.Password = model.Password;
                        // For Activity Log
                        if (!(userDetailsObj.UserName.Equals(model.Email)))
                        {
                            messageForActivityLog = messageForActivityLog + " Old UserName(Email) : " + userDetailsObj.UserName + ". New UserName(Email) : " + model.Email + ". ";
                        }
                        // userDetailsObj.UserName = model.Email;

                        // userProfileObj = dbKaveriOnlineContext.UMG_UserProfile.Where(x => x.UserID == UserId).FirstOrDefault();
                        if (!(userProfileObj.FirstName.Equals(model.FirstName) && userProfileObj.LastName.Equals(model.LastName)))
                        {
                            // For Activity Log
                            messageForActivityLog = messageForActivityLog + "Name Changed ";
                            messageForActivityLog = messageForActivityLog + "Old Name : " + userProfileObj.FirstName + " " + userProfileObj.LastName + ". New Name : " + model.FirstName + " " + model.LastName + ". ";
                        }

                        userProfileObj.FirstName = model.FirstName;
                        userProfileObj.LastName = model.LastName;
                        userProfileObj.Address1 = model.Address1;
                        if (!userProfileObj.MobileNumber.Equals(model.MobileNumber))
                        {
                            userProfileObj.IsMobileNumVerified = false;
                        }

                        userProfileObj.MobileNumber = model.MobileNumber;


                        userProfileObj.IDProofTypeID = model.IDProofID;
                        userProfileObj.IDProofNumber = model.IDProofNumber;

                        userProfileObj.Pincode = model.Pincode;
                        // uncommented by shubham bhagat on 18-04-2019 
                        userProfileObj.Email = model.Email;

                        userProfileObj.CountryID = (short)model.CountryID;

                        // For Activity Log

                        if (messageForActivityLog.Length < 1000)
                            ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.UserDetail), messageForActivityLog);
                        else
                        {
                            messageForActivityLog = messageForActivityLog.Substring(0, 999);
                            ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.UserDetail), messageForActivityLog);
                        }

                        dbKaveriOnlineContext.SaveChanges();
                        ts.Complete();

                        useractivationmodel.IsUpdatedSuccessfully = true;
                        useractivationmodel.ResponseMessage = "Office User Details Updated successfully";
                        return useractivationmodel;

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }

        }

        //public bool UpdateOfficeUser(OfficeUserDetailsModel model)
        //{
        //    KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
        //    try
        //    {
        //        UMG_UserProfile userProfileObj = new UMG_UserProfile();

        //        UMG_UserDetails userDetailsObj = new UMG_UserDetails();

        //        encryptedParameters = model.EncryptedId.Split('/');

        //        if (encryptedParameters.Length != 3)
        //        {
        //            throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
        //        }

        //        decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

        //        int UserId = Convert.ToInt32(decryptedParameters["UserID"].ToString().Trim());


        //        using (TransactionScope ts = new TransactionScope())
        //        {
        //            // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
        //            encryptedParameters = model.OfficeID.Split('/');
        //            if (!(encryptedParameters.Length == 3))
        //                throw new SecurityException("URL Tempered");

        //            decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
        //            short officeEncryptId = Convert.ToInt16(decryptedParameters["EncryptOfficeID"].ToString().Trim());

        //            // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
        //            encryptedParameters = model.RoleID.Split('/');
        //            if (!(encryptedParameters.Length == 3))
        //                throw new SecurityException("URL Tempered");

        //            decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
        //            short roleEncryptId = Convert.ToInt16(decryptedParameters["EncryptRoleID"].ToString().Trim());

        //            // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
        //            encryptedParameters = model.LevelID.Split('/');
        //            if (!(encryptedParameters.Length == 3))
        //                throw new SecurityException("URL Tempered");

        //            decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
        //            short levelEncryptId = Convert.ToInt16(decryptedParameters["EncryptLevelID"].ToString().Trim());


        //            String messageForActivityLog = "User Details Updated # " + model.Email + "- User Details Updated. ";

        //            userDetailsObj = dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.UserID == UserId).FirstOrDefault();
        //            //userDetailsObj.CreationDate = DateTime.Now;
        //            userDetailsObj.DefaultRoleID = roleEncryptId;
        //            userDetailsObj.IsActive = model.IsActive;
        //            userDetailsObj.IsLocked = false;
        //            userDetailsObj.IsFirstLogin = false;
        //            userDetailsObj.LevelID = levelEncryptId;
        //            userDetailsObj.OfficeID = officeEncryptId;
        //            // userDetailsObj.Password = model.Password;
        //            // For Activity Log
        //            if (!(userDetailsObj.UserName.Equals(model.Email)))
        //            {
        //                messageForActivityLog = messageForActivityLog + " Old UserName(Email) : " + userDetailsObj.UserName + ". New UserName(Email) : " + model.Email + ". ";
        //            }
        //            // userDetailsObj.UserName = model.Email;

        //            userProfileObj = dbKaveriOnlineContext.UMG_UserProfile.Where(x => x.UserID == UserId).FirstOrDefault();
        //            if (!(userProfileObj.FirstName.Equals(model.FirstName) && userProfileObj.LastName.Equals(model.LastName)))
        //            {
        //                // For Activity Log
        //                messageForActivityLog = messageForActivityLog + "Name Changed ";
        //                messageForActivityLog = messageForActivityLog + "Old Name : " + userProfileObj.FirstName + " " + userProfileObj.LastName + ". New Name : " + model.FirstName + " " + model.LastName + ". ";
        //            }

        //            userProfileObj.FirstName = model.FirstName;
        //            userProfileObj.LastName = model.LastName;
        //            userProfileObj.Address1 = model.Address1;
        //            userProfileObj.MobileNumber = model.MobileNumber;


        //            userProfileObj.IDProofTypeID = model.IDProofID;
        //            userProfileObj.IDProofNumber = model.IDProofNumber;

        //            userProfileObj.Pincode = model.Pincode;
        //            //  userProfileObj.Email = model.Email;

        //            userProfileObj.CountryID = (short)model.CountryID;

        //            // For Activity Log

        //            if (messageForActivityLog.Length < 1000)
        //                ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.UserDetail), messageForActivityLog);


        //            dbKaveriOnlineContext.SaveChanges();
        //            ts.Complete();

        //            return true;
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    finally
        //    {
        //        dbKaveriOnlineContext.Dispose();
        //    }

        //}

        public bool DeleteOfficeUser(string EncryptedId)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            UMG_UserProfile userProfileObj = new UMG_UserProfile();

            UMG_UserDetails userDetailsObj = new UMG_UserDetails();
            try
            {
                encryptedParameters = EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int UserID = Convert.ToInt32(decryptedParameters["UserID"].ToString().Trim());
                userProfileObj = dbKaveriOnlineContext.UMG_UserProfile.Where(x => x.UserID == UserID).FirstOrDefault();
                userDetailsObj = dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.UserID == UserID).FirstOrDefault();
                if (userDetailsObj != null && userProfileObj != null)
                {
                    dbKaveriOnlineContext.UMG_UserProfile.Remove(userProfileObj);
                    dbKaveriOnlineContext.UMG_UserDetails.Remove(userDetailsObj);
                    dbKaveriOnlineContext.SaveChanges();

                    return true;
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

        }

        public OfficeUserDetailsModel GetUserDetails(string EncryptedId)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                OfficeUserDetailsModel responseModel = new OfficeUserDetailsModel();

                UMG_UserDetails dbUserModel = new UMG_UserDetails();
                UMG_UserProfile UserProfile = new UMG_UserProfile();
                encryptedParameters = EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int UserID = Convert.ToInt32(decryptedParameters["UserID"].ToString().Trim());
                dbUserModel = dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.UserID == UserID).FirstOrDefault();

                UserProfile = dbKaveriOnlineContext.UMG_UserProfile.Where(x => x.UserID == dbUserModel.UserID).FirstOrDefault();

                MAS_OfficeMaster mas_OfficeObj = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == dbUserModel.OfficeID).FirstOrDefault();
                MAS_OfficeTypes mas_OfficeTypesObj = null;
                if (mas_OfficeObj != null)
                {
                    mas_OfficeTypesObj = dbKaveriOnlineContext.MAS_OfficeTypes.Where(x => x.OfficeTypeID == mas_OfficeObj.OfficeTypeID).FirstOrDefault();
                }


                responseModel.EncryptedId = EncryptedId;
                responseModel.FirstName = UserProfile.FirstName;
                responseModel.LastName = UserProfile.LastName;
                responseModel.Address1 = UserProfile.Address1;
                responseModel.CountryID = UserProfile.CountryID;
                responseModel.OfficeID = URLEncrypt.EncryptParameters(new String[] { "EncryptOfficeID=" + dbUserModel.OfficeID });
                //responseModel.OfficeID = Convert.ToString(dbUserModel.OfficeID);
                responseModel.LevelID = URLEncrypt.EncryptParameters(new String[] { "EncryptLevelID=" + dbUserModel.LevelID });
                //responseModel.LevelID = Convert.ToString(dbUserModel.LevelID);
                responseModel.RoleID = URLEncrypt.EncryptParameters(new String[] { "EncryptRoleID=" + dbUserModel.DefaultRoleID });
                //responseModel.RoleID = Convert.ToString(dbUserModel.DefaultRoleID);
                responseModel.MobileNumber = UserProfile.MobileNumber;
                responseModel.Email = UserProfile.Email;
                responseModel.IsActive = (bool)dbUserModel.IsActive;
                responseModel.Pincode = UserProfile.Pincode;
                responseModel.IDProofID = (short)UserProfile.IDProofTypeID;
                responseModel.IDProofNumber = UserProfile.IDProofNumber;
                // Commented by Shubham Bhagat on 8-4-2019
                //responseModel.RoleDropDown = GetRoleListByOfficeType(mas_OfficeTypesObj.OfficeTypeID, true, responseModel.RoleID);
                OfficeUserDetailsModel temp = GetRoleListByLevel(responseModel.LevelID, dbUserModel.DefaultRoleID, responseModel.RoleID,dbUserModel.OfficeID, responseModel.OfficeID);
                responseModel.RoleDropDown = temp.RoleDropDown;
             
                // on 11 - 4 - 2019 Commented by SB  due to requirement change
                //responseModel.OfficeNamesDropDown = GetOfficeNameList(dbUserModel.OfficeID, responseModel.OfficeID);
                responseModel.OfficeNamesDropDown = temp.OfficeNamesDropDown;
                //responseModel.LevelDetailsDropDown = GetLevelListByOfficeType(mas_OfficeTypesObj.OfficeTypeID, true, responseModel.LevelID);

                // commented by shubham bhagat on 12-04-2019 due to requirement change
                // Level List
                //short SRLevelID = (short)ApiCommonEnum.LevelDetails.SR;
                //short DRLevelID = (short)ApiCommonEnum.LevelDetails.DR;
                //short HeadOfficeLevelID = (short)ApiCommonEnum.LevelDetails.State;
                short OthersLevelID = (short)ApiCommonEnum.LevelDetails.Others;

                List<SelectListItem> objLevelNameList = new List<SelectListItem>();
                SelectListItem objLevelName = new SelectListItem();
                objLevelName.Text = "--Select Level--";
                objLevelName.Value = "0";
                objLevelNameList.Add(objLevelName);

                // commented by shubham bhagat on 12-04-2019 due to requirement change
                //List<UMG_LevelDetails> levelList = dbKaveriOnlineContext.UMG_LevelDetails.Where(x => x.LevelID == SRLevelID || x.LevelID == DRLevelID || x.LevelID == HeadOfficeLevelID).ToList();
                List<UMG_LevelDetails> levelList = dbKaveriOnlineContext.UMG_LevelDetails.Where(x => x.IsActive == true && x.LevelID != OthersLevelID).ToList();
                if (levelList != null)
                {
                    foreach (var item in levelList)
                    {
                        objLevelName = new SelectListItem();
                        objLevelName.Text = item.LevelName;
                        if (item.LevelID == dbUserModel.LevelID)  // To Add update
                            objLevelName.Value = responseModel.LevelID;
                        else                                       // To Add All
                            objLevelName.Value = URLEncrypt.EncryptParameters(new String[] { "EncryptLevelID=" + item.LevelID });
                        objLevelNameList.Add(objLevelName);
                    }
                }
                responseModel.LevelDetailsDropDown = objLevelNameList;
                responseModel.UserID = UserID;
                // Added By shubham bhagat on 18-04-2019
                responseModel.Username = dbUserModel.UserName;
                return responseModel;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }

        }

        #region Old Method commented by mayank on 06-08-2021
    //    public UserGridWrapperModel LoadOfficeUserDetailsGridData()
    //    {
    //        dbContext = new KaveriEntities();
    //        try
    //        {
    //            UserGridWrapperModel returnModel = new UserGridWrapperModel();
    //            List<OfficeUserDetailsModel> lstDatamodel = new List<OfficeUserDetailsModel>();
    //            OfficeUserDetailsModel GridModel = null;
    //            List<UserModelDataColumn> lstcolumn = new List<UserModelDataColumn>();

    //            // commented by m rafe on 24-12-19
    //            /*List<UMG_UserDetails> resultList = null;

    //            short OnlineUserRoleID = (short)ApiCommonEnum.RoleDetails.OnlineUser;
    //            short TechnicalAdminRoleID = (short)ApiCommonEnum.RoleDetails.TechnicalAdmin;
    //            short DepartmentAdminRoleID = (short)ApiCommonEnum.RoleDetails.DepartmentAdmin;
    //            short ApprovalAdminRoleID = (short)ApiCommonEnum.RoleDetails.ApprovalAdmin;
    //            resultList = dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.DefaultRoleID != OnlineUserRoleID
    //            && x.DefaultRoleID != TechnicalAdminRoleID && x.DefaultRoleID != DepartmentAdminRoleID &&
    //            x.DefaultRoleID != ApprovalAdminRoleID).ToList();
    //            foreach (var item in resultList)
    //            {

    //                GridModel = new OfficeUserDetailsModel();
    //                UMG_UserProfile UserProfile = new UMG_UserProfile();
    //                UserProfile = dbKaveriOnlineContext.UMG_UserProfile.Where(x => x.UserID == item.UserID).FirstOrDefault();
    //                GridModel.EncryptedId = URLEncrypt.EncryptParameters(new String[] { "UserID=" + item.UserID });
    //                GridModel.FirstName = UserProfile.FirstName + " " + UserProfile.LastName;
    //                GridModel.Address1 = UserProfile.Address1;
    //                GridModel.CountryName = (dbKaveriOnlineContext.MAS_Countries.Where(x => x.CountryID == UserProfile.CountryID)).FirstOrDefault().CountryName;
    //                GridModel.OfficeName = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == item.OfficeID).FirstOrDefault().OfficeName;
    //                GridModel.LevelDesc = dbKaveriOnlineContext.UMG_LevelDetails.Where(x => x.LevelID == item.LevelID).FirstOrDefault().LevelName;
    //                GridModel.RoleDesc = dbKaveriOnlineContext.UMG_RoleDetails.Where(x => x.RoleID == item.DefaultRoleID).FirstOrDefault().RoleName;
    //                GridModel.MobileNumber = UserProfile.MobileNumber;
    //                // Commented and changed by shubham bhagat on 20-04-2019 
    //                //GridModel.Email = UserProfile.Email;
    //                GridModel.Email = item.UserName;//  dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.UserID == item.UserID).Select(x=>x.UserName).FirstOrDefault();
    //                GridModel.IsActive = (bool)item.IsActive;
    //                if (GridModel.IsActive)
    //                    GridModel.IsActiveIcon = "<i class='fa fa-check  ' style='color:black'></i>";
    //                else
    //                    GridModel.IsActiveIcon = "<i class='fa fa-close  ' style='color:black'></i>";
    //                GridModel.EditBtn = "<a href='#'  onclick=UpdateOfficeUserDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
    //                //  GridModel.DeleteBtn = "<a href='#'  onclick=DeleteOfficeUserDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-trash fa-2x  ' style='color:black'></i></a>";
    //                lstDatamodel.Add(GridModel);
    //            }

    //*/

    //            var resultList = dbContext.USP_POPULATE_OFC_USER_DETAILS().ToList();


    //            foreach (var item in resultList)
    //            {

    //                GridModel = new OfficeUserDetailsModel();

    //                GridModel.EncryptedId = URLEncrypt.EncryptParameters(new String[] { "UserID=" + item.UserID });
    //                GridModel.FirstName = item.FirstName + " " + item.LastName;
    //                GridModel.Address1 = item.Address1;
    //                GridModel.CountryName = item.CountryName;
    //                GridModel.OfficeName = item.OfficeName;
    //                GridModel.LevelDesc = item.LevelName;
    //                GridModel.RoleDesc = item.RoleName;
    //                GridModel.MobileNumber = item.MobileNumber;
    //                // Commented and changed by shubham bhagat on 20-04-2019 
    //                //GridModel.Email = UserProfile.Email;
    //                GridModel.Email = item.username;//  dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.UserID == item.UserID).Select(x=>x.UserName).FirstOrDefault();
    //                GridModel.IsActive = (bool)item.IsActive;
    //                if (GridModel.IsActive)
    //                    GridModel.IsActiveIcon = "<i class='fa fa-check  ' style='color:black'></i>";
    //                else
    //                    GridModel.IsActiveIcon = "<i class='fa fa-close  ' style='color:black'></i>";
    //                GridModel.EditBtn = "<a href='#'  onclick=UpdateOfficeUserDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
    //                //  GridModel.DeleteBtn = "<a href='#'  onclick=DeleteOfficeUserDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-trash fa-2x  ' style='color:black'></i></a>";
    //                lstDatamodel.Add(GridModel);
    //            }

    //            lstcolumn.Add(new UserModelDataColumn { title = "SR NO", data = "LevelDesc" });
    //            lstcolumn.Add(new UserModelDataColumn { title = "Name", data = "FirstName" });
    //            lstcolumn.Add(new UserModelDataColumn { title = "Address", data = "Address1" });

    //            lstcolumn.Add(new UserModelDataColumn { title = "Country Name", data = "CountryName" });
    //            lstcolumn.Add(new UserModelDataColumn { title = "Office Name ", data = "OfficeName" });
    //            lstcolumn.Add(new UserModelDataColumn { title = "Level", data = "LevelDesc" });

    //            lstcolumn.Add(new UserModelDataColumn { title = "Role", data = "RoleDesc" });
    //            lstcolumn.Add(new UserModelDataColumn { title = "Mobile Number", data = "MobileNumber" });
    //            // Commented and changed by shubham bhagat on 20-04-2019 
    //            //lstcolumn.Add(new UserModelDataColumn { title = "Username(EmailID)", data = "Email" });
    //            lstcolumn.Add(new UserModelDataColumn { title = "Username", data = "Email" });
    //            lstcolumn.Add(new UserModelDataColumn { title = "Active Status", data = "IsActiveIcon" });


    //            lstcolumn.Add(new UserModelDataColumn { title = "Edit", data = "EditBtn" });
    //            // lstcolumn.Add(new UserModelDataColumn { title = "Delete", data = "DeleteBtn" });

    //            returnModel.dataArray = lstDatamodel.ToArray();
    //            returnModel.ColumnArray = lstcolumn.ToArray();
    //            return returnModel;

    //        }
    //        catch (Exception)
    //        {

    //            throw;
    //        }
    //        finally
    //        {

    //        }
    //    }
        #endregion

        #region New Method changes added by mayank on 06-08-2021
        public UserGridWrapperModel LoadOfficeUserDetailsGridData(int officeID, int LevelID)
        {
            dbContext = new KaveriEntities();
            try
            {
                UserGridWrapperModel returnModel = new UserGridWrapperModel();
                List<OfficeUserDetailsModel> lstDatamodel = new List<OfficeUserDetailsModel>();
                OfficeUserDetailsModel GridModel = null;
                List<UserModelDataColumn> lstcolumn = new List<UserModelDataColumn>();

                // commented by m rafe on 24-12-19
                /*List<UMG_UserDetails> resultList = null;

                short OnlineUserRoleID = (short)ApiCommonEnum.RoleDetails.OnlineUser;
                short TechnicalAdminRoleID = (short)ApiCommonEnum.RoleDetails.TechnicalAdmin;
                short DepartmentAdminRoleID = (short)ApiCommonEnum.RoleDetails.DepartmentAdmin;
                short ApprovalAdminRoleID = (short)ApiCommonEnum.RoleDetails.ApprovalAdmin;
                resultList = dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.DefaultRoleID != OnlineUserRoleID
                && x.DefaultRoleID != TechnicalAdminRoleID && x.DefaultRoleID != DepartmentAdminRoleID &&
                x.DefaultRoleID != ApprovalAdminRoleID).ToList();
                foreach (var item in resultList)
                {

                    GridModel = new OfficeUserDetailsModel();
                    UMG_UserProfile UserProfile = new UMG_UserProfile();
                    UserProfile = dbKaveriOnlineContext.UMG_UserProfile.Where(x => x.UserID == item.UserID).FirstOrDefault();
                    GridModel.EncryptedId = URLEncrypt.EncryptParameters(new String[] { "UserID=" + item.UserID });
                    GridModel.FirstName = UserProfile.FirstName + " " + UserProfile.LastName;
                    GridModel.Address1 = UserProfile.Address1;
                    GridModel.CountryName = (dbKaveriOnlineContext.MAS_Countries.Where(x => x.CountryID == UserProfile.CountryID)).FirstOrDefault().CountryName;
                    GridModel.OfficeName = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == item.OfficeID).FirstOrDefault().OfficeName;
                    GridModel.LevelDesc = dbKaveriOnlineContext.UMG_LevelDetails.Where(x => x.LevelID == item.LevelID).FirstOrDefault().LevelName;
                    GridModel.RoleDesc = dbKaveriOnlineContext.UMG_RoleDetails.Where(x => x.RoleID == item.DefaultRoleID).FirstOrDefault().RoleName;
                    GridModel.MobileNumber = UserProfile.MobileNumber;
                    // Commented and changed by shubham bhagat on 20-04-2019 
                    //GridModel.Email = UserProfile.Email;
                    GridModel.Email = item.UserName;//  dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.UserID == item.UserID).Select(x=>x.UserName).FirstOrDefault();
                    GridModel.IsActive = (bool)item.IsActive;
                    if (GridModel.IsActive)
                        GridModel.IsActiveIcon = "<i class='fa fa-check  ' style='color:black'></i>";
                    else
                        GridModel.IsActiveIcon = "<i class='fa fa-close  ' style='color:black'></i>";
                    GridModel.EditBtn = "<a href='#'  onclick=UpdateOfficeUserDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
                    //  GridModel.DeleteBtn = "<a href='#'  onclick=DeleteOfficeUserDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-trash fa-2x  ' style='color:black'></i></a>";
                    lstDatamodel.Add(GridModel);
                }

    */

                var resultList = dbContext.USP_POPULATE_OFC_USER_DETAILS(officeID, LevelID).ToList();


                foreach (var item in resultList)
                {

                    GridModel = new OfficeUserDetailsModel();

                    GridModel.EncryptedId = URLEncrypt.EncryptParameters(new String[] { "UserID=" + item.UserID });
                    GridModel.FirstName = item.FirstName + " " + item.LastName;
                    GridModel.Address1 = item.Address1;
                    GridModel.CountryName = item.CountryName;
                    GridModel.OfficeName = item.OfficeName;
                    GridModel.LevelDesc = item.LevelName;
                    GridModel.RoleDesc = item.RoleName;
                    GridModel.MobileNumber = item.MobileNumber;
                    // Commented and changed by shubham bhagat on 20-04-2019 
                    //GridModel.Email = UserProfile.Email;
                    GridModel.Email = item.username;//  dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.UserID == item.UserID).Select(x=>x.UserName).FirstOrDefault();
                    GridModel.IsActive = (bool)item.IsActive;
                    if (GridModel.IsActive)
                        GridModel.IsActiveIcon = "<i class='fa fa-check  ' style='color:black'></i>";
                    else
                        GridModel.IsActiveIcon = "<i class='fa fa-close  ' style='color:black'></i>";
                    GridModel.EditBtn = "<a href='#'  onclick=UpdateOfficeUserDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
                    //  GridModel.DeleteBtn = "<a href='#'  onclick=DeleteOfficeUserDetailsData('" + GridModel.EncryptedId + "'); ><i class='fa fa-trash fa-2x  ' style='color:black'></i></a>";
                    lstDatamodel.Add(GridModel);
                }

                lstcolumn.Add(new UserModelDataColumn { title = "SR NO", data = "LevelDesc" });
                lstcolumn.Add(new UserModelDataColumn { title = "Name", data = "FirstName" });
                lstcolumn.Add(new UserModelDataColumn { title = "Address", data = "Address1" });

                lstcolumn.Add(new UserModelDataColumn { title = "Country Name", data = "CountryName" });
                lstcolumn.Add(new UserModelDataColumn { title = "Office Name ", data = "OfficeName" });
                lstcolumn.Add(new UserModelDataColumn { title = "Level", data = "LevelDesc" });

                lstcolumn.Add(new UserModelDataColumn { title = "Role", data = "RoleDesc" });
                lstcolumn.Add(new UserModelDataColumn { title = "Mobile Number", data = "MobileNumber" });
                // Commented and changed by shubham bhagat on 20-04-2019 
                //lstcolumn.Add(new UserModelDataColumn { title = "Username(EmailID)", data = "Email" });
                lstcolumn.Add(new UserModelDataColumn { title = "Username", data = "Email" });
                lstcolumn.Add(new UserModelDataColumn { title = "Active Status", data = "IsActiveIcon" });


                lstcolumn.Add(new UserModelDataColumn { title = "Edit", data = "EditBtn" });
                // lstcolumn.Add(new UserModelDataColumn { title = "Delete", data = "DeleteBtn" });

                returnModel.dataArray = lstDatamodel.ToArray();
                returnModel.ColumnArray = lstcolumn.ToArray();
                return returnModel;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }
        #endregion

        //on 11-4-2019 Commented by SB due to requirement change
        //public OfficeUserDetailsModel GetOfficeUserDetailsList()
        //{
        //    OfficeUserDetailsModel model = new OfficeUserDetailsModel();
        //    model.OfficeNamesDropDown = GetOfficeNameList();

        //    List<SelectListItem> objLevelNameList = new List<SelectListItem>();
        //    SelectListItem objLevelName = new SelectListItem();
        //    List<UMG_LevelDetails> objLevelData = new List<UMG_LevelDetails>();
        //    objLevelName.Text = "--Select Level--";
        //    objLevelName.Value = "0";
        //    objLevelNameList.Add(objLevelName);

        //    model.LevelDetailsDropDown = objLevelNameList;

        //    List<SelectListItem> roleList = new List<SelectListItem>();
        //    SelectListItem objRole = new SelectListItem();
        //    objRole.Text = "--Select Role--";
        //    objRole.Value = "0";
        //    roleList.Add(objRole);

        //    model.RoleDropDown = roleList;
        //    return model;
        //}

        public OfficeUserDetailsModel GetOfficeUserDetailsList()
        {
            try
            {
                OfficeUserDetailsModel model = new OfficeUserDetailsModel();
                dbContext = new KaveriEntities();

                // Office List
                List<SelectListItem> objOfficeNameList = new List<SelectListItem>();
                SelectListItem objOfficeName = new SelectListItem();
                objOfficeName.Text = "--Select Office--";
                objOfficeName.Value = "0";
                objOfficeNameList.Add(objOfficeName);

                model.OfficeNamesDropDown = objOfficeNameList;

                // commented by shubham bhagat on 12-04-2019 due to requirement change
                //// Level List
                //short SRLevelID = (short)ApiCommonEnum.LevelDetails.SR;
                //short DRLevelID = (short)ApiCommonEnum.LevelDetails.DR;
                //short HeadOfficeLevelID = (short)ApiCommonEnum.LevelDetails.State;
                short OthersLevelID = (short)ApiCommonEnum.LevelDetails.Others;

                List<SelectListItem> objLevelNameList = new List<SelectListItem>();           
                SelectListItem objLevelName = new SelectListItem();
                objLevelName.Text = "--Select Level--";
                objLevelName.Value = "0";
                objLevelNameList.Add(objLevelName);

                // commented by shubham bhagat on 12-04-2019 due to requirement change
                //List<UMG_LevelDetails> levelList = dbContext.UMG_LevelDetails.Where(x => x.LevelID == SRLevelID || x.LevelID == DRLevelID || x.LevelID == HeadOfficeLevelID).ToList();
                List<UMG_LevelDetails> levelList = dbContext.UMG_LevelDetails.Where(x => x.IsActive == true && x.LevelID!=OthersLevelID).ToList();
                if (levelList != null)
                {
                    foreach (var item in levelList)
                    {
                        objLevelName = new SelectListItem();
                        objLevelName.Text = item.LevelName;
                        objLevelName.Value = URLEncrypt.EncryptParameters(new String[] { "EncryptLevelID=" + item.LevelID });
                        objLevelNameList.Add(objLevelName);
                    }
                }
                model.LevelDetailsDropDown = objLevelNameList;

                // Role List
                List<SelectListItem> roleList = new List<SelectListItem>();
                SelectListItem objRole = new SelectListItem();
                objRole.Text = "--Select Role--";
                objRole.Value = "0";
                roleList.Add(objRole);

                model.RoleDropDown = roleList;
                return model;
            }
            catch (Exception) { throw; }
            finally { if (dbContext != null) dbContext.Dispose(); }
        }

        public List<SelectListItem> GetRoleList()
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                List<SelectListItem> roleList = new List<SelectListItem>();
                SelectListItem objRole = new SelectListItem();
                objRole.Text = "--Select Role--";
                objRole.Value = "0";
                roleList.Add(objRole);

                // Commented on 4-1-2018 by Shubham Bhagat
                //List<UMG_RoleDetails> dblist= dbKaveriOnlineContext.UMG_RoleDetails.Where(x=>x.RoleID!=2).ToList();

                short OnlineUserId = Convert.ToInt16(Common.ApiCommonEnum.RoleDetails.OnlineUser);
                short SystemAdminId = Convert.ToInt16(Common.ApiCommonEnum.RoleDetails.TechnicalAdmin);

                List<UMG_RoleDetails> dblist = dbKaveriOnlineContext.UMG_RoleDetails.Where(x => x.RoleID != OnlineUserId && x.RoleID != SystemAdminId).OrderBy(c => c.RoleName).ToList();
                if (dblist != null)
                {
                    foreach (var role in dblist)
                    {
                        objRole = new SelectListItem();
                        objRole.Text = role.RoleName;

                        objRole.Value = role.RoleID.ToString();


                        roleList.Add(objRole);
                    }
                }
                return roleList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }
        }

        public List<SelectListItem> GetOfficeNameList(int officeId=0,String officeEncryptedId=null)
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {

                List<SelectListItem> objOfficeNametList = new List<SelectListItem>();
                SelectListItem objOfficeName = new SelectListItem();
                List<MAS_OfficeMaster> objOfficeNameData = new List<MAS_OfficeMaster>();
                objOfficeName.Text = "--Select Office--";
                objOfficeName.Value = "0";
                objOfficeNametList.Add(objOfficeName);

                short OthersOfficeTypeId = Convert.ToInt16(ApiCommonEnum.OfficeTypes.Others);
                short lawDepartmentOfficeTypeId = Convert.ToInt16(ApiCommonEnum.OfficeTypes.LawDepartment);

                // objOfficeNameData = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeTypeID != OthersOfficeTypeId && x.OfficeTypeID != lawDepartmentOfficeTypeId ).ToList();
                objOfficeNameData = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeTypeID != OthersOfficeTypeId && x.OfficeTypeID != lawDepartmentOfficeTypeId).OrderBy(c => c.OfficeName).ToList();

                // Commented on 4-1-2018 by Shubham Bhagat
                //foreach (var ON in objOfficeNameData)
                //{
                //    objOfficeName = new SelectListItem();
                //    objOfficeName.Text = ON.OfficeName;
                //    objOfficeName.Value = ON.OfficeID.ToString();
                //    objOfficeNametList.Add(objOfficeName);
                //}

                // Commented on 4-1-2018 by Shubham Bhagat
                if (objOfficeNameData != null)
                {
                    foreach (var ON in objOfficeNameData)
                    {
                        objOfficeName = new SelectListItem();

                        // Added by Shubham Bhagat on 5-1-2019
                        // For Adding office Details whose office user is not created 
                        UMG_UserDetails umg_UserDetails = dbKaveriOnlineContext.UMG_UserDetails.Where(x => x.OfficeID == ON.OfficeID).FirstOrDefault();
                        // Commented below condition to allow multiple users in one office by Shubham Bhagat on 8-4-2019
                        //if (umg_UserDetails == null)
                        if (umg_UserDetails != null)
                        {
                            if (ON.OfficeTypeID == 1)
                            {
                                objOfficeName.Text = ON.OfficeName + " ( IGR ) ";
                            }
                            if (ON.OfficeTypeID == 2)
                            {
                                objOfficeName.Text = ON.OfficeName + " ( DRO ) ";
                            }
                            if (ON.OfficeTypeID == 3)
                            {
                                objOfficeName.Text = ON.OfficeName + " ( SRO ) ";
                            }

                            // Added by Shubham Bhagat on 5-1-2019
                            //objOfficeName.Value = ON.OfficeID.ToString();
                            objOfficeName.Value = URLEncrypt.EncryptParameters(new String[] { "EncryptOfficeID=" + ON.OfficeID });

                            objOfficeNametList.Add(objOfficeName);
                        }
                        else
                        {
                        }
                    }
                }
                // For Adding office to office list in case of update office user
                if (officeId != 0)
                {
                    MAS_OfficeMaster mas_officeMaster= dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeId).FirstOrDefault();
                    if (mas_officeMaster!=null)
                    {
                        objOfficeName = new SelectListItem();

                        if (mas_officeMaster.OfficeTypeID == 1)
                        {
                            objOfficeName.Text = mas_officeMaster.OfficeName + " ( IGR ) ";
                        }
                        if (mas_officeMaster.OfficeTypeID == 2)
                        {
                            objOfficeName.Text = mas_officeMaster.OfficeName + " ( DRO ) ";
                        }
                        if (mas_officeMaster.OfficeTypeID == 3)
                        {
                            objOfficeName.Text = mas_officeMaster.OfficeName + " ( SRO ) ";
                        }

                        // Added by Shubham Bhagat on 5-1-2019
                        //objOfficeName.Value = ON.OfficeID.ToString();
                        objOfficeName.Value = officeEncryptedId;//URLEncrypt.EncryptParameters(new String[] { "EncryptOfficeID=" + mas_officeMaster.OfficeID });

                        objOfficeNametList.Add(objOfficeName);
                    }
                }
                return objOfficeNametList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }
        }

        public List<SelectListItem> GetLevelNameList()
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {

                List<SelectListItem> objLevelNameList = new List<SelectListItem>();
                SelectListItem objLevelName = new SelectListItem();
                List<UMG_LevelDetails> objLevelData = new List<UMG_LevelDetails>();
                objLevelName.Text = "--Select Office--";
                objLevelName.Value = "0";
                objLevelNameList.Add(objLevelName);

                objLevelData = dbKaveriOnlineContext.UMG_LevelDetails.Where(m => m.IsActive == true).ToList();

                if (objLevelData != null)
                {
                    foreach (var level in objLevelData)
                    {
                        objLevelName = new SelectListItem();
                        objLevelName.Text = level.LevelName;
                        objLevelName.Value = level.LevelID.ToString();
                        objLevelNameList.Add(objLevelName);
                    }
                }
                return objLevelNameList;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }
        }

        // Added By Shubham Bhagat on 15-12-2018        
        //public OfficeUserDetailsModel GetOfficeDetailsInfo(int officeDetailId) {
        //    KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
        //    OfficeUserDetailsModel userModel = new OfficeUserDetailsModel();
        //    try
        //    {               
        //        MAS_OfficeMaster mas_OfficeMaster = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeDetailId).FirstOrDefault();
        //        if (mas_OfficeMaster != null)
        //        {
        //            MAS_OfficeTypes mas_OfficeTypes = dbKaveriOnlineContext.MAS_OfficeTypes.Where(x => x.OfficeTypeID == mas_OfficeMaster.OfficeTypeID).FirstOrDefault();
        //            if (mas_OfficeMaster != null)
        //            {
        //                MAS_Districts mas_Districts = dbKaveriOnlineContext.MAS_Districts.Where(x => x.DistrictID == mas_OfficeMaster.DistrictID).FirstOrDefault();
        //                if (mas_OfficeMaster != null)
        //                {
        //                    userModel.Office_OfficeType = mas_OfficeTypes.OfficeTypeDesc;
        //                    userModel.Office_ShortName = mas_OfficeMaster.ShortName;
        //                    userModel.Office_District = mas_Districts.DistrictName;
        //                    return userModel;
        //                }
        //            }
        //        }
        //         return userModel; 
        //    }
        //    catch (Exception )
        //    {
        //        throw ;
        //    }
        //    finally
        //    {
        //        if (dbKaveriOnlineContext!=null) { dbKaveriOnlineContext.Dispose(); }    
        //    }
        //}

        public OfficeUserDetailsModel GetOfficeDetailsInfo(String officeDetailId)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            OfficeUserDetailsModel userModel = new OfficeUserDetailsModel();
            try
            {
                // Added by Shubham Bhagat on 5-1-2019
                encryptedParameters = officeDetailId.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int officeDetailIdInt = Convert.ToInt16(decryptedParameters["EncryptOfficeID"].ToString().Trim());


                MAS_OfficeMaster mas_OfficeMaster = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == officeDetailIdInt).FirstOrDefault();
                if (mas_OfficeMaster != null)
                {
                    MAS_OfficeTypes mas_OfficeTypes = dbKaveriOnlineContext.MAS_OfficeTypes.Where(x => x.OfficeTypeID == mas_OfficeMaster.OfficeTypeID).FirstOrDefault();
                    if (mas_OfficeTypes != null)
                    {
                        MAS_Districts mas_Districts = dbKaveriOnlineContext.MAS_Districts.Where(x => x.DistrictID == mas_OfficeMaster.DistrictID).FirstOrDefault();
                        if (mas_Districts != null)
                        {
                            userModel.Office_OfficeType = mas_OfficeTypes.OfficeTypeDesc;
                            userModel.Office_ShortName = mas_OfficeMaster.ShortName;
                            userModel.Office_District = mas_Districts.DistrictName;
                            // Added by Shubham Bhagat on 4-1-2019
                            userModel.RoleDropDown = GetRoleListByOfficeType(mas_OfficeTypes.OfficeTypeID);
                            // Added by Shubham Bhagat on 5-1-2019
                            userModel.LevelDetailsDropDown = GetLevelListByOfficeType(mas_OfficeTypes.OfficeTypeID);
                            return userModel;
                        }
                    }
                }
                return userModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null) { dbKaveriOnlineContext.Dispose(); }

            }
        }

        // Added by Shubham Bhagat on 4-1-2019
        public List<SelectListItem> GetRoleListByOfficeType(short officeTypeId, bool IsForUpdate = false, String roleEncryptId = null)
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {

                List<SelectListItem> roleList = new List<SelectListItem>();
                SelectListItem objRole = new SelectListItem();
                objRole.Text = "--Select Role--";
                objRole.Value = "0";
                roleList.Add(objRole);

                // Commented on 4-1-2018 by Shubham Bhagat
                //List<UMG_RoleDetails> dblist= dbKaveriOnlineContext.UMG_RoleDetails.Where(x=>x.RoleID!=2).ToList();

                short roleDetailsOnlineUserId = Convert.ToInt16(Common.ApiCommonEnum.RoleDetails.OnlineUser);
                short roleDetailsSystemAdminId = Convert.ToInt16(Common.ApiCommonEnum.RoleDetails.TechnicalAdmin);
                short roleDetailsIGRId = Convert.ToInt16(Common.ApiCommonEnum.RoleDetails.IGR);
                short roleDetailsDRId = Convert.ToInt16(Common.ApiCommonEnum.RoleDetails.DR);
                short roleDetailsSRId = Convert.ToInt16(Common.ApiCommonEnum.RoleDetails.SR);

                short officeTypeIGRId = Convert.ToInt16(Common.ApiCommonEnum.OfficeTypes.IGR);
                short officeTypeDROId = Convert.ToInt16(Common.ApiCommonEnum.OfficeTypes.DRO);
                short officeTypeSROId = Convert.ToInt16(Common.ApiCommonEnum.OfficeTypes.SRO);
                
                List<UMG_RoleDetails> dblist = null;
                if (officeTypeId == officeTypeIGRId)
                {
                    dblist = dbKaveriOnlineContext.UMG_RoleDetails.Where(x => x.RoleID != roleDetailsOnlineUserId && x.RoleID != roleDetailsSystemAdminId && x.RoleID == roleDetailsIGRId).OrderBy(c => c.RoleName).ToList();
                }
                else if (officeTypeId == officeTypeDROId)
                {
                    dblist = dbKaveriOnlineContext.UMG_RoleDetails.Where(x => x.RoleID != roleDetailsOnlineUserId && x.RoleID != roleDetailsSystemAdminId && x.RoleID == roleDetailsDRId).OrderBy(c => c.RoleName).ToList();
                }
                else if (officeTypeId == officeTypeSROId)
                {
                    dblist = dbKaveriOnlineContext.UMG_RoleDetails.Where(x => x.RoleID != roleDetailsOnlineUserId && x.RoleID != roleDetailsSystemAdminId && x.RoleID == roleDetailsSRId).OrderBy(c => c.RoleName).ToList();
                }
                if (dblist != null)
                {
                    foreach (var role in dblist)
                    {
                        objRole = new SelectListItem();
                        objRole.Text = role.RoleName;

                        // Added by Shubham Bhagat on 5-1-2019
                        //objRole.Value = role.RoleID.ToString();
                        // For Getting role list encrypt Id in case of update
                        if (IsForUpdate)
                        {
                            objRole.Value = roleEncryptId;// URLEncrypt.EncryptParameters(new String[] { "EncryptRoleID=" + role.RoleID });

                        }
                        else
                        {
                            objRole.Value = URLEncrypt.EncryptParameters(new String[] { "EncryptRoleID=" + role.RoleID });

                        }

                        roleList.Add(objRole);
                    }
                }
                return roleList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }
        }

        // Added By Shubham Bhagat on 5-1-2019
        public List<SelectListItem> GetLevelListByOfficeType(short officeTypeId, bool IsForUpdate = false, String levelEncryptId = null)
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                List<SelectListItem> objLevelNameList = new List<SelectListItem>();
                SelectListItem objLevelName = new SelectListItem();
                List<UMG_LevelDetails> objLevelData = new List<UMG_LevelDetails>();
                objLevelName.Text = "--Select Level--";
                objLevelName.Value = "0";
                objLevelNameList.Add(objLevelName);

                short officeTypeIGRId = Convert.ToInt16(Common.ApiCommonEnum.OfficeTypes.IGR);
                short officeTypeDROId = Convert.ToInt16(Common.ApiCommonEnum.OfficeTypes.DRO);
                short officeTypeSROId = Convert.ToInt16(Common.ApiCommonEnum.OfficeTypes.SRO);

                // commented by Shubham Bhagat on 8-4-2019
                //short levelDetailsStateRegistrarId = Convert.ToInt16(Common.ApiCommonEnum.LevelDetails.StateRegistrar);
                short levelDetailsHeadOfficeId = Convert.ToInt16(Common.ApiCommonEnum.LevelDetails.State);
                short levelDetailsDistrictRegistrarId = Convert.ToInt16(Common.ApiCommonEnum.LevelDetails.DR);
                short levelDetailsSubRegistrarId = Convert.ToInt16(Common.ApiCommonEnum.LevelDetails.SR);

                // commented by Shubham Bhagat on 8-4-2019
                //if (officeTypeId == officeTypeStateRegistrarOfficeId)
                //{
                //    objLevelData = dbKaveriOnlineContext.UMG_LevelDetails.Where(m => m.IsActive == true && m.LevelID == levelDetailsStateRegistrarId).ToList();
                //}
                if (officeTypeId == officeTypeIGRId)
                {
                    objLevelData = dbKaveriOnlineContext.UMG_LevelDetails.Where(m => m.IsActive == true && m.LevelID == levelDetailsHeadOfficeId).ToList();
                }
                else if (officeTypeId == officeTypeDROId)
                {
                    objLevelData = dbKaveriOnlineContext.UMG_LevelDetails.Where(m => m.IsActive == true && m.LevelID == levelDetailsDistrictRegistrarId).ToList();
                }
                else if (officeTypeId == officeTypeSROId)
                {
                    objLevelData = dbKaveriOnlineContext.UMG_LevelDetails.Where(m => m.IsActive == true && m.LevelID == levelDetailsSubRegistrarId).ToList();
                }

                if (objLevelData != null)
                {
                    foreach (var level in objLevelData)
                    {
                        objLevelName = new SelectListItem();
                        objLevelName.Text = level.LevelName;

                        // Added by Shubham Bhagat on 5-1-2019
                        // objLevelName.Value = level.LevelID.ToString();
                        // For Getting role list encrypt Id in case of update
                        if (IsForUpdate)
                        {
                            objLevelName.Value = levelEncryptId;// URLEncrypt.EncryptParameters(new String[] { "EncryptLevelID=" + level.LevelID });

                        }
                        else
                        {
                            objLevelName.Value = URLEncrypt.EncryptParameters(new String[] { "EncryptLevelID=" + level.LevelID });

                        }
                        objLevelNameList.Add(objLevelName);
                    }
                }
                return objLevelNameList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
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


        // Added by Shubham Bhagat on 8-4-2019
        public OfficeUserDetailsModel GetRoleListByLevel(String LevelID,int roleID=0,String roleEncryptId = null,int officeID=0, String officeEncryptId = null)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            OfficeUserDetailsModel userModel = new OfficeUserDetailsModel();
            try
            {
                List<SelectListItem> roleList = new List<SelectListItem>();
                SelectListItem objRole = new SelectListItem();
                objRole.Text = "--Select Role--";
                objRole.Value = "0";
                roleList.Add(objRole);

                List<SelectListItem> officeList = new List<SelectListItem>();
                SelectListItem objOffice = new SelectListItem();
                objOffice.Text = "--Select Office--";
                objOffice.Value = "0";
                officeList.Add(objOffice);

                // For encrypting Id used in dropdown commented by Shubham Bhagat on 5-1-2019
                encryptedParameters = LevelID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                short levelEncryptId = Convert.ToInt16(decryptedParameters["EncryptLevelID"].ToString().Trim());

                var dBRoleList = dbKaveriOnlineContext.UMG_RoleDetails.OrderBy(c => c.RoleName).ToList();
                short DepartmentAdminID= (short)ApiCommonEnum.RoleDetails.DepartmentAdmin;
                short ApprovalAdminID = (short)ApiCommonEnum.RoleDetails.ApprovalAdmin;
                if (dBRoleList != null)
                {
                    foreach (var item in dBRoleList)
                    {
                        if (item.RoleID != DepartmentAdminID && item.RoleID != ApprovalAdminID)
                        {
                            var isExist = dbKaveriOnlineContext.UMG_RoleLevelMapping.Where(x => x.LevelID == levelEncryptId && x.RoleID == item.RoleID).Any();
                            if (isExist)
                            {
                                objRole = new SelectListItem();
                                objRole.Text = item.RoleName;
                                if (roleID == item.RoleID) // To Add update
                                    objRole.Value = roleEncryptId;
                                else          // To Add all
                                {
                                    objRole.Value = URLEncrypt.EncryptParameters(new String[] { "EncryptRoleID=" + item.RoleID });
                                }
                                roleList.Add(objRole);
                            }
                        }
                    }
                }
                userModel.RoleDropDown = roleList;

                var dBMAS_OfficeMasterList = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.LevelID == levelEncryptId).OrderBy(c => c.OfficeName).ToList();

                if (dBMAS_OfficeMasterList != null)
                {
                    foreach (var item in dBMAS_OfficeMasterList)
                    {
                        objOffice = new SelectListItem();
                        objOffice.Text = item.OfficeName;
                        if (officeID == item.OfficeID) // To Add update
                            objOffice.Value = officeEncryptId;
                        else          // To Add all
                        {
                            objOffice.Value = URLEncrypt.EncryptParameters(new String[] { "EncryptOfficeID=" + item.OfficeID });
                        }
                        officeList.Add(objOffice);
                    }
                }
                userModel.OfficeNamesDropDown = officeList;

                return userModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                {
                   // dbKaveriOnlineContext.Dispose();
                }
            }
        }
    }
}