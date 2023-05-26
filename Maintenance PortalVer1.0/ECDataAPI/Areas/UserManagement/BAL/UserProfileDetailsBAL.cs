#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   UserProfileDetailsBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for User Management module.
*/
#endregion

using CustomModels.Models.Alerts;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.DAL;
//using KaveriUI.Models.UserRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.UserManagement.BAL
{
    public class UserProfileDetailsBAL
    {
        UserProfileDetailsDAL objDAL = null;
        /// <summary>
        /// Edits UserProfileDetails
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        public UserProfileModel EditUserProfileDetails(string EncryptedID)
        {
            objDAL = new UserProfileDetailsDAL();
            return objDAL.EditUserProfileDetails(EncryptedID);

        }

        /// <summary>
        /// Updates UserProfileDetails
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        public UserProfileDetailsResponseModel UpdateUserProfileDetails(UserProfileModel ViewModel)
        {
            objDAL = new UserProfileDetailsDAL();
            UserProfileDetailsResponseModel model = objDAL.UpdateUserProfileDetails(ViewModel);
            return model;

        }

        /// <summary>
        /// Save ChangedPassword
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        public ChangePasswordResponseModel SaveChangedPassword(ChangePasswordNewModel ViewModel)
        {
            objDAL = new UserProfileDetailsDAL();
            return objDAL.SaveChangedPassword(ViewModel);

        }
        /// <summary>
        /// Checks MobileNoAvailability
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CheckMobileNoAvailability(UserProfileModel model)
        {
            objDAL = new UserProfileDetailsDAL();
            return objDAL.CheckMobileNoAvailability(model);

        }

    }
}