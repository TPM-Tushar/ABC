#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   IntegrationCallExceptionsBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.IntegrationCallExceptions;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class IntegrationCallExceptionsBAL : IIntegrationCallExceptions
    {
        IIntegrationCallExceptions IIntegrationCallExceptionsDAL = new IntegrationCallExceptionsDAL();

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public IntegrationCallExceptionsModel GetOfficeList(String OfficeType)
        {
            return IIntegrationCallExceptionsDAL.GetOfficeList(OfficeType);
        }

        /// <summary>
        /// GetExceptionsDetails
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <param name="OfficeTypeID"></param>
        /// <returns></returns>
        public IEnumerable<IntegrationCallExceptionsModel> GetExceptionsDetails(String OfficeType, String OfficeTypeID)
        {
            return IIntegrationCallExceptionsDAL.GetExceptionsDetails(OfficeType, OfficeTypeID);
        }
    }
}