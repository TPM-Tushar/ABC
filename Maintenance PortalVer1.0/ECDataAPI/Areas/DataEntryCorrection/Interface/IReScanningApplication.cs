using CustomModels.Models.DataEntryCorrection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.DataEntryCorrection.Interface
{
    public interface IReScanningApplication
    {

        ReScanningApplicationViewModel ReScanningApplicationView(int officeID);
        DetailModel btnShowClick(int DRO, int SRO, string OrderNo, string Date, string DocNo, string DType, string BType, string FYear);


        int Upload(ReScanningApplicationReqModel req);

        string PCount(int SROCode, string DocNo, string Fyear, string BType);

        List<ReScanningApplicationOrderTableModel> LoadDocDetailsTable(int DroCode);


        DROrderFilePathResultModel ViewBtnClickOrderTable(string path);


        string GetFRN(int SROCode, string BType, string FYear, string DocNo);

        //PhotoThumbTableModel PhotoThumbAvailaibility(int SROCode, long DocumentNumber, int BookTypeID,string fyear);
    }
}
