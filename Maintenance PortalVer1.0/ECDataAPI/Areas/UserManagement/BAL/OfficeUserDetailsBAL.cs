#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   OfficeUserDetailsBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for User Management module.
*/
#endregion

using CustomModels.Models.Common;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.DAL;
using ECDataAPI.Areas.UserManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.UserManagement.BAL
{
    public class OfficeUserDetailsBAL : IOfficeUser
    {

        IOfficeUser objDAL = new OfficeUserDetailsDAL();


        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserActivationModel RegisterUser(OfficeUserDetailsModel model)
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

            return objDAL.CheckUserNameAvailability(encryptedID);

        }

        /// <summary>
        /// Updates OfficeUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserActivationModel UpdateOfficeUser(OfficeUserDetailsModel model)
        {
            return objDAL.UpdateOfficeUser(model);
        }
        //public bool UpdateOfficeUser(OfficeUserDetailsModel model)
        //{
        //    return objDAL.UpdateOfficeUser(model);
        //}

        /// <summary>
        /// Deletes OfficeUser
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public bool DeleteOfficeUser(string EncryptedId)
        {
           return objDAL.DeleteOfficeUser(EncryptedId);
        }

        /// <summary>
        /// Gets UserDetails
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public OfficeUserDetailsModel GetUserDetails(string EncryptedId)
        {
            return objDAL.GetUserDetails(EncryptedId);
        }

        /// <summary>
        /// Loads OfficeUserDetailsGridData
        /// </summary>
        /// <returns></returns>

        //Commented by mayank on 06-08-2021
        //public UserGridWrapperModel LoadOfficeUserDetailsGridData()
        //{
        //    return objDAL.LoadOfficeUserDetailsGridData();
        //}

        public UserGridWrapperModel LoadOfficeUserDetailsGridData(int officeID, int LevelID)
        {
            return objDAL.LoadOfficeUserDetailsGridData(officeID, LevelID);
        }

        /// <summary>
        /// Gets OfficeUserDetailsList
        /// </summary>
        /// <returns></returns>
        public OfficeUserDetailsModel GetOfficeUserDetailsList()
        {
            return objDAL.GetOfficeUserDetailsList();
        }

        /// <summary>
        /// Gets OfficeDetailsInfo
        /// </summary>
        /// <param name="officeDetailId"></param>
        /// <returns></returns>
        public OfficeUserDetailsModel GetOfficeDetailsInfo(String officeDetailId)
        {
            return objDAL.GetOfficeDetailsInfo(officeDetailId);
        }

        // Added by Shubham Bhagat on 8-4-2019
        public OfficeUserDetailsModel GetRoleListByLevel(String LevelID, int roleID = 0, String roleEncryptId = null, int officeID = 0, String officeEncryptId = null)
        {
            return objDAL.GetRoleListByLevel(LevelID);
        }
    }
}