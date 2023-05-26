#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ECDataAuditDetailsBAL.cs
    * Author Name       :   Harshit
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Log Analysis  module.
*/
#endregion

using CustomModels.Models.LogAnalysis.ECDataAuditDetails;
using ECDataAPI.Areas.LogAnalysis.DAL;
using ECDataAPI.Areas.LogAnalysis.Interface;
using System.Collections.Generic;

namespace ECDataAPI.Areas.LogAnalysis.BAL
{
    public class ECDataAuditDetails : IECDataAuditDetails
    {
        IECDataAuditDetails ecDataDAL = new ECDataAuditDetailsDAL();

        /// <summary>
        /// returns ECDataAuditDetails List
        /// </summary>
        /// <param name="WrapperModel"></param>
        /// <returns></returns>
        public List<ECDataAuditDetailsResponseModel> ECDataAuditDetailsList(ECDatatAuditDetailsWrapperModel WrapperModel)
        {
            return ecDataDAL.ECDataAuditDetailsList(WrapperModel);

        }

        /// <summary>
        /// Returns Total Count of ECDATAAuditdtails List
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ECDataAuditDetailsListTotalCount(ECDataAuditDetailsRequestModel model)
        {
            return ecDataDAL.ECDataAuditDetailsListTotalCount(model);
        }
        //public List<ECDataAuditDetailsResponseModel> GetECDataAuditDetailsListForPDF(ECDataAuditDetailsRequestModel model)
        //{
        //    return ecDataDAL.GetECDataAuditDetailsListForPDF(model);

        //}


        /// <summary>
        /// returns ECDataAuditDetails Request Model
        /// </summary>
        /// <returns></returns>
        public ECDataAuditDetailsRequestModel ECDataAuditDetailsRequestModel()
        {
            return ecDataDAL.ECDataAuditDetailsRequestModel();

        }


        //public string LoadMasterTables(long LogID, int LOGTYPEID, int SROCode, long ItemID)
        //{
        //    return ecDataDAL.LoadMasterTables(LogID, LOGTYPEID, SROCode, ItemID);

        //}
        public List<OfficeModificationOccurenceModel> GetOfficeWiseModificationOccurences(ECDatatAuditDetailsWrapperModel WrapperModel)
        {
            return ecDataDAL.GetOfficeWiseModificationOccurences(WrapperModel);

        }
      
        /// <summary>
        /// Returns MasterTableListModel
        /// </summary>
        /// <param name="wrapperModel"></param>
        /// <returns></returns>
        public List<MasterTableModel> MasterTablesListForPDF(WrapperModelForDescPDF wrapperModel)
        {
            return ecDataDAL.MasterTablesListForPDF(wrapperModel);

        }
        /// <summary>
        /// Returns GetECDataAuditDetailsList For PDF
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ECDataAuditDetailsResponseModel> GetECDataAuditDetailsListForPDF(ECDataAuditDetailsRequestModel model)
        {
            return ecDataDAL.GetECDataAuditDetailsListForPDF(model);

        }


    }
}