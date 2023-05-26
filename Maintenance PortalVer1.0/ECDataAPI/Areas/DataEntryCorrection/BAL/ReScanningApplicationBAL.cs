
using CustomModels.Models.DataEntryCorrection;
using ECDataAPI.Areas.DataEntryCorrection.DAL;
using ECDataAPI.Areas.DataEntryCorrection.Interface;
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.DataEntryCorrection.BAL
{
    public class ReScanningApplicationBAL : IReScanningApplication
    {
        IReScanningApplication ReScanningApplicationDAL = new ReScanningApplicationDAL();

        public ReScanningApplicationViewModel ReScanningApplicationView(int officeID)
        {
            return ReScanningApplicationDAL.ReScanningApplicationView(officeID);
        }



        public DetailModel btnShowClick(int DRO, int SRO, string OrderNo, string Date, string DocNo, string DType, string BType, string FYear)
        {
            return ReScanningApplicationDAL.btnShowClick(DRO, SRO, OrderNo, Date, DocNo, DType, BType, FYear);
        }


        public int Upload(ReScanningApplicationReqModel req)
        {
            return ReScanningApplicationDAL.Upload(req);
        }


        public DROrderFilePathResultModel ViewBtnClickOrderTable(string path)
        {
            return ReScanningApplicationDAL.ViewBtnClickOrderTable(path);
        }



        public List<ReScanningApplicationOrderTableModel> LoadDocDetailsTable(int DroCode)
        {
            return ReScanningApplicationDAL.LoadDocDetailsTable(DroCode);
        }

        public string PCount(int SROCode, string DocNo, string Fyear, string BType)
        {
            return ReScanningApplicationDAL.PCount(SROCode, DocNo, Fyear, BType);
        }


        //public int Test(string req)
        //{
        //    return ReScanningApplicationDAL.Test(req);
        //}


        //public PhotoThumbFailedTableModel PhotoThumbFailed(int SROCode)
        //{
        //    return PhotoThumbFailedDAL.PhotoThumbFailed(SROCode);
        //}



        public string GetFRN(int SROCode, string BType, string FYear, string DocNo)
        {
            return ReScanningApplicationDAL.GetFRN(SROCode, BType, FYear, DocNo);
        }

    }
}