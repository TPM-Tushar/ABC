using CustomModels.Models.DataEntryCorrection;
using CustomModels.Models.SupportEnclosure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.SupportEnclosure.Interface
{
    public interface IDataEntryCorrection
    {

        //Added by Madhusoodan on 05/08/2021
        DataEntryCorrectionOrderViewModel AddEditOrderDetails(int orderID, int OfficeID);

        DataEntryCorrectionViewModel LoadInsertUpdateDeleteView(int OfficeID);

        DataEntryCorrectionOrderResultModel SaveOrderDetails(DataEntryCorrectionOrderViewModel dataEntryCorrectionOrderViewModel);

        DROrderFilePathResultModel ViewBtnClickOrderTable(int OrderID);

        List<DataEntryCorrectionOrderTableModel> LoadDocDetailsTable(int OfficeID, int SROCode, int RoleID,bool IsExcel);
        bool DeleteDECOrder(int orderID);
        
        //Not in use. Can be removed
        //string EditBtnClickOrderTable(string DROrderNumber);

        string FinalizeDECOrder(int currentOrderID);
        
        DROrderFilePathResultModel GenerateNewOrderID(int OfficeID, int currentOrderID);


        DataEntryCorrectionOrderResultModel DeleteCurrentOrderFile(int orderID);
        //Added by mayank 13/08/2021
        bool CheckifOrderNoExist(string OrderNo, int OrderID,int OfficeID);

        DataEntryCorrectionResultModel LoadIndexIIDetails(int OrderID);
        //Added by mayank District enabled for DEC
        DataEntryCorrectionViewModel DataEntryCorrectionView(int officeID, int LevelID, long UserID);
        DataEntryCorrectionViewModel GetSroCodebyDistrict(int DroCode);

        

    }
}
