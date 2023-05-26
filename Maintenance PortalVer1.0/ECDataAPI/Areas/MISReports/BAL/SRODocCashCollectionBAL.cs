#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SRODocCashCollectionBAL.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.SRODocCashCollection;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class SRODocCashCollectionBAL
    {




        SRODocCashCollectionDAL misReportsDal = new SRODocCashCollectionDAL();

        /// <summary>
        /// Returns SRODocCashCollectionResponseModel to show SRODocCashCollectionView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public SRODocCashCollectionResponseModel SRODocCashCollectionView(int OfficeID)
        {
            return misReportsDal.SRODocCashCollectionView(OfficeID);

        }



        /// <summary>
        /// Returns List of SRODocCashDetailsModel required to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<SRODocCashDetailsModel> GetSRODocCashCollectionReportsDetails(SRODocCashCollectionResponseModel model)
        {
            return misReportsDal.GetSRODocCashCollectionReportsDetails(model);

        }


        /// <summary>
        /// Returns totalCount of List of SRODocCashDetailsModel 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int GetSRODocCashCollectionReportsDetailsTotalCount(SRODocCashCollectionResponseModel model)
        {
            return misReportsDal.GetSRODocCashCollectionReportsDetailsTotalCount(model);

        }

    }
}