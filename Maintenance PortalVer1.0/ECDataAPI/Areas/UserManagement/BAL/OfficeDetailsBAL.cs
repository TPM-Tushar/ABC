#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   OfficeDetailsBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for User Management module.
*/
#endregion

using ECDataAPI.Areas.UserManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.DAL;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.BAL
{
    public class OfficeDetailsBAL : IOfficeDetails
    {
        IOfficeDetails dalOfficeDetails = new OfficeDetailsDAL();

        /// <summary>
        /// Creates NewOffice
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OfficeDetailsModel CreateNewOffice(OfficeDetailsModel model)
        {
            return dalOfficeDetails.CreateNewOffice(model);
        }

        /// <summary>
        /// Deletes Office
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public bool DeleteOffice(string EncryptedId)
        {
            return dalOfficeDetails.DeleteOffice(EncryptedId);
        }

        //public OfficeDetailsModel GetAllOfficeDetailsList(int DistrictId)
        //{
        //    return dalOfficeDetails.GetAllOfficeDetailsList(DistrictId);
        //}

        /// <summary>
        /// Gets AllOfficeDetailsList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public OfficeDetailsModel GetAllOfficeDetailsList(int OfficeType)
        {
            return dalOfficeDetails.GetAllOfficeDetailsList(OfficeType);
        }

        /// <summary>
        /// Gets OfficeDetails
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public OfficeDetailsModel GetOfficeDetails(string EncryptedId)
        {
            return dalOfficeDetails.GetOfficeDetails(EncryptedId);
        }

        /// <summary>
        /// Loads OfficeDetailsGridData
        /// </summary>
        /// <returns></returns>
        public OfficeGridWrapperModel LoadOfficeDetailsGridData()
        {
            return dalOfficeDetails.LoadOfficeDetailsGridData();
        }

        /// <summary>
        /// Updates Office
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OfficeDetailsModel UpdateOffice(OfficeDetailsModel model)
        {
            return dalOfficeDetails.UpdateOffice(model);
        }

        /// <summary>
        /// Gets TalukasByDistrictID
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <param name="isForUpdate"></param>
        /// <param name="talukaId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetTalukasByDistrictID(short DistrictID, bool isForUpdate, int talukaId = 0)
        {
            return dalOfficeDetails.GetTalukasByDistrictID(DistrictID);
        }

    }
}