#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DblVerificationXMLLogBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.DblVerificationXMLLog;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class DblVerificationXMLLogBAL : IDblVerificationXMLLog
    {
        IDblVerificationXMLLog dalOBJ = new DblVerificationXMLLogDAL();


        /// <summary>
        /// DblVeriXMLLogDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DblVeriXMLLogWrapperModel DblVeriXMLLogDetails(DblVeriReqXMLLogModel model)
        {
            return dalOBJ.DblVeriXMLLogDetails(model);
        }

        /// <summary>
        /// DownloadDblVeriXML
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FileDownloadModel DownloadDblVeriXML(DblVeriReqXMLLogModel model)
        {
            return dalOBJ.DownloadDblVeriXML(model);
        }

      
    }
}