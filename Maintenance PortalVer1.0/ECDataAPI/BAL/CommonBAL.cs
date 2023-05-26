using CustomModels.Common;
using CustomModels.Models.Common;
using ECDataAPI.DAL;
using ECDataAPI.Interface;
using ECDataAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.BAL
{
    public class CommonBAL : ICommonInterface
    {
        ICommonInterface objDAL = new CommonDAL();
        public bool IsAuthorizedClient(ClientAuthenticationModel model)
        {
            return objDAL.IsAuthorizedClient(model);

        }
        public MasterDropDownModel FillMasterDropDownModel()
        {
            return objDAL.FillMasterDropDownModel();
        }

        /// <summary>
        /// Validation role has permission or not
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="area"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns>This Method Returns boolean after Authorization of Particular User</returns>

        public bool CheckUserPermissionForRole(short roleID, string area, string controller, string action)
        {
            return objDAL.CheckUserPermissionForRole(roleID, area, controller, action);
        }

        //public FileDisplayModel AddInformationToDocument(FileDisplayModel model)
        //{
        //    return objDAL.AddInformationToDocument(model);
        //}

        public List<SelectListItem> GetSROfficesListByDisrictID(long DistrictID)
        {
            return objDAL.GetSROfficesListByDisrictID(DistrictID);
        }

        public string GetSroName(int SROfficeID)
        {
            return objDAL.GetSroName(SROfficeID);

        }

        public string GetDroName(int DistrictID)
        {
            return objDAL.GetDroName(DistrictID);

        }

        public List<SelectListItem> GetSROOfficeListByDistrictIDWithFirstRecord(long DistrictID, string FirstRecord)
        {
            return objDAL.GetSROOfficeListByDistrictIDWithFirstRecord(DistrictID, FirstRecord);
        }

        public short GetLevelIdByOfficeId(short OfficeID)
        {
            return objDAL.GetLevelIdByOfficeId(OfficeID);
        }

        public Dictionary<int, int> GetSROOfficeListDictionary(long DistrictID)
        {
            return objDAL.GetSROOfficeListDictionary(DistrictID);
        }

        public List<SelectListItem> GetCDNumberList(int SROCode, string FirstRecord)
        {
            return objDAL.GetCDNumberList(SROCode, FirstRecord);
        }


        public MenuHighlightResponseModel GetMenuDetailsToHighlight(MenuHighlightReqModel reqModel)
        {
            return objDAL.GetMenuDetailsToHighlight(reqModel);
        }
    }
}