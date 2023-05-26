#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DocCentralizationStatusBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.DocCentralizationStatus;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class DocCentralizationStatusBAL : IDocCentralizationStatus
    {
        IDocCentralizationStatus DocCentralizationStatusDAL = new DocCentralizationStatusDAL();

        /// <summary>
        /// returns HighValueProperties Request Model
        /// </summary>
        /// <returns></returns>
        public DocCentrStatusReqModel DocCentralizationStatusView(int OfficeID)
        {
            return DocCentralizationStatusDAL.DocCentralizationStatusView(OfficeID);

        }

        /// <summary>
        /// returns HighValuePropDetailsResponseModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DocCentrStatusResModel LoadDocCentralizationStatusDataTable(DocCentrStatusReqModel model)
        {
            return DocCentralizationStatusDAL.LoadDocCentralizationStatusDataTable(model);

        }
    }
}