#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   CCConversionLogBAL.cs
    * Author Name       :   Madhusoodan Bisen
    * Creation Date     :   15-09-2020
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL for CC Conversion Logs.
*/
#endregion

using CustomModels.Models.Remittance.CCConversionLog;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class CCConversionLogBAL : ICCConversionLog
    {
        ICCConversionLog dalObj = new CCConversionLogDAL();

        /// <summary>
        /// To return model taken from CC Conversion Log from DAL
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public CCConversionLogWrapperModel CCConversionLogView()
        {
            try
            {
                return dalObj.CCConversionLogView();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get CC Conversion Logs from DAL
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns model</returns>
        public CCConversionLogWrapperModel CCConversionLogDetails(CCConversionLogReqModel model)
        {
            try
            {
                return dalObj.CCConversionLogDetails(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get total count of CC Conversion Logs from DAL
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns model</returns>
        public int GetCCConversionLogDetailsTotalCount(CCConversionLogReqModel model)
        {
            return dalObj.GetCCConversionLogDetailsTotalCount(model);
        }
    }
}