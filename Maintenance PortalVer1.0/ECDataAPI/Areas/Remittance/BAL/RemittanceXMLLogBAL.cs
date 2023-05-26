#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RemittanceXMLLogBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.RemittanceXMLLog;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class RemittanceXMLLogBAL : IRemittanceXMLLog
    {
        IRemittanceXMLLog dalOBJ = new RemittanceXMLLogDAL();

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public RemittXMLLogModel GetOfficeList(String OfficeType)
        {
            return dalOBJ.GetOfficeList(OfficeType);
        }

        /// <summary>
        /// RemittanceXMLLogDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RemittXMLLogModel RemittanceXMLLogDetails(REMRequestXMLLogModel model)
        {
            return dalOBJ.RemittanceXMLLogDetails(model);
        }

        /// <summary>
        /// DownloadREMLogXml
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FileDownloadModel DownloadREMLogXml(REMRequestXMLLogModel model)
        {
            return dalOBJ.DownloadREMLogXml(model);
        }
    }
}