using CustomModels.Models.MISReports.SROChallanCollection;
using ECDataAPI.Areas.MISReports.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class SROChallanCollectionBAL
    {
        SROChallanCollectionDAL misReportsDal = new SROChallanCollectionDAL();

        public SROChallanCollectionResponseModel SROChallanCollectionView(int OfficeID)
        {
            return misReportsDal.SROChallanCollectionView(OfficeID);

        }

       
        public List<SROChallanCollectionDetailsModel> GetSROChallanCollectionReportsDetails(SROChallanCollectionResponseModel model)
        {
            return misReportsDal.GetSROChallanCollectionReportsDetails(model);

        }


        public int GetSROChallanCollectionReportsDetailsTotalCount(SROChallanCollectionResponseModel model)
        {
            return misReportsDal.GetSROChallanCollectionReportsDetailsTotalCount(model);
        }


        public string GetSroName(int SROfficeID)
        {
            return misReportsDal.GetSroName(SROfficeID);

        }
    }
}