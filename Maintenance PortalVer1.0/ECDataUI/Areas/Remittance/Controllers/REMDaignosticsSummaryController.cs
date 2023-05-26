#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   REMDaignosticsSummaryController.cs
    * Author Name       :   Raman Kalegaonkar	
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.REMDashboard;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]

    public class REMDaignosticsSummaryController : Controller
    {
        #region Properties
        ServiceCaller caller = null;
        #endregion

        #region Methods
        /// <summary>
        /// Get Office List Summary
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Get Office List Summary")]
        public ActionResult GetOfficeListSummary()
        {
            try
            {
                RemittanceOfficeListSummaryModel model = new RemittanceOfficeListSummaryModel();
                caller = new ServiceCaller("REMDaignosticsSummaryApiController");
                model = caller.GetCall<RemittanceOfficeListSummaryModel>("GetOfficeListSummary");
                // Added BY SB on 2-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DiagnosticSummary;
                return View(model);
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }    
        }
        #endregion
    }
}