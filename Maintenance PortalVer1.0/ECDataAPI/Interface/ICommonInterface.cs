using CustomModels.Common;
using CustomModels.Models.Common;
using ECDataAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECDataAPI.Interface
{
    public interface ICommonInterface
    {
        bool IsAuthorizedClient(ClientAuthenticationModel model);
        MasterDropDownModel FillMasterDropDownModel();

        bool CheckUserPermissionForRole(short roleID, string area, string controller, string action);

      //  FileDisplayModel AddInformationToDocument(FileDisplayModel model);

       // FileDisplayModel AddMetaDataToDocument(SSRReportToBytesModel model, MetaDataInfoModel InfoModel);

        //string UploadSignedDocument(FileDisplayModel fileData);

        //FileDisplayModel GetSSRReportWithMetaData(SSRReportToBytesModel model);


        //FileDisplayModel GetFirmCertificateReportWithMetaData(FirmCertificateReportToAddMetadataModel model);

        List<SelectListItem> GetSROfficesListByDisrictID(long DistrictID);

        //Added By Raman Kalegaonkar on 20-05-2019
        string GetSroName(int SROfficeID);
        string GetDroName(int DistrictID);
        List<SelectListItem> GetSROOfficeListByDistrictIDWithFirstRecord(long DistrictID,string FirstRecord);

        short GetLevelIdByOfficeId(short OfficeID);
        Dictionary<int, int> GetSROOfficeListDictionary(long DistrictID);
        List<SelectListItem> GetCDNumberList(int SROCode, string FirstRecord);

        MenuHighlightResponseModel GetMenuDetailsToHighlight(MenuHighlightReqModel reqModel);

    }
}
