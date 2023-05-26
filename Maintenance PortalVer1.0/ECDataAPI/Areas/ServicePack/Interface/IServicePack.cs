/*File Header
 * Project Id: KARIGR [ IN-KA-IGR-02-05 ]
 * Project Name: Kaveri Maintainance Portal
 * File Name: IServicePack
 * Author :Harshit Gupta
 * Creation Date :17 May 2019
 * Desc : Contract  
 * ECR No : 300
*/


#region References
using CustomModels.Models.ServicePackDetails;
using System;
using System.Collections.Generic;
using System.Web.Mvc; 
#endregion

namespace ECDataAPI.Areas.ServicePack.Interface
{
    public interface IServicePack
    {
        List<SelectListItem> GetReleaseTypeList();
        List<SelectListItem> GetChangesTypesList(int ChangeTypeID = 0);
        string AddServicePackDetails(ServicePackViewModel servicePackViewModelObj);
        string UpdateServicePackDetails(ServicePackViewModel servicePackViewModelObj);
        string DeleteServicePackDetailsEntry(int spFixedIDToBeDeleted);
        string DeactivateServicePackDetailsEntry(int spIDToBeDeactivated);
        //Added and Changed by mayank on 14/09/2021 for Support Exe Release
        List<ServicePackViewModel> GetServicePackDetailsList(bool IsRequestForApprovalsList,bool IsRequestForReleasedServiceList, int RoleID);
        ServicePackViewModel GetServicePackDetails(int servicePackID);
        List<SelectListItem> GetChangesTypeListForEditCall(int servicePackID);
        List<SelectListItem> GetReleaseTypeListForEdit(int servicePackID);
        string SaveReleaseNotesDetails(ReleaseDetails releaseDetailsModelObj);
        string ActivateServicePackDetailsEntry(int spFixedIDToBeDeleted);
        //Changed by Omkar on 17-09-2020 

        bool CheckIfServicePackVersionAlreadyExists(int majorVersion, int minorVersion, bool releaseType);
        DownloadResponseModel DownloadServicePackFile(int servicePackID);

        DownloadResponseModel CheckIfFileExists(int servicePackID);

        
    }
}
