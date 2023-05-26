#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Maintenance Portal
    * File Name         :   SupportEnclosureDetailsBAL.cs
    * Author Name       :   Girish I
    * Creation Date     :   26-07-2019
    * Last Modified By  :   Girish I
    * Last Modified On  :   03-10-2019
    * Description       :   BAL for Support Enclosure
*/
#endregion

using CustomModels.Models.SupportEnclosure;
using ECDataAPI.Areas.SupportEnclosure.DAL;
using ECDataAPI.Areas.SupportEnclosure.Interface;
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.SupportEnclosure.BAL
{
    public class SupportEnclosureDetailsBAL : ISupportEnclosureDetails
    {
        ISupportEnclosureDetails supportEnclosureDal = new SupportEnclosureDetailsDAL();

        public SupportEnclosureDetailsResModel GetSupportDocumentEnclosureBytes(SupportEnclosureDetailsModel model)
        {
            try
            {
                using (EcDataService.ECDataService service = new EcDataService.ECDataService())
                {
                    FileContentResponseModel resServiceModel = new FileContentResponseModel();
                    FileContentRequestModel reqServiceModel = new FileContentRequestModel();
                    SupportEnclosureDetailsResModel resModel = null;
                    reqServiceModel.isZipRequired = false;
                    reqServiceModel.FilePath = model.FilePath;
                    resServiceModel = service.GetFileContent(reqServiceModel);

                    if (resServiceModel != null)
                    {
                        resModel = new SupportEnclosureDetailsResModel();
                        if (resServiceModel.isError)
                        {
                            resModel.IsError = resServiceModel.isError;
                            resModel.ErrorMessage = resServiceModel.sErrorMsg;
                        }
                        else
                        {
                            resModel.EnclosureFileContent = resServiceModel.FileContent;
                        }
                        return resModel;
                    }
                    else
                    {
                        resModel = new SupportEnclosureDetailsResModel();
                        resModel.IsError = true;
                        resModel.ErrorMessage = "Error Occured while Fetching file from Central Server";
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SupportEnclosureDetailsResModel GetSupportDocumentEnclosureTableData(SupportEnclosureDetailsViewModel model)
        {
            return supportEnclosureDal.GetSupportDocumentEnclosureTableData(model);
        }

        public int GetSupportDocumentEnclosureTotalCount(SupportEnclosureDetailsViewModel model)
        {
            return supportEnclosureDal.GetSupportDocumentEnclosureTotalCount(model);
        }

        public SupportEnclosureDetailsResModel GetSupportPartyEnclosureTableData(SupportEnclosureDetailsViewModel model)
        {
            return supportEnclosureDal.GetSupportPartyEnclosureTableData(model);
        }

        public int GetSupportPartyEnclosureTotalCount(SupportEnclosureDetailsViewModel model)
        {
            return supportEnclosureDal.GetSupportPartyEnclosureTotalCount(model);
        }

        public SupportEnclosureDetailsViewModel SupportEnclosureDetails(int OfficeID)
        {
            return supportEnclosureDal.SupportEnclosureDetails(OfficeID);
        }
    }
}