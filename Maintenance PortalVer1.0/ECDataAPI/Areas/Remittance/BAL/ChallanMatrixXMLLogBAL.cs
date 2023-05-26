#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ChallanMatrixXMLLogBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.ChallanMatrixXMLLog;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class ChallanMatrixXMLLogBAL: IChallanMatrixXMLLog
    {
        IChallanMatrixXMLLog dalOBJ = new ChallanMatrixXMLLogDAL();

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public ChallanMatrixWrapperModel GetOfficeList(String OfficeType)
        {
            return dalOBJ.GetOfficeList(OfficeType);
        }

        /// <summary>
        /// ChallanMatrixDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ChallanMatrixWrapperModel ChallanMatrixDetails(ChallanMatrixLogRequestModel model)
        {
            return dalOBJ.ChallanMatrixDetails(model);
        }

        /// <summary>
        /// DownloadChallanMatrixXML
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FileDownloadModel DownloadChallanMatrixXML(ChallanMatrixLogRequestModel model)
        {
            return dalOBJ.DownloadChallanMatrixXML(model);
        }
    }
}