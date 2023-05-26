#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   UserDetailBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for User Management module.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CustomModels.Models.Common;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.DAL;
using ECDataAPI.Areas.UserManagement.Interface;

namespace ECDataAPI.Areas.UserManagement.BAL
{
    public class UserDetailBAL:IUserDetail
    {

        IUserDetail objDAL = new UserDetailDAL();


        /// <summary>
        /// Registers User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserActivationModel RegisterUser(UserModel model)
        {
            return objDAL.RegisterUser(model);

        }

        /// <summary>
        /// Checks UserNameAvailability
        /// </summary>
        /// <param name="encryptedID"></param>
        /// <returns></returns>
        public bool CheckUserNameAvailability(string encryptedID)
        {

          //  bool result = objDAL.CheckUserNameAvailability(encryptedID);
            return objDAL.CheckUserNameAvailability(encryptedID);

        }

        /// <summary>
        ///  Activates Account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserActivationModel AccountActivation(UserActivationModel model)
        {
            return objDAL.AccountActivation(model);

        }

        /// <summary>
        /// ForgotPassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserActivationModel ForgotPassword(UserActivationModel model)
        {
            return objDAL.ForgotPassword(model);

        }

        /// <summary>
        /// Sends Email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SendEmail(EmailModel model)
        {
            return objDAL.SendEmail(model);

        }

        /// <summary>
        /// returns AccountActivationByLink
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string AccountActivationByLink(string Id)
        {
            return objDAL.AccountActivationByLink(Id);

        }

        /// <summary>
        /// Forgot Password By Link
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string ForgotPasswordByLink(ChangePasswordModel model)
        {
            return objDAL.ForgotPasswordByLink(model);
        }

        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public LoginResponseModel UserLogin(LoginViewModel model)
        {
            return objDAL.UserLogin(model);
        }
    }
}