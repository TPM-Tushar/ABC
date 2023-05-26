using CustomModels.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.Interface
{
    interface IOfficeDetails
    {
        //OfficeDetailsModel GetAllOfficeDetailsList(int DistrictCode);
        OfficeDetailsModel GetAllOfficeDetailsList(int OfficeTypeId);
        OfficeDetailsModel CreateNewOffice(OfficeDetailsModel model);
        OfficeGridWrapperModel LoadOfficeDetailsGridData();
        OfficeDetailsModel GetOfficeDetails(String EncryptedId);
        OfficeDetailsModel UpdateOffice(OfficeDetailsModel model);
        bool DeleteOffice(String EncryptedId);
        List<SelectListItem> GetTalukasByDistrictID(short DistrictID, bool isForUpdate = false, int talukaId = 0);
    }
}
