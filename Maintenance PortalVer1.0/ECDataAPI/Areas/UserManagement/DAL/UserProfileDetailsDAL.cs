using CustomModels.Models.Alerts;
using CustomModels.Models.UserManagement;
using ECDataAPI.DAL;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
//using KaveriUI.Models.UserRegistration;
//using KaveriUI.Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ECDataAPI.Areas.UserManagement.DAL
{
    public class UserProfileDetailsDAL
    {
        /// <summary>
        /// Edits UserProfileDetails
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>

        public UserProfileModel EditUserProfileDetails(string userID)
        {
            KaveriEntities dbContext = null;

            using (dbContext = new KaveriEntities())
            {

                UMG_UserProfile userProfileobj = new UMG_UserProfile();
                UserProfileModel userModelvar = new UserProfileModel();
                long SessionUserID = Convert.ToInt32(userID);
                userProfileobj = dbContext.UMG_UserProfile.Where(a => a.UserID == SessionUserID).FirstOrDefault();
                userModelvar.FirstName = userProfileobj.FirstName == null ? "-" : userProfileobj.FirstName;
                userModelvar.LastName = userProfileobj.LastName == null ? "-" : userProfileobj.LastName;
                userModelvar.Address1 = userProfileobj.Address1 == null ? "-" : userProfileobj.Address1;
                userModelvar.Pincode = userProfileobj.Pincode == null ? "-" : userProfileobj.Pincode;
                userModelvar.IDProofID = Convert.ToInt16(userProfileobj.IDProofTypeID);
                userModelvar.CountryID = userProfileobj.CountryID;
                userModelvar.MobileNumber = userProfileobj.MobileNumber == null ? "-" : userProfileobj.MobileNumber;
                userModelvar.Email = userProfileobj.Email == null ? "-" : userProfileobj.Email;

                userModelvar.IDProofNumber = userProfileobj.IDProofNumber == null ? "-" : userProfileobj.IDProofNumber;
                userModelvar.UserID = Convert.ToInt32(userProfileobj.UserID);
                var UMG_Countriesobj = dbContext.MAS_Countries.Where(a => a.CountryID == userProfileobj.CountryID).FirstOrDefault();
                if (UMG_Countriesobj != null)
                    userModelvar.CountryName = UMG_Countriesobj.CountryName;
                // int Id = Convert.ToInt16(UserProfileobj.IDProofNumber);
                var MAS_IDProofTypesobj = dbContext.MAS_IDProofTypes.Where(a => a.IDProofTypeID == userModelvar.IDProofID).FirstOrDefault();
                if (MAS_IDProofTypesobj != null)
                    userModelvar.IDProofName = MAS_IDProofTypesobj.Description == null ? "-" : MAS_IDProofTypesobj.Description;
                else
                    userModelvar.IDProofName = "-";

                // Added by shubham bhagat on 18-04-2019 
                UMG_UserDetails userDetailsOBJ = dbContext.UMG_UserDetails.Where(a => a.UserID == SessionUserID).FirstOrDefault();
                if (userDetailsOBJ != null)
                {
                    userModelvar.Username = userDetailsOBJ.UserName;
                }
                return userModelvar;
            }
        }

        /// <summary>
        /// Save ChangedPassword
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        public ChangePasswordResponseModel SaveChangedPassword(ChangePasswordNewModel model)
        {
           // viewModel = null;
            KaveriEntities dbContext = null;
            try
            {
                 
                ChangePasswordResponseModel changePasswordResponseModel = new ChangePasswordResponseModel();
                
                using (dbContext = new KaveriEntities())
                {
                    UMG_UserDetails userDetailsObj = new UMG_UserDetails();
                    userDetailsObj = dbContext.UMG_UserDetails.Where(a => a.UserID == model.userID).FirstOrDefault();

                    if (userDetailsObj.Password == model.CurrentPassword)//password validation
                    {

                        int NoOfPreviousPasswordsMatch = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfPreviousPasswordNotAllowed"]);

                        using (TransactionScope ts = new TransactionScope())
                        {
                             if (userDetailsObj.Password == model.NewPassword)
                            {
                                changePasswordResponseModel.status = false;
                                changePasswordResponseModel.message = "New Paassword should not be same as Old Password , Please enter different Password.";
                                return changePasswordResponseModel;
                            }

                            var isPasswordSameAsPrevious = (from t in dbContext.UMG_PasswordHistory where t.UserID == model.userID orderby
                                                         t.ChangeDateTime descending select t.Password).Take(NoOfPreviousPasswordsMatch).
                                                         Contains(model.NewPassword);
                            if (isPasswordSameAsPrevious)
                            {
                                changePasswordResponseModel.status = false;
                                changePasswordResponseModel.message = "New password should not be same as previous " + NoOfPreviousPasswordsMatch + " passwords. Kindly enter other password.";
                                return changePasswordResponseModel;
                            }
                            else
                            {
                              
                                userDetailsObj.Password = model.NewPassword;
                                userDetailsObj.PasswordChangeDate = DateTime.Now;
                                // On 9-4-2019 by Shubham Bhagat for password change on first login
                                userDetailsObj.IsFirstLoginDone = true;

                                UMG_PasswordHistory PasswordHistoryObj = new UMG_PasswordHistory();
                                PasswordHistoryObj.Password = model.NewPassword;
                                PasswordHistoryObj.ChangeDateTime = DateTime.Now;
                                PasswordHistoryObj.UserID = model.userID;
                                PasswordHistoryObj.ID = dbContext.UMG_PasswordHistory.Max(ad => (Int64?)ad.ID) == null ? 1 : dbContext.UMG_PasswordHistory.Max(ad => (Int64)ad.ID) + 1;
                                dbContext.UMG_PasswordHistory.Add(PasswordHistoryObj);
                                dbContext.Entry(userDetailsObj).State = EntityState.Modified;
                                                              

                                dbContext.SaveChanges();
                                ts.Complete();
                            }
                            changePasswordResponseModel.status = true;
                            changePasswordResponseModel.message = "Password Has Been Changed";
                            return changePasswordResponseModel;
                        }




                    }
                    else
                    {
                        changePasswordResponseModel.status = false;
                        changePasswordResponseModel.message = "Invalid Current Password";

                    }

                    return changePasswordResponseModel;
                }

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception )
            {
                throw ;
            }
           
        }

        /// <summary>
        /// Updates UserProfileDetails
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        public UserProfileDetailsResponseModel UpdateUserProfileDetails(UserProfileModel ViewModel)
        {
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                // bool ResponseModelVar;
                UserProfileDetailsResponseModel responseModelVar = new UserProfileDetailsResponseModel();
                UMG_UserProfile UserProfileobj = new UMG_UserProfile();
                UserProfileobj = dbContext.UMG_UserProfile.Where(a => a.UserID == ViewModel.UserID).FirstOrDefault();
                List<string> list = new List<string>();
                list.Add(UserProfileobj.MobileNumber);
                //var MobileNoList = dbContext.UMG_UserProfile.Select(a => a.MobileNumber).ToList();
                // commented by shubham bhagat on 18-04-2019
                //bool IsMobileNoExists = MobileNoList.Except(list).Contains(ViewModel.MobileNumber);//Ckeck if mobileNo alredy present or not.

                using (TransactionScope ts = new TransactionScope())
                {
                    //cc.CCAddressID = MaxId;
                    //cc.CCAddressID = Viewmodel.CCAddressID;

                    // commented by shubham bhagat on 18-04-2019
                    //if (IsMobileNoExists)
                    //{
                    //    responseModelVar.Status = false;
                    //    responseModelVar.ErrorMessage = "Mobile No alredy exist";
                    //    return responseModelVar;

                    //}

                    UserProfileobj.FirstName = ViewModel.FirstName;
                    UserProfileobj.LastName = ViewModel.LastName;
                    UserProfileobj.Address1 = ViewModel.Address1;

                    if (ViewModel.MobileNumber != UserProfileobj.MobileNumber)
                    {
                        UserProfileobj.IsMobileNumVerified = false;
                    }

                    UserProfileobj.MobileNumber = ViewModel.MobileNumber;
                    UserProfileobj.IDProofTypeID = ViewModel.IDProofID;
                    UserProfileobj.IDProofNumber = ViewModel.IDProofNumber;
                    UserProfileobj.CountryID = Convert.ToInt16(ViewModel.CountryID);
                    UserProfileobj.Pincode = ViewModel.Pincode;
                    // db.CC_ApplicantAddressDetails.Add(cc);

                    // Added by shubham bhagat on 18-04-2019 
                    UserProfileobj.Email = ViewModel.Email;
                    dbContext.Entry(UserProfileobj).State = EntityState.Modified;                  
                    int returnValue = dbContext.SaveChanges();
                    ts.Complete();


                    responseModelVar.Status = true;
                    responseModelVar.ErrorMessage = "Profile Updated Successfully";
                    return responseModelVar;

                }

            }
            catch (Exception )
            {

                throw ;

            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();

            }
        }

        /// <summary>
        /// Checks MobileNoAvailability
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CheckMobileNoAvailability(UserProfileModel model)
        {
            KaveriEntities dbcontext = null;
            bool IsMobileNoExists = false;
            try
            {
                dbcontext = new KaveriEntities();
                UMG_UserProfile objUserProfileModel = new UMG_UserProfile();
                objUserProfileModel = dbcontext.UMG_UserProfile.Where(a => a.UserID == model.UserID).FirstOrDefault();

                List<string> list = new List<string>();
                list.Add(objUserProfileModel.MobileNumber);
                var MobileNoList = dbcontext.UMG_UserProfile.Select(a => a.MobileNumber).ToList();
                IsMobileNoExists = MobileNoList.Except(list).Contains(model.MobileNumber);//Ckeck if mobileNo alredy present or not.
            }
            catch (Exception )
            {

                throw ;
            }
            finally
            {
                if (dbcontext != null)
                    dbcontext.Dispose();
            }
            return IsMobileNoExists;
        }
    }
}