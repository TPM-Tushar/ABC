#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   SroDD_POCollectionBAL.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports  module.
*/
#endregion

using CustomModels.Models.MISReports.SroDD_POCollection;
using ECDataAPI.Areas.MISReports.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class SroDD_POCollectionBAL
    {

        SroDD_POCollectionDAL misReportsDal = new SroDD_POCollectionDAL();

        /// <summary>
        /// Returns SroDD_POCollectionResponseModel required to show SroDD_POCollectionView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public SroDD_POCollectionResponseModel SroDD_POCollectionView(int OfficeID)
        {
            return misReportsDal.SroDD_POCollectionView(OfficeID);

        }

        /// <summary>
        /// Returns List of SroDD_POCollectionDetailsModel Required to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<SroDD_POCollectionDetailsModel> GetSroDD_POCollectionReportsDetails(SroDD_POCollectionResponseModel model)
        {
            return misReportsDal.GetSroDD_POCollectionReportsDetails(model);

        }


        /// <summary>
        /// Returns Total Count GetSroDD_POCollectionReportsDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int GetSroDD_POCollectionReportsDetailsTotalCount(SroDD_POCollectionResponseModel model)
        {
            return misReportsDal.GetSroDD_POCollectionReportsDetailsTotalCount(model);

        }

        /// <summary>
        /// Returns SRoName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns></returns>
        public string GetSroName(int SROfficeID)
        {
            return misReportsDal.GetSroName(SROfficeID);

        }

    }
}