/*File Header
 * Project Id: KARIGR [ IN-KA-IGR-02-05 ]
 * Project Name: Kaveri Maintainance Portal
 * File Name: ServicePackBAL.cs
 * Author :Harshit Gupta
 * Creation Date :17 May 2019
 * Desc : Call to DAL Layer
 * ECR No : 300
*/
#region References
using CustomModels.Models.ServicePackDetails;
using ECDataAPI.Areas.ServicePack.DAL;
using ECDataAPI.Areas.ServicePack.Interface;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
#endregion

namespace ECDataAPI.Areas.ServicePack.BAL
{
    public class ServicePackBAL : IServicePack
    {
        IServicePack objDAL = new ServicePackDAL();

        /// <summary>
        /// Get Release Type List
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetReleaseTypeList()
        {
            return objDAL.GetReleaseTypeList();
        }

        /// <summary>
        /// Get Release Type List For Edit
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetReleaseTypeListForEdit(int servicePackID)
        {
            return objDAL.GetReleaseTypeListForEdit(servicePackID);
        }

        /// <summary>
        /// Get Changes Types List
        /// </summary>
        /// <param name="ChangeTypeID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetChangesTypesList(int ChangeTypeID = 0)
        {
            return objDAL.GetChangesTypesList(ChangeTypeID);

        }

        /// <summary>
        /// Get Changes Type List For Edit Call
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetChangesTypeListForEditCall(int servicePackID)
        {
            return objDAL.GetChangesTypeListForEditCall(servicePackID);

        }

        /// <summary>
        /// Add Service Pack Details
        /// </summary>
        /// <param name="servicePackViewModelObj"></param>
        /// <returns></returns>
        public string AddServicePackDetails(ServicePackViewModel servicePackViewModelObj)
        {
            return objDAL.AddServicePackDetails(servicePackViewModelObj);
        }

        /// <summary>
        /// Update Service Pack Details
        /// </summary>
        /// <param name="servicePackViewModelObj"></param>
        /// <returns></returns>
        public string UpdateServicePackDetails(ServicePackViewModel servicePackViewModelObj)
        {
            return objDAL.UpdateServicePackDetails(servicePackViewModelObj);
        }

        /// <summary>
        /// Delete Service Pack Details Entry
        /// </summary>
        /// <param name="spFixedIDToBeDeleted"></param>
        /// <returns></returns>
        public string DeleteServicePackDetailsEntry(int spFixedIDToBeDeleted)
        {
            return objDAL.DeleteServicePackDetailsEntry(spFixedIDToBeDeleted);
        }

        /// <summary>
        /// Get Service Pack Details List
        /// </summary>
        /// <param name="IsRequestForApprovalsList"></param>
        /// <param name="IsRequestForReleasedServiceList"></param>
        /// <returns></returns>
        /// //Added and Changed by mayank on 14/09/2021 for Support Exe Release
        public List<ServicePackViewModel> GetServicePackDetailsList(bool IsRequestForApprovalsList, bool IsRequestForReleasedServiceList, int RoleID)
        {
            return objDAL.GetServicePackDetailsList(IsRequestForApprovalsList, IsRequestForReleasedServiceList,  RoleID);
        }

        /// <summary>
        /// Get Service Pack Details
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns></returns>
        public ServicePackViewModel GetServicePackDetails(int servicePackID)
        {
            return objDAL.GetServicePackDetails(servicePackID);
        }

        /// <summary>
        /// Deactivate Service Pack Details Entry
        /// </summary>
        /// <param name="spIDToBeDeactivated"></param>
        /// <returns></returns>
        public string DeactivateServicePackDetailsEntry(int spIDToBeDeactivated)
        {
            return objDAL.DeactivateServicePackDetailsEntry(spIDToBeDeactivated);
        }

        /// <summary>
        /// Save Release Notes Details
        /// </summary>
        /// <param name="releaseDetailsModelObj"></param>
        /// <returns></returns>
        public string SaveReleaseNotesDetails(ReleaseDetails releaseDetailsModelObj)
        {
            return objDAL.SaveReleaseNotesDetails(releaseDetailsModelObj);
        }

        /// <summary>
        /// Activate Service Pack Details Entry
        /// </summary>
        /// <param name="spFixedIDToBeDeleted"></param>
        /// <returns></returns>
        public string ActivateServicePackDetailsEntry(int spFixedIDToBeDeleted)
        {
            return objDAL.ActivateServicePackDetailsEntry(spFixedIDToBeDeleted);
        }

        //Changed by Omkar on 17-09-2020 
        /// <summary>
        /// Check If Service Pack Version Already Exists
        /// </summary>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <returns></returns>
        public bool CheckIfServicePackVersionAlreadyExists(int majorVersion, int minorVersion, bool releaseType)
        {
            return objDAL.CheckIfServicePackVersionAlreadyExists(majorVersion, minorVersion, releaseType);
        }

        /// <summary>
        /// Download Service Pack File
        /// </summary>
        /// <param name="servicePackID"></param>
        /// <returns></returns>
        public DownloadResponseModel DownloadServicePackFile(int servicePackID)
        {
            return objDAL.DownloadServicePackFile(servicePackID);
        }



        public DownloadResponseModel CheckIfFileExists(int servicePackID)
        {
            return objDAL.CheckIfFileExists(servicePackID);
        }
    }
}