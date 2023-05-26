#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IECDataAuditDetails.cs
    * Author Name       :   Harshit
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Interface for Log Analysis  module.
*/
#endregion

using CustomModels.Models.LogAnalysis.ECDataAuditDetails;
using System.Collections.Generic;

namespace ECDataAPI.Areas.LogAnalysis.Interface
{
    interface IECDataAuditDetails
    {
        List<ECDataAuditDetailsResponseModel> ECDataAuditDetailsList(ECDatatAuditDetailsWrapperModel WrapperModel);
        int ECDataAuditDetailsListTotalCount(ECDataAuditDetailsRequestModel model);
        ECDataAuditDetailsRequestModel ECDataAuditDetailsRequestModel();
        //string LoadMasterTables(long LogID, int LOGTYPEID, int SROCode, long ItemID);
        List<OfficeModificationOccurenceModel> GetOfficeWiseModificationOccurences(ECDatatAuditDetailsWrapperModel WrapperModel);
        List<MasterTableModel>  MasterTablesListForPDF(WrapperModelForDescPDF wrapperModel);
        List<ECDataAuditDetailsResponseModel> GetECDataAuditDetailsListForPDF(ECDataAuditDetailsRequestModel model);
    }
}
