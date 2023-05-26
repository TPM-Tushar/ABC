#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   OfficeWiseDiagnosticStatusBAL.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.OfficeWiseDiagnosticStatus;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class OfficeWiseDiagnosticStatusBAL : IOfficeWiseDiagnosticStatus
    {
        IOfficeWiseDiagnosticStatus dalOBJ = new OfficeWiseDiagnosticStatusDAL();

        /// <summary>
        /// OfficeWiseDiagnosticStatusModelView
        /// </summary>
        /// <returns></returns>
        public OfficeWiseDiagnosticStatusModel OfficeWiseDiagnosticStatusModelView()
        {
            return dalOBJ.OfficeWiseDiagnosticStatusModelView();
        }

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        public OfficeWiseDiagnosticStatusModel GetOfficeList(string OfficeType)
        {
            return dalOBJ.GetOfficeList(OfficeType);
        }

        /// <summary>
        /// GetOfficeWiseDiagnosticStatusDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OfficeWiseDiagnosticStatusListModel GetOfficeWiseDiagnosticStatusDetail(OfficeWiseDiagnosticStatusModel model)
        {
            return dalOBJ.GetOfficeWiseDiagnosticStatusDetail(model);
        }

        /// <summary>
        /// GetActionDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DiagnosticActionDetail GetActionDetail(DiagnosticActionDetail model)
        {
            return dalOBJ.GetActionDetail(model);
        }

        /// <summary>
        /// ExportOfficeWiseDiagnosticStatusToExcel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OfficeWiseDiagnosticStatusListModel ExportOfficeWiseDiagnosticStatusToExcel(OfficeWiseDiagnosticStatusModel model)
        {
            return dalOBJ.ExportOfficeWiseDiagnosticStatusToExcel(model);
        }

        public OfficeWiseDiagnosticStatusListModel GetDiagnosticStatusDetailByActionType(OfficeWiseDiagnosticStatusModel model)
        {
            return dalOBJ.GetDiagnosticStatusDetailByActionType(model);
        }
    }
}