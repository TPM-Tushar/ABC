/*File Header
 * Project Id: 
 * Project Name: Kaveri Maintainance Portal
 * File Name: KaveriIntegrationBAL.cs
 * Author : Shubham Bhagat
 * Creation Date :14 Oct 2019
 * Desc : Call to DAL Layer
 * ECR No : 
*/

using CustomModels.Models.KaveriIntegration;
using ECDataAPI.Areas.KaveriIntegration.DAL;
using ECDataAPI.Areas.KaveriIntegration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.KaveriIntegration.BAL
{
    public class KaveriIntegrationBAL : IKaveriIntegration
    {
        /// <summary>
        /// Kaveri Integration View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns Kaveri Integration Model</returns>
        public KaveriIntegrationModel KaveriIntegrationView(int OfficeID)
        {
            return (new KaveriIntegrationDAL().KaveriIntegrationView(OfficeID));
        }

        /// <summary>
        /// Load Kaveri Integration Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns Kaveri Integration Wrapper Model</returns>
        public KaveriIntegrationWrapperModel LoadKaveriIntegrationTable(KaveriIntegrationModel model)
        {
            return (new KaveriIntegrationDAL().LoadKaveriIntegrationTable(model));
        }

        /// <summary>
        /// Other Table Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns KI Details Wrapper Model</returns>
        public KIDetailsWrapperModel OtherTableDetails(KaveriIntegrationModel model)
        {
            return (new KaveriIntegrationDAL().OtherTableDetails(model));
        }
    }
}