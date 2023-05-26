#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ServicePackStatusBAL.cs
    * Author Name       :   Shubham Bhagat  
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.ServicePackStatus;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ServicePackStatusBAL : IServicePackStatus
    {
        IServicePackStatus misReportsDal = new ServicePackStatusDAL();

        /// <summary>
        /// Service Pack Status View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public ServicePackStatusModel ServicePackStatusView(int OfficeID)
        {
            return misReportsDal.ServicePackStatusView(OfficeID);
        }

        /// <summary>
        /// Service Pack Status Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ServicePackStatusDetails> ServicePackStatusDetails(ServicePackStatusModel model)
        {
            return misReportsDal.ServicePackStatusDetails(model);
        }

        /// <summary>
        /// Service Pack Status Total Count
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ServicePackStatusTotalCount(ServicePackStatusModel model)
        {
            return misReportsDal.ServicePackStatusTotalCount(model);
        }
    }
}