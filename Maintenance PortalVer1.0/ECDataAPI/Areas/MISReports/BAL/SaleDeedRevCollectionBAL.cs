#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SaleDeedRevCollectionBAL.cs
    * Author Name       :   Shubham Bhagat  
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.SaleDeedRevCollection;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class SaleDeedRevCollectionBAL: ISaleDeedRevCollection
    {
        ISaleDeedRevCollection misReportsDal = new SaleDeedRevCollectionDAL();
        /// <summary>
        /// Returns SaleDeedRevCollectionModel Required to show SaleDeedRevCollectionView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public SaleDeedRevCollectionModel SaleDeedRevCollectionView(int OfficeID)
        {
            return misReportsDal.SaleDeedRevCollectionView(OfficeID);

        }


        /// <summary>
        /// returns List of SaleDeedRevCollectionDetail to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SaleDeedRevCollectionOuterModel GetSaleDeedRevCollectionDetails(SaleDeedRevCollectionModel model)
        {
            return misReportsDal.GetSaleDeedRevCollectionDetails(model);

        }


        /// <summary>
        /// Returns Total Count of GetSaleDeedRevCollectionDetails List
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int GetSaleDeedRevCollectionDetailsTotalCount(SaleDeedRevCollectionModel model)
        {
            return misReportsDal.GetSaleDeedRevCollectionDetailsTotalCount(model);

        }

    }
}