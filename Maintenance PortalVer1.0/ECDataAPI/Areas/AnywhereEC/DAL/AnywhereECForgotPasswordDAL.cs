using CustomModels.Models.Alerts;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.AnywhereEC.Interface;
using ECDataAPI.Entity.KaveriEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ECDataAPI.Areas.AnywhereEC.DAL
{
    public class AnywhereECForgotPasswordDAL : IAnywhereECForgotPassword
    {

        public ChangePasswordNewModel SaveNewPasswordView(int userid)
        {
            KaveriEntities dbContext = null;
            try
            {
                ChangePasswordNewModel changePasswordNewModel = new ChangePasswordNewModel();
                using (dbContext = new KaveriEntities())
                {
                    //using above userid fetch the user and check if it has entry in anywhere ec table using mapping
                    UMG_UserDetails uMG_UserDetails = new UMG_UserDetails();
                    uMG_UserDetails = dbContext.UMG_UserDetails.Where(a => a.UserID == userid).FirstOrDefault();

                    //for testing hardcoded values is been used
                    changePasswordNewModel.Message = "User Found";
                }
                return changePasswordNewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ChangePasswordResponseModel SaveNewPassword(ChangePasswordNewModel model)
        {
            // viewModel = null;
            KaveriEntities dbContext = null;
            try
            {
                ChangePasswordResponseModel changePasswordResponseModel = new ChangePasswordResponseModel();
                using (dbContext = new KaveriEntities())
                {
                    UMG_UserDetails uMG_UserDetails = new UMG_UserDetails();
                    uMG_UserDetails = dbContext.UMG_UserDetails.Where(a => a.UserID == model.userID).FirstOrDefault();

                    var AnywhereECUserId = dbContext.REG_EC_KaveriPortal_UserMapping.Where(x => x.KaveriPortalUserID == model.userID).Select(x => x.AnywhereECUserID).FirstOrDefault();
                    if (AnywhereECUserId > 0)
                    {
                        REG_Users regUserDetails = new REG_Users();
                        regUserDetails = dbContext.REG_Users.Where(a => a.UserId == AnywhereECUserId).FirstOrDefault();
                        if(regUserDetails != null)
                        {
                            if ((bool)regUserDetails.IsActive)
                            {
                                if(regUserDetails.Password == model.NewPassword || regUserDetails.PrevPassword == model.NewPassword)
                                {
                                    changePasswordResponseModel.message="New password should not be same as previous passwords. Kindly enter other password.";
                                    changePasswordResponseModel.status = false;
                                    return changePasswordResponseModel;
                                }

                                using (TransactionScope ts = new TransactionScope())
                                {
                                    regUserDetails.PrevPassword = regUserDetails.Password;
                                    regUserDetails.Password = model.NewPassword;                                   
                                    regUserDetails.PasswordChangeDate = DateTime.Now;

                                    dbContext.Entry(regUserDetails).State = (System.Data.Entity.EntityState)EntityState.Modified;
                                    dbContext.SaveChanges();
                                    ts.Complete();
                                }

                                //send SMS
                                string SMS_Text = @"KAVERI notification for password reset: Your Login Name is : " + regUserDetails.LoginName + ", Password is : " + model.Password;
                                if(!String.IsNullOrEmpty(regUserDetails.PhoneNumber))
                                {
                                    PreRegApplicationDetailsService.ApplicationDetailsService smsService = new PreRegApplicationDetailsService.ApplicationDetailsService();
                                    smsService.SendSMS(SMS_Text, regUserDetails.PhoneNumber);
                                }
                                

                                changePasswordResponseModel.status = true;
                                changePasswordResponseModel.message = "Password Has Been Changed";
                                return changePasswordResponseModel;
                            }
                            else
                            {
                                changePasswordResponseModel.status = false;
                                changePasswordResponseModel.message = "User is not active";
                                return changePasswordResponseModel;
                            }
                        }
                        else
                        {
                            changePasswordResponseModel.status = false;
                            changePasswordResponseModel.message = "User not found";
                            return changePasswordResponseModel;
                        }                                              
                    }
                    else
                    {
                        changePasswordResponseModel.status = false;
                        changePasswordResponseModel.message = "User not found";
                        return changePasswordResponseModel;
                    }
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}